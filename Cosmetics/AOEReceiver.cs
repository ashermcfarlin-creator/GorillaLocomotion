// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.AOEReceiver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class AOEReceiver : MonoBehaviour
{
  public AOEContextEvent OnAOEReceived;
  [Tooltip("Quick toggle to disable receiving without disabling the GameObject.")]
  [SerializeField]
  private bool enabledForAOE = true;

  public void ReceiveAOE(in AOEReceiver.AOEContext AOEContext)
  {
    if (!this.enabledForAOE)
      return;
    this.OnAOEReceived?.Invoke(AOEContext);
  }

  [Serializable]
  public struct AOEContext
  {
    public Vector3 origin;
    public float radius;
    public GameObject instigator;
    public float baseStrength;
    public float finalStrength;
    public float distance;
    public float normalizedDistance;
  }
}
