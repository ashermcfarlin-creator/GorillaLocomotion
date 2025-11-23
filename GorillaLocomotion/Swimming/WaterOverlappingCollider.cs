// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Swimming.WaterOverlappingCollider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Swimming;

public struct WaterOverlappingCollider
{
  public bool playBigSplash;
  public bool playDripEffect;
  public bool overrideBoundingRadius;
  public float boundingRadiusOverride;
  public float scaleMultiplier;
  public Collider collider;
  public GorillaVelocityTracker velocityTracker;
  public WaterVolume.SurfaceQuery lastSurfaceQuery;
  public NetworkView photonViewForRPC;
  public bool surfaceDetected;
  public bool inWater;
  public bool inVolume;
  public float lastBoundingRadius;
  public Vector3 lastRipplePosition;
  public float lastRippleScale;
  public float lastRippleTime;
  public float lastInWaterTime;
  public float nextDripTime;

  public void PlayRippleEffect(
    GameObject rippleEffectPrefab,
    Vector3 surfacePoint,
    Vector3 surfaceNormal,
    float defaultRippleScale,
    float currentTime,
    WaterVolume volume)
  {
    this.lastRipplePosition = this.GetClosestPositionOnSurface(surfacePoint, surfaceNormal);
    this.lastBoundingRadius = this.GetBoundingRadiusOnSurface(surfaceNormal);
    this.lastRippleScale = (float) ((double) defaultRippleScale * (double) this.lastBoundingRadius * 2.0) * this.scaleMultiplier;
    this.lastRippleTime = currentTime;
    ObjectPools.instance.Instantiate(rippleEffectPrefab, this.lastRipplePosition, Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right), this.lastRippleScale).GetComponent<WaterRippleEffect>().PlayEffect(volume);
  }

  public void PlaySplashEffect(
    GameObject splashEffectPrefab,
    Vector3 splashPosition,
    float splashScale,
    bool bigSplash,
    bool enteringWater,
    WaterVolume volume)
  {
    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right);
    ObjectPools.instance.Instantiate(splashEffectPrefab, splashPosition, rotation, splashScale * this.scaleMultiplier).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, this.scaleMultiplier, volume);
    if (!((Object) this.photonViewForRPC != (Object) null))
      return;
    float time = Time.time;
    int index1 = -1;
    float num = time + 10f;
    for (int index2 = 0; index2 < WaterVolume.splashRPCSendTimes.Length; ++index2)
    {
      if ((double) WaterVolume.splashRPCSendTimes[index2] < (double) num)
      {
        num = WaterVolume.splashRPCSendTimes[index2];
        index1 = index2;
      }
    }
    if ((double) time - 0.5 <= (double) num)
      return;
    WaterVolume.splashRPCSendTimes[index1] = time;
    this.photonViewForRPC.SendRPC("RPC_PlaySplashEffect", RpcTarget.Others, (object) splashPosition, (object) rotation, (object) (float) ((double) splashScale * (double) this.scaleMultiplier), (object) this.lastBoundingRadius, (object) bigSplash, (object) enteringWater);
  }

  public void PlayDripEffect(
    GameObject rippleEffectPrefab,
    Vector3 surfacePoint,
    Vector3 surfaceNormal,
    float dripScale)
  {
    Vector3 positionOnSurface = this.GetClosestPositionOnSurface(surfacePoint, surfaceNormal);
    Vector3 vector3 = Vector3.ProjectOnPlane(Random.onUnitSphere * (this.overrideBoundingRadius ? this.boundingRadiusOverride : this.lastBoundingRadius) * 0.5f, surfaceNormal);
    ObjectPools.instance.Instantiate(rippleEffectPrefab, positionOnSurface + vector3, Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right), dripScale * this.scaleMultiplier);
  }

  public Vector3 GetClosestPositionOnSurface(Vector3 surfacePoint, Vector3 surfaceNormal)
  {
    return Vector3.ProjectOnPlane(this.collider.transform.position - surfacePoint, surfaceNormal) + surfacePoint;
  }

  private float GetBoundingRadiusOnSurface(Vector3 surfaceNormal)
  {
    if (this.overrideBoundingRadius)
    {
      this.lastBoundingRadius = this.boundingRadiusOverride;
      return this.boundingRadiusOverride;
    }
    Vector3 extents = this.collider.bounds.extents;
    Vector3 vector3_1 = Vector3.ProjectOnPlane(this.collider.transform.right * extents.x, surfaceNormal);
    Vector3 vector3_2 = Vector3.ProjectOnPlane(this.collider.transform.up * extents.y, surfaceNormal);
    Vector3 vector3_3 = Vector3.ProjectOnPlane(this.collider.transform.forward * extents.z, surfaceNormal);
    float sqrMagnitude1 = vector3_1.sqrMagnitude;
    float sqrMagnitude2 = vector3_2.sqrMagnitude;
    float sqrMagnitude3 = vector3_3.sqrMagnitude;
    if ((double) sqrMagnitude1 >= (double) sqrMagnitude2 && (double) sqrMagnitude1 >= (double) sqrMagnitude3)
      return vector3_1.magnitude;
    return (double) sqrMagnitude2 >= (double) sqrMagnitude1 && (double) sqrMagnitude2 >= (double) sqrMagnitude3 ? vector3_2.magnitude : vector3_3.magnitude;
  }
}
