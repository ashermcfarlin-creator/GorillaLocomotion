// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ThrowablePickupableCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag.Cosmetics;

public class ThrowablePickupableCosmetic : TransferrableObject
{
  [Tooltip("Child object with the PickupableCosmetic script")]
  [SerializeField]
  private PickupableVariant pickupableVariant;
  [Tooltip("cosmetics released at a greater distance from the dock than the threshold will be placed in world instead of returning to the dock")]
  [SerializeField]
  private float returnToDockDistanceThreshold = 0.7f;
  [FormerlySerializedAs("OnReturnToDockPosition")]
  [Space]
  public UnityEvent OnReturnToDockPositionLocal;
  public UnityEvent OnReturnToDockPositionShared;
  [FormerlySerializedAs("OnGrabFromDockPosition")]
  public UnityEvent OnGrabLocal;
  private RubberDuckEvents _events;
  private TransferrableObject transferrableObject;
  private bool isLocal;
  private NetPlayer owner;
  private CallLimiter callLimiterRelease = new CallLimiter(10, 2f);
  private CallLimiter callLimiterReturn = new CallLimiter(10, 2f);

  private new void Awake() => this.transferrableObject = this.GetComponent<TransferrableObject>();

  internal override void OnEnable()
  {
    base.OnEnable();
    if ((UnityEngine.Object) this._events == (UnityEngine.Object) null)
    {
      this._events = this.gameObject.GetOrAddComponent<RubberDuckEvents>();
      this.owner = (UnityEngine.Object) this.transferrableObject.myOnlineRig != (UnityEngine.Object) null ? this.transferrableObject.myOnlineRig.creator : ((UnityEngine.Object) this.transferrableObject.myRig != (UnityEngine.Object) null ? this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer : (NetPlayer) null);
      if (this.owner != null)
      {
        this._events.Init(this.owner);
        this.isLocal = this.owner.IsLocal;
      }
    }
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate.reliable = true;
    this._events.Deactivate.reliable = true;
    this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnReleaseEvent);
    this._events.Deactivate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnReturnToDockEvent);
  }

  internal override void OnDisable()
  {
    base.OnDisable();
    if ((UnityEngine.Object) this._events != (UnityEngine.Object) null)
    {
      this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnReleaseEvent);
      this._events.Deactivate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnReturnToDockEvent);
      this._events.Dispose();
      this._events = (RubberDuckEvents) null;
    }
    if (!((UnityEngine.Object) this.pickupableVariant != (UnityEngine.Object) null) || !this.pickupableVariant.enabled)
      return;
    this.pickupableVariant.DelayedPickup();
  }

  public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
  {
    if ((UnityEngine.Object) VRRigCache.Instance.localRig.Rig != (UnityEngine.Object) this.ownerRig)
      return;
    if (this.pickupableVariant.enabled)
    {
      if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
        this._events.Activate.RaiseOthers((object) false);
      this.transform.position = this.pickupableVariant.transform.position;
      this.transform.rotation = this.pickupableVariant.transform.rotation;
      this.pickupableVariant.Pickup();
      if ((UnityEngine.Object) grabbingHand == (UnityEngine.Object) EquipmentInteractor.instance.leftHand && this.currentState == TransferrableObject.PositionState.OnLeftArm)
      {
        this.canAutoGrabLeft = false;
        this.interpState = TransferrableObject.InterpolateState.None;
        this.currentState = TransferrableObject.PositionState.InRightHand;
      }
      else if ((UnityEngine.Object) grabbingHand == (UnityEngine.Object) EquipmentInteractor.instance.rightHand && this.currentState == TransferrableObject.PositionState.OnRightArm)
      {
        this.canAutoGrabRight = false;
        this.interpState = TransferrableObject.InterpolateState.None;
        this.currentState = TransferrableObject.PositionState.InLeftHand;
      }
    }
    this.OnGrabLocal?.Invoke();
    base.OnGrab(pointGrabbed, grabbingHand);
  }

  public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
  {
    if (!base.OnRelease(zoneReleased, releasingHand) || !((UnityEngine.Object) VRRigCache.Instance.localRig.Rig == (UnityEngine.Object) this.ownerRig))
      return false;
    Vector3 position = this.transform.position;
    Vector3 averageVelocity = GTPlayer.Instance.GetInteractPointVelocityTracker((UnityEngine.Object) releasingHand == (UnityEngine.Object) EquipmentInteractor.instance.leftHand).GetAverageVelocity(true);
    float scale = GTPlayer.Instance.scale;
    bool flag = (double) this.DistanceToDock() > (double) this.returnToDockDistanceThreshold;
    if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null)
    {
      if (flag && this._events.Activate != (PhotonEvent) null)
        this._events.Activate.RaiseAll((object) true, (object) position, (object) averageVelocity, (object) scale);
      else if (!flag && this._events.Deactivate != (PhotonEvent) null)
      {
        this._events.Deactivate.RaiseAll();
        this.OnReturnToDockPositionLocal?.Invoke();
      }
    }
    else if (flag)
    {
      this.OnReleaseEventLocal(position, averageVelocity, scale);
    }
    else
    {
      this.OnReturnToDockPositionLocal?.Invoke();
      this.OnReturnToDockPositionShared?.Invoke();
    }
    return true;
  }

  private void OnReleaseEvent(
    int sender,
    int target,
    object[] args,
    PhotonMessageInfoWrapped info)
  {
    if (sender != target || info.senderID != this.ownerRig.creator.ActorNumber)
      return;
    GorillaNot.IncrementRPCCall(info, nameof (OnReleaseEvent));
    if (!this.callLimiterRelease.CheckCallTime(Time.time) || !(args[0] is bool flag))
      return;
    if (flag)
    {
      if (!(args[1] is Vector3 newVal) || !(args[2] is Vector3 inVel) || !(args[3] is float num))
        return;
      Vector3 position = this.transform.position;
      Vector3 forward = this.transform.forward;
      position.SetValueSafe(in newVal);
      if (!this.ownerRig.IsPositionInRange(position, 20f))
        return;
      Vector3 playerSafe = this.ownerRig.ClampVelocityRelativeToPlayerSafe(inVel, 50f);
      float playerScale = num.ClampSafe(0.01f, 1f);
      this.OnReleaseEventLocal(position, playerSafe, playerScale);
    }
    else
      this.pickupableVariant.Pickup();
  }

  private void OnReturnToDockEvent(
    int sender,
    int target,
    object[] args,
    PhotonMessageInfoWrapped info)
  {
    if (sender != target || info.senderID != this.ownerRig.creator.ActorNumber)
      return;
    GorillaNot.IncrementRPCCall(info, nameof (OnReturnToDockEvent));
    if (!this.callLimiterReturn.CheckCallTime(Time.time))
      return;
    this.OnReturnToDockPositionShared?.Invoke();
  }

  private void OnReleaseEventLocal(
    Vector3 startPosition,
    Vector3 releaseVelocity,
    float playerScale)
  {
    this.pickupableVariant.Release((HoldableObject) this, startPosition, releaseVelocity, playerScale);
  }

  private float DistanceToDock()
  {
    float dock = 0.0f;
    if (this.currentState == TransferrableObject.PositionState.OnRightShoulder)
      dock = Vector3.Distance(this.ownerRig.myBodyDockPositions.rightBackTransform.position, this.transform.position);
    else if (this.currentState == TransferrableObject.PositionState.OnLeftShoulder)
      dock = Vector3.Distance(this.ownerRig.myBodyDockPositions.leftBackTransform.position, this.transform.position);
    else if (this.currentState == TransferrableObject.PositionState.OnRightArm)
      dock = Vector3.Distance(this.ownerRig.myBodyDockPositions.rightArmTransform.position, this.transform.position);
    else if (this.currentState == TransferrableObject.PositionState.OnLeftArm)
      dock = Vector3.Distance(this.ownerRig.myBodyDockPositions.leftArmTransform.position, this.transform.position);
    else if (this.currentState == TransferrableObject.PositionState.OnChest)
      dock = Vector3.Distance(this.ownerRig.myBodyDockPositions.chestTransform.position, this.transform.position);
    return dock;
  }
}
