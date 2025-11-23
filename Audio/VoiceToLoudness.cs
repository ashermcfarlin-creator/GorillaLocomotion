// Decompiled with JetBrains decompiler
// Type: GorillaTag.Audio.VoiceToLoudness
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Photon.Voice;
using Photon.Voice.Unity;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.Audio;

[RequireComponent(typeof (Recorder))]
public class VoiceToLoudness : MonoBehaviour
{
  [NonSerialized]
  public float loudness;
  private Recorder _recorder;

  protected void Awake() => this._recorder = this.GetComponent<Recorder>();

  protected void PhotonVoiceCreated(PhotonVoiceCreatedParams photonVoiceCreatedParams)
  {
    VoiceInfo info = photonVoiceCreatedParams.Voice.Info;
    if (!(photonVoiceCreatedParams.Voice is LocalVoiceAudioFloat voice))
      return;
    IProcessor<float>[] processorArray = new IProcessor<float>[1]
    {
      (IProcessor<float>) new ProcessVoiceDataToLoudness(this)
    };
    voice.AddPostProcessor(processorArray);
  }
}
