// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ChargeableCosmeticEffects
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class ChargeableCosmeticEffects : MonoBehaviour, ITickSystemTick
{
  [SerializeField]
  private float maxChargeSeconds = 1f;
  [SerializeField]
  private float chargeGainSpeed = 1f;
  [SerializeField]
  private float chargeLossSpeed = 1f;
  [Tooltip("This will remap the internal charge output to whatever you set. The remapped value will be output by 'whileCharging' and the 'continuousProperties' (keep in mind that the remapped value will then be used as an INPUT for the curves on each ContinuousProperty).\n\nIt should start at (0,0) and end at (1,1).\n\nDisabled if there are no ContinuousProperties and no whileCharging event callbacks.")]
  [SerializeField]
  private AnimationCurve masterChargeRemapCurve = AnimationCurves.Linear;
  [SerializeField]
  private bool isCharging;
  [SerializeField]
  private ContinuousPropertyArray continuousProperties;
  [SerializeField]
  private UnityEvent<float> whileCharging;
  [SerializeField]
  private UnityEvent onMaxCharge;
  [SerializeField]
  private UnityEvent onNoCharge;
  private float chargeTime;
  private float inverseMaxChargeSeconds;
  private bool hasFractionalsCached;

  private bool HasFractionals()
  {
    return this.continuousProperties.Count > 0 || this.whileCharging.GetPersistentEventCount() > 0;
  }

  private void Awake()
  {
    this.inverseMaxChargeSeconds = 1f / this.maxChargeSeconds;
    this.hasFractionalsCached = this.HasFractionals();
  }

  public void SetMaxChargeSeconds(float s)
  {
    this.maxChargeSeconds = s;
    this.inverseMaxChargeSeconds = 1f / this.maxChargeSeconds;
    this.SetChargeTime(this.chargeTime);
  }

  public void SetChargeState(bool state)
  {
    if (this.isCharging == state)
      return;
    TickSystem<object>.AddTickCallback((ITickSystemTick) this);
    this.isCharging = state;
  }

  public void StartCharging() => this.SetChargeState(true);

  public void StopCharging() => this.SetChargeState(false);

  public void ToggleCharging() => this.SetChargeState(!this.isCharging);

  public void SetChargeTime(float t)
  {
    if ((double) t >= (double) this.maxChargeSeconds)
    {
      if ((double) this.chargeTime >= (double) this.maxChargeSeconds)
        return;
      this.RunMaxCharge();
    }
    else if ((double) t <= 0.0)
    {
      if ((double) this.chargeTime <= 0.0)
        return;
      this.RunNoCharge();
    }
    else
    {
      TickSystem<object>.AddTickCallback((ITickSystemTick) this);
      this.chargeTime = t;
      if (!this.hasFractionalsCached)
        return;
      this.RunChargeFrac();
    }
  }

  public void SetChargeFrac(float f) => this.SetChargeTime(f * this.maxChargeSeconds);

  public void EmptyCharge() => this.SetChargeTime(0.0f);

  public void FillCharge() => this.SetChargeTime(this.maxChargeSeconds);

  public void EmptyAndStop()
  {
    this.isCharging = false;
    this.EmptyCharge();
  }

  public void FillAndStop()
  {
    this.StopCharging();
    this.FillCharge();
  }

  public void EmptyAndStart()
  {
    this.StartCharging();
    this.EmptyCharge();
  }

  public void FillAndStart()
  {
    this.isCharging = true;
    this.FillCharge();
  }

  private void OnEnable()
  {
    if (((double) this.chargeTime > 0.0 || !this.isCharging) && ((double) this.chargeTime < (double) this.maxChargeSeconds || this.isCharging) && ((double) this.chargeTime <= 0.0 || (double) this.chargeTime >= (double) this.maxChargeSeconds))
      return;
    TickSystem<object>.AddTickCallback((ITickSystemTick) this);
  }

  private void OnDisable() => TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);

  private void RunMaxCharge()
  {
    if (this.isCharging)
      TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
    else
      TickSystem<object>.AddTickCallback((ITickSystemTick) this);
    this.chargeTime = this.maxChargeSeconds;
    this.onMaxCharge?.Invoke();
    this.whileCharging?.Invoke(1f);
    this.continuousProperties.ApplyAll(1f);
  }

  private void RunNoCharge()
  {
    if (!this.isCharging)
      TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
    else
      TickSystem<object>.AddTickCallback((ITickSystemTick) this);
    this.chargeTime = 0.0f;
    this.onNoCharge?.Invoke();
    this.whileCharging?.Invoke(0.0f);
    this.continuousProperties.ApplyAll(0.0f);
  }

  private void RunChargeFrac()
  {
    float f = this.masterChargeRemapCurve.Evaluate(this.chargeTime * this.inverseMaxChargeSeconds);
    this.whileCharging?.Invoke(f);
    this.continuousProperties.ApplyAll(f);
  }

  public bool TickRunning { get; set; }

  public void Tick()
  {
    if (this.isCharging && (double) this.chargeTime < (double) this.maxChargeSeconds)
    {
      this.chargeTime += Time.deltaTime * this.chargeGainSpeed;
      if ((double) this.chargeTime >= (double) this.maxChargeSeconds)
      {
        this.RunMaxCharge();
      }
      else
      {
        if (!this.hasFractionalsCached)
          return;
        this.RunChargeFrac();
      }
    }
    else
    {
      if (this.isCharging || (double) this.chargeTime <= 0.0)
        return;
      this.chargeTime -= Time.deltaTime * this.chargeLossSpeed;
      if ((double) this.chargeTime <= 0.0)
      {
        this.RunNoCharge();
      }
      else
      {
        if (!this.hasFractionalsCached)
          return;
        this.RunChargeFrac();
      }
    }
  }
}
