// Decompiled with JetBrains decompiler
// Type: GorillaTag.MaterialDatasSO
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaLocomotion;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag;

[CreateAssetMenu(fileName = "MaterialDatasSO", menuName = "Gorilla Tag/MaterialDatasSO")]
public class MaterialDatasSO : ScriptableObject
{
  public List<GTPlayer.MaterialData> datas;
  public List<HashWrapper> surfaceEffects;
}
