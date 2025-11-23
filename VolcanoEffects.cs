// Decompiled with JetBrains decompiler
// Type: GorillaTag.VolcanoEffects
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaTag.GuidedRefs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag;

public class VolcanoEffects : BaseGuidedRefTargetMono
{
  [Tooltip("Only one VolcanoEffects should change shader globals in the scene (lava color, lava light) at a time.")]
  [SerializeField]
  private bool applyShaderGlobals = true;
  [Tooltip("Game trigger notification sounds will play through this.")]
  [SerializeField]
  private AudioSource forestSpeakerAudioSrc;
  [Tooltip("The accumulator value of rocks being thrown into the volcano has been reset.")]
  [SerializeField]
  private AudioClip warnVolcanoBellyEmptied;
  [Tooltip("Accept stone sounds will play through here.")]
  [SerializeField]
  private AudioSource volcanoAudioSource;
  [Tooltip("volcano ate rock but needs more.")]
  [SerializeField]
  private AudioClip volcanoAcceptStone;
  [Tooltip("volcano ate last needed rock.")]
  [SerializeField]
  private AudioClip volcanoAcceptLastStone;
  [Tooltip("This will be faded in while lava is rising.")]
  [SerializeField]
  private AudioSource[] lavaSurfaceAudioSrcs;
  [Tooltip("Emission will be adjusted for these particles during eruption.")]
  [SerializeField]
  private ParticleSystem[] lavaSpewParticleSystems;
  [Tooltip("Smoke emits during all states but it's intensity and color will change when erupting/idling.")]
  [SerializeField]
  private ParticleSystem[] smokeParticleSystems;
  [SerializeField]
  private VolcanoEffects.LavaStateFX drainedStateFX;
  [SerializeField]
  private VolcanoEffects.LavaStateFX eruptingStateFX;
  [SerializeField]
  private VolcanoEffects.LavaStateFX risingStateFX;
  [SerializeField]
  private VolcanoEffects.LavaStateFX fullStateFX;
  [SerializeField]
  private VolcanoEffects.LavaStateFX drainingStateFX;
  private VolcanoEffects.LavaStateFX currentStateFX;
  private ParticleSystem.EmissionModule[] lavaSpewEmissionModules;
  private float[] lavaSpewEmissionDefaultRateMultipliers;
  private ParticleSystem.Burst[][] lavaSpewDefaultEmitBursts;
  private ParticleSystem.Burst[][] lavaSpewAdjustedEmitBursts;
  private ParticleSystem.MainModule[] smokeMainModules;
  private ParticleSystem.EmissionModule[] smokeEmissionModules;
  private float[] smokeEmissionDefaultRateMultipliers;
  private int shaderProp_ZoneLiquidLightColor = Shader.PropertyToID("_ZoneLiquidLightColor");
  private int shaderProp_ZoneLiquidLightDistScale = Shader.PropertyToID("_ZoneLiquidLightDistScale");
  private float timeVolcanoBellyWasLastEmpty;
  private bool hasVolcanoAudioSrc;
  private bool hasForestSpeakerAudioSrc;

