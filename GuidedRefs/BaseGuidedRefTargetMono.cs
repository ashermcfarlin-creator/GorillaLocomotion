// Decompiled with JetBrains decompiler
// Type: GorillaTag.GuidedRefs.BaseGuidedRefTargetMono
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.GuidedRefs;

public abstract class BaseGuidedRefTargetMono : 
  MonoBehaviour,
  IGuidedRefTargetMono,
  IGuidedRefMonoBehaviour,
  IGuidedRefObject
{
  public GuidedRefBasicTargetInfo guidedRefTargetInfo;

  protected virtual void Awake() => ((IGuidedRefObject) this).GuidedRefInitialize();

  protected virtual void OnDestroy()
  {
    GuidedRefHub.UnregisterTarget<BaseGuidedRefTargetMono>(this);
  }

  GuidedRefBasicTargetInfo IGuidedRefTargetMono.GRefTargetInfo
  {
    get => this.guidedRefTargetInfo;
    set => this.guidedRefTargetInfo = value;
  }

  Object IGuidedRefTargetMono.GuidedRefTargetObject => (Object) this;

  void IGuidedRefObject.GuidedRefInitialize()
  {
    GuidedRefHub.RegisterTarget<BaseGuidedRefTargetMono>(this, this.guidedRefTargetInfo.hubIds, (Component) this);
  }

  Transform IGuidedRefMonoBehaviour.get_transform() => this.transform;

  int IGuidedRefObject.GetInstanceID() => this.GetInstanceID();
}
