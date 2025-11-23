// Decompiled with JetBrains decompiler
// Type: GorillaTag.Audio.ProcessVoiceDataToLoudness
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Photon.Voice;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.Audio;

internal class ProcessVoiceDataToLoudness : IProcessor<float>, IDisposable
{
  private VoiceToLoudness _voiceToLoudness;

  public ProcessVoiceDataToLoudness(VoiceToLoudness voiceToLoudness)
  {
    this._voiceToLoudness = voiceToLoudness;
  }

  public float[] Process(float[] buf)
  {
    float num = 0.0f;
    for (int index = 0; index < buf.Length; ++index)
      num += Mathf.Abs(buf[index]);
    this._voiceToLoudness.loudness = num / (float) buf.Length;
    return buf;
  }

  public void Dispose()
  {
  }
}
