// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.BundleManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaNetworking.Store;

public class BundleManager : MonoBehaviour
{
  public static volatile BundleManager instance;
  [FormerlySerializedAs("_TryOnBundlesStand")]
  public TryOnBundlesStand _tryOnBundlesStand;
  [SerializeField]
  private StoreBundleData nullBundleData;
  private List<StoreBundleData> _bundleScriptableObjects = new List<StoreBundleData>();
  [SerializeField]
  private List<StoreBundle> _storeBundles = new List<StoreBundle>();
  [FormerlySerializedAs("_SpawnedBundleStands")]
  [SerializeField]
  private List<SpawnedBundle> _spawnedBundleStands = new List<SpawnedBundle>();
  public Dictionary<string, StoreBundle> storeBundlesById = new Dictionary<string, StoreBundle>();
  public Dictionary<string, StoreBundle> storeBundlesBySKU = new Dictionary<string, StoreBundle>();
  [Header("Enable Advanced Search window in your settings to easily see all bundle prefabs")]
  [SerializeField]
  private List<BundleManager.BundleStandSpawn> BundleStands = new List<BundleManager.BundleStandSpawn>();
  [SerializeField]
  private StoreBundleData tryOnBundleButton1;
  [SerializeField]
  private StoreBundleData tryOnBundleButton2;
  [SerializeField]
  private StoreBundleData tryOnBundleButton3;
  [SerializeField]
  private StoreBundleData tryOnBundleButton4;
  [SerializeField]
  private StoreBundleData tryOnBundleButton5;

  private IEnumerable GetStoreBundles()
  {
    List<StoreBundleData> storeBundles = new List<StoreBundleData>();
    storeBundles.Add(this.nullBundleData);
    storeBundles.AddRange((IEnumerable<StoreBundleData>) this._bundleScriptableObjects);
    return (IEnumerable) storeBundles;
  }

