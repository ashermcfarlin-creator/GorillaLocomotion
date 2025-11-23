// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.DiceHoldable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class DiceHoldable : TransferrableObject
{
  [SerializeField]
  private DicePhysics dicePhysics;
  private RubberDuckEvents _events;

  internal override void OnEnable()
  {
    base.OnEnable();
    if ((UnityEngine.Object) this._events == (UnityEngine.Object) null)
    {
      this._events = this.gameObject.GetOrAddComponent<RubberDuckEvents>();
      NetPlayer player = (UnityEngine.Object) this.myOnlineRig != (UnityEngine.Object) null ? this.myOnlineRig.creator : ((UnityEngine.Object) this.myRig != (UnityEngine.Object) null ? (this.myRig.creator != null ? this.myRig.creator : NetworkSystem.Instance.LocalPlayer) : (NetPlayer) null);
      if (player != null)
        this._events.Init(player);
      else
        Debug.LogError((object) "Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
    }
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnDiceEvent);
  }

  internal override void OnDisable()
  {
    base.OnDisable();
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnDiceEvent);
    UnityEngine.Object.Destroy((UnityEngine.Object) this._events);
    this._events = (RubberDuckEvents) null;
  }

  private void OnDiceEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
  {
    GorillaNot.IncrementRPCCall(info, nameof (OnDiceEvent));
    if (sender != target || info.senderID != this.ownerRig.creator.ActorNumber)
      return;
    if ((bool) args[0])
    {
      Vector3 position = this.transform.position;
      Vector3 forward = this.transform.forward;
      ref Vector3 local1 = ref position;
      Vector3 vector3 = (Vector3) args[1];
      ref Vector3 local2 = ref vector3;
      local1.SetValueSafe(in local2);
      ref Vector3 local3 = ref forward;
      vector3 = (Vector3) args[2];
      ref Vector3 local4 = ref vector3;
      local3.SetValueSafe(in local4);
      float playerScale = ((float) args[3]).ClampSafe(0.01f, 1f);
      int landingSide = Mathf.Clamp((int) args[4], 1, 20);
      double finite = ((double) args[5]).GetFinite();
      this.ThrowDiceLocal(position, forward, playerScale, landingSide, finite);
    }
    else
      this.dicePhysics.EndThrow();
  }

  public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
  {
    if (this.dicePhysics.enabled)
    {
      if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
        this._events.Activate.RaiseOthers((object) false);
      this.transform.position = this.dicePhysics.transform.position;
      this.transform.rotation = this.dicePhysics.transform.rotation;
      this.dicePhysics.EndThrow();
      if ((UnityEngine.Object) grabbingHand == (UnityEngine.Object) EquipmentInteractor.instance.leftHand && this.currentState == TransferrableObject.PositionState.OnLeftArm)
      {
        this.canAutoGrabLeft = false;
        this.interpState = TransferrableObject.InterpolateState.None;
        this.currentState = TransferrableObject.PositionState.InLeftHand;
      }
      else if ((UnityEngine.Object) grabbingHand == (UnityEngine.Object) EquipmentInteractor.instance.rightHand && this.currentState == TransferrableObject.PositionState.OnRightArm)
      {
        this.canAutoGrabRight = false;
        this.interpState = TransferrableObject.InterpolateState.None;
        this.currentState = TransferrableObject.PositionState.InLeftHand;
      }
    }
    base.OnGrab(pointGrabbed, grabbingHand);
  }

  public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
  {
    if (!base.OnRelease(zoneReleased, releasingHand))
      return false;
    if ((UnityEngine.Object) zoneReleased == (UnityEngine.Object) null)
    {
      Vector3 position = this.transform.position;
      Vector3 averageVelocity = GTPlayer.Instance.GetInteractPointVelocityTracker((UnityEngine.Object) releasingHand == (UnityEngine.Object) EquipmentInteractor.instance.leftHand).GetAverageVelocity(true);
      int randomSide = this.dicePhysics.GetRandomSide();
      double startTime = PhotonNetwork.InRoom ? PhotonNetwork.Time : -1.0;
      float scale = GTPlayer.Instance.scale;
      if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
        this._events.Activate.RaiseOthers((object) true, (object) position, (object) averageVelocity, (object) scale, (object) randomSide, (object) startTime);
      this.ThrowDiceLocal(position, averageVelocity, scale, randomSide, startTime);
    }
    return true;
  }

  private void ThrowDiceLocal(
    Vector3 startPosition,
    Vector3 throwVelocity,
    float playerScale,
    int landingSide,
    double startTime)
  {
    this.dicePhysics.StartThrow(this, startPosition, throwVelocity, playerScale, landingSide, startTime);
  }
}
