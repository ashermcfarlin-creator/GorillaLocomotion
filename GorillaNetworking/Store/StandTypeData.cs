// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.StandTypeData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaNetworking.Store;

public class StandTypeData
{
  public string departmentID = "";
  public string displayID = "";
  public string standID = "";
  public string bustType = "";
  public string playFabID = "";

  public StandTypeData(string[] spawnData)
  {
    this.departmentID = spawnData[0];
    this.displayID = spawnData[1];
    this.standID = spawnData[2];
    this.bustType = spawnData[3];
    if (spawnData.Length == 5)
      this.playFabID = spawnData[4];
    Debug.Log((object) $"StoreStuff: StandTypeData: {this.departmentID}\n{this.displayID}\n{this.standID}\n{this.bustType}\n{this.playFabID}");
  }

  public StandTypeData(
    string departmentID,
    string displayID,
    string standID,
    HeadModel_CosmeticStand.BustType bustType,
    string playFabID)
  {
    this.departmentID = departmentID;
    this.displayID = displayID;
    this.standID = standID;
    this.bustType = bustType.ToString();
    this.playFabID = playFabID;
  }

  public enum EStandDataID
  {
    departmentID,
    displayID,
    standID,
    bustType,
    playFabID,
    Count,
  }
}
