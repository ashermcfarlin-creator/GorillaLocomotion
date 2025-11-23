// Decompiled with JetBrains decompiler
// Type: GorillaTag.CosmeticSystem.CosmeticAnchorAntiIntersectOffsets
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag.CosmeticSystem;

[Serializable]
public struct CosmeticAnchorAntiIntersectOffsets
{
  public CosmeticAnchorAntiClipEntry nameTag;
  public CosmeticAnchorAntiClipEntry leftArm;
  public CosmeticAnchorAntiClipEntry rightArm;
  public CosmeticAnchorAntiClipEntry chest;
  public CosmeticAnchorAntiClipEntry huntComputer;
  public CosmeticAnchorAntiClipEntry badge;
  public CosmeticAnchorAntiClipEntry builderWatch;
  public CosmeticAnchorAntiClipEntry friendshipBraceletLeft;
  [FormerlySerializedAs("friendshipBradceletRight")]
  public CosmeticAnchorAntiClipEntry friendshipBraceletRight;
  public static readonly CosmeticAnchorAntiIntersectOffsets Identity = new CosmeticAnchorAntiIntersectOffsets()
  {
    nameTag = CosmeticAnchorAntiClipEntry.Identity,
    leftArm = CosmeticAnchorAntiClipEntry.Identity,
    rightArm = CosmeticAnchorAntiClipEntry.Identity,
    chest = CosmeticAnchorAntiClipEntry.Identity,
    huntComputer = CosmeticAnchorAntiClipEntry.Identity,
    badge = CosmeticAnchorAntiClipEntry.Identity,
    builderWatch = CosmeticAnchorAntiClipEntry.Identity
  };
}
