// Decompiled with JetBrains decompiler
// Type: GorillaTag.ContainerLiquid
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag;

[AddComponentMenu("GorillaTag/ContainerLiquid (GTag)")]
[ExecuteInEditMode]
public class ContainerLiquid : MonoBehaviour
{
  [Tooltip("Used to determine the world space bounds of the container.")]
  public MeshRenderer meshRenderer;
  [Tooltip("Used to determine the local space bounds of the container.")]
  public MeshFilter meshFilter;
  [Tooltip("If you are only using the liquid mesh to calculate the volume of the container and do not need visuals then set this to true.")]
  public bool keepMeshHidden;
  [Tooltip("The object that will float on top of the liquid.")]
  public Transform floater;
  public bool useLiquidShader = true;
  public bool useLiquidVolume;
  public Vector2 liquidVolumeMinMax = Vector2.up;
  public string liquidColorShaderPropertyName = "_BaseColor";
  public string liquidPlaneNormalShaderPropertyName = "_LiquidPlaneNormal";
  public string liquidPlanePositionShaderPropertyName = "_LiquidPlanePosition";
  [Tooltip("Emits drips when pouring.")]
  public ParticleSystem spillParticleSystem;
  [SoundBankInfo]
  public SoundBankPlayer emptySoundBankPlayer;
  [SoundBankInfo]
  public SoundBankPlayer refillSoundBankPlayer;
  [SoundBankInfo]
  public SoundBankPlayer spillSoundBankPlayer;
  public Color liquidColor = new Color(0.33f, 0.25f, 0.21f, 1f);
  [Tooltip("The amount of liquid currently in the container. This value is passed to the shader.")]
  [Range(0.0f, 1f)]
  public float fillAmount = 0.85f;
  [Tooltip("This is what fillAmount will be after automatic refilling.")]
  public float refillAmount = 0.85f;
  [Tooltip("Set to a negative value to disable.")]
  public float refillDelay = 10f;
  [Tooltip("The point that the liquid should be considered empty and should be auto refilled.")]
  public float refillThreshold = 0.1f;
  public float wobbleMax = 0.2f;
  public float wobbleFrequency = 1f;
  public float recovery = 1f;
  public float thickness = 1f;
  public float maxSpillRate = 100f;
  [DebugReadout]
  private bool wasEmptyLastFrame;
  private int liquidColorShaderProp;
  private int liquidPlaneNormalShaderProp;
  private int liquidPlanePositionShaderProp;
  private float refillTimer;
  private float lastSineWave;
  private float lastWobble;
  private Vector2 temporalWobbleAmp;
  private Vector3 lastPos;
  private Vector3 lastVelocity;
  private Vector3 lastAngularVelocity;
  private Quaternion lastRot;
  private MaterialPropertyBlock matPropBlock;
  private Bounds localMeshBounds;
  private bool useFloater;
  private Vector3[] topVerts;

  [DebugReadout]
  public bool isEmpty => (double) this.fillAmount <= (double) this.refillThreshold;

  public Vector3 cupTopWorldPos { get; private set; }

  public Vector3 bottomLipWorldPos { get; private set; }

  public Vector3 liquidPlaneWorldPos { get; private set; }

  public Vector3 liquidPlaneWorldNormal { get; private set; }

  protected bool IsValidLiquidSurfaceValues()
  {
    return (!((Object) this.meshRenderer != (Object) null) ? 0 : ((Object) this.meshFilter != (Object) null ? 1 : 0)) != 0 && (Object) this.spillParticleSystem != (Object) null && !string.IsNullOrEmpty(this.liquidColorShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlaneNormalShaderPropertyName) && !string.IsNullOrEmpty(this.liquidPlanePositionShaderPropertyName);
  }

  protected void InitializeLiquidSurface()
  {
    this.liquidColorShaderProp = Shader.PropertyToID(this.liquidColorShaderPropertyName);
    this.liquidPlaneNormalShaderProp = Shader.PropertyToID(this.liquidPlaneNormalShaderPropertyName);
    this.liquidPlanePositionShaderProp = Shader.PropertyToID(this.liquidPlanePositionShaderPropertyName);
    this.localMeshBounds = this.meshFilter.sharedMesh.bounds;
  }

  protected void InitializeParticleSystem()
  {
    this.spillParticleSystem.main.startColor = (ParticleSystem.MinMaxGradient) this.liquidColor;
  }

  protected void Awake()
  {
    this.matPropBlock = new MaterialPropertyBlock();
    this.topVerts = this.GetTopVerts();
  }

  protected void OnEnable()
  {
    if (!Application.isPlaying)
      return;
    this.enabled = this.useLiquidShader && this.IsValidLiquidSurfaceValues();
    if (this.enabled)
      this.InitializeLiquidSurface();
    this.InitializeParticleSystem();
    this.useFloater = (Object) this.floater != (Object) null;
  }

