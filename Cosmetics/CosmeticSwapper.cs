// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.CosmeticSwapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaGameModes;
using GorillaNetworking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class CosmeticSwapper : MonoBehaviour
{
  [SerializeField]
  private List<string> cosmeticIDs = new List<string>();
  [SerializeField]
  private CosmeticSwapper.SwapMode swapMode = CosmeticSwapper.SwapMode.StepByStep;
  [SerializeField]
  private float stepTimeout = 10f;
  [Tooltip("Hold final step as long as the swapper is being called within the timeframe")]
  [SerializeField]
  private bool holdFinalStep = true;
  [SerializeField]
  private UnityEvent<VRRig> OnSwappingSequenceCompleted;
  [SerializeField]
  private List<GameModeType> gameModeExclusion = new List<GameModeType>();
  private CosmeticsController controller;

  private void Awake() => this.controller = CosmeticsController.instance;

  private void OnEnable()
  {
    PlayerCosmeticsSystem.UnlockTemporaryCosmeticsGlobal((IReadOnlyList<string>) this.cosmeticIDs);
  }

  private void OnDisable()
  {
    PlayerCosmeticsSystem.LockTemporaryCosmeticsGlobal((IReadOnlyList<string>) this.cosmeticIDs);
  }

  public void SwapInCosmetic(VRRig vrRig) => this.TriggerSwap(vrRig);

  public CosmeticSwapper.SwapMode GetCurrentMode() => this.swapMode;

  public bool ShouldHoldFinalStep() => this.holdFinalStep;

  public int GetCurrentStepIndex(VRRig rig)
  {
    return (Object) rig == (Object) null ? 0 : rig.CosmeticStepIndex;
  }

  public int GetNumberOfSteps() => this.cosmeticIDs.Count;

  private void TriggerSwap(VRRig rig)
  {
    if ((Object) GorillaGameManager.instance != (Object) null && this.gameModeExclusion.Contains(GorillaGameManager.instance.GameType()) || (Object) rig == (Object) null || (Object) this.controller == (Object) null || this.cosmeticIDs.Count == 0 || (Object) rig != (Object) GorillaTagger.Instance.offlineVRRig)
      return;
    rig.SetCosmeticSwapper(this, this.stepTimeout);
    if (this.swapMode == CosmeticSwapper.SwapMode.AllAtOnce)
    {
      foreach (string cosmeticId in this.cosmeticIDs)
      {
        CosmeticSwapper.CosmeticState? nullable = this.SwapInCosmeticWithReturn(cosmeticId, rig);
        if (nullable.HasValue)
          rig.AddNewSwappedCosmetic(nullable.Value);
      }
    }
    else
    {
      int cosmeticStepIndex = rig.CosmeticStepIndex;
      if (cosmeticStepIndex < 0 || cosmeticStepIndex >= this.cosmeticIDs.Count)
        return;
      CosmeticSwapper.CosmeticState? nullable = this.SwapInCosmeticWithReturn(this.cosmeticIDs[cosmeticStepIndex], rig);
      if (!nullable.HasValue)
        return;
      rig.AddNewSwappedCosmetic(nullable.Value);
      if (cosmeticStepIndex == this.cosmeticIDs.Count - 1)
      {
        if (this.holdFinalStep)
          rig.MarkFinalCosmeticStep();
        if (this.OnSwappingSequenceCompleted == null)
          return;
        this.OnSwappingSequenceCompleted.Invoke(rig);
      }
      else
        rig.UnmarkFinalCosmeticStep();
    }
  }

  private CosmeticSwapper.CosmeticState? SwapInCosmeticWithReturn(string nameOrId, VRRig rig)
  {
    if ((Object) this.controller == (Object) null)
      return new CosmeticSwapper.CosmeticState?();
    CosmeticsController.CosmeticItem newItem = this.FindItem(nameOrId);
    if (newItem.isNullItem)
    {
      Debug.LogWarning((object) ("Cosmetic not found: " + nameOrId));
      return new CosmeticSwapper.CosmeticState?();
    }
    bool isLeftHand;
    CosmeticsController.CosmeticSlots cosmeticSlot = this.GetCosmeticSlot(newItem, out isLeftHand);
    if (cosmeticSlot == CosmeticsController.CosmeticSlots.Count)
    {
      Debug.LogWarning((object) ("Could not determine slot for: " + newItem.displayName));
      return new CosmeticSwapper.CosmeticState?();
    }
    CosmeticsController.CosmeticItem cosmeticItem = this.controller.currentWornSet.items[(int) cosmeticSlot];
    this.controller.ApplyCosmeticItemToSet(this.controller.tempUnlockedSet, newItem, isLeftHand, false);
    this.controller.UpdateWornCosmetics(true);
    return new CosmeticSwapper.CosmeticState?(new CosmeticSwapper.CosmeticState()
    {
      cosmeticId = nameOrId,
      replacedItem = cosmeticItem,
      slot = cosmeticSlot,
      isLeftHand = isLeftHand
    });
  }

  public void RestorePreviousCosmetic(CosmeticSwapper.CosmeticState state, VRRig rig)
  {
    if ((Object) this.controller == (Object) null)
      return;
    CosmeticsController.CosmeticItem cosmeticItem = this.FindItem(state.cosmeticId);
    if (cosmeticItem.isNullItem)
      return;
    this.controller.RemoveCosmeticItemFromSet(this.controller.tempUnlockedSet, cosmeticItem.displayName, false);
    if (!state.replacedItem.isNullItem)
      this.controller.ApplyCosmeticItemToSet(this.controller.tempUnlockedSet, state.replacedItem, state.isLeftHand, false);
    this.controller.UpdateWornCosmetics(true);
  }

  private CosmeticsController.CosmeticItem FindItem(string nameOrId)
  {
    CosmeticsController.CosmeticItem cosmeticItem;
    if (this.controller.allCosmeticsDict.TryGetValue(nameOrId, out cosmeticItem))
      return cosmeticItem;
    string itemID;
    return this.controller.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(nameOrId, out itemID) ? this.controller.GetItemFromDict(itemID) : this.controller.nullItem;
  }

  private CosmeticsController.CosmeticSlots GetCosmeticSlot(
    CosmeticsController.CosmeticItem item,
    out bool isLeftHand)
  {
    isLeftHand = false;
    if (!item.isHoldable)
      return CosmeticsController.CategoryToNonTransferrableSlot(item.itemCategory);
    CosmeticsController.CosmeticSet currentWornSet = this.controller.currentWornSet;
    CosmeticsController.CosmeticItem cosmeticItem1 = currentWornSet.items[7];
    CosmeticsController.CosmeticItem cosmeticItem2 = currentWornSet.items[8];
    if (cosmeticItem1.isNullItem || !cosmeticItem2.isNullItem && item.itemName == cosmeticItem1.itemName)
      isLeftHand = true;
    return !isLeftHand ? CosmeticsController.CosmeticSlots.HandRight : CosmeticsController.CosmeticSlots.HandLeft;
  }

  public enum SwapMode
  {
    AllAtOnce,
    StepByStep,
  }

  public struct CosmeticState
  {
    public string cosmeticId;
    public CosmeticsController.CosmeticItem replacedItem;
    public CosmeticsController.CosmeticSlots slot;
    public bool isLeftHand;
  }
}
