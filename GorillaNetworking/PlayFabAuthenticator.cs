// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.PlayFabAuthenticator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using JetBrains.Annotations;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaNetworking;

public class PlayFabAuthenticator : MonoBehaviour
{
  public static volatile PlayFabAuthenticator instance;
  private string _playFabPlayerIdCache;
  private string _sessionTicket;
  private string _displayName;
  private string _nonce;
  public string userID;
  private string userToken;
  public PlatformTagJoin platform;
  private bool isSafeAccount;
  public Action<bool> OnSafetyUpdate;
  private PlayFabAuthenticator.SafetyType safetyType;
  private byte[] m_Ticket;
  private uint m_pcbTicket;
  public UnityEngine.UI.Text debugText;
  public bool screenDebugMode;
  public bool loginFailed;
  [FormerlySerializedAs("loginDisplayID")]
  public GameObject emptyObject;
  private int playFabAuthRetryCount;
  private int playFabMaxRetries = 5;
  private int playFabCacheRetryCount;
  private int playFabCacheMaxRetries = 5;
  public MetaAuthenticator metaAuthenticator;
  public SteamAuthenticator steamAuthenticator;
  public MothershipAuthenticator mothershipAuthenticator;
  public PhotonAuthenticator photonAuthenticator;
  [SerializeField]
  private bool dbg_isReturningPlayer;
  private SteamAuthTicket steamAuthTicketForPlayFab;
  private SteamAuthTicket steamAuthTicketForPhoton;
  private string steamAuthIdForPhoton;

  public GorillaComputer gorillaComputer => GorillaComputer.instance;

  public bool IsReturningPlayer { get; private set; }

  public bool postAuthSetSafety { get; private set; }

  private void Awake()
  {
    if ((UnityEngine.Object) PlayFabAuthenticator.instance == (UnityEngine.Object) null)
      PlayFabAuthenticator.instance = this;
    else if ((UnityEngine.Object) PlayFabAuthenticator.instance != (UnityEngine.Object) this)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    if ((UnityEngine.Object) PlayFabAuthenticator.instance.photonAuthenticator == (UnityEngine.Object) null)
      PlayFabAuthenticator.instance.photonAuthenticator = PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<PhotonAuthenticator>();
    this.platform = ScriptableObject.CreateInstance<PlatformTagJoin>();
    PlayFabSettings.CompressApiData = false;
    byte[] numArray = new byte[1];
    if (this.screenDebugMode)
      this.debugText.text = "";
    Debug.Log((object) "doing steam thing");
    if ((UnityEngine.Object) PlayFabAuthenticator.instance.steamAuthenticator == (UnityEngine.Object) null)
      PlayFabAuthenticator.instance.steamAuthenticator = PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<SteamAuthenticator>();
    this.platform.PlatformTag = "Steam";
    PlayFabSettings.TitleId = PlayFabAuthenticatorSettings.TitleId;
    PlayFabSettings.DisableFocusTimeCollection = true;
    this.BeginLoginFlow();
  }

  public void BeginLoginFlow()
  {
    if (!MothershipClientApiUnity.IsEnabled())
    {
      this.AuthenticateWithPlayFab();
    }
    else
    {
      if (!((UnityEngine.Object) PlayFabAuthenticator.instance.mothershipAuthenticator == (UnityEngine.Object) null))
        return;
      PlayFabAuthenticator.instance.mothershipAuthenticator = MothershipAuthenticator.Instance ?? PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<MothershipAuthenticator>();
      PlayFabAuthenticator.instance.mothershipAuthenticator.OnLoginSuccess += (Action) (() => PlayFabAuthenticator.instance.AuthenticateWithPlayFab());
      PlayFabAuthenticator.instance.mothershipAuthenticator.OnLoginFailure += (Action<string>) (errorMessage =>
      {
        this.loginFailed = true;
        this.ShowMothershipAuthErrorMessage(errorMessage);
      });
      PlayFabAuthenticator.instance.mothershipAuthenticator.BeginLoginFlow();
    }
  }

  private void Start()
  {
  }

  private void OnEnable()
  {
    NetworkSystem.Instance.OnCustomAuthenticationResponse += new Action<Dictionary<string, object>>(this.OnCustomAuthenticationResponse);
  }

