// Decompiled with JetBrains decompiler
// Type: GorillaTag.Audio.GTAudioOneShot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Audio;

internal static class GTAudioOneShot
{
  [OnEnterPlay_SetNull]
  internal static AudioSource audioSource;
  [OnEnterPlay_SetNull]
  internal static AnimationCurve defaultCurve;

  [field: OnEnterPlay_Set(false)]
  internal static bool isInitialized { get; private set; }

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  private static void Initialize()
  {
    if (GTAudioOneShot.isInitialized)
      return;
    AudioSource original = Resources.Load<AudioSource>("AudioSourceSingleton_Prefab");
    if ((Object) original == (Object) null)
    {
      Debug.LogError((object) "GTAudioOneShot: Failed to load AudioSourceSingleton_Prefab from resources!!!");
    }
    else
    {
      GTAudioOneShot.audioSource = Object.Instantiate<AudioSource>(original);
      GTAudioOneShot.defaultCurve = GTAudioOneShot.audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
      Object.DontDestroyOnLoad((Object) GTAudioOneShot.audioSource);
      GTAudioOneShot.isInitialized = true;
    }
  }

  internal static void Play(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
  {
    if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
      return;
    GTAudioOneShot.audioSource.pitch = pitch;
    GTAudioOneShot.audioSource.transform.position = position;
    GTAudioOneShot.audioSource.GTPlayOneShot(clip, volume);
  }

  internal static void Play(
    AudioClip clip,
    Vector3 position,
    AnimationCurve curve,
    float volume = 1f,
    float pitch = 1f)
  {
    if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
      return;
    GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
    GTAudioOneShot.Play(clip, position, volume, pitch);
    GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, GTAudioOneShot.defaultCurve);
  }
}
