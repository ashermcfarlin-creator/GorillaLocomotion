// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Climbing.HandHoldXSceneRef
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Climbing;

public class HandHoldXSceneRef : MonoBehaviour
{
  [SerializeField]
  public XSceneRef reference;

  public HandHold target
  {
    get
    {
      HandHold result;
      return this.reference.TryResolve<HandHold>(out result) ? result : (HandHold) null;
    }
  }

  public GameObject targetObject
  {
    get
    {
      GameObject result;
      return this.reference.TryResolve(out result) ? result : (GameObject) null;
    }
  }
}
