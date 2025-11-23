// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Swimming.UnderwaterParticleEffects
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using CjLib;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Swimming;

public class UnderwaterParticleEffects : MonoBehaviour
{
  public ParticleSystem underwaterFloaterParticles;
  public ParticleSystem underwaterBubbleParticles;
  public Camera playerCamera;
  public Vector3 floaterParticleBoxExtents = Vector3.one;
  public Vector3 floaterParticleBaseOffset = Vector3.forward;
  public AnimationCurve floaterSpeedVsOffsetDist = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  public Vector2 floaterSpeedVsOffsetDistMinMax = new Vector2(0.0f, 1f);
  private bool debugDraw;

  public void UpdateParticleEffect(
    bool waterSurfaceDetected,
    ref WaterVolume.SurfaceQuery waterSurface)
  {
    GTPlayer instance = GTPlayer.Instance;
    Plane plane = new Plane(waterSurface.surfaceNormal, waterSurface.surfacePoint);
    if ((!waterSurfaceDetected ? 0 : ((double) plane.GetDistanceToPoint(instance.headCollider.transform.position) < (double) instance.headCollider.radius ? 1 : 0)) != 0)
    {
      this.underwaterFloaterParticles.gameObject.SetActive(true);
      Vector3 averagedVelocity = instance.AveragedVelocity;
      float magnitude = averagedVelocity.magnitude;
      Vector3 vector3_1 = (double) magnitude > 1.0 / 1000.0 ? averagedVelocity / magnitude : this.playerCamera.transform.forward;
      float num1 = this.floaterSpeedVsOffsetDist.Evaluate(Mathf.Clamp(magnitude, this.floaterSpeedVsOffsetDistMinMax.x, this.floaterSpeedVsOffsetDistMinMax.y));
      Quaternion rotation = this.playerCamera.transform.rotation;
      Vector3 vector3_2 = this.playerCamera.transform.position + this.playerCamera.transform.rotation * this.floaterParticleBaseOffset + vector3_1 * num1;
      Vector3 point1 = vector3_2 + rotation * new Vector3(0.0f, this.floaterParticleBoxExtents.y, -this.floaterParticleBoxExtents.z);
      Vector3 point2 = vector3_2 + rotation * new Vector3(0.0f, this.floaterParticleBoxExtents.y, this.floaterParticleBoxExtents.z);
      float num2 = this.floaterParticleBoxExtents.z * 2f;
      float num3 = plane.GetDistanceToPoint(point1);
      float num4 = plane.GetDistanceToPoint(point2);
      Quaternion quaternion = rotation;
      Vector3 vector = vector3_2;
      if ((double) num3 > 0.0 || (double) num4 > 0.0)
      {
        if ((double) point1.y < (double) point2.y)
        {
          if ((double) num3 > 0.0)
          {
            point1 -= plane.normal * num3;
            num3 = 0.0f;
          }
          Vector3 rhs = (new Vector3(point2.x, point1.y, point2.z) - point1).normalized * num2;
          Vector3 axis = Vector3.Cross(point2 - point1, rhs);
          quaternion = Quaternion.AngleAxis((float) (((double) Mathf.Asin((point2.y - point1.y) / num2) - (double) Mathf.Asin(-num3 / num2)) * 57.295780181884766), axis) * this.playerCamera.transform.rotation;
          vector = point1 + quaternion * new Vector3(0.0f, -this.floaterParticleBoxExtents.y, this.floaterParticleBoxExtents.z);
        }
        else
        {
          if ((double) num4 > 0.0)
          {
            point2 -= plane.normal * num4;
            num4 = 0.0f;
          }
          Vector3 rhs = (new Vector3(point1.x, point2.y, point1.z) - point2).normalized * num2;
          Vector3 axis = Vector3.Cross(point1 - point2, rhs);
          quaternion = Quaternion.AngleAxis((float) (((double) Mathf.Asin((point1.y - point2.y) / num2) - (double) Mathf.Asin(-num4 / num2)) * 57.295780181884766), axis) * this.playerCamera.transform.rotation;
          vector = point2 + quaternion * new Vector3(0.0f, -this.floaterParticleBoxExtents.y, -this.floaterParticleBoxExtents.z);
        }
      }
      if (this.IsValid(vector))
      {
        this.underwaterFloaterParticles.transform.rotation = quaternion;
        this.underwaterFloaterParticles.transform.position = vector;
      }
      else
        this.underwaterFloaterParticles.gameObject.SetActive(false);
      if (!this.debugDraw)
        return;
      Vector3 vector3_3 = vector3_2 + rotation * new Vector3(0.0f, this.floaterParticleBoxExtents.y, -this.floaterParticleBoxExtents.z);
      Vector3 vector3_4 = vector3_2 + rotation * new Vector3(0.0f, this.floaterParticleBoxExtents.y, this.floaterParticleBoxExtents.z);
      DebugUtil.DrawSphere(vector3_3, 0.1f, 12, 12, Color.red, false, DebugUtil.Style.SolidColor);
      DebugUtil.DrawSphere(vector3_4, 0.1f, 12, 12, Color.red, false, DebugUtil.Style.SolidColor);
      DebugUtil.DrawLine(vector3_3, vector3_4, Color.red, false);
      Vector3 vector3_5 = vector + quaternion * new Vector3(0.0f, this.floaterParticleBoxExtents.y, -this.floaterParticleBoxExtents.z);
      Vector3 vector3_6 = vector + quaternion * new Vector3(0.0f, this.floaterParticleBoxExtents.y, this.floaterParticleBoxExtents.z);
      DebugUtil.DrawSphere(vector3_5, 0.1f, 12, 12, Color.green, false, DebugUtil.Style.SolidColor);
      DebugUtil.DrawSphere(vector3_6, 0.1f, 12, 12, Color.green, false, DebugUtil.Style.SolidColor);
      DebugUtil.DrawLine(vector3_5, vector3_6, Color.green, false);
    }
    else
      this.underwaterFloaterParticles.gameObject.SetActive(false);
  }

  private bool IsValid(Vector3 vector)
  {
    return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z);
  }
}
