// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.AOESender
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class AOESender : MonoBehaviour
{
  [Min(0.0f)]
  [SerializeField]
  private float radius = 3f;
  [SerializeField]
  private LayerMask layerMask = (LayerMask) -1;
  [SerializeField]
  private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;
  [Tooltip("If empty, all AOEReceiver targets pass. If not empty, only receivers with these tags pass.")]
  [SerializeField]
  private string[] includeTags;
  [SerializeField]
  private AOESender.FalloffMode falloffMode = AOESender.FalloffMode.Linear;
  [SerializeField]
  private AnimationCurve falloffCurve = AnimationCurve.Linear(0.0f, 1f, 1f, 0.0f);
  [Tooltip("Base strength before distance falloff.")]
  [SerializeField]
  private float strength = 1f;
  [Tooltip("Optional after falloff, applied as: max(minStrength, base*falloff).")]
  [SerializeField]
  private float minStrength;
  [SerializeField]
  private bool applyOnEnable;
  [Min(0.0f)]
  [SerializeField]
  private float repeatInterval;
  [SerializeField]
  [Tooltip("Max colliders captured per trigger/apply.")]
  private int maxColliders = 16 /*0x10*/;
  private Collider[] hits;
  private readonly HashSet<AOEReceiver> visited = new HashSet<AOEReceiver>();
  private float nextTime;

  private void Awake()
  {
    if (this.hits != null && this.hits.Length == this.maxColliders)
      return;
    this.hits = new Collider[Mathf.Max(8, this.maxColliders)];
  }

  private void OnEnable()
  {
    if (this.applyOnEnable)
      this.ApplyAOE();
    this.nextTime = Time.time + this.repeatInterval;
  }

  private void Update()
  {
    if ((double) this.repeatInterval <= 0.0 || (double) Time.time < (double) this.nextTime)
      return;
    this.ApplyAOE();
    this.nextTime = Time.time + this.repeatInterval;
  }

  public void ApplyAOE() => this.ApplyAOE(this.transform.position);

  public void ApplyAOE(Vector3 worldOrigin)
  {
    this.visited.Clear();
    int num1 = Physics.OverlapSphereNonAlloc(worldOrigin, this.radius, this.hits, (int) this.layerMask, this.triggerInteraction);
    float num2 = Mathf.Max(0.0001f, this.radius);
    for (int index = 0; index < num1; ++index)
    {
      Collider hit = this.hits[index];
      if ((bool) (Object) hit)
      {
        AOEReceiver componentInChildren = ((bool) (Object) hit.attachedRigidbody ? (Component) hit.attachedRigidbody.transform : (Component) hit.transform).GetComponentInChildren<AOEReceiver>(true);
        if ((Object) componentInChildren != (Object) null && this.TagValidation(componentInChildren.gameObject) && !this.visited.Contains(componentInChildren))
        {
          this.visited.Add(componentInChildren);
          float num3 = Vector3.Distance(worldOrigin, componentInChildren.transform.position);
          float t = Mathf.Clamp01(num3 / num2);
          float num4 = Mathf.Max(this.minStrength, this.strength * this.EvaluateFalloff(t));
          AOEReceiver.AOEContext AOEContext = new AOEReceiver.AOEContext()
          {
            origin = worldOrigin,
            radius = this.radius,
            instigator = this.gameObject,
            baseStrength = this.strength,
            finalStrength = num4,
            distance = num3,
            normalizedDistance = t
          };
          componentInChildren.ReceiveAOE(in AOEContext);
        }
      }
    }
  }

  private float EvaluateFalloff(float t)
  {
    switch (this.falloffMode)
    {
      case AOESender.FalloffMode.None:
        return 1f;
      case AOESender.FalloffMode.Linear:
        return 1f - t;
      case AOESender.FalloffMode.AnimationCurve:
        return Mathf.Max(0.0f, this.falloffCurve.Evaluate(t));
      default:
        return 1f;
    }
  }

  private bool TagValidation(GameObject go)
  {
    if ((Object) go == (Object) null)
      return false;
    if (this.includeTags == null || this.includeTags.Length == 0)
      return true;
    string tag = go.tag;
    foreach (string includeTag in this.includeTags)
    {
      if (!string.IsNullOrEmpty(includeTag) && tag == includeTag)
        return true;
    }
    return false;
  }

  private enum FalloffMode
  {
    None,
    Linear,
    AnimationCurve,
  }
}
