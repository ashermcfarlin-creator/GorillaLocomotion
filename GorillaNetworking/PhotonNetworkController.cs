// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.PhotonNetworkController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Fusion;
using GorillaLocomotion;
using GorillaTag;
using GorillaTagScripts;
using Photon.Pun;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#nullable disable
namespace GorillaNetworking;

public class PhotonNetworkController : MonoBehaviour
{
  [OnEnterPlay_SetNull]
  public static volatile PhotonNetworkController Instance;
  public int incrementCounter;
  public PlayFabAuthenticator playFabAuthenticator;
  public string[] serverRegions;
  public bool isPrivate;
  public string customRoomID;
  public GameObject playerOffset;
  public SkinnedMeshRenderer[] offlineVRRig;
  public bool attemptingToConnect;
  private int currentRegionIndex;
  public string currentGameType;
  public bool roomCosmeticsInitialized;
  public GameObject photonVoiceObjectPrefab;
  public Dictionary<string, bool> playerCosmeticsLookup = new Dictionary<string, bool>();
  private float lastHeadRightHandDistance;
  private float lastHeadLeftHandDistance;
  private float pauseTime;
  private float disconnectTime = 120f;
  public bool disableAFKKick;
  private float headRightHandDistance;
  private float headLeftHandDistance;
  private Quaternion headQuat;
  private Quaternion lastHeadQuat;
  public GameObject[] disableOnStartup;
  public GameObject[] enableOnStartup;
  public bool updatedName;
  private int[] playersInRegion;
  private int[] pingInRegion;
  private List<string> friendIDList = new List<string>();
  private JoinType currentJoinType;
  private string friendToFollow;
  private string keyToFollow;
  public string shuffler;
  public string keyStr;
  private string platformTag = "OTHER";
  private string startLevel;
  [SerializeField]
  private GTZone startZone;
  private GorillaGeoHideShowTrigger startGeoTrigger;
  public GorillaNetworkJoinTrigger privateTrigger;
  internal string initialGameMode = "";
  public GorillaNetworkJoinTrigger currentJoinTrigger;
  public string autoJoinRoom;
  public string autoJoinGameMode;
  private bool deferredJoin;
  private float partyJoinDeferredUntilTimestamp;
  private DateTime? timeWhenApplicationPaused;
  [NetworkPrefab]
  [SerializeField]
  private NetworkObject testPlayerPrefab;
  private List<GorillaNetworkJoinTrigger> allJoinTriggers = new List<GorillaNetworkJoinTrigger>();

  public List<string> FriendIDList
  {
    get => this.friendIDList;
    set => this.friendIDList = value;
  }

  public string StartLevel
  {
    get => this.startLevel;
    set => this.startLevel = value;
  }

  public GTZone StartZone
  {
    get => this.startZone;
    set => this.startZone = value;
  }

  public GTZone CurrentRoomZone
  {
    get
    {
      return !((UnityEngine.Object) this.currentJoinTrigger != (UnityEngine.Object) null) ? GTZone.none : this.currentJoinTrigger.zone;
    }
  }

  public GorillaGeoHideShowTrigger StartGeoTrigger
  {
    get => this.startGeoTrigger;
    set => this.startGeoTrigger = value;
  }

  public void Awake()
  {
    if ((UnityEngine.Object) PhotonNetworkController.Instance == (UnityEngine.Object) null)
      PhotonNetworkController.Instance = this;
    else if ((UnityEngine.Object) PhotonNetworkController.Instance != (UnityEngine.Object) this)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    this.updatedName = false;
    this.playersInRegion = new int[this.serverRegions.Length];
    this.pingInRegion = new int[this.serverRegions.Length];
  }

