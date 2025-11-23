// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.BackpackGrabbableCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class BackpackGrabbableCosmetic : HoldableObject
{
  [GorillaSoundLookup]
  public int materialIndex;
  [SerializeField]
  private bool useCapacity = true;
  [SerializeField]
  private float coolDownTimer = 2f;
  [SerializeField]
  private int maxCapacity;
  [SerializeField]
  private int startItemsCount;
  [Space]
  public UnityEvent OnReachedMaxCapacity;
  public UnityEvent OnFullyEmptied;
  public UnityEvent OnRefilled;
  private int currentItemsCount;
  private bool canGrab;
  private float lastGrabTime;

  private void Awake()
  {
    this.currentItemsCount = this.startItemsCount;
    this.canGrab = true;
  }

  public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
  {
  }

  public override void DropItemCleanup()
  {
  }

  public void Update()
  {
    if (this.canGrab || (double) Time.time - (double) this.lastGrabTime < (double) this.coolDownTimer)
      return;
    this.canGrab = true;
  }

  public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
  {
    if (this.IsEmpty())
    {
      Debug.LogWarning((object) "Can't remove item, Backpack is empty, need to refill.");
    }
    else
    {
      if (!this.canGrab)
        return;
      this.lastGrabTime = Time.time;
      this.canGrab = false;
      ((Object) grabbingHand == (Object) EquipmentInteractor.instance.leftHand ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance).TryCreateSnowball(this.materialIndex, out SnowballThrowable _);
      this.RemoveItem();
    }
  }

  public void AddItem()
  {
    if (!this.useCapacity)
      return;
    if (this.maxCapacity <= this.currentItemsCount)
    {
      Debug.LogWarning((object) "Can't add item, backpack is at full capacity.");
    }
    else
    {
      ++this.currentItemsCount;
      this.UpdateState();
    }
  }

  public void RemoveItem()
  {
    if (!this.useCapacity)
      return;
    if (this.currentItemsCount < 0)
    {
      Debug.LogWarning((object) "Can't remove item, Backpack is empty.");
    }
    else
    {
      --this.currentItemsCount;
      this.UpdateState();
    }
  }

  public void RefillBackpack()
  {
    if (!this.useCapacity || this.currentItemsCount == this.startItemsCount)
      return;
    this.currentItemsCount = this.startItemsCount;
    this.UpdateState();
  }

  public void EmptyBackpack()
  {
    if (!this.useCapacity || this.currentItemsCount == 0)
      return;
    this.currentItemsCount = 0;
    this.UpdateState();
  }

  public bool IsFull() => !this.useCapacity || this.maxCapacity == this.currentItemsCount;

  public bool IsEmpty() => this.useCapacity && this.currentItemsCount == 0;

  private void UpdateState()
  {
    if (!this.useCapacity)
      return;
    if (this.currentItemsCount == this.maxCapacity)
      this.OnReachedMaxCapacity?.Invoke();
    else if (this.currentItemsCount == 0)
    {
      this.OnFullyEmptied?.Invoke();
    }
    else
    {
      if (this.currentItemsCount != this.startItemsCount)
        return;
      this.OnRefilled?.Invoke();
    }
  }
}
