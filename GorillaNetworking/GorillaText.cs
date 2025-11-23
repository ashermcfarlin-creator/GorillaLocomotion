// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.GorillaText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaNetworking;

[Serializable]
public class GorillaText
{
  private string failureText;
  private string originalText = string.Empty;
  private bool failedState;
  private Material[] originalMaterials;
  private Material failureMaterial;
  internal Material[] currentMaterials;
  private UnityEvent<string> updateTextCallback;
  private UnityEvent<Material[]> updateMaterialCallback;

  public void Initialize(
    Material[] originalMaterials,
    Material failureMaterial,
    UnityEvent<string> callback = null,
    UnityEvent<Material[]> materialCallback = null)
  {
    this.failureMaterial = failureMaterial;
    this.originalMaterials = originalMaterials;
    this.currentMaterials = originalMaterials;
    Debug.Log((object) ("Original text = " + this.originalText));
    this.updateTextCallback = callback;
    this.updateMaterialCallback = materialCallback;
  }

  public string Text
  {
    get => this.originalText;
    set
    {
      if (this.originalText == value)
        return;
      this.originalText = value;
      if (this.failedState)
        return;
      this.updateTextCallback?.Invoke(value);
    }
  }

  public void EnableFailedState(string failText)
  {
    this.failedState = true;
    this.failureText = failText;
    this.updateTextCallback?.Invoke(failText);
    this.currentMaterials = (Material[]) this.originalMaterials.Clone();
    this.currentMaterials[0] = this.failureMaterial;
    this.updateMaterialCallback?.Invoke(this.currentMaterials);
  }

  public void DisableFailedState()
  {
    this.failedState = false;
    this.updateTextCallback?.Invoke(this.originalText);
    this.failureText = "";
    this.currentMaterials = this.originalMaterials;
    this.updateMaterialCallback?.Invoke(this.currentMaterials);
  }
}
