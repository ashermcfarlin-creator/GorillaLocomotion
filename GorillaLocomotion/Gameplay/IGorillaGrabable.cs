// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.IGorillaGrabable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Gameplay;

internal interface IGorillaGrabable
{
  string name { get; }

  bool MomentaryGrabOnly();

  bool CanBeGrabbed(GorillaGrabber grabber);

  void OnGrabbed(
    GorillaGrabber grabber,
    out Transform grabbedTransform,
    out Vector3 localGrabbedPosition);

  void OnGrabReleased(GorillaGrabber grabber);
}