  public void Start()
  {
    this.StartCoroutine(this.DisableOnStart());
    NetworkSystem instance1 = NetworkSystem.Instance;
    instance1.OnJoinedRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>) instance1.OnJoinedRoomEvent + new Action(this.OnJoinedRoom);
    NetworkSystem instance2 = NetworkSystem.Instance;
    instance2.OnReturnedToSinglePlayer = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>) instance2.OnReturnedToSinglePlayer + new Action(this.OnDisconnected);
    PhotonNetwork.NetworkingClient.LoadBalancingPeer.ReuseEventInstance = true;
  }

  private IEnumerator DisableOnStart()
  {
    ZoneManagement.SetActiveZone(this.StartZone);
    yield break;
  }

  public void FixedUpdate()
  {
    this.headRightHandDistance = (GTPlayer.Instance.headCollider.transform.position - GTPlayer.Instance.GetControllerTransform(false).position).magnitude;
    this.headLeftHandDistance = (GTPlayer.Instance.headCollider.transform.position - GTPlayer.Instance.GetControllerTransform(true).position).magnitude;
    this.headQuat = GTPlayer.Instance.headCollider.transform.rotation;
    if (!this.disableAFKKick && (double) Quaternion.Angle(this.headQuat, this.lastHeadQuat) <= 0.0099999997764825821 && (double) Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) < 1.0 / 1000.0 && (double) Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) < 1.0 / 1000.0 && (double) this.pauseTime + (double) this.disconnectTime < (double) Time.realtimeSinceStartup)
    {
      this.pauseTime = Time.realtimeSinceStartup;
      NetworkSystem.Instance.ReturnToSinglePlayer();
    }
    else if ((double) Quaternion.Angle(this.headQuat, this.lastHeadQuat) > 0.0099999997764825821 || (double) Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) >= 1.0 / 1000.0 || (double) Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) >= 1.0 / 1000.0)
      this.pauseTime = Time.realtimeSinceStartup;
    this.lastHeadRightHandDistance = this.headRightHandDistance;
    this.lastHeadLeftHandDistance = this.headLeftHandDistance;
    this.lastHeadQuat = this.headQuat;
    if (!this.deferredJoin || (double) Time.time < (double) this.partyJoinDeferredUntilTimestamp)
      return;
    if (((double) this.partyJoinDeferredUntilTimestamp != 0.0 || NetworkSystem.Instance.netState == NetSystemState.Idle) && (UnityEngine.Object) this.currentJoinTrigger != (UnityEngine.Object) null)
    {
      this.deferredJoin = false;
      this.partyJoinDeferredUntilTimestamp = 0.0f;
      if ((UnityEngine.Object) this.currentJoinTrigger == (UnityEngine.Object) this.privateTrigger)
        this.AttemptToJoinSpecificRoom(this.customRoomID, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
      else
        this.AttemptToJoinPublicRoom(this.currentJoinTrigger, this.currentJoinType);
    }
    else
    {
      if (NetworkSystem.Instance.netState == NetSystemState.PingRecon || NetworkSystem.Instance.netState == NetSystemState.Initialization)
        return;
      this.deferredJoin = false;
      this.partyJoinDeferredUntilTimestamp = 0.0f;
    }
  }

  public void DeferJoining(float duration)
  {
    this.partyJoinDeferredUntilTimestamp = Mathf.Max(this.partyJoinDeferredUntilTimestamp, Time.time + duration);
  }

  public void ClearDeferredJoin()
  {
    this.partyJoinDeferredUntilTimestamp = 0.0f;
    this.deferredJoin = false;
  }

  public void AttemptToJoinPublicRoom(
    GorillaNetworkJoinTrigger triggeredTrigger,
    JoinType roomJoinType = JoinType.Solo,
    List<(string, string)> additionalCustomProperties = null)
  {
    this.AttemptToJoinPublicRoomAsync(triggeredTrigger, roomJoinType, additionalCustomProperties);
  }

  private async void AttemptToJoinPublicRoomAsync(
    GorillaNetworkJoinTrigger triggeredTrigger,
    JoinType roomJoinType,
    List<(string, string)> additionalCustomProperties)
  {
    PhotonNetworkController networkController = this;
    string desiredGameMode;
    if (((!KIDManager.KidEnabledAndReady ? 1 : (KIDManager.CheckFeatureOptIn(EKIDFeatures.Multiplayer).hasOptedInPreviously ? 1 : 0)) == 0 ? 0 : (networkController.enabled ? 1 : 0)) == 0)
      desiredGameMode = (string) null;
    else if (NetworkSystem.Instance.netState == NetSystemState.Connecting)
      desiredGameMode = (string) null;
    else if (NetworkSystem.Instance.netState == NetSystemState.Disconnecting)
      desiredGameMode = (string) null;
    else if (NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon || (double) Time.time < (double) networkController.partyJoinDeferredUntilTimestamp)
    {
      networkController.currentJoinTrigger = triggeredTrigger;
      networkController.currentJoinType = roomJoinType;
      networkController.deferredJoin = true;
      desiredGameMode = (string) null;
    }
    else
    {
      networkController.deferredJoin = false;
      desiredGameMode = triggeredTrigger.GetFullDesiredGameModeString();
      if (NetworkSystem.Instance.InRoom)
      {
        if (NetworkSystem.Instance.SessionIsPrivate)
        {
          if (roomJoinType != JoinType.JoinWithNearby && roomJoinType != JoinType.ForceJoinWithParty)
          {
            desiredGameMode = (string) null;
            return;
          }
        }
        else if (NetworkSystem.Instance.GameModeString.StartsWith(desiredGameMode))
        {
          desiredGameMode = (string) null;
          return;
        }
      }
      if (roomJoinType == JoinType.JoinWithParty || roomJoinType == JoinType.ForceJoinWithParty)
        await networkController.SendPartyFollowCommands();
      networkController.currentJoinTrigger = triggeredTrigger;
      networkController.currentJoinType = roomJoinType;
      if (PlayFabClientAPI.IsClientLoggedIn())
        networkController.playFabAuthenticator.SetDisplayName(NetworkSystem.Instance.GetMyNickName());
      RoomConfig opts = RoomConfig.AnyPublicConfig();
      if (networkController.currentJoinType == JoinType.JoinWithNearby || networkController.currentJoinType == JoinType.JoinWithElevator)
        opts.SetFriendIDs(networkController.FriendIDList);
      else if (networkController.currentJoinType == JoinType.JoinWithParty || networkController.currentJoinType == JoinType.ForceJoinWithParty)
        opts.SetFriendIDs(FriendshipGroupDetection.Instance.PartyMemberIDs.ToList<string>());
      ExitGames.Client.Photon.Hashtable hashtable1 = new ExitGames.Client.Photon.Hashtable();
      hashtable1.Add((object) "gameMode", (object) desiredGameMode);
      hashtable1.Add((object) "platform", (object) networkController.platformTag);
      hashtable1.Add((object) "queueName", (object) GorillaComputer.instance.currentQueue);
      hashtable1.Add((object) "language", (object) LocalisationManager.CurrentLanguage.ToString());
      ExitGames.Client.Photon.Hashtable hashtable2 = hashtable1;
      if (additionalCustomProperties != null)
      {
        foreach ((string, string) additionalCustomProperty in additionalCustomProperties)
          hashtable2.Add((object) additionalCustomProperty.Item1, (object) additionalCustomProperty.Item2);
      }
      opts.CustomProps = hashtable2;
      opts.MaxPlayers = networkController.currentJoinTrigger.GetRoomSize();
      Debug.Log((object) $"AttemptToJoinPublicRoom: MaxPlayers: {opts.MaxPlayers}");
      int room = (int) await NetworkSystem.Instance.ConnectToRoom((string) null, opts);
      desiredGameMode = (string) null;
    }
  }

  public void AttemptToJoinRankedPublicRoom(
    GorillaNetworkJoinTrigger triggeredTrigger,
    JoinType roomJoinType = JoinType.Solo)
  {
    string mmrTier = RankedProgressionManager.Instance.GetRankedMatchmakingTier().ToString();
    string platform = "PC";
    this.AttemptToJoinRankedPublicRoomAsync(triggeredTrigger, mmrTier, platform, roomJoinType);
  }

  private async void AttemptToJoinRankedPublicRoomAsync(
    GorillaNetworkJoinTrigger triggeredTrigger,
    string mmrTier,
    string platform,
    JoinType roomJoinType)
  {
    PhotonNetworkController networkController = this;
    if (((!KIDManager.KidEnabledAndReady ? 1 : (KIDManager.CheckFeatureOptIn(EKIDFeatures.Multiplayer).hasOptedInPreviously ? 1 : 0)) == 0 ? 0 : (networkController.enabled ? 1 : 0)) == 0 || NetworkSystem.Instance.netState == NetSystemState.Connecting || NetworkSystem.Instance.netState == NetSystemState.Disconnecting)
      return;
    if (NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon || (double) Time.time < (double) networkController.partyJoinDeferredUntilTimestamp)
    {
      networkController.currentJoinTrigger = triggeredTrigger;
      networkController.currentJoinType = roomJoinType;
      networkController.deferredJoin = true;
    }
    else
    {
      networkController.deferredJoin = false;
      string desiredGameModeString = triggeredTrigger.GetFullDesiredGameModeString();
      if (NetworkSystem.Instance.InRoom)
        return;
      networkController.currentJoinTrigger = triggeredTrigger;
      networkController.currentJoinType = roomJoinType;
      if (PlayFabClientAPI.IsClientLoggedIn())
        networkController.playFabAuthenticator.SetDisplayName(NetworkSystem.Instance.GetMyNickName());
      RoomConfig opts = RoomConfig.AnyPublicConfig();
      ExitGames.Client.Photon.Hashtable hashtable1 = new ExitGames.Client.Photon.Hashtable();
      hashtable1.Add((object) "gameMode", (object) desiredGameModeString);
      hashtable1.Add((object) nameof (mmrTier), (object) mmrTier);
      hashtable1.Add((object) nameof (platform), (object) platform);
      ExitGames.Client.Photon.Hashtable hashtable2 = hashtable1;
      opts.CustomProps = hashtable2;
      opts.MaxPlayers = networkController.currentJoinTrigger.GetRoomSize();
      int room = (int) await NetworkSystem.Instance.ConnectToRoom((string) null, opts);
    }
  }

  private async Task SendPartyFollowCommands()
  {
    PhotonNetworkController instance = PhotonNetworkController.Instance;
    int num = UnityEngine.Random.Range(0, 99);
    string str1 = num.ToString().PadLeft(2, '0');
    num = UnityEngine.Random.Range(0, 99999999);
    string str2 = num.ToString().PadLeft(8, '0');
    string str3 = str1 + str2;
    instance.shuffler = str3;
    PhotonNetworkController.Instance.keyStr = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
    RoomSystem.SendPartyFollowCommand(PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
    PhotonNetwork.SendAllOutgoingCommands();
    await Task.Delay(200);
  }

  public void AttemptToJoinSpecificRoom(string roomID, JoinType roomJoinType)
  {
    this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, (Action<NetJoinResult>) null);
  }

  public void AttemptToJoinSpecificRoomWithCallback(
    string roomID,
    JoinType roomJoinType,
    Action<NetJoinResult> callback)
  {
    this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, callback);
  }

  public async Task AttemptToJoinSpecificRoomAsync(
    string roomID,
    JoinType roomJoinType,
    Action<NetJoinResult> callback)
  {
    Task<NetJoinResult> connectToRoomTask;
    if (await KIDManager.UseKID() && !KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer))
      connectToRoomTask = (Task<NetJoinResult>) null;
    else if (NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon)
    {
      this.deferredJoin = true;
      this.customRoomID = roomID;
      this.currentJoinType = roomJoinType;
      this.currentJoinTrigger = this.privateTrigger;
      connectToRoomTask = (Task<NetJoinResult>) null;
    }
    else if (NetworkSystem.Instance.netState != NetSystemState.Idle && NetworkSystem.Instance.netState != NetSystemState.InGame)
    {
      connectToRoomTask = (Task<NetJoinResult>) null;
    }
    else
    {
      this.customRoomID = roomID;
      this.currentJoinType = roomJoinType;
      this.currentJoinTrigger = this.privateTrigger;
      this.deferredJoin = false;
      if (this.currentJoinType == JoinType.JoinWithParty || this.currentJoinType == JoinType.ForceJoinWithParty)
        await this.SendPartyFollowCommands();
      string desiredGameModeString = this.currentJoinTrigger.GetFullDesiredGameModeString();
      ExitGames.Client.Photon.Hashtable hashtable1 = new ExitGames.Client.Photon.Hashtable();
      hashtable1.Add((object) "gameMode", (object) desiredGameModeString);
      hashtable1.Add((object) "platform", (object) this.platformTag);
      hashtable1.Add((object) "queueName", (object) GorillaComputer.instance.currentQueue);
      ExitGames.Client.Photon.Hashtable hashtable2 = hashtable1;
      RoomConfig opts = new RoomConfig();
      opts.createIfMissing = true;
      opts.isJoinable = true;
      opts.isPublic = false;
      opts.MaxPlayers = RoomSystem.GetRoomSizeForCreate(this.currentJoinTrigger.networkZone);
      Debug.Log((object) $"[AttemptToJoinSpecificRoomAsync] Room MaxPlayers = {opts.MaxPlayers}");
      opts.CustomProps = hashtable2;
      if (roomJoinType == JoinType.FriendStationPublic)
        opts.isPublic = true;
      if (PlayFabClientAPI.IsClientLoggedIn())
        this.playFabAuthenticator.SetDisplayName(NetworkSystem.Instance.GetMyNickName());
      connectToRoomTask = NetworkSystem.Instance.ConnectToRoom(roomID, opts);
      if (callback == null)
      {
        connectToRoomTask = (Task<NetJoinResult>) null;
      }
      else
      {
        int num = (int) await connectToRoomTask;
        Debug.Log((object) ("AttemptToJoinSpecificRoomAsync ConnectToRoom Result: " + connectToRoomTask.Result.ToString()));
        callback(connectToRoomTask.Result);
        connectToRoomTask = (Task<NetJoinResult>) null;
      }
    }
  }

  private void DisconnectCleanup()
  {
    if (ApplicationQuittingState.IsQuitting)
      return;
    if ((UnityEngine.Object) GorillaParent.instance != (UnityEngine.Object) null)
    {
      foreach (GorillaScoreboardSpawner componentsInChild in GorillaParent.instance.GetComponentsInChildren<GorillaScoreboardSpawner>())
        componentsInChild.OnLeftRoom();
    }
    this.attemptingToConnect = true;
    foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.offlineVRRig)
    {
      if ((UnityEngine.Object) skinnedMeshRenderer != (UnityEngine.Object) null)
        skinnedMeshRenderer.enabled = true;
    }
    if ((UnityEngine.Object) GorillaComputer.instance != (UnityEngine.Object) null && !ApplicationQuittingState.IsQuitting)
      this.UpdateTriggerScreens();
    GTPlayer.Instance.maxJumpSpeed = 6.5f;
    GTPlayer.Instance.jumpMultiplier = 1.1f;
    GorillaNot.instance.currentMasterClient = (NetPlayer) null;
    GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(false);
    this.initialGameMode = "";
  }

  public void OnJoinedRoom()
  {
    if (NetworkSystem.Instance.GameModeString.IsNullOrEmpty())
      NetworkSystem.Instance.ReturnToSinglePlayer();
    this.initialGameMode = NetworkSystem.Instance.GameModeString;
    if (NetworkSystem.Instance.SessionIsPrivate)
    {
      this.currentJoinTrigger = this.privateTrigger;
      PhotonNetworkController.Instance.UpdateTriggerScreens();
    }
    else if (this.currentJoinType != JoinType.FollowingParty)
    {
      bool flag = false;
      for (int index = 0; index < GorillaComputer.instance.allowedMapsToJoin.Length; ++index)
      {
        if (NetworkSystem.Instance.GameModeString.StartsWith(GorillaComputer.instance.allowedMapsToJoin[index]))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        GorillaComputer.instance.roomNotAllowed = true;
        NetworkSystem.Instance.ReturnToSinglePlayer();
        return;
      }
    }
    NetworkSystem.Instance.SetMyTutorialComplete();
    VRRigCache.Instance.InstantiateNetworkObject();
    if (NetworkSystem.Instance.IsMasterClient)
      GorillaGameModes.GameMode.LoadGameModeFromProperty(this.initialGameMode);
    GorillaComputer.instance.roomFull = false;
    GorillaComputer.instance.roomNotAllowed = false;
    if (this.currentJoinType == JoinType.JoinWithParty || this.currentJoinType == JoinType.JoinWithNearby || this.currentJoinType == JoinType.ForceJoinWithParty || this.currentJoinType == JoinType.JoinWithElevator)
    {
      this.keyToFollow = NetworkSystem.Instance.LocalPlayer.UserId + this.keyStr;
      NetworkSystem.Instance.BroadcastMyRoom(true, this.keyToFollow, this.shuffler);
    }
    GorillaNot.instance.currentMasterClient = (NetPlayer) null;
    this.UpdateCurrentJoinTrigger();
    this.UpdateTriggerScreens();
    NetworkSystem.Instance.MultiplayerStarted();
  }

  public void RegisterJoinTrigger(GorillaNetworkJoinTrigger trigger)
  {
    this.allJoinTriggers.Add(trigger);
  }

  private void UpdateCurrentJoinTrigger()
  {
    GorillaNetworkJoinTrigger fullGameModeString = GorillaComputer.instance.GetJoinTriggerFromFullGameModeString(NetworkSystem.Instance.GameModeString);
    if ((UnityEngine.Object) fullGameModeString != (UnityEngine.Object) null)
      this.currentJoinTrigger = fullGameModeString;
    else if (NetworkSystem.Instance.SessionIsPrivate)
    {
      if (!((UnityEngine.Object) this.currentJoinTrigger != (UnityEngine.Object) this.privateTrigger))
        return;
      Debug.LogError((object) "IN a private game but private trigger isnt current");
    }
    else
      Debug.LogError((object) "Not in private room and unabel tp update jointrigger.");
  }

  public void UpdateTriggerScreens()
  {
    foreach (GorillaNetworkJoinTrigger allJoinTrigger in this.allJoinTriggers)
      allJoinTrigger.UpdateUI();
  }

  public void AttemptToFollowIntoPub(
    string userIDToFollow,
    int actorNumberToFollow,
    string newKeyStr,
    string shufflerStr,
    JoinType joinType)
  {
    this.friendToFollow = userIDToFollow;
    this.keyToFollow = userIDToFollow + newKeyStr;
    this.shuffler = shufflerStr;
    this.currentJoinType = joinType;
    this.ClearDeferredJoin();
    if (!NetworkSystem.Instance.InRoom)
      return;
    NetworkSystem.Instance.JoinFriendsRoom(this.friendToFollow, actorNumberToFollow, this.keyToFollow, this.shuffler);
  }

  public void OnDisconnected() => this.DisconnectCleanup();

  public void OnApplicationQuit()
  {
    if (!PhotonNetwork.IsConnected)
      return;
    int num = PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion != "dev" ? 1 : 0;
  }

  private string ReturnRoomName() => this.isPrivate ? this.customRoomID : this.RandomRoomName();

  private string RandomRoomName()
  {
    string nameToCheck = "";
    for (int index = 0; index < 4; ++index)
      nameToCheck += "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(UnityEngine.Random.Range(0, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Length), 1);
    return GorillaComputer.instance.CheckAutoBanListForName(nameToCheck) ? nameToCheck : this.RandomRoomName();
  }

  private string GetRegionWithLowestPing()
  {
    int num = 10000;
    int index1 = 0;
    for (int index2 = 0; index2 < this.serverRegions.Length; ++index2)
    {
      Debug.Log((object) $"ping in region {this.serverRegions[index2]} is {this.pingInRegion[index2].ToString()}");
      if (this.pingInRegion[index2] < num && this.pingInRegion[index2] > 0)
      {
        num = this.pingInRegion[index2];
        index1 = index2;
      }
    }
    return this.serverRegions[index1];
  }

  public int TotalUsers()
  {
    int num1 = 0;
    foreach (int num2 in this.playersInRegion)
      num1 += num2;
    return num1;
  }

  public string CurrentState()
  {
    if ((UnityEngine.Object) NetworkSystem.Instance == (UnityEngine.Object) null)
      Debug.Log((object) "Null netsys!!!");
    return NetworkSystem.Instance.netState.ToString();
  }

  private void OnApplicationPause(bool pause)
  {
    if (pause)
    {
      this.timeWhenApplicationPaused = new DateTime?(DateTime.Now);
    }
    else
    {
      if ((DateTime.Now - (this.timeWhenApplicationPaused ?? DateTime.Now)).TotalSeconds > (double) this.disconnectTime)
      {
        this.timeWhenApplicationPaused = new DateTime?();
        NetworkSystem.Instance?.ReturnToSinglePlayer();
      }
      if (!((UnityEngine.Object) NetworkSystem.Instance != (UnityEngine.Object) null) || NetworkSystem.Instance.InRoom || NetworkSystem.Instance.netState != NetSystemState.InGame)
        return;
      NetworkSystem.Instance?.ReturnToSinglePlayer();
    }
  }

  private void OnApplicationFocus(bool focus)
  {
    if (focus || !((UnityEngine.Object) NetworkSystem.Instance != (UnityEngine.Object) null) || NetworkSystem.Instance.InRoom || NetworkSystem.Instance.netState != NetSystemState.InGame)
      return;
    NetworkSystem.Instance?.ReturnToSinglePlayer();
  }
}
