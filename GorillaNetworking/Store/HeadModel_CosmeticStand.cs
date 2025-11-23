// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.HeadModel_CosmeticStand
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTag;
using GorillaTag.CosmeticSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

#nullable disable
namespace GorillaNetworking.Store;

public class HeadModel_CosmeticStand : HeadModel
{
  public HeadModel_CosmeticStand.BustType bustType = HeadModel_CosmeticStand.BustType.JewelryBox;
  [SerializeField]
  private List<GameObject> _manuallySpawnedCosmeticParts = new List<GameObject>();
  public GameObject mannequin;
  public Material defaultMannequinFace;
  public Material defaultMannequinChest;
  public Material defaultMannequinBody;
  [DebugReadout]
  protected new readonly List<HeadModel._CosmeticPartLoadInfo> _currentPartLoadInfos = new List<HeadModel._CosmeticPartLoadInfo>(1);
  [DebugReadout]
  private readonly Dictionary<AsyncOperationHandle, int> _loadOp_to_partInfoIndex = new Dictionary<AsyncOperationHandle, int>(1);

  private string mountID => "Mount_" + this.bustType.ToString();

  public void LoadCosmeticParts(CosmeticSO cosmeticInfo, bool forRightSide = false)
  {
    this.ClearManuallySpawnedCosmeticParts();
    this.ClearCosmetics();
    if ((UnityEngine.Object) cosmeticInfo == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "Dynamic Cosmetics - LoadWardRobeParts -  No Cosmetic Info");
    }
    else
    {
      Debug.Log((object) ("Dynamic Cosmetics - Loading Wardrobe Parts for " + cosmeticInfo.info.playFabID));
      this.HandleLoadCosmeticParts(cosmeticInfo, forRightSide);
    }
  }

  private void ResetMannequinSkin()
  {
    this.mannequin.GetComponent<SkinnedMeshRenderer>();
    SkinnedMeshRenderer component1;
    if (this.mannequin.TryGetComponent<SkinnedMeshRenderer>(out component1))
    {
      Material[] materialArray = new Material[3]
      {
        this.defaultMannequinBody,
        this.defaultMannequinChest,
        this.defaultMannequinFace
      };
      component1.sharedMaterials = materialArray;
    }
    else
    {
      MeshRenderer component2;
      if (!this.mannequin.TryGetComponent<MeshRenderer>(out component2))
        return;
      Material[] materialArray = new Material[3]
      {
        this.defaultMannequinBody,
        this.defaultMannequinChest,
        this.defaultMannequinFace
      };
      component2.sharedMaterials = materialArray;
    }
  }

  private void HandleLoadCosmeticParts(CosmeticSO cosmeticInfo, bool forRightSide)
  {
    if (cosmeticInfo.info.category == (StringEnum<CosmeticsController.CosmeticCategory>) CosmeticsController.CosmeticCategory.Set && !cosmeticInfo.info.hasStoreParts)
    {
      foreach (CosmeticSO setCosmetic in cosmeticInfo.info.setCosmetics)
        this.HandleLoadCosmeticParts(setCosmetic, forRightSide);
    }
    else
    {
      CosmeticPart[] cosmeticPartArray;
      if (cosmeticInfo.info.storeParts.Length != 0)
      {
        cosmeticPartArray = cosmeticInfo.info.storeParts;
      }
      else
      {
        if (cosmeticInfo.info.category == (StringEnum<CosmeticsController.CosmeticCategory>) CosmeticsController.CosmeticCategory.Fur)
        {
          CosmeticPart[] functionalParts = cosmeticInfo.info.functionalParts;
          int index = 0;
          if (index < functionalParts.Length)
          {
            GameObject gameObject = this.LoadAndInstantiatePrefab(functionalParts[index].prefabAssetRef, this.transform);
            gameObject.GetComponent<GorillaSkinToggle>().ApplyToMannequin(this.mannequin);
            UnityEngine.Object.DestroyImmediate((UnityEngine.Object) gameObject);
            return;
          }
        }
        cosmeticPartArray = cosmeticInfo.info.wardrobeParts;
      }
      foreach (CosmeticPart cosmeticPart in cosmeticPartArray)
      {
        foreach (CosmeticAttachInfo attachAnchor in cosmeticPart.attachAnchors)
        {
          if ((!forRightSide || !(attachAnchor.selectSide == (StringEnum<ECosmeticSelectSide>) ECosmeticSelectSide.Left)) && (forRightSide || !(attachAnchor.selectSide == (StringEnum<ECosmeticSelectSide>) ECosmeticSelectSide.Right)))
          {
            HeadModel._CosmeticPartLoadInfo partLoadInfo = new HeadModel._CosmeticPartLoadInfo()
            {
              playFabId = cosmeticInfo.info.playFabID,
              prefabAssetRef = cosmeticPart.prefabAssetRef,
              attachInfo = attachAnchor,
              xform = (Transform) null
            };
            GameObject instantiateEdObject = this.LoadAndInstantiatePrefab(cosmeticPart.prefabAssetRef, this.transform);
            partLoadInfo.xform = instantiateEdObject.transform;
            this._manuallySpawnedCosmeticParts.Add(instantiateEdObject);
            instantiateEdObject.SetActive(true);
            switch (this.bustType)
            {
              case HeadModel_CosmeticStand.BustType.Disabled:
                this.PositionWithWardRobeOffsets(partLoadInfo);
                continue;
              case HeadModel_CosmeticStand.BustType.GorillaHead:
              case HeadModel_CosmeticStand.BustType.GorillaTorso:
              case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
              case HeadModel_CosmeticStand.BustType.GuitarStand:
              case HeadModel_CosmeticStand.BustType.JewelryBox:
              case HeadModel_CosmeticStand.BustType.Table:
              case HeadModel_CosmeticStand.BustType.PinDisplay:
              case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
                this.PositionWardRobeItems(instantiateEdObject, partLoadInfo);
                continue;
              case HeadModel_CosmeticStand.BustType.GorillaMannequin:
                this._manuallySpawnedCosmeticParts.Remove(instantiateEdObject);
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object) instantiateEdObject);
                continue;
              default:
                this.PositionWithWardRobeOffsets(partLoadInfo);
                continue;
            }
          }
        }
      }
    }
  }

  public void LoadCosmeticPartsV2(string playFabId, bool forRightSide = false)
  {
    this.ClearManuallySpawnedCosmeticParts();
    this.ClearCosmetics();
    CosmeticInfoV2 cosmeticInfo;
    if (!CosmeticsController.instance.TryGetCosmeticInfoV2(playFabId, out cosmeticInfo))
    {
      switch (playFabId)
      {
        case "null":
          break;
        case "NOTHING":
          break;
        case "Slingshot":
          break;
        default:
          Debug.LogError((object) $"HeadModel.playFabId: Cosmetic id \"{playFabId}\" not found in `CosmeticsController`.", (UnityEngine.Object) this);
          break;
      }
    }
    else
      this.HandleLoadingAllPieces(playFabId, forRightSide, cosmeticInfo);
  }

  private void HandleLoadingAllPieces(
    string playFabId,
    bool forRightSide,
    CosmeticInfoV2 cosmeticInfo)
  {
    CosmeticPart[] cosmeticPartArray;
    if (cosmeticInfo.storeParts.Length != 0)
    {
      cosmeticPartArray = cosmeticInfo.storeParts;
    }
    else
    {
      if (cosmeticInfo.category == (StringEnum<CosmeticsController.CosmeticCategory>) CosmeticsController.CosmeticCategory.Fur)
      {
        this.HandleLoadingFur(playFabId, forRightSide, cosmeticInfo);
        return;
      }
      if (cosmeticInfo.category == (StringEnum<CosmeticsController.CosmeticCategory>) CosmeticsController.CosmeticCategory.Set)
      {
        foreach (CosmeticSO setCosmetic in cosmeticInfo.setCosmetics)
          this.HandleLoadingAllPieces(playFabId, forRightSide, setCosmetic.info);
        return;
      }
      cosmeticPartArray = cosmeticInfo.wardrobeParts;
    }
    foreach (CosmeticPart cosmeticPart in cosmeticPartArray)
    {
      foreach (CosmeticAttachInfo attachAnchor in cosmeticPart.attachAnchors)
      {
        if ((!forRightSide || !(attachAnchor.selectSide == (StringEnum<ECosmeticSelectSide>) ECosmeticSelectSide.Left)) && (forRightSide || !(attachAnchor.selectSide == (StringEnum<ECosmeticSelectSide>) ECosmeticSelectSide.Right)))
        {
          HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = new HeadModel._CosmeticPartLoadInfo()
          {
            playFabId = playFabId,
            prefabAssetRef = cosmeticPart.prefabAssetRef,
            attachInfo = attachAnchor,
            loadOp = cosmeticPart.prefabAssetRef.InstantiateAsync(this.transform),
            xform = (Transform) null
          };
          cosmeticPartLoadInfo.loadOp.Completed += new Action<AsyncOperationHandle<GameObject>>(this._HandleLoadCosmeticPartsV2);
          this._loadOp_to_partInfoIndex[(AsyncOperationHandle) cosmeticPartLoadInfo.loadOp] = this._currentPartLoadInfos.Count;
          this._currentPartLoadInfos.Add(cosmeticPartLoadInfo);
        }
      }
    }
  }

  private void _HandleLoadCosmeticPartsV2(AsyncOperationHandle<GameObject> loadOp)
  {
    int num;
    if (!this._loadOp_to_partInfoIndex.TryGetValue((AsyncOperationHandle) loadOp, out num))
    {
      if (loadOp.Status != AsyncOperationStatus.Succeeded || !(bool) (UnityEngine.Object) loadOp.Result)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) loadOp.Result);
    }
    else
    {
      HeadModel._CosmeticPartLoadInfo currentPartLoadInfo = this._currentPartLoadInfos[num];
      if (loadOp.Status == AsyncOperationStatus.Failed)
      {
        Debug.Log((object) $"HeadModel: Failed to load a part for cosmetic \"{currentPartLoadInfo.playFabId}\"! Waiting for 10 seconds before trying again.", (UnityEngine.Object) this);
        GTDelayedExec.Add((IDelayedExecListener) this, 10f, num);
      }
      else
      {
        currentPartLoadInfo.xform = loadOp.Result.transform;
        this._manuallySpawnedCosmeticParts.Add(currentPartLoadInfo.xform.gameObject);
        switch (this.bustType)
        {
          case HeadModel_CosmeticStand.BustType.Disabled:
            this.PositionWithWardRobeOffsets(currentPartLoadInfo);
            break;
          case HeadModel_CosmeticStand.BustType.GorillaHead:
            this.PositionWithWardRobeOffsets(currentPartLoadInfo);
            break;
          case HeadModel_CosmeticStand.BustType.GorillaTorso:
            this.PositionWithWardRobeOffsets(currentPartLoadInfo);
            break;
          case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
            this.PositionWithWardRobeOffsets(currentPartLoadInfo);
            break;
          case HeadModel_CosmeticStand.BustType.GorillaMannequin:
            this._manuallySpawnedCosmeticParts.Remove(currentPartLoadInfo.xform.gameObject);
            UnityEngine.Object.DestroyImmediate((UnityEngine.Object) currentPartLoadInfo.xform.gameObject);
            break;
          case HeadModel_CosmeticStand.BustType.GuitarStand:
            this.PositionWardRobeItems(currentPartLoadInfo);
            break;
          case HeadModel_CosmeticStand.BustType.JewelryBox:
            this.PositionWardRobeItems(currentPartLoadInfo);
            break;
          case HeadModel_CosmeticStand.BustType.Table:
            this.PositionWardRobeItems(currentPartLoadInfo);
            break;
          case HeadModel_CosmeticStand.BustType.PinDisplay:
            this.PositionWardRobeItems(currentPartLoadInfo);
            break;
          case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
            this.PositionWardRobeItems(currentPartLoadInfo);
            break;
          default:
            this.PositionWithWardRobeOffsets(currentPartLoadInfo);
            break;
        }
        currentPartLoadInfo.xform.gameObject.SetActive(true);
      }
    }
  }

  private void HandleLoadingFur(string playFabId, bool forRightSide, CosmeticInfoV2 cosmeticInfo)
  {
    foreach (CosmeticPart functionalPart in cosmeticInfo.functionalParts)
    {
      foreach (CosmeticAttachInfo attachAnchor in functionalPart.attachAnchors)
      {
        if ((!forRightSide || !(attachAnchor.selectSide == (StringEnum<ECosmeticSelectSide>) ECosmeticSelectSide.Left)) && (forRightSide || !(attachAnchor.selectSide == (StringEnum<ECosmeticSelectSide>) ECosmeticSelectSide.Right)))
        {
          HeadModel._CosmeticPartLoadInfo cosmeticPartLoadInfo = new HeadModel._CosmeticPartLoadInfo()
          {
            playFabId = playFabId,
            prefabAssetRef = functionalPart.prefabAssetRef,
            attachInfo = attachAnchor,
            loadOp = functionalPart.prefabAssetRef.InstantiateAsync(this.transform),
            xform = (Transform) null
          };
          cosmeticPartLoadInfo.loadOp.Completed += new Action<AsyncOperationHandle<GameObject>>(this._HandleLoadCosmeticPartsV2Fur);
          this._loadOp_to_partInfoIndex[(AsyncOperationHandle) cosmeticPartLoadInfo.loadOp] = this._currentPartLoadInfos.Count;
          this._currentPartLoadInfos.Add(cosmeticPartLoadInfo);
        }
      }
    }
  }

  private void _HandleLoadCosmeticPartsV2Fur(AsyncOperationHandle<GameObject> loadOp)
  {
    int num;
    if (!this._loadOp_to_partInfoIndex.TryGetValue((AsyncOperationHandle) loadOp, out num))
    {
      if (loadOp.Status != AsyncOperationStatus.Succeeded || !(bool) (UnityEngine.Object) loadOp.Result)
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) loadOp.Result);
    }
    else
    {
      HeadModel._CosmeticPartLoadInfo currentPartLoadInfo = this._currentPartLoadInfos[num];
      if (loadOp.Status == AsyncOperationStatus.Failed)
      {
        Debug.Log((object) $"HeadModel: Failed to load a part for cosmetic \"{currentPartLoadInfo.playFabId}\"! Waiting for 10 seconds before trying again.", (UnityEngine.Object) this);
        GTDelayedExec.Add((IDelayedExecListener) this, 10f, num);
      }
      else
      {
        currentPartLoadInfo.xform = loadOp.Result.transform;
        currentPartLoadInfo.xform.GetComponent<GorillaSkinToggle>().ApplyToMannequin(this.mannequin);
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) currentPartLoadInfo.xform.gameObject);
      }
    }
  }

  public void SetStandType(HeadModel_CosmeticStand.BustType newBustType)
  {
    this.bustType = newBustType;
  }

  private void PositionWardRobeItems(
    GameObject instantiateEdObject,
    HeadModel._CosmeticPartLoadInfo partLoadInfo)
  {
    Transform childRecursive = instantiateEdObject.transform.FindChildRecursive(this.mountID);
    if ((UnityEngine.Object) childRecursive != (UnityEngine.Object) null)
    {
      Debug.Log((object) ("Dynamic Cosmetics - Mount Found: " + this.mountID));
      instantiateEdObject.transform.position = this.transform.position;
      instantiateEdObject.transform.rotation = this.transform.rotation;
      instantiateEdObject.transform.localPosition = childRecursive.localPosition;
      instantiateEdObject.transform.localRotation = childRecursive.localRotation;
    }
    else
    {
      switch (this.bustType)
      {
        case HeadModel_CosmeticStand.BustType.GuitarStand:
        case HeadModel_CosmeticStand.BustType.JewelryBox:
        case HeadModel_CosmeticStand.BustType.Table:
        case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
          instantiateEdObject.transform.position = this.transform.position;
          instantiateEdObject.transform.rotation = this.transform.rotation;
          break;
        default:
          this.PositionWithWardRobeOffsets(partLoadInfo);
          break;
      }
    }
  }

  private void PositionWardRobeItems(HeadModel._CosmeticPartLoadInfo partLoadInfo)
  {
    Transform childRecursive = partLoadInfo.xform.FindChildRecursive(this.mountID);
    if ((UnityEngine.Object) childRecursive != (UnityEngine.Object) null)
    {
      Debug.Log((object) ("Dynamic Cosmetics - Mount Found: " + this.mountID));
      partLoadInfo.xform.position = this.transform.position;
      partLoadInfo.xform.rotation = this.transform.rotation;
      partLoadInfo.xform.localPosition = childRecursive.localPosition;
      partLoadInfo.xform.localRotation = childRecursive.localRotation;
    }
    else
    {
      switch (this.bustType)
      {
        case HeadModel_CosmeticStand.BustType.GuitarStand:
        case HeadModel_CosmeticStand.BustType.JewelryBox:
        case HeadModel_CosmeticStand.BustType.Table:
        case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
          partLoadInfo.xform.position = this.transform.position;
          partLoadInfo.xform.rotation = this.transform.rotation;
          break;
        default:
          this.PositionWithWardRobeOffsets(partLoadInfo);
          break;
      }
    }
  }

  private void PositionWithWardRobeOffsets(HeadModel._CosmeticPartLoadInfo partLoadInfo)
  {
    Debug.Log((object) ("Dynamic Cosmetics - Mount Not Found: " + this.mountID));
    partLoadInfo.xform.localPosition = partLoadInfo.attachInfo.offset.pos;
    partLoadInfo.xform.localRotation = partLoadInfo.attachInfo.offset.rot;
    partLoadInfo.xform.localScale = partLoadInfo.attachInfo.offset.scale;
  }

  public void ClearManuallySpawnedCosmeticParts()
  {
    foreach (UnityEngine.Object spawnedCosmeticPart in this._manuallySpawnedCosmeticParts)
      UnityEngine.Object.DestroyImmediate(spawnedCosmeticPart);
    this._manuallySpawnedCosmeticParts.Clear();
  }

  public void ClearCosmetics()
  {
    this.ResetMannequinSkin();
    for (int index = this.transform.childCount - 1; index >= 0; --index)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.transform.GetChild(index).gameObject);
  }

  private GameObject LoadAndInstantiatePrefab(
    GTAssetRef<GameObject> prefabAssetRef,
    Transform parent)
  {
    return (GameObject) null;
  }

  public void UpdateCosmeticsMountPositions(CosmeticSO findCosmeticInAllCosmeticsArraySO)
  {
  }

  public enum BustType
  {
    Disabled,
    GorillaHead,
    GorillaTorso,
    GorillaTorsoPost,
    GorillaMannequin,
    GuitarStand,
    JewelryBox,
    Table,
    PinDisplay,
    TagEffectDisplay,
  }
}
