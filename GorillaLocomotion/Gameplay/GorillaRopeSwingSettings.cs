// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.GorillaRopeSwingSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Gameplay;

[CreateAssetMenu(fileName = "GorillaRopeSwingSettings", menuName = "ScriptableObjects/GorillaRopeSwingSettings", order = 0)]
public class GorillaRopeSwingSettings : ScriptableObject
{
  public float inheritVelocityMultiplier = 1f;
  public float frictionWhenNotHeld = 0.25f;
}
