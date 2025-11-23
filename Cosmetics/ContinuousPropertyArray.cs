// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ContinuousPropertyArray
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

[Serializable]
public class ContinuousPropertyArray
{
  [Tooltip("Divides the input value by this number before being fed into the property array. Unless you know what you're doing, you should probably leave this at 1. You can accomplish the same thing by changing the maximum X value for all the curves/gradients, this is just a shorthand.")]
  [SerializeField]
  private float maxExpectedValue = 1f;
  private float inverseMaximum;
  [Tooltip("Determines how quickly the internal value lerps towards the input value. A low number will take a long time to match but will be more resistant to fluctuations, visa versa for a high value. A good starting point is 5 to 10.")]
  [SerializeField]
  private float responsiveness = 5f;
  [Tooltip("If true (default behavior), the input value will be used directly. Disable this if you need better control over how smoothly the properties get applied.")]
  [SerializeField]
  private bool instant = true;
  [SerializeField]
  private ContinuousProperty[] list;
  private List<int> uniqueShaderPropertyIndices;
  private MaterialPropertyBlock mpb;
  private bool initialized;
  private float value;
  private float lastApplyTime;
  [NonSerialized]
  public bool cachedRigIsLocal;

  public int Count => this.list.Length;

  private void InitIfNeeded()
  {
    if (this.initialized)
      return;
    this.initialized = true;
    this.inverseMaximum = 1f / this.maxExpectedValue;
    this.value = 0.0f;
    this.lastApplyTime = Time.time - Time.deltaTime;
    for (int index = 0; index < this.list.Length; ++index)
      this.list[index].Init();
    if (Application.isPlaying)
    {
      for (int index = 0; index < this.list.Length; ++index)
        this.list[index].InitThreshold();
    }
    this.uniqueShaderPropertyIndices = new List<int>();
    this.mpb = new MaterialPropertyBlock();
    ContinuousPropertyArray.PropertyComparer propertyComparer = new ContinuousPropertyArray.PropertyComparer();
    Array.Sort<ContinuousProperty>(this.list, (IComparer<ContinuousProperty>) propertyComparer);
    if (!this.list[0].IsShaderProperty_Cached)
      return;
    for (int index = 0; index < this.list.Length; ++index)
    {
      if (this.list[index].IsShaderProperty_Cached)
      {
        if (index == this.list.Length - 1 || index > 0 && propertyComparer.Compare(this.list[index - 1], this.list[index]) != 0)
          this.uniqueShaderPropertyIndices.Add(index);
      }
      else
      {
        this.uniqueShaderPropertyIndices.Add(index);
        break;
      }
    }
  }

  public void ApplyAll(bool leftHand, float f) => this.ApplyAll(f);

  public void ApplyAll(float f)
  {
    if (this.list.Length == 0)
      return;
    this.InitIfNeeded();
    float deltaTime = Time.time - this.lastApplyTime;
    this.value = this.instant ? f * this.inverseMaximum : Mathf.Lerp(this.value, f * this.inverseMaximum, 1f - Mathf.Exp(-this.responsiveness * deltaTime));
    this.lastApplyTime = Time.time;
    int index1 = int.MaxValue;
    if (this.uniqueShaderPropertyIndices.Count > 0)
    {
      index1 = 0;
      ((Renderer) this.list[0].Target).GetPropertyBlock(this.mpb, this.list[0].IntValue);
    }
    bool cachedRigIsLocal = this.cachedRigIsLocal;
    for (int index2 = 0; index2 < this.list.Length; ++index2)
    {
      this.list[index2].SetRigIsLocal(cachedRigIsLocal);
      this.list[index2].Apply(this.value, deltaTime, this.mpb);
      if (index1 < this.uniqueShaderPropertyIndices.Count && index2 >= this.uniqueShaderPropertyIndices[index1] - 1)
      {
        ((Renderer) this.list[index2].Target).SetPropertyBlock(this.mpb, this.list[0].IntValue);
        if (++index1 < this.uniqueShaderPropertyIndices.Count)
          ((Renderer) this.list[index2 + 1].Target).GetPropertyBlock(this.mpb, this.list[index2 + 1].IntValue);
      }
    }
  }

  private class PropertyComparer : IComparer<ContinuousProperty>
  {
    public int Compare(ContinuousProperty x, ContinuousProperty y)
    {
      return !x.IsShaderProperty_Cached || !y.IsShaderProperty_Cached ? y.IsShaderProperty_Cached.CompareTo(x.IsShaderProperty_Cached) : (x.GetTargetInstanceID() ^ x.IntValue).CompareTo(y.GetTargetInstanceID() ^ y.IntValue);
    }
  }
}
