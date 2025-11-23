// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.GorillaNetworkJoinTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaGameModes;
using GorillaTagScripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaNetworking;

public class GorillaNetworkJoinTrigger : GorillaTriggerBox
{
  public GameObject[] makeSureThisIsDisabled;
  public GameObject[] makeSureThisIsEnabled;
  public GTZone zone;
  public GroupJoinZoneA groupJoinRequiredZones;
  public GroupJoinZoneB groupJoinRequiredZonesB;
  [FormerlySerializedAs("gameModeName")]
  public string networkZone;
  public string componentTypeToAdd;
  public GameObject componentTarget;
  public GorillaFriendCollider myCollider;
  public GorillaNetworkJoinTrigger primaryTriggerForMyZone;
  public bool ignoredIfInParty;
  private JoinTriggerUI ui;
  private bool didRegisterForCallbacks;
  public AdditionalCustomProperty[] additionalJoinCustomProperties;
  private static bool triggerJoinsDisabled;

  public GroupJoinZoneAB groupJoinRequiredZonesAB
  {
    get
    {
      return new GroupJoinZoneAB()
      {
        a = this.groupJoinRequiredZones,
        b = this.groupJoinRequiredZonesB
      };
    }
  }

  private void Start()
  {
    if ((UnityEngine.Object) this.primaryTriggerForMyZone == (UnityEngine.Object) null)
      this.primaryTriggerForMyZone = this;
    if ((UnityEngine.Object) this.primaryTriggerForMyZone == (UnityEngine.Object) this)
      GorillaComputer.instance.RegisterPrimaryJoinTrigger(this);
    PhotonNetworkController.Instance.RegisterJoinTrigger(this);
    if (this.didRegisterForCallbacks || !((UnityEngine.Object) this.ui != (UnityEngine.Object) null))
      return;
    this.didRegisterForCallbacks = true;
    FriendshipGroupDetection.Instance.AddGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
  }

  public void RegisterUI(JoinTriggerUI ui)
  {
    this.ui = ui;
    if (!this.didRegisterForCallbacks && (UnityEngine.Object) FriendshipGroupDetection.Instance != (UnityEngine.Object) null)
    {
      this.didRegisterForCallbacks = true;
      FriendshipGroupDetection.Instance.AddGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
    }
    this.UpdateUI();
  }

  public void UnregisterUI(JoinTriggerUI ui) => this.ui = (JoinTriggerUI) null;

  private void OnDestroy()
  {
    if (!this.didRegisterForCallbacks)
      return;
    FriendshipGroupDetection.Instance.RemoveGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
  }

  private void OnGroupPositionsChanged(GroupJoinZoneAB groupZone) => this.UpdateUI();

