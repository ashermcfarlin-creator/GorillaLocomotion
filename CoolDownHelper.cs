// Decompiled with JetBrains decompiler
// Type: GorillaTag.CoolDownHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable disable
namespace GorillaTag;

[Serializable]
public class CoolDownHelper
{
  public float coolDown;
  [NonSerialized]
  public float checkTime;

  public CoolDownHelper()
  {
    this.coolDown = 1f;
    this.checkTime = 0.0f;
  }

  public CoolDownHelper(float cd)
  {
    this.coolDown = cd;
    this.checkTime = 0.0f;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public bool CheckCooldown()
  {
    float unscaledTime = Time.unscaledTime;
    if ((double) unscaledTime < (double) this.checkTime)
      return false;
    this.OnCheckPass();
    this.checkTime = unscaledTime + this.coolDown;
    return true;
  }

  public virtual void Start() => this.checkTime = Time.unscaledTime + this.coolDown;

  public virtual void Stop() => this.checkTime = float.MaxValue;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public virtual void OnCheckPass()
  {
  }
}
