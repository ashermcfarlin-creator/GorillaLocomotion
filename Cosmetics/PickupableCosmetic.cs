// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.PickupableCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag.Cosmetics;

public class PickupableCosmetic : PickupableVariant
{
  [SerializeField]
  private InteractionPoint interactionPoint;
  [SerializeField]
  private Rigidbody rb;
  [SerializeField]
  private Transform raycastOrigin;
  [Tooltip("Allow player to grab the placed object")]
  [SerializeField]
  private bool allowPickupFromGround = true;
  [SerializeField]
  private float autoPickupAfterSeconds;
  [SerializeField]
  private float autoPickupDistance;
  [Tooltip("Amount to offset the placed object from the hit position in the hit normal direction")]
  [SerializeField]
  private float placementOffset;
  [Tooltip("Prevent sticking if the hit surface normal is not within 40 degrees of world up")]
  [SerializeField]
  private bool dontStickToWall;
  [Tooltip("Layers to raycast against for placement")]
  [SerializeField]
  private LayerMask floorLayerMask = (LayerMask) 134218241 /*0x08000201*/;
  [Tooltip("The distance to check if the banner is close to the floor (from a raycast check).")]
  public float RaycastCheckDist = 0.2f;
  [Tooltip("How many checks should we attempt for a raycast.")]
  public int RaycastChecksMax = 12;
  [FormerlySerializedAs("OnPickup")]
  [Space]
  public UnityEvent OnPickupShared;
  [FormerlySerializedAs("OnPlaced")]
  public UnityEvent OnPlacedShared;
  [SerializeField]
  private bool isBreakable;
  [Tooltip("Particle system played OnBrokenShared")]
  [SerializeField]
  private ParticleSystem breakEffect;
  [Tooltip("Renderers disabled OnBrokenShared and enabled OnPickupShared")]
  [SerializeField]
  private Renderer[] hideOnBreak = new Renderer[0];
  [Tooltip("Time after BreakPlaceable to reset item")]
  [SerializeField]
  private float respawnDelay = 0.5f;
  [FormerlySerializedAs("OnBroken")]
  [Space]
  public UnityEvent OnBrokenShared;
  private static int breakableBitmask = 32 /*0x20*/;
  private bool placedOnFloor;
  private float placedOnFloorTime = -1f;
  private bool broken;
  private float brokenTime = -1f;
  private VRRig cachedLocalRig;
  private HoldableObject holdableParent;
  private TransferrableObject transferrableParent;
  private RigOwnedPhysicsBody rigOwnedPhysicsBody;
  private double throwSettledTime = -1.0;
  private int landingSide;
  private float scale;
  private Collider bodyCollider;
  [Tooltip("How many directions to test per physics tick (spreads work across frames).")]
  [SerializeField]
  [Min(1f)]
  private int raysPerStep = 3;
  [Tooltip("Run a raycast step only every N physics ticks (1 = every FixedUpdate).")]
  [SerializeField]
  [Min(1f)]
  private int stepEveryNFrames = 2;
  [Tooltip("Small skin so rays start just outside our own collider volume.")]
  [SerializeField]
  [Range(0.005f, 0.1f)]
  private float selfSkinOffset = 0.02f;
  [SerializeField]
  private bool debugPlacementRays;
  private int currentRayIndex;
  private int frameCounter;
  private static readonly Dictionary<int, Vector3[]> directionCache = new Dictionary<int, Vector3[]>();
  private static readonly Vector3[] tmpEmpty = Array.Empty<Vector3>();

  private void Awake()
  {
    this.rigOwnedPhysicsBody = this.GetComponent<RigOwnedPhysicsBody>();
    this.bodyCollider = this.GetComponent<Collider>();
  }

  private void Start() => this.enabled = false;

  private void OnEnable()
  {
    if (!((UnityEngine.Object) this.rigOwnedPhysicsBody != (UnityEngine.Object) null))
      return;
    this.rigOwnedPhysicsBody.enabled = true;
  }

  private void OnDisable()
  {
    if (!((UnityEngine.Object) this.rigOwnedPhysicsBody != (UnityEngine.Object) null))
      return;
    this.rigOwnedPhysicsBody.enabled = false;
  }

