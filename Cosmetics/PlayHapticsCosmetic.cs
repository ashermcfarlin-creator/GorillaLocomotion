// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.PlayHapticsCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaLocomotion;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class PlayHapticsCosmetic : MonoBehaviour
{
  [SerializeField]
  private float hapticDuration;
  [SerializeField]
  private float hapticStrength;
  [SerializeField]
  private float minHapticStrengthThreshold;
  [SerializeField]
  private float maxHapticStrengthThreshold;
  [Tooltip("Only check this box if you are not setting the left/hand right from the subscriber")]
  [SerializeField]
  private bool leftHand;
  private TransferrableObject parentTransferable;

  private void Awake()
  {
    this.parentTransferable = this.GetComponentInParent<TransferrableObject>();
  }

  public void PlayHaptics()
  {
    GorillaTagger.Instance.StartVibration(this.leftHand, this.hapticStrength, this.hapticDuration);
  }

  public void PlayHapticsTransferableObject()
  {
    if (!((Object) this.parentTransferable != (Object) null) || !this.parentTransferable.IsMyItem())
      return;
    GorillaTagger.Instance.StartVibration(this.parentTransferable.InLeftHand(), this.hapticStrength, this.hapticDuration);
  }

  public void PlayHaptics(bool isLeftHand)
  {
    GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
  }

  public void PlayHapticsBothHands(bool isLeftHand)
  {
    this.PlayHaptics(false);
    this.PlayHaptics(true);
  }

  public void PlayHaptics(bool isLeftHand, float value)
  {
    GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
  }

  public void PlayHapticsBothHands(bool isLeftHand, float value)
  {
    this.PlayHaptics(false, value);
    this.PlayHaptics(true, value);
  }

  public void PlayHaptics(bool isLeftHand, Collider other)
  {
    GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
  }

  public void PlayHapticsBothHands(bool isLeftHand, Collider other)
  {
    this.PlayHaptics(false, other);
    this.PlayHaptics(true, other);
  }

  public void PlayHaptics(bool isLeftHand, Collision other)
  {
    GorillaTagger.Instance.StartVibration(isLeftHand, this.hapticStrength, this.hapticDuration);
  }

  public void PlayHapticsBothHands(bool isLeftHand, Collision other)
  {
    this.PlayHaptics(false, other);
    this.PlayHaptics(true, other);
  }

  public void PlayHapticsByButtonValue(bool isLeftHand, float strength)
  {
    float amplitude = Mathf.InverseLerp(this.minHapticStrengthThreshold, this.maxHapticStrengthThreshold, strength);
    GorillaTagger.Instance.StartVibration(isLeftHand, amplitude, this.hapticDuration);
  }

  public void PlayHapticsByButtonValueBothHands(bool isLeftHand, float strength)
  {
    this.PlayHapticsByButtonValue(false, strength);
    this.PlayHapticsByButtonValue(true, strength);
  }

  public void PlayHapticsByVelocity(bool isLeftHand, float velocity)
  {
    float amplitude = Mathf.InverseLerp(this.minHapticStrengthThreshold, this.maxHapticStrengthThreshold, GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand).GetAverageVelocity(true).magnitude);
    GorillaTagger.Instance.StartVibration(isLeftHand, amplitude, this.hapticDuration);
  }

  public void PlayHapticsByVelocityBothHands(bool isLeftHand, float velocity)
  {
    this.PlayHapticsByVelocity(false, velocity);
    this.PlayHapticsByVelocity(true, velocity);
  }
}
