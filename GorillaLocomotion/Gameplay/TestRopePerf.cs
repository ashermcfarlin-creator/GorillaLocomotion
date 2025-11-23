// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.TestRopePerf
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Gameplay;

public class TestRopePerf : MonoBehaviour
{
  [SerializeField]
  private GameObject ropesOld;
  [SerializeField]
  private GameObject ropesCustom;
  [SerializeField]
  private GameObject ropesCustomVectorized;

  private IEnumerator Start()
  {
    yield break;
  }
}
