// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.SmoothScaleModifierCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class SmoothScaleModifierCosmetic : MonoBehaviour
{
  [Tooltip("The GameObject to scale up or down. This should reference the cosmetic mesh or object you want to visually modify.")]
  [SerializeField]
  private GameObject objectPrefab;
  [Tooltip("The target scale applied when scaling is triggered.")]
  [SerializeField]
  private Vector3 targetScale = new Vector3(2f, 2f, 2f);
  [Tooltip("Speed at which the object scales toward its target or initial size")]
  [SerializeField]
  private float speed = 2f;
  [Tooltip("Invoked once when the object reaches the target scale.")]
  public UnityEvent onScaled;
  [Tooltip("Invoked once when the object returns to its initial scale.")]
  public UnityEvent onReset;
  private SmoothScaleModifierCosmetic.State currentState;
  private Vector3 initialScale;

  private void Awake() => this.initialScale = this.objectPrefab.transform.localScale;

  private void OnEnable() => this.UpdateState(SmoothScaleModifierCosmetic.State.Reset);

  private void Update()
  {
    switch (this.currentState)
    {
      case SmoothScaleModifierCosmetic.State.Reset:
        this.SmoothScale(this.objectPrefab.transform.localScale, this.initialScale);
        if ((double) Vector3.Distance(this.objectPrefab.transform.localScale, this.initialScale) >= 0.0099999997764825821)
          break;
        this.objectPrefab.transform.localScale = this.initialScale;
        if (this.onReset != null)
          this.onReset.Invoke();
        this.UpdateState(SmoothScaleModifierCosmetic.State.None);
        break;
      case SmoothScaleModifierCosmetic.State.Scaling:
        this.SmoothScale(this.objectPrefab.transform.localScale, this.targetScale);
        if ((double) Vector3.Distance(this.objectPrefab.transform.localScale, this.targetScale) >= 0.0099999997764825821)
          break;
        this.objectPrefab.transform.localScale = this.targetScale;
        if (this.onScaled != null)
          this.onScaled.Invoke();
        this.UpdateState(SmoothScaleModifierCosmetic.State.Scaled);
        break;
    }
  }

  private void SmoothScale(Vector3 initial, Vector3 target)
  {
    this.objectPrefab.transform.localScale = Vector3.MoveTowards(initial, target, this.speed * Time.deltaTime);
  }

  private void UpdateState(SmoothScaleModifierCosmetic.State newState)
  {
    this.currentState = newState;
  }

  public void TriggerScale()
  {
    if (this.currentState == SmoothScaleModifierCosmetic.State.Scaled)
      return;
    this.UpdateState(SmoothScaleModifierCosmetic.State.Scaling);
  }

  public void TriggerReset()
  {
    if (this.currentState == SmoothScaleModifierCosmetic.State.Reset)
      return;
    this.UpdateState(SmoothScaleModifierCosmetic.State.Reset);
  }

  private enum State
  {
    None,
    Reset,
    Scaling,
    Scaled,
  }
}
