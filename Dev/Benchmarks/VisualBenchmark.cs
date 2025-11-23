// Decompiled with JetBrains decompiler
// Type: GorillaTag.Dev.Benchmarks.VisualBenchmark
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

#nullable disable
namespace GorillaTag.Dev.Benchmarks;

public class VisualBenchmark : MonoBehaviour
{
  [Tooltip("the camera will be moved and rotated to these spots and record stats.")]
  public Transform[] benchmarkLocations;
  [Tooltip("How long to wait before calling GC.Collect() to clean up memory.")]
  public float collectGarbageDelay = 2f;
  [Tooltip("How long to wait before recording stats after the camera was moved to a new location.\nThis + collectGarbageDelay is the total time spent at each location.")]
  private float recordStatsDelay = 2f;
  [Tooltip("The camera to use for profiling. If null, a new camera will be created.")]
  private Camera cam;
  private VisualBenchmark.StatInfo[] availableRenderStats;
  private ProfilerRecorder[] renderStatsRecorders;
  private static bool isQuitting = true;
  private int currentLocationIndex;
  private VisualBenchmark.EState state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;
  private float lastTime;
  private readonly StringBuilder sb = new StringBuilder(1024 /*0x0400*/);

  protected void Awake()
  {
    Application.quitting += (Action) (() => VisualBenchmark.isQuitting = true);
    List<ProfilerRecorderHandle> outRecorderHandleList = new List<ProfilerRecorderHandle>(5500);
    ProfilerRecorderHandle.GetAvailable(outRecorderHandleList);
    Debug.Log((object) $"poop Available stats: {outRecorderHandleList.Count}", (UnityEngine.Object) this);
    List<VisualBenchmark.StatInfo> statInfoList = new List<VisualBenchmark.StatInfo>(600);
    foreach (ProfilerRecorderHandle handle in outRecorderHandleList)
    {
      ProfilerRecorderDescription description = ProfilerRecorderHandle.GetDescription(handle);
      if ((int) (ushort) description.Category == (int) (ushort) ProfilerCategory.Render)
        statInfoList.Add(new VisualBenchmark.StatInfo()
        {
          name = description.Name,
          unit = description.UnitType
        });
    }
    this.availableRenderStats = statInfoList.ToArray();
    Debug.Log((object) $"poop availableRenderStats: {statInfoList.Count}", (UnityEngine.Object) this);
    List<Transform> transformList = new List<Transform>(this.benchmarkLocations.Length);
    foreach (Transform benchmarkLocation in this.benchmarkLocations)
    {
      if ((UnityEngine.Object) benchmarkLocation != (UnityEngine.Object) null)
        transformList.Add(benchmarkLocation);
    }
    this.benchmarkLocations = transformList.ToArray();
  }

  protected void OnEnable()
  {
    this.renderStatsRecorders = new ProfilerRecorder[this.availableRenderStats.Length];
    for (int index = 0; index < this.availableRenderStats.Length; ++index)
      this.renderStatsRecorders[index] = ProfilerRecorder.StartNew(ProfilerCategory.Render, this.availableRenderStats[index].name);
    this.state = VisualBenchmark.EState.Setup;
  }

  protected void OnDisable()
  {
    foreach (ProfilerRecorder renderStatsRecorder in this.renderStatsRecorders)
      renderStatsRecorder.Dispose();
  }

  protected void LateUpdate()
  {
    if (VisualBenchmark.isQuitting)
      return;
    switch (this.state)
    {
      case VisualBenchmark.EState.Setup:
        Debug.Log((object) "poop start");
        this.sb.Clear();
        this.currentLocationIndex = 0;
        this.lastTime = Time.realtimeSinceStartup;
        this.state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;
        break;
      case VisualBenchmark.EState.WaitingBeforeCollectingGarbage:
        Debug.Log((object) "poop wait 1");
        if ((double) Time.realtimeSinceStartup - (double) this.lastTime < (double) this.collectGarbageDelay)
          break;
        this.lastTime = Time.time;
        GC.Collect();
        this.state = VisualBenchmark.EState.WaitingBeforeRecordingStats;
        break;
      case VisualBenchmark.EState.WaitingBeforeRecordingStats:
        Debug.Log((object) "poop wait 2");
        if ((double) Time.time - (double) this.lastTime < (double) this.recordStatsDelay)
          break;
        this.lastTime = Time.time;
        this.RecordLocationStats(this.benchmarkLocations[this.currentLocationIndex]);
        if (this.currentLocationIndex < this.benchmarkLocations.Length - 1)
        {
          ++this.currentLocationIndex;
          this.state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;
          break;
        }
        this.state = VisualBenchmark.EState.TearDown;
        break;
      case VisualBenchmark.EState.TearDown:
        Debug.Log((object) "poop teardown");
        Debug.Log((object) this.sb.ToString());
        this.state = VisualBenchmark.EState.Setup;
        if (this.sb.Length <= this.sb.Capacity)
          break;
        Debug.Log((object) ("Capacity exceeded on string builder, increase string builder's capacity. " + $"capacity={this.sb.Capacity}, length={this.sb.Length}"), (UnityEngine.Object) this);
        break;
    }
  }

  private void RecordLocationStats(Transform xform)
  {
    this.sb.Append("Location: ");
    this.sb.Append(xform.name);
    this.sb.Append("\n");
    this.sb.Append("pos=");
    this.sb.Append(xform.position.ToString("F3"));
    this.sb.Append(" rot=");
    this.sb.Append(xform.rotation.ToString("F3"));
    this.sb.Append(" scale=");
    this.sb.Append(xform.lossyScale.ToString("F3"));
    this.sb.Append("\n");
    for (int index = 0; index < this.renderStatsRecorders.Length; ++index)
    {
      this.sb.Append(this.availableRenderStats[index].name);
      this.sb.Append(": ");
      switch (this.availableRenderStats[index].unit)
      {
        case ProfilerMarkerDataUnit.TimeNanoseconds:
          this.sb.Append((double) this.renderStatsRecorders[index].LastValue / 1000000.0);
          this.sb.Append("ms");
          break;
        case ProfilerMarkerDataUnit.Bytes:
          this.sb.Append((double) this.renderStatsRecorders[index].LastValue / 1024.0);
          this.sb.Append("kb");
          break;
        default:
          this.sb.Append(this.renderStatsRecorders[index].LastValue);
          this.sb.Append(' ');
          this.sb.Append(this.availableRenderStats[index].unit.ToString());
          break;
      }
      this.sb.Append('\n');
    }
  }

  private struct StatInfo
  {
    public string name;
    public ProfilerMarkerDataUnit unit;
  }

  private enum EState
  {
    Setup,
    WaitingBeforeCollectingGarbage,
    WaitingBeforeRecordingStats,
    TearDown,
  }
}
