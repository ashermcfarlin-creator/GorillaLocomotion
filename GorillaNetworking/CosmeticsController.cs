// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.CosmeticsController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using CosmeticRoom;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking.Store;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaNetworking;

public class CosmeticsController : MonoBehaviour, IGorillaSliceableSimple, IBuildValidation
{
  [FormerlySerializedAs("v2AllCosmeticsInfoAssetRef")]
  [FormerlySerializedAs("newSysAllCosmeticsAssetRef")]
  [SerializeField]
  public GTAssetRef<AllCosmeticsArraySO> v2_allCosmeticsInfoAssetRef;
  private readonly Dictionary<string, CosmeticInfoV2> _allCosmeticsDictV2 = new Dictionary<string, CosmeticInfoV2>();
  public Action V2_allCosmeticsInfoAssetRef_OnPostLoad;
  public const int maximumTransferrableItems = 5;
  [OnEnterPlay_SetNull]
  public static volatile CosmeticsController instance;
  public Action V2_OnGetCosmeticsPlayFabCatalogData_PostSuccess;
  public Action OnGetCurrency;
  [FormerlySerializedAs("allCosmetics")]
  [SerializeField]
  private List<CosmeticsController.CosmeticItem> _allCosmetics;
  public Dictionary<string, CosmeticsController.CosmeticItem> _allCosmeticsDict = new Dictionary<string, CosmeticsController.CosmeticItem>(2048 /*0x0800*/);
  public Dictionary<string, string> _allCosmeticsItemIDsfromDisplayNamesDict = new Dictionary<string, string>(2048 /*0x0800*/);
  public CosmeticsController.CosmeticItem nullItem;
  public string catalog;
  private string[] tempStringArray;
  private CosmeticsController.CosmeticItem tempItem;
  private VRRigAnchorOverrides anchorOverrides;
  public List<CatalogItem> catalogItems;
  public bool tryTwice;
  public CustomMapCosmeticsData customMapCosmeticsData;
  [NonSerialized]
  public CosmeticsController.CosmeticSet tryOnSet = new CosmeticsController.CosmeticSet();
  public int numFittingRoomButtons = 12;
  public List<FittingRoom> fittingRooms = new List<FittingRoom>();
  public CosmeticStand[] cosmeticStands;
  public List<CosmeticsController.CosmeticItem> currentCart = new List<CosmeticsController.CosmeticItem>();
  public CosmeticsController.PurchaseItemStages currentPurchaseItemStage;
  public List<ItemCheckout> itemCheckouts = new List<ItemCheckout>();
  public CosmeticsController.CosmeticItem itemToBuy;
  private List<string> playerIDList = new List<string>();
  private List<string> inventoryStringList = new List<string>();
  private bool foundCosmetic;
  private int attempts;
  private string finalLine;
  private string leftCheckoutPurchaseButtonString;
  private string rightCheckoutPurchaseButtonString;
  private bool leftCheckoutPurchaseButtonOn;
  private bool rightCheckoutPurchaseButtonOn;
  private bool isLastHandTouchedLeft;
  private CosmeticsController.CosmeticSet cachedSet = new CosmeticsController.CosmeticSet();
  public readonly List<WardrobeInstance> wardrobes = new List<WardrobeInstance>();
  public List<CosmeticsController.CosmeticItem> unlockedCosmetics = new List<CosmeticsController.CosmeticItem>(2048 /*0x0800*/);
  public List<CosmeticsController.CosmeticItem> unlockedHats = new List<CosmeticsController.CosmeticItem>(512 /*0x0200*/);
  public List<CosmeticsController.CosmeticItem> unlockedFaces = new List<CosmeticsController.CosmeticItem>(512 /*0x0200*/);
  public List<CosmeticsController.CosmeticItem> unlockedBadges = new List<CosmeticsController.CosmeticItem>(512 /*0x0200*/);
  public List<CosmeticsController.CosmeticItem> unlockedPaws = new List<CosmeticsController.CosmeticItem>(512 /*0x0200*/);
  public List<CosmeticsController.CosmeticItem> unlockedChests = new List<CosmeticsController.CosmeticItem>(512 /*0x0200*/);
  public List<CosmeticsController.CosmeticItem> unlockedFurs = new List<CosmeticsController.CosmeticItem>(512 /*0x0200*/);
  public List<CosmeticsController.CosmeticItem> unlockedShirts = new List<CosmeticsController.CosmeticItem>(512 /*0x0200*/);
  public List<CosmeticsController.CosmeticItem> unlockedPants = new List<CosmeticsController.CosmeticItem>(512 /*0x0200*/);
  public List<CosmeticsController.CosmeticItem> unlockedBacks = new List<CosmeticsController.CosmeticItem>(512 /*0x0200*/);
  public List<CosmeticsController.CosmeticItem> unlockedArms = new List<CosmeticsController.CosmeticItem>(512 /*0x0200*/);
  public List<CosmeticsController.CosmeticItem> unlockedTagFX = new List<CosmeticsController.CosmeticItem>(512 /*0x0200*/);
  public List<CosmeticsController.CosmeticItem> unlockedThrowables = new List<CosmeticsController.CosmeticItem>(512 /*0x0200*/);
  public int[] cosmeticsPages = new int[11];
  private List<CosmeticsController.CosmeticItem>[] itemLists = new List<CosmeticsController.CosmeticItem>[11];
  private int wardrobeType;
  [NonSerialized]
  public CosmeticsController.CosmeticSet currentWornSet = new CosmeticsController.CosmeticSet();
  [NonSerialized]
  public CosmeticsController.CosmeticSet tempUnlockedSet = new CosmeticsController.CosmeticSet();
  [NonSerialized]
  public CosmeticsController.CosmeticSet activeMergedSet = new CosmeticsController.CosmeticSet();
  public string concatStringCosmeticsAllowed = "";
  public Action OnCosmeticsUpdated;
  public int currencyBalance;
  public string currencyName;
  public List<CurrencyBoard> currencyBoards;
  public string itemToPurchase;
  public bool buyingBundle;
  public bool confirmedDidntPlayInBeta;
  public bool playedInBeta;
  public bool gotMyDaily;
  public bool checkedDaily;
  public string currentPurchaseID;
  public bool hasPrice;
  private int searchIndex;
  private int iterator;
  private CosmeticsController.CosmeticItem cosmeticItemVar;
  [SerializeField]
  private CosmeticSO m_earlyAccessSupporterPackCosmeticSO;
  public EarlyAccessButton[] earlyAccessButtons;
  private BundleList bundleList = new BundleList();
  public string BundleSkuName = "2024_i_lava_you_pack";
  public string BundlePlayfabItemName = "LSABG.";
  public int BundleShinyRocks = 10000;
  public DateTime currentTime;
  public string lastDailyLogin;
  public UserDataRecord userDataRecord;
  public int secondsUntilTomorrow;
  public float secondsToWaitToCheckDaily = 10f;
  private int updateCosmeticsRetries;
  private int maxUpdateCosmeticsRetries;
  private GetUserInventoryResult latestInventory;
  private string returnString;
  private bool checkoutCartButtonPressedWithLeft;
  private Callback<MicroTxnAuthorizationResponse_t> _steamMicroTransactionAuthorizationResponse;
  private static readonly List<CosmeticsController.CosmeticSlots> _g_default_outAppliedSlotsList_for_applyCosmeticItemToSet = new List<CosmeticsController.CosmeticSlots>(16 /*0x10*/);
  [SerializeField]
  private CosmeticOutfitSystemConfig outfitSystemConfig;
  private CosmeticsController.CosmeticSet[] savedOutfits;
  private Vector3[] savedColors;
  private static CosmeticsController.OutfitData outfitDataTemp;
  private string outfitStringMothership = string.Empty;
  private string outfitStringPendingSave = string.Empty;
  private static bool saveOutfitInProgress = false;
  private static bool loadOutfitsInProgress = false;
  private static bool loadedSavedOutfits = false;
  private static int selectedOutfit = 0;
  private static readonly Vector3 defaultColor = new Vector3(0.0f, 0.0f, 0.0f);
  public Action OnOutfitsUpdated;
  public static Action<float, float, float> OnPlayerColorSet;
  private StringBuilder sb = new StringBuilder(256 /*0x0100*/);

  public CosmeticInfoV2[] v2_allCosmetics { get; private set; }

  public bool v2_allCosmeticsInfoAssetRef_isLoaded { get; private set; }

  public bool v2_isGetCosmeticsPlayCatalogDataWaitingForCallback { get; private set; }

  public bool v2_isCosmeticPlayFabCatalogDataLoaded { get; private set; }

  private void V2Awake()
  {
    this._allCosmetics = (List<CosmeticsController.CosmeticItem>) null;
    this.StartCoroutine(this.V2_allCosmeticsInfoAssetRefSO_LoadCoroutine());
  }

  private IEnumerator V2_allCosmeticsInfoAssetRefSO_LoadCoroutine()
  {
    while (!(bool) (UnityEngine.Object) PlayFabAuthenticator.instance)
      yield return (object) new WaitForSeconds(1f);
    float[] retryWaitTimes = new float[15]
    {
      1f,
      2f,
      4f,
      4f,
      10f,
      10f,
      10f,
      10f,
      10f,
      10f,
      10f,
      10f,
      10f,
      10f,
      30f
    };
    int retryCount = 0;
    AsyncOperationHandle<AllCosmeticsArraySO> newSysAllCosmeticsAsyncOp;
    while (true)
    {
      Debug.Log((object) ($"Attempting to load runtime key \"{this.v2_allCosmeticsInfoAssetRef.RuntimeKey}\" " + $"(Attempt: {retryCount + 1})"));
      newSysAllCosmeticsAsyncOp = this.v2_allCosmeticsInfoAssetRef.LoadAssetAsync();
      yield return (object) newSysAllCosmeticsAsyncOp;
      if (!ApplicationQuittingState.IsQuitting)
      {
        if (!newSysAllCosmeticsAsyncOp.IsValid())
          Debug.LogError((object) "`newSysAllCosmeticsAsyncOp` (should never happen) became invalid some how.");
        if (newSysAllCosmeticsAsyncOp.Status != AsyncOperationStatus.Succeeded)
        {
          Debug.LogError((object) $"{$"Failed to load \"{this.v2_allCosmeticsInfoAssetRef.RuntimeKey}\". "}Error: {newSysAllCosmeticsAsyncOp.OperationException.Message}");
          yield return (object) new WaitForSecondsRealtime(retryWaitTimes[Mathf.Min(retryCount, retryWaitTimes.Length - 1)]);
          ++retryCount;
          newSysAllCosmeticsAsyncOp = new AsyncOperationHandle<AllCosmeticsArraySO>();
        }
        else
          goto label_8;
      }
      else
        break;
    }
    yield break;
label_8:
    this.V2_allCosmeticsInfoAssetRef_LoadSucceeded(newSysAllCosmeticsAsyncOp.Result);
  }

  private void V2_allCosmeticsInfoAssetRef_LoadSucceeded(AllCosmeticsArraySO allCosmeticsSO)
  {
    this.v2_allCosmetics = new CosmeticInfoV2[allCosmeticsSO.sturdyAssetRefs.Length];
    for (int index = 0; index < allCosmeticsSO.sturdyAssetRefs.Length; ++index)
      this.v2_allCosmetics[index] = allCosmeticsSO.sturdyAssetRefs[index].obj.info;
    this._allCosmetics = new List<CosmeticsController.CosmeticItem>(allCosmeticsSO.sturdyAssetRefs.Length);
    for (int index = 0; index < this.v2_allCosmetics.Length; ++index)
    {
      CosmeticInfoV2 v2AllCosmetic = this.v2_allCosmetics[index];
      string playFabId = v2AllCosmetic.playFabID;
      this._allCosmeticsDictV2[playFabId] = v2AllCosmetic;
      this._allCosmetics.Add(new CosmeticsController.CosmeticItem()
      {
        itemName = playFabId,
        itemCategory = (CosmeticsController.CosmeticCategory) v2AllCosmetic.category,
        isHoldable = v2AllCosmetic.hasHoldableParts,
        displayName = playFabId,
        itemPicture = v2AllCosmetic.icon,
        overrideDisplayName = v2AllCosmetic.displayName,
        bothHandsHoldable = v2AllCosmetic.usesBothHandSlots,
        isNullItem = false
      });
    }
    this.v2_allCosmeticsInfoAssetRef_isLoaded = true;
    Action assetRefOnPostLoad = this.V2_allCosmeticsInfoAssetRef_OnPostLoad;
    if (assetRefOnPostLoad == null)
      return;
    assetRefOnPostLoad();
  }

  public bool TryGetCosmeticInfoV2(string playFabId, out CosmeticInfoV2 cosmeticInfo)
  {
    return this._allCosmeticsDictV2.TryGetValue(playFabId, out cosmeticInfo);
  }

  private void V2_ConformCosmeticItemV1DisplayName(ref CosmeticsController.CosmeticItem cosmetic)
  {
    if (cosmetic.itemName == cosmetic.displayName)
      return;
    cosmetic.overrideDisplayName = cosmetic.displayName;
    cosmetic.displayName = cosmetic.itemName;
  }

  internal void InitializeCosmeticStands()
  {
    foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
    {
      if ((UnityEngine.Object) cosmeticStand != (UnityEngine.Object) null)
        cosmeticStand.InitializeCosmetic();
    }
  }

  [field: OnEnterPlay_Set(false)]
  public static bool hasInstance { get; private set; }

  public List<CosmeticsController.CosmeticItem> allCosmetics
  {
    get => this._allCosmetics;
    set => this._allCosmetics = value;
  }

  public bool allCosmeticsDict_isInitialized { get; private set; }

  public Dictionary<string, CosmeticsController.CosmeticItem> allCosmeticsDict
  {
    get => this._allCosmeticsDict;
  }

  public bool allCosmeticsItemIDsfromDisplayNamesDict_isInitialized { get; private set; }

  public Dictionary<string, string> allCosmeticsItemIDsfromDisplayNamesDict
  {
    get => this._allCosmeticsItemIDsfromDisplayNamesDict;
  }

  public CosmeticAnchorAntiIntersectOffsets defaultClipOffsets
  {
    get => CosmeticAnchorAntiIntersectOffsets.Identity;
  }

  public bool isHidingCosmeticsFromRemotePlayers { get; private set; }

  public void AddWardrobeInstance(WardrobeInstance instance)
  {
    this.wardrobes.Add(instance);
    if (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
      return;
    this.UpdateWardrobeModelsAndButtons();
  }

  public void RemoveWardrobeInstance(WardrobeInstance instance) => this.wardrobes.Remove(instance);

  public int CurrencyBalance => this.currencyBalance;

  public void Awake()
  {
    if ((UnityEngine.Object) CosmeticsController.instance == (UnityEngine.Object) null)
    {
      CosmeticsController.instance = this;
      CosmeticsController.hasInstance = true;
    }
    else if ((UnityEngine.Object) CosmeticsController.instance != (UnityEngine.Object) this)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      return;
    }
    this.V2Awake();
    if (!this.gameObject.activeSelf)
      return;
    this.catalog = "DLC";
    this.currencyName = "SR";
    this.nullItem = new CosmeticsController.CosmeticItem();
    this.nullItem.itemName = "null";
    this.nullItem.displayName = "NOTHING";
    this.nullItem.itemPicture = Resources.Load<Sprite>("CosmeticNull_Icon");
    this.nullItem.itemPictureResourceString = "";
    this.nullItem.overrideDisplayName = "NOTHING";
    this.nullItem.meshAtlasResourceString = "";
    this.nullItem.meshResourceString = "";
    this.nullItem.materialResourceString = "";
    this.nullItem.isNullItem = true;
    this._allCosmeticsDict[this.nullItem.itemName] = this.nullItem;
    this._allCosmeticsItemIDsfromDisplayNamesDict[this.nullItem.displayName] = this.nullItem.itemName;
    for (int index = 0; index < 16 /*0x10*/; ++index)
    {
      this.tryOnSet.items[index] = this.nullItem;
      this.tempUnlockedSet.items[index] = this.nullItem;
      this.activeMergedSet.items[index] = this.nullItem;
    }
    this.cosmeticsPages[0] = 0;
    this.cosmeticsPages[1] = 0;
    this.cosmeticsPages[2] = 0;
    this.cosmeticsPages[3] = 0;
    this.cosmeticsPages[4] = 0;
    this.cosmeticsPages[5] = 0;
    this.cosmeticsPages[6] = 0;
    this.cosmeticsPages[7] = 0;
    this.cosmeticsPages[8] = 0;
    this.cosmeticsPages[9] = 0;
    this.cosmeticsPages[10] = 0;
    this.itemLists[0] = this.unlockedHats;
    this.itemLists[1] = this.unlockedFaces;
    this.itemLists[2] = this.unlockedBadges;
    this.itemLists[3] = this.unlockedPaws;
    this.itemLists[4] = this.unlockedFurs;
    this.itemLists[5] = this.unlockedShirts;
    this.itemLists[6] = this.unlockedPants;
    this.itemLists[7] = this.unlockedArms;
    this.itemLists[8] = this.unlockedBacks;
    this.itemLists[9] = this.unlockedChests;
    this.itemLists[10] = this.unlockedTagFX;
    this.updateCosmeticsRetries = 0;
    this.maxUpdateCosmeticsRetries = 5;
    this.inventoryStringList.Clear();
    this.inventoryStringList.Add("Inventory");
    this.StartCoroutine(this.CheckCanGetDaily());
  }