  public void UpdateUI()
  {
    if ((UnityEngine.Object) this.ui == (UnityEngine.Object) null || (UnityEngine.Object) NetworkSystem.Instance == (UnityEngine.Object) null)
      return;
    if (GorillaScoreboardTotalUpdater.instance.offlineTextErrorString != null)
      this.ui.SetState(JoinTriggerVisualState.ConnectionError, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
    else if (NetworkSystem.Instance.SessionIsPrivate)
      this.ui.SetState(JoinTriggerVisualState.InPrivateRoom, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
    else if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString == this.GetFullDesiredGameModeString())
      this.ui.SetState(JoinTriggerVisualState.AlreadyInRoom, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
    else if (FriendshipGroupDetection.Instance.IsInParty)
      this.ui.SetState(this.CanPartyJoin() ? JoinTriggerVisualState.LeaveRoomAndPartyJoin : JoinTriggerVisualState.AbandonPartyAndSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
    else if (!NetworkSystem.Instance.InRoom)
      this.ui.SetState(JoinTriggerVisualState.NotConnectedSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
    else if ((UnityEngine.Object) PhotonNetworkController.Instance.currentJoinTrigger == (UnityEngine.Object) this.primaryTriggerForMyZone)
      this.ui.SetState(JoinTriggerVisualState.ChangingGameModeSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
    else
      this.ui.SetState(JoinTriggerVisualState.LeaveRoomAndSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
  }

  private string GetActiveNetworkZone()
  {
    return PhotonNetworkController.Instance.currentJoinTrigger.networkZone.ToUpper();
  }

  private string GetDesiredNetworkZone() => this.networkZone.ToUpper();

  public static string GetActiveGameType() => GameMode.ActiveGameMode?.GameModeName() ?? "";

  public string GetDesiredGameType()
  {
    return GameMode.GameModeZoneMapping.VerifyModeForZone(this.zone, Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true), NetworkSystem.Instance.SessionIsPrivate).ToString();
  }

  public string GetDesiredGameTypeLocalized()
  {
    return GorillaGameManager.GameModeEnumToName(GameMode.GameModeZoneMapping.VerifyModeForZone(this.zone, Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true), NetworkSystem.Instance.SessionIsPrivate));
  }

  public virtual string GetFullDesiredGameModeString()
  {
    return this.networkZone + GorillaComputer.instance.currentQueue + this.GetDesiredGameType();
  }

  public virtual byte GetRoomSize() => RoomSystem.GetRoomSizeForCreate(this.networkZone);

  public bool CanPartyJoin() => this.CanPartyJoin(FriendshipGroupDetection.Instance.partyZone);

  public bool CanPartyJoin(GroupJoinZoneAB zone) => (this.groupJoinRequiredZonesAB & zone) == zone;

  public override void OnBoxTriggered()
  {
    base.OnBoxTriggered();
    if (GorillaNetworkJoinTrigger.triggerJoinsDisabled)
    {
      Debug.Log((object) "GorillaNetworkJoinTrigger::OnBoxTriggered - blocking join call");
    }
    else
    {
      GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
      if (NetworkSystem.Instance.groupJoinInProgress)
        return;
      List<(string, string)> additionalCustomProperties = new List<(string, string)>();
      foreach (AdditionalCustomProperty joinCustomProperty in this.additionalJoinCustomProperties)
        additionalCustomProperties.Add((joinCustomProperty.key, joinCustomProperty.value));
      if (FriendshipGroupDetection.Instance.IsInParty)
      {
        if (this.ignoredIfInParty || NetworkSystem.Instance.netState == NetSystemState.Connecting || NetworkSystem.Instance.netState == NetSystemState.Disconnecting || NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon)
          return;
        if (NetworkSystem.Instance.InRoom)
        {
          if (NetworkSystem.Instance.GameModeString == this.GetFullDesiredGameModeString())
          {
            Debug.Log((object) $"JoinTrigger: Ignoring party join/leave because {this.networkZone} is already the game mode");
            return;
          }
          if (NetworkSystem.Instance.SessionIsPrivate)
          {
            Debug.Log((object) "JoinTrigger: Ignoring party join/leave because we're in a private room");
            return;
          }
        }
        if (this.CanPartyJoin())
        {
          Debug.Log((object) $"JoinTrigger: Attempting party join in 1 second! <{this.groupJoinRequiredZones}> accepts <{FriendshipGroupDetection.Instance.partyZone}>");
          PhotonNetworkController.Instance.DeferJoining(1f);
          FriendshipGroupDetection.Instance.SendAboutToGroupJoin();
          PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, JoinType.JoinWithParty, additionalCustomProperties);
          return;
        }
        Debug.Log((object) $"JoinTrigger: LeaveGroup: Leaving party and will solo join, wanted <{this.groupJoinRequiredZones}> but got <{FriendshipGroupDetection.Instance.partyZone}>");
        FriendshipGroupDetection.Instance.LeaveParty();
        PhotonNetworkController.Instance.DeferJoining(1f);
      }
      else
      {
        Debug.Log((object) "JoinTrigger: Solo join (not in a group)");
        PhotonNetworkController.Instance.ClearDeferredJoin();
      }
      PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, additionalCustomProperties: additionalCustomProperties);
    }
  }

  public static void DisableTriggerJoins()
  {
    Debug.Log((object) "[GorillaNetworkJoinTrigger::DisableTriggerJoins] Disabling Trigger-based Room Joins...");
    GorillaNetworkJoinTrigger.triggerJoinsDisabled = true;
  }

  public static void EnableTriggerJoins()
  {
    Debug.Log((object) "[GorillaNetworkJoinTrigger::EnableTriggerJoins] Enabling Trigger-based Room Joins...");
    GorillaNetworkJoinTrigger.triggerJoinsDisabled = false;
  }
}
