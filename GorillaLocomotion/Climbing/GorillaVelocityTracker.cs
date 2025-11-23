// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Climbing.GorillaVelocityTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaLocomotion.Climbing;

public class GorillaVelocityTracker : MonoBehaviour, ITickSystemTick
{
  [SerializeField]
  private int maxDataPoints = 20;
  [SerializeField]
  private Transform relativeTo;
  [Tooltip("Use in Editor to trigger events when above or higher than a desired latest velocity.")]
  [SerializeField]
  private bool useVelocityEvents;
  [SerializeField]
  private float latestVelocityThreshold;
  public UnityEvent OnLatestBelowThreshold;
  public UnityEvent OnLatestAboveThreshold;
  [SerializeField]
  private bool useWorldSpaceForEvents;
  private bool wasAboveThreshold;
  private int currentDataPointIndex;
  private GorillaVelocityTracker.VelocityDataPoint[] localSpaceData;
  private GorillaVelocityTracker.VelocityDataPoint[] worldSpaceData;
  private Transform trans;
  private Vector3 lastWorldSpacePos;
  private Vector3 lastLocalSpacePos;
  private bool isRelativeTo;
  private int lastTickedFrame = -1;

  public bool TickRunning { get; set; }

  public void ResetState()
  {
    this.trans = this.transform;
    this.localSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
    PopulateArray(this.localSpaceData);
    this.worldSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
    PopulateArray(this.worldSpaceData);
    this.isRelativeTo = (Object) this.relativeTo != (Object) null;
    this.lastLocalSpacePos = this.GetPosition(false);
    this.lastWorldSpacePos = this.GetPosition(true);
    this.wasAboveThreshold = false;

    void PopulateArray(GorillaVelocityTracker.VelocityDataPoint[] array)
    {
      for (int index = 0; index < this.maxDataPoints; ++index)
        array[index] = new GorillaVelocityTracker.VelocityDataPoint();
    }
  }

  private void Awake() => this.ResetState();

  private void OnEnable() => TickSystem<object>.AddTickCallback((ITickSystemTick) this);

  private void OnDisable()
  {
    this.ResetState();
    TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
  }

  public void SetRelativeTo(Transform tf)
  {
    this.relativeTo = tf;
    this.isRelativeTo = (Object) tf != (Object) null;
  }

  private Vector3 GetPosition(bool worldSpace)
  {
    if (worldSpace)
      return this.trans.position;
    return this.isRelativeTo ? this.relativeTo.InverseTransformPoint(this.trans.position) : this.trans.localPosition;
  }

  public void Tick()
  {
    if (Time.frameCount <= this.lastTickedFrame)
      return;
    Vector3 position1 = this.GetPosition(false);
    Vector3 position2 = this.GetPosition(true);
    GorillaVelocityTracker.VelocityDataPoint velocityDataPoint1 = this.localSpaceData[this.currentDataPointIndex];
    velocityDataPoint1.delta = (position1 - this.lastLocalSpacePos) / Time.deltaTime;
    velocityDataPoint1.time = Time.time;
    this.localSpaceData[this.currentDataPointIndex] = velocityDataPoint1;
    GorillaVelocityTracker.VelocityDataPoint velocityDataPoint2 = this.worldSpaceData[this.currentDataPointIndex];
    velocityDataPoint2.delta = (position2 - this.lastWorldSpacePos) / Time.deltaTime;
    velocityDataPoint2.time = Time.time;
    this.worldSpaceData[this.currentDataPointIndex] = velocityDataPoint2;
    this.lastLocalSpacePos = position1;
    this.lastWorldSpacePos = position2;
    ++this.currentDataPointIndex;
    if (this.currentDataPointIndex >= this.maxDataPoints)
      this.currentDataPointIndex = 0;
    if (this.useVelocityEvents)
      this.GetLatestVelocity(this.useWorldSpaceForEvents);
    this.lastTickedFrame = Time.frameCount;
  }

