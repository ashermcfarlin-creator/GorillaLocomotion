// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ChickenSword
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class ChickenSword : MonoBehaviour
{
  [SerializeField]
  private float rechargeCooldown;
  [SerializeField]
  private GorillaVelocityTracker velocityTracker;
  [SerializeField]
  private float hitVelocityThreshold;
  [SerializeField]
  private TransferrableObject transferrableObject;
  [SerializeField]
  private CosmeticSwapper cosmeticSwapper;
  [Space]
  [Space]
  public UnityEvent OnDeflatedShared;
  public UnityEvent<bool> OnDeflatedLocal;
  public UnityEvent OnRechargedShared;
  public UnityEvent<bool> OnRechargedLocal;
  public UnityEvent<VRRig> OnHitTargetShared;
  public UnityEvent<bool> OnHitTargetLocal;
  public UnityEvent<VRRig> OnReachedLastTransformationStepShared;
  private float lastHitTime;
  private ChickenSword.SwordState currentState;
  private bool hitReceievd;
  private RubberDuckEvents _events;
  private CallLimiter callLimiter = new CallLimiter(10, 2f);

  private void Awake()
  {
    this.lastHitTime = float.PositiveInfinity;
    this.SwitchState(ChickenSword.SwordState.Ready);
  }

  internal void OnEnable()
  {
    if ((UnityEngine.Object) this._events == (UnityEngine.Object) null)
    {
      this._events = this.gameObject.GetOrAddComponent<RubberDuckEvents>();
      NetPlayer player = (UnityEngine.Object) this.transferrableObject.myOnlineRig != (UnityEngine.Object) null ? this.transferrableObject.myOnlineRig.creator : ((UnityEngine.Object) this.transferrableObject.myRig != (UnityEngine.Object) null ? (this.transferrableObject.myRig.creator != null ? this.transferrableObject.myRig.creator : NetworkSystem.Instance.LocalPlayer) : (NetPlayer) null);
      if (player != null)
        this._events.Init(player);
      else
        Debug.LogError((object) "Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
    }
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnReachedLastTransformationStep);
  }

  private void OnDisable()
  {
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnReachedLastTransformationStep);
    this._events.Dispose();
    this._events = (RubberDuckEvents) null;
  }

  private void Update()
  {
    switch (this.currentState)
    {
      case ChickenSword.SwordState.Ready:
        if (!this.hitReceievd)
          break;
        this.hitReceievd = false;
        this.lastHitTime = Time.time;
        this.SwitchState(ChickenSword.SwordState.Deflated);
        this.OnDeflatedShared?.Invoke();
        if (!(bool) (UnityEngine.Object) this.transferrableObject || !this.transferrableObject.IsMyItem())
          break;
        this.OnDeflatedLocal?.Invoke(this.transferrableObject.InLeftHand());
        break;
      case ChickenSword.SwordState.Deflated:
        if ((double) Time.time - (double) this.lastHitTime <= (double) this.rechargeCooldown)
          break;
        this.lastHitTime = float.PositiveInfinity;
        this.SwitchState(ChickenSword.SwordState.Ready);
        this.OnRechargedShared?.Invoke();
        if (!(bool) (UnityEngine.Object) this.transferrableObject || !this.transferrableObject.IsMyItem())
          break;
        this.OnRechargedLocal?.Invoke(this.transferrableObject.InLeftHand());
        break;
    }
  }

  public void OnHitTargetSync(VRRig playerRig)
  {
    if ((UnityEngine.Object) this.velocityTracker == (UnityEngine.Object) null)
      return;
    Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true);
    if (this.currentState != ChickenSword.SwordState.Ready || (double) averageVelocity.magnitude <= (double) this.hitVelocityThreshold)
      return;
    this.hitReceievd = true;
    this.OnHitTargetShared?.Invoke(playerRig);
    if ((bool) (UnityEngine.Object) this.transferrableObject && this.transferrableObject.IsMyItem())
      this.OnHitTargetLocal?.Invoke(this.transferrableObject.InLeftHand());
    if (!((UnityEngine.Object) this.cosmeticSwapper != (UnityEngine.Object) null) || !((UnityEngine.Object) playerRig == (UnityEngine.Object) GorillaTagger.Instance.offlineVRRig) || this.cosmeticSwapper.GetCurrentStepIndex(playerRig) < this.cosmeticSwapper.GetNumberOfSteps() || !PhotonNetwork.InRoom || !((UnityEngine.Object) this._events != (UnityEngine.Object) null) || !(this._events.Activate != (PhotonEvent) null))
      return;
    this._events.Activate.RaiseAll();
  }

  private void OnReachedLastTransformationStep(
    int sender,
    int target,
    object[] args,
    PhotonMessageInfoWrapped info)
  {
    if (sender != target)
      return;
    GorillaNot.IncrementRPCCall(info, nameof (OnReachedLastTransformationStep));
    RigContainer playerRig;
    if (!this.callLimiter.CheckCallTime(Time.time) || !VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender.ActorNumber), out playerRig) || !playerRig.Rig.IsPositionInRange(this.transform.position, 6f))
      return;
    this.OnReachedLastTransformationStepShared?.Invoke(playerRig.Rig);
  }

  private void SwitchState(ChickenSword.SwordState newState) => this.currentState = newState;

  private enum SwordState
  {
    Ready,
    Deflated,
  }
}
