// Decompiled with JetBrains decompiler
// Type: GorillaTag.MonkeFX.MonkeFXSettingsSO
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.MonkeFX;

[CreateAssetMenu(fileName = "MeshGenerator", menuName = "ScriptableObjects/MeshGenerator", order = 1)]
public class MonkeFXSettingsSO : ScriptableObject
{
  public GTDirectAssetRef<Mesh>[] sourceMeshes;
  [HideInInspector]
  public Mesh combinedMesh;

  protected void Awake() => GorillaTag.MonkeFX.MonkeFX.Register(this);
}
