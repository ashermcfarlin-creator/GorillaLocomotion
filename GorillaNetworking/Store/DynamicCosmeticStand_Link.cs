// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.DynamicCosmeticStand_Link
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaNetworking.Store;

public class DynamicCosmeticStand_Link : MonoBehaviour
{
  public DynamicCosmeticStand stand;

  public void SetStandType(HeadModel_CosmeticStand.BustType type) => this.stand.SetStandType(type);

  public void SpawnItemOntoStand(string PlayFabID) => this.stand.SpawnItemOntoStand(PlayFabID);

  public void SaveCosmeticMountPosition() => this.stand.UpdateCosmeticsMountPositions();

  public void ClearCosmeticItems() => this.stand.ClearCosmetics();
}
