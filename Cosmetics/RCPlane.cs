// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.RCPlane
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

#nullable disable
namespace GorillaTag.Cosmetics;

public class RCPlane : RCVehicle
{
  public Vector2 pitchVelocityTargetMinMax = new Vector2(-180f, 180f);
  public Vector2 pitchVelocityRampTimeMinMax = new Vector2(-0.75f, 0.75f);
  public float rollVelocityTarget = 180f;
  public float rollVelocityRampTime = 0.75f;
  public float thrustVelocityTarget = 15f;
  public float thrustAccelTime = 2f;
  [SerializeField]
  private float pitchVelocityFollowRateAngle = 60f;
  [SerializeField]
  private float pitchVelocityFollowRateMagnitude = 5f;
  [SerializeField]
  private float maxDrag = 0.1f;
  [SerializeField]
  private Vector2 liftVsSpeedInput = new Vector2(0.0f, 4f);
  [SerializeField]
  private Vector2 liftVsSpeedOutput = new Vector2(0.5f, 1f);
  [SerializeField]
  private AnimationCurve liftVsAttackCurve = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  private AnimationCurve dragVsAttackCurve = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  private Vector2 gravityCompensationRange = new Vector2(0.5f, 1f);
  [SerializeField]
  private List<Collider> nonCrashColliders = new List<Collider>();
  [SerializeField]
  private Transform propeller;
  [SerializeField]
  private Transform leftAileronUpper;
  [SerializeField]
  private Transform leftAileronLower;
  [SerializeField]
  private Transform rightAileronUpper;
  [SerializeField]
  private Transform rightAileronLower;
  [SerializeField]
  private AudioSource audioSource;
  [SerializeField]
  private AudioClip motorSound;
  [SerializeField]
  private AudioClip crashSound;
  [SerializeField]
  private Vector2 motorSoundVolumeMinMax = new Vector2(0.02f, 0.1f);
  [SerializeField]
  private float crashSoundVolume = 0.12f;
  private float motorVolumeRampTime = 1f;
  private float propellerAngle;
  private float propellerSpinRate;
  private const float propellerIdleAcc = 1f;
  private const float propellerIdleSpinRate = 0.6f;
  private const float propellerMaxAcc = 6.66666651f;
  private const float propellerMaxSpinRate = 5f;
  public float initialSpeed = 3f;
  private float pitch;
  private float pitchVel;
  private Vector2 pitchAccelMinMax;
  private float roll;
  private float rollVel;
  private float rollAccel;
  private float thrustAccel;
  private float motorLevel;
  private float leftAileronLevel;
  private float rightAileronLevel;
  private Vector2 aileronAngularRange = new Vector2(-30f, 45f);
  private float aileronAngularAcc = 120f;
  private float leftAileronAngle;
  private float rightAileronAngle;

  protected override void Awake()
  {
    base.Awake();
    this.pitchAccelMinMax.x = this.pitchVelocityTargetMinMax.x / this.pitchVelocityRampTimeMinMax.x;
    this.pitchAccelMinMax.y = this.pitchVelocityTargetMinMax.y / this.pitchVelocityRampTimeMinMax.y;
    this.rollAccel = this.rollVelocityTarget / this.rollVelocityRampTime;
    this.thrustAccel = this.thrustVelocityTarget / this.thrustAccelTime;
  }

  protected override void AuthorityBeginMobilization()
  {
    base.AuthorityBeginMobilization();
    this.rb.linearVelocity = this.transform.forward * this.initialSpeed * this.transform.lossyScale.x;
  }

