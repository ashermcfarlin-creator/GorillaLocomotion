// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.StreetLightSaber
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaLocomotion.Climbing;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class StreetLightSaber : MonoBehaviour
{
  [SerializeField]
  private float autoSwitchTimer = 5f;
  [SerializeField]
  private TrailRenderer trailRenderer;
  [SerializeField]
  private Renderer meshRenderer;
  [SerializeField]
  private string shaderColorProperty;
  [SerializeField]
  private int materialIndex;
  [SerializeField]
  private GorillaVelocityTracker velocityTracker;
  [SerializeField]
  private float minHitVelocityThreshold;
  private static readonly StreetLightSaber.State[] values = (StreetLightSaber.State[]) Enum.GetValues(typeof (StreetLightSaber.State));
  [Space]
  [Header("Staff State Settings")]
  public StreetLightSaber.StaffStates[] allStates = new StreetLightSaber.StaffStates[0];
  private int currentIndex;
  private Dictionary<StreetLightSaber.State, StreetLightSaber.StaffStates> allStatesDict = new Dictionary<StreetLightSaber.State, StreetLightSaber.StaffStates>();
  private bool autoSwitch;
  private float autoSwitchEnabledTime;
  private int hashId;
  private Material instancedMaterial;

  private StreetLightSaber.State CurrentState => StreetLightSaber.values[this.currentIndex];

  private void Awake()
  {
    foreach (StreetLightSaber.StaffStates allState in this.allStates)
      this.allStatesDict[allState.state] = allState;
    this.currentIndex = 0;
    this.autoSwitchEnabledTime = 0.0f;
    this.hashId = Shader.PropertyToID(this.shaderColorProperty);
    Material[] sharedMaterials = this.meshRenderer.sharedMaterials;
    this.instancedMaterial = new Material(sharedMaterials[this.materialIndex]);
    sharedMaterials[this.materialIndex] = this.instancedMaterial;
    this.meshRenderer.sharedMaterials = sharedMaterials;
  }

  private void Update()
  {
    if (!this.autoSwitch || (double) Time.time - (double) this.autoSwitchEnabledTime <= (double) this.autoSwitchTimer)
      return;
    this.UpdateStateAuto();
  }

  private void OnDestroy() => this.allStatesDict.Clear();

  private void OnEnable() => this.ForceSwitchTo(StreetLightSaber.State.Off);

  public void UpdateStateManual()
  {
    this.SwitchState((this.currentIndex + 1) % StreetLightSaber.values.Length);
  }

  private void UpdateStateAuto()
  {
    StreetLightSaber.State state = this.CurrentState == StreetLightSaber.State.Green ? StreetLightSaber.State.Red : StreetLightSaber.State.Green;
    this.SwitchState(Array.IndexOf<StreetLightSaber.State>(StreetLightSaber.values, state));
    this.autoSwitchEnabledTime = Time.time;
  }

  public void EnableAutoSwitch(bool enable) => this.autoSwitch = enable;

  public void ResetStaff() => this.ForceSwitchTo(StreetLightSaber.State.Off);

  public void HitReceived(Vector3 contact)
  {
    if (!((UnityEngine.Object) this.velocityTracker != (UnityEngine.Object) null) || (double) this.velocityTracker.GetLatestVelocity(true).magnitude < (double) this.minHitVelocityThreshold)
      return;
    this.allStatesDict[this.CurrentState]?.OnSuccessfulHit.Invoke(contact);
  }

  private void SwitchState(int newIndex)
  {
    if (newIndex == this.currentIndex)
      return;
    StreetLightSaber.State currentState = this.CurrentState;
    StreetLightSaber.State key = StreetLightSaber.values[newIndex];
    StreetLightSaber.StaffStates staffStates1;
    if (this.allStatesDict.TryGetValue(currentState, out staffStates1))
      staffStates1.onExitState?.Invoke();
    this.currentIndex = newIndex;
    StreetLightSaber.StaffStates staffStates2;
    if (!this.allStatesDict.TryGetValue(key, out staffStates2))
      return;
    staffStates2.onEnterState?.Invoke();
    if ((UnityEngine.Object) this.trailRenderer != (UnityEngine.Object) null)
      this.trailRenderer.startColor = staffStates2.color;
    if (!((UnityEngine.Object) this.meshRenderer != (UnityEngine.Object) null))
      return;
    this.instancedMaterial.SetColor(this.hashId, staffStates2.color);
  }

  private void ForceSwitchTo(StreetLightSaber.State targetState)
  {
    int newIndex = Array.IndexOf<StreetLightSaber.State>(StreetLightSaber.values, targetState);
    if (newIndex < 0)
      return;
    this.SwitchState(newIndex);
  }

  [Serializable]
  public class StaffStates
  {
    public StreetLightSaber.State state;
    public Color color;
    public UnityEvent onEnterState;
    public UnityEvent onExitState;
    public UnityEvent<Vector3> OnSuccessfulHit;
  }

  public enum State
  {
    Off,
    Green,
    Red,
  }
}
