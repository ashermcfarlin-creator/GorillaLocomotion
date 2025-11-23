// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.EvolvingCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class EvolvingCosmetic : MonoBehaviour, ITickSystemTick
{
  [SerializeField]
  private bool enableLooping;
  [SerializeField]
  private int loopToStageOnComplete = 1;
  [SerializeField]
  private EvolvingCosmetic.EvolutionStage[] stages;
  private RubberDuckEvents networkEvents;
  private VRRig myRig;
  private CallLimiter callLimiter = new CallLimiter(5, 10f);
  private int activeStageIndex;
  private EvolvingCosmetic.EvolutionStage activeStage;
  private int nextEventIndex;
  private EvolvingCosmetic.EvolutionStage.EventAtTime nextEvent;
  private float totalElapsedTime;
  private float totalTimeOfPreviousStages;
  private float totalDuration;
  private float timeAtLoopStart;
  private float loopDuration;
  private Coroutine sendProgressDelayCoroutine;

  private int LoopMaxValue => this.stages.Length;

  private void Awake()
  {
    this.gameObject.GetOrAddComponent<RubberDuckEvents>(ref this.networkEvents);
    this.myRig = this.GetComponentInParent<VRRig>();
    for (int index = 0; index < this.stages.Length; ++index)
    {
      this.totalDuration += this.stages[index].Duration;
      if (this.enableLooping)
      {
        if (index < this.loopToStageOnComplete - 1)
          this.timeAtLoopStart += this.stages[index].Duration;
        else
          this.loopDuration += this.stages[index].Duration;
      }
    }
  }

  private void OnEnable()
  {
    if (this.stages.Length == 0)
      return;
    NetPlayer player = this.myRig.creator ?? NetworkSystem.Instance.LocalPlayer;
    if (player != null)
    {
      this.networkEvents.Init(player);
      TickSystem<object>.AddTickCallback((ITickSystemTick) this);
      NetworkSystem instance = NetworkSystem.Instance;
      instance.OnPlayerJoined = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>) instance.OnPlayerJoined + new Action<NetPlayer>(this.SendElapsedTime);
      this.networkEvents.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ReceiveElapsedTime);
      this.FirstStage();
    }
    else
      Debug.LogError((object) "Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
  }

  private void OnDisable()
  {
    if ((UnityEngine.Object) this.networkEvents != (UnityEngine.Object) null)
    {
      TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
      NetworkSystem instance = NetworkSystem.Instance;
      instance.OnPlayerJoined = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>) instance.OnPlayerJoined - new Action<NetPlayer>(this.SendElapsedTime);
      this.networkEvents.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.ReceiveElapsedTime);
      this.FirstStage();
    }
    this.callLimiter?.Reset();
  }

  private void Log(bool isComplete, bool isEvent)
  {
  }

  private void FirstStage()
  {
    this.activeStageIndex = 0;
    this.activeStage = this.stages[0];
    this.nextEventIndex = 0;
    this.nextEvent = this.activeStage.GetEventOrNull(0);
    this.totalElapsedTime = 0.0f;
    this.totalTimeOfPreviousStages = 0.0f;
    this.HandleStages();
  }

  private void HandleStages()
  {
    while (true)
    {
      float num = this.totalElapsedTime - this.totalTimeOfPreviousStages;
      this.activeStage.continuousProperties.ApplyAll(Mathf.Min(num / this.activeStage.Duration, 1f));
      for (; this.nextEvent != null && (double) num >= (double) this.nextEvent.absoluteTime; this.nextEvent = this.activeStage.GetEventOrNull(++this.nextEventIndex))
      {
        this.nextEvent.onTimeReached?.Invoke();
        this.Log(false, true);
      }
      if ((double) num >= (double) this.activeStage.Duration)
      {
        ++this.activeStageIndex;
        if (this.activeStageIndex < this.stages.Length || this.enableLooping)
        {
          if (this.activeStageIndex >= this.stages.Length)
          {
            this.activeStageIndex = this.loopToStageOnComplete - 1;
            this.totalTimeOfPreviousStages = this.timeAtLoopStart;
            this.totalElapsedTime -= this.loopDuration;
          }
          else
            this.totalTimeOfPreviousStages += this.activeStage.Duration;
          this.activeStage = this.stages[this.activeStageIndex];
          this.nextEventIndex = 0;
          this.nextEvent = this.activeStage.GetEventOrNull(0);
          if (!this.activeStage.HasDuration)
          {
            this.totalElapsedTime = this.totalTimeOfPreviousStages + this.activeStage.Duration * 0.5f;
            TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
          }
          else
            TickSystem<object>.AddTickCallback((ITickSystemTick) this);
          this.Log(false, false);
        }
        else
          goto label_7;
      }
      else
        break;
    }
    return;
label_7:
    this.totalElapsedTime = this.totalDuration;
    TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
    this.Log(true, false);
  }

  public bool TickRunning { get; set; }

  public void Tick()
  {
    this.totalElapsedTime = Mathf.Clamp(this.totalElapsedTime + Mathf.Max(this.activeStage.DeltaTime(Time.deltaTime), 0.0f), 0.0f, this.totalDuration * 1.01f);
    this.HandleStages();
  }

  public void CompleteManualStage()
  {
    if (this.activeStage.HasDuration)
      return;
    this.ForceNextStage();
  }

  public void ForceNextStage()
  {
    this.totalElapsedTime = this.totalTimeOfPreviousStages + this.activeStage.Duration;
    this.HandleStages();
  }

  private void SendElapsedTime(NetPlayer player)
  {
    if (this.sendProgressDelayCoroutine != null)
      this.StopCoroutine(this.sendProgressDelayCoroutine);
    this.sendProgressDelayCoroutine = this.StartCoroutine(this.SendElapsedTimeDelayed());
  }

  private IEnumerator SendElapsedTimeDelayed()
  {
    yield return (object) new WaitForSeconds(1f);
    this.sendProgressDelayCoroutine = (Coroutine) null;
    this.networkEvents.Activate.RaiseOthers((object) this.totalElapsedTime);
  }

  private void ReceiveElapsedTime(
    int sender,
    int target,
    object[] args,
    PhotonMessageInfoWrapped info)
  {
    if (sender != target)
      return;
    GorillaNot.IncrementRPCCall(info, nameof (ReceiveElapsedTime));
    if (info.senderID != this.myRig.creator.ActorNumber || !this.callLimiter.CheckCallServerTime((double) Time.unscaledTime) || args.Length != 1 || !(args[0] is float num) || !float.IsFinite(num) || (double) num > (double) this.totalDuration || (double) num < 0.0)
      return;
    this.totalElapsedTime = num;
    this.HandleStages();
  }

  [Serializable]
  private class EvolutionStage
  {
    private const float MIN_STAGE_TIME = 0.01f;
    public string debugName;
    public EvolvingCosmetic.EvolutionStage.ProgressionFlags progressionFlags = EvolvingCosmetic.EvolutionStage.ProgressionFlags.Time;
    [SerializeField]
    private float durationSeconds = float.NaN;
    public ThermalReceiver thermalReceiver;
    public AnimationCurve celsiusSpeedupMult = AnimationCurve.Linear(0.0f, 0.0f, 100f, 2f);
    public ContinuousPropertyArray continuousProperties;
    [SerializeField]
    private EvolvingCosmetic.EvolutionStage.EventAtTime[] events;

    private bool HasAnyFlag(
      EvolvingCosmetic.EvolutionStage.ProgressionFlags flag)
    {
      return (this.progressionFlags & flag) != 0;
    }

    public bool HasDuration
    {
      get
      {
        return this.HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags.Time | EvolvingCosmetic.EvolutionStage.ProgressionFlags.Temperature);
      }
    }

    public bool HasTime => this.HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags.Time);

    public bool HasTemperature
    {
      get => this.HasAnyFlag(EvolvingCosmetic.EvolutionStage.ProgressionFlags.Temperature);
    }

    public float Duration => !this.HasDuration ? 1f : this.durationSeconds;

    public float DeltaTime(float deltaTime)
    {
      return (float) ((this.HasTime ? (double) deltaTime : 0.0) + (this.HasTemperature ? (double) deltaTime * (double) this.celsiusSpeedupMult.Evaluate(this.thermalReceiver.celsius) : 0.0));
    }

    public EvolvingCosmetic.EvolutionStage.EventAtTime GetEventOrNull(int index)
    {
      return this.events == null || index < 0 || index >= this.events.Length ? (EvolvingCosmetic.EvolutionStage.EventAtTime) null : this.events[index];
    }

    [Flags]
    public enum ProgressionFlags
    {
      None = 0,
      Time = 1,
      Temperature = 2,
    }

    [Serializable]
    public class EventAtTime : IComparable<EvolvingCosmetic.EvolutionStage.EventAtTime>
    {
      public string debugName;
      public float time;
      public EvolvingCosmetic.EvolutionStage.EventAtTime.Type type;
      public float absoluteTime;
      public UnityEvent onTimeReached;

      private string DynamicTimeLabel
      {
        get
        {
          return this.type != EvolvingCosmetic.EvolutionStage.EventAtTime.Type.DurationFraction ? "Time" : "Fraction";
        }
      }

      public int CompareTo(EvolvingCosmetic.EvolutionStage.EventAtTime other)
      {
        return this.absoluteTime.CompareTo(other.absoluteTime);
      }

      public enum Type
      {
        SecondsFromBeginning,
        SecondsBeforeEnd,
        DurationFraction,
      }
    }
  }
}
