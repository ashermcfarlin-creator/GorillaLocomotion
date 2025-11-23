// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.RCDragon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

#nullable disable
namespace GorillaTag.Cosmetics;

public class RCDragon : RCVehicle
{
  [SerializeField]
  private float maxAscendSpeed = 6f;
  [SerializeField]
  private float ascendAccelTime = 3f;
  [SerializeField]
  private float ascendWhileFlyingAccelBoost;
  [SerializeField]
  private float gravityCompensation = 0.9f;
  [SerializeField]
  private float crashedGravityCompensation = 0.5f;
  [SerializeField]
  private float maxTurnRate = 90f;
  [SerializeField]
  private float turnAccelTime = 0.75f;
  [SerializeField]
  private float maxHorizontalSpeed = 6f;
  [SerializeField]
  private float horizontalAccelTime = 2f;
  [SerializeField]
  private float maxHorizontalTiltAngle = 45f;
  [SerializeField]
  private float horizontalTiltTime = 2f;
  [SerializeField]
  private Vector2 motorSoundVolumeMinMax = new Vector2(0.1f, 0.8f);
  [SerializeField]
  private float crashSoundVolume = 0.1f;
  [SerializeField]
  private float breathFireVolume = 0.5f;
  [SerializeField]
  private float wingFlapVolume = 0.1f;
  [SerializeField]
  private Animation animation;
  [SerializeField]
  private string wingFlapAnimName;
  [SerializeField]
  private float wingFlapAnimSpeed = 1f;
  [SerializeField]
  private string dockedAnimName;
  [SerializeField]
  private string idleAnimName;
  [SerializeField]
  private string crashAnimName;
  [SerializeField]
  private float crashAnimSpeed = 1f;
  [SerializeField]
  private string mouthClosedAnimName;
  [SerializeField]
  private string mouthBreathFireAnimName;
  private bool shouldFlap;
  private bool isFlapping;
  private float nextFlapEventAnimTime;
  [SerializeField]
  private float flapAnimEventTime = 0.25f;
  [SerializeField]
  private GameObject fireBreath;
  [SerializeField]
  private float fireBreathDuration;
  private float fireBreathTimeRemaining;
  [SerializeField]
  private Collider crashCollider;
  [SerializeField]
  private AudioSource audioSource;
  [SerializeField]
  private List<AudioClip> breathFireSound;
  [SerializeField]
  private List<AudioClip> wingFlapSound;
  [SerializeField]
  private AudioClip crashSound;
  private float turnRate;
  private float turnAngle;
  private float tiltAngle;
  private float ascendAccel;
  private float turnAccel;
  private float tiltAccel;
  private float horizontalAccel;
  private float motorVolumeRampTime = 1f;
  private float motorLevel;

  protected override void AuthorityBeginDocked()
  {
    base.AuthorityBeginDocked();
    this.turnRate = 0.0f;
    this.turnAngle = Vector3.SignedAngle(Vector3.forward, Vector3.ProjectOnPlane(this.transform.forward, Vector3.up), Vector3.up);
    this.motorLevel = 0.0f;
    if (!((Object) this.connectedRemote == (Object) null))
      return;
    this.gameObject.SetActive(false);
  }

  protected override void Awake()
  {
    base.Awake();
    this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
    this.turnAccel = this.maxTurnRate / this.turnAccelTime;
    this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
    this.tiltAccel = this.maxHorizontalTiltAngle / this.horizontalTiltTime;
    this.shouldFlap = false;
    this.isFlapping = false;
    this.StopBreathFire();
    if ((Object) this.animation != (Object) null)
    {
      this.animation[this.wingFlapAnimName].speed = this.wingFlapAnimSpeed;
      this.animation[this.crashAnimName].speed = this.crashAnimSpeed;
      this.animation[this.mouthClosedAnimName].layer = 1;
      this.animation[this.mouthBreathFireAnimName].layer = 1;
    }
    this.nextFlapEventAnimTime = this.flapAnimEventTime;
  }

  protected override void OnDisable()
  {
    base.OnDisable();
    this.audioSource.GTStop();
  }

  public void StartBreathFire()
  {
    if (!string.IsNullOrEmpty(this.mouthBreathFireAnimName))
      this.animation.CrossFade(this.mouthBreathFireAnimName, 0.1f);
    if ((Object) this.fireBreath != (Object) null)
      this.fireBreath.SetActive(true);
    this.PlayRandomSound(this.breathFireSound, this.breathFireVolume);
    this.fireBreathTimeRemaining = this.fireBreathDuration;
  }

  public void StopBreathFire()
  {
    if (!string.IsNullOrEmpty(this.mouthClosedAnimName))
      this.animation.CrossFade(this.mouthClosedAnimName, 0.1f);
    if ((Object) this.fireBreath != (Object) null)
      this.fireBreath.SetActive(false);
    this.fireBreathTimeRemaining = -1f;
  }

