// Decompiled with JetBrains decompiler
// Type: GorillaTag.ExpectedUsersDecayTimer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag;

[Serializable]
internal class ExpectedUsersDecayTimer : TickSystemTimerAbstract
{
  public float decayTime = 15f;
  private Dictionary<string, float> expectedUsers = new Dictionary<string, float>(10);

  public override void OnTimedEvent()
  {
    if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.IsMasterClient)
      return;
    int num1 = 0;
    if (PhotonNetwork.CurrentRoom.ExpectedUsers == null || PhotonNetwork.CurrentRoom.ExpectedUsers.Length == 0)
      return;
    foreach (string expectedUser in PhotonNetwork.CurrentRoom.ExpectedUsers)
    {
      float num2;
      if (this.expectedUsers.TryGetValue(expectedUser, out num2))
      {
        if ((double) num2 + (double) this.decayTime < (double) Time.time)
          ++num1;
      }
      else
        this.expectedUsers.Add(expectedUser, Time.time);
    }
    if (num1 < PhotonNetwork.CurrentRoom.ExpectedUsers.Length || num1 == 0)
      return;
    PhotonNetwork.CurrentRoom.ClearExpectedUsers();
    this.expectedUsers.Clear();
  }

  public override void Stop()
  {
    base.Stop();
    this.expectedUsers.Clear();
  }
}