  protected void LateUpdate()
  {
    this.UpdateRefillTimer();
    Transform transform = this.transform;
    Vector3 position1 = transform.position;
    Quaternion rotation = transform.rotation;
    Bounds bounds = this.meshRenderer.bounds;
    this.liquidPlaneWorldPos = Vector3.Lerp(new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), new Vector3(bounds.center.x, bounds.max.y, bounds.center.z), this.fillAmount);
    Vector3 vector3_1 = transform.InverseTransformPoint(this.liquidPlaneWorldPos);
    float deltaTime = Time.deltaTime;
    this.temporalWobbleAmp = Vector2.Lerp(this.temporalWobbleAmp, Vector2.zero, deltaTime * this.recovery);
    float num1 = Mathf.Lerp(this.lastSineWave, Mathf.Sin(6.28318548f * this.wobbleFrequency * Time.realtimeSinceStartup), deltaTime * Mathf.Clamp(this.lastVelocity.magnitude + this.lastAngularVelocity.magnitude, this.thickness, 10f));
    Vector2 vector2 = this.temporalWobbleAmp * num1;
    this.liquidPlaneWorldNormal = new Vector3(vector2.x, -1f, vector2.y).normalized;
    Vector3 vector3_2 = transform.InverseTransformDirection(this.liquidPlaneWorldNormal);
    if (this.useLiquidShader)
    {
      this.matPropBlock.SetVector(this.liquidPlaneNormalShaderProp, (Vector4) vector3_2);
      this.matPropBlock.SetVector(this.liquidPlanePositionShaderProp, (Vector4) vector3_1);
      this.matPropBlock.SetVector(this.liquidColorShaderProp, (Vector4) this.liquidColor.linear);
      if (this.useLiquidVolume)
      {
        float num2 = MathUtils.Linear(this.fillAmount, 0.0f, 1f, this.liquidVolumeMinMax.x, this.liquidVolumeMinMax.y);
        this.matPropBlock.SetFloat(ShaderProps._LiquidFill, num2);
      }
      this.meshRenderer.SetPropertyBlock(this.matPropBlock);
    }
    if (this.useFloater)
      this.floater.localPosition = this.floater.localPosition.WithY(Mathf.Lerp(this.localMeshBounds.min.y, this.localMeshBounds.max.y, this.fillAmount));
    Vector3 vector3_3 = (this.lastPos - position1) / deltaTime;
    Vector3 angularVelocity = GorillaMath.GetAngularVelocity(this.lastRot, rotation);
    this.temporalWobbleAmp.x += Mathf.Clamp((vector3_3.x + vector3_3.y * 0.2f + angularVelocity.z + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
    this.temporalWobbleAmp.y += Mathf.Clamp((vector3_3.z + vector3_3.y * 0.2f + angularVelocity.x + angularVelocity.y) * this.wobbleMax, -this.wobbleMax, this.wobbleMax);
    this.lastPos = position1;
    this.lastRot = rotation;
    this.lastSineWave = num1;
    this.lastVelocity = vector3_3;
    this.lastAngularVelocity = angularVelocity;
    this.meshRenderer.enabled = !this.keepMeshHidden && !this.isEmpty;
    float num3 = this.localMeshBounds.extents.x * transform.lossyScale.x;
    Vector3 position2 = this.localMeshBounds.center + new Vector3(0.0f, this.localMeshBounds.extents.y, 0.0f);
    this.cupTopWorldPos = transform.TransformPoint(position2);
    Vector3 up = transform.up;
    Vector3 rhs = transform.InverseTransformDirection(Vector3.down);
    float num4 = float.MinValue;
    Vector3 position3 = Vector3.zero;
    for (int index = 0; index < this.topVerts.Length; ++index)
    {
      float num5 = Vector3.Dot(this.topVerts[index], rhs);
      if ((double) num5 > (double) num4)
      {
        num4 = num5;
        position3 = this.topVerts[index];
      }
    }
    this.bottomLipWorldPos = transform.TransformPoint(position3);
    float t = Mathf.Clamp01((float) (((double) this.liquidPlaneWorldPos.y - (double) this.bottomLipWorldPos.y) / ((double) num3 * 2.0)));
    bool flag = (double) t > 9.9999997473787516E-06;
    ParticleSystem.EmissionModule emission = this.spillParticleSystem.emission with
    {
      enabled = flag
    };
    if (flag)
    {
      if (!this.spillSoundBankPlayer.isPlaying)
        this.spillSoundBankPlayer.Play();
      this.spillParticleSystem.transform.position = Vector3.Lerp(this.bottomLipWorldPos, this.cupTopWorldPos, t);
      this.spillParticleSystem.shape.radius = num3 * t;
      ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
      float num6 = t * this.maxSpillRate;
      rateOverTime.constant = num6;
      emission.rateOverTime = rateOverTime;
      this.fillAmount -= (float) ((double) num6 * (double) deltaTime * 0.0099999997764825821);
    }
    if (this.isEmpty && !this.wasEmptyLastFrame && !this.emptySoundBankPlayer.isPlaying)
      this.emptySoundBankPlayer.Play();
    else if (!this.isEmpty && this.wasEmptyLastFrame && !this.refillSoundBankPlayer.isPlaying)
      this.refillSoundBankPlayer.Play();
    this.wasEmptyLastFrame = this.isEmpty;
  }

  public void UpdateRefillTimer()
  {
    if ((double) this.refillDelay < 0.0 || !this.isEmpty)
      return;
    if ((double) this.refillTimer < 0.0)
    {
      this.refillTimer = this.refillDelay;
      this.fillAmount = this.refillAmount;
    }
    else
      this.refillTimer -= Time.deltaTime;
  }

  private Vector3[] GetTopVerts()
  {
    Vector3[] vertices = this.meshFilter.sharedMesh.vertices;
    List<Vector3> vector3List = new List<Vector3>(vertices.Length);
    float num = float.MinValue;
    foreach (Vector3 vector3 in vertices)
    {
      if ((double) vector3.y > (double) num)
        num = vector3.y;
    }
    foreach (Vector3 vector3 in vertices)
    {
      if ((double) Mathf.Abs(vector3.y - num) < 1.0 / 1000.0)
        vector3List.Add(vector3);
    }
    return vector3List.ToArray();
  }
}
