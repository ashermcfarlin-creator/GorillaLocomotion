// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.HandControlledCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class HandControlledCosmetic : MonoBehaviour, ITickSystemTick
{
  [SerializeField]
  private HandControlledSettingsSO activeSettings;
  [SerializeField]
  private HandControlledSettingsSO inactiveSettings;
  [SerializeField]
  private Vector3 handPositionOffset;
  [SerializeField]
  private Quaternion rightHandRotation;
  [SerializeField]
  private Quaternion leftHandRotation;
  private Quaternion handRotationOffset;
  [SerializeField]
  private BezierCurve controlIndicatorCurve;
  [SerializeField]
  private Transform debugRelativePositionTransform1;
  [SerializeField]
  private Transform debugRelativePositionTransform2;
  private VRRig myRig;
  private Transform controllingHand;
  private Vector3 startHandRelativePosition;
  private Vector3 lowAngleLimits;
  private Vector3 highAngleLimits;
  private Vector3 localEuler;
  private Quaternion startHandInverseRotation;
  private Quaternion initialRotation;
  private bool isActive;

  public void Awake()
  {
    this.myRig = this.GetComponentInParent<VRRig>();
    this.initialRotation = this.transform.localRotation;
    this.enabled = false;
    if ((Object) this.debugRelativePositionTransform1 != (Object) null)
      Object.Destroy((Object) this.debugRelativePositionTransform1.gameObject);
    if (!((Object) this.debugRelativePositionTransform2 != (Object) null))
      return;
    Object.Destroy((Object) this.debugRelativePositionTransform2.gameObject);
  }

  private void SetControlIndicatorPoints()
  {
    if (!this.myRig.isOfflineVRRig || !((Object) this.controllingHand != (Object) null) || !((Object) this.controlIndicatorCurve != (Object) null) || this.controlIndicatorCurve.points == null)
      return;
    this.controlIndicatorCurve.points[0] = this.controllingHand.position;
    this.controlIndicatorCurve.points[1] = this.controlIndicatorCurve.points[0] + this.myRig.scaleFactor * this.controllingHand.up;
    this.controlIndicatorCurve.points[2] = this.transform.position;
  }

  private Vector3 GetRelativeHandPosition()
  {
    return this.controllingHand.TransformPoint(this.handPositionOffset) - this.myRig.bodyTransform.position;
  }

  public void StartControl(bool leftHand, float flexValue)
  {
    if (!this.enabled || !this.gameObject.activeInHierarchy)
      return;
    this.lowAngleLimits = this.activeSettings.angleLimits;
    this.highAngleLimits = 360f * Vector3.one - this.lowAngleLimits;
    this.handRotationOffset = leftHand ? this.leftHandRotation : this.rightHandRotation;
    this.controllingHand = leftHand ? this.myRig.leftHand.rigTarget.transform : this.myRig.rightHand.rigTarget.transform;
    this.startHandRelativePosition = this.GetRelativeHandPosition();
    this.startHandInverseRotation = Quaternion.Inverse(this.controllingHand.rotation * this.handRotationOffset);
    this.isActive = true;
    this.SetControlIndicatorPoints();
    TickSystem<object>.AddTickCallback((ITickSystemTick) this);
  }

  public void StopControl()
  {
    this.localEuler = this.transform.localRotation.eulerAngles;
    this.isActive = false;
    this.SetControlIndicatorPoints();
  }

  public void OnEnable()
  {
  }

  public void OnDisable()
  {
    this.transform.localRotation = this.initialRotation;
    this.StopControl();
    TickSystem<object>.RemoveTickCallback((ITickSystemTick) this);
  }

  private float ReverseClampDegrees(float value, float low, float high)
  {
    value = Mathf.Repeat(value, 360f);
    if ((double) value <= (double) low || (double) value >= (double) high)
      return value;
    return (double) value >= 180.0 ? high : low;
  }

  public bool TickRunning { get; set; }

  public void Tick()
  {
    if (this.isActive)
    {
      switch (this.activeSettings.rotationControl)
      {
        case HandControlledCosmetic.RotationControl.Angle:
          Quaternion rotation = this.controllingHand.rotation * this.handRotationOffset;
          this.localEuler += this.activeSettings.inputSensitivity * (this.startHandInverseRotation * rotation).eulerAngles;
          float t = 1f - Mathf.Exp(-this.activeSettings.inputDecaySpeed * Time.deltaTime);
          this.startHandInverseRotation = Quaternion.Slerp(this.startHandInverseRotation, Quaternion.Inverse(rotation), t);
          break;
        case HandControlledCosmetic.RotationControl.Translation:
          Vector3 relativeHandPosition = this.GetRelativeHandPosition();
          float num1 = Vector3.SignedAngle(new Vector3(this.startHandRelativePosition.x, 0.0f, this.startHandRelativePosition.z), new Vector3(relativeHandPosition.x, 0.0f, relativeHandPosition.z), Vector3.up);
          float num2 = (float) (50.0 * ((double) this.startHandRelativePosition.y - (double) relativeHandPosition.y)) / this.myRig.scaleFactor;
          float time = Vector3.Distance(this.startHandRelativePosition, relativeHandPosition) / this.myRig.scaleFactor;
          this.localEuler += Time.deltaTime * new Vector3(this.activeSettings.verticalSensitivity.Evaluate(time) * num2, this.activeSettings.horizontalSensitivity.Evaluate(time) * num1, 0.0f);
          this.startHandRelativePosition = Vector3.MoveTowards(this.startHandRelativePosition, relativeHandPosition, Time.deltaTime * this.activeSettings.inputDecayCurve.Evaluate(time));
          break;
      }
      for (int index = 0; index < 3; ++index)
        this.localEuler[index] = this.ReverseClampDegrees(this.localEuler[index], this.lowAngleLimits[index], this.highAngleLimits[index]);
      this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, Quaternion.Euler(this.localEuler), 1f - Mathf.Exp(-this.activeSettings.rotationSpeed * Time.deltaTime));
    }
    else
    {
      Quaternion quaternion = Quaternion.Slerp(this.transform.localRotation, this.initialRotation, 1f - Mathf.Exp(-this.inactiveSettings.rotationSpeed * Time.deltaTime));
      this.transform.localRotation = quaternion;
      this.localEuler = quaternion.eulerAngles;
    }
  }

  public enum RotationControl
  {
    Angle,
    Translation,
  }
}
