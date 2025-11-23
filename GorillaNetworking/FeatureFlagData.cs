// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.FeatureFlagData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace GorillaNetworking;

[Serializable]
internal class FeatureFlagData
{
  public string name;
  public int value;
  public string valueType;
  public List<string> alwaysOnForUsers;
}
