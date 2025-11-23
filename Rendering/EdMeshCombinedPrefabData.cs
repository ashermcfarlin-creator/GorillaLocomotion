// Decompiled with JetBrains decompiler
// Type: GorillaTag.Rendering.EdMeshCombinedPrefabData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag.Rendering;

[Serializable]
public class EdMeshCombinedPrefabData
{
  public string path;
  public List<Renderer> disabled = new List<Renderer>(512 /*0x0200*/);
  public List<GameObject> combined = new List<GameObject>(64 /*0x40*/);

  public void Clear()
  {
  }
}
