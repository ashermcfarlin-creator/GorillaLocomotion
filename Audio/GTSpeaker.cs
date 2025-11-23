// Decompiled with JetBrains decompiler
// Type: GorillaTag.Audio.GTSpeaker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Photon.Voice;
using Photon.Voice.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag.Audio;

public class GTSpeaker : Speaker
{
  [FormerlySerializedAs("UseExternalAudioSources")]
  public bool BroadcastExternal;
  [SerializeField]
  private AudioSource[] _externalAudioSources;
  private List<IAudioOut<float>> _externalAudioOutputs;
  private int _frequency;
  private int _channels;
  private int _frameSamplesPerChannel;
  private bool _initializedExternalAudioSources;
  private bool _audioOutputStarted;

  public void Start()
  {
    LoudSpeakerNetwork componentInChildren = this.transform.root.GetComponentInChildren<LoudSpeakerNetwork>();
    if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
      return;
    this.AddExternalAudioSources(componentInChildren.SpeakerSources);
  }

  public void AddExternalAudioSources(AudioSource[] audioSources)
  {
    if (this._initializedExternalAudioSources)
      return;
    this._externalAudioSources = audioSources;
    this.InitializeExternalAudioSources();
    if (!this._audioOutputStarted)
      return;
    this.ExternalAudioOutputStart(this._frequency, this._channels, this._frameSamplesPerChannel);
  }

  protected override void Initialize()
  {
    if (this.IsInitialized)
    {
      if (!this.Logger.IsWarningEnabled)
        return;
      this.Logger.LogWarning("Already initialized.", Array.Empty<object>());
    }
    else
      base.Initialize();
  }

  private void InitializeExternalAudioSources()
  {
    this._initializedExternalAudioSources = true;
    this._externalAudioOutputs = new List<IAudioOut<float>>();
    AudioOutDelayControl.PlayDelayConfig pdc = new AudioOutDelayControl.PlayDelayConfig()
    {
      Low = this.playbackDelaySettings.MinDelaySoft,
      High = this.playbackDelaySettings.MaxDelaySoft,
      Max = this.playbackDelaySettings.MaxDelayHard
    };
    foreach (AudioSource externalAudioSource in this._externalAudioSources)
      this._externalAudioOutputs.Add(this.GetAudioOutFactoryFromSource(externalAudioSource, pdc)());
  }

  private Func<IAudioOut<float>> GetAudioOutFactoryFromSource(
    AudioSource source,
    AudioOutDelayControl.PlayDelayConfig pdc)
  {
    return (Func<IAudioOut<float>>) (() => (IAudioOut<float>) new UnityAudioOut(source, pdc, (Photon.Voice.ILogger) this.Logger, string.Empty, this.Logger.IsDebugEnabled));
  }

  protected override void OnAudioFrame(FrameOut<float> frame)
  {
    base.OnAudioFrame(frame);
    if (!this.BroadcastExternal)
      return;
    foreach (IAudioOut<float> externalAudioOutput in this._externalAudioOutputs)
    {
      externalAudioOutput.Push(frame.Buf);
      if (frame.EndOfStream)
        externalAudioOutput.Flush();
    }
  }

  protected override void AudioOutputStart(int frequency, int channels, int frameSamplesPerChannel)
  {
    this._audioOutputStarted = true;
    this._frequency = frequency;
    this._channels = channels;
    this._frameSamplesPerChannel = frameSamplesPerChannel;
    base.AudioOutputStart(frequency, channels, frameSamplesPerChannel);
    this.ExternalAudioOutputStart(frequency, channels, frameSamplesPerChannel);
  }

  private void ExternalAudioOutputStart(int frequency, int channels, int frameSamplesPerChannel)
  {
    if (this._externalAudioOutputs == null)
      return;
    foreach (IAudioOut<float> externalAudioOutput in this._externalAudioOutputs)
    {
      if (!externalAudioOutput.IsPlaying)
      {
        externalAudioOutput.Start(frequency, channels, frameSamplesPerChannel);
        externalAudioOutput.ToggleAudioSource(false);
      }
    }
  }

  protected override void AudioOutputStop()
  {
    this._audioOutputStarted = false;
    if (this._externalAudioOutputs != null)
    {
      foreach (IAudioOut<float> externalAudioOutput in this._externalAudioOutputs)
        externalAudioOutput.Stop();
    }
    base.AudioOutputStop();
  }

  protected override void AudioOutputService()
  {
    base.AudioOutputService();
    if (this._externalAudioOutputs == null)
      return;
    foreach (IAudioOut<float> externalAudioOutput in this._externalAudioOutputs)
    {
      if (!externalAudioOutput.IsPlaying)
        externalAudioOutput.Service();
    }
  }

  public void ToggleAudioSource(bool toggle)
  {
    if (this._externalAudioOutputs == null)
      return;
    foreach (IAudioOut<float> externalAudioOutput in this._externalAudioOutputs)
      externalAudioOutput.ToggleAudioSource(toggle);
  }
}