  private void OnDisable()
  {
    NetworkSystem.Instance.OnCustomAuthenticationResponse -= new Action<Dictionary<string, object>>(this.OnCustomAuthenticationResponse);
    this.steamAuthTicketForPhoton?.Dispose();
    this.steamAuthTicketForPlayFab?.Dispose();
  }

  public void RefreshSteamAuthTicketForPhoton(
    Action<string> successCallback,
    Action<EResult> failureCallback)
  {
    this.steamAuthTicketForPhoton?.Dispose();
    this.steamAuthTicketForPhoton = (SteamAuthTicket) this.steamAuthenticator.GetAuthTicketForWebApi(this.steamAuthIdForPhoton, successCallback, failureCallback);
  }

  private void OnCustomAuthenticationResponse(Dictionary<string, object> response)
  {
    this.steamAuthTicketForPhoton?.Dispose();
    object obj;
    if (response.TryGetValue("SteamAuthIdForPhoton", out obj) && obj is string str)
      this.steamAuthIdForPhoton = str;
    else
      this.steamAuthIdForPhoton = (string) null;
  }

  public void AuthenticateWithPlayFab()
  {
    Debug.Log((object) "authenticating with playFab!");
    GorillaServer instance = GorillaServer.Instance;
    if ((instance != null ? (instance.FeatureFlagsReady ? 1 : 0) : 0) != 0)
    {
      if (KIDManager.KidEnabled)
      {
        Debug.Log((object) "[KID] Is Enabled - Enabling safeties by platform and age category");
        this.DefaultSafetiesByAgeCategory();
      }
    }
    else
      this.postAuthSetSafety = true;
    if (SteamManager.Initialized)
    {
      this.userID = SteamUser.GetSteamID().ToString();
      Debug.Log((object) "trying to auth with steam");
      this.steamAuthTicketForPlayFab = (SteamAuthTicket) this.steamAuthenticator.GetAuthTicket((Action<string>) (ticket =>
      {
        Debug.Log((object) "Got steam auth session ticket!");
        PlayFabClientAPI.LoginWithSteam(new LoginWithSteamRequest()
        {
          CreateAccount = new bool?(true),
          SteamTicket = ticket
        }, new Action<LoginResult>(this.OnLoginWithSteamResponse), new Action<PlayFabError>(this.OnPlayFabError));
      }), (Action<EResult>) (result => this.StartCoroutine(this.DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame())));
    }
    else
      this.StartCoroutine(this.DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame());
  }

  private IEnumerator VerifyKidAuthenticated(DateTime accountCreationDateTime)
  {
    PlayFabAuthenticator fabAuthenticator1 = this;
    Task<DateTime?> getNewPlayerDateTimeTask = KIDManager.CheckKIDNewPlayerDateTime();
    yield return (object) new WaitUntil((Func<bool>) (() => getNewPlayerDateTimeTask.IsCompleted));
    DateTime? result = getNewPlayerDateTimeTask.Result;
    if (result.HasValue && KIDManager.KidEnabled)
    {
      PlayFabAuthenticator fabAuthenticator2 = fabAuthenticator1;
      DateTime dateTime = accountCreationDateTime;
      DateTime? nullable = result;
      int num = nullable.HasValue ? (dateTime < nullable.GetValueOrDefault() ? 1 : 0) : 0;
      fabAuthenticator2.IsReturningPlayer = num != 0;
    }
  }

  private IEnumerator DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame()
  {
    PlayFabAuthenticator context = this;
    yield return (object) null;
    if ((UnityEngine.Object) context.gorillaComputer != (UnityEngine.Object) null)
    {
      context.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.");
      context.gorillaComputer.screenText.Text = "UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.";
      Debug.Log((object) "Couldn't authenticate steam account");
    }
    else
      Debug.LogError((object) "PlayFabAuthenticator: gorillaComputer is null, so could not set GeneralFailureMessage notifying user that the steam account could not be authenticated.", (UnityEngine.Object) context);
  }

