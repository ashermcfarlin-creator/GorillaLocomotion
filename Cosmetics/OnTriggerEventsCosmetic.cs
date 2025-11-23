// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.OnTriggerEventsCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaLocomotion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

[RequireComponent(typeof (Collider))]
public class OnTriggerEventsCosmetic : MonoBehaviour
{
  [Tooltip("List of per-condition listeners. Each entry specifies when (Enter/Stay/Exit), what to trigger with (layers/tags), and which UnityEvents to fire.")]
  public OnTriggerEventsCosmetic.Listener[] eventListeners = new OnTriggerEventsCosmetic.Listener[0];
  private OnTriggerEventsCosmetic.Listener[] enterListeners = Array.Empty<OnTriggerEventsCosmetic.Listener>();
  private OnTriggerEventsCosmetic.Listener[] stayListeners = Array.Empty<OnTriggerEventsCosmetic.Listener>();
  private OnTriggerEventsCosmetic.Listener[] exitListeners = Array.Empty<OnTriggerEventsCosmetic.Listener>();
  private Collider myCollider;
  private VRRig rig;
  private TransferrableObject parentTransferable;

  private bool IsMyItem() => (UnityEngine.Object) this.rig != (UnityEngine.Object) null && this.rig.isOfflineVRRig;

  private void Awake()
  {
    Collider[] components = this.GetComponents<Collider>();
    if (components == null || components.Length == 0)
    {
      Debug.LogError((object) "OnTriggerEventsCosmetic requires at least one Collider on the same GameObject.");
      this.enabled = false;
    }
    else
    {
      bool flag = false;
      foreach (Collider collider in components)
      {
        if ((UnityEngine.Object) collider != (UnityEngine.Object) null && (collider.isTrigger || (UnityEngine.Object) collider.attachedRigidbody != (UnityEngine.Object) null))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        Debug.LogWarning((object) ("OnTriggerEventsCosmetic: Collider is not set to Trigger. OnTrigger will not fire. Path=" + this.transform.GetPathQ()), (UnityEngine.Object) this.transform);
      this.rig = this.GetComponentInParent<VRRig>();
      if ((UnityEngine.Object) this.rig == (UnityEngine.Object) null && (UnityEngine.Object) this.gameObject.GetComponentInParent<GTPlayer>() != (UnityEngine.Object) null)
        this.rig = GorillaTagger.Instance.offlineVRRig;
      this.parentTransferable = this.GetComponentInParent<TransferrableObject>();
      List<OnTriggerEventsCosmetic.Listener> listenerList1 = new List<OnTriggerEventsCosmetic.Listener>();
      List<OnTriggerEventsCosmetic.Listener> listenerList2 = new List<OnTriggerEventsCosmetic.Listener>();
      List<OnTriggerEventsCosmetic.Listener> listenerList3 = new List<OnTriggerEventsCosmetic.Listener>();
      if (this.eventListeners != null)
      {
        for (int index = 0; index < this.eventListeners.Length; ++index)
        {
          OnTriggerEventsCosmetic.Listener eventListener = this.eventListeners[index];
          if (eventListener.tagSet == null)
            eventListener.tagSet = eventListener.triggerTagsList == null || eventListener.triggerTagsList.Count <= 0 ? new HashSet<string>() : new HashSet<string>((IEnumerable<string>) eventListener.triggerTagsList);
          if (eventListener.eventType == OnTriggerEventsCosmetic.EventType.TriggerEnter)
            listenerList1.Add(eventListener);
          else if (eventListener.eventType == OnTriggerEventsCosmetic.EventType.TriggerStay)
            listenerList2.Add(eventListener);
          else if (eventListener.eventType == OnTriggerEventsCosmetic.EventType.TriggerExit)
            listenerList3.Add(eventListener);
        }
      }
      this.enterListeners = listenerList1.Count > 0 ? listenerList1.ToArray() : Array.Empty<OnTriggerEventsCosmetic.Listener>();
      this.stayListeners = listenerList2.Count > 0 ? listenerList2.ToArray() : Array.Empty<OnTriggerEventsCosmetic.Listener>();
      this.exitListeners = listenerList3.Count > 0 ? listenerList3.ToArray() : Array.Empty<OnTriggerEventsCosmetic.Listener>();
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!OnTriggerEventsCosmetic.IsOtherUsable(other))
      return;
    this.Dispatch(this.enterListeners, other);
  }

  private void OnTriggerStay(Collider other)
  {
    if (!OnTriggerEventsCosmetic.IsOtherUsable(other))
      return;
    this.Dispatch(this.stayListeners, other);
  }

  private void OnTriggerExit(Collider other)
  {
    if (!OnTriggerEventsCosmetic.IsOtherUsable(other))
      return;
    this.Dispatch(this.exitListeners, other);
  }

  private static bool IsOtherUsable(Collider other)
  {
    if ((UnityEngine.Object) other == (UnityEngine.Object) null)
      return false;
    GameObject gameObject = other.gameObject;
    return !((UnityEngine.Object) gameObject == (UnityEngine.Object) null) && gameObject.activeInHierarchy;
  }

  private void Dispatch(OnTriggerEventsCosmetic.Listener[] listeners, Collider other)
  {
    if (listeners == null || listeners.Length == 0)
      return;
    int layer = other.gameObject.layer;
    bool flag = (bool) (UnityEngine.Object) this.parentTransferable && this.parentTransferable.InLeftHand();
    Vector3 position = (UnityEngine.Object) this.myCollider != (UnityEngine.Object) null ? this.myCollider.bounds.center : this.transform.position;
    for (int index = 0; index < listeners.Length; ++index)
    {
      OnTriggerEventsCosmetic.Listener listener = listeners[index];
      if ((listener.syncForEveryoneInRoom || this.IsMyItem()) && (!listener.fireOnlyWhileHeld || !(bool) (UnityEngine.Object) this.parentTransferable || this.parentTransferable.InHand()) && (listener.tagSet == null || listener.tagSet.Count <= 0 || OnTriggerEventsCosmetic.CompareTagAny(other.gameObject, listener.tagSet)) && (1 << layer & listener.triggerLayerMask.value) != 0)
      {
        listener.listenerComponent?.Invoke(flag, other);
        Vector3 vector3 = other.ClosestPoint(position);
        listener.listenerComponentContactPoint?.Invoke(vector3);
        VRRig componentInParent = other.GetComponentInParent<VRRig>();
        if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null)
          listener.onTriggeredVRRig?.Invoke(componentInParent);
      }
    }
  }

