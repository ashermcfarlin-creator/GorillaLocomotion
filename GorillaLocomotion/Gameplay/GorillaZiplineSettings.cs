// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.GorillaZiplineSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Gameplay;

[CreateAssetMenu(fileName = "GorillaZiplineSettings", menuName = "ScriptableObjects/GorillaZiplineSettings", order = 0)]
public class GorillaZiplineSettings : ScriptableObject
{
  public float minSlidePitch = 0.5f;
  public float maxSlidePitch = 1f;
  public float minSlideVolume;
  public float maxSlideVolume = 0.2f;
  public float maxSpeed = 10f;
  public float gravityMulti = 1.1f;
  [Header("Friction")]
  public float friction = 0.25f;
  public float maxFriction = 1f;
  public float maxFrictionSpeed = 15f;
}