  public void Start()
  {
    PlayFabTitleDataCache.Instance.GetTitleData("BundleData", (Action<string>) (data => this.bundleList.FromJson(data)), (Action<PlayFabError>) (e => Debug.LogError((object) $"Error getting bundle data: {e}")));
    this.anchorOverrides = GorillaTagger.Instance.offlineVRRig.GetComponent<VRRigAnchorOverrides>();
  }

  public void OnEnable()
  {
    GorillaSlicerSimpleManager.RegisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.Update);
    if (!SteamManager.Initialized || this._steamMicroTransactionAuthorizationResponse != null)
      return;
    this._steamMicroTransactionAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(new Callback<MicroTxnAuthorizationResponse_t>.DispatchDelegate(this.ProcessSteamCallback));
  }

  public void OnDisable()
  {
    this._steamMicroTransactionAuthorizationResponse?.Unregister();
    GorillaSlicerSimpleManager.UnregisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.Update);
  }

  public void SliceUpdate()
  {
  }

  public static bool CompareCategoryToSavedCosmeticSlots(
    CosmeticsController.CosmeticCategory category,
    CosmeticsController.CosmeticSlots slot)
  {
    switch (category)
    {
      case CosmeticsController.CosmeticCategory.Hat:
        return slot == CosmeticsController.CosmeticSlots.Hat;
      case CosmeticsController.CosmeticCategory.Badge:
        return CosmeticsController.CosmeticSlots.Badge == slot;
      case CosmeticsController.CosmeticCategory.Face:
        return CosmeticsController.CosmeticSlots.Face == slot;
      case CosmeticsController.CosmeticCategory.Paw:
        return slot == CosmeticsController.CosmeticSlots.HandRight || slot == CosmeticsController.CosmeticSlots.HandLeft;
      case CosmeticsController.CosmeticCategory.Chest:
        return CosmeticsController.CosmeticSlots.Chest == slot;
      case CosmeticsController.CosmeticCategory.Fur:
        return CosmeticsController.CosmeticSlots.Fur == slot;
      case CosmeticsController.CosmeticCategory.Shirt:
        return CosmeticsController.CosmeticSlots.Shirt == slot;
      case CosmeticsController.CosmeticCategory.Back:
        return slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.BackRight;
      case CosmeticsController.CosmeticCategory.Arms:
        return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.ArmRight;
      case CosmeticsController.CosmeticCategory.Pants:
        return CosmeticsController.CosmeticSlots.Pants == slot;
      case CosmeticsController.CosmeticCategory.TagEffect:
        return CosmeticsController.CosmeticSlots.TagEffect == slot;
      default:
        return false;
    }
  }

  public static CosmeticsController.CosmeticSlots CategoryToNonTransferrableSlot(
    CosmeticsController.CosmeticCategory category)
  {
    switch (category)
    {
      case CosmeticsController.CosmeticCategory.Hat:
        return CosmeticsController.CosmeticSlots.Hat;
      case CosmeticsController.CosmeticCategory.Badge:
        return CosmeticsController.CosmeticSlots.Badge;
      case CosmeticsController.CosmeticCategory.Face:
        return CosmeticsController.CosmeticSlots.Face;
      case CosmeticsController.CosmeticCategory.Paw:
        return CosmeticsController.CosmeticSlots.HandRight;
      case CosmeticsController.CosmeticCategory.Chest:
        return CosmeticsController.CosmeticSlots.Chest;
      case CosmeticsController.CosmeticCategory.Fur:
        return CosmeticsController.CosmeticSlots.Fur;
      case CosmeticsController.CosmeticCategory.Shirt:
        return CosmeticsController.CosmeticSlots.Shirt;
      case CosmeticsController.CosmeticCategory.Back:
        return CosmeticsController.CosmeticSlots.Back;
      case CosmeticsController.CosmeticCategory.Arms:
        return CosmeticsController.CosmeticSlots.Arms;
      case CosmeticsController.CosmeticCategory.Pants:
        return CosmeticsController.CosmeticSlots.Pants;
      case CosmeticsController.CosmeticCategory.TagEffect:
        return CosmeticsController.CosmeticSlots.TagEffect;
      default:
        return CosmeticsController.CosmeticSlots.Count;
    }
  }

  private CosmeticsController.CosmeticSlots DropPositionToCosmeticSlot(
    BodyDockPositions.DropPositions pos)
  {
    switch (pos)
    {
      case BodyDockPositions.DropPositions.LeftArm:
        return CosmeticsController.CosmeticSlots.ArmLeft;
      case BodyDockPositions.DropPositions.RightArm:
        return CosmeticsController.CosmeticSlots.ArmRight;
      case BodyDockPositions.DropPositions.Chest:
        return CosmeticsController.CosmeticSlots.Chest;
      case BodyDockPositions.DropPositions.LeftBack:
        return CosmeticsController.CosmeticSlots.BackLeft;
      case BodyDockPositions.DropPositions.RightBack:
        return CosmeticsController.CosmeticSlots.BackRight;
      default:
        return CosmeticsController.CosmeticSlots.Count;
    }
  }

  private static BodyDockPositions.DropPositions CosmeticSlotToDropPosition(
    CosmeticsController.CosmeticSlots slot)
  {
    switch (slot)
    {
      case CosmeticsController.CosmeticSlots.ArmLeft:
        return BodyDockPositions.DropPositions.LeftArm;
      case CosmeticsController.CosmeticSlots.ArmRight:
        return BodyDockPositions.DropPositions.RightArm;
      case CosmeticsController.CosmeticSlots.BackLeft:
        return BodyDockPositions.DropPositions.LeftBack;
      case CosmeticsController.CosmeticSlots.BackRight:
        return BodyDockPositions.DropPositions.RightBack;
      case CosmeticsController.CosmeticSlots.Chest:
        return BodyDockPositions.DropPositions.Chest;
      default:
        return BodyDockPositions.DropPositions.None;
    }
  }

  public void AddItemCheckout(ItemCheckout newItemCheckout)
  {
    if (this.itemCheckouts.Contains(newItemCheckout))
      return;
    this.itemCheckouts.Add(newItemCheckout);
    this.UpdateShoppingCart();
    this.FormattedPurchaseText(this.finalLine, this.leftCheckoutPurchaseButtonString, this.rightCheckoutPurchaseButtonString, this.leftCheckoutPurchaseButtonOn, this.rightCheckoutPurchaseButtonOn);
    if (this.itemToBuy.isNullItem)
      return;
    this.RefreshItemToBuyPreview();
  }

  public void RemoveItemCheckout(ItemCheckout checkoutToRemove)
  {
    this.itemCheckouts.Remove(checkoutToRemove);
  }

  public void AddFittingRoom(FittingRoom newFittingRoom)
  {
    if (this.fittingRooms.Contains(newFittingRoom))
      return;
    this.fittingRooms.Add(newFittingRoom);
    this.UpdateShoppingCart();
  }

  public void RemoveFittingRoom(FittingRoom fittingRoomToRemove)
  {
    this.fittingRooms.Remove(fittingRoomToRemove);
  }

  private void SaveItemPreference(
    CosmeticsController.CosmeticSlots slot,
    int slotIdx,
    CosmeticsController.CosmeticItem newItem)
  {
    PlayerPrefs.SetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), newItem.itemName);
    PlayerPrefs.Save();
  }

  public void SaveCurrentItemPreferences()
  {
    for (int index = 0; index < 16 /*0x10*/; ++index)
      this.SaveItemPreference((CosmeticsController.CosmeticSlots) index, index, this.currentWornSet.items[index]);
  }

  private void ApplyCosmeticToSet(
    CosmeticsController.CosmeticSet set,
    CosmeticsController.CosmeticItem newItem,
    int slotIdx,
    CosmeticsController.CosmeticSlots slot,
    bool applyToPlayerPrefs,
    List<CosmeticsController.CosmeticSlots> appliedSlots)
  {
    CosmeticsController.CosmeticItem newItem1 = set.items[slotIdx].itemName == newItem.itemName ? this.nullItem : newItem;
    set.items[slotIdx] = newItem1;
    if (applyToPlayerPrefs)
      this.SaveItemPreference(slot, slotIdx, newItem1);
    appliedSlots.Add(slot);
  }

  private void PrivApplyCosmeticItemToSet(
    CosmeticsController.CosmeticSet set,
    CosmeticsController.CosmeticItem newItem,
    bool isLeftHand,
    bool applyToPlayerPrefs,
    List<CosmeticsController.CosmeticSlots> appliedSlots)
  {
    if (newItem.isNullItem)
      return;
    if (CosmeticsController.CosmeticSet.IsHoldable(newItem))
    {
      BodyDockPositions.DockingResult dockingResult = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>().ToggleWithHandedness(newItem.displayName, isLeftHand, newItem.bothHandsHoldable);
      foreach (BodyDockPositions.DropPositions pos in dockingResult.positionsDisabled)
      {
        CosmeticsController.CosmeticSlots cosmeticSlot = this.DropPositionToCosmeticSlot(pos);
        if (cosmeticSlot != CosmeticsController.CosmeticSlots.Count)
        {
          int slotIdx = (int) cosmeticSlot;
          set.items[slotIdx] = this.nullItem;
          if (applyToPlayerPrefs)
            this.SaveItemPreference(cosmeticSlot, slotIdx, this.nullItem);
        }
      }
      foreach (BodyDockPositions.DropPositions pos in dockingResult.dockedPosition)
      {
        if (pos != BodyDockPositions.DropPositions.None)
        {
          CosmeticsController.CosmeticSlots cosmeticSlot = this.DropPositionToCosmeticSlot(pos);
          int slotIdx = (int) cosmeticSlot;
          set.items[slotIdx] = newItem;
          if (applyToPlayerPrefs)
            this.SaveItemPreference(cosmeticSlot, slotIdx, newItem);
          appliedSlots.Add(cosmeticSlot);
        }
      }
    }
    else if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Paw)
    {
      CosmeticsController.CosmeticSlots slot1 = isLeftHand ? CosmeticsController.CosmeticSlots.HandLeft : CosmeticsController.CosmeticSlots.HandRight;
      int slotIdx1 = (int) slot1;
      this.ApplyCosmeticToSet(set, newItem, slotIdx1, slot1, applyToPlayerPrefs, appliedSlots);
      CosmeticsController.CosmeticSlots slot2 = CosmeticsController.CosmeticSet.OppositeSlot(slot1);
      int slotIdx2 = (int) slot2;
      if (newItem.bothHandsHoldable)
      {
        this.ApplyCosmeticToSet(set, this.nullItem, slotIdx2, slot2, applyToPlayerPrefs, appliedSlots);
      }
      else
      {
        if (set.items[slotIdx2].itemName == newItem.itemName)
          this.ApplyCosmeticToSet(set, this.nullItem, slotIdx2, slot2, applyToPlayerPrefs, appliedSlots);
        if (!set.items[slotIdx2].bothHandsHoldable)
          return;
        this.ApplyCosmeticToSet(set, this.nullItem, slotIdx2, slot2, applyToPlayerPrefs, appliedSlots);
      }
    }
    else
    {
      CosmeticsController.CosmeticSlots transferrableSlot = CosmeticsController.CategoryToNonTransferrableSlot(newItem.itemCategory);
      int slotIdx = (int) transferrableSlot;
      this.ApplyCosmeticToSet(set, newItem, slotIdx, transferrableSlot, applyToPlayerPrefs, appliedSlots);
    }
  }

  public void ApplyCosmeticItemToSet(
    CosmeticsController.CosmeticSet set,
    CosmeticsController.CosmeticItem newItem,
    bool isLeftHand,
    bool applyToPlayerPrefs)
  {
    this.ApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, CosmeticsController._g_default_outAppliedSlotsList_for_applyCosmeticItemToSet);
  }

  public void ApplyCosmeticItemToSet(
    CosmeticsController.CosmeticSet set,
    CosmeticsController.CosmeticItem newItem,
    bool isLeftHand,
    bool applyToPlayerPrefs,
    List<CosmeticsController.CosmeticSlots> outAppliedSlotsList)
  {
    outAppliedSlotsList.Clear();
    if (newItem.itemCategory == CosmeticsController.CosmeticCategory.Set)
    {
      bool flag = false;
      Dictionary<CosmeticsController.CosmeticItem, bool> dictionary = new Dictionary<CosmeticsController.CosmeticItem, bool>();
      foreach (string bundledItem in newItem.bundledItems)
      {
        CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(bundledItem);
        if (this.AnyMatch(set, itemFromDict))
        {
          flag = true;
          dictionary.Add(itemFromDict, true);
        }
        else
          dictionary.Add(itemFromDict, false);
      }
      foreach (KeyValuePair<CosmeticsController.CosmeticItem, bool> keyValuePair in dictionary)
      {
        if (flag)
        {
          if (keyValuePair.Value)
            this.PrivApplyCosmeticItemToSet(set, keyValuePair.Key, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
        }
        else
          this.PrivApplyCosmeticItemToSet(set, keyValuePair.Key, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
      }
    }
    else
      this.PrivApplyCosmeticItemToSet(set, newItem, isLeftHand, applyToPlayerPrefs, outAppliedSlotsList);
  }

  public void RemoveCosmeticItemFromSet(
    CosmeticsController.CosmeticSet set,
    string itemName,
    bool applyToPlayerPrefs)
  {
    this.cachedSet.CopyItems(set);
    for (int index = 0; index < 16 /*0x10*/; ++index)
    {
      if (set.items[index].displayName == itemName)
      {
        set.items[index] = this.nullItem;
        if (applyToPlayerPrefs)
          this.SaveItemPreference((CosmeticsController.CosmeticSlots) index, index, this.nullItem);
      }
    }
    VRRig offlineVrRig = GorillaTagger.Instance.offlineVRRig;
    BodyDockPositions component = offlineVrRig.GetComponent<BodyDockPositions>();
    set.ActivateCosmetics(this.cachedSet, offlineVrRig, component, offlineVrRig.cosmeticsObjectRegistry);
  }

  public void PressFittingRoomButton(FittingRoomButton pressedFittingRoomButton, bool isLeftHand)
  {
    BundleManager.instance._tryOnBundlesStand.ClearSelectedBundle();
    this.ApplyCosmeticItemToSet(this.tryOnSet, pressedFittingRoomButton.currentCosmeticItem, isLeftHand, false);
    this.UpdateShoppingCart();
    this.UpdateWornCosmetics(true);
  }

  public CosmeticsController.EWearingCosmeticSet CheckIfCosmeticSetMatchesItemSet(
    CosmeticsController.CosmeticSet set,
    string itemName)
  {
    CosmeticsController.EWearingCosmeticSet ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.NotASet;
    CosmeticsController.CosmeticItem cosmeticItem = this.allCosmeticsDict[itemName];
    if (cosmeticItem.bundledItems.Length != 0)
    {
      foreach (string bundledItem in cosmeticItem.bundledItems)
      {
        if (this.AnyMatch(set, this.allCosmeticsDict[bundledItem]))
        {
          switch (ewearingCosmeticSet)
          {
            case CosmeticsController.EWearingCosmeticSet.NotASet:
              ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Complete;
              continue;
            case CosmeticsController.EWearingCosmeticSet.NotWearing:
              ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Partial;
              continue;
            default:
              continue;
          }
        }
        else
        {
          switch (ewearingCosmeticSet)
          {
            case CosmeticsController.EWearingCosmeticSet.NotASet:
              ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.NotWearing;
              continue;
            case CosmeticsController.EWearingCosmeticSet.Complete:
              ewearingCosmeticSet = CosmeticsController.EWearingCosmeticSet.Partial;
              continue;
            default:
              continue;
          }
        }
      }
    }
    return ewearingCosmeticSet;
  }

  public void PressCosmeticStandButton(CosmeticStand pressedStand)
  {
    this.searchIndex = this.currentCart.IndexOf(pressedStand.thisCosmeticItem);
    if (this.searchIndex != -1)
    {
      GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_remove, pressedStand.thisCosmeticItem);
      this.currentCart.RemoveAt(this.searchIndex);
      pressedStand.isOn = false;
      for (int index = 0; index < 16 /*0x10*/; ++index)
      {
        if (pressedStand.thisCosmeticItem.itemName == this.tryOnSet.items[index].itemName)
          this.tryOnSet.items[index] = this.nullItem;
      }
    }
    else
    {
      GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_add, pressedStand.thisCosmeticItem);
      this.currentCart.Insert(0, pressedStand.thisCosmeticItem);
      pressedStand.isOn = true;
      if (this.currentCart.Count > this.numFittingRoomButtons)
      {
        foreach (CosmeticStand cosmeticStand in this.cosmeticStands)
        {
          if (!((UnityEngine.Object) cosmeticStand == (UnityEngine.Object) null) && cosmeticStand.thisCosmeticItem.itemName == this.currentCart[this.numFittingRoomButtons].itemName)
          {
            cosmeticStand.isOn = false;
            cosmeticStand.UpdateColor();
            break;
          }
        }
        this.currentCart.RemoveAt(this.numFittingRoomButtons);
      }
    }
    pressedStand.UpdateColor();
    this.UpdateShoppingCart();
  }

  public void PressWardrobeItemButton(
    CosmeticsController.CosmeticItem cosmeticItem,
    bool isLeftHand,
    bool isTempCosm)
  {
    if (cosmeticItem.isNullItem)
      return;
    CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(cosmeticItem.itemName);
    if (isTempCosm)
      this.PressTemporaryWardrobeItemButton(itemFromDict, isLeftHand);
    else
      this.PressWardrobeItemButton(itemFromDict, isLeftHand);
    this.UpdateWornCosmetics(true);
    Action cosmeticsUpdated = this.OnCosmeticsUpdated;
    if (cosmeticsUpdated == null)
      return;
    cosmeticsUpdated();
  }

  private void PressWardrobeItemButton(CosmeticsController.CosmeticItem item, bool isLeftHand)
  {
    List<CosmeticsController.CosmeticSlots> cosmeticSlotsList = CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Get();
    if (cosmeticSlotsList.Capacity < 16 /*0x10*/)
      cosmeticSlotsList.Capacity = 16 /*0x10*/;
    this.ApplyCosmeticItemToSet(this.currentWornSet, item, isLeftHand, true, cosmeticSlotsList);
    foreach (int index in cosmeticSlotsList)
      this.tryOnSet.items[index] = this.nullItem;
    CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Release(cosmeticSlotsList);
    this.UpdateShoppingCart();
  }

  private void PressTemporaryWardrobeItemButton(
    CosmeticsController.CosmeticItem item,
    bool isLeftHand)
  {
    this.ApplyCosmeticItemToSet(this.tempUnlockedSet, item, isLeftHand, false);
  }

  public void PressWardrobeFunctionButton(string function)
  {
    switch (function)
    {
      case "arms":
        if (this.wardrobeType == 6)
          return;
        this.wardrobeType = 6;
        break;
      case "back":
        if (this.wardrobeType == 7)
          return;
        this.wardrobeType = 7;
        break;
      case "badge":
        if (this.wardrobeType == 2)
          return;
        this.wardrobeType = 2;
        break;
      case "chest":
        if (this.wardrobeType == 8)
          return;
        this.wardrobeType = 8;
        break;
      case "face":
        if (this.wardrobeType == 1)
          return;
        this.wardrobeType = 1;
        break;
      case "fur":
        if (this.wardrobeType == 4)
          return;
        this.wardrobeType = 4;
        break;
      case "hand":
        if (this.wardrobeType == 3)
          return;
        this.wardrobeType = 3;
        break;
      case "hat":
        if (this.wardrobeType == 0)
          return;
        this.wardrobeType = 0;
        break;
      case "left":
        this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] - 1;
        if (this.cosmeticsPages[this.wardrobeType] < 0)
        {
          this.cosmeticsPages[this.wardrobeType] = (this.itemLists[this.wardrobeType].Count - 1) / 3;
          break;
        }
        break;
      case "outfit":
        if (this.wardrobeType == 5)
          return;
        this.wardrobeType = 5;
        break;
      case "reserved":
        if (this.wardrobeType == 9)
          return;
        this.wardrobeType = 9;
        break;
      case "right":
        this.cosmeticsPages[this.wardrobeType] = this.cosmeticsPages[this.wardrobeType] + 1;
        if (this.cosmeticsPages[this.wardrobeType] > (this.itemLists[this.wardrobeType].Count - 1) / 3)
        {
          this.cosmeticsPages[this.wardrobeType] = 0;
          break;
        }
        break;
      case "tagEffect":
        if (this.wardrobeType == 10)
          return;
        this.wardrobeType = 10;
        break;
    }
    this.UpdateWardrobeModelsAndButtons();
    Action cosmeticsUpdated = this.OnCosmeticsUpdated;
    if (cosmeticsUpdated == null)
      return;
    cosmeticsUpdated();
  }

  public void ClearCheckout(bool sendEvent)
  {
    if (sendEvent)
      GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_cancel, (IList<CosmeticsController.CosmeticItem>) this.currentCart);
    this.itemToBuy = this.nullItem;
    this.RefreshItemToBuyPreview();
    this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
    this.ProcessPurchaseItemState((string) null, false);
  }

  public bool RemoveItemFromCart(CosmeticsController.CosmeticItem cosmeticItem)
  {
    this.searchIndex = this.currentCart.IndexOf(cosmeticItem);
    if (this.searchIndex == -1)
      return false;
    this.currentCart.RemoveAt(this.searchIndex);
    for (int index = 0; index < 16 /*0x10*/; ++index)
    {
      if (cosmeticItem.itemName == this.tryOnSet.items[index].itemName)
        this.tryOnSet.items[index] = this.nullItem;
    }
    return true;
  }

  public void ClearCheckoutAndCart(bool sendEvent)
  {
    this.currentCart.Clear();
    this.tryOnSet.ClearSet(this.nullItem);
    this.ClearCheckout(sendEvent);
  }

  public void PressCheckoutCartButton(CheckoutCartButton pressedCheckoutCartButton, bool isLeftHand)
  {
    if (this.currentPurchaseItemStage == CosmeticsController.PurchaseItemStages.Buying)
      return;
    this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
    this.tryOnSet.ClearSet(this.nullItem);
    if (this.itemToBuy.displayName == pressedCheckoutCartButton.currentCosmeticItem.displayName)
    {
      this.itemToBuy = this.nullItem;
      this.RefreshItemToBuyPreview();
    }
    else
    {
      this.itemToBuy = pressedCheckoutCartButton.currentCosmeticItem;
      this.checkoutCartButtonPressedWithLeft = isLeftHand;
      this.RefreshItemToBuyPreview();
    }
    this.ProcessPurchaseItemState((string) null, isLeftHand);
    this.UpdateShoppingCart();
  }

  private void RefreshItemToBuyPreview()
  {
    if (this.itemToBuy.bundledItems != null && this.itemToBuy.bundledItems.Length != 0)
    {
      List<string> stringList = new List<string>();
      foreach (string bundledItem in this.itemToBuy.bundledItems)
      {
        this.tempItem = this.GetItemFromDict(bundledItem);
        stringList.Add(this.tempItem.displayName);
      }
      for (this.iterator = 0; this.iterator < this.itemCheckouts.Count; ++this.iterator)
      {
        if (!this.itemCheckouts[this.iterator].IsNull())
          this.itemCheckouts[this.iterator].checkoutHeadModel.SetCosmeticActiveArray(stringList.ToArray(), new bool[stringList.Count]);
      }
    }
    else
    {
      for (this.iterator = 0; this.iterator < this.itemCheckouts.Count; ++this.iterator)
      {
        if (!this.itemCheckouts[this.iterator].IsNull())
          this.itemCheckouts[this.iterator].checkoutHeadModel.SetCosmeticActive(this.itemToBuy.displayName);
      }
    }
    this.ApplyCosmeticItemToSet(this.tryOnSet, this.itemToBuy, this.checkoutCartButtonPressedWithLeft, false);
    this.UpdateWornCosmetics(true);
  }

  public void PressPurchaseItemButton(PurchaseItemButton pressedPurchaseItemButton, bool isLeftHand)
  {
    this.ProcessPurchaseItemState(pressedPurchaseItemButton.buttonSide, isLeftHand);
  }

  public void PurchaseBundle(StoreBundle bundleToPurchase)
  {
    if (!(bundleToPurchase.playfabBundleID != "NULL"))
      return;
    this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
    this.ProcessPurchaseItemState("left", false);
    this.buyingBundle = true;
    this.itemToPurchase = bundleToPurchase.playfabBundleID;
    this.SteamPurchase();
  }

  public void PressEarlyAccessButton()
  {
    this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
    this.ProcessPurchaseItemState("left", false);
    this.buyingBundle = true;
    this.itemToPurchase = this.BundlePlayfabItemName;
    ATM_Manager.instance.shinyRocksCost = (float) this.BundleShinyRocks;
    this.SteamPurchase();
  }

  public void PressPurchaseBundleButton(string PlayFabItemName)
  {
    BundleManager.instance.BundlePurchaseButtonPressed(PlayFabItemName);
  }

  public void ProcessPurchaseItemState(string buttonSide, bool isLeftHand)
  {
    switch (this.currentPurchaseItemStage)
    {
      case CosmeticsController.PurchaseItemStages.Start:
        this.itemToBuy = this.nullItem;
        this.FormattedPurchaseText("SELECT AN ITEM FROM YOUR CART TO PURCHASE!");
        this.UpdateShoppingCart();
        break;
      case CosmeticsController.PurchaseItemStages.CheckoutButtonPressed:
        GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.checkout_start, (IList<CosmeticsController.CosmeticItem>) this.currentCart);
        this.searchIndex = this.unlockedCosmetics.FindIndex((Predicate<CosmeticsController.CosmeticItem>) (x => this.itemToBuy.itemName == x.itemName));
        if (this.searchIndex > -1)
        {
          this.FormattedPurchaseText("YOU ALREADY OWN THIS ITEM!", "-", "-", true, true);
          this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemOwned;
          break;
        }
        if (this.itemToBuy.cost <= this.currencyBalance)
        {
          this.FormattedPurchaseText("DO YOU WANT TO BUY THIS ITEM?", "NO!", "YES!");
          this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.ItemSelected;
          break;
        }
        this.FormattedPurchaseText("INSUFFICIENT SHINY ROCKS FOR THIS ITEM!", "-", "-", true, true);
        this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Start;
        break;
      case CosmeticsController.PurchaseItemStages.ItemSelected:
        if (buttonSide == "right")
        {
          GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.item_select, this.itemToBuy);
          this.FormattedPurchaseText("ARE YOU REALLY SURE?", "YES! I NEED IT!", "LET ME THINK ABOUT IT");
          this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement;
          break;
        }
        this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
        this.ProcessPurchaseItemState((string) null, isLeftHand);
        break;
      case CosmeticsController.PurchaseItemStages.FinalPurchaseAcknowledgement:
        if (buttonSide == "left")
        {
          this.FormattedPurchaseText("PURCHASING ITEM...", "-", "-", true, true);
          this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Buying;
          this.isLastHandTouchedLeft = isLeftHand;
          this.PurchaseItem();
          break;
        }
        this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.CheckoutButtonPressed;
        this.ProcessPurchaseItemState((string) null, isLeftHand);
        break;
      case CosmeticsController.PurchaseItemStages.Success:
        this.FormattedPurchaseText("SUCCESS! ENJOY YOUR NEW ITEM!", "-", "-", true, true);
        GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed += this.itemToBuy.itemName;
        CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
        if (itemFromDict.bundledItems != null)
        {
          foreach (string bundledItem in itemFromDict.bundledItems)
            GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed += bundledItem;
        }
        this.tryOnSet.ClearSet(this.nullItem);
        this.UpdateShoppingCart();
        this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true);
        this.UpdateShoppingCart();
        this.UpdateWornCosmetics();
        this.UpdateWardrobeModelsAndButtons();
        Action cosmeticsUpdated = this.OnCosmeticsUpdated;
        if (cosmeticsUpdated == null)
          break;
        cosmeticsUpdated();
        break;
      case CosmeticsController.PurchaseItemStages.Failure:
        this.FormattedPurchaseText("ERROR IN PURCHASING ITEM! NO MONEY WAS SPENT. SELECT ANOTHER ITEM.", "-", "-", true, true);
        break;
    }
  }

  public void FormattedPurchaseText(
    string finalLineVar,
    string leftPurchaseButtonText = null,
    string rightPurchaseButtonText = null,
    bool leftButtonOn = false,
    bool rightButtonOn = false)
  {
    this.finalLine = finalLineVar;
    if (leftPurchaseButtonText != null)
    {
      this.leftCheckoutPurchaseButtonString = leftPurchaseButtonText;
      this.leftCheckoutPurchaseButtonOn = leftButtonOn;
    }
    if (rightPurchaseButtonText != null)
    {
      this.rightCheckoutPurchaseButtonString = rightPurchaseButtonText;
      this.rightCheckoutPurchaseButtonOn = rightButtonOn;
    }
    string newText = $"SELECTION: {this.GetItemDisplayName(this.itemToBuy)}\nITEM COST: {this.itemToBuy.cost.ToString()}\nYOU HAVE: {this.currencyBalance.ToString()}\n\n{this.finalLine}";
    for (this.iterator = 0; this.iterator < this.itemCheckouts.Count; ++this.iterator)
    {
      if (!this.itemCheckouts[this.iterator].IsNull())
        this.itemCheckouts[this.iterator].UpdatePurchaseText(newText, leftPurchaseButtonText, rightPurchaseButtonText, leftButtonOn, rightButtonOn);
    }
  }

  public void PurchaseItem()
  {
    PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
    {
      ItemId = this.itemToBuy.itemName,
      Price = this.itemToBuy.cost,
      VirtualCurrency = this.currencyName,
      CatalogVersion = this.catalog
    }, (Action<PurchaseItemResult>) (result =>
    {
      if (result.Items.Count > 0)
      {
        foreach (ItemInstance itemInstance in result.Items)
        {
          CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(this.itemToBuy.itemName);
          if (itemFromDict.itemCategory == CosmeticsController.CosmeticCategory.Set)
          {
            this.UnlockItem(itemInstance.ItemId);
            foreach (string bundledItem in itemFromDict.bundledItems)
              this.UnlockItem(bundledItem);
          }
          else
            this.UnlockItem(itemInstance.ItemId);
        }
        this.UpdateMyCosmetics();
        if (NetworkSystem.Instance.InRoom)
          this.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.itemToBuy.itemName));
        this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Success;
        this.currencyBalance -= this.itemToBuy.cost;
        this.UpdateShoppingCart();
        this.ProcessPurchaseItemState((string) null, this.isLastHandTouchedLeft);
      }
      else
      {
        this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
        this.ProcessPurchaseItemState((string) null, false);
      }
    }), (Action<PlayFabError>) (error =>
    {
      this.currentPurchaseItemStage = CosmeticsController.PurchaseItemStages.Failure;
      this.ProcessPurchaseItemState((string) null, false);
    }));
  }

  private void UnlockItem(string itemIdToUnlock, bool relock = false)
  {
    int index = this.allCosmetics.FindIndex((Predicate<CosmeticsController.CosmeticItem>) (x => itemIdToUnlock == x.itemName));
    if (index <= -1)
      return;
    this.ModifyUnlockList(this.unlockedCosmetics, index, relock);
    if (relock)
      this.concatStringCosmeticsAllowed.Replace(this.allCosmetics[index].itemName, string.Empty);
    else
      this.concatStringCosmeticsAllowed += this.allCosmetics[index].itemName;
    switch (this.allCosmetics[index].itemCategory)
    {
      case CosmeticsController.CosmeticCategory.Hat:
        this.ModifyUnlockList(this.unlockedHats, index, relock);
        break;
      case CosmeticsController.CosmeticCategory.Badge:
        this.ModifyUnlockList(this.unlockedBadges, index, relock);
        break;
      case CosmeticsController.CosmeticCategory.Face:
        this.ModifyUnlockList(this.unlockedFaces, index, relock);
        break;
      case CosmeticsController.CosmeticCategory.Paw:
        if (!this.allCosmetics[index].isThrowable)
        {
          this.ModifyUnlockList(this.unlockedPaws, index, relock);
          break;
        }
        this.ModifyUnlockList(this.unlockedThrowables, index, relock);
        break;
      case CosmeticsController.CosmeticCategory.Chest:
        this.ModifyUnlockList(this.unlockedChests, index, relock);
        break;
      case CosmeticsController.CosmeticCategory.Fur:
        this.ModifyUnlockList(this.unlockedFurs, index, relock);
        break;
      case CosmeticsController.CosmeticCategory.Shirt:
        this.ModifyUnlockList(this.unlockedShirts, index, relock);
        break;
      case CosmeticsController.CosmeticCategory.Back:
        this.ModifyUnlockList(this.unlockedBacks, index, relock);
        break;
      case CosmeticsController.CosmeticCategory.Arms:
        this.ModifyUnlockList(this.unlockedArms, index, relock);
        break;
      case CosmeticsController.CosmeticCategory.Pants:
        this.ModifyUnlockList(this.unlockedPants, index, relock);
        break;
      case CosmeticsController.CosmeticCategory.TagEffect:
        this.ModifyUnlockList(this.unlockedTagFX, index, relock);
        break;
      case CosmeticsController.CosmeticCategory.Set:
        foreach (string bundledItem in this.allCosmetics[index].bundledItems)
          this.UnlockItem(bundledItem);
        break;
    }
  }

  private void ModifyUnlockList(
    List<CosmeticsController.CosmeticItem> list,
    int index,
    bool relock)
  {
    if (!relock && !list.Contains(this.allCosmetics[index]))
    {
      list.Add(this.allCosmetics[index]);
    }
    else
    {
      if (!relock || !list.Contains(this.allCosmetics[index]))
        return;
      list.Remove(this.allCosmetics[index]);
    }
  }

  private IEnumerator CheckIfMyCosmeticsUpdated(string itemToBuyID)
  {
    Debug.Log((object) "Cosmetic updated check!");
    yield return (object) new WaitForSeconds(1f);
    this.foundCosmetic = false;
    this.attempts = 0;
    while (!this.foundCosmetic && this.attempts < 10 && NetworkSystem.Instance.InRoom)
    {
      this.playerIDList.Clear();
      if (this.UseNewCosmeticsPath())
      {
        this.playerIDList.Add("Inventory");
        PlayFabClientAPI.GetSharedGroupData(new PlayFab.ClientModels.GetSharedGroupDataRequest()
        {
          Keys = this.playerIDList,
          SharedGroupId = NetworkSystem.Instance.LocalPlayer.UserId + "Inventory"
        }, (Action<GetSharedGroupDataResult>) (result =>
        {
          ++this.attempts;
          foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
          {
            if (keyValuePair.Value.Value.Contains(itemToBuyID))
            {
              PhotonNetwork.RaiseEvent((byte) 199, (object) null, new RaiseEventOptions()
              {
                Receivers = ReceiverGroup.Others
              }, SendOptions.SendReliable);
              this.foundCosmetic = true;
            }
          }
          if (!this.foundCosmetic)
            return;
          this.UpdateWornCosmetics(true);
        }), (Action<PlayFabError>) (error =>
        {
          ++this.attempts;
          this.ReauthOrBan(error);
        }));
        yield return (object) new WaitForSeconds(1f);
      }
      else
      {
        this.playerIDList.Add(PhotonNetwork.LocalPlayer.ActorNumber.ToString());
        PlayFabClientAPI.GetSharedGroupData(new PlayFab.ClientModels.GetSharedGroupDataRequest()
        {
          Keys = this.playerIDList,
          SharedGroupId = NetworkSystem.Instance.RoomName + Regex.Replace(NetworkSystem.Instance.CurrentRegion, "[^a-zA-Z0-9]", "").ToUpper()
        }, (Action<GetSharedGroupDataResult>) (result =>
        {
          ++this.attempts;
          foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
          {
            if (keyValuePair.Value.Value.Contains(itemToBuyID))
            {
              NetworkSystemRaiseEvent.RaiseEvent((byte) 199, (object) null, NetworkSystemRaiseEvent.neoOthers, true);
              this.foundCosmetic = true;
            }
            else
              Debug.Log((object) ("didnt find it, updating attempts and trying again in a bit. current attempt is " + this.attempts.ToString()));
          }
          if (!this.foundCosmetic)
            return;
          this.UpdateWornCosmetics(true);
        }), (Action<PlayFabError>) (error =>
        {
          ++this.attempts;
          if (error.Error == PlayFabErrorCode.NotAuthenticated)
            PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
          else if (error.Error == PlayFabErrorCode.AccountBanned)
            GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
          Debug.Log((object) ("Got error retrieving user data, on attempt " + this.attempts.ToString()));
          Debug.Log((object) error.GenerateErrorReport());
        }));
        yield return (object) new WaitForSeconds(1f);
      }
    }
    Debug.Log((object) "done!");
  }

  public void UpdateWardrobeModelsAndButtons()
  {
    if (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
      return;
    foreach (WardrobeInstance wardrobe in this.wardrobes)
    {
      wardrobe.wardrobeItemButtons[0].currentCosmeticItem = this.cosmeticsPages[this.wardrobeType] * 3 < this.itemLists[this.wardrobeType].Count ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3] : this.nullItem;
      wardrobe.wardrobeItemButtons[1].currentCosmeticItem = this.cosmeticsPages[this.wardrobeType] * 3 + 1 < this.itemLists[this.wardrobeType].Count ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 1] : this.nullItem;
      wardrobe.wardrobeItemButtons[2].currentCosmeticItem = this.cosmeticsPages[this.wardrobeType] * 3 + 2 < this.itemLists[this.wardrobeType].Count ? this.itemLists[this.wardrobeType][this.cosmeticsPages[this.wardrobeType] * 3 + 2] : this.nullItem;
      for (this.iterator = 0; this.iterator < wardrobe.wardrobeItemButtons.Length; ++this.iterator)
      {
        CosmeticsController.CosmeticItem currentCosmeticItem = wardrobe.wardrobeItemButtons[this.iterator].currentCosmeticItem;
        wardrobe.wardrobeItemButtons[this.iterator].isOn = !currentCosmeticItem.isNullItem && this.AnyMatch(this.currentWornSet, currentCosmeticItem);
        wardrobe.wardrobeItemButtons[this.iterator].UpdateColor();
      }
      wardrobe.wardrobeItemButtons[0].controlledModel.SetCosmeticActive(wardrobe.wardrobeItemButtons[0].currentCosmeticItem.displayName);
      wardrobe.wardrobeItemButtons[1].controlledModel.SetCosmeticActive(wardrobe.wardrobeItemButtons[1].currentCosmeticItem.displayName);
      wardrobe.wardrobeItemButtons[2].controlledModel.SetCosmeticActive(wardrobe.wardrobeItemButtons[2].currentCosmeticItem.displayName);
      wardrobe.selfDoll.SetCosmeticActiveArray(this.currentWornSet.ToDisplayNameArray(), this.currentWornSet.ToOnRightSideArray());
    }
  }

  public int GetCategorySize(CosmeticsController.CosmeticCategory category)
  {
    int indexForCategory = this.GetIndexForCategory(category);
    return indexForCategory != -1 ? this.itemLists[indexForCategory].Count : 0;
  }

  public CosmeticsController.CosmeticItem GetCosmetic(int category, int cosmeticIndex)
  {
    return cosmeticIndex >= this.itemLists[category].Count || cosmeticIndex < 0 ? this.nullItem : this.itemLists[category][cosmeticIndex];
  }

  public CosmeticsController.CosmeticItem GetCosmetic(
    CosmeticsController.CosmeticCategory category,
    int cosmeticIndex)
  {
    return this.GetCosmetic(this.GetIndexForCategory(category), cosmeticIndex);
  }

  private int GetIndexForCategory(CosmeticsController.CosmeticCategory category)
  {
    switch (category)
    {
      case CosmeticsController.CosmeticCategory.Hat:
        return 0;
      case CosmeticsController.CosmeticCategory.Badge:
        return 2;
      case CosmeticsController.CosmeticCategory.Face:
        return 1;
      case CosmeticsController.CosmeticCategory.Paw:
        return 3;
      case CosmeticsController.CosmeticCategory.Chest:
        return 9;
      case CosmeticsController.CosmeticCategory.Fur:
        return 4;
      case CosmeticsController.CosmeticCategory.Shirt:
        return 5;
      case CosmeticsController.CosmeticCategory.Back:
        return 8;
      case CosmeticsController.CosmeticCategory.Arms:
        return 7;
      case CosmeticsController.CosmeticCategory.Pants:
        return 6;
      case CosmeticsController.CosmeticCategory.TagEffect:
        return 10;
      default:
        return 0;
    }
  }

  public bool IsCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic)
  {
    return this.AnyMatch(this.currentWornSet, cosmetic);
  }

  public bool IsCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic, bool tempSet)
  {
    return !tempSet ? this.IsCosmeticEquipped(cosmetic) : this.IsTemporaryCosmeticEquipped(cosmetic);
  }

  public bool IsTemporaryCosmeticEquipped(CosmeticsController.CosmeticItem cosmetic)
  {
    return this.AnyMatch(this.tempUnlockedSet, cosmetic);
  }

  public CosmeticsController.CosmeticItem GetSlotItem(
    CosmeticsController.CosmeticSlots slot,
    bool checkOpposite = true,
    bool tempSet = false)
  {
    int index = (int) slot;
    if (checkOpposite)
      index = (int) CosmeticsController.CosmeticSet.OppositeSlot(slot);
    return !tempSet ? this.currentWornSet.items[index] : this.tempUnlockedSet.items[index];
  }

  public string[] GetCurrentlyWornCosmetics(bool tempSet = false)
  {
    return !tempSet ? this.currentWornSet.ToDisplayNameArray() : this.tempUnlockedSet.ToDisplayNameArray();
  }

  public bool[] GetCurrentRightEquippedSided(bool tempSet = false)
  {
    return !tempSet ? this.currentWornSet.ToOnRightSideArray() : this.tempUnlockedSet.ToOnRightSideArray();
  }

  public void UpdateShoppingCart()
  {
    for (this.iterator = 0; this.iterator < this.itemCheckouts.Count; ++this.iterator)
    {
      if (!this.itemCheckouts[this.iterator].IsNull())
        this.itemCheckouts[this.iterator].UpdateFromCart(this.currentCart, this.itemToBuy);
    }
    for (this.iterator = 0; this.iterator < this.fittingRooms.Count; ++this.iterator)
    {
      if (!this.fittingRooms[this.iterator].IsNull())
        this.fittingRooms[this.iterator].UpdateFromCart(this.currentCart, this.tryOnSet);
    }
    if (!CosmeticsV2Spawner_Dirty.allPartsInstantiated)
      return;
    this.UpdateWardrobeModelsAndButtons();
  }

  public void UpdateWornCosmetics() => this.UpdateWornCosmetics(false, false);

  public void UpdateWornCosmetics(bool sync) => this.UpdateWornCosmetics(sync, false);

  public void UpdateWornCosmetics(bool sync, bool playfx)
  {
    VRRig localRig = VRRig.LocalRig;
    this.activeMergedSet.MergeInSets(this.currentWornSet, this.tempUnlockedSet, (Predicate<string>) (id => PlayerCosmeticsSystem.IsTemporaryCosmeticAllowed(localRig, id)));
    GorillaTagger.Instance.offlineVRRig.LocalUpdateCosmeticsWithTryon(this.activeMergedSet, this.tryOnSet, playfx);
    if (!sync || !((UnityEngine.Object) GorillaTagger.Instance.myVRRig != (UnityEngine.Object) null))
      return;
    if (this.isHidingCosmeticsFromRemotePlayers)
      GorillaTagger.Instance.myVRRig.SendRPC("RPC_HideAllCosmetics", RpcTarget.All);
    else
      GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.Others, (object) this.activeMergedSet.ToPackedIDArray(), (object) this.tryOnSet.ToPackedIDArray(), (object) playfx);
  }

  public CosmeticsController.CosmeticItem GetItemFromDict(string itemID)
  {
    return !this.allCosmeticsDict.TryGetValue(itemID, out this.cosmeticItemVar) ? this.nullItem : this.cosmeticItemVar;
  }

  public string GetItemNameFromDisplayName(string displayName)
  {
    return !this.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(displayName, out this.returnString) ? "null" : this.returnString;
  }

  public CosmeticSO GetCosmeticSOFromDisplayName(string displayName)
  {
    string nameFromDisplayName = this.GetItemNameFromDisplayName(displayName);
    if (nameFromDisplayName.Equals("null"))
      return (CosmeticSO) null;
    AllCosmeticsArraySO asset = this.v2_allCosmeticsInfoAssetRef.Asset as AllCosmeticsArraySO;
    if ((UnityEngine.Object) asset == (UnityEngine.Object) null)
    {
      GTDev.LogWarning<string>("null AllCosmeticsArraySO");
      return (CosmeticSO) null;
    }
    CosmeticSO soFromDisplayName = asset.SearchForCosmeticSO(nameFromDisplayName);
    if ((UnityEngine.Object) soFromDisplayName != (UnityEngine.Object) null)
      return soFromDisplayName;
    GTDev.Log<string>("Could not find cosmetic info for " + nameFromDisplayName);
    return (CosmeticSO) null;
  }

  public CosmeticAnchorAntiIntersectOffsets GetClipOffsetsFromDisplayName(string displayName)
  {
    string nameFromDisplayName = this.GetItemNameFromDisplayName(displayName);
    if (nameFromDisplayName.Equals("null"))
      return this.defaultClipOffsets;
    AllCosmeticsArraySO asset = this.v2_allCosmeticsInfoAssetRef.Asset as AllCosmeticsArraySO;
    if ((UnityEngine.Object) asset == (UnityEngine.Object) null)
    {
      GTDev.LogWarning<string>("null AllCosmeticsArraySO");
      return this.defaultClipOffsets;
    }
    CosmeticSO cosmeticSo = asset.SearchForCosmeticSO(nameFromDisplayName);
    if ((UnityEngine.Object) cosmeticSo != (UnityEngine.Object) null)
      return cosmeticSo.info.anchorAntiIntersectOffsets;
    GTDev.Log<string>("Could not find cosmetic info for " + nameFromDisplayName);
    return this.defaultClipOffsets;
  }

  public bool AnyMatch(CosmeticsController.CosmeticSet set, CosmeticsController.CosmeticItem item)
  {
    if (item.itemCategory != CosmeticsController.CosmeticCategory.Set)
      return set.IsActive(item.displayName);
    if (item.bundledItems.Length == 1)
      return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0]));
    if (item.bundledItems.Length == 2)
      return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1]));
    if (item.bundledItems.Length < 3)
      return false;
    return this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[0])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[1])) || this.AnyMatch(set, this.GetItemFromDict(item.bundledItems[2]));
  }

  public void Initialize()
  {
    if (!this.gameObject.activeSelf || this.v2_isCosmeticPlayFabCatalogDataLoaded || this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback)
      return;
    if (this.v2_allCosmeticsInfoAssetRef_isLoaded)
    {
      this.GetCosmeticsPlayFabCatalogData();
    }
    else
    {
      this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback = true;
      this.V2_allCosmeticsInfoAssetRef_OnPostLoad += new Action(this.GetCosmeticsPlayFabCatalogData);
    }
  }

  public void GetLastDailyLogin()
  {
    PlayFabClientAPI.GetUserReadOnlyData(new PlayFab.ClientModels.GetUserDataRequest(), (Action<GetUserDataResult>) (result =>
    {
      if (result.Data.TryGetValue("DailyLogin", out this.userDataRecord))
      {
        this.lastDailyLogin = this.userDataRecord.Value;
      }
      else
      {
        this.lastDailyLogin = "NONE";
        this.StartCoroutine(this.GetMyDaily());
      }
    }), (Action<PlayFabError>) (error =>
    {
      Debug.Log((object) "Got error getting read-only user data:");
      Debug.Log((object) error.GenerateErrorReport());
      this.lastDailyLogin = "FAILED";
      if (error.Error == PlayFabErrorCode.NotAuthenticated)
      {
        PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
      }
      else
      {
        if (error.Error != PlayFabErrorCode.AccountBanned)
          return;
        Application.Quit();
        NetworkSystem.Instance.ReturnToSinglePlayer();
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) PhotonNetworkController.Instance);
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) GTPlayer.Instance);
        foreach (UnityEngine.Object @object in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
          UnityEngine.Object.Destroy(@object);
      }
    }));
  }

  private IEnumerator CheckCanGetDaily()
  {
    CosmeticsController cosmeticsController = this;
    while (!KIDManager.InitialisationComplete)
      yield return (object) new WaitForSeconds(1f);
    while (true)
    {
      while (!((UnityEngine.Object) GorillaComputer.instance != (UnityEngine.Object) null) || GorillaComputer.instance.startupMillis == 0L)
        yield return (object) new WaitForSeconds(1f);
      cosmeticsController.currentTime = new DateTime((GorillaComputer.instance.startupMillis + (long) ((double) Time.realtimeSinceStartup * 1000.0)) * 10000L);
      cosmeticsController.secondsUntilTomorrow = (int) (cosmeticsController.currentTime.AddDays(1.0).Date - cosmeticsController.currentTime).TotalSeconds;
      if (cosmeticsController.lastDailyLogin == null || cosmeticsController.lastDailyLogin == "")
        cosmeticsController.GetLastDailyLogin();
      else if (cosmeticsController.currentTime.ToString("o").Substring(0, 10) == cosmeticsController.lastDailyLogin)
      {
        cosmeticsController.checkedDaily = true;
        cosmeticsController.gotMyDaily = true;
      }
      else if (cosmeticsController.currentTime.ToString("o").Substring(0, 10) != cosmeticsController.lastDailyLogin)
      {
        cosmeticsController.checkedDaily = true;
        cosmeticsController.gotMyDaily = false;
        cosmeticsController.StartCoroutine(cosmeticsController.GetMyDaily());
      }
      else if (cosmeticsController.lastDailyLogin == "FAILED")
        cosmeticsController.GetLastDailyLogin();
      cosmeticsController.secondsToWaitToCheckDaily = cosmeticsController.checkedDaily ? 60f : 10f;
      cosmeticsController.UpdateCurrencyBoards();
      yield return (object) new WaitForSeconds(cosmeticsController.secondsToWaitToCheckDaily);
    }
  }

  private IEnumerator GetMyDaily()
  {
    CosmeticsController cosmeticsController = this;
    yield return (object) new WaitForSeconds(10f);
    // ISSUE: reference to a compiler-generated method
    GorillaServer.Instance.TryDistributeCurrency(new Action<ExecuteFunctionResult>(cosmeticsController.\u003CGetMyDaily\u003Eb__214_0), (Action<PlayFabError>) (error =>
    {
      if (error.Error == PlayFabErrorCode.NotAuthenticated)
      {
        PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
      }
      else
      {
        if (error.Error != PlayFabErrorCode.AccountBanned)
          return;
        Application.Quit();
        NetworkSystem.Instance.ReturnToSinglePlayer();
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) PhotonNetworkController.Instance);
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) GTPlayer.Instance);
        foreach (UnityEngine.Object @object in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
          UnityEngine.Object.Destroy(@object);
      }
    }));
  }

  public void GetCosmeticsPlayFabCatalogData()
  {
    this.v2_isGetCosmeticsPlayCatalogDataWaitingForCallback = false;
    if (!this.v2_allCosmeticsInfoAssetRef_isLoaded)
      throw new Exception("Method `GetCosmeticsPlayFabCatalogData` was called before `v2_allCosmeticsInfoAssetRef` was loaded. Listen to callback `V2_allCosmeticsInfoAssetRef_OnPostLoad` or check `v2_allCosmeticsInfoAssetRef_isLoaded` before trying to get PlayFab catalog data.");
    PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), (Action<GetUserInventoryResult>) (result => PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest()
    {
      CatalogVersion = this.catalog
    }, (Action<GetCatalogItemsResult>) (result2 =>
    {
      this.unlockedCosmetics.Clear();
      this.unlockedHats.Clear();
      this.unlockedBadges.Clear();
      this.unlockedFaces.Clear();
      this.unlockedPaws.Clear();
      this.unlockedFurs.Clear();
      this.unlockedShirts.Clear();
      this.unlockedPants.Clear();
      this.unlockedArms.Clear();
      this.unlockedBacks.Clear();
      this.unlockedChests.Clear();
      this.unlockedTagFX.Clear();
      this.unlockedThrowables.Clear();
      this.catalogItems = result2.Catalog;
      foreach (CatalogItem catalogItem1 in this.catalogItems)
      {
        CatalogItem catalogItem = catalogItem1;
        if (!BuilderSetManager.IsItemIDBuilderItem(catalogItem.ItemId))
        {
          this.searchIndex = this.allCosmetics.FindIndex((Predicate<CosmeticsController.CosmeticItem>) (x => catalogItem.ItemId == x.itemName));
          if (this.searchIndex > -1)
          {
            this.tempStringArray = (string[]) null;
            this.hasPrice = false;
            if (catalogItem.Bundle != null)
              this.tempStringArray = catalogItem.Bundle.BundledItems.ToArray();
            uint num;
            if (catalogItem.VirtualCurrencyPrices.TryGetValue(this.currencyName, out num))
              this.hasPrice = true;
            CosmeticsController.CosmeticItem allCosmetic = this.allCosmetics[this.searchIndex] with
            {
              itemName = catalogItem.ItemId,
              displayName = catalogItem.DisplayName,
              cost = (int) num,
              bundledItems = this.tempStringArray,
              canTryOn = this.hasPrice
            };
            if (allCosmetic.itemCategory == CosmeticsController.CosmeticCategory.Paw)
            {
              CosmeticInfoV2 v2AllCosmetic = this.v2_allCosmetics[this.searchIndex];
              allCosmetic.isThrowable = v2AllCosmetic.isThrowable && !v2AllCosmetic.hasWardrobeParts;
            }
            if (allCosmetic.displayName == null)
            {
              string str = "null";
              if ((bool) (UnityEngine.Object) this.allCosmetics[this.searchIndex].itemPicture)
                str = this.allCosmetics[this.searchIndex].itemPicture.name;
              string debugCosmeticSoName = this.v2_allCosmetics[this.searchIndex].debugCosmeticSOName;
              Debug.LogError((object) $"{$"Cosmetic encountered with a null displayName at index {this.searchIndex}! "}Setting displayName to id: \"{this.allCosmetics[this.searchIndex].itemName}\". iconName=\"{str}\".cosmeticSOName=\"{debugCosmeticSoName}\". ");
              allCosmetic.displayName = allCosmetic.itemName;
            }
            this.V2_ConformCosmeticItemV1DisplayName(ref allCosmetic);
            this._allCosmetics[this.searchIndex] = allCosmetic;
            this._allCosmeticsDict[allCosmetic.itemName] = allCosmetic;
            this._allCosmeticsItemIDsfromDisplayNamesDict[allCosmetic.displayName] = allCosmetic.itemName;
            this._allCosmeticsItemIDsfromDisplayNamesDict[allCosmetic.overrideDisplayName] = allCosmetic.itemName;
          }
        }
      }
      for (int index = this._allCosmetics.Count - 1; index > -1; --index)
      {
        this.tempItem = this._allCosmetics[index];
        if (this.tempItem.itemCategory == CosmeticsController.CosmeticCategory.Set && this.tempItem.canTryOn)
        {
          foreach (string bundledItem in this.tempItem.bundledItems)
          {
            string setItemName = bundledItem;
            this.searchIndex = this._allCosmetics.FindIndex((Predicate<CosmeticsController.CosmeticItem>) (x => setItemName == x.itemName));
            if (this.searchIndex > -1)
            {
              this.tempItem = this._allCosmetics[this.searchIndex];
              this.tempItem.canTryOn = true;
              this._allCosmetics[this.searchIndex] = this.tempItem;
              this._allCosmeticsDict[this._allCosmetics[this.searchIndex].itemName] = this.tempItem;
              this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this.tempItem.itemName;
            }
          }
        }
      }
      foreach (KeyValuePair<string, StoreBundle> keyValuePair in BundleManager.instance.storeBundlesById)
      {
        string str;
        StoreBundle storeBundle;
        keyValuePair.Deconstruct(ref str, ref storeBundle);
        string key = str;
        StoreBundle bundleData = storeBundle;
        int index1 = this._allCosmetics.FindIndex((Predicate<CosmeticsController.CosmeticItem>) (x => bundleData.playfabBundleID == x.itemName));
        if (index1 > 0 && this._allCosmetics[index1].bundledItems != null)
        {
          foreach (string bundledItem in this._allCosmetics[index1].bundledItems)
          {
            string setItemName = bundledItem;
            this.searchIndex = this._allCosmetics.FindIndex((Predicate<CosmeticsController.CosmeticItem>) (x => setItemName == x.itemName));
            if (this.searchIndex > -1)
            {
              this.tempItem = this._allCosmetics[this.searchIndex];
              this.tempItem.canTryOn = true;
              this._allCosmetics[this.searchIndex] = this.tempItem;
              this._allCosmeticsDict[this._allCosmetics[this.searchIndex].itemName] = this.tempItem;
              this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this.tempItem.itemName;
            }
          }
        }
        if (!bundleData.HasPrice)
        {
          int index2 = this.catalogItems.FindIndex((Predicate<CatalogItem>) (ci => ci.Bundle != null && ci.ItemId == bundleData.playfabBundleID));
          if (index2 > 0)
          {
            uint bundlePrice;
            if (this.catalogItems[index2].VirtualCurrencyPrices.TryGetValue("RM", out bundlePrice))
              BundleManager.instance.storeBundlesById[key].TryUpdatePrice(bundlePrice);
            else
              BundleManager.instance.storeBundlesById[key].TryUpdatePrice();
          }
        }
      }
      this.searchIndex = this._allCosmetics.FindIndex((Predicate<CosmeticsController.CosmeticItem>) (x => "Slingshot" == x.itemName));
      this._allCosmeticsDict["Slingshot"] = this.searchIndex >= 0 ? this._allCosmetics[this.searchIndex] : throw new MissingReferenceException("CosmeticsController: Cannot find default slingshot! it is required for players that do not have another slingshot equipped and are playing Paintbrawl.");
      this._allCosmeticsItemIDsfromDisplayNamesDict[this._allCosmetics[this.searchIndex].displayName] = this._allCosmetics[this.searchIndex].itemName;
      this.allCosmeticsDict_isInitialized = true;
      this.allCosmeticsItemIDsfromDisplayNamesDict_isInitialized = true;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (ItemInstance itemInstance in result.Inventory)
      {
        ItemInstance item = itemInstance;
        if (!BuilderSetManager.IsItemIDBuilderItem(item.ItemId))
        {
          if (item.ItemId == this.m_earlyAccessSupporterPackCosmeticSO.info.playFabID)
          {
            foreach (CosmeticSO setCosmetic in this.m_earlyAccessSupporterPackCosmeticSO.info.setCosmetics)
            {
              CosmeticsController.CosmeticItem cosmeticItem;
              if (this.allCosmeticsDict.TryGetValue(setCosmetic.info.playFabID, out cosmeticItem))
                this.unlockedCosmetics.Add(cosmeticItem);
            }
          }
          BundleManager.instance.MarkBundleOwnedByPlayFabID(item.ItemId);
          if (!dictionary.ContainsKey(item.ItemId))
          {
            this.searchIndex = this.allCosmetics.FindIndex((Predicate<CosmeticsController.CosmeticItem>) (x => item.ItemId == x.itemName));
            if (this.searchIndex > -1)
            {
              dictionary[item.ItemId] = item.ItemId;
              this.unlockedCosmetics.Add(this.allCosmetics[this.searchIndex]);
            }
          }
        }
      }
      foreach (CosmeticsController.CosmeticItem unlockedCosmetic in this.unlockedCosmetics)
      {
        if (unlockedCosmetic.itemCategory == CosmeticsController.CosmeticCategory.Hat && !this.unlockedHats.Contains(unlockedCosmetic))
          this.unlockedHats.Add(unlockedCosmetic);
        else if (unlockedCosmetic.itemCategory == CosmeticsController.CosmeticCategory.Face && !this.unlockedFaces.Contains(unlockedCosmetic))
          this.unlockedFaces.Add(unlockedCosmetic);
        else if (unlockedCosmetic.itemCategory == CosmeticsController.CosmeticCategory.Badge && !this.unlockedBadges.Contains(unlockedCosmetic))
          this.unlockedBadges.Add(unlockedCosmetic);
        else if (unlockedCosmetic.itemCategory == CosmeticsController.CosmeticCategory.Paw)
        {
          if (!unlockedCosmetic.isThrowable && !this.unlockedPaws.Contains(unlockedCosmetic))
            this.unlockedPaws.Add(unlockedCosmetic);
          else if (unlockedCosmetic.isThrowable && !this.unlockedThrowables.Contains(unlockedCosmetic))
            this.unlockedThrowables.Add(unlockedCosmetic);
        }
        else if (unlockedCosmetic.itemCategory == CosmeticsController.CosmeticCategory.Fur && !this.unlockedFurs.Contains(unlockedCosmetic))
          this.unlockedFurs.Add(unlockedCosmetic);
        else if (unlockedCosmetic.itemCategory == CosmeticsController.CosmeticCategory.Shirt && !this.unlockedShirts.Contains(unlockedCosmetic))
          this.unlockedShirts.Add(unlockedCosmetic);
        else if (unlockedCosmetic.itemCategory == CosmeticsController.CosmeticCategory.Arms && !this.unlockedArms.Contains(unlockedCosmetic))
          this.unlockedArms.Add(unlockedCosmetic);
        else if (unlockedCosmetic.itemCategory == CosmeticsController.CosmeticCategory.Back && !this.unlockedBacks.Contains(unlockedCosmetic))
          this.unlockedBacks.Add(unlockedCosmetic);
        else if (unlockedCosmetic.itemCategory == CosmeticsController.CosmeticCategory.Chest && !this.unlockedChests.Contains(unlockedCosmetic))
          this.unlockedChests.Add(unlockedCosmetic);
        else if (unlockedCosmetic.itemCategory == CosmeticsController.CosmeticCategory.Pants && !this.unlockedPants.Contains(unlockedCosmetic))
          this.unlockedPants.Add(unlockedCosmetic);
        else if (unlockedCosmetic.itemCategory == CosmeticsController.CosmeticCategory.TagEffect && !this.unlockedTagFX.Contains(unlockedCosmetic))
          this.unlockedTagFX.Add(unlockedCosmetic);
        this.concatStringCosmeticsAllowed += unlockedCosmetic.itemName;
      }
      BuilderSetManager.instance.OnGotInventoryItems(result, result2);
      this.currencyBalance = result.VirtualCurrency[this.currencyName];
      int num1;
      this.playedInBeta = result.VirtualCurrency.TryGetValue("TC", out num1) && num1 > 0;
      Action onGetCurrency = this.OnGetCurrency;
      if (onGetCurrency != null)
        onGetCurrency();
      BundleManager.instance.CheckIfBundlesOwned();
      StoreUpdater.instance.Initialize();
      this.currentWornSet.LoadFromPlayerPreferences(this);
      this.LoadSavedOutfits();
      if (!ATM_Manager.instance.alreadyBegan)
      {
        ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Begin);
        ATM_Manager.instance.alreadyBegan = true;
      }
      this.ProcessPurchaseItemState((string) null, false);
      this.UpdateShoppingCart();
      this.UpdateCurrencyBoards();
      if (this.UseNewCosmeticsPath())
        this.ConfirmIndividualCosmeticsSharedGroup(result);
      Action cosmeticsUpdated = this.OnCosmeticsUpdated;
      if (cosmeticsUpdated != null)
        cosmeticsUpdated();
      this.v2_isCosmeticPlayFabCatalogDataLoaded = true;
      Action catalogDataPostSuccess = this.V2_OnGetCosmeticsPlayFabCatalogData_PostSuccess;
      if (catalogDataPostSuccess != null)
        catalogDataPostSuccess();
      if (CosmeticsV2Spawner_Dirty.startedAllPartsInstantiated || CosmeticsV2Spawner_Dirty.allPartsInstantiated)
        return;
      CosmeticsV2Spawner_Dirty.StartInstantiatingPrefabs();
    }), (Action<PlayFabError>) (error =>
    {
      if (error.Error == PlayFabErrorCode.NotAuthenticated)
        PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
      else if (error.Error == PlayFabErrorCode.AccountBanned)
      {
        Application.Quit();
        NetworkSystem.Instance.ReturnToSinglePlayer();
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) PhotonNetworkController.Instance);
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) GTPlayer.Instance);
        foreach (UnityEngine.Object @object in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
          UnityEngine.Object.Destroy(@object);
      }
      if (this.tryTwice)
        return;
      this.tryTwice = true;
      this.GetCosmeticsPlayFabCatalogData();
    }))), (Action<PlayFabError>) (error =>
    {
      if (error.Error == PlayFabErrorCode.NotAuthenticated)
        PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
      else if (error.Error == PlayFabErrorCode.AccountBanned)
      {
        Application.Quit();
        NetworkSystem.Instance.ReturnToSinglePlayer();
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) PhotonNetworkController.Instance);
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) GTPlayer.Instance);
        foreach (UnityEngine.Object @object in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
          UnityEngine.Object.Destroy(@object);
      }
      if (this.tryTwice)
        return;
      this.tryTwice = true;
      this.GetCosmeticsPlayFabCatalogData();
    }));
  }

  public void SteamPurchase()
  {
    if (string.IsNullOrEmpty(this.itemToPurchase))
    {
      Debug.Log((object) "Unable to start steam purchase process. itemToPurchase is not set.");
    }
    else
    {
      Debug.Log((object) $"attempting to purchase item through steam. Is this a bundle purchase: {this.buyingBundle}");
      PlayFabClientAPI.StartPurchase(this.GetStartPurchaseRequest(), new Action<StartPurchaseResult>(this.ProcessStartPurchaseResponse), new Action<PlayFabError>(this.ProcessSteamPurchaseError));
    }
  }

  private StartPurchaseRequest GetStartPurchaseRequest()
  {
    return new StartPurchaseRequest()
    {
      CatalogVersion = this.catalog,
      Items = new List<ItemPurchaseRequest>()
      {
        new ItemPurchaseRequest()
        {
          ItemId = this.itemToPurchase,
          Quantity = 1U,
          Annotation = "Purchased via in-game store"
        }
      }
    };
  }

  private void ProcessStartPurchaseResponse(StartPurchaseResult result)
  {
    Debug.Log((object) "successfully started purchase. attempted to pay for purchase through steam");
    this.currentPurchaseID = result.OrderId;
    PlayFabClientAPI.PayForPurchase(CosmeticsController.GetPayForPurchaseRequest(this.currentPurchaseID), new Action<PayForPurchaseResult>(CosmeticsController.ProcessPayForPurchaseResult), new Action<PlayFabError>(this.ProcessSteamPurchaseError));
  }

  private static PayForPurchaseRequest GetPayForPurchaseRequest(string orderId)
  {
    return new PayForPurchaseRequest()
    {
      OrderId = orderId,
      ProviderName = "Steam",
      Currency = "RM"
    };
  }

  private static void ProcessPayForPurchaseResult(PayForPurchaseResult result)
  {
    Debug.Log((object) "succeeded on sending request for paying with steam! waiting for response");
  }

  private void ProcessSteamCallback(MicroTxnAuthorizationResponse_t callBackResponse)
  {
    Debug.Log((object) "Steam has called back that the user has finished the payment interaction");
    if (callBackResponse.m_bAuthorized == (byte) 0)
      Debug.Log((object) "Steam has indicated that the payment was not authorised.");
    if (this.buyingBundle)
      PlayFabClientAPI.ConfirmPurchase(this.GetConfirmBundlePurchaseRequest(), (Action<ConfirmPurchaseResult>) (_ => this.ProcessConfirmPurchaseSuccess()), new Action<PlayFabError>(this.ProcessConfirmPurchaseError));
    else
      PlayFabClientAPI.ConfirmPurchase(this.GetConfirmATMPurchaseRequest(), (Action<ConfirmPurchaseResult>) (_ => this.ProcessConfirmPurchaseSuccess()), new Action<PlayFabError>(this.ProcessConfirmPurchaseError));
  }

  private ConfirmPurchaseRequest GetConfirmBundlePurchaseRequest()
  {
    return new ConfirmPurchaseRequest()
    {
      OrderId = this.currentPurchaseID
    };
  }

  private ConfirmPurchaseRequest GetConfirmATMPurchaseRequest()
  {
    return new ConfirmPurchaseRequest()
    {
      OrderId = this.currentPurchaseID,
      CustomTags = new Dictionary<string, string>()
      {
        {
          "NexusCreatorId",
          ATM_Manager.instance.ValidatedCreatorCode
        },
        {
          "PlayerName",
          GorillaComputer.instance.savedName
        },
        {
          "Location",
          GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone.ToString()
        }
      }
    };
  }

  private void ProcessConfirmPurchaseSuccess()
  {
    if (this.buyingBundle)
    {
      this.buyingBundle = false;
      if (PhotonNetwork.InRoom)
        NetworkSystemRaiseEvent.RaiseEvent((byte) 9, (object) new object[0], NetworkSystemRaiseEvent.newWeb, true);
      this.StartCoroutine(this.CheckIfMyCosmeticsUpdated(this.BundlePlayfabItemName));
    }
    else
      ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Success);
    this.GetCurrencyBalance();
    this.UpdateCurrencyBoards();
    this.GetCosmeticsPlayFabCatalogData();
    GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
  }

  private void ProcessConfirmPurchaseError(PlayFabError error)
  {
    this.ProcessSteamPurchaseError(error);
    ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Failure);
    this.UpdateCurrencyBoards();
  }

  private void ProcessSteamPurchaseError(PlayFabError error)
  {
    switch (error.Error)
    {
      case PlayFabErrorCode.AccountBanned:
        PhotonNetwork.Disconnect();
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) PhotonNetworkController.Instance);
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) GTPlayer.Instance);
        foreach (UnityEngine.Object @object in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
          UnityEngine.Object.Destroy(@object);
        Application.Quit();
        break;
      case PlayFabErrorCode.FailedByPaymentProvider:
        Debug.Log((object) $"Attempted to pay for order, but has been Failed by Steam with error: {error}");
        break;
      case PlayFabErrorCode.InsufficientFunds:
        Debug.Log((object) $"Attempting to do purchase through steam, steam has returned insufficient funds: {error}");
        break;
      case PlayFabErrorCode.InvalidPaymentProvider:
        Debug.Log((object) $"Attempted to connect to steam as payment provider, but received error: {error}");
        break;
      case PlayFabErrorCode.PurchaseInitializationFailure:
      case PlayFabErrorCode.InvalidPurchaseTransactionStatus:
      case PlayFabErrorCode.DuplicatePurchaseTransactionId:
        Debug.Log((object) $"Attempted to pay for order {this.currentPurchaseID}, however received an error: {error}");
        break;
      case PlayFabErrorCode.NotAuthenticated:
        PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
        break;
      case PlayFabErrorCode.PurchaseDoesNotExist:
        Debug.Log((object) $"Attempting to confirm purchase for order {this.currentPurchaseID} but received error: {error}");
        break;
      case PlayFabErrorCode.InternalServerError:
        Debug.Log((object) $"PlayFab threw an internal server error: {error}");
        break;
      case PlayFabErrorCode.StoreNotFound:
        Debug.Log((object) $"Attempted to load {this.itemToPurchase} from {this.catalog} but received an error: {error}");
        break;
      default:
        Debug.Log((object) $"Steam purchase flow returned error: {error}");
        break;
    }
    ATM_Manager.instance.SwitchToStage(ATM_Manager.ATMStages.Failure);
  }

  public void UpdateCurrencyBoards()
  {
    this.FormattedPurchaseText(this.finalLine);
    for (this.iterator = 0; this.iterator < this.currencyBoards.Count; ++this.iterator)
    {
      if (this.currencyBoards[this.iterator].IsNotNull())
        this.currencyBoards[this.iterator].UpdateCurrencyBoard(this.checkedDaily, this.gotMyDaily, this.currencyBalance, this.secondsUntilTomorrow);
    }
  }

  public void AddCurrencyBoard(CurrencyBoard newCurrencyBoard)
  {
    if (this.currencyBoards.Contains(newCurrencyBoard))
      return;
    this.currencyBoards.Add(newCurrencyBoard);
    newCurrencyBoard.UpdateCurrencyBoard(this.checkedDaily, this.gotMyDaily, this.currencyBalance, this.secondsUntilTomorrow);
  }

  public void RemoveCurrencyBoard(CurrencyBoard currencyBoardToRemove)
  {
    this.currencyBoards.Remove(currencyBoardToRemove);
  }

  public void GetCurrencyBalance()
  {
    PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), (Action<GetUserInventoryResult>) (result =>
    {
      this.currencyBalance = result.VirtualCurrency[this.currencyName];
      this.UpdateCurrencyBoards();
      Action onGetCurrency = this.OnGetCurrency;
      if (onGetCurrency == null)
        return;
      onGetCurrency();
    }), (Action<PlayFabError>) (error =>
    {
      if (error.Error == PlayFabErrorCode.NotAuthenticated)
      {
        PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
      }
      else
      {
        if (error.Error != PlayFabErrorCode.AccountBanned)
          return;
        Application.Quit();
        NetworkSystem.Instance.ReturnToSinglePlayer();
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) PhotonNetworkController.Instance);
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) GTPlayer.Instance);
        foreach (UnityEngine.Object @object in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
          UnityEngine.Object.Destroy(@object);
      }
    }));
  }

  public string GetItemDisplayName(CosmeticsController.CosmeticItem item)
  {
    return item.overrideDisplayName != null && item.overrideDisplayName != "" ? item.overrideDisplayName : item.displayName;
  }

  public void UpdateMyCosmetics()
  {
    if (NetworkSystem.Instance.InRoom)
    {
      if ((UnityEngine.Object) GorillaServer.Instance != (UnityEngine.Object) null && GorillaServer.Instance.NewCosmeticsPathShouldSetSharedGroupData())
        this.UpdateMyCosmeticsForRoom(true);
      if (!((UnityEngine.Object) GorillaServer.Instance != (UnityEngine.Object) null) || !GorillaServer.Instance.NewCosmeticsPathShouldSetRoomData())
        return;
      this.UpdateMyCosmeticsForRoom(false);
    }
    else
    {
      if (!((UnityEngine.Object) GorillaServer.Instance != (UnityEngine.Object) null) || !GorillaServer.Instance.NewCosmeticsPathShouldSetSharedGroupData())
        return;
      this.UpdateMyCosmeticsNotInRoom();
    }
  }

  private void UpdateMyCosmeticsNotInRoom()
  {
    if (!((UnityEngine.Object) GorillaServer.Instance != (UnityEngine.Object) null))
      return;
    GorillaServer.Instance.UpdateUserCosmetics();
  }

  private void UpdateMyCosmeticsForRoom(bool shouldSetSharedGroupData)
  {
    byte eventCode = 9;
    if (shouldSetSharedGroupData)
      eventCode = (byte) 10;
    RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
    raiseEventOptions.Flags = new WebFlags((byte) 3);
    object[] eventContent = new object[0];
    PhotonNetwork.RaiseEvent(eventCode, (object) eventContent, raiseEventOptions, SendOptions.SendReliable);
  }

  private void AlreadyOwnAllBundleButtons()
  {
    foreach (EarlyAccessButton earlyAccessButton in this.earlyAccessButtons)
      earlyAccessButton.AlreadyOwn();
  }

  private bool UseNewCosmeticsPath()
  {
    return (UnityEngine.Object) GorillaServer.Instance != (UnityEngine.Object) null && GorillaServer.Instance.NewCosmeticsPathShouldReadSharedGroupData();
  }

  public void CheckCosmeticsSharedGroup()
  {
    ++this.updateCosmeticsRetries;
    if (this.updateCosmeticsRetries >= this.maxUpdateCosmeticsRetries)
      return;
    this.StartCoroutine(this.WaitForNextCosmeticsAttempt());
  }

  private IEnumerator WaitForNextCosmeticsAttempt()
  {
    yield return (object) new WaitForSeconds((float) (int) Mathf.Pow(3f, (float) (this.updateCosmeticsRetries + 1)));
    this.ConfirmIndividualCosmeticsSharedGroup(this.latestInventory);
  }

  private void ConfirmIndividualCosmeticsSharedGroup(GetUserInventoryResult inventory)
  {
    Debug.Log((object) "confirming individual cosmetics with shared group");
    this.latestInventory = inventory;
    if (PhotonNetwork.LocalPlayer.UserId == null)
      this.StartCoroutine(this.WaitForNextCosmeticsAttempt());
    else
      PlayFabClientAPI.GetSharedGroupData(new PlayFab.ClientModels.GetSharedGroupDataRequest()
      {
        Keys = this.inventoryStringList,
        SharedGroupId = PhotonNetwork.LocalPlayer.UserId + "Inventory"
      }, (Action<GetSharedGroupDataResult>) (result =>
      {
        bool flag = true;
        foreach (KeyValuePair<string, PlayFab.ClientModels.SharedGroupDataRecord> keyValuePair in result.Data)
        {
          if (!(keyValuePair.Key != "Inventory"))
          {
            foreach (ItemInstance itemInstance in inventory.Inventory)
            {
              if (itemInstance.CatalogVersion == CosmeticsController.instance.catalog && !keyValuePair.Value.Value.Contains(itemInstance.ItemId))
              {
                flag = false;
                break;
              }
            }
          }
          else
            break;
        }
        if (!flag || result.Data.Count == 0)
          this.UpdateMyCosmetics();
        else
          this.updateCosmeticsRetries = 0;
      }), (Action<PlayFabError>) (error =>
      {
        this.ReauthOrBan(error);
        this.CheckCosmeticsSharedGroup();
      }));
  }

  public void ReauthOrBan(PlayFabError error)
  {
    if (error.Error == PlayFabErrorCode.NotAuthenticated)
    {
      PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
    }
    else
    {
      if (error.Error != PlayFabErrorCode.AccountBanned)
        return;
      Application.Quit();
      PhotonNetwork.Disconnect();
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) PhotonNetworkController.Instance);
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) GTPlayer.Instance);
      foreach (UnityEngine.Object @object in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        UnityEngine.Object.Destroy(@object);
    }
  }

  public void ProcessExternalUnlock(string itemID, bool autoEquip, bool isLeftHand)
  {
    this.UnlockItem(itemID);
    GorillaTagger.Instance.offlineVRRig.concatStringOfCosmeticsAllowed += itemID;
    this.UpdateMyCosmetics();
    if (!autoEquip)
      return;
    CosmeticsController.CosmeticItem itemFromDict = this.GetItemFromDict(itemID);
    GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.external_item_claim, itemFromDict);
    List<CosmeticsController.CosmeticSlots> cosmeticSlotsList = CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Get();
    if (cosmeticSlotsList.Capacity < 16 /*0x10*/)
      cosmeticSlotsList.Capacity = 16 /*0x10*/;
    this.ApplyCosmeticItemToSet(this.currentWornSet, itemFromDict, isLeftHand, true, cosmeticSlotsList);
    foreach (int index in cosmeticSlotsList)
      this.tryOnSet.items[index] = this.nullItem;
    CollectionPool<List<CosmeticsController.CosmeticSlots>, CosmeticsController.CosmeticSlots>.Release(cosmeticSlotsList);
    this.UpdateShoppingCart();
    this.UpdateWornCosmetics(true);
    Action cosmeticsUpdated = this.OnCosmeticsUpdated;
    if (cosmeticsUpdated == null)
      return;
    cosmeticsUpdated();
  }

  public void AddTempUnlockToWardrobe(string cosmeticID)
  {
    int index = this.allCosmetics.FindIndex((Predicate<CosmeticsController.CosmeticItem>) (x => cosmeticID == x.itemName));
    if (index < 0)
      return;
    switch (this.allCosmetics[index].itemCategory)
    {
      case CosmeticsController.CosmeticCategory.Hat:
        this.ModifyUnlockList(this.unlockedHats, index, false);
        break;
      case CosmeticsController.CosmeticCategory.Badge:
        this.ModifyUnlockList(this.unlockedBadges, index, false);
        break;
      case CosmeticsController.CosmeticCategory.Face:
        this.ModifyUnlockList(this.unlockedFaces, index, false);
        break;
      case CosmeticsController.CosmeticCategory.Paw:
        if (!this.allCosmetics[index].isThrowable)
        {
          this.ModifyUnlockList(this.unlockedPaws, index, false);
          break;
        }
        this.ModifyUnlockList(this.unlockedThrowables, index, false);
        break;
      case CosmeticsController.CosmeticCategory.Chest:
        this.ModifyUnlockList(this.unlockedChests, index, false);
        break;
      case CosmeticsController.CosmeticCategory.Fur:
        this.ModifyUnlockList(this.unlockedFurs, index, false);
        break;
      case CosmeticsController.CosmeticCategory.Shirt:
        this.ModifyUnlockList(this.unlockedShirts, index, false);
        break;
      case CosmeticsController.CosmeticCategory.Back:
        this.ModifyUnlockList(this.unlockedBacks, index, false);
        break;
      case CosmeticsController.CosmeticCategory.Arms:
        this.ModifyUnlockList(this.unlockedArms, index, false);
        break;
      case CosmeticsController.CosmeticCategory.Pants:
        this.ModifyUnlockList(this.unlockedPants, index, false);
        break;
      case CosmeticsController.CosmeticCategory.TagEffect:
        this.ModifyUnlockList(this.unlockedTagFX, index, false);
        break;
      case CosmeticsController.CosmeticCategory.Set:
        foreach (string bundledItem in this.allCosmetics[index].bundledItems)
          this.AddTempUnlockToWardrobe(bundledItem);
        break;
    }
    Action cosmeticsUpdated = this.OnCosmeticsUpdated;
    if (cosmeticsUpdated == null)
      return;
    cosmeticsUpdated();
  }

  public void RemoveTempUnlockFromWardrobe(string cosmeticID)
  {
    int index = this.allCosmetics.FindIndex((Predicate<CosmeticsController.CosmeticItem>) (x => cosmeticID == x.itemName));
    if (index < 0)
      return;
    switch (this.allCosmetics[index].itemCategory)
    {
      case CosmeticsController.CosmeticCategory.Hat:
        this.ModifyUnlockList(this.unlockedHats, index, true);
        break;
      case CosmeticsController.CosmeticCategory.Badge:
        this.ModifyUnlockList(this.unlockedBadges, index, true);
        break;
      case CosmeticsController.CosmeticCategory.Face:
        this.ModifyUnlockList(this.unlockedFaces, index, true);
        break;
      case CosmeticsController.CosmeticCategory.Paw:
        if (!this.allCosmetics[index].isThrowable)
        {
          this.ModifyUnlockList(this.unlockedPaws, index, true);
          break;
        }
        this.ModifyUnlockList(this.unlockedThrowables, index, true);
        break;
      case CosmeticsController.CosmeticCategory.Chest:
        this.ModifyUnlockList(this.unlockedChests, index, true);
        break;
      case CosmeticsController.CosmeticCategory.Fur:
        this.ModifyUnlockList(this.unlockedFurs, index, true);
        break;
      case CosmeticsController.CosmeticCategory.Shirt:
        this.ModifyUnlockList(this.unlockedShirts, index, true);
        break;
      case CosmeticsController.CosmeticCategory.Back:
        this.ModifyUnlockList(this.unlockedBacks, index, true);
        break;
      case CosmeticsController.CosmeticCategory.Arms:
        this.ModifyUnlockList(this.unlockedArms, index, true);
        break;
      case CosmeticsController.CosmeticCategory.Pants:
        this.ModifyUnlockList(this.unlockedPants, index, true);
        break;
      case CosmeticsController.CosmeticCategory.TagEffect:
        this.ModifyUnlockList(this.unlockedTagFX, index, true);
        break;
      case CosmeticsController.CosmeticCategory.Set:
        foreach (string bundledItem in this.allCosmetics[index].bundledItems)
          this.RemoveTempUnlockFromWardrobe(bundledItem);
        break;
    }
    Action cosmeticsUpdated = this.OnCosmeticsUpdated;
    if (cosmeticsUpdated == null)
      return;
    cosmeticsUpdated();
  }

  public bool BuildValidationCheck()
  {
    if (!((UnityEngine.Object) this.m_earlyAccessSupporterPackCosmeticSO == (UnityEngine.Object) null))
      return true;
    Debug.LogError((object) "m_earlyAccessSupporterPackCosmeticSO is empty, everything will break!");
    return false;
  }

  public void SetHideCosmeticsFromRemotePlayers(bool hideCosmetics)
  {
    if (hideCosmetics == this.isHidingCosmeticsFromRemotePlayers)
      return;
    this.isHidingCosmeticsFromRemotePlayers = hideCosmetics;
    GorillaTagger.Instance.offlineVRRig.reliableState.SetIsDirty();
    this.UpdateWornCosmetics(true);
  }

  public bool ValidatePackedItems(int[] packed)
  {
    if (packed.Length == 0)
      return true;
    int num1 = 0;
    int num2 = packed[0];
    for (int index = 0; index < 16 /*0x10*/; ++index)
    {
      if ((num2 & 1 << index) != 0)
        ++num1;
    }
    return packed.Length == num1 + 1;
  }

  public static int SelectedOutfit => CosmeticsController.selectedOutfit;

  public static bool CanScrollOutfits()
  {
    return CosmeticsController.loadedSavedOutfits && !CosmeticsController.saveOutfitInProgress;
  }

  public void PressWardrobeScrollOutfit(bool forward)
  {
    int selectedOutfit = CosmeticsController.selectedOutfit;
    int newOutfitIndex;
    if (forward)
    {
      newOutfitIndex = (selectedOutfit + 1) % this.outfitSystemConfig.maxOutfits;
    }
    else
    {
      newOutfitIndex = selectedOutfit - 1;
      if (newOutfitIndex < 0)
        newOutfitIndex = this.outfitSystemConfig.maxOutfits - 1;
    }
    this.LoadSavedOutfit(newOutfitIndex);
  }

  public void LoadSavedOutfit(int newOutfitIndex)
  {
    if (!CosmeticsController.CanScrollOutfits() || newOutfitIndex == CosmeticsController.selectedOutfit || newOutfitIndex < 0 || newOutfitIndex >= this.outfitSystemConfig.maxOutfits)
      return;
    this.savedOutfits[CosmeticsController.selectedOutfit].CopyItems(this.currentWornSet);
    this.savedColors[CosmeticsController.selectedOutfit] = new Vector3(VRRig.LocalRig.playerColor.r, VRRig.LocalRig.playerColor.g, VRRig.LocalRig.playerColor.b);
    this.SaveOutfitsToMothership();
    CosmeticsController.selectedOutfit = newOutfitIndex;
    PlayerPrefs.SetInt(this.outfitSystemConfig.selectedOutfitPref, CosmeticsController.selectedOutfit);
    PlayerPrefs.Save();
    CosmeticsController.CosmeticSet savedOutfit = this.savedOutfits[CosmeticsController.selectedOutfit];
    bool flag = true;
    for (int i = 0; i < 16 /*0x10*/; ++i)
    {
      CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots) i;
      if ((cosmeticSlots == CosmeticsController.CosmeticSlots.ArmLeft ? 1 : (cosmeticSlots == CosmeticsController.CosmeticSlots.ArmRight ? 1 : 0)) == 0 | flag)
        this.ApplyNewItem(savedOutfit, i);
    }
    this.UpdateMonkeColor(this.savedColors[CosmeticsController.selectedOutfit], true);
    this.SaveCurrentItemPreferences();
    this.UpdateShoppingCart();
    this.UpdateWornCosmetics(true, true);
    this.UpdateWardrobeModelsAndButtons();
    Action cosmeticsUpdated = this.OnCosmeticsUpdated;
    if (cosmeticsUpdated == null)
      return;
    cosmeticsUpdated();
  }

  private void ApplyNewItem(CosmeticsController.CosmeticSet outfit, int i)
  {
    this.currentWornSet.items[i] = outfit.items[i];
    if (outfit.items[i].isNullItem)
      return;
    this.tryOnSet.items[i] = this.nullItem;
  }

  private void LoadSavedOutfits()
  {
    if (CosmeticsController.loadedSavedOutfits || CosmeticsController.loadOutfitsInProgress)
      return;
    CosmeticsController.loadOutfitsInProgress = true;
    this.savedOutfits = new CosmeticsController.CosmeticSet[this.outfitSystemConfig.maxOutfits];
    this.savedColors = new Vector3[this.outfitSystemConfig.maxOutfits];
    if (MothershipClientApiUnity.GetUserDataValue(this.outfitSystemConfig.mothershipKey, new Action<MothershipUserData>(this.GetSavedOutfitsSuccess), new Action<MothershipError, int>(this.GetSavedOutfitsFail)))
      return;
    GTDev.LogError<string>("CosmeticsController LoadSavedOutfits GetUserDataValue failed");
    this.ClearOutfits();
    CosmeticsController.loadOutfitsInProgress = false;
    CosmeticsController.loadedSavedOutfits = true;
    Action onOutfitsUpdated = this.OnOutfitsUpdated;
    if (onOutfitsUpdated == null)
      return;
    onOutfitsUpdated();
  }

  private void GetSavedOutfitsSuccess(MothershipUserData response)
  {
    if ((response == null || response.value == null ? 0 : (response.value.Length > 0 ? 1 : 0)) != 0)
    {
      try
      {
        this.outfitStringMothership = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(response.value));
        this.StringToOutfits(this.outfitStringMothership);
      }
      catch (Exception ex)
      {
        GTDev.LogError<string>("CosmeticsController GetSavedOutfitsSuccess error decoding " + ex.Message);
        this.ClearOutfits();
      }
    }
    else
      this.ClearOutfits();
    this.GetSavedOutfitsComplete();
  }

  private void GetSavedOutfitsFail(MothershipError error, int status)
  {
    GTDev.LogError<string>($"CosmeticsController GetSavedOutfitsFail {status} {error.Message}");
    this.ClearOutfits();
    this.GetSavedOutfitsComplete();
  }

  private void GetSavedOutfitsComplete()
  {
    int index = PlayerPrefs.GetInt(this.outfitSystemConfig.selectedOutfitPref, 0);
    if (index < 0 || index >= this.outfitSystemConfig.maxOutfits)
    {
      index = 0;
    }
    else
    {
      CosmeticsController.CosmeticSet other = new CosmeticsController.CosmeticSet();
      other.LoadFromPlayerPreferences(this);
      if (other.HasAnyItems())
        this.savedOutfits[index].CopyItems(other);
      float x = PlayerPrefs.GetFloat("redValue", 0.0f);
      float y = PlayerPrefs.GetFloat("greenValue", 0.0f);
      float z = PlayerPrefs.GetFloat("blueValue", 0.0f);
      if ((double) x > 0.0 || (double) y > 0.0 || (double) z > 0.0)
        this.savedColors[index] = new Vector3(x, y, z);
    }
    CosmeticsController.selectedOutfit = index;
    this.currentWornSet.CopyItems(this.savedOutfits[CosmeticsController.selectedOutfit]);
    this.UpdateMonkeColor(this.savedColors[CosmeticsController.selectedOutfit], true);
    CosmeticsController.loadedSavedOutfits = true;
    CosmeticsController.loadOutfitsInProgress = false;
    Action onOutfitsUpdated = this.OnOutfitsUpdated;
    if (onOutfitsUpdated == null)
      return;
    onOutfitsUpdated();
  }

  private void UpdateMonkeColor(Vector3 col, bool saveToPrefs)
  {
    float red = Mathf.Clamp(col.x, 0.0f, 1f);
    float green = Mathf.Clamp(col.y, 0.0f, 1f);
    float blue = Mathf.Clamp(col.z, 0.0f, 1f);
    GorillaTagger.Instance.UpdateColor(red, green, blue);
    GorillaComputer.instance.UpdateColor(red, green, blue);
    if (CosmeticsController.OnPlayerColorSet != null)
      CosmeticsController.OnPlayerColorSet(red, green, blue);
    if (NetworkSystem.Instance.InRoom)
      GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, (object) red, (object) green, (object) blue);
    if (!saveToPrefs)
      return;
    PlayerPrefs.SetFloat("redValue", red);
    PlayerPrefs.SetFloat("greenValue", green);
    PlayerPrefs.SetFloat("blueValue", blue);
    PlayerPrefs.Save();
  }

  private void SaveOutfitsToMothership()
  {
    if (!CosmeticsController.loadedSavedOutfits || CosmeticsController.saveOutfitInProgress)
      return;
    string mothershipKey = this.outfitSystemConfig.mothershipKey;
    this.outfitStringPendingSave = this.OutfitsToString();
    if (this.outfitStringPendingSave.Equals(this.outfitStringMothership))
      return;
    CosmeticsController.saveOutfitInProgress = true;
    if (MothershipClientApiUnity.SetUserDataValue(mothershipKey, this.outfitStringPendingSave, new Action<SetUserDataResponse>(this.SaveOutfitsToMothershipSuccess), new Action<MothershipError, int>(this.SaveOutfitsToMothershipFail)))
      return;
    GTDev.LogError<string>("CosmeticsController SaveOutfitToMothership SetUserDataValue failed");
    CosmeticsController.saveOutfitInProgress = false;
  }

  private void SaveOutfitsToMothershipSuccess(SetUserDataResponse response)
  {
    this.outfitStringMothership = this.outfitStringPendingSave;
    CosmeticsController.saveOutfitInProgress = false;
    Action onOutfitsUpdated = this.OnOutfitsUpdated;
    if (onOutfitsUpdated != null)
      onOutfitsUpdated();
    response.Dispose();
  }

  private void SaveOutfitsToMothershipFail(MothershipError error, int status)
  {
    GTDev.LogError<string>($"CosmeticsController SaveOutfitsToMothershipFail {status} " + error.Message);
    CosmeticsController.saveOutfitInProgress = false;
  }

  private string OutfitsToString()
  {
    if (!CosmeticsController.loadedSavedOutfits)
      return string.Empty;
    CosmeticsController.outfitDataTemp = new CosmeticsController.OutfitData();
    this.sb.Clear();
    for (int index1 = 0; index1 < this.savedOutfits.Length; ++index1)
    {
      CosmeticsController.outfitDataTemp.Clear();
      CosmeticsController.CosmeticSet savedOutfit = this.savedOutfits[index1];
      for (int index2 = 0; index2 < savedOutfit.items.Length; ++index2)
      {
        CosmeticsController.CosmeticItem cosmeticItem = savedOutfit.items[index2];
        string str = cosmeticItem.isNullItem || string.IsNullOrEmpty(cosmeticItem.displayName) ? "null" : cosmeticItem.displayName;
        CosmeticsController.outfitDataTemp.itemIDs.Add(str);
      }
      if ((UnityEngine.Object) VRRig.LocalRig != (UnityEngine.Object) null)
        CosmeticsController.outfitDataTemp.color = this.savedColors[index1];
      this.sb.Append(JsonUtility.ToJson((object) CosmeticsController.outfitDataTemp));
      if (index1 < this.savedOutfits.Length - 1)
        this.sb.Append(this.outfitSystemConfig.outfitSeparator);
    }
    return this.sb.ToString();
  }

  private void ClearOutfits()
  {
    for (int index = 0; index < this.savedOutfits.Length; ++index)
    {
      this.savedOutfits[index] = new CosmeticsController.CosmeticSet();
      this.savedOutfits[index].ClearSet(this.nullItem);
      this.savedColors[index] = CosmeticsController.defaultColor;
    }
  }

  private void StringToOutfits(string response)
  {
    if (response.IsNullOrEmpty())
    {
      this.ClearOutfits();
    }
    else
    {
      try
      {
        string[] strArray = response.Split(this.outfitSystemConfig.outfitSeparator, StringSplitOptions.None);
        for (int index = 0; index < this.outfitSystemConfig.maxOutfits; ++index)
        {
          this.savedOutfits[index] = new CosmeticsController.CosmeticSet();
          if (index >= strArray.Length)
          {
            this.savedOutfits[index].ClearSet(this.nullItem);
            this.savedColors[index] = CosmeticsController.defaultColor;
          }
          else
          {
            string str = strArray[index];
            if (str.IsNullOrEmpty())
            {
              this.savedOutfits[index].ClearSet(this.nullItem);
              this.savedColors[index] = CosmeticsController.defaultColor;
            }
            else
            {
              Vector3 color;
              this.savedOutfits[index].ParseSetFromString(this, str, out color);
              this.savedColors[index] = color;
            }
          }
        }
      }
      catch (Exception ex)
      {
        GTDev.LogError<string>("CosmeticsController StringToOutfit Error parsing " + ex.Message);
        this.ClearOutfits();
      }
    }
  }

  public enum PurchaseItemStages
  {
    Start,
    CheckoutButtonPressed,
    ItemSelected,
    ItemOwned,
    FinalPurchaseAcknowledgement,
    Buying,
    Success,
    Failure,
  }

  public enum CosmeticCategory
  {
    None,
    Hat,
    Badge,
    Face,
    Paw,
    Chest,
    Fur,
    Shirt,
    Back,
    Arms,
    Pants,
    TagEffect,
    Count,
    Set,
  }

  public enum CosmeticSlots
  {
    Hat,
    Badge,
    Face,
    ArmLeft,
    ArmRight,
    BackLeft,
    BackRight,
    HandLeft,
    HandRight,
    Chest,
    Fur,
    Shirt,
    Pants,
    Back,
    Arms,
    TagEffect,
    Count,
  }

  [Serializable]
  public class CosmeticSet
  {
    public CosmeticsController.CosmeticItem[] items;
    public string[] returnArray = new string[16 /*0x10*/];
    private static int[][] intArrays = new int[22][]
    {
      new int[0],
      new int[1],
      new int[2],
      new int[3],
      new int[4],
      new int[5],
      new int[6],
      new int[7],
      new int[8],
      new int[9],
      new int[10],
      new int[11],
      new int[12],
      new int[13],
      new int[14],
      new int[15],
      new int[16 /*0x10*/],
      new int[17],
      new int[18],
      new int[19],
      new int[20],
      new int[21]
    };
    private static CosmeticsController.CosmeticSet _emptySet;
    private static char[] nameScratchSpace = new char[6];

    public event CosmeticsController.CosmeticSet.OnSetActivatedHandler onSetActivatedEvent;

    protected void OnSetActivated(
      CosmeticsController.CosmeticSet prevSet,
      CosmeticsController.CosmeticSet currentSet,
      NetPlayer netPlayer)
    {
      if (this.onSetActivatedEvent == null)
        return;
      this.onSetActivatedEvent(prevSet, currentSet, netPlayer);
    }

    public static CosmeticsController.CosmeticSet EmptySet
    {
      get
      {
        if (CosmeticsController.CosmeticSet._emptySet == null)
        {
          string[] itemNames = new string[16 /*0x10*/];
          for (int index = 0; index < itemNames.Length; ++index)
            itemNames[index] = "NOTHING";
          CosmeticsController.CosmeticSet._emptySet = new CosmeticsController.CosmeticSet(itemNames, CosmeticsController.instance);
        }
        return CosmeticsController.CosmeticSet._emptySet;
      }
    }

    public CosmeticSet() => this.items = new CosmeticsController.CosmeticItem[16 /*0x10*/];

    public CosmeticSet(string[] itemNames, CosmeticsController controller)
    {
      this.items = new CosmeticsController.CosmeticItem[16 /*0x10*/];
      for (int index = 0; index < itemNames.Length; ++index)
      {
        string itemName = itemNames[index];
        string nameFromDisplayName = controller.GetItemNameFromDisplayName(itemName);
        this.items[index] = controller.GetItemFromDict(nameFromDisplayName);
      }
    }

    public CosmeticSet(int[] itemNamesPacked, CosmeticsController controller)
    {
      this.items = new CosmeticsController.CosmeticItem[16 /*0x10*/];
      int num1 = itemNamesPacked.Length != 0 ? itemNamesPacked[0] : 0;
      int index1 = 1;
      for (int index2 = 0; index2 < this.items.Length; ++index2)
      {
        if ((num1 & 1 << index2) != 0)
        {
          int num2 = itemNamesPacked[index1];
          CosmeticsController.CosmeticSet.nameScratchSpace[0] = (char) (65 + num2 % 26);
          CosmeticsController.CosmeticSet.nameScratchSpace[1] = (char) (65 + num2 / 26 % 26);
          CosmeticsController.CosmeticSet.nameScratchSpace[2] = (char) (65 + num2 / 676 % 26);
          CosmeticsController.CosmeticSet.nameScratchSpace[3] = (char) (65 + num2 / 17576 % 26);
          CosmeticsController.CosmeticSet.nameScratchSpace[4] = (char) (65 + num2 / 456976 % 26);
          CosmeticsController.CosmeticSet.nameScratchSpace[5] = '.';
          this.items[index2] = controller.GetItemFromDict(new string(CosmeticsController.CosmeticSet.nameScratchSpace));
          ++index1;
        }
        else
          this.items[index2] = controller.GetItemFromDict("null");
      }
    }

    public void CopyItems(CosmeticsController.CosmeticSet other)
    {
      for (int index = 0; index < this.items.Length; ++index)
        this.items[index] = other.items[index];
    }

    public void MergeSets(
      CosmeticsController.CosmeticSet tryOn,
      CosmeticsController.CosmeticSet current)
    {
      for (int index = 0; index < 16 /*0x10*/; ++index)
        this.items[index] = tryOn != null ? (tryOn.items[index].isNullItem ? current.items[index] : tryOn.items[index]) : current.items[index];
    }

    public void MergeInSets(
      CosmeticsController.CosmeticSet playerPref,
      CosmeticsController.CosmeticSet tempOverrideSet,
      Predicate<string> predicate)
    {
      int num = 16 /*0x10*/;
      for (int index = 0; index < num; ++index)
      {
        bool flag = predicate(tempOverrideSet.items[index].itemName);
        this.items[index] = flag ? tempOverrideSet.items[index] : playerPref.items[index];
      }
    }

    public void ClearSet(CosmeticsController.CosmeticItem nullItem)
    {
      for (int index = 0; index < 16 /*0x10*/; ++index)
        this.items[index] = nullItem;
    }

    public bool IsActive(string name)
    {
      int num = 16 /*0x10*/;
      for (int index = 0; index < num; ++index)
      {
        if (this.items[index].displayName == name)
          return true;
      }
      return false;
    }

    public bool HasItemOfCategory(CosmeticsController.CosmeticCategory category)
    {
      int num = 16 /*0x10*/;
      for (int index = 0; index < num; ++index)
      {
        if (!this.items[index].isNullItem && this.items[index].itemCategory == category)
          return true;
      }
      return false;
    }

    public bool HasItem(string name)
    {
      int num = 16 /*0x10*/;
      for (int index = 0; index < num; ++index)
      {
        if (!this.items[index].isNullItem && this.items[index].displayName == name)
          return true;
      }
      return false;
    }

    public bool HasAnyItems()
    {
      if (this.items == null || this.items.Length < 1)
        return false;
      for (int index = 0; index < this.items.Length; ++index)
      {
        if (!this.items[index].isNullItem)
          return true;
      }
      return false;
    }

    public static bool IsSlotLeftHanded(CosmeticsController.CosmeticSlots slot)
    {
      return slot == CosmeticsController.CosmeticSlots.ArmLeft || slot == CosmeticsController.CosmeticSlots.BackLeft || slot == CosmeticsController.CosmeticSlots.HandLeft;
    }

    public static bool IsSlotRightHanded(CosmeticsController.CosmeticSlots slot)
    {
      return slot == CosmeticsController.CosmeticSlots.ArmRight || slot == CosmeticsController.CosmeticSlots.BackRight || slot == CosmeticsController.CosmeticSlots.HandRight;
    }

    public static bool IsHoldable(CosmeticsController.CosmeticItem item) => item.isHoldable;

    public static CosmeticsController.CosmeticSlots OppositeSlot(
      CosmeticsController.CosmeticSlots slot)
    {
      switch (slot)
      {
        case CosmeticsController.CosmeticSlots.Hat:
          return CosmeticsController.CosmeticSlots.Hat;
        case CosmeticsController.CosmeticSlots.Badge:
          return CosmeticsController.CosmeticSlots.Badge;
        case CosmeticsController.CosmeticSlots.Face:
          return CosmeticsController.CosmeticSlots.Face;
        case CosmeticsController.CosmeticSlots.ArmLeft:
          return CosmeticsController.CosmeticSlots.ArmRight;
        case CosmeticsController.CosmeticSlots.ArmRight:
          return CosmeticsController.CosmeticSlots.ArmLeft;
        case CosmeticsController.CosmeticSlots.BackLeft:
          return CosmeticsController.CosmeticSlots.BackRight;
        case CosmeticsController.CosmeticSlots.BackRight:
          return CosmeticsController.CosmeticSlots.BackLeft;
        case CosmeticsController.CosmeticSlots.HandLeft:
          return CosmeticsController.CosmeticSlots.HandRight;
        case CosmeticsController.CosmeticSlots.HandRight:
          return CosmeticsController.CosmeticSlots.HandLeft;
        case CosmeticsController.CosmeticSlots.Chest:
          return CosmeticsController.CosmeticSlots.Chest;
        case CosmeticsController.CosmeticSlots.Fur:
          return CosmeticsController.CosmeticSlots.Fur;
        case CosmeticsController.CosmeticSlots.Shirt:
          return CosmeticsController.CosmeticSlots.Shirt;
        case CosmeticsController.CosmeticSlots.Pants:
          return CosmeticsController.CosmeticSlots.Pants;
        case CosmeticsController.CosmeticSlots.Back:
          return CosmeticsController.CosmeticSlots.Back;
        case CosmeticsController.CosmeticSlots.Arms:
          return CosmeticsController.CosmeticSlots.Arms;
        case CosmeticsController.CosmeticSlots.TagEffect:
          return CosmeticsController.CosmeticSlots.TagEffect;
        default:
          return CosmeticsController.CosmeticSlots.Count;
      }
    }

    public static string SlotPlayerPreferenceName(CosmeticsController.CosmeticSlots slot)
    {
      return "slot_" + slot.ToString();
    }

    private void ActivateCosmetic(
      CosmeticsController.CosmeticSet prevSet,
      VRRig rig,
      int slotIndex,
      CosmeticItemRegistry cosmeticsObjectRegistry,
      BodyDockPositions bDock)
    {
      CosmeticsController.CosmeticItem cosmeticItem1 = prevSet.items[slotIndex];
      string nameFromDisplayName1 = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem1.displayName);
      CosmeticsController.CosmeticItem cosmeticItem2 = this.items[slotIndex];
      string nameFromDisplayName2 = CosmeticsController.instance.GetItemNameFromDisplayName(cosmeticItem2.displayName);
      CosmeticsController.CosmeticSlots cosmeticSlots = (CosmeticsController.CosmeticSlots) slotIndex;
      BodyDockPositions.DropPositions dropPosition = CosmeticsController.CosmeticSlotToDropPosition(cosmeticSlots);
      if (cosmeticItem2.itemCategory != CosmeticsController.CosmeticCategory.None && !CosmeticsController.CompareCategoryToSavedCosmeticSlots(cosmeticItem2.itemCategory, cosmeticSlots) || cosmeticItem2.isHoldable && dropPosition == BodyDockPositions.DropPositions.None)
        return;
      if (nameFromDisplayName1 == nameFromDisplayName2)
      {
        if (cosmeticItem2.isNullItem)
          return;
        CosmeticItemInstance cosmeticItemInstance = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
        if (cosmeticItemInstance == null)
          return;
        if (!rig.IsItemAllowed(nameFromDisplayName2))
          cosmeticItemInstance.DisableItem(cosmeticSlots);
        else
          cosmeticItemInstance.EnableItem(cosmeticSlots, rig);
      }
      else
      {
        if (!cosmeticItem1.isNullItem)
        {
          if (cosmeticItem1.isHoldable)
            bDock.TransferrableItemDisableAtPosition(dropPosition);
          cosmeticsObjectRegistry.Cosmetic(cosmeticItem1.displayName)?.DisableItem(cosmeticSlots);
        }
        if (cosmeticItem2.isNullItem)
          return;
        if (cosmeticItem2.isHoldable)
          bDock.TransferrableItemEnableAtPosition(cosmeticItem2.displayName, dropPosition);
        CosmeticItemInstance cosmeticItemInstance = cosmeticsObjectRegistry.Cosmetic(cosmeticItem2.displayName);
        if (!rig.IsItemAllowed(nameFromDisplayName2) || cosmeticItemInstance == null)
          return;
        cosmeticItemInstance.EnableItem(cosmeticSlots, rig);
        if ((!rig.isLocal || cosmeticSlots != CosmeticsController.CosmeticSlots.Hat) && cosmeticSlots != CosmeticsController.CosmeticSlots.Face)
          return;
        PlayerPrefFlags.TouchIf(PlayerPrefFlags.Flag.SHOW_1P_COSMETICS, false);
      }
    }

    public void ActivateCosmetics(
      CosmeticsController.CosmeticSet prevSet,
      VRRig rig,
      BodyDockPositions bDock,
      CosmeticItemRegistry cosmeticsObjectRegistry)
    {
      int num = 16 /*0x10*/;
      for (int slotIndex = 0; slotIndex < num; ++slotIndex)
        this.ActivateCosmetic(prevSet, rig, slotIndex, cosmeticsObjectRegistry, bDock);
      this.OnSetActivated(prevSet, this, rig.creator);
    }

    public void DeactivateAllCosmetcs(
      BodyDockPositions bDock,
      CosmeticsController.CosmeticItem nullItem,
      CosmeticItemRegistry cosmeticObjectRegistry)
    {
      bDock.DisableAllTransferableItems();
      int num = 16 /*0x10*/;
      for (int index = 0; index < num; ++index)
      {
        CosmeticsController.CosmeticItem cosmeticItem = this.items[index];
        if (!cosmeticItem.isNullItem)
        {
          CosmeticsController.CosmeticSlots cosmeticSlot = (CosmeticsController.CosmeticSlots) index;
          cosmeticObjectRegistry.Cosmetic(cosmeticItem.displayName)?.DisableItem(cosmeticSlot);
          this.items[index] = nullItem;
        }
      }
    }

    public void LoadFromPlayerPreferences(CosmeticsController controller)
    {
      int num = 16 /*0x10*/;
      for (int index = 0; index < num; ++index)
      {
        CosmeticsController.CosmeticSlots slot = (CosmeticsController.CosmeticSlots) index;
        string itemID = PlayerPrefs.GetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), "NOTHING");
        if (itemID == "null" || itemID == "NOTHING")
        {
          this.items[index] = controller.nullItem;
        }
        else
        {
          CosmeticsController.CosmeticItem item = controller.GetItemFromDict(itemID);
          if (item.isNullItem)
          {
            Debug.Log((object) $"LoadFromPlayerPreferences: Could not find item stored in player prefs: \"{itemID}\"");
            this.items[index] = controller.nullItem;
          }
          else
            this.items[index] = CosmeticsController.CompareCategoryToSavedCosmeticSlots(item.itemCategory, slot) ? (controller.unlockedCosmetics.FindIndex((Predicate<CosmeticsController.CosmeticItem>) (x => item.itemName == x.itemName)) < 0 ? controller.nullItem : item) : controller.nullItem;
        }
      }
    }

    public void ParseSetFromString(
      CosmeticsController controller,
      string setString,
      out Vector3 color)
    {
      color = CosmeticsController.defaultColor;
      if (setString.IsNullOrEmpty())
      {
        this.ClearSet(controller.nullItem);
        GTDev.LogError<string>("CosmeticsController ParseSetFromString: null string");
      }
      else
      {
        int num = 16 /*0x10*/;
        CosmeticsController.OutfitData outfitData = new CosmeticsController.OutfitData();
        try
        {
          outfitData = JsonUtility.FromJson<CosmeticsController.OutfitData>(setString);
          color = outfitData.color;
        }
        catch (Exception ex)
        {
          char ch = ',';
          if ((UnityEngine.Object) controller.outfitSystemConfig != (UnityEngine.Object) null)
            ch = controller.outfitSystemConfig.itemSeparator;
          string[] collection = setString.Split(ch, num, StringSplitOptions.None);
          if (collection == null || collection.Length > num)
          {
            this.ClearSet(controller.nullItem);
            GTDev.LogError<string>($"CosmeticsController ParseSetFromString: wrong number of slots {collection.Length} {setString}");
            return;
          }
          outfitData.Clear();
          outfitData.itemIDs = new List<string>((IEnumerable<string>) collection);
        }
        try
        {
          for (int index = 0; index < num; ++index)
          {
            CosmeticsController.CosmeticSlots slot = (CosmeticsController.CosmeticSlots) index;
            string str = index < outfitData.itemIDs.Count ? outfitData.itemIDs[index] : "null";
            if (str.IsNullOrEmpty() || str == "null" || str == "NOTHING")
            {
              this.items[index] = controller.nullItem;
            }
            else
            {
              CosmeticsController.CosmeticItem item = controller.GetItemFromDict(str);
              if (item.isNullItem)
              {
                GTDev.Log<string>($"CosmeticsController ParseSetFromString: Could not find item stored in player prefs: \"{str}\"");
                this.items[index] = controller.nullItem;
              }
              else
                this.items[index] = CosmeticsController.CompareCategoryToSavedCosmeticSlots(item.itemCategory, slot) ? (controller.unlockedCosmetics.FindIndex((Predicate<CosmeticsController.CosmeticItem>) (x => item.itemName == x.itemName)) < 0 ? controller.nullItem : item) : controller.nullItem;
            }
          }
        }
        catch (Exception ex)
        {
          this.ClearSet(controller.nullItem);
          GTDev.LogError<string>("CosmeticsController: Issue parsing saved outfit string: " + ex.Message);
        }
      }
    }

    public string[] ToDisplayNameArray()
    {
      int num = 16 /*0x10*/;
      for (int index = 0; index < num; ++index)
        this.returnArray[index] = string.IsNullOrEmpty(this.items[index].displayName) ? "null" : this.items[index].displayName;
      return this.returnArray;
    }

    public int[] ToPackedIDArray()
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 16 /*0x10*/;
      for (int index = 0; index < num3; ++index)
      {
        if (!this.items[index].isNullItem && this.items[index].itemName.Length == 6)
        {
          num1 |= 1 << index;
          ++num2;
        }
      }
      if (num1 == 0)
        return CosmeticsController.CosmeticSet.intArrays[0];
      int[] intArray = CosmeticsController.CosmeticSet.intArrays[num2 + 1];
      intArray[0] = num1;
      int index1 = 1;
      for (int index2 = 0; index2 < num3; ++index2)
      {
        if ((num1 & 1 << index2) != 0)
        {
          string itemName = this.items[index2].itemName;
          intArray[index1] = (int) itemName[0] - 65 + 26 * ((int) itemName[1] - 65 + 26 * ((int) itemName[2] - 65 + 26 * ((int) itemName[3] - 65 + 26 * ((int) itemName[4] - 65))));
          ++index1;
        }
      }
      return intArray;
    }

    public string[] HoldableDisplayNames(bool leftHoldables)
    {
      int num = 16 /*0x10*/;
      int length = 0;
      for (int slot = 0; slot < num; ++slot)
      {
        if (this.items[slot].isHoldable && this.items[slot].isHoldable && this.items[slot].itemCategory != CosmeticsController.CosmeticCategory.Chest)
        {
          if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots) slot)))
            ++length;
          else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots) slot)))
            ++length;
        }
      }
      if (length == 0)
        return (string[]) null;
      int index = 0;
      string[] strArray = new string[length];
      for (int slot = 0; slot < num; ++slot)
      {
        if (this.items[slot].isHoldable)
        {
          if (leftHoldables && BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots) slot)))
          {
            strArray[index] = this.items[slot].displayName;
            ++index;
          }
          else if (!leftHoldables && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots) slot)))
          {
            strArray[index] = this.items[slot].displayName;
            ++index;
          }
        }
      }
      return strArray;
    }

    public bool[] ToOnRightSideArray()
    {
      int length = 16 /*0x10*/;
      bool[] onRightSideArray = new bool[length];
      for (int slot = 0; slot < length; ++slot)
        onRightSideArray[slot] = this.items[slot].isHoldable && this.items[slot].itemCategory != CosmeticsController.CosmeticCategory.Chest && !BodyDockPositions.IsPositionLeft(CosmeticsController.CosmeticSlotToDropPosition((CosmeticsController.CosmeticSlots) slot));
      return onRightSideArray;
    }

    public delegate void OnSetActivatedHandler(
      CosmeticsController.CosmeticSet prevSet,
      CosmeticsController.CosmeticSet currentSet,
      NetPlayer netPlayer);
  }

  [Serializable]
  public struct CosmeticItem
  {
    [Tooltip("Should match the spreadsheet item name.")]
    public string itemName;
    [Tooltip("Determines what wardrobe section the item will show up in.")]
    public CosmeticsController.CosmeticCategory itemCategory;
    [Tooltip("If this is a holdable item.")]
    public bool isHoldable;
    [Tooltip("If this is a throwable item and hidden on the wardrobe.")]
    public bool isThrowable;
    [Tooltip("Icon shown in the store menus & hunt watch.")]
    public Sprite itemPicture;
    public string displayName;
    public string itemPictureResourceString;
    [Tooltip("The name shown on the store checkout screen.")]
    public string overrideDisplayName;
    [DebugReadout]
    [NonSerialized]
    public int cost;
    [DebugReadout]
    [NonSerialized]
    public string[] bundledItems;
    [DebugReadout]
    [NonSerialized]
    public bool canTryOn;
    [Tooltip("Set to true if the item takes up both left and right wearable hand slots at the same time. Used for things like mittens/gloves.")]
    public bool bothHandsHoldable;
    public bool bLoadsFromResources;
    public bool bUsesMeshAtlas;
    public Vector3 rotationOffset;
    public Vector3 positionOffset;
    public string meshAtlasResourceString;
    public string meshResourceString;
    public string materialResourceString;
    [HideInInspector]
    public bool isNullItem;
  }

  [Serializable]
  public class IAPRequestBody
  {
    public string userID;
    public string nonce;
    public string platform;
    public string sku;
    public Dictionary<string, string> customTags;
  }

  public enum EWearingCosmeticSet
  {
    NotASet,
    NotWearing,
    Partial,
    Complete,
  }

  public class OutfitData
  {
    public const int OUTFIT_DATA_VERSION = 1;
    public int version;
    public List<string> itemIDs;
    public Vector3 color;

    public OutfitData()
    {
      this.version = 1;
      this.itemIDs = new List<string>(16 /*0x10*/);
      this.color = CosmeticsController.defaultColor;
    }

    public void Clear()
    {
      this.itemIDs.Clear();
      this.color = CosmeticsController.defaultColor;
    }
  }
}
