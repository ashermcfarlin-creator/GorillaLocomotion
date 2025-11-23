// Decompiled with JetBrains decompiler
// Type: GorillaTag.GuidedRefs.GuidedRefBasicTargetInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaTag.GuidedRefs;

[Serializable]
public struct GuidedRefBasicTargetInfo
{
  [SerializeField]
  public GuidedRefTargetIdSO targetId;
  [Tooltip("Used to filter down which relay the target can belong to. If null or empty then all parents with a GuidedRefRelayHub will be used.")]
  [SerializeField]
  public GuidedRefHubIdSO[] hubIds;
  [DebugOption]
  [SerializeField]
  public bool hackIgnoreDuplicateRegistration;
}
