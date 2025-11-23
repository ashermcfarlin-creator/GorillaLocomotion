// Decompiled with JetBrains decompiler
// Type: GorillaExtensions.ListExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaExtensions;

public static class ListExtensions
{
  public static TCol ShuffleIntoCollection<TCol, TVal>(this List<TVal> list) where TCol : ICollection<TVal>, new()
  {
    List<TVal> valList1 = new List<TVal>((IEnumerable<TVal>) list);
    TCol col = new TCol();
    int count = valList1.Count;
    while (count > 1)
    {
      --count;
      int index1 = Random.Range(0, count);
      List<TVal> valList2 = valList1;
      int index2 = count;
      List<TVal> valList3 = valList1;
      int index3 = index1;
      TVal val1 = valList1[index1];
      TVal val2 = valList1[count];
      TVal val3;
      valList2[index2] = val3 = val1;
      valList3[index3] = val3 = val2;
    }
    foreach (TVal val in valList1)
      col.Add(val);
    return col;
  }
}
