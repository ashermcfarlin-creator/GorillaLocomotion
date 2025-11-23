// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.MedusaEyeLantern
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaLocomotion.Climbing;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class MedusaEyeLantern : MonoBehaviour
{
  [SerializeField]
  private DistanceCheckerCosmetic distanceChecker;
  [SerializeField]
  private TransferrableObject transferableParent;
  [SerializeField]
  private GorillaVelocityTracker velocityTracker;
  [SerializeField]
  private Transform rotatingObjectTransform;
  [Space]
  [Header("Rotation Settings")]
  [SerializeField]
  private float maxRotationAngle = 50f;
  [SerializeField]
  private float sloshVelocityThreshold = 1f;
  [SerializeField]
  private float rotationSmoothing = 10f;
  [SerializeField]
  private float rotationSpeedMultiplier = 5f;
  [Space]
  [Header("Target Tracking Settings")]
  [SerializeField]
  private float lookAtEyeAngleThreshold = 90f;
  [SerializeField]
  private float targetHeadAngleThreshold = 5f;
  [SerializeField]
  private float lookAtTargetSpeed = 5f;
  [SerializeField]
  private float warmUpProgressTime = 3f;
  [SerializeField]
  private float resetCooldown = 5f;
  [SerializeField]
  private float faceDistanceOffset = 0.2f;
  [SerializeField]
  private float petrificationDuration = 0.2f;
  [Space]
  [Header("Eye State Settings")]
  public MedusaEyeLantern.EyeState[] allStates = new MedusaEyeLantern.EyeState[0];
  public UnityEvent<VRRig> OnPetrification;
  private Quaternion initialRotation;
  private Quaternion targetRotation;
  private MedusaEyeLantern.State currentState;
  private MedusaEyeLantern.State lastState;
  private float petrificationStarted = float.PositiveInfinity;
  private float warmupCounter;
  private Dictionary<MedusaEyeLantern.State, MedusaEyeLantern.EyeState> allStatesDict = new Dictionary<MedusaEyeLantern.State, MedusaEyeLantern.EyeState>();
  private VRRig targetRig;
  private float resetTargetTimer = 1f;
  private float resetTargetTime = float.PositiveInfinity;

  private void Awake()
  {
    foreach (MedusaEyeLantern.EyeState allState in this.allStates)
      this.allStatesDict.Add(allState.eyeState, allState);
  }

  private void OnDestroy() => this.allStatesDict.Clear();

  private void Start()
  {
    if ((UnityEngine.Object) this.rotatingObjectTransform == (UnityEngine.Object) null)
      this.rotatingObjectTransform = this.transform;
    this.initialRotation = this.rotatingObjectTransform.localRotation;
    this.SwitchState(MedusaEyeLantern.State.DORMANT);
  }

  private void Update()
  {
    if (!this.transferableParent.InHand() && this.currentState != MedusaEyeLantern.State.DORMANT)
      this.SwitchState(MedusaEyeLantern.State.DORMANT);
    if (!this.transferableParent.InHand())
      return;
    this.UpdateState();
    if ((UnityEngine.Object) this.velocityTracker == (UnityEngine.Object) null || (UnityEngine.Object) this.rotatingObjectTransform == (UnityEngine.Object) null)
      return;
    Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true);
    Vector3 vector3 = new Vector3(averageVelocity.x, 0.0f, averageVelocity.z);
    float magnitude = vector3.magnitude;
    Vector3 normalized = vector3.normalized;
    this.targetRotation = this.initialRotation * Quaternion.Euler((float) ((double) Mathf.Clamp(-normalized.z, -1f, 1f) * (double) this.maxRotationAngle * ((double) magnitude * (double) this.rotationSpeedMultiplier)), 0.0f, (float) ((double) Mathf.Clamp(normalized.x, -1f, 1f) * (double) this.maxRotationAngle * ((double) magnitude * (double) this.rotationSpeedMultiplier)));
    if ((double) magnitude > (double) this.sloshVelocityThreshold)
      this.SwitchState(MedusaEyeLantern.State.SLOSHING);
    if ((double) magnitude < 0.01)
      this.targetRotation = this.initialRotation;
    if (this.EyeIsLockedOn())
      return;
    this.rotatingObjectTransform.localRotation = Quaternion.Slerp(this.rotatingObjectTransform.localRotation, this.targetRotation, Time.deltaTime * this.rotationSmoothing);
  }

  public void HandleOnNoOneInRange()
  {
    this.SwitchState(MedusaEyeLantern.State.RESET);
    this.resetTargetTime = Time.time;
    this.rotatingObjectTransform.localRotation = this.initialRotation;
  }

  public void HandleOnNewPlayerDetected(VRRig target, float distance)
  {
    this.targetRig = target;
    if (this.currentState == MedusaEyeLantern.State.SLOSHING)
      return;
    this.SwitchState(MedusaEyeLantern.State.TRACKING);
  }

  private void Sloshing()
  {
    Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true);
    if ((double) new Vector3(averageVelocity.x, 0.0f, averageVelocity.z).magnitude >= 0.01)
      return;
    this.SwitchState(MedusaEyeLantern.State.DORMANT);
  }

  private void FaceTarget()
  {
    if ((UnityEngine.Object) this.targetRig == (UnityEngine.Object) null || (UnityEngine.Object) this.rotatingObjectTransform == (UnityEngine.Object) null)
      return;
    Vector3 vector3 = this.targetRig.tagSound.transform.position - this.rotatingObjectTransform.position;
    Vector3 normalized1 = vector3.normalized;
    vector3 = new Vector3(normalized1.x, 0.0f, normalized1.z);
    Vector3 normalized2 = vector3.normalized;
    Debug.DrawRay(this.rotatingObjectTransform.position, this.rotatingObjectTransform.forward * 0.3f, Color.blue);
    Debug.DrawRay(this.rotatingObjectTransform.position, normalized2 * 0.3f, Color.green);
    if ((double) normalized2.sqrMagnitude <= 1.0 / 1000.0)
      return;
    vector3 = this.rotatingObjectTransform.forward;
    if (180.0 - (double) (Mathf.Acos(Mathf.Clamp(Vector3.Dot(vector3.normalized, normalized2), -1f, 1f)) * 57.29578f) < (double) this.targetHeadAngleThreshold && this.currentState == MedusaEyeLantern.State.TRACKING)
      this.SwitchState(MedusaEyeLantern.State.WARMUP);
    else
      this.rotatingObjectTransform.rotation = Quaternion.RotateTowards(this.rotatingObjectTransform.rotation, Quaternion.LookRotation(-normalized2, Vector3.up), this.lookAtTargetSpeed * Time.deltaTime);
  }

  private bool IsTargetLookingAtEye()
  {
    if ((UnityEngine.Object) this.targetRig == (UnityEngine.Object) null || (UnityEngine.Object) this.rotatingObjectTransform == (UnityEngine.Object) null)
      return false;
    Transform transform = this.targetRig.tagSound.transform;
    Vector3 vector3 = this.rotatingObjectTransform.position - this.rotatingObjectTransform.forward * this.faceDistanceOffset - transform.position;
    Vector3 normalized = vector3.normalized;
    vector3 = transform.up;
    double num = (double) Mathf.Acos(Mathf.Clamp(Vector3.Dot(vector3.normalized, normalized), -1f, 1f)) * 57.295780181884766;
    Debug.DrawRay(transform.position, transform.up * 0.3f, Color.magenta);
    Debug.DrawRay(transform.position, normalized * 0.3f, Color.yellow);
    double eyeAngleThreshold = (double) this.lookAtEyeAngleThreshold;
    return num < eyeAngleThreshold;
  }

  private void UpdateState()
  {
    switch (this.currentState)
    {
      case MedusaEyeLantern.State.SLOSHING:
        this.Sloshing();
        break;
      case MedusaEyeLantern.State.DORMANT:
        this.warmupCounter = 0.0f;
        this.petrificationStarted = float.PositiveInfinity;
        if ((UnityEngine.Object) this.targetRig != (UnityEngine.Object) null && (this.targetRig.transform.position - this.transform.position).IsShorterThan(this.distanceChecker.distanceThreshold))
        {
          this.SwitchState(MedusaEyeLantern.State.TRACKING);
          break;
        }
        break;
      case MedusaEyeLantern.State.TRACKING:
        this.FaceTarget();
        break;
      case MedusaEyeLantern.State.WARMUP:
        this.warmupCounter += Time.deltaTime;
        this.FaceTarget();
        if ((double) this.warmupCounter > (double) this.warmUpProgressTime)
        {
          this.SwitchState(MedusaEyeLantern.State.PRIMING);
          this.warmupCounter = 0.0f;
          break;
        }
        break;
      case MedusaEyeLantern.State.PRIMING:
        this.FaceTarget();
        if (this.IsTargetLookingAtEye())
        {
          this.OnPetrification?.Invoke(this.targetRig);
          this.SwitchState(MedusaEyeLantern.State.PETRIFICATION);
          this.petrificationStarted = Time.time;
          break;
        }
        break;
      case MedusaEyeLantern.State.PETRIFICATION:
        if ((double) Time.time - (double) this.petrificationStarted > (double) this.petrificationDuration)
        {
          this.SwitchState(MedusaEyeLantern.State.COOLDOWN);
          break;
        }
        break;
      case MedusaEyeLantern.State.COOLDOWN:
        if ((double) Time.time - (double) this.petrificationStarted > (double) this.resetCooldown)
        {
          this.SwitchState(MedusaEyeLantern.State.DORMANT);
          this.petrificationStarted = float.PositiveInfinity;
          break;
        }
        break;
      case MedusaEyeLantern.State.RESET:
        if ((double) Time.time - (double) this.resetTargetTime > (double) this.resetTargetTimer)
        {
          this.resetTargetTime = float.PositiveInfinity;
          this.SwitchState(MedusaEyeLantern.State.DORMANT);
          break;
        }
        break;
    }
    this.PlayHaptic(this.currentState);
  }

  private void SwitchState(MedusaEyeLantern.State newState)
  {
    this.lastState = this.currentState;
    this.currentState = newState;
    MedusaEyeLantern.EyeState eyeState1;
    if (this.lastState != this.currentState && this.allStatesDict.TryGetValue(newState, out eyeState1))
      eyeState1.onEnterState?.Invoke();
    MedusaEyeLantern.EyeState eyeState2;
    if (this.lastState == this.currentState || !this.allStatesDict.TryGetValue(this.lastState, out eyeState2))
      return;
    eyeState2.onExitState?.Invoke();
  }

  private void PlayHaptic(MedusaEyeLantern.State state)
  {
    if (!this.transferableParent.IsMyItem())
      return;
    MedusaEyeLantern.EyeState eyeState;
    this.allStatesDict.TryGetValue(state, out eyeState);
    if (this.currentState == MedusaEyeLantern.State.WARMUP)
    {
      float time = Mathf.Clamp01(this.warmupCounter / this.warmUpProgressTime);
      if (eyeState == null || eyeState.hapticStrength == null)
        return;
      float amplitude = eyeState.hapticStrength.Evaluate(time);
      GorillaTagger.Instance.StartVibration(this.transferableParent.InLeftHand(), amplitude, Time.deltaTime);
    }
    else
    {
      if (eyeState == null || eyeState.hapticStrength == null)
        return;
      float amplitude = eyeState.hapticStrength.Evaluate(0.5f);
      GorillaTagger.Instance.StartVibration(this.transferableParent.InLeftHand(), amplitude, Time.deltaTime);
    }
  }

  private bool EyeIsLockedOn()
  {
    return this.currentState == MedusaEyeLantern.State.TRACKING || this.currentState == MedusaEyeLantern.State.WARMUP || this.currentState == MedusaEyeLantern.State.PRIMING;
  }

  [Serializable]
  public class EyeState
  {
    public MedusaEyeLantern.State eyeState;
    public AnimationCurve hapticStrength;
    public UnityEvent onEnterState;
    public UnityEvent onExitState;
  }

  public enum State
  {
    SLOSHING,
    DORMANT,
    TRACKING,
    WARMUP,
    PRIMING,
    PETRIFICATION,
    COOLDOWN,
    RESET,
  }
}
