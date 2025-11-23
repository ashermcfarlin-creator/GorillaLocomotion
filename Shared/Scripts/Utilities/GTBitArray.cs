// Decompiled with JetBrains decompiler
// Type: GorillaTag.Shared.Scripts.Utilities.GTBitArray
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace GorillaTag.Shared.Scripts.Utilities;

public sealed class GTBitArray
{
  public readonly int Length;
  private readonly uint[] _data;

  public bool this[int idx]
  {
    get
    {
      if (idx < 0 || idx >= this.Length)
        throw new ArgumentOutOfRangeException();
      return ((ulong) this._data[idx / 32 /*0x20*/] & (ulong) (1 << idx % 32 /*0x20*/)) > 0UL;
    }
    set
    {
      if (idx < 0 || idx >= this.Length)
        throw new ArgumentOutOfRangeException();
      int index = idx / 32 /*0x20*/;
      int num = idx % 32 /*0x20*/;
      if (value)
        this._data[index] |= (uint) (1 << num);
      else
        this._data[index] &= (uint) ~(1 << num);
    }
  }

  public GTBitArray(int length)
  {
    this.Length = length;
    this._data = length % 32 /*0x20*/ == 0 ? new uint[length / 32 /*0x20*/] : new uint[length / 32 /*0x20*/ + 1];
    for (int index = 0; index < this._data.Length; ++index)
      this._data[index] = 0U;
  }

  public void Clear()
  {
    for (int index = 0; index < this._data.Length; ++index)
      this._data[index] = 0U;
  }

  public void CopyFrom(GTBitArray other)
  {
    if (this.Length != other.Length)
      throw new ArgumentException("Can only copy bit arrays of the same length.");
    for (int index = 0; index < this._data.Length; ++index)
      this._data[index] = other._data[index];
  }
}
