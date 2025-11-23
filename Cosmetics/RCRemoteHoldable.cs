// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.RCRemoteHoldable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaNetworking;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

#nullable disable
namespace GorillaTag.Cosmetics;

public class RCRemoteHoldable : TransferrableObject, ISnapTurnOverride
{
  [SerializeField]
  private Transform joystickTransform;
  [SerializeField]
  private Transform triggerTransform;
  [SerializeField]
  private Transform buttonTransform;
  private RCVehicle targetVehicle;
  private float joystickLeanDegrees = 30f;
  private float triggerPullDegrees = 40f;
  private float buttonPressDepth = 0.005f;
  private Quaternion initialJoystickRotation;
  private Quaternion initialTriggerRotation;
  private Quaternion initialButtonRotation;
  private Vector3 initialButtonPosition;
  private bool currentlyHeld;
  private XRNode xrNode;
  private RCRemoteHoldable.RCInput currentInput;
  [HideInInspector]
  public RCCosmeticNetworkSync networkSync;
  private string networkSyncPrefabName = "RCCosmeticNetworkSync";
  private RubberDuckEvents _events;
  private object[] emptyArgs = new object[0];

  public XRNode XRNode => this.xrNode;

  public RCVehicle Vehicle => this.targetVehicle;

  public bool TurnOverrideActive()
  {
    return this.gameObject.activeSelf && this.currentlyHeld && this.xrNode == XRNode.RightHand;
  }

  protected override void Awake()
  {
    base.Awake();
    this.initialJoystickRotation = this.joystickTransform.localRotation;
    this.initialTriggerRotation = this.triggerTransform.localRotation;
    if (!((UnityEngine.Object) this.buttonTransform != (UnityEngine.Object) null))
      return;
    this.initialButtonRotation = this.buttonTransform.localRotation;
    this.initialButtonPosition = this.buttonTransform.localPosition;
  }

  internal override void OnEnable()
  {
    base.OnEnable();
    if (!this._TryFindRemoteVehicle())
    {
      this.gameObject.SetActive(false);
    }
    else
    {
      if (this._events.IsNotNull() || this.gameObject.TryGetComponent<RubberDuckEvents>(out this._events))
      {
        this._events = this.gameObject.GetOrAddComponent<RubberDuckEvents>();
        NetPlayer player = (UnityEngine.Object) this.myOnlineRig != (UnityEngine.Object) null ? this.myOnlineRig.creator : ((UnityEngine.Object) this.myRig != (UnityEngine.Object) null ? (this.myRig.creator != null ? this.myRig.creator : NetworkSystem.Instance.LocalPlayer) : (NetPlayer) null);
        if (player != null)
          this._events.Init(player);
        else
          Debug.LogError((object) "Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
        this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnStartConnectionEvent);
      }
      this.WakeUpRemoteVehicle();
    }
  }

