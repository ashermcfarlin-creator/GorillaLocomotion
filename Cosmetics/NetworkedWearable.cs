// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.NetworkedWearable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaNetworking;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag.Cosmetics;

public class NetworkedWearable : MonoBehaviour, ISpawnable, ITickSystemTick
{
  [Tooltip("Whether the wearable state is toggled on by default.")]
  [SerializeField]
  private bool startTrue;
  [Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
  [SerializeField]
  private CosmeticsController.CosmeticCategory assignedSlot;
  [FormerlySerializedAs("IsTwoHanded")]
  [SerializeField]
  private bool isTwoHanded;
  private const string listenInfo = "listenForChangesLocal should be false in most cases";
  private const string listenDetails = "listenForChangesLocal should be false in most cases\nIf you have a first person part and a local rig part that both need to react to a state change\ncall the Toggle/Set functions to change the state from one prefab and check \nlistenForChangesLocal on the other prefab ";
  [SerializeField]
  private bool listenForChangesLocal;
  private VRRig.WearablePackedStateSlots wearableSlot;
  private VRRig.WearablePackedStateSlots leftSlot = VRRig.WearablePackedStateSlots.LeftHand;
  private VRRig.WearablePackedStateSlots rightSlot = VRRig.WearablePackedStateSlots.RightHand;
  private VRRig myRig;
  private bool isLocal;
  private bool value;
  private bool leftHandValue;
  private bool rightHandValue;
  [SerializeField]
  protected UnityEvent OnWearableStateTrue;
  [SerializeField]
  protected UnityEvent OnWearableStateFalse;
  [SerializeField]
  protected UnityEvent OnLeftWearableStateTrue;
  [SerializeField]
  protected UnityEvent OnLeftWearableStateFalse;
  [SerializeField]
  protected UnityEvent OnRightWearableStateTrue;
  [SerializeField]
  protected UnityEvent OnRightWearableStateFalse;

  private void Awake()
  {
    if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw)
      this.isTwoHanded = false;
    this.wearableSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, true);
    this.leftSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, true);
    this.rightSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, false);
  }

  private void OnEnable()
  {
    if (!this.IsSpawned)
      return;
    if (this.isLocal && !this.listenForChangesLocal)
    {
      this.SetWearableStateBool(this.startTrue);
    }
    else
    {
      if (this.TickRunning)
        return;
      TickSystem<object>.AddTickCallback((ITickSystemTick) this);
    }
  }

  public void ToggleWearableStateBool()
  {
    if (!this.isLocal || !this.IsSpawned || !NetworkedWearable.IsCategoryValid(this.assignedSlot) || (Object) this.myRig == (Object) null)
      return;
    if (this.listenForChangesLocal)
      GTDev.LogError<string>($"NetworkedWearable with listenForChangesLocal calling ToggleWearableStateBool on object {this.gameObject.name}.You should not change state from a listener");
    else if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.isTwoHanded)
    {
      GTDev.LogWarning<string>($"NetworkedWearable calling ToggleWearableStateBool on two handed object {this.gameObject.name}. please use ToggleLeftWearableStateBool or ToggleRightWearableStateBool instead");
      this.ToggleLeftWearableStateBool();
    }
    else
    {
      this.value = !this.value;
      this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int) this.wearableSlot, this.value);
      this.OnWearableStateChanged();
    }
  }

  public void SetWearableStateBool(bool newState)
  {
    if (!this.isLocal || !this.IsSpawned || !NetworkedWearable.IsCategoryValid(this.assignedSlot) || (Object) this.myRig == (Object) null)
      return;
    if (this.listenForChangesLocal)
      GTDev.LogError<string>($"NetworkedWearable with listenForChangesLocal calling SetWearableStateBool on object {this.gameObject.name}.You should not change state from a listener");
    else if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.isTwoHanded)
    {
      GTDev.LogWarning<string>($"NetworkedWearable calling SetWearableStateBool on two handed object {this.gameObject.name}. please use SetLeftWearableStateBool or SetRightWearableStateBool instead");
      this.SetLeftWearableStateBool(newState);
    }
    else
    {
      if (this.value == newState)
        return;
      this.value = newState;
      this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int) this.wearableSlot, this.value);
      this.OnWearableStateChanged();
    }
  }

  public void ToggleLeftWearableStateBool()
  {
    if (!this.isLocal || !this.IsSpawned || !NetworkedWearable.IsCategoryValid(this.assignedSlot) || (Object) this.myRig == (Object) null)
      return;
    if (this.listenForChangesLocal)
      GTDev.LogError<string>($"NetworkedWearable with listenForChangesLocal calling ToggleLeftWearableStateBool on object {this.gameObject.name}.You should not change state from a listener");
    else if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
    {
      GTDev.LogWarning<string>($"NetworkedWearable calling ToggleLeftWearableStateBool on one handed object {this.gameObject.name}. Please use ToggleWearableStateBool instead");
      this.ToggleWearableStateBool();
    }
    else
    {
      this.leftHandValue = !this.leftHandValue;
      this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int) this.leftSlot, this.leftHandValue);
      this.OnLeftStateChanged();
    }
  }

  public void ToggleRightWearableStateBool()
  {
    if (!this.isLocal || !this.IsSpawned || !NetworkedWearable.IsCategoryValid(this.assignedSlot) || (Object) this.myRig == (Object) null)
      return;
    if (this.listenForChangesLocal)
      GTDev.LogError<string>($"NetworkedWearable with listenForChangesLocal calling ToggleRightWearableStateBool on object {this.gameObject.name}.You should not change state from a listener");
    else if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
    {
      GTDev.LogWarning<string>($"NetworkedWearable calling ToggleRightWearableStateBool on one handed object {this.gameObject.name}. Please use ToggleWearableStateBool instead");
      this.ToggleWearableStateBool();
    }
    else
    {
      this.rightHandValue = !this.rightHandValue;
      this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int) this.rightSlot, this.rightHandValue);
      this.OnRightStateChanged();
    }
  }

  public void SetLeftWearableStateBool(bool newState)
  {
    if (!this.isLocal || !this.IsSpawned || !NetworkedWearable.IsCategoryValid(this.assignedSlot) || (Object) this.myRig == (Object) null)
      return;
    if (this.listenForChangesLocal)
      GTDev.LogError<string>($"NetworkedWearable with listenForChangesLocal calling SetLeftWearableStateBool on object {this.gameObject.name}.You should not change state from a listener");
    else if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
    {
      GTDev.LogWarning<string>($"NetworkedWearable calling SetLeftWearableStateBool on one handed object {this.gameObject.name}. Please use SetWearableStateBool instead");
      this.SetWearableStateBool(newState);
    }
    else
    {
      if (this.leftHandValue == newState)
        return;
      this.leftHandValue = newState;
      this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int) this.leftSlot, this.leftHandValue);
      this.OnLeftStateChanged();
    }
  }

  public void SetRightWearableStateBool(bool newState)
  {
    if (!this.isLocal || !this.IsSpawned || !NetworkedWearable.IsCategoryValid(this.assignedSlot) || (Object) this.myRig == (Object) null)
      return;
    if (this.listenForChangesLocal)
      GTDev.LogError<string>($"NetworkedWearable with listenForChangesLocal calling SetRightWearableStateBool on object {this.gameObject.name}.You should not change state from a listener");
    else if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
    {
      GTDev.LogWarning<string>($"NetworkedWearable calling SetRightWearableStateBool on one handed object {this.gameObject.name}. Please use SetWearableStateBool instead");
      this.SetWearableStateBool(newState);
    }
    else
    {
      if (this.rightHandValue == newState)
        return;
      this.rightHandValue = newState;
      this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int) this.rightSlot, this.rightHandValue);
      this.OnRightStateChanged();
    }
  }

  public void OnDisable()
  {
    if (this.isLocal && !this.listenForChangesLocal)
    {
      this.SetWearableStateBool(false);
    }
    else
    {
      if (!this.TickRunning)
        return;
      TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
    }
  }

  private void OnWearableStateChanged()
  {
    if (this.value)
      this.OnWearableStateTrue?.Invoke();
    else
      this.OnWearableStateFalse?.Invoke();
  }

  private void OnLeftStateChanged()
  {
    if (this.leftHandValue)
      this.OnLeftWearableStateTrue?.Invoke();
    else
      this.OnLeftWearableStateFalse?.Invoke();
  }

  private void OnRightStateChanged()
  {
    if (this.rightHandValue)
      this.OnRightWearableStateTrue?.Invoke();
    else
      this.OnRightWearableStateFalse?.Invoke();
  }

  public bool IsSpawned { get; set; }

  public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

  public void OnSpawn(VRRig rig)
  {
    if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.CosmeticSelectedSide == ECosmeticSelectSide.Both)
      GTDev.LogWarning<string>($"NetworkedWearable: Cosmetic {this.gameObject.name} with category {this.assignedSlot} has select side Both, assuming left side!");
    if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
      GTDev.LogError<string>($"NetworkedWearable: Cosmetic {this.gameObject.name} spawned with invalid category {this.assignedSlot}!");
    this.myRig = rig;
    this.isLocal = rig.isLocal;
    this.wearableSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, this.CosmeticSelectedSide != ECosmeticSelectSide.Right);
    Debug.Log((object) $"Networked Wearable {this.gameObject.name} Select Side {this.CosmeticSelectedSide} slot {this.wearableSlot}");
  }

  public void OnDespawn()
  {
  }

  public bool TickRunning { get; set; }

  public void Tick()
  {
    if (this.isLocal && !this.listenForChangesLocal || !this.IsSpawned)
      return;
    if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.isTwoHanded)
    {
      bool flag1 = GTBitOps.ReadBit(this.myRig.WearablePackedStates, (int) this.leftSlot);
      if (this.leftHandValue != flag1)
      {
        this.leftHandValue = flag1;
        this.OnLeftStateChanged();
      }
      bool flag2 = GTBitOps.ReadBit(this.myRig.WearablePackedStates, (int) this.rightSlot);
      if (this.rightHandValue == flag2)
        return;
      this.rightHandValue = flag2;
      this.OnRightStateChanged();
    }
    else
    {
      bool flag = GTBitOps.ReadBit(this.myRig.WearablePackedStates, (int) this.wearableSlot);
      if (this.value == flag)
        return;
      this.value = flag;
      this.OnWearableStateChanged();
    }
  }

  public static bool IsCategoryValid(CosmeticsController.CosmeticCategory category)
  {
    switch (category)
    {
      case CosmeticsController.CosmeticCategory.Hat:
      case CosmeticsController.CosmeticCategory.Badge:
      case CosmeticsController.CosmeticCategory.Face:
      case CosmeticsController.CosmeticCategory.Paw:
      case CosmeticsController.CosmeticCategory.Fur:
      case CosmeticsController.CosmeticCategory.Shirt:
      case CosmeticsController.CosmeticCategory.Pants:
        return true;
      default:
        return false;
    }
  }

  private VRRig.WearablePackedStateSlots CosmeticCategoryToWearableSlot(
    CosmeticsController.CosmeticCategory category,
    bool isLeft)
  {
    switch (category)
    {
      case CosmeticsController.CosmeticCategory.Hat:
        return VRRig.WearablePackedStateSlots.Hat;
      case CosmeticsController.CosmeticCategory.Badge:
        return VRRig.WearablePackedStateSlots.Badge;
      case CosmeticsController.CosmeticCategory.Face:
        return VRRig.WearablePackedStateSlots.Face;
      case CosmeticsController.CosmeticCategory.Paw:
        return !isLeft ? VRRig.WearablePackedStateSlots.RightHand : VRRig.WearablePackedStateSlots.LeftHand;
      case CosmeticsController.CosmeticCategory.Fur:
        return VRRig.WearablePackedStateSlots.Fur;
      case CosmeticsController.CosmeticCategory.Shirt:
        return VRRig.WearablePackedStateSlots.Shirt;
      case CosmeticsController.CosmeticCategory.Pants:
        return VRRig.WearablePackedStateSlots.Pants1;
      default:
        GTDev.LogWarning<string>($"NetworkedWearable: {category} item cannot set wearable state");
        return VRRig.WearablePackedStateSlots.Hat;
    }
  }
}
