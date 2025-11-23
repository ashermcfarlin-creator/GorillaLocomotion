// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.CosmeticSwipeReactor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

[RequireComponent(typeof (Collider))]
public class CosmeticSwipeReactor : MonoBehaviour, ITickSystemTick
{
  [SerializeField]
  private CosmeticSwipeReactor.Axis localSwipeAxis = CosmeticSwipeReactor.Axis.Y;
  private Vector3 swipeDir = Vector3.up;
  [Tooltip("Distance hand can move perpindicular to the swipe without cancelling the gesture")]
  [SerializeField]
  private float lateralMovementTolerance = 0.1f;
  [Tooltip("How far the hand has to move along the axis to count as a swipe\nThis distance must be contained within the trigger area")]
  [SerializeField]
  private float swipeDistance = 0.3f;
  [SerializeField]
  private float minimumVelocity = 0.1f;
  [SerializeField]
  private float maximumVelocity = 3f;
  [Tooltip("Delay after completing a swipe before starting the next")]
  [SerializeField]
  private float swipeCooldown = 0.25f;
  [SerializeField]
  private bool resetCooldownOnTriggerExit = true;
  [Tooltip("Amplitude of haptics from normalized swiped distance")]
  [SerializeField]
  private AnimationCurve swipeHaptics = AnimationCurve.EaseInOut(0.0f, 0.02f, 1f, 0.5f);
  public UnityEvent<bool> OnSwipe;
  public UnityEvent<bool> OnReverseSwipe;
  private VRRig _rig;
  private Collider col;
  private bool isLocal;
  private bool handInTriggerR;
  private bool handInTriggerL;
  private GorillaTriggerColliderHandIndicator handIndicatorR;
  private GorillaTriggerColliderHandIndicator handIndicatorL;
  private Vector3 startPosR;
  private Vector3 startPosL;
  private Vector3 lastFramePosR;
  private Vector3 lastFramePosL;
  private float distanceR;
  private float distanceL;
  private bool swipingUpL;
  private bool swipingUpR;
  private double cooldownEndL = double.MinValue;
  private double cooldownEndR = double.MinValue;
  private bool isCoolingDownL;
  private bool isCoolingDownR;

  private void Awake()
  {
    this._rig = this.GetComponentInParent<VRRig>();
    if ((Object) this._rig == (Object) null && (Object) this.gameObject.GetComponentInParent<GTPlayer>() != (Object) null)
      this._rig = GorillaTagger.Instance.offlineVRRig;
    this.isLocal = (Object) this._rig != (Object) null && this._rig.isLocal;
    this.col = this.GetComponent<Collider>();
    switch (this.localSwipeAxis)
    {
      case CosmeticSwipeReactor.Axis.X:
        this.swipeDir = Vector3.right;
        break;
      case CosmeticSwipeReactor.Axis.Y:
        this.swipeDir = Vector3.up;
        break;
      case CosmeticSwipeReactor.Axis.Z:
        this.swipeDir = Vector3.forward;
        break;
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!this.isLocal || !this.enabled)
      return;
    GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
    if ((Object) component != (Object) null)
    {
      if (component.isLeftHand)
      {
        this.handIndicatorL = component;
        this.ResetProgress(true, this.transform.InverseTransformPoint(component.transform.position));
        this.handInTriggerL = true;
      }
      else
      {
        this.handIndicatorR = component;
        this.ResetProgress(false, this.transform.InverseTransformPoint(component.transform.position));
        this.handInTriggerR = true;
      }
    }
    if (!this.handInTriggerL && !this.handInTriggerR || this.TickRunning)
      return;
    TickSystem<object>.AddTickCallback((ITickSystemTick) this);
  }

  private void OnTriggerExit(Collider other)
  {
    if (!this.isLocal || !this.enabled)
      return;
    GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
    if ((Object) component != (Object) null)
    {
      if (component.isLeftHand)
      {
        this.handInTriggerL = false;
        if (this.resetCooldownOnTriggerExit)
        {
          this.isCoolingDownL = false;
          this.cooldownEndL = double.MinValue;
        }
      }
      else
      {
        this.handInTriggerR = false;
        if (this.resetCooldownOnTriggerExit)
        {
          this.isCoolingDownR = false;
          this.cooldownEndR = double.MinValue;
        }
      }
    }
    if (this.handInTriggerL || this.handInTriggerR || !this.TickRunning)
      return;
    TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
  }

