// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.GlowBugsInJar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using Photon.Pun;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class GlowBugsInJar : MonoBehaviour
{
  [SerializeField]
  private TransferrableObject transferrableObject;
  [Space]
  [Tooltip("Time interval - every X seconds update the glow value")]
  [SerializeField]
  private float glowUpdateInterval = 2f;
  [Tooltip("step increment - increase the glow value one step for N amount")]
  [SerializeField]
  private float glowIncreaseStepAmount = 0.1f;
  [Tooltip("step decrement - decrease the glow value one step for N amount")]
  [SerializeField]
  private float glowDecreaseStepAmount = 0.2f;
  [Space]
  [SerializeField]
  private string shaderProperty = "_EmissionColor";
  [SerializeField]
  private Renderer[] renderers;
  private bool shakeStarted = true;
  private static int EmissionColor;
  private float currentGlowAmount;
  private float shakeTimer;
  private RubberDuckEvents _events;
  private CallLimiter callLimiter = new CallLimiter(10, 2f);

  private void OnEnable()
  {
    this.shakeStarted = false;
    this.UpdateGlow(0.0f);
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
    if (!this.callLimiter.CheckCallTime(Time.time) || args == null || args.Length != 1 || !(args[0] is bool flag))
      return;
    if (flag)
      this.ShakeStartLocal();
    else
      this.ShakeEndLocal();
  }

  public void HandleOnShakeStart()
  {
    if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
      this._events.Activate.RaiseOthers((object) true);
    this.ShakeStartLocal();
  }

  private void ShakeStartLocal()
  {
    this.currentGlowAmount = 0.0f;
    this.shakeStarted = true;
    this.shakeTimer = 0.0f;
  }

  public void HandleOnShakeEnd()
  {
    if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
      this._events.Activate.RaiseOthers((object) false);
    this.ShakeEndLocal();
  }

  private void ShakeEndLocal()
  {
    this.shakeStarted = false;
    this.shakeTimer = 0.0f;
  }

  public void Update()
  {
    if (this.shakeStarted)
    {
      ++this.shakeTimer;
      if ((double) this.shakeTimer < (double) this.glowUpdateInterval || (double) this.currentGlowAmount >= 1.0)
        return;
      this.currentGlowAmount += this.glowIncreaseStepAmount;
      this.UpdateGlow(this.currentGlowAmount);
      this.shakeTimer = 0.0f;
    }
    else
    {
      ++this.shakeTimer;
      if ((double) this.shakeTimer < (double) this.glowUpdateInterval || (double) this.currentGlowAmount <= 0.0)
        return;
      this.currentGlowAmount -= this.glowDecreaseStepAmount;
      this.UpdateGlow(this.currentGlowAmount);
      this.shakeTimer = 0.0f;
    }
  }

  private void UpdateGlow(float value)
  {
    if (this.renderers.Length == 0)
      return;
    for (int index = 0; index < this.renderers.Length; ++index)
    {
      Material material = this.renderers[index].material;
      material.SetColor(this.shaderProperty, material.GetColor(this.shaderProperty) with
      {
        a = value
      });
      material.EnableKeyword("_EMISSION");
    }
  }
}
