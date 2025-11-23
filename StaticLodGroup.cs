// Decompiled with JetBrains decompiler
// Type: GorillaTag.StaticLodGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag;

[DefaultExecutionOrder(2000)]
public class StaticLodGroup : MonoBehaviour
{
  public const int k_monoDefaultExecutionOrder = 2000;
  private int index;
  public float collisionEnableDistance = 3f;
  public float uiFadeDistanceMax = 10f;

  protected void Awake() => this.index = StaticLodManager.Register(this);

  protected void OnEnable() => StaticLodManager.SetEnabled(this.index, true);

  protected void OnDisable() => StaticLodManager.SetEnabled(this.index, false);

  private void OnDestroy() => StaticLodManager.Unregister(this.index);
}
