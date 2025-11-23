// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.NoncontrollableBroomstick
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

#nullable disable
namespace GorillaLocomotion.Gameplay;

public class NoncontrollableBroomstick : MonoBehaviour, IGorillaGrabable
{
  public SplineContainer unitySpline;
  public BezierSpline spline;
  public float duration = 30f;
  public float smoothRotationTrackingRate = 0.5f;
  public bool lookForward = true;
  [SerializeField]
  private float SplineProgressOffet;
  private float progress;
  private float smoothRotationTrackingRateExp;
  [SerializeField]
  private bool constantVelocity;
  private float progressPerFixedUpdate;
  private double secondsToCycles;
  private NativeSpline nativeSpline;
  [SerializeField]
  private bool momentaryGrabOnly = true;

  private void Start()
  {
    this.smoothRotationTrackingRateExp = Mathf.Exp(this.smoothRotationTrackingRate);
    this.progressPerFixedUpdate = Time.fixedDeltaTime / this.duration;
    this.progress = this.SplineProgressOffet;
    this.secondsToCycles = 1.0 / (double) this.duration;
    if (!((Object) this.unitySpline != (Object) null))
      return;
    this.nativeSpline = new NativeSpline((ISpline) this.unitySpline.Spline, (float4x4) this.unitySpline.transform.localToWorldMatrix, Allocator.Persistent);
  }

  protected virtual void FixedUpdate()
  {
    this.progress = !PhotonNetwork.InRoom ? (float) (((double) this.progress + (double) this.progressPerFixedUpdate) % 1.0) : (float) ((PhotonNetwork.Time * this.secondsToCycles + (double) this.SplineProgressOffet) % 1.0);
    Quaternion a = Quaternion.identity;
    if ((Object) this.unitySpline != (Object) null)
    {
      float3 position;
      float3 tangent;
      this.nativeSpline.Evaluate<NativeSpline>(this.progress, out position, out tangent, out float3 _);
      this.transform.position = (Vector3) position;
      if (this.lookForward)
        a = Quaternion.LookRotation(new Vector3(tangent.x, tangent.y, tangent.z));
    }
    else if ((Object) this.spline != (Object) null)
    {
      this.transform.position = this.spline.GetPoint(this.progress, this.constantVelocity);
      if (this.lookForward)
        a = Quaternion.LookRotation(this.spline.GetDirection(this.progress, this.constantVelocity));
    }
    if (!this.lookForward)
      return;
    this.transform.rotation = Quaternion.Slerp(a, this.transform.rotation, Mathf.Exp(-this.smoothRotationTrackingRateExp * Time.deltaTime));
  }

  bool IGorillaGrabable.CanBeGrabbed(GorillaGrabber grabber) => true;

  void IGorillaGrabable.OnGrabbed(
    GorillaGrabber g,
    out Transform grabbedObject,
    out Vector3 grabbedLocalPosition)
  {
    grabbedObject = this.transform;
    grabbedLocalPosition = this.transform.InverseTransformPoint(g.transform.position);
  }

  void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
  {
  }

  private void OnDestroy() => this.nativeSpline.Dispose();

  public bool MomentaryGrabOnly() => this.momentaryGrabOnly;

  string IGorillaGrabable.get_name() => this.name;
}
