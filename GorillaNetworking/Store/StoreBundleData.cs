// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.StoreBundleData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaNetworking.Store;

public class StoreBundleData : ScriptableObject
{
  public string playfabBundleID = "NULL";
  public string bundleSKU = "NULL SKU";
  public Sprite bundleImage;
  public string bundleDescriptionText = "THE NULL_BUNDLE PACK WITH 10,000 SHINY ROCKS IN THIS LIMITED TIME DLC!";

  public void OnValidate()
  {
    if (this.playfabBundleID.Contains(' '))
      Debug.LogError((object) ("ERROR THERE IS A SPACE IN THE PLAYFAB BUNDLE ID " + this.name));
    if (!this.bundleSKU.Contains(' '))
      return;
    Debug.LogError((object) ("ERROR THERE IS A SPACE IN THE BUNDLE SKU " + this.name));
  }
}
