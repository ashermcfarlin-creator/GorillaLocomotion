// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Swimming.WaterCurrent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using AA;
using CjLib;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Swimming;

public class WaterCurrent : MonoBehaviour
{
  [SerializeField]
  private List<CatmullRomSpline> splines = new List<CatmullRomSpline>();
  [SerializeField]
  private float fullEffectDistance = 1f;
  [SerializeField]
  private float fadeDistance = 0.5f;
  [SerializeField]
  private float currentSpeed = 1f;
  [SerializeField]
  private float currentAccel = 10f;
  [SerializeField]
  private float velocityAnticipationAdjustment = 0.05f;
  [SerializeField]
  private float inwardCurrentFullEffectRadius = 1f;
  [SerializeField]
  private float inwardCurrentNoEffectRadius = 0.25f;
  [SerializeField]
  private float inwardCurrentSpeed = 1f;
  [SerializeField]
  private float inwardCurrentAccel = 10f;
  [SerializeField]
  private float dampingHalfLife = 0.25f;
  [SerializeField]
  private bool debugDrawCurrentQueries;
  private Vector3 debugCurrentVelocity = Vector3.zero;
  private Vector3 debugSplinePoint = Vector3.zero;

  public float Speed => this.currentSpeed;

  public float Accel => this.currentAccel;

  public float InwardSpeed => this.inwardCurrentSpeed;

  public float InwardAccel => this.inwardCurrentAccel;

  public bool GetCurrentAtPoint(
    Vector3 worldPoint,
    Vector3 startingVelocity,
    float dt,
    out Vector3 currentVelocity,
    out Vector3 velocityChange)
  {
    float num1 = (float) (((double) this.fullEffectDistance + (double) this.fadeDistance) * ((double) this.fullEffectDistance + (double) this.fadeDistance));
    bool currentAtPoint = false;
    velocityChange = Vector3.zero;
    currentVelocity = Vector3.zero;
    float num2 = 0.0001f;
    float magnitude1 = startingVelocity.magnitude;
    if ((double) magnitude1 > (double) num2)
    {
      Vector3 vector3 = startingVelocity / magnitude1 * Spring.DamperDecayExact(magnitude1, this.dampingHalfLife, dt);
      velocityChange += vector3 - startingVelocity;
    }
    for (int index = 0; index < this.splines.Count; ++index)
    {
      CatmullRomSpline spline = this.splines[index];
      Vector3 linePoint;
      float evaluationOnSpline = spline.GetClosestEvaluationOnSpline(worldPoint, out linePoint);
      Vector3 vector3 = spline.Evaluate(evaluationOnSpline);
      Vector3 vector = vector3 - worldPoint;
      if ((double) vector.sqrMagnitude < (double) num1)
      {
        currentAtPoint = true;
        float magnitude2 = vector.magnitude;
        float num3 = (double) magnitude2 > (double) this.fullEffectDistance ? 1f - Mathf.Clamp01((magnitude2 - this.fullEffectDistance) / this.fadeDistance) : 1f;
        float t = Mathf.Clamp01(evaluationOnSpline + this.velocityAnticipationAdjustment);
        Vector3 forwardTangent = spline.GetForwardTangent(t);
        if ((double) this.currentSpeed > (double) num2 && (double) Vector3.Dot(startingVelocity, forwardTangent) < (double) num3 * (double) this.currentSpeed)
          velocityChange += forwardTangent * (this.currentAccel * dt);
        else if ((double) this.currentSpeed < (double) num2 && (double) Vector3.Dot(startingVelocity, forwardTangent) > (double) num3 * (double) this.currentSpeed)
          velocityChange -= forwardTangent * (this.currentAccel * dt);
        currentVelocity += forwardTangent * num3 * this.currentSpeed;
        float num4 = Mathf.InverseLerp(this.inwardCurrentNoEffectRadius, this.inwardCurrentFullEffectRadius, magnitude2);
        if ((double) num4 > (double) num2)
        {
          linePoint = Vector3.ProjectOnPlane(vector, forwardTangent);
          Vector3 normalized = linePoint.normalized;
          if ((double) this.inwardCurrentSpeed > (double) num2 && (double) Vector3.Dot(startingVelocity, normalized) < (double) num4 * (double) this.inwardCurrentSpeed)
            velocityChange += normalized * (this.InwardAccel * dt);
          else if ((double) this.inwardCurrentSpeed < (double) num2 && (double) Vector3.Dot(startingVelocity, normalized) > (double) num4 * (double) this.inwardCurrentSpeed)
            velocityChange -= normalized * (this.InwardAccel * dt);
        }
        this.debugSplinePoint = vector3;
      }
    }
    this.debugCurrentVelocity = velocityChange.normalized;
    return currentAtPoint;
  }

  private void Update()
  {
    if (!this.debugDrawCurrentQueries)
      return;
    DebugUtil.DrawSphere(this.debugSplinePoint, 0.15f, 12, 12, Color.green, false);
    DebugUtil.DrawArrow(this.debugSplinePoint, this.debugSplinePoint + this.debugCurrentVelocity, 0.1f, 0.1f, 12, 0.1f, Color.yellow, false);
  }

  private void OnDrawGizmosSelected()
  {
    int num = 16 /*0x10*/;
    for (int index1 = 0; index1 < this.splines.Count; ++index1)
    {
      CatmullRomSpline spline = this.splines[index1];
      Vector3 vector3_1 = spline.Evaluate(0.0f);
      for (int index2 = 1; index2 <= num; ++index2)
      {
        float t = (float) index2 / (float) num;
        Vector3 center = spline.Evaluate(t);
        Vector3 vector3_2 = center - vector3_1;
        Quaternion rotation = Quaternion.LookRotation(spline.GetForwardTangent(t), Vector3.up);
        Gizmos.color = new Color(0.0f, 0.5f, 0.75f);
        this.DrawGizmoCircle(center, rotation, this.fullEffectDistance);
        Gizmos.color = new Color(0.0f, 0.25f, 0.5f);
        this.DrawGizmoCircle(center, rotation, this.fullEffectDistance + this.fadeDistance);
      }
    }
  }

  private void DrawGizmoCircle(Vector3 center, Quaternion rotation, float radius)
  {
    Vector3 vector3_1 = Vector3.right * radius;
    int num = 16 /*0x10*/;
    for (int index = 1; index <= num; ++index)
    {
      float f = (float) ((double) index / (double) num * 2.0 * 3.1415927410125732);
      Vector3 vector3_2 = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0.0f) * radius;
      Gizmos.DrawLine(center + rotation * vector3_1, center + rotation * vector3_2);
      vector3_1 = vector3_2;
    }
  }
}
