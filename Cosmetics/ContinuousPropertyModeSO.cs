// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ContinuousPropertyModeSO
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class ContinuousPropertyModeSO : ScriptableObject
{
  public ContinuousProperty.Type type;
  public ContinuousProperty.DataFlags flags;
  public ContinuousPropertyModeSO.CastData[] castData;
  [Space]
  public ContinuousPropertyModeSO.DescriptionStyle descriptionStyle;
  [TextArea]
  public string afterSentence;
  [TextArea]
  public string replaceDescription;

  private string GetTestDescription
  {
    get
    {
      return this.castData.Length == 0 ? "" : "Sample Description: " + this.GetDescriptionForCast(this.castData[0].target);
    }
  }

  public bool IsCastValid(ContinuousProperty.Cast cast)
  {
    for (int index = 0; index < this.castData.Length; ++index)
    {
      if (ContinuousProperty.CastMatches(this.castData[index].target, cast))
        return true;
    }
    return false;
  }

  public ContinuousProperty.Cast GetClosestCast(ContinuousProperty.Cast cast)
  {
    for (int index = 0; index < this.castData.Length; ++index)
    {
      if (ContinuousProperty.CastMatches(this.castData[index].target, cast))
        return this.castData[index].target;
    }
    return ContinuousProperty.Cast.Null;
  }

  public ContinuousProperty.DataFlags GetFlagsForCast(ContinuousProperty.Cast cast)
  {
    for (int index = 0; index < this.castData.Length; ++index)
    {
      if (this.castData[index].target == cast)
        return this.castData[index].additionalFlags | this.flags;
    }
    return this.flags;
  }

  public ContinuousProperty.DataFlags GetFlagsForClosestCast(ContinuousProperty.Cast cast)
  {
    for (int index = 0; index < this.castData.Length; ++index)
    {
      if (ContinuousProperty.CastMatches(this.castData[index].target, cast))
        return this.castData[index].additionalFlags | this.flags;
    }
    return this.flags;
  }

  public string GetDescriptionForCast(ContinuousProperty.Cast cast)
  {
    for (int index = 0; index < this.castData.Length; ++index)
    {
      if (ContinuousProperty.CastMatches(this.castData[index].target, cast) || this.castData.Length == 1)
      {
        if (!this.replaceDescription.IsNullOrEmpty())
          return this.replaceDescription;
        switch (this.descriptionStyle)
        {
          case ContinuousPropertyModeSO.DescriptionStyle.Continuous:
            return $"sets the {this.castData[index].whatItSets} on the {this.castData[index].target.ToString()} using the height of the curve at the provided time.{(" " + this.afterSentence).TrimEnd()}";
          case ContinuousPropertyModeSO.DescriptionStyle.SingleThreshold:
            return $"{this.castData[index].whatItSets} the {this.type.ToString()} when entering the 'true' part of the range.";
          case ContinuousPropertyModeSO.DescriptionStyle.DualThreshold:
            string[] strArray = this.castData[index].whatItSets.Split('|', StringSplitOptions.None);
            if (strArray.Length != 2)
              return $"Error! '{this.name}'s '{this.castData[index].target}.{"whatItSets"}' does not have two string separated by '|'.";
            return $"{strArray[0]} the {this.castData[index].target.ToString()} when entering the 'true' part of the range, {strArray[1]} the {this.castData[index].target.ToString()} when entering the 'false' part of the range.";
          default:
            continue;
        }
      }
    }
    return "Invalid target\n\n" + this.ListValidCasts();
  }

  public string ListValidCasts()
  {
    return "Valid targets: " + string.Join<ContinuousProperty.Cast>(", ", ((IEnumerable<ContinuousPropertyModeSO.CastData>) this.castData).Select<ContinuousPropertyModeSO.CastData, ContinuousProperty.Cast>((Func<ContinuousPropertyModeSO.CastData, ContinuousProperty.Cast>) (x => x.target)));
  }

  [Serializable]
  public struct CastData
  {
    public ContinuousProperty.Cast target;
    public ContinuousProperty.DataFlags additionalFlags;
    public string whatItSets;
  }

  public enum DescriptionStyle
  {
    Continuous,
    SingleThreshold,
    DualThreshold,
  }
}
