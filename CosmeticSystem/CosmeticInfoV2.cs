// Decompiled with JetBrains decompiler
// Type: GorillaTag.CosmeticSystem.CosmeticInfoV2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaNetworking;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.CosmeticSystem;

[Serializable]
public struct CosmeticInfoV2(string displayName) : ISerializationCallbackReceiver
{
  public bool enabled = true;
  [Tooltip("// TODO: (2024-09-27 MattO) season will determine what addressables bundle it will be in and wheter it should be active based on release time of season.\n\nThe assigned season will determine what folder the Cosmetic will go in and how it will be listed in the Cosmetic Browser.")]
  [Delayed]
  public SeasonSO season = (SeasonSO) null;
  [Tooltip("Name that is displayed in the store during purchasing.")]
  [Delayed]
  public string displayName = displayName;
  [Tooltip("ID used on the PlayFab servers that must be unique. If this does not exist on the playfab servers then an error will be thrown. In notion search for \"Cosmetics - Adding a PlayFab ID\".")]
  [Delayed]
  public string playFabID = "";
  public Sprite icon = (Sprite) null;
  [Tooltip("Category determines which category button in the user's wardrobe (which are the two rows of buttons with equivalent names) have to be pressed to access the cosmetic along with others in the same category.")]
  public StringEnum<CosmeticsController.CosmeticCategory> category = (StringEnum<CosmeticsController.CosmeticCategory>) CosmeticsController.CosmeticCategory.None;
  [Obsolete("(2024-08-13 MattO) Will be removed after holdables array is fully implemented. Check length of `holdableParts` instead.")]
  [HideInInspector]
  public bool isHoldable = false;
  public bool isThrowable = false;
  public bool usesBothHandSlots = false;
  public bool hideWardrobeMannequin = false;
  public const string holdableParts_infoBoxShortMsg = "\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).";
  public const string holdableParts_infoBoxDetailedMsg = "\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).\n\nHoldables are prefabs that have Holdable components. The prefab asset's transform will be moved between the listed \n attach points on \"Gorilla Player Networked.prefab\" when grabbed by the player \n";
  [Space]
  [Tooltip("\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).\n\nHoldables are prefabs that have Holdable components. The prefab asset's transform will be moved between the listed \n attach points on \"Gorilla Player Networked.prefab\" when grabbed by the player \n")]
  public CosmeticPart[] holdableParts = new CosmeticPart[0];
  public const string functionalParts_infoBoxShortMsg = "\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.";
  public const string functionalParts_infoBoxDetailedMsg = "\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.\n\nThese individual parts which also handle the core functionality of the cosmetic. In most cases there will only be one part, there can be multiple parts for cases like rings which might be on both left and right hands.\n\nThese parts will be parented to the bones of  \"Gorilla Player Networked.prefab\" instances which includes the VRRig component.\n\nIf a \"First Person View\" part or \"Local Rig Part\" is set it will be enabled instead of the wearable parts for the local player";
  [Space]
  [Tooltip("\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.\n\nThese individual parts which also handle the core functionality of the cosmetic. In most cases there will only be one part, there can be multiple parts for cases like rings which might be on both left and right hands.\n\nThese parts will be parented to the bones of  \"Gorilla Player Networked.prefab\" instances which includes the VRRig component.\n\nIf a \"First Person View\" part or \"Local Rig Part\" is set it will be enabled instead of the wearable parts for the local player")]
  public CosmeticPart[] functionalParts = new CosmeticPart[0];
  public const string wardrobeParts_infoBoxShortMsg = "\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.";
  public const string wardrobeParts_infoBoxDetailedMsg = "\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\nThese prefabs are shown on the satellite wardrobe, and in the store (if \"Store Parts\" is left empty)";
  [Space]
  [Tooltip("\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\nThese prefabs are shown on the satellite wardrobe, and in the store (if \"Store Parts\" is left empty)")]
  public CosmeticPart[] wardrobeParts = new CosmeticPart[0];
  public const string storeParts_infoBoxShortMsg = "\"Store Parts\" are spawned into the Dynamic Cosmetic Stands in city.";
  public const string storeParts_infoBoxDetailedMsg = "\"Store Parts\" are spawned into the Dynamic Cosmetic Stands in city.\nStore parts only need to be specified if the store display should be different than the wardrobe display";
  [Space]
  [Tooltip("\"Store Parts\" are spawned into the Dynamic Cosmetic Stands in city.\nStore parts only need to be specified if the store display should be different than the wardrobe display")]
  public CosmeticPart[] storeParts = new CosmeticPart[0];
  public const string firstPersonViewParts_infoBoxShortMsg = "\"First Person View Parts\" will be attached to the local monke's camera.\nFirst person parts are enabled instead of \"Wearable Parts\" for the local player";
  public const string firstPersonViewParts_infoBoxDetailedMsg = "\"First Person View Parts\" will be attached to the local monke's camera.\nFirst person parts are enabled instead of \"Wearable Parts\" for the local player\nThese are used for any peripheral view meshes on the No Mirror layer, usually on HAT or FACE items";
  [Space]
  [Tooltip("\"First Person View Parts\" will be attached to the local monke's camera.\nFirst person parts are enabled instead of \"Wearable Parts\" for the local player\nThese are used for any peripheral view meshes on the No Mirror layer, usually on HAT or FACE items")]
  public CosmeticPart[] firstPersonViewParts = new CosmeticPart[0];
  public const string localRigParts_infoBoxShortMsg = "\"Local Mirror Parts\" will be attached to the local player's rig instead of \"Wearable Parts\".";
  public const string localRigParts_infoBoxDetailedMsg = "\"Local Mirror Parts\" will be attached to the local player's rig instead of \"Wearable Parts\".\nThese objects can be used in addition to first person view parts.\nThese can be used for mirror view meshes (usually HAT or FACE items)\nAny item with GTPosRotConstraints should be parented to the rig and not the camera";
  [Space]
  [Tooltip("\"Local Mirror Parts\" will be attached to the local player's rig instead of \"Wearable Parts\".\nThese objects can be used in addition to first person view parts.\nThese can be used for mirror view meshes (usually HAT or FACE items)\nAny item with GTPosRotConstraints should be parented to the rig and not the camera")]
  public CosmeticPart[] localRigParts = new CosmeticPart[0];
  [Space]
  [Tooltip("When this cosmetic is equipped, these offsets will be applied to the other objects on the player that are likely to clip\nSHIRT items ususally offset the badge, nametag, and chest items\n PAW items usually offset the hunt computer and builder watch")]
  public CosmeticAnchorAntiIntersectOffsets anchorAntiIntersectOffsets = new CosmeticAnchorAntiIntersectOffsets();
  [Space]
  [Tooltip("TODO COMMENT")]
  public CosmeticSO[] setCosmetics = new CosmeticSO[0];
  [NonSerialized]
  public string debugCosmeticSOName = "__UNINITIALIZED__";

