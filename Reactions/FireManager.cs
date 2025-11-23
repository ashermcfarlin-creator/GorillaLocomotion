// Decompiled with JetBrains decompiler
// Type: GorillaTag.Reactions.FireManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTag.Audio;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable disable
namespace GorillaTag.Reactions;

public class FireManager : ITickSystemPost
{
  [OnEnterPlay_Clear]
  private static readonly Dictionary<int, FireInstance> _kGObjInstId_to_fire = new Dictionary<int, FireInstance>(256 /*0x0100*/);
  [OnEnterPlay_Clear]
  private static readonly List<FireInstance> _kEnabledReactions = new List<FireInstance>(256 /*0x0100*/);
  [OnEnterPlay_Clear]
  private static readonly List<FireInstance> _kFiresToDespawn = new List<FireInstance>(256 /*0x0100*/);
  [OnEnterPlay_Clear]
  private static readonly Dictionary<Vector3Int, int> _fireSpatialGrid = new Dictionary<Vector3Int, int>(256 /*0x0100*/);
  private const float _kSpatialGridCellSize = 0.2f;
  private const int _kMaxAudioSources = 8;
  [OnEnterPlay_Set(0)]
  private static int _activeAudioSources;
  private static readonly int shaderProp_EmissionColor = ShaderProps._EmissionColor;

  [field: OnEnterPlay_SetNull]
  internal static FireManager instance { get; private set; }

