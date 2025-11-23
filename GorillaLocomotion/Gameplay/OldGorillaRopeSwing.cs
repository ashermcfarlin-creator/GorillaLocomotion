// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.OldGorillaRopeSwing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

#nullable disable
namespace GorillaLocomotion.Gameplay;

public class OldGorillaRopeSwing : MonoBehaviourPun
{
  public const float kPlayerMass = 0.8f;
  public const float ropeBitGenOffset = 1f;
  public const float MAX_ROPE_SPEED = 15f;
  [SerializeField]
  private GameObject prefabRopeBit;
  public Rigidbody[] bones = Array.Empty<Rigidbody>();
  private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();
  [NonSerialized]
  public float lastGrabTime;
  [SerializeField]
  private AudioSource ropeCreakSFX;
  private bool localPlayerOn;
  private XRNode localPlayerXRNode;
  private Rigidbody localGrabbedRigid;
  private const float MAX_VELOCITY_FOR_IDLE = 0.1f;
  private const float TIME_FOR_IDLE = 2f;
  private float potentialIdleTimer;
  [Header("Config")]
  [SerializeField]
  private int ropeLength = 8;
  [SerializeField]
  private GorillaRopeSwingSettings settings;

  public bool isIdle { get; private set; }

  private void Awake() => this.SetIsIdle(true);

  private void OnDisable()
  {
    if (this.isIdle)
      return;
    this.SetIsIdle(true);
  }

