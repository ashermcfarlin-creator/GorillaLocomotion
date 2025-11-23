// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.GorillaComputer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaGameModes;
using GorillaTag;
using GorillaTagScripts;
using GorillaTagScripts.VirtualStumpCustomMaps;
using KID.Model;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

#nullable disable
namespace GorillaNetworking;

public class GorillaComputer : MonoBehaviour, IMatchmakingCallbacks, IGorillaSliceableSimple
{
  private const string VERSION_MISMATCH_KEY = "VERSION_MISMATCH";
  private const string CONNECTION_ISSUE_KEY = "CONNECTION_ISSUE";
  private const string NO_CONNECTION_KEY = "NO_CONNECTION";
  private const string STARTUP_INTRO_KEY = "STARTUP_INTRO";
  private const string STARTUP_PLAYERS_ONLINE_KEY = "STARTUP_PLAYERS_ONLINE";
  private const string STARTUP_USERS_BANNED_KEY = "STARTUP_USERS_BANNED";
  private const string STARTUP_PRESS_KEY_KEY = "STARTUP_PRESS_KEY";
  private const string STARTUP_PRESS_KEY_SHORT_KEY = "STARTUP_PRESS_KEY_SHORT";
  private const string STARTUP_MANAGED_KEY = "STARTUP_MANAGED";
  private const string COLOR_SELECT_INTRO_KEY = "COLOR_SELECT_INTRO";
  private const string CURRENT_SELECTED_LANGUAGE_KEY = "CURRENT_SELECTED_LANGUAGE";
  private const string CHANGE_TO_KEY = "CHANGE_TO";
  private const string CONFIRM_LANGUAGE_KEY = "CONFIRM_LANGUAGE";
  private const string COLOR_RED_KEY = "COLOR_RED";
  private const string COLOR_GREEN_KEY = "COLOR_GREEN";
  private const string COLOR_BLUE_KEY = "COLOR_BLUE";
  private const string ROOM_INTRO_KEY = "ROOM_INTRO";
  private const string ROOM_OPTION_KEY = "ROOM_OPTION";
  private const string ROOM_TEXT_CURRENT_ROOM_KEY = "ROOM_TEXT_CURRENT_ROOM";
  private const string PLAYERS_IN_ROOM_KEY = "PLAYERS_IN_ROOM";
  private const string NOT_IN_ROOM_KEY = "NOT_IN_ROOM";
  private const string PLAYERS_ONLINE_KEY = "PLAYERS_ONLINE";
  private const string ROOM_TO_JOIN_KEY = "ROOM_TO_JOIN";
  private const string ROOM_FULL_KEY = "ROOM_FULL";
  private const string ROOM_JOIN_NOT_ALLOWED_KEY = "ROOM_JOIN_NOT_ALLOWED";
  private const string LANGUAGE_KEY = "LANGUAGE";
  private const string NAME_SCREEN_KEY = "NAME_SCREEN";
  private const string CURRENT_NAME_KEY = "CURRENT_NAME";
  private const string NEW_NAME_KEY = "NEW_NAME";
  private const string TURN_SCREEN_KEY = "TURN_SCREEN";
  private const string TURN_SCREEN_TURNING_SPEED_KEY = "TURN_SCREEN_TURNING_SPEED";
  private const string TURN_SCREEN_TURN_TYPE_KEY = "TURN_SCREEN_TURN_TYPE";
  private const string TURN_SCREEN_TURN_SPEED_KEY = "TURN_SCREEN_TURN_SPEED";
  private const string TURN_TYPE_SNAP_TURN_KEY = "TURN_TYPE_SNAP_TURN";
  private const string TURN_TYPE_SMOOTH_TURN_KEY = "TURN_TYPE_SMOOTH_TURN";
  private const string TURN_TYPE_NO_TURN_KEY = "TURN_TYPE_NO_TURN";
  private const string QUEUE_SCREEN_KEY = "QUEUE_SCREEN";
  private const string BEAT_OBSTACLE_COURSE_KEY = "BEAT_OBSTACLE_COURSE";
  private const string COMPETITIVE_DESC_KEY = "COMPETITIVE_DESC";
  private const string QUEUE_SCREEN_ALL_QUEUES_KEY = "QUEUE_SCREEN_ALL_QUEUES";
  private const string QUEUE_SCREEN_DEFAULT_QUEUES_KEY = "QUEUE_SCREEN_DEFAULT_QUEUES";
  private const string CURRENT_QUEUE_KEY = "CURRENT_QUEUE";
  private const string DEFAULT_QUEUE_KEY = "DEFAULT_QUEUE";
  private const string MINIGAMES_QUEUE_KEY = "MINIGAMES_QUEUE";
  private const string COMPETITIVE_QUEUE_KEY = "COMPETITIVE_QUEUE";
  private const string MIC_SCREEN_INTRO_KEY = "MIC_SCREEN_INTRO";
  private const string MIC_SCREEN_OPTIONS_KEY = "MIC_SCREEN_OPTIONS";
  private const string MIC_SCREEN_CURRENT_KEY = "MIC_SCREEN_CURRENT";
  private const string MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP_KEY = "MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP";
  private const string MIC_SCREEN_MIC_DISABLED_KEY = "MIC_SCREEN_MIC_DISABLED";
  private const string MIC_SCREEN_NO_MIC_KEY = "MIC_SCREEN_NO_MIC";
  private const string MIC_SCREEN_NO_PERMISSIONS_KEY = "MIC_SCREEN_NO_PERMISSIONS";
  private const string MIC_SCREEN_PUSH_TO_TALK_TOOLTIP_KEY = "MIC_SCREEN_PUSH_TO_TALK_TOOLTIP";
  private const string MIC_SCREEN_INPUT_TEST_LABEL_KEY = "MIC_SCREEN_INPUT_TEST_LABEL";
  private const string MIC_SCREEN_INPUT_TEST_NO_MIC_KEY = "MIC_SCREEN_INPUT_TEST_NO_MIC";
  private const string ALL_CHAT_MIC_KEY = "ALL_CHAT_MIC";
  private const string PUSH_TO_TALK_MIC_KEY = "PUSH_TO_TALK_MIC";
  private const string PUSH_TO_MUTE_MIC_KEY = "PUSH_TO_MUTE_MIC";
  private const string OPEN_MIC_KEY = "OPEN_MIC";
  private const string AUTOMOD_SCREEN_INTRO_KEY = "AUTOMOD_SCREEN_INTRO";
  private const string AUTOMOD_SCREEN_OPTIONS_KEY = "AUTOMOD_SCREEN_OPTIONS";
  private const string AUTOMOD_SCREEN_CURRENT_KEY = "AUTOMOD_SCREEN_CURRENT";
  private const string AUTOMOD_AGGRESSIVE_KEY = "AUTOMOD_AGGRESSIVE";
  private const string AUTOMOD_MODERATE_KEY = "AUTOMOD_MODERATE";
  private const string AUTOMOD_OFF_KEY = "AUTOMOD_OFF";
  private const string VOICE_CHAT_SCREEN_INTRO_OLD_KEY = "VOICE_CHAT_SCREEN_INTRO_OLD";
  private const string VOICE_CHAT_SCREEN_OPTIONS_OLD_KEY = "VOICE_CHAT_SCREEN_OPTIONS_OLD";
  private const string VOICE_CHAT_SCREEN_CURRENT_OLD_KEY = "VOICE_CHAT_SCREEN_CURRENT_OLD";
  private const string TRUE_KEY = "TRUE";
  private const string FALSE_KEY = "FALSE";
  private const string VOICE_CHAT_SCREEN_INTRO_KEY = "VOICE_CHAT_SCREEN_INTRO";
  private const string VOICE_CHAT_SCREEN_OPTIONS_KEY = "VOICE_CHAT_SCREEN_OPTIONS";
  private const string VOICE_CHAT_SCREEN_CURRENT_KEY = "VOICE_CHAT_SCREEN_CURRENT";
  private const string VOICE_OPTION_HUMAN_KEY = "VOICE_OPTION_HUMAN";
  private const string VOICE_OPTION_MONKE_KEY = "VOICE_OPTION_MONKE";
  private const string VOICE_OPTION_OFF_KEY = "VOICE_OPTION_OFF";
  private const string VISUALS_SCREEN_INTRO_KEY = "VISUALS_SCREEN_INTRO";
  private const string VISUALS_SCREEN_OPTIONS_KEY = "VISUALS_SCREEN_OPTIONS";
  private const string VISUALS_SCREEN_CURRENT_KEY = "VISUALS_SCREEN_CURRENT";
  private const string VISUALS_SCREEN_VOLUME_KEY = "VISUALS_SCREEN_VOLUME";
  private const string CREDITS_KEY = "CREDITS";
  private const string CREDITS_PRESS_ENTER_KEY = "CREDITS_PRESS_ENTER";
  private const string CREDITS_CONTINUED_KEY = "CREDITS_CONTINUED";
  private const string TIME_SCREEN_KEY = "TIME_SCREEN";
  private const string GROUP_SCREEN_LIMITED_OLD_KEY = "GROUP_SCREEN_LIMITED_OLD";
  private const string GROUP_SCREEN_FULL_OLD_KEY = "GROUP_SCREEN_FULL_OLD";
  private const string GROUP_SCREEN_SELECTION_OLD_KEY = "GROUP_SCREEN_SELECTION_OLD";
  private const string PLATFORM_STEAM_KEY = "PLATFORM_STEAM";
  private const string PLATFORM_QUEST_KEY = "PLATFORM_QUEST";
  private const string PLATFORM_PSVR_KEY = "PLATFORM_PSVR";
  private const string PLATFORM_PICO_KEY = "PLATFORM_PICO";
  private const string PLATFORM_OCULUS_PC_KEY = "PLATFORM_OCULUS_PC";
  private const string SUPPORT_SCREEN_INTRO_KEY = "SUPPORT_SCREEN_INTRO";
  private const string SUPPORT_SCREEN_DETAILS_PLAYERID_KEY = "SUPPORT_SCREEN_DETAILS_PLAYERID";
  private const string SUPPORT_SCREEN_DETAILS_VERSION_KEY = "SUPPORT_SCREEN_DETAILS_VERSION";
  private const string SUPPORT_SCREEN_DETAILS_PLATFORM_KEY = "SUPPORT_SCREEN_DETAILS_PLATFORM";
  private const string SUPPORT_SCREEN_DETAILS_BUILD_DATE_KEY = "SUPPORT_SCREEN_DETAILS_BUILD_DATE";
  private const string SUPPORT_SCREEN_INITIAL_KEY = "SUPPORT_SCREEN_INITIAL";
  private const string SUPPORT_SCREEN_INITIAL_WARNING_KEY = "SUPPORT_SCREEN_INITIAL_WARNING";
  private const string OCULUS_BUILD_CODE_KEY = "OCULUS_BUILD_CODE";
  private const string LOADING_SCREEN_KEY = "LOADING_SCREEN";
  private const string WARNING_SCREEN_KEY = "WARNING_SCREEN";
  private const string WARNING_SCREEN_CONFIRMATION_KEY = "WARNING_SCREEN_CONFIRMATION";
  private const string WARNING_SCREEN_TYPE_YES_KEY = "WARNING_SCREEN_TYPE_YES";
  private const string FUNCTION_ROOM_KEY = "FUNCTION_ROOM";
  private const string FUNCTION_NAME_KEY = "FUNCTION_NAME";
  private const string FUNCTION_COLOR_KEY = "FUNCTION_COLOR";
  private const string FUNCTION_TURN_KEY = "FUNCTION_TURN";
  private const string FUNCTION_MIC_KEY = "FUNCTION_MIC";
  private const string FUNCTION_QUEUE_KEY = "FUNCTION_QUEUE";
  private const string FUNCTION_GROUP_KEY = "FUNCTION_GROUP";
  private const string FUNCTION_VOICE_KEY = "FUNCTION_VOICE";
  private const string FUNCTION_AUTOMOD_KEY = "FUNCTION_AUTOMOD";
  private const string FUNCTION_ITEMS_KEY = "FUNCTION_ITEMS";
  private const string FUNCTION_CREDITS_KEY = "FUNCTION_CREDITS";
  private const string FUNCTION_LANGUAGE_KEY = "FUNCTION_LANGUAGE";
  private const string FUNCTION_SUPPORT_KEY = "FUNCTION_SUPPORT";
  private const string COMPUTER_KEYBOARD_DELETE_KEY = "COMPUTER_KEYBOARD_DELETE";
  private const string COMPUTER_KEYBOARD_ENTER_KEY = "COMPUTER_KEYBOARD_ENTER";
  private const string COMPUTER_KEYBOARD_OPTION1_KEY = "COMPUTER_KEYBOARD_OPTION1";
  private const string COMPUTER_KEYBOARD_OPTION2_KEY = "COMPUTER_KEYBOARD_OPTION2";
  private const string COMPUTER_KEYBOARD_OPTION3_KEY = "COMPUTER_KEYBOARD_OPTION3";
  private const string WARNING_SCREEN_YES_INPUT_KEY = "WARNING_SCREEN_YES_INPUT";
  private const string GROUP_SCREEN_ENTER_PARTY_KEY = "GROUP_SCREEN_ENTER_PARTY";
  private const string GROUP_SCREEN_ENTER_NOPARTY_KEY = "GROUP_SCREEN_ENTER_NOPARTY";
  private const string GROUP_SCREEN_CANNOT_JOIN_KEY = "GROUP_SCREEN_CANNOT_JOIN";
  private const string GROUP_SCREEN_ACTIVE_ZONES_KEY = "GROUP_SCREEN_ACTIVE_ZONES";
  private const string GROUP_SCREEN_DESTINATIONS_KEY = "GROUP_SCREEN_DESTINATIONS";
  private const string NAME_SCREEN_TOGGLE_NAMETAGS_KEY = "NAME_SCREEN_TOGGLE_NAMETAGS";
  private const string NAME_SCREEN_KID_PROHIBITED_VERB_KEY = "NAME_SCREEN_KID_PROHIBITED_VERB";
  private const string NAME_SCREEN_DISABLED_KEY = "NAME_SCREEN_DISABLED";
  private const string ON_KEY = "ON_KEY";
  private const string OFF_KEY = "OFF_KEY";
  private const string KID_PROHIBITED_MESSAGE_KEY = "KID_PROHIBITED_MESSAGE";
  private const string KID_PERMISSION_NEEDED_KEY = "KID_PERMISSION_NEEDED";
  private const string KID_WAITING_PERMISSION_KEY = "KID_WAITING_PERMISSION";
  private const string KID_REFRESH_PERMISSIONS_KEY = "KID_REFRESH_PERMISSIONS";
  private const string KID_CHECK_AGAIN_COOLDOWN_KEY = "KID_CHECK_AGAIN_COOLDOWN";
  private const string STARTUP_TROOP_TEXT_KEY = "STARTUP_TROOP_TEXT";
  private const string ROOM_GROUP_TRAVEL_KEY = "ROOM_GROUP_TRAVEL";
  private const string ROOM_PARTY_WARNING_KEY = "ROOM_PARTY_WARNING";
  private const string ROOM_GAME_LABEL_KEY = "ROOM_GAME_LABEL";
  private const string ROOM_SCREEN_KID_PROHIBITED_VERB_KEY = "ROOM_SCREEN_KID_PROHIBITED_VERB";
  private const string ROOM_SCREEN_DISABLED_KEY = "ROOM_SCREEN_DISABLED";
  private const string REDEMPTION_INTRO_KEY = "REDEMPTION_INTRO";
  private const string REDEMPTION_CODE_LABEL_KEY = "REDEMPTION_CODE_LABEL";
  private const string REDEMPTION_CODE_INVALID_KEY = "REDEMPTION_CODE_INVALID";
  private const string REDEMPTION_CODE_VALIDATING_KEY = "REDEMPTION_CODE_VALIDATING";
  private const string REDEMPTION_CODE_ALREADY_USED_KEY = "REDEMPTION_CODE_ALREADY_USED";
  private const string REDEMPTION_CODE_SUCCESS_KEY = "REDEMPTION_CODE_SUCCESS";
  private const string LIMITED_ONLINE_FUNC_KEY = "LIMITED_ONLINE_FUNC";
  private const string CURRENT_MODE_KEY = "CURRENT_MODE";
  private const string SUPPORT_META_ACCOUNT_TYPE_KEY = "SUPPORT_META_ACCOUNT_TYPE";
  private const string SUPPORT_FINAL_QUEST_ONE_KEY = "SUPPORT_FINAL_QUEST_ONE";
  private const string SUPPORT_KID_ACCOUNT_TYPE_KEY = "SUPPORT_KID_ACCOUNT_TYPE";
  private const string VOICE_SCREEN_KID_PROHIBITED_VERB_KEY = "VOICE_SCREEN_KID_PROHIBITED_VERB";
  private const string VOICE_SCREEN_DISABLED_KEY = "VOICE_SCREEN_DISABLED";
  private const string MIC_SCREEN_GUARDIAN_FEATURE_DESC_KEY = "VOICE_SCREEN_GUARDIAN_FEATURE_DESC";
  private const string VOICE_SCREEN_KID_CURRENT_VOICE_KEY = "VOICE_SCREEN_KID_CURRENT_VOICE";
  private const string MIC_SCREEN_PUSH_KEY_INSTRUCTIONS_KEY = "MIC_SCREEN_PUSH_KEY_INSTRUCTIONS";
  private const string TROOP_SCREEN_INTRO_KEY = "TROOP_SCREEN_INTRO";
  private const string TROOP_SCREEN_INSTRUCTIONS_KEY = "TROOP_SCREEN_INSTRUCTIONS";
  private const string TROOP_SCREEN_CURRENT_TROOP_KEY = "TROOP_SCREEN_CURRENT_TROOP";
  private const string TROOP_SCREEN_IN_QUEUE_KEY = "TROOP_SCREEN_IN_QUEUE";
  private const string TROOP_SCREEN_PLAYERS_IN_TROOP_KEY = "TROOP_SCREEN_PLAYERS_IN_TROOP";
  private const string TROOP_SCREEN_DEFAULT_QUEUE_KEY = "TROOP_SCREEN_DEFAULT_QUEUE";
  private const string TROOP_SCREEN_CURRENT_QUEUE_KEY = "TROOP_SCREEN_CURRENT_QUEUE";
  private const string TROOP_SCREEN_TROOP_QUEUE_KEY = "TROOP_SCREEN_TROOP_QUEUE";
  private const string TROOP_SCREEN_LEAVE_KEY = "TROOP_SCREEN_LEAVE";
  private const string TROOP_SCREEN_NOT_IN_TROOP_KEY = "TROOP_SCREEN_NOT_IN_TROOP";
  private const string TROOP_SCREEN_JOIN_TROOP_KEY = "TROOP_SCREEN_JOIN_TROOP";
  private const string TROOP_SCREEN_KID_PROHIBITED_VERB_KEY = "TROOP_SCREEN_KID_PROHIBITED_VERB";
  private const string TROOP_SCREEN_DISABLED_KEY = "TROOP_SCREEN_DISABLED";
  private const string TROOP_SCREEN_KID_DESC_KEY = "TROOP_SCREEN_KID_DESC";
  private const bool HIDE_SCREENS = false;
  public const string NAMETAG_PLAYER_PREF_KEY = "nameTagsOn";
  [OnEnterPlay_SetNull]
  public static volatile GorillaComputer instance;
  [OnEnterPlay_Set(false)]
  public static bool hasInstance;
  [OnEnterPlay_SetNull]
  private static Action<bool> onNametagSettingChangedAction;
  public bool tryGetTimeAgain;
  public Material unpressedMaterial;
  public Material pressedMaterial;
  public string currentTextField;
  public float buttonFadeTime;
  public string offlineTextInitialString;
  public GorillaText screenText;
  public GorillaText functionSelectText;
  public GorillaText wallScreenText;
  private Locale _lastLocaleChecked_Version;
  private Locale _lastLocaleChecked_Connect;
  private string _cachedVersionMismatch = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";
  private string _cachedUnableToConnect = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";
  public Material wrongVersionMaterial;
  public MeshRenderer wallScreenRenderer;
  public MeshRenderer computerScreenRenderer;
  public long startupMillis;
  public DateTime startupTime;
  public string lastPressedGameMode;
  public WatchableStringSO currentGameMode;
  public WatchableStringSO currentGameModeText;
  public int includeUpdatedServerSynchTest;
  public PhotonNetworkController networkController;
  public float updateCooldown = 1f;
  private float defaultUpdateCooldown;
  private float micUpdateCooldown = 0.01f;
  public float lastUpdateTime;
  private float deltaTime;
  public bool isConnectedToMaster;
  public bool internetFailure;
  public string[] _allowedMapsToJoin;
  public bool limitOnlineScreens;
  [Header("State vars")]
  public bool stateUpdated;
  public bool screenChanged;
  public bool initialized;
  public List<GorillaComputer.StateOrderItem> OrderList = new List<GorillaComputer.StateOrderItem>()
  {
    new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Room),
    new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Name),
    new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Language, "Lang"),
    new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Turn),
    new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Mic),
    new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Queue),
    new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Troop),
    new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Group),
    new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Voice),
    new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.AutoMute, "Automod"),
    new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Visuals, "Items"),
    new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Credits),
    new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Support)
  };
  public string Pointer = "<-";
  public int highestCharacterCount;
  public List<string> FunctionNames = new List<string>();
  public int FunctionsCount;
  [Header("Room vars")]
  public string roomToJoin;
  public bool roomFull;
  public bool roomNotAllowed;
  [Header("Mic vars")]
  public string pttType;
  private GorillaSpeakerLoudness speakerLoudness;
  private float micInputTestTimer;
  public float micInputTestTimerThreshold = 10f;
  [Header("Automute vars")]
  public string autoMuteType;
  [Header("Queue vars")]
  public string currentQueue;
  public bool allowedInCompetitive;
  [Header("Group Vars")]
  public string groupMapJoin;
  public int groupMapJoinIndex;
  public GorillaFriendCollider friendJoinCollider;
  [Header("Troop vars")]
  public string troopName;
  public bool troopQueueActive;
  public string troopToJoin;
  private bool rememberTroopQueueState;
  [Header("Join Triggers")]
  public Dictionary<string, GorillaNetworkJoinTrigger> primaryTriggersByZone = new Dictionary<string, GorillaNetworkJoinTrigger>();
  public string voiceChatOn;
  [Header("Mode select vars")]
  public ModeSelectButton[] modeSelectButtons;
  public string version;
  public string buildDate;
  public string buildCode;
  [Header("Cosmetics")]
  public bool disableParticles;
  public float instrumentVolume;
  [Header("Credits")]
  public CreditsView creditsView;
  [Header("Handedness")]
  public bool leftHanded;
  [Header("Name state vars")]
  public string savedName;
  public string currentName;
  public TextAsset exactOneWeekFile;
  public TextAsset anywhereOneWeekFile;
  public TextAsset anywhereTwoWeekFile;
  private List<GorillaComputer.ComputerState> _filteredStates = new List<GorillaComputer.ComputerState>();
  private List<GorillaComputer.StateOrderItem> _activeOrderList = new List<GorillaComputer.StateOrderItem>();
  private Stack<GorillaComputer.ComputerState> stateStack = new Stack<GorillaComputer.ComputerState>();
  private GorillaComputer.ComputerState currentComputerState;
  private GorillaComputer.ComputerState previousComputerState;
  private int currentStateIndex;
  private int usersBanned;
  private float redValue;
  private string redText;
  private float blueValue;
  private string blueText;
  private float greenValue;
  private string greenText;
  private int colorCursorLine;
  private string warningConfirmationInputString = string.Empty;
  private bool displaySupport;
  private string[] exactOneWeek;
  private string[] anywhereOneWeek;
  private string[] anywhereTwoWeek;
  private GorillaComputer.RedemptionResult redemptionResult;
  private string redemptionCode = "";
  private bool playerInVirtualStump;
  private string virtualStumpRoomPrepend = "";
  private WaitForSeconds waitOneSecond = new WaitForSeconds(1f);
  private Coroutine LoadingRoutine;
  private List<string> topTroops = new List<string>();
  private bool hasRequestedInitialTroopPopulation;
  private int currentTroopPopulation = -1;
  private float lastCheckedWifi;
  private float checkIfDisconnectedSeconds = 10f;
  private float checkIfConnectedSeconds = 1f;
  private bool didInitializeGameMode;
  private float troopPopulationCheckCooldown = 3f;
  private float nextPopulationCheckTime;
  public Action OnServerTimeUpdated;
  private const string ENABLED_COLOUR = "#85ffa5";
  private const string DISABLED_COLOUR = "\"RED\"";
  private const string FAMILY_PORTAL_URL = "k-id.com/code";
  private float _updateAttemptCooldown = 15f;
  private float _nextUpdateAttemptTime;
  private bool _waitingForUpdatedSession;
  private GorillaComputer.EKidScreenState _currentScreentState = GorillaComputer.EKidScreenState.Show_OTP;
  private string[] _interestedPermissionNames = new string[3]
  {
    "custom-username",
    "voice-chat",
    "join-groups"
  };
  private const string LANG_SCREEN_TITLE_KEY = "LANG_SCREEN_TITLE";
  private const string LANG_SCREEN_INSTRUCTIONS_KEY = "LANG_SCREEN_INSTRUCTIONS";
  private const string LANG_SCREEN_CURRENT_LANGUAGE_KEY = "LANG_SCREEN_CURRENT_LANGUAGE";
  private StringBuilder _languagesDisplaySB = new StringBuilder();
  private Locale _previousLocalisationSetting;

  public string versionMismatch
  {
    get
    {
      if ((UnityEngine.Object) this._lastLocaleChecked_Version != (UnityEngine.Object) null && (UnityEngine.Object) this._lastLocaleChecked_Version == (UnityEngine.Object) LocalisationManager.CurrentLanguage && !string.IsNullOrEmpty(this._cachedVersionMismatch))
        return this._cachedVersionMismatch;
      string defaultResult = "PLEASE UPDATE TO THE LATEST VERSION OF GORILLA TAG. YOU'RE ON AN OLD VERSION. FEEL FREE TO RUN AROUND, BUT YOU WON'T BE ABLE TO PLAY WITH ANYONE ELSE.";
      string result;
      LocalisationManager.TryGetKeyForCurrentLocale("VERSION_MISMATCH", out result, defaultResult);
      this._lastLocaleChecked_Version = LocalisationManager.CurrentLanguage;
      this._cachedVersionMismatch = result;
      return this._cachedVersionMismatch;
    }
  }

  public string unableToConnect
  {
    get
    {
      if ((UnityEngine.Object) this._lastLocaleChecked_Connect != (UnityEngine.Object) null && (UnityEngine.Object) this._lastLocaleChecked_Connect == (UnityEngine.Object) LocalisationManager.CurrentLanguage && !string.IsNullOrEmpty(this._cachedUnableToConnect))
        return this._cachedUnableToConnect;
      string defaultResult = "UNABLE TO CONNECT TO THE INTERNET. PLEASE CHECK YOUR CONNECTION AND RESTART THE GAME.";
      string result;
      LocalisationManager.TryGetKeyForCurrentLocale("CONNECTION_ISSUE", out result, defaultResult);
      this._lastLocaleChecked_Connect = LocalisationManager.CurrentLanguage;
      this._cachedUnableToConnect = result;
      return this._cachedUnableToConnect;
    }
  }

  public DateTime GetServerTime()
  {
    return this.startupTime + TimeSpan.FromSeconds((double) Time.realtimeSinceStartup);
  }

  public string[] allowedMapsToJoin
  {
    get => this._allowedMapsToJoin;
    set => this._allowedMapsToJoin = value;
  }

  public string VStumpRoomPrepend => this.virtualStumpRoomPrepend;

  public GorillaComputer.ComputerState currentState
  {
    get
    {
      GorillaComputer.ComputerState currentState;
      this.stateStack.TryPeek(ref currentState);
      return currentState;
    }
  }

  public string NameTagPlayerPref
  {
    get
    {
      if (!((UnityEngine.Object) PlayFabAuthenticator.instance == (UnityEngine.Object) null))
        return "nameTagsOn-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
      Debug.LogError((object) "Trying to access PlayFab Authenticator Instance, but it is null. Will use a shared key for the nametag instead");
      return "nameTagsOn";
    }
  }

  public bool NametagsEnabled { get; private set; }

  public GorillaComputer.RedemptionResult RedemptionStatus
  {
    get => this.redemptionResult;
    set
    {
      this.redemptionResult = value;
      this.UpdateScreen();
    }
  }

  public string RedemptionCode
  {
    get => this.redemptionCode;
    set => this.redemptionCode = value;
  }

  private void Awake()
  {
    if ((UnityEngine.Object) GorillaComputer.instance == (UnityEngine.Object) null)
    {
      GorillaComputer.instance = this;
      GorillaComputer.hasInstance = true;
    }
    else if ((UnityEngine.Object) GorillaComputer.instance != (UnityEngine.Object) this)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    Debug.Log((object) $"==== GORILLA TAG - VERSION: {this.version}, BUILD NUMBER: {this.buildCode}, BUILD DATE: {this.buildDate} ====\r\n\r\n      ___   ___\r\n     /   ---   \\\r\n    C|  @   @  |D\r\n      \\  . .  /\r\n       |     |\r\n       | _._ |\r\n       \\_____/\r\n\r\n\r\n");
    this._activeOrderList = this.OrderList;
    this.defaultUpdateCooldown = this.updateCooldown;
  }

  private void Start()
  {
    Debug.Log((object) "Computer Init");
    this.Initialise();
  }

  public void OnEnable()
  {
    KIDManager.RegisterSessionUpdatedCallback_VoiceChat(new Action<bool, KID.Model.Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
    KIDManager.RegisterSessionUpdatedCallback_CustomUsernames(new Action<bool, KID.Model.Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
    GorillaSlicerSimpleManager.RegisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.Update);
  }

  public void OnDisable()
  {
    KIDManager.UnregisterSessionUpdatedCallback_VoiceChat(new Action<bool, KID.Model.Permission.ManagedByEnum>(this.SetVoiceChatBySafety));
    KIDManager.UnregisterSessionUpdatedCallback_CustomUsernames(new Action<bool, KID.Model.Permission.ManagedByEnum>(this.OnKIDSessionUpdated_CustomNicknames));
    GorillaSlicerSimpleManager.UnregisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.Update);
  }

  protected void OnDestroy()
  {
    if ((UnityEngine.Object) GorillaComputer.instance == (UnityEngine.Object) this)
    {
      GorillaComputer.hasInstance = false;
      GorillaComputer.instance = (GorillaComputer) null;
    }
    KIDManager.UnregisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
  }

  public void SliceUpdate()
  {
    if (this.internetFailure && (double) Time.time < (double) this.lastCheckedWifi + (double) this.checkIfConnectedSeconds || !this.internetFailure && (double) Time.time < (double) this.lastCheckedWifi + (double) this.checkIfDisconnectedSeconds)
    {
      if (this.internetFailure || !this.isConnectedToMaster || (double) Time.time <= (double) this.lastUpdateTime + (double) this.updateCooldown)
        return;
      this.deltaTime = Time.time - this.lastUpdateTime;
      this.lastUpdateTime = Time.time;
      this.UpdateScreen();
    }
    else
    {
      this.lastCheckedWifi = Time.time;
      this.stateUpdated = false;
      if (!this.CheckInternetConnection())
      {
        string defaultResult = "NO WIFI OR LAN CONNECTION DETECTED.";
        string result;
        LocalisationManager.TryGetKeyForCurrentLocale("NO_CONNECTION", out result, defaultResult);
        this.UpdateFailureText(result);
        this.internetFailure = true;
      }
      else if (this.internetFailure)
      {
        if (this.CheckInternetConnection())
          this.internetFailure = false;
        this.RestoreFromFailureState();
        this.UpdateScreen();
      }
      else
      {
        if (!this.isConnectedToMaster || (double) Time.time <= (double) this.lastUpdateTime + (double) this.updateCooldown)
          return;
        this.deltaTime = Time.time - this.lastUpdateTime;
        this.lastUpdateTime = Time.time;
        this.UpdateScreen();
      }
    }
  }

  private void Initialise()
  {
    GameEvents.OnGorrillaKeyboardButtonPressedEvent.AddListener(new UnityAction<GorillaKeyboardBindings>(this.PressButton));
    RoomSystem.JoinedRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>) RoomSystem.JoinedRoomEvent + new Action(this.UpdateScreen);
    RoomSystem.LeftRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>) RoomSystem.LeftRoomEvent + new Action(this.UpdateScreen);
    RoomSystem.PlayerJoinedEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>) RoomSystem.PlayerJoinedEvent + new Action<NetPlayer>(this.PlayerCountChangedCallback);
    RoomSystem.PlayerLeftEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>) RoomSystem.PlayerLeftEvent + new Action<NetPlayer>(this.PlayerCountChangedCallback);
    LocalisationManager.RegisterOnLanguageChanged((Action) (() =>
    {
      this.RefreshFunctionNames();
      this.UpdateGameModeText();
    }));
    this.RefreshFunctionNames();
    this.InitialiseRoomScreens();
    this.InitialiseStrings();
    this.InitialiseAllRoomStates();
    this.UpdateScreen();
    this.virtualStumpRoomPrepend = System.Text.Encoding.ASCII.GetString(new byte[1]
    {
      Convert.ToByte(64 /*0x40*/)
    });
    this.initialized = true;
  }

  private void InitialiseRoomScreens()
  {
    this.screenText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.ScreenTextChangedEvent, GameEvents.ScreenTextMaterialsEvent);
    this.functionSelectText.Initialize(this.computerScreenRenderer.materials, this.wrongVersionMaterial, GameEvents.FunctionSelectTextChangedEvent);
  }

  private void InitialiseStrings()
  {
    this.roomToJoin = "";
    this.redText = "";
    this.blueText = "";
    this.greenText = "";
    this.currentName = "";
    this.savedName = "";
  }

  private void InitialiseAllRoomStates()
  {
    this.SwitchState(GorillaComputer.ComputerState.Startup);
    this.InitialiseLanguageScreen();
    this.InitializeNameState();
    this.InitializeRoomState();
    this.InitializeTurnState();
    this.InitializeStartupState();
    this.InitializeQueueState();
    this.InitializeMicState();
    this.InitializeGroupState();
    this.InitializeVoiceState();
    this.InitializeAutoMuteState();
    this.InitializeGameMode();
    this.InitializeVisualsState();
    this.InitializeCreditsState();
    this.InitializeTimeState();
    this.InitializeSupportState();
    this.InitializeTroopState();
    this.InitializeKIdState();
    this.InitializeRedeemState();
  }

  private void InitializeStartupState()
  {
  }

  private void InitializeRoomState()
  {
  }

  private void InitializeColorState()
  {
    this.redValue = PlayerPrefs.GetFloat("redValue", 0.0f);
    this.greenValue = PlayerPrefs.GetFloat("greenValue", 0.0f);
    this.blueValue = PlayerPrefs.GetFloat("blueValue", 0.0f);
    this.blueText = Mathf.Floor(this.blueValue * 9f).ToString();
    this.redText = Mathf.Floor(this.redValue * 9f).ToString();
    this.greenText = Mathf.Floor(this.greenValue * 9f).ToString();
    this.colorCursorLine = 0;
    GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
  }

  private void InitializeNameState()
  {
    int num = PlayerPrefs.GetInt("nameTagsOn", -1);
    KID.Model.Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
    switch (permissionDataByFeature.ManagedBy)
    {
      case KID.Model.Permission.ManagedByEnum.PLAYER:
        this.NametagsEnabled = num != -1 ? num > 0 : permissionDataByFeature.Enabled;
        break;
      case KID.Model.Permission.ManagedByEnum.GUARDIAN:
        this.NametagsEnabled = permissionDataByFeature.Enabled && num > 0;
        break;
      case KID.Model.Permission.ManagedByEnum.PROHIBITED:
        this.NametagsEnabled = false;
        break;
    }
    this.savedName = PlayerPrefs.GetString("playerName", "gorilla");
    NetworkSystem.Instance.SetMyNickName(this.savedName);
    this.currentName = this.savedName;
    VRRigCache.Instance.localRig.Rig.UpdateName();
    this.exactOneWeek = this.exactOneWeekFile.text.Split('\n', StringSplitOptions.None);
    this.anywhereOneWeek = this.anywhereOneWeekFile.text.Split('\n', StringSplitOptions.None);
    this.anywhereTwoWeek = this.anywhereTwoWeekFile.text.Split('\n', StringSplitOptions.None);
    for (int index = 0; index < this.exactOneWeek.Length; ++index)
      this.exactOneWeek[index] = this.exactOneWeek[index].ToLower().TrimEnd('\r', '\n');
    for (int index = 0; index < this.anywhereOneWeek.Length; ++index)
      this.anywhereOneWeek[index] = this.anywhereOneWeek[index].ToLower().TrimEnd('\r', '\n');
    for (int index = 0; index < this.anywhereTwoWeek.Length; ++index)
      this.anywhereTwoWeek[index] = this.anywhereTwoWeek[index].ToLower().TrimEnd('\r', '\n');
  }

  private void InitializeTurnState() => GorillaSnapTurn.LoadSettingsFromPlayerPrefs();

  private void InitializeMicState()
  {
    this.pttType = PlayerPrefs.GetString("pttType", "OPEN MIC");
    if (!(this.pttType == "ALL CHAT"))
      return;
    this.pttType = "OPEN MIC";
    PlayerPrefs.SetString("pttType", this.pttType);
    PlayerPrefs.Save();
  }

  private void InitializeAutoMuteState()
  {
    switch (PlayerPrefs.GetInt("autoMute", 1))
    {
      case 0:
        this.autoMuteType = "OFF";
        break;
      case 1:
        this.autoMuteType = "MODERATE";
        break;
      case 2:
        this.autoMuteType = "AGGRESSIVE";
        break;
    }
  }

  private void InitializeQueueState()
  {
    this.currentQueue = PlayerPrefs.GetString("currentQueue", "DEFAULT");
    this.allowedInCompetitive = PlayerPrefs.GetInt("allowedInCompetitive", 0) == 1;
    if (this.allowedInCompetitive || !(this.currentQueue == "COMPETITIVE"))
      return;
    PlayerPrefs.SetString("currentQueue", "DEFAULT");
    PlayerPrefs.Save();
    this.currentQueue = "DEFAULT";
  }

  private void InitializeGroupState()
  {
    this.groupMapJoin = PlayerPrefs.GetString("groupMapJoin", "FOREST");
    this.groupMapJoinIndex = PlayerPrefs.GetInt("groupMapJoinIndex", 0);
    this.allowedMapsToJoin = this.friendJoinCollider.myAllowedMapsToJoin;
  }

  private void InitializeTroopState()
  {
    bool flag = false;
    this.troopToJoin = this.troopName = PlayerPrefs.GetString("troopName", string.Empty);
    if (!this.rememberTroopQueueState && ((PlayerPrefs.GetInt("troopQueueActive", 0) == 1 ? 1 : 0) | (!(this.currentQueue != "DEFAULT") || !(this.currentQueue != "COMPETITIVE") ? (false ? 1 : 0) : (this.currentQueue != "MINIGAMES" ? 1 : 0))) != 0)
    {
      this.currentQueue = "DEFAULT";
      PlayerPrefs.SetInt("troopQueueActive", 0);
      PlayerPrefs.SetString("currentQueue", this.currentQueue);
      PlayerPrefs.Save();
    }
    this.troopQueueActive = PlayerPrefs.GetInt("troopQueueActive", 0) == 1;
    if (this.troopQueueActive && !this.IsValidTroopName(this.troopName))
    {
      this.troopQueueActive = false;
      PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
      this.currentQueue = "DEFAULT";
      PlayerPrefs.SetString("currentQueue", this.currentQueue);
      flag = true;
    }
    if (this.troopQueueActive)
      this.StartCoroutine(this.HandleInitialTroopQueueState());
    if (!flag)
      return;
    PlayerPrefs.Save();
  }

  private IEnumerator HandleInitialTroopQueueState()
  {
    Debug.Log((object) "HandleInitialTroopQueueState()");
    while (!PlayFabCloudScriptAPI.IsEntityLoggedIn())
      yield return (object) null;
    this.RequestTroopPopulation();
    while (this.currentTroopPopulation < 0)
      yield return (object) null;
    if (this.currentTroopPopulation < 2)
    {
      Debug.Log((object) "Low population - starting in DEFAULT queue");
      this.JoinDefaultQueue();
    }
  }

  private void InitializeVoiceState()
  {
    KID.Model.Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
    string str = PlayerPrefs.GetString("voiceChatOn", "");
    string defaultValue = "FALSE";
    switch (permissionDataByFeature.ManagedBy)
    {
      case KID.Model.Permission.ManagedByEnum.PLAYER:
        defaultValue = !string.IsNullOrEmpty(str) ? str : (permissionDataByFeature.Enabled ? "TRUE" : "FALSE");
        break;
      case KID.Model.Permission.ManagedByEnum.GUARDIAN:
        defaultValue = !permissionDataByFeature.Enabled ? "FALSE" : (string.IsNullOrEmpty(str) ? "FALSE" : str);
        break;
      case KID.Model.Permission.ManagedByEnum.PROHIBITED:
        defaultValue = "FALSE";
        break;
    }
    this.voiceChatOn = PlayerPrefs.GetString("voiceChatOn", defaultValue);
  }

  public void InitializeGameMode(string gameMode)
  {
    this.leftHanded = PlayerPrefs.GetInt("leftHanded", 0) == 1;
    this.OnModeSelectButtonPress(gameMode, this.leftHanded);
    GameModePages.SetSelectedGameModeShared(gameMode);
    this.didInitializeGameMode = true;
  }

  private void InitializeGameMode()
  {
    if (this.didInitializeGameMode)
      return;
    GameModeType gameModeType1 = GameModeType.SuperInfect;
    string gameMode = PlayerPrefs.GetString("currentGameModePostSI", gameModeType1.ToString());
    GameModeType gameModeType2;
    try
    {
      gameModeType2 = Enum.Parse<GameModeType>(gameMode, true);
    }
    catch
    {
      gameModeType2 = GameModeType.SuperInfect;
      gameMode = GameModeType.SuperInfect.ToString();
    }
    if (gameModeType2 != GameModeType.Casual && gameModeType2 != GameModeType.Infection && gameModeType2 != GameModeType.HuntDown && gameModeType2 != GameModeType.Paintbrawl && gameModeType2 != GameModeType.Ambush && gameModeType2 != GameModeType.PropHunt && gameModeType2 != GameModeType.SuperInfect)
    {
      gameModeType1 = GameModeType.SuperInfect;
      PlayerPrefs.SetString("currentGameModePostSI", gameModeType1.ToString());
      PlayerPrefs.Save();
      gameModeType1 = GameModeType.SuperInfect;
      gameMode = gameModeType1.ToString();
    }
    this.leftHanded = PlayerPrefs.GetInt("leftHanded", 0) == 1;
    this.OnModeSelectButtonPress(gameMode, this.leftHanded);
    GameModePages.SetSelectedGameModeShared(gameMode);
  }

  private void InitializeCreditsState()
  {
  }

  private void InitializeTimeState()
  {
    BetterDayNightManager.instance.currentSetting = TimeSettings.Normal;
  }

  private void InitializeSupportState() => this.displaySupport = false;

  private void InitializeVisualsState()
  {
    this.disableParticles = PlayerPrefs.GetString("disableParticles", "FALSE") == "TRUE";
    GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
    this.instrumentVolume = PlayerPrefs.GetFloat("instrumentVolume", 0.1f);
  }

  private void InitializeRedeemState()
  {
    this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
  }

  private bool CheckInternetConnection() => Application.internetReachability != 0;

  public void OnConnectedToMasterStuff()
  {
    if (this.isConnectedToMaster)
      return;
    this.isConnectedToMaster = true;
    GorillaServer instance = GorillaServer.Instance;
    ReturnCurrentVersionRequest request = new ReturnCurrentVersionRequest();
    request.CurrentVersion = NetworkSystemConfig.AppVersionStripped;
    request.UpdatedSynchTest = new int?(this.includeUpdatedServerSynchTest);
    Action<ExecuteFunctionResult> successCallback = new Action<ExecuteFunctionResult>(this.OnReturnCurrentVersion);
    Action<PlayFabError> errorCallback = new Action<PlayFabError>(GorillaComputer.OnErrorShared);
    instance.ReturnCurrentVersion(request, successCallback, errorCallback);
    if (this.startupMillis == 0L && !this.tryGetTimeAgain)
      this.GetCurrentTime();
    int platform = (int) Application.platform;
    this.SaveModAccountData();
    bool safety = PlayFabAuthenticator.instance.GetSafety();
    if (KIDManager.KidEnabledAndReady || KIDManager.HasSession)
      return;
    this.SetComputerSettingsBySafety((safety ? 1 : 0) != 0, new GorillaComputer.ComputerState[4]
    {
      GorillaComputer.ComputerState.Voice,
      GorillaComputer.ComputerState.AutoMute,
      GorillaComputer.ComputerState.Name,
      GorillaComputer.ComputerState.Group
    }, false);
  }

  private void OnReturnCurrentVersion(ExecuteFunctionResult result)
  {
    JsonObject functionResult = (JsonObject) result.FunctionResult;
    if (functionResult != null)
    {
      object s;
      if (functionResult.TryGetValue("SynchTime", out s))
        Debug.Log((object) ("message value is: " + (string) s));
      if (functionResult.TryGetValue("Fail", out s) && (bool) s)
        this.GeneralFailureMessage(this.versionMismatch);
      else if (functionResult.TryGetValue("ResultCode", out s) && (ulong) s != 0UL)
      {
        this.GeneralFailureMessage(this.versionMismatch);
      }
      else
      {
        if (functionResult.TryGetValue("QueueStats", out s) && ((JsonObject) s).TryGetValue("TopTroops", out s))
        {
          this.topTroops.Clear();
          foreach (object obj in (List<object>) s)
            this.topTroops.Add(obj.ToString());
        }
        if (functionResult.TryGetValue("BannedUsers", out s))
          this.usersBanned = int.Parse((string) s);
        this.UpdateScreen();
      }
    }
    else
      this.GeneralFailureMessage(this.versionMismatch);
  }

  public void SaveModAccountData()
  {
    string path = Application.persistentDataPath + "/DoNotShareWithAnyoneEVERNoMatterWhatTheySay.txt";
    if (File.Exists(path))
      return;
    GorillaServer.Instance.ReturnMyOculusHash((Action<ExecuteFunctionResult>) (result =>
    {
      object obj;
      if (!((JsonObject) result.FunctionResult).TryGetValue("oculusHash", out obj))
        return;
      StreamWriter streamWriter = new StreamWriter(path);
      streamWriter.Write($"{PlayFabAuthenticator.instance.GetPlayFabPlayerId()}.{(string) obj}");
      streamWriter.Close();
    }), (Action<PlayFabError>) (error =>
    {
      if (error.Error == PlayFabErrorCode.NotAuthenticated)
      {
        PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
      }
      else
      {
        if (error.Error != PlayFabErrorCode.AccountBanned)
          return;
        GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
      }
    }));
  }

  public void PressButton(GorillaKeyboardBindings buttonPressed)
  {
    if (this.currentState == GorillaComputer.ComputerState.Startup)
    {
      this.ProcessStartupState(buttonPressed);
      this.UpdateScreen();
    }
    else
    {
      this.RequestTroopPopulation();
      bool flag = true;
      switch (buttonPressed)
      {
        case GorillaKeyboardBindings.up:
          flag = false;
          this.DecreaseState();
          break;
        case GorillaKeyboardBindings.down:
          flag = false;
          this.IncreaseState();
          break;
      }
      if (flag)
      {
        switch (this.currentState)
        {
          case GorillaComputer.ComputerState.Name:
            this.ProcessNameState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.Turn:
            this.ProcessTurnState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.Mic:
            this.ProcessMicState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.Room:
            this.ProcessRoomState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.Queue:
            this.ProcessQueueState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.Group:
            this.ProcessGroupState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.Voice:
            this.ProcessVoiceState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.AutoMute:
            this.ProcessAutoMuteState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.Credits:
            this.ProcessCreditsState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.Visuals:
            this.ProcessVisualsState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.NameWarning:
            this.ProcessNameWarningState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.Support:
            this.ProcessSupportState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.Troop:
            this.ProcessTroopState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.KID:
            this.ProcessKIdState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.Redemption:
            this.ProcessRedemptionState(buttonPressed);
            break;
          case GorillaComputer.ComputerState.Language:
            this.ProcessLanguageState(buttonPressed);
            break;
        }
      }
      this.UpdateScreen();
    }
  }

  public void OnModeSelectButtonPress(string gameMode, bool leftHand)
  {
    this.lastPressedGameMode = gameMode;
    PlayerPrefs.SetString("currentGameModePostSI", gameMode);
    if (leftHand != this.leftHanded)
    {
      PlayerPrefs.SetInt("leftHanded", leftHand ? 1 : 0);
      this.leftHanded = leftHand;
    }
    PlayerPrefs.Save();
    if (FriendshipGroupDetection.Instance.IsInParty)
      FriendshipGroupDetection.Instance.SendRequestPartyGameMode(gameMode);
    else
      this.SetGameModeWithoutButton(gameMode);
  }

  public void SetGameModeWithoutButton(string gameMode)
  {
    this.currentGameMode.Value = gameMode;
    this.UpdateGameModeText();
    PhotonNetworkController.Instance.UpdateTriggerScreens();
  }

  public void RegisterPrimaryJoinTrigger(GorillaNetworkJoinTrigger trigger)
  {
    this.primaryTriggersByZone[trigger.networkZone] = trigger;
  }

  private GorillaNetworkJoinTrigger GetSelectedMapJoinTrigger()
  {
    GorillaNetworkJoinTrigger selectedMapJoinTrigger;
    this.primaryTriggersByZone.TryGetValue(this.allowedMapsToJoin[Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex)], out selectedMapJoinTrigger);
    return selectedMapJoinTrigger;
  }

  public GorillaNetworkJoinTrigger GetJoinTriggerForZone(string zone)
  {
    GorillaNetworkJoinTrigger joinTriggerForZone;
    this.primaryTriggersByZone.TryGetValue(zone, out joinTriggerForZone);
    return joinTriggerForZone;
  }

  public GorillaNetworkJoinTrigger GetJoinTriggerFromFullGameModeString(string gameModeString)
  {
    foreach (KeyValuePair<string, GorillaNetworkJoinTrigger> keyValuePair in this.primaryTriggersByZone)
    {
      if (gameModeString.StartsWith(keyValuePair.Key))
        return keyValuePair.Value;
    }
    return (GorillaNetworkJoinTrigger) null;
  }

  public void OnGroupJoinButtonPress(
    int mapJoinIndex,
    GorillaFriendCollider chosenFriendJoinCollider)
  {
    Debug.Log((object) $"On Group button press. Map:{mapJoinIndex.ToString()} - collider: {chosenFriendJoinCollider.name}");
    if (mapJoinIndex >= this.allowedMapsToJoin.Length)
    {
      this.roomNotAllowed = true;
      this.currentStateIndex = 0;
      this.SwitchState(this.GetState(this.currentStateIndex));
    }
    else
    {
      GorillaNetworkJoinTrigger selectedMapJoinTrigger = this.GetSelectedMapJoinTrigger();
      if (FriendshipGroupDetection.Instance.IsInParty)
      {
        if ((UnityEngine.Object) selectedMapJoinTrigger != (UnityEngine.Object) null && selectedMapJoinTrigger.CanPartyJoin())
        {
          PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.ForceJoinWithParty);
          this.currentStateIndex = 0;
          this.SwitchState(this.GetState(this.currentStateIndex));
        }
        else
          this.UpdateScreen();
      }
      else
      {
        if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.SessionIsPrivate)
          return;
        PhotonNetworkController.Instance.FriendIDList = new List<string>((IEnumerable<string>) chosenFriendJoinCollider.playerIDsCurrentlyTouching);
        foreach (string friendId in this.networkController.FriendIDList)
          Debug.Log((object) ("Friend ID:" + friendId));
        PhotonNetworkController.Instance.shuffler = UnityEngine.Random.Range(0, 99).ToString().PadLeft(2, '0') + UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
        PhotonNetworkController.Instance.keyStr = UnityEngine.Random.Range(0, 99999999).ToString().PadLeft(8, '0');
        RoomSystem.SendNearbyFollowCommand(chosenFriendJoinCollider, PhotonNetworkController.Instance.shuffler, PhotonNetworkController.Instance.keyStr);
        PhotonNetwork.SendAllOutgoingCommands();
        PhotonNetworkController.Instance.AttemptToJoinPublicRoom(selectedMapJoinTrigger, JoinType.JoinWithNearby);
        this.currentStateIndex = 0;
        this.SwitchState(this.GetState(this.currentStateIndex));
      }
    }
  }

  public void CompQueueUnlockButtonPress()
  {
    this.allowedInCompetitive = true;
    PlayerPrefs.SetInt("allowedInCompetitive", 1);
    PlayerPrefs.Save();
    if (!((UnityEngine.Object) RankedProgressionManager.Instance != (UnityEngine.Object) null))
      return;
    RankedProgressionManager.Instance.RequestUnlockCompetitiveQueue(true);
  }

  private void SwitchState(GorillaComputer.ComputerState newState, bool clearStack = true)
  {
    if (this.currentComputerState == GorillaComputer.ComputerState.Mic && this.currentComputerState != newState)
      this.updateCooldown = this.defaultUpdateCooldown;
    else if (newState == GorillaComputer.ComputerState.Mic)
      this.updateCooldown = this.micUpdateCooldown;
    if (this.previousComputerState != this.currentComputerState)
      this.previousComputerState = this.currentComputerState;
    this.currentComputerState = newState;
    if (this.LoadingRoutine != null)
      this.StopCoroutine(this.LoadingRoutine);
    if (clearStack)
      this.stateStack.Clear();
    this.stateStack.Push(newState);
  }

  private void PopState()
  {
    this.currentComputerState = this.previousComputerState;
    if (this.stateStack.Count <= 1)
    {
      Debug.LogError((object) "Can't pop into an empty stack");
    }
    else
    {
      int num = (int) this.stateStack.Pop();
      this.UpdateScreen();
    }
  }

  private void SwitchToWarningState()
  {
    this.warningConfirmationInputString = string.Empty;
    this.SwitchState(GorillaComputer.ComputerState.NameWarning, false);
  }

  private void SwitchToLoadingState()
  {
    this.SwitchState(GorillaComputer.ComputerState.Loading, false);
  }

  private void ProcessStartupState(GorillaKeyboardBindings buttonPressed)
  {
    this.SwitchState(this.GetState(this.currentStateIndex));
  }

  private void ProcessColorState(GorillaKeyboardBindings buttonPressed)
  {
    switch (buttonPressed)
    {
      case GorillaKeyboardBindings.enter:
        break;
      case GorillaKeyboardBindings.option1:
        this.colorCursorLine = 0;
        break;
      case GorillaKeyboardBindings.option2:
        this.colorCursorLine = 1;
        break;
      case GorillaKeyboardBindings.option3:
        this.colorCursorLine = 2;
        break;
      default:
        int num = (int) buttonPressed;
        if (num >= 10)
          break;
        switch (this.colorCursorLine)
        {
          case 0:
            this.redText = num.ToString();
            this.redValue = (float) num / 9f;
            PlayerPrefs.SetFloat("redValue", this.redValue);
            break;
          case 1:
            this.greenText = num.ToString();
            this.greenValue = (float) num / 9f;
            PlayerPrefs.SetFloat("greenValue", this.greenValue);
            break;
          case 2:
            this.blueText = num.ToString();
            this.blueValue = (float) num / 9f;
            PlayerPrefs.SetFloat("blueValue", this.blueValue);
            break;
        }
        GorillaTagger.Instance.UpdateColor(this.redValue, this.greenValue, this.blueValue);
        PlayerPrefs.Save();
        if (!NetworkSystem.Instance.InRoom)
          break;
        GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, (object) this.redValue, (object) this.greenValue, (object) this.blueValue);
        break;
    }
  }

  public void ProcessNameState(GorillaKeyboardBindings buttonPressed)
  {
    if (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
      return;
    switch (buttonPressed)
    {
      case GorillaKeyboardBindings.delete:
        if (this.currentName.Length <= 0 || !this.NametagsEnabled)
          break;
        this.currentName = this.currentName.Substring(0, this.currentName.Length - 1);
        break;
      case GorillaKeyboardBindings.enter:
        if (!(this.currentName != this.savedName) || !(this.currentName != "") || !this.NametagsEnabled)
          break;
        this.CheckAutoBanListForPlayerName(this.currentName);
        break;
      case GorillaKeyboardBindings.option1:
        this.UpdateNametagSetting(!this.NametagsEnabled);
        break;
      default:
        if (!this.NametagsEnabled || this.currentName.Length >= 12 || buttonPressed >= GorillaKeyboardBindings.up && buttonPressed <= GorillaKeyboardBindings.option3)
          break;
        this.currentName += buttonPressed < GorillaKeyboardBindings.up ? ((int) buttonPressed).ToString() : buttonPressed.ToString();
        break;
    }
  }

  private void ProcessRoomState(GorillaKeyboardBindings buttonPressed)
  {
    if (this.limitOnlineScreens)
      return;
    bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
    switch (buttonPressed)
    {
      case GorillaKeyboardBindings.delete:
        if (!flag || (!this.playerInVirtualStump || this.roomToJoin.Length <= 1) && (this.playerInVirtualStump || this.roomToJoin.Length <= 0))
          break;
        this.roomToJoin = this.roomToJoin.Substring(0, this.roomToJoin.Length - 1);
        break;
      case GorillaKeyboardBindings.enter:
        if (!flag || (this.playerInVirtualStump || !(this.roomToJoin != "")) && (!this.playerInVirtualStump || this.roomToJoin.Length <= 1))
          break;
        this.CheckAutoBanListForRoomName(this.roomToJoin);
        break;
      case GorillaKeyboardBindings.option1:
        if (FriendshipGroupDetection.Instance.IsInParty)
        {
          FriendshipGroupDetection.Instance.LeaveParty();
          this.DisconnectAfterDelay(1f);
          break;
        }
        NetworkSystem.Instance.ReturnToSinglePlayer();
        break;
      case GorillaKeyboardBindings.option2:
        this.RequestUpdatedPermissions();
        break;
      case GorillaKeyboardBindings.option3:
        break;
      default:
        if (!flag || this.roomToJoin.Length >= 10)
          break;
        this.roomToJoin += buttonPressed < GorillaKeyboardBindings.up ? ((int) buttonPressed).ToString() : buttonPressed.ToString();
        break;
    }
  }

  private async void DisconnectAfterDelay(float seconds)
  {
    await Task.Delay((int) (1000.0 * (double) seconds));
    await NetworkSystem.Instance.ReturnToSinglePlayer();
  }

  private void ProcessTurnState(GorillaKeyboardBindings buttonPressed)
  {
    int factor = (int) buttonPressed;
    if (factor < 10)
    {
      GorillaSnapTurn.UpdateAndSaveTurnFactor(factor);
    }
    else
    {
      string mode = string.Empty;
      switch (buttonPressed)
      {
        case GorillaKeyboardBindings.option1:
          mode = "SNAP";
          break;
        case GorillaKeyboardBindings.option2:
          mode = "SMOOTH";
          break;
        case GorillaKeyboardBindings.option3:
          mode = "NONE";
          break;
      }
      if (mode.Length <= 0)
        return;
      GorillaSnapTurn.UpdateAndSaveTurnType(mode);
    }
  }

  private void ProcessMicState(GorillaKeyboardBindings buttonPressed)
  {
    switch (buttonPressed)
    {
      case GorillaKeyboardBindings.option1:
        this.pttType = "OPEN MIC";
        PlayerPrefs.SetString("pttType", this.pttType);
        PlayerPrefs.Save();
        break;
      case GorillaKeyboardBindings.option2:
        this.pttType = "PUSH TO TALK";
        PlayerPrefs.SetString("pttType", this.pttType);
        PlayerPrefs.Save();
        break;
      case GorillaKeyboardBindings.option3:
        this.pttType = "PUSH TO MUTE";
        PlayerPrefs.SetString("pttType", this.pttType);
        PlayerPrefs.Save();
        break;
    }
  }

  private void ProcessQueueState(GorillaKeyboardBindings buttonPressed)
  {
    if (this.limitOnlineScreens)
      return;
    switch (buttonPressed)
    {
      case GorillaKeyboardBindings.option1:
        this.JoinQueue("DEFAULT");
        break;
      case GorillaKeyboardBindings.option2:
        this.JoinQueue("MINIGAMES");
        break;
      case GorillaKeyboardBindings.option3:
        if (!this.allowedInCompetitive)
          break;
        this.JoinQueue("COMPETITIVE");
        break;
    }
  }

  public void JoinTroop(string newTroopName)
  {
    if (!this.IsValidTroopName(newTroopName))
      return;
    this.currentTroopPopulation = -1;
    this.troopName = newTroopName;
    PlayerPrefs.SetString("troopName", this.troopName);
    if (this.troopQueueActive)
    {
      this.currentQueue = this.GetQueueNameForTroop(this.troopName);
      PlayerPrefs.SetString("currentQueue", this.currentQueue);
    }
    PlayerPrefs.Save();
    this.JoinTroopQueue();
  }

  public void JoinTroopQueue()
  {
    if (!this.IsValidTroopName(this.troopName))
      return;
    this.currentTroopPopulation = -1;
    this.JoinQueue(this.GetQueueNameForTroop(this.troopName), true);
    this.RequestTroopPopulation(true);
  }

  private void RequestTroopPopulation(bool forceUpdate = false)
  {
    if (!PlayFabCloudScriptAPI.IsEntityLoggedIn() || !(!this.hasRequestedInitialTroopPopulation | forceUpdate) || (double) this.nextPopulationCheckTime > (double) Time.time)
      return;
    this.nextPopulationCheckTime = Time.time + this.troopPopulationCheckCooldown;
    this.hasRequestedInitialTroopPopulation = true;
    GorillaServer instance = GorillaServer.Instance;
    ReturnQueueStatsRequest request = new ReturnQueueStatsRequest();
    request.queueName = this.troopName;
    Action<ExecuteFunctionResult> successCallback = (Action<ExecuteFunctionResult>) (result =>
    {
      Debug.Log((object) "Troop pop received");
      object obj;
      if (((JsonObject) result.FunctionResult).TryGetValue("PlayerCount", out obj))
      {
        this.currentTroopPopulation = int.Parse(obj.ToString());
        if (this.currentComputerState != GorillaComputer.ComputerState.Queue)
          return;
        this.UpdateScreen();
      }
      else
        this.currentTroopPopulation = 0;
    });
    Action<PlayFabError> errorCallback = (Action<PlayFabError>) (error =>
    {
      Debug.LogError((object) $"Error requesting troop population: {error}");
      this.currentTroopPopulation = -1;
    });
    instance.ReturnQueueStats(request, successCallback, errorCallback);
  }

  public void JoinDefaultQueue() => this.JoinQueue("DEFAULT");

  public void LeaveTroop()
  {
    if (this.IsValidTroopName(this.troopName))
      this.troopToJoin = this.troopName;
    this.currentTroopPopulation = -1;
    this.troopName = string.Empty;
    PlayerPrefs.SetString("troopName", this.troopName);
    if (this.troopQueueActive)
      this.JoinDefaultQueue();
    PlayerPrefs.Save();
  }

  public string GetCurrentTroop() => this.troopQueueActive ? this.troopName : this.currentQueue;

  public int GetCurrentTroopPopulation()
  {
    return this.troopQueueActive ? this.currentTroopPopulation : -1;
  }

  private void JoinQueue(string queueName, bool isTroopQueue = false)
  {
    this.currentQueue = queueName;
    this.troopQueueActive = isTroopQueue;
    this.currentTroopPopulation = -1;
    PlayerPrefs.SetString("currentQueue", this.currentQueue);
    PlayerPrefs.SetInt("troopQueueActive", this.troopQueueActive ? 1 : 0);
    PlayerPrefs.Save();
  }

  private void ProcessGroupState(GorillaKeyboardBindings buttonPressed)
  {
    if (this.limitOnlineScreens)
      return;
    switch (buttonPressed)
    {
      case GorillaKeyboardBindings.one:
        this.groupMapJoin = "FOREST";
        this.groupMapJoinIndex = 0;
        PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
        PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
        PlayerPrefs.Save();
        break;
      case GorillaKeyboardBindings.two:
        this.groupMapJoin = "CAVE";
        this.groupMapJoinIndex = 1;
        PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
        PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
        PlayerPrefs.Save();
        break;
      case GorillaKeyboardBindings.three:
        this.groupMapJoin = "CANYON";
        this.groupMapJoinIndex = 2;
        PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
        PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
        PlayerPrefs.Save();
        break;
      case GorillaKeyboardBindings.four:
        this.groupMapJoin = "CITY";
        this.groupMapJoinIndex = 3;
        PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
        PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
        PlayerPrefs.Save();
        break;
      case GorillaKeyboardBindings.five:
        this.groupMapJoin = "CLOUDS";
        this.groupMapJoinIndex = 4;
        PlayerPrefs.SetString("groupMapJoin", this.groupMapJoin);
        PlayerPrefs.SetInt("groupMapJoinIndex", this.groupMapJoinIndex);
        PlayerPrefs.Save();
        break;
      case GorillaKeyboardBindings.enter:
        this.OnGroupJoinButtonPress(Mathf.Min(this.allowedMapsToJoin.Length - 1, this.groupMapJoinIndex), this.friendJoinCollider);
        break;
    }
    this.roomFull = false;
  }

  private void ProcessTroopState(GorillaKeyboardBindings buttonPressed)
  {
    if (this.limitOnlineScreens)
      return;
    int num = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) ? 1 : 0;
    bool flag = this.IsValidTroopName(this.troopName);
    if (num != 0)
    {
      switch (buttonPressed)
      {
        case GorillaKeyboardBindings.delete:
          if (flag || this.troopToJoin.Length <= 0)
            break;
          this.troopToJoin = this.troopToJoin.Substring(0, this.troopToJoin.Length - 1);
          break;
        case GorillaKeyboardBindings.enter:
          if (flag)
            break;
          this.CheckAutoBanListForTroopName(this.troopToJoin);
          break;
        case GorillaKeyboardBindings.option1:
          this.JoinTroopQueue();
          break;
        case GorillaKeyboardBindings.option2:
          this.JoinDefaultQueue();
          break;
        case GorillaKeyboardBindings.option3:
          this.LeaveTroop();
          break;
        default:
          if (flag || this.troopToJoin.Length >= 12)
            break;
          this.troopToJoin += buttonPressed < GorillaKeyboardBindings.up ? ((int) buttonPressed).ToString() : buttonPressed.ToString();
          break;
      }
    }
    else
    {
      switch (buttonPressed)
      {
        case GorillaKeyboardBindings.option2:
          if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
          {
            this.ProcessScreen_SetupKID();
            break;
          }
          this.RequestUpdatedPermissions();
          break;
        case GorillaKeyboardBindings.option3:
          if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
            break;
          this.ProcessScreen_SetupKID();
          break;
      }
    }
  }

  private bool IsValidTroopName(string troop)
  {
    if (string.IsNullOrEmpty(troop) || troop.Length > 12)
      return false;
    return this.allowedInCompetitive || troop != "COMPETITIVE";
  }

  private string GetQueueNameForTroop(string troop) => troop;

  private void ProcessVoiceState(GorillaKeyboardBindings buttonPressed)
  {
    if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
    {
      switch (buttonPressed)
      {
        case GorillaKeyboardBindings.option1:
          this.SetVoice(true);
          break;
        case GorillaKeyboardBindings.option2:
          this.SetVoice(false);
          break;
      }
    }
    else
    {
      switch (buttonPressed)
      {
        case GorillaKeyboardBindings.option2:
          if (this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
          {
            this.ProcessScreen_SetupKID();
            break;
          }
          this.RequestUpdatedPermissions();
          break;
        case GorillaKeyboardBindings.option3:
          if (this._currentScreentState != GorillaComputer.EKidScreenState.Show_OTP)
            return;
          this.ProcessScreen_SetupKID();
          break;
      }
    }
    RigContainer.RefreshAllRigVoices();
  }

  private void ProcessAutoMuteState(GorillaKeyboardBindings buttonPressed)
  {
    switch (buttonPressed)
    {
      case GorillaKeyboardBindings.option1:
        this.autoMuteType = "AGGRESSIVE";
        PlayerPrefs.SetInt("autoMute", 2);
        PlayerPrefs.Save();
        RigContainer.RefreshAllRigVoices();
        break;
      case GorillaKeyboardBindings.option2:
        this.autoMuteType = "MODERATE";
        PlayerPrefs.SetInt("autoMute", 1);
        PlayerPrefs.Save();
        RigContainer.RefreshAllRigVoices();
        break;
      case GorillaKeyboardBindings.option3:
        this.autoMuteType = "OFF";
        PlayerPrefs.SetInt("autoMute", 0);
        PlayerPrefs.Save();
        RigContainer.RefreshAllRigVoices();
        break;
    }
    this.UpdateScreen();
  }

  private void ProcessVisualsState(GorillaKeyboardBindings buttonPressed)
  {
    int num = (int) buttonPressed;
    if (num < 10)
    {
      this.instrumentVolume = (float) num / 50f;
      PlayerPrefs.SetFloat("instrumentVolume", this.instrumentVolume);
      PlayerPrefs.Save();
    }
    else if (buttonPressed != GorillaKeyboardBindings.option1)
    {
      if (buttonPressed != GorillaKeyboardBindings.option2)
        return;
      this.disableParticles = true;
      PlayerPrefs.SetString("disableParticles", "TRUE");
      PlayerPrefs.Save();
      GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
    }
    else
    {
      this.disableParticles = false;
      PlayerPrefs.SetString("disableParticles", "FALSE");
      PlayerPrefs.Save();
      GorillaTagger.Instance.ShowCosmeticParticles(!this.disableParticles);
    }
  }

  private void ProcessCreditsState(GorillaKeyboardBindings buttonPressed)
  {
    if (buttonPressed != GorillaKeyboardBindings.enter)
      return;
    this.creditsView.ProcessButtonPress(buttonPressed);
  }

  private void ProcessSupportState(GorillaKeyboardBindings buttonPressed)
  {
    if (buttonPressed != GorillaKeyboardBindings.enter)
      return;
    this.displaySupport = true;
  }

  private void ProcessRedemptionState(GorillaKeyboardBindings buttonPressed)
  {
    if (this.RedemptionStatus == GorillaComputer.RedemptionResult.Checking)
      return;
    switch (buttonPressed)
    {
      case GorillaKeyboardBindings.delete:
        if (this.redemptionCode.Length <= 0)
          break;
        this.redemptionCode = this.redemptionCode.Substring(0, this.redemptionCode.Length - 1);
        break;
      case GorillaKeyboardBindings.enter:
        if (this.redemptionCode != "")
        {
          if (this.redemptionCode.Length < 8)
          {
            this.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
            break;
          }
          CodeRedemption.Instance.HandleCodeRedemption(this.redemptionCode);
          this.RedemptionStatus = GorillaComputer.RedemptionResult.Checking;
          break;
        }
        if (this.RedemptionStatus == GorillaComputer.RedemptionResult.Success)
          break;
        this.RedemptionStatus = GorillaComputer.RedemptionResult.Empty;
        break;
      default:
        if (this.redemptionCode.Length >= 8 || buttonPressed >= GorillaKeyboardBindings.up && buttonPressed <= GorillaKeyboardBindings.option3)
          break;
        this.redemptionCode += buttonPressed < GorillaKeyboardBindings.up ? ((int) buttonPressed).ToString() : buttonPressed.ToString();
        break;
    }
  }

  private void ProcessNameWarningState(GorillaKeyboardBindings buttonPressed)
  {
    if (this.warningConfirmationInputString.ToLower() == "yes")
      this.PopState();
    else if (buttonPressed == GorillaKeyboardBindings.delete)
    {
      if (this.warningConfirmationInputString.Length <= 0)
        return;
      this.warningConfirmationInputString = this.warningConfirmationInputString.Substring(0, this.warningConfirmationInputString.Length - 1);
    }
    else
    {
      if (this.warningConfirmationInputString.Length >= 3)
        return;
      this.warningConfirmationInputString += buttonPressed.ToString();
    }
  }

  public void UpdateScreen()
  {
    if ((UnityEngine.Object) NetworkSystem.Instance != (UnityEngine.Object) null && !NetworkSystem.Instance.WrongVersion)
    {
      this.UpdateFunctionScreen();
      switch (this.currentState)
      {
        case GorillaComputer.ComputerState.Startup:
          this.StartupScreen();
          break;
        case GorillaComputer.ComputerState.Name:
          this.NameScreen();
          break;
        case GorillaComputer.ComputerState.Turn:
          this.TurnScreen();
          break;
        case GorillaComputer.ComputerState.Mic:
          this.MicScreen();
          break;
        case GorillaComputer.ComputerState.Room:
          this.RoomScreen();
          break;
        case GorillaComputer.ComputerState.Queue:
          this.QueueScreen();
          break;
        case GorillaComputer.ComputerState.Group:
          this.GroupScreen();
          break;
        case GorillaComputer.ComputerState.Voice:
          this.VoiceScreen();
          break;
        case GorillaComputer.ComputerState.AutoMute:
          this.AutomuteScreen();
          break;
        case GorillaComputer.ComputerState.Credits:
          this.CreditsScreen();
          break;
        case GorillaComputer.ComputerState.Visuals:
          this.VisualsScreen();
          break;
        case GorillaComputer.ComputerState.Time:
          this.TimeScreen();
          break;
        case GorillaComputer.ComputerState.NameWarning:
          this.NameWarningScreen();
          break;
        case GorillaComputer.ComputerState.Loading:
          this.LoadingScreen();
          break;
        case GorillaComputer.ComputerState.Support:
          this.SupportScreen();
          break;
        case GorillaComputer.ComputerState.Troop:
          this.TroopScreen();
          break;
        case GorillaComputer.ComputerState.KID:
          this.KIdScreen();
          break;
        case GorillaComputer.ComputerState.Redemption:
          this.RedemptionScreen();
          break;
        case GorillaComputer.ComputerState.Language:
          this.LanguageScreen();
          break;
      }
    }
    this.UpdateGameModeText();
  }

  private void LoadingScreen()
  {
    string tmp = "LOADING";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("LOADING_SCREEN", out result, tmp);
    this.screenText.Text = result;
    this.LoadingRoutine = this.StartCoroutine(LoadingScreenLocal());

    IEnumerator LoadingScreenLocal()
    {
      int dotsCount = 0;
      while (this.currentState == GorillaComputer.ComputerState.Loading)
      {
        ++dotsCount;
        if (dotsCount == 3)
          dotsCount = 0;
        tmp = "LOADING";
        string result;
        LocalisationManager.TryGetKeyForCurrentLocale("LOADING_SCREEN", out result, tmp);
        this.screenText.Text = result;
        for (int index = 0; index < dotsCount; ++index)
          this.screenText.Text += ". ";
        yield return (object) this.waitOneSecond;
      }
    }
  }

  private void NameWarningScreen()
  {
    string defaultResult1 = "<color=red>WARNING: PLEASE CHOOSE A BETTER NAME\n\nENTERING ANOTHER BAD NAME WILL RESULT IN A BAN</color>";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN", out result, defaultResult1);
    this.screenText.Text = result;
    if (this.warningConfirmationInputString.ToLower() == "yes")
    {
      string defaultResult2 = "\n\nPRESS ANY KEY TO CONTINUE";
      LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN_CONFIRMATION", out result, defaultResult2);
      this.screenText.Text += result;
    }
    else
    {
      string defaultResult3 = "\n\nTYPE 'YES' TO CONFIRM:";
      LocalisationManager.TryGetKeyForCurrentLocale("WARNING_SCREEN_TYPE_YES", out result, defaultResult3);
      this.screenText.Text += result.TrailingSpace();
      this.screenText.Text += this.warningConfirmationInputString;
    }
  }

  private void SupportScreen()
  {
    this.screenText.Text = "";
    if (this.displaySupport)
    {
      string upper = PlayFabAuthenticator.instance.platform.ToString().ToUpper();
      string defaultResult1 = !(upper == "PC") ? upper : "OCULUS PC";
      string key;
      switch (defaultResult1)
      {
        case "OCULUS PC":
          key = "PLATFORM_OCULUS_PC";
          break;
        case "STEAM":
          key = "PLATFORM_STEAM";
          break;
        case "PSVR":
          key = "PLATFORM_PSVR";
          break;
        case "PICO":
          key = "PLATFORM_PICO";
          break;
        case "QUEST":
          key = "PLATFORM_QUEST";
          break;
        default:
          key = "UNKNOWN_PLATFORM";
          break;
      }
      string result;
      LocalisationManager.TryGetKeyForCurrentLocale(key, out result, defaultResult1);
      string str = result;
      string defaultResult2 = "SUPPORT";
      LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INTRO", out result, defaultResult2);
      this.screenText.Text += result;
      string defaultResult3 = "\n\nPLAYERID";
      LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_PLAYERID", out result, defaultResult3);
      GorillaText screenText1 = this.screenText;
      screenText1.Text = $"{screenText1.Text}{result}   ";
      this.screenText.Text += PlayFabAuthenticator.instance.GetPlayFabPlayerId();
      string defaultResult4 = "\nVERSION";
      LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_VERSION", out result, defaultResult4);
      GorillaText screenText2 = this.screenText;
      screenText2.Text = $"{screenText2.Text}{result}    ";
      this.screenText.Text += this.version.ToUpper();
      string defaultResult5 = "\nPLATFORM";
      LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_PLATFORM", out result, defaultResult5);
      GorillaText screenText3 = this.screenText;
      screenText3.Text = $"{screenText3.Text}{result}   ";
      this.screenText.Text += str;
      string defaultResult6 = "\nBUILD DATE";
      LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_DETAILS_BUILD_DATE", out result, defaultResult6);
      GorillaText screenText4 = this.screenText;
      screenText4.Text = $"{screenText4.Text}{result} ";
      this.screenText.Text += this.buildDate;
      if (!KIDManager.KidEnabled)
        return;
      string defaultResult7 = "\nk-ID ACCOUNT TYPE:";
      LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_KID_ACCOUNT_TYPE", out result, defaultResult7);
      this.screenText.Text += result.TrailingSpace();
      this.screenText.Text += KIDManager.GetActiveAccountStatusNiceString().ToUpper();
    }
    else
    {
      string defaultResult8 = "SUPPORT";
      string result;
      LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INTRO", out result, defaultResult8);
      this.screenText.Text += result;
      string defaultResult9 = "\n\nPRESS ENTER TO DISPLAY SUPPORT AND ACCOUNT INFORMATION";
      LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INITIAL", out result, defaultResult9);
      this.screenText.Text += result;
      string defaultResult10 = "\n\n\n\n<color=red>DO NOT SHARE ACCOUNT INFORMATION WITH ANYONE OTHER THAN ANOTHER AXIOM</color>";
      LocalisationManager.TryGetKeyForCurrentLocale("SUPPORT_SCREEN_INITIAL_WARNING", out result, defaultResult10);
      this.screenText.Text += result;
    }
  }

  private void TimeScreen()
  {
    string defaultResult = "UPDATE TIME SETTINGS. (LOCALLY ONLY). \nPRESS OPTION 1 FOR NORMAL MODE. \nPRESS OPTION 2 FOR STATIC MODE. \nPRESS 1-10 TO CHANGE TIME OF DAY. \nCURRENT MODE: {currentSetting}.\nTIME OF DAY: {currentTimeOfDay}.\n";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("TIME_SCREEN", out result, defaultResult);
    this.screenText.Text = result.Replace("{currentSetting}", BetterDayNightManager.instance.currentSetting.ToString().ToUpper()).Replace("{currentTimeOfDay}", BetterDayNightManager.instance.currentTimeOfDay.ToUpper());
  }

  private void CreditsScreen() => this.screenText.Text = this.creditsView.GetScreenText();

  private void VisualsScreen()
  {
    string defaultResult1 = "UPDATE ITEMS SETTINGS.";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_INTRO", out result, defaultResult1);
    this.screenText.Text = result.TrailingSpace();
    string defaultResult2 = "PRESS OPTION 1 TO ENABLE ITEM PARTICLES. PRESS OPTION 2 TO DISABLE ITEM PARTICLES. PRESS 1-10 TO CHANGE INSTRUMENT VOLUME FOR OTHER PLAYERS.";
    LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_OPTIONS", out result, defaultResult2);
    this.screenText.Text += result;
    string defaultResult3 = "\n\nITEM PARTICLES ON:";
    LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_CURRENT", out result, defaultResult3);
    this.screenText.Text += result.TrailingSpace();
    string str = this.disableParticles ? "FALSE" : "TRUE";
    LocalisationManager.TryGetKeyForCurrentLocale(str, out result, str);
    this.screenText.Text += result;
    string defaultResult4 = "\nINSTRUMENT VOLUME:";
    LocalisationManager.TryGetKeyForCurrentLocale("VISUALS_SCREEN_VOLUME", out result, defaultResult4);
    this.screenText.Text += result.TrailingSpace();
    this.screenText.Text += Mathf.CeilToInt(this.instrumentVolume * 50f).ToString();
  }

  private void VoiceScreen()
  {
    KID.Model.Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
    if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Voice_Chat))
    {
      string defaultResult1 = "CHOOSE WHICH TYPE OF VOICE YOU WANT TO HEAR AND SPEAK.";
      string result;
      LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_INTRO", out result, defaultResult1);
      this.screenText.Text = result;
      string defaultResult2 = "\nPRESS OPTION 1 = HUMAN VOICES.\nPRESS OPTION 2 = MONKE VOICES.";
      LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_OPTIONS", out result, defaultResult2);
      this.screenText.Text += result;
      string defaultResult3 = "\n\nVOICE TYPE:";
      LocalisationManager.TryGetKeyForCurrentLocale("VOICE_CHAT_SCREEN_CURRENT", out result, defaultResult3);
      this.screenText.Text += result.TrailingSpace();
      string key = this.voiceChatOn == "TRUE" ? "VOICE_OPTION_HUMAN" : (this.voiceChatOn == "FALSE" ? "VOICE_OPTION_MONKE" : "VOICE_OPTION_OFF");
      string str = this.voiceChatOn == "TRUE" ? "HUMAN" : (this.voiceChatOn == "FALSE" ? "MONKE" : "OFF");
      ref string local = ref result;
      string defaultResult4 = str;
      LocalisationManager.TryGetKeyForCurrentLocale(key, out local, defaultResult4);
      this.screenText.Text += result;
    }
    else if (permissionDataByFeature.ManagedBy == KID.Model.Permission.ManagedByEnum.PROHIBITED)
      this.VoiceScreen_KIdProhibited();
    else
      this.VoiceScreen_Permission();
  }

  private void AutomuteScreen()
  {
    string defaultResult1 = "AUTOMOD AUTOMATICALLY MUTES PLAYERS WHEN THEY JOIN YOUR ROOM IF A LOT OF OTHER PLAYERS HAVE MUTED THEM";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_INTRO", out result, defaultResult1);
    this.screenText.Text = result;
    string defaultResult2 = "\nPRESS OPTION 1 FOR AGGRESSIVE MUTING\nPRESS OPTION 2 FOR MODERATE MUTING\nPRESS OPTION 3 TO TURN AUTOMOD OFF";
    LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_OPTIONS", out result, defaultResult2);
    this.screenText.Text += result;
    string defaultResult3 = "\n\nCURRENT AUTOMOD LEVEL: ";
    LocalisationManager.TryGetKeyForCurrentLocale("AUTOMOD_SCREEN_CURRENT", out result, defaultResult3);
    this.screenText.Text += result.TrailingSpace();
    string key = "AUTOMOD_OFF";
    switch (this.autoMuteType)
    {
      case "OFF":
        key = "AUTOMOD_OFF";
        break;
      case "MODERATE":
        key = "AUTOMOD_MODERATE";
        break;
      case "AGGRESSIVE":
        key = "AUTOMOD_AGGRESSIVE";
        break;
    }
    LocalisationManager.TryGetKeyForCurrentLocale(key, out result, this.autoMuteType);
    this.screenText.Text += result;
  }

  private void GroupScreen()
  {
    if (this.limitOnlineScreens)
    {
      this.LimitedOnlineFunctionalityScreen();
    }
    else
    {
      string result = "";
      string str1 = this.allowedMapsToJoin.Length > 1 ? this.groupMapJoin : this.allowedMapsToJoin[0].ToUpper();
      string str2 = "";
      if (this.allowedMapsToJoin.Length > 1)
      {
        string defaultResult = "\n\nUSE NUMBER KEYS TO SELECT DESTINATION\n1: FOREST, 2: CAVE, 3: CANYON, 4: CITY, 5: CLOUDS.";
        LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_DESTINATIONS", out result, defaultResult);
        str2 = result;
      }
      string defaultResult1 = "\n\nACTIVE ZONE WILL BE:";
      LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ACTIVE_ZONES", out result, defaultResult1);
      string str3 = result.TrailingSpace() + str1 + str2;
      if (FriendshipGroupDetection.Instance.IsInParty)
      {
        GorillaNetworkJoinTrigger selectedMapJoinTrigger = this.GetSelectedMapJoinTrigger();
        string str4 = "";
        if (selectedMapJoinTrigger.CanPartyJoin())
        {
          string defaultResult2 = "\n\n<color=red>CANNOT JOIN BECAUSE YOUR GROUP IS NOT HERE</color>";
          LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_CANNOT_JOIN", out result, defaultResult2);
          str4 = result;
        }
        string defaultResult3 = "PRESS ENTER TO JOIN A PUBLIC GAME WITH YOUR FRIENDSHIP GROUP.";
        LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ENTER_PARTY", out result, defaultResult3);
        this.screenText.Text = result;
        this.screenText.Text += str3 + str4;
      }
      else
      {
        string defaultResult4 = "PRESS ENTER TO JOIN A PUBLIC GAME AND BRING EVERYONE IN THIS ROOM WITH YOU.";
        LocalisationManager.TryGetKeyForCurrentLocale("GROUP_SCREEN_ENTER_NOPARTY", out result, defaultResult4);
        this.screenText.Text = result;
        this.screenText.Text += str3;
      }
    }
  }

  private void MicScreen()
  {
    if (KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).ManagedBy == KID.Model.Permission.ManagedByEnum.PROHIBITED)
    {
      this.MicScreen_KIdProhibited();
    }
    else
    {
      bool flag1 = false;
      string str = "";
      if (Microphone.devices.Length == 0)
      {
        flag1 = true;
        str = "NO MICROPHONE DETECTED";
      }
      if (flag1)
      {
        string result;
        LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_MIC_DISABLED", out result, "MIC DISABLED: ");
        this.screenText.Text = result + str;
      }
      else
      {
        string defaultResult1 = "PRESS OPTION 1 = ALL CHAT.\nPRESS OPTION 2 = PUSH TO TALK.\nPRESS OPTION 3 = PUSH TO MUTE.";
        string result;
        LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_OPTIONS", out result, defaultResult1);
        this.screenText.Text = result;
        string defaultResult2 = "\n\nCURRENT MIC SETTING:";
        LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_CURRENT", out result, defaultResult2);
        this.screenText.Text += result.TrailingSpace();
        string key = "";
        switch (this.pttType)
        {
          case "PUSH TO MUTE":
            key = "PUSH_TO_MUTE_MIC";
            break;
          case "PUSH TO TALK":
            key = "PUSH_TO_TALK_MIC";
            break;
          case "OPEN MIC":
            key = "OPEN_MIC";
            break;
          case "ALL CHAT":
            key = "OPEN_MIC";
            break;
        }
        LocalisationManager.TryGetKeyForCurrentLocale(key, out result, this.pttType);
        this.screenText.Text += result;
        if (this.pttType == "PUSH TO MUTE")
        {
          string defaultResult3 = "- MIC IS OPEN.\n- HOLD ANY FACE BUTTON TO MUTE.\n\n";
          LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_PUSH_TO_MUTE_TOOLTIP", out result, defaultResult3);
          this.screenText.Text += result;
        }
        else if (this.pttType == "PUSH TO TALK")
        {
          string defaultResult4 = "- MIC IS MUTED.\n- HOLD ANY FACE BUTTON TO TALK.\n\n";
          LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_PUSH_TO_TALK_TOOLTIP", out result, defaultResult4);
          this.screenText.Text += result;
        }
        else
          this.screenText.Text += "\n\n\n";
        if ((UnityEngine.Object) this.speakerLoudness == (UnityEngine.Object) null)
          this.speakerLoudness = GorillaTagger.Instance.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
        if (!((UnityEngine.Object) this.speakerLoudness != (UnityEngine.Object) null))
          return;
        float num1 = Mathf.Sqrt(this.speakerLoudness.LoudnessNormalized);
        Debug.Log((object) ("Loudness: " + num1.ToString()));
        if ((double) num1 <= 0.0099999997764825821)
          this.micInputTestTimer += this.deltaTime;
        else
          this.micInputTestTimer = 0.0f;
        if (this.pttType != "OPEN MIC")
        {
          int num2 = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand) ? 1 : 0;
          bool flag2 = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
          bool flag3 = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
          bool flag4 = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
          int num3 = flag2 ? 1 : 0;
          bool flag5 = (num2 | num3 | (flag3 ? 1 : 0) | (flag4 ? 1 : 0)) != 0;
          if (flag5 && this.pttType == "PUSH TO MUTE")
          {
            string defaultResult5 = "INPUT TEST: ";
            LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out result, defaultResult5);
            this.screenText.Text += result;
            return;
          }
          if (!flag5 && this.pttType == "PUSH TO TALK")
          {
            string defaultResult6 = "INPUT TEST: ";
            LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out result, defaultResult6);
            this.screenText.Text += result;
            return;
          }
        }
        if ((double) this.micInputTestTimer >= (double) this.micInputTestTimerThreshold)
        {
          string defaultResult7 = "NO MIC INPUT DETECTED. CHECK MIC SETTINGS IN THE OPERATING SYSTEM.";
          LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_NO_MIC", out result, defaultResult7);
          this.screenText.Text += result;
        }
        else
        {
          string defaultResult8 = "INPUT TEST: ";
          LocalisationManager.TryGetKeyForCurrentLocale("MIC_SCREEN_INPUT_TEST_LABEL", out result, defaultResult8);
          this.screenText.Text += result;
          for (int index = 0; index < Mathf.FloorToInt(num1 * 50f); ++index)
            this.screenText.Text += "|";
        }
      }
    }
  }

  private void QueueScreen()
  {
    if (this.limitOnlineScreens)
    {
      this.LimitedOnlineFunctionalityScreen();
    }
    else
    {
      string defaultResult1 = "THIS OPTION AFFECTS WHO YOU PLAY WITH. DEFAULT IS FOR ANYONE TO PLAY NORMALLY. MINIGAMES IS FOR PEOPLE LOOKING TO PLAY WITH THEIR OWN MADE UP RULES.";
      string result;
      LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN", out result, defaultResult1);
      this.screenText.Text = result.TrailingSpace();
      if (this.allowedInCompetitive)
      {
        string defaultResult2 = "COMPETITIVE IS FOR PLAYERS WHO WANT TO PLAY THE GAME AND TRY AS HARD AS THEY CAN.";
        LocalisationManager.TryGetKeyForCurrentLocale("COMPETITIVE_DESC", out result, defaultResult2);
        this.screenText.Text += result.TrailingSpace();
        string defaultResult3 = "PRESS OPTION 1 FOR DEFAULT, OPTION 2 FOR MINIGAMES, OR OPTION 3 FOR COMPETITIVE.";
        LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN_ALL_QUEUES", out result, defaultResult3);
        this.screenText.Text += result;
      }
      else
      {
        string defaultResult4 = "BEAT THE OBSTACLE COURSE IN CITY TO ALLOW COMPETITIVE PLAY.";
        LocalisationManager.TryGetKeyForCurrentLocale("BEAT_OBSTACLE_COURSE", out result, defaultResult4);
        this.screenText.Text += result.TrailingSpace();
        string defaultResult5 = "PRESS OPTION 1 FOR DEFAULT, OR OPTION 2 FOR MINIGAMES.";
        LocalisationManager.TryGetKeyForCurrentLocale("QUEUE_SCREEN_DEFAULT_QUEUES", out result, defaultResult5);
        this.screenText.Text += result;
      }
      string defaultResult6 = "\n\nCURRENT QUEUE:";
      LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_QUEUE", out result, defaultResult6);
      this.screenText.Text += result.TrailingSpace();
      string key;
      switch (this.currentQueue)
      {
        case "COMPETITIVE":
          key = "COMPETITIVE_QUEUE";
          break;
        case "MINIGAMES":
          key = "MINIGAMES_QUEUE";
          break;
        default:
          key = "DEFAULT_QUEUE";
          break;
      }
      string currentQueue = this.currentQueue;
      LocalisationManager.TryGetKeyForCurrentLocale(key, out result, currentQueue);
      this.screenText.Text += result;
    }
  }

  private void TroopScreen()
  {
    if (this.limitOnlineScreens)
    {
      this.LimitedOnlineFunctionalityScreen();
    }
    else
    {
      KID.Model.Permission permissionDataByFeature1 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
      KID.Model.Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
      bool flag1 = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer);
      bool flag2 = this.IsValidTroopName(this.troopName);
      this.screenText.Text = string.Empty;
      string result1 = "";
      if (flag1)
      {
        string defaultResult1 = "PLAY WITH A PERSISTENT GROUP ACROSS MULTIPLE ROOMS.";
        LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_INTRO", out result1, defaultResult1);
        this.screenText.Text = result1;
        if (!flag2)
        {
          string defaultResult2 = " PRESS ENTER TO JOIN OR CREATE A TROOP.";
          LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_INSTRUCTIONS", out result1, defaultResult2);
          this.screenText.Text += result1;
        }
      }
      string defaultResult3 = "\n\nCURRENT TROOP: ";
      LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_CURRENT_TROOP", out result1, defaultResult3);
      this.screenText.Text += result1.TrailingSpace();
      if (flag2)
      {
        this.screenText.Text += this.troopName;
        if (flag1)
        {
          bool flag3 = this.currentTroopPopulation > -1;
          if (this.troopQueueActive)
          {
            string defaultResult4 = "\n  -IN TROOP QUEUE-";
            LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_IN_QUEUE", out result1, defaultResult4);
            this.screenText.Text += result1;
            if (flag3)
            {
              string defaultResult5 = "\n\nPLAYERS IN TROOP: ";
              LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_PLAYERS_IN_TROOP", out result1, defaultResult5);
              this.screenText.Text += result1.TrailingSpace();
              this.screenText.Text += Mathf.Max(1, this.currentTroopPopulation).ToString();
            }
            string defaultResult6 = "\n\nPRESS OPTION 2 FOR DEFAULT QUEUE.";
            LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_DEFAULT_QUEUE", out result1, defaultResult6);
            this.screenText.Text += result1;
          }
          else
          {
            string defaultResult7 = "\n  -IN {currentQueue} QUEUE-";
            LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_CURRENT_QUEUE", out result1, defaultResult7);
            string key;
            switch (this.currentQueue)
            {
              case "MINIGAMES":
                key = "MINIGAMES_QUEUE";
                break;
              case "COMPETITIVE":
                key = "COMPETITIVE_QUEUE";
                break;
              default:
                key = "DEFAULT_QUEUE";
                break;
            }
            string currentQueue = this.currentQueue;
            string result2;
            LocalisationManager.TryGetKeyForCurrentLocale(key, out result2, currentQueue);
            result1 = result1.Replace("{currentQueue}", result2);
            this.screenText.Text += result1;
            if (flag3)
            {
              string defaultResult8 = "\n\nPLAYERS IN TROOP: ";
              LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_PLAYERS_IN_TROOP", out result1, defaultResult8);
              this.screenText.Text += result1.TrailingSpace();
              this.screenText.Text += Mathf.Max(1, this.currentTroopPopulation).ToString();
            }
            string defaultResult9 = "\n\nPRESS OPTION 1 FOR TROOP QUEUE.";
            LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_TROOP_QUEUE", out result1, defaultResult9);
            this.screenText.Text += result1;
          }
          string defaultResult10 = "\nPRESS OPTION 3 TO LEAVE YOUR TROOP.";
          LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_LEAVE", out result1, defaultResult10);
          this.screenText.Text += result1;
        }
      }
      else
      {
        string defaultResult11 = "-NOT IN TROOP-";
        LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_NOT_IN_TROOP", out result1, defaultResult11);
        this.screenText.Text += result1;
      }
      if (flag1)
      {
        if (flag2)
          return;
        string defaultResult12 = "\n\nTROOP TO JOIN: ";
        LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_JOIN_TROOP", out result1, defaultResult12);
        this.screenText.Text += result1.TrailingSpace();
        this.screenText.Text += this.troopToJoin;
      }
      else if (permissionDataByFeature1.ManagedBy == KID.Model.Permission.ManagedByEnum.PROHIBITED || permissionDataByFeature2.ManagedBy == KID.Model.Permission.ManagedByEnum.PROHIBITED)
        this.TroopScreen_KIdProhibited();
      else
        this.TroopScreen_Permission();
    }
  }

  private void TurnScreen()
  {
    string defaultResult1 = "PRESS OPTION 1 TO USE SNAP TURN. PRESS OPTION 2 TO USE SMOOTH TURN. PRESS OPTION 3 TO USE NO ARTIFICIAL TURNING.";
    string str1 = "";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN", out result, defaultResult1);
    string str2 = str1 + result.TrailingSpace();
    string defaultResult2 = "PRESS THE NUMBER KEYS TO CHOOSE A TURNING SPEED.";
    LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURNING_SPEED", out result, defaultResult2);
    string str3 = str2 + result;
    string defaultResult3 = "\n CURRENT TURN TYPE: ";
    LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURN_TYPE", out result, defaultResult3);
    string str4 = str3 + result;
    string key = "TURN_TYPE_NO_TURN";
    switch (GorillaSnapTurn.CachedSnapTurnRef.turnType)
    {
      case "SNAP":
        key = "TURN_TYPE_SNAP_TURN";
        break;
      case "SMOOTH":
        key = "TURN_TYPE_SMOOTH_TURN";
        break;
      case "NONE":
        key = "TURN_TYPE_NO_TURN";
        break;
      default:
        Debug.LogError((object) $"[LOCALIZATION::GORILLA_COMPUTER::TURN] Could not match [{GorillaSnapTurn.CachedSnapTurnRef.turnType}] to any case. Defaulting to NO_TURN");
        break;
    }
    LocalisationManager.TryGetKeyForCurrentLocale(key, out result, GorillaSnapTurn.CachedSnapTurnRef.turnType);
    string str5 = str4 + result;
    string defaultResult4 = "\nCURRENT TURN SPEED: ";
    LocalisationManager.TryGetKeyForCurrentLocale("TURN_SCREEN_TURN_SPEED", out result, defaultResult4);
    this.screenText.Text = str5 + result + GorillaSnapTurn.CachedSnapTurnRef.turnFactor.ToString();
  }

  private void NameScreen()
  {
    KID.Model.Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
    if (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
    {
      string defaultResult1 = "PRESS ENTER TO CHANGE YOUR NAME TO THE ENTERED NEW NAME.\n\n";
      string result;
      LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN", out result, defaultResult1);
      this.screenText.Text = result;
      string defaultResult2 = "CURRENT NAME: ";
      LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_NAME", out result, defaultResult2);
      this.screenText.Text += result.TrailingSpace();
      this.screenText.Text += this.savedName;
      if (this.NametagsEnabled)
      {
        string defaultResult3 = "NEW NAME: ";
        LocalisationManager.TryGetKeyForCurrentLocale("NEW_NAME", out result, defaultResult3);
        this.screenText.Text += result.TrailingSpace();
        this.screenText.Text += this.currentName;
      }
      string defaultResult4 = "PRESS OPTION 1 TO TOGGLE NAMETAGS.\nCURRENTLY NAMETAGS ARE: ";
      LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_TOGGLE_NAMETAGS", out result, defaultResult4);
      string key = this.NametagsEnabled ? "ON_KEY" : "OFF_KEY";
      this.screenText.Text += result.TrailingSpace();
      string str = this.NametagsEnabled ? "ON" : "OFF";
      ref string local = ref result;
      string defaultResult5 = str;
      LocalisationManager.TryGetKeyForCurrentLocale(key, out local, defaultResult5);
      this.screenText.Text += result;
    }
    else if (permissionDataByFeature.ManagedBy == KID.Model.Permission.ManagedByEnum.PROHIBITED)
      this.NameScreen_KIdProhibited();
    else
      this.NameScreen_Permission();
  }

  private void StartupScreen()
  {
    string defaultResult = string.Empty;
    if (KIDManager.GetActiveAccountStatus() == AgeStatusType.DIGITALMINOR)
    {
      defaultResult = "YOU ARE PLAYING ON A MANAGED ACCOUNT. SOME SETTINGS MAY BE DISABLED WITHOUT PARENT OR GUARDIAN APPROVAL\n\n";
      string result;
      if (LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_MANAGED", out result, defaultResult))
        defaultResult = result;
    }
    string empty = string.Empty;
    string result1;
    LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_INTRO", out result1, "GORILLA OS\n\n");
    this.screenText.Text = result1;
    this.screenText.Text += defaultResult;
    LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_PLAYERS_ONLINE", out result1, "{playersOnline} PLAYERS ONLINE\n\n");
    this.screenText.Text += result1.Replace("{playersOnline}", HowManyMonke.ThisMany.ToString());
    LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_USERS_BANNED", out result1, "{usersBanned} USERS BANNED YESTERDAY\n\n");
    this.screenText.Text += result1.Replace("{usersBanned}", this.usersBanned.ToString());
    LocalisationManager.TryGetKeyForCurrentLocale("STARTUP_PRESS_KEY", out result1, "PRESS ANY KEY TO BEGIN");
    this.screenText.Text += result1;
  }

  private void ColourScreen()
  {
    this.screenText.Text = "USE THE OPTIONS BUTTONS TO SELECT THE COLOR TO UPDATE, THEN PRESS 0-9 TO SET A NEW VALUE.";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("COLOR_SELECT_INTRO", out result, this.screenText.Text);
    this.screenText.Text += result;
    LocalisationManager.TryGetKeyForCurrentLocale("COLOR_RED", out result, this.screenText.Text);
    this.screenText.Text += "\n\n";
    this.screenText.Text += result;
    GorillaText screenText1 = this.screenText;
    string text1 = screenText1.Text;
    int num = Mathf.FloorToInt(this.redValue * 9f);
    string str1 = num.ToString();
    string str2 = this.colorCursorLine == 0 ? "<--" : "";
    screenText1.Text = text1 + str1 + str2;
    LocalisationManager.TryGetKeyForCurrentLocale("COLOR_GREEN", out result, this.screenText.Text);
    this.screenText.Text += "\n\n";
    this.screenText.Text += result;
    GorillaText screenText2 = this.screenText;
    string text2 = screenText2.Text;
    num = Mathf.FloorToInt(this.greenValue * 9f);
    string str3 = num.ToString();
    string str4 = this.colorCursorLine == 1 ? "<--" : "";
    screenText2.Text = text2 + str3 + str4;
    LocalisationManager.TryGetKeyForCurrentLocale("COLOR_BLUE", out result, this.screenText.Text);
    this.screenText.Text += "\n\n";
    this.screenText.Text += result;
    GorillaText screenText3 = this.screenText;
    string text3 = screenText3.Text;
    num = Mathf.FloorToInt(this.blueValue * 9f);
    string str5 = num.ToString();
    string str6 = this.colorCursorLine == 2 ? "<--" : "";
    screenText3.Text = text3 + str5 + str6;
  }

  private void RoomScreen()
  {
    if (this.limitOnlineScreens)
    {
      this.LimitedOnlineFunctionalityScreen();
    }
    else
    {
      KID.Model.Permission permissionDataByFeature1 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Groups);
      KID.Model.Permission permissionDataByFeature2 = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Multiplayer);
      bool optedInPreviously = KIDManager.CheckFeatureOptIn(EKIDFeatures.Multiplayer).hasOptedInPreviously;
      int num = (!KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) ? 0 : (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer) ? 1 : 0)) & (optedInPreviously ? 1 : 0);
      this.screenText.Text = "";
      string result = "";
      if (num != 0)
      {
        string defaultResult = "PRESS ENTER TO JOIN OR CREATE A CUSTOM ROOM WITH THE ENTERED CODE.";
        LocalisationManager.TryGetKeyForCurrentLocale("ROOM_INTRO", out result, defaultResult);
        this.screenText.Text += result.TrailingSpace();
      }
      string defaultResult1 = "PRESS OPTION 1 TO DISCONNECT FROM THE CURRENT ROOM.";
      LocalisationManager.TryGetKeyForCurrentLocale("ROOM_OPTION", out result, defaultResult1);
      this.screenText.Text += result.TrailingSpace();
      if (FriendshipGroupDetection.Instance.IsInParty)
      {
        if (FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider))
        {
          string defaultResult2 = "YOUR GROUP WILL TRAVEL WITH YOU.";
          LocalisationManager.TryGetKeyForCurrentLocale("ROOM_GROUP_TRAVEL", out result, defaultResult2);
          this.screenText.Text += result.TrailingSpace();
        }
        else
        {
          string defaultResult3 = "<color=red>YOU WILL LEAVE YOUR PARTY UNLESS YOU GATHER THEM HERE FIRST!</color> ";
          LocalisationManager.TryGetKeyForCurrentLocale("ROOM_PARTY_WARNING", out result, defaultResult3);
          this.screenText.Text += result;
        }
      }
      string defaultResult4 = "\n\nCURRENT ROOM:";
      LocalisationManager.TryGetKeyForCurrentLocale("ROOM_TEXT_CURRENT_ROOM", out result, defaultResult4);
      this.screenText.Text += result.TrailingSpace();
      if (NetworkSystem.Instance.InRoom)
      {
        this.screenText.Text += NetworkSystem.Instance.RoomName.TrailingSpace();
        if (NetworkSystem.Instance.SessionIsPrivate)
        {
          string str = GameMode.ActiveGameMode?.GameModeNameRoomLabel();
          if (!string.IsNullOrEmpty(str))
            this.screenText.Text += str;
        }
        string defaultResult5 = "\n\nPLAYERS IN ROOM:";
        LocalisationManager.TryGetKeyForCurrentLocale("PLAYERS_IN_ROOM", out result, defaultResult5);
        this.screenText.Text += result.TrailingSpace();
        this.screenText.Text += NetworkSystem.Instance.RoomPlayerCount.ToString();
      }
      else
      {
        string defaultResult6 = "-NOT IN ROOM-";
        LocalisationManager.TryGetKeyForCurrentLocale("NOT_IN_ROOM", out result, defaultResult6);
        this.screenText.Text += result;
        string defaultResult7 = "\n\nPLAYERS ONLINE:";
        LocalisationManager.TryGetKeyForCurrentLocale("PLAYERS_ONLINE", out result, defaultResult7);
        this.screenText.Text += result.TrailingSpace();
        this.screenText.Text += HowManyMonke.ThisMany.ToString();
      }
      if (num != 0)
      {
        string defaultResult8 = "\n\nROOM TO JOIN:";
        LocalisationManager.TryGetKeyForCurrentLocale("ROOM_TO_JOIN", out result, defaultResult8);
        this.screenText.Text += result.TrailingSpace();
        this.screenText.Text += this.roomToJoin;
        if (this.roomFull)
        {
          string defaultResult9 = "\n\nROOM FULL. JOIN ROOM FAILED.";
          LocalisationManager.TryGetKeyForCurrentLocale("ROOM_FULL", out result, defaultResult9);
          this.screenText.Text += result;
        }
        else
        {
          if (!this.roomNotAllowed)
            return;
          string defaultResult10 = "\n\nCANNOT JOIN ROOM TYPE FROM HERE.";
          LocalisationManager.TryGetKeyForCurrentLocale("ROOM_JOIN_NOT_ALLOWED", out result, defaultResult10);
          this.screenText.Text += result;
        }
      }
      else if (permissionDataByFeature1.ManagedBy == KID.Model.Permission.ManagedByEnum.PROHIBITED || permissionDataByFeature2.ManagedBy == KID.Model.Permission.ManagedByEnum.PROHIBITED)
        this.RoomScreen_KIdProhibited();
      else
        this.RoomScreen_Permission();
    }
  }

  private void RedemptionScreen()
  {
    string defaultResult1 = "TYPE REDEMPTION CODE AND PRESS ENTER";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_INTRO", out result, defaultResult1);
    this.screenText.Text = result;
    string defaultResult2 = "\n\nCODE: " + this.redemptionCode;
    LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_LABEL", out result, defaultResult2);
    this.screenText.Text += result.TrailingSpace();
    this.screenText.Text += this.redemptionCode;
    switch (this.RedemptionStatus)
    {
      case GorillaComputer.RedemptionResult.Invalid:
        string defaultResult3 = "\n\nINVALID CODE";
        LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_INVALID", out result, defaultResult3);
        this.screenText.Text += result;
        break;
      case GorillaComputer.RedemptionResult.Checking:
        string defaultResult4 = "\n\nVALIDATING...";
        LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_VALIDATING", out result, defaultResult4);
        this.screenText.Text += result;
        break;
      case GorillaComputer.RedemptionResult.AlreadyUsed:
        string defaultResult5 = "\n\nCODE ALREADY CLAIMED";
        LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_ALREADY_USED", out result, defaultResult5);
        this.screenText.Text += result;
        break;
      case GorillaComputer.RedemptionResult.Success:
        string defaultResult6 = "\n\nSUCCESSFULLY CLAIMED!";
        LocalisationManager.TryGetKeyForCurrentLocale("REDEMPTION_CODE_SUCCESS", out result, defaultResult6);
        this.screenText.Text += result;
        break;
    }
  }

  private void LimitedOnlineFunctionalityScreen()
  {
    string defaultResult = "NOT AVAILABLE IN RANKED PLAY";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("LIMITED_ONLINE_FUNC", out result, defaultResult);
    this.screenText.Text = result;
  }

  private void UpdateGameModeText()
  {
    string defaultResult1 = "CURRENT MODE";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("CURRENT_MODE", out result, defaultResult1);
    this.currentGameModeText.Value = result;
    if (!NetworkSystem.Instance.InRoom || (UnityEngine.Object) GorillaGameManager.instance == (UnityEngine.Object) null)
    {
      string defaultResult2 = "-NOT IN ROOM-";
      LocalisationManager.TryGetKeyForCurrentLocale("NOT_IN_ROOM", out result, defaultResult2);
      this.currentGameModeText.Value += result;
    }
    else
    {
      WatchableStringSO currentGameModeText = this.currentGameModeText;
      currentGameModeText.Value = $"{currentGameModeText.Value}\n{GorillaGameManager.instance.GameModeName()}";
    }
  }

  private void UpdateFunctionScreen()
  {
    this.functionSelectText.Text = this.GetOrderListForScreen(this.currentState);
  }

  private void CheckAutoBanListForRoomName(string nameToCheck)
  {
    this.SwitchToLoadingState();
    this.CheckForBadRoomName(nameToCheck);
  }

  private void CheckAutoBanListForPlayerName(string nameToCheck)
  {
    this.SwitchToLoadingState();
    this.CheckForBadPlayerName(nameToCheck);
  }

  private void CheckAutoBanListForTroopName(string nameToCheck)
  {
    if (!this.IsValidTroopName(this.troopToJoin))
      return;
    this.SwitchToLoadingState();
    this.CheckForBadTroopName(nameToCheck);
  }

  private void CheckForBadRoomName(string nameToCheck)
  {
    GorillaServer instance = GorillaServer.Instance;
    CheckForBadNameRequest request = new CheckForBadNameRequest();
    request.name = nameToCheck;
    request.forRoom = true;
    request.forTroop = false;
    Action<ExecuteFunctionResult> successCallback = new Action<ExecuteFunctionResult>(this.OnRoomNameChecked);
    Action<PlayFabError> errorCallback = new Action<PlayFabError>(this.OnErrorNameCheck);
    instance.CheckForBadName(request, successCallback, errorCallback);
  }

  private void CheckForBadPlayerName(string nameToCheck)
  {
    GorillaServer instance = GorillaServer.Instance;
    CheckForBadNameRequest request = new CheckForBadNameRequest();
    request.name = nameToCheck;
    request.forRoom = false;
    request.forTroop = false;
    Action<ExecuteFunctionResult> successCallback = new Action<ExecuteFunctionResult>(this.OnPlayerNameChecked);
    Action<PlayFabError> errorCallback = new Action<PlayFabError>(this.OnErrorNameCheck);
    instance.CheckForBadName(request, successCallback, errorCallback);
  }

  private void CheckForBadTroopName(string nameToCheck)
  {
    GorillaServer instance = GorillaServer.Instance;
    CheckForBadNameRequest request = new CheckForBadNameRequest();
    request.name = nameToCheck;
    request.forRoom = false;
    request.forTroop = true;
    Action<ExecuteFunctionResult> successCallback = new Action<ExecuteFunctionResult>(this.OnTroopNameChecked);
    Action<PlayFabError> errorCallback = new Action<PlayFabError>(this.OnErrorNameCheck);
    instance.CheckForBadName(request, successCallback, errorCallback);
  }

  private void OnRoomNameChecked(ExecuteFunctionResult result)
  {
    object obj;
    if (((JsonObject) result.FunctionResult).TryGetValue(nameof (result), out obj))
    {
      switch (int.Parse(obj.ToString()))
      {
        case 0:
          if (FriendshipGroupDetection.Instance.IsInParty && !FriendshipGroupDetection.Instance.IsPartyWithinCollider(this.friendJoinCollider))
            FriendshipGroupDetection.Instance.LeaveParty();
          if (this.playerInVirtualStump)
            CustomMapManager.UnloadMap(false);
          this.networkController.AttemptToJoinSpecificRoom(this.roomToJoin, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
          break;
        case 1:
          this.roomToJoin = "";
          this.roomToJoin += this.playerInVirtualStump ? this.virtualStumpRoomPrepend : "";
          this.SwitchToWarningState();
          break;
        case 2:
          this.roomToJoin = "";
          this.roomToJoin += this.playerInVirtualStump ? this.virtualStumpRoomPrepend : "";
          GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
          break;
      }
    }
    if (this.currentState != GorillaComputer.ComputerState.Loading)
      return;
    this.PopState();
  }

  private void OnPlayerNameChecked(ExecuteFunctionResult result)
  {
    object obj;
    if (((JsonObject) result.FunctionResult).TryGetValue(nameof (result), out obj))
    {
      switch (int.Parse(obj.ToString()))
      {
        case 0:
          NetworkSystem.Instance.SetMyNickName(this.currentName);
          CustomMapsTerminal.RequestDriverNickNameRefresh();
          break;
        case 1:
          NetworkSystem.Instance.SetMyNickName("gorilla");
          CustomMapsTerminal.RequestDriverNickNameRefresh();
          this.currentName = "gorilla";
          this.SwitchToWarningState();
          break;
        case 2:
          NetworkSystem.Instance.SetMyNickName("gorilla");
          CustomMapsTerminal.RequestDriverNickNameRefresh();
          this.currentName = "gorilla";
          GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
          break;
      }
    }
    this.SetLocalNameTagText(this.currentName);
    this.savedName = this.currentName;
    PlayerPrefs.SetString("playerName", this.currentName);
    PlayerPrefs.Save();
    if (NetworkSystem.Instance.InRoom)
      GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, (object) this.redValue, (object) this.greenValue, (object) this.blueValue);
    if (this.currentState != GorillaComputer.ComputerState.Loading)
      return;
    this.PopState();
  }

  private void OnTroopNameChecked(ExecuteFunctionResult result)
  {
    object obj;
    if (((JsonObject) result.FunctionResult).TryGetValue(nameof (result), out obj))
    {
      switch (int.Parse(obj.ToString()))
      {
        case 0:
          this.JoinTroop(this.troopToJoin);
          break;
        case 1:
          this.troopToJoin = string.Empty;
          this.SwitchToWarningState();
          break;
        case 2:
          this.troopToJoin = string.Empty;
          GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
          break;
      }
    }
    if (this.currentState != GorillaComputer.ComputerState.Loading)
      return;
    this.PopState();
  }

  private void OnErrorNameCheck(PlayFabError error)
  {
    if (this.currentState == GorillaComputer.ComputerState.Loading)
      this.PopState();
    GorillaComputer.OnErrorShared(error);
  }

  public bool CheckAutoBanListForName(string nameToCheck)
  {
    nameToCheck = nameToCheck.ToLower();
    nameToCheck = new string(Array.FindAll<char>(nameToCheck.ToCharArray(), (Predicate<char>) (c => char.IsLetterOrDigit(c))));
    foreach (string str in this.anywhereTwoWeek)
    {
      if (nameToCheck.IndexOf(str) >= 0)
        return false;
    }
    foreach (string str in this.anywhereOneWeek)
    {
      if (nameToCheck.IndexOf(str) >= 0 && !nameToCheck.Contains("fagol"))
        return false;
    }
    foreach (string str in this.exactOneWeek)
    {
      if (str == nameToCheck)
        return false;
    }
    return true;
  }

  public void UpdateColor(float red, float green, float blue)
  {
    this.redValue = Mathf.Clamp(red, 0.0f, 1f);
    this.greenValue = Mathf.Clamp(green, 0.0f, 1f);
    this.blueValue = Mathf.Clamp(blue, 0.0f, 1f);
  }

  public void UpdateFailureText(string failMessage)
  {
    GorillaScoreboardTotalUpdater.instance.SetOfflineFailureText(failMessage);
    PhotonNetworkController.Instance.UpdateTriggerScreens();
    this.screenText.EnableFailedState(failMessage);
    this.functionSelectText.EnableFailedState(failMessage);
  }

  private void RestoreFromFailureState()
  {
    GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
    PhotonNetworkController.Instance.UpdateTriggerScreens();
    this.screenText.DisableFailedState();
    this.functionSelectText.DisableFailedState();
  }

  public void GeneralFailureMessage(string failMessage)
  {
    this.isConnectedToMaster = false;
    NetworkSystem.Instance.SetWrongVersion();
    this.UpdateFailureText(failMessage);
    this.UpdateScreen();
  }

  private static void OnErrorShared(PlayFabError error)
  {
    if (error.Error == PlayFabErrorCode.NotAuthenticated)
      PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
    else if (error.Error == PlayFabErrorCode.AccountBanned)
      GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
    if (error.ErrorMessage == "The account making this request is currently banned")
    {
      using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        KeyValuePair<string, List<string>> current = enumerator.Current;
        if (current.Value[0] != "Indefinite")
          GorillaComputer.instance.GeneralFailureMessage($"YOUR ACCOUNT {PlayFabAuthenticator.instance.GetPlayFabPlayerId()} HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: {current.Key}\nHOURS LEFT: {((int) ((DateTime.Parse(current.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString()}");
        else
          GorillaComputer.instance.GeneralFailureMessage($"YOUR ACCOUNT {PlayFabAuthenticator.instance.GetPlayFabPlayerId()} HAS BEEN BANNED INDEFINITELY.\nREASON: {current.Key}");
      }
    }
    else
    {
      if (!(error.ErrorMessage == "The IP making this request is currently banned"))
        return;
      using (Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        KeyValuePair<string, List<string>> current = enumerator.Current;
        if (current.Value[0] != "Indefinite")
          GorillaComputer.instance.GeneralFailureMessage($"THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: {current.Key}\nHOURS LEFT: {((int) ((DateTime.Parse(current.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString()}");
        else
          GorillaComputer.instance.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + current.Key);
      }
    }
  }

  private void DecreaseState()
  {
    --this.currentStateIndex;
    if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
      --this.currentStateIndex;
    if (this.currentStateIndex < 0)
      this.currentStateIndex = this.FunctionsCount - 1;
    this.SwitchState(this.GetState(this.currentStateIndex));
  }

  private void IncreaseState()
  {
    ++this.currentStateIndex;
    if (this.GetState(this.currentStateIndex) == GorillaComputer.ComputerState.Time)
      ++this.currentStateIndex;
    if (this.currentStateIndex >= this.FunctionsCount)
      this.currentStateIndex = 0;
    this.SwitchState(this.GetState(this.currentStateIndex));
  }

  public GorillaComputer.ComputerState GetState(int index)
  {
    try
    {
      return this._activeOrderList[index].State;
    }
    catch
    {
      return this._activeOrderList[0].State;
    }
  }

  public int GetStateIndex(GorillaComputer.ComputerState state)
  {
    return this._activeOrderList.FindIndex((Predicate<GorillaComputer.StateOrderItem>) (s => s.State == state));
  }

  public string GetOrderListForScreen(GorillaComputer.ComputerState currentState)
  {
    StringBuilder stringBuilder = new StringBuilder();
    int stateIndex = this.GetStateIndex(currentState);
    for (int index = 0; index < this.FunctionsCount; ++index)
    {
      stringBuilder.Append(this.FunctionNames[index]);
      if (index == stateIndex)
        stringBuilder.Append(this.Pointer);
      if (index < this.FunctionsCount - 1)
        stringBuilder.Append("\n");
    }
    return stringBuilder.ToString();
  }

  private void GetCurrentTime()
  {
    this.tryGetTimeAgain = true;
    PlayFabClientAPI.GetTime(new GetTimeRequest(), new Action<GetTimeResult>(this.OnGetTimeSuccess), new Action<PlayFabError>(this.OnGetTimeFailure));
  }

  private void OnGetTimeSuccess(GetTimeResult result)
  {
    this.startupMillis = (long) (TimeSpan.FromTicks(result.Time.Ticks).TotalMilliseconds - (double) Time.realtimeSinceStartup * 1000.0);
    this.startupTime = result.Time - TimeSpan.FromSeconds((double) Time.realtimeSinceStartup);
    Action serverTimeUpdated = this.OnServerTimeUpdated;
    if (serverTimeUpdated == null)
      return;
    serverTimeUpdated();
  }

  private void OnGetTimeFailure(PlayFabError error)
  {
    this.startupMillis = (long) (TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - (double) Time.realtimeSinceStartup * 1000.0);
    this.startupTime = DateTime.UtcNow - TimeSpan.FromSeconds((double) Time.realtimeSinceStartup);
    Action serverTimeUpdated = this.OnServerTimeUpdated;
    if (serverTimeUpdated != null)
      serverTimeUpdated();
    if (error.Error == PlayFabErrorCode.NotAuthenticated)
    {
      PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
    }
    else
    {
      if (error.Error != PlayFabErrorCode.AccountBanned)
        return;
      GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
    }
  }

  private void PlayerCountChangedCallback(NetPlayer player) => this.UpdateScreen();

  public void SetNameBySafety(bool isSafety)
  {
    if (!isSafety)
      return;
    PlayerPrefs.SetString("playerNameBackup", this.currentName);
    this.currentName = "gorilla" + UnityEngine.Random.Range(0, 9999).ToString().PadLeft(4, '0');
    this.savedName = this.currentName;
    NetworkSystem.Instance.SetMyNickName(this.currentName);
    this.SetLocalNameTagText(this.currentName);
    PlayerPrefs.SetString("playerName", this.currentName);
    PlayerPrefs.Save();
    if (!NetworkSystem.Instance.InRoom)
      return;
    GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, (object) this.redValue, (object) this.greenValue, (object) this.blueValue);
  }

  public void SetLocalNameTagText(string newName) => VRRig.LocalRig.SetNameTagText(newName);

  public void SetComputerSettingsBySafety(
    bool isSafety,
    GorillaComputer.ComputerState[] toFilterOut,
    bool shouldHide)
  {
    this._activeOrderList = this.OrderList;
    if (!isSafety)
    {
      this._activeOrderList = this.OrderList;
      if (this._filteredStates.Count > 0 && toFilterOut.Length != 0)
      {
        for (int index = 0; index < toFilterOut.Length; ++index)
        {
          if (this._filteredStates.Contains(toFilterOut[index]))
            this._filteredStates.Remove(toFilterOut[index]);
        }
      }
    }
    else if (shouldHide)
    {
      for (int index = 0; index < toFilterOut.Length; ++index)
      {
        if (!this._filteredStates.Contains(toFilterOut[index]))
          this._filteredStates.Add(toFilterOut[index]);
      }
    }
    if (this._filteredStates.Count > 0)
    {
      int index = 0;
      for (int count = this._activeOrderList.Count; index < count; ++index)
      {
        if (this._filteredStates.Contains(this._activeOrderList[index].State))
        {
          this._activeOrderList.RemoveAt(index);
          --index;
          --count;
        }
      }
    }
    this.FunctionsCount = this._activeOrderList.Count;
    this.FunctionNames.Clear();
    this._activeOrderList.ForEach((Action<GorillaComputer.StateOrderItem>) (s =>
    {
      string name = s.GetName();
      if (name.Length > this.highestCharacterCount)
        this.highestCharacterCount = name.Length;
      this.FunctionNames.Add(name);
    }));
    for (int index1 = 0; index1 < this.FunctionsCount; ++index1)
    {
      int num = this.highestCharacterCount - this.FunctionNames[index1].Length;
      for (int index2 = 0; index2 < num; ++index2)
        this.FunctionNames[index1] += " ";
    }
    this.UpdateScreen();
  }

  public void KID_SetVoiceChatSettingOnStart(
    bool voiceChatEnabled,
    KID.Model.Permission.ManagedByEnum managedBy,
    bool hasOptedInPreviously)
  {
    if (managedBy == KID.Model.Permission.ManagedByEnum.PROHIBITED)
      return;
    this.SetVoice(voiceChatEnabled, !hasOptedInPreviously);
  }

  private void SetVoice(bool setting, bool saveSetting = true)
  {
    this.voiceChatOn = setting ? "TRUE" : "FALSE";
    if (setting && !KIDManager.CheckFeatureOptIn(EKIDFeatures.Voice_Chat).hasOptedInPreviously)
    {
      KIDManager.SetFeatureOptIn(EKIDFeatures.Voice_Chat, true);
      KIDManager.SendOptInPermissions();
    }
    if (!saveSetting)
      return;
    PlayerPrefs.SetString("voiceChatOn", this.voiceChatOn);
    PlayerPrefs.Save();
  }

  public bool CheckVoiceChatEnabled() => this.voiceChatOn == "TRUE";

  private void SetVoiceChatBySafety(bool voiceChatEnabled, KID.Model.Permission.ManagedByEnum managedBy)
  {
    this.SetComputerSettingsBySafety((!voiceChatEnabled ? 1 : 0) != 0, new GorillaComputer.ComputerState[3]
    {
      GorillaComputer.ComputerState.Voice,
      GorillaComputer.ComputerState.AutoMute,
      GorillaComputer.ComputerState.Mic
    }, false);
    string str = PlayerPrefs.GetString("voiceChatOn", "");
    if (KIDManager.KidEnabledAndReady)
    {
      KID.Model.Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat);
      if (permissionDataByFeature != null)
      {
        (bool requiresOptIn, bool hasOptedInPreviously) tuple = KIDManager.CheckFeatureOptIn(EKIDFeatures.Voice_Chat, permissionDataByFeature);
        if (tuple.requiresOptIn && !tuple.hasOptedInPreviously)
          str = "FALSE";
      }
      else
        Debug.LogErrorFormat($"[KID] Could not find permission data for [{EKIDFeatures.Voice_Chat.ToStandardisedString()}]");
    }
    switch (managedBy)
    {
      case KID.Model.Permission.ManagedByEnum.PLAYER:
        this.voiceChatOn = !string.IsNullOrEmpty(str) ? str : (voiceChatEnabled ? "TRUE" : "FALSE");
        break;
      case KID.Model.Permission.ManagedByEnum.GUARDIAN:
        this.voiceChatOn = !KIDManager.GetPermissionDataByFeature(EKIDFeatures.Voice_Chat).Enabled ? "FALSE" : (!string.IsNullOrEmpty(str) ? str : "TRUE");
        break;
      case KID.Model.Permission.ManagedByEnum.PROHIBITED:
        this.voiceChatOn = "FALSE";
        break;
    }
    RigContainer.RefreshAllRigVoices();
    Debug.Log((object) $"[KID] On Session Update - Voice Chat Permission changed - Has enabled voiceChat? [{voiceChatEnabled.ToString()}]");
  }

  public void SetNametagSetting(
    bool setting,
    KID.Model.Permission.ManagedByEnum managedBy,
    bool hasOptedInPreviously)
  {
    switch (managedBy)
    {
      case KID.Model.Permission.ManagedByEnum.GUARDIAN:
        int num = PlayerPrefs.GetInt(this.NameTagPlayerPref, 1);
        setting = setting && num == 1;
        this.UpdateNametagSetting(setting, false);
        break;
      case KID.Model.Permission.ManagedByEnum.PROHIBITED:
        break;
      default:
        setting = PlayerPrefs.GetInt(this.NameTagPlayerPref, setting ? 1 : 0) == 1;
        this.UpdateNametagSetting(setting, !hasOptedInPreviously & setting);
        break;
    }
  }

  public static void RegisterOnNametagSettingChanged(Action<bool> callback)
  {
    GorillaComputer.onNametagSettingChangedAction += callback;
  }

  public static void UnregisterOnNametagSettingChanged(Action<bool> callback)
  {
    GorillaComputer.onNametagSettingChangedAction -= callback;
  }

  private void UpdateNametagSetting(bool newSettingValue, bool saveSetting = true)
  {
    if (newSettingValue)
      KIDManager.SetFeatureOptIn(EKIDFeatures.Custom_Nametags, true);
    this.NametagsEnabled = newSettingValue;
    NetworkSystem.Instance.SetMyNickName(this.NametagsEnabled ? this.savedName : NetworkSystem.Instance.GetMyDefaultName());
    if (NetworkSystem.Instance.InRoom)
      GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, (object) this.redValue, (object) this.greenValue, (object) this.blueValue);
    Action<bool> settingChangedAction = GorillaComputer.onNametagSettingChangedAction;
    if (settingChangedAction != null)
      settingChangedAction(this.NametagsEnabled);
    if (!saveSetting)
      return;
    PlayerPrefs.SetInt(this.NameTagPlayerPref, this.NametagsEnabled ? 1 : 0);
    PlayerPrefs.Save();
  }

  void IMatchmakingCallbacks.OnFriendListUpdate(List<Photon.Realtime.FriendInfo> friendList)
  {
  }

  void IMatchmakingCallbacks.OnCreatedRoom()
  {
  }

  void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
  {
  }

  void IMatchmakingCallbacks.OnJoinedRoom()
  {
  }

  void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
  {
  }

  void IMatchmakingCallbacks.OnLeftRoom()
  {
  }

  void IMatchmakingCallbacks.OnPreLeavingRoom()
  {
  }

  void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
  {
    if (returnCode != (short) 32765)
      return;
    this.roomFull = true;
  }

  public void SetInVirtualStump(bool inVirtualStump)
  {
    this.playerInVirtualStump = inVirtualStump;
    this.roomToJoin = this.playerInVirtualStump ? this.virtualStumpRoomPrepend + this.roomToJoin : this.roomToJoin.RemoveAll(this.virtualStumpRoomPrepend);
  }

  public bool IsPlayerInVirtualStump() => this.playerInVirtualStump;

  public void SetLimitOnlineScreens(bool isLimited)
  {
    this.limitOnlineScreens = isLimited;
    this.UpdateScreen();
  }

  private void InitializeKIdState()
  {
    KIDManager.RegisterSessionUpdateCallback_AnyPermission(new Action(this.OnSessionUpdate_GorillaComputer));
  }

  private void UpdateKidState()
  {
    this._currentScreentState = GorillaComputer.EKidScreenState.Ready;
  }

  private void RequestUpdatedPermissions()
  {
    if (!KIDManager.KidEnabledAndReady || this._waitingForUpdatedSession || (double) Time.time < (double) this._nextUpdateAttemptTime)
      return;
    this._waitingForUpdatedSession = true;
    this.UpdateSession();
  }

  private async void UpdateSession()
  {
    this._nextUpdateAttemptTime = Time.time + this._updateAttemptCooldown;
    await KIDManager.UpdateSession();
    this._waitingForUpdatedSession = false;
  }

  private void OnSessionUpdate_GorillaComputer()
  {
    this.UpdateKidState();
    this.UpdateScreen();
  }

  private void ProcessScreen_SetupKID()
  {
    if (KIDManager.KidEnabledAndReady)
      return;
    Debug.LogError((object) "[KID] Unable to start k-ID Flow. Kid is disabled");
  }

  private bool GuardianConsentMessage(string setupKIDButtonName, string featureDescription)
  {
    string defaultResult1 = "PARENT/GUARDIAN PERMISSION REQUIRED TO ";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("KID_PERMISSION_NEEDED", out result, defaultResult1);
    this.screenText.Text += result;
    GorillaText screenText = this.screenText;
    screenText.Text = $"{screenText.Text}{featureDescription}!";
    if (this._waitingForUpdatedSession)
    {
      string defaultResult2 = "\n\nWAITING FOR PARENT/GUARDIAN CONSENT!";
      LocalisationManager.TryGetKeyForCurrentLocale("KID_WAITING_PERMISSION", out result, defaultResult2);
      this.screenText.Text += result;
      return true;
    }
    if ((double) Time.time >= (double) this._nextUpdateAttemptTime)
    {
      string defaultResult3 = "\n\nPRESS OPTION 2 TO REFRESH PERMISSIONS!";
      LocalisationManager.TryGetKeyForCurrentLocale("KID_REFRESH_PERMISSIONS", out result, defaultResult3);
      this.screenText.Text += result;
    }
    else
    {
      string defaultResult4 = "CHECK AGAIN IN {time} SECONDS!";
      LocalisationManager.TryGetKeyForCurrentLocale("KID_CHECK_AGAIN_COOLDOWN", out result, defaultResult4);
      this.screenText.Text += result.Replace("{time}", ((int) ((double) this._nextUpdateAttemptTime - (double) Time.time)).ToString());
    }
    return false;
  }

  private void ProhibitedMessage(string verb)
  {
    string str = $"\n\nYOU ARE NOT ALLOWED TO {verb} IN YOUR JURISDICTION.";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("KID_PROHIBITED_MESSAGE", out result, "SET CUSTOM NICKNAMES");
    this.screenText.Text += result.Replace("{verb}", verb);
  }

  private void RoomScreen_Permission()
  {
    if (!KIDManager.KidEnabled)
    {
      string defaultResult = "YOU CANNOT USE THE PRIVATE ROOM FEATURE RIGHT NOW";
      string result;
      LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_DISABLED", out result, defaultResult);
      this.screenText.Text = result;
    }
    else
    {
      this.screenText.Text = "";
      string defaultResult = "CREATE OR JOIN PRIVATE ROOMS";
      string result;
      LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_KID_PROHIBITED_VERB", out result, defaultResult);
      this.GuardianConsentMessage("OPTION 3", result);
    }
  }

  private void RoomScreen_KIdProhibited()
  {
    string defaultResult = "CREATE OR JOIN PRIVATE ROOMS";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("ROOM_SCREEN_KID_PROHIBITED_VERB", out result, defaultResult);
    this.ProhibitedMessage(result);
  }

  private void VoiceScreen_Permission()
  {
    string defaultResult1 = "VOICE TYPE: \"MONKE\"\n\n";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_KID_CURRENT_VOICE", out result, defaultResult1);
    this.screenText.Text = result;
    if (!KIDManager.KidEnabled)
    {
      string defaultResult2 = "YOU CANNOT USE THE HUMAN VOICE TYPE FEATURE RIGHT NOW";
      LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_DISABLED", out result, defaultResult2);
      this.screenText.Text += result;
    }
    else
    {
      string defaultResult3 = "ENABLE HUMAN VOICE CHAT";
      LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_GUARDIAN_FEATURE_DESC", out result, defaultResult3);
      this.GuardianConsentMessage("OPTION 3", result);
    }
  }

  private void VoiceScreen_KIdProhibited()
  {
    string defaultResult = "USE THE VOICE CHAT";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_KID_PROHIBITED_VERB", out result, defaultResult);
    this.ProhibitedMessage(result);
  }

  private void MicScreen_Permission()
  {
    this.screenText.Text = "";
    string defaultResult = "ENABLE HUMAN VOICE CHAT";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("VOICE_SCREEN_GUARDIAN_FEATURE_DESC", out result, defaultResult);
    this.GuardianConsentMessage("OPTION 3", result);
  }

  private void MicScreen_KIdProhibited() => this.VoiceScreen_KIdProhibited();

  private void NameScreen_Permission()
  {
    if (!KIDManager.KidEnabled)
    {
      string defaultResult = "YOU CANNOT USE THE CUSTOM NICKNAME FEATURE RIGHT NOW";
      string result;
      LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_DISABLED", out result, defaultResult);
      this.screenText.Text += result;
    }
    else
    {
      this.screenText.Text = "";
      string result;
      LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_KID_PROHIBITED_VERB", out result, "SET CUSTOM NICKNAMES");
      this.GuardianConsentMessage("OPTION 3", result);
    }
  }

  private void NameScreen_KIdProhibited()
  {
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("NAME_SCREEN_KID_PROHIBITED_VERB", out result, "SET CUSTOM NICKNAMES");
    this.ProhibitedMessage(result);
  }

  private void OnKIDSessionUpdated_CustomNicknames(
    bool showCustomNames,
    KID.Model.Permission.ManagedByEnum managedBy)
  {
    this.SetComputerSettingsBySafety((!showCustomNames && managedBy != KID.Model.Permission.ManagedByEnum.PLAYER || managedBy == KID.Model.Permission.ManagedByEnum.PROHIBITED ? 1 : 0) != 0, new GorillaComputer.ComputerState[1]
    {
      GorillaComputer.ComputerState.Name
    }, false);
    int num = PlayerPrefs.GetInt(this.NameTagPlayerPref, -1);
    bool flag = num > 0;
    switch (managedBy)
    {
      case KID.Model.Permission.ManagedByEnum.PLAYER:
        this.NametagsEnabled = !showCustomNames ? num != -1 && flag : num == -1 || flag;
        break;
      case KID.Model.Permission.ManagedByEnum.GUARDIAN:
        this.NametagsEnabled = showCustomNames && (flag || num == -1);
        break;
      case KID.Model.Permission.ManagedByEnum.PROHIBITED:
        this.NametagsEnabled = false;
        break;
    }
    if (this.NametagsEnabled)
      NetworkSystem.Instance.SetMyNickName(this.savedName);
    Action<bool> settingChangedAction = GorillaComputer.onNametagSettingChangedAction;
    if (settingChangedAction == null)
      return;
    settingChangedAction(this.NametagsEnabled);
  }

  private void TroopScreen_Permission()
  {
    this.screenText.Text = "";
    if (!KIDManager.KidEnabled)
    {
      string result;
      LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_DISABLED", out result, "YOU CANNOT USE THE TROOPS FEATURE RIGHT NOW");
      this.screenText.Text += result;
    }
    else
    {
      string result;
      LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_KID_DESC", out result, "JOIN TROOPS");
      this.GuardianConsentMessage("OPTION 3", result);
    }
  }

  private void TroopScreen_KIdProhibited()
  {
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("TROOP_SCREEN_KID_PROHIBITED_VERB", out result, "CREATE OR JOIN TROOPS");
    this.ProhibitedMessage(result);
  }

  private void ProcessKIdState(GorillaKeyboardBindings buttonPressed)
  {
    if (buttonPressed != GorillaKeyboardBindings.option1 || this._currentScreentState != GorillaComputer.EKidScreenState.Ready)
      return;
    this.RequestUpdatedPermissions();
  }

  private void KIdScreen()
  {
    if (!KIDManager.KidEnabledAndReady)
      return;
    if (!KIDManager.HasSession)
      this.GuardianConsentMessage("OPTION 3", "");
    else
      this.KIdScreen_DisplayPermissions();
  }

  private void KIdScreen_DisplayPermissions()
  {
    AgeStatusType activeAccountStatus = KIDManager.GetActiveAccountStatus();
    string str1 = !KIDManager.InitialisationSuccessful ? "NOT READY" : activeAccountStatus.ToString();
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine("k-ID Account Status:\t" + str1);
    if (activeAccountStatus == (AgeStatusType) 0)
    {
      stringBuilder.AppendLine("\nPress 'OPTION 1' to get permissions!");
      this.screenText.Text = stringBuilder.ToString();
    }
    else if (this._waitingForUpdatedSession)
    {
      stringBuilder.AppendLine("\nWAITING FOR PARENT/GUARDIAN CONSENT!");
      this.screenText.Text = stringBuilder.ToString();
    }
    else
    {
      stringBuilder.AppendLine("\nPermissions:");
      List<KID.Model.Permission> allPermissionsData = KIDManager.GetAllPermissionsData();
      int count = allPermissionsData.Count;
      int num = 1;
      for (int index = 0; index < count; ++index)
      {
        if (((IEnumerable<string>) this._interestedPermissionNames).Contains<string>(allPermissionsData[index].Name))
        {
          string str2 = allPermissionsData[index].Enabled ? "<color=#85ffa5>" : "<color=\"RED\">";
          stringBuilder.AppendLine($"[{num.ToString()}] {str2}{allPermissionsData[index].Name}</color>");
          ++num;
        }
      }
      stringBuilder.AppendLine("\nTO REFRESH PERMISSIONS PRESS OPTION 1!");
      this.screenText.Text = stringBuilder.ToString();
    }
  }

  private string GetLocalisedLanguageScreen() => this.GetLanguageScreenLocalisation();

  private void GetLangaugesList(ref string langStr)
  {
    this._languagesDisplaySB.Clear();
    int maxLength = 12;
    int num1 = 3;
    int num2 = 0;
    StringBuilder stringBuilder = new StringBuilder();
    foreach (KeyValuePair<int, Locale> allBinding in LocalisationManager.GetAllBindings())
    {
      ++num2;
      string upper = LocalisationManager.LocaleToFriendlyString(allBinding.Value).ToUpper();
      string str = $"{allBinding.Key}) {upper}";
      stringBuilder.Append(str);
      int remainingChars = this.GetRemainingChars(upper, maxLength);
      stringBuilder.Append(' ', remainingChars);
      if (num2 >= num1)
      {
        this._languagesDisplaySB.AppendLine(stringBuilder.ToString());
        stringBuilder.Clear();
        num2 = 0;
      }
    }
    this._languagesDisplaySB.AppendLine(stringBuilder.ToString());
    langStr = $"{langStr}{this._languagesDisplaySB.ToString()}\n";
  }

  private int GetRemainingChars(string value, int maxLength)
  {
    return !(value == "日本語") ? Mathf.Clamp(maxLength - value.Length, 0, maxLength) : (LocalisationManager.CurrentLanguage.Identifier.Code == "ja" ? 7 : 7);
  }

  private string GetLanguageScreenLocalisation()
  {
    string str1 = "";
    string result;
    LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_TITLE", out result, "CHOOSE YOUR LANGUAGE\n");
    string langStr = str1 + result;
    this.GetLangaugesList(ref langStr);
    LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_INSTRUCTIONS", out result, "PRESS NUMBER KEYS TO CHOOSE A LANGUAGE\n");
    string str2 = langStr + result;
    LocalisationManager.TryGetKeyForCurrentLocale("LANG_SCREEN_CURRENT_LANGUAGE", out result, "CURRENT LANGUAGE: ");
    return str2 + result.TrailingSpace() + LocalisationManager.LocaleToFriendlyString().ToUpper();
  }

  private void InitialiseLanguageScreen()
  {
    this._previousLocalisationSetting = LocalisationManager.CurrentLanguage;
    LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
  }

  private void LanguageScreen() => this.screenText.Text = this.GetLocalisedLanguageScreen();

  private void ProcessLanguageState(GorillaKeyboardBindings buttonPressed)
  {
    int result;
    Locale loc;
    if (!buttonPressed.FromNumberBindingToInt(out result) || !LocalisationManager.TryGetLocaleBinding(result, out loc))
      return;
    LocalisationManager.Instance.OnLanguageButtonPressed(loc.Identifier.Code, true);
    this.RefreshFunctionNames();
  }

  private void OnLanguageChanged()
  {
    if ((UnityEngine.Object) this._previousLocalisationSetting == (UnityEngine.Object) LocalisationManager.CurrentLanguage)
    {
      Debug.Log((object) $"[LOCALISATION::GORILLA_COMPUTER] Language changed, but no different to previous setting [{this._previousLocalisationSetting.ToString()}]");
    }
    else
    {
      this._previousLocalisationSetting = LocalisationManager.CurrentLanguage;
      this.RefreshFunctionNames();
    }
  }

  private void RefreshFunctionNames()
  {
    this.FunctionNames.Clear();
    this.FunctionsCount = this.OrderList.Count;
    this.highestCharacterCount = int.MinValue;
    this.OrderList.ForEach((Action<GorillaComputer.StateOrderItem>) (s =>
    {
      string name = s.GetName();
      if (name.Length > this.highestCharacterCount)
        this.highestCharacterCount = name.Length;
      this.FunctionNames.Add(name);
    }));
    for (int index1 = 0; index1 < this.FunctionsCount; ++index1)
    {
      int num = this.highestCharacterCount - this.FunctionNames[index1].Length;
      for (int index2 = 0; index2 < num; ++index2)
        this.FunctionNames[index1] += " ";
    }
  }

  public enum ComputerState
  {
    Startup,
    Color,
    Name,
    Turn,
    Mic,
    Room,
    Queue,
    Group,
    Voice,
    AutoMute,
    Credits,
    Visuals,
    Time,
    NameWarning,
    Loading,
    Support,
    Troop,
    KID,
    Redemption,
    Language,
  }

  private enum NameCheckResult
  {
    Success,
    Warning,
    Ban,
  }

  public enum RedemptionResult
  {
    Empty,
    Invalid,
    Checking,
    AlreadyUsed,
    Success,
  }

  [Serializable]
  public class StateOrderItem
  {
    public GorillaComputer.ComputerState State;
    [Tooltip("Case not important - ToUpper applied at runtime")]
    public string OverrideName = "";
    public LocalizedString StringReference;
    private Locale _previousLocale;
    private string _cachedTranslation = "";

    public StateOrderItem()
    {
    }

    public StateOrderItem(GorillaComputer.ComputerState state) => this.State = state;

    public StateOrderItem(GorillaComputer.ComputerState state, string overrideName)
    {
      this.State = state;
      this.OverrideName = overrideName;
    }

    public string GetName()
    {
      if ((UnityEngine.Object) this._previousLocale == (UnityEngine.Object) LocalizationSettings.SelectedLocale && !string.IsNullOrEmpty(this._cachedTranslation))
        return this._cachedTranslation;
      if (this.StringReference == null || this.StringReference.IsEmpty)
        return this.GetPreLocalisedName();
      this._previousLocale = LocalizationSettings.SelectedLocale;
      this._cachedTranslation = this.StringReference.GetLocalizedString()?.ToUpper();
      if (string.IsNullOrEmpty(this._cachedTranslation))
      {
        if (LocalisationManager.ApplicationRunning)
          Debug.LogError((object) $"[LOCALIZATION::STATE_ORDER_ITEM] Failed to get translation for selected locale [{this._previousLocale?.LocaleName ?? "NULL"}, for item [{this.State.GetName<GorillaComputer.ComputerState>()}]");
        this._cachedTranslation = "";
      }
      return this._cachedTranslation;
    }

    public string GetPreLocalisedName()
    {
      return !string.IsNullOrEmpty(this.OverrideName) ? this.OverrideName.ToUpper() : this.State.ToString().ToUpper();
    }
  }

  private enum EKidScreenState
  {
    Ready,
    Show_OTP,
    Show_Setup_Screen,
  }
}
