// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.DynamicCosmeticStand
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#nullable disable
namespace GorillaNetworking.Store;

public class DynamicCosmeticStand : MonoBehaviour, iFlagForBaking
{
  public HeadModel_CosmeticStand DisplayHeadModel;
  public GorillaPressableButton AddToCartButton;
  [HideInInspector]
  public Text slotPriceText;
  [HideInInspector]
  public Text addToCartText;
  public TMP_Text slotPriceTextTMP;
  public TMP_Text addToCartTextTMP;
  private CosmeticsController.CosmeticItem thisCosmeticItem;
  [FormerlySerializedAs("StandID")]
  public string StandName;
  public string _thisCosmeticName = "";
  public GameObject GorillaHeadModel;
  public GameObject GorillaTorsoModel;
  public GameObject GorillaTorsoPostModel;
  public GameObject GorillaMannequinModel;
  public GameObject GuitarStandModel;
  public GameObject GuitarStandMount;
  public GameObject JeweleryBoxModel;
  public GameObject JeweleryBoxMount;
  public GameObject TableMount;
  [FormerlySerializedAs("PinDisplayMounnt")]
  [FormerlySerializedAs("PinDisplayMountn")]
  public GameObject PinDisplayMount;
  public GameObject root;
  public GameObject TagEffectDisplayMount;
  public GameObject TageEffectDisplayModel;
  private UnityEngine.SceneManagement.Scene customMapScene;
  private int searchIndex;

  public virtual void SetForBaking()
  {
    this.GorillaHeadModel.SetActive(true);
    this.GorillaTorsoModel.SetActive(true);
    this.GorillaTorsoPostModel.SetActive(true);
    this.GorillaMannequinModel.SetActive(true);
    this.JeweleryBoxModel.SetActive(true);
    this.root.SetActive(true);
    this.DisplayHeadModel.gameObject.SetActive(false);
  }

  public void OnEnable()
  {
    this.addToCartTextTMP.gameObject.SetActive(true);
    this.slotPriceTextTMP.gameObject.SetActive(true);
  }

  public void OnDisable()
  {
    this.addToCartTextTMP.gameObject.SetActive(false);
    this.slotPriceTextTMP.gameObject.SetActive(false);
  }

  public virtual void SetForGame()
  {
    this.DisplayHeadModel.gameObject.SetActive(true);
    this.SetStandType(this.DisplayHeadModel.bustType);
  }

  public string thisCosmeticName
  {
    get => this._thisCosmeticName;
    set => this._thisCosmeticName = value;
  }

  public void InitializeCosmetic()
  {
    this.thisCosmeticItem = CosmeticsController.instance.allCosmetics.Find((Predicate<CosmeticsController.CosmeticItem>) (x => this.thisCosmeticName == x.displayName || this.thisCosmeticName == x.overrideDisplayName || this.thisCosmeticName == x.itemName));
    if ((UnityEngine.Object) this.slotPriceText != (UnityEngine.Object) null)
      this.slotPriceText.text = $"{this.thisCosmeticItem.itemCategory.ToString().ToUpper()} {this.thisCosmeticItem.cost.ToString()}";
    if (!((UnityEngine.Object) this.slotPriceTextTMP != (UnityEngine.Object) null))
      return;
    this.slotPriceTextTMP.text = $"{this.thisCosmeticItem.itemCategory.ToString().ToUpper()} {this.thisCosmeticItem.cost.ToString()}";
  }

  public void SpawnItemOntoStand(string PlayFabID)
  {
    this.ClearCosmetics();
    if (PlayFabID.IsNullOrEmpty())
    {
      GTDev.LogWarning<string>("ManuallyInitialize: PlayFabID is null or empty for " + this.StandName);
    }
    else
    {
      if (StoreController.instance.IsNotNull() && Application.isPlaying)
        StoreController.instance.RemoveStandFromPlayFabIDDictionary(this);
      this.thisCosmeticName = PlayFabID;
      if (this.thisCosmeticName.Length == 5)
        this.thisCosmeticName += ".";
      if (Application.isPlaying)
        this.DisplayHeadModel.LoadCosmeticPartsV2(this.thisCosmeticName);
      else
        this.DisplayHeadModel.LoadCosmeticParts(StoreController.FindCosmeticInAllCosmeticsArraySO(this.thisCosmeticName));
      if (!StoreController.instance.IsNotNull() || !Application.isPlaying)
        return;
      StoreController.instance.AddStandToPlayfabIDDictionary(this);
    }
  }

  public void ClearCosmetics()
  {
    this.thisCosmeticName = "";
    this.DisplayHeadModel.ClearManuallySpawnedCosmeticParts();
    this.DisplayHeadModel.ClearCosmetics();
  }

