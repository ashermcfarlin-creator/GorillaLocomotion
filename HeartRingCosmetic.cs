// Decompiled with JetBrains decompiler
// Type: GorillaTag.HeartRingCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag;

[DefaultExecutionOrder(1250)]
public class HeartRingCosmetic : MonoBehaviour
{
  public GameObject effects;
  [SerializeField]
  private bool isHauntedVoiceChanger;
  [SerializeField]
  private float hauntedVoicePitch = 0.75f;
  [AssignInCorePrefab]
  public float effectActivationRadius = 0.15f;
  private readonly Vector3 headToMouthOffset = new Vector3(0.0f, 13f / 625f, 0.171f);
  private VRRig ownerRig;
  private Transform ownerHead;
  private ParticleSystem particleSystem;
  private AudioSource audioSource;
  private float maxEmissionRate;
  private float maxVolume;
  private const float emissionFadeTime = 0.1f;
  private const float volumeFadeTime = 2f;

  protected void Awake() => Application.quitting += (Action) (() => this.enabled = false);

  protected void OnEnable()
  {
    this.particleSystem = this.effects.GetComponentInChildren<ParticleSystem>(true);
    this.audioSource = this.effects.GetComponentInChildren<AudioSource>(true);
    this.ownerRig = this.GetComponentInParent<VRRig>();
    bool flag = (UnityEngine.Object) this.ownerRig != (UnityEngine.Object) null && this.ownerRig.head != null && (UnityEngine.Object) this.ownerRig.head.rigTarget != (UnityEngine.Object) null;
    this.enabled = flag;
    this.effects.SetActive(flag);
    if (!flag)
    {
      Debug.LogError((object) ("Disabling HeartRingCosmetic. Could not find owner head. Scene path: " + this.transform.GetPath()), (UnityEngine.Object) this);
    }
    else
    {
      this.ownerHead = (UnityEngine.Object) this.ownerRig != (UnityEngine.Object) null ? this.ownerRig.head.rigTarget.transform : this.transform;
      this.maxEmissionRate = this.particleSystem.emission.rateOverTime.constant;
      this.maxVolume = this.audioSource.volume;
    }
  }

  protected void LateUpdate()
  {
    Transform transform = this.transform;
    Vector3 position = transform.position;
    float x = transform.lossyScale.x;
    float num = this.effectActivationRadius * this.effectActivationRadius * x * x;
    bool flag = (double) (this.ownerHead.TransformPoint(this.headToMouthOffset) - position).sqrMagnitude < (double) num;
    ParticleSystem.EmissionModule emission = this.particleSystem.emission;
    emission.rateOverTime = (ParticleSystem.MinMaxCurve) Mathf.Lerp(emission.rateOverTime.constant, flag ? this.maxEmissionRate : 0.0f, Time.deltaTime / 0.1f);
    this.audioSource.volume = Mathf.Lerp(this.audioSource.volume, flag ? this.maxVolume : 0.0f, Time.deltaTime / 2f);
    this.ownerRig.UsingHauntedRing = this.isHauntedVoiceChanger & flag;
    if (!this.ownerRig.UsingHauntedRing)
      return;
    this.ownerRig.HauntedRingVoicePitch = this.hauntedVoicePitch;
  }
}
