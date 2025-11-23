// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ShakeReactorCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag.Cosmetics;

public class ShakeReactorCosmetic : MonoBehaviour, ISpawnable
{
  [Header("Speed Source")]
  [Tooltip("Speed component provider")]
  [SerializeField]
  private SimpleSpeedTracker speedTracker;
  [Header("Settings")]
  [Tooltip("Minimum reversals-per-second required to consider motion a shake - Hz.")]
  [SerializeField]
  private float shakeRateThreshold = 1f;
  [Tooltip("Minimum distance traveled between direction reversals to count as a valid half-cycle.")]
  [SerializeField]
  private float shakeAmplitudeThreshold = 0.1f;
  [Tooltip("Minimum angle change (degrees) between consecutive lobes to register a reversal. Higher = stricter.")]
  [SerializeField]
  [Range(10f, 170f)]
  private float angleToleranceDeg = 120f;
  [Tooltip("Minimum speed required to accept a direction reversal, ignores tiny jitter near stop.")]
  [SerializeField]
  private float minSpeedForReversal = 0.2f;
  [Tooltip("After a shake ends, how long to wait before ShakeStartLocal can fire again")]
  [SerializeField]
  private float startCooldownSeconds = 0.2f;
  [SerializeField]
  private bool useMaxes;
  [Tooltip("If enabled, exceeding this rate is considered a max shake.")]
  [SerializeField]
  private float maxShakeRate = 6f;
  [Tooltip("If enabled, exceeding this amplitude per half cycle is considered a max shake.")]
  [SerializeField]
  private float maxShakeAmplitude = 0.3f;
  [Header("Continuous Output")]
  [SerializeField]
  private ContinuousPropertyArray continuousProperties;
  [Header("Advanced")]
  [Tooltip("When no hard max amplitude is defined, strength is mapped to Threshold × this multiplier.")]
  [SerializeField]
  private float softMaxMultiplier = 3f;
  [FormerlySerializedAs("ShakeStart")]
  [Header("Events")]
  public UnityEvent ShakeStartLocal;
  public UnityEvent ShakeStartShared;
  [FormerlySerializedAs("ShakeEnd")]
  public UnityEvent ShakeEndLocal;
  public UnityEvent ShakeEndShared;
  public UnityEvent MaxShake;
  [Header("Debug")]
  public bool isShaking;
  public float lastAmplitudeMeters;
  public float debugCurrentHalfCycleDistance;
  public float debugCurrentRateHz;
  private const int kFrequencyHistoryCount = 1;
  private const float kNoReversalGraceMultiplier = 1f;
  private readonly Queue<float> recentHalfCycleDurations = new Queue<float>();
  private Vector3 lastVelocityDir;
  private bool hasLastDir;
  private float lastReversalTime;
  private Vector3 lastPosition;
  private float pathSinceLastReversal;
  private float nextAllowedShakeStartTime;
  private const float kEpsilon = 1E-05f;
  private const float kTinyVelocitySqr = 1E-06f;
  private const float kMinHalfCycleDuration = 0.0005f;
  private const float kHalfPerCycle = 0.5f;
  private RubberDuckEvents _events;
  private CallLimiter callLimiter = new CallLimiter(10, 1f);
  private VRRig myRig;
  private bool subscribed;

