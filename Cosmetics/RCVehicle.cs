// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.RCVehicle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

#nullable disable
namespace GorillaTag.Cosmetics;

public class RCVehicle : MonoBehaviour, ISpawnable
{
  [SerializeField]
  private Transform leftDockParent;
  [SerializeField]
  private Transform rightDockParent;
  [SerializeField]
  private float maxRange = 100f;
  [SerializeField]
  private float maxDisconnectionTime = 10f;
  [SerializeField]
  private float crashRespawnDelay = 3f;
  [SerializeField]
  private bool crashOnHit;
  [SerializeField]
  private float crashOnHitSpeedThreshold = 5f;
  [SerializeField]
  [Range(0.0f, 1f)]
  private float hitVelocityTransfer = 0.5f;
  [SerializeField]
  [Range(0.0f, 1f)]
  private float projectileVelocityTransfer = 0.1f;
  [SerializeField]
  private float hitMaxHitSpeed = 4f;
  [SerializeField]
  [Range(0.0f, 1f)]
  private float joystickDeadzone = 0.1f;
  [Header("RCVehicle - Shared Event")]
  public UnityEvent OnHitImpact;
  protected RCVehicle.State localState;
  protected RCVehicle.State localStatePrev;
  protected float stateStartTime;
  protected RCRemoteHoldable connectedRemote;
  protected RCCosmeticNetworkSync networkSync;
  protected bool hasNetworkSync;
  protected RCRemoteHoldable.RCInput activeInput;
  protected Rigidbody rb;
  private bool waitingForTriggerRelease;
  private float disconnectionTime;
  private bool useLeftDock;
  private BoneOffset dockLeftOffset = new BoneOffset(GTHardCodedBones.EBone.forearm_L, new Vector3(-0.062f, 0.283f, -0.136f), new Vector3(275f, 0.0f, 25f));
  private BoneOffset dockRightOffset = new BoneOffset(GTHardCodedBones.EBone.forearm_R, new Vector3(0.069f, 0.265f, -0.128f), new Vector3(275f, 0.0f, 335f));
  private float networkSyncFollowRateExp = 2f;
  private Transform[] _vrRigBones;

  public bool HasLocalAuthority
  {
    get
    {
      if (!PhotonNetwork.InRoom)
        return true;
      return (Object) this.networkSync != (Object) null && this.networkSync.photonView.IsMine;
    }
  }

  public virtual void WakeUpRemote(RCCosmeticNetworkSync sync)
  {
    this.networkSync = sync;
    this.hasNetworkSync = (Object) sync != (Object) null;
    if (this.HasLocalAuthority || this.enabled && this.gameObject.activeSelf)
      return;
    this.localStatePrev = RCVehicle.State.Disabled;
    this.enabled = true;
    this.gameObject.SetActive(true);
    this.RemoteUpdate(Time.deltaTime);
  }

  public virtual void StartConnection(RCRemoteHoldable remote, RCCosmeticNetworkSync sync)
  {
    this.connectedRemote = remote;
    this.networkSync = sync;
    this.hasNetworkSync = (Object) sync != (Object) null;
    this.enabled = true;
    this.gameObject.SetActive(true);
    this.useLeftDock = remote.XRNode == XRNode.LeftHand;
    if (!this.HasLocalAuthority || this.localState == RCVehicle.State.Mobilized)
      return;
    this.AuthorityBeginDocked();
  }

  public virtual void EndConnection()
  {
    this.connectedRemote = (RCRemoteHoldable) null;
    this.activeInput = new RCRemoteHoldable.RCInput();
    this.disconnectionTime = Time.time;
  }

  protected virtual void ResetToSpawnPosition()
  {
    if ((Object) this.rb == (Object) null)
      this.rb = this.GetComponent<Rigidbody>();
    if ((Object) this.rb != (Object) null)
      this.rb.isKinematic = true;
    this.transform.parent = this.useLeftDock ? this.leftDockParent : this.rightDockParent;
    this.transform.SetLocalPositionAndRotation(this.useLeftDock ? this.dockLeftOffset.pos : this.dockRightOffset.pos, this.useLeftDock ? this.dockLeftOffset.rot : this.dockRightOffset.rot);
    this.transform.localScale = this.useLeftDock ? this.dockLeftOffset.scale : this.dockRightOffset.scale;
  }

