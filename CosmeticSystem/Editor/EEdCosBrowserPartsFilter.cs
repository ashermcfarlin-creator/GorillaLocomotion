// Decompiled with JetBrains decompiler
// Type: GorillaTag.CosmeticSystem.Editor.EEdCosBrowserPartsFilter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace GorillaTag.CosmeticSystem.Editor;

[Flags]
public enum EEdCosBrowserPartsFilter
{
  None = 0,
  NoParts = 1,
  Holdable = 2,
  Functional = 4,
  Wardrobe = 8,
  Store = 16, // 0x00000010
  FirstPerson = 32, // 0x00000020
  LocalRig = 64, // 0x00000040
  All = LocalRig | FirstPerson | Store | Wardrobe | Functional | Holdable | NoParts, // 0x0000007F
}
