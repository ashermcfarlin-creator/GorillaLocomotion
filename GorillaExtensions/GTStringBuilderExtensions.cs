// Decompiled with JetBrains decompiler
// Type: GorillaExtensions.GTStringBuilderExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Cysharp.Text;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable disable
namespace GorillaExtensions;

public static class GTStringBuilderExtensions
{
  public static IEnumerable<ReadOnlyMemory<char>> GetSegmentsOfMem(
    this Utf16ValueStringBuilder sb,
    int maxCharsPerSegment = 16300)
  {
    int num1 = 0;
    List<ReadOnlyMemory<char>> segmentsOfMem = new List<ReadOnlyMemory<char>>(64 /*0x40*/);
    int num2;
    for (ReadOnlyMemory<char> readOnlyMemory = sb.AsMemory(); num1 < readOnlyMemory.Length; num1 = num2 + 1)
    {
      num2 = Mathf.Min(num1 + maxCharsPerSegment, readOnlyMemory.Length);
      if (num2 < readOnlyMemory.Length)
      {
        int num3 = -1;
        for (int index = num2 - 1; index >= num1; --index)
        {
          if (readOnlyMemory.Span[index] == '\n')
          {
            num3 = index;
            break;
          }
        }
        if (num3 != -1)
          num2 = num3;
      }
      segmentsOfMem.Add(readOnlyMemory.Slice(num1, num2 - num1));
    }
    return (IEnumerable<ReadOnlyMemory<char>>) segmentsOfMem;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void GTAddPath(
    this Utf16ValueStringBuilder stringBuilderToAddTo,
    GameObject gameObject)
  {
    gameObject.transform.GetPathQ(ref stringBuilderToAddTo);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void GTAddPath(
    this Utf16ValueStringBuilder stringBuilderToAddTo,
    Transform transform)
  {
    transform.GetPathQ(ref stringBuilderToAddTo);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void Q(this Utf16ValueStringBuilder sb, string value)
  {
    sb.Append('"');
    sb.Append(value);
    sb.Append('"');
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b)
  {
    sb.Append(a);
    sb.Append(b);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c)
  {
    sb.Append(a);
    sb.Append(b);
    sb.Append(c);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void GTMany(
    this Utf16ValueStringBuilder sb,
    string a,
    string b,
    string c,
    string d)
  {
    sb.Append(a);
    sb.Append(b);
    sb.Append(c);
    sb.Append(d);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void GTMany(
    this Utf16ValueStringBuilder sb,
    string a,
    string b,
    string c,
    string d,
    string e)
  {
    sb.Append(a);
    sb.Append(b);
    sb.Append(c);
    sb.Append(d);
    sb.Append(e);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void GTMany(
    this Utf16ValueStringBuilder sb,
    string a,
    string b,
    string c,
    string d,
    string e,
    string f)
  {
    sb.Append(a);
    sb.Append(b);
    sb.Append(c);
    sb.Append(d);
    sb.Append(e);
    sb.Append(f);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void GTMany(
    this Utf16ValueStringBuilder sb,
    string a,
    string b,
    string c,
    string d,
    string e,
    string f,
    string g)
  {
    sb.Append(a);
    sb.Append(b);
    sb.Append(c);
    sb.Append(d);
    sb.Append(e);
    sb.Append(f);
    sb.Append(g);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void GTMany(
    this Utf16ValueStringBuilder sb,
    string a,
    string b,
    string c,
    string d,
    string e,
    string f,
    string g,
    string h)
  {
    sb.Append(a);
    sb.Append(b);
    sb.Append(c);
    sb.Append(d);
    sb.Append(e);
    sb.Append(f);
    sb.Append(g);
    sb.Append(h);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void GTMany(
    this Utf16ValueStringBuilder sb,
    string a,
    string b,
    string c,
    string d,
    string e,
    string f,
    string g,
    string h,
    string i)
  {
    sb.Append(a);
    sb.Append(b);
    sb.Append(c);
    sb.Append(d);
    sb.Append(e);
    sb.Append(f);
    sb.Append(g);
    sb.Append(h);
    sb.Append(i);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void GTMany(
    this Utf16ValueStringBuilder sb,
    string a,
    string b,
    string c,
    string d,
    string e,
    string f,
    string g,
    string h,
    string i,
    string j)
  {
    sb.Append(a);
    sb.Append(b);
    sb.Append(c);
    sb.Append(d);
    sb.Append(e);
    sb.Append(f);
    sb.Append(g);
    sb.Append(h);
    sb.Append(i);
    sb.Append(j);
  }
}
