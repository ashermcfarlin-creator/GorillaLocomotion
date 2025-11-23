// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.FingerFlexEvent2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using System;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class FingerFlexEvent2 : MonoBehaviour, ITickSystemTick
{
  public FingerFlexEvent2.FlexEvent[] list;
  private VRRig myRig;
  private TransferrableObject myTransferrable;

  private bool TryLinkToNextEvent(int index)
  {
    if (index < this.list.Length - 1)
    {
      if (this.list[index].IsFlexTrigger && this.list[index + 1].IsReleaseTrigger)
      {
        this.list[index].linkIndex = index + 1;
        this.list[index + 1].linkIndex = index;
        return true;
      }
      this.list[index + 1].linkIndex = -1;
    }
    this.list[index].linkIndex = -1;
    return false;
  }

  private void Awake()
  {
    this.myRig = this.GetComponentInParent<VRRig>();
    this.myTransferrable = this.GetComponentInParent<TransferrableObject>();
    for (int index = 0; index < this.list.Length; ++index)
    {
      FingerFlexEvent2.FlexEvent flexEvent1 = this.list[index];
      if (this.myTransferrable.IsNull() && flexEvent1.UsesTransferrable)
        this.myTransferrable = this.GetComponentInParent<TransferrableObject>();
      if (flexEvent1.tryLink && this.TryLinkToNextEvent(index))
      {
        FingerFlexEvent2.FlexEvent flexEvent2 = this.list[index + 1];
        flexEvent1.releaseThreshold = flexEvent2.releaseThreshold;
        flexEvent2.flexThreshold = flexEvent1.flexThreshold;
        flexEvent2.fingerType = flexEvent1.fingerType;
        flexEvent2.handType = flexEvent1.handType;
        flexEvent2.networked = flexEvent1.networked;
        ++index;
      }
    }
  }

  private void CalcFlex(bool disable)
  {
    for (int index = 0; index < this.list.Length; ++index)
    {
      FingerFlexEvent2.FlexEvent flexEvent1 = this.list[index];
      if ((flexEvent1.networked || this.myRig.isOfflineVRRig) && (!flexEvent1.UsesTransferrable || !this.myTransferrable.IsNull()))
      {
        bool leftHand = false;
        bool flag1 = false;
        bool flag2 = false;
        switch (flexEvent1.handType)
        {
          case FingerFlexEvent2.FlexEvent.HandType.TransferrableHeldHand:
            leftHand = this.myTransferrable.currentState == TransferrableObject.PositionState.InLeftHand;
            flag1 = this.myTransferrable.currentState == TransferrableObject.PositionState.InRightHand;
            flag2 = leftHand | flag1;
            break;
          case FingerFlexEvent2.FlexEvent.HandType.TransferrableEquippedSide:
            leftHand = (this.myTransferrable.storedZone & (BodyDockPositions.DropPositions.LeftArm | BodyDockPositions.DropPositions.LeftBack)) != 0;
            flag1 = (this.myTransferrable.storedZone & (BodyDockPositions.DropPositions.RightArm | BodyDockPositions.DropPositions.RightBack)) != 0;
            break;
          case FingerFlexEvent2.FlexEvent.HandType.LeftHand:
            leftHand = true;
            break;
          case FingerFlexEvent2.FlexEvent.HandType.RightHand:
            flag1 = true;
            break;
        }
        if (!(leftHand & flag1) && (leftHand || flag1 || flexEvent1.wasHeld))
        {
          float num1;
          if (disable || flexEvent1.wasHeld && !flag2)
          {
            num1 = 0.0f;
          }
          else
          {
            FingerFlexEvent2.FlexEvent.FingerType fingerType = flexEvent1.fingerType;
            float num2;
            switch (fingerType)
            {
              case FingerFlexEvent2.FlexEvent.FingerType.Thumb:
                num2 = leftHand ? this.myRig.leftThumb.calcT : this.myRig.rightThumb.calcT;
                break;
              case FingerFlexEvent2.FlexEvent.FingerType.Index:
                num2 = leftHand ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT;
                break;
              case FingerFlexEvent2.FlexEvent.FingerType.Middle:
                num2 = leftHand ? this.myRig.leftMiddle.calcT : this.myRig.rightMiddle.calcT;
                break;
              case FingerFlexEvent2.FlexEvent.FingerType.IndexAndMiddle:
                num2 = leftHand ? Mathf.Min(this.myRig.leftIndex.calcT, this.myRig.leftMiddle.calcT) : Mathf.Min(this.myRig.rightIndex.calcT, this.myRig.rightMiddle.calcT);
                break;
              case FingerFlexEvent2.FlexEvent.FingerType.IndexOrMiddle:
                num2 = leftHand ? Mathf.Max(this.myRig.leftIndex.calcT, this.myRig.leftMiddle.calcT) : Mathf.Max(this.myRig.rightIndex.calcT, this.myRig.rightMiddle.calcT);
                break;
              default:
                // ISSUE: reference to a compiler-generated method
                \u003CPrivateImplementationDetails\u003E.ThrowSwitchExpressionException((object) fingerType);
                break;
            }
            num1 = num2;
          }
          float flexValue = num1;
          flexEvent1.ProcessState(leftHand, flexValue);
          flexEvent1.wasHeld = flag2 && !disable;
          if (flexEvent1.IsLinked)
          {
            FingerFlexEvent2.FlexEvent flexEvent2 = this.list[index + 1];
            flexEvent2.ProcessState(leftHand, flexValue);
            flexEvent2.wasHeld = flag2;
            ++index;
          }
        }
      }
    }
  }

  public void OnEnable() => TickSystem<object>.AddTickCallback((ITickSystemTick) this);

  public void OnDisable()
  {
    TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
    this.CalcFlex(true);
  }

  public bool TickRunning { get; set; }

  public void Tick() => this.CalcFlex(false);

  [Serializable]
  public class FlexEvent
  {
    public FingerFlexEvent2.FlexEvent.TriggerType triggerType;
    public bool tryLink = true;
    [HideInInspector]
    public int linkIndex = -1;
    [Space]
    public FingerFlexEvent2.FlexEvent.FingerType fingerType = FingerFlexEvent2.FlexEvent.FingerType.Index;
    [Space]
    public FingerFlexEvent2.FlexEvent.HandType handType;
    private const string ADVANCED = "Advanced Properties";
    [Tooltip("When this is checked, all players in the room will fire the event. Otherwise, only the local player will fire it. You should usually leave this on, unless you're using it for something local like controller haptics.")]
    public bool networked = true;
    [Range(0.01f, 0.75f)]
    public float flexThreshold = 0.75f;
    [Range(0.01f, 1f)]
    public float releaseThreshold = 0.01f;
    public ContinuousPropertyArray continuousProperties;
    public UnityEvent<bool, float> unityEvent;
    [NonSerialized]
    public bool wasHeld;
    [NonSerialized]
    public bool marginError;
    private FingerFlexEvent2.FlexEvent.RangeState currentState;
    private FingerFlexEvent2.FlexEvent.RangeState lastState;
    private float lastThresholdTime = -100000f;

    public bool IsFlexTrigger => this.triggerType == FingerFlexEvent2.FlexEvent.TriggerType.OnFlex;

    public bool IsReleaseTrigger
    {
      get => this.triggerType == FingerFlexEvent2.FlexEvent.TriggerType.OnRelease;
    }

    public bool UsesTransferrable
    {
      get
      {
        FingerFlexEvent2.FlexEvent.HandType handType = this.handType;
        return (handType == FingerFlexEvent2.FlexEvent.HandType.TransferrableHeldHand ? 0 : (handType != FingerFlexEvent2.FlexEvent.HandType.TransferrableEquippedSide ? 1 : 0)) == 0;
      }
    }

    public bool HasValidLink => this.linkIndex >= 0;

    public bool IsLinked => this.tryLink && this.linkIndex >= 0;

    private bool ShowMainProperties => !this.IsLinked || this.IsFlexTrigger;

    private bool ShowFlexThreshold => this.ShowMainProperties;

    private bool ShowReleaseThreshold
    {
      get => (!this.IsLinked || this.IsReleaseTrigger) && !this.IsFlexTrigger;
    }

    public void ProcessState(bool leftHand, float flexValue)
    {
      this.currentState = (double) flexValue < (double) this.releaseThreshold ? FingerFlexEvent2.FlexEvent.RangeState.Below : ((double) flexValue >= (double) this.flexThreshold ? FingerFlexEvent2.FlexEvent.RangeState.Above : FingerFlexEvent2.FlexEvent.RangeState.Within);
      if (this.ShowMainProperties && this.currentState != this.lastState && this.continuousProperties != null && this.continuousProperties.Count > 0)
        this.continuousProperties.ApplyAll(Mathf.InverseLerp(this.releaseThreshold, this.flexThreshold, flexValue));
      if (this.currentState == FingerFlexEvent2.FlexEvent.RangeState.Above && this.lastState == FingerFlexEvent2.FlexEvent.RangeState.Below)
      {
        this.lastThresholdTime = Time.time;
        this.lastState = FingerFlexEvent2.FlexEvent.RangeState.Above;
        if (!this.IsFlexTrigger)
          return;
        this.unityEvent?.Invoke(leftHand, flexValue);
      }
      else
      {
        if (this.currentState != FingerFlexEvent2.FlexEvent.RangeState.Below || this.lastState != FingerFlexEvent2.FlexEvent.RangeState.Above)
          return;
        this.lastThresholdTime = Time.time;
        this.lastState = FingerFlexEvent2.FlexEvent.RangeState.Below;
        if (!this.IsReleaseTrigger)
          return;
        this.unityEvent?.Invoke(leftHand, flexValue);
      }
    }

    public enum TriggerType
    {
      OnFlex = 0,
      OnRelease = 2,
    }

    public enum FingerType
    {
      Thumb,
      Index,
      Middle,
      IndexAndMiddle,
      IndexOrMiddle,
    }

    public enum HandType
    {
      TransferrableHeldHand,
      TransferrableEquippedSide,
      LeftHand,
      RightHand,
    }

    private enum RangeState
    {
      Below,
      Within,
      Above,
    }
  }
}
