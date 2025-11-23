// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.VelocityBasedAudioTriggerCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaLocomotion.Climbing;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class VelocityBasedAudioTriggerCosmetic : MonoBehaviour
{
  [SerializeField]
  private GorillaVelocityTracker velocityTracker;
  [SerializeField]
  private AudioSource audioSource;
  [SerializeField]
  private AudioClip audioClip;
  [SerializeField]
  private SoundBankPlayer soundBank;
  [Tooltip(" Minimum velocity to trigger audio")]
  [SerializeField]
  private float minVelocityThreshold = 0.5f;
  [SerializeField]
  private float maxVelocity = 2f;
  [SerializeField]
  private float minOutputVolume;
  [SerializeField]
  private float maxOutputVolume = 1f;

  private void Awake()
  {
    if ((Object) this.audioClip != (Object) null)
      this.audioSource.clip = this.audioClip;
    if (!((Object) this.soundBank != (Object) null) || !((Object) this.audioSource != (Object) null))
      return;
    this.soundBank.audioSource = this.audioSource;
  }

  private void Update()
  {
    Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true);
    if ((double) averageVelocity.magnitude < (double) this.minVelocityThreshold)
      return;
    float num = Mathf.Lerp(this.minOutputVolume, this.maxOutputVolume, Mathf.InverseLerp(this.minVelocityThreshold, this.maxVelocity, averageVelocity.magnitude));
    this.audioSource.volume = num;
    if ((Object) this.audioSource != (Object) null && !this.audioSource.isPlaying && (Object) this.audioClip != (Object) null)
    {
      this.audioSource.clip = this.audioClip;
      if (!this.audioSource.isActiveAndEnabled)
        return;
      this.audioSource.GTPlay();
    }
    else
    {
      if (!((Object) this.soundBank != (Object) null) || !((Object) this.soundBank.soundBank != (Object) null) || this.soundBank.isPlaying)
        return;
      this.soundBank.Play(new float?(num));
    }
  }
}
