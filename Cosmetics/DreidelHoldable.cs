// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.DreidelHoldable
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

public class DreidelHoldable : TransferrableObject
{
  [SerializeField]
  private Dreidel dreidelAnimation;
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
    this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnDreidelSpin);
  }

  internal override void OnDisable()
  {
    base.OnDisable();
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnDreidelSpin);
    UnityEngine.Object.Destroy((UnityEngine.Object) this._events);
    this._events = (RubberDuckEvents) null;
  }

  private void OnDreidelSpin(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
  {
    GorillaNot.IncrementRPCCall(info, nameof (OnDreidelSpin));
    if (sender != target || info.senderID != this.ownerRig.creator.ActorNumber)
      return;
    Vector3 v1 = (Vector3) args[0];
    Vector3 v2 = (Vector3) args[1];
    float duration = (float) args[2];
    double startTime = (double) args[6];
    if (!v1.IsValid() || !v2.IsValid() || !float.IsFinite(duration) || !double.IsFinite(startTime))
      return;
    bool counterClockwise = (bool) args[3];
    Dreidel.Side side = (Dreidel.Side) args[4];
    Dreidel.Variation variation = (Dreidel.Variation) args[5];
    this.StartSpinLocal(v1, v2, duration, counterClockwise, side, variation, startTime);
  }

  public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
  {
    base.OnGrab(pointGrabbed, grabbingHand);
    if (!((UnityEngine.Object) this.dreidelAnimation != (UnityEngine.Object) null))
      return;
    this.dreidelAnimation.TryCheckForSurfaces();
  }

  public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
  {
    if (!base.OnRelease(zoneReleased, releasingHand))
      return false;
    if ((UnityEngine.Object) this.dreidelAnimation != (UnityEngine.Object) null)
      this.dreidelAnimation.TrySetIdle();
    return true;
  }

  public override void OnActivate()
  {
    base.OnActivate();
    Vector3 surfacePoint;
    Vector3 surfaceNormal;
    float randomDuration;
    Dreidel.Side randomSide;
    Dreidel.Variation randomVariation;
    double startTime;
    if (!((UnityEngine.Object) this.dreidelAnimation != (UnityEngine.Object) null) || !this.dreidelAnimation.TryGetSpinStartData(out surfacePoint, out surfaceNormal, out randomDuration, out randomSide, out randomVariation, out startTime))
      return;
    bool counterClockwise = this.currentState == TransferrableObject.PositionState.InLeftHand;
    if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
      this._events.Activate.RaiseAll((object) surfacePoint, (object) surfaceNormal, (object) randomDuration, (object) counterClockwise, (object) (int) randomSide, (object) (int) randomVariation, (object) startTime);
    else
      this.StartSpinLocal(surfacePoint, surfaceNormal, randomDuration, counterClockwise, randomSide, randomVariation, startTime);
  }

  private void StartSpinLocal(
    Vector3 surfacePoint,
    Vector3 surfaceNormal,
    float duration,
    bool counterClockwise,
    Dreidel.Side side,
    Dreidel.Variation variation,
    double startTime)
  {
    if (!((UnityEngine.Object) this.dreidelAnimation != (UnityEngine.Object) null))
      return;
    this.dreidelAnimation.SetSpinStartData(surfacePoint, surfaceNormal, duration, counterClockwise, side, variation, startTime);
    this.dreidelAnimation.Spin();
  }

  public void DebugSpinDreidel()
  {
    Transform transform = GTPlayer.Instance.headCollider.transform;
    Vector3 origin = transform.position + transform.forward * 0.5f;
    float num = 2f;
    Vector3 down = Vector3.down;
    UnityEngine.RaycastHit raycastHit;
    ref UnityEngine.RaycastHit local = ref raycastHit;
    double maxDistance = (double) num;
    int layerMask = GTPlayer.Instance.locomotionEnabledLayers.value;
    if (!Physics.Raycast(origin, down, out local, (float) maxDistance, layerMask, QueryTriggerInteraction.Ignore))
      return;
    Vector3 point = raycastHit.point;
    Vector3 normal = raycastHit.normal;
    float duration = UnityEngine.Random.Range(7f, 10f);
    Dreidel.Side side = (Dreidel.Side) UnityEngine.Random.Range(0, 4);
    Dreidel.Variation variation = (Dreidel.Variation) UnityEngine.Random.Range(0, 5);
    bool counterClockwise = this.currentState == TransferrableObject.PositionState.InLeftHand;
    double startTime = PhotonNetwork.InRoom ? PhotonNetwork.Time : -1.0;
    if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
      this._events.Activate.RaiseAll((object) point, (object) normal, (object) duration, (object) counterClockwise, (object) (int) side, (object) (int) variation, (object) startTime);
    else
      this.StartSpinLocal(point, normal, duration, counterClockwise, side, variation, startTime);
  }
}
