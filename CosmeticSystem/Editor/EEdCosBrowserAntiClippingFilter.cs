// Decompiled with JetBrains decompiler
// Type: GorillaTag.CosmeticSystem.Editor.EEdCosBrowserAntiClippingFilter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace GorillaTag.CosmeticSystem.Editor;

[Flags]
public enum EEdCosBrowserAntiClippingFilter
{
  None = 0,
  NameTag = 1,
  LeftArm = 2,
  RightArm = 4,
  Chest = 8,
  HuntComputer = 16, // 0x00000010
  Badge = 32, // 0x00000020
  BuilderWatch = 64, // 0x00000040
  FriendshipBraceletLeft = 128, // 0x00000080
  FriendshipBraceletRight = 256, // 0x00000100
  All = FriendshipBraceletRight | FriendshipBraceletLeft | BuilderWatch | Badge | HuntComputer | Chest | RightArm | LeftArm | NameTag, // 0x000001FF
}
