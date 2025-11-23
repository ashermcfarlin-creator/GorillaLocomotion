// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.EdibleWearable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using Photon.Pun;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class EdibleWearable : MonoBehaviour
{
  [Tooltip("Check when using non cosmetic edible items like honeycomb")]
  public bool isNonRespawnable;
  [Tooltip("Eating sounds are played through this AudioSource using PlayOneShot.")]
  public AudioSource audioSource;
  [Tooltip("Volume each bite should play at.")]
  public float volume = 0.08f;
  [Tooltip("The slot this cosmetic resides.")]
  public VRRig.WearablePackedStateSlots wearablePackedStateSlot = VRRig.WearablePackedStateSlots.LeftHand;
  [Tooltip("Time between bites.")]
  public float biteCooldown = 1f;
  [Tooltip("How long it takes to pop back to the uneaten state after being fully eaten.")]
  public float respawnTime = 7f;
  [Tooltip("Distance from mouth to item required to trigger a bite.")]
  public float biteDistance = 0.5f;
  [Tooltip("Offset from Gorilla's head to mouth.")]
  public Vector3 gorillaHeadMouthOffset = new Vector3(0.0f, 13f / 625f, 0.171f);
  [Tooltip("Offset from edible's transform to the bite point.")]
  public Vector3 edibleBiteOffset = new Vector3(0.0f, 0.0f, 0.0f);
  public EdibleWearable.EdibleStateInfo[] edibleStateInfos;
  private VRRig ownerRig;
  private bool isLocal;
  private bool isHandSlot;
  private bool isLeftHand;
  private GTBitOps.BitWriteInfo stateBitsWriteInfo;
  private int edibleState;
  private int previousEdibleState;
  private float lastEatTime;
  private float lastFullyEatenTime;
  private bool wasInBiteZoneLastFrame;

  protected void Awake()
  {
    this.edibleState = 0;
    this.previousEdibleState = 0;
    this.ownerRig = this.GetComponentInParent<VRRig>();
    this.isLocal = (UnityEngine.Object) this.ownerRig != (UnityEngine.Object) null && this.ownerRig.isOfflineVRRig;
    this.isHandSlot = this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.LeftHand || this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.RightHand;
    this.isLeftHand = this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.LeftHand;
    this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int) this.wearablePackedStateSlot];
  }

  protected void OnEnable()
  {
    if ((UnityEngine.Object) this.ownerRig == (UnityEngine.Object) null)
    {
      Debug.LogError((object) $"EdibleWearable \"{this.transform.GetPath()}\": Deactivating because ownerRig is null.", (UnityEngine.Object) this);
      this.gameObject.SetActive(false);
    }
    else
    {
      for (int index = 0; index < this.edibleStateInfos.Length; ++index)
        this.edibleStateInfos[index].gameObject.SetActive(index == this.edibleState);
    }
  }

  protected virtual void LateUpdate()
  {
    if (this.isLocal)
      this.LateUpdateLocal();
    else
      this.LateUpdateReplicated();
    this.LateUpdateShared();
  }

  protected virtual void LateUpdateLocal()
  {
    if (this.edibleState == this.edibleStateInfos.Length - 1)
    {
      if (!this.isNonRespawnable && (double) Time.time > (double) this.lastFullyEatenTime + (double) this.respawnTime)
      {
        this.edibleState = 0;
        this.previousEdibleState = 0;
        this.OnEdibleHoldableStateChange();
      }
      if (this.isNonRespawnable && (double) Time.time > (double) this.lastFullyEatenTime)
      {
        this.edibleState = 0;
        this.previousEdibleState = 0;
        this.OnEdibleHoldableStateChange();
        GorillaGameManager.instance.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer).netView.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, (object) false, (object) this.isLeftHand);
      }
    }
    else if ((double) Time.time > (double) this.lastEatTime + (double) this.biteCooldown)
    {
      Vector3 vector3_1 = this.transform.TransformPoint(this.edibleBiteOffset);
      bool flag = false;
      float num = this.biteDistance * this.biteDistance;
      if (!GorillaParent.hasInstance)
        return;
      Vector3 vector3_2 = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.TransformPoint(this.gorillaHeadMouthOffset) - vector3_1;
      if ((double) vector3_2.sqrMagnitude < (double) num)
        flag = true;
      foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
      {
        if (!flag)
        {
          if (vrrig.head != null)
          {
            if (!((UnityEngine.Object) vrrig.head.rigTarget == (UnityEngine.Object) null))
            {
              vector3_2 = vrrig.head.rigTarget.transform.TransformPoint(this.gorillaHeadMouthOffset) - vector3_1;
              if ((double) vector3_2.sqrMagnitude < (double) num)
                flag = true;
            }
            else
              break;
          }
          else
            break;
        }
      }
      if (flag && !this.wasInBiteZoneLastFrame && this.edibleState < this.edibleStateInfos.Length)
      {
        ++this.edibleState;
        this.lastEatTime = Time.time;
        this.lastFullyEatenTime = Time.time;
      }
      this.wasInBiteZoneLastFrame = flag;
    }
    this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, this.edibleState);
  }

  protected virtual void LateUpdateReplicated()
  {
    this.edibleState = GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
  }

  protected virtual void LateUpdateShared()
  {
    int edibleState = this.edibleState;
    if (edibleState != this.previousEdibleState)
      this.OnEdibleHoldableStateChange();
    this.previousEdibleState = edibleState;
  }

  protected virtual void OnEdibleHoldableStateChange()
  {
    if (this.previousEdibleState >= 0 && this.previousEdibleState < this.edibleStateInfos.Length)
      this.edibleStateInfos[this.previousEdibleState].gameObject.SetActive(false);
    if (this.edibleState >= 0 && this.edibleState < this.edibleStateInfos.Length)
      this.edibleStateInfos[this.edibleState].gameObject.SetActive(true);
    if (this.edibleState > 0 && this.edibleState < this.edibleStateInfos.Length && (UnityEngine.Object) this.audioSource != (UnityEngine.Object) null)
      this.audioSource.GTPlayOneShot(this.edibleStateInfos[this.edibleState].sound, this.volume);
    if (this.edibleState == this.edibleStateInfos.Length && (UnityEngine.Object) this.audioSource != (UnityEngine.Object) null)
      this.audioSource.GTPlayOneShot(this.edibleStateInfos[this.edibleState - 1].sound, this.volume);
    float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
    float fixedDeltaTime = Time.fixedDeltaTime;
    if (!this.isLocal || !this.isHandSlot)
      return;
    GorillaTagger.Instance.StartVibration(this.isLeftHand, amplitude, fixedDeltaTime);
  }

  [Serializable]
  public struct EdibleStateInfo
  {
    [Tooltip("Will be activated when this stage is reached.")]
    public GameObject gameObject;
    [Tooltip("Will be played when this stage is reached.")]
    public AudioClip sound;
  }
}
