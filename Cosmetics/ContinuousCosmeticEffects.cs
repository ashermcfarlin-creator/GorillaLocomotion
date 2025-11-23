// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ContinuousCosmeticEffects
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag.Cosmetics;

public class ContinuousCosmeticEffects : MonoBehaviour
{
  [FormerlySerializedAs("properties")]
  [SerializeField]
  private ContinuousPropertyArray continuousProperties;

  public void ApplyAll(float f) => this.continuousProperties.ApplyAll(f);
}
