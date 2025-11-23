// Decompiled with JetBrains decompiler
// Type: GorillaTag.Audio.PlanarSound
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Audio;

public class PlanarSound : MonoBehaviour
{
  private Transform cameraXform;
  private bool hasCamera;
  [SerializeField]
  private bool limitDistance;
  [SerializeField]
  private float maxDistance = 1f;

  protected void OnEnable()
  {
    if (!((Object) Camera.main != (Object) null))
      return;
    this.cameraXform = Camera.main.transform;
    this.hasCamera = true;
  }

  protected void LateUpdate()
  {
    if (!this.hasCamera)
      return;
    Transform transform = this.transform;
    Vector3 vector3 = transform.parent.InverseTransformPoint(this.cameraXform.position) with
    {
      y = 0.0f
    };
    if (this.limitDistance && (double) vector3.sqrMagnitude > (double) this.maxDistance * (double) this.maxDistance)
      vector3 = vector3.normalized * this.maxDistance;
    transform.localPosition = vector3;
  }
}
