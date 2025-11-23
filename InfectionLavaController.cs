// Decompiled with JetBrains decompiler
// Type: GorillaTag.InfectionLavaController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaTag.GuidedRefs;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag;

public class InfectionLavaController : 
  MonoBehaviour,
  IGorillaSerializeableScene,
  IGorillaSerializeable,
  ITickSystemPost,
  IGuidedRefReceiverMono,
  IGuidedRefMonoBehaviour,
  IGuidedRefObject
{
  [OnEnterPlay_SetNull]
  private static InfectionLavaController instance;
  [SerializeField]
  private float lavaMeshMinScale = 3.17f;
  [Tooltip("If you throw rocks into the volcano quickly enough, then it will raise to this height.")]
  [SerializeField]
  private float lavaMeshMaxScale = 8.941086f;
  [SerializeField]
  private float eruptTime = 3f;
  [SerializeField]
  private float riseTime = 10f;
  [SerializeField]
  private float fullTime = 240f;
  [SerializeField]
  private float drainTime = 10f;
  [SerializeField]
  private float lagResolutionLavaProgressPerSecond = 0.2f;
  [SerializeField]
  private AnimationCurve lavaProgressAnimationCurve = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [Header("Volcano Activation")]
  [SerializeField]
  [Range(0.0f, 1f)]
  private float activationVotePercentageDefaultQueue = 0.42f;
  [SerializeField]
  [Range(0.0f, 1f)]
  private float activationVotePercentageCompetitiveQueue = 0.6f;
  [SerializeField]
  private Gradient lavaActivationGradient;
  [SerializeField]
  private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  private float lavaActivationVisualMovementProgressPerSecond = 1f;
  [SerializeField]
  private bool debugLavaActivationVotes;
  [Header("Scene References")]
  [SerializeField]
  private Transform lavaMeshTransform;
  [SerializeField]
  private GuidedRefReceiverFieldInfo lavaMeshTransform_gRef = new GuidedRefReceiverFieldInfo(true);
  [SerializeField]
  private Transform lavaSurfacePlaneTransform;
  [SerializeField]
  private GuidedRefReceiverFieldInfo lavaSurfacePlaneTransform_gRef = new GuidedRefReceiverFieldInfo(true);
  [SerializeField]
  private WaterVolume lavaVolume;
  [SerializeField]
  private GuidedRefReceiverFieldInfo lavaVolume_gRef = new GuidedRefReceiverFieldInfo(true);
  [SerializeField]
  private MeshRenderer lavaActivationRenderer;
  [SerializeField]
  private GuidedRefReceiverFieldInfo lavaActivationRenderer_gRef = new GuidedRefReceiverFieldInfo(true);
  [SerializeField]
  private Transform lavaActivationStartPos;
  [SerializeField]
  private GuidedRefReceiverFieldInfo lavaActivationStartPos_gRef = new GuidedRefReceiverFieldInfo(true);
  [SerializeField]
  private Transform lavaActivationEndPos;
  [SerializeField]
  private GuidedRefReceiverFieldInfo lavaActivationEndPos_gRef = new GuidedRefReceiverFieldInfo(true);
  [SerializeField]
  private SlingshotProjectileHitNotifier lavaActivationProjectileHitNotifier;
  [SerializeField]
  private GuidedRefReceiverFieldInfo lavaActivationProjectileHitNotifier_gRef = new GuidedRefReceiverFieldInfo(true);
  [SerializeField]
  private VolcanoEffects[] volcanoEffects;
  [SerializeField]
  private GuidedRefReceiverArrayInfo volcanoEffects_gRefs = new GuidedRefReceiverArrayInfo(true);
  [DebugReadout]
  private InfectionLavaController.LavaSyncData reliableState;
  private int[] lavaActivationVotePlayerIds = new int[10];
  private int lavaActivationVoteCount;
  private float localLagLavaProgressOffset;
  [DebugReadout]
  private float lavaProgressLinear;
  [DebugReadout]
  private float lavaProgressSmooth;
  private double lastTagSelfRPCTime;
  private const string lavaRockProjectileTag = "LavaRockProjectile";
  private double currentTime;
  private double prevTime;
  private float activationProgessSmooth;
  private float lavaScale;
  private GorillaSerializerScene networkObject;
  private bool guidedRefsFullyResolved;

  public static InfectionLavaController Instance => InfectionLavaController.instance;

  public bool LavaCurrentlyActivated => this.reliableState.state != 0;

  public Plane LavaPlane
  {
    get => new Plane(this.lavaSurfacePlaneTransform.up, this.lavaSurfacePlaneTransform.position);
  }

  public Vector3 SurfaceCenter => this.lavaSurfacePlaneTransform.position;

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

  private bool InCompetitiveQueue
  {
    get
    {
      return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("COMPETITIVE");
    }
  }

  private void Awake()
  {
    if (InfectionLavaController.instance.IsNotNull())
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
    else
    {
      InfectionLavaController.instance = this;
      RoomSystem.LeftRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>) RoomSystem.LeftRoomEvent + new Action(this.OnLeftRoom);
      RoomSystem.PlayerLeftEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>) RoomSystem.PlayerLeftEvent + new Action<NetPlayer>(this.OnPlayerLeftRoom);
      ((IGuidedRefObject) this).GuidedRefInitialize();
      if ((UnityEngine.Object) this.lavaVolume != (UnityEngine.Object) null)
        this.lavaVolume.ColliderEnteredWater += new WaterVolume.WaterVolumeEvent(this.OnColliderEnteredLava);
      if (!((UnityEngine.Object) this.lavaActivationProjectileHitNotifier != (UnityEngine.Object) null))
        return;
      this.lavaActivationProjectileHitNotifier.OnProjectileHit += new SlingshotProjectileHitNotifier.ProjectileHitEvent(this.OnActivationLavaProjectileHit);
    }
  }

  protected void OnEnable()
  {
    if (!this.guidedRefsFullyResolved)
      return;
    this.VerifyReferences();
    TickSystem<object>.AddPostTickCallback((ITickSystemPost) this);
  }

  void IGorillaSerializeableScene.OnSceneLinking(GorillaSerializerScene netObj)
  {
    this.networkObject = netObj;
  }

  protected void OnDisable() => TickSystem<object>.RemovePostTickCallback((ITickSystemPost) this);

  private void VerifyReferences()
  {
    this.IfNullThenLogAndDisableSelf((UnityEngine.Object) this.lavaMeshTransform, "lavaMeshTransform");
    this.IfNullThenLogAndDisableSelf((UnityEngine.Object) this.lavaSurfacePlaneTransform, "lavaSurfacePlaneTransform");
    this.IfNullThenLogAndDisableSelf((UnityEngine.Object) this.lavaVolume, "lavaVolume");
    this.IfNullThenLogAndDisableSelf((UnityEngine.Object) this.lavaActivationRenderer, "lavaActivationRenderer");
    this.IfNullThenLogAndDisableSelf((UnityEngine.Object) this.lavaActivationStartPos, "lavaActivationStartPos");
    this.IfNullThenLogAndDisableSelf((UnityEngine.Object) this.lavaActivationEndPos, "lavaActivationEndPos");
    this.IfNullThenLogAndDisableSelf((UnityEngine.Object) this.lavaActivationProjectileHitNotifier, "lavaActivationProjectileHitNotifier");
    for (int index = 0; index < this.volcanoEffects.Length; ++index)
      this.IfNullThenLogAndDisableSelf((UnityEngine.Object) this.volcanoEffects[index], "volcanoEffects", index);
  }

  private void IfNullThenLogAndDisableSelf(UnityEngine.Object obj, string fieldName, int index = -1)
  {
    if (obj != (UnityEngine.Object) null)
      return;
    fieldName = index != -1 ? $"{fieldName}[{index}]" : fieldName;
    Debug.LogError((object) $"InfectionLavaController: Disabling self because reference `{fieldName}` is null.", (UnityEngine.Object) this);
    this.enabled = false;
  }

  private void OnDestroy()
  {
    if ((UnityEngine.Object) InfectionLavaController.instance == (UnityEngine.Object) this)
      InfectionLavaController.instance = (InfectionLavaController) null;
    TickSystem<object>.RemovePostTickCallback((ITickSystemPost) this);
    this.UpdateLava(0.0f);
    if ((UnityEngine.Object) this.lavaVolume != (UnityEngine.Object) null)
      this.lavaVolume.ColliderEnteredWater -= new WaterVolume.WaterVolumeEvent(this.OnColliderEnteredLava);
    if (!((UnityEngine.Object) this.lavaActivationProjectileHitNotifier != (UnityEngine.Object) null))
      return;
    this.lavaActivationProjectileHitNotifier.OnProjectileHit -= new SlingshotProjectileHitNotifier.ProjectileHitEvent(this.OnActivationLavaProjectileHit);
  }

  bool ITickSystemPost.PostTickRunning { get; set; }

  void ITickSystemPost.PostTick()
  {
    this.prevTime = this.currentTime;
    this.currentTime = NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble;
    if (this.networkObject.HasAuthority)
      this.UpdateReliableState(this.currentTime, ref this.reliableState);
    this.UpdateLocalState(this.currentTime, this.reliableState);
    this.localLagLavaProgressOffset = Mathf.MoveTowards(this.localLagLavaProgressOffset, 0.0f, this.lagResolutionLavaProgressPerSecond * Time.deltaTime);
    this.UpdateLava(this.lavaProgressSmooth + this.localLagLavaProgressOffset);
    this.UpdateVolcanoActivationLava((float) this.reliableState.activationProgress);
    this.CheckLocalPlayerAgainstLava(this.currentTime);
  }

  private void JumpToState(InfectionLavaController.RisingLavaState state)
  {
    this.reliableState.state = state;
    switch (state)
    {
      case InfectionLavaController.RisingLavaState.Drained:
        for (int index = 0; index < this.volcanoEffects.Length; ++index)
          this.volcanoEffects[index]?.SetDrainedState();
        break;
      case InfectionLavaController.RisingLavaState.Erupting:
        for (int index = 0; index < this.volcanoEffects.Length; ++index)
          this.volcanoEffects[index]?.SetEruptingState();
        break;
      case InfectionLavaController.RisingLavaState.Rising:
        for (int index = 0; index < this.volcanoEffects.Length; ++index)
          this.volcanoEffects[index]?.SetRisingState();
        break;
      case InfectionLavaController.RisingLavaState.Full:
        for (int index = 0; index < this.volcanoEffects.Length; ++index)
          this.volcanoEffects[index]?.SetFullState();
        break;
      case InfectionLavaController.RisingLavaState.Draining:
        for (int index = 0; index < this.volcanoEffects.Length; ++index)
          this.volcanoEffects[index]?.SetDrainingState();
        break;
    }
  }

  private void UpdateReliableState(
    double currentTime,
    ref InfectionLavaController.LavaSyncData syncData)
  {
    if (currentTime < syncData.stateStartTime)
      syncData.stateStartTime = currentTime;
    switch (syncData.state)
    {
      case InfectionLavaController.RisingLavaState.Erupting:
        if (currentTime <= syncData.stateStartTime + (double) this.eruptTime)
          break;
        syncData.state = InfectionLavaController.RisingLavaState.Rising;
        syncData.stateStartTime = currentTime;
        for (int index = 0; index < this.volcanoEffects.Length; ++index)
          this.volcanoEffects[index]?.SetRisingState();
        break;
      case InfectionLavaController.RisingLavaState.Rising:
        if (currentTime <= syncData.stateStartTime + (double) this.riseTime)
          break;
        syncData.state = InfectionLavaController.RisingLavaState.Full;
        syncData.stateStartTime = currentTime;
        for (int index = 0; index < this.volcanoEffects.Length; ++index)
          this.volcanoEffects[index]?.SetFullState();
        break;
      case InfectionLavaController.RisingLavaState.Full:
        if (currentTime <= syncData.stateStartTime + (double) this.fullTime)
          break;
        syncData.state = InfectionLavaController.RisingLavaState.Draining;
        syncData.stateStartTime = currentTime;
        for (int index = 0; index < this.volcanoEffects.Length; ++index)
          this.volcanoEffects[index]?.SetDrainingState();
        break;
      case InfectionLavaController.RisingLavaState.Draining:
        syncData.activationProgress = (double) Mathf.MoveTowards((float) syncData.activationProgress, 0.0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float) this.PlayerCount) * Time.deltaTime);
        if (currentTime <= syncData.stateStartTime + (double) this.drainTime)
          break;
        syncData.state = InfectionLavaController.RisingLavaState.Drained;
        syncData.stateStartTime = currentTime;
        for (int index = 0; index < this.volcanoEffects.Length; ++index)
          this.volcanoEffects[index]?.SetDrainedState();
        break;
      default:
        if (syncData.activationProgress > 1.0)
        {
          if (this.lavaActivationVoteCount < Mathf.RoundToInt((float) this.PlayerCount * (this.InCompetitiveQueue ? this.activationVotePercentageCompetitiveQueue : this.activationVotePercentageDefaultQueue)))
            break;
          for (int index = 0; index < this.lavaActivationVoteCount; ++index)
            this.lavaActivationVotePlayerIds[index] = 0;
          this.lavaActivationVoteCount = 0;
          syncData.state = InfectionLavaController.RisingLavaState.Erupting;
          syncData.stateStartTime = currentTime;
          syncData.activationProgress = 1.0;
          for (int index = 0; index < this.volcanoEffects.Length; ++index)
            this.volcanoEffects[index]?.SetEruptingState();
          break;
        }
        float num = Mathf.Clamp((float) (currentTime - this.prevTime), 0.0f, 0.1f);
        double activationProgress = syncData.activationProgress;
        syncData.activationProgress = (double) Mathf.MoveTowards((float) syncData.activationProgress, 0.0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float) this.PlayerCount) * num);
        if ((activationProgress <= 0.0 ? 0 : (syncData.activationProgress <= double.Epsilon ? 1 : 0)) == 0)
          break;
        foreach (VolcanoEffects volcanoEffect in this.volcanoEffects)
          volcanoEffect.OnVolcanoBellyEmpty();
        break;
    }
  }

  private void UpdateLocalState(double currentTime, InfectionLavaController.LavaSyncData syncData)
  {
    switch (syncData.state)
    {
      case InfectionLavaController.RisingLavaState.Erupting:
        this.lavaProgressLinear = 0.0f;
        this.lavaProgressSmooth = 0.0f;
        float time1 = (float) (currentTime - syncData.stateStartTime);
        float progress1 = Mathf.Clamp01(time1 / this.eruptTime);
        foreach (VolcanoEffects volcanoEffect in this.volcanoEffects)
        {
          if ((UnityEngine.Object) volcanoEffect != (UnityEngine.Object) null)
            volcanoEffect.UpdateEruptingState(time1, this.eruptTime - time1, progress1);
        }
        break;
      case InfectionLavaController.RisingLavaState.Rising:
        this.lavaProgressLinear = Mathf.Clamp01((float) (currentTime - syncData.stateStartTime) / this.riseTime);
        this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
        float time2 = (float) (currentTime - syncData.stateStartTime);
        foreach (VolcanoEffects volcanoEffect in this.volcanoEffects)
        {
          if ((UnityEngine.Object) volcanoEffect != (UnityEngine.Object) null)
            volcanoEffect.UpdateRisingState(time2, this.riseTime - time2, this.lavaProgressLinear);
        }
        break;
      case InfectionLavaController.RisingLavaState.Full:
        this.lavaProgressLinear = 1f;
        this.lavaProgressSmooth = 1f;
        float time3 = (float) (currentTime - syncData.stateStartTime);
        float progress2 = Mathf.Clamp01(this.fullTime / time3);
        foreach (VolcanoEffects volcanoEffect in this.volcanoEffects)
        {
          if ((UnityEngine.Object) volcanoEffect != (UnityEngine.Object) null)
            volcanoEffect.UpdateFullState(time3, this.fullTime - time3, progress2);
        }
        break;
      case InfectionLavaController.RisingLavaState.Draining:
        float time4 = (float) (currentTime - syncData.stateStartTime);
        float progress3 = Mathf.Clamp01(time4 / this.drainTime);
        this.lavaProgressLinear = 1f - progress3;
        this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
        foreach (VolcanoEffects volcanoEffect in this.volcanoEffects)
        {
          if ((UnityEngine.Object) volcanoEffect != (UnityEngine.Object) null)
            volcanoEffect.UpdateDrainingState(time4, this.riseTime - time4, progress3);
        }
        break;
      default:
        this.lavaProgressLinear = 0.0f;
        this.lavaProgressSmooth = 0.0f;
        float time5 = (float) (currentTime - syncData.stateStartTime);
        foreach (VolcanoEffects volcanoEffect in this.volcanoEffects)
        {
          if ((UnityEngine.Object) volcanoEffect != (UnityEngine.Object) null)
            volcanoEffect.UpdateDrainedState(time5);
        }
        break;
    }
  }

  private void UpdateLava(float fillProgress)
  {
    this.lavaScale = Mathf.Lerp(this.lavaMeshMinScale, this.lavaMeshMaxScale, fillProgress);
    if (!((UnityEngine.Object) this.lavaMeshTransform != (UnityEngine.Object) null))
      return;
    this.lavaMeshTransform.localScale = new Vector3(this.lavaMeshTransform.localScale.x, this.lavaMeshTransform.localScale.y, this.lavaScale);
  }

  private void UpdateVolcanoActivationLava(float activationProgress)
  {
    this.activationProgessSmooth = Mathf.MoveTowards(this.activationProgessSmooth, activationProgress, this.lavaActivationVisualMovementProgressPerSecond * Time.deltaTime);
    this.lavaActivationRenderer.material.SetColor(ShaderProps._BaseColor, this.lavaActivationGradient.Evaluate(activationProgress));
    this.lavaActivationRenderer.transform.position = Vector3.Lerp(this.lavaActivationStartPos.position, this.lavaActivationEndPos.position, this.activationProgessSmooth);
  }

  private void CheckLocalPlayerAgainstLava(double currentTime)
  {
    if (!GTPlayer.Instance.InWater || !((UnityEngine.Object) GTPlayer.Instance.CurrentWaterVolume == (UnityEngine.Object) this.lavaVolume))
      return;
    this.LocalPlayerInLava(currentTime, false);
  }

  private void OnColliderEnteredLava(WaterVolume volume, Collider collider)
  {
    if (!((UnityEngine.Object) collider == (UnityEngine.Object) GTPlayer.Instance.bodyCollider))
      return;
    this.LocalPlayerInLava(NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble, true);
  }

  private void LocalPlayerInLava(double currentTime, bool enteredLavaThisFrame)
  {
    GorillaGameManager instance = GorillaGameManager.instance;
    if (!((UnityEngine.Object) instance != (UnityEngine.Object) null) || !instance.CanAffectPlayer(NetworkSystem.Instance.LocalPlayer, enteredLavaThisFrame) || !(currentTime - this.lastTagSelfRPCTime > 0.5 | enteredLavaThisFrame))
      return;
    this.lastTagSelfRPCTime = currentTime;
    GameMode.ReportHit();
  }

  public void OnActivationLavaProjectileHit(SlingshotProjectile projectile, Collision collision)
  {
    if (!projectile.gameObject.CompareTag("LavaRockProjectile"))
      return;
    this.AddLavaRock(projectile.projectileOwner.ActorNumber);
  }

  private void AddLavaRock(int playerId)
  {
    if (!this.networkObject.HasAuthority || this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
      return;
    this.reliableState.activationProgress += (double) this.lavaActivationRockProgressVsPlayerCount.Evaluate((float) this.PlayerCount);
    this.AddVoteForVolcanoActivation(playerId);
    foreach (VolcanoEffects volcanoEffect in this.volcanoEffects)
      volcanoEffect.OnStoneAccepted(this.reliableState.activationProgress);
  }

  private void AddVoteForVolcanoActivation(int playerId)
  {
    if (!this.networkObject.HasAuthority || this.lavaActivationVoteCount >= 10)
      return;
    bool flag = false;
    for (int index = 0; index < this.lavaActivationVoteCount; ++index)
    {
      if (this.lavaActivationVotePlayerIds[index] == playerId)
        flag = true;
    }
    if (flag)
      return;
    this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount] = playerId;
    ++this.lavaActivationVoteCount;
  }

  private void RemoveVoteForVolcanoActivation(int playerId)
  {
    if (!this.networkObject.HasAuthority)
      return;
    for (int index = 0; index < this.lavaActivationVoteCount; ++index)
    {
      if (this.lavaActivationVotePlayerIds[index] == playerId)
      {
        this.lavaActivationVotePlayerIds[index] = this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount - 1];
        --this.lavaActivationVoteCount;
        break;
      }
    }
  }

  void IGorillaSerializeable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
  {
    stream.SendNext((object) (int) this.reliableState.state);
    stream.SendNext((object) this.reliableState.stateStartTime);
    stream.SendNext((object) this.reliableState.activationProgress);
    stream.SendNext((object) this.lavaActivationVoteCount);
    stream.SendNext((object) this.lavaActivationVotePlayerIds[0]);
    stream.SendNext((object) this.lavaActivationVotePlayerIds[1]);
    stream.SendNext((object) this.lavaActivationVotePlayerIds[2]);
    stream.SendNext((object) this.lavaActivationVotePlayerIds[3]);
    stream.SendNext((object) this.lavaActivationVotePlayerIds[4]);
    stream.SendNext((object) this.lavaActivationVotePlayerIds[5]);
    stream.SendNext((object) this.lavaActivationVotePlayerIds[6]);
    stream.SendNext((object) this.lavaActivationVotePlayerIds[7]);
    stream.SendNext((object) this.lavaActivationVotePlayerIds[8]);
    stream.SendNext((object) this.lavaActivationVotePlayerIds[9]);
  }

  void IGorillaSerializeable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
  {
    InfectionLavaController.RisingLavaState next = (InfectionLavaController.RisingLavaState) stream.ReceiveNext();
    this.reliableState.stateStartTime = ((double) stream.ReceiveNext()).GetFinite();
    this.reliableState.activationProgress = ((double) stream.ReceiveNext()).ClampSafe(0.0, 2.0);
    this.lavaActivationVoteCount = (int) stream.ReceiveNext();
    this.lavaActivationVotePlayerIds[0] = (int) stream.ReceiveNext();
    this.lavaActivationVotePlayerIds[1] = (int) stream.ReceiveNext();
    this.lavaActivationVotePlayerIds[2] = (int) stream.ReceiveNext();
    this.lavaActivationVotePlayerIds[3] = (int) stream.ReceiveNext();
    this.lavaActivationVotePlayerIds[4] = (int) stream.ReceiveNext();
    this.lavaActivationVotePlayerIds[5] = (int) stream.ReceiveNext();
    this.lavaActivationVotePlayerIds[6] = (int) stream.ReceiveNext();
    this.lavaActivationVotePlayerIds[7] = (int) stream.ReceiveNext();
    this.lavaActivationVotePlayerIds[8] = (int) stream.ReceiveNext();
    this.lavaActivationVotePlayerIds[9] = (int) stream.ReceiveNext();
    float lavaProgressSmooth = this.lavaProgressSmooth;
    if (next != this.reliableState.state)
      this.JumpToState(next);
    this.UpdateLocalState(NetworkSystem.Instance.SimTime, this.reliableState);
    this.localLagLavaProgressOffset = lavaProgressSmooth - this.lavaProgressSmooth;
  }

  public void OnPlayerLeftRoom(NetPlayer otherNetPlayer)
  {
    this.RemoveVoteForVolcanoActivation(otherNetPlayer.ActorNumber);
  }

  private void OnLeftRoom()
  {
    for (int index = 0; index < this.lavaActivationVotePlayerIds.Length; ++index)
    {
      if (this.lavaActivationVotePlayerIds[index] != NetworkSystem.Instance.LocalPlayerID)
        this.RemoveVoteForVolcanoActivation(this.lavaActivationVotePlayerIds[index]);
    }
  }

  void IGorillaSerializeableScene.OnNetworkObjectDisable()
  {
  }

  void IGorillaSerializeableScene.OnNetworkObjectEnable()
  {
  }

  int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

  void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
  {
    this.guidedRefsFullyResolved = true;
    this.VerifyReferences();
    TickSystem<object>.AddPostTickCallback((ITickSystemPost) this);
  }

  public void OnGuidedRefTargetDestroyed(int fieldId)
  {
    this.guidedRefsFullyResolved = false;
    TickSystem<object>.RemovePostTickCallback((ITickSystemPost) this);
  }

  void IGuidedRefObject.GuidedRefInitialize()
  {
    GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaMeshTransform_gRef", ref this.lavaMeshTransform_gRef);
    GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaSurfacePlaneTransform_gRef", ref this.lavaSurfacePlaneTransform_gRef);
    GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaVolume_gRef", ref this.lavaVolume_gRef);
    GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationRenderer_gRef", ref this.lavaActivationRenderer_gRef);
    GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationStartPos_gRef", ref this.lavaActivationStartPos_gRef);
    GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationEndPos_gRef", ref this.lavaActivationEndPos_gRef);
    GuidedRefHub.RegisterReceiverField<InfectionLavaController>(this, "lavaActivationProjectileHitNotifier_gRef", ref this.lavaActivationProjectileHitNotifier_gRef);
    GuidedRefHub.RegisterReceiverArray<InfectionLavaController, VolcanoEffects>(this, "volcanoEffects_gRefs", ref this.volcanoEffects, ref this.volcanoEffects_gRefs);
    GuidedRefHub.ReceiverFullyRegistered<InfectionLavaController>(this);
  }

  bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
  {
    return GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaMeshTransform, this.lavaMeshTransform_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaSurfacePlaneTransform, this.lavaSurfacePlaneTransform_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, WaterVolume>(this, ref this.lavaVolume, this.lavaVolume_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, MeshRenderer>(this, ref this.lavaActivationRenderer, this.lavaActivationRenderer_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaActivationStartPos, this.lavaActivationStartPos_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, Transform>(this, ref this.lavaActivationEndPos, this.lavaActivationEndPos_gRef, target) || GuidedRefHub.TryResolveField<InfectionLavaController, SlingshotProjectileHitNotifier>(this, ref this.lavaActivationProjectileHitNotifier, this.lavaActivationProjectileHitNotifier_gRef, target) || GuidedRefHub.TryResolveArrayItem<InfectionLavaController, VolcanoEffects>(this, (IList<VolcanoEffects>) this.volcanoEffects, this.volcanoEffects_gRefs, target);
  }

  Transform IGuidedRefMonoBehaviour.get_transform() => this.transform;

  int IGuidedRefObject.GetInstanceID() => this.GetInstanceID();

  public enum RisingLavaState
  {
    Drained,
    Erupting,
    Rising,
    Full,
    Draining,
  }

  private struct LavaSyncData
  {
    public InfectionLavaController.RisingLavaState state;
    public double stateStartTime;
    public double activationProgress;
  }
}
