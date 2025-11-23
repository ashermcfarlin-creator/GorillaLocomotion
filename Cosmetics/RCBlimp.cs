// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.RCBlimp
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

#nullable disable
namespace GorillaTag.Cosmetics;

public class RCBlimp : RCVehicle
{
  [SerializeField]
  private float maxAscendSpeed = 6f;
  [SerializeField]
  private float ascendAccelTime = 3f;
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
  private float deflateSoundVolume = 0.1f;
  [SerializeField]
  private Collider crashCollider;
  [SerializeField]
  private Transform leftPropeller;
  [SerializeField]
  private Transform rightPropeller;
  [SerializeField]
  private SkinnedMeshRenderer blimpMesh;
  [SerializeField]
  private AudioSource audioSource;
  [SerializeField]
  private AudioClip motorSound;
  [SerializeField]
  private AudioClip deflateSound;
  private float turnRate;
  private float turnAngle;
  private float tiltAngle;
  private float ascendAccel;
  private float turnAccel;
  private float tiltAccel;
  private float horizontalAccel;
  private float leftPropellerAngle;
  private float rightPropellerAngle;
  private float leftPropellerSpinRate;
  private float rightPropellerSpinRate;
  private float blimpDeflateBlendWeight;
  private float deflateRate = Mathf.Exp(1f);
  private const float propellerIdleAcc = 1f;
  private const float propellerIdleSpinRate = 0.6f;
  private const float propellerMaxAcc = 6.66666651f;
  private const float propellerMaxSpinRate = 5f;
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
  }

  protected override void OnDisable()
  {
    base.OnDisable();
    this.audioSource.GTStop();
  }

  protected override void AuthorityUpdate(float dt)
  {
    base.AuthorityUpdate(dt);
    this.motorLevel = 0.0f;
    if (this.localState == RCVehicle.State.Mobilized)
      this.motorLevel = Mathf.Max(Mathf.Max(Mathf.Abs(this.activeInput.joystick.y), Mathf.Abs(this.activeInput.joystick.x)), this.activeInput.trigger);
    if (!((Object) this.networkSync != (Object) null))
      return;
    this.networkSync.syncedState.dataA = (byte) Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * (float) byte.MaxValue), 0, (int) byte.MaxValue);
  }

  protected override void RemoteUpdate(float dt)
  {
    base.RemoteUpdate(dt);
    if (this.localState != RCVehicle.State.Mobilized || !((Object) this.networkSync != (Object) null))
      return;
    this.motorLevel = Mathf.Clamp01((float) this.networkSync.syncedState.dataA / (float) byte.MaxValue);
  }

  protected override void SharedUpdate(float dt)
  {
    base.SharedUpdate(dt);
    switch (this.localState)
    {
      case RCVehicle.State.DockedLeft:
      case RCVehicle.State.DockedRight:
        if (this.localStatePrev != RCVehicle.State.DockedLeft && this.localStatePrev != RCVehicle.State.DockedRight)
        {
          this.audioSource.GTStop();
          this.blimpDeflateBlendWeight = 0.0f;
          this.blimpMesh.SetBlendShapeWeight(0, 0.0f);
          this.crashCollider.enabled = false;
        }
        this.leftPropellerSpinRate = Mathf.MoveTowards(this.leftPropellerSpinRate, 0.6f, 6.66666651f * dt);
        this.rightPropellerSpinRate = Mathf.MoveTowards(this.rightPropellerSpinRate, 0.6f, 6.66666651f * dt);
        this.leftPropellerAngle += this.leftPropellerSpinRate * 360f * dt;
        this.rightPropellerAngle += this.rightPropellerSpinRate * 360f * dt;
        this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.leftPropellerAngle, 0.0f, -90f));
        this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.rightPropellerAngle, 0.0f, 90f));
        break;
      case RCVehicle.State.Mobilized:
        if (this.localStatePrev != RCVehicle.State.Mobilized)
        {
          this.audioSource.loop = true;
          this.audioSource.clip = this.motorSound;
          this.audioSource.volume = 0.0f;
          this.audioSource.GTPlay();
          this.blimpDeflateBlendWeight = 0.0f;
          this.blimpMesh.SetBlendShapeWeight(0, 0.0f);
          this.crashCollider.enabled = false;
        }
        this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel), this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
        this.blimpDeflateBlendWeight = 0.0f;
        float num1 = this.activeInput.joystick.y * 5f;
        double num2 = (double) this.activeInput.joystick.x * 5.0;
        float target1 = Mathf.Clamp((float) (num2 + (double) num1 + 0.60000002384185791), -5f, 5f);
        float target2 = Mathf.Clamp((float) (-num2 + (double) num1 + 0.60000002384185791), -5f, 5f);
        this.leftPropellerSpinRate = Mathf.MoveTowards(this.leftPropellerSpinRate, target1, 6.66666651f * dt);
        this.rightPropellerSpinRate = Mathf.MoveTowards(this.rightPropellerSpinRate, target2, 6.66666651f * dt);
        this.leftPropellerAngle += this.leftPropellerSpinRate * 360f * dt;
        this.rightPropellerAngle += this.rightPropellerSpinRate * 360f * dt;
        this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.leftPropellerAngle, 0.0f, -90f));
        this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.rightPropellerAngle, 0.0f, 90f));
        break;
      case RCVehicle.State.Crashed:
        if (this.localStatePrev != RCVehicle.State.Crashed)
        {
          this.audioSource.GTStop();
          this.audioSource.clip = (AudioClip) null;
          this.audioSource.loop = false;
          this.audioSource.volume = this.deflateSoundVolume;
          if ((Object) this.deflateSound != (Object) null)
            this.audioSource.GTPlayOneShot(this.deflateSound);
          this.leftPropellerSpinRate = 0.0f;
          this.rightPropellerSpinRate = 0.0f;
          this.leftPropellerAngle = 0.0f;
          this.rightPropellerAngle = 0.0f;
          this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, -90f));
          this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 90f));
          this.crashCollider.enabled = true;
        }
        this.blimpDeflateBlendWeight = Mathf.Lerp(1f, this.blimpDeflateBlendWeight, Mathf.Exp(-this.deflateRate * dt));
        this.blimpMesh.SetBlendShapeWeight(0, this.blimpDeflateBlendWeight * 100f);
        break;
    }
  }

  private void FixedUpdate()
  {
    if (!this.HasLocalAuthority)
      return;
    float fixedDeltaTime = Time.fixedDeltaTime;
    float x = this.transform.lossyScale.x;
    if (this.localState == RCVehicle.State.Mobilized)
    {
      float num1 = this.maxAscendSpeed * x;
      float b1 = this.maxHorizontalSpeed * x;
      float num2 = this.ascendAccel * x;
      Vector3 linearVelocity = this.rb.linearVelocity;
      Vector3 normalized = new Vector3(this.transform.forward.x, 0.0f, this.transform.forward.z).normalized;
      this.turnAngle = Vector3.SignedAngle(Vector3.forward, normalized, Vector3.up);
      this.tiltAngle = Vector3.SignedAngle(normalized, this.transform.forward, this.transform.right);
      this.turnRate = Mathf.MoveTowards(this.turnRate, this.activeInput.joystick.x * this.maxTurnRate, this.turnAccel * fixedDeltaTime);
      this.turnAngle += this.turnRate * fixedDeltaTime;
      float num3 = Vector3.Dot(normalized, linearVelocity);
      this.tiltAngle = Mathf.MoveTowards(this.tiltAngle, Mathf.Lerp(-this.maxHorizontalTiltAngle, this.maxHorizontalTiltAngle, Mathf.InverseLerp(-b1, b1, num3)), this.tiltAccel * fixedDeltaTime);
      this.transform.rotation = Quaternion.Euler(new Vector3(this.tiltAngle, this.turnAngle, 0.0f));
      Vector3 b2 = new Vector3(linearVelocity.x, 0.0f, linearVelocity.z);
      this.rb.AddForce((Vector3.Lerp(normalized * this.activeInput.joystick.y * b1, b2, Mathf.Exp(-this.horizontalAccelTime * fixedDeltaTime)) - b2) * this.rb.mass, ForceMode.Impulse);
      float num4 = this.activeInput.trigger * num1;
      if ((double) num4 > 0.0099999997764825821 && (double) linearVelocity.y < (double) num4)
        this.rb.AddForce(Vector3.up * num2 * this.rb.mass, ForceMode.Force);
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
