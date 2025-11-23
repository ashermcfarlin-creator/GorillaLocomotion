// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.StoreUpdateEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using LitJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaNetworking.Store;

public class StoreUpdateEvent
{
  public string PedestalID;
  public string ItemName;
  public DateTime StartTimeUTC;
  public DateTime EndTimeUTC;

  public StoreUpdateEvent()
  {
  }

  public StoreUpdateEvent(
    string pedestalID,
    string itemName,
    DateTime startTimeUTC,
    DateTime endTimeUTC)
  {
    this.PedestalID = pedestalID;
    this.ItemName = itemName;
    this.StartTimeUTC = startTimeUTC;
    this.EndTimeUTC = endTimeUTC;
  }

  public static string SerializeAsJSon(StoreUpdateEvent storeEvent)
  {
    return JsonUtility.ToJson((object) storeEvent);
  }

  public static string SerializeArrayAsJSon(StoreUpdateEvent[] storeEvents)
  {
    return JsonConvert.SerializeObject((object) storeEvents);
  }

  public static StoreUpdateEvent DeserializeFromJSon(string json)
  {
    return JsonUtility.FromJson<StoreUpdateEvent>(json);
  }

  public static StoreUpdateEvent[] DeserializeFromJSonArray(string json)
  {
    List<StoreUpdateEvent> storeUpdateEventList = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
    storeUpdateEventList.Sort((Comparison<StoreUpdateEvent>) ((x, y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC)));
    return storeUpdateEventList.ToArray();
  }

  public static List<StoreUpdateEvent> DeserializeFromJSonList(string json)
  {
    List<StoreUpdateEvent> storeUpdateEventList = JsonMapper.ToObject<List<StoreUpdateEvent>>(json);
    storeUpdateEventList.Sort((Comparison<StoreUpdateEvent>) ((x, y) => x.StartTimeUTC.CompareTo(y.StartTimeUTC)));
    return storeUpdateEventList;
  }
}
