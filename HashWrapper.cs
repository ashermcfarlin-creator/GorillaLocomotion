// Decompiled with JetBrains decompiler
// Type: GorillaTag.HashWrapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaTag;

[Serializable]
public struct HashWrapper(int hash = -1) : IEquatable<int>
{
  [SerializeField]
  private int hashCode = hash;
  public const int NULL_HASH = -1;

  public override int GetHashCode() => this.hashCode;

  public override bool Equals(object obj) => this.hashCode.Equals(obj);

  public bool Equals(int i) => this.hashCode.Equals(i);

  public static implicit operator int(in HashWrapper hash) => hash.hashCode;
}
