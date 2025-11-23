// Decompiled with JetBrains decompiler
// Type: GorillaTag.TickSystemTimer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace GorillaTag;

[Serializable]
internal class TickSystemTimer : TickSystemTimerAbstract
{
  public Action callback;

  public TickSystemTimer()
  {
  }

  public TickSystemTimer(float cd)
    : base(cd)
  {
  }

  public TickSystemTimer(float cd, Action cb)
    : base(cd)
  {
    this.callback = cb;
  }

  public TickSystemTimer(Action cb) => this.callback = cb;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public override void OnTimedEvent()
  {
    Action callback = this.callback;
    if (callback == null)
      return;
    callback();
  }
}
