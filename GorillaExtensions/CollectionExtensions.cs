// Decompiled with JetBrains decompiler
// Type: GorillaExtensions.CollectionExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

#nullable disable
namespace GorillaExtensions;

public static class CollectionExtensions
{
  public static void AddAll<T>(this ICollection<T> collection, IEnumerable<T> ts)
  {
    foreach (T t in ts)
      collection.Add(t);
  }
}
