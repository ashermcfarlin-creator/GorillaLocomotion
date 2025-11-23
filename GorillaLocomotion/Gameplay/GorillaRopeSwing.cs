// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.GorillaRopeSwing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaLocomotion.Climbing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

#nullable disable
namespace GorillaLocomotion.Gameplay;

public class GorillaRopeSwing : MonoBehaviour, IBuilderPieceComponent
{
  public int ropeId;
  public string staticId;
  public bool useStaticId;
  protected float ropeBitGenOffset = 1f;
  [SerializeField]
  protected GameObject prefabRopeBit;
  [SerializeField]
  private bool supportMovingAtRuntime;
  public Transform[] nodes = Array.Empty<Transform>();
  private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();
  [NonSerialized]
  public float lastGrabTime;
  [SerializeField]
  private AudioSource ropeCreakSFX;
  public GorillaVelocityTracker velocityTracker;
  private bool localPlayerOn;
  private int localPlayerBoneIndex;
  private XRNode localPlayerXRNode;
  private const float MAX_VELOCITY_FOR_IDLE = 0.5f;
  private const float TIME_FOR_IDLE = 2f;
  private float potentialIdleTimer;
  [SerializeField]
  protected int ropeLength = 8;
  [SerializeField]
  private GorillaRopeSwingSettings settings;
  private bool hasMonkeBlockParent;
  private BuilderPiece monkeBlockParent;
  [NonSerialized]
  public int ropeDataStartIndex;
  [NonSerialized]
  public int ropeDataIndexOffset;
  [SerializeField]
  private LayerMask wallLayerMask;
  private UnityEngine.RaycastHit[] nodeHits = new UnityEngine.RaycastHit[1];
  private float scaleFactor = 1f;
  private bool started;
  private int lastNodeCheckIndex = 2;

  private void EdRecalculateId() => this.CalculateId(true);

  public bool isIdle { get; private set; }

  public bool isFullyIdle { get; private set; }

  public bool SupportsMovingAtRuntime => this.supportMovingAtRuntime;

  public bool hasPlayers => this.localPlayerOn || this.remotePlayers.Count > 0;

  protected virtual void Awake()
  {
    this.transform.rotation = Quaternion.identity;
    this.scaleFactor = (float) (((double) this.transform.lossyScale.x + (double) this.transform.lossyScale.y + (double) this.transform.lossyScale.z) / 3.0);
    this.SetIsIdle(true);
  }

  protected virtual void Start()
  {
    if (!this.useStaticId)
      this.CalculateId();
    RopeSwingManager.Register(this);
    this.started = true;
  }

  private void OnDestroy()
  {
    if (!((UnityEngine.Object) RopeSwingManager.instance != (UnityEngine.Object) null))
      return;
    RopeSwingManager.Unregister(this);
  }

  protected virtual void OnEnable()
  {
    this.transform.rotation = Quaternion.identity;
    this.scaleFactor = (float) (((double) this.transform.lossyScale.x + (double) this.transform.lossyScale.y + (double) this.transform.lossyScale.z) / 3.0);
    this.SetIsIdle(true, true);
    VectorizedCustomRopeSimulation.Register(this);
    GorillaRopeSwingUpdateManager.RegisterRopeSwing(this);
  }

  private void OnDisable()
  {
    if (!this.isIdle)
      this.SetIsIdle(true, true);
    VectorizedCustomRopeSimulation.Unregister(this);
    GorillaRopeSwingUpdateManager.UnregisterRopeSwing(this);
  }

  internal void CalculateId(bool force = false)
  {
    Transform transform = this.transform;
    int i1 = StaticHash.Compute(TransformUtils.GetScenePath(transform).GetStaticHash(), this.GetType().Name.GetStaticHash());
    if (this.useStaticId)
    {
      if (string.IsNullOrEmpty(this.staticId) | force)
      {
        Vector3 position = transform.position;
        int i2 = StaticHash.Compute(position.x, position.y, position.z);
        int instanceId = transform.GetInstanceID();
        this.staticId = $"#ID_{StaticHash.Compute(i1, i2, instanceId):X8}";
      }
      this.ropeId = this.staticId.GetStaticHash();
    }
    else
      this.ropeId = Application.isPlaying ? i1 : 0;
  }

