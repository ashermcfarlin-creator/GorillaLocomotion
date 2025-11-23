// Decompiled with JetBrains decompiler
// Type: GorillaTag.Reactions.ShakeReaction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.Reactions;

public class ShakeReaction : MonoBehaviour, ITickSystemPost
{
  [SerializeField]
  private Transform shakeXform;
  [SerializeField]
  private float velocityThreshold = 5f;
  [SerializeField]
  private SoundBankPlayer shakeSoundBankPlayer;
  [SerializeField]
  private float shakeSoundCooldown = 1f;
  [SerializeField]
  private AudioSource loopSoundAudioSource;
  [SerializeField]
  private float loopSoundBaseVolume = 1f;
  [SerializeField]
  private float loopSoundSustainDuration = 1f;
  [SerializeField]
  private float loopSoundFadeInDuration = 1f;
  [SerializeField]
  private AnimationCurve loopSoundFadeInCurve;
  [SerializeField]
  private float loopSoundFadeOutDuration = 1f;
  [SerializeField]
  private AnimationCurve loopSoundFadeOutCurve;
  [SerializeField]
  private ParticleSystem particles;
  [SerializeField]
  private AnimationCurve emissionCurve;
  [SerializeField]
  private float particleDuration = 5f;
  private const int sampleHistorySize = 256 /*0x0100*/;
  private float[] sampleHistoryTime;
  private Vector3[] sampleHistoryPos;
  private Vector3[] sampleHistoryVel;
  private int currentIndex;
  private float lastShakeSoundTime = float.MinValue;
  private float lastShakeTime = float.MinValue;
  private float maxEmissionRate;
  private bool hasLoopSound;
  private bool hasShakeSound;
  private bool hasParticleSystem;
  [DebugReadout]
  private float poopVelocity;

  private float loopSoundTotalDuration
  {
    get
    {
      return this.loopSoundFadeInDuration + this.loopSoundSustainDuration + this.loopSoundFadeOutDuration;
    }
  }

  bool ITickSystemPost.PostTickRunning { get; set; }

  protected void Awake()
  {
    this.sampleHistoryPos = new Vector3[256 /*0x0100*/];
    this.sampleHistoryTime = new float[256 /*0x0100*/];
    this.sampleHistoryVel = new Vector3[256 /*0x0100*/];
    if ((UnityEngine.Object) this.particles != (UnityEngine.Object) null)
      this.maxEmissionRate = this.particles.emission.rateOverTime.constant;
    Application.quitting += new Action(this.HandleApplicationQuitting);
  }

  protected void OnEnable()
  {
    float unscaledTime = Time.unscaledTime;
    Vector3 position = this.shakeXform.position;
    for (int index = 0; index < 256 /*0x0100*/; ++index)
    {
      this.sampleHistoryTime[index] = unscaledTime;
      this.sampleHistoryPos[index] = position;
      this.sampleHistoryVel[index] = Vector3.zero;
    }
    if ((UnityEngine.Object) this.loopSoundAudioSource != (UnityEngine.Object) null)
    {
      this.loopSoundAudioSource.loop = true;
      this.loopSoundAudioSource.GTPlay();
    }
    this.hasLoopSound = (UnityEngine.Object) this.loopSoundAudioSource != (UnityEngine.Object) null;
    this.hasShakeSound = (UnityEngine.Object) this.shakeSoundBankPlayer != (UnityEngine.Object) null;
    this.hasParticleSystem = (UnityEngine.Object) this.particles != (UnityEngine.Object) null;
    TickSystem<object>.AddPostTickCallback((ITickSystemPost) this);
  }

  protected void OnDisable()
  {
    if ((UnityEngine.Object) this.loopSoundAudioSource != (UnityEngine.Object) null)
      this.loopSoundAudioSource.GTStop();
    TickSystem<object>.RemovePostTickCallback((ITickSystemPost) this);
  }

  private void HandleApplicationQuitting()
  {
    TickSystem<object>.RemovePostTickCallback((ITickSystemPost) this);
  }

  void ITickSystemPost.PostTick()
  {
    float unscaledTime = Time.unscaledTime;
    Vector3 position = this.shakeXform.position;
    int index = (this.currentIndex - 1 + 256 /*0x0100*/) % 256 /*0x0100*/;
    this.currentIndex = (this.currentIndex + 1) % 256 /*0x0100*/;
    this.sampleHistoryTime[this.currentIndex] = unscaledTime;
    float num1 = unscaledTime - this.sampleHistoryTime[index];
    this.sampleHistoryPos[this.currentIndex] = position;
    this.sampleHistoryVel[this.currentIndex] = (double) num1 <= 0.0 ? Vector3.zero : (position - this.sampleHistoryPos[index]) / num1;
    float sqrMagnitude = (this.sampleHistoryVel[index] - this.sampleHistoryVel[this.currentIndex]).sqrMagnitude;
    this.poopVelocity = Mathf.Round(Mathf.Sqrt(sqrMagnitude) * 1000f) / 1000f;
    float num2 = this.shakeXform.lossyScale.x * this.velocityThreshold * this.velocityThreshold;
    if ((double) sqrMagnitude >= (double) num2)
      this.lastShakeTime = unscaledTime;
    float num3 = unscaledTime - this.lastShakeTime;
    float time = Mathf.Clamp01(num3 / this.particleDuration);
    if (this.hasParticleSystem)
      this.particles.emission.rateOverTime = (ParticleSystem.MinMaxCurve) (this.emissionCurve.Evaluate(time) * this.maxEmissionRate);
    if (this.hasShakeSound && (double) this.lastShakeTime - (double) this.lastShakeSoundTime > (double) this.shakeSoundCooldown)
    {
      this.shakeSoundBankPlayer.Play();
      this.lastShakeSoundTime = unscaledTime;
    }
    if (!this.hasLoopSound)
      return;
    if ((double) num3 < (double) this.loopSoundFadeInDuration)
      this.loopSoundAudioSource.volume = this.loopSoundBaseVolume * this.loopSoundFadeInCurve.Evaluate(Mathf.Clamp01(num3 / this.loopSoundFadeInDuration));
    else if ((double) num3 < (double) this.loopSoundFadeInDuration + (double) this.loopSoundSustainDuration)
      this.loopSoundAudioSource.volume = this.loopSoundBaseVolume;
    else
      this.loopSoundAudioSource.volume = this.loopSoundBaseVolume * this.loopSoundFadeOutCurve.Evaluate(Mathf.Clamp01((num3 - this.loopSoundFadeInDuration - this.loopSoundSustainDuration) / this.loopSoundFadeOutDuration));
  }
}
