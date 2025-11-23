// Decompiled with JetBrains decompiler
// Type: GorillaTag.ScienceExperimentPlatformGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaTag.GuidedRefs;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag;

public class ScienceExperimentPlatformGenerator : 
  MonoBehaviourPun,
  ITickSystemPost,
  IGuidedRefReceiverMono,
  IGuidedRefMonoBehaviour,
  IGuidedRefObject
{
  [SerializeField]
  private GameObject spawnedPrefab;
  [SerializeField]
  private float scaleFactor = 0.03f;
  [Header("Random Bubbles")]
  [SerializeField]
  private Vector2 surfaceRadiusSpawnRange = new Vector2(0.1f, 0.7f);
  [SerializeField]
  private Vector2 lifetimeRange = new Vector2(5f, 10f);
  [SerializeField]
  private Vector2 sizeRange = new Vector2(0.5f, 2f);
  [SerializeField]
  private AnimationCurve rockCountVsLavaProgress = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  [FormerlySerializedAs("rockCountMultiplier")]
  private float bubbleCountMultiplier = 80f;
  [SerializeField]
  private int maxBubbleCount = 100;
  [SerializeField]
  private AnimationCurve rockLifetimeMultiplierVsLavaProgress = AnimationCurve.Linear(0.0f, 1f, 1f, 1f);
  [SerializeField]
  private AnimationCurve rockMaxSizeMultiplierVsLavaProgress = AnimationCurve.Linear(0.0f, 1f, 1f, 1f);
  [SerializeField]
  private AnimationCurve spawnRadiusMultiplierVsLavaProgress = AnimationCurve.Linear(0.0f, 1f, 1f, 1f);
  [SerializeField]
  private AnimationCurve rockSizeVsLifetime = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [Header("Bubble Trails")]
  [SerializeField]
  private AnimationCurve trailSpawnRateVsProgress = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  private float trailSpawnRateMultiplier = 1f;
  [SerializeField]
  private AnimationCurve trailBubbleLifetimeVsProgress = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  private AnimationCurve trailBubbleBoundaryRadiusVsProgress = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  private float trailBubbleLifetimeMultiplier = 6f;
  [SerializeField]
  private float trailDistanceBetweenSpawns = 3f;
  [SerializeField]
  private float trailMaxTurnAngle = 55f;
  [SerializeField]
  private float trailBubbleSize = 1.5f;
  [SerializeField]
  private AnimationCurve trailCountVsProgress = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  private float trailCountMultiplier = 12f;
  [SerializeField]
  private Vector2 trailEdgeAvoidanceSpawnsMinMax = new Vector2(3f, 1f);
  [Header("Feedback Effects")]
  [SerializeField]
  private float bubblePopAnticipationTime = 2f;
  [SerializeField]
  private float bubblePopWobbleFrequency = 25f;
  [SerializeField]
  private float bubblePopWobbleAmplitude = 0.01f;
  [SerializeField]
  private Transform liquidSurfacePlane;
  [SerializeField]
  private GuidedRefReceiverFieldInfo liquidSurfacePlane_gRef = new GuidedRefReceiverFieldInfo(true);
  private List<ScienceExperimentPlatformGenerator.BubbleData> activeBubbles = new List<ScienceExperimentPlatformGenerator.BubbleData>();
  private List<ScienceExperimentPlatformGenerator.BubbleData> trailHeads = new List<ScienceExperimentPlatformGenerator.BubbleData>();
  private List<ScienceExperimentPlatformGenerator.BubbleSpawnDebug> bubbleSpawnDebug = new List<ScienceExperimentPlatformGenerator.BubbleSpawnDebug>();
  private ScienceExperimentManager scienceExperimentManager;

  private void Awake()
  {
    ((IGuidedRefObject) this).GuidedRefInitialize();
    this.scienceExperimentManager = this.GetComponent<ScienceExperimentManager>();
  }

  private void OnEnable()
  {
    if (((IGuidedRefReceiverMono) this).GuidedRefsWaitingToResolveCount > 0)
      return;
    TickSystem<object>.AddPostTickCallback((ITickSystemPost) this);
  }

  protected void OnDisable() => TickSystem<object>.RemovePostTickCallback((ITickSystemPost) this);

  bool ITickSystemPost.PostTickRunning { get; set; }

  void ITickSystemPost.PostTick()
  {
    double currentTime = PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.unscaledTimeAsDouble;
    this.UpdateTrails(currentTime);
    this.RemoveExpiredBubbles(currentTime);
    this.SpawnNewBubbles(currentTime);
    this.UpdateActiveBubbles(currentTime);
  }

  private void RemoveExpiredBubbles(double currentTime)
  {
    for (int index = this.activeBubbles.Count - 1; index >= 0; --index)
    {
      if ((double) Mathf.Clamp01((float) (currentTime - this.activeBubbles[index].spawnTime) / this.activeBubbles[index].lifetime) >= 1.0)
      {
        this.activeBubbles[index].bubble.Pop();
        this.activeBubbles.RemoveAt(index);
      }
    }
  }

  private void SpawnNewBubbles(double currentTime)
  {
    if (!this.photonView.IsMine || this.scienceExperimentManager.GameState != ScienceExperimentManager.RisingLiquidState.Rising)
      return;
    int num = Mathf.Min((int) ((double) this.rockCountVsLavaProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * (double) this.bubbleCountMultiplier), this.maxBubbleCount) - this.activeBubbles.Count;
    if (this.activeBubbles.Count >= this.maxBubbleCount)
      return;
    for (int index = 0; index < num; ++index)
      this.SpawnRockAuthority(currentTime, this.scienceExperimentManager.RiseProgressLinear);
  }

  private void UpdateActiveBubbles(double currentTime)
  {
    if ((Object) this.liquidSurfacePlane == (Object) null)
      return;
    float y = this.liquidSurfacePlane.transform.position.y;
    float num1 = this.bubblePopWobbleAmplitude * Mathf.Sin((float) ((double) this.bubblePopWobbleFrequency * 0.5 * 3.1415927410125732) * Time.time);
    for (int index = 0; index < this.activeBubbles.Count; ++index)
    {
      ScienceExperimentPlatformGenerator.BubbleData activeBubble = this.activeBubbles[index];
      float time = Mathf.Clamp01((float) (currentTime - activeBubble.spawnTime) / activeBubble.lifetime);
      float num2 = activeBubble.spawnSize * this.rockSizeVsLifetime.Evaluate(time) * this.scaleFactor;
      activeBubble.position.y = y;
      activeBubble.bubble.body.gameObject.transform.localScale = Vector3.one * num2;
      activeBubble.bubble.body.MovePosition(activeBubble.position);
      float num3 = (float) ((double) activeBubble.lifetime + activeBubble.spawnTime - currentTime);
      if ((double) num3 < (double) this.bubblePopAnticipationTime)
      {
        float num4 = Mathf.Clamp01((float) (1.0 - (double) num3 / (double) this.bubblePopAnticipationTime));
        activeBubble.bubble.bubbleMesh.transform.localScale = Vector3.one * (float) (1.0 + (double) num4 * (double) num1);
      }
      this.activeBubbles[index] = activeBubble;
    }
  }

  private void UpdateTrails(double currentTime)
  {
    if (!this.photonView.IsMine)
      return;
    int num1 = (int) ((double) this.trailCountVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * (double) this.trailCountMultiplier) - this.trailHeads.Count;
    if (num1 > 0 && this.scienceExperimentManager.GameState == ScienceExperimentManager.RisingLiquidState.Rising)
    {
      for (int index = 0; index < num1; ++index)
        this.SpawnTrailAuthority(currentTime, this.scienceExperimentManager.RiseProgressLinear);
    }
    else if (num1 < 0)
    {
      for (int index = 0; index > num1; --index)
        this.trailHeads.RemoveAt(0);
    }
    float num2 = this.trailSpawnRateVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailSpawnRateMultiplier;
    float num3 = this.trailBubbleBoundaryRadiusVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.surfaceRadiusSpawnRange.y;
    for (int index = this.trailHeads.Count - 1; index >= 0; --index)
    {
      if (currentTime - this.trailHeads[index].spawnTime > (double) num2)
      {
        float num4 = -this.trailMaxTurnAngle;
        float num5 = this.trailMaxTurnAngle;
        float num6 = Vector3.SignedAngle(this.trailHeads[index].direction, this.trailHeads[index].position - this.liquidSurfacePlane.transform.position, Vector3.up);
        float num7 = num3 - Vector3.Distance(this.trailHeads[index].position, this.liquidSurfacePlane.transform.position);
        if ((double) num7 < (double) this.trailEdgeAvoidanceSpawnsMinMax.x * (double) this.trailDistanceBetweenSpawns * (double) this.scaleFactor)
        {
          float num8 = Mathf.InverseLerp(this.trailEdgeAvoidanceSpawnsMinMax.x * this.trailDistanceBetweenSpawns * this.scaleFactor, this.trailEdgeAvoidanceSpawnsMinMax.y * this.trailDistanceBetweenSpawns * this.scaleFactor, num7);
          if ((double) num6 > 0.0)
          {
            float b = num6 - 90f * num8;
            num5 = Mathf.Min(num5, b);
            num4 = Mathf.Min(num4, num5 - this.trailMaxTurnAngle);
          }
          else
          {
            float b = num6 + 90f * num8;
            num4 = Mathf.Max(num4, b);
            num5 = Mathf.Max(num5, num4 + this.trailMaxTurnAngle);
          }
        }
        Vector3 direction = Quaternion.AngleAxis(Random.Range(num4, num5), Vector3.up) * this.trailHeads[index].direction;
        Vector3 vector3 = this.trailHeads[index].position + direction * this.trailDistanceBetweenSpawns * this.scaleFactor - this.liquidSurfacePlane.transform.position;
        if ((double) vector3.sqrMagnitude > (double) this.surfaceRadiusSpawnRange.y * (double) this.surfaceRadiusSpawnRange.y)
          vector3 = vector3.normalized * this.surfaceRadiusSpawnRange.y;
        Vector2 surfacePosLocal = new Vector2(vector3.x, vector3.z);
        float trailBubbleSize = this.trailBubbleSize;
        float lifetime = this.trailBubbleLifetimeVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailBubbleLifetimeMultiplier;
        this.trailHeads.RemoveAt(index);
        this.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, (object) surfacePosLocal, (object) trailBubbleSize, (object) lifetime, (object) currentTime);
        this.SpawnSodaBubbleLocal(surfacePosLocal, trailBubbleSize, lifetime, currentTime, true, direction);
      }
    }
  }

  private void SpawnRockAuthority(double currentTime, float lavaProgress)
  {
    if (!this.photonView.IsMine)
      return;
    float num1 = this.rockLifetimeMultiplierVsLavaProgress.Evaluate(lavaProgress);
    float num2 = this.rockMaxSizeMultiplierVsLavaProgress.Evaluate(lavaProgress);
    float lifetime = Random.Range(this.lifetimeRange.x, this.lifetimeRange.y) * num1;
    float spawnSize = Random.Range(this.sizeRange.x, this.sizeRange.y * num2);
    float num3 = this.spawnRadiusMultiplierVsLavaProgress.Evaluate(lavaProgress);
    Vector2 positionWithClearance = this.GetSpawnPositionWithClearance(Random.insideUnitCircle.normalized * Random.Range(this.surfaceRadiusSpawnRange.x, this.surfaceRadiusSpawnRange.y) * num3, spawnSize * this.scaleFactor, this.surfaceRadiusSpawnRange.y, this.liquidSurfacePlane.transform.position);
    this.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, (object) positionWithClearance, (object) spawnSize, (object) lifetime, (object) currentTime);
    this.SpawnSodaBubbleLocal(positionWithClearance, spawnSize, lifetime, currentTime);
  }

  private void SpawnTrailAuthority(double currentTime, float lavaProgress)
  {
    if (!this.photonView.IsMine)
      return;
    float lifetime = this.trailBubbleLifetimeVsProgress.Evaluate(this.scienceExperimentManager.RiseProgressLinear) * this.trailBubbleLifetimeMultiplier;
    float trailBubbleSize = this.trailBubbleSize;
    Vector2 positionWithClearance = this.GetSpawnPositionWithClearance(Random.insideUnitCircle.normalized * Random.Range(this.surfaceRadiusSpawnRange.x, this.surfaceRadiusSpawnRange.y), trailBubbleSize * this.scaleFactor, this.surfaceRadiusSpawnRange.y, this.liquidSurfacePlane.transform.position);
    Vector3 direction = Quaternion.AngleAxis(Random.Range(0.0f, 360f), Vector3.up) * Vector3.forward;
    this.photonView.RPC("SpawnSodaBubbleRPC", RpcTarget.Others, (object) positionWithClearance, (object) trailBubbleSize, (object) lifetime, (object) currentTime);
    this.SpawnSodaBubbleLocal(positionWithClearance, trailBubbleSize, lifetime, currentTime, true, direction);
  }

  private void SpawnSodaBubbleLocal(
    Vector2 surfacePosLocal,
    float spawnSize,
    float lifetime,
    double spawnTime,
    bool addAsTrail = false,
    Vector3 direction = default (Vector3))
  {
    if (this.activeBubbles.Count >= this.maxBubbleCount)
      return;
    Vector3 vector3 = this.liquidSurfacePlane.transform.position + new Vector3(surfacePosLocal.x, 0.0f, surfacePosLocal.y);
    ScienceExperimentPlatformGenerator.BubbleData bubbleData = new ScienceExperimentPlatformGenerator.BubbleData()
    {
      position = vector3,
      spawnSize = spawnSize,
      lifetime = lifetime,
      spawnTime = spawnTime,
      isTrail = false
    };
    bubbleData.bubble = ObjectPools.instance.Instantiate(this.spawnedPrefab, bubbleData.position, Quaternion.identity, 0.0f).GetComponent<SodaBubble>();
    if (this.photonView.IsMine & addAsTrail)
    {
      bubbleData.direction = direction;
      bubbleData.isTrail = true;
      this.trailHeads.Add(bubbleData);
    }
    this.activeBubbles.Add(bubbleData);
  }

  [PunRPC]
  public void SpawnSodaBubbleRPC(
    Vector2 surfacePosLocal,
    float spawnSize,
    float lifetime,
    double spawnTime,
    PhotonMessageInfo info)
  {
    GorillaNot.IncrementRPCCall(info, nameof (SpawnSodaBubbleRPC));
    if (info.Sender != PhotonNetwork.MasterClient || !float.IsFinite(spawnSize) || !float.IsFinite(lifetime) || !double.IsFinite(spawnTime))
      return;
    float time = Mathf.Clamp01(this.scienceExperimentManager.RiseProgressLinear);
    surfacePosLocal.ClampThisMagnitudeSafe(this.surfaceRadiusSpawnRange.y);
    spawnSize = Mathf.Clamp(spawnSize, this.sizeRange.x, this.sizeRange.y * this.rockMaxSizeMultiplierVsLavaProgress.Evaluate(time));
    lifetime = Mathf.Clamp(lifetime, this.lifetimeRange.x, this.lifetimeRange.y * this.rockLifetimeMultiplierVsLavaProgress.Evaluate(time));
    double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : Time.unscaledTimeAsDouble;
    spawnTime = (double) Mathf.Abs((float) (spawnTime - num)) < 10.0 ? spawnTime : num;
    this.SpawnSodaBubbleLocal(surfacePosLocal, spawnSize, lifetime, spawnTime);
  }

  private Vector2 GetSpawnPositionWithClearance(
    Vector2 inputPosition,
    float inputSize,
    float maxDistance,
    Vector3 lavaSurfaceOrigin)
  {
    Vector2 positionWithClearance = inputPosition;
    for (int index = 0; index < this.activeBubbles.Count; ++index)
    {
      Vector3 vector3 = this.activeBubbles[index].position - lavaSurfaceOrigin;
      Vector2 vector2_1 = new Vector2(vector3.x, vector3.z);
      Vector2 vector2_2 = positionWithClearance - vector2_1;
      float num = (float) (((double) inputSize + (double) this.activeBubbles[index].spawnSize * (double) this.scaleFactor) * 0.5);
      if ((double) vector2_2.sqrMagnitude < (double) num * (double) num)
      {
        float magnitude = vector2_2.magnitude;
        if ((double) magnitude > 1.0 / 1000.0)
        {
          Vector2 vector2_3 = vector2_2 / magnitude;
          positionWithClearance += vector2_3 * (num - magnitude);
          if ((double) positionWithClearance.sqrMagnitude > (double) maxDistance * (double) maxDistance)
            positionWithClearance = positionWithClearance.normalized * maxDistance;
        }
      }
    }
    if ((double) positionWithClearance.sqrMagnitude > (double) this.surfaceRadiusSpawnRange.y * (double) this.surfaceRadiusSpawnRange.y)
      positionWithClearance = positionWithClearance.normalized * this.surfaceRadiusSpawnRange.y;
    return positionWithClearance;
  }

  void IGuidedRefObject.GuidedRefInitialize()
  {
    GuidedRefHub.RegisterReceiverField<ScienceExperimentPlatformGenerator>(this, "liquidSurfacePlane", ref this.liquidSurfacePlane_gRef);
    GuidedRefHub.ReceiverFullyRegistered<ScienceExperimentPlatformGenerator>(this);
  }

  int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

  bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
  {
    return GuidedRefHub.TryResolveField<ScienceExperimentPlatformGenerator, Transform>(this, ref this.liquidSurfacePlane, this.liquidSurfacePlane_gRef, target);
  }

  void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
  {
    if (!this.enabled)
      return;
    TickSystem<object>.AddPostTickCallback((ITickSystemPost) this);
  }

  void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
  {
    TickSystem<object>.RemovePostTickCallback((ITickSystemPost) this);
  }

  Transform IGuidedRefMonoBehaviour.get_transform() => this.transform;

  int IGuidedRefObject.GetInstanceID() => this.GetInstanceID();

  private struct BubbleData
  {
    public Vector3 position;
    public Vector3 direction;
    public float spawnSize;
    public float lifetime;
    public double spawnTime;
    public bool isTrail;
    public SodaBubble bubble;
  }

  private struct BubbleSpawnDebug
  {
    public Vector3 initialPosition;
    public Vector3 initialDirection;
    public Vector3 spawnPosition;
    public float minAngle;
    public float maxAngle;
    public float edgeCorrectionAngle;
    public double spawnTime;
  }
}
