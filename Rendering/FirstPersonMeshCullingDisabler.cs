// Decompiled with JetBrains decompiler
// Type: GorillaTag.Rendering.FirstPersonMeshCullingDisabler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Rendering;

public class FirstPersonMeshCullingDisabler : MonoBehaviour
{
  private Mesh[] meshes;
  private Transform[] xforms;

  protected void Awake()
  {
    MeshFilter[] componentsInChildren = this.GetComponentsInChildren<MeshFilter>();
    if (componentsInChildren == null)
      return;
    this.meshes = new Mesh[componentsInChildren.Length];
    this.xforms = new Transform[componentsInChildren.Length];
    for (int index = 0; index < componentsInChildren.Length; ++index)
    {
      this.meshes[index] = componentsInChildren[index].mesh;
      this.xforms[index] = componentsInChildren[index].transform;
    }
  }

  protected void OnEnable()
  {
    Camera main = Camera.main;
    if ((Object) main == (Object) null)
      return;
    Transform transform = main.transform;
    Vector3 position1 = transform.position;
    Vector3 vector3 = Vector3.Normalize(transform.forward);
    float nearClipPlane = main.nearClipPlane;
    float num = (float) (((double) main.farClipPlane - (double) nearClipPlane) / 2.0) + nearClipPlane;
    Vector3 position2 = position1 + vector3 * num;
    for (int index = 0; index < this.meshes.Length; ++index)
    {
      Vector3 center = this.xforms[index].InverseTransformPoint(position2);
      this.meshes[index].bounds = new Bounds(center, Vector3.one);
    }
  }
}