  protected virtual void AuthorityBeginDocked()
  {
    this.localState = this.useLeftDock ? RCVehicle.State.DockedLeft : RCVehicle.State.DockedRight;
    if ((Object) this.networkSync != (Object) null)
      this.networkSync.syncedState.state = (byte) this.localState;
    this.stateStartTime = Time.time;
    this.waitingForTriggerRelease = true;
    this.ResetToSpawnPosition();
    if (!((Object) this.connectedRemote == (Object) null))
      return;
    this.SetDisabledState();
  }

  protected virtual void AuthorityBeginMobilization()
  {
    this.localState = RCVehicle.State.Mobilized;
    if ((Object) this.networkSync != (Object) null)
      this.networkSync.syncedState.state = (byte) this.localState;
    this.stateStartTime = Time.time;
    this.transform.parent = (Transform) null;
    this.rb.isKinematic = false;
  }

  protected virtual void AuthorityBeginCrash()
  {
    this.localState = RCVehicle.State.Crashed;
    if ((Object) this.networkSync != (Object) null)
      this.networkSync.syncedState.state = (byte) this.localState;
    this.stateStartTime = Time.time;
  }

  protected virtual void SetDisabledState()
  {
    this.localState = RCVehicle.State.Disabled;
    if ((Object) this.networkSync != (Object) null)
      this.networkSync.syncedState.state = (byte) this.localState;
    this.ResetToSpawnPosition();
    this.enabled = false;
    this.gameObject.SetActive(false);
  }

  protected virtual void Awake() => this.rb = this.GetComponent<Rigidbody>();

  protected virtual void OnEnable()
  {
  }

  bool ISpawnable.IsSpawned { get; set; }

  ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

  void ISpawnable.OnSpawn(VRRig rig)
  {
    VRRig vrRig = rig;
    if (vrRig == null)
    {
      GTDev.LogError<string>("RCVehicle: Could not find VRRig in parents. If you are trying to make this a world item rather than a cosmetic then you'll have to refactor how it teleports back to the arms.", (Object) this);
    }
    else
    {
      string outErrorMsg;
      if (!GTHardCodedBones.TryGetBoneXforms(vrRig, out this._vrRigBones, out outErrorMsg))
      {
        Debug.LogError((object) ("RCVehicle: " + outErrorMsg), (Object) this);
      }
      else
      {
        if ((Object) this.leftDockParent == (Object) null && !GTHardCodedBones.TryGetBoneXform(this._vrRigBones, (GTHardCodedBones.EBone) this.dockLeftOffset.bone, out this.leftDockParent))
          GTDev.LogError<string>("RCVehicle: Could not find left dock transform.", (Object) this);
        if (!((Object) this.rightDockParent == (Object) null) || GTHardCodedBones.TryGetBoneXform(this._vrRigBones, (GTHardCodedBones.EBone) this.dockRightOffset.bone, out this.rightDockParent))
          return;
        GTDev.LogError<string>("RCVehicle: Could not find right dock transform.", (Object) this);
      }
    }
  }

  void ISpawnable.OnDespawn()
  {
  }

  protected virtual void OnDisable()
  {
    this.localState = RCVehicle.State.Disabled;
    this.localStatePrev = RCVehicle.State.Disabled;
  }

  public void ApplyRemoteControlInput(RCRemoteHoldable.RCInput rcInput)
  {
    this.activeInput.joystick.y = Mathf.Sign(rcInput.joystick.y) * Mathf.Lerp(0.0f, 1f, Mathf.InverseLerp(this.joystickDeadzone, 1f, Mathf.Abs(rcInput.joystick.y)));
    this.activeInput.joystick.x = Mathf.Sign(rcInput.joystick.x) * Mathf.Lerp(0.0f, 1f, Mathf.InverseLerp(this.joystickDeadzone, 1f, Mathf.Abs(rcInput.joystick.x)));
    this.activeInput.trigger = Mathf.Clamp(rcInput.trigger, -1f, 1f);
    this.activeInput.buttons = rcInput.buttons;
  }

  private void Update()
  {
    float deltaTime = Time.deltaTime;
    if (this.HasLocalAuthority)
      this.AuthorityUpdate(deltaTime);
    else
      this.RemoteUpdate(deltaTime);
    this.SharedUpdate(deltaTime);
    this.localStatePrev = this.localState;
  }

