// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.DrillFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag.Cosmetics;

public class DrillFX : MonoBehaviour
{
  [SerializeField]
  private ParticleSystem fx;
  [SerializeField]
  private AnimationCurve fxEmissionCurve;
  [SerializeField]
  private float fxMinRadiusScale = 0.01f;
  [Tooltip("Right click menu has custom menu items. Anything starting with \"- \" is custom.")]
  [SerializeField]
  private AudioSource loopAudio;
  [SerializeField]
  private AnimationCurve loopAudioVolumeCurve;
  [Tooltip("Higher value makes it reach the target volume faster.")]
  [SerializeField]
  private float loopAudioVolumeTransitionSpeed = 3f;
  [FormerlySerializedAs("layerMask")]
  [Tooltip("The collision layers the line cast should intersect with")]
  [SerializeField]
  private LayerMask lineCastLayerMask;
  [Tooltip("The position in local space that the line cast starts.")]
  [SerializeField]
  private Vector3 lineCastStart = Vector3.zero;
  [Tooltip("The position in local space that the line cast ends.")]
  [SerializeField]
  private Vector3 lineCastEnd = Vector3.forward;
  private static bool appIsQuitting;
  private static bool appIsQuittingHandlerIsSubscribed;
  private float maxDepth;
  private bool hasFX;
  private ParticleSystem.EmissionModule fxEmissionModule;
  private float fxEmissionMaxRate;
  private ParticleSystem.ShapeModule fxShapeModule;
  private float fxShapeMaxRadius;
  private bool hasAudio;
  private float audioMaxVolume;

  protected void Awake()
  {
    if (!DrillFX.appIsQuittingHandlerIsSubscribed)
    {
      DrillFX.appIsQuittingHandlerIsSubscribed = true;
      Application.quitting += new Action(DrillFX.HandleApplicationQuitting);
    }
    this.hasFX = (UnityEngine.Object) this.fx != (UnityEngine.Object) null;
    if (this.hasFX)
    {
      this.fxEmissionModule = this.fx.emission;
      this.fxEmissionMaxRate = this.fxEmissionModule.rateOverTimeMultiplier;
      this.fxShapeModule = this.fx.shape;
      this.fxShapeMaxRadius = this.fxShapeModule.radius;
    }
    this.hasAudio = (UnityEngine.Object) this.loopAudio != (UnityEngine.Object) null;
    if (!this.hasAudio)
      return;
    this.audioMaxVolume = this.loopAudio.volume;
    this.loopAudio.volume = 0.0f;
    this.loopAudio.loop = true;
    this.loopAudio.GTPlay();
  }

  protected void OnEnable()
  {
    if (DrillFX.appIsQuitting)
      return;
    if (this.hasFX)
      this.fxEmissionModule.rateOverTimeMultiplier = 0.0f;
    if (this.hasAudio)
    {
      this.loopAudio.volume = 0.0f;
      this.loopAudio.loop = true;
      this.loopAudio.GTPlay();
    }
    this.ValidateLineCastPositions();
  }

  protected void OnDisable()
  {
    if (DrillFX.appIsQuitting)
      return;
    if (this.hasFX)
      this.fxEmissionModule.rateOverTimeMultiplier = 0.0f;
    if (!this.hasAudio)
      return;
    this.loopAudio.volume = 0.0f;
    this.loopAudio.GTStop();
  }

  protected void LateUpdate()
  {
    if (DrillFX.appIsQuitting)
      return;
    Transform transform = this.transform;
    UnityEngine.RaycastHit hitInfo;
    Vector3 position = Physics.Linecast(transform.TransformPoint(this.lineCastStart), transform.TransformPoint(this.lineCastEnd), out hitInfo, (int) this.lineCastLayerMask, QueryTriggerInteraction.Ignore) ? hitInfo.point : this.lineCastEnd;
    Vector3 b = transform.InverseTransformPoint(position);
    float num = Mathf.Clamp01(Vector3.Distance(this.lineCastStart, b) / this.maxDepth);
    if (this.hasFX)
    {
      this.fxEmissionModule.rateOverTimeMultiplier = this.fxEmissionMaxRate * this.fxEmissionCurve.Evaluate(num);
      this.fxShapeModule.position = b;
      this.fxShapeModule.radius = Mathf.Lerp(this.fxShapeMaxRadius, this.fxMinRadiusScale * this.fxShapeMaxRadius, num);
    }
    if (!this.hasAudio)
      return;
    this.loopAudio.volume = Mathf.MoveTowards(this.loopAudio.volume, this.audioMaxVolume * this.loopAudioVolumeCurve.Evaluate(num), this.loopAudioVolumeTransitionSpeed * Time.deltaTime);
  }

  private static void HandleApplicationQuitting() => DrillFX.appIsQuitting = true;

  private bool ValidateLineCastPositions()
  {
    this.maxDepth = Vector3.Distance(this.lineCastStart, this.lineCastEnd);
    if ((double) this.maxDepth > 1.4012984643248171E-45)
      return true;
    if (Application.isPlaying)
    {
      Debug.Log((object) "DrillFX: lineCastStart and End are too close together. Disabling component.", (UnityEngine.Object) this);
      this.enabled = false;
    }
    return false;
  }
}
