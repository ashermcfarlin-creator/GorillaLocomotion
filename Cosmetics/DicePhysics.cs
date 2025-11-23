// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.DicePhysics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class DicePhysics : MonoBehaviour
{
  [SerializeField]
  private DicePhysics.DiceType diceType = DicePhysics.DiceType.D20;
  [SerializeField]
  private float landingTime = 5f;
  [SerializeField]
  private float postLandingTime = 2f;
  [SerializeField]
  private LayerMask surfaceLayers;
  [SerializeField]
  private AnimationCurve angleDeltaVsStrengthCurve = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  private AnimationCurve landingTimeVsStrengthCurve = AnimationCurve.Linear(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  private float strength = 1f;
  [SerializeField]
  private float damping = 0.5f;
  [SerializeField]
  private bool forceLandingSide;
  [SerializeField]
  private int forcedLandingSide = 20;
  [SerializeField]
  private bool allowPickupFromGround = true;
  [SerializeField]
  private float bounceAmplification = 1f;
  [SerializeField]
  private DicePhysics.CosmeticRollOverride[] cosmeticRollOverrides;
  [SerializeField]
  private UnityEvent onBestRoll;
  [SerializeField]
  private UnityEvent onWorstRoll;
  [SerializeField]
  private UnityEvent onRollFinished;
  [SerializeField]
  private Rigidbody rb;
  [SerializeField]
  private InteractionPoint interactionPoint;
  private VRRig cachedLocalRig;
  private DiceHoldable holdableParent;
  private double throwStartTime = -1.0;
  private double throwSettledTime = -1.0;
  private int landingSide;
  private float scale;
  private Vector3 prevVelocity = Vector3.zero;
  private Vector3 velocity = Vector3.zero;
  private const float a = 38.8333321f;
  private const float b = 77.66666f;
  private Vector3[] d20SideDirections = new Vector3[20]
  {
    Quaternion.AngleAxis(144f, Vector3.up) * Quaternion.AngleAxis(38.8333321f, Vector3.forward) * -Vector3.up,
    Quaternion.AngleAxis(324f, -Vector3.up) * Quaternion.AngleAxis(38.8333321f, -Vector3.forward) * Vector3.up,
    Quaternion.AngleAxis(288f, Vector3.up) * Quaternion.AngleAxis(38.8333321f, Vector3.forward) * -Vector3.up,
    Quaternion.AngleAxis(180f, -Vector3.up) * Quaternion.AngleAxis(38.8333321f, -Vector3.forward) * Vector3.up,
    Quaternion.AngleAxis(252f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
    Quaternion.AngleAxis(108f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
    Quaternion.AngleAxis(72f, Vector3.up) * Quaternion.AngleAxis(38.8333321f, Vector3.forward) * -Vector3.up,
    Quaternion.AngleAxis(36f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
    Quaternion.AngleAxis(216f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
    Quaternion.AngleAxis(0.0f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
    Quaternion.AngleAxis(180f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
    Quaternion.AngleAxis(324f, -Vector3.up) * Quaternion.AngleAxis(77.66666f, -Vector3.forward) * Vector3.up,
    Quaternion.AngleAxis(144f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
    Quaternion.AngleAxis(108f, -Vector3.up) * Quaternion.AngleAxis(38.8333321f, -Vector3.forward) * Vector3.up,
    Quaternion.AngleAxis(72f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
    Quaternion.AngleAxis(288f, Vector3.up) * Quaternion.AngleAxis(77.66666f, Vector3.forward) * -Vector3.up,
    Quaternion.AngleAxis(0.0f, Vector3.up) * Quaternion.AngleAxis(38.8333321f, Vector3.forward) * -Vector3.up,
    Quaternion.AngleAxis(252f, -Vector3.up) * Quaternion.AngleAxis(38.8333321f, -Vector3.forward) * Vector3.up,
    Quaternion.AngleAxis(216f, Vector3.up) * Quaternion.AngleAxis(38.8333321f, Vector3.forward) * -Vector3.up,
    Quaternion.AngleAxis(36f, -Vector3.up) * Quaternion.AngleAxis(38.8333321f, -Vector3.forward) * Vector3.up
  };
  private Vector3[] d6SideDirections = new Vector3[6]
  {
    new Vector3(0.0f, -1f, 0.0f),
    new Vector3(-1f, 0.0f, 0.0f),
    new Vector3(0.0f, 0.0f, -1f),
    new Vector3(0.0f, 0.0f, 1f),
    new Vector3(1f, 0.0f, 0.0f),
    new Vector3(0.0f, 1f, 0.0f)
  };

  public int GetRandomSide()
  {
    if (this.diceType != DicePhysics.DiceType.D6)
    {
      if (this.forceLandingSide)
        return Mathf.Clamp(this.forcedLandingSide, 1, 20);
      int rollSide;
      return this.CheckCosmeticRollOverride(out rollSide) ? Mathf.Clamp(rollSide, 1, 20) : UnityEngine.Random.Range(1, 21);
    }
    if (this.forceLandingSide)
      return Mathf.Clamp(this.forcedLandingSide, 1, 6);
    int rollSide1;
    return this.CheckCosmeticRollOverride(out rollSide1) ? Mathf.Clamp(rollSide1, 1, 6) : UnityEngine.Random.Range(1, 7);
  }

  public Vector3 GetSideDirection(int side)
  {
    if (this.diceType == DicePhysics.DiceType.D6)
      return this.d6SideDirections[Mathf.Clamp(side - 1, 0, 5)];
    return this.d20SideDirections[Mathf.Clamp(side - 1, 0, 19)];
  }

  public void StartThrow(
    DiceHoldable holdable,
    Vector3 startPosition,
    Vector3 velocity,
    float playerScale,
    int side,
    double startTime)
  {
    this.holdableParent = holdable;
    this.transform.parent = (Transform) null;
    this.transform.position = startPosition;
    this.transform.localScale = Vector3.one * playerScale;
    this.rb.isKinematic = false;
    this.rb.useGravity = true;
    this.rb.linearVelocity = velocity;
    if (!this.allowPickupFromGround && (UnityEngine.Object) this.interactionPoint != (UnityEngine.Object) null)
      this.interactionPoint.enabled = false;
    this.throwStartTime = startTime > 0.0 ? startTime : (double) Time.time;
    this.throwSettledTime = -1.0;
    this.scale = playerScale;
    this.landingSide = Mathf.Clamp(side, 1, 20);
    this.prevVelocity = Vector3.zero;
    velocity = Vector3.zero;
    this.enabled = true;
  }

  public void EndThrow()
  {
    this.rb.isKinematic = true;
    this.rb.linearVelocity = Vector3.zero;
    if ((UnityEngine.Object) this.holdableParent != (UnityEngine.Object) null)
      this.transform.parent = this.holdableParent.transform;
    this.transform.localPosition = Vector3.zero;
    this.transform.localRotation = Quaternion.identity;
    this.transform.localScale = Vector3.one;
    this.scale = 1f;
    this.throwStartTime = -1.0;
    if ((UnityEngine.Object) this.interactionPoint != (UnityEngine.Object) null)
      this.interactionPoint.enabled = true;
    this.onRollFinished.Invoke();
    this.enabled = false;
  }

  private void FixedUpdate()
  {
    double num1 = PhotonNetwork.InRoom ? PhotonNetwork.Time : (double) Time.time;
    float num2 = (float) (num1 - this.throwStartTime);
    UnityEngine.RaycastHit hitInfo;
    if (Physics.Raycast(this.transform.position, Vector3.down, out hitInfo, 0.1f * this.scale, this.surfaceLayers.value, QueryTriggerInteraction.Ignore))
    {
      Vector3 normal = hitInfo.normal;
      Vector3 vector3 = this.transform.rotation * this.GetSideDirection(this.landingSide);
      Vector3 normalized = Vector3.Cross(vector3, normal).normalized;
      float f = Vector3.SignedAngle(vector3, normal, normalized);
      float num3 = Mathf.Sign(f) * this.angleDeltaVsStrengthCurve.Evaluate(Mathf.Clamp01(Mathf.Abs(f) / 180f));
      double a = (double) this.landingTimeVsStrengthCurve.Evaluate(Mathf.Clamp01(num2 / this.landingTime));
      float magnitude = this.rb.linearVelocity.magnitude;
      double b = (double) Mathf.Clamp01(1f - Mathf.Min(magnitude, 1f));
      this.rb.AddTorque(this.strength * (Mathf.Max((float) a, (float) b) * num3 * normalized) - this.damping * this.rb.angularVelocity, ForceMode.Acceleration);
      if (!this.rb.isKinematic && (double) magnitude < 0.0099999997764825821 && (double) f < 2.0)
      {
        this.rb.isKinematic = true;
        this.throwSettledTime = num1;
        this.InvokeLandingEffects(this.landingSide);
      }
      else if (!this.rb.isKinematic && (double) num2 > (double) this.landingTime)
      {
        this.rb.isKinematic = true;
        this.throwSettledTime = num1;
        this.transform.rotation = Quaternion.FromToRotation(vector3, normal) * this.transform.rotation;
        this.InvokeLandingEffects(this.landingSide);
      }
    }
    if ((double) num2 > (double) this.landingTime + (double) this.postLandingTime || this.rb.isKinematic && num1 - this.throwSettledTime > (double) this.postLandingTime)
      this.EndThrow();
    this.prevVelocity = this.velocity;
    this.velocity = this.rb.linearVelocity;
  }

  private void OnCollisionEnter(Collision collision)
  {
    float magnitude = collision.impulse.magnitude;
    if ((double) magnitude <= 1.0 / 1000.0)
      return;
    this.rb.linearVelocity = Vector3.Reflect(this.prevVelocity, collision.impulse / magnitude) * this.bounceAmplification;
  }

  private void InvokeLandingEffects(int side)
  {
    if (this.diceType != DicePhysics.DiceType.D6)
    {
      if (side == 20)
      {
        this.onBestRoll.Invoke();
      }
      else
      {
        if (side != 1)
          return;
        this.onWorstRoll.Invoke();
      }
    }
    else if (side == 6)
    {
      this.onBestRoll.Invoke();
    }
    else
    {
      if (side != 1)
        return;
      this.onWorstRoll.Invoke();
    }
  }

  private bool CheckCosmeticRollOverride(out int rollSide)
  {
    if (this.cosmeticRollOverrides.Length != 0)
    {
      if ((UnityEngine.Object) this.cachedLocalRig == (UnityEngine.Object) null)
      {
        RigContainer playerRig;
        this.cachedLocalRig = !PhotonNetwork.InRoom || !VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out playerRig) || !((UnityEngine.Object) playerRig.Rig != (UnityEngine.Object) null) ? GorillaTagger.Instance.offlineVRRig : playerRig.Rig;
      }
      if ((UnityEngine.Object) this.cachedLocalRig != (UnityEngine.Object) null)
      {
        int num = -1;
        for (int index = 0; index < this.cosmeticRollOverrides.Length; ++index)
        {
          if (this.cosmeticRollOverrides[index].cosmeticName != null && this.cachedLocalRig.cosmeticSet != null && this.cachedLocalRig.cosmeticSet.HasItem(this.cosmeticRollOverrides[index].cosmeticName) && (!this.cosmeticRollOverrides[index].requireHolding || EquipmentInteractor.instance.leftHandHeldEquipment != null && EquipmentInteractor.instance.leftHandHeldEquipment.name == this.cosmeticRollOverrides[index].cosmeticName || EquipmentInteractor.instance.rightHandHeldEquipment != null && EquipmentInteractor.instance.rightHandHeldEquipment.name == this.cosmeticRollOverrides[index].cosmeticName) && this.cosmeticRollOverrides[index].landingSide > num)
            num = this.cosmeticRollOverrides[index].landingSide;
        }
        if (num > 0)
        {
          rollSide = num;
          return true;
        }
      }
    }
    rollSide = 0;
    return false;
  }

  private enum DiceType
  {
    D6,
    D20,
  }

  [Serializable]
  private struct CosmeticRollOverride
  {
    public string cosmeticName;
    public int landingSide;
    public bool requireHolding;
  }
}