  private static bool CompareTagAny(GameObject go, HashSet<string> tagSet)
  {
    if (tagSet == null || tagSet.Count == 0)
      return true;
    foreach (string tag in tagSet)
    {
      if (!string.IsNullOrEmpty(tag) && go.CompareTag(tag))
        return true;
    }
    return false;
  }

  private bool IsTagValid(GameObject obj, OnTriggerEventsCosmetic.Listener listener)
  {
    return listener == null || listener.tagSet == null || listener.tagSet.Count == 0 || OnTriggerEventsCosmetic.CompareTagAny(obj, listener.tagSet);
  }

  [Serializable]
  public class Listener
  {
    [Tooltip("Only trigger interactions with objects on these layers.")]
    public LayerMask triggerLayerMask;
    [Tooltip("Optional tag whitelist. If non-empty, triggers must match at least one of these tags.")]
    public List<string> triggerTagsList = new List<string>();
    [Tooltip("Choose which trigger phase invokes this listener: Enter, Stay, or Exit.")]
    public OnTriggerEventsCosmetic.EventType eventType;
    public UnityEvent<bool, Collider> listenerComponent;
    public UnityEvent<Vector3> listenerComponentContactPoint;
    public UnityEvent<VRRig> onTriggeredVRRig;
    [Tooltip("If true, fire for everyone in the room. If false, only fire when this item is owned locally (offline rig).")]
    public bool syncForEveryoneInRoom = true;
    [Tooltip("If true, only fire while this item is held. Requires a TransferrableObject on this object or a parent.")]
    public bool fireOnlyWhileHeld = true;
    [NonSerialized]
    public HashSet<string> tagSet;
  }

  public enum EventType
  {
    TriggerEnter,
    TriggerStay,
    TriggerExit,
  }
}
