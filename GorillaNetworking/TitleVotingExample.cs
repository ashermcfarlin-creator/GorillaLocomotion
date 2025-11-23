// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.TitleVotingExample
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using JetBrains.Annotations;
using Newtonsoft.Json;
using Oculus.Platform;
using Oculus.Platform.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#nullable disable
namespace GorillaNetworking;

public class TitleVotingExample : MonoBehaviour
{
  private string Nonce = "";
  private int PollId = 5;
  private bool includeInactive = true;
  private int Option;
  private bool isPrediction;
  private int fetchPollsRetryCount;
  private int voteRetryCount;
  private int maxRetriesOnFail = 3;

  public async void Start()
  {
    await this.WaitForSessionToken();
    this.FetchPollsAndVote();
  }

  public void Update()
  {
  }

  private async Task WaitForSessionToken()
  {
    while (!(bool) (UnityEngine.Object) PlayFabAuthenticator.instance || PlayFabAuthenticator.instance.GetPlayFabPlayerId().IsNullOrEmpty() || PlayFabAuthenticator.instance.GetPlayFabSessionTicket().IsNullOrEmpty() || PlayFabAuthenticator.instance.userID.IsNullOrEmpty())
    {
      await Task.Yield();
      await Task.Delay(1000);
    }
  }

  public void FetchPollsAndVote()
  {
    this.StartCoroutine(this.DoFetchPolls(new TitleVotingExample.FetchPollsRequest()
    {
      TitleId = PlayFabAuthenticatorSettings.TitleId,
      PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
      PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
      IncludeInactive = this.includeInactive
    }, new Action<List<TitleVotingExample.FetchPollsResponse>>(this.OnFetchPollsResponse)));
  }

  private void GetNonceForVotingCallback([CanBeNull] Message<UserProof> message)
  {
    if (message != null)
      this.Nonce = message.Data?.ToString();
    this.StartCoroutine(this.DoVote(new TitleVotingExample.VoteRequest()
    {
      PollId = this.PollId,
      TitleId = PlayFabAuthenticatorSettings.TitleId,
      PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
      OculusId = PlayFabAuthenticator.instance.userID,
      UserPlatform = PlayFabAuthenticator.instance.platform.ToString(),
      UserNonce = this.Nonce,
      PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
      OptionIndex = this.Option,
      IsPrediction = this.isPrediction
    }, new Action<TitleVotingExample.VoteResponse>(this.OnVoteSuccess)));
  }

  public void Vote() => this.GetNonceForVotingCallback((Message<UserProof>) null);

