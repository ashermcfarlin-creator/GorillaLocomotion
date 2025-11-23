// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Swimming.WaterParameters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Swimming;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WaterParameters", order = 1)]
public class WaterParameters : ScriptableObject
{
  [Header("Splash Effect")]
  public bool playSplashEffect = true;
  public GameObject splashEffect;
  public float splashEffectScale = 1f;
  public bool sendSplashEffectRPCs;
  public float splashSpeedRequirement = 0.8f;
  public float bigSplashSpeedRequirement = 1.9f;
  public Gradient splashColorBySpeedGradient;
  [Header("Ripple Effect")]
  public bool playRippleEffect = true;
  public GameObject rippleEffect;
  public float rippleEffectScale = 1f;
  public float defaultDistanceBetweenRipples = 0.75f;
  public float minDistanceBetweenRipples = 0.2f;
  public float minTimeBetweenRipples = 0.75f;
  public Color rippleSpriteColor = Color.white;
  [Header("Drip Effect")]
  public bool playDripEffect = true;
  public float postExitDripDuration = 1.5f;
  public float perDripTimeDelay = 0.2f;
  public float perDripTimeRandRange = 0.15f;
  public float perDripDefaultRadius = 0.01f;
  public float perDripRadiusRandRange = 0.01f;
  [Header("Misc")]
  public float recomputeSurfaceForColliderDist = 0.2f;
  public bool allowBubblesInVolume;
}
