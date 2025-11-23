// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ContinuousPropertyTimeline
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTag.CosmeticSystem;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class ContinuousPropertyTimeline : MonoBehaviour, ITickSystemTick, ISpawnable
{
  [SerializeField]
  private float durationSeconds = 1f;
  [SerializeField]
  private float backwardDuration = 1f;
  [Tooltip("If true, the the timeline can move at a different speed when playing backwards.")]
  [SerializeField]
  private bool separateBackwardDuration;
  [Tooltip("When this object is enabled for the first time, should it immediately start playing from the beginning?")]
  [SerializeField]
  private bool startPlaying;
  [Tooltip("Determine what happens when the timeline reaches the end (or beginning while playing backwards).")]
  [SerializeField]
  private ContinuousPropertyTimeline.TimelineEndBehavior endBehavior;
  [SerializeField]
  private ContinuousPropertyArray continuousProperties;
  [SerializeField]
  private FlagEvents<ContinuousPropertyTimeline.TimelineEvent> events;
  private float time;
  private float inverseDuration;
  private float backwardDeltaMult;
  private bool IsForward = true;
  private bool IsPlaying;
  private VRRig myRig;

  private bool IsBackward
  {
    get => !this.IsForward;
    set => this.IsForward = !value;
  }

  private bool IsPaused
  {
    get => !this.IsPlaying;
    set => this.IsPlaying = !value;
  }

  public void TimelinePlay()
  {
    this.IsPlaying = true;
    TickSystem<object>.AddTickCallback((ITickSystemTick) this);
  }

  public void TimelinePause()
  {
    this.IsPaused = true;
    TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
  }

  public void TimelineToggleDirection() => this.IsForward = !this.IsForward;

  public void TimelineTogglePlay()
  {
    if (this.IsPlaying)
      this.TimelinePause();
    else
      this.TimelinePlay();
  }

  public void TimelinePlayForward()
  {
    this.IsForward = true;
    this.TimelinePlay();
  }

  public void TimelinePlayBackward()
  {
    this.IsBackward = true;
    this.TimelinePlay();
  }

  public void TimelinePlayFromBeginning()
  {
    this.time = 0.0f;
    this.TimelinePlayForward();
    this.OnReachedBeginning();
  }

  public void TimelinePlayFromEnd()
  {
    this.time = this.durationSeconds;
    this.TimelinePlayBackward();
    this.OnReachedEnd();
  }

  public void TimelineScrubToTime(float t)
  {
    if ((double) t <= 0.0)
    {
      this.time = 0.0f;
      this.OnReachedBeginning();
    }
    else if ((double) t >= (double) this.durationSeconds)
    {
      this.time = this.durationSeconds;
      this.OnReachedEnd();
    }
    else
      this.time = t;
  }

  public void TimelineScrubToFraction(float f)
  {
    this.TimelineScrubToTime(f * this.durationSeconds);
  }

  public void TimelineSetDuration(float d)
  {
    this.durationSeconds = d;
    this.inverseDuration = 1f / this.durationSeconds;
    this.backwardDeltaMult = this.durationSeconds / this.backwardDuration;
  }

  public void TimelineSetBackwardDuration(float d)
  {
    this.separateBackwardDuration = true;
    this.backwardDuration = d;
    this.backwardDeltaMult = this.durationSeconds / this.backwardDuration;
  }

  private void Awake() => this.IsPlaying = this.startPlaying;

  private void OnEnable()
  {
    if ((UnityEngine.Object) this.myRig == (UnityEngine.Object) null)
      this.myRig = this.GetComponentInParent<VRRig>();
    this.inverseDuration = 1f / this.durationSeconds;
    this.backwardDeltaMult = this.durationSeconds / this.backwardDuration;
    this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnEnable, (UnityEngine.Object) this.myRig != (UnityEngine.Object) null && this.myRig.isLocal);
    if (!this.IsPlaying)
      return;
    TickSystem<object>.AddTickCallback((ITickSystemTick) this);
  }

  private void OnDisable()
  {
    this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnDisable, (UnityEngine.Object) this.myRig != (UnityEngine.Object) null && this.myRig.isLocal);
    TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
  }

  private void OnReachedEnd()
  {
    if (this.IsForward)
    {
      switch (this.endBehavior)
      {
        case ContinuousPropertyTimeline.TimelineEndBehavior.Stop:
          this.TimelinePause();
          this.time = this.durationSeconds;
          break;
        case ContinuousPropertyTimeline.TimelineEndBehavior.Loop:
          this.TimelinePlayFromBeginning();
          break;
        case ContinuousPropertyTimeline.TimelineEndBehavior.PingPong:
          this.IsBackward = true;
          this.time = this.durationSeconds;
          break;
      }
    }
    this.continuousProperties.cachedRigIsLocal = (UnityEngine.Object) this.myRig != (UnityEngine.Object) null && this.myRig.isLocal;
    this.continuousProperties.ApplyAll(1f);
    this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnReachedEnd, (UnityEngine.Object) this.myRig != (UnityEngine.Object) null && this.myRig.isLocal);
  }

  private void OnReachedBeginning()
  {
    if (this.IsBackward)
    {
      switch (this.endBehavior)
      {
        case ContinuousPropertyTimeline.TimelineEndBehavior.Stop:
          this.TimelinePause();
          this.time = 0.0f;
          break;
        case ContinuousPropertyTimeline.TimelineEndBehavior.Loop:
          this.TimelinePlayFromEnd();
          break;
        case ContinuousPropertyTimeline.TimelineEndBehavior.PingPong:
          this.IsForward = true;
          this.time = 0.0f;
          break;
      }
    }
    this.continuousProperties.cachedRigIsLocal = (UnityEngine.Object) this.myRig != (UnityEngine.Object) null && this.myRig.isLocal;
    this.continuousProperties.ApplyAll(0.0f);
    this.events.InvokeAll(ContinuousPropertyTimeline.TimelineEvent.OnReachedBeginning, (UnityEngine.Object) this.myRig != (UnityEngine.Object) null && this.myRig.isLocal);
  }

  private void InBetween()
  {
    float f = this.time * this.inverseDuration;
    this.continuousProperties.cachedRigIsLocal = (UnityEngine.Object) this.myRig != (UnityEngine.Object) null && this.myRig.isLocal;
    this.continuousProperties.ApplyAll(f);
  }

  public bool TickRunning { get; set; }

  public void Tick()
  {
    if (this.IsForward)
    {
      this.time += Time.deltaTime;
      if ((double) this.time >= (double) this.durationSeconds)
        this.OnReachedEnd();
      else
        this.InBetween();
    }
    else
    {
      this.time -= Time.deltaTime * this.backwardDeltaMult;
      if ((double) this.time <= 0.0)
        this.OnReachedBeginning();
      else
        this.InBetween();
    }
  }

  public bool IsSpawned { get; set; }

  public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

  public void OnSpawn(VRRig rig) => this.myRig = rig;

  public void OnDespawn()
  {
  }

  private enum TimelineEndBehavior
  {
    Stop,
    Loop,
    PingPong,
  }

  [Flags]
  private enum TimelineEvent
  {
    OnReachedEnd = 1,
    OnReachedBeginning = 2,
    OnEnable = 4,
    OnDisable = 8,
  }
}
