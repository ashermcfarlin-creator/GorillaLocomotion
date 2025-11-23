// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Swimming.UnderwaterCameraEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using CjLib;
using GorillaTag;
using System;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Swimming;

[ExecuteAlways]
public class UnderwaterCameraEffect : MonoBehaviour
{
  private const float edgeBuffer = 0.04f;
  [SerializeField]
  private Camera targetCamera;
  [SerializeField]
  private MeshRenderer planeRenderer;
  [SerializeField]
  private UnderwaterParticleEffects underwaterParticleEffect;
  [SerializeField]
  private float distanceFromCamera = 0.02f;
  [SerializeField]
  [DebugOption]
  private bool debugDraw;
  private float cachedAspectRatio = 1f;
  private float cachedFov = 90f;
  private readonly Vector3[] frustumPlaneCornersLocal = new Vector3[4];
  private Vector2 frustumPlaneExtents;
  private GTPlayer player;
  private WaterVolume.SurfaceQuery waterSurface;
  private const string kShaderKeyword_GlobalCameraTouchingWater = "_GLOBAL_CAMERA_TOUCHING_WATER";
  private const string kShaderKeyword_GlobalCameraFullyUnderwater = "_GLOBAL_CAMERA_FULLY_UNDERWATER";
  private int shaderParam_GlobalCameraOverlapWaterSurfacePlane = Shader.PropertyToID("_GlobalCameraOverlapWaterSurfacePlane");
  private bool hasTargetCamera;
  [DebugReadout]
  private UnderwaterCameraEffect.CameraOverlapWaterState cameraOverlapWaterState;

  private void SetOffScreenPosition()
  {
    this.transform.localScale = new Vector3((float) (2.0 * ((double) this.frustumPlaneExtents.x + 0.039999999105930328)), 0.0f, 1f);
    this.transform.localPosition = new Vector3(0.0f, (float) -((double) this.frustumPlaneExtents.y + 0.039999999105930328), this.distanceFromCamera);
  }

  private void SetFullScreenPosition()
  {
    this.transform.localScale = new Vector3((float) (2.0 * ((double) this.frustumPlaneExtents.x + 0.039999999105930328)), (float) (2.0 * ((double) this.frustumPlaneExtents.y + 0.039999999105930328)), 1f);
    this.transform.localPosition = new Vector3(0.0f, 0.0f, this.distanceFromCamera);
  }

  private void OnEnable()
  {
    if ((UnityEngine.Object) this.targetCamera == (UnityEngine.Object) null)
      this.targetCamera = Camera.main;
    this.hasTargetCamera = (UnityEngine.Object) this.targetCamera != (UnityEngine.Object) null;
    this.InitializeShaderProperties();
  }

  private void Start()
  {
    this.player = GTPlayer.Instance;
    this.cachedAspectRatio = this.targetCamera.aspect;
    this.cachedFov = this.targetCamera.fieldOfView;
    this.CalculateFrustumPlaneBounds(this.cachedFov, this.cachedAspectRatio);
    this.SetOffScreenPosition();
  }