  public void Awake()
  {
    if ((UnityEngine.Object) BundleManager.instance == (UnityEngine.Object) null)
    {
      BundleManager.instance = this;
    }
    else
    {
      if (!((UnityEngine.Object) BundleManager.instance != (UnityEngine.Object) this))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
  }

  private void Start()
  {
    this.GenerateBundleDictionaries();
    this.Initialize();
  }

  private void Initialize()
  {
    foreach (StoreBundle storeBundle in this._storeBundles)
      storeBundle.InitializebundleStands();
  }

  private void ValidateBundleData()
  {
    foreach (StoreBundle storeBundle in this._storeBundles)
      storeBundle.ValidateBundleData();
  }

  private void SpawnBundleStands()
  {
    foreach (StoreBundle storeBundle in this._storeBundles)
    {
      foreach (BundleStand bundleStand in storeBundle.bundleStands)
      {
        if ((UnityEngine.Object) bundleStand != (UnityEngine.Object) null)
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) bundleStand.gameObject);
      }
    }
    this._spawnedBundleStands.Clear();
    this.storeBundlesById.Clear();
    this.storeBundlesBySKU.Clear();
    this._storeBundles.Clear();
    this._bundleScriptableObjects.Clear();
    foreach (Component component in UnityEngine.Object.FindObjectsByType<BundleStand>(FindObjectsSortMode.None))
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component.gameObject);
    for (int index = 0; index < this.BundleStands.Count; ++index)
    {
      if ((UnityEngine.Object) this.BundleStands[index].spawnLocation == (UnityEngine.Object) null)
        Debug.LogError((object) ("No spawn location set for Bundle Stand " + index.ToString()));
      else if ((UnityEngine.Object) this.BundleStands[index].bundleStand == (UnityEngine.Object) null)
        Debug.LogError((object) ("No Bundle Stand set for Bundle Stand " + index.ToString()));
    }
    this.GenerateAllStoreBundleReferences();
    if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton1))
      this.tryOnBundleButton1 = this.nullBundleData;
    if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton2))
      this.tryOnBundleButton2 = this.nullBundleData;
    if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton3))
      this.tryOnBundleButton3 = this.nullBundleData;
    if (!this._bundleScriptableObjects.Contains(this.tryOnBundleButton4))
      this.tryOnBundleButton4 = this.nullBundleData;
    if (this._bundleScriptableObjects.Contains(this.tryOnBundleButton5))
      return;
    this.tryOnBundleButton4 = this.nullBundleData;
  }

  public void ClearEverything()
  {
    foreach (StoreBundle storeBundle in this._storeBundles)
    {
      foreach (BundleStand bundleStand in storeBundle.bundleStands)
      {
        if ((UnityEngine.Object) bundleStand != (UnityEngine.Object) null)
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) bundleStand.gameObject);
      }
    }
    this._spawnedBundleStands.Clear();
    this.storeBundlesById.Clear();
    this.storeBundlesBySKU.Clear();
    this._storeBundles.Clear();
    this._bundleScriptableObjects.Clear();
    this.tryOnBundleButton1 = this.nullBundleData;
    this.tryOnBundleButton2 = this.nullBundleData;
    this.tryOnBundleButton3 = this.nullBundleData;
    this.tryOnBundleButton4 = this.nullBundleData;
    this.tryOnBundleButton5 = this.nullBundleData;
    foreach (Component component in UnityEngine.Object.FindObjectsByType<BundleStand>(FindObjectsSortMode.None))
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component.gameObject);
  }

  public void GenerateAllStoreBundleReferences()
  {
  }

  private void AddNewBundleStand(BundleStand bundleStand)
  {
    foreach (StoreBundle storeBundle in this._storeBundles)
    {
      if (storeBundle.playfabBundleID == bundleStand._bundleDataReference.playfabBundleID)
      {
        storeBundle.bundleStands.Add(bundleStand);
        return;
      }
    }
    this._storeBundles.Add(new StoreBundle(bundleStand._bundleDataReference)
    {
      bundleStands = {
        bundleStand
      }
    });
  }

  public void GenerateBundleDictionaries()
  {
    this.storeBundlesById.Clear();
    this.storeBundlesBySKU.Clear();
    foreach (StoreBundle storeBundle in this._storeBundles)
    {
      this.storeBundlesById.Add(storeBundle.playfabBundleID, storeBundle);
      this.storeBundlesBySKU.Add(storeBundle.bundleSKU, storeBundle);
    }
  }

  public void BundlePurchaseButtonPressed(string playFabItemName)
  {
    CosmeticsController.instance.PurchaseBundle(this.storeBundlesById[playFabItemName]);
  }

  public void FixBundles()
  {
    this._storeBundles.Clear();
    for (int index = this._spawnedBundleStands.Count - 1; index >= 0; --index)
    {
      if ((UnityEngine.Object) this._spawnedBundleStands[index].bundleStand == (UnityEngine.Object) null)
        this._spawnedBundleStands.RemoveAt(index);
    }
    foreach (BundleStand bundleStand in UnityEngine.Object.FindObjectsByType<BundleStand>(FindObjectsSortMode.None))
    {
      BundleStand bundle = bundleStand;
      if (this._spawnedBundleStands.Any<SpawnedBundle>((Func<SpawnedBundle, bool>) (x => x.spawnLocationPath == bundle.transform.parent.gameObject.GetPath(3))))
      {
        SpawnedBundle spawnedBundle = this._spawnedBundleStands.First<SpawnedBundle>((Func<SpawnedBundle, bool>) (x => x.spawnLocationPath == bundle.transform.parent.gameObject.GetPath(3)));
        if (spawnedBundle != null && (UnityEngine.Object) spawnedBundle.bundleStand != (UnityEngine.Object) bundle)
        {
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) spawnedBundle.bundleStand.gameObject);
          spawnedBundle.bundleStand = bundle;
        }
      }
      else
        this._spawnedBundleStands.Add(new SpawnedBundle()
        {
          spawnLocationPath = bundle.transform.parent.gameObject.GetPath(3),
          bundleStand = bundle
        });
    }
    this.GenerateAllStoreBundleReferences();
  }

  public StoreBundleData[] GetTryOnButtons()
  {
    return new StoreBundleData[5]
    {
      this.tryOnBundleButton1,
      this.tryOnBundleButton2,
      this.tryOnBundleButton3,
      this.tryOnBundleButton4,
      this.tryOnBundleButton5
    };
  }

  public void NotifyBundleOfErrorByPlayFabID(string ItemId)
  {
    StoreBundle storeBundle;
    if (!this.storeBundlesById.TryGetValue(ItemId, out storeBundle))
      return;
    foreach (BundleStand bundleStand in storeBundle.bundleStands)
      bundleStand.ErrorHappened();
  }

  public void NotifyBundleOfErrorBySKU(string ItemSKU)
  {
    StoreBundle storeBundle;
    if (!this.storeBundlesBySKU.TryGetValue(ItemSKU, out storeBundle))
      return;
    foreach (BundleStand bundleStand in storeBundle.bundleStands)
      bundleStand.ErrorHappened();
  }

  public void MarkBundleOwnedByPlayFabID(string ItemId)
  {
    if (!this.storeBundlesById.ContainsKey(ItemId))
      return;
    this.storeBundlesById[ItemId].isOwned = true;
    foreach (BundleStand bundleStand in this.storeBundlesById[ItemId].bundleStands)
      bundleStand.NotifyAlreadyOwn();
  }

  public void MarkBundleOwnedBySKU(string SKU)
  {
    if (!this.storeBundlesBySKU.ContainsKey(SKU))
      return;
    this.storeBundlesBySKU[SKU].isOwned = true;
    foreach (BundleStand bundleStand in this.storeBundlesBySKU[SKU].bundleStands)
      bundleStand.NotifyAlreadyOwn();
  }

  public void CheckIfBundlesOwned()
  {
    foreach (StoreBundle storeBundle in this.storeBundlesById.Values)
    {
      if (storeBundle.isOwned)
      {
        foreach (BundleStand bundleStand in storeBundle.bundleStands)
          bundleStand.NotifyAlreadyOwn();
      }
    }
  }

  public void PressTryOnBundleButton(TryOnBundleButton pressedTryOnBundleButton, bool isLeftHand)
  {
    if (!this._tryOnBundlesStand.IsNotNull())
      return;
    this._tryOnBundlesStand.PressTryOnBundleButton(pressedTryOnBundleButton, isLeftHand);
  }

  public void PressPurchaseTryOnBundleButton() => this._tryOnBundlesStand.PurchaseButtonPressed();

  public void UpdateBundlePrice(string productSku, string productFormattedPrice)
  {
    if (!this.storeBundlesBySKU.ContainsKey(productSku))
      return;
    this.storeBundlesBySKU[productSku].TryUpdatePrice(productFormattedPrice);
  }

  public void CheckForNoPriceBundlesAndDefaultPrice()
  {
    foreach (KeyValuePair<string, StoreBundle> keyValuePair in this.storeBundlesBySKU)
    {
      string str;
      StoreBundle storeBundle1;
      keyValuePair.Deconstruct(ref str, ref storeBundle1);
      StoreBundle storeBundle2 = storeBundle1;
      if (!storeBundle2.HasPrice)
        storeBundle2.TryUpdatePrice();
    }
  }

  [Serializable]
  public class BundleStandSpawn
  {
    public EndCapSpawnPoint spawnLocation;
    public BundleStand bundleStand;

    private static IEnumerable GetEndCapSpawnPoints()
    {
      return (IEnumerable) ((IEnumerable<EndCapSpawnPoint>) UnityEngine.Object.FindObjectsByType<EndCapSpawnPoint>(FindObjectsSortMode.None)).Select<EndCapSpawnPoint, ValueDropdownItem>((Func<EndCapSpawnPoint, ValueDropdownItem>) (x => new ValueDropdownItem($"{x.transform.parent.parent.name}/{x.transform.parent.name}/{x.name}", (object) x)));
    }
  }
}
