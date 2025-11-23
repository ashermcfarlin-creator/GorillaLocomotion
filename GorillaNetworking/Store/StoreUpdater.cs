// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.StoreUpdater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using FXP;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace GorillaNetworking.Store;

public class StoreUpdater : MonoBehaviour
{
  public static volatile StoreUpdater instance;
  private DateTime StoreItemsChangeTimeUTC;
  private Dictionary<string, CosmeticItemPrefab> cosmeticItemPrefabsDictionary = new Dictionary<string, CosmeticItemPrefab>();
  private Dictionary<string, List<StoreUpdateEvent>> pedestalUpdateEvents = new Dictionary<string, List<StoreUpdateEvent>>();
  private Dictionary<string, Coroutine> pedestalUpdateCoroutines = new Dictionary<string, Coroutine>();
  private Dictionary<string, Coroutine> pedestalClearCartCoroutines = new Dictionary<string, Coroutine>();
  private string tempJson;
  private bool bLoadFromJSON = true;
  private bool bUsePlaceHolderJSON;

  public DateTime DateTimeNowServerAdjusted => GorillaComputer.instance.GetServerTime();

  public void Awake()
  {
    if ((UnityEngine.Object) StoreUpdater.instance == (UnityEngine.Object) null)
    {
      StoreUpdater.instance = this;
    }
    else
    {
      if (!((UnityEngine.Object) StoreUpdater.instance != (UnityEngine.Object) this))
        return;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
  }

  private void OnApplicationFocus(bool hasFocus)
  {
    if (hasFocus)
      this.HandleHMDMounted();
    else
      this.HandleHMDUnmounted();
  }

  public void Initialize()
  {
    this.FindAllCosmeticItemPrefabs();
    OVRManager.HMDMounted += new Action(this.HandleHMDMounted);
    OVRManager.HMDUnmounted += new Action(this.HandleHMDUnmounted);
    OVRManager.HMDLost += new Action(this.HandleHMDUnmounted);
    OVRManager.HMDAcquired += new Action(this.HandleHMDMounted);
    Debug.Log((object) "StoreUpdater - Starting");
    if (!this.bLoadFromJSON)
      return;
    this.StartCoroutine(this.InitializeTitleData());
  }

  private void ServerTimeUpdater() => this.StartCoroutine(this.InitializeTitleData());

  public void OnDestroy()
  {
    OVRManager.HMDMounted -= new Action(this.HandleHMDMounted);
    OVRManager.HMDUnmounted -= new Action(this.HandleHMDUnmounted);
    OVRManager.HMDLost -= new Action(this.HandleHMDUnmounted);
    OVRManager.HMDAcquired -= new Action(this.HandleHMDMounted);
  }

  private void HandleHMDUnmounted()
  {
    foreach (string key in this.pedestalUpdateCoroutines.Keys)
    {
      if (this.pedestalUpdateCoroutines[key] != null)
        this.StopCoroutine(this.pedestalUpdateCoroutines[key]);
    }
    foreach (string key in this.cosmeticItemPrefabsDictionary.Keys)
    {
      if ((UnityEngine.Object) this.cosmeticItemPrefabsDictionary[key] != (UnityEngine.Object) null)
        this.cosmeticItemPrefabsDictionary[key].StopCountdownCoroutine();
    }
  }

  private void HandleHMDMounted()
  {
    foreach (string key in this.cosmeticItemPrefabsDictionary.Keys)
    {
      if ((UnityEngine.Object) this.cosmeticItemPrefabsDictionary[key] != (UnityEngine.Object) null && this.pedestalUpdateEvents.ContainsKey(key) && this.cosmeticItemPrefabsDictionary[key].gameObject.activeInHierarchy)
      {
        this.CheckEventsOnResume(this.pedestalUpdateEvents[key]);
        this.StartNextEvent(key, false);
      }
    }
  }

  private void FindAllCosmeticItemPrefabs()
  {
    foreach (CosmeticItemPrefab cosmeticItemPrefab in UnityEngine.Object.FindObjectsByType<CosmeticItemPrefab>(FindObjectsSortMode.None))
    {
      if (this.cosmeticItemPrefabsDictionary.ContainsKey(cosmeticItemPrefab.PedestalID))
      {
        Debug.LogWarning((object) ("StoreUpdater - Duplicate Pedestal ID " + cosmeticItemPrefab.PedestalID));
      }
      else
      {
        Debug.Log((object) ("StoreUpdater - Adding Pedestal " + cosmeticItemPrefab.PedestalID));
        this.cosmeticItemPrefabsDictionary.Add(cosmeticItemPrefab.PedestalID, cosmeticItemPrefab);
      }
    }
  }

  private IEnumerator HandlePedestalUpdate(StoreUpdateEvent updateEvent, bool playFX)
  {
    StoreUpdater storeUpdater = this;
    storeUpdater.cosmeticItemPrefabsDictionary[updateEvent.PedestalID].SetStoreUpdateEvent(updateEvent, playFX);
    yield return (object) new WaitForSeconds((float) (updateEvent.EndTimeUTC.ToUniversalTime() - storeUpdater.DateTimeNowServerAdjusted).TotalSeconds);
    if (storeUpdater.pedestalClearCartCoroutines.ContainsKey(updateEvent.PedestalID))
    {
      if (storeUpdater.pedestalClearCartCoroutines[updateEvent.PedestalID] != null)
        storeUpdater.StopCoroutine(storeUpdater.pedestalClearCartCoroutines[updateEvent.PedestalID]);
      storeUpdater.pedestalClearCartCoroutines[updateEvent.PedestalID] = storeUpdater.StartCoroutine(storeUpdater.HandleClearCart(updateEvent));
    }
    else
      storeUpdater.pedestalClearCartCoroutines.Add(updateEvent.PedestalID, storeUpdater.StartCoroutine(storeUpdater.HandleClearCart(updateEvent)));
    if (storeUpdater.cosmeticItemPrefabsDictionary[updateEvent.PedestalID].gameObject.activeInHierarchy)
    {
      storeUpdater.pedestalUpdateEvents[updateEvent.PedestalID].RemoveAt(0);
      storeUpdater.StartNextEvent(updateEvent.PedestalID, true);
    }
  }

  private IEnumerator HandleClearCart(StoreUpdateEvent updateEvent)
  {
    yield return (object) new WaitForSeconds(Math.Clamp((float) (updateEvent.EndTimeUTC.ToUniversalTime() - this.DateTimeNowServerAdjusted).TotalSeconds + 60f, 0.0f, 60f));
    if (CosmeticsController.instance.RemoveItemFromCart(CosmeticsController.instance.GetItemFromDict(updateEvent.ItemName)))
    {
      CosmeticsController.instance.ClearCheckout(true);
      CosmeticsController.instance.UpdateShoppingCart();
      CosmeticsController.instance.UpdateWornCosmetics(true);
    }
  }

  private void StartNextEvent(string pedestalID, bool playFX)
  {
    if (this.pedestalUpdateEvents[pedestalID].Count > 0)
    {
      Coroutine coroutine = this.StartCoroutine(this.HandlePedestalUpdate(this.pedestalUpdateEvents[pedestalID].First<StoreUpdateEvent>(), playFX));
      if (this.pedestalUpdateCoroutines.ContainsKey(pedestalID))
      {
        if (this.pedestalUpdateCoroutines[pedestalID] != null && this.pedestalUpdateCoroutines[pedestalID] != null)
          this.StopCoroutine(this.pedestalUpdateCoroutines[pedestalID]);
        this.pedestalUpdateCoroutines[pedestalID] = coroutine;
      }
      else
        this.pedestalUpdateCoroutines.Add(pedestalID, coroutine);
      if (this.pedestalUpdateEvents[pedestalID].Count != 0 || this.bLoadFromJSON)
        return;
      this.GetStoreUpdateEventsPlaceHolder(pedestalID);
    }
    else
    {
      if (this.bLoadFromJSON)
        return;
      this.GetStoreUpdateEventsPlaceHolder(pedestalID);
      this.StartNextEvent(pedestalID, true);
    }
  }

  private void GetStoreUpdateEventsPlaceHolder(string PedestalID)
  {
    List<StoreUpdateEvent> storeUpdateEventList = new List<StoreUpdateEvent>();
    List<StoreUpdateEvent> tempEvents = this.CreateTempEvents(PedestalID, 1, 15);
    this.CheckEvents(tempEvents);
    if (this.pedestalUpdateEvents.ContainsKey(PedestalID))
      this.pedestalUpdateEvents[PedestalID].AddRange((IEnumerable<StoreUpdateEvent>) tempEvents);
    else
      this.pedestalUpdateEvents.Add(PedestalID, tempEvents);
  }

  private void CheckEvents(List<StoreUpdateEvent> updateEvents)
  {
    for (int index = 0; index < updateEvents.Count; ++index)
    {
      if (updateEvents[index].EndTimeUTC.ToUniversalTime() < this.DateTimeNowServerAdjusted)
      {
        updateEvents.RemoveAt(index);
        --index;
      }
    }
  }

  private void CheckEventsOnResume(List<StoreUpdateEvent> updateEvents)
  {
    bool flag = false;
    for (int index = 0; index < updateEvents.Count; ++index)
    {
      if (updateEvents[index].EndTimeUTC.ToUniversalTime() < this.DateTimeNowServerAdjusted)
      {
        if ((double) Math.Clamp((float) (updateEvents[index].EndTimeUTC.ToUniversalTime() - this.DateTimeNowServerAdjusted).TotalSeconds + 60f, 0.0f, 60f) <= 0.0)
          flag ^= CosmeticsController.instance.RemoveItemFromCart(CosmeticsController.instance.GetItemFromDict(updateEvents[index].ItemName));
        else if (this.pedestalClearCartCoroutines.ContainsKey(updateEvents[index].PedestalID))
        {
          if (this.pedestalClearCartCoroutines[updateEvents[index].PedestalID] != null)
            this.StopCoroutine(this.pedestalClearCartCoroutines[updateEvents[index].PedestalID]);
          this.pedestalClearCartCoroutines[updateEvents[index].PedestalID] = this.StartCoroutine(this.HandleClearCart(updateEvents[index]));
        }
        else
          this.pedestalClearCartCoroutines.Add(updateEvents[index].PedestalID, this.StartCoroutine(this.HandleClearCart(updateEvents[index])));
        updateEvents.RemoveAt(index);
        --index;
      }
    }
    if (!flag)
      return;
    CosmeticsController.instance.ClearCheckout(true);
    CosmeticsController.instance.UpdateShoppingCart();
    CosmeticsController.instance.UpdateWornCosmetics(true);
  }

  private IEnumerator InitializeTitleData()
  {
    yield return (object) new WaitForSeconds(1f);
    PlayFabTitleDataCache.Instance.UpdateData();
    yield return (object) new WaitForSeconds(1f);
    this.GetEventsFromTitleData();
  }

  private void GetEventsFromTitleData()
  {
    Debug.Log((object) "StoreUpdater - GetEventsFromTitleData");
    if (this.bUsePlaceHolderJSON)
      this.HandleRecievingEventsFromTitleData(StoreUpdateEvent.DeserializeFromJSonList(StoreUpdateEvent.SerializeArrayAsJSon(this.CreateTempEvents("Pedestal1", 2, 120, new DateTime(2024, 2, 13, 16 /*0x10*/, 0, 0, DateTimeKind.Utc)).ToArray())));
    else
      PlayFabTitleDataCache.Instance.GetTitleData("TOTD", (Action<string>) (result =>
      {
        Debug.Log((object) ("StoreUpdater - Recieved TitleData : " + result));
        this.HandleRecievingEventsFromTitleData(StoreUpdateEvent.DeserializeFromJSonList(result));
      }), (Action<PlayFabError>) (error => Debug.Log((object) ("StoreUpdater - Error Title Data : " + error.ErrorMessage))));
  }

  private void HandleRecievingEventsFromTitleData(List<StoreUpdateEvent> updateEvents)
  {
    Debug.Log((object) "StoreUpdater - HandleRecievingEventsFromTitleData");
    this.CheckEvents(updateEvents);
    if (CosmeticsController.instance.GetItemFromDict("LBAEY.").isNullItem)
    {
      Debug.LogWarning((object) "StoreUpdater - CosmeticsController is not initialized.  Reinitializing TitleData");
      this.StartCoroutine(this.InitializeTitleData());
    }
    else
    {
      foreach (StoreUpdateEvent updateEvent in updateEvents)
      {
        if (this.pedestalUpdateEvents.ContainsKey(updateEvent.PedestalID))
        {
          this.pedestalUpdateEvents[updateEvent.PedestalID].Add(updateEvent);
        }
        else
        {
          this.pedestalUpdateEvents.Add(updateEvent.PedestalID, new List<StoreUpdateEvent>());
          this.pedestalUpdateEvents[updateEvent.PedestalID].Add(updateEvent);
        }
      }
      Debug.Log((object) "StoreUpdater - Starting Events");
      foreach (string key in this.pedestalUpdateEvents.Keys)
      {
        if (this.cosmeticItemPrefabsDictionary.ContainsKey(key))
        {
          Debug.Log((object) ("StoreUpdater - Starting Event " + key));
          this.StartNextEvent(key, false);
        }
      }
      foreach (string key in this.cosmeticItemPrefabsDictionary.Keys)
      {
        if (!this.pedestalUpdateEvents.ContainsKey(key))
        {
          Debug.Log((object) ("StoreUpdater - Adding PlaceHolder Events " + key));
          this.GetStoreUpdateEventsPlaceHolder(key);
          this.StartNextEvent(key, false);
        }
      }
    }
  }

  private void PrintJSONEvents()
  {
    string json = StoreUpdateEvent.SerializeArrayAsJSon(this.CreateTempEvents("Pedestal1", 5, 28).ToArray());
    foreach (StoreUpdateEvent deserializeFromJson in StoreUpdateEvent.DeserializeFromJSonList(json))
      Debug.Log((object) $"Event : {deserializeFromJson.ItemName} : {deserializeFromJson.StartTimeUTC.ToString()} : {deserializeFromJson.EndTimeUTC.ToString()}");
    Debug.Log((object) ("NewEvents :\n" + json));
    this.tempJson = json;
  }

  private List<StoreUpdateEvent> CreateTempEvents(
    string PedestalID,
    int minuteDelay,
    int totalEvents)
  {
    string[] strArray = new string[14]
    {
      "LBAEY.",
      "LBAEZ.",
      "LBAFA.",
      "LBAFB.",
      "LBAFC.",
      "LBAFD.",
      "LBAFE.",
      "LBAFF.",
      "LBAFG.",
      "LBAFH.",
      "LBAFO.",
      "LBAFP.",
      "LBAFQ.",
      "LBAFR."
    };
    List<StoreUpdateEvent> tempEvents = new List<StoreUpdateEvent>();
    for (int index = 0; index < totalEvents; ++index)
    {
      StoreUpdateEvent storeUpdateEvent = new StoreUpdateEvent(PedestalID, strArray[index % 14], DateTime.UtcNow + TimeSpan.FromMinutes((double) (minuteDelay * index)), DateTime.UtcNow + TimeSpan.FromMinutes((double) (minuteDelay * (index + 1))));
      tempEvents.Add(storeUpdateEvent);
    }
    return tempEvents;
  }

  private List<StoreUpdateEvent> CreateTempEvents(
    string PedestalID,
    int minuteDelay,
    int totalEvents,
    DateTime startTime)
  {
    string[] strArray = new string[14]
    {
      "LBAEY.",
      "LBAEZ.",
      "LBAFA.",
      "LBAFB.",
      "LBAFC.",
      "LBAFD.",
      "LBAFE.",
      "LBAFF.",
      "LBAFG.",
      "LBAFH.",
      "LBAFO.",
      "LBAFP.",
      "LBAFQ.",
      "LBAFR."
    };
    List<StoreUpdateEvent> tempEvents = new List<StoreUpdateEvent>();
    for (int index = 0; index < totalEvents; ++index)
    {
      StoreUpdateEvent storeUpdateEvent = new StoreUpdateEvent(PedestalID, strArray[index % 14], startTime + TimeSpan.FromMinutes((double) (minuteDelay * index)), startTime + TimeSpan.FromMinutes((double) (minuteDelay * (index + 1))));
      tempEvents.Add(storeUpdateEvent);
    }
    return tempEvents;
  }

  public void PedestalAsleep(CosmeticItemPrefab pedestal)
  {
    if (!this.pedestalUpdateCoroutines.ContainsKey(pedestal.PedestalID) || this.pedestalUpdateCoroutines[pedestal.PedestalID] == null)
      return;
    this.StopCoroutine(this.pedestalUpdateCoroutines[pedestal.PedestalID]);
  }

  public void PedestalAwakened(CosmeticItemPrefab pedestal)
  {
    if (!this.cosmeticItemPrefabsDictionary.ContainsKey(pedestal.PedestalID))
      this.cosmeticItemPrefabsDictionary.Add(pedestal.PedestalID, pedestal);
    if (!this.pedestalUpdateEvents.ContainsKey(pedestal.PedestalID))
      return;
    this.CheckEventsOnResume(this.pedestalUpdateEvents[pedestal.PedestalID]);
    this.StartNextEvent(pedestal.PedestalID, false);
  }
}