  private void Update()
  {
    if (this.localPlayerOn && (bool) (UnityEngine.Object) this.localGrabbedRigid)
    {
      double magnitude = (double) this.localGrabbedRigid.linearVelocity.magnitude;
      if (magnitude > 2.5 && !this.ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
        this.ropeCreakSFX.GTPlay();
      float amplitude = MathUtils.Linear((float) magnitude, 0.0f, 10f, -0.07f, 0.5f);
      if ((double) amplitude > 0.0)
        GorillaTagger.Instance.DoVibration(this.localPlayerXRNode, amplitude, Time.deltaTime);
    }
    if (this.isIdle)
      return;
    if (!this.localPlayerOn && this.remotePlayers.Count == 0)
    {
      foreach (Rigidbody bone in this.bones)
      {
        float magnitude = bone.linearVelocity.magnitude;
        float maxDistanceDelta = Time.deltaTime * this.settings.frictionWhenNotHeld;
        if ((double) maxDistanceDelta < (double) magnitude - 0.10000000149011612)
          bone.linearVelocity = Vector3.MoveTowards(bone.linearVelocity, Vector3.zero, maxDistanceDelta);
      }
    }
    bool flag = false;
    for (int index = 0; index < this.bones.Length; ++index)
    {
      if ((double) this.bones[index].linearVelocity.magnitude > 0.10000000149011612)
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      this.potentialIdleTimer += Time.deltaTime;
    else
      this.potentialIdleTimer = 0.0f;
    if ((double) this.potentialIdleTimer < 2.0)
      return;
    this.SetIsIdle(true);
    this.potentialIdleTimer = 0.0f;
  }

  private void SetIsIdle(bool idle)
  {
    this.isIdle = idle;
    this.ToggleIsKinematic(idle);
    if (!idle)
      return;
    for (int index = 0; index < this.bones.Length; ++index)
    {
      this.bones[index].linearVelocity = Vector3.zero;
      this.bones[index].angularVelocity = Vector3.zero;
      this.bones[index].transform.localRotation = Quaternion.identity;
    }
  }

  private void ToggleIsKinematic(bool kinematic)
  {
    for (int index = 0; index < this.bones.Length; ++index)
    {
      this.bones[index].isKinematic = kinematic;
      this.bones[index].interpolation = !kinematic ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
    }
  }

  public Rigidbody GetBone(int index)
  {
    return index >= this.bones.Length ? ((IEnumerable<Rigidbody>) this.bones).Last<Rigidbody>() : this.bones[index];
  }

  public int GetBoneIndex(Rigidbody r)
  {
    for (int boneIndex = 0; boneIndex < this.bones.Length; ++boneIndex)
    {
      if ((UnityEngine.Object) this.bones[boneIndex] == (UnityEngine.Object) r)
        return boneIndex;
    }
    return this.bones.Length - 1;
  }

  public void AttachLocalPlayer(XRNode xrNode, Rigidbody rigid, Vector3 offset, Vector3 velocity)
  {
    int boneIndex = this.GetBoneIndex(rigid);
    velocity *= this.settings.inheritVelocityMultiplier;
    if (GorillaTagger.hasInstance && (bool) (UnityEngine.Object) GorillaTagger.Instance.offlineVRRig)
    {
      GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = this.photonView.ViewID;
      GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = boneIndex;
      GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = xrNode == XRNode.LeftHand;
      GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
    }
    List<Vector3> vector3List1 = new List<Vector3>();
    List<Vector3> vector3List2 = new List<Vector3>();
    if (this.remotePlayers.Count <= 0)
    {
      foreach (Rigidbody bone in this.bones)
      {
        vector3List1.Add(bone.transform.localEulerAngles);
        vector3List2.Add(bone.linearVelocity);
      }
    }
    if ((double) Time.time - (double) this.lastGrabTime > 1.0 && (this.remotePlayers.Count == 0 || (double) velocity.magnitude > 2.0))
      this.SetVelocity_RPC(boneIndex, velocity, ropeRotations: vector3List1.ToArray(), ropeVelocities: vector3List2.ToArray());
    this.lastGrabTime = Time.time;
    this.ropeCreakSFX.transform.parent = this.GetBone(Math.Max(0, boneIndex - 2)).transform;
    this.ropeCreakSFX.transform.localPosition = Vector3.zero;
    this.localPlayerOn = true;
    this.localPlayerXRNode = xrNode;
    this.localGrabbedRigid = rigid;
  }

  public void DetachLocalPlayer()
  {
    if (GorillaTagger.hasInstance && (bool) (UnityEngine.Object) GorillaTagger.Instance.offlineVRRig)
      GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
    this.localPlayerOn = false;
    this.localGrabbedRigid = (Rigidbody) null;
  }

  public bool AttachRemotePlayer(
    int playerId,
    int boneIndex,
    Transform offsetTransform,
    Vector3 offset)
  {
    Rigidbody bone = this.GetBone(boneIndex);
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
    return true;
  }

  public void DetachRemotePlayer(int playerId) => this.remotePlayers.Remove(playerId);

  public void SetVelocity_RPC(
    int boneIndex,
    Vector3 velocity,
    bool wholeRope = true,
    Vector3[] ropeRotations = null,
    Vector3[] ropeVelocities = null)
  {
    if (NetworkSystem.Instance.InRoom)
      this.photonView.RPC("SetVelocity", RpcTarget.All, (object) boneIndex, (object) velocity, (object) wholeRope, (object) ropeRotations, (object) ropeVelocities);
    else
      this.SetVelocity(boneIndex, velocity, wholeRope, ropeRotations, ropeVelocities);
  }

  [PunRPC]
  public void SetVelocity(
    int boneIndex,
    Vector3 velocity,
    bool wholeRope = true,
    Vector3[] ropeRotations = null,
    Vector3[] ropeVelocities = null)
  {
    this.SetIsIdle(false);
    if (ropeRotations != null && ropeVelocities != null && ropeRotations.Length != 0)
    {
      this.ToggleIsKinematic(true);
      for (int index = 0; index < ropeRotations.Length; ++index)
      {
        if (index != 0)
        {
          this.bones[index].transform.localRotation = Quaternion.Euler(ropeRotations[index]);
          this.bones[index].linearVelocity = ropeVelocities[index];
        }
      }
      this.ToggleIsKinematic(false);
    }
    Rigidbody bone1 = this.GetBone(boneIndex);
    if (!(bool) (UnityEngine.Object) bone1)
      return;
    if (wholeRope)
    {
      int num = 0;
      float maxLength = Mathf.Min(velocity.magnitude, 15f);
      foreach (Rigidbody bone2 in this.bones)
      {
        bone2.linearVelocity = Vector3.ClampMagnitude(velocity / (float) boneIndex * (float) num, maxLength);
        ++num;
      }
    }
    else
      bone1.linearVelocity = velocity;
  }
}
