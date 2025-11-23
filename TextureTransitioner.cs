// Decompiled with JetBrains decompiler
// Type: GorillaTag.TextureTransitioner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag;

[ExecuteAlways]
public class TextureTransitioner : MonoBehaviour, IResettableItem
{
  public bool editorPreview;
  [Tooltip("The component that will drive the texture transitions.")]
  public MonoBehaviour dynamicFloatComponent;
  [Tooltip("Set these values so that after remap 0 is the first texture in the textures list and 1 is the last.")]
  public GorillaMath.RemapFloatInfo remapInfo;
  public TextureTransitioner.DirectionRetentionMode directionRetentionMode;
  public string texTransitionShaderParamName = "_TexTransition";
  public string tex1ShaderParamName = "_MainTex";
  public string tex2ShaderParamName = "_Tex2";
  public Texture[] textures;
  public Renderer[] renderers;
  [NonSerialized]
  public IDynamicFloat iDynamicFloat;
  [NonSerialized]
  public int texTransitionShaderParam;
  [NonSerialized]
  public int tex1ShaderParam;
  [NonSerialized]
  public int tex2ShaderParam;
  [DebugReadout]
  [NonSerialized]
  public float normalizedValue;
  [DebugReadout]
  [NonSerialized]
  public int transitionPercent;
  [DebugReadout]
  [NonSerialized]
  public int tex1Index;
  [DebugReadout]
  [NonSerialized]
  public int tex2Index;

  protected void Awake()
  {
    if (Application.isPlaying || this.editorPreview)
      TextureTransitionerManager.EnsureInstanceIsAvailable();
    this.RefreshShaderParams();
    this.iDynamicFloat = (IDynamicFloat) this.dynamicFloatComponent;
    this.ResetToDefaultState();
  }

  protected void OnEnable()
  {
    TextureTransitionerManager.Register(this);
    if (Application.isPlaying && !this.remapInfo.IsValid())
    {
      Debug.LogError((object) ("Bad min/max values for remapRanges: " + this.GetComponentPath<TextureTransitioner>()), (UnityEngine.Object) this);
      this.enabled = false;
    }
    if (Application.isPlaying && this.textures.Length == 0)
    {
      Debug.LogError((object) ("Textures array is empty: " + this.GetComponentPath<TextureTransitioner>()), (UnityEngine.Object) this);
      this.enabled = false;
    }
    if (!Application.isPlaying || this.iDynamicFloat != null)
      return;
    if ((UnityEngine.Object) this.dynamicFloatComponent == (UnityEngine.Object) null)
      Debug.LogError((object) ("dynamicFloatComponent cannot be null: " + this.GetComponentPath<TextureTransitioner>()), (UnityEngine.Object) this);
    this.iDynamicFloat = (IDynamicFloat) this.dynamicFloatComponent;
    if (this.iDynamicFloat != null)
      return;
    Debug.LogError((object) ("Component assigned to dynamicFloatComponent does not implement IDynamicFloat: " + this.GetComponentPath<TextureTransitioner>()), (UnityEngine.Object) this);
    this.enabled = false;
  }

  protected void OnDisable() => TextureTransitionerManager.Unregister(this);

  private void RefreshShaderParams()
  {
    this.texTransitionShaderParam = Shader.PropertyToID(this.texTransitionShaderParamName);
    this.tex1ShaderParam = Shader.PropertyToID(this.tex1ShaderParamName);
    this.tex2ShaderParam = Shader.PropertyToID(this.tex2ShaderParamName);
  }

  public void ResetToDefaultState()
  {
    this.normalizedValue = 0.0f;
    this.transitionPercent = 0;
    this.tex1Index = 0;
    this.tex2Index = 0;
  }

  public enum DirectionRetentionMode
  {
    None,
    IncreaseOnly,
    DecreaseOnly,
  }
}