  private void LateUpdate()
  {
    if (!this.hasTargetCamera || !(bool) (UnityEngine.Object) this.player)
      return;
    if (this.player.HeadOverlappingWaterVolumes.Count < 1)
    {
      this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
      if (this.planeRenderer.enabled)
      {
        this.planeRenderer.enabled = false;
        this.SetOffScreenPosition();
      }
      if (!((UnityEngine.Object) this.underwaterParticleEffect != (UnityEngine.Object) null) || !this.underwaterParticleEffect.gameObject.activeInHierarchy)
        return;
      this.underwaterParticleEffect.UpdateParticleEffect(false, ref this.waterSurface);
    }
    else
    {
      if ((double) this.targetCamera.aspect != (double) this.cachedAspectRatio || (double) this.targetCamera.fieldOfView != (double) this.cachedFov)
      {
        this.cachedAspectRatio = this.targetCamera.aspect;
        this.cachedFov = this.targetCamera.fieldOfView;
        this.CalculateFrustumPlaneBounds(this.cachedFov, this.cachedAspectRatio);
      }
      bool waterSurfaceDetected = false;
      float num1 = float.MinValue;
      Vector3 position = this.targetCamera.transform.position;
      for (int index = 0; index < this.player.HeadOverlappingWaterVolumes.Count; ++index)
      {
        WaterVolume.SurfaceQuery result;
        if (this.player.HeadOverlappingWaterVolumes[index].GetSurfaceQueryForPoint(position, out result))
        {
          float num2 = Vector3.Dot(result.surfacePoint - position, result.surfaceNormal);
          if ((double) num2 > (double) num1)
          {
            waterSurfaceDetected = true;
            num1 = num2;
            this.waterSurface = result;
          }
        }
      }
      if (waterSurfaceDetected)
      {
        Vector3 inPoint = this.targetCamera.transform.InverseTransformPoint(this.waterSurface.surfacePoint);
        Vector3 point;
        Vector3 direction;
        if (this.IntersectPlanes(new Plane(Vector3.forward, -this.distanceFromCamera), new Plane(this.targetCamera.transform.InverseTransformDirection(this.waterSurface.surfaceNormal), inPoint), out point, out direction))
        {
          Vector3 normalized = Vector3.Cross(direction, Vector3.forward).normalized;
          float num3 = Vector3.Dot(new Vector3(point.x, point.y, 0.0f), normalized);
          if ((double) num3 > (double) this.frustumPlaneExtents.y + 0.039999999105930328)
          {
            this.SetFullScreenPosition();
            this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged);
          }
          else if ((double) num3 < -((double) this.frustumPlaneExtents.y + 0.039999999105930328))
          {
            this.SetOffScreenPosition();
            this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
          }
          else
          {
            float y = num3 + (this.GetFrustumCoverageDistance(-normalized) + 0.04f);
            this.transform.localScale = new Vector3(this.GetFrustumCoverageDistance(direction) + 0.04f + (this.GetFrustumCoverageDistance(-direction) + 0.04f), y, 1f);
            this.transform.localPosition = normalized * (num3 - y * 0.5f) + new Vector3(0.0f, 0.0f, this.distanceFromCamera);
            this.transform.localRotation = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, normalized, Vector3.forward), Vector3.forward);
            this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged);
          }
          if (this.debugDraw)
          {
            Vector3 vector3_1 = this.targetCamera.transform.TransformPoint(point);
            Vector3 vector3_2 = this.targetCamera.transform.TransformDirection(direction);
            DebugUtil.DrawLine(vector3_1 - 2f * this.frustumPlaneExtents.x * vector3_2, vector3_1 + 2f * this.frustumPlaneExtents.x * vector3_2, Color.white, false);
          }
        }
        else if (new Plane(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint).GetSide(this.targetCamera.transform.position))
        {
          this.SetFullScreenPosition();
          this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged);
        }
        else
        {
          this.SetOffScreenPosition();
          this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
        }
      }
      else
      {
        this.SetOffScreenPosition();
        this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
      }
      if (!((UnityEngine.Object) this.underwaterParticleEffect != (UnityEngine.Object) null) || !this.underwaterParticleEffect.gameObject.activeInHierarchy)
        return;
      this.underwaterParticleEffect.UpdateParticleEffect(waterSurfaceDetected, ref this.waterSurface);
    }
  }

  [DebugOption]
  private void InitializeShaderProperties()
  {
    Shader.DisableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
    Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
    Shader.SetGlobalVector(this.shaderParam_GlobalCameraOverlapWaterSurfacePlane, new Vector4(this.waterSurface.surfaceNormal.x, this.waterSurface.surfaceNormal.y, this.waterSurface.surfaceNormal.z, -Vector3.Dot(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint)));
  }

  private void SetCameraOverlapState(
    UnderwaterCameraEffect.CameraOverlapWaterState state)
  {
    if (state != this.cameraOverlapWaterState || state == UnderwaterCameraEffect.CameraOverlapWaterState.Uninitialized)
    {
      this.cameraOverlapWaterState = state;
      switch (this.cameraOverlapWaterState)
      {
        case UnderwaterCameraEffect.CameraOverlapWaterState.Uninitialized:
        case UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater:
          Shader.DisableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
          Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
          break;
        case UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged:
          Shader.EnableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
          Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
          break;
        case UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged:
          Shader.EnableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
          Shader.EnableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
          break;
      }
    }
    if (this.cameraOverlapWaterState != UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged)
      return;
    Plane plane = new Plane(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint);
    Shader.SetGlobalVector(this.shaderParam_GlobalCameraOverlapWaterSurfacePlane, new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance));
  }

  private void CalculateFrustumPlaneBounds(float fieldOfView, float aspectRatio)
  {
    float num = Mathf.Tan((float) (Math.PI / 180.0 * (double) fieldOfView * 0.5)) * this.distanceFromCamera;
    float x = (float) ((double) aspectRatio * (double) num + 0.039999999105930328);
    float y = (float) (1.0 / (double) aspectRatio * (double) num + 0.039999999105930328);
    this.frustumPlaneExtents = new Vector2(x, y);
    this.frustumPlaneCornersLocal[0] = new Vector3(-x, -y, this.distanceFromCamera);
    this.frustumPlaneCornersLocal[1] = new Vector3(-x, y, this.distanceFromCamera);
    this.frustumPlaneCornersLocal[2] = new Vector3(x, y, this.distanceFromCamera);
    this.frustumPlaneCornersLocal[3] = new Vector3(x, -y, this.distanceFromCamera);
  }

  private bool IntersectPlanes(Plane p1, Plane p2, out Vector3 point, out Vector3 direction)
  {
    direction = Vector3.Cross(p1.normal, p2.normal);
    float num = Vector3.Dot(direction, direction);
    if ((double) num < (double) Mathf.Epsilon)
    {
      point = Vector3.zero;
      return false;
    }
    point = Vector3.Cross(direction, p1.distance * p2.normal - p2.distance * p1.normal) / num;
    return true;
  }

  private float GetFrustumCoverageDistance(Vector3 localDirection)
  {
    float coverageDistance = float.MinValue;
    for (int index = 0; index < this.frustumPlaneCornersLocal.Length; ++index)
    {
      float num = Vector3.Dot(this.frustumPlaneCornersLocal[index], localDirection);
      if ((double) num > (double) coverageDistance)
        coverageDistance = num;
    }
    return coverageDistance;
  }

  private enum CameraOverlapWaterState
  {
    Uninitialized,
    OutOfWater,
    PartiallySubmerged,
    FullySubmerged,
  }
}
