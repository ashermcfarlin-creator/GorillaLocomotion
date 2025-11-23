// Decompiled with JetBrains decompiler
// Type: GorillaTag.CosmeticSystem.CosmeticSO
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.CosmeticSystem;

[CreateAssetMenu(fileName = "Untitled_CosmeticSO", menuName = "- Gorilla Tag/CosmeticSO", order = 0)]
public class CosmeticSO : ScriptableObject
{
  public CosmeticInfoV2 info = new CosmeticInfoV2("UNNAMED");
  public int propHuntWeight = 1;

  private bool ShowPropHuntWeight() => true;

  public void OnEnable() => this.info.debugCosmeticSOName = this.name;
}
