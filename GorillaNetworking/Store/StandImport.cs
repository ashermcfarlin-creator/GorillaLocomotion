// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.StandImport
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaNetworking.Store;

public class StandImport
{
  public List<StandTypeData> standData = new List<StandTypeData>();

  public void DecomposeFromTitleDataString(string data)
  {
    foreach (string dataString in data.Split("\\n", StringSplitOptions.None))
      this.DecomposeStandDataTitleData(dataString);
  }

  public void DecomposeStandDataTitleData(string dataString)
  {
    string[] spawnData = dataString.Split("\\t", StringSplitOptions.None);
    if (spawnData.Length == 5)
      this.standData.Add(new StandTypeData(spawnData));
    else if (spawnData.Length == 4)
    {
      this.standData.Add(new StandTypeData(spawnData));
    }
    else
    {
      string str1 = "";
      foreach (string str2 in spawnData)
        str1 = $"{str1}{str2}|";
      Debug.LogError((object) ("Store Importer Data String is not valid : " + str1));
    }
  }

  public void DeserializeFromJSON(string JSONString)
  {
    this.standData = JsonConvert.DeserializeObject<List<StandTypeData>>(JSONString);
  }

  public void DecomposeStandData(string dataString)
  {
    string[] spawnData = dataString.Split('\t', StringSplitOptions.None);
    if (spawnData.Length == 5)
      this.standData.Add(new StandTypeData(spawnData));
    else if (spawnData.Length == 4)
    {
      this.standData.Add(new StandTypeData(spawnData));
    }
    else
    {
      string str1 = "";
      foreach (string str2 in spawnData)
        str1 = $"{str1}{str2}|";
      Debug.LogError((object) ("Store Importer Data String is not valid : " + str1));
    }
  }
}
