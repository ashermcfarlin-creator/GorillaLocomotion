// Decompiled with JetBrains decompiler
// Type: GorillaTag.Audio.LoudSpeakerNetwork
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Photon.Voice.Unity;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag.Audio;

public class LoudSpeakerNetwork : MonoBehaviour
{
  [SerializeField]
  private AudioSource[] _speakerSources;
  [SerializeField]
  private List<Speaker> _currentSpeakers;
  [SerializeField]
  private int _currentSpeakerActor = -1;
  public bool ReparentLocalSpeaker = true;
  private RigContainer _rigContainer;
  private GTRecorder _localRecorder;

  public AudioSource[] SpeakerSources => this._speakerSources;

  private void Awake()
  {
    if (this._speakerSources == null || this._speakerSources.Length == 0)
      this._speakerSources = this.transform.GetComponentsInChildren<AudioSource>();
    this._currentSpeakers = new List<Speaker>();
  }

  private void Start()
  {
    RigContainer rigContainer;
    if (!this.GetParentRigContainer(out rigContainer) || !((Object) rigContainer.Voice != (Object) null))
      return;
    GTSpeaker speakerInUse = (GTSpeaker) rigContainer.Voice.SpeakerInUse;
    if (!((Object) speakerInUse != (Object) null))
      return;
    speakerInUse.AddExternalAudioSources(this._speakerSources);
  }

  private bool GetParentRigContainer(out RigContainer rigContainer)
  {
    if ((Object) this._rigContainer == (Object) null)
      this._rigContainer = this.transform.GetComponentInParent<RigContainer>();
    rigContainer = this._rigContainer;
    return (Object) rigContainer != (Object) null;
  }

  private void OnEnable()
  {
    RigContainer rigContainer;
    if (!this.GetParentRigContainer(out rigContainer))
      return;
    rigContainer.AddLoudSpeakerNetwork(this);
  }

  private void OnDisable()
  {
    RigContainer rigContainer;
    if (!this.GetParentRigContainer(out rigContainer))
      return;
    rigContainer.RemoveLoudSpeakerNetwork(this);
  }

  public void AddSpeaker(Speaker speaker)
  {
    if (this._currentSpeakers.Contains(speaker))
      return;
    this._currentSpeakers.Add(speaker);
  }

  public void RemoveSpeaker(Speaker speaker) => this._currentSpeakers.Remove(speaker);

  public void StartBroadcastSpeakerOutput(VRRig player)
  {
    GorillaTagger.Instance.rigSerializer.BroadcastLoudSpeakerNetwork(true, player.OwningNetPlayer.ActorNumber);
  }

  public void BroadcastLoudSpeakerNetwork(int actorNumber, bool isLocal = false)
  {
    if (isLocal)
    {
      if ((Object) this._localRecorder == (Object) null)
        this._localRecorder = (GTRecorder) NetworkSystem.Instance.LocalRecorder;
      if (!((Object) this._localRecorder != (Object) null))
        return;
      this._localRecorder.DebugEchoMode = true;
      if (!this.ReparentLocalSpeaker)
        return;
      Transform transform = this._rigContainer.Voice.SpeakerInUse.transform;
      transform.transform.SetParent(this.transform, false);
      transform.localPosition = Vector3.zero;
    }
    else
    {
      using (List<Speaker>.Enumerator enumerator = this._currentSpeakers.GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          GTSpeaker current = (GTSpeaker) enumerator.Current;
          current.ToggleAudioSource(true);
          current.BroadcastExternal = true;
          RigContainer playerRig;
          if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out playerRig))
          {
            Transform transform = playerRig.Voice.SpeakerInUse.transform;
            transform.SetParent(this.transform, false);
            transform.localPosition = Vector3.zero;
          }
        }
      }
      this._currentSpeakerActor = actorNumber;
    }
  }

  public void StopBroadcastSpeakerOutput(VRRig player)
  {
    GorillaTagger.Instance.rigSerializer.BroadcastLoudSpeakerNetwork(false, player.OwningNetPlayer.ActorNumber);
  }

  public void StopBroadcastLoudSpeakerNetwork(int actorNumber, bool isLocal = false)
  {
    if (isLocal)
    {
      if ((Object) this._localRecorder == (Object) null)
        this._localRecorder = (GTRecorder) NetworkSystem.Instance.LocalRecorder;
      if (!((Object) this._localRecorder != (Object) null))
        return;
      this._localRecorder.DebugEchoMode = false;
      RigContainer rigContainer;
      if (!this.ReparentLocalSpeaker || !this.GetParentRigContainer(out rigContainer))
        return;
      Transform transform = rigContainer.Voice.SpeakerInUse.transform;
      transform.SetParent(rigContainer.SpeakerHead, false);
      transform.localPosition = Vector3.zero;
    }
    else
    {
      if (actorNumber != this._currentSpeakerActor)
        return;
      using (List<Speaker>.Enumerator enumerator = this._currentSpeakers.GetEnumerator())
      {
        if (enumerator.MoveNext())
        {
          GTSpeaker current = (GTSpeaker) enumerator.Current;
          current.ToggleAudioSource(false);
          current.BroadcastExternal = false;
          RigContainer playerRig;
          if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out playerRig))
          {
            Transform transform = playerRig.Voice.SpeakerInUse.transform;
            transform.SetParent(playerRig.SpeakerHead, false);
            transform.localPosition = Vector3.zero;
          }
        }
      }
      this._currentSpeakerActor = -1;
    }
  }
}
