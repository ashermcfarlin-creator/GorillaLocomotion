// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.CosmeticEffectsOnPlayers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag.Cosmetics;

public class CosmeticEffectsOnPlayers : MonoBehaviour, ISpawnable
{
  public CosmeticEffectsOnPlayers.CosmeticEffect[] allEffects = new CosmeticEffectsOnPlayers.CosmeticEffect[0];
  private VRRig myRig;
  private Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> allEffectsDict = new Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>();

  private bool ShouldAffectRig(VRRig rig, CosmeticEffectsOnPlayers.TargetType target)
  {
    bool flag1 = (UnityEngine.Object) rig == (UnityEngine.Object) this.myRig;
    bool flag2;
    switch (target)
    {
      case CosmeticEffectsOnPlayers.TargetType.Owner:
        flag2 = flag1;
        break;
      case CosmeticEffectsOnPlayers.TargetType.Others:
        flag2 = !flag1;
        break;
      case CosmeticEffectsOnPlayers.TargetType.All:
        flag2 = true;
        break;
      default:
        flag2 = false;
        break;
    }
    return flag2;
  }

  private void Awake()
  {
    foreach (CosmeticEffectsOnPlayers.CosmeticEffect allEffect in this.allEffects)
      this.allEffectsDict.TryAdd(allEffect.effectType, allEffect);
  }

