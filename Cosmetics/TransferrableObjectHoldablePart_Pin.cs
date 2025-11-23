// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.TransferrableObjectHoldablePart_Pin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class TransferrableObjectHoldablePart_Pin : TransferrableObjectHoldablePart
{
  [SerializeField]
  private float breakStrengthThreshold = 0.8f;
  [SerializeField]
  private float maxHandSnapDistance = 0.5f;
  [SerializeField]
  private Transform pin;
  public UnityEvent OnBreak;
  public UnityEvent OnBreakLocal;
  public UnityEvent OnEnableHoldable;

  protected void OnEnable() => this.OnEnableHoldable?.Invoke();

  protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
  {
    if (!rig.isOfflineVRRig)
      return;
    Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(isHeldLeftHand);
    if ((double) GTPlayer.Instance.GetInteractPointVelocityTracker(isHeldLeftHand).GetAverageVelocity(true).magnitude > (double) this.breakStrengthThreshold || (controllerTransform.position - this.pin.transform.position).IsLongerThan(this.maxHandSnapDistance))
    {
      this.OnRelease((DropZone) null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
      this.OnBreak?.Invoke();
      if (!(bool) (Object) this.transferrableParentObject || !this.transferrableParentObject.IsMyItem())
        return;
      this.OnBreakLocal?.Invoke();
    }
    else
      controllerTransform.position = this.pin.position;
  }
}