  private IEnumerator DoFetchPolls(
    TitleVotingExample.FetchPollsRequest data,
    Action<List<TitleVotingExample.FetchPollsResponse>> callback)
  {
    UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.VotingApiBaseUrl + "/api/FetchPoll", "POST");
    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson((object) data));
    bool retry = false;
    request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bytes);
    request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");
    yield return (object) request.SendWebRequest();
    if (request.result == UnityWebRequest.Result.Success)
    {
      callback(JsonConvert.DeserializeObject<List<TitleVotingExample.FetchPollsResponse>>(request.downloadHandler.text));
    }
    else
    {
      Debug.LogError((object) ($"FetchPolls Error: {request.responseCode} -- raw response: " + request.downloadHandler.text));
      long responseCode = request.responseCode;
      if (responseCode >= 500L && responseCode < 600L)
      {
        retry = true;
        Debug.LogError((object) $"HTTP {request.responseCode} error: {request.error}");
      }
      else if (request.result == UnityWebRequest.Result.ConnectionError)
        retry = true;
    }
    if (retry)
    {
      if (this.fetchPollsRetryCount < this.maxRetriesOnFail)
      {
        int seconds = (int) Mathf.Pow(2f, (float) (this.fetchPollsRetryCount + 1));
        Debug.LogWarning((object) $"Retrying Title Voting FetchPolls... Retry attempt #{this.fetchPollsRetryCount + 1}, waiting for {seconds} seconds");
        ++this.fetchPollsRetryCount;
        yield return (object) new WaitForSeconds((float) seconds);
        this.FetchPollsAndVote();
      }
      else
      {
        Debug.LogError((object) "Maximum FetchPolls retries attempted. Please check your network connection.");
        this.fetchPollsRetryCount = 0;
        callback((List<TitleVotingExample.FetchPollsResponse>) null);
      }
    }
  }

  private IEnumerator DoVote(
    TitleVotingExample.VoteRequest data,
    Action<TitleVotingExample.VoteResponse> callback)
  {
    UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.VotingApiBaseUrl + "/api/Vote", "POST");
    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson((object) data));
    bool retry = false;
    request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bytes);
    request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");
    yield return (object) request.SendWebRequest();
    if (request.result == UnityWebRequest.Result.Success)
    {
      callback(JsonConvert.DeserializeObject<TitleVotingExample.VoteResponse>(request.downloadHandler.text));
    }
    else
    {
      Debug.LogError((object) ($"Vote Error: {request.responseCode} -- raw response: " + request.downloadHandler.text));
      long responseCode = request.responseCode;
      if (responseCode >= 500L && responseCode < 600L)
      {
        retry = true;
        Debug.LogError((object) $"HTTP {request.responseCode} error: {request.error}");
      }
      else if (request.responseCode == 409L)
        Debug.LogWarning((object) "User already voted on this poll!");
      else if (request.result == UnityWebRequest.Result.ConnectionError)
        retry = true;
    }
    if (retry)
    {
      if (this.voteRetryCount < this.maxRetriesOnFail)
      {
        int seconds = (int) Mathf.Pow(2f, (float) (this.voteRetryCount + 1));
        Debug.LogWarning((object) $"Retrying Voting... Retry attempt #{this.voteRetryCount + 1}, waiting for {seconds} seconds");
        ++this.voteRetryCount;
        yield return (object) new WaitForSeconds((float) seconds);
        this.Vote();
      }
      else
      {
        Debug.LogError((object) "Maximum Vote retries attempted. Please check your network connection.");
        this.voteRetryCount = 0;
        callback((TitleVotingExample.VoteResponse) null);
      }
    }
  }

  private void OnFetchPollsResponse(
    [CanBeNull] List<TitleVotingExample.FetchPollsResponse> response)
  {
    if (response != null)
    {
      Debug.Log((object) ("Got polls: " + JsonConvert.SerializeObject((object) response)));
      this.Vote();
    }
    else
      Debug.LogError((object) "Error: Could not fetch polls!");
  }

  private void OnVoteSuccess([CanBeNull] TitleVotingExample.VoteResponse response)
  {
    if (response != null)
      Debug.Log((object) ("Voted! " + JsonConvert.SerializeObject((object) response)));
    else
      Debug.LogError((object) "Error: Could not vote!");
  }

  [Serializable]
  private class FetchPollsRequest
  {
    public string TitleId;
    public string PlayFabId;
    public string PlayFabTicket;
    public bool IncludeInactive;
  }

  [Serializable]
  private class FetchPollsResponse
  {
    public int PollId;
    public string Question;
    public List<string> VoteOptions;
    public List<int> VoteCount;
    public List<int> PredictionCount;
    public DateTime StartTime;
    public DateTime EndTime;
  }

  [Serializable]
  private class VoteRequest
  {
    public int PollId;
    public string TitleId;
    public string PlayFabId;
    public string OculusId;
    public string UserNonce;
    public string UserPlatform;
    public int OptionIndex;
    public bool IsPrediction;
    public string PlayFabTicket;
  }

  [Serializable]
  private class VoteResponse
  {
    public int PollId { get; set; }

    public string TitleId { get; set; }

    public List<string> VoteOptions { get; set; }

    public List<int> VoteCount { get; set; }

    public List<int> PredictionCount { get; set; }
  }
}