  protected override void Awake()
  {
    base.Awake();
    if (this.RemoveNullsFromArray<ParticleSystem>(ref this.lavaSpewParticleSystems))
      this.LogNullsFoundInArray("lavaSpewParticleSystems");
    if (this.RemoveNullsFromArray<ParticleSystem>(ref this.smokeParticleSystems))
      this.LogNullsFoundInArray("smokeParticleSystems");
    this.hasVolcanoAudioSrc = (UnityEngine.Object) this.volcanoAudioSource != (UnityEngine.Object) null;
    this.hasForestSpeakerAudioSrc = (UnityEngine.Object) this.forestSpeakerAudioSrc != (UnityEngine.Object) null;
    this.lavaSpewEmissionModules = new ParticleSystem.EmissionModule[this.lavaSpewParticleSystems.Length];
    this.lavaSpewEmissionDefaultRateMultipliers = new float[this.lavaSpewParticleSystems.Length];
    this.lavaSpewDefaultEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
    this.lavaSpewAdjustedEmitBursts = new ParticleSystem.Burst[this.lavaSpewParticleSystems.Length][];
    for (int index1 = 0; index1 < this.lavaSpewParticleSystems.Length; ++index1)
    {
      ParticleSystem.EmissionModule emission = this.lavaSpewParticleSystems[index1].emission;
      this.lavaSpewEmissionDefaultRateMultipliers[index1] = emission.rateOverTimeMultiplier;
      this.lavaSpewDefaultEmitBursts[index1] = new ParticleSystem.Burst[emission.burstCount];
      this.lavaSpewAdjustedEmitBursts[index1] = new ParticleSystem.Burst[emission.burstCount];
      for (int index2 = 0; index2 < emission.burstCount; ++index2)
      {
        ParticleSystem.Burst burst = emission.GetBurst(index2);
        this.lavaSpewDefaultEmitBursts[index1][index2] = burst;
        this.lavaSpewAdjustedEmitBursts[index1][index2] = new ParticleSystem.Burst(burst.time, burst.minCount, burst.maxCount, burst.cycleCount, burst.repeatInterval);
        this.lavaSpewAdjustedEmitBursts[index1][index2].count = burst.count;
      }
      this.lavaSpewEmissionModules[index1] = emission;
    }
    this.smokeMainModules = new ParticleSystem.MainModule[this.smokeParticleSystems.Length];
    this.smokeEmissionModules = new ParticleSystem.EmissionModule[this.smokeParticleSystems.Length];
    this.smokeEmissionDefaultRateMultipliers = new float[this.smokeParticleSystems.Length];
    for (int index = 0; index < this.smokeParticleSystems.Length; ++index)
    {
      this.smokeMainModules[index] = this.smokeParticleSystems[index].main;
      this.smokeEmissionModules[index] = this.smokeParticleSystems[index].emission;
      this.smokeEmissionDefaultRateMultipliers[index] = this.smokeEmissionModules[index].rateOverTimeMultiplier;
    }
    this.InitState(this.drainedStateFX);
    this.InitState(this.eruptingStateFX);
    this.InitState(this.risingStateFX);
    this.InitState(this.fullStateFX);
    this.InitState(this.drainingStateFX);
    this.currentStateFX = this.drainedStateFX;
    this.UpdateDrainedState(0.0f);
  }

  public void OnVolcanoBellyEmpty()
  {
    if (!this.hasForestSpeakerAudioSrc || (double) Time.time - (double) this.timeVolcanoBellyWasLastEmpty < (double) this.warnVolcanoBellyEmptied.length)
      return;
    this.forestSpeakerAudioSrc.gameObject.SetActive(true);
    this.forestSpeakerAudioSrc.GTPlayOneShot(this.warnVolcanoBellyEmptied);
  }

  public void OnStoneAccepted(double activationProgress)
  {
    if (!this.hasVolcanoAudioSrc)
      return;
    this.volcanoAudioSource.gameObject.SetActive(true);
    if (activationProgress > 1.0)
      this.volcanoAudioSource.GTPlayOneShot(this.volcanoAcceptLastStone);
    else
      this.volcanoAudioSource.GTPlayOneShot(this.volcanoAcceptStone);
  }

  private void InitState(VolcanoEffects.LavaStateFX fx)
  {
    fx.startSoundExists = (UnityEngine.Object) fx.startSound != (UnityEngine.Object) null;
    fx.endSoundExists = (UnityEngine.Object) fx.endSound != (UnityEngine.Object) null;
    fx.loop1Exists = (UnityEngine.Object) fx.loop1AudioSrc != (UnityEngine.Object) null;
    fx.loop2Exists = (UnityEngine.Object) fx.loop2AudioSrc != (UnityEngine.Object) null;
    if (fx.loop1Exists)
    {
      fx.loop1DefaultVolume = fx.loop1AudioSrc.volume;
      fx.loop1AudioSrc.volume = 0.0f;
    }
    if (!fx.loop2Exists)
      return;
    fx.loop2DefaultVolume = fx.loop2AudioSrc.volume;
    fx.loop2AudioSrc.volume = 0.0f;
  }

  private void SetLavaAudioEnabled(bool toEnable)
  {
    foreach (Component lavaSurfaceAudioSrc in this.lavaSurfaceAudioSrcs)
      lavaSurfaceAudioSrc.gameObject.SetActive(toEnable);
  }

  private void SetLavaAudioEnabled(bool toEnable, float volume)
  {
    foreach (AudioSource lavaSurfaceAudioSrc in this.lavaSurfaceAudioSrcs)
    {
      lavaSurfaceAudioSrc.volume = volume;
      lavaSurfaceAudioSrc.gameObject.SetActive(toEnable);
    }
  }

