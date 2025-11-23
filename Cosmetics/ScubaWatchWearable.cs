// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ScubaWatchWearable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaLocomotion;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

[ExecuteAlways]
public class ScubaWatchWearable : MonoBehaviour
{
  public bool onLeftHand;
  [Tooltip("The transform that will be rotated to indicate the current depth.")]
  public Transform dialNeedle;
  [Tooltip("If your rotation is not zeroed out then click the Auto button to use the current rotation as 0.")]
  public Quaternion initialDialRotation;
  [Tooltip("The range of depth values that the dial will rotate between.")]
  public Vector2 depthRange = new Vector2(0.0f, 20f);
  [Tooltip("The range of rotation values that the dial will rotate between.")]
  public Vector2 dialRotationRange = new Vector2(0.0f, 360f);
  [Tooltip("The axis that the dial will rotate around.")]
  public Vector3 dialRotationAxis = Vector3.right;
  [Tooltip("The current depth of the player.")]
  [DebugOption]
  private float currentDepth;

  protected void Update()
  {
    GTPlayer instance = GTPlayer.Instance;
    this.currentDepth = !this.onLeftHand ? (!((Object) instance.RightHandWaterVolume != (Object) null) ? 0.0f : Mathf.Max(-instance.RightHandWaterSurface.surfacePlane.GetDistanceToPoint(instance.LastRightHandPosition), 0.0f)) : (!((Object) instance.LeftHandWaterVolume != (Object) null) ? 0.0f : Mathf.Max(-instance.LeftHandWaterSurface.surfacePlane.GetDistanceToPoint(instance.LastLeftHandPosition), 0.0f));
    this.dialNeedle.localRotation = this.initialDialRotation * Quaternion.AngleAxis(Mathf.Lerp(this.dialRotationRange.x, this.dialRotationRange.y, (float) (((double) this.currentDepth - (double) this.depthRange.x) / ((double) this.depthRange.y - (double) this.depthRange.x))), this.dialRotationAxis);
  }
}
