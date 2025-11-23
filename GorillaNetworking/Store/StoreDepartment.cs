// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.StoreDepartment
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaNetworking.Store;

public class StoreDepartment : MonoBehaviour
{
  public StoreDisplay[] Displays;
  public string departmentName = "";

  private void FindAllDisplays()
  {
    this.Displays = this.GetComponentsInChildren<StoreDisplay>();
    for (int index = this.Displays.Length - 1; index >= 0; --index)
    {
      if (string.IsNullOrEmpty(this.Displays[index].displayName))
      {
        this.Displays[index] = this.Displays[this.Displays.Length - 1];
        Array.Resize<StoreDisplay>(ref this.Displays, this.Displays.Length - 1);
      }
    }
  }
}
