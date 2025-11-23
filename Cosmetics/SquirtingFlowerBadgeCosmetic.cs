// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.SquirtingFlowerBadgeCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTag.CosmeticSystem;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class SquirtingFlowerBadgeCosmetic : MonoBehaviour, ISpawnable, IFingerFlexListener
{
  [SerializeField]
  private ParticleSystem particlesToPlay;
  [SerializeField]
  private GameObject objectToEnable;
  [SerializeField]
  private AudioClip audioToPlay;
  [SerializeField]
  private AudioSource audioSource;
  [SerializeField]
  private float coolDownTimer = 2f;
  [SerializeField]
  private bool leftHand;
  private float triggeredTime;
  private bool restartTimer;
  private bool buttonReleased = true;

  public VRRig MyRig { get; private set; }

  public bool IsSpawned { get; set; }

  public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

  public void OnSpawn(VRRig rig) => this.MyRig = rig;

  public void OnDespawn()
  {
  }

  private void Update()
  {
    if (this.restartTimer || (double) Time.time - (double) this.triggeredTime < (double) this.coolDownTimer)
      return;
    this.restartTimer = true;
  }

  private void OnPlayEffectLocal()
  {
    if ((Object) this.particlesToPlay != (Object) null)
      this.particlesToPlay.Play();
    if ((Object) this.objectToEnable != (Object) null)
      this.objectToEnable.SetActive(true);
    if ((Object) this.audioSource != (Object) null && (Object) this.audioToPlay != (Object) null)
      this.audioSource.GTPlayOneShot(this.audioToPlay);
    this.restartTimer = false;
    this.triggeredTime = Time.time;
  }

  public void OnButtonPressed(bool isLeftHand, float value)
  {
    if (!this.FingerFlexValidation(isLeftHand) || !this.restartTimer || !this.buttonReleased)
      return;
    this.OnPlayEffectLocal();
    this.buttonReleased = false;
  }

  public void OnButtonReleased(bool isLeftHand, float value)
  {
    if (!this.FingerFlexValidation(isLeftHand))
      return;
    this.buttonReleased = true;
  }

  public void OnButtonPressStayed(bool isLeftHand, float value)
  {
  }

  public bool FingerFlexValidation(bool isLeftHand)
  {
    return (!this.leftHand || isLeftHand) && !(!this.leftHand & isLeftHand);
  }
}
