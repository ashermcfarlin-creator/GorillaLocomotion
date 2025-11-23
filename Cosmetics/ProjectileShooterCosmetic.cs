// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ProjectileShooterCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag.Cosmetics;

public class ProjectileShooterCosmetic : MonoBehaviour, ITickSystemTick
{
  private const string CHARGE_STR = "allowCharging";
  private const string CHARGE_MSG = "only enabled when allowCharging is true.";
  private const string HAPTICS_STR = "enableHaptics";
  private const string MOVE_STR = "IsMovementShoot";
  [SerializeField]
  private HashWrapper projectilePrefab;
  [SerializeField]
  private HashWrapper projectileTrailPrefab;
  [FormerlySerializedAs("launchActivatorType")]
  [SerializeField]
  private ProjectileShooterCosmetic.ShootActivator shootActivatorType;
  [FormerlySerializedAs("launchDirectionType")]
  [SerializeField]
  private ProjectileShooterCosmetic.ShootDirection shootDirectionType;
  [SerializeField]
  private Vector3 offsetRigPosition;
  [FormerlySerializedAs("launchTransform")]
  [SerializeField]
  private Transform shootFromTransform;
  [SerializeField]
  private bool drawShootVector;
  [FormerlySerializedAs("cooldown")]
  [SerializeField]
  private float cooldownSeconds;
  [Space]
  [SerializeField]
  private bool enableHaptics = true;
  [FormerlySerializedAs("hapticsIntensity")]
  [SerializeField]
  private float shootHapticsIntensity = 0.5f;
  [FormerlySerializedAs("hapticsDuration")]
  [SerializeField]
  private float shootHapticsDuration = 0.2f;
  [SerializeField]
  [Tooltip("only enabled when allowCharging is true.")]
  private float chargeHapticsIntensity = 0.3f;
  [SerializeField]
  [Tooltip("only enabled when allowCharging is true.")]
  private float maxChargeHapticsIntensity = 0.3f;
  [SerializeField]
  private bool hapticsBothHands;
  [Space]
  [SerializeField]
  private GorillaVelocityEstimator velocityEstimator;
  [SerializeField]
  private float velocityEstimatorStartGestureSpeed = 0.5f;
  [SerializeField]
  private float velocityEstimatorStopGestureSpeed = 0.2f;
  [SerializeField]
  private float velocityEstimatorMinRigDotProduct = 0.5f;
  [SerializeField]
  private bool logVelocityEstimatorSpeed;
  [FormerlySerializedAs("launchMinSpeed")]
  [SerializeField]
  [Tooltip("only enabled when allowCharging is true.")]
  private float shootMinSpeed;
  [FormerlySerializedAs("launchMaxSpeed")]
  [SerializeField]
  private float shootMaxSpeed;
  [SerializeField]
  private bool allowCharging;
  [SerializeField]
  private float maxChargeSeconds = 2f;
  [SerializeField]
  private float snapToMaxChargeAt = 9999999f;
  [SerializeField]
  private float chargeDecaySpeed = 9999999f;
  [SerializeField]
  private bool runChargeCancelledEventOnShoot;
  [SerializeField]
  private AnimationCurve chargeRateCurve = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  private AnimationCurve chargeToShotSpeedCurve = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [FormerlySerializedAs("onReadyToShoot")]
  public UnityEvent onCooldownFinished;
  public ContinuousPropertyArray continuousChargingProperties;
  public UnityEvent<float> whileCharging;
  public UnityEvent onMaxCharge;
  public UnityEvent onChargeCancelled;
  [FormerlySerializedAs("onLaunchProjectileShared")]
  public UnityEvent<float> onShoot;
  [FormerlySerializedAs("onOwnerLaunchProjectile")]
  public UnityEvent<float> onShootLocal;
  [SerializeField]
  private int numberOfProgressSteps;
  public UnityEvent<int> onMovedToNextStep;
  public UnityEvent<int> onReachedLastProgressStep;
  private int currentStep = -1;
  private int lastStep = -1;
  private bool isPressed;
  private bool velocityEstimatorThresholdMet;
  private float cooldownRemaining;
  private float chargeTime;
  private TransferrableObject transferrableObject;
  private VRRig rig;
  private bool isLocal;
  private Transform debugShootDirection;

  private bool IsMovementShoot()
  {
    return this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold;
  }

  private bool IsRigDirection()
  {
    return this.shootDirectionType == ProjectileShooterCosmetic.ShootDirection.LineFromRigToLaunchTransform;
  }

  public bool shootingAllowed { get; set; } = true;

  private bool IsCoolingDown => (double) this.cooldownRemaining > 0.0;