  protected internal override void Pickup(bool isAutoPickup = false)
  {
    if (!isAutoPickup)
      this.OnPickupShared?.Invoke();
    this.rb.linearVelocity = Vector3.zero;
    this.rb.isKinematic = true;
    if ((UnityEngine.Object) this.holdableParent != (UnityEngine.Object) null)
      this.transform.parent = this.holdableParent.transform;
    this.transform.localPosition = Vector3.zero;
    this.transform.localRotation = Quaternion.identity;
    this.transform.localScale = Vector3.one;
    this.scale = 1f;
    this.placedOnFloorTime = -1f;
    this.placedOnFloor = false;
    this.broken = false;
    this.brokenTime = -1f;
    if (this.isBreakable && (UnityEngine.Object) this.transferrableParent != (UnityEngine.Object) null && this.transferrableParent.IsLocalObject())
    {
      this.transferrableParent.itemState &= (TransferrableObject.ItemStates) ~PickupableCosmetic.breakableBitmask;
      if ((UnityEngine.Object) this.breakEffect != (UnityEngine.Object) null && this.breakEffect.isPlaying)
        this.breakEffect.Stop();
    }
    this.ShowRenderers(true);
    if ((UnityEngine.Object) this.interactionPoint != (UnityEngine.Object) null)
      this.interactionPoint.enabled = true;
    this.enabled = false;
  }

  protected internal override void DelayedPickup()
  {
    this.StartCoroutine(this.DelayedPickup_Internal());
  }

