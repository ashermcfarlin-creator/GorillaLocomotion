// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.GorillaServer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Newtonsoft.Json;
using PlayFab;
using PlayFab.CloudScriptModels;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaNetworking;

public class GorillaServer : MonoBehaviour, ISerializationCallbackReceiver
{
  public static volatile GorillaServer Instance;
  public string FeatureFlagsTitleDataKey = "DeployFeatureFlags";
  public List<string> DefaultDeployFeatureFlagsEnabled = new List<string>();
  private TitleDataFeatureFlags featureFlags = new TitleDataFeatureFlags();
  private bool debug;
  private JsonSerializerSettings serializationSettings = new JsonSerializerSettings()
  {
    NullValueHandling = NullValueHandling.Ignore,
    DefaultValueHandling = DefaultValueHandling.Ignore,
    MissingMemberHandling = MissingMemberHandling.Ignore,
    ObjectCreationHandling = ObjectCreationHandling.Replace,
    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    TypeNameHandling = TypeNameHandling.Auto
  };

  public bool FeatureFlagsReady => this.featureFlags.ready;

  private PlayFab.CloudScriptModels.EntityKey playerEntity
  {
    get
    {
      return new PlayFab.CloudScriptModels.EntityKey()
      {
        Id = PlayFabSettings.staticPlayer.EntityId,
        Type = PlayFabSettings.staticPlayer.EntityType
      };
    }
  }

  public void Start() => this.featureFlags.FetchFeatureFlags();

  private void Awake()
  {
    if ((UnityEngine.Object) GorillaServer.Instance == (UnityEngine.Object) null)
      GorillaServer.Instance = this;
    else
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
  }

