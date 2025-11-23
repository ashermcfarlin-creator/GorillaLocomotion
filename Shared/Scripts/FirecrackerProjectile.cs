// Decompiled with JetBrains decompiler
// Type: GorillaTag.Shared.Scripts.FirecrackerProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTag.Cosmetics;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Shared.Scripts;

public class FirecrackerProjectile : MonoBehaviour, ITickSystemTick, IProjectile
{
  [SerializeField]
  private GameObject explosionEffect;
  [SerializeField]
  private float forceBackToPoolAfterSec = 20f;
  [SerializeField]
  private float explosionTime = 5f;
  [SerializeField]
  private GameObject disableWhenHit;
  [SerializeField]
  private float sizzleDuration;
  [SerializeField]
  private AudioClip sizzleAudioClip;
  [Space]
  public UnityEvent OnEnableObject;
  public UnityEvent<FirecrackerProjectile, Vector3> OnCollisionEntered;
  public UnityEvent<FirecrackerProjectile, Vector3> OnDetonationStart;
  public UnityEvent<FirecrackerProjectile> OnDetonationComplete;
  private Rigidbody rb;
  private float timeCreated = float.PositiveInfinity;
  private float timeExploded = float.PositiveInfinity;
  private AudioSource audioSource;
  private TickSystemTimer m_timer = new TickSystemTimer(40f);
  private bool collisionEntered;
  [SerializeField]
  private bool useTransferrableObjectState;
  [SerializeField]
  protected UnityEvent OnResetProjectileState;
  [SerializeField]
  protected string boolADebugName;
  [SerializeField]
  protected UnityEvent OnItemStateBoolATrue;
  [SerializeField]
  protected UnityEvent OnItemStateBoolAFalse;
  [SerializeField]
  protected string boolBDebugName;
  [SerializeField]
  protected UnityEvent OnItemStateBoolBTrue;
  [SerializeField]
  protected UnityEvent OnItemStateBoolBFalse;
  [SerializeField]
  protected string boolCDebugName;
  [SerializeField]
  protected UnityEvent OnItemStateBoolCTrue;
  [SerializeField]
  protected UnityEvent OnItemStateBoolCFalse;
  [SerializeField]
  protected string boolDDebugName;
  [SerializeField]
  protected UnityEvent OnItemStateBoolDTrue;
  [SerializeField]
  protected UnityEvent OnItemStateBoolDFalse;
  [SerializeField]
  protected UnityEvent<int> OnItemStateIntChanged;

  public bool TickRunning { get; set; }

  public void Tick()
  {
    if ((double) Time.time - (double) this.timeCreated <= (double) this.forceBackToPoolAfterSec && (double) Time.time - (double) this.timeExploded <= (double) this.explosionTime)
      return;
    this.OnDetonationComplete?.Invoke(this);
  }

  private void OnEnable()
  {
    TickSystem<object>.AddCallbackTarget((object) this);
    this.m_timer.Start();
    this.timeExploded = float.PositiveInfinity;
    this.timeCreated = float.PositiveInfinity;
    this.collisionEntered = false;
    if ((bool) (UnityEngine.Object) this.disableWhenHit)
      this.disableWhenHit.SetActive(true);
    this.OnEnableObject?.Invoke();
  }

  private void OnDisable()
  {
    TickSystem<object>.RemoveCallbackTarget((object) this);
    this.m_timer.Stop();
    if (!this.useTransferrableObjectState)
      return;
    this.OnResetProjectileState?.Invoke();
  }

  private void Awake()
  {
    this.rb = this.GetComponent<Rigidbody>();
    this.audioSource = this.GetComponent<AudioSource>();
    this.m_timer.callback = new Action(this.Detonate);
  }

  private void Detonate()
  {
    this.m_timer.Stop();
    this.timeExploded = Time.time;
    if ((bool) (UnityEngine.Object) this.disableWhenHit)
      this.disableWhenHit.SetActive(false);
    this.collisionEntered = false;
  }

  internal void SetTransferrableState(TransferrableObject.SyncOptions syncType, int state)
  {
    if (!this.useTransferrableObjectState)
      return;
    switch (syncType)
    {
      case TransferrableObject.SyncOptions.Bool:
        int num = (state & 1) != 0 ? 1 : 0;
        bool flag1 = (state & 2) != 0;
        bool flag2 = (state & 4) != 0;
        bool flag3 = (state & 8) != 0;
        if (num != 0)
          this.OnItemStateBoolATrue?.Invoke();
        else
          this.OnItemStateBoolAFalse?.Invoke();
        if (flag1)
          this.OnItemStateBoolBTrue?.Invoke();
        else
          this.OnItemStateBoolBFalse?.Invoke();
        if (flag2)
          this.OnItemStateBoolCTrue?.Invoke();
        else
          this.OnItemStateBoolCFalse?.Invoke();
        if (flag3)
        {
          this.OnItemStateBoolDTrue?.Invoke();
          break;
        }
        this.OnItemStateBoolDFalse?.Invoke();
        break;
      case TransferrableObject.SyncOptions.Int:
        this.OnItemStateIntChanged?.Invoke(state);
        break;
    }
  }

  public void Launch(
    Vector3 startPosition,
    Quaternion startRotation,
    Vector3 velocity,
    float chargeFrac,
    VRRig ownerRig,
    int progress)
  {
    this.transform.position = startPosition;
    this.transform.rotation = startRotation;
    this.transform.localScale = Vector3.one * ownerRig.scaleFactor;
    this.rb.linearVelocity = velocity;
  }

  private void OnCollisionEnter(Collision other)
  {
    if (this.collisionEntered)
      return;
    Vector3 point = other.contacts[0].point;
    Vector3 normal = other.contacts[0].normal;
    this.OnCollisionEntered?.Invoke(this, normal);
    if ((double) this.sizzleDuration > 0.0)
    {
      this.StartCoroutine(this.Sizzle(point, normal));
    }
    else
    {
      this.OnDetonationStart?.Invoke(this, point);
      this.Detonate(point, normal);
    }
    this.collisionEntered = true;
  }

  private IEnumerator Sizzle(Vector3 contactPoint, Vector3 normal)
  {
    FirecrackerProjectile firecrackerProjectile = this;
    if ((bool) (UnityEngine.Object) firecrackerProjectile.audioSource && (UnityEngine.Object) firecrackerProjectile.sizzleAudioClip != (UnityEngine.Object) null)
      firecrackerProjectile.audioSource.GTPlayOneShot(firecrackerProjectile.sizzleAudioClip);
    yield return (object) new WaitForSeconds(firecrackerProjectile.sizzleDuration);
    firecrackerProjectile.OnDetonationStart?.Invoke(firecrackerProjectile, contactPoint);
    firecrackerProjectile.Detonate(contactPoint, normal);
  }

  private void Detonate(Vector3 contactPoint, Vector3 normal)
  {
    this.timeExploded = Time.time;
    GameObject gameObject = ObjectPools.instance.Instantiate(this.explosionEffect, contactPoint);
    gameObject.transform.up = normal;
    gameObject.transform.position = this.transform.position;
    SoundBankPlayer component;
    if (gameObject.TryGetComponent<SoundBankPlayer>(out component) && (bool) (UnityEngine.Object) component.soundBank)
      component.Play();
    if ((bool) (UnityEngine.Object) this.disableWhenHit)
      this.disableWhenHit.SetActive(false);
    this.collisionEntered = false;
  }
}
