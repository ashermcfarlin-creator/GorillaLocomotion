// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.SeedPacketHoldable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

[RequireComponent(typeof (TransferrableObject))]
public class SeedPacketHoldable : MonoBehaviour
{
  [SerializeField]
  private float cooldown;
  [SerializeField]
  private ParticleSystem particles;
  [SerializeField]
  private float pouringAngle;
  [SerializeField]
  private float pouringRaycastDistance = 5f;
  [SerializeField]
  private LayerMask raycastLayerMask;
  [SerializeField]
  private float placeEffectDelayMultiplier = 10f;
  [SerializeField]
  private GameObject flowerEffectPrefab;
  private List<SeedPacketTriggerHandler> pooledObjects = new List<SeedPacketTriggerHandler>();
  private CallLimiter callLimiter = new CallLimiter(10, 3f);
  private int flowerEffectHash;
  private Vector3 hitPoint;
  private TransferrableObject transferrableObject;
  private bool isPouring = true;
  private float pouringStartedTime;
  private RubberDuckEvents _events;

  private void Awake()
  {
    this.transferrableObject = this.GetComponent<TransferrableObject>();
    this.flowerEffectHash = PoolUtils.GameObjHashCode(this.flowerEffectPrefab);
  }

  private void OnEnable()
  {
    if ((UnityEngine.Object) this._events == (UnityEngine.Object) null)
    {
      this._events = this.gameObject.GetOrAddComponent<RubberDuckEvents>();
      NetPlayer player = (UnityEngine.Object) this.transferrableObject.myOnlineRig != (UnityEngine.Object) null ? this.transferrableObject.myOnlineRig.creator : ((UnityEngine.Object) this.transferrableObject.myRig != (UnityEngine.Object) null ? this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer : (NetPlayer) null);
      if (player != null)
        this._events.Init(player);
    }
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SyncTriggerEffect);
  }

  private void OnDisable()
  {
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.SyncTriggerEffect);
    this._events.Dispose();
    this._events = (RubberDuckEvents) null;
  }

  private void OnDestroy() => this.pooledObjects.Clear();

  private void Update()
  {
    if (!this.transferrableObject.InHand())
      return;
    if (!this.isPouring && (double) Vector3.Angle(this.transform.up, Vector3.down) <= (double) this.pouringAngle)
    {
      this.StartPouring();
      UnityEngine.RaycastHit hitInfo;
      if (Physics.Raycast(this.transform.position, Vector3.down, out hitInfo, this.pouringRaycastDistance, (int) this.raycastLayerMask))
      {
        this.hitPoint = hitInfo.point;
        this.Invoke("SpawnEffect", hitInfo.distance * this.placeEffectDelayMultiplier);
      }
    }
    if (!this.isPouring || (double) Time.time - (double) this.pouringStartedTime < (double) this.cooldown)
      return;
    this.isPouring = false;
  }

  private void StartPouring()
  {
    if ((bool) (UnityEngine.Object) this.particles)
      this.particles.Play();
    this.isPouring = true;
    this.pouringStartedTime = Time.time;
  }

  private void SpawnEffect()
  {
    GameObject gameObject = ObjectPools.instance.Instantiate(this.flowerEffectHash);
    gameObject.transform.position = this.hitPoint;
    SeedPacketTriggerHandler component;
    if (!gameObject.TryGetComponent<SeedPacketTriggerHandler>(out component))
      return;
    this.pooledObjects.Add(component);
    component.onTriggerEntered.AddListener(new UnityAction<SeedPacketTriggerHandler>(this.SyncTriggerEffectForOthers));
  }

  private void SyncTriggerEffectForOthers(
    SeedPacketTriggerHandler seedPacketTriggerHandlerTriggerHandlerEvent)
  {
    int num = this.pooledObjects.IndexOf(seedPacketTriggerHandlerTriggerHandlerEvent);
    if (!PhotonNetwork.InRoom || !((UnityEngine.Object) this._events != (UnityEngine.Object) null) || !(this._events.Activate != (PhotonEvent) null))
      return;
    this._events.Activate.RaiseOthers((object) num);
  }

  private void SyncTriggerEffect(
    int sender,
    int target,
    object[] args,
    PhotonMessageInfoWrapped info)
  {
    if (sender != target || args.Length != 1)
      return;
    GorillaNot.IncrementRPCCall(info, nameof (SyncTriggerEffect));
    if (!this.callLimiter.CheckCallTime(Time.time))
      return;
    int index = (int) args[0];
    if (index < 0 && index >= this.pooledObjects.Count)
      return;
    this.pooledObjects[index].ToggleEffects();
  }
}
