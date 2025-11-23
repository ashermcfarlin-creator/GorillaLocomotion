// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.ExtensionMethods
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaNetworking;

public static class ExtensionMethods
{
  public static void SafeInvoke<T>(this Action<T> action, T data)
  {
    try
    {
      if (action == null)
        return;
      action(data);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) $"[PlayFabTitleDataCache::SafeInvoke] Failure invoking action: {ex}");
    }
  }

  public static void AddOrUpdate<TKey, TValue>(
    this Dictionary<TKey, TValue> dict,
    TKey key,
    TValue value)
  {
    if (dict.ContainsKey(key))
      dict[key] = value;
    else
      dict.Add(key, value);
  }
}
