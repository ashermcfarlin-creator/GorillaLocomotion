// Decompiled with JetBrains decompiler
// Type: GorillaTag.DelegateListProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace GorillaTag;

public class DelegateListProcessor : DelegateListProcessorPlusMinus<DelegateListProcessor, Action>
{
  public DelegateListProcessor()
  {
  }

  public DelegateListProcessor(int capacity)
    : base(capacity)
  {
  }

  public void Invoke() => this.ProcessList();

  public void InvokeSafe() => this.ProcessListSafe();

  protected override void ProcessItem(in Action del) => del();
}
