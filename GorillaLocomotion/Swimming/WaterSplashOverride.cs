// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Swimming.WaterSplashOverride
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Swimming;

public class WaterSplashOverride : MonoBehaviour
{
  public bool suppressWaterEffects;
  public bool playBigSplash;
  public bool playDrippingEffect = true;
  public bool scaleByPlayersScale;
  public bool overrideBoundingRadius;
  public float boundingRadiusOverride = 1f;
}