  private void OnEnable()
  {
    this.lastReversalTime = Time.time;
    this.pathSinceLastReversal = 0.0f;
    this.recentHalfCycleDurations.Clear();
    this.hasLastDir = false;
    this.lastPosition = (UnityEngine.Object) this.speedTracker != (UnityEngine.Object) null ? this.speedTracker.transform.position : this.transform.position;
    this.isShaking = false;
    this.debugCurrentHalfCycleDistance = 0.0f;
    this.debugCurrentRateHz = 0.0f;
    this.lastAmplitudeMeters = 0.0f;
    this.nextAllowedShakeStartTime = Time.time;
    if ((UnityEngine.Object) this.myRig == (UnityEngine.Object) null)
      this.myRig = this.GetComponentInParent<VRRig>();
    if ((UnityEngine.Object) this._events == (UnityEngine.Object) null)
      this._events = this.gameObject.GetOrAddComponent<RubberDuckEvents>();
    NetPlayer player = (UnityEngine.Object) this.myRig != (UnityEngine.Object) null ? this.myRig.creator ?? NetworkSystem.Instance.LocalPlayer : NetworkSystem.Instance.LocalPlayer;
    if (player != null)
      this._events.Init(player);
    if (this.subscribed || !(this._events.Activate != (PhotonEvent) null))
      return;
    this._events.Activate.reliable = true;
    this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnShake);
    this.subscribed = true;
  }

  private void OnDisable()
  {
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnShake);
    this.subscribed = false;
    this._events.Dispose();
    this._events = (RubberDuckEvents) null;
  }

  private void Update()
  {
    if ((UnityEngine.Object) this.myRig != (UnityEngine.Object) null && !this.myRig.isLocal)
      return;
    if ((UnityEngine.Object) this.speedTracker == (UnityEngine.Object) null)
    {
      if (!this.isShaking)
        return;
      this.isShaking = false;
      if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
        this._events.Activate.RaiseOthers((object) this.isShaking);
      this.ShakeEndShared?.Invoke();
      this.ShakeEndLocal?.Invoke();
      this.nextAllowedShakeStartTime = Time.time + Mathf.Max(0.0f, this.startCooldownSeconds);
    }
    else
    {
      Vector3 position = this.speedTracker.transform.position;
      float magnitude1 = (position - this.lastPosition).magnitude;
      if ((double) magnitude1 > 0.0)
      {
        this.pathSinceLastReversal += magnitude1;
        this.debugCurrentHalfCycleDistance = this.pathSinceLastReversal;
      }
      Vector3 worldVelocity = this.speedTracker.GetWorldVelocity();
      float magnitude2 = worldVelocity.magnitude;
      Vector3 to = (double) worldVelocity.sqrMagnitude > 9.9999999747524271E-07 ? worldVelocity.normalized : this.lastVelocityDir;
      bool flag1 = false;
      if (this.hasLastDir)
      {
        if ((double) Vector3.Angle(this.lastVelocityDir, to) >= (double) this.angleToleranceDeg && (double) magnitude2 >= (double) this.minSpeedForReversal)
        {
          float duration = Time.time - this.lastReversalTime;
          if ((double) duration > 0.00050000002374872565)
          {
            this.EnqueueHalfCycle(duration);
            this.lastAmplitudeMeters = this.pathSinceLastReversal;
            this.lastReversalTime = Time.time;
            this.pathSinceLastReversal = 0.0f;
            flag1 = true;
          }
        }
      }
      else
      {
        this.hasLastDir = true;
        this.lastVelocityDir = to;
        this.lastReversalTime = Time.time;
      }
      this.lastVelocityDir = to;
      this.lastPosition = position;
      float halfCycleDuration = this.GetAverageHalfCycleDuration();
      float b1 = Time.time - this.lastReversalTime;
      float num1 = Mathf.Max((double) halfCycleDuration > 9.9999997473787516E-06 ? halfCycleDuration : float.PositiveInfinity, b1);
      float num2 = (double) num1 < double.PositiveInfinity ? 0.5f / num1 : 0.0f;
      this.debugCurrentRateHz = num2;
      bool flag2 = (double) num2 >= (double) this.shakeRateThreshold;
      bool flag3 = (double) this.lastAmplitudeMeters >= (double) this.shakeAmplitudeThreshold;
      if (!this.isShaking)
      {
        if ((double) Time.time >= (double) this.nextAllowedShakeStartTime & flag2 & flag3)
        {
          this.isShaking = true;
          if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
            this._events.Activate.RaiseOthers((object) this.isShaking);
          this.ShakeStartLocal?.Invoke();
          this.ShakeStartShared?.Invoke();
        }
      }
      else
      {
        bool flag4 = (double) Time.time - (double) this.lastReversalTime > (double) (1f * ((double) this.shakeRateThreshold > 9.9999997473787516E-06 ? 0.5f / this.shakeRateThreshold : float.PositiveInfinity));
        if (((flag2 ? 0 : (!flag1 ? 1 : 0)) | (flag4 ? 1 : 0)) != 0)
        {
          this.isShaking = false;
          if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
            this._events.Activate.RaiseOthers((object) this.isShaking);
          this.ShakeEndLocal?.Invoke();
          this.ShakeEndShared?.Invoke();
          this.nextAllowedShakeStartTime = Time.time + Mathf.Max(0.0f, this.startCooldownSeconds);
        }
      }
      if (this.useMaxes && this.isShaking && (double) num2 >= (double) this.maxShakeRate | (double) this.lastAmplitudeMeters >= (double) this.maxShakeAmplitude)
        this.MaxShake?.Invoke();
      float strength01 = 0.0f;
      if (this.isShaking)
      {
        float a = Mathf.Max(1E-05f, this.shakeAmplitudeThreshold);
        if (this.useMaxes && (double) this.maxShakeAmplitude > (double) a)
        {
          strength01 = Mathf.InverseLerp(a, this.maxShakeAmplitude, this.lastAmplitudeMeters);
        }
        else
        {
          float b2 = Mathf.Max(a, this.shakeAmplitudeThreshold * Mathf.Max(1f, this.softMaxMultiplier));
          strength01 = Mathf.InverseLerp(a, b2, this.lastAmplitudeMeters);
        }
      }
      this.ApplyStrength(strength01);
    }
  }

  private void EnqueueHalfCycle(float duration)
  {
    this.recentHalfCycleDurations.Enqueue(duration);
    while (this.recentHalfCycleDurations.Count > Mathf.Max(1, 1))
    {
      double num = (double) this.recentHalfCycleDurations.Dequeue();
    }
  }

  private float GetAverageHalfCycleDuration()
  {
    if (this.recentHalfCycleDurations.Count == 0)
      return 0.0f;
    float num = 0.0f;
    foreach (float halfCycleDuration in this.recentHalfCycleDurations)
      num += halfCycleDuration;
    return num / (float) this.recentHalfCycleDurations.Count;
  }

  private void ApplyStrength(float strength01)
  {
    if (this.continuousProperties == null)
      return;
    this.continuousProperties.ApplyAll(strength01);
  }

  private void OnShake(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
  {
    if (sender != target || info.senderID != this.myRig.creator.ActorNumber)
      return;
    GorillaNot.IncrementRPCCall(info, nameof (OnShake));
    if (!this.callLimiter.CheckCallTime(Time.time) || args.Length != 1 || !(args[0] is bool flag))
      return;
    if (flag)
      this.ShakeStartShared?.Invoke();
    else
      this.ShakeEndShared?.Invoke();
  }

  public bool IsSpawned { get; set; }

  public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

  public void OnSpawn(VRRig rig) => this.myRig = rig;

  public void OnDespawn()
  {
  }
}
