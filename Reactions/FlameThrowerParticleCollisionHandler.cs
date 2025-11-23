// Decompiled with JetBrains decompiler
// Type: GorillaTag.Reactions.FlameThrowerParticleCollisionHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaNetworking;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag.Reactions;

public class FlameThrowerParticleCollisionHandler : MonoBehaviour
{
  [Tooltip("The defaults are numbers for the flamethrower hair dryer.")]
  private readonly float _maxParticleHitReactionRate = 2f;
  [Tooltip("Must be in the global object pool and have a tag.")]
  [SerializeField]
  private GameObject _prefabToSpawn;
  [Tooltip("How much to extinguish any hit fire by.")]
  [SerializeField]
  private float _extinguishAmount;
  private ParticleSystem _particleSystem;
  private List<ParticleCollisionEvent> _collisionEvents;
  private bool _hasPrefabToSpawn;
  private bool _isPrefabInPool;
  private double _lastCollisionTime;
  private SinglePool _pool;

  protected void OnEnable()
  {
    if ((Object) GorillaComputer.instance == (Object) null)
    {
      Debug.LogError((object) ("FlameThrowerParticleCollisionHandler: Disabling because GorillaComputer not found! Hierarchy path: " + this.transform.GetPath()), (Object) this);
      this.enabled = false;
    }
    else
    {
      if ((Object) this._prefabToSpawn != (Object) null && !this._isPrefabInPool)
      {
        if (this._prefabToSpawn.CompareTag("Untagged"))
        {
          Debug.LogError((object) ("FlameThrowerParticleCollisionHandler: Disabling because Spawn Prefab has no tag! Hierarchy path: " + this.transform.GetPath()), (Object) this);
          this.enabled = false;
          return;
        }
        this._isPrefabInPool = ObjectPools.instance.DoesPoolExist(this._prefabToSpawn);
        if (!this._isPrefabInPool)
        {
          Debug.LogError((object) ("FlameThrowerParticleCollisionHandler: Disabling because Spawn Prefab not in pool! Hierarchy path: " + this.transform.GetPath()), (Object) this);
          this.enabled = false;
          return;
        }
        this._pool = ObjectPools.instance.GetPoolByObjectType(this._prefabToSpawn);
      }
      this._hasPrefabToSpawn = (Object) this._prefabToSpawn != (Object) null && this._isPrefabInPool;
      if ((Object) this._particleSystem == (Object) null)
        this._particleSystem = this.GetComponent<ParticleSystem>();
      if ((Object) this._particleSystem == (Object) null)
      {
        Debug.LogError((object) ("FlameThrowerParticleCollisionHandler: Disabling because could not find ParticleSystem! Hierarchy path: " + this.transform.GetPath()), (Object) this);
        this.enabled = false;
      }
      else
      {
        if (this._collisionEvents != null)
          return;
        this._collisionEvents = new List<ParticleCollisionEvent>(this._particleSystem.main.maxParticles);
      }
    }
  }

  protected void OnParticleCollision(GameObject other)
  {
    if ((double) this._maxParticleHitReactionRate < 9.9999997473787516E-06 || !FireManager.hasInstance)
      return;
    double num = GTTime.TimeAsDouble();
    if (num - this._lastCollisionTime < 1.0 / (double) this._maxParticleHitReactionRate || this._particleSystem.GetCollisionEvents(other, this._collisionEvents) <= 0)
      return;
    if (this._hasPrefabToSpawn && this._isPrefabInPool && this._pool.GetInactiveCount() > 0)
    {
      ParticleCollisionEvent collisionEvent = this._collisionEvents[0];
      FireManager.SpawnFire(this._pool, collisionEvent.intersection, collisionEvent.normal, this.transform.lossyScale.x);
    }
    if ((double) this._extinguishAmount > 0.0)
      FireManager.Extinguish(other, this._extinguishAmount);
    this._lastCollisionTime = num;
  }
}
