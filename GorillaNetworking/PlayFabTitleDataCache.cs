// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.PlayFabTitleDataCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using LitJson;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaNetworking;

public class PlayFabTitleDataCache : MonoBehaviour
{
  public PlayFabTitleDataCache.DataUpdate OnTitleDataUpdate;
  private const string FileName = "TitleDataCache.json";
  private readonly List<PlayFabTitleDataCache.DataRequest> requests = new List<PlayFabTitleDataCache.DataRequest>();
  private Dictionary<string, Dictionary<string, string>> localizedTitleData = new Dictionary<string, Dictionary<string, string>>();
  private Dictionary<string, bool> localesUpdated = new Dictionary<string, bool>();
  private bool isFirstLoad = true;
  private Coroutine updateDataCoroutine;

  public static PlayFabTitleDataCache Instance { get; private set; }

  private static string FilePath
  {
    get => Path.Combine(Application.persistentDataPath, "TitleDataCache.json");
  }

  public void GetTitleData(
    string name,
    Action<string> callback,
    Action<PlayFabError> errorCallback,
    bool ignoreCache = false)
  {
    Dictionary<string, string> dictionary;
    string data;
    if (!ignoreCache && !this.isFirstLoad && this.localizedTitleData.TryGetValue(LocalisationManager.CurrentLanguage.Identifier.Code, out dictionary) && dictionary.TryGetValue(name, out data))
    {
      callback.SafeInvoke<string>(data);
    }
    else
    {
      this.requests.Add(new PlayFabTitleDataCache.DataRequest()
      {
        Name = name,
        Callback = callback,
        ErrorCallback = errorCallback
      });
      this.TryUpdateData();
    }
  }

