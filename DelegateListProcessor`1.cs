// Decompiled with JetBrains decompiler
// Type: GorillaTag.DelegateListProcessor`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace GorillaTag;

public class DelegateListProcessor<T> : 
  DelegateListProcessorPlusMinus<DelegateListProcessor<T>, Action<T>>
{
  private T m_data;

  public DelegateListProcessor()
  {
  }

  public DelegateListProcessor(int capacity)
    : base(capacity)
  {
  }

  public void InvokeSafe(in T data)
  {
    this.m_data = data;
    this.ProcessListSafe();
    this.m_data = default (T);
  }

  public void Invoke(in T data)
  {
    this.m_data = data;
    this.ProcessList();
    this.m_data = default (T);
  }

  protected override void ProcessItem(in Action<T> item) => item(this.m_data);
}
