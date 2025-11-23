// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ThrowableHoldableCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.Shared.Scripts;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag.Cosmetics;

public class ThrowableHoldableCosmetic : TransferrableObject
{
  [Tooltip("Projectile prefab from the global object pool that gets spawned when this object is thrown")]
  [FormerlySerializedAs("firecrackerProjectilePrefab")]
  [SerializeField]
  private GameObject projectilePrefab;
  [Tooltip(" A second projectile prefab that will be spawned if UseAlternativeProjectile is called")]
  [SerializeField]
  private GameObject alternativeProjectilePrefab;
  [Tooltip("Objects on the body that should be hidden when the projectile is spawned")]
  [SerializeField]
  private GameObject disableWhenThrown;
  private CallLimiter firecrackerCallLimiter = new CallLimiter(10, 3f);
  private CosmeticEffectsOnPlayers playersEffect;
  private int projectileHash;
  private int alternativeProjectileHash;
  private int currentProjectileHash;
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
    }
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnThrowEvent);
  }

  protected override void Awake()
  {
    base.Awake();
    this.projectileHash = PoolUtils.GameObjHashCode(this.projectilePrefab);
    if ((UnityEngine.Object) this.alternativeProjectilePrefab != (UnityEngine.Object) null)
      this.alternativeProjectileHash = PoolUtils.GameObjHashCode(this.alternativeProjectilePrefab);
    this.currentProjectileHash = this.projectileHash;
    this.playersEffect = this.GetComponentInChildren<CosmeticEffectsOnPlayers>();
  }

  public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
  {
    if (!this.disableWhenThrown.gameObject.activeSelf)
      return;
    base.OnGrab(pointGrabbed, grabbingHand);
  }

  public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
  {
    if (!base.OnRelease(zoneReleased, releasingHand) || (UnityEngine.Object) VRRigCache.Instance.localRig.Rig != (UnityEngine.Object) this.ownerRig)
      return false;
    Vector3 position = this.transform.position;
    Quaternion rotation = this.transform.rotation;
    Vector3 averageVelocity = GTPlayer.Instance.GetInteractPointVelocityTracker((UnityEngine.Object) releasingHand == (UnityEngine.Object) EquipmentInteractor.instance.leftHand).GetAverageVelocity(true);
    float scale = GTPlayer.Instance.scale;
    if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
      this._events.Activate.RaiseOthers((object) position, (object) rotation, (object) averageVelocity, (object) scale);
    this.OnThrowLocal(position, rotation, averageVelocity, this.ownerRig);
    return true;
  }

  internal override void OnDisable()
  {
    base.OnDisable();
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnThrowEvent);
    this._events.Dispose();
    this._events = (RubberDuckEvents) null;
  }

  public void UseAlternativeProjectile()
  {
    if (!((UnityEngine.Object) this.alternativeProjectilePrefab != (UnityEngine.Object) null))
      return;
    this.currentProjectileHash = this.alternativeProjectileHash;
  }

  private void OnThrowEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
  {
    if (sender != target || args.Length != 4 || info.senderID != this.ownerRig.creator.ActorNumber)
      return;
    GorillaNot.IncrementRPCCall(info, nameof (OnThrowEvent));
    if (!this.firecrackerCallLimiter.CheckCallTime(Time.time) || !(args[0] is Vector3 v) || !(args[1] is Quaternion q) || !(args[2] is Vector3 inVel) || !(args[3] is float num1))
      return;
    Vector3 playerSafe = this.targetRig.ClampVelocityRelativeToPlayerSafe(inVel, 40f);
    double num2 = (double) num1.ClampSafe(0.01f, 1f);
    if (!q.IsValid() || !v.IsValid() || !this.targetRig.IsPositionInRange(v, 4f))
      return;
    this.OnThrowLocal(v, q, playerSafe, this.ownerRig);
  }

  private void OnThrowLocal(
    Vector3 startPos,
    Quaternion rotation,
    Vector3 velocity,
    VRRig ownerRig)
  {
    this.disableWhenThrown.SetActive(false);
    IProjectile component = ObjectPools.instance.Instantiate(this.currentProjectileHash).GetComponent<IProjectile>();
    switch (component)
    {
      case FirecrackerProjectile firecrackerProjectile:
        if (this.networkedStateEvents != TransferrableObject.SyncOptions.None)
        {
          int state = (int) (this.itemState & ~TransferrableObject.ItemStates.Part0Held);
          firecrackerProjectile.SetTransferrableState(this.networkedStateEvents, state);
        }
        firecrackerProjectile.OnDetonationComplete.AddListener(new UnityAction<FirecrackerProjectile>(this.HitComplete));
        firecrackerProjectile.OnDetonationStart.AddListener(new UnityAction<FirecrackerProjectile, Vector3>(this.HitStart));
        break;
      case FartBagThrowable fartBagThrowable:
        fartBagThrowable.OnDeflated += new Action<IProjectile>(this.HitComplete);
        fartBagThrowable.ParentTransferable = (TransferrableObject) this;
        break;
    }
    component.Launch(startPos, rotation, velocity, 1f, ownerRig);
    this.currentProjectileHash = this.projectileHash;
  }

  private void HitStart(FirecrackerProjectile firecracker, Vector3 contactPos)
  {
    if ((UnityEngine.Object) firecracker == (UnityEngine.Object) null || (UnityEngine.Object) this.playersEffect == (UnityEngine.Object) null)
      return;
    this.playersEffect.ApplyAllEffectsByDistance(contactPos);
  }

  private void HitComplete(IProjectile projectile)
  {
    if (projectile == null)
      return;
    if (this.IsLocalObject() && this.networkedStateEvents != TransferrableObject.SyncOptions.None && this.resetOnDocked)
    {
      switch (this.networkedStateEvents)
      {
        case TransferrableObject.SyncOptions.Bool:
          this.ResetStateBools();
          break;
        case TransferrableObject.SyncOptions.Int:
          this.SetItemStateInt(0);
          break;
      }
    }
    this.disableWhenThrown.SetActive(true);
    switch (projectile)
    {
      case FirecrackerProjectile firecrackerProjectile:
        firecrackerProjectile.OnDetonationStart.RemoveListener(new UnityAction<FirecrackerProjectile, Vector3>(this.HitStart));
        firecrackerProjectile.OnDetonationComplete.RemoveListener(new UnityAction<FirecrackerProjectile>(this.HitComplete));
        ObjectPools.instance.Destroy(firecrackerProjectile.gameObject);
        break;
      case FartBagThrowable fartBagThrowable:
        fartBagThrowable.OnDeflated -= new Action<IProjectile>(this.HitComplete);
        ObjectPools.instance.Destroy(fartBagThrowable.gameObject);
        break;
    }
  }
}
