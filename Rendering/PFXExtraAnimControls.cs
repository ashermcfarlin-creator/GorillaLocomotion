// Decompiled with JetBrains decompiler
// Type: GorillaTag.Rendering.PFXExtraAnimControls
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Rendering;

public class PFXExtraAnimControls : MonoBehaviour
{
  public float emitRateMult = 1f;
  public float emitBurstProbabilityMult = 1f;
  [SerializeField]
  private ParticleSystem[] particleSystems;
  private ParticleSystem.EmissionModule[] emissionModules;
  private ParticleSystem.Burst[][] cachedEmitBursts;
  private ParticleSystem.Burst[][] adjustedEmitBursts;

  protected void Awake()
  {
    this.emissionModules = new ParticleSystem.EmissionModule[this.particleSystems.Length];
    this.cachedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
    this.adjustedEmitBursts = new ParticleSystem.Burst[this.particleSystems.Length][];
    for (int index1 = 0; index1 < this.particleSystems.Length; ++index1)
    {
      ParticleSystem.EmissionModule emission = this.particleSystems[index1].emission;
      this.cachedEmitBursts[index1] = new ParticleSystem.Burst[emission.burstCount];
      this.adjustedEmitBursts[index1] = new ParticleSystem.Burst[emission.burstCount];
      for (int index2 = 0; index2 < emission.burstCount; ++index2)
      {
        this.cachedEmitBursts[index1][index2] = emission.GetBurst(index2);
        this.adjustedEmitBursts[index1][index2] = emission.GetBurst(index2);
      }
      this.emissionModules[index1] = emission;
    }
  }

  protected void LateUpdate()
  {
    for (int index1 = 0; index1 < this.emissionModules.Length; ++index1)
    {
      this.emissionModules[index1].rateOverTimeMultiplier = this.emitRateMult;
      Mathf.Min(this.emissionModules[index1].burstCount, this.cachedEmitBursts[index1].Length);
      for (int index2 = 0; index2 < this.cachedEmitBursts[index1].Length; ++index2)
        this.adjustedEmitBursts[index1][index2].probability = this.cachedEmitBursts[index1][index2].probability * this.emitBurstProbabilityMult;
      this.emissionModules[index1].SetBursts(this.adjustedEmitBursts[index1]);
    }
  }
}
