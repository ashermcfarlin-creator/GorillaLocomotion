// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.SimpleTransformAnimatorCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag.Cosmetics;

public class SimpleTransformAnimatorCosmetic : MonoBehaviour, ITickSystemTick
{
  private SimpleTransformAnimatorCosmetic.animModes animMode;
  [Tooltip("Shapes how the transform will interpolate over the course of the animation.")]
  public AnimationCurve InterpolationCurve = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  [Tooltip("The object that will animate (blend) between the poses.")]
  private Transform targetTransform;
  [SerializeField]
  [Tooltip("Start pose (blend value 0).")]
  private Transform poseA;
  [SerializeField]
  [Tooltip("End pose (blend value 1).")]
  private Transform poseB;
  [FormerlySerializedAs("transitionTime")]
  [SerializeField]
  [Tooltip("Total time (in seconds) to animate fully between poses.")]
  private float animationDuration = 1f;
  [SerializeField]
  [Tooltip("Controls what aspect of the transform is affected by the blend.")]
  private SimpleTransformAnimatorCosmetic.animatedPropertyChoices animatedProperties = SimpleTransformAnimatorCosmetic.animatedPropertyChoices.PositionAndRotation;
  private bool loopAnim;
  private float posBlendCurrent;
  private float posBlendTarget;
  private bool isAnimating;

  private void DebugToggle() => this.Toggle();

  private void DebugA() => this.TogglePoseA();

  private void DebugB() => this.TogglePoseB();

  public bool TickRunning { get; set; }

  private void OnEnable()
  {
    this.posBlendCurrent = this.posBlendTarget;
    this.UpdateTransform();
  }

  private void OnDisable()
  {
    if (!this.TickRunning)
      return;
    TickSystem<object>.RemoveCallbackTarget((object) this);
    this.TickRunning = false;
  }

  private void CheckAnimationNeeded()
  {
    bool flag1 = false;
    bool flag2 = Mathf.Approximately(this.posBlendCurrent, this.posBlendTarget);
    switch (this.animMode)
    {
      case SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos:
        flag1 = !flag2;
        break;
      case SimpleTransformAnimatorCosmetic.animModes.animateOneshot:
        flag1 = this.loopAnim || !flag2;
        break;
    }
    if (flag1 && !this.TickRunning)
    {
      TickSystem<object>.AddCallbackTarget((object) this);
      this.TickRunning = true;
      this.isAnimating = true;
    }
    else
    {
      if (flag1 || !this.TickRunning)
        return;
      TickSystem<object>.RemoveCallbackTarget((object) this);
      this.TickRunning = false;
      this.isAnimating = false;
    }
  }

  public void Tick()
  {
    this.posBlendCurrent = Mathf.MoveTowards(this.posBlendCurrent, this.posBlendTarget, Time.deltaTime * (1f / this.animationDuration));
    switch (this.animMode)
    {
      default:
        this.UpdateTransform();
        this.CheckAnimationNeeded();
        break;
    }
  }

  private void UpdateTransform()
  {
    Vector3 position = this.targetTransform.position;
    Quaternion rotation = this.targetTransform.rotation;
    float t = this.InterpolationCurve.Evaluate(this.posBlendCurrent);
    if (this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.Position || this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.PositionAndRotation)
      position = Vector3.Lerp(this.poseA.position, this.poseB.position, t);
    if (this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.Rotation || this.animatedProperties == SimpleTransformAnimatorCosmetic.animatedPropertyChoices.PositionAndRotation)
      rotation = Quaternion.Slerp(this.poseA.rotation, this.poseB.rotation, t);
    this.targetTransform.SetPositionAndRotation(position, rotation);
  }

  public void Toggle()
  {
    this.animMode = SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos;
    this.posBlendTarget = (double) this.posBlendTarget < 0.5 ? 1f : 0.0f;
    this.CheckAnimationNeeded();
  }

  public void TogglePoseA()
  {
    this.animMode = SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos;
    this.posBlendTarget = 0.0f;
    this.CheckAnimationNeeded();
  }

  public void TogglePoseB()
  {
    this.animMode = SimpleTransformAnimatorCosmetic.animModes.stepToTargetPos;
    this.posBlendTarget = 1f;
    this.CheckAnimationNeeded();
  }

  public void playAnimationOneshot()
  {
    this.animMode = SimpleTransformAnimatorCosmetic.animModes.animateOneshot;
    this.posBlendCurrent = 0.0f;
    this.posBlendTarget = 1f;
    this.CheckAnimationNeeded();
  }

  private void DebugPlayAnimationOneShot() => this.playAnimationOneshot();

  public enum animatedPropertyChoices
  {
    Position,
    Rotation,
    PositionAndRotation,
  }

  public enum animModes
  {
    stepToTargetPos,
    animateBounce,
    animateOneshot,
  }
}
