// Decompiled with JetBrains decompiler
// Type: GorillaTag.CosmeticSystem.AllCosmeticsArraySO
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.CosmeticSystem;

public class AllCosmeticsArraySO : ScriptableObject
{
  [SerializeField]
  public GTDirectAssetRef<CosmeticSO>[] sturdyAssetRefs;

  public CosmeticSO SearchForCosmeticSO(string playfabId)
  {
    foreach (GTDirectAssetRef<CosmeticSO> sturdyAssetRef in this.sturdyAssetRefs)
    {
      CosmeticSO cosmeticSo = (CosmeticSO) sturdyAssetRef;
      if (cosmeticSo.info.playFabID == playfabId)
        return cosmeticSo;
    }
    Debug.LogWarning((object) ("AllCosmeticsArraySO - SearchForCosmeticSO - No Cosmetic found with playfabId: " + playfabId), (Object) this);
    return (CosmeticSO) null;
  }
}
