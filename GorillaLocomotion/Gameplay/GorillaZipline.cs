// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.GorillaZipline
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaLocomotion.Climbing;
using System;
using Unity.Mathematics;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Gameplay;

public class GorillaZipline : MonoBehaviour
{
  [SerializeField]
  protected Transform segmentsRoot;
  [SerializeField]
  protected GameObject segmentPrefab;
  [SerializeField]
  protected GorillaClimbable slideHelper;
  [SerializeField]
  private AudioSource audioSlide;
  protected BezierSpline spline;
  [SerializeField]
  private Transform climbOffsetHelper;
  [SerializeField]
  private GorillaZiplineSettings settings;
  [SerializeField]
  protected float ziplineDistance = 15f;
  [SerializeField]
  protected float segmentDistance = 0.9f;
  private GorillaHandClimber currentClimber;
  private float currentT;
  private const float inheritVelocityRechargeRate = 0.2f;
  private const float inheritVelocityValueOnRelease = 0.55f;
  private float currentInheritVelocityMulti = 1f;

  public float currentSpeed { get; private set; }

  protected void FindTFromDistance(ref float t, float distance, int steps = 1000)
  {
    float num1 = distance / (float) steps;
    Vector3 b = this.spline.GetPointLocal(t);
    float num2 = 0.0f;
    for (int index = 0; index < 1000; ++index)
    {
      t += num1;
      if ((double) t >= 1.0 || (double) t <= 0.0)
        break;
      Vector3 pointLocal = this.spline.GetPointLocal(t);
      num2 += Vector3.Distance(pointLocal, b);
      if ((double) num2 >= (double) Mathf.Abs(distance))
        break;
      b = pointLocal;
    }
  }

  private float FindSlideHelperSpot(Vector3 grabPoint)
  {
    int num1 = 0;
    int num2 = 200;
    float t = 1f / 1000f;
    float num3 = 1f / (float) num2;
    float3 y = (float3) this.transform.InverseTransformPoint(grabPoint);
    float slideHelperSpot = 0.0f;
    float num4 = float.PositiveInfinity;
    for (; num1 < num2; ++num1)
    {
      float num5 = math.distancesq((float3) this.spline.GetPointLocal(t), y);
      if ((double) num5 < (double) num4)
      {
        num4 = num5;
        slideHelperSpot = t;
      }
      t += num3;
    }
    return slideHelperSpot;
  }

  protected virtual void Start()
  {
    this.spline = this.GetComponent<BezierSpline>();
    this.slideHelper.onBeforeClimb += new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb);
  }

  private void OnDestroy()
  {
    this.slideHelper.onBeforeClimb -= new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb);
  }

  public Vector3 GetCurrentDirection() => this.spline.GetDirection(this.currentT);

  protected void OnBeforeClimb(GorillaHandClimber hand, GorillaClimbableRef climbRef)
  {
    int num1 = (UnityEngine.Object) this.currentClimber == (UnityEngine.Object) null ? 1 : 0;
    this.currentClimber = hand;
    if ((bool) (UnityEngine.Object) climbRef)
    {
      this.climbOffsetHelper.SetParent(climbRef.transform);
      this.climbOffsetHelper.position = hand.transform.position;
      this.climbOffsetHelper.localPosition = new Vector3(0.0f, 0.0f, this.climbOffsetHelper.localPosition.z);
    }
    this.currentT = this.FindSlideHelperSpot(this.climbOffsetHelper.position);
    this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
    if (num1 == 0)
      return;
    Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
    float num2 = Vector3.Dot(averagedVelocity.normalized, this.spline.GetDirection(this.currentT));
    this.currentSpeed = averagedVelocity.magnitude * num2 * this.currentInheritVelocityMulti;
  }

  private void Update()
  {
    if ((bool) (UnityEngine.Object) this.currentClimber)
    {
      this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.settings.maxSpeed, Physics.gravity.y * this.spline.GetDirection(this.currentT).y * this.settings.gravityMulti * Time.deltaTime);
      this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, 0.0f, MathUtils.Linear(this.currentSpeed, 0.0f, this.settings.maxFrictionSpeed, this.settings.friction, this.settings.maxFriction) * Time.deltaTime);
      this.currentSpeed = Mathf.Min(this.currentSpeed, this.settings.maxSpeed);
      this.currentSpeed = Mathf.Max(this.currentSpeed, -this.settings.maxSpeed);
      float num = Mathf.Abs(this.currentSpeed);
      this.FindTFromDistance(ref this.currentT, this.currentSpeed * Time.deltaTime);
      this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
      if (!this.audioSlide.gameObject.activeSelf)
        this.audioSlide.gameObject.SetActive(true);
      this.audioSlide.volume = MathUtils.Linear(num, 0.0f, this.settings.maxSpeed, this.settings.minSlideVolume, this.settings.maxSlideVolume);
      this.audioSlide.pitch = MathUtils.Linear(num, 0.0f, this.settings.maxSpeed, this.settings.minSlidePitch, this.settings.maxSlidePitch);
      if (!this.audioSlide.isPlaying)
        this.audioSlide.GTPlay();
      float amplitude = MathUtils.Linear(num, 0.0f, this.settings.maxSpeed, -0.1f, 0.75f);
      if ((double) amplitude > 0.0)
        GorillaTagger.Instance.DoVibration(this.currentClimber.xrNode, amplitude, Time.deltaTime);
      if (!this.spline.Loop)
      {
        if ((double) this.currentT >= 1.0 || (double) this.currentT <= 0.0)
          this.currentClimber.ForceStopClimbing(doDontReclimb: true);
      }
      else if ((double) this.currentT >= 1.0)
        this.currentT = 0.0f;
      else if ((double) this.currentT <= 0.0)
        this.currentT = 1f;
      if (!this.slideHelper.isBeingClimbed)
        this.Stop();
    }
    if ((double) this.currentInheritVelocityMulti >= 1.0)
      return;
    this.currentInheritVelocityMulti += Time.deltaTime * 0.2f;
    this.currentInheritVelocityMulti = Mathf.Min(this.currentInheritVelocityMulti, 1f);
  }

  private void Stop()
  {
    this.currentClimber = (GorillaHandClimber) null;
    this.audioSlide.GTStop();
    this.audioSlide.gameObject.SetActive(false);
    this.currentInheritVelocityMulti = 0.55f;
    this.currentSpeed = 0.0f;
  }
}
