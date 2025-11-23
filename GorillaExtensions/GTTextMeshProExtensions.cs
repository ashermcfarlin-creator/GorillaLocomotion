// Decompiled with JetBrains decompiler
// Type: GorillaExtensions.GTTextMeshProExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Cysharp.Text;
using System;
using TMPro;

#nullable disable
namespace GorillaExtensions;

public static class GTTextMeshProExtensions
{
  public static void SetTextToZString(
    this TMP_Text textMono,
    Utf16ValueStringBuilder zStringBuilder)
  {
    ArraySegment<char> arraySegment = zStringBuilder.AsArraySegment();
    textMono.SetCharArray(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
  }
}
