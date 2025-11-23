// Decompiled with JetBrains decompiler
// Type: GorillaTag.GTStripGameObjectFromBuildAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace GorillaTag;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class GTStripGameObjectFromBuildAttribute : Attribute
{
  public string Condition { get; }

  public GTStripGameObjectFromBuildAttribute(string condition = "") => this.Condition = condition;
}
