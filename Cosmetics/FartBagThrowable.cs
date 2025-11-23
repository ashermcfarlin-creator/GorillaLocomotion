// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.FartBagThrowable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using Photon.Pun;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class FartBagThrowable : MonoBehaviour, IProjectile
{
  [SerializeField]
  private GameObject deflationEffect;
  [SerializeField]
  private float destroyWhenDeflateDelay = 3f;
  [SerializeField]
  private float forceDestroyAfterSec = 10f;
  [SerializeField]
  private float placementOffset = 0.2f;
  [SerializeField]
  private UpdateBlendShapeCosmetic updateBlendShapeCosmetic;
  [SerializeField]
  private LayerMask floorLayerMask;
  [SerializeField]
  private LayerMask handLayerMask;
  [SerializeField]
  private Rigidbody rigidbody;
  private bool placedOnFloor;
  private float placedOnFloorTime;
  private float timeCreated;
  private bool deflated;
  private Vector3 handContactPoint;
  private Vector3 handNormalVector;
  private CallLimiter callLimiter = new CallLimiter(10, 2f);
  private RubberDuckEvents _events;

  public TransferrableObject ParentTransferable { get; set; }

  public event Action<IProjectile> OnDeflated;

  private void OnEnable()
  {
    this.placedOnFloor = false;
    this.deflated = false;
    this.handContactPoint = Vector3.negativeInfinity;
    this.handNormalVector = Vector3.zero;
    this.timeCreated = float.PositiveInfinity;
    this.placedOnFloorTime = float.PositiveInfinity;
    if (!(bool) (UnityEngine.Object) this.updateBlendShapeCosmetic)
      return;
    this.updateBlendShapeCosmetic.ResetBlend();
  }

  private void Update()
  {
    if ((double) Time.time - (double) this.timeCreated <= (double) this.forceDestroyAfterSec)
      return;
    this.DeflateLocal();
  }

  public void Launch(
    Vector3 startPosition,
    Quaternion startRotation,
    Vector3 velocity,
    float chargeFrac,
    VRRig ownerRig,
    int progress)
  {
    this.transform.position = startPosition;
    this.transform.rotation = startRotation;
    this.transform.localScale = Vector3.one * ownerRig.scaleFactor;
    this.rigidbody.linearVelocity = velocity;
    this.timeCreated = Time.time;
    this.InitialPhotonEvent();
  }

  private void InitialPhotonEvent()
  {
    this._events = this.gameObject.GetOrAddComponent<RubberDuckEvents>();
    if ((bool) (UnityEngine.Object) this.ParentTransferable)
    {
      NetPlayer player = (UnityEngine.Object) this.ParentTransferable.myOnlineRig != (UnityEngine.Object) null ? this.ParentTransferable.myOnlineRig.creator : ((UnityEngine.Object) this.ParentTransferable.myRig != (UnityEngine.Object) null ? this.ParentTransferable.myRig.creator ?? NetworkSystem.Instance.LocalPlayer : (NetPlayer) null);
      if ((UnityEngine.Object) this._events != (UnityEngine.Object) null && player != null)
        this._events.Init(player);
    }
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.DeflateEvent);
  }

  private void OnTriggerEnter(Collider other)
  {
    if ((this.handLayerMask.value & 1 << other.gameObject.layer) == 0 || !this.placedOnFloor)
      return;
    this.handContactPoint = other.ClosestPoint(this.transform.position);
    this.handNormalVector = (this.handContactPoint - this.transform.position).normalized;
    if ((double) Time.time - (double) this.placedOnFloorTime <= 0.30000001192092896)
      return;
    this.Deflate();
  }

  private void OnCollisionEnter(Collision other)
  {
    if ((this.floorLayerMask.value & 1 << other.gameObject.layer) == 0)
      return;
    this.placedOnFloor = true;
    this.placedOnFloorTime = Time.time;
    Vector3 normal = other.contacts[0].normal;
    this.transform.position = other.contacts[0].point + normal * this.placementOffset;
    this.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(this.transform.forward, normal).normalized, normal);
  }

  private void Deflate()
  {
    if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
      this._events.Activate.RaiseOthers((object) this.handContactPoint, (object) this.handNormalVector);
    this.DeflateLocal();
  }

  private void DeflateEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
  {
    if (sender != target || args.Length != 2)
      return;
    GorillaNot.IncrementRPCCall(info, nameof (DeflateEvent));
    if (!this.callLimiter.CheckCallTime(Time.time) || !(args[0] is Vector3 position) || !(args[1] is Vector3 vector3))
      return;
    ref Vector3 local1 = ref vector3;
    float num = 10000f;
    ref float local2 = ref num;
    if (!local1.IsValid(in local2))
      return;
    ref Vector3 local3 = ref position;
    num = 10000f;
    ref float local4 = ref num;
    if (!local3.IsValid(in local4) || !this.ParentTransferable.targetRig.IsPositionInRange(position, 4f))
      return;
    this.handNormalVector = vector3;
    this.handContactPoint = position;
    this.DeflateLocal();
  }

  private void DeflateLocal()
  {
    if (this.deflated)
      return;
    GameObject gameObject = ObjectPools.instance.Instantiate(this.deflationEffect, this.handContactPoint);
    gameObject.transform.up = this.handNormalVector;
    gameObject.transform.position = this.transform.position;
    SoundBankPlayer componentInChildren = gameObject.GetComponentInChildren<SoundBankPlayer>();
    if ((bool) (UnityEngine.Object) componentInChildren.soundBank)
      componentInChildren.Play();
    this.placedOnFloor = false;
    this.timeCreated = float.PositiveInfinity;
    if ((bool) (UnityEngine.Object) this.updateBlendShapeCosmetic)
      this.updateBlendShapeCosmetic.FullyBlend();
    this.deflated = true;
    this.Invoke("DisableObject", this.destroyWhenDeflateDelay);
  }

  private void DisableObject()
  {
    Action<IProjectile> onDeflated = this.OnDeflated;
    if (onDeflated != null)
      onDeflated((IProjectile) this);
    this.deflated = false;
  }

  private void OnDestroy()
  {
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.DeflateEvent);
    this._events.Dispose();
    this._events = (RubberDuckEvents) null;
  }
}
