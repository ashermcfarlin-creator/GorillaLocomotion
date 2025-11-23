// Decompiled with JetBrains decompiler
// Type: GorillaTag.DayNightWatchWearable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag;

public class DayNightWatchWearable : MonoBehaviour
{
  [Tooltip("The transform that will be rotated to indicate the current time.")]
  public Transform clockNeedle;
  [FormerlySerializedAs("dialRotationAxis")]
  [Tooltip("The axis that the needle will rotate around.")]
  public Vector3 needleRotationAxis = Vector3.right;
  private BetterDayNightManager dayNightManager;
  [DebugOption]
  private float rotationDegree;
  private string currentTimeOfDay;
  private Quaternion initialRotation;

  private void Start()
  {
    if (!(bool) (Object) this.dayNightManager)
      this.dayNightManager = BetterDayNightManager.instance;
    this.rotationDegree = 0.0f;
    if (!(bool) (Object) this.clockNeedle)
      return;
    this.initialRotation = this.clockNeedle.localRotation;
  }

  private void Update()
  {
    this.currentTimeOfDay = this.dayNightManager.currentTimeOfDay;
    this.rotationDegree = (float) (360.0 * ((ITimeOfDaySystem) this.dayNightManager).currentTimeInSeconds / ((ITimeOfDaySystem) this.dayNightManager).totalTimeInSeconds);
    this.rotationDegree = Mathf.Floor(this.rotationDegree);
    if (!(bool) (Object) this.clockNeedle)
      return;
    this.clockNeedle.localRotation = this.initialRotation * Quaternion.AngleAxis(this.rotationDegree, this.needleRotationAxis);
  }
}
