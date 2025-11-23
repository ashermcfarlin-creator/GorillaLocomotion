// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.CosmeticItemRegistry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTag.CosmeticSystem;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaNetworking;

public class CosmeticItemRegistry
{
  private bool _isInitialized;
  private Dictionary<string, CosmeticItemInstance> nameToCosmeticMap = new Dictionary<string, CosmeticItemInstance>();
  private GameObject nullItem;

  public void Initialize(GameObject[] cosmeticGObjs)
  {
    if (this._isInitialized)
      return;
    this._isInitialized = true;
    foreach (GameObject cosmeticGobj in cosmeticGObjs)
    {
      string str = cosmeticGobj.name.Replace("LEFT.", "").Replace("RIGHT.", "").TrimEnd();
      CosmeticItemInstance cosmeticItemInstance;
      if (this.nameToCosmeticMap.ContainsKey(str))
      {
        cosmeticItemInstance = this.nameToCosmeticMap[str];
      }
      else
      {
        cosmeticItemInstance = new CosmeticItemInstance();
        CosmeticSO soFromDisplayName = CosmeticsController.instance.GetCosmeticSOFromDisplayName(str);
        cosmeticItemInstance.clippingOffsets = (Object) soFromDisplayName != (Object) null ? soFromDisplayName.info.anchorAntiIntersectOffsets : CosmeticsController.instance.defaultClipOffsets;
        cosmeticItemInstance.isHoldableItem = (Object) soFromDisplayName != (Object) null && soFromDisplayName.info.hasHoldableParts;
        this.nameToCosmeticMap.Add(str, cosmeticItemInstance);
      }
      HoldableObject component = cosmeticGobj.GetComponent<HoldableObject>();
      bool flag1 = cosmeticGobj.name.Contains("LEFT.");
      bool flag2 = cosmeticGobj.name.Contains("RIGHT.");
      if (cosmeticItemInstance.isHoldableItem && (Object) component != (Object) null)
      {
        if (component is SnowballThrowable || component is TransferrableObject)
          cosmeticItemInstance.holdableObjects.Add(cosmeticGobj);
        else if (flag1)
          cosmeticItemInstance.leftObjects.Add(cosmeticGobj);
        else if (flag2)
          cosmeticItemInstance.rightObjects.Add(cosmeticGobj);
        else
          cosmeticItemInstance.objects.Add(cosmeticGobj);
      }
      else if (flag1)
        cosmeticItemInstance.leftObjects.Add(cosmeticGobj);
      else if (flag2)
        cosmeticItemInstance.rightObjects.Add(cosmeticGobj);
      else
        cosmeticItemInstance.objects.Add(cosmeticGobj);
      cosmeticItemInstance.dbgname = str;
    }
  }

  public CosmeticItemInstance Cosmetic(string itemName)
  {
    if (!this._isInitialized)
    {
      Debug.LogError((object) "Tried to use CosmeticItemRegistry before it was initialized!");
      return (CosmeticItemInstance) null;
    }
    if (string.IsNullOrEmpty(itemName) || itemName == "NOTHING")
      return (CosmeticItemInstance) null;
    CosmeticItemInstance cosmeticItemInstance;
    return !this.nameToCosmeticMap.TryGetValue(itemName, out cosmeticItemInstance) ? (CosmeticItemInstance) null : cosmeticItemInstance;
  }
}
