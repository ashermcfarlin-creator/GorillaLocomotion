// Decompiled with JetBrains decompiler
// Type: GorillaTag.Audio.LoudSpeakerTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Audio;

public class LoudSpeakerTrigger : MonoBehaviour
{
  public float PitchAdjustment = 1f;
  [SerializeField]
  private LoudSpeakerNetwork _network;
  [SerializeField]
  private GTRecorder _recorder;

  public void SetRecorder(GTRecorder recorder) => this._recorder = recorder;

  public void OnPlayerEnter(VRRig player)
  {
    if (!((Object) this._recorder != (Object) null) || !((Object) this._network != (Object) null))
      return;
    this._recorder.AllowPitchAdjustment = true;
    this._recorder.PitchAdjustment = this.PitchAdjustment;
    this._network.StartBroadcastSpeakerOutput(player);
  }

  public void OnPlayerExit(VRRig player)
  {
    if (!((Object) this._recorder != (Object) null) || !((Object) this._network != (Object) null))
      return;
    this._recorder.AllowPitchAdjustment = false;
    this._recorder.PitchAdjustment = 1f;
    this._network.StopBroadcastSpeakerOutput(player);
  }
}