  protected virtual void AuthorityUpdate(float dt)
  {
    switch (this.localState)
    {
      case RCVehicle.State.Mobilized:
        if ((Object) this.networkSync != (Object) null)
        {
          this.networkSync.syncedState.position = this.transform.position;
          this.networkSync.syncedState.rotation = this.transform.rotation;
        }
        if ((((double) (this.transform.position - this.leftDockParent.position).sqrMagnitude > (double) this.maxRange * (double) this.maxRange ? 1 : 0) | (!((Object) this.connectedRemote == (Object) null) ? (false ? 1 : 0) : ((double) Time.time - (double) this.disconnectionTime > (double) this.maxDisconnectionTime ? 1 : 0))) == 0)
          break;
        this.AuthorityBeginCrash();
        break;
      case RCVehicle.State.Crashed:
        if ((double) Time.time <= (double) this.stateStartTime + (double) this.crashRespawnDelay)
          break;
        this.AuthorityBeginDocked();
        break;
      default:
        if (this.localState != this.localStatePrev)
          this.ResetToSpawnPosition();
        if ((Object) this.connectedRemote == (Object) null)
        {
          this.SetDisabledState();
          break;
        }
        if (this.waitingForTriggerRelease && (double) this.activeInput.trigger < 0.25)
          this.waitingForTriggerRelease = false;
        if (this.waitingForTriggerRelease || (double) this.activeInput.trigger <= 0.25)
          break;
        this.AuthorityBeginMobilization();
        break;
    }
  }

  protected virtual void RemoteUpdate(float dt)
  {
    if ((Object) this.networkSync == (Object) null)
    {
      this.SetDisabledState();
    }
    else
    {
      this.localState = (RCVehicle.State) this.networkSync.syncedState.state;
      switch (this.localState)
      {
        case RCVehicle.State.Disabled:
          this.SetDisabledState();
          break;
        case RCVehicle.State.DockedRight:
          if (this.localStatePrev == RCVehicle.State.DockedRight)
            break;
          this.useLeftDock = false;
          this.ResetToSpawnPosition();
          break;
        case RCVehicle.State.Mobilized:
          if (this.localStatePrev != RCVehicle.State.Mobilized)
          {
            this.rb.isKinematic = true;
            this.transform.parent = (Transform) null;
          }
          this.transform.position = Vector3.Lerp(this.networkSync.syncedState.position, this.transform.position, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
          this.transform.rotation = Quaternion.Slerp(this.networkSync.syncedState.rotation, this.transform.rotation, Mathf.Exp(-this.networkSyncFollowRateExp * dt));
          break;
        case RCVehicle.State.Crashed:
          if (this.localStatePrev == RCVehicle.State.Crashed)
            break;
          this.rb.isKinematic = false;
          this.transform.parent = (Transform) null;
          if (this.localStatePrev == RCVehicle.State.Mobilized)
            break;
          this.transform.position = this.networkSync.syncedState.position;
          this.transform.rotation = this.networkSync.syncedState.rotation;
          break;
        default:
          if (this.localStatePrev == RCVehicle.State.DockedLeft)
            break;
          this.useLeftDock = true;
          this.ResetToSpawnPosition();
          break;
      }
    }
  }

  protected virtual void SharedUpdate(float dt)
  {
  }

  public virtual void AuthorityApplyImpact(Vector3 hitVelocity, bool isProjectile)
  {
    if (this.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
    {
      float num = isProjectile ? this.projectileVelocityTransfer : this.hitVelocityTransfer;
      this.rb.AddForce(Vector3.ClampMagnitude(hitVelocity * num, this.hitMaxHitSpeed) * this.rb.mass, ForceMode.Impulse);
      if (isProjectile || this.crashOnHit && (double) hitVelocity.sqrMagnitude > (double) this.crashOnHitSpeedThreshold * (double) this.crashOnHitSpeedThreshold)
        this.AuthorityBeginCrash();
    }
    this.OnHitImpact?.Invoke();
  }

  protected float NormalizeAngle180(float angle)
  {
    angle = (float) (((double) angle + 180.0) % 360.0);
    if ((double) angle < 0.0)
      angle += 360f;
    return angle - 180f;
  }

  protected static void AddScaledGravityCompensationForce(
    Rigidbody rb,
    float scaleFactor,
    float gravityCompensation)
  {
    Vector3 gravity = Physics.gravity;
    Vector3 vector3_1 = -gravity * gravityCompensation;
    Vector3 vector3_2 = gravity + vector3_1;
    Vector3 vector3_3 = vector3_2 * scaleFactor - vector3_2;
    rb.AddForce((vector3_1 + vector3_3) * rb.mass, ForceMode.Force);
  }

  protected enum State
  {
    Disabled,
    DockedLeft,
    DockedRight,
    Mobilized,
    Crashed,
  }
}