  private void Awake()
  {
    if ((UnityEngine.Object) PlayFabTitleDataCache.Instance != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
    else
      PlayFabTitleDataCache.Instance = this;
  }

  private void Start()
  {
    this.UpdateData();
    LocalisationManager.RegisterOnLanguageChanged(new Action(this.TryUpdateData));
  }

  private void OnDestroy()
  {
    LocalisationManager.UnregisterOnLanguageChanged(new Action(this.TryUpdateData));
  }

  private void TryUpdateData()
  {
    if (this.isFirstLoad || this.updateDataCoroutine != null)
      return;
    this.UpdateData();
  }

  public CacheImport LoadDataFromFile()
  {
    try
    {
      if (File.Exists(PlayFabTitleDataCache.FilePath))
        return JsonMapper.ToObject<CacheImport>(File.ReadAllText(PlayFabTitleDataCache.FilePath)) ?? new CacheImport();
      UnityEngine.Debug.LogWarning((object) $"[PlayFabTitleDataCache::LoadDataFromFile] Title data file {PlayFabTitleDataCache.FilePath} does not exist!");
      return (CacheImport) null;
    }
    catch (Exception ex)
    {
      UnityEngine.Debug.LogError((object) $"[PlayFabTitleDataCache::LoadDataFromFile] Error reading PlayFab title data from file: {ex}");
      return (CacheImport) null;
    }
  }

  private static void SaveDataToFile(
    string filepath,
    Dictionary<string, Dictionary<string, string>> titleData)
  {
    try
    {
      string json = JsonMapper.ToJson((object) new CacheImport()
      {
        DeploymentId = MothershipClientApiUnity.DeploymentId,
        TitleData = titleData
      });
      File.WriteAllText(filepath, json);
    }
    catch (Exception ex)
    {
      UnityEngine.Debug.LogError((object) $"[PlayFabTitleDataCache::SaveDataToFile] Error writing PlayFab title data to file: {ex}");
    }
  }

  public void UpdateData() => this.updateDataCoroutine = this.StartCoroutine(this.UpdateDataCo());

  private IEnumerator UpdateDataCo()
  {
    try
    {
      CacheImport oldCache = this.LoadDataFromFile();
      string currentLocale = LocalisationManager.CurrentLanguage.Identifier.Code;
      Dictionary<string, string> titleData;
      if (!this.localizedTitleData.TryGetValue(currentLocale, out titleData))
      {
        this.localizedTitleData[currentLocale] = new Dictionary<string, string>();
        titleData = this.localizedTitleData[currentLocale];
      }
      Dictionary<string, string> oldLocalizedCache;
      if (oldCache == null || oldCache.TitleData == null || !oldCache.TitleData.TryGetValue(currentLocale, out oldLocalizedCache))
        oldLocalizedCache = new Dictionary<string, string>();
      yield return (object) new WaitUntil((Func<bool>) (() => MothershipClientApiUnity.IsClientLoggedIn()));
      bool wipeOldData = oldCache == null || oldCache.DeploymentId != MothershipClientApiUnity.DeploymentId;
      Dictionary<string, string> newTitleData = (Dictionary<string, string>) null;
      string mothershipError = (string) null;
      Stopwatch sw = Stopwatch.StartNew();
      UnityEngine.Debug.Log((object) "[PlayFabTitleDataCache::UpdateDataCo] Starting Mothership API call");
      StringVector stringVector = new StringVector();
      foreach (PlayFabTitleDataCache.DataRequest request in this.requests)
        stringVector.Add(request.Name);
      bool finished = false;
      UnityEngine.Debug.Log((object) ("[PlayFabTitleDataCache::UpdateDataCo] Keys to fetch: " + string.Join(", ", (IEnumerable<string>) stringVector)));
      UnityEngine.Debug.Log((object) $"[PlayFabTitleDataCache::UpdateDataCo] Calling MothershipClientApiUnity.ListMothershipTitleData with TitleId={MothershipClientApiUnity.TitleId}, EnvironmentId={MothershipClientApiUnity.EnvironmentId}, DeploymentId={MothershipClientApiUnity.DeploymentId}, keys count={stringVector.Count}");
      if (!MothershipClientApiUnity.ListMothershipTitleData(MothershipClientApiUnity.TitleId, MothershipClientApiUnity.EnvironmentId, MothershipClientApiUnity.DeploymentId, stringVector, (Action<ListClientMothershipTitleDataResponse>) (response =>
      {
        UnityEngine.Debug.Log((object) $"[PlayFabTitleDataCache::UpdateDataCo] Mothership API success callback - Response: {response != null}, Results: {response?.Results?.Count.GetValueOrDefault()}");
        if (response != null && response.Results != null)
        {
          newTitleData = new Dictionary<string, string>();
          for (int index = 0; index < response.Results.Count; ++index)
          {
            MothershipTitleDataShort result = response.Results[index];
            // ISSUE: variable of a boxed type
            __Boxed<int> local = (ValueType) index;
            string key = result.key;
            string data = result.data;
            // ISSUE: variable of a boxed type
            __Boxed<int> length = (ValueType) (data != null ? data.Length : 0);
            UnityEngine.Debug.Log((object) $"[PlayFabTitleDataCache::UpdateDataCo] Processing title data item {local}: key='{key}', data length={length}");
            if (!string.IsNullOrEmpty(result.key))
              newTitleData[result.key] = result.data;
          }
          mothershipError = (string) null;
          UnityEngine.Debug.Log((object) $"[PlayFabTitleDataCache::UpdateDataCo] Successfully processed {newTitleData.Count} title data items");
        }
        else
        {
          mothershipError = "Failed to fetch title data - response or results were null";
          UnityEngine.Debug.LogError((object) ("[PlayFabTitleDataCache::UpdateDataCo] " + mothershipError));
        }
        finished = true;
      }), (Action<MothershipError, int>) ((error, statusCode) =>
      {
        mothershipError = $"Error fetching title data: {error?.Message ?? "Unknown error"} (Status: {statusCode})";
        UnityEngine.Debug.LogError((object) ("[PlayFabTitleDataCache::UpdateDataCo] Mothership API error callback - " + mothershipError));
        finished = true;
      })))
      {
        mothershipError = "Mothership API call was not sent.";
        UnityEngine.Debug.LogError((object) ("[PlayFabTitleDataCache::UpdateDataCo] " + mothershipError));
      }
      UnityEngine.Debug.Log((object) "[PlayFabTitleDataCache::UpdateDataCo] Waiting for Mothership API response");
      yield return (object) new WaitUntil((Func<bool>) (() => finished));
      UnityEngine.Debug.Log((object) $"[PlayFabTitleDataCache::UpdateDataCo] {sw.Elapsed.TotalSeconds:N5}s");
      if (newTitleData != null)
      {
        UnityEngine.Debug.Log((object) $"[PlayFabTitleDataCache::UpdateDataCo] Processing {newTitleData.Count} new title data items");
        if (wipeOldData)
        {
          this.localizedTitleData.Clear();
          this.localizedTitleData[currentLocale] = new Dictionary<string, string>();
          titleData = this.localizedTitleData[currentLocale];
        }
        if (!this.localesUpdated.ContainsKey(currentLocale))
          titleData.Clear();
        foreach (KeyValuePair<string, string> keyValuePair in newTitleData)
        {
          string str1;
          string str2;
          keyValuePair.Deconstruct(ref str1, ref str2);
          string key = str1;
          string str3 = str2;
          UnityEngine.Debug.Log((object) ("[PlayFabTitleDataCache::UpdateDataCo] Updating title data key: " + key));
          titleData[key] = str3;
          for (int index = this.requests.Count - 1; index >= 0; --index)
          {
            PlayFabTitleDataCache.DataRequest request = this.requests[index];
            if (request.Name == key)
            {
              Action<string> callback = request.Callback;
              if (callback != null)
                callback(str3);
              this.requests.RemoveAt(index);
              break;
            }
          }
          string str4;
          if (oldLocalizedCache.TryGetValue(key, out str4) && str4 != str3)
            this.OnTitleDataUpdate?.Invoke(key);
        }
        this.localesUpdated[currentLocale] = true;
        PlayFabTitleDataCache.SaveDataToFile(PlayFabTitleDataCache.FilePath, this.localizedTitleData);
      }
      oldCache = (CacheImport) null;
      currentLocale = (string) null;
      titleData = (Dictionary<string, string>) null;
      oldLocalizedCache = (Dictionary<string, string>) null;
      sw = (Stopwatch) null;
    }
    finally
    {
      this.ClearRequestWithError();
      this.isFirstLoad = false;
      this.updateDataCoroutine = (Coroutine) null;
    }
  }

  private static string MD5(string value)
  {
    byte[] hash = new MD5CryptoServiceProvider().ComputeHash(System.Text.Encoding.Default.GetBytes(value));
    StringBuilder stringBuilder = new StringBuilder();
    foreach (byte num in hash)
      stringBuilder.Append(num.ToString("x2"));
    return stringBuilder.ToString();
  }

  private void ClearRequestWithError(PlayFabError e = null)
  {
    if (e == null)
      e = new PlayFabError();
    foreach (PlayFabTitleDataCache.DataRequest request in this.requests)
      request.ErrorCallback.SafeInvoke<PlayFabError>(e);
    this.requests.Clear();
  }

  [Serializable]
  public sealed class DataUpdate : UnityEvent<string>
  {
  }

  private class DataRequest
  {
    public string Name { get; set; }

    public Action<string> Callback { get; set; }

    public Action<PlayFabError> ErrorCallback { get; set; }
  }
}
