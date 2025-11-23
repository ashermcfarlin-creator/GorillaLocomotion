// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.VenusFlyTrapHoldable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using Photon.Pun;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

[RequireComponent(typeof (TransferrableObject))]
public class VenusFlyTrapHoldable : MonoBehaviour, ITickSystemTick
{
  [SerializeField]
  private GameObject lipA;
  [SerializeField]
  private GameObject lipB;
  [SerializeField]
  private Vector3 targetRotationA;
  [SerializeField]
  private Vector3 targetRotationB;
  [SerializeField]
  private float closedDuration = 3f;
  [SerializeField]
  private float speed = 2f;
  [SerializeField]
  private UnityLayer layers;
  [SerializeField]
  private TriggerEventNotifier triggerEventNotifier;
  [SerializeField]
  private float hapticStrength = 0.5f;
  [SerializeField]
  private float hapticDuration = 0.1f;
  [SerializeField]
  private GameObject bug;
  [SerializeField]
  private AudioSource audioSource;
  [SerializeField]
  private AudioClip closingAudio;
  [SerializeField]
  private AudioClip openingAudio;
  [SerializeField]
  private AudioClip flyLoopingAudio;
  private CallLimiter callLimiter = new CallLimiter(10, 2f);
  private float closedStartedTime;
  private VenusFlyTrapHoldable.VenusState state;
  private Quaternion localRotA;
  private Quaternion localRotB;
  private RubberDuckEvents _events;
  private TransferrableObject transferrableObject;

  public bool TickRunning { get; set; }

  private void Awake() => this.transferrableObject = this.GetComponent<TransferrableObject>();

