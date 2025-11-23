// Decompiled with JetBrains decompiler
// Type: GorillaTag.ListProcessorAbstract`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace GorillaTag;

public abstract class ListProcessorAbstract<T> : ListProcessor<T>
{
  protected ListProcessorAbstract()
  {
    this.m_itemProcessorDelegate = new InAction<T>(this.ProcessItem);
  }

  protected ListProcessorAbstract(int capacity)
    : base(capacity)
  {
    this.m_itemProcessorDelegate = new InAction<T>(this.ProcessItem);
  }

  protected abstract void ProcessItem(in T item);
}
