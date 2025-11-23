// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.MaterialChangerCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class MaterialChangerCosmetic : MonoBehaviour
{
  [SerializeField]
  private SkinnedMeshRenderer targetRenderer;
  [SerializeField]
  private int materialIndex;

  public void ChangeMaterial(Material newMaterial)
  {
    if ((Object) this.targetRenderer == (Object) null || (Object) newMaterial == (Object) null || this.materialIndex < 0)
      return;
    Material[] materials = this.targetRenderer.materials;
    if (this.materialIndex >= materials.Length)
    {
      Debug.LogWarning((object) $"Material index {this.materialIndex} is out of range.");
    }
    else
    {
      materials[this.materialIndex] = newMaterial;
      this.targetRenderer.materials = materials;
    }
  }

  public void ChangeAllMaterials(Material newMat)
  {
    if ((Object) this.targetRenderer == (Object) null || (Object) newMat == (Object) null)
      return;
    Material[] materialArray = new Material[this.targetRenderer.materials.Length];
    for (int index = 0; index < materialArray.Length; ++index)
      materialArray[index] = newMat;
    this.targetRenderer.materials = materialArray;
  }
}
