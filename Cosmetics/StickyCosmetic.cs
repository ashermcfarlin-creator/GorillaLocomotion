// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.StickyCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class StickyCosmetic : MonoBehaviour
{
  [Tooltip("Optional reference to an UpdateBlendShapeCosmetic component. Used to drive extension length based on blend shape weight (e.g. finger flex input).")]
  [SerializeField]
  private UpdateBlendShapeCosmetic blendShapeCosmetic;
  [Tooltip("Defines which physics layers this sticky object can attach to when extending (checked via raycast).")]
  [SerializeField]
  private LayerMask collisionLayers;
  [Tooltip("Transform origin from which the raycast will be fired forward to detect stickable surfaces.")]
  [SerializeField]
  private Transform rayOrigin;
  [Tooltip("Transform representing the start or base position of the sticky object (where extension originates).")]
  [SerializeField]
  private Transform startPosition;
  [Tooltip("Rigidbody controlling the physical end of the sticky object (the part that extends and can attach).")]
  [SerializeField]
  private Rigidbody endRigidbody;
  [Tooltip("Parent transform the end object will reattach to when fully retracted. This keeps local transform resets consistent.")]
  [SerializeField]
  private Transform endPositionParent;
  [Tooltip("Maximum distance the object can extend from its start position (in meters).")]
  [SerializeField]
  private float maxObjectLength = 0.7f;
  [Tooltip("If the sticky object remains stuck but the distance from start exceeds this threshold, it will automatically unstuck and begin retracting.")]
  [SerializeField]
  private float autoRetractThreshold = 1f;
  [Tooltip("Speed (units per second) at which the end rigidbody retracts toward its start position when returning.")]
  [SerializeField]
  private float retractSpeed = 5f;
  [Tooltip("If the sticky end remains extended but doesn’t stick to anything, it will automatically start retracting after this many seconds.")]
  [SerializeField]
  private float retractAfterSecond = 2f;
  [Tooltip("Invoked when the sticky object successfully attaches to a surface.")]
  public UnityEvent onStick;
  [Tooltip("Invoked when the sticky object becomes unstuck — either manually or automatically.")]
  public UnityEvent onUnstick;
  private StickyCosmetic.ObjectState currentState;
  private float rayLength;
  private bool stick;
  private StickyCosmetic.ObjectState lastState;
  private float extendingStartedTime;

  private void Start()
  {
    this.endRigidbody.isKinematic = false;
    this.endRigidbody.useGravity = false;
    this.UpdateState(StickyCosmetic.ObjectState.Idle);
  }

  public void Extend()
  {
    if (this.currentState != StickyCosmetic.ObjectState.Idle && this.currentState != StickyCosmetic.ObjectState.Extending)
      return;
    this.UpdateState(StickyCosmetic.ObjectState.Extending);
  }

  public void Retract() => this.UpdateState(StickyCosmetic.ObjectState.Retracting);

  private void Extend_Internal()
  {
    if (this.endRigidbody.isKinematic)
      return;
    this.rayLength = Mathf.Lerp(0.0f, this.maxObjectLength, this.blendShapeCosmetic.GetBlendValue() / this.blendShapeCosmetic.maxBlendShapeWeight);
    this.endRigidbody.MovePosition(this.startPosition.position + this.startPosition.forward * this.rayLength);
  }

  private void Retract_Internal()
  {
    this.endRigidbody.isKinematic = false;
    this.endRigidbody.MovePosition(Vector3.MoveTowards(this.endRigidbody.position, this.startPosition.position, this.retractSpeed * Time.fixedDeltaTime));
  }

  private void FixedUpdate()
  {
    switch (this.currentState)
    {
      case StickyCosmetic.ObjectState.Extending:
        if ((double) Time.time - (double) this.extendingStartedTime > (double) this.retractAfterSecond)
          this.UpdateState(StickyCosmetic.ObjectState.AutoRetract);
        this.Extend_Internal();
        if (Physics.Raycast(this.rayOrigin.position, this.rayOrigin.forward, out RaycastHit _, this.rayLength, (int) this.collisionLayers))
        {
          this.endRigidbody.isKinematic = true;
          this.endRigidbody.transform.parent = (Transform) null;
          this.onStick?.Invoke();
          this.UpdateState(StickyCosmetic.ObjectState.Stuck);
          break;
        }
        break;
      case StickyCosmetic.ObjectState.Retracting:
        if ((double) Vector3.Distance(this.endRigidbody.position, this.startPosition.position) <= 0.0099999997764825821)
        {
          this.endRigidbody.position = this.startPosition.position;
          Transform transform = this.endRigidbody.transform;
          transform.parent = this.endPositionParent;
          transform.localRotation = (Quaternion) quaternion.identity;
          transform.localScale = Vector3.one;
          if (this.lastState == StickyCosmetic.ObjectState.AutoUnstuck || this.lastState == StickyCosmetic.ObjectState.AutoRetract)
          {
            this.UpdateState(StickyCosmetic.ObjectState.JustRetracted);
            break;
          }
          this.UpdateState(StickyCosmetic.ObjectState.Idle);
          break;
        }
        this.Retract_Internal();
        break;
      case StickyCosmetic.ObjectState.Stuck:
        if (this.endRigidbody.isKinematic && (this.endRigidbody.position - this.startPosition.position).IsLongerThan(this.autoRetractThreshold))
        {
          this.UpdateState(StickyCosmetic.ObjectState.AutoUnstuck);
          break;
        }
        break;
      case StickyCosmetic.ObjectState.AutoUnstuck:
        this.UpdateState(StickyCosmetic.ObjectState.Retracting);
        break;
      case StickyCosmetic.ObjectState.AutoRetract:
        this.UpdateState(StickyCosmetic.ObjectState.Retracting);
        break;
    }
    Debug.DrawRay(this.rayOrigin.position, this.rayOrigin.forward * this.rayLength, Color.red);
  }

  private void UpdateState(StickyCosmetic.ObjectState newState)
  {
    this.lastState = this.currentState;
    if (this.lastState == StickyCosmetic.ObjectState.Stuck && newState != this.currentState)
      this.onUnstick.Invoke();
    if (this.lastState != StickyCosmetic.ObjectState.Extending && newState == StickyCosmetic.ObjectState.Extending)
      this.extendingStartedTime = Time.time;
    this.currentState = newState;
  }

  private enum ObjectState
  {
    Extending,
    Retracting,
    Stuck,
    JustRetracted,
    Idle,
    AutoUnstuck,
    AutoRetract,
  }
}