  public bool hasHoldableParts
  {
    get
    {
      CosmeticPart[] holdableParts = this.holdableParts;
      return holdableParts != null && holdableParts.Length > 0;
    }
  }

  public bool hasWardrobeParts
  {
    get
    {
      CosmeticPart[] wardrobeParts = this.wardrobeParts;
      return wardrobeParts != null && wardrobeParts.Length > 0;
    }
  }

  public bool hasStoreParts
  {
    get
    {
      CosmeticPart[] storeParts = this.storeParts;
      return storeParts != null && storeParts.Length > 0;
    }
  }

  public bool hasFunctionalParts
  {
    get
    {
      CosmeticPart[] functionalParts = this.functionalParts;
      return functionalParts != null && functionalParts.Length > 0;
    }
  }

  public bool hasFirstPersonViewParts
  {
    get
    {
      CosmeticPart[] firstPersonViewParts = this.firstPersonViewParts;
      return firstPersonViewParts != null && firstPersonViewParts.Length > 0;
    }
  }

  public bool hasLocalRigParts
  {
    get
    {
      CosmeticPart[] localRigParts = this.localRigParts;
      return localRigParts != null && localRigParts.Length > 0;
    }
  }

  void ISerializationCallbackReceiver.OnBeforeSerialize()
  {
  }

  void ISerializationCallbackReceiver.OnAfterDeserialize()
  {
    this._OnAfterDeserialize_InitializePartsArray(ref this.holdableParts, ECosmeticPartType.Holdable);
    this._OnAfterDeserialize_InitializePartsArray(ref this.functionalParts, ECosmeticPartType.Functional);
    this._OnAfterDeserialize_InitializePartsArray(ref this.wardrobeParts, ECosmeticPartType.Wardrobe);
    this._OnAfterDeserialize_InitializePartsArray(ref this.storeParts, ECosmeticPartType.Store);
    this._OnAfterDeserialize_InitializePartsArray(ref this.firstPersonViewParts, ECosmeticPartType.FirstPerson);
    this._OnAfterDeserialize_InitializePartsArray(ref this.localRigParts, ECosmeticPartType.LocalRig);
    if (this.setCosmetics != null)
      return;
    this.setCosmetics = Array.Empty<CosmeticSO>();
  }

  private void _OnAfterDeserialize_InitializePartsArray(
    ref CosmeticPart[] parts,
    ECosmeticPartType partType)
  {
    for (int index = 0; index < parts.Length; ++index)
    {
      parts[index].partType = partType;
      ref CosmeticAttachInfo[] local = ref parts[index].attachAnchors;
      if (local == null)
        local = Array.Empty<CosmeticAttachInfo>();
    }
  }
}
