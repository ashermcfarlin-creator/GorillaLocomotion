// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.StoreDisplay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaNetworking.Store;

public class StoreDisplay : MonoBehaviour
{
  public string displayName = "";
  public DynamicCosmeticStand[] Stands;

  private void GetAllDynamicCosmeticStands()
  {
    this.Stands = this.GetComponentsInChildren<DynamicCosmeticStand>();
  }

  private void SetDisplayNameForAllStands()
  {
    foreach (DynamicCosmeticStand componentsInChild in this.GetComponentsInChildren<DynamicCosmeticStand>())
      componentsInChild.CopyChildsName();
  }
}
