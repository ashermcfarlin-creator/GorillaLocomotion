// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.SprayCanCosmeticNetworked
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class SprayCanCosmeticNetworked : MonoBehaviour
{
  [SerializeField]
  private TransferrableObject transferrableObject;
  private RubberDuckEvents _events;
  private CallLimiter callLimiter = new CallLimiter(10, 1f);
  public UnityEvent HandleOnShakeStart;
  public UnityEvent HandleOnShakeEnd;

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
    this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnShakeEvent);
  }

  private void OnDisable()
  {
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnShakeEvent);
    this._events.Dispose();
    this._events = (RubberDuckEvents) null;
  }

  private void OnShakeEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
  {
    if (sender != target)
      return;
    GorillaNot.IncrementRPCCall(info, nameof (OnShakeEvent));
    if (info.Sender != this.transferrableObject.myOnlineRig?.creator || !this.callLimiter.CheckCallTime(Time.time) || !(args[0] is bool flag))
      return;
    if (flag)
      this.HandleOnShakeStart?.Invoke();
    else
      this.HandleOnShakeEnd?.Invoke();
  }

  public void OnShakeStart()
  {
    if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
      this._events.Activate.RaiseOthers((object) true);
    this.HandleOnShakeStart?.Invoke();
  }

  public void OnShakeEnd()
  {
    if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
      this._events.Activate.RaiseOthers((object) false);
    this.HandleOnShakeEnd?.Invoke();
  }
}
