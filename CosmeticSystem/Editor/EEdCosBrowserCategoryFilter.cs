// Decompiled with JetBrains decompiler
// Type: GorillaTag.CosmeticSystem.Editor.EEdCosBrowserCategoryFilter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace GorillaTag.CosmeticSystem.Editor;

[Flags]
public enum EEdCosBrowserCategoryFilter
{
  None = 0,
  Hat = 1,
  Badge = 2,
  Face = 4,
  Paw = 8,
  Chest = 16, // 0x00000010
  Fur = 32, // 0x00000020
  Shirt = 64, // 0x00000040
  Back = 128, // 0x00000080
  Arms = 256, // 0x00000100
  Pants = 512, // 0x00000200
  TagEffect = 1024, // 0x00000400
  Set = 4096, // 0x00001000
  All = Set | TagEffect | Pants | Arms | Back | Shirt | Fur | Chest | Paw | Face | Badge | Hat, // 0x000017FF
}
