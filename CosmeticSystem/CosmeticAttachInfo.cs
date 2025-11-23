// Decompiled with JetBrains decompiler
// Type: GorillaTag.CosmeticSystem.CosmeticAttachInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.CosmeticSystem;

[Serializable]
public struct CosmeticAttachInfo(
  ECosmeticSelectSide selectSide,
  GTHardCodedBones.EBone parentBone,
  XformOffset offset)
{
  [Tooltip("(Not used for holdables) Determines if the cosmetic part be shown depending on the hand that is used to press the in-game wardrobe \"EQUIP\" button.\n- Both: Show no matter what hand is used.\n- Left: Only show if the left hand selected.\n- Right: Only show if the right hand selected.\n")]
  public StringEnum<ECosmeticSelectSide> selectSide = (StringEnum<ECosmeticSelectSide>) selectSide;
  public GTHardCodedBones.SturdyEBone parentBone = (GTHardCodedBones.SturdyEBone) parentBone;
  public XformOffset offset = offset;

  public static CosmeticAttachInfo Identity
  {
    get
    {
      return new CosmeticAttachInfo()
      {
        selectSide = (StringEnum<ECosmeticSelectSide>) ECosmeticSelectSide.Both,
        parentBone = (GTHardCodedBones.SturdyEBone) GTHardCodedBones.EBone.None,
        offset = XformOffset.Identity
      };
    }
  }
}
