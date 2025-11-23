// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.DistanceCheckerCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class DistanceCheckerCosmetic : MonoBehaviour, ISpawnable
{
  [SerializeField]
  private Transform distanceFrom;
  [SerializeField]
  private DistanceCheckerCosmetic.DistanceCondition distanceTo;
  [Tooltip("Receive events when above or below this distance")]
  public float distanceThreshold;
  public UnityEvent onOneIsBelowThreshold;
  public UnityEvent onAllAreAboveThreshold;
  public UnityEvent<VRRig, float> onClosestPlayerBelowThresholdChanged;
  private VRRig myRig;
  private DistanceCheckerCosmetic.State currentState;
  private Vector3 closestDistance;
  private VRRig currentClosestPlayer;
  private VRRig ownerRig;
  private TransferrableObject transferableObject;

  public bool IsSpawned { get; set; }

  public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

  public void OnSpawn(VRRig rig) => this.myRig = rig;

  public void OnDespawn()
  {
  }

  private void OnEnable()
  {
    this.currentState = DistanceCheckerCosmetic.State.None;
    this.transferableObject = this.GetComponentInParent<TransferrableObject>();
    if ((Object) this.transferableObject != (Object) null)
      this.ownerRig = this.transferableObject.ownerRig;
    this.ResetClosestPlayer();
  }

  private void Update() => this.UpdateDistance();

  private bool IsBelowThreshold(Vector3 distance) => distance.IsShorterThan(this.distanceThreshold);

  private bool IsAboveThreshold(Vector3 distance) => distance.IsLongerThan(this.distanceThreshold);

  private void UpdateClosestPlayer(bool others = false)
  {
    if (!PhotonNetwork.InRoom)
    {
      this.ResetClosestPlayer();
    }
    else
    {
      VRRig currentClosestPlayer = this.currentClosestPlayer;
      this.closestDistance = Vector3.positiveInfinity;
      this.currentClosestPlayer = (VRRig) null;
      foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
      {
        if (!others || !((Object) this.ownerRig != (Object) null) || !((Object) vrrig == (Object) this.ownerRig))
        {
          Vector3 distance = vrrig.transform.position - this.distanceFrom.position;
          if (this.IsBelowThreshold(distance) && (double) distance.sqrMagnitude < (double) this.closestDistance.sqrMagnitude)
          {
            this.closestDistance = distance;
            this.currentClosestPlayer = vrrig;
          }
        }
      }
      if (!((Object) this.currentClosestPlayer != (Object) null) || !((Object) this.currentClosestPlayer != (Object) currentClosestPlayer))
        return;
      this.onClosestPlayerBelowThresholdChanged?.Invoke(this.currentClosestPlayer, this.closestDistance.magnitude);
    }
  }

  private void ResetClosestPlayer()
  {
    this.closestDistance = Vector3.positiveInfinity;
    this.currentClosestPlayer = (VRRig) null;
  }

  private void UpdateDistance()
  {
    bool flag = true;
    switch (this.distanceTo)
    {
      case DistanceCheckerCosmetic.DistanceCondition.Owner:
        Vector3 distance = this.myRig.transform.position - this.distanceFrom.position;
        if (this.IsBelowThreshold(distance))
        {
          this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
          break;
        }
        if (!this.IsAboveThreshold(distance))
          break;
        this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
        break;
      case DistanceCheckerCosmetic.DistanceCondition.Others:
        this.UpdateClosestPlayer(true);
        if (!PhotonNetwork.InRoom)
          break;
        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
        {
          if ((!((Object) this.ownerRig != (Object) null) || !((Object) vrrig == (Object) this.ownerRig)) && this.IsBelowThreshold(vrrig.transform.position - this.distanceFrom.position))
          {
            this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
            flag = false;
          }
        }
        if (!flag)
          break;
        this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
        break;
      case DistanceCheckerCosmetic.DistanceCondition.Everyone:
        this.UpdateClosestPlayer();
        if (!PhotonNetwork.InRoom)
          break;
        foreach (Component vrrig in GorillaParent.instance.vrrigs)
        {
          if (this.IsBelowThreshold(vrrig.transform.position - this.distanceFrom.position))
          {
            this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
            flag = false;
          }
        }
        if (!flag)
          break;
        this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
        break;
    }
  }

  private void UpdateState(DistanceCheckerCosmetic.State newState)
  {
    if (this.currentState == newState)
      return;
    this.currentState = newState;
    if (this.currentState == DistanceCheckerCosmetic.State.AboveThreshold)
    {
      this.onAllAreAboveThreshold?.Invoke();
    }
    else
    {
      if (this.currentState != DistanceCheckerCosmetic.State.BelowThreshold)
        return;
      this.onOneIsBelowThreshold?.Invoke();
    }
  }

  private enum State
  {
    AboveThreshold,
    BelowThreshold,
    None,
  }

  private enum DistanceCondition
  {
    None,
    Owner,
    Others,
    Everyone,
  }
}
