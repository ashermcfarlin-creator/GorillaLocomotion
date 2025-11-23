// Decompiled with JetBrains decompiler
// Type: GorillaTag.Audio.DuplicateAudioSource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Audio;

public class DuplicateAudioSource : MonoBehaviour
{
  public AudioSource TargetAudioSource;
  [SerializeField]
  private AudioSource _audioSource;
  [SerializeField]
  private bool _isDuplicating;

  public void SetTargetAudioSource(AudioSource target)
  {
    this.TargetAudioSource = target;
    this.StartDuplicating();
  }

  [ContextMenu("Start Duplicating")]
  public void StartDuplicating()
  {
    this._isDuplicating = true;
    this._audioSource.loop = this.TargetAudioSource.loop;
    this._audioSource.clip = this.TargetAudioSource.clip;
    if (!this.TargetAudioSource.isPlaying)
      return;
    this._audioSource.Play();
  }

  [ContextMenu("Stop Duplicating")]
  public void StopDuplicating()
  {
    this._isDuplicating = false;
    this._audioSource.Stop();
  }

  public void LateUpdate()
  {
    if (!this._isDuplicating)
      return;
    if (this.TargetAudioSource.isPlaying && !this._audioSource.isPlaying)
    {
      this._audioSource.Play();
    }
    else
    {
      if (this.TargetAudioSource.isPlaying || !this._audioSource.isPlaying)
        return;
      this._audioSource.Stop();
    }
  }
}
