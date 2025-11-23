// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.StoreController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using PlayFab;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace GorillaNetworking.Store;

public class StoreController : MonoBehaviour
{
  public static volatile StoreController instance;
  public List<StoreDepartment> Departments;
  private Dictionary<string, DynamicCosmeticStand> CosmeticStandsDict;
  public Dictionary<string, List<DynamicCosmeticStand>> StandsByPlayfabID;
  public AllCosmeticsArraySO AllCosmeticsArraySO;
  public bool LoadFromTitleData;
  private string exportHeader = "Department ID\tDisplay ID\tStand ID\tStand Type\tPlayFab ID";

  public void Awake()
  {
    if ((UnityEngine.Object) StoreController.instance == (UnityEngine.Object) null)
    {
      StoreController.instance = this;
    }
    else
    {
      if (!((UnityEngine.Object) StoreController.instance != (UnityEngine.Object) this))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
  }

  public void Start()
  {
  }

  public void CreateDynamicCosmeticStandsDictionatary()
  {
    this.CosmeticStandsDict = new Dictionary<string, DynamicCosmeticStand>();
    foreach (StoreDepartment department in this.Departments)
    {
      if (!department.departmentName.IsNullOrEmpty())
      {
        foreach (StoreDisplay display in department.Displays)
        {
          if (!display.displayName.IsNullOrEmpty())
          {
            foreach (DynamicCosmeticStand stand in display.Stands)
            {
              if (!stand.StandName.IsNullOrEmpty())
              {
                if (!this.CosmeticStandsDict.ContainsKey($"{department.departmentName}|{display.displayName}|{stand.StandName}"))
                  this.CosmeticStandsDict.Add($"{department.departmentName}|{display.displayName}|{stand.StandName}", stand);
                else
                  Debug.LogError((object) $"StoreStuff: Duplicate Stand Name: {department.departmentName}|{display.displayName}|{stand.StandName} Please Fix Gameobject : {stand.gameObject.GetPath()}{stand.gameObject.name}");
              }
            }
          }
        }
      }
    }
  }

  private void Create_StandsByPlayfabIDDictionary()
  {
    this.StandsByPlayfabID = new Dictionary<string, List<DynamicCosmeticStand>>();
    foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
      this.AddStandToPlayfabIDDictionary(dynamicCosmeticStand);
  }

  public void AddStandToPlayfabIDDictionary(DynamicCosmeticStand dynamicCosmeticStand)
  {
    if (dynamicCosmeticStand.StandName.IsNullOrEmpty() || dynamicCosmeticStand.thisCosmeticName.IsNullOrEmpty())
      return;
    if (this.StandsByPlayfabID.ContainsKey(dynamicCosmeticStand.thisCosmeticName))
      this.StandsByPlayfabID[dynamicCosmeticStand.thisCosmeticName].Add(dynamicCosmeticStand);
    else
      this.StandsByPlayfabID.Add(dynamicCosmeticStand.thisCosmeticName, new List<DynamicCosmeticStand>()
      {
        dynamicCosmeticStand
      });
  }

  public void RemoveStandFromPlayFabIDDictionary(DynamicCosmeticStand dynamicCosmeticStand)
  {
    List<DynamicCosmeticStand> dynamicCosmeticStandList;
    if (!this.StandsByPlayfabID.TryGetValue(dynamicCosmeticStand.thisCosmeticName, out dynamicCosmeticStandList))
      return;
    dynamicCosmeticStandList.Remove(dynamicCosmeticStand);
  }

  public void ExportCosmeticStandLayoutWithItems()
  {
  }

  public void ExportCosmeticStandLayoutWITHOUTItems()
  {
  }

  public void ImportCosmeticStandLayout()
  {
  }

  private void InitializeFromTitleData()
  {
    PlayFabTitleDataCache.Instance.GetTitleData("StoreLayoutData", (Action<string>) (data => this.ImportCosmeticStandLayoutFromTitleData(data)), (Action<PlayFabError>) (e => Debug.LogError((object) $"Error getting StoreLayoutData data: {e}")));
  }

  private void ImportCosmeticStandLayoutFromTitleData(string TSVData)
  {
    StandImport standImport = new StandImport();
    standImport.DecomposeFromTitleDataString(TSVData);
    foreach (StandTypeData standTypeData in standImport.standData)
    {
      string key = $"{standTypeData.departmentID}|{standTypeData.displayID}|{standTypeData.standID}";
      if (this.CosmeticStandsDict.ContainsKey(key))
      {
        Debug.Log((object) $"StoreStuff: Stand Updated: {standTypeData.departmentID}|{standTypeData.displayID}|{standTypeData.standID}|{standTypeData.bustType}|{standTypeData.playFabID}|");
        this.CosmeticStandsDict[key].SetStandTypeString(standTypeData.bustType);
        Debug.Log((object) $"Manually Initializing Stand: {key} |||| {standTypeData.playFabID}");
        this.CosmeticStandsDict[key].SpawnItemOntoStand(standTypeData.playFabID);
        this.CosmeticStandsDict[key].InitializeCosmetic();
      }
    }
  }

  public void InitalizeCosmeticStands()
  {
    this.CreateDynamicCosmeticStandsDictionatary();
    foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
      dynamicCosmeticStand.InitializeCosmetic();
    this.Create_StandsByPlayfabIDDictionary();
    if (!this.LoadFromTitleData)
      return;
    this.InitializeFromTitleData();
  }

  public void LoadCosmeticOntoStand(string standID, string playFabId)
  {
    if (!this.CosmeticStandsDict.ContainsKey(standID))
      return;
    this.CosmeticStandsDict[standID].SpawnItemOntoStand(playFabId);
    Debug.Log((object) $"StoreStuff: Cosmetic Loaded Onto Stand: {standID} | {playFabId}");
  }

  public void ClearCosmetics()
  {
    foreach (StoreDepartment department in this.Departments)
    {
      foreach (StoreDisplay display in department.Displays)
      {
        foreach (DynamicCosmeticStand stand in display.Stands)
          stand.ClearCosmetics();
      }
    }
  }

  public static CosmeticSO FindCosmeticInAllCosmeticsArraySO(string playfabId)
  {
    if ((UnityEngine.Object) StoreController.instance == (UnityEngine.Object) null)
      StoreController.instance = UnityEngine.Object.FindAnyObjectByType<StoreController>();
    return StoreController.instance.AllCosmeticsArraySO.SearchForCosmeticSO(playfabId);
  }

  public DynamicCosmeticStand FindCosmeticStandByCosmeticName(string PlayFabID)
  {
    foreach (DynamicCosmeticStand standByCosmeticName in this.CosmeticStandsDict.Values)
    {
      if (standByCosmeticName.thisCosmeticName == PlayFabID)
        return standByCosmeticName;
    }
    return (DynamicCosmeticStand) null;
  }

  public void FindAllDepartments()
  {
    this.Departments = ((IEnumerable<StoreDepartment>) UnityEngine.Object.FindObjectsByType<StoreDepartment>(FindObjectsSortMode.None)).ToList<StoreDepartment>();
  }

  public void SaveAllCosmeticsPositions()
  {
    foreach (StoreDepartment department in this.Departments)
    {
      foreach (StoreDisplay display in department.Displays)
      {
        foreach (DynamicCosmeticStand stand in display.Stands)
        {
          Debug.Log((object) $"StoreStuff: Saving Items mount transform: {department.departmentName}|{display.displayName}|{stand.StandName}|{stand.DisplayHeadModel.bustType.ToString()}|{stand.thisCosmeticName}");
          stand.UpdateCosmeticsMountPositions();
        }
      }
    }
  }

  public static void SetForGame()
  {
    if ((UnityEngine.Object) StoreController.instance == (UnityEngine.Object) null)
      StoreController.instance = UnityEngine.Object.FindAnyObjectByType<StoreController>();
    StoreController.instance.CreateDynamicCosmeticStandsDictionatary();
    foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.CosmeticStandsDict.Values)
    {
      dynamicCosmeticStand.SetStandType(dynamicCosmeticStand.DisplayHeadModel.bustType);
      dynamicCosmeticStand.SpawnItemOntoStand(dynamicCosmeticStand.thisCosmeticName);
    }
  }
}
