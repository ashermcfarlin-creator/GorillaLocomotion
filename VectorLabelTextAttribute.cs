// Decompiled with JetBrains decompiler
// Type: GorillaTag.VectorLabelTextAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Diagnostics;
using UnityEngine;

#nullable disable
namespace GorillaTag;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
[Conditional("UNITY_EDITOR")]
public class VectorLabelTextAttribute : PropertyAttribute
{
  public VectorLabelTextAttribute(params string[] labels)
    : this(-1, labels)
  {
  }

  public VectorLabelTextAttribute(int width, params string[] labels)
  {
  }
}
