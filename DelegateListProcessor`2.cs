// Decompiled with JetBrains decompiler
// Type: GorillaTag.DelegateListProcessor`2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace GorillaTag;

public class DelegateListProcessor<T1, T2> : 
  DelegateListProcessorPlusMinus<DelegateListProcessor<T1, T2>, Action<T1, T2>>
{
  private T1 m_data1;
  private T2 m_data2;

  public DelegateListProcessor()
  {
  }

  public DelegateListProcessor(int capacity)
    : base(capacity)
  {
  }

  public void InvokeSafe(in T1 data1, in T2 data2)
  {
    this.SetData(in data1, in data2);
    this.ProcessListSafe();
    this.ResetData();
  }

  public void Invoke(in T1 data1, in T2 data2)
  {
    this.SetData(in data1, in data2);
    this.ProcessList();
    this.ResetData();
  }

  protected override void ProcessItem(in Action<T1, T2> item) => item(this.m_data1, this.m_data2);

  private void SetData(in T1 data1, in T2 data2)
  {
    this.m_data1 = data1;
    this.m_data2 = data2;
  }

  private void ResetData()
  {
    this.m_data1 = default (T1);
    this.m_data2 = default (T2);
  }
}
