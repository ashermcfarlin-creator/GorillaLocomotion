// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.StoreItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.IO;
using UnityEngine;

#nullable disable
namespace GorillaNetworking.Store;

[Serializable]
public class StoreItem
{
  public string itemName = "";
  public int itemCategory;
  public string itemPictureResourceString = "";
  public string displayName = "";
  public string overrideDisplayName = "";
  public string[] bundledItems = new string[0];
  public bool canTryOn;
  public bool bothHandsHoldable;
  public string AssetBundleName = "";
  public bool bUsesMeshAtlas;
  public string MeshAtlasResourceName = "";
  public string MeshResourceName = "";
  public string MaterialResrouceName = "";
  public Vector3 translationOffset = Vector3.zero;
  public Vector3 rotationOffset = Vector3.zero;
  public Vector3 scale = Vector3.one;

  public static void SerializeItemsAsJSON(StoreItem[] items)
  {
    string str = "";
    foreach (StoreItem storeItem in items)
      str = $"{str}{JsonUtility.ToJson((object) storeItem)};";
    Debug.LogError((object) str);
    File.WriteAllText(Application.dataPath + "/Resources/StoreItems/FeaturedStoreItemsList.json", str);
  }

  public static void ConvertCosmeticItemToSToreItem(
    CosmeticsController.CosmeticItem cosmeticItem,
    ref StoreItem storeItem)
  {
    storeItem.itemName = cosmeticItem.itemName;
    storeItem.itemCategory = (int) cosmeticItem.itemCategory;
    storeItem.itemPictureResourceString = cosmeticItem.itemPictureResourceString;
    storeItem.displayName = cosmeticItem.displayName;
    storeItem.overrideDisplayName = cosmeticItem.overrideDisplayName;
    storeItem.bundledItems = cosmeticItem.bundledItems;
    storeItem.canTryOn = cosmeticItem.canTryOn;
    storeItem.bothHandsHoldable = cosmeticItem.bothHandsHoldable;
    storeItem.AssetBundleName = "";
    storeItem.bUsesMeshAtlas = cosmeticItem.bUsesMeshAtlas;
    storeItem.MeshResourceName = cosmeticItem.meshResourceString;
    storeItem.MeshAtlasResourceName = cosmeticItem.meshAtlasResourceString;
    storeItem.MaterialResrouceName = cosmeticItem.materialResourceString;
  }
}
