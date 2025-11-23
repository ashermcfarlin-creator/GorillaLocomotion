// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.CosmeticItemInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTag;
using GorillaTag.CosmeticSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaNetworking;

public class CosmeticItemInstance
{
  public List<GameObject> leftObjects = new List<GameObject>();
  public List<GameObject> rightObjects = new List<GameObject>();
  public List<GameObject> objects = new List<GameObject>();
  public List<GameObject> holdableObjects = new List<GameObject>();
  public CosmeticAnchorAntiIntersectOffsets clippingOffsets;
  public bool isHoldableItem;
  public string dbgname;
  private BodyDockPositions _bodyDockPositions;
  private VRRigAnchorOverrides _anchorOverrides;
  private CosmeticsController.CosmeticSlots activeSlot;

  private void EnableItem(GameObject obj, bool enable)
  {
    try
    {
      obj.SetActive(enable);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) $"Exception while enabling cosmetic: {ex}");
    }
  }

  private void ApplyClippingOffsets(bool itemEnabled)
  {
    if ((UnityEngine.Object) this._bodyDockPositions == (UnityEngine.Object) null || !((UnityEngine.Object) this._anchorOverrides != (UnityEngine.Object) null))
      return;
    if (this.clippingOffsets.nameTag.enabled)
      this._anchorOverrides.UpdateNameTagOffset(itemEnabled ? this.clippingOffsets.nameTag.offset : XformOffset.Identity, itemEnabled, this.activeSlot);
    if (this.clippingOffsets.leftArm.enabled)
      this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnLeftArm, this.clippingOffsets.leftArm.offset, itemEnabled, this._bodyDockPositions.leftArmTransform);
    if (this.clippingOffsets.rightArm.enabled)
      this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnRightArm, this.clippingOffsets.rightArm.offset, itemEnabled, this._bodyDockPositions.rightArmTransform);
    if (this.clippingOffsets.chest.enabled)
      this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnChest, this.clippingOffsets.chest.offset, itemEnabled, this._anchorOverrides.chestDefaultTransform);
    if (this.clippingOffsets.huntComputer.enabled)
      this._anchorOverrides.UpdateHuntWatchOffset(this.clippingOffsets.huntComputer.offset, itemEnabled);
    if (this.clippingOffsets.badge.enabled)
      this._anchorOverrides.UpdateBadgeOffset(itemEnabled ? this.clippingOffsets.badge.offset : XformOffset.Identity, itemEnabled, this.activeSlot);
    if (this.clippingOffsets.builderWatch.enabled)
      this._anchorOverrides.UpdateBuilderWatchOffset(this.clippingOffsets.builderWatch.offset, itemEnabled);
    if (this.clippingOffsets.friendshipBraceletLeft.enabled)
      this._anchorOverrides.UpdateFriendshipBraceletOffset(this.clippingOffsets.friendshipBraceletLeft.offset, true, itemEnabled);
    if (!this.clippingOffsets.friendshipBraceletRight.enabled)
      return;
    this._anchorOverrides.UpdateFriendshipBraceletOffset(this.clippingOffsets.friendshipBraceletRight.offset, false, itemEnabled);
  }

  public void DisableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
  {
    bool flag1 = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
    bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
    foreach (GameObject gameObject in this.objects)
      this.EnableItem(gameObject, false);
    if (flag1)
    {
      foreach (GameObject leftObject in this.leftObjects)
        this.EnableItem(leftObject, false);
    }
    if (flag2)
    {
      foreach (GameObject rightObject in this.rightObjects)
        this.EnableItem(rightObject, false);
    }
    this.ApplyClippingOffsets(false);
  }

  public void EnableItem(CosmeticsController.CosmeticSlots cosmeticSlot, VRRig rig)
  {
    bool flag1 = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
    bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
    this.activeSlot = cosmeticSlot;
    if ((UnityEngine.Object) rig != (UnityEngine.Object) null && (UnityEngine.Object) this._anchorOverrides == (UnityEngine.Object) null)
    {
      this._anchorOverrides = rig.gameObject.GetComponent<VRRigAnchorOverrides>();
      this._bodyDockPositions = rig.GetComponent<BodyDockPositions>();
    }
    foreach (GameObject gameObject in this.objects)
    {
      this.EnableItem(gameObject, true);
      if (cosmeticSlot == CosmeticsController.CosmeticSlots.Badge)
      {
        if (this.objects.Count > 1)
        {
          GTHardCodedBones.EBone eBone;
          if (GTHardCodedBones.TryGetFirstBoneInParents(gameObject.transform, out eBone, out Transform _) && eBone == GTHardCodedBones.EBone.body)
            this._anchorOverrides.CurrentBadgeTransform = gameObject.transform;
        }
        else
          this._anchorOverrides.CurrentBadgeTransform = gameObject.transform;
      }
    }
    if (flag1)
    {
      foreach (GameObject leftObject in this.leftObjects)
        this.EnableItem(leftObject, true);
    }
    if (flag2)
    {
      foreach (GameObject rightObject in this.rightObjects)
        this.EnableItem(rightObject, true);
    }
    this.ApplyClippingOffsets(true);
  }
}
