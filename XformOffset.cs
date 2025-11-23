// Decompiled with JetBrains decompiler
// Type: GorillaTag.XformOffset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag;

[Serializable]
public struct XformOffset
{
  [Tooltip("The position of the offset relative to the parent bone.")]
  public Vector3 pos;
  [FormerlySerializedAs("_edRotQuat")]
  [FormerlySerializedAs("rot")]
  [HideInInspector]
  [SerializeField]
  private Quaternion _rotQuat;
  [FormerlySerializedAs("_edRotEulerAngles")]
  [FormerlySerializedAs("_edRotEuler")]
  [HideInInspector]
  [SerializeField]
  private Vector3 _rotEulerAngles;
  [Tooltip("The scale of the offset relative to the parent bone.")]
  public Vector3 scale;
  public static readonly XformOffset Identity = new XformOffset()
  {
    _rotQuat = Quaternion.identity,
    scale = Vector3.one
  };

  [Tooltip("The rotation of the offset relative to the parent bone.")]
  public Quaternion rot
  {
    get => this._rotQuat;
    set => this._rotQuat = value;
  }

  public XformOffset(Vector3 pos, Quaternion rot, Vector3 scale)
  {
    this.pos = pos;
    this._rotQuat = rot;
    this._rotEulerAngles = rot.eulerAngles;
    this.scale = scale;
  }

  public XformOffset(Vector3 pos, Vector3 rot, Vector3 scale)
  {
    this.pos = pos;
    this._rotQuat = Quaternion.Euler(rot);
    this._rotEulerAngles = rot;
    this.scale = scale;
  }

  public XformOffset(Vector3 pos, Quaternion rot)
  {
    this.pos = pos;
    this._rotQuat = rot;
    this._rotEulerAngles = rot.eulerAngles;
    this.scale = Vector3.one;
  }

  public XformOffset(Vector3 pos, Vector3 rot)
  {
    this.pos = pos;
    this._rotQuat = Quaternion.Euler(rot);
    this._rotEulerAngles = rot;
    this.scale = Vector3.one;
  }

  public XformOffset(Transform parentXform, Transform childXform)
  {
    this.pos = parentXform.InverseTransformPoint(childXform.position);
    this._rotQuat = Quaternion.Inverse(parentXform.rotation) * childXform.rotation;
    this._rotEulerAngles = this._rotQuat.eulerAngles;
    this.scale = childXform.lossyScale.SafeDivide(parentXform.lossyScale);
  }

  public XformOffset(Matrix4x4 matrix)
  {
    this.pos = matrix.GetPosition();
    this.scale = matrix.lossyScale;
    if ((double) Vector3.Dot(Vector3.Cross((Vector3) matrix.GetColumn(0), (Vector3) matrix.GetColumn(1)), (Vector3) matrix.GetColumn(2)) < 0.0)
      this.scale = -this.scale;
    Matrix4x4 matrix4x4 = matrix;
    matrix4x4.SetColumn(0, matrix4x4.GetColumn(0) / this.scale.x);
    matrix4x4.SetColumn(1, matrix4x4.GetColumn(1) / this.scale.y);
    matrix4x4.SetColumn(2, matrix4x4.GetColumn(2) / this.scale.z);
    this._rotQuat = Quaternion.LookRotation((Vector3) matrix4x4.GetColumn(2), (Vector3) matrix4x4.GetColumn(1));
    this._rotEulerAngles = this._rotQuat.eulerAngles;
  }

  public bool Approx(XformOffset other)
  {
    return this.pos.Approx(other.pos) && this._rotQuat.Approx(other._rotQuat) && this.scale.Approx(other.scale);
  }
}
