// Decompiled with JetBrains decompiler
// Type: GorillaTag.ScienceExperimentManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using CjLib;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;

#nullable disable
namespace GorillaTag;

[NetworkBehaviourWeaved(76)]
public class ScienceExperimentManager : NetworkComponent, ITickSystemTick
{
  public static volatile ScienceExperimentManager instance;
  [SerializeField]
  private ScienceExperimentManager.TagBehavior tagBehavior = ScienceExperimentManager.TagBehavior.Infect;
  [SerializeField]
  private float minScale = 1f;
  [SerializeField]
  private float maxScale = 10f;
  [SerializeField]
  private float riseTimeFast = 30f;
  [SerializeField]
  private float riseTimeMedium = 60f;
  [SerializeField]
  private float riseTimeSlow = 120f;
  [SerializeField]
  private float riseTimeExtraSlow = 240f;
  [SerializeField]
  private float preDrainWaitTime = 3f;
  [SerializeField]
  private float maxFullTime = 5f;
  [SerializeField]
  private float drainTime = 10f;
  [SerializeField]
  private float fullyDrainedWaitTime = 3f;
  [SerializeField]
  private float lagResolutionLavaProgressPerSecond = 0.2f;
  [SerializeField]
  private AnimationCurve animationCurve = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  private float lavaProgressToDisableRefreshWater = 0.18f;
  [SerializeField]
  private float lavaProgressToEnableRefreshWater = 0.08f;
  [SerializeField]
  private float entryLiquidMaxScale = 5f;
  [SerializeField]
  private Vector2 entryLiquidScaleSyncOpeningTop = Vector2.zero;
  [SerializeField]
  private Vector2 entryLiquidScaleSyncOpeningBottom = Vector2.zero;
  [SerializeField]
  private float entryBridgeQuadMaxScaleY = 0.0915f;
  [SerializeField]
  private Vector2 entryBridgeQuadMinMaxZHeight = new Vector2(0.245f, 0.337f);
  [SerializeField]
  private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  public GameObject waterBalloonPrefab;
  [SerializeField]
  private Vector2 rotatingRingRandomAngleRange = Vector2.zero;
  [SerializeField]
  private bool rotatingRingQuantizeAngles;
  [SerializeField]
  private float rotatingRingAngleSnapDegrees = 9f;
  [SerializeField]
  private float drainBlockerSlideTime = 4f;
  [SerializeField]
  private Vector2 sodaFizzParticleEmissionMinMax = new Vector2(30f, 100f);
  [SerializeField]
  private float infrequentUpdatePeriod = 3f;
  [SerializeField]
  private bool optPlayersOutOfRoomGameMode;
  [SerializeField]
  private bool debugDrawPlayerGameState;
  private ScienceExperimentSceneElements elements;
  private NetPlayer[] allPlayersInRoom;
  private ScienceExperimentManager.RotatingRingState[] rotatingRings = new ScienceExperimentManager.RotatingRingState[0];
  private const int maxPlayerCount = 10;
  private ScienceExperimentManager.PlayerGameState[] inGamePlayerStates = new ScienceExperimentManager.PlayerGameState[10];
  private int inGamePlayerCount;
  private int lastWinnerId = -1;
  private string lastWinnerName = "None";
  private List<ScienceExperimentManager.PlayerGameState> sortedPlayerStates = new List<ScienceExperimentManager.PlayerGameState>();
  private ScienceExperimentManager.SyncData reliableState;
  private ScienceExperimentManager.RiseSpeed nextRoundRiseSpeed = ScienceExperimentManager.RiseSpeed.Slow;
  private float riseTime = 120f;
  private float riseProgress;
  private float riseProgressLinear;
  private float localLagRiseProgressOffset;
  private double lastInfrequentUpdateTime = -10.0;
  private string mentoProjectileTag = "ScienceCandyProjectile";
  private double currentTime;
  private double prevTime;
  private float ringRotationProgress = 1f;
  private float drainBlockerSlideSpeed;
  private float[] riseTimeLookup;
  [Header("Scene References")]
  public Transform ringParent;
  public Transform liquidMeshTransform;
  public Transform liquidSurfacePlane;
  public Transform entryWayLiquidMeshTransform;
  public Transform entryWayBridgeQuadTransform;
  public Transform drainBlocker;
  public Transform drainBlockerClosedPosition;
  public Transform drainBlockerOpenPosition;
  public WaterVolume liquidVolume;
  public WaterVolume entryLiquidVolume;
  public WaterVolume bottleLiquidVolume;
  public WaterVolume refreshWaterVolume;
  public CompositeTriggerEvents gameAreaTriggerNotifier;
  public SlingshotProjectileHitNotifier sodaWaterProjectileTriggerNotifier;
  public AudioSource eruptionAudioSource;
  public AudioSource drainAudioSource;
  public AudioSource rotatingRingsAudioSource;
  private ParticleSystem.EmissionModule fizzParticleEmission;
  private bool hasPlayedEruptionEffects;
  private bool hasPlayedDrainEffects;
  [SerializeField]
  private float debugRotateRingsTime = 10f;
  private Coroutine rotateRingsCoroutine;
  private bool debugRandomizingRings;
  [WeaverGenerated]
  [DefaultForProperty("Data", 0, 76)]
  [DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
  private ScienceExperimentManager.ScienceManagerData _Data;

  private bool RefreshWaterAvailable
  {
    get
    {
      if (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Drained || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Erupting || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Rising && (double) this.riseProgress < (double) this.lavaProgressToDisableRefreshWater)
        return true;
      return this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Draining && (double) this.riseProgress < (double) this.lavaProgressToEnableRefreshWater;
    }
  }

  public ScienceExperimentManager.RisingLiquidState GameState => this.reliableState.state;

  public float RiseProgress => this.riseProgress;

  public float RiseProgressLinear => this.riseProgressLinear;

  private int PlayerCount
  {
    get
    {
      int playerCount = 1;
      GorillaGameManager instance = GorillaGameManager.instance;
      if ((UnityEngine.Object) instance != (UnityEngine.Object) null && instance.currentNetPlayerArray != null)
        playerCount = instance.currentNetPlayerArray.Length;
      return playerCount;
    }
  }

  protected override void Awake()
  {
    base.Awake();
    if ((UnityEngine.Object) ScienceExperimentManager.instance == (UnityEngine.Object) null)
    {
      ScienceExperimentManager.instance = this;
      NetworkSystem.Instance.RegisterSceneNetworkItem(this.gameObject);
      this.riseTimeLookup = new float[4]
      {
        this.riseTimeFast,
        this.riseTimeMedium,
        this.riseTimeSlow,
        this.riseTimeExtraSlow
      };
      this.riseTime = this.riseTimeLookup[(int) this.nextRoundRiseSpeed];
      this.allPlayersInRoom = RoomSystem.PlayersInRoom.ToArray();
      GorillaGameManager.OnTouch += new GorillaGameManager.OnTouchDelegate(this.OnPlayerTagged);
      RoomSystem.PlayerLeftEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>) RoomSystem.PlayerLeftEvent + new Action<NetPlayer>(this.OnPlayerLeftRoom);
      RoomSystem.LeftRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>) RoomSystem.LeftRoomEvent + new Action(this.OnLeftRoom);
      this.rotatingRings = new ScienceExperimentManager.RotatingRingState[this.ringParent.childCount];
      for (int index = 0; index < this.rotatingRings.Length; ++index)
      {
        this.rotatingRings[index].ringTransform = this.ringParent.GetChild(index);
        this.rotatingRings[index].initialAngle = 0.0f;
        this.rotatingRings[index].resultingAngle = 0.0f;
      }
      this.gameAreaTriggerNotifier.CompositeTriggerEnter += new CompositeTriggerEvents.TriggerEvent(this.OnColliderEnteredVolume);
      this.gameAreaTriggerNotifier.CompositeTriggerExit += new CompositeTriggerEvents.TriggerEvent(this.OnColliderExitedVolume);
      this.liquidVolume.ColliderEnteredWater += new WaterVolume.WaterVolumeEvent(this.OnColliderEnteredSoda);
      this.liquidVolume.ColliderExitedWater += new WaterVolume.WaterVolumeEvent(this.OnColliderExitedSoda);
      this.entryLiquidVolume.ColliderEnteredWater += new WaterVolume.WaterVolumeEvent(this.OnColliderEnteredSoda);
      this.entryLiquidVolume.ColliderExitedWater += new WaterVolume.WaterVolumeEvent(this.OnColliderExitedSoda);
      if ((UnityEngine.Object) this.bottleLiquidVolume != (UnityEngine.Object) null)
      {
        this.bottleLiquidVolume.ColliderEnteredWater += new WaterVolume.WaterVolumeEvent(this.OnColliderEnteredSoda);
        this.bottleLiquidVolume.ColliderExitedWater += new WaterVolume.WaterVolumeEvent(this.OnColliderExitedSoda);
      }
      if ((UnityEngine.Object) this.refreshWaterVolume != (UnityEngine.Object) null)
      {
        this.refreshWaterVolume.ColliderEnteredWater += new WaterVolume.WaterVolumeEvent(this.OnColliderEnteredRefreshWater);
        this.refreshWaterVolume.ColliderExitedWater += new WaterVolume.WaterVolumeEvent(this.OnColliderExitedRefreshWater);
      }
      if ((UnityEngine.Object) this.sodaWaterProjectileTriggerNotifier != (UnityEngine.Object) null)
        this.sodaWaterProjectileTriggerNotifier.OnProjectileTriggerEnter += new SlingshotProjectileHitNotifier.ProjectileTriggerEvent(this.OnProjectileEnteredSodaWater);
      this.drainBlockerSlideSpeed = Vector3.Distance(this.drainBlockerClosedPosition.position, this.drainBlockerOpenPosition.position) / this.drainBlockerSlideTime;
    }
    else
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
  }

  internal override void OnEnable()
  {
    NetworkBehaviourUtils.InternalOnEnable((SimulationBehaviour) this);
    base.OnEnable();
    TickSystem<object>.AddTickCallback((ITickSystemTick) this);
  }

  internal override void OnDisable()
  {
    NetworkBehaviourUtils.InternalOnDisable((SimulationBehaviour) this);
    base.OnDisable();
    TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
  }

  private void OnDestroy()
  {
    NetworkBehaviourUtils.InternalOnDestroy((SimulationBehaviour) this);
    GorillaGameManager.OnTouch -= new GorillaGameManager.OnTouchDelegate(this.OnPlayerTagged);
    if ((UnityEngine.Object) this.gameAreaTriggerNotifier != (UnityEngine.Object) null)
    {
      this.gameAreaTriggerNotifier.CompositeTriggerEnter -= new CompositeTriggerEvents.TriggerEvent(this.OnColliderEnteredVolume);
      this.gameAreaTriggerNotifier.CompositeTriggerExit -= new CompositeTriggerEvents.TriggerEvent(this.OnColliderExitedVolume);
    }
    if ((UnityEngine.Object) this.liquidVolume != (UnityEngine.Object) null)
    {
      this.liquidVolume.ColliderEnteredWater -= new WaterVolume.WaterVolumeEvent(this.OnColliderEnteredSoda);
      this.liquidVolume.ColliderExitedWater -= new WaterVolume.WaterVolumeEvent(this.OnColliderExitedSoda);
    }
    if ((UnityEngine.Object) this.entryLiquidVolume != (UnityEngine.Object) null)
    {
      this.entryLiquidVolume.ColliderEnteredWater -= new WaterVolume.WaterVolumeEvent(this.OnColliderEnteredSoda);
      this.entryLiquidVolume.ColliderExitedWater -= new WaterVolume.WaterVolumeEvent(this.OnColliderExitedSoda);
    }
    if ((UnityEngine.Object) this.bottleLiquidVolume != (UnityEngine.Object) null)
    {
      this.bottleLiquidVolume.ColliderEnteredWater -= new WaterVolume.WaterVolumeEvent(this.OnColliderEnteredSoda);
      this.bottleLiquidVolume.ColliderExitedWater -= new WaterVolume.WaterVolumeEvent(this.OnColliderExitedSoda);
    }
    if ((UnityEngine.Object) this.refreshWaterVolume != (UnityEngine.Object) null)
    {
      this.refreshWaterVolume.ColliderEnteredWater -= new WaterVolume.WaterVolumeEvent(this.OnColliderEnteredRefreshWater);
      this.refreshWaterVolume.ColliderExitedWater -= new WaterVolume.WaterVolumeEvent(this.OnColliderExitedRefreshWater);
    }
    if (!((UnityEngine.Object) this.sodaWaterProjectileTriggerNotifier != (UnityEngine.Object) null))
      return;
    this.sodaWaterProjectileTriggerNotifier.OnProjectileTriggerEnter -= new SlingshotProjectileHitNotifier.ProjectileTriggerEvent(this.OnProjectileEnteredSodaWater);
  }

  public void InitElements(ScienceExperimentSceneElements elements)
  {
    this.elements = elements;
    this.fizzParticleEmission = elements.sodaFizzParticles.emission;
    elements.sodaFizzParticles.gameObject.SetActive(false);
    elements.sodaEruptionParticles.gameObject.SetActive(false);
    RoomSystem.LeftRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>) RoomSystem.LeftRoomEvent + new Action(this.OnLeftRoom);
  }

  public void DeInitElements() => this.elements = (ScienceExperimentSceneElements) null;

  public Transform GetElement(ScienceExperimentElementID elementID)
  {
    switch (elementID)
    {
      case ScienceExperimentElementID.Platform1:
        return this.rotatingRings[0].ringTransform;
      case ScienceExperimentElementID.Platform2:
        return this.rotatingRings[1].ringTransform;
      case ScienceExperimentElementID.Platform3:
        return this.rotatingRings[2].ringTransform;
      case ScienceExperimentElementID.Platform4:
        return this.rotatingRings[3].ringTransform;
      case ScienceExperimentElementID.Platform5:
        return this.rotatingRings[4].ringTransform;
      case ScienceExperimentElementID.LiquidMesh:
        return this.liquidMeshTransform;
      case ScienceExperimentElementID.EntryChamberLiquidMesh:
        return this.entryWayLiquidMeshTransform;
      case ScienceExperimentElementID.EntryChamberBridgeQuad:
        return this.entryWayBridgeQuadTransform;
      case ScienceExperimentElementID.DrainBlocker:
        return this.drainBlocker;
      default:
        Debug.LogError((object) $"Unhandled ScienceExperiment element ID! {elementID}");
        return (Transform) null;
    }
  }

  bool ITickSystemTick.TickRunning { get; set; }

  void ITickSystemTick.Tick()
  {
    this.prevTime = this.currentTime;
    this.currentTime = NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.unscaledTimeAsDouble;
    this.lastInfrequentUpdateTime = this.lastInfrequentUpdateTime > this.currentTime ? this.currentTime : this.lastInfrequentUpdateTime;
    if (this.currentTime > this.lastInfrequentUpdateTime + (double) this.infrequentUpdatePeriod)
    {
      this.InfrequentUpdate();
      this.lastInfrequentUpdateTime = this.currentTime;
    }
    if (this.IsMine)
      this.UpdateReliableState(this.currentTime, ref this.reliableState);
    this.UpdateLocalState(this.currentTime, this.reliableState);
    this.localLagRiseProgressOffset = Mathf.MoveTowards(this.localLagRiseProgressOffset, 0.0f, this.lagResolutionLavaProgressPerSecond * Time.deltaTime);
    this.UpdateLiquid(this.riseProgress + this.localLagRiseProgressOffset);
    this.UpdateRotatingRings(this.ringRotationProgress);
    this.UpdateRefreshWater();
    this.UpdateDrainBlocker(this.currentTime);
    this.DisableObjectsInContactWithLava(this.liquidMeshTransform.localScale.z);
    this.UpdateEffects();
    if (!this.debugDrawPlayerGameState)
      return;
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      NetPlayer targetPlayer = (NetPlayer) null;
      if (NetworkSystem.Instance.InRoom)
        targetPlayer = NetworkSystem.Instance.GetPlayer(this.inGamePlayerStates[index].playerId);
      else if (this.inGamePlayerStates[index].playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
        targetPlayer = NetworkSystem.Instance.LocalPlayer;
      RigContainer playerRig;
      if (targetPlayer != null && VRRigCache.Instance.TryGetVrrig(targetPlayer, out playerRig) && (UnityEngine.Object) playerRig.Rig != (UnityEngine.Object) null)
      {
        float num = 0.03f;
        DebugUtil.DrawSphere(playerRig.Rig.transform.position + Vector3.up * 0.5f * num, 0.16f * num, 12, 12, this.inGamePlayerStates[index].touchedLiquid ? Color.red : Color.green, style: DebugUtil.Style.SolidColor);
      }
    }
  }

  private void InfrequentUpdate()
  {
    this.allPlayersInRoom = RoomSystem.PlayersInRoom.ToArray();
    if (this.IsMine)
    {
      for (int index1 = this.inGamePlayerCount - 1; index1 >= 0; --index1)
      {
        int playerId = this.inGamePlayerStates[index1].playerId;
        bool flag = false;
        for (int index2 = 0; index2 < this.allPlayersInRoom.Length; ++index2)
        {
          if (this.allPlayersInRoom[index2].ActorNumber == playerId)
            flag = true;
        }
        if (!flag)
        {
          if (index1 < this.inGamePlayerCount - 1)
            this.inGamePlayerStates[index1] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
          this.inGamePlayerStates[this.inGamePlayerCount - 1] = new ScienceExperimentManager.PlayerGameState();
          --this.inGamePlayerCount;
        }
      }
    }
    if (!this.optPlayersOutOfRoomGameMode)
      return;
    for (int index3 = 0; index3 < this.allPlayersInRoom.Length; ++index3)
    {
      bool flag = false;
      for (int index4 = 0; index4 < this.inGamePlayerCount; ++index4)
      {
        if (this.allPlayersInRoom[index3].ActorNumber == this.inGamePlayerStates[index4].playerId)
          flag = true;
      }
      if (flag)
        GorillaGameModes.GameMode.OptOut(this.allPlayersInRoom[index3]);
      else
        GorillaGameModes.GameMode.OptIn(this.allPlayersInRoom[index3]);
    }
  }

  private bool PlayerInGame(Player player)
  {
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      if (this.inGamePlayerStates[index].playerId == player.ActorNumber)
        return true;
    }
    return false;
  }

  private void UpdateReliableState(
    double currentTime,
    ref ScienceExperimentManager.SyncData syncData)
  {
    if (currentTime < syncData.stateStartTime)
      syncData.stateStartTime = currentTime;
    switch (syncData.state)
    {
      case ScienceExperimentManager.RisingLiquidState.Erupting:
        if (currentTime <= syncData.stateStartTime + (double) this.fullyDrainedWaitTime)
          break;
        this.riseTime = this.riseTimeLookup[(int) this.nextRoundRiseSpeed];
        syncData.stateStartLiquidProgressLinear = 0.0f;
        syncData.state = ScienceExperimentManager.RisingLiquidState.Rising;
        syncData.stateStartTime = currentTime;
        break;
      case ScienceExperimentManager.RisingLiquidState.Rising:
        if (GetAlivePlayerCount() <= 0)
        {
          this.UpdateWinner();
          syncData.stateStartLiquidProgressLinear = Mathf.Clamp01((float) (currentTime - syncData.stateStartTime) / this.riseTime);
          syncData.state = ScienceExperimentManager.RisingLiquidState.PreDrainDelay;
          syncData.stateStartTime = currentTime;
          break;
        }
        if (currentTime <= syncData.stateStartTime + (double) this.riseTime)
          break;
        syncData.stateStartLiquidProgressLinear = 1f;
        syncData.state = ScienceExperimentManager.RisingLiquidState.Full;
        syncData.stateStartTime = currentTime;
        break;
      case ScienceExperimentManager.RisingLiquidState.Full:
        if (GetAlivePlayerCount() > 0 && currentTime <= syncData.stateStartTime + (double) this.maxFullTime)
          break;
        this.UpdateWinner();
        syncData.stateStartLiquidProgressLinear = 1f;
        syncData.state = ScienceExperimentManager.RisingLiquidState.PreDrainDelay;
        syncData.stateStartTime = currentTime;
        break;
      case ScienceExperimentManager.RisingLiquidState.PreDrainDelay:
        if (currentTime <= syncData.stateStartTime + (double) this.preDrainWaitTime)
          break;
        syncData.state = ScienceExperimentManager.RisingLiquidState.Draining;
        syncData.stateStartTime = currentTime;
        syncData.activationProgress = 0.0;
        for (int index = 0; index < this.rotatingRings.Length; ++index)
        {
          float num1 = Mathf.Repeat(this.rotatingRings[index].resultingAngle, 360f);
          float num2 = UnityEngine.Random.Range(this.rotatingRingRandomAngleRange.x, this.rotatingRingRandomAngleRange.y);
          float num3 = (double) UnityEngine.Random.Range(0.0f, 1f) > 0.5 ? 1f : -1f;
          this.rotatingRings[index].initialAngle = num1;
          this.rotatingRings[index].resultingAngle = num1 + num3 * num2;
        }
        break;
      case ScienceExperimentManager.RisingLiquidState.Draining:
        double num4 = (1.0 - (double) syncData.stateStartLiquidProgressLinear) * (double) this.drainTime;
        if (currentTime + num4 <= syncData.stateStartTime + (double) this.drainTime)
          break;
        syncData.stateStartLiquidProgressLinear = 0.0f;
        syncData.state = ScienceExperimentManager.RisingLiquidState.Drained;
        syncData.stateStartTime = currentTime;
        syncData.activationProgress = 0.0;
        break;
      default:
        if (GetAlivePlayerCount() > 0 && syncData.activationProgress > 1.0)
        {
          syncData.state = ScienceExperimentManager.RisingLiquidState.Erupting;
          syncData.stateStartTime = currentTime;
          syncData.stateStartLiquidProgressLinear = 0.0f;
          syncData.activationProgress = 1.0;
          break;
        }
        float num5 = Mathf.Clamp((float) (currentTime - this.prevTime), 0.0f, 0.1f);
        syncData.activationProgress = (double) Mathf.MoveTowards((float) syncData.activationProgress, 0.0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float) this.PlayerCount) * num5);
        break;
    }

    int GetAlivePlayerCount()
    {
      int alivePlayerCount = 0;
      for (int index = 0; index < this.inGamePlayerCount; ++index)
      {
        if (!this.inGamePlayerStates[index].touchedLiquid)
          ++alivePlayerCount;
      }
      return alivePlayerCount;
    }
  }

  private void UpdateLocalState(double currentTime, ScienceExperimentManager.SyncData syncData)
  {
    switch (syncData.state)
    {
      case ScienceExperimentManager.RisingLiquidState.Rising:
        this.riseProgressLinear = Mathf.Clamp01((float) (currentTime - syncData.stateStartTime) / this.riseTime);
        this.riseProgress = this.animationCurve.Evaluate(this.riseProgressLinear);
        this.ringRotationProgress = 1f;
        break;
      case ScienceExperimentManager.RisingLiquidState.Full:
        this.riseProgressLinear = 1f;
        this.riseProgress = 1f;
        this.ringRotationProgress = 1f;
        break;
      case ScienceExperimentManager.RisingLiquidState.PreDrainDelay:
        this.riseProgressLinear = syncData.stateStartLiquidProgressLinear;
        this.riseProgress = this.animationCurve.Evaluate(this.riseProgressLinear);
        this.ringRotationProgress = 1f;
        break;
      case ScienceExperimentManager.RisingLiquidState.Draining:
        double num = (1.0 - (double) syncData.stateStartLiquidProgressLinear) * (double) this.drainTime;
        this.riseProgressLinear = Mathf.Clamp01((float) (1.0 - (currentTime + num - syncData.stateStartTime) / (double) this.drainTime));
        this.riseProgress = this.animationCurve.Evaluate(this.riseProgressLinear);
        this.ringRotationProgress = (float) (currentTime - syncData.stateStartTime) / (this.drainTime * syncData.stateStartLiquidProgressLinear);
        break;
      default:
        this.riseProgressLinear = 0.0f;
        this.riseProgress = 0.0f;
        if (this.debugRandomizingRings)
          break;
        this.ringRotationProgress = 1f;
        break;
    }
  }

  private void UpdateLiquid(float fillProgress)
  {
    float z1 = Mathf.Lerp(this.minScale, this.maxScale, fillProgress);
    this.liquidMeshTransform.localScale = new Vector3(1f, 1f, z1);
    this.liquidMeshTransform.gameObject.SetActive(this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Rising || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Full || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.PreDrainDelay || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Draining);
    if (!((UnityEngine.Object) this.entryWayLiquidMeshTransform != (UnityEngine.Object) null))
      return;
    float y = 0.0f;
    float z2;
    float z3;
    if ((double) z1 < (double) this.entryLiquidScaleSyncOpeningBottom.y)
    {
      z2 = this.entryLiquidScaleSyncOpeningBottom.x;
      z3 = this.entryBridgeQuadMinMaxZHeight.x;
    }
    else if ((double) z1 < (double) this.entryLiquidScaleSyncOpeningTop.y)
    {
      float t = Mathf.InverseLerp(this.entryLiquidScaleSyncOpeningBottom.y, this.entryLiquidScaleSyncOpeningTop.y, z1);
      z2 = Mathf.Lerp(this.entryLiquidScaleSyncOpeningBottom.x, this.entryLiquidScaleSyncOpeningTop.x, t);
      z3 = Mathf.Lerp(this.entryBridgeQuadMinMaxZHeight.x, this.entryBridgeQuadMinMaxZHeight.y, t);
      y = this.entryBridgeQuadMaxScaleY * Mathf.Sin(t * 3.14159274f);
    }
    else
    {
      z2 = Mathf.Lerp(this.entryLiquidScaleSyncOpeningTop.x, this.entryLiquidMaxScale, Mathf.InverseLerp(this.entryLiquidScaleSyncOpeningTop.y, 0.6f * this.maxScale, z1));
      z3 = this.entryBridgeQuadMinMaxZHeight.y;
    }
    this.entryWayLiquidMeshTransform.localScale = new Vector3(this.entryWayLiquidMeshTransform.localScale.x, this.entryWayLiquidMeshTransform.localScale.y, z2);
    this.entryWayBridgeQuadTransform.localScale = new Vector3(this.entryWayBridgeQuadTransform.localScale.x, y, this.entryWayBridgeQuadTransform.localScale.z);
    this.entryWayBridgeQuadTransform.localPosition = new Vector3(this.entryWayBridgeQuadTransform.localPosition.x, this.entryWayBridgeQuadTransform.localPosition.y, z3);
  }

  private void UpdateRotatingRings(float rotationProgress)
  {
    for (int index = 0; index < this.rotatingRings.Length; ++index)
    {
      float angle = Mathf.Lerp(this.rotatingRings[index].initialAngle, this.rotatingRings[index].resultingAngle, rotationProgress);
      this.rotatingRings[index].ringTransform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
    }
  }

  private void UpdateDrainBlocker(double currentTime)
  {
    if (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Draining)
    {
      if ((double) this.drainTime - (currentTime - this.reliableState.stateStartTime + (double) ((1f - this.reliableState.stateStartLiquidProgressLinear) * this.drainTime)) < (double) this.drainBlockerSlideTime)
        this.drainBlocker.position = Vector3.MoveTowards(this.drainBlocker.position, this.drainBlockerClosedPosition.position, this.drainBlockerSlideSpeed * Time.deltaTime);
      else
        this.drainBlocker.position = Vector3.MoveTowards(this.drainBlocker.position, this.drainBlockerOpenPosition.position, this.drainBlockerSlideSpeed * Time.deltaTime);
    }
    else
      this.drainBlocker.position = this.drainBlockerClosedPosition.position;
  }

  private void UpdateEffects()
  {
    switch (this.reliableState.state)
    {
      case ScienceExperimentManager.RisingLiquidState.Drained:
        this.hasPlayedEruptionEffects = false;
        this.hasPlayedDrainEffects = false;
        this.eruptionAudioSource.GTStop();
        this.drainAudioSource.GTStop();
        this.rotatingRingsAudioSource.GTStop();
        if (!((UnityEngine.Object) this.elements != (UnityEngine.Object) null))
          break;
        this.elements.sodaEruptionParticles.gameObject.SetActive(false);
        this.elements.sodaFizzParticles.gameObject.SetActive(true);
        if (this.reliableState.activationProgress > 1.0 / 1000.0)
        {
          this.fizzParticleEmission.rateOverTimeMultiplier = Mathf.Lerp(this.sodaFizzParticleEmissionMinMax.x, this.sodaFizzParticleEmissionMinMax.y, (float) this.reliableState.activationProgress);
          break;
        }
        this.fizzParticleEmission.rateOverTimeMultiplier = 0.0f;
        break;
      case ScienceExperimentManager.RisingLiquidState.Erupting:
        if (this.hasPlayedEruptionEffects)
          break;
        this.eruptionAudioSource.loop = true;
        this.eruptionAudioSource.GTPlay();
        this.hasPlayedEruptionEffects = true;
        if (!((UnityEngine.Object) this.elements != (UnityEngine.Object) null))
          break;
        this.elements.sodaEruptionParticles.gameObject.SetActive(true);
        this.fizzParticleEmission.rateOverTimeMultiplier = this.sodaFizzParticleEmissionMinMax.y;
        break;
      case ScienceExperimentManager.RisingLiquidState.Rising:
        if (!((UnityEngine.Object) this.elements != (UnityEngine.Object) null))
          break;
        this.fizzParticleEmission.rateOverTimeMultiplier = 0.0f;
        break;
      case ScienceExperimentManager.RisingLiquidState.Draining:
        this.hasPlayedEruptionEffects = false;
        this.eruptionAudioSource.GTStop();
        if ((UnityEngine.Object) this.elements != (UnityEngine.Object) null)
        {
          this.elements.sodaFizzParticles.gameObject.SetActive(false);
          this.elements.sodaEruptionParticles.gameObject.SetActive(false);
          this.fizzParticleEmission.rateOverTimeMultiplier = 0.0f;
        }
        if (this.hasPlayedDrainEffects)
          break;
        this.drainAudioSource.loop = true;
        this.drainAudioSource.GTPlay();
        this.rotatingRingsAudioSource.loop = true;
        this.rotatingRingsAudioSource.GTPlay();
        this.hasPlayedDrainEffects = true;
        break;
      default:
        if ((UnityEngine.Object) this.elements != (UnityEngine.Object) null)
        {
          this.elements.sodaFizzParticles.gameObject.SetActive(false);
          this.elements.sodaEruptionParticles.gameObject.SetActive(false);
          this.fizzParticleEmission.rateOverTimeMultiplier = 0.0f;
        }
        this.hasPlayedEruptionEffects = false;
        this.hasPlayedDrainEffects = false;
        this.eruptionAudioSource.GTStop();
        this.drainAudioSource.GTStop();
        this.rotatingRingsAudioSource.GTStop();
        break;
    }
  }

  private void DisableObjectsInContactWithLava(float lavaScale)
  {
    if ((UnityEngine.Object) this.elements == (UnityEngine.Object) null)
      return;
    Plane plane = new Plane(this.liquidSurfacePlane.up, this.liquidSurfacePlane.position);
    if (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Rising)
    {
      for (int index = 0; index < this.elements.disableByLiquidList.Count; ++index)
      {
        if (!plane.GetSide(this.elements.disableByLiquidList[index].target.position + this.elements.disableByLiquidList[index].heightOffset * Vector3.up))
          this.elements.disableByLiquidList[index].target.gameObject.SetActive(false);
      }
    }
    else
    {
      if (this.reliableState.state != ScienceExperimentManager.RisingLiquidState.Draining)
        return;
      for (int index = 0; index < this.elements.disableByLiquidList.Count; ++index)
      {
        if (plane.GetSide(this.elements.disableByLiquidList[index].target.position + this.elements.disableByLiquidList[index].heightOffset * Vector3.up))
          this.elements.disableByLiquidList[index].target.gameObject.SetActive(true);
      }
    }
  }

  private void UpdateWinner()
  {
    float num = -1f;
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      if (!this.inGamePlayerStates[index].touchedLiquid)
      {
        this.lastWinnerId = this.inGamePlayerStates[index].playerId;
        break;
      }
      if ((double) this.inGamePlayerStates[index].touchedLiquidAtProgress > (double) num)
      {
        num = this.inGamePlayerStates[index].touchedLiquidAtProgress;
        this.lastWinnerId = this.inGamePlayerStates[index].playerId;
      }
    }
    this.RefreshWinnerName();
  }

  private void RefreshWinnerName()
  {
    NetPlayer playerFromId = this.GetPlayerFromId(this.lastWinnerId);
    if (playerFromId != null)
      this.lastWinnerName = playerFromId.NickName;
    else
      this.lastWinnerName = "None";
  }

  private NetPlayer GetPlayerFromId(int id)
  {
    if (NetworkSystem.Instance.InRoom)
      return NetworkSystem.Instance.GetPlayer(id);
    return id == NetworkSystem.Instance.LocalPlayer.ActorNumber ? NetworkSystem.Instance.LocalPlayer : (NetPlayer) null;
  }

  private void UpdateRefreshWater()
  {
    if (!((UnityEngine.Object) this.refreshWaterVolume != (UnityEngine.Object) null))
      return;
    if (this.RefreshWaterAvailable && !this.refreshWaterVolume.gameObject.activeSelf)
    {
      this.refreshWaterVolume.gameObject.SetActive(true);
    }
    else
    {
      if (this.RefreshWaterAvailable || !this.refreshWaterVolume.gameObject.activeSelf)
        return;
      this.refreshWaterVolume.gameObject.SetActive(false);
    }
  }

  private void ResetGame()
  {
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      ScienceExperimentManager.PlayerGameState inGamePlayerState = this.inGamePlayerStates[index] with
      {
        touchedLiquid = false,
        touchedLiquidAtProgress = -1f
      };
      this.inGamePlayerStates[index] = inGamePlayerState;
    }
  }

  public void RestartGame()
  {
    if (!this.IsMine)
      return;
    this.riseTime = this.riseTimeLookup[(int) this.nextRoundRiseSpeed];
    this.reliableState.state = ScienceExperimentManager.RisingLiquidState.Erupting;
    this.reliableState.stateStartTime = NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : (double) Time.time;
    this.reliableState.stateStartLiquidProgressLinear = 0.0f;
    this.reliableState.activationProgress = 1.0;
    this.ResetGame();
  }

  public void DebugErupt()
  {
    if (!this.IsMine)
      return;
    this.riseTime = this.riseTimeLookup[(int) this.nextRoundRiseSpeed];
    this.reliableState.state = ScienceExperimentManager.RisingLiquidState.Erupting;
    this.reliableState.stateStartTime = NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : (double) Time.time;
    this.reliableState.stateStartLiquidProgressLinear = 0.0f;
    this.reliableState.activationProgress = 1.0;
  }

  public void RandomizeRings()
  {
    for (int index = 0; index < this.rotatingRings.Length; ++index)
    {
      float num1 = Mathf.Repeat(this.rotatingRings[index].resultingAngle, 360f);
      float num2 = UnityEngine.Random.Range(this.rotatingRingRandomAngleRange.x, this.rotatingRingRandomAngleRange.y);
      float num3 = (double) UnityEngine.Random.Range(0.0f, 1f) > 0.5 ? 1f : -1f;
      this.rotatingRings[index].initialAngle = num1;
      float num4 = num1 + num3 * num2;
      if (this.rotatingRingQuantizeAngles)
        num4 = Mathf.Round(num4 / this.rotatingRingAngleSnapDegrees) * this.rotatingRingAngleSnapDegrees;
      this.rotatingRings[index].resultingAngle = num4;
    }
    if (this.rotateRingsCoroutine != null)
      this.StopCoroutine(this.rotateRingsCoroutine);
    this.rotateRingsCoroutine = this.StartCoroutine(this.RotateRingsCoroutine());
  }

  private IEnumerator RotateRingsCoroutine()
  {
    if ((double) this.debugRotateRingsTime > 0.0099999997764825821)
    {
      float routineStartTime = Time.time;
      this.ringRotationProgress = 0.0f;
      this.debugRandomizingRings = true;
      while ((double) this.ringRotationProgress < 1.0)
      {
        this.ringRotationProgress = (Time.time - routineStartTime) / this.debugRotateRingsTime;
        yield return (object) null;
      }
    }
    this.debugRandomizingRings = false;
    this.ringRotationProgress = 1f;
  }

  public bool GetMaterialIfPlayerInGame(int playerActorNumber, out int materialIndex)
  {
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      if (this.inGamePlayerStates[index].playerId == playerActorNumber)
      {
        if (this.inGamePlayerStates[index].touchedLiquid)
        {
          materialIndex = 12;
          return true;
        }
        materialIndex = 0;
        return true;
      }
    }
    materialIndex = 0;
    return false;
  }

  private void OnPlayerTagged(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
  {
    if (!this.IsMine)
      return;
    int index1 = -1;
    int index2 = -1;
    for (int index3 = 0; index3 < this.inGamePlayerCount; ++index3)
    {
      if (this.inGamePlayerStates[index3].playerId == taggedPlayer.ActorNumber)
        index1 = index3;
      else if (this.inGamePlayerStates[index3].playerId == taggingPlayer.ActorNumber)
        index2 = index3;
      if (index1 != -1 && index2 != -1)
        break;
    }
    if (index1 == -1 || index2 == -1)
      return;
    switch (this.tagBehavior)
    {
      case ScienceExperimentManager.TagBehavior.Infect:
        if (!this.inGamePlayerStates[index2].touchedLiquid || this.inGamePlayerStates[index1].touchedLiquid)
          break;
        ScienceExperimentManager.PlayerGameState inGamePlayerState1 = this.inGamePlayerStates[index1] with
        {
          touchedLiquid = true,
          touchedLiquidAtProgress = this.riseProgressLinear
        };
        this.inGamePlayerStates[index1] = inGamePlayerState1;
        break;
      case ScienceExperimentManager.TagBehavior.Revive:
        if (this.inGamePlayerStates[index2].touchedLiquid || !this.inGamePlayerStates[index1].touchedLiquid)
          break;
        ScienceExperimentManager.PlayerGameState inGamePlayerState2 = this.inGamePlayerStates[index1] with
        {
          touchedLiquid = false,
          touchedLiquidAtProgress = -1f
        };
        this.inGamePlayerStates[index1] = inGamePlayerState2;
        break;
    }
  }

  private void OnColliderEnteredVolume(Collider collider)
  {
    VRRig component = collider.attachedRigidbody.gameObject.GetComponent<VRRig>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || component.creator == null)
      return;
    this.PlayerEnteredGameArea(component.creator.ActorNumber);
  }

  private void OnColliderExitedVolume(Collider collider)
  {
    VRRig component = collider.attachedRigidbody.gameObject.GetComponent<VRRig>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || component.creator == null)
      return;
    this.PlayerExitedGameArea(component.creator.ActorNumber);
  }

  private void OnColliderEnteredSoda(WaterVolume volume, Collider collider)
  {
    if (!((UnityEngine.Object) collider == (UnityEngine.Object) GTPlayer.Instance.bodyCollider))
      return;
    if (this.IsMine)
      this.PlayerTouchedLava(NetworkSystem.Instance.LocalPlayer.ActorNumber);
    else
      this.GetView.RPC("PlayerTouchedLavaRPC", RpcTarget.MasterClient);
  }

  private void OnColliderExitedSoda(WaterVolume volume, Collider collider)
  {
  }

  private void OnColliderEnteredRefreshWater(WaterVolume volume, Collider collider)
  {
    if (!((UnityEngine.Object) collider == (UnityEngine.Object) GTPlayer.Instance.bodyCollider))
      return;
    if (this.IsMine)
      this.PlayerTouchedRefreshWater(NetworkSystem.Instance.LocalPlayer.ActorNumber);
    else
      this.GetView.RPC("PlayerTouchedRefreshWaterRPC", RpcTarget.MasterClient);
  }

  private void OnColliderExitedRefreshWater(WaterVolume volume, Collider collider)
  {
  }

  private void OnProjectileEnteredSodaWater(SlingshotProjectile projectile, Collider collider)
  {
    if (!projectile.gameObject.CompareTag(this.mentoProjectileTag))
      return;
    this.AddLavaRock(projectile.projectileOwner.ActorNumber);
  }

  private void AddLavaRock(int playerId)
  {
    if (!this.IsMine || this.reliableState.state != ScienceExperimentManager.RisingLiquidState.Drained)
      return;
    bool flag = false;
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      if (!this.inGamePlayerStates[index].touchedLiquid)
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      return;
    this.reliableState.activationProgress += (double) this.lavaActivationRockProgressVsPlayerCount.Evaluate((float) this.PlayerCount);
  }

  public void OnWaterBalloonHitPlayer(NetPlayer hitPlayer)
  {
    bool flag = false;
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      if (this.inGamePlayerStates[index].playerId == hitPlayer.ActorNumber)
        flag = true;
    }
    if (!flag)
      return;
    if (hitPlayer == NetworkSystem.Instance.LocalPlayer)
      this.ValidateLocalPlayerWaterBalloonHit(hitPlayer.ActorNumber);
    else
      this.GetView.RPC("ValidateLocalPlayerWaterBalloonHitRPC", RpcTarget.Others, (object) hitPlayer.ActorNumber);
  }

  [Networked]
  [NetworkedWeaved(0, 76)]
  private unsafe ScienceExperimentManager.ScienceManagerData Data
  {
    get
    {
      if ((IntPtr) this.Ptr == IntPtr.Zero)
        throw new InvalidOperationException("Error when accessing ScienceExperimentManager.Data. Networked properties can only be accessed when Spawned() has been called.");
      return *(ScienceExperimentManager.ScienceManagerData*) (this.Ptr + 0);
    }
    set
    {
      if ((IntPtr) this.Ptr == IntPtr.Zero)
        throw new InvalidOperationException("Error when accessing ScienceExperimentManager.Data. Networked properties can only be accessed when Spawned() has been called.");
      *(ScienceExperimentManager.ScienceManagerData*) (this.Ptr + 0) = value;
    }
  }

  public override void WriteDataFusion()
  {
    this.Data = new ScienceExperimentManager.ScienceManagerData((int) this.reliableState.state, this.reliableState.stateStartTime, this.reliableState.stateStartLiquidProgressLinear, this.reliableState.activationProgress, (int) this.nextRoundRiseSpeed, this.riseTime, this.lastWinnerId, this.inGamePlayerCount, this.inGamePlayerStates, this.rotatingRings);
  }

  public override void ReadDataFusion()
  {
    int lastWinnerId = this.lastWinnerId;
    int nextRoundRiseSpeed = (int) this.nextRoundRiseSpeed;
    this.reliableState.state = (ScienceExperimentManager.RisingLiquidState) this.Data.reliableState;
    this.reliableState.stateStartTime = this.Data.stateStartTime;
    this.reliableState.stateStartLiquidProgressLinear = this.Data.stateStartLiquidProgressLinear.ClampSafe(0.0f, 1f);
    this.reliableState.activationProgress = this.Data.activationProgress.GetFinite();
    this.nextRoundRiseSpeed = (ScienceExperimentManager.RiseSpeed) this.Data.nextRoundRiseSpeed;
    this.riseTime = this.Data.riseTime.GetFinite();
    this.lastWinnerId = this.Data.lastWinnerId;
    this.inGamePlayerCount = Mathf.Clamp(this.Data.inGamePlayerCount, 0, 10);
    for (int index = 0; index < 10; ++index)
    {
      this.inGamePlayerStates[index].playerId = this.Data.playerIdArray[index];
      this.inGamePlayerStates[index].touchedLiquid = this.Data.touchedLiquidArray[index];
      this.inGamePlayerStates[index].touchedLiquidAtProgress = this.Data.touchedLiquidAtProgressArray[index].ClampSafe(0.0f, 1f);
    }
    for (int index = 0; index < this.rotatingRings.Length; ++index)
    {
      this.rotatingRings[index].initialAngle = this.Data.initialAngleArray[index].GetFinite();
      this.rotatingRings[index].resultingAngle = this.Data.resultingAngleArray[index].GetFinite();
    }
    float riseProgress = this.riseProgress;
    this.UpdateLocalState(NetworkSystem.Instance.SimTime, this.reliableState);
    this.localLagRiseProgressOffset = riseProgress - this.riseProgress;
    if (lastWinnerId == this.lastWinnerId)
      return;
    this.RefreshWinnerName();
  }

  protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
  {
    stream.SendNext((object) (int) this.reliableState.state);
    stream.SendNext((object) this.reliableState.stateStartTime);
    stream.SendNext((object) this.reliableState.stateStartLiquidProgressLinear);
    stream.SendNext((object) this.reliableState.activationProgress);
    stream.SendNext((object) (int) this.nextRoundRiseSpeed);
    stream.SendNext((object) this.riseTime);
    stream.SendNext((object) this.lastWinnerId);
    stream.SendNext((object) this.inGamePlayerCount);
    for (int index = 0; index < 10; ++index)
    {
      stream.SendNext((object) this.inGamePlayerStates[index].playerId);
      stream.SendNext((object) this.inGamePlayerStates[index].touchedLiquid);
      stream.SendNext((object) this.inGamePlayerStates[index].touchedLiquidAtProgress);
    }
    for (int index = 0; index < this.rotatingRings.Length; ++index)
    {
      stream.SendNext((object) this.rotatingRings[index].initialAngle);
      stream.SendNext((object) this.rotatingRings[index].resultingAngle);
    }
  }

  protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
  {
    int lastWinnerId = this.lastWinnerId;
    int nextRoundRiseSpeed = (int) this.nextRoundRiseSpeed;
    this.reliableState.state = (ScienceExperimentManager.RisingLiquidState) stream.ReceiveNext();
    this.reliableState.stateStartTime = ((double) stream.ReceiveNext()).GetFinite();
    this.reliableState.stateStartLiquidProgressLinear = ((float) stream.ReceiveNext()).ClampSafe(0.0f, 1f);
    this.reliableState.activationProgress = ((double) stream.ReceiveNext()).GetFinite();
    this.nextRoundRiseSpeed = (ScienceExperimentManager.RiseSpeed) stream.ReceiveNext();
    this.riseTime = ((float) stream.ReceiveNext()).GetFinite();
    this.lastWinnerId = (int) stream.ReceiveNext();
    this.inGamePlayerCount = (int) stream.ReceiveNext();
    this.inGamePlayerCount = Mathf.Clamp(this.inGamePlayerCount, 0, 10);
    for (int index = 0; index < 10; ++index)
    {
      this.inGamePlayerStates[index].playerId = (int) stream.ReceiveNext();
      this.inGamePlayerStates[index].touchedLiquid = (bool) stream.ReceiveNext();
      this.inGamePlayerStates[index].touchedLiquidAtProgress = ((float) stream.ReceiveNext()).ClampSafe(0.0f, 1f);
    }
    for (int index = 0; index < this.rotatingRings.Length; ++index)
    {
      this.rotatingRings[index].initialAngle = ((float) stream.ReceiveNext()).GetFinite();
      this.rotatingRings[index].resultingAngle = ((float) stream.ReceiveNext()).GetFinite();
    }
    float riseProgress = this.riseProgress;
    this.UpdateLocalState(NetworkSystem.Instance.SimTime, this.reliableState);
    this.localLagRiseProgressOffset = riseProgress - this.riseProgress;
    if (lastWinnerId == this.lastWinnerId)
      return;
    this.RefreshWinnerName();
  }

  private void PlayerEnteredGameArea(int pId)
  {
    if (!this.IsMine)
      return;
    bool flag1 = false;
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      if (this.inGamePlayerStates[index].playerId == pId)
      {
        flag1 = true;
        break;
      }
    }
    if (flag1 || this.inGamePlayerCount >= 10)
      return;
    bool flag2 = false;
    this.inGamePlayerStates[this.inGamePlayerCount] = new ScienceExperimentManager.PlayerGameState()
    {
      playerId = pId,
      touchedLiquid = flag2,
      touchedLiquidAtProgress = -1f
    };
    ++this.inGamePlayerCount;
    if (!this.optPlayersOutOfRoomGameMode)
      return;
    GorillaGameModes.GameMode.OptOut(pId);
  }

  private void PlayerExitedGameArea(int playerId)
  {
    if (!this.IsMine)
      return;
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      if (this.inGamePlayerStates[index].playerId == playerId)
      {
        this.inGamePlayerStates[index] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
        --this.inGamePlayerCount;
        if (!this.optPlayersOutOfRoomGameMode)
          break;
        GorillaGameModes.GameMode.OptIn(playerId);
        break;
      }
    }
  }

  [PunRPC]
  public void PlayerTouchedLavaRPC(PhotonMessageInfo info)
  {
    GorillaNot.IncrementRPCCall(info, nameof (PlayerTouchedLavaRPC));
    this.PlayerTouchedLava(info.Sender.ActorNumber);
  }

  [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
  public unsafe void RPC_PlayerTouchedLava(RpcInfo info = default (RpcInfo))
  {
    if (this.InvokeRpc)
      this.InvokeRpc = false;
    else
      goto label_3;
label_2:
    PhotonMessageInfoWrapped infoWrapped = new PhotonMessageInfoWrapped(info);
    GorillaNot.IncrementRPCCall(infoWrapped, "PlayerTouchedLavaRPC");
    this.PlayerTouchedLava(infoWrapped.Sender.ActorNumber);
    return;
label_3:
    NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized((NetworkBehaviour) this);
    if (this.Runner.Stage == SimulationStages.Resimulate)
      return;
    int localAuthorityMask = this.Object.GetLocalAuthorityMask();
    if ((localAuthorityMask & 7) == 0)
    {
      NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedLava(Fusion.RpcInfo)", this.Object, 7);
    }
    else
    {
      if ((localAuthorityMask & 1) != 1)
      {
        int num1 = 8;
        if (!SimulationMessage.CanAllocateUserPayload(num1))
        {
          NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedLava(Fusion.RpcInfo)", num1);
          return;
        }
        if (this.Runner.HasAnyActiveConnections())
        {
          SimulationMessage* message = SimulationMessage.Allocate(this.Runner.Simulation, num1);
          *(RpcHeader*) ((IntPtr) message + 28) = RpcHeader.Create(this.Object.Id, this.ObjectIndex, 1);
          int num2 = 8;
          message->Offset = num2 * 8;
          this.Runner.SendRpc(message);
        }
        if ((localAuthorityMask & 1) == 0)
          return;
      }
      info = RpcInfo.FromLocal(this.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
      goto label_2;
    }
  }

  private void PlayerTouchedLava(int playerId)
  {
    if (!this.IsMine)
      return;
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      if (this.inGamePlayerStates[index].playerId == playerId)
      {
        ScienceExperimentManager.PlayerGameState inGamePlayerState = this.inGamePlayerStates[index];
        if (!inGamePlayerState.touchedLiquid)
          inGamePlayerState.touchedLiquidAtProgress = this.riseProgressLinear;
        inGamePlayerState.touchedLiquid = true;
        this.inGamePlayerStates[index] = inGamePlayerState;
        break;
      }
    }
  }

  [PunRPC]
  private void PlayerTouchedRefreshWaterRPC(PhotonMessageInfo info)
  {
    GorillaNot.IncrementRPCCall(info, nameof (PlayerTouchedRefreshWaterRPC));
    this.PlayerTouchedRefreshWater(info.Sender.ActorNumber);
  }

  [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
  private unsafe void RPC_PlayerTouchedRefreshWater(RpcInfo info = default (RpcInfo))
  {
    if (this.InvokeRpc)
      this.InvokeRpc = false;
    else
      goto label_3;
label_2:
    PhotonMessageInfoWrapped infoWrapped = new PhotonMessageInfoWrapped(info);
    GorillaNot.IncrementRPCCall(infoWrapped, "PlayerTouchedRefreshWaterRPC");
    this.PlayerTouchedRefreshWater(infoWrapped.Sender.ActorNumber);
    return;
label_3:
    NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized((NetworkBehaviour) this);
    if (this.Runner.Stage == SimulationStages.Resimulate)
      return;
    int localAuthorityMask = this.Object.GetLocalAuthorityMask();
    if ((localAuthorityMask & 7) == 0)
    {
      NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedRefreshWater(Fusion.RpcInfo)", this.Object, 7);
    }
    else
    {
      if ((localAuthorityMask & 1) != 1)
      {
        int num1 = 8;
        if (!SimulationMessage.CanAllocateUserPayload(num1))
        {
          NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedRefreshWater(Fusion.RpcInfo)", num1);
          return;
        }
        if (this.Runner.HasAnyActiveConnections())
        {
          SimulationMessage* message = SimulationMessage.Allocate(this.Runner.Simulation, num1);
          *(RpcHeader*) ((IntPtr) message + 28) = RpcHeader.Create(this.Object.Id, this.ObjectIndex, 2);
          int num2 = 8;
          message->Offset = num2 * 8;
          this.Runner.SendRpc(message);
        }
        if ((localAuthorityMask & 1) == 0)
          return;
      }
      info = RpcInfo.FromLocal(this.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
      goto label_2;
    }
  }

  private void PlayerTouchedRefreshWater(int playerId)
  {
    if (!this.IsMine || !this.RefreshWaterAvailable)
      return;
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      if (this.inGamePlayerStates[index].playerId == playerId)
      {
        ScienceExperimentManager.PlayerGameState inGamePlayerState = this.inGamePlayerStates[index] with
        {
          touchedLiquid = false,
          touchedLiquidAtProgress = -1f
        };
        this.inGamePlayerStates[index] = inGamePlayerState;
        break;
      }
    }
  }

  [PunRPC]
  private void ValidateLocalPlayerWaterBalloonHitRPC(int playerId, PhotonMessageInfo info)
  {
    GorillaNot.IncrementRPCCall(info, nameof (ValidateLocalPlayerWaterBalloonHitRPC));
    if (playerId != NetworkSystem.Instance.LocalPlayer.ActorNumber)
      return;
    this.ValidateLocalPlayerWaterBalloonHit(playerId);
  }

  [Rpc(InvokeLocal = false)]
  private unsafe void RPC_ValidateLocalPlayerWaterBalloonHit(int playerId, RpcInfo info = default (RpcInfo))
  {
    if (this.InvokeRpc)
    {
      this.InvokeRpc = false;
      GorillaNot.IncrementRPCCall(new PhotonMessageInfoWrapped(info), "ValidateLocalPlayerWaterBalloonHitRPC");
      if (playerId != NetworkSystem.Instance.LocalPlayer.ActorNumber)
        return;
      this.ValidateLocalPlayerWaterBalloonHit(playerId);
    }
    else
    {
      NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized((NetworkBehaviour) this);
      if (this.Runner.Stage == SimulationStages.Resimulate)
        return;
      if ((this.Object.GetLocalAuthorityMask() & 7) == 0)
      {
        NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_ValidateLocalPlayerWaterBalloonHit(System.Int32,Fusion.RpcInfo)", this.Object, 7);
      }
      else
      {
        int num1 = 8 + 4;
        if (!SimulationMessage.CanAllocateUserPayload(num1))
          NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.ScienceExperimentManager::RPC_ValidateLocalPlayerWaterBalloonHit(System.Int32,Fusion.RpcInfo)", num1);
        else if (this.Runner.HasAnyActiveConnections())
        {
          SimulationMessage* message = SimulationMessage.Allocate(this.Runner.Simulation, num1);
          byte* numPtr = (byte*) ((IntPtr) message + 28);
          *(RpcHeader*) numPtr = RpcHeader.Create(this.Object.Id, this.ObjectIndex, 3);
          int num2 = 8;
          *(int*) (numPtr + num2) = playerId;
          int num3 = num2 + 4;
          message->Offset = num3 * 8;
          this.Runner.SendRpc(message);
        }
      }
    }
  }

  private void ValidateLocalPlayerWaterBalloonHit(int playerId)
  {
    if (playerId != NetworkSystem.Instance.LocalPlayer.ActorNumber || GTPlayer.Instance.InWater)
      return;
    if (this.IsMine)
      this.PlayerHitByWaterBalloon(NetworkSystem.Instance.LocalPlayer.ActorNumber);
    else
      this.GetView.RPC("PlayerHitByWaterBalloonRPC", RpcTarget.MasterClient, (object) PhotonNetwork.LocalPlayer.ActorNumber);
  }

  [PunRPC]
  private void PlayerHitByWaterBalloonRPC(int playerId, PhotonMessageInfo info)
  {
    GorillaNot.IncrementRPCCall(info, nameof (PlayerHitByWaterBalloonRPC));
    this.PlayerHitByWaterBalloon(playerId);
  }

  [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
  private unsafe void RPC_PlayerHitByWaterBalloon(int playerId, RpcInfo info = default (RpcInfo))
  {
    if (this.InvokeRpc)
      this.InvokeRpc = false;
    else
      goto label_3;
label_2:
    GorillaNot.IncrementRPCCall(new PhotonMessageInfoWrapped(info), "PlayerHitByWaterBalloonRPC");
    this.PlayerHitByWaterBalloon(playerId);
    return;
label_3:
    NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized((NetworkBehaviour) this);
    if (this.Runner.Stage == SimulationStages.Resimulate)
      return;
    int localAuthorityMask = this.Object.GetLocalAuthorityMask();
    if ((localAuthorityMask & 7) == 0)
    {
      NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerHitByWaterBalloon(System.Int32,Fusion.RpcInfo)", this.Object, 7);
    }
    else
    {
      if ((localAuthorityMask & 1) != 1)
      {
        int num1 = 8 + 4;
        if (!SimulationMessage.CanAllocateUserPayload(num1))
        {
          NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerHitByWaterBalloon(System.Int32,Fusion.RpcInfo)", num1);
          return;
        }
        if (this.Runner.HasAnyActiveConnections())
        {
          SimulationMessage* message = SimulationMessage.Allocate(this.Runner.Simulation, num1);
          byte* numPtr = (byte*) ((IntPtr) message + 28);
          *(RpcHeader*) numPtr = RpcHeader.Create(this.Object.Id, this.ObjectIndex, 4);
          int num2 = 8;
          *(int*) (numPtr + num2) = playerId;
          int num3 = num2 + 4;
          message->Offset = num3 * 8;
          this.Runner.SendRpc(message);
        }
        if ((localAuthorityMask & 1) == 0)
          return;
      }
      info = RpcInfo.FromLocal(this.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
      goto label_2;
    }
  }

  private void PlayerHitByWaterBalloon(int playerId)
  {
    if (!this.IsMine)
      return;
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      if (this.inGamePlayerStates[index].playerId == playerId)
      {
        ScienceExperimentManager.PlayerGameState inGamePlayerState = this.inGamePlayerStates[index] with
        {
          touchedLiquid = false,
          touchedLiquidAtProgress = -1f
        };
        this.inGamePlayerStates[index] = inGamePlayerState;
        break;
      }
    }
  }

  public void OnPlayerLeftRoom(NetPlayer otherPlayer)
  {
    this.PlayerExitedGameArea(otherPlayer.ActorNumber);
  }

  public void OnLeftRoom()
  {
    this.inGamePlayerCount = 0;
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      if (this.inGamePlayerStates[index].playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
      {
        this.inGamePlayerStates[0] = this.inGamePlayerStates[index];
        this.inGamePlayerCount = 1;
        break;
      }
    }
  }

  protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
  {
    base.OnOwnerSwitched(newOwningPlayer);
    if (!NetworkSystem.Instance.IsMasterClient)
      return;
    for (int index = 0; index < this.inGamePlayerCount; ++index)
    {
      if (!Utils.PlayerInRoom(this.inGamePlayerStates[index].playerId))
      {
        this.inGamePlayerStates[index] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
        --this.inGamePlayerCount;
        --index;
      }
    }
  }

  [WeaverGenerated]
  public override void CopyBackingFieldsToState([In] bool obj0)
  {
    base.CopyBackingFieldsToState(obj0);
    this.Data = this._Data;
  }

  [WeaverGenerated]
  public override void CopyStateToBackingFields()
  {
    base.CopyStateToBackingFields();
    this._Data = this.Data;
  }

  [NetworkRpcWeavedInvoker(1, 7, 1)]
  [Preserve]
  [WeaverGenerated]
  protected static unsafe void RPC_PlayerTouchedLava\u0040Invoker(
    NetworkBehaviour behaviour,
    SimulationMessage* message)
  {
    byte* numPtr = (byte*) ((IntPtr) message + 28);
    RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
    behaviour.InvokeRpc = true;
    ((ScienceExperimentManager) behaviour).RPC_PlayerTouchedLava(info);
  }

  [NetworkRpcWeavedInvoker(2, 7, 1)]
  [Preserve]
  [WeaverGenerated]
  protected static unsafe void RPC_PlayerTouchedRefreshWater\u0040Invoker(
    NetworkBehaviour behaviour,
    SimulationMessage* message)
  {
    byte* numPtr = (byte*) ((IntPtr) message + 28);
    RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
    behaviour.InvokeRpc = true;
    ((ScienceExperimentManager) behaviour).RPC_PlayerTouchedRefreshWater(info);
  }

  [NetworkRpcWeavedInvoker(3, 7, 7)]
  [Preserve]
  [WeaverGenerated]
  protected static unsafe void RPC_ValidateLocalPlayerWaterBalloonHit\u0040Invoker(
    NetworkBehaviour behaviour,
    SimulationMessage* message)
  {
    byte* numPtr = (byte*) ((IntPtr) message + 28);
    int num1 = 8;
    int num2 = *(int*) (numPtr + num1);
    int num3 = num1 + 4;
    int playerId = num2;
    RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
    behaviour.InvokeRpc = true;
    ((ScienceExperimentManager) behaviour).RPC_ValidateLocalPlayerWaterBalloonHit(playerId, info);
  }

  [NetworkRpcWeavedInvoker(4, 7, 1)]
  [Preserve]
  [WeaverGenerated]
  protected static unsafe void RPC_PlayerHitByWaterBalloon\u0040Invoker(
    NetworkBehaviour behaviour,
    SimulationMessage* message)
  {
    byte* numPtr = (byte*) ((IntPtr) message + 28);
    int num1 = 8;
    int num2 = *(int*) (numPtr + num1);
    int num3 = num1 + 4;
    int playerId = num2;
    RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
    behaviour.InvokeRpc = true;
    ((ScienceExperimentManager) behaviour).RPC_PlayerHitByWaterBalloon(playerId, info);
  }

  public enum RisingLiquidState
  {
    Drained,
    Erupting,
    Rising,
    Full,
    PreDrainDelay,
    Draining,
  }

  private enum RiseSpeed
  {
    Fast,
    Medium,
    Slow,
    ExtraSlow,
  }

  private enum TagBehavior
  {
    None,
    Infect,
    Revive,
  }

  [Serializable]
  public struct PlayerGameState
  {
    public int playerId;
    public bool touchedLiquid;
    public float touchedLiquidAtProgress;
  }

  private struct SyncData
  {
    public ScienceExperimentManager.RisingLiquidState state;
    public double stateStartTime;
    public float stateStartLiquidProgressLinear;
    public double activationProgress;
  }

  private struct RotatingRingState
  {
    public Transform ringTransform;
    public float initialAngle;
    public float resultingAngle;
  }

  [Serializable]
  private struct DisableByLiquidData
  {
    public Transform target;
    public float heightOffset;
  }

  [NetworkStructWeaved(76)]
  [StructLayout(LayoutKind.Explicit, Size = 304)]
  private struct ScienceManagerData : INetworkStruct
  {
    [FieldOffset(0)]
    public int reliableState;
    [FieldOffset(4)]
    public double stateStartTime;
    [FieldOffset(12)]
    public float stateStartLiquidProgressLinear;
    [FieldOffset(16 /*0x10*/)]
    public double activationProgress;
    [FieldOffset(24)]
    public int nextRoundRiseSpeed;
    [FieldOffset(28)]
    public float riseTime;
    [FieldOffset(32 /*0x20*/)]
    public int lastWinnerId;
    [FieldOffset(36)]
    public int inGamePlayerCount;
    [FixedBufferProperty(typeof (NetworkArray<int>), typeof (UnityArraySurrogate\u0040ElementReaderWriterInt32), 10, order = -2147483647 /*0x80000001*/)]
    [WeaverGenerated]
    [SerializeField]
    [FieldOffset(40)]
    private FixedStorage\u004010 _playerIdArray;
    [FixedBufferProperty(typeof (NetworkArray<bool>), typeof (UnityArraySurrogate\u0040ElementReaderWriterBoolean), 10, order = -2147483647 /*0x80000001*/)]
    [WeaverGenerated]
    [SerializeField]
    [FieldOffset(80 /*0x50*/)]
    private FixedStorage\u004010 _touchedLiquidArray;
    [FixedBufferProperty(typeof (NetworkArray<float>), typeof (UnityArraySurrogate\u0040ElementReaderWriterSingle), 10, order = -2147483647 /*0x80000001*/)]
    [WeaverGenerated]
    [SerializeField]
    [FieldOffset(120)]
    private FixedStorage\u004010 _touchedLiquidAtProgressArray;
    [FixedBufferProperty(typeof (NetworkLinkedList<float>), typeof (UnityLinkedListSurrogate\u0040ElementReaderWriterSingle), 5, order = -2147483647 /*0x80000001*/)]
    [WeaverGenerated]
    [SerializeField]
    [FieldOffset(160 /*0xA0*/)]
    private FixedStorage\u004018 _initialAngleArray;
    [FixedBufferProperty(typeof (NetworkLinkedList<float>), typeof (UnityLinkedListSurrogate\u0040ElementReaderWriterSingle), 5, order = -2147483647 /*0x80000001*/)]
    [WeaverGenerated]
    [SerializeField]
    [FieldOffset(232)]
    private FixedStorage\u004018 _resultingAngleArray;

    [Networked]
    [Capacity(10)]
    [NetworkedWeavedArray(10, 1, typeof (ElementReaderWriterInt32))]
    [NetworkedWeaved(10, 10)]
    public unsafe NetworkArray<int> playerIdArray
    {
      get
      {
        return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage\u004010>(ref this._playerIdArray), 10, ElementReaderWriterInt32.GetInstance());
      }
    }

    [Networked]
    [Capacity(10)]
    [NetworkedWeavedArray(10, 1, typeof (ElementReaderWriterBoolean))]
    [NetworkedWeaved(20, 10)]
    public unsafe NetworkArray<bool> touchedLiquidArray
    {
      get
      {
        return new NetworkArray<bool>(Native.ReferenceToPointer<FixedStorage\u004010>(ref this._touchedLiquidArray), 10, ElementReaderWriterBoolean.GetInstance());
      }
    }

    [Networked]
    [Capacity(10)]
    [NetworkedWeavedArray(10, 1, typeof (ElementReaderWriterSingle))]
    [NetworkedWeaved(30, 10)]
    public unsafe NetworkArray<float> touchedLiquidAtProgressArray
    {
      get
      {
        return new NetworkArray<float>(Native.ReferenceToPointer<FixedStorage\u004010>(ref this._touchedLiquidAtProgressArray), 10, ElementReaderWriterSingle.GetInstance());
      }
    }

    [Networked]
    [Capacity(5)]
    [NetworkedWeavedLinkedList(5, 1, typeof (ElementReaderWriterSingle))]
    [NetworkedWeaved(40, 18)]
    public unsafe NetworkLinkedList<float> initialAngleArray
    {
      get
      {
        return new NetworkLinkedList<float>(Native.ReferenceToPointer<FixedStorage\u004018>(ref this._initialAngleArray), 5, ElementReaderWriterSingle.GetInstance());
      }
    }

    [Networked]
    [Capacity(5)]
    [NetworkedWeavedLinkedList(5, 1, typeof (ElementReaderWriterSingle))]
    [NetworkedWeaved(58, 18)]
    public unsafe NetworkLinkedList<float> resultingAngleArray
    {
      get
      {
        return new NetworkLinkedList<float>(Native.ReferenceToPointer<FixedStorage\u004018>(ref this._resultingAngleArray), 5, ElementReaderWriterSingle.GetInstance());
      }
    }

    public ScienceManagerData(
      int reliableState,
      double stateStartTime,
      float stateStartLiquidProgressLinear,
      double activationProgress,
      int nextRoundRiseSpeed,
      float riseTime,
      int lastWinnerId,
      int inGamePlayerCount,
      ScienceExperimentManager.PlayerGameState[] playerStates,
      ScienceExperimentManager.RotatingRingState[] rings)
    {
      this.reliableState = reliableState;
      this.stateStartTime = stateStartTime;
      this.stateStartLiquidProgressLinear = stateStartLiquidProgressLinear;
      this.activationProgress = activationProgress;
      this.nextRoundRiseSpeed = nextRoundRiseSpeed;
      this.riseTime = riseTime;
      this.lastWinnerId = lastWinnerId;
      this.inGamePlayerCount = inGamePlayerCount;
      foreach (ScienceExperimentManager.RotatingRingState ring in rings)
      {
        NetworkLinkedList<float> networkLinkedList = this.initialAngleArray;
        networkLinkedList.Add(ring.initialAngle);
        networkLinkedList = this.resultingAngleArray;
        networkLinkedList.Add(ring.resultingAngle);
      }
      int[] source1 = new int[10];
      bool[] source2 = new bool[10];
      float[] source3 = new float[10];
      for (int index = 0; index < 10; ++index)
      {
        source1[index] = playerStates[index].playerId;
        source2[index] = playerStates[index].touchedLiquid;
        source3[index] = playerStates[index].touchedLiquidAtProgress;
      }
      this.playerIdArray.CopyFrom(source1, 0, source1.Length);
      this.touchedLiquidArray.CopyFrom(source2, 0, source2.Length);
      this.touchedLiquidAtProgressArray.CopyFrom(source3, 0, source3.Length);
    }
  }
}
