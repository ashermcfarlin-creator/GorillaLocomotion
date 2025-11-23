// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.IProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public interface IProjectile
{
  void Launch(
    Vector3 startPosition,
    Quaternion startRotation,
    Vector3 velocity,
    float chargeFrac,
    VRRig ownerRig,
    int progressStep = -1);
}
