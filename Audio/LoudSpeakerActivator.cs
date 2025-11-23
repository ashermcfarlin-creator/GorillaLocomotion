// Decompiled with JetBrains decompiler
// Type: GorillaTag.Audio.LoudSpeakerActivator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Audio;

public class LoudSpeakerActivator : MonoBehaviour
{
  public float PitchAdjustment = 1f;
  public float VolumeAdjustment = 2.5f;
  public bool IsBroadcasting;
  [SerializeField]
  private LoudSpeakerNetwork _network;
  [SerializeField]
  private GTRecorder _recorder;
  private bool _isLocal;
  private VRRig _nonlocalRig;

  private void Awake()
  {
    this._isLocal = this.IsParentedToLocalRig();
    if (this._isLocal)
      return;
    this._nonlocalRig = this.transform.root.GetComponent<VRRig>();
  }

  private bool IsParentedToLocalRig()
  {
    if ((Object) VRRigCache.Instance.localRig == (Object) null)
      return false;
    for (Transform parent = this.transform.parent; (Object) parent != (Object) null; parent = parent.parent)
    {
      if ((Object) parent == (Object) VRRigCache.Instance.localRig.transform)
        return true;
    }
    return false;
  }

  public void SetRecorder(GTRecorder recorder) => this._recorder = recorder;

  public void StartLocalBroadcast()
  {
    if (!this._isLocal)
    {
      if (!((Object) this._network != (Object) null) || !((Object) this._nonlocalRig != (Object) null))
        return;
      this._network.StartBroadcastSpeakerOutput(this._nonlocalRig);
    }
    else
    {
      if (this.IsBroadcasting)
        return;
      if ((Object) this._recorder == (Object) null && (Object) NetworkSystem.Instance.LocalRecorder != (Object) null)
        this.SetRecorder((GTRecorder) NetworkSystem.Instance.LocalRecorder);
      if (!((Object) this._recorder != (Object) null) || !((Object) this._network != (Object) null))
        return;
      this.IsBroadcasting = true;
      this._recorder.AllowPitchAdjustment = true;
      this._recorder.PitchAdjustment = this.PitchAdjustment;
      this._recorder.AllowVolumeAdjustment = true;
      this._recorder.VolumeAdjustment = this.VolumeAdjustment;
      this._network.StartBroadcastSpeakerOutput(VRRigCache.Instance.localRig.Rig);
    }
  }

  public void StopLocalBroadcast()
  {
    if (!this._isLocal)
    {
      if (!((Object) this._network != (Object) null) || !((Object) this._nonlocalRig != (Object) null))
        return;
      this._network.StopBroadcastSpeakerOutput(this._nonlocalRig);
    }
    else
    {
      if (!this.IsBroadcasting)
        return;
      if ((Object) this._recorder == (Object) null && (Object) NetworkSystem.Instance.LocalRecorder != (Object) null)
        this.SetRecorder((GTRecorder) NetworkSystem.Instance.LocalRecorder);
      if (!((Object) this._recorder != (Object) null) || !((Object) this._network != (Object) null))
        return;
      this.IsBroadcasting = false;
      this._recorder.AllowPitchAdjustment = false;
      this._recorder.PitchAdjustment = 1f;
      this._recorder.AllowVolumeAdjustment = false;
      this._recorder.VolumeAdjustment = 1f;
      this._network.StopBroadcastSpeakerOutput(VRRigCache.Instance.localRig.Rig);
    }
  }
}
