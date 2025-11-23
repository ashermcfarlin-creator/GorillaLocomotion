// Decompiled with JetBrains decompiler
// Type: GorillaTag.GTColor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaTag;

public static class GTColor
{
  public static Color RandomHSV(GTColor.HSVRanges ranges)
  {
    return Color.HSVToRGB(UnityEngine.Random.Range(ranges.h.x, ranges.h.y), UnityEngine.Random.Range(ranges.s.x, ranges.s.y), UnityEngine.Random.Range(ranges.v.x, ranges.v.y));
  }

  [Serializable]
  public struct HSVRanges(
    float hMin = 0.0f,
    float hMax = 1f,
    float sMin = 0.0f,
    float sMax = 1f,
    float vMin = 0.0f,
    float vMax = 1f)
  {
    public Vector2 h = new Vector2(hMin, hMax);
    public Vector2 s = new Vector2(sMin, sMax);
    public Vector2 v = new Vector2(vMin, vMax);
  }
}
