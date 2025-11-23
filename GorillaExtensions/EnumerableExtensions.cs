// Decompiled with JetBrains decompiler
// Type: GorillaExtensions.EnumerableExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace GorillaExtensions;

public static class EnumerableExtensions
{
  public static TValue MinBy<TValue, TKey>(
    this IEnumerable<TValue> ts,
    Func<TValue, TKey> keyGetter)
    where TKey : struct, IComparable<TKey>
  {
    TValue obj = default (TValue);
    TKey? nullable = new TKey?();
    foreach (TValue t in ts)
    {
      TKey key = keyGetter(t);
      if (!nullable.HasValue || key.CompareTo(nullable.Value) < 0)
      {
        obj = t;
        nullable = new TKey?(key);
      }
    }
    if (!nullable.HasValue)
      throw new ArgumentException("Cannot calculate MinBy on an empty IEnumerable.");
    return obj;
  }

  public static IEnumerable<T> Peek<T>(this IEnumerable<T> ts, Action<T> action)
  {
    foreach (T t in ts)
    {
      action(t);
      yield return t;
    }
  }
}
