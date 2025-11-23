// Decompiled with JetBrains decompiler
// Type: GorillaTag.BoneOffset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTag.CosmeticSystem;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag;

[Serializable]
public struct BoneOffset
{
  public GTHardCodedBones.SturdyEBone bone;
  public XformOffset offset;
  public static readonly BoneOffset Identity = new BoneOffset()
  {
    bone = (GTHardCodedBones.SturdyEBone) GTHardCodedBones.EBone.None,
    offset = XformOffset.Identity
  };

  public Vector3 pos => this.offset.pos;

  public Quaternion rot => this.offset.rot;

  public Vector3 scale => this.offset.scale;

  public BoneOffset(GTHardCodedBones.EBone bone)
  {
    this.bone = (GTHardCodedBones.SturdyEBone) bone;
    this.offset = XformOffset.Identity;
  }

  public BoneOffset(GTHardCodedBones.EBone bone, XformOffset offset)
  {
    this.bone = (GTHardCodedBones.SturdyEBone) bone;
    this.offset = offset;
  }

  public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot)
  {
    this.bone = (GTHardCodedBones.SturdyEBone) bone;
    this.offset = new XformOffset(pos, rot);
  }

  public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Vector3 rotAngles)
  {
    this.bone = (GTHardCodedBones.SturdyEBone) bone;
    this.offset = new XformOffset(pos, rotAngles);
  }

  public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot, Vector3 scale)
  {
    this.bone = (GTHardCodedBones.SturdyEBone) bone;
    this.offset = new XformOffset(pos, rot, scale);
  }

  public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Vector3 rotAngles, Vector3 scale)
  {
    this.bone = (GTHardCodedBones.SturdyEBone) bone;
    this.offset = new XformOffset(pos, rotAngles, scale);
  }
}
