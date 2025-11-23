// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.GorillaNetworkLeaveRoomTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaTagScripts;
using System.Threading.Tasks;
using UnityEngine;

#nullable disable
namespace GorillaNetworking;

public class GorillaNetworkLeaveRoomTrigger : GorillaTriggerBox
{
  [SerializeField]
  private bool excludePrivateRooms;

  public override void OnBoxTriggered()
  {
    base.OnBoxTriggered();
    if (!NetworkSystem.Instance.InRoom || this.excludePrivateRooms && NetworkSystem.Instance.SessionIsPrivate)
      return;
    if (FriendshipGroupDetection.Instance.IsInParty)
    {
      FriendshipGroupDetection.Instance.LeaveParty();
      this.DisconnectAfterDelay(1f);
    }
    else
      NetworkSystem.Instance.ReturnToSinglePlayer();
  }

  private async void DisconnectAfterDelay(float seconds)
  {
    await Task.Delay((int) (1000.0 * (double) seconds));
    await NetworkSystem.Instance.ReturnToSinglePlayer();
  }
}
