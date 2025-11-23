// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.Dreidel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using CjLib;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class Dreidel : MonoBehaviour
{
  [Header("References")]
  [SerializeField]
  private Transform spinTransform;
  [SerializeField]
  private MeshCollider dreidelCollider;
  [SerializeField]
  private AudioSource audioSource;
  [SerializeField]
  private AudioClip spinLoopAudio;
  [SerializeField]
  private AudioClip fallSound;
  [SerializeField]
  private AudioClip gimelConfettiSound;
  [SerializeField]
  private ParticleSystem gimelConfetti;
  [Header("Offsets")]
  [SerializeField]
  private Vector3 centerOfMassOffset = Vector3.zero;
  [SerializeField]
  private Vector3 bottomPointOffset = Vector3.zero;
  [SerializeField]
  private Vector2 bodyRect = Vector2.one;
  [SerializeField]
  private float confettiHeight = 0.125f;
  [Header("Surface Detection")]
  [SerializeField]
  private float surfaceCheckDistance = 0.15f;
  [SerializeField]
  private float surfaceUprightThreshold = 0.5f;
  [SerializeField]
  private float surfaceDreidelAngleThreshold = 0.9f;
  [SerializeField]
  private LayerMask surfaceLayers;
  [Header("Spin Paramss")]
  [SerializeField]
  private float spinSpeedStart = 2f;
  [SerializeField]
  private float spinSpeedEnd = 1f;
  [SerializeField]
  private float spinTime = 10f;
  [SerializeField]
  private Vector2 spinTimeRange = new Vector2(7f, 12f);
  [SerializeField]
  private float spinWobbleFrequency = 0.1f;
  [SerializeField]
  private float spinWobbleAmplitude = 0.01f;
  [SerializeField]
  private float spinWobbleAmplitudeEndMin = 0.01f;
  [SerializeField]
  private float tiltFrontBack;
  [SerializeField]
  private float tiltLeftRight;
  [SerializeField]
  private float groundTrackingDampingRatio = 0.9f;
  [SerializeField]
  private float groundTrackingFrequency = 1f;
  [Header("Motion Path")]
  [SerializeField]
  private float pathMoveSpeed = 0.1f;
  [SerializeField]
  private float pathStartTurnRate = 360f;
  [SerializeField]
  private float pathEndTurnRate = 90f;
  [SerializeField]
  private float pathTurnRateSinOffset = 180f;
  [Header("Falling Params")]
  [SerializeField]
  private float spinSpeedStopRate = 1f;
  [SerializeField]
  private float tumbleFallDampingRatio = 0.4f;
  [SerializeField]
  private float tumbleFallFrequency = 6f;
  [SerializeField]
  private float tumbleFallFrontBackDampingRatio = 0.4f;
  [SerializeField]
  private float tumbleFallFrontBackFrequency = 6f;
  [SerializeField]
  private float smoothFallDampingRatio = 0.9f;
  [SerializeField]
  private float smoothFallFrequency = 2f;
  [SerializeField]
  private float slowTurnDampingRatio = 0.9f;
  [SerializeField]
  private float slowTurnFrequency = 2f;
  [SerializeField]
  private float bounceFallSwitchTime = 0.5f;
  [SerializeField]
  private float slowTurnSwitchTime = 0.5f;
  [SerializeField]
  private float respawnTimeAfterLanding = 3f;
  [SerializeField]
  private float fallTimeTumble = 3f;
  [SerializeField]
  private float fallTimeSlowTurn = 5f;
  private Dreidel.State state;
  private double stateStartTime;
  private float spinSpeed;
  private float spinAngle;
  private Vector3 spinAxis = Vector3.up;
  private bool canStartSpin;
  private double spinStartTime = -1.0;
  private float tiltWobble;
  private bool falseTargetReached;
  private bool hasLanded;
  private Vector3 pathOffset = Vector3.zero;
  private Vector3 pathDir = Vector3.forward;
  private Vector3 surfacePlanePoint;
  private Vector3 surfacePlaneNormal;
  private FloatSpring tiltFrontBackSpring;
  private FloatSpring tiltLeftRightSpring;
  private FloatSpring spinSpeedSpring;
  private Vector3Spring groundPointSpring;
  private Vector2[] landingTiltValues = new Vector2[4]
  {
    new Vector2(1f, -1f),
    new Vector2(1f, 0.0f),
    new Vector2(-1f, 1f),
    new Vector2(-1f, 0.0f)
  };
  private Vector2 landingTiltLeadingTarget = Vector2.zero;
  private Vector2 landingTiltTarget = Vector2.zero;
  [Header("Debug Params")]
  [SerializeField]
  private Dreidel.Side landingSide;
  [SerializeField]
  private Dreidel.Variation landingVariation;
  [SerializeField]
  private bool spinCounterClockwise;
  [SerializeField]
  private bool debugDraw;

  public bool TrySetIdle()
  {
    if (this.state != Dreidel.State.Idle && this.state != Dreidel.State.FindingSurface && this.state != Dreidel.State.Fallen)
      return false;
    this.StartIdle();
    return true;
  }

  public bool TryCheckForSurfaces()
  {
    if (this.state != Dreidel.State.Idle && this.state != Dreidel.State.FindingSurface)
      return false;
    this.StartFindingSurfaces();
    return true;
  }

  public void Spin() => this.StartSpin();

  public bool TryGetSpinStartData(
    out Vector3 surfacePoint,
    out Vector3 surfaceNormal,
    out float randomDuration,
    out Dreidel.Side randomSide,
    out Dreidel.Variation randomVariation,
    out double startTime)
  {
    if (this.canStartSpin)
    {
      surfacePoint = this.surfacePlanePoint;
      surfaceNormal = this.surfacePlaneNormal;
      randomDuration = Random.Range(this.spinTimeRange.x, this.spinTimeRange.y);
      randomSide = (Dreidel.Side) Random.Range(0, 4);
      randomVariation = (Dreidel.Variation) Random.Range(0, 5);
      startTime = PhotonNetwork.InRoom ? PhotonNetwork.Time : -1.0;
      return true;
    }
    surfacePoint = Vector3.zero;
    surfaceNormal = Vector3.zero;
    randomDuration = 0.0f;
    randomSide = Dreidel.Side.Shin;
    randomVariation = Dreidel.Variation.Tumble;
    startTime = -1.0;
    return false;
  }

  public void SetSpinStartData(
    Vector3 surfacePoint,
    Vector3 surfaceNormal,
    float duration,
    bool counterClockwise,
    Dreidel.Side side,
    Dreidel.Variation variation,
    double startTime)
  {
    this.surfacePlanePoint = surfacePoint;
    this.surfacePlaneNormal = surfaceNormal;
    this.spinTime = duration;
    this.spinStartTime = startTime;
    this.spinCounterClockwise = counterClockwise;
    this.landingSide = side;
    this.landingVariation = variation;
  }

  private void LateUpdate()
  {
    float deltaTime = Time.deltaTime;
    double num1 = PhotonNetwork.InRoom ? PhotonNetwork.Time : (double) Time.time;
    this.canStartSpin = false;
    switch (this.state)
    {
      case Dreidel.State.FindingSurface:
        float num2 = (Object) GTPlayer.Instance != (Object) null ? GTPlayer.Instance.scale : 1f;
        Vector3 down = Vector3.down;
        Vector3 origin = this.transform.parent.position - down * 2f * this.surfaceCheckDistance * num2;
        float num3 = (float) (3.0 * (double) this.surfaceCheckDistance + -(double) this.bottomPointOffset.y) * num2;
        Vector3 direction = down;
        UnityEngine.RaycastHit raycastHit;
        ref UnityEngine.RaycastHit local = ref raycastHit;
        double maxDistance = (double) num3;
        int layerMask = this.surfaceLayers.value;
        if (Physics.Raycast(origin, direction, out local, (float) maxDistance, layerMask, QueryTriggerInteraction.Ignore) && (double) Vector3.Dot(raycastHit.normal, Vector3.up) > (double) this.surfaceUprightThreshold && (double) Vector3.Dot(raycastHit.normal, this.spinTransform.up) > (double) this.surfaceDreidelAngleThreshold)
        {
          this.canStartSpin = true;
          this.surfacePlanePoint = raycastHit.point;
          this.surfacePlaneNormal = raycastHit.normal;
          this.AlignToSurfacePlane();
          this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
          this.UpdateSpinTransform();
          break;
        }
        this.canStartSpin = false;
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        this.spinTransform.localRotation = Quaternion.identity;
        this.spinTransform.localPosition = Vector3.zero;
        break;
      case Dreidel.State.Spinning:
        float t1 = Mathf.Clamp01((float) (num1 - this.stateStartTime) / this.spinTime);
        this.spinSpeed = Mathf.Lerp(this.spinSpeedStart, this.spinSpeedEnd, t1);
        this.spinAngle += (float) ((this.spinCounterClockwise ? -1.0 : 1.0) * (double) this.spinSpeed * 360.0) * deltaTime;
        float tiltWobble = this.tiltWobble;
        float num4 = Mathf.Sin((float) ((double) this.spinWobbleFrequency * 2.0 * 3.1415927410125732) * (float) (num1 - this.stateStartTime));
        float t2 = (float) (0.5 * (double) num4 + 0.5);
        this.tiltWobble = Mathf.Lerp(this.spinWobbleAmplitudeEndMin * t1, this.spinWobbleAmplitude * t1, t2);
        if ((double) this.landingTiltTarget.y == 0.0)
          this.tiltFrontBack = this.landingVariation == Dreidel.Variation.Tumble || this.landingVariation == Dreidel.Variation.Smooth ? Mathf.Sign(this.landingTiltTarget.x) * this.tiltWobble : Mathf.Sign(this.landingTiltLeadingTarget.x) * this.tiltWobble;
        else
          this.tiltLeftRight = this.landingVariation == Dreidel.Variation.Tumble || this.landingVariation == Dreidel.Variation.Smooth ? Mathf.Sign(this.landingTiltTarget.y) * this.tiltWobble : Mathf.Sign(this.landingTiltLeadingTarget.y) * this.tiltWobble;
        float num5 = Mathf.Lerp(this.pathStartTurnRate, this.pathEndTurnRate, t1) + num4 * this.pathTurnRateSinOffset;
        if (this.spinCounterClockwise)
        {
          this.pathDir = Vector3.ProjectOnPlane(Quaternion.AngleAxis(-num5 * deltaTime, Vector3.up) * this.pathDir, Vector3.up);
          this.pathDir.Normalize();
        }
        else
        {
          this.pathDir = Vector3.ProjectOnPlane(Quaternion.AngleAxis(-num5 * deltaTime, Vector3.up) * this.pathDir, Vector3.up);
          this.pathDir.Normalize();
        }
        this.pathOffset += this.pathDir * this.pathMoveSpeed * deltaTime;
        this.AlignToSurfacePlane();
        this.UpdateSpinTransform();
        if ((double) t1 - (double) Mathf.Epsilon < 1.0 || (double) this.tiltWobble <= 0.89999997615814209 * (double) this.spinWobbleAmplitude || (double) tiltWobble >= (double) this.tiltWobble)
          break;
        this.StartFall();
        break;
      case Dreidel.State.Falling:
        float num6 = this.fallTimeTumble;
        switch (this.landingVariation)
        {
          case Dreidel.Variation.Bounce:
          case Dreidel.Variation.SlowTurn:
          case Dreidel.Variation.FalseSlowTurn:
            bool flag1 = this.landingVariation != Dreidel.Variation.Bounce;
            bool flag2 = this.landingVariation == Dreidel.Variation.FalseSlowTurn;
            float num7 = flag1 ? this.slowTurnSwitchTime : this.bounceFallSwitchTime;
            if (flag1)
              num6 = this.fallTimeSlowTurn;
            if (num1 - this.stateStartTime < (double) num7)
            {
              this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltLeadingTarget.x, this.tumbleFallFrontBackFrequency, this.tumbleFallFrontBackDampingRatio, deltaTime);
              this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltLeadingTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
            }
            else
            {
              this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltTarget.x, this.tumbleFallFrontBackFrequency, this.tumbleFallFrontBackDampingRatio, deltaTime);
              if (flag2)
              {
                if (!this.falseTargetReached && (double) Mathf.Abs(this.landingTiltTarget.y - this.tiltLeftRight) > 0.49000000953674316)
                {
                  this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.slowTurnFrequency, this.slowTurnDampingRatio, deltaTime);
                }
                else
                {
                  this.falseTargetReached = true;
                  this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltLeadingTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime);
                }
              }
              else
                this.tiltLeftRight = !flag1 || (double) Mathf.Abs(this.landingTiltTarget.y - this.tiltLeftRight) <= 0.44999998807907104 ? this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.tumbleFallFrequency, this.tumbleFallDampingRatio, deltaTime) : this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, this.slowTurnFrequency, this.slowTurnDampingRatio, deltaTime);
            }
            this.spinSpeed = Mathf.MoveTowards(this.spinSpeed, 0.0f, this.spinSpeedStopRate * deltaTime);
            this.spinAngle += (float) ((this.spinCounterClockwise ? -1.0 : 1.0) * (double) this.spinSpeed * 360.0) * deltaTime;
            break;
          default:
            this.spinSpeed = Mathf.MoveTowards(this.spinSpeed, 0.0f, this.spinSpeedStopRate * deltaTime);
            this.spinAngle += (float) ((this.spinCounterClockwise ? -1.0 : 1.0) * (double) this.spinSpeed * 360.0) * deltaTime;
            float angularFrequency1 = this.landingVariation == Dreidel.Variation.Smooth ? this.smoothFallFrequency : this.tumbleFallFrontBackFrequency;
            float dampingRatio1 = this.landingVariation == Dreidel.Variation.Smooth ? this.smoothFallDampingRatio : this.tumbleFallFrontBackDampingRatio;
            float angularFrequency2 = this.landingVariation == Dreidel.Variation.Smooth ? this.smoothFallFrequency : this.tumbleFallFrequency;
            float dampingRatio2 = this.landingVariation == Dreidel.Variation.Smooth ? this.smoothFallDampingRatio : this.tumbleFallDampingRatio;
            this.tiltFrontBack = this.tiltFrontBackSpring.TrackDampingRatio(this.landingTiltTarget.x, angularFrequency1, dampingRatio1, deltaTime);
            this.tiltLeftRight = this.tiltLeftRightSpring.TrackDampingRatio(this.landingTiltTarget.y, angularFrequency2, dampingRatio2, deltaTime);
            break;
        }
        this.AlignToSurfacePlane();
        this.UpdateSpinTransform();
        float num8 = (float) (num1 - this.stateStartTime);
        if ((double) num8 <= (double) num6)
          break;
        if (!this.hasLanded)
        {
          this.hasLanded = true;
          if (this.landingSide == Dreidel.Side.Gimel)
          {
            this.gimelConfetti.transform.position = this.spinTransform.position + Vector3.up * this.confettiHeight;
            this.gimelConfetti.gameObject.SetActive(true);
            this.audioSource.GTPlayOneShot(this.gimelConfettiSound);
          }
        }
        if ((double) num8 <= (double) num6 + (double) this.respawnTimeAfterLanding)
          break;
        this.StartIdle();
        break;
      case Dreidel.State.Fallen:
        break;
      default:
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        this.spinTransform.localRotation = Quaternion.identity;
        this.spinTransform.localPosition = Vector3.zero;
        break;
    }
  }

  private void StartIdle()
  {
    this.state = Dreidel.State.Idle;
    this.stateStartTime = PhotonNetwork.InRoom ? PhotonNetwork.Time : (double) Time.time;
    this.canStartSpin = false;
    this.spinAngle = 0.0f;
    this.transform.localPosition = Vector3.zero;
    this.transform.localRotation = Quaternion.identity;
    this.spinTransform.localRotation = Quaternion.identity;
    this.spinTransform.localPosition = Vector3.zero;
    this.tiltFrontBack = 0.0f;
    this.tiltLeftRight = 0.0f;
    this.pathOffset = Vector3.zero;
    this.pathDir = Vector3.forward;
    this.gimelConfetti.gameObject.SetActive(false);
    this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
    this.UpdateSpinTransform();
  }

  private void StartFindingSurfaces()
  {
    this.state = Dreidel.State.FindingSurface;
    this.stateStartTime = PhotonNetwork.InRoom ? PhotonNetwork.Time : (double) Time.time;
    this.canStartSpin = false;
    this.spinAngle = 0.0f;
    this.transform.localPosition = Vector3.zero;
    this.transform.localRotation = Quaternion.identity;
    this.spinTransform.localRotation = Quaternion.identity;
    this.spinTransform.localPosition = Vector3.zero;
    this.tiltFrontBack = 0.0f;
    this.tiltLeftRight = 0.0f;
    this.pathOffset = Vector3.zero;
    this.pathDir = Vector3.forward;
    this.gimelConfetti.gameObject.SetActive(false);
    this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
    this.UpdateSpinTransform();
  }

  private void StartSpin()
  {
    this.state = Dreidel.State.Spinning;
    this.stateStartTime = this.spinStartTime > 0.0 ? this.spinStartTime : (double) Time.time;
    this.canStartSpin = false;
    this.spinSpeed = this.spinSpeedStart;
    this.tiltWobble = 0.0f;
    this.audioSource.loop = true;
    this.audioSource.clip = this.spinLoopAudio;
    this.audioSource.GTPlay();
    this.gimelConfetti.gameObject.SetActive(false);
    this.AlignToSurfacePlane();
    this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
    this.UpdateSpinTransform();
    this.pathOffset = Vector3.zero;
    this.pathDir = Vector3.forward;
  }

  private void StartFall()
  {
    this.state = Dreidel.State.Falling;
    this.stateStartTime = PhotonNetwork.InRoom ? PhotonNetwork.Time : (double) Time.time;
    this.canStartSpin = false;
    this.falseTargetReached = false;
    this.hasLanded = false;
    if (this.landingVariation == Dreidel.Variation.FalseSlowTurn)
    {
      if (this.spinCounterClockwise)
        this.GetTiltVectorsForSideWithPrev(this.landingSide, out this.landingTiltLeadingTarget, out this.landingTiltTarget);
      else
        this.GetTiltVectorsForSideWithNext(this.landingSide, out this.landingTiltLeadingTarget, out this.landingTiltTarget);
    }
    else if (this.spinCounterClockwise)
      this.GetTiltVectorsForSideWithNext(this.landingSide, out this.landingTiltTarget, out this.landingTiltLeadingTarget);
    else
      this.GetTiltVectorsForSideWithPrev(this.landingSide, out this.landingTiltTarget, out this.landingTiltLeadingTarget);
    this.spinSpeedSpring.Reset(this.spinSpeed, 0.0f);
    this.tiltFrontBackSpring.Reset(this.tiltFrontBack, 0.0f);
    this.tiltLeftRightSpring.Reset(this.tiltLeftRight, 0.0f);
    this.groundPointSpring.Reset(this.GetGroundContactPoint(), Vector3.zero);
    this.audioSource.loop = false;
    this.audioSource.GTPlayOneShot(this.fallSound);
    this.gimelConfetti.gameObject.SetActive(false);
  }

  private Vector3 GetGroundContactPoint()
  {
    Vector3 position1 = this.spinTransform.position;
    this.dreidelCollider.enabled = true;
    Vector3 position2 = this.dreidelCollider.ClosestPoint(position1 - this.transform.up);
    this.dreidelCollider.enabled = false;
    float num = Vector3.Dot(position2 - position1, this.spinTransform.up);
    if ((double) num > 0.0)
      position2 -= num * this.spinTransform.up;
    return this.spinTransform.InverseTransformPoint(position2);
  }

  private void GetTiltVectorsForSideWithPrev(
    Dreidel.Side side,
    out Vector2 sideTilt,
    out Vector2 prevSideTilt)
  {
    int index = side <= Dreidel.Side.Shin ? 3 : (int) (side - 1);
    if (side == Dreidel.Side.Hey || side == Dreidel.Side.Nun)
    {
      sideTilt = this.landingTiltValues[(int) side];
      prevSideTilt = this.landingTiltValues[index];
      prevSideTilt.x = sideTilt.x;
    }
    else
    {
      prevSideTilt = this.landingTiltValues[index];
      sideTilt = this.landingTiltValues[(int) side];
      sideTilt.x = prevSideTilt.x;
    }
  }

  private void GetTiltVectorsForSideWithNext(
    Dreidel.Side side,
    out Vector2 sideTilt,
    out Vector2 nextSideTilt)
  {
    int index = (int) (side + 1) % 4;
    if (side == Dreidel.Side.Hey || side == Dreidel.Side.Nun)
    {
      sideTilt = this.landingTiltValues[(int) side];
      nextSideTilt = this.landingTiltValues[index];
      nextSideTilt.x = sideTilt.x;
    }
    else
    {
      nextSideTilt = this.landingTiltValues[index];
      sideTilt = this.landingTiltValues[(int) side];
      sideTilt.x = nextSideTilt.x;
    }
  }

  private void AlignToSurfacePlane()
  {
    Vector3 forward = Vector3.forward;
    if ((double) Vector3.Dot(Vector3.up, this.surfacePlaneNormal) < 0.99989998340606689)
      forward = Quaternion.AngleAxis(90f, Vector3.Cross(this.surfacePlaneNormal, Vector3.up)) * this.surfacePlaneNormal;
    Quaternion quaternion = Quaternion.LookRotation(forward, this.surfacePlaneNormal);
    this.transform.position = this.surfacePlanePoint;
    this.transform.rotation = quaternion;
  }

  private void UpdateSpinTransform()
  {
    Vector3 position = this.spinTransform.position;
    Vector3 vector3 = this.spinTransform.TransformPoint(this.groundPointSpring.TrackDampingRatio(this.GetGroundContactPoint(), this.groundTrackingFrequency, this.groundTrackingDampingRatio, Time.deltaTime));
    Quaternion quaternion = Quaternion.AngleAxis(90f * this.tiltLeftRight, Vector3.forward) * Quaternion.AngleAxis(90f * this.tiltFrontBack, Vector3.right);
    this.spinAxis = this.transform.InverseTransformDirection(this.transform.up);
    this.spinTransform.localRotation = Quaternion.AngleAxis(this.spinAngle, this.spinAxis) * quaternion;
    this.spinTransform.localPosition = this.transform.InverseTransformVector(Vector3.Dot(position - vector3, this.transform.up) * this.transform.up) + this.pathOffset;
    this.spinTransform.TransformPoint(this.bottomPointOffset);
  }

  private enum State
  {
    Idle,
    FindingSurface,
    Spinning,
    Falling,
    Fallen,
  }

  public enum Side
  {
    Shin,
    Hey,
    Gimel,
    Nun,
    Count,
  }

  public enum Variation
  {
    Tumble,
    Smooth,
    Bounce,
    SlowTurn,
    FalseSlowTurn,
    Count,
  }
}
