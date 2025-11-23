// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.GorillaNetworkLobbyJoinTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaNetworking;

public class GorillaNetworkLobbyJoinTrigger : GorillaTriggerBox
{
  public GameObject[] makeSureThisIsDisabled;
  public GameObject[] makeSureThisIsEnabled;
  public string gameModeName;
  public PhotonNetworkController photonNetworkController;
  public string componentTypeToRemove;
  public GameObject componentRemoveTarget;
  public string componentTypeToAdd;
  public GameObject componentAddTarget;
  public GameObject gorillaParent;
  public GameObject joinFailedBlock;
}
