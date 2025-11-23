// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.SnakeInCanHoldable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class SnakeInCanHoldable : TransferrableObject
{
  [SerializeField]
  private float jumpSpeed;
  [SerializeField]
  private Transform stretchedPoint;
  [SerializeField]
  private Transform compressedPoint;
  [SerializeField]
  private GameObject topRigObject;
  [SerializeField]
  private GameObject disableObjectBeforeTrigger;
  private CallLimiter snakeInCanCallLimiter = new CallLimiter(10, 2f);
  private Vector3 topRigPosition;
  private Vector3 originalTopRigPosition;
  private RubberDuckEvents _events;

  protected override void Awake()
  {
    base.Awake();
    this.topRigPosition = this.topRigObject.transform.position;
  }

  internal override void OnEnable()
  {
    base.OnEnable();
    this.disableObjectBeforeTrigger.SetActive(false);
    if ((UnityEngine.Object) this.compressedPoint != (UnityEngine.Object) null)
      this.topRigObject.transform.position = this.compressedPoint.position;
    if ((UnityEngine.Object) this._events == (UnityEngine.Object) null)
    {
      this._events = this.gameObject.GetOrAddComponent<RubberDuckEvents>();
      NetPlayer player = (UnityEngine.Object) this.myOnlineRig != (UnityEngine.Object) null ? this.myOnlineRig.creator : ((UnityEngine.Object) this.myRig != (UnityEngine.Object) null ? (this.myRig.creator != null ? this.myRig.creator : NetworkSystem.Instance.LocalPlayer) : (NetPlayer) null);
      if (player != null)
        this._events.Init(player);
    }
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnEnableObject);
  }

  internal override void OnDisable()
  {
    base.OnDisable();
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnEnableObject);
    this._events.Dispose();
    this._events = (RubberDuckEvents) null;
  }

  public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
  {
    if (!base.OnRelease(zoneReleased, releasingHand) || (UnityEngine.Object) VRRigCache.Instance.localRig.Rig != (UnityEngine.Object) this.ownerRig)
      return false;
    if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
      this._events.Activate.RaiseOthers((object) false);
    this.EnableObjectLocal(false);
    return true;
  }

  private void OnEnableObject(int sender, int target, object[] arg, PhotonMessageInfoWrapped info)
  {
    if (info.senderID != this.ownerRig.creator.ActorNumber || arg.Length != 1 || !(arg[0] is bool) || sender != target)
      return;
    GorillaNot.IncrementRPCCall(info, nameof (OnEnableObject));
    if (!this.snakeInCanCallLimiter.CheckCallTime(Time.time))
      return;
    this.EnableObjectLocal((bool) arg[0]);
  }

  private void EnableObjectLocal(bool enable)
  {
    this.disableObjectBeforeTrigger.SetActive(enable);
    if (enable)
    {
      if ((UnityEngine.Object) this.stretchedPoint != (UnityEngine.Object) null)
        this.StartCoroutine(this.SmoothTransition());
      else
        this.topRigObject.transform.position = this.topRigPosition;
    }
    else
    {
      if (!((UnityEngine.Object) this.compressedPoint != (UnityEngine.Object) null))
        return;
      this.topRigObject.transform.position = this.compressedPoint.position;
    }
  }

  private IEnumerator SmoothTransition()
  {
    while ((double) Vector3.Distance(this.topRigObject.transform.position, this.stretchedPoint.position) > 0.0099999997764825821)
    {
      this.topRigObject.transform.position = Vector3.MoveTowards(this.topRigObject.transform.position, this.stretchedPoint.position, this.jumpSpeed * Time.deltaTime);
      yield return (object) null;
    }
    this.topRigObject.transform.position = this.stretchedPoint.position;
  }

  public void OnButtonPressed() => this.EnableObjectLocal(true);
}
