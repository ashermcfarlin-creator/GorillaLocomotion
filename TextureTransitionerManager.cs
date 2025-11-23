// Decompiled with JetBrains decompiler
// Type: GorillaTag.TextureTransitionerManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag;

[ExecuteAlways]
public class TextureTransitionerManager : MonoBehaviour
{
  public static readonly List<TextureTransitioner> components = new List<TextureTransitioner>(256 /*0x0100*/);
  private MaterialPropertyBlock matPropBlock;

  public static TextureTransitionerManager instance { get; private set; }

  protected void Awake()
  {
    if ((Object) TextureTransitionerManager.instance != (Object) null)
    {
      Object.Destroy((Object) this.gameObject);
    }
    else
    {
      TextureTransitionerManager.instance = this;
      if (Application.isPlaying)
        Object.DontDestroyOnLoad((Object) this.gameObject);
      this.matPropBlock = new MaterialPropertyBlock();
    }
  }

  protected void LateUpdate()
  {
    foreach (TextureTransitioner component in TextureTransitionerManager.components)
    {
      int length = component.textures.Length;
      float a = Mathf.Clamp01(component.remapInfo.Remap(component.iDynamicFloat.floatValue));
      switch (component.directionRetentionMode)
      {
        case TextureTransitioner.DirectionRetentionMode.IncreaseOnly:
          a = Mathf.Max(a, component.normalizedValue);
          break;
        case TextureTransitioner.DirectionRetentionMode.DecreaseOnly:
          a = Mathf.Min(a, component.normalizedValue);
          break;
      }
      double num1 = (double) a * (double) (length - 1);
      float num2 = (float) (num1 % 1.0);
      int num3 = (int) ((double) num2 * 1000.0);
      int index1 = (int) num1;
      int index2 = Mathf.Min(length - 1, index1 + 1);
      if (num3 != component.transitionPercent || index1 != component.tex1Index || index2 != component.tex2Index)
      {
        this.matPropBlock.SetFloat(component.texTransitionShaderParam, num2);
        this.matPropBlock.SetTexture(component.tex1ShaderParam, component.textures[index1]);
        this.matPropBlock.SetTexture(component.tex2ShaderParam, component.textures[index2]);
        foreach (Renderer renderer in component.renderers)
          renderer.SetPropertyBlock(this.matPropBlock);
        component.normalizedValue = a;
        component.transitionPercent = num3;
        component.tex1Index = index1;
        component.tex2Index = index2;
      }
    }
  }

  public static void EnsureInstanceIsAvailable()
  {
    if ((Object) TextureTransitionerManager.instance != (Object) null)
      return;
    GameObject gameObject = new GameObject();
    TextureTransitionerManager.instance = gameObject.AddComponent<TextureTransitionerManager>();
    gameObject.name = "TextureTransitionerManager (Singleton)";
  }

  public static void Register(TextureTransitioner component)
  {
    TextureTransitionerManager.components.Add(component);
  }

  public static void Unregister(TextureTransitioner component)
  {
    TextureTransitionerManager.components.Remove(component);
  }
}
