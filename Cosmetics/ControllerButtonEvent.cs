// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ControllerButtonEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

#nullable disable
namespace GorillaTag.Cosmetics;

public class ControllerButtonEvent : MonoBehaviour, ISpawnable
{
  [SerializeField]
  private float gripValue = 0.75f;
  [SerializeField]
  private float gripReleaseValue = 0.01f;
  [SerializeField]
  private float triggerValue = 0.75f;
  [SerializeField]
  private float triggerReleaseValue = 0.01f;
  [SerializeField]
  private ControllerButtonEvent.ButtonType buttonType;
  [Tooltip("How many frames should pass to trigger a press stayed button")]
  [SerializeField]
  private int frameInterval = 20;
  public UnityEvent<bool, float> onButtonPressed;
  public UnityEvent<bool, float> onButtonReleased;
  public UnityEvent<bool, float> onButtonPressStayed;
  private float triggerLastValue;
  private float gripLastValue;
  private bool primaryLastValue;
  private bool secondaryLastValue;
  private int frameCounter;
  private bool inLeftHand;
  private VRRig myRig;

  public bool IsSpawned { get; set; }

  public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

  public void OnSpawn(VRRig rig) => this.myRig = rig;

  public void OnDespawn()
  {
  }

  private bool IsMyItem() => (Object) this.myRig != (Object) null && this.myRig.isOfflineVRRig;

  private void Awake()
  {
    this.triggerLastValue = 0.0f;
    this.gripLastValue = 0.0f;
    this.primaryLastValue = false;
    this.secondaryLastValue = false;
    this.frameCounter = 0;
  }

  public void LateUpdate()
  {
    if (!this.IsMyItem())
      return;
    XRNode node = this.inLeftHand ? XRNode.LeftHand : XRNode.RightHand;
    switch (this.buttonType)
    {
      case ControllerButtonEvent.ButtonType.trigger:
        float num1 = ControllerInputPoller.TriggerFloat(node);
        if ((double) num1 > (double) this.triggerValue)
          ++this.frameCounter;
        if ((double) num1 > (double) this.triggerValue && (double) this.triggerLastValue < (double) this.triggerValue)
          this.onButtonPressed?.Invoke(this.inLeftHand, num1);
        else if ((double) num1 <= (double) this.triggerReleaseValue && (double) this.triggerLastValue > (double) this.triggerReleaseValue)
        {
          this.onButtonReleased?.Invoke(this.inLeftHand, num1);
          this.frameCounter = 0;
        }
        else if ((double) num1 > (double) this.triggerValue && (double) this.triggerLastValue >= (double) this.triggerValue && this.frameCounter % this.frameInterval == 0)
        {
          this.onButtonPressStayed?.Invoke(this.inLeftHand, num1);
          this.frameCounter = 0;
        }
        this.triggerLastValue = num1;
        break;
      case ControllerButtonEvent.ButtonType.primary:
        bool flag1 = ControllerInputPoller.PrimaryButtonPress(node);
        if (flag1)
          ++this.frameCounter;
        if (flag1 && !this.primaryLastValue)
          this.onButtonPressed?.Invoke(this.inLeftHand, 1f);
        else if (!flag1 && this.primaryLastValue)
        {
          this.onButtonReleased?.Invoke(this.inLeftHand, 0.0f);
          this.frameCounter = 0;
        }
        else if (flag1 && this.primaryLastValue && this.frameCounter % this.frameInterval == 0)
        {
          this.onButtonPressStayed?.Invoke(this.inLeftHand, 1f);
          this.frameCounter = 0;
        }
        this.primaryLastValue = flag1;
        break;
      case ControllerButtonEvent.ButtonType.secondary:
        bool flag2 = ControllerInputPoller.SecondaryButtonPress(node);
        if (flag2)
          ++this.frameCounter;
        if (flag2 && !this.secondaryLastValue)
          this.onButtonPressed?.Invoke(this.inLeftHand, 1f);
        else if (!flag2 && this.secondaryLastValue)
        {
          this.onButtonReleased?.Invoke(this.inLeftHand, 0.0f);
          this.frameCounter = 0;
        }
        else if (flag2 && this.secondaryLastValue && this.frameCounter % this.frameInterval == 0)
        {
          this.onButtonPressStayed?.Invoke(this.inLeftHand, 1f);
          this.frameCounter = 0;
        }
        this.secondaryLastValue = flag2;
        break;
      case ControllerButtonEvent.ButtonType.grip:
        float num2 = ControllerInputPoller.GripFloat(node);
        if ((double) num2 > (double) this.gripValue)
          ++this.frameCounter;
        if ((double) num2 > (double) this.gripValue && (double) this.gripLastValue < (double) this.gripValue)
          this.onButtonPressed?.Invoke(this.inLeftHand, num2);
        else if ((double) num2 <= (double) this.gripReleaseValue && (double) this.gripLastValue > (double) this.gripReleaseValue)
        {
          this.onButtonReleased?.Invoke(this.inLeftHand, num2);
          this.frameCounter = 0;
        }
        else if ((double) num2 > (double) this.gripValue && (double) this.gripLastValue >= (double) this.gripValue && this.frameCounter % this.frameInterval == 0)
        {
          this.onButtonPressStayed?.Invoke(this.inLeftHand, num2);
          this.frameCounter = 0;
        }
        this.gripLastValue = num2;
        break;
    }
  }

  private enum ButtonType
  {
    trigger,
    primary,
    secondary,
    grip,
  }
}