  public bool TickRunning { get; set; }

  public void Tick()
  {
    if (this.handInTriggerL)
      this.ProcessHandMovement(this.handIndicatorL, this.startPosL, ref this.lastFramePosL, ref this.swipingUpL, ref this.distanceL, ref this.isCoolingDownL, ref this.cooldownEndL);
    if (this.handInTriggerR)
      this.ProcessHandMovement(this.handIndicatorR, this.startPosR, ref this.lastFramePosR, ref this.swipingUpR, ref this.distanceR, ref this.isCoolingDownR, ref this.cooldownEndR);
    if (this.handInTriggerL || this.handInTriggerR || !this.TickRunning)
      return;
    TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
  }

  private void ResetProgress(bool left, Vector3 pos)
  {
    if (left)
    {
      this.startPosL = pos;
      this.lastFramePosL = this.startPosL;
      this.distanceL = 0.0f;
    }
    else
    {
      this.startPosR = pos;
      this.lastFramePosR = this.startPosR;
      this.distanceR = 0.0f;
    }
  }

  private void ProcessHandMovement(
    GorillaTriggerColliderHandIndicator hand,
    Vector3 start,
    ref Vector3 last,
    ref bool swipingUp,
    ref float dist,
    ref bool isCoolingDown,
    ref double cooldownEndTime)
  {
    if (isCoolingDown)
    {
      if (Time.timeAsDouble < cooldownEndTime)
        return;
      isCoolingDown = false;
      cooldownEndTime = double.MinValue;
      this.ResetProgress(hand.isLeftHand, this.transform.InverseTransformPoint(hand.transform.position));
    }
    else
    {
      Vector3 vector3 = this.transform.InverseTransformPoint(hand.transform.position);
      float num = Mathf.Abs(this.GetAxisComponent(hand.currentVelocity));
      if ((double) num < (double) this.minimumVelocity * (double) this._rig.scaleFactor || (double) num > (double) this.maximumVelocity * (double) this._rig.scaleFactor)
      {
        this.ResetProgress(hand.isLeftHand, vector3);
      }
      else
      {
        float f = this.GetAxisComponent(vector3) - this.GetAxisComponent(last);
        if ((double) f >= 0.0 && !swipingUp)
        {
          swipingUp = true;
          this.ResetProgress(hand.isLeftHand, vector3);
        }
        else if ((double) f < 0.0 & swipingUp)
        {
          swipingUp = false;
          this.ResetProgress(hand.isLeftHand, vector3);
        }
        else if ((double) (this.GetLateralMovement(start) - this.GetLateralMovement(vector3)).sqrMagnitude > (double) this.lateralMovementTolerance * (double) this.lateralMovementTolerance)
        {
          this.ResetProgress(hand.isLeftHand, vector3);
        }
        else
        {
          last = vector3;
          dist += Mathf.Abs(f);
          GorillaTagger.Instance.StartVibration(hand.isLeftHand, this.swipeHaptics.Evaluate(dist / this.swipeDistance), Time.deltaTime);
          if ((double) dist < (double) this.swipeDistance)
            return;
          if (swipingUp)
          {
            this.OnSwipe?.Invoke(hand.isLeftHand);
            cooldownEndTime = Time.timeAsDouble + (double) this.swipeCooldown;
            isCoolingDown = true;
          }
          else
          {
            this.OnReverseSwipe?.Invoke(hand.isLeftHand);
            cooldownEndTime = Time.timeAsDouble + (double) this.swipeCooldown;
            isCoolingDown = true;
          }
          this.ResetProgress(hand.isLeftHand, vector3);
        }
      }
    }
  }

  private float GetAxisComponent(Vector3 vec)
  {
    switch (this.localSwipeAxis)
    {
      case CosmeticSwipeReactor.Axis.X:
        return vec.x;
      case CosmeticSwipeReactor.Axis.Y:
        return vec.y;
      default:
        return vec.z;
    }
  }

  private Vector2 GetLateralMovement(Vector3 vec)
  {
    switch (this.localSwipeAxis)
    {
      case CosmeticSwipeReactor.Axis.X:
        return new Vector2(vec.y, vec.z);
      case CosmeticSwipeReactor.Axis.Y:
        return new Vector2(vec.x, vec.z);
      default:
        return new Vector2(vec.x, vec.y);
    }
  }

  public enum Axis
  {
    X,
    Y,
    Z,
  }
}
