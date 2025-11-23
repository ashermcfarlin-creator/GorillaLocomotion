// Decompiled with JetBrains decompiler
// Type: GorillaTag.GTAppState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaTag;

public static class GTAppState
{
  [field: OnEnterPlay_Set(false)]
  public static bool isQuitting { get; private set; }

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
  private static void HandleOnSubsystemRegistration()
  {
    GTAppState.isQuitting = false;
    Application.quitting += (Action) (() => GTAppState.isQuitting = true);
    Debug.Log((object) $"GTAppState:\n- SystemInfo.operatingSystem={SystemInfo.operatingSystem}\n- SystemInfo.maxTextureArraySlices={SystemInfo.maxTextureArraySlices.ToString()}\n");
  }

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
  private static void HandleOnAfterSceneLoad()
  {
  }
}
