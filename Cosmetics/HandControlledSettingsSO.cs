// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.HandControlledSettingsSO
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class HandControlledSettingsSO : ScriptableObject
{
  private const string SENS_TT = "The difference between the current input and cached input is magnified by this number.";
  public HandControlledCosmetic.RotationControl rotationControl;
  [Tooltip("The difference between the current input and cached input is magnified by this number.")]
  public float inputSensitivity = 2f;
  [Tooltip("The difference between the current input and cached input is magnified by this number.")]
  public AnimationCurve verticalSensitivity = AnimationCurve.Constant(0.0f, 1f, 2f);
  [Tooltip("The difference between the current input and cached input is magnified by this number.")]
  public AnimationCurve horizontalSensitivity = AnimationCurve.Constant(0.0f, 1f, 2f);
  [Tooltip("How quickly the cached input approaches the current input. A high value will function more like a mouse, while a low value will function more like a joystick.")]
  public float inputDecaySpeed = 1f;
  [Tooltip("How quickly the cached input approaches the current input, as a function of distance. A high value will function more like a mouse, while a low value will function more like a joystick.")]
  public AnimationCurve inputDecayCurve = AnimationCurve.Constant(0.0f, 2f, 1f);
  [Tooltip("How quickly the transform approaches the intended angle (smaller value = more lag).")]
  public float rotationSpeed = 20f;
  [Tooltip("The transform's local rotation cannot exceed these euler angles.")]
  public Vector3 angleLimits = new Vector3(45f, 360f, 0.0f);

  private bool IsAngle => this.rotationControl == HandControlledCosmetic.RotationControl.Angle;

  private bool IsTranslation
  {
    get => this.rotationControl == HandControlledCosmetic.RotationControl.Translation;
  }
}