  private void ResetState()
  {
    if (this.currentStateFX == null)
      return;
    this.currentStateFX.startSoundPlayed = false;
    this.currentStateFX.endSoundPlayed = false;
    if (this.currentStateFX.startSoundExists)
      this.currentStateFX.startSoundAudioSrc.gameObject.SetActive(false);
    if (this.currentStateFX.endSoundExists)
      this.currentStateFX.endSoundAudioSrc.gameObject.SetActive(false);
    if (this.currentStateFX.loop1Exists)
      this.currentStateFX.loop1AudioSrc.gameObject.SetActive(false);
    if (!this.currentStateFX.loop2Exists)
      return;
    this.currentStateFX.loop2AudioSrc.gameObject.SetActive(false);
  }

  private void UpdateState(float time, float timeRemaining, float progress)
  {
    if (this.currentStateFX == null)
      return;
    if (this.currentStateFX.startSoundExists && !this.currentStateFX.startSoundPlayed && (double) time >= (double) this.currentStateFX.startSoundDelay)
    {
      this.currentStateFX.startSoundPlayed = true;
      this.currentStateFX.startSoundAudioSrc.gameObject.SetActive(true);
      this.currentStateFX.startSoundAudioSrc.GTPlayOneShot(this.currentStateFX.startSound, this.currentStateFX.startSoundVol);
    }
    if (this.currentStateFX.endSoundExists && !this.currentStateFX.endSoundPlayed && (double) timeRemaining <= (double) this.currentStateFX.endSound.length + (double) this.currentStateFX.endSoundPadTime)
    {
      this.currentStateFX.endSoundPlayed = true;
      this.currentStateFX.endSoundAudioSrc.gameObject.SetActive(true);
      this.currentStateFX.endSoundAudioSrc.GTPlayOneShot(this.currentStateFX.endSound, this.currentStateFX.endSoundVol);
    }
    if (this.currentStateFX.loop1Exists)
    {
      this.currentStateFX.loop1AudioSrc.volume = this.currentStateFX.loop1VolAnim.Evaluate(progress) * this.currentStateFX.loop1DefaultVolume;
      if (!this.currentStateFX.loop1AudioSrc.isPlaying)
      {
        this.currentStateFX.loop1AudioSrc.gameObject.SetActive(true);
        this.currentStateFX.loop1AudioSrc.GTPlay();
      }
    }
    if (this.currentStateFX.loop2Exists)
    {
      this.currentStateFX.loop2AudioSrc.volume = this.currentStateFX.loop2VolAnim.Evaluate(progress) * this.currentStateFX.loop2DefaultVolume;
      if (!this.currentStateFX.loop2AudioSrc.isPlaying)
      {
        this.currentStateFX.loop2AudioSrc.gameObject.SetActive(true);
        this.currentStateFX.loop2AudioSrc.GTPlay();
      }
    }
    for (int index = 0; index < this.smokeMainModules.Length; ++index)
    {
      this.smokeMainModules[index].startColor = (ParticleSystem.MinMaxGradient) this.currentStateFX.smokeStartColorAnim.Evaluate(progress);
      this.smokeEmissionModules[index].rateOverTimeMultiplier = this.currentStateFX.smokeEmissionAnim.Evaluate(progress) * this.smokeEmissionDefaultRateMultipliers[index];
    }
    this.SetParticleEmissionRateAndBurst(this.currentStateFX.lavaSpewEmissionAnim.Evaluate(progress), this.lavaSpewEmissionModules, this.lavaSpewEmissionDefaultRateMultipliers, this.lavaSpewDefaultEmitBursts, this.lavaSpewAdjustedEmitBursts);
    if (!this.applyShaderGlobals)
      return;
    Shader.SetGlobalColor(this.shaderProp_ZoneLiquidLightColor, this.currentStateFX.lavaLightColor.Evaluate(progress) * this.currentStateFX.lavaLightIntensityAnim.Evaluate(progress));
    Shader.SetGlobalFloat(this.shaderProp_ZoneLiquidLightDistScale, this.currentStateFX.lavaLightAttenuationAnim.Evaluate(progress));
  }

  public void SetDrainedState()
  {
    this.ResetState();
    this.SetLavaAudioEnabled(false);
    this.currentStateFX = this.drainedStateFX;
  }

  public void UpdateDrainedState(float time)
  {
    this.ResetState();
    this.UpdateState(time, float.MaxValue, float.MinValue);
  }

  public void SetEruptingState()
  {
    this.ResetState();
    this.SetLavaAudioEnabled(false, 0.0f);
    this.currentStateFX = this.eruptingStateFX;
  }

  public void UpdateEruptingState(float time, float timeRemaining, float progress)
  {
    this.UpdateState(time, timeRemaining, progress);
  }

  public void SetRisingState()
  {
    this.ResetState();
    this.SetLavaAudioEnabled(true, 0.0f);
    this.currentStateFX = this.risingStateFX;
  }

