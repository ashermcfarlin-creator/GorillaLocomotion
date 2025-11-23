// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.Summer.Projectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTag.Reactions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#nullable disable
namespace GorillaTag.Cosmetics.Summer;

public class Projectile : MonoBehaviour, IProjectile
{
  [SerializeField]
  private AudioSource audioSource;
  [SerializeField]
  private GameObject impactEffect;
  [SerializeField]
  private AudioClip launchAudio;
  [SerializeField]
  private LayerMask collisionLayerMasks;
  [SerializeField]
  private List<string> collisionTags = new List<string>();
  [SerializeField]
  private bool destroyOnCollisionEnter;
  [SerializeField]
  private float destroyDelay = 1f;
  [Tooltip("Distance from the surface that the particle should spawn.")]
  [SerializeField]
  private float impactEffectOffset = 0.1f;
  [SerializeField]
  private SpawnWorldEffects spawnWorldEffects;
  private ConstantForce forceComponent;
  public UnityEvent<float> onLaunchShared;
  public UnityEvent onImpactShared;
  private bool impactEffectSpawned;
  private Rigidbody rigidbody;

  protected void Awake()
  {
    this.rigidbody = this.GetComponentInChildren<Rigidbody>();
    this.impactEffectSpawned = false;
    this.forceComponent = this.GetComponent<ConstantForce>();
  }

  protected void OnEnable()
  {
  }

  public void Launch(
    Vector3 startPosition,
    Quaternion startRotation,
    Vector3 velocity,
    float chargeFrac,
    VRRig ownerRig,
    int progressStep)
  {
    Transform transform = this.transform;
    transform.SetPositionAndRotation(startPosition, startRotation);
    transform.localScale = Vector3.one * ownerRig.scaleFactor;
    if ((Object) this.rigidbody != (Object) null)
      this.rigidbody.linearVelocity = velocity;
    if ((bool) (Object) this.audioSource && (bool) (Object) this.launchAudio)
      this.audioSource.GTPlayOneShot(this.launchAudio);
    this.onLaunchShared?.Invoke(chargeFrac);
  }

  private bool IsTagValid(GameObject obj) => this.collisionTags.Contains(obj.tag);

  private void HandleImpact(GameObject hitObject, Vector3 hitPosition, Vector3 hitNormal)
  {
    if (this.impactEffectSpawned || this.collisionTags.Count > 0 && !this.IsTagValid(hitObject) || (1 << hitObject.layer & (int) this.collisionLayerMasks) == 0)
      return;
    this.SpawnImpactEffect(this.impactEffect, hitPosition, hitNormal);
    if ((Object) this.impactEffect != (Object) null)
    {
      SoundBankPlayer component = this.impactEffect.GetComponent<SoundBankPlayer>();
      if ((Object) component != (Object) null && !component.playOnEnable)
        component.Play();
    }
    this.impactEffectSpawned = true;
    if (!this.destroyOnCollisionEnter)
      return;
    if ((double) this.destroyDelay > 0.0)
      this.Invoke("DestroyProjectile", this.destroyDelay);
    else
      this.DestroyProjectile();
  }

  private void GetColliderHitInfo(Collider other, out Vector3 position, out Vector3 normal)
  {
    Vector3 vector3 = Time.fixedDeltaTime * 2f * this.rigidbody.linearVelocity;
    Vector3 origin = this.transform.position - vector3;
    float magnitude = vector3.magnitude;
    UnityEngine.RaycastHit hitInfo;
    other.Raycast(new Ray(origin, vector3 / magnitude), out hitInfo, 2f * magnitude);
    position = hitInfo.point;
    normal = hitInfo.normal;
  }

  private void OnCollisionEnter(Collision other)
  {
    ContactPoint contact = other.GetContact(0);
    this.HandleImpact(other.gameObject, contact.point, contact.normal);
  }

  private void OnCollisionStay(Collision other)
  {
    ContactPoint contact = other.GetContact(0);
    this.HandleImpact(other.gameObject, contact.point, contact.normal);
  }

  private void OnTriggerEnter(Collider other)
  {
    Vector3 position;
    Vector3 normal;
    this.GetColliderHitInfo(other, out position, out normal);
    this.HandleImpact(other.gameObject, position, normal);
  }

  private void OnTriggerStay(Collider other)
  {
    Transform transform = this.transform;
    this.HandleImpact(other.gameObject, transform.position, -transform.forward);
  }

  private void SpawnImpactEffect(GameObject prefab, Vector3 position, Vector3 normal)
  {
    if ((Object) prefab != (Object) null)
    {
      Vector3 position1 = position + normal * this.impactEffectOffset;
      GameObject gameObject = ObjectPools.instance.Instantiate(prefab, position1);
      gameObject.transform.up = normal;
      gameObject.transform.position = position1;
    }
    this.onImpactShared.Invoke();
    if (!((Object) this.spawnWorldEffects != (Object) null))
      return;
    this.spawnWorldEffects.RequestSpawn(position, normal);
  }

  private void DestroyProjectile()
  {
    this.impactEffectSpawned = false;
    if ((bool) (Object) this.forceComponent)
      this.forceComponent.enabled = false;
    if (ObjectPools.instance.DoesPoolExist(this.gameObject))
      ObjectPools.instance.Destroy(this.gameObject);
    else
      Object.Destroy((Object) this.gameObject);
  }
}