  public bool IsBreathingFire() => (double) this.fireBreathTimeRemaining >= 0.0;

  private void PlayRandomSound(List<AudioClip> clips, float volume)
  {
    if (clips == null || clips.Count == 0)
      return;
    this.PlaySound(clips[Random.Range(0, clips.Count)], volume);
  }

  private void PlaySound(AudioClip clip, float volume)
  {
    if ((Object) this.audioSource == (Object) null || (Object) clip == (Object) null)
      return;
    this.audioSource.GTStop();
    this.audioSource.clip = (AudioClip) null;
    this.audioSource.loop = false;
    this.audioSource.volume = volume;
    this.audioSource.GTPlayOneShot(clip);
  }

  protected override void AuthorityUpdate(float dt)
  {
    base.AuthorityUpdate(dt);
    this.motorLevel = 0.0f;
    if (this.localState == RCVehicle.State.Mobilized)
    {
      this.motorLevel = Mathf.Max(Mathf.Max(Mathf.Abs(this.activeInput.joystick.y), Mathf.Abs(this.activeInput.joystick.x)), this.activeInput.trigger);
      if (!this.IsBreathingFire() && this.activeInput.buttons > (byte) 0)
        this.StartBreathFire();
    }
    if (!((Object) this.networkSync != (Object) null))
      return;
    this.networkSync.syncedState.dataA = (byte) Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * (float) byte.MaxValue), 0, (int) byte.MaxValue);
    this.networkSync.syncedState.dataB = this.activeInput.buttons;
    this.networkSync.syncedState.dataC = this.shouldFlap ? (byte) 1 : (byte) 0;
  }

  protected override void RemoteUpdate(float dt)
  {
    base.RemoteUpdate(dt);
    if (this.localState != RCVehicle.State.Mobilized || !((Object) this.networkSync != (Object) null))
      return;
    this.motorLevel = Mathf.Clamp01((float) this.networkSync.syncedState.dataA / (float) byte.MaxValue);
    if (!this.IsBreathingFire() && this.networkSync.syncedState.dataB > (byte) 0)
      this.StartBreathFire();
    this.shouldFlap = this.networkSync.syncedState.dataC > (byte) 0;
  }

  protected override void SharedUpdate(float dt)
  {
    base.SharedUpdate(dt);
    switch (this.localState)
    {
      case RCVehicle.State.DockedLeft:
      case RCVehicle.State.DockedRight:
        if (this.localStatePrev == RCVehicle.State.DockedLeft || this.localStatePrev == RCVehicle.State.DockedRight)
          break;
        this.audioSource.GTStop();
        if ((Object) this.crashCollider != (Object) null)
          this.crashCollider.enabled = false;
        if ((Object) this.animation != (Object) null)
          this.animation.Play(this.dockedAnimName);
        if (!this.IsBreathingFire())
          break;
        this.StopBreathFire();
        break;
      case RCVehicle.State.Mobilized:
        if (this.localStatePrev != RCVehicle.State.Mobilized && (Object) this.crashCollider != (Object) null)
          this.crashCollider.enabled = false;
        if ((Object) this.animation != (Object) null)
        {
          if (!this.isFlapping && this.shouldFlap)
          {
            this.animation.CrossFade(this.wingFlapAnimName, 0.1f);
            this.nextFlapEventAnimTime = this.flapAnimEventTime;
          }
          else if (this.isFlapping && !this.shouldFlap)
            this.animation.CrossFade(this.idleAnimName, 0.15f);
          this.isFlapping = this.shouldFlap;
          if (this.isFlapping && !this.IsBreathingFire())
          {
            AnimationState animationState = this.animation[this.wingFlapAnimName];
            if ((double) animationState.normalizedTime * (double) animationState.length > (double) this.nextFlapEventAnimTime)
            {
              this.PlayRandomSound(this.wingFlapSound, this.wingFlapVolume);
              this.nextFlapEventAnimTime = (Mathf.Floor(animationState.normalizedTime) + 1f) * animationState.length + this.flapAnimEventTime;
            }
          }
        }
        GTTime.TimeAsDouble();
        if (this.IsBreathingFire())
        {
          this.fireBreathTimeRemaining -= dt;
          if ((double) this.fireBreathTimeRemaining <= 0.0)
            this.StopBreathFire();
        }
        this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel), this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
        break;
      case RCVehicle.State.Crashed:
        if (this.localStatePrev == RCVehicle.State.Crashed)
          break;
        this.PlaySound(this.crashSound, this.crashSoundVolume);
        if ((Object) this.crashCollider != (Object) null)
          this.crashCollider.enabled = true;
        if ((Object) this.animation != (Object) null)
          this.animation.CrossFade(this.crashAnimName, 0.05f);
        if (!this.IsBreathingFire())
          break;
        this.StopBreathFire();
        break;
    }
  }

  private void FixedUpdate()
  {
    if (!this.HasLocalAuthority)
      return;
    float x = this.transform.lossyScale.x;
    float fixedDeltaTime = Time.fixedDeltaTime;
    this.shouldFlap = false;
    if (this.localState == RCVehicle.State.Mobilized)
    {
      float num1 = this.maxAscendSpeed * x;
      float b1 = this.maxHorizontalSpeed * x;
      float num2 = this.ascendAccel * x;
      float num3 = this.ascendWhileFlyingAccelBoost * x;
      float num4 = 0.5f * x;
      float num5 = 45f;
      Vector3 linearVelocity = this.rb.linearVelocity;
      Vector3 normalized = new Vector3(this.transform.forward.x, 0.0f, this.transform.forward.z).normalized;
      this.turnAngle = Vector3.SignedAngle(Vector3.forward, normalized, Vector3.up);
      this.tiltAngle = Vector3.SignedAngle(normalized, this.transform.forward, this.transform.right);
      this.turnRate = Mathf.MoveTowards(this.turnRate, this.activeInput.joystick.x * this.maxTurnRate, this.turnAccel * fixedDeltaTime);
      this.turnAngle += this.turnRate * fixedDeltaTime;
      float f = Vector3.Dot(normalized, linearVelocity);
      this.tiltAngle = Mathf.MoveTowards(this.tiltAngle, Mathf.Lerp(-this.maxHorizontalTiltAngle, this.maxHorizontalTiltAngle, Mathf.InverseLerp(-b1, b1, f)), this.tiltAccel * fixedDeltaTime);
      this.transform.rotation = Quaternion.Euler(new Vector3(this.tiltAngle, this.turnAngle, 0.0f));
      Vector3 b2 = new Vector3(linearVelocity.x, 0.0f, linearVelocity.z);
      this.rb.AddForce((Vector3.Lerp(normalized * this.activeInput.joystick.y * b1, b2, Mathf.Exp(-this.horizontalAccelTime * fixedDeltaTime)) - b2) * this.rb.mass, ForceMode.Impulse);
      float num6 = this.activeInput.trigger * num1;
      if ((double) num6 > 0.0099999997764825821 && (double) linearVelocity.y < (double) num6)
        this.rb.AddForce(Vector3.up * num2 * this.rb.mass, ForceMode.Force);
      bool flag1 = (double) Mathf.Abs(f) > (double) num4;
      bool flag2 = (double) Mathf.Abs(this.turnRate) > (double) num5;
      if (flag1 | flag2)
        this.rb.AddForce(Vector3.up * num3 * this.rb.mass, ForceMode.Force);
      this.shouldFlap = (double) num6 > 0.0099999997764825821 | flag1 | flag2;
      if (!this.rb.useGravity)
        return;
      RCVehicle.AddScaledGravityCompensationForce(this.rb, x, this.gravityCompensation);
    }
    else
    {
      if (this.localState != RCVehicle.State.Crashed || !this.rb.useGravity)
        return;
      RCVehicle.AddScaledGravityCompensationForce(this.rb, x, this.crashedGravityCompensation);
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    bool isProjectile = other.gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
    bool flag = other.gameObject.IsOnLayer(UnityLayer.GorillaHand);
    if (!other.isTrigger && this.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
    {
      this.AuthorityBeginCrash();
    }
    else
    {
      if (!(isProjectile | flag) || this.localState != RCVehicle.State.Mobilized)
        return;
      Vector3 hitVelocity = Vector3.zero;
      if (flag)
      {
        GorillaHandClimber component = other.gameObject.GetComponent<GorillaHandClimber>();
        if ((Object) component != (Object) null)
          hitVelocity = GTPlayer.Instance.GetHandVelocityTracker(component.xrNode == XRNode.LeftHand).GetAverageVelocity(true);
      }
      else if ((Object) other.attachedRigidbody != (Object) null)
        hitVelocity = other.attachedRigidbody.linearVelocity;
      if (!isProjectile && (double) hitVelocity.sqrMagnitude <= 0.0099999997764825821)
        return;
      if (this.HasLocalAuthority)
      {
        this.AuthorityApplyImpact(hitVelocity, isProjectile);
      }
      else
      {
        if (!((Object) this.networkSync != (Object) null))
          return;
        this.networkSync.photonView.RPC("HitRCVehicleRPC", RpcTarget.Others, (object) hitVelocity, (object) isProjectile);
      }
    }
  }
}