  public void UpdateRisingState(float time, float timeRemaining, float progress)
  {
    this.UpdateState(time, timeRemaining, progress);
    foreach (AudioSource lavaSurfaceAudioSrc in this.lavaSurfaceAudioSrcs)
      lavaSurfaceAudioSrc.volume = Mathf.Lerp(0.0f, 1f, Mathf.Clamp01(time));
  }

  public void SetFullState()
  {
    this.ResetState();
    this.SetLavaAudioEnabled(true, 1f);
    this.currentStateFX = this.fullStateFX;
  }

  public void UpdateFullState(float time, float timeRemaining, float progress)
  {
    this.UpdateState(time, timeRemaining, progress);
  }

  public void SetDrainingState()
  {
    this.ResetState();
    this.SetLavaAudioEnabled(true, 1f);
    this.currentStateFX = this.drainingStateFX;
  }

  public void UpdateDrainingState(float time, float timeRemaining, float progress)
  {
    this.UpdateState(time, timeRemaining, progress);
    foreach (AudioSource lavaSurfaceAudioSrc in this.lavaSurfaceAudioSrcs)
      lavaSurfaceAudioSrc.volume = Mathf.Lerp(1f, 0.0f, progress);
  }

  private void SetParticleEmissionRateAndBurst(
    float multiplier,
    ParticleSystem.EmissionModule[] emissionModules,
    float[] defaultRateMultipliers,
    ParticleSystem.Burst[][] defaultEmitBursts,
    ParticleSystem.Burst[][] adjustedEmitBursts)
  {
    for (int index1 = 0; index1 < emissionModules.Length; ++index1)
    {
      emissionModules[index1].rateOverTimeMultiplier = multiplier * defaultRateMultipliers[index1];
      int num = Mathf.Min(emissionModules[index1].burstCount, defaultEmitBursts[index1].Length);
      for (int index2 = 0; index2 < num; ++index2)
        adjustedEmitBursts[index1][index2].probability = defaultEmitBursts[index1][index2].probability * multiplier;
      emissionModules[index1].SetBursts(adjustedEmitBursts[index1]);
    }
  }

  private bool RemoveNullsFromArray<T>(ref T[] array) where T : UnityEngine.Object
  {
    List<T> objList = new List<T>(array.Length);
    foreach (T obj in array)
    {
      if ((UnityEngine.Object) obj != (UnityEngine.Object) null)
        objList.Add(obj);
    }
    int length1 = array.Length;
    array = objList.ToArray();
    int length2 = array.Length;
    return length1 != length2;
  }

  private void LogNullsFoundInArray(string nameOfArray)
  {
    Debug.LogError((object) $"Null reference found in {nameOfArray} array of component: \"{this.GetComponentPath<VolcanoEffects>()}\"", (UnityEngine.Object) this);
  }

  [Serializable]
  public class LavaStateFX
  {
    public AudioClip startSound;
    public AudioSource startSoundAudioSrc;
    [Tooltip("Multiplied by the AudioSource's volume.")]
    public float startSoundVol = 1f;
    [FormerlySerializedAs("startSoundPad")]
    public float startSoundDelay;
    public AudioClip endSound;
    public AudioSource endSoundAudioSrc;
    [Tooltip("Multiplied by the AudioSource's volume.")]
    public float endSoundVol = 1f;
    [Tooltip("How much time should there be between the end of the clip playing and the end of the state.")]
    public float endSoundPadTime;
    public AudioSource loop1AudioSrc;
    public AnimationCurve loop1VolAnim;
    public AudioSource loop2AudioSrc;
    public AnimationCurve loop2VolAnim;
    public AnimationCurve lavaSpewEmissionAnim;
    public AnimationCurve smokeEmissionAnim;
    public Gradient smokeStartColorAnim;
    public Gradient lavaLightColor;
    public AnimationCurve lavaLightIntensityAnim = AnimationCurve.Constant(0.0f, 1f, 60f);
    public AnimationCurve lavaLightAttenuationAnim = AnimationCurve.Constant(0.0f, 1f, 0.1f);
    [NonSerialized]
    public bool startSoundExists;
    [NonSerialized]
    public bool startSoundPlayed;
    [NonSerialized]
    public bool endSoundExists;
    [NonSerialized]
    public bool endSoundPlayed;
    [NonSerialized]
    public bool loop1Exists;
    [NonSerialized]
    public float loop1DefaultVolume;
    [NonSerialized]
    public bool loop2Exists;
    [NonSerialized]
    public float loop2DefaultVolume;
  }
}
