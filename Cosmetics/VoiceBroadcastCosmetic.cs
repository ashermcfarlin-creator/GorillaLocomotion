// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.VoiceBroadcastCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTag.Audio;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

[RequireComponent(typeof (LoudSpeakerActivator))]
public class VoiceBroadcastCosmetic : MonoBehaviour, IGorillaSliceableSimple
{
  public TalkingCosmeticType talkingCosmeticType;
  [Tooltip("How loud the Gorilla voice should be before detecting as talking.")]
  [SerializeField]
  public float minVolume = 0.1f;
  [Tooltip("How long the initial speaking section needs to last to trigger the talking animation.")]
  [SerializeField]
  public float minSpeakingTime = 0.15f;
  [SerializeField]
  private Animation simpleAnimation;
  [SerializeField]
  private string talkAnimationTriggerName;
  private int talkAnimationTrigger;
  private const string EVENTS = "Events";
  [SerializeField]
  private UnityEvent onStartListening;
  [SerializeField]
  private UnityEvent onStartSpeaking;
  [SerializeField]
  private UnityEvent onStopSpeaking;
  [SerializeField]
  private UnityEvent onStopListening;
  private float speakingTime;
  private bool isListening;
  private bool isSpeaking;
  private VoiceBroadcastCosmeticWearable wearable;
  private LoudSpeakerActivator loudSpeaker;
  private GorillaSpeakerLoudness gsl;
  private Animator animator;
  private float lastSliceUpdateTime;

  private void Awake()
  {
    this.loudSpeaker = this.GetComponent<LoudSpeakerActivator>();
    this.animator = this.GetComponent<Animator>();
    this.talkAnimationTrigger = Animator.StringToHash(this.talkAnimationTriggerName);
    this.gsl = this.GetComponentInParent<GorillaSpeakerLoudness>();
  }

  public void SetWearable(VoiceBroadcastCosmeticWearable wearable) => this.wearable = wearable;

  private void StartBroadcast()
  {
    this.loudSpeaker.StartLocalBroadcast();
    this.onStartListening?.Invoke();
    this.wearable.OnCosmeticStartListening();
    this.lastSliceUpdateTime = Time.time;
    GorillaSlicerSimpleManager.RegisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
  }

  private void StopBroadcast()
  {
    this.loudSpeaker.StopLocalBroadcast();
    this.onStopListening?.Invoke();
    this.wearable.OnCosmeticStopListening();
    GorillaSlicerSimpleManager.UnregisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
  }

  public void OnEnable()
  {
    this.isListening = false;
    this.speakingTime = 0.0f;
  }

  public void OnDisable()
  {
    this.isListening = false;
    this.speakingTime = 0.0f;
    this.StopBroadcast();
  }

  public void SetListenState(bool listening)
  {
    if (this.isListening == listening || !this.enabled || !this.gameObject.activeInHierarchy)
      return;
    this.isListening = listening;
    this.speakingTime = 0.0f;
    if (listening)
      this.StartBroadcast();
    else
      this.StopBroadcast();
  }

  public void SliceUpdate()
  {
    float num = Time.time - this.lastSliceUpdateTime;
    this.lastSliceUpdateTime = Time.time;
    if ((Object) this.gsl != (Object) null && this.gsl.IsSpeaking && (double) this.gsl.LoudnessNormalized >= (double) this.minVolume)
    {
      this.speakingTime += num;
      if ((double) this.speakingTime < (double) this.minSpeakingTime)
        return;
      if ((Object) this.animator != (Object) null)
        this.animator.SetTrigger(this.talkAnimationTrigger);
      if ((Object) this.simpleAnimation != (Object) null && !this.simpleAnimation.isPlaying)
        this.simpleAnimation.Play();
      if (this.isSpeaking)
        return;
      this.onStartSpeaking?.Invoke();
      this.isSpeaking = true;
    }
    else
    {
      this.speakingTime = 0.0f;
      if (!this.isSpeaking)
        return;
      this.onStopSpeaking?.Invoke();
      this.isSpeaking = false;
    }
  }

  private void ResetToFirstFrame()
  {
    this.simpleAnimation.Rewind();
    this.simpleAnimation.Play();
    this.simpleAnimation.Sample();
    this.simpleAnimation.Stop();
  }
}
