// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.GorillaNetworkRankedJoinTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace GorillaNetworking;

public class GorillaNetworkRankedJoinTrigger : GorillaNetworkJoinTrigger
{
  public override string GetFullDesiredGameModeString()
  {
    return this.networkZone + this.GetDesiredGameType();
  }

  public override void OnBoxTriggered()
  {
    GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
    PhotonNetworkController.Instance.ClearDeferredJoin();
    PhotonNetworkController.Instance.AttemptToJoinRankedPublicRoom((GorillaNetworkJoinTrigger) this);
  }
}