  private IEnumerator DelayedPickup_Internal()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    PickupableCosmetic pickupableCosmetic = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      pickupableCosmetic.Pickup(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(1f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected internal override void Release(
    HoldableObject holdable,
    Vector3 startPosition,
    Vector3 velocity,
    float playerScale)
  {
    this.holdableParent = holdable;
    this.transform.parent = (Transform) null;
    this.transform.position = startPosition;
    this.transform.localScale = Vector3.one * playerScale;
    this.rb.isKinematic = false;
    this.rb.useGravity = true;
    this.rb.linearVelocity = velocity;
    this.rb.detectCollisions = true;
    if (!this.allowPickupFromGround && (UnityEngine.Object) this.interactionPoint != (UnityEngine.Object) null)
      this.interactionPoint.enabled = false;
    this.scale = playerScale;
    this.enabled = true;
    this.transferrableParent = this.holdableParent as TransferrableObject;
    this.currentRayIndex = 0;
    this.frameCounter = 0;
  }

  private void FixedUpdate()
  {
    if (this.isBreakable && this.broken)
    {
      if ((double) Time.time <= (double) this.respawnDelay + (double) this.brokenTime)
        return;
      this.Pickup(false);
    }
    else
    {
      if (this.isBreakable && this.placedOnFloor)
      {
        bool flag = (this.transferrableParent.itemState & (TransferrableObject.ItemStates) PickupableCosmetic.breakableBitmask) != 0;
        if (flag != this.broken & flag)
          this.OnBreakReplicated();
      }
      if ((double) this.autoPickupAfterSeconds > 0.0 && this.placedOnFloor && (double) Time.time - (double) this.placedOnFloorTime > (double) this.autoPickupAfterSeconds)
      {
        this.Pickup(true);
        ThrowablePickupableCosmetic transferrableParent = this.transferrableParent as ThrowablePickupableCosmetic;
        if ((bool) (UnityEngine.Object) transferrableParent)
          transferrableParent.OnReturnToDockPositionShared?.Invoke();
      }
      if ((double) this.autoPickupDistance > 0.0 && (UnityEngine.Object) this.transferrableParent != (UnityEngine.Object) null && (this.transferrableParent.ownerRig.transform.position - this.transform.position).IsLongerThan(this.autoPickupDistance))
        this.Pickup(false);
      if (this.placedOnFloor || !this.enabled)
        return;
      ++this.frameCounter;
      if (this.frameCounter % this.stepEveryNFrames != 0)
        return;
      float maxDistance = this.RaycastCheckDist * this.scale;
      int layerMask = this.floorLayerMask.value;
      Vector3[] cachedDirections = this.GetCachedDirections(this.RaycastChecksMax);
      int num = 0;
      while (num < this.raysPerStep && this.currentRayIndex < cachedDirections.Length)
      {
        Vector3 vector3 = cachedDirections[this.currentRayIndex];
        ++this.currentRayIndex;
        ++num;
        UnityEngine.RaycastHit hitInfo;
        if (Physics.Raycast(this.GetSafeRayOrigin(this.raycastOrigin.position, vector3), vector3, out hitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore) && (!this.dontStickToWall || (double) Vector3.Angle(hitInfo.normal, Vector3.up) < 40.0))
        {
          this.SettleBanner(hitInfo);
          this.OnPlacedShared?.Invoke();
          this.placedOnFloor = true;
          this.placedOnFloorTime = Time.time;
          break;
        }
      }
      if (this.currentRayIndex < cachedDirections.Length)
        return;
      this.currentRayIndex = 0;
    }
  }

  private void SettleBanner(UnityEngine.RaycastHit hitInfo)
  {
    this.rb.isKinematic = true;
    this.rb.useGravity = false;
    this.rb.detectCollisions = false;
    Vector3 normal = hitInfo.normal;
    this.transform.position = hitInfo.point + normal * this.placementOffset;
    this.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(this.transform.forward, normal).normalized, normal);
  }

  private Vector3 GetFibonacciSphereDirection(int index, int total)
  {
    double f1 = (double) Mathf.Acos((float) (1.0 - 2.0 * ((double) index + 0.5) / (double) total));
    float f2 = (float) (3.1415927410125732 * (1.0 + (double) Mathf.Sqrt(5f)) * ((double) index + 0.5));
    return new Vector3(Mathf.Sin((float) f1) * Mathf.Cos(f2), Mathf.Sin((float) f1) * Mathf.Sin(f2), Mathf.Cos((float) f1)).normalized;
  }

  private Vector3[] GetCachedDirections(int count)
  {
    if (count <= 0)
      return PickupableCosmetic.tmpEmpty;
    Vector3[] cachedDirections;
    if (PickupableCosmetic.directionCache.TryGetValue(count, out cachedDirections))
      return cachedDirections;
    cachedDirections = new Vector3[count];
    for (int index = 0; index < count; ++index)
      cachedDirections[index] = this.GetFibonacciSphereDirection(index, count);
    PickupableCosmetic.directionCache[count] = cachedDirections;
    return cachedDirections;
  }

  private Vector3 GetSafeRayOrigin(Vector3 rawOrigin, Vector3 dir)
  {
    float num = this.selfSkinOffset;
    if ((UnityEngine.Object) this.bodyCollider != (UnityEngine.Object) null)
      num = Mathf.Max(this.selfSkinOffset, this.bodyCollider.bounds.extents.magnitude * 0.05f);
    return rawOrigin - dir.normalized * num;
  }

  public void BreakPlaceable()
  {
    if (!this.isBreakable || !this.placedOnFloor)
      return;
    if ((UnityEngine.Object) this.transferrableParent != (UnityEngine.Object) null && this.transferrableParent.IsLocalObject())
      this.transferrableParent.itemState |= (TransferrableObject.ItemStates) PickupableCosmetic.breakableBitmask;
    else
      GTDev.LogError<string>($"PickupableCosmetic {this.gameObject.name} has no TransferrableObject parent. Break effects cannot be replicated");
  }

  private void OnBreakReplicated() => this.PlayBreakEffects();

  protected virtual void PlayBreakEffects()
  {
    if (!this.isBreakable || !this.placedOnFloor || this.broken)
      return;
    this.broken = true;
    this.brokenTime = Time.time;
    if ((UnityEngine.Object) this.breakEffect != (UnityEngine.Object) null)
    {
      if (this.breakEffect.isPlaying)
        this.breakEffect.Stop();
      this.breakEffect.Play();
    }
    if ((UnityEngine.Object) this.interactionPoint != (UnityEngine.Object) null)
      this.interactionPoint.enabled = false;
    this.ShowRenderers(false);
    this.OnBrokenShared?.Invoke();
  }

  protected virtual void ShowRenderers(bool visible)
  {
    if (this.hideOnBreak.IsNullOrEmpty<Renderer>())
      return;
    for (int index = 0; index < this.hideOnBreak.Length; ++index)
    {
      Renderer renderer = this.hideOnBreak[index];
      if (!((UnityEngine.Object) renderer == (UnityEngine.Object) null))
        renderer.forceRenderingOff = !visible;
    }
  }
}
