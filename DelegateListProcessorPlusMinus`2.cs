// Decompiled with JetBrains decompiler
// Type: GorillaTag.DelegateListProcessorPlusMinus`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace GorillaTag;

public abstract class DelegateListProcessorPlusMinus<T1, T2> : ListProcessorAbstract<T2>
  where T1 : DelegateListProcessorPlusMinus<T1, T2>, new()
  where T2 : Delegate
{
  protected DelegateListProcessorPlusMinus()
  {
  }

  protected DelegateListProcessorPlusMinus(int capacity)
    : base(capacity)
  {
  }

  public static T1 operator +(DelegateListProcessorPlusMinus<T1, T2> left, T2 right)
  {
    if (left == null)
      left = (DelegateListProcessorPlusMinus<T1, T2>) new T1();
    if ((object) right == null)
      return (T1) left;
    left.Add(in right);
    return (T1) left;
  }

  public static T1 operator -(DelegateListProcessorPlusMinus<T1, T2> left, T2 right)
  {
    if (left == null)
      return default (T1);
    if ((object) right == null)
      return (T1) left;
    left.Remove(in right);
    return (T1) left;
  }
}
