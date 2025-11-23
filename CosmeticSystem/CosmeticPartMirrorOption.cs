// Decompiled with JetBrains decompiler
// Type: GorillaTag.CosmeticSystem.CosmeticPartMirrorOption
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.CosmeticSystem;

[Serializable]
public struct CosmeticPartMirrorOption
{
  public ECosmeticPartMirrorAxis axis;
  [Tooltip("This will multiply the local scale for the selected axis by -1.")]
  public bool negativeScale;
}
