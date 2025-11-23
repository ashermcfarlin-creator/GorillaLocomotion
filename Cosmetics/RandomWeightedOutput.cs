// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.RandomWeightedOutput
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

[RequireComponent(typeof (NetworkedRandomProvider))]
public class RandomWeightedOutput : MonoBehaviour
{
  [Header("Network Provider")]
  [Tooltip("For best result, pick Float01 or Double01 as the output mode in your NetworkedRandomProvider")]
  [SerializeField]
  private NetworkedRandomProvider networkProvider;
  [Header("Weighted Outputs")]
  [SerializeField]
  private List<RandomWeightedOutput.WeightedOutput> outputs = new List<RandomWeightedOutput.WeightedOutput>();
  [Header("Event")]
  [SerializeField]
  public UnityEvent<int> onAnyPick = new UnityEvent<int>();
  [SerializeField]
  private bool debugLog;

  private void Awake()
  {
    if (!((UnityEngine.Object) this.networkProvider == (UnityEngine.Object) null))
      return;
    this.networkProvider = this.GetComponentInParent<NetworkedRandomProvider>();
  }

  public void PickNextRandom()
  {
    int deterministicPickIndex = this.GetDeterministicPickIndex();
    if (deterministicPickIndex < 0)
      return;
    this.outputs[deterministicPickIndex].onPick?.Invoke();
    this.onAnyPick?.Invoke(deterministicPickIndex);
    if (!this.debugLog)
      return;
    Debug.Log((object) $"[RandomWeightedOutput] Picked '{this.outputs[deterministicPickIndex].name}' (idx={deterministicPickIndex})");
  }

  private int GetDeterministicPickIndex()
  {
    if ((UnityEngine.Object) this.networkProvider == (UnityEngine.Object) null)
      return -1;
    List<int> intList1 = new List<int>(this.outputs.Count);
    for (int index = 0; index < this.outputs.Count; ++index)
    {
      RandomWeightedOutput.WeightedOutput output = this.outputs[index];
      if (output != null && output.enabled && (double) output.weight > 0.0)
        intList1.Add(index);
    }
    if (intList1.Count == 0)
      return -1;
    double num1 = 0.0;
    foreach (int index in intList1)
      num1 += (double) this.outputs[index].weight;
    if (num1 <= 0.0)
      return intList1[0];
    double num2 = (double) this.networkProvider.GetSelectedAsFloat() * num1;
    double num3 = 0.0;
    for (int index1 = 0; index1 < intList1.Count; ++index1)
    {
      int index2 = intList1[index1];
      num3 += (double) this.outputs[index2].weight;
      if (num2 < num3)
        return index2;
    }
    List<int> intList2 = intList1;
    return intList2[intList2.Count - 1];
  }

  [Serializable]
  public class WeightedOutput
  {
    [SerializeField]
    public string name = "Event";
    [SerializeField]
    [Range(0.0f, 100f)]
    public float weight = 1f;
    [SerializeField]
    public bool enabled = true;
    [SerializeField]
    public UnityEvent onPick = new UnityEvent();
  }
}