  public void SetStandType(HeadModel_CosmeticStand.BustType newBustType)
  {
    this.DisplayHeadModel.SetStandType(newBustType);
    this.GorillaHeadModel.SetActive(false);
    this.GorillaTorsoModel.SetActive(false);
    this.GorillaTorsoPostModel.SetActive(false);
    this.GorillaMannequinModel.SetActive(false);
    this.GuitarStandModel.SetActive(false);
    this.JeweleryBoxModel.SetActive(false);
    this.AddToCartButton.gameObject.SetActive(true);
    this.slotPriceText?.gameObject.SetActive(true);
    this.slotPriceTextTMP?.gameObject.SetActive(true);
    this.addToCartText?.gameObject.SetActive(true);
    this.addToCartTextTMP?.gameObject.SetActive(true);
    switch (newBustType)
    {
      case HeadModel_CosmeticStand.BustType.Disabled:
        this.ClearCosmetics();
        this.thisCosmeticName = "";
        this.AddToCartButton.gameObject.SetActive(false);
        this.slotPriceText?.gameObject.SetActive(false);
        this.slotPriceTextTMP?.gameObject.SetActive(false);
        this.addToCartText?.gameObject.SetActive(false);
        this.addToCartTextTMP?.gameObject.SetActive(false);
        this.DisplayHeadModel.transform.localPosition = Vector3.zero;
        this.DisplayHeadModel.transform.localRotation = Quaternion.identity;
        this.root.SetActive(false);
        break;
      case HeadModel_CosmeticStand.BustType.GorillaHead:
        this.root.SetActive(true);
        this.GorillaHeadModel.SetActive(true);
        this.DisplayHeadModel.transform.localPosition = this.GorillaHeadModel.transform.localPosition;
        this.DisplayHeadModel.transform.localRotation = this.GorillaHeadModel.transform.localRotation;
        break;
      case HeadModel_CosmeticStand.BustType.GorillaTorso:
        this.root.SetActive(true);
        this.GorillaTorsoModel.SetActive(true);
        this.DisplayHeadModel.transform.localPosition = this.GorillaTorsoModel.transform.localPosition;
        this.DisplayHeadModel.transform.localRotation = this.GorillaTorsoModel.transform.localRotation;
        break;
      case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
        this.root.SetActive(true);
        this.GorillaTorsoPostModel.SetActive(true);
        this.DisplayHeadModel.transform.localPosition = this.GorillaTorsoPostModel.transform.localPosition;
        this.DisplayHeadModel.transform.localRotation = this.GorillaTorsoPostModel.transform.localRotation;
        break;
      case HeadModel_CosmeticStand.BustType.GorillaMannequin:
        this.root.SetActive(true);
        this.GorillaMannequinModel.SetActive(true);
        this.DisplayHeadModel.transform.localPosition = this.GorillaMannequinModel.transform.localPosition;
        this.DisplayHeadModel.transform.localRotation = this.GorillaMannequinModel.transform.localRotation;
        break;
      case HeadModel_CosmeticStand.BustType.GuitarStand:
        this.root.SetActive(true);
        this.GuitarStandModel.SetActive(true);
        this.DisplayHeadModel.transform.localPosition = this.GuitarStandMount.transform.localPosition;
        this.DisplayHeadModel.transform.localRotation = this.GuitarStandMount.transform.localRotation;
        break;
      case HeadModel_CosmeticStand.BustType.JewelryBox:
        this.root.SetActive(true);
        this.JeweleryBoxModel.SetActive(true);
        this.DisplayHeadModel.transform.localPosition = this.JeweleryBoxMount.transform.localPosition;
        this.DisplayHeadModel.transform.localRotation = this.JeweleryBoxMount.transform.localRotation;
        break;
      case HeadModel_CosmeticStand.BustType.Table:
        this.root.SetActive(true);
        this.DisplayHeadModel.transform.localPosition = this.TableMount.transform.localPosition;
        this.DisplayHeadModel.transform.localRotation = this.TableMount.transform.localRotation;
        break;
      case HeadModel_CosmeticStand.BustType.PinDisplay:
        this.root.SetActive(true);
        this.DisplayHeadModel.transform.localPosition = this.PinDisplayMount.transform.localPosition;
        this.DisplayHeadModel.transform.localRotation = this.PinDisplayMount.transform.localRotation;
        break;
      case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
        this.root.SetActive(true);
        break;
      default:
        this.root.SetActive(true);
        this.DisplayHeadModel.transform.localPosition = Vector3.zero;
        this.DisplayHeadModel.transform.localRotation = Quaternion.identity;
        break;
    }
    this.SpawnItemOntoStand(this.thisCosmeticName);
  }

