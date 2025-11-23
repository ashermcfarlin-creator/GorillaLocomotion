// Decompiled with JetBrains decompiler
// Type: GorillaTag.Audio.GTRecorder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Photon.Voice.Unity;
using System.Collections;
using UnityEngine;

#nullable disable
namespace GorillaTag.Audio;

public class GTRecorder : Recorder, ITickSystemPost
{
  public bool AllowPitchAdjustment;
  public float PitchAdjustment = 1f;
  public bool AllowVolumeAdjustment;
  public float VolumeAdjustment = 1f;
  public float DebugEchoLength = 5f;
  private GTMicWrapper _micWrapper;
  private Coroutine _testEchoCoroutine;

  public bool PostTickRunning { get; set; }

  private void OnEnable() => TickSystem<object>.AddPostTickCallback((ITickSystemPost) this);

  private void OnDisable() => TickSystem<object>.RemovePostTickCallback((ITickSystemPost) this);

  protected override MicWrapper CreateMicWrapper(
    string micDev,
    int samplingRateInt,
    VoiceLogger logger)
  {
    this._micWrapper = new GTMicWrapper(micDev, samplingRateInt, this.AllowPitchAdjustment, this.PitchAdjustment, this.AllowVolumeAdjustment, this.VolumeAdjustment, (Photon.Voice.ILogger) logger);
    return (MicWrapper) this._micWrapper;
  }

  private IEnumerator DoTestEcho()
  {
    GTRecorder gtRecorder = this;
    gtRecorder.DebugEchoMode = true;
    yield return (object) new WaitForSeconds(gtRecorder.DebugEchoLength);
    gtRecorder.DebugEchoMode = false;
    yield return (object) null;
    gtRecorder._testEchoCoroutine = (Coroutine) null;
  }

  public void PostTick()
  {
    if (this._micWrapper == null)
      return;
    this._micWrapper.UpdateWrapper(this.AllowPitchAdjustment, this.PitchAdjustment, this.AllowVolumeAdjustment, this.VolumeAdjustment);
  }
}