  public void SetKnockbackStrengthMultiplier(float value)
  {
    foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> keyValuePair in this.allEffectsDict)
      keyValuePair.Value.knockbackStrengthMultiplier = value;
  }

  public void ApplyAllEffects() => this.ApplyAllEffectsByDistance(this.transform.position);

  public void ApplyAllEffectsByDistance(Transform _transform)
  {
    this.ApplyAllEffectsByDistance(_transform.position);
  }

  public void ApplyAllEffectsByDistance(Vector3 position)
  {
    foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in this.allEffectsDict)
    {
      switch (effect.Key)
      {
        case CosmeticEffectsOnPlayers.EFFECTTYPE.Skin:
          this.ApplySkinByDistance(effect, position);
          continue;
        case CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback:
          this.ApplyTagWithKnockbackByDistance(effect, position);
          continue;
        case CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback:
          this.ApplyInstantKnockbackByDistance(effect, position);
          continue;
        case CosmeticEffectsOnPlayers.EFFECTTYPE.SFX:
          this.PlaySfxByDistance(effect, position);
          continue;
        case CosmeticEffectsOnPlayers.EFFECTTYPE.VFX:
          this.PlayVFXByDistance(effect, position);
          continue;
        default:
          continue;
      }
    }
  }

  public void ApplyAllEffectsForRig(VRRig rig)
  {
    foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in this.allEffectsDict)
    {
      switch (effect.Key)
      {
        case CosmeticEffectsOnPlayers.EFFECTTYPE.Skin:
          this.ApplySkinForRig(effect, rig);
          continue;
        case CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback:
          this.ApplyTagWithKnockbackForRig(effect, rig);
          continue;
        case CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback:
          this.ApplyInstantKnockbackForRig(effect, rig);
          continue;
        case CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride:
          this.ApplyVOForRig(effect, rig);
          continue;
        case CosmeticEffectsOnPlayers.EFFECTTYPE.SFX:
          this.PlaySfxForRig(effect, rig);
          continue;
        case CosmeticEffectsOnPlayers.EFFECTTYPE.VFX:
          this.PlayVFXForRig(effect, rig);
          continue;
        default:
          continue;
      }
    }
  }

  private void ApplySkinByDistance(
    KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect,
    Vector3 position)
  {
    if (!effect.Value.IsGameModeAllowed())
      return;
    effect.Value.EffectStartedTime = Time.time;
    IEnumerable<VRRig> vrRigs;
    if (!PhotonNetwork.InRoom)
      vrRigs = (IEnumerable<VRRig>) new VRRig[1]
      {
        GorillaTagger.Instance.offlineVRRig
      };
    else
      vrRigs = (IEnumerable<VRRig>) GorillaParent.instance.vrrigs;
    foreach (VRRig rig in vrRigs)
    {
      if (this.ShouldAffectRig(rig, effect.Value.target) && (rig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
      {
        if ((UnityEngine.Object) rig == (UnityEngine.Object) this.myRig)
          effect.Value.EffectDuration = effect.Value.effectDurationOwner;
        rig.SpawnSkinEffects(effect);
      }
    }
  }

  private void ApplySkinForRig(
    KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect,
    VRRig vrRig)
  {
    if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
      return;
    effect.Value.EffectStartedTime = Time.time;
    if ((UnityEngine.Object) vrRig == (UnityEngine.Object) this.myRig)
      effect.Value.EffectDuration = effect.Value.effectDurationOwner;
    vrRig.SpawnSkinEffects(effect);
  }

  private void ApplyTagWithKnockbackForRig(
    KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect,
    VRRig vrRig)
  {
    if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
      return;
    effect.Value.EffectStartedTime = Time.time;
    if ((UnityEngine.Object) vrRig == (UnityEngine.Object) this.myRig)
      effect.Value.EffectDuration = effect.Value.effectDurationOwner;
    vrRig.EnableHitWithKnockBack(effect);
  }

  private void ApplyTagWithKnockbackByDistance(
    KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect,
    Vector3 position)
  {
    if (!effect.Value.IsGameModeAllowed())
      return;
    effect.Value.EffectStartedTime = Time.time;
    IEnumerable<VRRig> vrRigs;
    if (!PhotonNetwork.InRoom)
      vrRigs = (IEnumerable<VRRig>) new VRRig[1]
      {
        GorillaTagger.Instance.offlineVRRig
      };
    else
      vrRigs = (IEnumerable<VRRig>) GorillaParent.instance.vrrigs;
    foreach (VRRig rig in vrRigs)
    {
      if (this.ShouldAffectRig(rig, effect.Value.target) && (rig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
      {
        if ((UnityEngine.Object) rig == (UnityEngine.Object) this.myRig)
          effect.Value.EffectDuration = effect.Value.effectDurationOwner;
        rig.EnableHitWithKnockBack(effect);
      }
    }
  }

  private void ApplyInstantKnockbackForRig(
    KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect,
    VRRig vrRig)
  {
    if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
      return;
    effect.Value.EffectStartedTime = Time.time;
    if ((UnityEngine.Object) vrRig == (UnityEngine.Object) this.myRig)
      effect.Value.EffectDuration = effect.Value.effectDurationOwner;
    Vector3 vector3 = vrRig.transform.position - this.transform.position;
    float strength = (1f / vector3.magnitude * effect.Value.knockbackStrength * effect.Value.knockbackStrengthMultiplier).ClampSafe(effect.Value.minKnockbackStrength, effect.Value.maxKnockbackStrength);
    if (effect.Value.applyScaleToKnockbackStrength)
      strength *= vrRig.scaleFactor;
    RoomSystem.HitPlayer(vrRig.creator, vector3.normalized, strength);
    vrRig.ApplyInstanceKnockBack(effect);
  }

  private void ApplyInstantKnockbackByDistance(
    KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect,
    Vector3 position)
  {
    if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(GorillaTagger.Instance.offlineVRRig, effect.Value.target))
      return;
    effect.Value.EffectStartedTime = Time.time;
    if ((UnityEngine.Object) GorillaTagger.Instance.offlineVRRig == (UnityEngine.Object) this.myRig)
      effect.Value.EffectDuration = effect.Value.effectDurationOwner;
    Vector3 vector3_1 = GorillaTagger.Instance.offlineVRRig.transform.position - position;
    if (!vector3_1.IsShorterThan(effect.Value.effectDistanceRadius))
      return;
    float magnitude = vector3_1.magnitude;
    GTPlayer instance = GTPlayer.Instance;
    if (effect.Value.specialVerticalForce && (instance.IsHandTouching(true) || instance.IsHandTouching(false) || instance.BodyOnGround))
    {
      Vector3 vector3_2 = -Physics.gravity.normalized;
      Vector3 vector3_3 = Vector3.ProjectOnPlane(vector3_1, vector3_2);
      vector3_1 = ((double) Vector3.Dot(vector3_1 / magnitude, vector3_2) > 0.0 ? vector3_1 : vector3_3) + vector3_3.magnitude * vector3_2;
    }
    float speed = (effect.Value.knockbackStrength * effect.Value.knockbackStrengthMultiplier / magnitude).ClampSafe(effect.Value.minKnockbackStrength, effect.Value.maxKnockbackStrength);
    if (effect.Value.applyScaleToKnockbackStrength)
      speed *= instance.scale;
    instance.ApplyKnockback(vector3_1.normalized, speed, effect.Value.forceOffTheGround);
    GorillaTagger.Instance.offlineVRRig.ApplyInstanceKnockBack(effect);
  }

  private void ApplyVOForRig(
    KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect,
    VRRig rig)
  {
    if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(rig, effect.Value.target))
      return;
    effect.Value.EffectStartedTime = Time.time;
    if ((UnityEngine.Object) rig == (UnityEngine.Object) this.myRig)
      effect.Value.EffectDuration = effect.Value.effectDurationOwner;
    rig.ActivateVOEffect(effect);
  }

  private void PlaySfxForRig(
    KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect,
    VRRig vrRig)
  {
    if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
      return;
    effect.Value.EffectStartedTime = Time.time;
    if ((UnityEngine.Object) vrRig == (UnityEngine.Object) this.myRig)
      effect.Value.EffectDuration = effect.Value.effectDurationOwner;
    vrRig.PlayCosmeticEffectSFX(effect);
  }

  private void PlaySfxByDistance(
    KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect,
    Vector3 position)
  {
    if (!effect.Value.IsGameModeAllowed())
      return;
    effect.Value.EffectStartedTime = Time.time;
    IEnumerable<VRRig> vrRigs;
    if (!PhotonNetwork.InRoom)
      vrRigs = (IEnumerable<VRRig>) new VRRig[1]
      {
        GorillaTagger.Instance.offlineVRRig
      };
    else
      vrRigs = (IEnumerable<VRRig>) GorillaParent.instance.vrrigs;
    foreach (VRRig rig in vrRigs)
    {
      if (this.ShouldAffectRig(rig, effect.Value.target) && (rig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
      {
        if ((UnityEngine.Object) rig == (UnityEngine.Object) this.myRig)
          effect.Value.EffectDuration = effect.Value.effectDurationOwner;
        rig.PlayCosmeticEffectSFX(effect);
      }
    }
  }

  private void PlayVFXForRig(
    KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect,
    VRRig vrRig)
  {
    if (!effect.Value.IsGameModeAllowed() || !this.ShouldAffectRig(vrRig, effect.Value.target))
      return;
    effect.Value.EffectStartedTime = Time.time;
    if ((UnityEngine.Object) vrRig == (UnityEngine.Object) this.myRig)
      effect.Value.EffectDuration = effect.Value.effectDurationOwner;
    vrRig.SpawnVFXEffect(effect);
  }

  private void PlayVFXByDistance(
    KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect,
    Vector3 position)
  {
    if (!effect.Value.IsGameModeAllowed())
      return;
    effect.Value.EffectStartedTime = Time.time;
    IEnumerable<VRRig> vrRigs;
    if (!PhotonNetwork.InRoom)
      vrRigs = (IEnumerable<VRRig>) new VRRig[1]
      {
        GorillaTagger.Instance.offlineVRRig
      };
    else
      vrRigs = (IEnumerable<VRRig>) GorillaParent.instance.vrrigs;
    foreach (VRRig rig in vrRigs)
    {
      if (this.ShouldAffectRig(rig, effect.Value.target) && (rig.transform.position - position).IsShorterThan(effect.Value.effectDistanceRadius))
      {
        if ((UnityEngine.Object) rig == (UnityEngine.Object) this.myRig)
          effect.Value.EffectDuration = effect.Value.effectDurationOwner;
        rig.SpawnVFXEffect(effect);
      }
    }
  }

  public bool IsSpawned { get; set; }

  public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

  public void OnSpawn(VRRig rig) => this.myRig = rig;

  public void OnDespawn()
  {
  }

  [Serializable]
  public enum TargetType
  {
    Owner,
    Others,
    All,
  }

  [Serializable]
  public class CosmeticEffect
  {
    public GameModeType[] excludeForGameModes;
    public CosmeticEffectsOnPlayers.EFFECTTYPE effectType;
    public float effectDistanceRadius;
    public CosmeticEffectsOnPlayers.TargetType target = CosmeticEffectsOnPlayers.TargetType.All;
    public float effectDurationOthers;
    public float effectDurationOwner;
    public GorillaSkin newSkin;
    [Tooltip("Use object pools")]
    public GameObject knockbackVFX;
    [FormerlySerializedAs("knockbackStrengthMultiplier")]
    public float knockbackStrength;
    public bool applyScaleToKnockbackStrength;
    [Tooltip("force pushing players with hands on the ground")]
    public bool forceOffTheGround;
    [Tooltip("Take the horizontal magnitude of the knockback, and add it opposite gravity. For example, being hit sideways will also impart a large upwards force. Breaks conservation of energy, but feels better to the player.")]
    public bool specialVerticalForce;
    [FormerlySerializedAs("minStrengthClamp")]
    public float minKnockbackStrength = 0.5f;
    [FormerlySerializedAs("maxStrengthClamp")]
    public float maxKnockbackStrength = 6f;
    public AudioClip[] voiceOverrideNormalClips;
    public AudioClip[] voiceOverrideLoudClips;
    public float voiceOverrideNormalVolume = 0.5f;
    public float voiceOverrideLoudVolume = 0.8f;
    public float voiceOverrideLoudThreshold = 0.175f;
    [Tooltip("plays sfx on player")]
    public List<AudioClip> sfxAudioClip;
    [Tooltip("plays vfx on player, must be in the global object pool and have a tag.")]
    public GameObject VFXGameObject;
    private HashSet<GameModeType> modesHash;

    public float knockbackStrengthMultiplier { get; set; }

    public bool IsGameModeAllowed()
    {
      return !((IEnumerable<GameModeType>) this.excludeForGameModes).Contains<GameModeType>((UnityEngine.Object) GameMode.ActiveGameMode != (UnityEngine.Object) null ? GameMode.ActiveGameMode.GameType() : GameModeType.Casual);
    }

    public float EffectDuration
    {
      get => this.effectDurationOthers;
      set => this.effectDurationOthers = value;
    }

    public float EffectStartedTime { get; set; }

    private bool IsSkin() => this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.Skin;

    private bool IsTagKnockback()
    {
      return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback;
    }

    private bool IsInstantKnockback()
    {
      return this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback;
    }

    private bool HasKnockback()
    {
      CosmeticEffectsOnPlayers.EFFECTTYPE effectType = this.effectType;
      return (effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback ? 0 : (effectType != CosmeticEffectsOnPlayers.EFFECTTYPE.InstantKnockback ? 1 : 0)) == 0;
    }

    private bool IsVO() => this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride;

    private bool IsSFX() => this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.SFX;

    private bool IsVFX() => this.effectType == CosmeticEffectsOnPlayers.EFFECTTYPE.VFX;

    private HashSet<GameModeType> Modes
    {
      get
      {
        if (this.modesHash == null)
          this.modesHash = new HashSet<GameModeType>((IEnumerable<GameModeType>) this.excludeForGameModes);
        return this.modesHash;
      }
    }
  }

  public enum EFFECTTYPE
  {
    Skin = 0,
    [Obsolete("FPV has been removed, do not use, use Stick Object To Player instead")] TagWithKnockback = 2,
    InstantKnockback = 3,
    VoiceOverride = 4,
    SFX = 5,
    VFX = 6,
  }
}
