// Decompiled with JetBrains decompiler
// Type: GorillaTag.GuidedRefs.GRef
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable disable
namespace GorillaTag.GuidedRefs;

public static class GRef
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool ShouldResolveNow(GRef.EResolveModes mode)
  {
    return Application.isPlaying && (mode & GRef.EResolveModes.Runtime) == GRef.EResolveModes.Runtime;
  }

  public static bool IsAnyResolveModeOn(GRef.EResolveModes mode) => mode != 0;

  [Flags]
  public enum EResolveModes
  {
    None = 0,
    Runtime = 1,
    SceneProcessing = 2,
  }
}
