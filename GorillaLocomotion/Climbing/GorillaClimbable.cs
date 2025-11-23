// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Climbing.GorillaClimbable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Climbing;

public class GorillaClimbable : MonoBehaviour
{
  public bool snapX;
  public bool snapY;
  public bool snapZ;
  public float maxDistanceSnap = 0.05f;
  public AudioClip clip;
  public AudioClip clipOnFullRelease;
  public Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb;
  public bool climbOnlyWhileSmall;
  public bool IsPlayerAttached;
  [NonSerialized]
  public bool isBeingClimbed;
  [NonSerialized]
  public Collider colliderCache;

  private void Awake() => this.colliderCache = this.GetComponent<Collider>();
}
