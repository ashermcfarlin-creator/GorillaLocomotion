// Decompiled with JetBrains decompiler
// Type: GorillaTag.Rendering.WaterBubbleParticleVolumeCollector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable
namespace GorillaTag.Rendering;

public class WaterBubbleParticleVolumeCollector : MonoBehaviour
{
  public ParticleSystem[] particleSystems;
  private ParticleSystem.TriggerModule[] particleTriggerModules;
  private ParticleSystem.EmissionModule[] particleEmissionModules;
  private Collider[] bubbleableVolumeColliders;
  private bool emissionEnabled;

  protected void Awake()
  {
    List<WaterVolume> componentsInHierarchy = SceneManager.GetActiveScene().GetComponentsInHierarchy<WaterVolume>();
    List<Collider> colliderList = new List<Collider>(componentsInHierarchy.Count * 4);
    foreach (WaterVolume waterVolume in componentsInHierarchy)
    {
      if (!((Object) waterVolume.Parameters != (Object) null) || waterVolume.Parameters.allowBubblesInVolume)
      {
        foreach (Collider volumeCollider in waterVolume.volumeColliders)
        {
          if (!((Object) volumeCollider == (Object) null))
            colliderList.Add(volumeCollider);
        }
      }
    }
    this.bubbleableVolumeColliders = colliderList.ToArray();
    this.particleTriggerModules = new ParticleSystem.TriggerModule[this.particleSystems.Length];
    this.particleEmissionModules = new ParticleSystem.EmissionModule[this.particleSystems.Length];
    for (int index = 0; index < this.particleSystems.Length; ++index)
    {
      this.particleTriggerModules[index] = this.particleSystems[index].trigger;
      this.particleEmissionModules[index] = this.particleSystems[index].emission;
    }
    for (int index1 = 0; index1 < this.particleSystems.Length; ++index1)
    {
      ParticleSystem.TriggerModule particleTriggerModule = this.particleTriggerModules[index1];
      for (int index2 = 0; index2 < colliderList.Count; ++index2)
        particleTriggerModule.SetCollider(index2, (Component) this.bubbleableVolumeColliders[index2]);
    }
    this.SetEmissionState(false);
  }

  protected void LateUpdate()
  {
    bool headInWater = GTPlayer.Instance.HeadInWater;
    if (headInWater && !this.emissionEnabled)
    {
      this.SetEmissionState(true);
    }
    else
    {
      if (headInWater || !this.emissionEnabled)
        return;
      this.SetEmissionState(false);
    }
  }

  private void SetEmissionState(bool setEnabled)
  {
    float num = setEnabled ? 1f : 0.0f;
    for (int index = 0; index < this.particleEmissionModules.Length; ++index)
      this.particleEmissionModules[index].rateOverTimeMultiplier = num;
    this.emissionEnabled = setEnabled;
  }
}
