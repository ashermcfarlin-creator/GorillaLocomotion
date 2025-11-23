// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.PickupableVariant
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class PickupableVariant : MonoBehaviour
{
  protected internal virtual void Release(
    HoldableObject holdable,
    Vector3 startPosition,
    Vector3 releaseVelocity,
    float playerScale)
  {
  }

  protected internal virtual void Pickup(bool isAutoPickup = false)
  {
  }

  protected internal virtual void DelayedPickup()
  {
  }
}
