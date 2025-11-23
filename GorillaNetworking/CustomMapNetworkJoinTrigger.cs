// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.CustomMapNetworkJoinTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace GorillaNetworking;

public class CustomMapNetworkJoinTrigger : GorillaNetworkJoinTrigger
{
  public override string GetFullDesiredGameModeString()
  {
    return $"{this.networkZone}{GorillaComputer.instance.currentQueue}{CustomMapLoader.LoadedMapModId.ToString()}_{CustomMapLoader.LoadedMapModFileId.ToString()}{this.GetDesiredGameType()}";
  }

  public override byte GetRoomSize() => CustomMapLoader.GetRoomSizeForCurrentlyLoadedMap();
}
