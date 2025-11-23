// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.HeadlessHead
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class HeadlessHead : HoldableObject
{
  [Tooltip("The slot this cosmetic resides.")]
  public VRRig.WearablePackedStateSlots wearablePackedStateSlot = VRRig.WearablePackedStateSlots.Face;
  [SerializeField]
  private Vector3 offsetFromLeftHand = new Vector3(0.0f, 13f / 625f, 0.171f);
  [SerializeField]
  private Vector3 offsetFromRightHand = new Vector3(0.0f, 13f / 625f, 0.171f);
  [SerializeField]
  private Quaternion rotationFromLeftHand = Quaternion.Euler(14.0639734f, 52.56744f, 10.0674076f);
  [SerializeField]
  private Quaternion rotationFromRightHand = Quaternion.Euler(14.0639734f, 52.56744f, 10.0674076f);
  private Vector3 baseLocalPosition;
  private VRRig ownerRig;
  private bool isLocal;
  private bool isHeld;
  private bool isHeldLeftHand;
  private GTBitOps.BitWriteInfo stateBitsWriteInfo;
  [SerializeField]
  private MeshRenderer firstPersonRenderer;
  [SerializeField]
  private float firstPersonHiddenRadius;
  [SerializeField]
  private Transform firstPersonHideCenter;
  [SerializeField]
  private Transform holdAnchorPoint;
  private bool hasFirstPersonRenderer;
  private Vector3 blendingFromPosition;
  private Quaternion blendingFromRotation;
  private float blendFraction;
  private bool wasHeld;
  private bool wasHeldLeftHand;
  [SerializeField]
  private float blendDuration = 0.3f;

  protected void Awake()
  {
    this.ownerRig = this.GetComponentInParent<VRRig>();
    if ((Object) this.ownerRig == (Object) null)
      this.ownerRig = GorillaTagger.Instance.offlineVRRig;
    this.isLocal = this.ownerRig.isOfflineVRRig;
    this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int) this.wearablePackedStateSlot];
    this.baseLocalPosition = this.transform.localPosition;
    this.hasFirstPersonRenderer = (Object) this.firstPersonRenderer != (Object) null;
  }

  protected void OnEnable()
  {
    if ((Object) this.ownerRig == (Object) null)
    {
      Debug.LogError((object) $"HeadlessHead \"{this.transform.GetPath()}\": Deactivating because ownerRig is null.", (Object) this);
      this.gameObject.SetActive(false);
    }
    else
      this.ownerRig.bodyRenderer.SetCosmeticBodyType(GorillaBodyType.NoHead);
  }

  private void OnDisable()
  {
    this.ownerRig.bodyRenderer.SetCosmeticBodyType(GorillaBodyType.Default);
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
    this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, (this.isHeld ? 1 : 0) + (this.isHeldLeftHand ? 2 : 0));
  }

  protected virtual void LateUpdateReplicated()
  {
    int num = GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
    this.isHeld = num != 0;
    this.isHeldLeftHand = (num & 2) != 0;
  }

  protected virtual void LateUpdateShared()
  {
    if (this.isHeld != this.wasHeld || this.isHeldLeftHand != this.wasHeldLeftHand)
    {
      this.blendingFromPosition = this.transform.position;
      this.blendingFromRotation = this.transform.rotation;
      this.blendFraction = 0.0f;
    }
    Quaternion b1;
    Vector3 b2;
    if (this.isHeldLeftHand)
    {
      b1 = this.ownerRig.leftHandTransform.rotation * this.rotationFromLeftHand;
      b2 = this.ownerRig.leftHandTransform.TransformPoint(this.offsetFromLeftHand) - b1 * this.holdAnchorPoint.transform.localPosition;
    }
    else if (this.isHeld)
    {
      b1 = this.ownerRig.rightHandTransform.rotation * this.rotationFromRightHand;
      b2 = this.ownerRig.rightHandTransform.TransformPoint(this.offsetFromRightHand) - b1 * this.holdAnchorPoint.transform.localPosition;
    }
    else
    {
      b1 = this.transform.parent.rotation;
      b2 = this.transform.parent.TransformPoint(this.baseLocalPosition);
    }
    if ((double) this.blendFraction < 1.0)
    {
      this.blendFraction += Time.deltaTime / this.blendDuration;
      b1 = Quaternion.Lerp(this.blendingFromRotation, b1, this.blendFraction);
      b2 = Vector3.Lerp(this.blendingFromPosition, b2, this.blendFraction);
    }
    this.transform.rotation = b1;
    this.transform.position = b2;
    if (this.hasFirstPersonRenderer)
      this.firstPersonRenderer.enabled = (this.firstPersonHideCenter.transform.position - GTPlayer.Instance.headCollider.transform.position).IsLongerThan(this.firstPersonHiddenRadius * this.transform.lossyScale.x);
    this.wasHeld = this.isHeld;
    this.wasHeldLeftHand = this.isHeldLeftHand;
  }

  public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
  {
  }

  public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
  {
    this.isHeld = true;
    this.isHeldLeftHand = (Object) grabbingHand == (Object) EquipmentInteractor.instance.leftHand;
    EquipmentInteractor.instance.UpdateHandEquipment((IHoldableObject) this, this.isHeldLeftHand);
  }

  public override void DropItemCleanup()
  {
    this.isHeld = false;
    this.isHeldLeftHand = false;
  }

  public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
  {
    if (EquipmentInteractor.instance.rightHandHeldEquipment == this && (Object) releasingHand != (Object) EquipmentInteractor.instance.rightHand || EquipmentInteractor.instance.leftHandHeldEquipment == this && (Object) releasingHand != (Object) EquipmentInteractor.instance.leftHand)
      return false;
    EquipmentInteractor.instance.UpdateHandEquipment((IHoldableObject) null, this.isHeldLeftHand);
    this.isHeld = false;
    this.isHeldLeftHand = false;
    return true;
  }
}