  private void Awake()
  {
    this.transferrableObject = this.GetComponent<TransferrableObject>();
    this.rig = (Object) this.transferrableObject == (Object) null ? this.GetComponentInParent<VRRig>() : this.transferrableObject.ownerRig;
    this.onMovedToNextStep?.Invoke(this.currentStep);
    this.isLocal = (Object) this.transferrableObject != (Object) null && this.transferrableObject.IsMyItem() || (Object) this.rig != (Object) null && (Object) this.rig == (Object) GorillaTagger.Instance.offlineVRRig;
  }

  public bool TickRunning { get; set; }

  public void Tick()
  {
    if (this.IsCoolingDown)
    {
      this.cooldownRemaining -= Time.deltaTime;
      if ((double) this.cooldownRemaining <= 0.0)
      {
        this.cooldownRemaining = 0.0f;
        this.onCooldownFinished?.Invoke();
        if (this.isPressed)
          this.SetPressState(true);
        if (!this.allowCharging && this.shootActivatorType != ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
          TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
      }
    }
    if (this.IsCoolingDown || !this.allowCharging)
      return;
    if (this.isPressed)
    {
      if ((double) this.chargeTime < (double) this.maxChargeSeconds)
      {
        this.chargeTime += Time.deltaTime;
        if ((double) this.chargeTime >= (double) this.maxChargeSeconds || (double) this.chargeTime >= (double) this.snapToMaxChargeAt)
        {
          this.chargeTime = this.maxChargeSeconds;
          this.onMaxCharge?.Invoke();
        }
      }
      float chargeFrac = this.GetChargeFrac();
      this.continuousChargingProperties?.ApplyAll(chargeFrac);
      this.whileCharging?.Invoke(chargeFrac);
      this.TryRunHaptics((double) chargeFrac >= 1.0 ? this.maxChargeHapticsIntensity : chargeFrac * this.chargeHapticsIntensity, Time.deltaTime);
      this.lastStep = this.currentStep;
      this.currentStep = Mathf.Clamp(Mathf.FloorToInt(chargeFrac * (float) this.numberOfProgressSteps), 0, this.numberOfProgressSteps - 1);
      if (this.currentStep >= 0 && this.currentStep != this.lastStep)
      {
        this.onMovedToNextStep?.Invoke(this.currentStep);
        if (this.currentStep == this.numberOfProgressSteps - 1)
          this.onReachedLastProgressStep?.Invoke(this.currentStep);
      }
      if (this.shootActivatorType != ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
        return;
      Vector3 linearVelocity = this.velocityEstimator.linearVelocity;
      float magnitude = linearVelocity.magnitude;
      float num1 = Vector3.Dot(linearVelocity / magnitude, this.GetVectorFromBodyToLaunchPosition().normalized);
      float num2 = magnitude * Mathf.Ceil(num1 - this.velocityEstimatorMinRigDotProduct);
      if ((double) num2 >= (double) this.velocityEstimatorStartGestureSpeed)
      {
        this.velocityEstimatorThresholdMet = true;
      }
      else
      {
        if (!this.velocityEstimatorThresholdMet || (double) num2 >= (double) this.velocityEstimatorStopGestureSpeed)
          return;
        this.TryShoot();
      }
    }
    else
    {
      if ((double) this.chargeTime <= 0.0)
        return;
      this.chargeTime -= Time.deltaTime * this.chargeDecaySpeed;
      if ((double) this.chargeTime <= 0.0)
      {
        this.chargeTime = 0.0f;
        TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
        this.continuousChargingProperties?.ApplyAll(0.0f);
        this.whileCharging?.Invoke(0.0f);
      }
      else
      {
        float chargeFrac = this.GetChargeFrac();
        this.continuousChargingProperties?.ApplyAll(chargeFrac);
        this.whileCharging?.Invoke(chargeFrac);
      }
    }
  }

  private Vector3 GetVectorFromBodyToLaunchPosition()
  {
    return this.shootFromTransform.position - this.rig.bodyTransform.TransformPoint(this.offsetRigPosition);
  }

  private void GetShootPositionAndRotation(out Vector3 position, out Quaternion rotation)
  {
    switch (this.shootDirectionType)
    {
      case ProjectileShooterCosmetic.ShootDirection.LineFromRigToLaunchTransform:
        position = this.shootFromTransform.position;
        rotation = Quaternion.LookRotation(position - this.rig.bodyTransform.TransformPoint(this.offsetRigPosition));
        break;
      default:
        this.shootFromTransform.GetPositionAndRotation(out position, out rotation);
        break;
    }
  }

  private void Shoot()
  {
    float chargeFrac = this.GetChargeFrac();
    float num = Mathf.Lerp(this.shootMinSpeed, this.shootMaxSpeed, this.chargeToShotSpeedCurve.Evaluate(chargeFrac));
    GameObject newProjectile = ObjectPools.instance.Instantiate((int) ref this.projectilePrefab);
    newProjectile.transform.localScale = Vector3.one * this.rig.scaleFactor;
    IProjectile component = newProjectile.GetComponent<IProjectile>();
    if (component != null)
    {
      Vector3 position;
      Quaternion rotation;
      this.GetShootPositionAndRotation(out position, out rotation);
      Vector3 velocity = rotation * Vector3.forward * (num * this.rig.scaleFactor);
      component.Launch(position, rotation, velocity, chargeFrac, this.rig, this.currentStep);
      if ((int) ref this.projectileTrailPrefab != -1)
        this.AttachTrail((int) ref this.projectileTrailPrefab, newProjectile, position, false, false);
    }
    this.onShoot?.Invoke(chargeFrac);
    this.continuousChargingProperties.ApplyAll(0.0f);
    this.whileCharging?.Invoke(0.0f);
    if (this.isLocal)
      this.onShootLocal?.Invoke(chargeFrac);
    if (this.allowCharging && this.runChargeCancelledEventOnShoot)
      this.onChargeCancelled?.Invoke();
    this.TryRunHaptics(chargeFrac * this.shootHapticsIntensity, this.shootHapticsDuration);
    this.SetPressState(false);
    this.cooldownRemaining = this.cooldownSeconds;
    this.chargeTime = 0.0f;
    this.currentStep = -1;
    TickSystem<object>.AddTickCallback((ITickSystemTick) this);
  }

  private bool TryShoot()
  {
    if ((this.IsCoolingDown || !this.shootingAllowed || this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.ButtonReleasedFullCharge) && (this.shootActivatorType != ProjectileShooterCosmetic.ShootActivator.ButtonReleasedFullCharge || (double) this.chargeTime < (double) this.maxChargeSeconds))
      return false;
    this.Shoot();
    return true;
  }

  private void TryRunHaptics(float intensity, float duration)
  {
    if (!this.enableHaptics || !this.isLocal || (double) intensity <= 0.0)
      return;
    bool forLeftController = (Object) this.transferrableObject != (Object) null && this.transferrableObject.InLeftHand();
    GorillaTagger.Instance.StartVibration(forLeftController, intensity, duration);
    if (!this.hapticsBothHands)
      return;
    GorillaTagger.Instance.StartVibration(!forLeftController, intensity, duration);
  }

  private float GetChargeFrac()
  {
    if (!this.allowCharging)
      return 1f;
    if ((double) this.chargeTime <= 0.0)
      return 0.0f;
    return (double) this.chargeTime < (double) this.maxChargeSeconds ? this.chargeRateCurve.Evaluate(this.chargeTime / this.maxChargeSeconds) : 1f;
  }

  private void SetPressState(bool pressed)
  {
    this.isPressed = pressed;
    this.velocityEstimatorThresholdMet = false;
  }

  public void OnButtonPressed()
  {
    this.SetPressState(true);
    if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.ButtonPressed)
    {
      this.TryShoot();
    }
    else
    {
      if (!this.allowCharging && this.shootActivatorType != ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold)
        return;
      TickSystem<object>.AddTickCallback((ITickSystemTick) this);
    }
  }

  public void OnButtonReleased()
  {
    if (this.shootActivatorType == ProjectileShooterCosmetic.ShootActivator.VelocityEstimatorThreshold && this.velocityEstimatorThresholdMet)
      return;
    int num;
    switch (this.shootActivatorType)
    {
      case ProjectileShooterCosmetic.ShootActivator.ButtonReleased:
      case ProjectileShooterCosmetic.ShootActivator.ButtonReleasedFullCharge:
        num = this.TryShoot() ? 1 : 0;
        break;
      default:
        num = 0;
        break;
    }
    if (num != 0)
      return;
    this.SetPressState(false);
    if (!this.allowCharging)
      return;
    this.continuousChargingProperties?.ApplyAll(0.0f);
    this.whileCharging?.Invoke(0.0f);
    this.onChargeCancelled?.Invoke();
  }

  public void ResetShoot()
  {
    this.isPressed = false;
    this.velocityEstimatorThresholdMet = false;
    this.currentStep = -1;
    this.lastStep = -1;
    TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
  }

  private void AttachTrail(
    int trailHash,
    GameObject newProjectile,
    Vector3 location,
    bool blueTeam,
    bool orangeTeam)
  {
    GameObject gameObject = ObjectPools.instance.Instantiate(trailHash);
    SlingshotProjectileTrail component = gameObject.GetComponent<SlingshotProjectileTrail>();
    if (component.IsNull())
      ObjectPools.instance.Destroy(gameObject);
    newProjectile.transform.position = location;
    component.AttachTrail(newProjectile, blueTeam, orangeTeam);
  }

  private enum ShootActivator
  {
    ButtonReleased,
    ButtonPressed,
    ButtonStayed,
    VelocityEstimatorThreshold,
    ButtonReleasedFullCharge,
  }

  private enum ShootDirection
  {
    LaunchTransformRotation,
    LineFromRigToLaunchTransform,
  }
}
