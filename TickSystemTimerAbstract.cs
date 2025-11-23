// Decompiled with JetBrains decompiler
// Type: GorillaTag.TickSystemTimerAbstract
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.CompilerServices;

#nullable disable
namespace GorillaTag;

[Serializable]
internal abstract class TickSystemTimerAbstract : CoolDownHelper, ITickSystemPre
{
  [NonSerialized]
  internal bool registered;

  bool ITickSystemPre.PreTickRunning
  {
    get => this.registered;
    set => this.registered = value;
  }

  public bool Running => this.registered;

  protected TickSystemTimerAbstract()
  {
  }

  protected TickSystemTimerAbstract(float cd)
    : base(cd)
  {
  }

  public override void Start()
  {
    base.Start();
    TickSystem<object>.AddPreTickCallback((ITickSystemPre) this);
  }

  public override void Stop()
  {
    base.Stop();
    TickSystem<object>.RemovePreTickCallback((ITickSystemPre) this);
  }

  public override void OnCheckPass() => this.OnTimedEvent();

  public abstract void OnTimedEvent();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  void ITickSystemPre.PreTick() => this.CheckCooldown();
}