  private void OnLoginWithSteamResponse(LoginResult obj)
  {
    this._playFabPlayerIdCache = obj.PlayFabId;
    this._sessionTicket = obj.SessionTicket;
    this.StartCoroutine(this.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest()
    {
      Platform = this.platform.ToString(),
      SessionTicket = this._sessionTicket,
      PlayFabId = this._playFabPlayerIdCache,
      TitleId = PlayFabSettings.TitleId,
      MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
      MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
      MothershipToken = MothershipClientContext.Token,
      MothershipId = MothershipClientContext.MothershipId
    }, new Action<PlayFabAuthenticator.CachePlayFabIdResponse>(this.OnCachePlayFabIdRequest)));
  }

  private void OnCachePlayFabIdRequest(
    [CanBeNull] PlayFabAuthenticator.CachePlayFabIdResponse response)
  {
    if (response != null)
    {
      this.steamAuthIdForPhoton = response.SteamAuthIdForPhoton;
      DateTime result;
      if (DateTime.TryParse(response.AccountCreationIsoTimestamp, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result))
        this.StartCoroutine(this.VerifyKidAuthenticated(result));
      Debug.Log((object) "Successfully cached PlayFab Id.  Continuing!");
      this.AdvanceLogin();
    }
    else
      Debug.LogError((object) "Could not cache PlayFab Id.  Cannot continue.");
  }

  private void AdvanceLogin()
  {
    this.LogMessage("PlayFab authenticated ... Getting Nonce");
    this.RefreshSteamAuthTicketForPhoton((Action<string>) (ticket =>
    {
      this._nonce = ticket;
      Debug.Log((object) "Got nonce!  Authenticating...");
      this.AuthenticateWithPhoton();
    }), (Action<EResult>) (result =>
    {
      Debug.LogWarning((object) "Failed to get nonce!");
      this.AuthenticateWithPhoton();
    }));
  }

  private void AuthenticateWithPhoton()
  {
    this.photonAuthenticator.SetCustomAuthenticationParameters(new Dictionary<string, object>()
    {
      {
        "AppId",
        (object) PlayFabSettings.TitleId
      },
      {
        "AppVersion",
        (object) (NetworkSystemConfig.AppVersion ?? "-1")
      },
      {
        "Ticket",
        (object) this._sessionTicket
      },
      {
        "Nonce",
        (object) this._nonce
      },
      {
        "MothershipEnvId",
        (object) MothershipClientApiUnity.EnvironmentId
      },
      {
        "MothershipDeploymentId",
        (object) MothershipClientApiUnity.DeploymentId
      },
      {
        "MothershipToken",
        (object) MothershipClientContext.Token
      }
    });
    this.GetPlayerDisplayName(this._playFabPlayerIdCache);
    GorillaServer.Instance.AddOrRemoveDLCOwnership((Action<ExecuteFunctionResult>) (result =>
    {
      Debug.Log((object) "got results! updating!");
      if (!((UnityEngine.Object) GorillaTagger.Instance != (UnityEngine.Object) null))
        return;
      GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
    }), (Action<PlayFabError>) (error =>
    {
      Debug.Log((object) "Got error retrieving user data:");
      Debug.Log((object) error.GenerateErrorReport());
      if (!((UnityEngine.Object) GorillaTagger.Instance != (UnityEngine.Object) null))
        return;
      GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
    }));
    if ((UnityEngine.Object) CosmeticsController.instance != (UnityEngine.Object) null)
    {
      Debug.Log((object) "initializing cosmetics");
      CosmeticsController.instance.Initialize();
    }
    if ((UnityEngine.Object) this.gorillaComputer != (UnityEngine.Object) null)
      this.gorillaComputer.OnConnectedToMasterStuff();
    else
      this.StartCoroutine(this.ComputerOnConnectedToMaster());
    if ((UnityEngine.Object) RankedProgressionManager.Instance != (UnityEngine.Object) null)
      RankedProgressionManager.Instance.LoadStats();
    if (!((UnityEngine.Object) PhotonNetworkController.Instance != (UnityEngine.Object) null))
      return;
    Debug.Log((object) "Finish authenticating");
    NetworkSystem.Instance.FinishAuthenticating();
  }

  private IEnumerator ComputerOnConnectedToMaster()
  {
    WaitForEndOfFrame frameYield = new WaitForEndOfFrame();
    while ((UnityEngine.Object) this.gorillaComputer == (UnityEngine.Object) null)
      yield return (object) frameYield;
    this.gorillaComputer.OnConnectedToMasterStuff();
  }

  private void OnPlayFabError(PlayFabError obj)
  {
    this.LogMessage(obj.ErrorMessage);
    Debug.Log((object) ("OnPlayFabError(): " + obj.ErrorMessage));
    this.loginFailed = true;
    if (obj.ErrorMessage == "The account making this request is currently banned")
    {
      using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        KeyValuePair<string, List<string>> current = enumerator.Current;
        if (current.Value[0] != "Indefinite")
          this.gorillaComputer.GeneralFailureMessage($"YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: {current.Key}\nHOURS LEFT: {((int) ((DateTime.Parse(current.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString()}");
        else
          this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + current.Key);
      }
    }
    else if (obj.ErrorMessage == "The IP making this request is currently banned")
    {
      using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        KeyValuePair<string, List<string>> current = enumerator.Current;
        if (current.Value[0] != "Indefinite")
          this.gorillaComputer.GeneralFailureMessage($"THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: {current.Key}\nHOURS LEFT: {((int) ((DateTime.Parse(current.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString()}");
        else
          this.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + current.Key);
      }
    }
    else
    {
      if (!((UnityEngine.Object) this.gorillaComputer != (UnityEngine.Object) null))
        return;
      this.gorillaComputer.GeneralFailureMessage(this.gorillaComputer.unableToConnect);
    }
  }

  private void LogMessage(string message)
  {
  }

  private void GetPlayerDisplayName(string playFabId)
  {
    PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
    {
      PlayFabId = playFabId,
      ProfileConstraints = new PlayerProfileViewConstraints()
      {
        ShowDisplayName = true
      }
    }, (Action<GetPlayerProfileResult>) (result => this._displayName = result.PlayerProfile.DisplayName), (Action<PlayFabError>) (error => Debug.LogError((object) error.GenerateErrorReport())));
  }

  public void SetDisplayName(string playerName)
  {
    if (this._displayName != null && (this._displayName.Length <= 4 || !(this._displayName.Substring(0, this._displayName.Length - 4) != playerName)))
      return;
    PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest()
    {
      DisplayName = playerName
    }, (Action<UpdateUserTitleDisplayNameResult>) (result => this._displayName = playerName), (Action<PlayFabError>) (error => Debug.LogError((object) error.GenerateErrorReport())));
  }

  public void ScreenDebug(string debugString)
  {
    Debug.Log((object) debugString);
    if (!this.screenDebugMode)
      return;
    UnityEngine.UI.Text debugText = this.debugText;
    debugText.text = $"{debugText.text}{debugString}\n";
  }

  public void ScreenDebugClear() => this.debugText.text = "";

  public IEnumerator PlayfabAuthenticate(
    PlayFabAuthenticator.PlayfabAuthRequestData data,
    Action<PlayFabAuthenticator.PlayfabAuthResponseData> callback)
  {
    UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/PlayFabAuthentication", "POST");
    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson((object) data));
    bool retry = false;
    request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bytes);
    request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");
    request.timeout = 15;
    yield return (object) request.SendWebRequest();
    if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
    {
      callback(JsonUtility.FromJson<PlayFabAuthenticator.PlayfabAuthResponseData>(request.downloadHandler.text));
    }
    else
    {
      if (request.responseCode == 403L)
      {
        Debug.LogError((object) $"HTTP {request.responseCode}: {request.error}, with body: {request.downloadHandler.text}");
        this.ShowBanMessage(JsonUtility.FromJson<PlayFabAuthenticator.BanInfo>(request.downloadHandler.text));
        callback((PlayFabAuthenticator.PlayfabAuthResponseData) null);
      }
      if (request.result == UnityWebRequest.Result.ProtocolError && request.responseCode != 400L)
      {
        retry = true;
        Debug.LogError((object) $"HTTP {request.responseCode} error: {request.error} message:{request.downloadHandler.text}");
      }
      else if (request.result == UnityWebRequest.Result.ConnectionError)
      {
        retry = true;
        Debug.LogError((object) $"NETWORK ERROR: {request.error}\nMessage: {request.downloadHandler.text}");
      }
      else
      {
        Debug.LogError((object) $"HTTP ERROR: {request.error}\nMessage: {request.downloadHandler.text}");
        retry = true;
      }
    }
    if (retry)
    {
      if (this.playFabAuthRetryCount < this.playFabMaxRetries)
      {
        int seconds = (int) Mathf.Pow(2f, (float) (this.playFabAuthRetryCount + 1));
        Debug.LogWarning((object) $"Retrying PlayFab auth... Retry attempt #{this.playFabAuthRetryCount + 1}, waiting for {seconds} seconds");
        ++this.playFabAuthRetryCount;
        yield return (object) new WaitForSeconds((float) seconds);
      }
      else
      {
        Debug.LogError((object) "Maximum retries attempted. Please check your network connection.");
        callback((PlayFabAuthenticator.PlayfabAuthResponseData) null);
        this.ShowPlayFabAuthErrorMessage(request.downloadHandler.text);
      }
    }
  }

  private void ShowMothershipAuthErrorMessage(string errorMessage)
  {
    try
    {
      this.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE WITH MOTHERSHIP.\nREASON: " + errorMessage);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) $"Failed to show Mothership auth error message: {ex}");
    }
  }

  private void ShowPlayFabAuthErrorMessage(string errorJson)
  {
    try
    {
      this.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE WITH PLAYFAB.\nREASON: " + JsonUtility.FromJson<PlayFabAuthenticator.ErrorInfo>(errorJson).Message);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) $"Failed to show PlayFab auth error message: {ex}");
    }
  }

  private void ShowBanMessage(PlayFabAuthenticator.BanInfo banInfo)
  {
    try
    {
      if (banInfo.BanExpirationTime == null || banInfo.BanMessage == null)
        return;
      if (banInfo.BanExpirationTime != "Indefinite")
        this.gorillaComputer.GeneralFailureMessage($"YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: {banInfo.BanMessage}\nHOURS LEFT: {((int) ((DateTime.Parse(banInfo.BanExpirationTime) - DateTime.UtcNow).TotalHours + 1.0)).ToString()}");
      else
        this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + banInfo.BanMessage);
    }
    catch (Exception ex)
    {
      Debug.LogError((object) $"Failed to show ban message: {ex}");
    }
  }

  public IEnumerator CachePlayFabId(
    PlayFabAuthenticator.CachePlayFabIdRequest data,
    Action<PlayFabAuthenticator.CachePlayFabIdResponse> callback)
  {
    PlayFabAuthenticator fabAuthenticator1 = this;
    Debug.Log((object) "Trying to cache playfab Id");
    UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/CachePlayFabId", "POST");
    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson((object) data));
    bool retry = false;
    request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bytes);
    request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");
    request.timeout = 15;
    yield return (object) request.SendWebRequest();
    if (request.result != UnityWebRequest.Result.ConnectionError && request.result != UnityWebRequest.Result.ProtocolError)
    {
      if (request.responseCode == 200L)
        callback(JsonUtility.FromJson<PlayFabAuthenticator.CachePlayFabIdResponse>(request.downloadHandler.text));
    }
    else if (request.result == UnityWebRequest.Result.ProtocolError && request.responseCode != 400L)
    {
      retry = true;
      Debug.LogError((object) $"HTTP {request.responseCode} error: {request.error}");
    }
    else
      retry = request.result == UnityWebRequest.Result.ConnectionError || true;
    if (retry)
    {
      if (fabAuthenticator1.playFabCacheRetryCount < fabAuthenticator1.playFabCacheMaxRetries)
      {
        int seconds = (int) Mathf.Pow(2f, (float) (fabAuthenticator1.playFabCacheRetryCount + 1));
        Debug.LogWarning((object) $"Retrying PlayFab auth... Retry attempt #{fabAuthenticator1.playFabCacheRetryCount + 1}, waiting for {seconds} seconds");
        ++fabAuthenticator1.playFabCacheRetryCount;
        yield return (object) new WaitForSeconds((float) seconds);
        PlayFabAuthenticator fabAuthenticator2 = fabAuthenticator1;
        PlayFabAuthenticator fabAuthenticator3 = fabAuthenticator1;
        PlayFabAuthenticator.CachePlayFabIdRequest data1 = new PlayFabAuthenticator.CachePlayFabIdRequest();
        data1.Platform = fabAuthenticator1.platform.ToString();
        data1.SessionTicket = fabAuthenticator1._sessionTicket;
        data1.PlayFabId = fabAuthenticator1._playFabPlayerIdCache;
        data1.TitleId = PlayFabSettings.TitleId;
        data1.MothershipEnvId = MothershipClientApiUnity.EnvironmentId;
        data1.MothershipDeploymentId = MothershipClientApiUnity.DeploymentId;
        data1.MothershipToken = MothershipClientContext.Token;
        data1.MothershipId = MothershipClientContext.MothershipId;
        Action<PlayFabAuthenticator.CachePlayFabIdResponse> callback1 = new Action<PlayFabAuthenticator.CachePlayFabIdResponse>(fabAuthenticator1.OnCachePlayFabIdRequest);
        IEnumerator routine = fabAuthenticator3.CachePlayFabId(data1, callback1);
        fabAuthenticator2.StartCoroutine(routine);
      }
      else
      {
        Debug.LogError((object) "Maximum retries attempted. Please check your network connection.");
        callback((PlayFabAuthenticator.CachePlayFabIdResponse) null);
        fabAuthenticator1.ShowPlayFabAuthErrorMessage(request.downloadHandler.text);
      }
    }
  }

  public void DefaultSafetiesByAgeCategory()
  {
    Debug.Log((object) "[KID::PLAYFAB_AUTHENTICATOR] Defaulting Safety Settings to Disabled because age category data unavailable on this platform");
    this.SetSafety(false, true);
  }

  public void SetSafety(bool isSafety, bool isAutoSet, bool setPlayfab = false)
  {
    this.postAuthSetSafety = false;
    Action<bool> onSafetyUpdate = this.OnSafetyUpdate;
    if (onSafetyUpdate != null)
      onSafetyUpdate(isSafety);
    Debug.Log((object) $"[KID] Setting safety to: [{isSafety.ToString()}]");
    this.isSafeAccount = isSafety;
    this.safetyType = PlayFabAuthenticator.SafetyType.None;
    if (isSafety)
    {
      if (isAutoSet)
      {
        PlayerPrefs.SetInt("autoSafety", 1);
        this.safetyType = PlayFabAuthenticator.SafetyType.Auto;
      }
      else
      {
        PlayerPrefs.SetInt("optSafety", 1);
        this.safetyType = PlayFabAuthenticator.SafetyType.OptIn;
      }
    }
    else
    {
      if (isAutoSet)
        PlayerPrefs.SetInt("autoSafety", 0);
      else
        PlayerPrefs.SetInt("optSafety", 0);
      PlayerPrefs.Save();
    }
  }

  public string GetPlayFabSessionTicket() => this._sessionTicket;

  public string GetPlayFabPlayerId() => this._playFabPlayerIdCache;

  public bool GetSafety() => this.isSafeAccount;

  public PlayFabAuthenticator.SafetyType GetSafetyType() => this.safetyType;

  public string GetUserID() => this.userID;

  public enum SafetyType
  {
    None,
    Auto,
    OptIn,
  }

  [Serializable]
  public class CachePlayFabIdRequest
  {
    public string Platform;
    public string SessionTicket;
    public string PlayFabId;
    public string TitleId;
    public string MothershipEnvId;
    public string MothershipDeploymentId;
    public string MothershipToken;
    public string MothershipId;
  }

  [Serializable]
  public class PlayfabAuthRequestData
  {
    public string AppId;
    public string Nonce;
    public string OculusId;
    public string Platform;
    public string AgeCategory;
    public string MothershipEnvId;
    public string MothershipDeploymentId;
    public string MothershipToken;
    public string MothershipId;
  }

  [Serializable]
  public class PlayfabAuthResponseData
  {
    public string SessionTicket;
    public string EntityToken;
    public string PlayFabId;
    public string EntityId;
    public string EntityType;
    public string AccountCreationIsoTimestamp;
  }

  [Serializable]
  public class CachePlayFabIdResponse
  {
    public string PlayFabId;
    public string SteamAuthIdForPhoton;
    public string AccountCreationIsoTimestamp;
  }

  private class ErrorInfo
  {
    public string Message;
    public string Error;
  }

  private class BanInfo
  {
    public string BanMessage;
    public string BanExpirationTime;
  }
}