  private void OnEnable()
  {
    TickSystem<object>.AddCallbackTarget((object) this);
    this.triggerEventNotifier.TriggerEnterEvent += new TriggerEventNotifier.TriggerEvent(this.TriggerEntered);
    this.state = VenusFlyTrapHoldable.VenusState.Open;
    this.localRotA = this.lipA.transform.localRotation;
    this.localRotB = this.lipB.transform.localRotation;
    if ((UnityEngine.Object) this._events == (UnityEngine.Object) null)
    {
      this._events = this.gameObject.GetOrAddComponent<RubberDuckEvents>();
      NetPlayer player = (UnityEngine.Object) this.transferrableObject.myOnlineRig != (UnityEngine.Object) null ? this.transferrableObject.myOnlineRig.creator : ((UnityEngine.Object) this.transferrableObject.myRig != (UnityEngine.Object) null ? this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer : (NetPlayer) null);
      if (player != null)
        this._events.Init(player);
    }
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnTriggerEvent);
  }

  private void OnDisable()
  {
    TickSystem<object>.RemoveCallbackTarget((object) this);
    this.triggerEventNotifier.TriggerEnterEvent -= new TriggerEventNotifier.TriggerEvent(this.TriggerEntered);
    if (!((UnityEngine.Object) this._events != (UnityEngine.Object) null))
      return;
    this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnTriggerEvent);
    this._events.Dispose();
    this._events = (RubberDuckEvents) null;
  }

  public void Tick()
  {
    if (this.transferrableObject.InHand() && (bool) (UnityEngine.Object) this.audioSource && !this.audioSource.isPlaying && (UnityEngine.Object) this.flyLoopingAudio != (UnityEngine.Object) null)
    {
      this.audioSource.clip = this.flyLoopingAudio;
      this.audioSource.GTPlay();
    }
    if (!this.transferrableObject.InHand() && (bool) (UnityEngine.Object) this.audioSource && this.audioSource.isPlaying)
      this.audioSource.GTStop();
    if (this.state == VenusFlyTrapHoldable.VenusState.Open)
      return;
    if (this.state == VenusFlyTrapHoldable.VenusState.Closed && (double) Time.time - (double) this.closedStartedTime >= (double) this.closedDuration)
    {
      this.UpdateState(VenusFlyTrapHoldable.VenusState.Opening);
      if ((bool) (UnityEngine.Object) this.audioSource && (UnityEngine.Object) this.openingAudio != (UnityEngine.Object) null)
        this.audioSource.GTPlayOneShot(this.openingAudio);
    }
    if (this.state == VenusFlyTrapHoldable.VenusState.Closing)
    {
      this.SmoothRotation(true);
    }
    else
    {
      if (this.state != VenusFlyTrapHoldable.VenusState.Opening)
        return;
      this.SmoothRotation(false);
    }
  }

  private void SmoothRotation(bool isClosing)
  {
    if (isClosing)
    {
      Quaternion b1 = Quaternion.Euler(this.targetRotationB);
      this.lipB.transform.localRotation = Quaternion.Lerp(this.lipB.transform.localRotation, b1, Time.deltaTime * this.speed);
      Quaternion b2 = Quaternion.Euler(this.targetRotationA);
      this.lipA.transform.localRotation = Quaternion.Lerp(this.lipA.transform.localRotation, b2, Time.deltaTime * this.speed);
      if ((double) Quaternion.Angle(this.lipB.transform.localRotation, b1) >= 1.0 || (double) Quaternion.Angle(this.lipA.transform.localRotation, b2) >= 1.0)
        return;
      this.lipB.transform.localRotation = b1;
      this.lipA.transform.localRotation = b2;
      this.UpdateState(VenusFlyTrapHoldable.VenusState.Closed);
    }
    else
    {
      this.lipB.transform.localRotation = Quaternion.Lerp(this.lipB.transform.localRotation, this.localRotB, (float) ((double) Time.deltaTime * (double) this.speed / 2.0));
      this.lipA.transform.localRotation = Quaternion.Lerp(this.lipA.transform.localRotation, this.localRotA, (float) ((double) Time.deltaTime * (double) this.speed / 2.0));
      if ((double) Quaternion.Angle(this.lipB.transform.localRotation, this.localRotB) >= 1.0 || (double) Quaternion.Angle(this.lipA.transform.localRotation, this.localRotA) >= 1.0)
        return;
      this.lipB.transform.localRotation = this.localRotB;
      this.lipA.transform.localRotation = this.localRotA;
      this.UpdateState(VenusFlyTrapHoldable.VenusState.Open);
    }
  }

  private void UpdateState(VenusFlyTrapHoldable.VenusState newState)
  {
    this.state = newState;
    if (this.state != VenusFlyTrapHoldable.VenusState.Closed)
      return;
    this.closedStartedTime = Time.time;
  }

  private void TriggerEntered(TriggerEventNotifier notifier, Collider other)
  {
    if (this.state != VenusFlyTrapHoldable.VenusState.Open || !other.gameObject.IsOnLayer(this.layers))
      return;
    if (PhotonNetwork.InRoom && (UnityEngine.Object) this._events != (UnityEngine.Object) null && this._events.Activate != (PhotonEvent) null)
      this._events.Activate.RaiseOthers();
    this.OnTriggerLocal();
    GorillaTriggerColliderHandIndicator componentInChildren = other.GetComponentInChildren<GorillaTriggerColliderHandIndicator>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      return;
    GorillaTagger.Instance.StartVibration(componentInChildren.isLeftHand, this.hapticStrength, this.hapticDuration);
  }

  private void OnTriggerEvent(
    int sender,
    int target,
    object[] args,
    PhotonMessageInfoWrapped info)
  {
    if (sender != target)
      return;
    GorillaNot.IncrementRPCCall(info, nameof (OnTriggerEvent));
    if (!this.callLimiter.CheckCallTime(Time.time))
      return;
    this.OnTriggerLocal();
  }

  private void OnTriggerLocal()
  {
    this.UpdateState(VenusFlyTrapHoldable.VenusState.Closing);
    if (!(bool) (UnityEngine.Object) this.audioSource || !((UnityEngine.Object) this.closingAudio != (UnityEngine.Object) null))
      return;
    this.audioSource.GTPlayOneShot(this.closingAudio);
  }

  private enum VenusState
  {
    Closed,
    Open,
    Closing,
    Opening,
  }
}
