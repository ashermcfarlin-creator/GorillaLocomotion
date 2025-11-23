// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Climbing.GorillaHandClimber
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

#nullable disable
namespace GorillaLocomotion.Climbing;

public class GorillaHandClimber : MonoBehaviour
{
  [SerializeField]
  private GTPlayer player;
  [SerializeField]
  private EquipmentInteractor equipmentInteractor;
  private List<GorillaClimbable> potentialClimbables = new List<GorillaClimbable>();
  [Header("Non-hand input should have the component disabled")]
  public XRNode xrNode = XRNode.LeftHand;
  [NonSerialized]
  public bool isClimbing;
  [NonSerialized]
  public bool queuedToBecomeValidToGrabAgain;
  [NonSerialized]
  public GorillaClimbable dontReclimbLast;
  [NonSerialized]
  public Vector3 lastAutoReleasePos = Vector3.zero;
  public GorillaGrabber grabber;
  public Transform handRoot;
  private const float DIST_FOR_CLEAR_RELEASE = 0.35f;
  private const float DIST_FOR_GRAB = 0.15f;
  private Collider col;
  private bool canRelease = true;

  public bool isClimbingOrGrabbing => this.isClimbing || this.grabber.isGrabbing;

  private void Awake()
  {
    this.col = this.GetComponent<Collider>();
    this.grabber = this.GetComponent<GorillaGrabber>();
  }

  public void CheckHandClimber()
  {
    for (int index = this.potentialClimbables.Count - 1; index >= 0; --index)
    {
      GorillaClimbable potentialClimbable = this.potentialClimbables[index];
      if ((UnityEngine.Object) potentialClimbable == (UnityEngine.Object) null || !potentialClimbable.isActiveAndEnabled)
        this.potentialClimbables.RemoveAt(index);
      else if (potentialClimbable.climbOnlyWhileSmall && !ZoneManagement.IsInZone(GTZone.monkeBlocksShared) && (double) this.player.scale > 0.99000000953674316)
        this.potentialClimbables.RemoveAt(index);
    }
    bool grab = ControllerInputPoller.GetGrab(this.xrNode);
    bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
    if (!this.isClimbing)
    {
      if (this.queuedToBecomeValidToGrabAgain && (double) Vector3.Distance(this.lastAutoReleasePos, this.handRoot.localPosition) >= 0.34999999403953552)
        this.queuedToBecomeValidToGrabAgain = false;
      if (grabRelease)
      {
        this.queuedToBecomeValidToGrabAgain = false;
        this.dontReclimbLast = (GorillaClimbable) null;
      }
      GorillaClimbable closestClimbable = this.GetClosestClimbable();
      if (((this.queuedToBecomeValidToGrabAgain ? 0 : ((bool) (UnityEngine.Object) closestClimbable ? 1 : 0)) & (grab ? 1 : 0)) != 0 && this.CanInitiateClimb() && (UnityEngine.Object) closestClimbable != (UnityEngine.Object) this.dontReclimbLast)
      {
        if (closestClimbable is GorillaClimbableRef climbableRef)
          this.player.BeginClimbing(climbableRef.climb, this, climbableRef);
        else
          this.player.BeginClimbing(closestClimbable, this);
      }
    }
    else if (grabRelease && this.canRelease)
      this.player.EndClimbing(this, false);
    this.grabber.CheckGrabber(this.CanInitiateClimb() & grab);
  }

  private bool CanInitiateClimb()
  {
    return !this.isClimbing && !this.equipmentInteractor.GetIsHolding(this.xrNode) && !this.equipmentInteractor.builderPieceInteractor.GetIsHolding(this.xrNode) && !this.equipmentInteractor.IsGrabDisabled(this.xrNode) && !GamePlayerLocal.IsHandHolding(this.xrNode) && !this.player.inOverlay;
  }

  public void SetCanRelease(bool canRelease) => this.canRelease = canRelease;

  public GorillaClimbable GetClosestClimbable()
  {
    if (this.potentialClimbables.Count == 0)
      return (GorillaClimbable) null;
    if (this.potentialClimbables.Count == 1)
      return this.potentialClimbables[0];
    Vector3 position = this.transform.position;
    Bounds bounds = this.col.bounds;
    float num1 = 0.15f;
    GorillaClimbable closestClimbable = (GorillaClimbable) null;
    foreach (GorillaClimbable potentialClimbable in this.potentialClimbables)
    {
      float num2;
      if ((bool) (UnityEngine.Object) potentialClimbable.colliderCache)
      {
        if (bounds.Intersects(potentialClimbable.colliderCache.bounds))
        {
          Vector3 b = potentialClimbable.colliderCache.ClosestPoint(position);
          num2 = Vector3.Distance(position, b);
        }
        else
          continue;
      }
      else
        num2 = Vector3.Distance(position, potentialClimbable.transform.position);
      if ((double) num2 < (double) num1)
      {
        closestClimbable = potentialClimbable;
        num1 = num2;
      }
    }
    return closestClimbable;
  }

  private void OnTriggerEnter(Collider other)
  {
    GorillaClimbable component1;
    if (other.TryGetComponent<GorillaClimbable>(out component1))
    {
      this.potentialClimbables.Add(component1);
    }
    else
    {
      GorillaClimbableRef component2;
      if (!other.TryGetComponent<GorillaClimbableRef>(out component2))
        return;
      this.potentialClimbables.Add((GorillaClimbable) component2);
    }
  }

  private void OnTriggerExit(Collider other)
  {
    GorillaClimbable component1;
    if (other.TryGetComponent<GorillaClimbable>(out component1))
    {
      this.potentialClimbables.Remove(component1);
    }
    else
    {
      GorillaClimbableRef component2;
      if (!other.TryGetComponent<GorillaClimbableRef>(out component2))
        return;
      this.potentialClimbables.Remove((GorillaClimbable) component2);
    }
  }

  public void ForceStopClimbing(bool startingNewClimb = false, bool doDontReclimb = false)
  {
    this.player.EndClimbing(this, startingNewClimb, doDontReclimb);
  }
}
