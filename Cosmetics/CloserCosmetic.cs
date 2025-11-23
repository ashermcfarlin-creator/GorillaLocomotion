// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.CloserCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class CloserCosmetic : MonoBehaviour, ITickSystemTick
{
  [SerializeField]
  private GameObject sideA;
  [SerializeField]
  private GameObject sideB;
  [SerializeField]
  private Vector3 maxRotationA;
  [SerializeField]
  private Vector3 maxRotationB;
  [SerializeField]
  private bool useFingerFlexValueAsStrength;
  private Quaternion localRotA;
  private Quaternion localRotB;
  private CloserCosmetic.State currentState;
  private float fingerValue;

  public bool TickRunning { get; set; }

  private void OnEnable()
  {
    TickSystem<object>.AddCallbackTarget((object) this);
    this.localRotA = this.sideA.transform.localRotation;
    this.localRotB = this.sideB.transform.localRotation;
    this.fingerValue = 0.0f;
    this.UpdateState(CloserCosmetic.State.Opening);
  }

  private void OnDisable() => TickSystem<object>.RemoveCallbackTarget((object) this);

  public void Tick()
  {
    switch (this.currentState)
    {
      case CloserCosmetic.State.Closing:
        this.Closing();
        break;
      case CloserCosmetic.State.Opening:
        this.Opening();
        break;
    }
  }

  public void Close(bool leftHand, float fingerFlexValue)
  {
    this.UpdateState(CloserCosmetic.State.Closing);
    this.fingerValue = fingerFlexValue;
  }

  public void Open(bool leftHand, float fingerFlexValue)
  {
    this.UpdateState(CloserCosmetic.State.Opening);
    this.fingerValue = fingerFlexValue;
  }

  private void Closing()
  {
    float t = this.useFingerFlexValueAsStrength ? Mathf.Clamp01(this.fingerValue) : 1f;
    Quaternion b1 = Quaternion.Slerp(this.localRotB, Quaternion.Euler(this.maxRotationB), t);
    this.sideB.transform.localRotation = b1;
    Quaternion b2 = Quaternion.Slerp(this.localRotA, Quaternion.Euler(this.maxRotationA), t);
    this.sideA.transform.localRotation = b2;
    if ((double) Quaternion.Angle(this.sideB.transform.localRotation, b1) >= 0.10000000149011612 || (double) Quaternion.Angle(this.sideA.transform.localRotation, b2) >= 0.10000000149011612)
      return;
    this.UpdateState(CloserCosmetic.State.None);
  }

  private void Opening()
  {
    float t = this.useFingerFlexValueAsStrength ? Mathf.Clamp01(this.fingerValue) : 1f;
    Quaternion b1 = Quaternion.Slerp(this.sideB.transform.localRotation, this.localRotB, t);
    this.sideB.transform.localRotation = b1;
    Quaternion b2 = Quaternion.Slerp(this.sideA.transform.localRotation, this.localRotA, t);
    this.sideA.transform.localRotation = b2;
    if ((double) Quaternion.Angle(this.sideB.transform.localRotation, b1) >= 0.10000000149011612 || (double) Quaternion.Angle(this.sideA.transform.localRotation, b2) >= 0.10000000149011612)
      return;
    this.UpdateState(CloserCosmetic.State.None);
  }

  private void UpdateState(CloserCosmetic.State newState) => this.currentState = newState;

  private enum State
  {
    Closing,
    Opening,
    None,
  }
}
