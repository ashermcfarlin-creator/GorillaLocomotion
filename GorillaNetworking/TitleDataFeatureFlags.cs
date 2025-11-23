// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.TitleDataFeatureFlags
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Newtonsoft.Json;
using PlayFab;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaNetworking;

public class TitleDataFeatureFlags
{
  public string TitleDataKey = "DeployFeatureFlags";
  public Dictionary<string, bool> defaults = new Dictionary<string, bool>()
  {
    {
      "2024-06-CosmeticsAuthenticationV2",
      true
    },
    {
      "2025-04-CosmeticsAuthenticationV2-SetData",
      false
    },
    {
      "2025-04-CosmeticsAuthenticationV2-ReadData",
      false
    },
    {
      "2025-04-CosmeticsAuthenticationV2-Compat",
      true
    }
  };
  private Dictionary<string, int> flagValueByName = new Dictionary<string, int>();
  private Dictionary<string, List<string>> flagValueByUser = new Dictionary<string, List<string>>();
  private Dictionary<string, bool> logSent = new Dictionary<string, bool>();

  public bool ready { get; private set; }

  public void FetchFeatureFlags()
  {
    PlayFabTitleDataCache.Instance.GetTitleData(this.TitleDataKey, (Action<string>) (json =>
    {
      FeatureFlagListData featureFlagListData = JsonUtility.FromJson<FeatureFlagListData>(json);
      foreach (FeatureFlagData flag in featureFlagListData.flags)
      {
        if (flag.valueType == "percent")
          this.flagValueByName.AddOrUpdate<string, int>(flag.name, flag.value);
        List<string> alwaysOnForUsers = flag.alwaysOnForUsers;
        // ISSUE: explicit non-virtual call
        if ((alwaysOnForUsers != null ? (__nonvirtual (alwaysOnForUsers.Count) > 0 ? 1 : 0) : 0) != 0)
          this.flagValueByUser.AddOrUpdate<string, List<string>>(flag.name, flag.alwaysOnForUsers);
      }
      Debug.Log((object) $"GorillaServer: Fetched flags ({featureFlagListData})");
      this.ready = true;
    }), (Action<PlayFabError>) (e =>
    {
      Debug.LogError((object) ("Error fetching rollout feature flags: " + e.ErrorMessage));
      this.ready = true;
    }));
  }

  public bool IsEnabledForUser(string flagName)
  {
    bool flag1;
    this.logSent.TryGetValue(flagName, out flag1);
    this.logSent[flagName] = true;
    string playFabPlayerId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
    if (!flag1)
      Debug.Log((object) $"GorillaServer: Checking flag {flagName} for {playFabPlayerId}\nFlag values:\n{JsonConvert.SerializeObject((object) this.flagValueByName)}\n\nDefaults:\n{JsonConvert.SerializeObject((object) this.defaults)}");
    List<string> stringList;
    // ISSUE: explicit non-virtual call
    if (this.flagValueByUser.TryGetValue(flagName, out stringList) && stringList != null && __nonvirtual (stringList.Contains(playFabPlayerId)))
      return true;
    int num1;
    if (!this.flagValueByName.TryGetValue(flagName, out num1))
    {
      if (!flag1)
        Debug.Log((object) "GorillaServer: Returning default");
      bool flag2;
      return this.defaults.TryGetValue(flagName, out flag2) & flag2;
    }
    if (!flag1)
      Debug.Log((object) $"GorillaServer: Rollout % is {num1}");
    if (num1 <= 0)
    {
      if (!flag1)
        Debug.Log((object) $"GorillaServer: {flagName} is off (<=0%).");
      return false;
    }
    if (num1 >= 100)
    {
      if (!flag1)
        Debug.Log((object) $"GorillaServer: {flagName} is on (>=100%).");
      return true;
    }
    uint num2 = XXHash32.Compute(ReadOnlySpan<byte>.op_Implicit(System.Text.Encoding.UTF8.GetBytes(playFabPlayerId))) % 100U;
    if (!flag1)
      Debug.Log((object) $"GorillaServer: Partial rollout, seed = {num2} flag value = {(long) num2 < (long) num1}");
    return (long) num2 < (long) num1;
  }
}
