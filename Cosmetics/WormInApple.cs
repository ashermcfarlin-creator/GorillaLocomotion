// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.WormInApple
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class WormInApple : MonoBehaviour
{
  [SerializeField]
  private UpdateBlendShapeCosmetic blendShapeCosmetic;
  public UnityEvent OnHandTapped;

  public void OnHandTap()
  {
    if (!(bool) (Object) this.blendShapeCosmetic || (double) this.blendShapeCosmetic.GetBlendValue() <= 0.5)
      return;
    this.OnHandTapped?.Invoke();
  }
}
