// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Swimming.RigidbodyWaterInteraction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using AA;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Swimming;

[RequireComponent(typeof (Rigidbody))]
public class RigidbodyWaterInteraction : MonoBehaviour
{
  public bool applyDamping = true;
  public bool applyBuoyancyForce = true;
  public bool applyAngularDrag = true;
  public bool applyWaterCurrents = true;
  public bool applySurfaceTorque = true;
  public float underWaterDampingHalfLife = 0.25f;
  public float waterSurfaceDampingHalfLife = 1f;
  public float underWaterBuoyancyFactor = 0.5f;
  public float angularDrag = 0.5f;
  public float surfaceTorqueAmount = 0.5f;
  public bool enablePreciseWaterCollision;
  public float objectRadiusForWaterCollision = 0.25f;
  [Range(0.0f, 1f)]
  public float buoyancyEquilibrium = 0.8f;
  private Rigidbody rb;
  private List<WaterVolume> overlappingWaterVolumes = new List<WaterVolume>();
  private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16 /*0x10*/);
  private float baseAngularDrag = 0.05f;

  protected void Awake()
  {
    this.rb = this.GetComponent<Rigidbody>();
    this.baseAngularDrag = this.rb.angularDamping;
    RigidbodyWaterInteractionManager.RegisterRBWI(this);
  }

  protected void OnEnable()
  {
    this.overlappingWaterVolumes.Clear();
    RigidbodyWaterInteractionManager.RegisterRBWI(this);
  }

  protected void OnDisable()
  {
    this.overlappingWaterVolumes.Clear();
    RigidbodyWaterInteractionManager.UnregisterRBWI(this);
  }

  private void OnDestroy() => RigidbodyWaterInteractionManager.UnregisterRBWI(this);

  public void InvokeFixedUpdate()
  {
    if (this.rb.isKinematic)
      return;
    bool flag1 = this.overlappingWaterVolumes.Count > 0;
    WaterVolume.SurfaceQuery surfaceQuery = new WaterVolume.SurfaceQuery();
    float num1 = float.MinValue;
    if (flag1 && this.enablePreciseWaterCollision)
    {
      Vector3 point = this.transform.position + Vector3.down * 2f * this.objectRadiusForWaterCollision * this.buoyancyEquilibrium;
      bool flag2 = false;
      this.activeWaterCurrents.Clear();
      for (int index = 0; index < this.overlappingWaterVolumes.Count; ++index)
      {
        WaterVolume.SurfaceQuery result;
        if (this.overlappingWaterVolumes[index].GetSurfaceQueryForPoint(point, out result))
        {
          float num2 = Vector3.Dot(result.surfacePoint - point, result.surfaceNormal);
          if ((double) num2 > (double) num1)
          {
            num1 = num2;
            surfaceQuery = result;
            flag2 = true;
          }
          WaterCurrent current = this.overlappingWaterVolumes[index].Current;
          if (this.applyWaterCurrents && (Object) current != (Object) null && (double) num2 > 0.0 && !this.activeWaterCurrents.Contains(current))
            this.activeWaterCurrents.Add(current);
        }
      }
      flag1 = flag2 && (double) num1 > -(1.0 - (double) this.buoyancyEquilibrium) * 2.0 * (double) this.objectRadiusForWaterCollision & (double) this.transform.position.y + (this.enablePreciseWaterCollision ? (double) this.objectRadiusForWaterCollision : 0.0) - ((double) surfaceQuery.surfacePoint.y - (double) surfaceQuery.maxDepth) > 0.0;
    }
    if (flag1)
    {
      float fixedDeltaTime = Time.fixedDeltaTime;
      Vector3 lhs1 = this.rb.linearVelocity;
      Vector3 zero1 = Vector3.zero;
      if (this.applyWaterCurrents)
      {
        Vector3 zero2 = Vector3.zero;
        for (int index = 0; index < this.activeWaterCurrents.Count; ++index)
        {
          WaterCurrent activeWaterCurrent = this.activeWaterCurrents[index];
          Vector3 vector3_1 = lhs1 + zero1;
          Vector3 position = this.transform.position;
          Vector3 startingVelocity = vector3_1;
          double dt = (double) fixedDeltaTime;
          Vector3 vector3_2;
          ref Vector3 local1 = ref vector3_2;
          Vector3 vector3_3;
          ref Vector3 local2 = ref vector3_3;
          if (activeWaterCurrent.GetCurrentAtPoint(position, startingVelocity, (float) dt, out local1, out local2))
          {
            zero2 += vector3_2;
            zero1 += vector3_3;
          }
        }
        if (this.enablePreciseWaterCollision)
        {
          Vector3 position = (surfaceQuery.surfacePoint + (this.transform.position + Vector3.down * this.objectRadiusForWaterCollision)) * 0.5f;
          this.rb.AddForceAtPosition(zero1 * this.rb.mass, position, ForceMode.Impulse);
        }
        else
          lhs1 += zero1;
      }
      if (this.applyBuoyancyForce)
      {
        Vector3 zero3 = Vector3.zero;
        Vector3 lhs2 = !this.enablePreciseWaterCollision ? -Physics.gravity * this.underWaterBuoyancyFactor * fixedDeltaTime : -Physics.gravity * this.underWaterBuoyancyFactor * Mathf.InverseLerp(0.0f, 2f * this.objectRadiusForWaterCollision * this.buoyancyEquilibrium, num1) * fixedDeltaTime;
        if ((double) zero1.sqrMagnitude > 1.0 / 1000.0)
        {
          float magnitude = zero1.magnitude;
          Vector3 rhs = zero1 / magnitude;
          float num3 = Vector3.Dot(lhs2, rhs);
          if ((double) num3 < 0.0)
            lhs2 -= num3 * rhs;
        }
        lhs1 += lhs2;
      }
      float magnitude1 = lhs1.magnitude;
      if ((double) magnitude1 > 1.0 / 1000.0 && this.applyDamping)
      {
        Vector3 vector3 = lhs1 / magnitude1;
        float num4 = Spring.DamperDecayExact(magnitude1, this.underWaterDampingHalfLife, fixedDeltaTime);
        if (this.enablePreciseWaterCollision)
        {
          double a = (double) Spring.DamperDecayExact(magnitude1, this.waterSurfaceDampingHalfLife, fixedDeltaTime);
          float num5 = (float) ((double) Mathf.Clamp((float) -((double) this.transform.position.y - (double) surfaceQuery.surfacePoint.y) / this.objectRadiusForWaterCollision, -1f, 1f) * 0.5 + 0.5);
          double b = (double) num4;
          double t = (double) num5;
          lhs1 = Mathf.Lerp((float) a, (float) b, (float) t) * vector3;
        }
        else
          lhs1 = num4 * vector3;
      }
      if (this.applySurfaceTorque && this.enablePreciseWaterCollision)
      {
        float num6 = this.transform.position.y - surfaceQuery.surfacePoint.y;
        if ((double) num6 < (double) this.objectRadiusForWaterCollision && (double) num6 > 0.0)
        {
          Vector3 rhs = lhs1 - Vector3.Dot(lhs1, surfaceQuery.surfaceNormal) * surfaceQuery.surfaceNormal;
          Vector3 normalized = Vector3.Cross(surfaceQuery.surfaceNormal, rhs).normalized;
          float num7 = Vector3.Dot(this.rb.angularVelocity, normalized);
          float num8 = rhs.magnitude / this.objectRadiusForWaterCollision - num7;
          if ((double) num8 > 0.0)
            this.rb.AddTorque(this.surfaceTorqueAmount * num8 * normalized, ForceMode.Acceleration);
        }
      }
      this.rb.linearVelocity = lhs1;
      this.rb.angularDamping = this.angularDrag;
    }
    else
      this.rb.angularDamping = this.baseAngularDrag;
  }

  protected void OnTriggerEnter(Collider other)
  {
    WaterVolume component = other.GetComponent<WaterVolume>();
    if (!((Object) component != (Object) null) || this.overlappingWaterVolumes.Contains(component))
      return;
    this.overlappingWaterVolumes.Add(component);
  }

  protected void OnTriggerExit(Collider other)
  {
    WaterVolume component = other.GetComponent<WaterVolume>();
    if (!((Object) component != (Object) null) || !this.overlappingWaterVolumes.Contains(component))
      return;
    this.overlappingWaterVolumes.Remove(component);
  }
}