  private void AddToQueue(
    ref List<GorillaVelocityTracker.VelocityDataPoint> dataPoints,
    GorillaVelocityTracker.VelocityDataPoint newData)
  {
    dataPoints.Add(newData);
    if (dataPoints.Count < this.maxDataPoints)
      return;
    dataPoints.RemoveAt(0);
  }

  public Vector3 GetAverageVelocity(bool worldSpace = false, float maxTimeFromPast = 0.15f, bool doMagnitudeCheck = false)
  {
    float num1 = maxTimeFromPast / 2f;
    GorillaVelocityTracker.VelocityDataPoint[] velocityDataPointArray = !worldSpace ? this.localSpaceData : this.worldSpaceData;
    if (velocityDataPointArray.Length <= 1)
      return Vector3.zero;
    Vector3 total = Vector3.zero;
    float totalMag = 0.0f;
    int added = 0;
    float num2 = Time.time - maxTimeFromPast;
    float num3 = Time.time - num1;
    int num4 = 0;
    int index = this.currentDataPointIndex;
    for (; num4 < this.maxDataPoints; ++num4)
    {
      GorillaVelocityTracker.VelocityDataPoint point = velocityDataPointArray[index];
      if (doMagnitudeCheck && added > 1 && (double) point.time >= (double) num3)
      {
        if ((double) point.delta.magnitude >= (double) totalMag / (double) added)
          AddPoint(point);
      }
      else if ((double) point.time >= (double) num2)
        AddPoint(point);
      ++index;
      if (index >= this.maxDataPoints)
        index = 0;
    }
    return added > 0 ? total / (float) added : Vector3.zero;

    void AddPoint(GorillaVelocityTracker.VelocityDataPoint point)
    {
      total += point.delta;
      totalMag += point.delta.magnitude;
      added++;
    }
  }

  public Vector3 GetLatestVelocity(bool worldSpace = false)
  {
    GorillaVelocityTracker.VelocityDataPoint[] velocityDataPointArray = !worldSpace ? this.localSpaceData : this.worldSpaceData;
    if ((double) velocityDataPointArray[this.currentDataPointIndex].delta.magnitude >= (double) this.latestVelocityThreshold && !this.wasAboveThreshold)
    {
      this.OnLatestAboveThreshold?.Invoke();
      this.wasAboveThreshold = true;
    }
    else if ((double) velocityDataPointArray[this.currentDataPointIndex].delta.magnitude < (double) this.latestVelocityThreshold && this.wasAboveThreshold)
    {
      this.OnLatestBelowThreshold?.Invoke();
      this.wasAboveThreshold = false;
    }
    return velocityDataPointArray[this.currentDataPointIndex].delta;
  }

  public float GetAverageSpeedChangeMagnitudeInDirection(
    Vector3 dir,
    bool worldSpace = false,
    float maxTimeFromPast = 0.05f)
  {
    GorillaVelocityTracker.VelocityDataPoint[] velocityDataPointArray = !worldSpace ? this.localSpaceData : this.worldSpaceData;
    if (velocityDataPointArray.Length <= 1)
      return 0.0f;
    float num1 = 0.0f;
    int num2 = 0;
    float num3 = Time.time - maxTimeFromPast;
    bool flag = false;
    Vector3 vector3 = Vector3.zero;
    for (int index = 0; index < velocityDataPointArray.Length; ++index)
    {
      if ((double) velocityDataPointArray[index].time >= (double) num3)
      {
        if (!flag)
        {
          vector3 = velocityDataPointArray[index].delta;
          flag = true;
        }
        else
        {
          num1 += Mathf.Abs(Vector3.Dot(velocityDataPointArray[index].delta - vector3, dir));
          ++num2;
        }
      }
    }
    return num2 <= 0 ? 0.0f : num1 / (float) num2;
  }

  public class VelocityDataPoint
  {
    public Vector3 delta;
    public float time = -1f;
  }
}