  public void ReturnCurrentVersion(
    ReturnCurrentVersionRequest request,
    Action<ExecuteFunctionResult> successCallback,
    Action<PlayFabError> errorCallback)
  {
    successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnCurrentVersion result");
    errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnCurrentVersion error");
    Debug.Log((object) "GorillaServer: ReturnCurrentVersion V2 call");
    PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
    {
      Entity = this.playerEntity,
      FunctionName = "ReturnCurrentVersionV2",
      FunctionParameter = (object) request
    }, successCallback, errorCallback);
  }

  public void ReturnMyOculusHash(
    Action<ExecuteFunctionResult> successCallback,
    Action<PlayFabError> errorCallback)
  {
    successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnMyOculusHash result");
    errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnMyOculusHash error");
    Debug.Log((object) "GorillaServer: ReturnMyOculusHash V2 call");
    PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
    {
      Entity = this.playerEntity,
      FunctionName = "ReturnMyOculusHashV2",
      FunctionParameter = (object) new{  }
    }, successCallback, errorCallback);
  }

  public void TryDistributeCurrency(
    Action<ExecuteFunctionResult> successCallback,
    Action<PlayFabError> errorCallback)
  {
    successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "TryDistributeCurrency result");
    errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "TryDistributeCurrency error");
    Debug.Log((object) "GorillaServer: TryDistributeCurrency V2 call");
    PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
    {
      Entity = this.playerEntity,
      FunctionName = "TryDistributeCurrencyV2",
      FunctionParameter = (object) new{  }
    }, successCallback, errorCallback);
  }

  public void AddOrRemoveDLCOwnership(
    Action<ExecuteFunctionResult> successCallback,
    Action<PlayFabError> errorCallback)
  {
    successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "AddOrRemoveDLCOwnership result");
    errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "AddOrRemoveDLCOwnership error");
    Debug.Log((object) "GorillaServer: AddOrRemoveDLCOwnership V2 call");
    PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
    {
      Entity = this.playerEntity,
      FunctionName = "AddOrRemoveDLCOwnershipV2",
      FunctionParameter = (object) new{  }
    }, successCallback, errorCallback);
  }

  public void BroadcastMyRoom(
    BroadcastMyRoomRequest request,
    Action<ExecuteFunctionResult> successCallback,
    Action<PlayFabError> errorCallback)
  {
    successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "BroadcastMyRoom result");
    errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "BroadcastMyRoom error");
    Debug.Log((object) $"GorillaServer: BroadcastMyRoom V2 call ({request})");
    PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
    {
      Entity = this.playerEntity,
      FunctionName = "BroadcastMyRoomV2",
      FunctionParameter = (object) request
    }, successCallback, errorCallback);
  }

  public bool NewCosmeticsPath()
  {
    return this.featureFlags.IsEnabledForUser("2024-06-CosmeticsAuthenticationV2");
  }

  public bool NewCosmeticsPathShouldSetSharedGroupData()
  {
    return this.featureFlags.IsEnabledForUser("2025-04-CosmeticsAuthenticationV2-SetData");
  }

  public bool NewCosmeticsPathShouldReadSharedGroupData()
  {
    return this.featureFlags.IsEnabledForUser("2025-04-CosmeticsAuthenticationV2-ReadData");
  }

  public bool NewCosmeticsPathShouldSetRoomData()
  {
    return this.featureFlags.IsEnabledForUser("2025-04-CosmeticsAuthenticationV2-Compat");
  }

  public void UpdateUserCosmetics()
  {
    PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
    {
      Entity = this.playerEntity,
      FunctionName = "UpdatePersonalCosmeticsList",
      FunctionParameter = (object) new{  },
      GeneratePlayStreamEvent = new bool?(false)
    }, (Action<ExecuteFunctionResult>) (result =>
    {
      if (!((UnityEngine.Object) CosmeticsController.instance != (UnityEngine.Object) null))
        return;
      CosmeticsController.instance.CheckCosmeticsSharedGroup();
    }), (Action<PlayFabError>) (error => { }));
  }

  public void GetAcceptedAgreements(
    GetAcceptedAgreementsRequest request,
    Action<Dictionary<string, string>> successCallback,
    Action<PlayFabError> errorCallback)
  {
    successCallback = this.DebugWrapCb<Dictionary<string, string>>(successCallback, "GetAcceptedAgreements result");
    errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "GetAcceptedAgreements json error");
    Debug.Log((object) $"GorillaServer: GetAcceptedAgreements call ({request})");
    PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
    {
      Entity = this.playerEntity,
      FunctionName = nameof (GetAcceptedAgreements),
      FunctionParameter = (object) string.Join(",", request.AgreementKeys),
      GeneratePlayStreamEvent = new bool?(false)
    }, (Action<ExecuteFunctionResult>) (result =>
    {
      try
      {
        successCallback(JsonConvert.DeserializeObject<Dictionary<string, string>>(Convert.ToString(result.FunctionResult)));
      }
      catch (Exception ex)
      {
        errorCallback(new PlayFabError()
        {
          ErrorMessage = $"Invalid format for GetAcceptedAgreements ({ex})",
          Error = PlayFabErrorCode.JsonParseError
        });
      }
    }), errorCallback);
  }

  public void SubmitAcceptedAgreements(
    SubmitAcceptedAgreementsRequest request,
    Action<ExecuteFunctionResult> successCallback,
    Action<PlayFabError> errorCallback)
  {
    successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "SubmitAcceptedAgreements result");
    errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "SubmitAcceptedAgreements error");
    Debug.Log((object) $"GorillaServer: SubmitAcceptedAgreements call ({request})");
    PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
    {
      Entity = this.playerEntity,
      FunctionName = nameof (SubmitAcceptedAgreements),
      FunctionParameter = (object) request.Agreements,
      GeneratePlayStreamEvent = new bool?(false)
    }, successCallback, errorCallback);
  }

  public void UploadGorillanalytics(object uploadData)
  {
    Debug.Log((object) $"GorillaServer: UploadGorillanalytics call ({uploadData})");
    PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
    {
      Entity = this.playerEntity,
      FunctionName = "Gorillanalytics",
      FunctionParameter = uploadData,
      GeneratePlayStreamEvent = new bool?(false)
    }, (Action<ExecuteFunctionResult>) (result => Debug.Log((object) $"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete")), (Action<PlayFabError>) (error => Debug.Log((object) ("Error uploading Gorillanalytics: " + error.GenerateErrorReport()))));
  }

  public void CheckForBadName(
    CheckForBadNameRequest request,
    Action<ExecuteFunctionResult> successCallback,
    Action<PlayFabError> errorCallback)
  {
    successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "CheckForBadName result");
    errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "CheckForBadName error");
    Debug.Log((object) $"GorillaServer: CheckForBadName call ({request})");
    PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
    {
      Entity = this.playerEntity,
      FunctionName = nameof (CheckForBadName),
      FunctionParameter = (object) new
      {
        name = request.name,
        forRoom = request.forRoom.ToString(),
        forTroop = request.forTroop.ToString()
      },
      GeneratePlayStreamEvent = new bool?(false)
    }, successCallback, errorCallback);
  }

  public void GetRandomName(
    Action<ExecuteFunctionResult> successCallback,
    Action<PlayFabError> errorCallback)
  {
    successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "GetRandomName result");
    errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "GetRandomName error");
    Debug.Log((object) "GorillaServer: GetRandomName call");
    PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
    {
      Entity = this.playerEntity,
      FunctionName = nameof (GetRandomName),
      GeneratePlayStreamEvent = new bool?(false)
    }, successCallback, errorCallback);
  }

  public void ReturnQueueStats(
    ReturnQueueStatsRequest request,
    Action<ExecuteFunctionResult> successCallback,
    Action<PlayFabError> errorCallback)
  {
    successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnQueueStats result");
    errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnQueueStats error");
    Debug.Log((object) "GorillaServer: ReturnQueueStats call");
    PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
    {
      Entity = this.playerEntity,
      FunctionName = nameof (ReturnQueueStats),
      FunctionParameter = (object) new
      {
        QueueName = request.queueName
      },
      GeneratePlayStreamEvent = new bool?(false)
    }, successCallback, errorCallback);
  }

  private Action<T> DebugWrapCb<T>(Action<T> cb, string label)
  {
    return (Action<T>) (arg =>
    {
      if (this.debug)
      {
        try
        {
          Debug.Log((object) $"GorillaServer: {label} ({JsonConvert.SerializeObject((object) arg, this.serializationSettings)})");
        }
        catch (Exception ex)
        {
          Debug.LogError((object) $"GorillaServer: {label} Error printing failure log: {ex}");
        }
      }
      cb(arg);
    });
  }

  private ExecuteFunctionResult toFunctionResult(PlayFab.ClientModels.ExecuteCloudScriptResult csResult)
  {
    FunctionExecutionError functionExecutionError = (FunctionExecutionError) null;
    if (csResult.Error != null)
      functionExecutionError = new FunctionExecutionError()
      {
        Error = csResult.Error.Error,
        Message = csResult.Error.Message,
        StackTrace = csResult.Error.StackTrace
      };
    ExecuteFunctionResult functionResult = new ExecuteFunctionResult();
    functionResult.CustomData = csResult.CustomData;
    functionResult.Error = functionExecutionError;
    functionResult.ExecutionTimeMilliseconds = Convert.ToInt32(Math.Round(csResult.ExecutionTimeSeconds * 1000.0));
    functionResult.FunctionName = csResult.FunctionName;
    functionResult.FunctionResult = csResult.FunctionResult;
    functionResult.FunctionResultTooLarge = csResult.FunctionResultTooLarge;
    return functionResult;
  }

  public void OnBeforeSerialize()
  {
    this.FeatureFlagsTitleDataKey = this.featureFlags.TitleDataKey;
    this.DefaultDeployFeatureFlagsEnabled.Clear();
    foreach (KeyValuePair<string, bool> keyValuePair in this.featureFlags.defaults)
    {
      if (keyValuePair.Value)
        this.DefaultDeployFeatureFlagsEnabled.Add(keyValuePair.Key);
    }
  }

  public void OnAfterDeserialize()
  {
    this.featureFlags.TitleDataKey = this.FeatureFlagsTitleDataKey;
    foreach (string key in this.DefaultDeployFeatureFlagsEnabled)
      this.featureFlags.defaults.AddOrUpdate<string, bool>(key, true);
  }

  public bool CheckIsInKIDOptInCohort() => this.featureFlags.IsEnabledForUser("2025-04-KIDOptIn");

  public bool CheckIsInKIDRequiredCohort()
  {
    return this.featureFlags.IsEnabledForUser("2025-04-KIDRequired");
  }

  public bool CheckOptedInKID() => KIDManager.HasOptedInToKID;

  public bool CheckIsTZE_Enabled()
  {
    return this.featureFlags.IsEnabledForUser("2025-10-TelemetryZoneEventSampling");
  }

  public bool CheckIsMothershipTelemetryEnabled()
  {
    return this.featureFlags.IsEnabledForUser("2025-09-MothershipAnalyticsSampleRate");
  }

  public bool CheckIsPlayFabTelemetryEnabled()
  {
    return this.featureFlags.IsEnabledForUser("2025-09-PlayFabAnalyticsSampleRate");
  }
}
