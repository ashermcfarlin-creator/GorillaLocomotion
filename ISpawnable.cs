// Decompiled with JetBrains decompiler
// Type: GorillaTag.ISpawnable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTag.CosmeticSystem;

#nullable disable
namespace GorillaTag;

public interface ISpawnable
{
  bool IsSpawned { get; set; }

  ECosmeticSelectSide CosmeticSelectedSide { get; set; }

  void OnSpawn(VRRig rig);

  void OnDespawn();
}
