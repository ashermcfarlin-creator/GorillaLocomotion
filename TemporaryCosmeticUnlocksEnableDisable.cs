// Decompiled with JetBrains decompiler
// Type: GorillaTag.TemporaryCosmeticUnlocksEnableDisable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaNetworking;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag;

public class TemporaryCosmeticUnlocksEnableDisable : MonoBehaviour
{
  [SerializeField]
  private CosmeticWardrobe m_wardrobe;
  [SerializeField]
  private GameObject m_cosmeticAreaTrigger;
  private TickSystemTimer m_timer;

  private void Awake()
  {
    if (this.m_wardrobe.IsNull() || this.m_cosmeticAreaTrigger.IsNull())
    {
      Debug.LogError((object) "TemporaryCosmeticUnlocksEnableDisable: reference is null, disabling self");
      this.enabled = false;
    }
    if (!CosmeticsController.instance.IsNull() && this.m_wardrobe.WardrobeButtonsInitialized())
      return;
    this.enabled = false;
    this.m_timer = new TickSystemTimer(0.05f, new Action(this.CheckWardrobeRady));
    this.m_timer.Start();
  }

  private void OnEnable()
  {
    bool tempUnlocksEnabled = PlayerCosmeticsSystem.TempUnlocksEnabled;
    this.m_wardrobe.UseTemporarySet = tempUnlocksEnabled;
    this.m_cosmeticAreaTrigger.SetActive(tempUnlocksEnabled);
  }

  private void CheckWardrobeRady()
  {
    if (CosmeticsController.instance.IsNotNull() && this.m_wardrobe.WardrobeButtonsInitialized())
    {
      this.m_timer.Stop();
      this.m_timer = (TickSystemTimer) null;
      this.enabled = true;
    }
    else
      this.m_timer.Start();
  }
}
