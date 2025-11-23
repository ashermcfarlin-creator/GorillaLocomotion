// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.SO_NetworkVoiceSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using ExitGames.Client.Photon;
using Photon.Voice;
using Photon.Voice.Unity;
using POpusCodec.Enums;
using UnityEngine;

#nullable disable
namespace GorillaNetworking;

[CreateAssetMenu(fileName = "VoiceSettings", menuName = "Gorilla Tag/VoiceSettings")]
public class SO_NetworkVoiceSettings : ScriptableObject
{
  [Header("Voice settings")]
  public bool AutoConnectAndJoin = true;
  public bool AutoLeaveAndDisconnect = true;
  public bool WorkInOfflineMode = true;
  public DebugLevel LogLevel = DebugLevel.ERROR;
  public DebugLevel GlobalRecordersLogLevel = DebugLevel.INFO;
  public DebugLevel GlobalSpeakersLogLevel = DebugLevel.INFO;
  public bool CreateSpeakerIfNotFound;
  public int UpdateInterval = 50;
  public bool SupportLogger;
  public int BackgroundTimeout = 60000;
  [Header("Recorder Settings")]
  public bool RecordOnlyWhenEnabled;
  public bool RecordOnlyWhenJoined = true;
  public bool StopRecordingWhenPaused;
  public bool TransmitEnabled = true;
  public bool AutoStart = true;
  public bool Encrypt;
  public byte InterestGroup;
  public bool DebugEcho;
  public bool ReliableMode;
  [Header("Recorder Codec Parameters")]
  public OpusCodec.FrameDuration FrameDuration = OpusCodec.FrameDuration.Frame60ms;
  public SamplingRate SamplingRate = SamplingRate.Sampling16000;
  [Range(6000f, 510000f)]
  public int Bitrate = 20000;
  [Header("Recorder Audio Source Settings")]
  public Recorder.InputSourceType InputSourceType;
  public Recorder.MicType MicrophoneType;
  public bool UseFallback = true;
  public bool Detect = true;
  [Range(0.0f, 1f)]
  public float Threshold = 0.07f;
  public int Delay = 500;
}
