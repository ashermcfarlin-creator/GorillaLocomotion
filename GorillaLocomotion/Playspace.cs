// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Playspace
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaLocomotion;

public sealed class Playspace : MonoBehaviour
{
  [SerializeField]
  private GameObject _localGorillaHead;
  [SerializeField]
  private float _sphereRadius;
  private float _sqrSphereRadius;
  [SerializeField]
  private float _defaultChaseSpeed;
  [SerializeField]
  private float _snapToThreshold;
  private float _sqrSnapToThreshold;

  private void Awake()
  {
    this._sqrSphereRadius = this._sphereRadius * this._sphereRadius;
    this._sqrSnapToThreshold = this._snapToThreshold * this._snapToThreshold;
  }

  private void Update()
  {
    Vector3 vector3 = this._localGorillaHead.transform.position - this.transform.position;
    float sqrMagnitude = vector3.sqrMagnitude;
    if ((GTPlayer.Instance.enableHoverMode ? 1 : (GTPlayer.Instance.isClimbing ? 1 : 0)) != 0 || (double) vector3.sqrMagnitude > (double) this._sqrSnapToThreshold)
    {
      this.transform.position = this._localGorillaHead.transform.position;
    }
    else
    {
      Vector3 normalized = vector3.normalized;
      vector3 = this.GetChaseSpeed() * Time.deltaTime * normalized;
      this.transform.position = (double) vector3.sqrMagnitude > (double) sqrMagnitude ? this._localGorillaHead.transform.position : this.transform.position + vector3;
      if ((double) (this._localGorillaHead.transform.position - this.transform.position).sqrMagnitude <= (double) this._sqrSphereRadius)
        return;
      this._localGorillaHead.transform.position = this.transform.position + this._sphereRadius * normalized;
    }
  }

  private float GetChaseSpeed() => this._defaultChaseSpeed;

  private void OnDrawGizmosSelected()
  {
    Gizmos.DrawWireSphere(this.transform.position, this._sphereRadius);
  }
}
