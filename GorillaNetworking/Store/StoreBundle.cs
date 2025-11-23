// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.StoreBundle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaNetworking.Store;

[Serializable]
public class StoreBundle
{
  private static readonly string defaultPrice = "$--.--";
  private static readonly string defaultCurrencySymbol = "$";
  [NonSerialized]
  public string purchaseButtonStringFormat = "THE {0}\n{1}";
  [SerializeField]
  public List<BundleStand> bundleStands;
  public bool isOwned;
  private string _price = StoreBundle.defaultPrice;
  private string _bundleName = "";
  public string purchaseButtonText = "";
  [FormerlySerializedAs("storeBundleDataReference")]
  [SerializeField]
  [ReadOnly]
  private StoreBundleData _storeBundleDataReference;

  public string playfabBundleID => this._storeBundleDataReference.playfabBundleID;

  public string bundleSKU => this._storeBundleDataReference.bundleSKU;

  public Sprite bundleImage => this._storeBundleDataReference.bundleImage;

  public string price => this._price;

  public string bundleName
  {
    get
    {
      if (this._bundleName.IsNullOrEmpty())
      {
        int index = CosmeticsController.instance.allCosmetics.FindIndex((Predicate<CosmeticsController.CosmeticItem>) (x => this.playfabBundleID == x.itemName));
        this._bundleName = index <= -1 ? "NULL_BUNDLE_NAME" : (CosmeticsController.instance.allCosmetics[index].overrideDisplayName.IsNullOrEmpty() ? CosmeticsController.instance.allCosmetics[index].displayName : CosmeticsController.instance.allCosmetics[index].overrideDisplayName);
      }
      return this._bundleName;
    }
  }

  public bool HasPrice
  {
    get => !string.IsNullOrEmpty(this.price) && this.price != StoreBundle.defaultPrice;
  }

  public string bundleDescriptionText => this._storeBundleDataReference.bundleDescriptionText;

  public StoreBundle()
  {
    this.isOwned = false;
    this.bundleStands = new List<BundleStand>();
  }

  public StoreBundle(StoreBundleData data)
  {
    this.isOwned = false;
    this.bundleStands = new List<BundleStand>();
    this._storeBundleDataReference = data;
  }

  public void InitializebundleStands()
  {
    foreach (BundleStand bundleStand in this.bundleStands)
    {
      bundleStand.UpdateDescriptionText(this.bundleDescriptionText);
      bundleStand.InitializeEventListeners();
    }
  }

  public void TryUpdatePrice(uint bundlePrice)
  {
    this.TryUpdatePrice(((Decimal) bundlePrice / 100M).ToString());
  }

  public void TryUpdatePrice(string bundlePrice = null)
  {
    if (!string.IsNullOrEmpty(bundlePrice))
      this._price = Decimal.TryParse(bundlePrice, out Decimal _) ? StoreBundle.defaultCurrencySymbol + bundlePrice : bundlePrice;
    this.UpdatePurchaseButtonText();
  }

  public void UpdatePurchaseButtonText()
  {
    this.purchaseButtonText = string.Format(this.purchaseButtonStringFormat, (object) this.bundleName, (object) this.price);
    foreach (BundleStand bundleStand in this.bundleStands)
      bundleStand.UpdatePurchaseButtonText(this.purchaseButtonText);
  }

  public void ValidateBundleData()
  {
    if ((UnityEngine.Object) this._storeBundleDataReference == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "StoreBundleData is null");
      foreach (BundleStand bundleStand in this.bundleStands)
      {
        if ((UnityEngine.Object) bundleStand == (UnityEngine.Object) null)
          Debug.LogError((object) "BundleStand is null");
        else if ((UnityEngine.Object) bundleStand._bundleDataReference != (UnityEngine.Object) null)
        {
          this._storeBundleDataReference = bundleStand._bundleDataReference;
          Debug.LogError((object) "BundleStand StoreBundleData is not equal to StoreBundle StoreBundleData");
        }
      }
    }
    if ((UnityEngine.Object) this._storeBundleDataReference == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "StoreBundleData is null");
    }
    else
    {
      if (this._storeBundleDataReference.playfabBundleID.IsNullOrEmpty())
        Debug.LogError((object) "playfabBundleID is null");
      if (this._storeBundleDataReference.bundleSKU.IsNullOrEmpty())
        Debug.LogError((object) "bundleSKU is null");
      if ((UnityEngine.Object) this._storeBundleDataReference.bundleImage == (UnityEngine.Object) null)
        Debug.LogError((object) "bundleImage is null");
      if (!this._storeBundleDataReference.bundleDescriptionText.IsNullOrEmpty())
        return;
      Debug.LogError((object) "bundleDescriptionText is null");
    }
  }
}