  public void CopyChildsName()
  {
    foreach (DynamicCosmeticStand componentsInChild in this.gameObject.GetComponentsInChildren<DynamicCosmeticStand>(true))
    {
      if ((UnityEngine.Object) componentsInChild != (UnityEngine.Object) this)
        this.StandName = componentsInChild.StandName;
    }
  }

  public void PressCosmeticStandButton()
  {
    this.searchIndex = CosmeticsController.instance.currentCart.IndexOf(this.thisCosmeticItem);
    if (this.searchIndex != -1)
    {
      GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_remove, this.thisCosmeticItem);
      CosmeticsController.instance.currentCart.RemoveAt(this.searchIndex);
      foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.StandsByPlayfabID[this.thisCosmeticItem.itemName])
      {
        dynamicCosmeticStand.AddToCartButton.isOn = false;
        dynamicCosmeticStand.AddToCartButton.UpdateColor();
      }
      for (int index = 0; index < 16 /*0x10*/; ++index)
      {
        if (this.thisCosmeticItem.itemName == CosmeticsController.instance.tryOnSet.items[index].itemName)
          CosmeticsController.instance.tryOnSet.items[index] = CosmeticsController.instance.nullItem;
      }
    }
    else
    {
      GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_add, this.thisCosmeticItem);
      CosmeticsController.instance.currentCart.Insert(0, this.thisCosmeticItem);
      foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.StandsByPlayfabID[this.thisCosmeticName])
      {
        dynamicCosmeticStand.AddToCartButton.isOn = true;
        dynamicCosmeticStand.AddToCartButton.UpdateColor();
      }
      if (CosmeticsController.instance.currentCart.Count > CosmeticsController.instance.numFittingRoomButtons)
      {
        foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.StandsByPlayfabID[CosmeticsController.instance.currentCart[CosmeticsController.instance.numFittingRoomButtons].itemName])
        {
          dynamicCosmeticStand.AddToCartButton.isOn = false;
          dynamicCosmeticStand.AddToCartButton.UpdateColor();
        }
        CosmeticsController.instance.currentCart.RemoveAt(CosmeticsController.instance.numFittingRoomButtons);
      }
    }
    CosmeticsController.instance.UpdateShoppingCart();
  }

  public void SetStandTypeString(string bustTypeString)
  {
    switch (bustTypeString)
    {
      case "Disabled":
        this.SetStandType(HeadModel_CosmeticStand.BustType.Disabled);
        break;
      case "GorillaHead":
        this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaHead);
        break;
      case "GorillaMannequin":
        this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaMannequin);
        break;
      case "GorillaTorso":
        this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaTorso);
        break;
      case "GorillaTorsoPost":
        this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaTorsoPost);
        break;
      case "GuitarStand":
        this.SetStandType(HeadModel_CosmeticStand.BustType.GuitarStand);
        break;
      case "JewelryBox":
        this.SetStandType(HeadModel_CosmeticStand.BustType.JewelryBox);
        break;
      case "PinDisplay":
        this.SetStandType(HeadModel_CosmeticStand.BustType.PinDisplay);
        break;
      case "Table":
        this.SetStandType(HeadModel_CosmeticStand.BustType.Table);
        break;
      case "TagEffectDisplay":
        this.SetStandType(HeadModel_CosmeticStand.BustType.TagEffectDisplay);
        break;
      default:
        this.SetStandType(HeadModel_CosmeticStand.BustType.Table);
        break;
    }
  }

  public void UpdateCosmeticsMountPositions()
  {
    this.DisplayHeadModel.UpdateCosmeticsMountPositions(StoreController.FindCosmeticInAllCosmeticsArraySO(this.thisCosmeticName));
  }

  public void InitializeForCustomMapCosmeticItem(
    GTObjectPlaceholder.ECustomMapCosmeticItem cosmeticItemSlot,
    UnityEngine.SceneManagement.Scene scene)
  {
    this.StandName = "CustomMapCosmeticItemStand-" + cosmeticItemSlot.ToString();
    this.customMapScene = scene;
    this.ClearCosmetics();
    CustomMapCosmeticItem foundItem;
    if (!CosmeticsController.instance.customMapCosmeticsData.TryGetItem(cosmeticItemSlot, out foundItem))
      return;
    this.thisCosmeticName = foundItem.playFabID;
    this.SetStandType(foundItem.bustType);
    this.InitializeCosmetic();
  }

  public bool IsFromCustomMapScene(UnityEngine.SceneManagement.Scene scene)
  {
    return this.customMapScene == scene;
  }
}