  [field: OnEnterPlay_Set(false)]
  internal static bool hasInstance { get; private set; }

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
  private static void Initialize()
  {
    if (ApplicationQuittingState.IsQuitting || FireManager.hasInstance)
      return;
    FireManager.instance = new FireManager();
    FireManager.hasInstance = true;
    TickSystem<object>.AddPostTickCallback((ITickSystemPost) FireManager.instance);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static void Register(FireInstance f)
  {
    if (ApplicationQuittingState.IsQuitting)
      return;
    int instanceId = f.gameObject.GetInstanceID();
    if (!FireManager._kGObjInstId_to_fire.TryAdd(instanceId, f))
    {
      if ((Object) f == (Object) null)
      {
        Debug.LogError((object) "FireManager: You tried to register null!", (Object) f);
        return;
      }
      Debug.LogError((object) $"FireManager: \"{f.name}\" was attempted to be registered more than once!", (Object) f);
    }
    f.GetComponentAndSetFieldIfNullElseLogAndDisable<Collider>(ref f._collider, "_collider", "Collider", caller: nameof (Register));
    f.GetComponentAndSetFieldIfNullElseLogAndDisable<ThermalSourceVolume>(ref f._thermalVolume, "_thermalVolume", "ThermalSourceVolume", caller: nameof (Register));
    f.GetComponentAndSetFieldIfNullElseLogAndDisable<ParticleSystem>(ref f._particleSystem, "_particleSystem", "ParticleSystem", caller: nameof (Register));
    f.GetComponentAndSetFieldIfNullElseLogAndDisable<AudioSource>(ref f._loopingAudioSource, "_loopingAudioSource", "AudioSource", caller: nameof (Register));
    f.DisableIfNull<AudioClip>(f._extinguishSound.obj, "_extinguishSound", "AudioClip", nameof (Register));
    f.DisableIfNull<AudioClip>(f._igniteSound.obj, "_igniteSound", "AudioClip", nameof (Register));
    f._defaultTemperature = f._thermalVolume.celsius;
    f._timeSinceExtinguished = -f._stayExtinguishedDuration;
    f._psEmissionModule = f._particleSystem.emission;
    f._psDefaultEmissionRate = f._psEmissionModule.rateOverTime.constant;
    f._deathStateDuration = 0.0f;
    if (f._emissiveRenderers == null)
      return;
    f._emiRenderers_matPropBlocks = new MaterialPropertyBlock[f._emissiveRenderers.Length];
    f._emiRenderers_defaultColors = new Color[f._emissiveRenderers.Length];
    for (int index = 0; index < f._emissiveRenderers.Length; ++index)
    {
      f._emiRenderers_matPropBlocks[index] = new MaterialPropertyBlock();
      f._emissiveRenderers[index].GetPropertyBlock(f._emiRenderers_matPropBlocks[index]);
      f._emiRenderers_defaultColors[index] = f._emiRenderers_matPropBlocks[index].GetColor(FireManager.shaderProp_EmissionColor);
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static void Unregister(FireInstance reactable)
  {
    if (ApplicationQuittingState.IsQuitting)
      return;
    int instanceId = reactable.gameObject.GetInstanceID();
    FireManager._kGObjInstId_to_fire.Remove(instanceId);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static Vector3Int GetSpatialGridPos(Vector3 pos)
  {
    Vector3 vector3 = pos / 0.2f;
    return new Vector3Int((int) vector3.x, (int) vector3.y, (int) vector3.z);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static void ResetFireValues(FireInstance f)
  {
    f._timeSinceExtinguished = Mathf.Min(f._timeSinceExtinguished, f._stayExtinguishedDuration);
    f._timeSinceDyingStart = 0.0f;
    f._isDespawning = false;
    f._timeAlive = 0.0f;
    f._thermalVolume.celsius = f._defaultTemperature;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static void SpawnFire(SinglePool pool, Vector3 pos, Vector3 normal, float scale)
  {
    int key;
    if (FireManager._fireSpatialGrid.TryGetValue(FireManager.GetSpatialGridPos(pos), out key))
    {
      FireManager.ResetFireValues(FireManager._kGObjInstId_to_fire[key]);
    }
    else
    {
      GameObject gameObject = pool.Instantiate(false);
      gameObject.transform.position = pos;
      gameObject.transform.up = normal;
      gameObject.transform.localScale = Vector3.one * scale;
      gameObject.SetActive(true);
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static void OnEnable(FireInstance f)
  {
    if (ApplicationQuittingState.IsQuitting || (Object) ObjectPools.instance == (Object) null || !ObjectPools.instance.initialized)
      return;
    FireManager.ResetFireValues(f);
    f._spatialGridPosition = FireManager.GetSpatialGridPos(f.transform.position);
    FireManager._fireSpatialGrid.Add(f._spatialGridPosition, f.gameObject.GetInstanceID());
    FireManager._kEnabledReactions.Add(f);
    if (GTAudioOneShot.isInitialized && (double) Time.realtimeSinceStartup > 10.0)
      GTAudioOneShot.Play((AudioClip) f._igniteSound, f.transform.position, f._igniteSoundVolume);
    if (8 > FireManager._activeAudioSources)
    {
      ++FireManager._activeAudioSources;
      f._loopingAudioSource.enabled = true;
    }
    else
      f._loopingAudioSource.enabled = false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static void OnDisable(FireInstance f)
  {
    if (ApplicationQuittingState.IsQuitting)
      return;
    FireManager._kEnabledReactions.Remove(f);
    FireManager._fireSpatialGrid.Remove(f._spatialGridPosition);
    FireManager._activeAudioSources = Mathf.Min(FireManager._activeAudioSources - (f._loopingAudioSource.enabled ? 1 : 0), 0);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static void OnTriggerEnter(FireInstance f, Collider other)
  {
    if (f._isDespawning || ApplicationQuittingState.IsQuitting || other.gameObject.layer != 4)
      return;
    FireManager.Extinguish(f.gameObject, float.MaxValue);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static void Extinguish(GameObject gObj, float extinguishAmount)
  {
    FireInstance fireInstance;
    if (ApplicationQuittingState.IsQuitting || !FireManager._kGObjInstId_to_fire.TryGetValue(gObj.GetInstanceID(), out fireInstance))
      return;
    float a = fireInstance._thermalVolume.celsius - extinguishAmount;
    if ((double) a > 0.0 || (double) fireInstance._thermalVolume.celsius <= 1.0 / 1000.0)
      return;
    fireInstance._thermalVolume.celsius = Mathf.Max(a, 0.0f);
    fireInstance._timeSinceExtinguished = 0.0f;
    GTAudioOneShot.Play((AudioClip) fireInstance._extinguishSound, fireInstance.transform.position, fireInstance._extinguishSoundVolume);
    if (!fireInstance._despawnOnExtinguish)
      return;
    fireInstance._isDespawning = true;
    fireInstance._timeSinceDyingStart = 0.0f;
  }

  bool ITickSystemPost.PostTickRunning { get; set; }

  void ITickSystemPost.PostTick()
  {
    if (ApplicationQuittingState.IsQuitting)
      return;
    foreach (FireInstance kEnabledReaction in FireManager._kEnabledReactions)
    {
      kEnabledReaction._timeAlive += Time.unscaledDeltaTime;
      bool flag1 = (double) kEnabledReaction._timeSinceExtinguished < (double) kEnabledReaction._stayExtinguishedDuration;
      kEnabledReaction._timeSinceExtinguished += Time.unscaledDeltaTime;
      bool flag2 = (double) kEnabledReaction._timeSinceExtinguished < (double) kEnabledReaction._stayExtinguishedDuration;
      if (kEnabledReaction._isDespawning)
      {
        kEnabledReaction._timeSinceDyingStart += Time.unscaledDeltaTime;
        if ((double) kEnabledReaction._timeSinceDyingStart >= (double) kEnabledReaction._deathStateDuration || (double) kEnabledReaction._thermalVolume.celsius < -9999.0)
          FireManager._kFiresToDespawn.Add(kEnabledReaction);
      }
      if (!kEnabledReaction._isDespawning && kEnabledReaction._despawnOnExtinguish && (double) kEnabledReaction._timeAlive > (double) kEnabledReaction._maxLifetime)
      {
        kEnabledReaction._isDespawning = true;
        kEnabledReaction._timeSinceDyingStart = 0.0f;
        GTAudioOneShot.Play((AudioClip) kEnabledReaction._extinguishSound, kEnabledReaction.transform.position, kEnabledReaction._extinguishSoundVolume);
      }
      if (!kEnabledReaction._isDespawning && flag1 != flag2)
      {
        if (flag2)
        {
          if (kEnabledReaction._despawnOnExtinguish)
          {
            kEnabledReaction._isDespawning = true;
            kEnabledReaction._timeSinceDyingStart = 0.0f;
          }
          GTAudioOneShot.Play((AudioClip) kEnabledReaction._extinguishSound, kEnabledReaction.transform.position, kEnabledReaction._extinguishSoundVolume);
        }
        else
          GTAudioOneShot.Play((AudioClip) kEnabledReaction._igniteSound, kEnabledReaction.transform.position, kEnabledReaction._igniteSoundVolume);
      }
      float num1 = kEnabledReaction._thermalVolume.celsius + kEnabledReaction._reheatSpeed * Time.unscaledDeltaTime;
      if (kEnabledReaction._isDespawning)
        num1 = (double) kEnabledReaction._deathStateDuration > 0.0 ? Mathf.Lerp(kEnabledReaction._thermalVolume.celsius, 0.0f, kEnabledReaction._timeSinceDyingStart / kEnabledReaction._deathStateDuration) : 0.0f;
      float num2 = (double) num1 > (double) kEnabledReaction._defaultTemperature ? kEnabledReaction._defaultTemperature : num1;
      kEnabledReaction._thermalVolume.celsius = num2;
      float num3 = num2 / kEnabledReaction._defaultTemperature;
      kEnabledReaction._loopingAudioSource.volume = num3;
      for (int index = 0; index < kEnabledReaction._emissiveRenderers.Length; ++index)
        kEnabledReaction._emiRenderers_matPropBlocks[index].SetColor(FireManager.shaderProp_EmissionColor, kEnabledReaction._emiRenderers_defaultColors[index] * num3);
    }
    foreach (FireInstance fireInstance in FireManager._kFiresToDespawn)
      ObjectPools.instance.Destroy(fireInstance.gameObject);
    FireManager._kFiresToDespawn.Clear();
  }
}
