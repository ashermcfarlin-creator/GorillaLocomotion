// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.OnCollisionEventsCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics;

[RequireComponent(typeof (Collider))]
public class OnCollisionEventsCosmetic : MonoBehaviour
{
  [Tooltip("List of per-condition listeners. Each entry specifies when (Enter/Stay/Exit), what to collide with (layers/tags), and which UnityEvents to fire.")]
  public OnCollisionEventsCosmetic.Listener[] eventListeners = new OnCollisionEventsCosmetic.Listener[0];
  private OnCollisionEventsCosmetic.Listener[] enterListeners = Array.Empty<OnCollisionEventsCosmetic.Listener>();
  private OnCollisionEventsCosmetic.Listener[] stayListeners = Array.Empty<OnCollisionEventsCosmetic.Listener>();
  private OnCollisionEventsCosmetic.Listener[] exitListeners = Array.Empty<OnCollisionEventsCosmetic.Listener>();
  private Collider myCollider;
  private VRRig rig;
  private TransferrableObject parentTransferable;

  private bool IsMyItem() => (UnityEngine.Object) this.rig != (UnityEngine.Object) null && this.rig.isOfflineVRRig;

  private void Awake()
  {
    this.myCollider = this.GetComponent<Collider>();
    if ((UnityEngine.Object) this.myCollider == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "OnCollisionEventsCosmetic requires a Collider on the same GameObject.");
      this.enabled = false;
    }
    else
    {
      if (this.myCollider.isTrigger)
        Debug.LogWarning((object) "OnCollisionEventsCosmetic: Collider is set to Trigger. OnCollision will not fire. Set it to non-trigger for collisions.");
      this.rig = this.GetComponentInParent<VRRig>();
      this.parentTransferable = this.GetComponentInParent<TransferrableObject>();
      List<OnCollisionEventsCosmetic.Listener> listenerList1 = new List<OnCollisionEventsCosmetic.Listener>();
      List<OnCollisionEventsCosmetic.Listener> listenerList2 = new List<OnCollisionEventsCosmetic.Listener>();
      List<OnCollisionEventsCosmetic.Listener> listenerList3 = new List<OnCollisionEventsCosmetic.Listener>();
      if (this.eventListeners != null)
      {
        for (int index = 0; index < this.eventListeners.Length; ++index)
        {
          OnCollisionEventsCosmetic.Listener eventListener = this.eventListeners[index];
          if (eventListener.tagSet == null)
            eventListener.tagSet = eventListener.collisionTagsList == null || eventListener.collisionTagsList.Count <= 0 ? new HashSet<string>() : new HashSet<string>((IEnumerable<string>) eventListener.collisionTagsList);
          if (eventListener.eventType == OnCollisionEventsCosmetic.EventType.CollisionEnter)
            listenerList1.Add(eventListener);
          else if (eventListener.eventType == OnCollisionEventsCosmetic.EventType.CollisionStay)
            listenerList2.Add(eventListener);
          else if (eventListener.eventType == OnCollisionEventsCosmetic.EventType.CollisionExit)
            listenerList3.Add(eventListener);
        }
      }
      this.enterListeners = listenerList1.Count > 0 ? listenerList1.ToArray() : Array.Empty<OnCollisionEventsCosmetic.Listener>();
      this.stayListeners = listenerList2.Count > 0 ? listenerList2.ToArray() : Array.Empty<OnCollisionEventsCosmetic.Listener>();
      this.exitListeners = listenerList3.Count > 0 ? listenerList3.ToArray() : Array.Empty<OnCollisionEventsCosmetic.Listener>();
    }
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (!OnCollisionEventsCosmetic.IsCollisionUsable(collision))
      return;
    this.Dispatch(this.enterListeners, collision);
  }

  private void OnCollisionStay(Collision collision)
  {
    if (!OnCollisionEventsCosmetic.IsCollisionUsable(collision))
      return;
    this.Dispatch(this.stayListeners, collision);
  }

  private void OnCollisionExit(Collision collision)
  {
    if (!OnCollisionEventsCosmetic.IsCollisionUsable(collision))
      return;
    this.Dispatch(this.exitListeners, collision);
  }

  private static bool IsCollisionUsable(Collision collision)
  {
    if (collision == null)
      return false;
    Collider collider = collision.collider;
    if ((UnityEngine.Object) collider == (UnityEngine.Object) null)
      return false;
    GameObject gameObject = collider.gameObject;
    return !((UnityEngine.Object) gameObject == (UnityEngine.Object) null) && gameObject.activeInHierarchy;
  }

  private void Dispatch(OnCollisionEventsCosmetic.Listener[] listeners, Collision collision)
  {
    if (listeners == null || listeners.Length == 0)
      return;
    Collider collider = collision.collider;
    GameObject gameObject = (UnityEngine.Object) collider != (UnityEngine.Object) null ? collider.gameObject : (GameObject) null;
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      return;
    int layer = gameObject.layer;
    bool flag = (bool) (UnityEngine.Object) this.parentTransferable && this.parentTransferable.InLeftHand();
    Vector3 position = (UnityEngine.Object) this.myCollider != (UnityEngine.Object) null ? this.myCollider.bounds.center : this.transform.position;
    Vector3 vector3 = collision.contactCount <= 0 ? collider.ClosestPoint(position) : collision.GetContact(0).point;
    for (int index = 0; index < listeners.Length; ++index)
    {
      OnCollisionEventsCosmetic.Listener listener = listeners[index];
      if ((listener.syncForEveryoneInRoom || this.IsMyItem()) && (!listener.fireOnlyWhileHeld || !(bool) (UnityEngine.Object) this.parentTransferable || this.parentTransferable.InHand()) && (listener.tagSet == null || listener.tagSet.Count <= 0 || OnCollisionEventsCosmetic.CompareTagAny(gameObject, listener.tagSet)) && (1 << layer & listener.collisionLayerMask.value) != 0)
      {
        if (listener.listenerComponent != null)
          listener.listenerComponent.Invoke(flag, collision);
        if (listener.listenerComponentContactPoint != null)
          listener.listenerComponentContactPoint.Invoke(vector3);
        VRRig componentInParent = gameObject.GetComponentInParent<VRRig>();
        if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null && listener.onCollidedVRRig != null)
          listener.onCollidedVRRig.Invoke(componentInParent);
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

  private bool IsTagValid(GameObject obj, OnCollisionEventsCosmetic.Listener listener)
  {
    return listener == null || listener.tagSet == null || listener.tagSet.Count == 0 || OnCollisionEventsCosmetic.CompareTagAny(obj, listener.tagSet);
  }

  [Serializable]
  public class Listener
  {
    [Tooltip("Only collisions with objects on these layers will be considered.")]
    public LayerMask collisionLayerMask;
    [Tooltip("Optional tag whitelist. If non-empty, collisions must match at least one of these tags.")]
    public List<string> collisionTagsList = new List<string>();
    [Tooltip("Choose which collision phase triggers this listener: Enter, Stay, or Exit.")]
    public OnCollisionEventsCosmetic.EventType eventType;
    public UnityEvent<bool, Collision> listenerComponent;
    public UnityEvent<Vector3> listenerComponentContactPoint;
    public UnityEvent<VRRig> onCollidedVRRig;
    [Tooltip("If true, fire for everyone in the room. If false, only fire when this item is owned locally (offline rig).")]
    public bool syncForEveryoneInRoom = true;
    [Tooltip("If true, only fire while this item is held. Requires a TransferrableObject on this object or a parent.")]
    public bool fireOnlyWhileHeld = true;
    [NonSerialized]
    public HashSet<string> tagSet;
  }

  public enum EventType
  {
    CollisionEnter,
    CollisionStay,
    CollisionExit,
  }
}
