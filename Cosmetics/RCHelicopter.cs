// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.RCHelicopter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class RCHelicopter : RCVehicle
{
  [SerializeField]
  private float maxAscendSpeed = 6f;
  [SerializeField]
  private float ascendAccelTime = 3f;
  [SerializeField]
  private float gravityCompensation = 0.5f;
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
  private Vector2 mainPropellerSpinRateRange = new Vector2(3f, 15f);
  [SerializeField]
  private float backPropellerSpinRate = 5f;
  [SerializeField]
  private Transform verticalPropeller;
  [SerializeField]
  private Transform turnPropeller;
  private Quaternion verticalPropellerBaseRotation;
  private Quaternion turnPropellerBaseRotation;
  private float turnRate;
  private float ascendAccel;
  private float turnAccel;
  private float horizontalAccel;

  protected override void AuthorityBeginDocked()
  {
    base.AuthorityBeginDocked();
    this.turnRate = 0.0f;
    this.verticalPropeller.localRotation = this.verticalPropellerBaseRotation;
    this.turnPropeller.localRotation = this.turnPropellerBaseRotation;
    if (!((Object) this.connectedRemote == (Object) null))
      return;
    this.gameObject.SetActive(false);
  }

  protected override void Awake()
  {
    base.Awake();
    this.verticalPropellerBaseRotation = this.verticalPropeller.localRotation;
    this.turnPropellerBaseRotation = this.turnPropeller.localRotation;
    this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
    this.turnAccel = this.maxTurnRate / this.turnAccelTime;
    this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
  }

  protected override void SharedUpdate(float dt)
  {
    if (this.localState != RCVehicle.State.Mobilized)
      return;
    this.verticalPropeller.Rotate(new Vector3(0.0f, Mathf.Lerp(this.mainPropellerSpinRateRange.x, this.mainPropellerSpinRateRange.y, this.activeInput.trigger) * dt, 0.0f), Space.Self);
    this.turnPropeller.Rotate(new Vector3(this.activeInput.joystick.x * this.backPropellerSpinRate * dt, 0.0f, 0.0f), Space.Self);
  }

  private void FixedUpdate()
  {
    if (!this.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
      return;
    float fixedDeltaTime = Time.fixedDeltaTime;
    Vector3 linearVelocity = this.rb.linearVelocity;
    double magnitude = (double) linearVelocity.magnitude;
    this.turnRate = Mathf.MoveTowards(this.turnRate, this.activeInput.joystick.x * this.maxTurnRate, this.turnAccel * fixedDeltaTime);
    float f = this.activeInput.joystick.y * this.maxHorizontalSpeed;
    this.transform.rotation = Quaternion.Euler(new Vector3(Mathf.Sign(this.activeInput.joystick.y) * Mathf.Lerp(0.0f, this.maxHorizontalTiltAngle, Mathf.Abs(this.activeInput.joystick.y)), this.turnAccel, 0.0f));
    double num1 = (double) Mathf.Abs(f);
    Vector3 normalized = Vector3.ProjectOnPlane(this.transform.forward, Vector3.up).normalized;
    float num2 = Vector3.Dot(normalized, linearVelocity);
    if (num1 > 0.0099999997764825821 && ((double) f > 0.0 && (double) f > (double) num2 || (double) f < 0.0 && (double) f < (double) num2))
      this.rb.AddForce(normalized * Mathf.Sign(f) * this.horizontalAccel * fixedDeltaTime * this.rb.mass, ForceMode.Force);
    float num3 = this.activeInput.trigger * this.maxAscendSpeed;
    if ((double) num3 > 0.0099999997764825821 && (double) linearVelocity.y < (double) num3)
      this.rb.AddForce(Vector3.up * this.ascendAccel * this.rb.mass, ForceMode.Force);
    if (!this.rb.useGravity)
      return;
    this.rb.AddForce(-Physics.gravity * this.gravityCompensation * this.rb.mass, ForceMode.Force);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.isTrigger || !this.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
      return;
    this.AuthorityBeginCrash();
  }
}