  public void InvokeUpdate()
  {
    if (this.isIdle)
      this.isFullyIdle = true;
    if (!this.isIdle)
    {
      int nodeIndex = -1;
      if (this.localPlayerOn)
        nodeIndex = this.localPlayerBoneIndex;
      else if (this.remotePlayers.Count > 0)
        nodeIndex = this.remotePlayers.First<KeyValuePair<int, int>>().Value;
      Vector3 vector3;
      if (nodeIndex >= 0)
      {
        vector3 = VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, nodeIndex);
        if ((double) vector3.magnitude > 2.0 && !this.ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
          this.ropeCreakSFX.GTPlay();
      }
      if (this.localPlayerOn)
      {
        vector3 = this.velocityTracker.GetLatestVelocity(true);
        float amplitude = MathUtils.Linear(vector3.magnitude / this.scaleFactor, 0.0f, 10f, -0.07f, 0.5f);
        if ((double) amplitude > 0.0)
          GorillaTagger.Instance.DoVibration(this.localPlayerXRNode, amplitude, Time.deltaTime);
      }
      Transform bone = this.GetBone(this.lastNodeCheckIndex);
      Vector3 nodeVelocity = VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, this.lastNodeCheckIndex);
      if (Physics.SphereCastNonAlloc(bone.position, 0.2f * this.scaleFactor, nodeVelocity.normalized, this.nodeHits, 0.4f * this.scaleFactor, (int) this.wallLayerMask, QueryTriggerInteraction.Ignore) > 0)
        this.SetVelocity(this.lastNodeCheckIndex, Vector3.zero, false, new PhotonMessageInfoWrapped());
      if ((double) nodeVelocity.magnitude <= 0.34999999403953552)
        this.potentialIdleTimer += Time.deltaTime;
      else
        this.potentialIdleTimer = 0.0f;
      if ((double) this.potentialIdleTimer >= 2.0)
      {
        this.SetIsIdle(true);
        this.potentialIdleTimer = 0.0f;
      }
      ++this.lastNodeCheckIndex;
      if (this.lastNodeCheckIndex > this.nodes.Length)
        this.lastNodeCheckIndex = 2;
    }
    if (!this.hasMonkeBlockParent || !this.supportMovingAtRuntime)
      return;
    this.transform.rotation = Quaternion.Euler(0.0f, this.transform.parent.rotation.eulerAngles.y, 0.0f);
  }

  private void SetIsIdle(bool idle, bool resetPos = false)
  {
    this.isIdle = idle;
    this.ropeCreakSFX.gameObject.SetActive(!idle);
    if (idle)
    {
      this.ToggleVelocityTracker(false);
      if (!resetPos)
        return;
      Vector3 zero = Vector3.zero;
      for (int index = 0; index < this.nodes.Length; ++index)
      {
        this.nodes[index].transform.localRotation = Quaternion.identity;
        this.nodes[index].transform.localPosition = zero;
        zero += new Vector3(0.0f, -this.ropeBitGenOffset, 0.0f);
      }
    }
    else
      this.isFullyIdle = false;
  }

  public Transform GetBone(int index)
  {
    return index >= this.nodes.Length ? ((IEnumerable<Transform>) this.nodes).Last<Transform>() : this.nodes[index];
  }

  public int GetBoneIndex(Transform r)
  {
    for (int boneIndex = 0; boneIndex < this.nodes.Length; ++boneIndex)
    {
      if ((UnityEngine.Object) this.nodes[boneIndex] == (UnityEngine.Object) r)
        return boneIndex;
    }
    return this.nodes.Length - 1;
  }

  public void AttachLocalPlayer(
    XRNode xrNode,
    Transform grabbedBone,
    Vector3 offset,
    Vector3 velocity)
  {
    int boneIndex = this.GetBoneIndex(grabbedBone);
    this.localPlayerBoneIndex = boneIndex;
    velocity /= this.scaleFactor;
    velocity *= this.settings.inheritVelocityMultiplier;
    if (GorillaTagger.hasInstance && (bool) (UnityEngine.Object) GorillaTagger.Instance.offlineVRRig)
    {
      GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = this.ropeId;
      GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = boneIndex;
      GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = xrNode == XRNode.LeftHand;
      GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
      GorillaTagger.Instance.offlineVRRig.grabbedRopeIsPhotonView = false;
    }
    this.RefreshAllBonesMass();
    List<Vector3> vector3List = new List<Vector3>();
    if (this.remotePlayers.Count <= 0)
    {
      foreach (Transform node in this.nodes)
        vector3List.Add(node.position);
    }
    velocity.y = 0.0f;
    if ((double) Time.time - (double) this.lastGrabTime > 1.0 && (this.remotePlayers.Count == 0 || (double) velocity.magnitude > 2.5))
      RopeSwingManager.instance.SendSetVelocity_RPC(this.ropeId, boneIndex, velocity, true);
    this.lastGrabTime = Time.time;
    this.ropeCreakSFX.transform.parent = this.GetBone(Math.Max(0, boneIndex - 3)).transform;
    this.ropeCreakSFX.transform.localPosition = Vector3.zero;
    this.localPlayerOn = true;
    this.localPlayerXRNode = xrNode;
    this.ToggleVelocityTracker(true, boneIndex, offset);
  }

  public void DetachLocalPlayer()
  {
    if (GorillaTagger.hasInstance && (bool) (UnityEngine.Object) GorillaTagger.Instance.offlineVRRig)
      GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
    this.localPlayerOn = false;
    this.localPlayerBoneIndex = 0;
    this.RefreshAllBonesMass();
  }

  private void ToggleVelocityTracker(bool enable, int boneIndex = 0, Vector3 offset = default (Vector3))
  {
    if (enable)
    {
      this.velocityTracker.transform.SetParent(this.GetBone(boneIndex));
      this.velocityTracker.transform.localPosition = offset;
      this.velocityTracker.ResetState();
    }
    this.velocityTracker.gameObject.SetActive(enable);
    if (!enable)
      return;
    this.velocityTracker.Tick();
  }

  private void RefreshAllBonesMass()
  {
    int furthestBoneIndex = 0;
    foreach (KeyValuePair<int, int> remotePlayer in this.remotePlayers)
    {
      if (remotePlayer.Value > furthestBoneIndex)
        furthestBoneIndex = remotePlayer.Value;
    }
    if (this.localPlayerBoneIndex > furthestBoneIndex)
      furthestBoneIndex = this.localPlayerBoneIndex;
    VectorizedCustomRopeSimulation.instance.SetMassForPlayers(this, this.hasPlayers, furthestBoneIndex);
  }

  public bool AttachRemotePlayer(
    int playerId,
    int boneIndex,
    Transform offsetTransform,
    Vector3 offset)
  {
    Transform bone = this.GetBone(boneIndex);
    if ((UnityEngine.Object) bone == (UnityEngine.Object) null)
      return false;
    offsetTransform.SetParent(bone.transform);
    offsetTransform.localPosition = offset;
    offsetTransform.localRotation = Quaternion.identity;
    if (this.remotePlayers.ContainsKey(playerId))
    {
      Debug.LogError((object) "already on the list!");
      return false;
    }
    this.remotePlayers.Add(playerId, boneIndex);
    this.RefreshAllBonesMass();
    return true;
  }

  public void DetachRemotePlayer(int playerId)
  {
    this.remotePlayers.Remove(playerId);
    this.RefreshAllBonesMass();
  }

  public void SetVelocity(
    int boneIndex,
    Vector3 velocity,
    bool wholeRope,
    PhotonMessageInfoWrapped info)
  {
    if (!this.isActiveAndEnabled || !velocity.IsValid())
      return;
    velocity.x = Mathf.Clamp(velocity.x, -100f, 100f);
    velocity.y = Mathf.Clamp(velocity.y, -100f, 100f);
    velocity.z = Mathf.Clamp(velocity.z, -100f, 100f);
    boneIndex = Mathf.Clamp(boneIndex, 0, this.nodes.Length);
    Transform bone = this.GetBone(boneIndex);
    if (!(bool) (UnityEngine.Object) bone)
      return;
    if (info.Sender != null && !info.Sender.IsLocal)
    {
      VRRig rigForPlayer = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
      if (!(bool) (UnityEngine.Object) rigForPlayer || (double) Vector3.Distance(bone.position, rigForPlayer.transform.position) > 5.0)
        return;
    }
    this.SetIsIdle(false);
    if (!(bool) (UnityEngine.Object) bone)
      return;
    VectorizedCustomRopeSimulation.instance.SetVelocity(this, velocity, wholeRope, boneIndex);
  }

  public void OnPieceCreate(int pieceType, int pieceId)
  {
    this.monkeBlockParent = this.GetComponentInParent<BuilderPiece>();
    this.hasMonkeBlockParent = (UnityEngine.Object) this.monkeBlockParent != (UnityEngine.Object) null;
    this.staticId = $"#ID_{StaticHash.Compute(pieceType, pieceId):X8}";
    this.ropeId = this.staticId.GetStaticHash();
    if (!this.started || RopeSwingManager.instance.TryGetRope(this.ropeId, out GorillaRopeSwing _))
      return;
    RopeSwingManager.Register(this);
  }

  public void OnPieceDestroy() => RopeSwingManager.Unregister(this);

  public void OnPiecePlacementDeserialized()
  {
    VectorizedCustomRopeSimulation.Unregister(this);
    this.transform.rotation = Quaternion.identity;
    this.scaleFactor = (float) (((double) this.transform.lossyScale.x + (double) this.transform.lossyScale.y + (double) this.transform.lossyScale.z) / 3.0);
    this.SetIsIdle(true, true);
    VectorizedCustomRopeSimulation.Register(this);
    if (!((UnityEngine.Object) this.monkeBlockParent != (UnityEngine.Object) null))
      return;
    this.supportMovingAtRuntime = this.IsAttachedToMovingPiece();
  }

  public void OnPieceActivate()
  {
    if (!((UnityEngine.Object) this.monkeBlockParent != (UnityEngine.Object) null))
      return;
    this.supportMovingAtRuntime = this.IsAttachedToMovingPiece();
  }

  private bool IsAttachedToMovingPiece()
  {
    return this.monkeBlockParent.attachIndex >= 0 && this.monkeBlockParent.attachIndex < this.monkeBlockParent.gridPlanes.Count && (UnityEngine.Object) this.monkeBlockParent.gridPlanes[this.monkeBlockParent.attachIndex].GetMovingParentGrid() != (UnityEngine.Object) null;
  }

  public void OnPieceDeactivate() => this.supportMovingAtRuntime = false;
}
