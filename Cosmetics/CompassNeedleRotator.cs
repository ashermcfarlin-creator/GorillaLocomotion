// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.CompassNeedleRotator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class CompassNeedleRotator : MonoBehaviour
{
  private const float smoothTime = 0.005f;
  private float currentVelocity;

  protected void OnEnable()
  {
    this.currentVelocity = 0.0f;
    this.transform.localRotation = Quaternion.identity;
  }

  protected void LateUpdate()
  {
    Transform transform = this.transform;
    Vector3 forward = transform.forward with { y = 0.0f };
    forward.Normalize();
    transform.Rotate(transform.up, Mathf.SmoothDamp(Vector3.SignedAngle(forward, Vector3.forward, Vector3.up), 0.0f, ref this.currentVelocity, 0.005f), Space.World);
  }
}