  protected override void AuthorityUpdate(float dt)
  {
    base.AuthorityUpdate(dt);
    this.motorLevel = 0.0f;
    if (this.localState == RCVehicle.State.Mobilized)
      this.motorLevel = this.activeInput.trigger;
    this.leftAileronLevel = 0.0f;
    this.rightAileronLevel = 0.0f;
    float magnitude = this.activeInput.joystick.magnitude;
    if ((double) magnitude > 0.0099999997764825821)
    {
      float num1 = Mathf.Abs(this.activeInput.joystick.x) / magnitude;
      float num2 = Mathf.Abs(this.activeInput.joystick.y) / magnitude;
      this.leftAileronLevel = Mathf.Clamp((float) ((double) num1 * (double) this.activeInput.joystick.x + (double) num2 * -(double) this.activeInput.joystick.y), -1f, 1f);
      this.rightAileronLevel = Mathf.Clamp((float) ((double) num1 * (double) this.activeInput.joystick.x + (double) num2 * (double) this.activeInput.joystick.y), -1f, 1f);
    }
    if (!((UnityEngine.Object) this.networkSync != (UnityEngine.Object) null))
      return;
    this.networkSync.syncedState.dataA = (byte) Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * (float) byte.MaxValue), 0, (int) byte.MaxValue);
    this.networkSync.syncedState.dataB = (byte) Mathf.Clamp(Mathf.FloorToInt(this.leftAileronLevel * 126f), -126, 126);
    this.networkSync.syncedState.dataC = (byte) Mathf.Clamp(Mathf.FloorToInt(this.rightAileronLevel * 126f), -126, 126);
  }

  protected override void RemoteUpdate(float dt)
  {
    base.RemoteUpdate(dt);
    if (!((UnityEngine.Object) this.networkSync != (UnityEngine.Object) null))
      return;
    this.motorLevel = Mathf.Clamp01((float) this.networkSync.syncedState.dataA / (float) byte.MaxValue);
    this.leftAileronLevel = Mathf.Clamp((float) this.networkSync.syncedState.dataB / 126f, -1f, 1f);
    this.rightAileronLevel = Mathf.Clamp((float) this.networkSync.syncedState.dataC / 126f, -1f, 1f);
  }

  protected override void SharedUpdate(float dt)
  {
    base.SharedUpdate(dt);
    switch (this.localState)
    {
      case RCVehicle.State.DockedLeft:
      case RCVehicle.State.DockedRight:
        this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 0.6f, 6.66666651f * dt);
        this.propellerAngle += this.propellerSpinRate * 360f * dt;
        this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, this.propellerAngle));
        break;
      case RCVehicle.State.Mobilized:
        if (this.localStatePrev != RCVehicle.State.Mobilized)
        {
          this.audioSource.loop = true;
          this.audioSource.clip = this.motorSound;
          this.audioSource.volume = 0.0f;
          this.audioSource.GTPlay();
        }
        this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel), this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
        this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 5f, 6.66666651f * dt);
        this.propellerAngle += this.propellerSpinRate * 360f * dt;
        this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, this.propellerAngle));
        break;
      case RCVehicle.State.Crashed:
        if (this.localStatePrev != RCVehicle.State.Crashed)
        {
          this.audioSource.GTStop();
          this.audioSource.clip = (AudioClip) null;
          this.audioSource.loop = false;
          this.audioSource.volume = this.crashSoundVolume;
          if ((UnityEngine.Object) this.crashSound != (UnityEngine.Object) null)
            this.audioSource.GTPlayOneShot(this.crashSound);
        }
        this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 0.0f, 13.333333f * dt);
        this.propellerAngle += this.propellerSpinRate * 360f * dt;
        this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, this.propellerAngle));
        break;
    }
    float target1 = Mathf.Lerp(this.aileronAngularRange.x, this.aileronAngularRange.y, Mathf.InverseLerp(-1f, 1f, this.leftAileronLevel));
    float target2 = Mathf.Lerp(this.aileronAngularRange.x, this.aileronAngularRange.y, Mathf.InverseLerp(-1f, 1f, this.rightAileronLevel));
    this.leftAileronAngle = Mathf.MoveTowards(this.leftAileronAngle, target1, this.aileronAngularAcc * Time.deltaTime);
    this.rightAileronAngle = Mathf.MoveTowards(this.rightAileronAngle, target2, this.aileronAngularAcc * Time.deltaTime);
    Quaternion quaternion1 = Quaternion.Euler(0.0f, -90f, 90f + this.leftAileronAngle);
    Quaternion quaternion2 = Quaternion.Euler(0.0f, 90f, this.rightAileronAngle - 90f);
    this.leftAileronLower.localRotation = quaternion1;
    this.leftAileronUpper.localRotation = quaternion1;
    this.rightAileronLower.localRotation = quaternion2;
    this.rightAileronUpper.localRotation = quaternion2;
  }

  private void FixedUpdate()
  {
    if (!this.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
      return;
    float x = this.transform.lossyScale.x;
    float b1 = this.thrustVelocityTarget * x;
    float num1 = this.thrustAccel * x;
    float fixedDeltaTime = Time.fixedDeltaTime;
    this.pitch = this.NormalizeAngle180(this.pitch);
    this.roll = this.NormalizeAngle180(this.roll);
    float pitch = this.pitch;
    float roll = this.roll;
    if ((double) this.activeInput.joystick.y >= 0.0)
    {
      this.pitchVel = Mathf.MoveTowards(this.pitchVel, this.activeInput.joystick.y * this.pitchVelocityTargetMinMax.y, this.pitchAccelMinMax.y * fixedDeltaTime);
      this.pitch += this.pitchVel * fixedDeltaTime;
    }
    else
    {
      this.pitchVel = Mathf.MoveTowards(this.pitchVel, -this.activeInput.joystick.y * this.pitchVelocityTargetMinMax.x, this.pitchAccelMinMax.x * fixedDeltaTime);
      this.pitch += this.pitchVel * fixedDeltaTime;
    }
    this.rollVel = Mathf.MoveTowards(this.rollVel, -this.activeInput.joystick.x * this.rollVelocityTarget, this.rollAccel * fixedDeltaTime);
    this.roll += this.rollVel * fixedDeltaTime;
    this.transform.rotation = this.transform.rotation * Quaternion.Euler(new Vector3(this.pitch - pitch, 0.0f, this.roll - roll));
    this.rb.angularVelocity = Vector3.zero;
    Vector3 linearVelocity = this.rb.linearVelocity;
    float magnitude = linearVelocity.magnitude;
    float current = Mathf.Max(Vector3.Dot(this.transform.forward, linearVelocity), 0.0f);
    float target = this.activeInput.trigger * b1;
    float num2 = 0.1f * x;
    if ((double) target > (double) num2 && (double) target > (double) current)
      this.rb.AddForce(this.transform.forward * (Mathf.MoveTowards(current, target, num1 * fixedDeltaTime) - current) * this.rb.mass, ForceMode.Impulse);
    float b2 = 0.01f * x;
    float num3 = this.liftVsAttackCurve.Evaluate(Vector3.Dot(linearVelocity / Mathf.Max(magnitude, b2), this.transform.forward)) * Mathf.Lerp(this.liftVsSpeedOutput.x, this.liftVsSpeedOutput.y, Mathf.InverseLerp(this.liftVsSpeedInput.x, this.liftVsSpeedInput.y, magnitude / x));
    this.rb.AddForce((Vector3.RotateTowards(linearVelocity, this.transform.forward * magnitude, this.pitchVelocityFollowRateAngle * ((float) Math.PI / 180f) * fixedDeltaTime, this.pitchVelocityFollowRateMagnitude * fixedDeltaTime) - linearVelocity) * num3 * this.rb.mass, ForceMode.Impulse);
    float num4 = this.dragVsAttackCurve.Evaluate(Vector3.Dot(linearVelocity.normalized, this.transform.up));
    this.rb.AddForce(-linearVelocity * this.maxDrag * num4 * this.rb.mass, ForceMode.Force);
    if (!this.rb.useGravity)
      return;
    float gravityCompensation = Mathf.Lerp(this.gravityCompensationRange.x, this.gravityCompensationRange.y, Mathf.InverseLerp(0.0f, b1, current / x));
    RCVehicle.AddScaledGravityCompensationForce(this.rb, x, gravityCompensation);
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (this.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
    {
      for (int index = 0; index < collision.contactCount; ++index)
      {
        if (!this.nonCrashColliders.Contains(collision.GetContact(index).thisCollider))
          this.AuthorityBeginCrash();
      }
    }
    else
    {
      bool isProjectile = collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
      bool flag = collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaHand);
      if (!(isProjectile | flag) || this.localState != RCVehicle.State.Mobilized)
        return;
      Vector3 hitVelocity = Vector3.zero;
      if (flag)
      {
        GorillaHandClimber component = collision.collider.gameObject.GetComponent<GorillaHandClimber>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          hitVelocity = GTPlayer.Instance.GetHandVelocityTracker(component.xrNode == XRNode.LeftHand).GetAverageVelocity(true);
      }
      else if ((UnityEngine.Object) collision.rigidbody != (UnityEngine.Object) null)
        hitVelocity = collision.rigidbody.linearVelocity;
      if (!isProjectile && (double) hitVelocity.sqrMagnitude <= 0.0099999997764825821)
        return;
      if (this.HasLocalAuthority)
      {
        this.AuthorityApplyImpact(hitVelocity, isProjectile);
      }
      else
      {
        if (!((UnityEngine.Object) this.networkSync != (UnityEngine.Object) null))
          return;
        this.networkSync.photonView.RPC("HitRCVehicleRPC", RpcTarget.Others, (object) hitVelocity, (object) isProjectile);
      }
    }
  }
}