  internal override void OnDisable()
  {
    base.OnDisable();
    GorillaSnapTurn component = (UnityEngine.Object) GorillaTagger.Instance != (UnityEngine.Object) null ? GorillaTagger.Instance.GetComponent<GorillaSnapTurn>() : (GorillaSnapTurn) null;
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      component.UnsetTurningOverride((ISnapTurnOverride) this);
    if ((UnityEngine.Object) this.networkSync != (UnityEngine.Object) null && this.networkSync.photonView.IsMine)
    {
      PhotonNetwork.Destroy(this.networkSync.gameObject);
      this.networkSync = (RCCosmeticNetworkSync) null;
    }
    if (!this._events.IsNotNull())
      return;
    this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnStartConnectionEvent);
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    GorillaSnapTurn component = (UnityEngine.Object) GorillaTagger.Instance != (UnityEngine.Object) null ? GorillaTagger.Instance.GetComponent<GorillaSnapTurn>() : (GorillaSnapTurn) null;
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    component.UnsetTurningOverride((ISnapTurnOverride) this);
  }

  public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
  {
    base.OnGrab(pointGrabbed, grabbingHand);
    if (PhotonNetwork.InRoom && (UnityEngine.Object) this.networkSync != (UnityEngine.Object) null && this.networkSync.photonView.Owner == null)
    {
      PhotonNetwork.Destroy(this.networkSync.gameObject);
      this.networkSync = (RCCosmeticNetworkSync) null;
    }
    if ((UnityEngine.Object) this.networkSync == (UnityEngine.Object) null && PhotonNetwork.InRoom)
    {
      GameObject gameObject = PhotonNetwork.Instantiate(this.networkSyncPrefabName, Vector3.zero, Quaternion.identity, data: new object[1]
      {
        (object) this.myIndex
      });
      this.networkSync = (UnityEngine.Object) gameObject != (UnityEngine.Object) null ? gameObject.GetComponent<RCCosmeticNetworkSync>() : (RCCosmeticNetworkSync) null;
    }
    this.currentlyHeld = true;
    bool flag = (UnityEngine.Object) grabbingHand == (UnityEngine.Object) EquipmentInteractor.instance.rightHand;
    this.xrNode = flag ? XRNode.RightHand : XRNode.LeftHand;
    GorillaSnapTurn component = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
    if (flag)
      component.SetTurningOverride((ISnapTurnOverride) this);
    else
      component.UnsetTurningOverride((ISnapTurnOverride) this);
    if ((UnityEngine.Object) this.targetVehicle != (UnityEngine.Object) null)
      this.targetVehicle.StartConnection(this, this.networkSync);
    if (!PhotonNetwork.InRoom || !((UnityEngine.Object) this._events != (UnityEngine.Object) null) || !(this._events.Activate != (PhotonEvent) null))
      return;
    this._events.Activate.RaiseOthers(this.emptyArgs);
  }

  public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
  {
    if (!base.OnRelease(zoneReleased, releasingHand))
      return false;
    this.currentlyHeld = false;
    this.currentInput = new RCRemoteHoldable.RCInput();
    if ((UnityEngine.Object) this.targetVehicle != (UnityEngine.Object) null)
      this.targetVehicle.EndConnection();
    this.joystickTransform.localRotation = this.initialJoystickRotation;
    this.triggerTransform.localRotation = this.initialTriggerRotation;
    GorillaTagger.Instance.GetComponent<GorillaSnapTurn>().UnsetTurningOverride((ISnapTurnOverride) this);
    return true;
  }

  private void Update()
  {
    if (!this.currentlyHeld)
      return;
    this.currentInput.joystick = ControllerInputPoller.Primary2DAxis(this.xrNode);
    this.currentInput.trigger = ControllerInputPoller.TriggerFloat(this.xrNode);
    this.currentInput.buttons = ControllerInputPoller.PrimaryButtonPress(this.xrNode) ? (byte) 1 : (byte) 0;
    if ((UnityEngine.Object) this.targetVehicle != (UnityEngine.Object) null)
      this.targetVehicle.ApplyRemoteControlInput(this.currentInput);
    this.joystickTransform.localRotation = this.initialJoystickRotation * Quaternion.Euler(this.joystickLeanDegrees * this.currentInput.joystick.y, 0.0f, -this.joystickLeanDegrees * this.currentInput.joystick.x);
    this.triggerTransform.localRotation = this.initialTriggerRotation * Quaternion.Euler(this.triggerPullDegrees * this.currentInput.trigger, 0.0f, 0.0f);
    if (!((UnityEngine.Object) this.buttonTransform != (UnityEngine.Object) null))
      return;
    this.buttonTransform.localPosition = this.initialButtonPosition + this.initialButtonRotation * new Vector3(0.0f, 0.0f, (float) (-(double) this.buttonPressDepth * (this.currentInput.buttons > (byte) 0 ? 1.0 : 0.0)));
  }

  public void OnStartConnectionEvent(
    int sender,
    int target,
    object[] args,
    PhotonMessageInfoWrapped info)
  {
    if (sender != target || info.senderID != this.ownerRig.creator.ActorNumber)
      return;
    this.WakeUpRemoteVehicle();
  }

  public void WakeUpRemoteVehicle()
  {
    if (!((UnityEngine.Object) this.networkSync != (UnityEngine.Object) null) || !this.targetVehicle.IsNotNull() || this.targetVehicle.HasLocalAuthority)
      return;
    this.targetVehicle.WakeUpRemote(this.networkSync);
  }

  private bool _TryFindRemoteVehicle()
  {
    if ((UnityEngine.Object) this.targetVehicle != (UnityEngine.Object) null)
      return true;
    VRRig componentInParent = this.GetComponentInParent<VRRig>(true);
    if (componentInParent.IsNull())
    {
      Debug.LogError((object) "RCRemoteHoldable: unable to find parent vrrig");
      return false;
    }
    CosmeticItemInstance cosmeticItemInstance = componentInParent.cosmeticsObjectRegistry.Cosmetic(this.name);
    int instanceId = this.gameObject.GetInstanceID();
    return this._TryFindRemoteVehicle_InCosmeticInstanceArray(instanceId, cosmeticItemInstance.objects) || this._TryFindRemoteVehicle_InCosmeticInstanceArray(instanceId, cosmeticItemInstance.leftObjects) || this._TryFindRemoteVehicle_InCosmeticInstanceArray(instanceId, cosmeticItemInstance.rightObjects);
  }

  private bool _TryFindRemoteVehicle_InCosmeticInstanceArray(
    int thisGobjInstId,
    List<GameObject> gameObjects)
  {
    foreach (GameObject gameObject in gameObjects)
    {
      if (gameObject.GetInstanceID() != thisGobjInstId)
      {
        this.targetVehicle = gameObject.GetComponentInChildren<RCVehicle>(true);
        if (this.targetVehicle != null)
          return true;
      }
    }
    return false;
  }

  public struct RCInput
  {
    public Vector2 joystick;
    public float trigger;
    public byte buttons;
  }
}
