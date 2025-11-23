// Decompiled with JetBrains decompiler
// Type: GorillaTag.Rendering.ZoneLiquidEffectableManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag.Rendering;

public class ZoneLiquidEffectableManager : MonoBehaviour
{
  private readonly List<ZoneLiquidEffectable> zoneLiquidEffectables = new List<ZoneLiquidEffectable>(32 /*0x20*/);

  public static ZoneLiquidEffectableManager instance { get; private set; }

  public static bool hasInstance { get; private set; }

  protected void Awake()
  {
    if (ZoneLiquidEffectableManager.hasInstance && (Object) ZoneLiquidEffectableManager.instance != (Object) this)
      Object.Destroy((Object) this.gameObject);
    else
      ZoneLiquidEffectableManager.SetInstance(this);
  }

  protected void OnDestroy()
  {
    if (!((Object) ZoneLiquidEffectableManager.instance == (Object) this))
      return;
    ZoneLiquidEffectableManager.hasInstance = false;
    ZoneLiquidEffectableManager.instance = (ZoneLiquidEffectableManager) null;
  }

  protected void LateUpdate()
  {
    int layerMask = UnityLayer.Water.ToLayerMask();
    foreach (ZoneLiquidEffectable liquidEffectable in this.zoneLiquidEffectables)
    {
      Transform transform = liquidEffectable.transform;
      liquidEffectable.inLiquidVolume = Physics.CheckSphere(transform.position, liquidEffectable.radius * transform.lossyScale.x, layerMask);
      if (liquidEffectable.inLiquidVolume != liquidEffectable.wasInLiquidVolume)
      {
        for (int index = 0; index < liquidEffectable.childRenderers.Length; ++index)
        {
          if (liquidEffectable.inLiquidVolume)
          {
            liquidEffectable.childRenderers[index].material.EnableKeyword("_WATER_EFFECT");
            liquidEffectable.childRenderers[index].material.EnableKeyword("_HEIGHT_BASED_WATER_EFFECT");
          }
          else
          {
            liquidEffectable.childRenderers[index].material.DisableKeyword("_WATER_EFFECT");
            liquidEffectable.childRenderers[index].material.DisableKeyword("_HEIGHT_BASED_WATER_EFFECT");
          }
        }
      }
      liquidEffectable.wasInLiquidVolume = liquidEffectable.inLiquidVolume;
    }
  }

  private static void CreateManager()
  {
    ZoneLiquidEffectableManager.SetInstance(new GameObject(nameof (ZoneLiquidEffectableManager)).AddComponent<ZoneLiquidEffectableManager>());
  }

  private static void SetInstance(ZoneLiquidEffectableManager manager)
  {
    ZoneLiquidEffectableManager.instance = manager;
    ZoneLiquidEffectableManager.hasInstance = true;
    if (!Application.isPlaying)
      return;
    Object.DontDestroyOnLoad((Object) manager);
  }

  public static void Register(ZoneLiquidEffectable effect)
  {
    if (!ZoneLiquidEffectableManager.hasInstance)
      ZoneLiquidEffectableManager.CreateManager();
    if ((Object) effect == (Object) null || ZoneLiquidEffectableManager.instance.zoneLiquidEffectables.Contains(effect))
      return;
    ZoneLiquidEffectableManager.instance.zoneLiquidEffectables.Add(effect);
    effect.inLiquidVolume = false;
    for (int index = 0; index < effect.childRenderers.Length; ++index)
    {
      if (!((Object) effect.childRenderers[index] == (Object) null))
      {
        Material sharedMaterial = effect.childRenderers[index].sharedMaterial;
        if (!((Object) sharedMaterial == (Object) null) || sharedMaterial.shader.keywordSpace.FindKeyword("_WATER_EFFECT").isValid)
        {
          effect.inLiquidVolume = sharedMaterial.IsKeywordEnabled("_WATER_EFFECT") && sharedMaterial.IsKeywordEnabled("_HEIGHT_BASED_WATER_EFFECT");
          break;
        }
      }
    }
  }

  public static void Unregister(ZoneLiquidEffectable effect)
  {
    ZoneLiquidEffectableManager.instance.zoneLiquidEffectables.Remove(effect);
  }
}
