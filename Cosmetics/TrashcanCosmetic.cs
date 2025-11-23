// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.TrashcanCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class TrashcanCosmetic : MonoBehaviour
{
  public float minScoringDistance = 2f;
  public UnityEvent OnScored;

  public void OnBasket(bool isLeftHand, Collider other)
  {
    SlingshotProjectile component;
    if (!other.TryGetComponent<SlingshotProjectile>(out component) || (double) component.GetDistanceTraveled() < (double) this.minScoringDistance)
      return;
    this.OnScored?.Invoke();
    component.DestroyAfterRelease();
  }
}
