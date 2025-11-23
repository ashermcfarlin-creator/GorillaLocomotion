// Decompiled with JetBrains decompiler
// Type: GorillaExtensions.GorillaMath
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable
namespace GorillaExtensions;

public static class GorillaMath
{
  public static Vector3 GetAngularVelocity(Quaternion oldRotation, Quaternion newRotation)
  {
    Quaternion quaternion = newRotation * Quaternion.Inverse(oldRotation);
    if ((double) Mathf.Abs(quaternion.w) > 0.99951171875)
      return Vector3.zero;
    float num;
    if ((double) quaternion.w < 0.0)
    {
      float f = Mathf.Acos(-quaternion.w);
      num = (float) (-2.0 * (double) f / ((double) Mathf.Sin(f) * (double) Time.deltaTime));
    }
    else
    {
      float f = Mathf.Acos(quaternion.w);
      num = (float) (2.0 * (double) f / ((double) Mathf.Sin(f) * (double) Time.deltaTime));
    }
    Vector3 angularVelocity = new Vector3(quaternion.x * num, quaternion.y * num, quaternion.z * num);
    if (float.IsNaN(angularVelocity.z))
      angularVelocity = Vector3.zero;
    return angularVelocity;
  }

  public static float FastInvSqrt(float z)
  {
    if ((double) z == 0.0)
      return 0.0f;
    GorillaMath.FloatIntUnion floatIntUnion;
    floatIntUnion.tmp = 0;
    float num = 0.5f * z;
    floatIntUnion.f = z;
    floatIntUnion.tmp = 1597463174 - (floatIntUnion.tmp >> 1);
    floatIntUnion.f *= (float) (1.5 - (double) num * (double) floatIntUnion.f * (double) floatIntUnion.f);
    return floatIntUnion.f * z;
  }

  public static float Dot2(in Vector3 v) => Vector3.Dot(v, v);

  public static Vector4 RaycastToCappedCone(
    in Vector3 rayOrigin,
    in Vector3 rayDirection,
    in Vector3 coneTip,
    in Vector3 coneBase,
    in float coneTipRadius,
    in float coneBaseRadius)
  {
    Vector3 vector3_1 = coneBase - coneTip;
    Vector3 vector3_2 = rayOrigin - coneTip;
    Vector3 lhs = rayOrigin - coneBase;
    float z = Vector3.Dot(vector3_1, vector3_1);
    float num1 = Vector3.Dot(vector3_2, vector3_1);
    float num2 = Vector3.Dot(lhs, vector3_1);
    float num3 = Vector3.Dot(rayDirection, vector3_1);
    if ((double) num1 < 0.0)
    {
      if ((double) GorillaMath.Dot2(vector3_2 * num3 - rayDirection * num1) < (double) coneTipRadius * (double) coneTipRadius * (double) num3 * (double) num3)
      {
        Vector3 vector3_3 = -vector3_1 * GorillaMath.FastInvSqrt(z);
        return new Vector4(-num1 / num3, vector3_3.x, vector3_3.y, vector3_3.z);
      }
    }
    else if ((double) num2 > 0.0 && (double) GorillaMath.Dot2(lhs * num3 - rayDirection * num2) < (double) coneBaseRadius * (double) coneBaseRadius * (double) num3 * (double) num3)
    {
      Vector3 vector3_4 = vector3_1 * GorillaMath.FastInvSqrt(z);
      return new Vector4(-num2 / num3, vector3_4.x, vector3_4.y, vector3_4.z);
    }
    float num4 = Vector3.Dot(rayDirection, vector3_2);
    float num5 = Vector3.Dot(vector3_2, vector3_2);
    float num6 = coneTipRadius - coneBaseRadius;
    float num7 = z + num6 * num6;
    float num8 = (float) ((double) z * (double) z - (double) num3 * (double) num3 * (double) num7);
    float num9 = (float) ((double) z * (double) z * (double) num4 - (double) num1 * (double) num3 * (double) num7 + (double) z * (double) coneTipRadius * ((double) num6 * (double) num3 * 1.0));
    float num10 = (float) ((double) z * (double) z * (double) num5 - (double) num1 * (double) num1 * (double) num7 + (double) z * (double) coneTipRadius * ((double) num6 * (double) num1 * 2.0 - (double) z * (double) coneTipRadius));
    float f = (float) ((double) num9 * (double) num9 - (double) num8 * (double) num10);
    if ((double) f < 0.0)
      return -Vector4.one;
    float x = (-num9 - Mathf.Sqrt(f)) / num8;
    float num11 = num1 + x * num3;
    if ((double) num11 <= 0.0 || (double) num11 >= (double) z)
      return -Vector4.one;
    Vector3 normalized = (z * (z * (vector3_2 + x * rayDirection) + num6 * vector3_1 * coneTipRadius) - vector3_1 * num7 * num11).normalized;
    return new Vector4(x, normalized.x, normalized.y, normalized.z);
  }

  public static void LineSegClosestPoints(
    Vector3 a,
    Vector3 u,
    Vector3 b,
    Vector3 v,
    out Vector3 lineAPoint,
    out Vector3 lineBPoint)
  {
    lineAPoint = a;
    lineBPoint = b;
    Vector3 lhs = b - a;
    float num1 = Vector3.Dot(lhs, u);
    float num2 = Vector3.Dot(lhs, v);
    float num3 = Vector3.Dot(u, u);
    float num4 = Vector3.Dot(u, v);
    float num5 = Vector3.Dot(v, v);
    float f = (float) ((double) num3 * (double) num5 - (double) num4 * (double) num4);
    if ((double) Mathf.Abs(f) < 0.001)
      return;
    float num6 = (float) ((double) num1 * (double) num5 - (double) num2 * (double) num4) / f;
    double num7 = ((double) num1 * (double) num4 - (double) num2 * (double) num3) / (double) f;
    float num8 = Mathf.Clamp(num6, 0.0f, 1f);
    float num9 = (Mathf.Clamp((float) num7, 0.0f, 1f) * num4 + num1) / num3;
    float num10 = (num8 * num4 - num2) / num5;
    float num11 = Mathf.Clamp(num9, 0.0f, 1f);
    float num12 = Mathf.Clamp(num10, 0.0f, 1f);
    lineAPoint = a + num11 * u;
    lineBPoint = b + num12 * v;
  }

  [Serializable]
  public struct RemapFloatInfo(float fromMin = 0.0f, float toMin = 0.0f, float fromMax = 1f, float toMax = 1f)
  {
    public float fromMin = fromMin;
    public float toMin = toMin;
    public float fromMax = fromMax;
    public float toMax = toMax;

    public void OnValidate()
    {
      if ((double) this.fromMin < (double) this.fromMax)
        this.fromMin = this.fromMax + float.Epsilon;
      if ((double) this.toMin >= (double) this.toMax)
        return;
      this.toMin = this.toMax + float.Epsilon;
    }

    public bool IsValid()
    {
      return (double) this.fromMin < (double) this.fromMax && (double) this.toMin < (double) this.toMax;
    }

    public float Remap(float value)
    {
      return this.toMin + (float) (((double) value - (double) this.fromMin) / ((double) this.fromMax - (double) this.fromMin) * ((double) this.toMax - (double) this.toMin));
    }
  }

  [StructLayout(LayoutKind.Explicit)]
  private struct FloatIntUnion
  {
    [FieldOffset(0)]
    public float f;
    [FieldOffset(0)]
    public int tmp;
  }
}
