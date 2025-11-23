// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.VoiceBroadcastCosmeticWearable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

public class VoiceBroadcastCosmeticWearable : MonoBehaviour, IGorillaSliceableSimple
{
  public TalkingCosmeticType talkingCosmeticType;
  [SerializeField]
  private bool headDistanceActivation = true;
  [SerializeField]
  private float headDistance = 0.4f;
  [SerializeField]
  private float toggleCooldown = 0.5f;
  private bool toggleState;
  private float lastToggleTime;
  [SerializeField]
  private UnityEvent onStartListening;
  [SerializeField]
  private UnityEvent onStopListening;
  private List<VoiceBroadcastCosmetic> voiceBroadcasters;
  private Collider playerHeadCollider;

  private void Start()
  {
    VoiceBroadcastCosmetic[] componentsInChildren = this.GetComponentInParent<VRRig>().GetComponentsInChildren<VoiceBroadcastCosmetic>(true);
    this.voiceBroadcasters = new List<VoiceBroadcastCosmetic>();
    foreach (VoiceBroadcastCosmetic broadcastCosmetic in componentsInChildren)
    {
      if (broadcastCosmetic.talkingCosmeticType == this.talkingCosmeticType)
      {
        this.voiceBroadcasters.Add(broadcastCosmetic);
        broadcastCosmetic.SetWearable(this);
      }
    }
  }

  public void OnEnable()
  {
    if ((Object) this.playerHeadCollider == (Object) null)
      this.playerHeadCollider = (Collider) this.GetComponentInParent<VRRig>()?.rigContainer.HeadCollider;
    if (!this.headDistanceActivation || !((Object) this.playerHeadCollider != (Object) null))
      return;
    GorillaSlicerSimpleManager.RegisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
  }

  public void OnDisable()
  {
    GorillaSlicerSimpleManager.UnregisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
  }

  public void SliceUpdate()
  {
    if ((double) Time.time - (double) this.lastToggleTime < (double) this.toggleCooldown)
      return;
    bool listening = (double) (this.transform.position - this.playerHeadCollider.transform.position).sqrMagnitude <= (double) this.headDistance * (double) this.headDistance;
    if (listening == this.toggleState)
      return;
    this.toggleState = listening;
    this.lastToggleTime = Time.time;
    if (listening)
      this.onStartListening?.Invoke();
    else
      this.onStopListening?.Invoke();
    for (int index = 0; index < this.voiceBroadcasters.Count; ++index)
      this.voiceBroadcasters[index].SetListenState(listening);
  }

  public void OnCosmeticStartListening()
  {
    if (this.headDistanceActivation)
      return;
    this.onStartListening?.Invoke();
  }

  public void OnCosmeticStopListening()
  {
    if (this.headDistanceActivation)
      return;
    this.onStopListening?.Invoke();
  }
}
