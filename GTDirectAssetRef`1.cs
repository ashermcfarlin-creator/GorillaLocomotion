// Decompiled with JetBrains decompiler
// Type: GorillaTag.GTDirectAssetRef`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag;

[Serializable]
public struct GTDirectAssetRef<T>(T theObj) : IEquatable<T> where T : UnityEngine.Object
{
  [SerializeField]
  [HideInInspector]
  internal T _obj = theObj;
  [FormerlySerializedAs("assetPath")]
  public string edAssetPath = (string) null;

  public T obj
  {
    get => this._obj;
    set
    {
      this._obj = value;
      this.edAssetPath = (string) null;
    }
  }

  public static implicit operator T(GTDirectAssetRef<T> refObject) => refObject.obj;

  public static implicit operator GTDirectAssetRef<T>(T other)
  {
    return new GTDirectAssetRef<T>() { obj = other };
  }

  public bool Equals(T other) => (UnityEngine.Object) this.obj == (UnityEngine.Object) other;

  public override bool Equals(object other) => other is T other1 && this.Equals(other1);

  public override int GetHashCode()
  {
    return !((UnityEngine.Object) this.obj != (UnityEngine.Object) null) ? 0 : this.obj.GetHashCode();
  }

  public static bool operator ==(GTDirectAssetRef<T> left, T right) => left.Equals(right);

  public static bool operator !=(GTDirectAssetRef<T> left, T right) => !(left == right);
}
