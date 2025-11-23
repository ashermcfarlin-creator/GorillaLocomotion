// Decompiled with JetBrains decompiler
// Type: GorillaTag.CosmeticCameraDisableNotifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaTag;

[RequireComponent(typeof (VRRigCollection))]
public class CosmeticCameraDisableNotifier : MonoBehaviour
{
  private VRRigCollection _vrrigCollection;
  [SerializeField]
  private Camera _cosmeticCamera;

  private void Awake()
  {
    if (!this.TryGetComponent<VRRigCollection>(out this._vrrigCollection))
      this._vrrigCollection = this.AddComponent<VRRigCollection>();
    this._vrrigCollection.playerEnteredCollection += new Action<RigContainer>(this.PlayerEnteredTryOnSpace);
    this._vrrigCollection.playerLeftCollection += new Action<RigContainer>(this.PlayerLeftTryOnSpace);
  }

  private void PlayerEnteredTryOnSpace(RigContainer playerRig)
  {
    if (!playerRig.Rig.isLocal)
      return;
    this._cosmeticCamera.enabled = false;
  }

  private void PlayerLeftTryOnSpace(RigContainer playerRig)
  {
    if (!playerRig.Rig.isLocal)
      return;
    this._cosmeticCamera.enabled = true;
  }
}
