// Decompiled with JetBrains decompiler
// Type: GorillaTag.CosmeticTryOnNotifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag;

[RequireComponent(typeof (VRRigCollection))]
public class CosmeticTryOnNotifier : MonoBehaviour
{
  private VRRigCollection m_vrrigCollection;
  [SerializeField]
  private CosmeticTryOnNotifier.Mode mode;
  [SerializeField]
  private StringList unlockList;

  private void Awake()
  {
    if (!this.TryGetComponent<VRRigCollection>(out this.m_vrrigCollection))
      this.m_vrrigCollection = this.AddComponent<VRRigCollection>();
    this.m_vrrigCollection.playerEnteredCollection += new Action<RigContainer>(this.PlayerEnteredTryOnSpace);
    this.m_vrrigCollection.playerLeftCollection += new Action<RigContainer>(this.PlayerLeftTryOnSpace);
  }

  private void PlayerEnteredTryOnSpace(RigContainer playerRig)
  {
    switch (this.mode)
    {
      case CosmeticTryOnNotifier.Mode.TRY_ON:
        PlayerCosmeticsSystem.SetRigTryOn(true, playerRig);
        break;
      case CosmeticTryOnNotifier.Mode.ENABLE_LIST:
        PlayerCosmeticsSystem.UnlockTemporaryCosmeticsForPlayer(playerRig, (IReadOnlyList<string>) this.unlockList.Strings);
        break;
    }
  }

  private void PlayerLeftTryOnSpace(RigContainer playerRig)
  {
    switch (this.mode)
    {
      case CosmeticTryOnNotifier.Mode.TRY_ON:
        PlayerCosmeticsSystem.SetRigTryOn(false, playerRig);
        break;
      case CosmeticTryOnNotifier.Mode.ENABLE_LIST:
        PlayerCosmeticsSystem.LockTemporaryCosmeticsForPlayer(playerRig, (IReadOnlyList<string>) this.unlockList.Strings);
        break;
    }
  }

  private enum Mode
  {
    TRY_ON,
    ENABLE_LIST,
    ENABLE_LIST_TITLEDATA,
  }
}
