// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.FriendshipBracelet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaNetworking;

public class FriendshipBracelet : MonoBehaviour
{
  [SerializeField]
  private SkinnedMeshRenderer[] braceletStrings;
  [SerializeField]
  private MeshRenderer[] braceletBeads;
  [SerializeField]
  private MeshRenderer[] braceletBananas;
  [SerializeField]
  private bool isLeftHand;
  [SerializeField]
  private AudioClip braceletFormedSound;
  [SerializeField]
  private AudioClip braceletBrokenSound;
  [SerializeField]
  private ParticleSystem braceletFormedParticle;
  [SerializeField]
  private ParticleSystem braceletBrokenParticle;
  private VRRig ownerRig;

  protected void Awake() => this.ownerRig = this.GetComponentInParent<VRRig>();

  private AudioSource GetAudioSource()
  {
    return !this.isLeftHand ? this.ownerRig.rightHandPlayer : this.ownerRig.leftHandPlayer;
  }

  private void OnEnable() => this.PlayAppearEffects();

  public void PlayAppearEffects()
  {
    this.GetAudioSource().GTPlayOneShot(this.braceletFormedSound);
    if (!(bool) (Object) this.braceletFormedParticle)
      return;
    this.braceletFormedParticle.Play();
  }

  private void OnDisable()
  {
    if (!this.ownerRig.gameObject.activeInHierarchy)
      return;
    this.GetAudioSource().GTPlayOneShot(this.braceletBrokenSound);
    if (!(bool) (Object) this.braceletBrokenParticle)
      return;
    this.braceletBrokenParticle.Play();
  }

  public void UpdateBeads(List<Color> colors, int selfIndex)
  {
    int num1 = colors.Count - 1;
    int num2 = (this.braceletBeads.Length - num1) / 2;
    for (int index1 = 0; index1 < this.braceletBeads.Length; ++index1)
    {
      int index2 = index1 - num2;
      if (index2 >= 0 && index2 < num1)
      {
        this.braceletBeads[index1].enabled = true;
        this.braceletBeads[index1].material.color = colors[index2];
        this.braceletBananas[index1].gameObject.SetActive(index2 == selfIndex);
      }
      else
      {
        this.braceletBeads[index1].enabled = false;
        this.braceletBananas[index1].gameObject.SetActive(false);
      }
    }
    foreach (Renderer braceletString in this.braceletStrings)
      braceletString.material.color = colors[colors.Count - 1];
  }
}
