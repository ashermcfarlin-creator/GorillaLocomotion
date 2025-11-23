// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.GhostReactorProgression
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaNetworking;

public class GhostReactorProgression : MonoBehaviour
{
  public static GhostReactorProgression instance;
  private string progressionTrackId = "a0208736-e696-489b-81cd-c0c772489cc5";
  private GRPlayer _grPlayer;
  private GhostReactor _reactor;
  public static GRProgressionScriptableObject grPSO;
  public const string grPSODirectory = "ProgressionTiersData";

  public void Awake() => GhostReactorProgression.instance = this;

  public void Start()
  {
    if ((UnityEngine.Object) ProgressionManager.Instance != (UnityEngine.Object) null)
    {
      ProgressionManager.Instance.OnTrackRead += new Action<string, int>(this.OnTrackRead);
      ProgressionManager.Instance.OnTrackSet += new Action<string, int>(this.OnTrackSet);
      ProgressionManager.Instance.OnNodeUnlocked += (Action<string, string>) ((a, b) => this.OnNodeUnlocked());
    }
    else
      Debug.Log((object) "GRP: ProgressionManager is null!");
  }

  public async void GetStartingProgression(GRPlayer grPlayer)
  {
    await ProgressionUtil.WaitForMothershipSessionToken();
    this._grPlayer = grPlayer;
    ProgressionManager.Instance.GetProgression(this.progressionTrackId);
    if (!this._grPlayer.gamePlayer.IsLocal())
      return;
    this._grPlayer.mothershipId = MothershipClientContext.MothershipId;
    ProgressionManager.Instance.GetShiftCredit(this._grPlayer.mothershipId);
  }

  public void SetProgression(int progressionAmountToAdd, GRPlayer grPlayer)
  {
    this._grPlayer = grPlayer;
    ProgressionManager.Instance.SetProgression(this.progressionTrackId, progressionAmountToAdd);
  }

  public void UnlockProgressionTreeNode(string treeId, string nodeId, GhostReactor reactor)
  {
    this._reactor = reactor;
    ProgressionManager.Instance.UnlockNode(treeId, nodeId);
  }

  private void OnTrackRead(string trackId, int progress)
  {
    if ((UnityEngine.Object) this._grPlayer == (UnityEngine.Object) null)
      Debug.Log((object) "GRP: OnTrackRead Failure: player is null");
    else if (trackId != this.progressionTrackId)
      Debug.Log((object) $"GRP: OnTrackRead Failure: track [{trackId}] progressionTrack [{this.progressionTrackId}] progress {progress}");
    else
      this._grPlayer.SetProgressionData(progress, progress);
  }

  private void OnTrackSet(string trackId, int progress)
  {
    if ((UnityEngine.Object) this._grPlayer == (UnityEngine.Object) null || trackId != this.progressionTrackId)
      return;
    this._grPlayer.SetProgressionData(progress, this._grPlayer.CurrentProgression.redeemedPoints);
  }

  private void OnNodeUnlocked()
  {
    if (!((UnityEngine.Object) this._reactor != (UnityEngine.Object) null) || !((UnityEngine.Object) this._reactor.toolProgression != (UnityEngine.Object) null))
      return;
    this._reactor.toolProgression.UpdateInventory();
    this._reactor.toolProgression.SetPendingTreeToProcess();
    this._reactor.UpdateLocalPlayerFromProgression();
  }

  public static (int tier, int grade, int totalPointsToNextLevel, int partialPointsToNextLevel) GetGradePointDetails(
    int points)
  {
    GhostReactorProgression.LoadGRPSO();
    int num1 = 0;
    int num2 = 0;
    int index;
    for (index = 0; index < GhostReactorProgression.grPSO.progressionData.Count; ++index)
    {
      num2 = num1;
      num1 += GhostReactorProgression.grPSO.progressionData[index].grades * GhostReactorProgression.grPSO.progressionData[index].pointsPerGrade;
      if (points < num1)
        break;
    }
    if (points > num1)
      return (index - 1, 0, 0, 0);
    int pointsPerGrade = GhostReactorProgression.grPSO.progressionData[index].pointsPerGrade;
    int num3 = (points - num2) / pointsPerGrade;
    int num4 = (points - num2) % pointsPerGrade;
    return (index, num3, pointsPerGrade, num4);
  }

  public static string GetTitleNameAndGrade(int points)
  {
    GhostReactorProgression.LoadGRPSO();
    int num = 0;
    for (int index = 0; index < GhostReactorProgression.grPSO.progressionData.Count; ++index)
    {
      num += GhostReactorProgression.grPSO.progressionData[index].grades * GhostReactorProgression.grPSO.progressionData[index].pointsPerGrade;
      if (points < num)
        return $"{GhostReactorProgression.grPSO.progressionData[index].tierName} {(GhostReactorProgression.grPSO.progressionData[index].grades - Mathf.FloorToInt((float) ((num - points) / GhostReactorProgression.grPSO.progressionData[index].pointsPerGrade)) + 1).ToString()}";
    }
    return "null";
  }

  public static string GetTitleName(int points)
  {
    GhostReactorProgression.LoadGRPSO();
    int num = 0;
    for (int index = 0; index < GhostReactorProgression.grPSO.progressionData.Count; ++index)
    {
      num += GhostReactorProgression.grPSO.progressionData[index].grades * GhostReactorProgression.grPSO.progressionData[index].pointsPerGrade;
      if (points < num)
        return GhostReactorProgression.grPSO.progressionData[index].tierName;
    }
    return "null";
  }

  public static string GetTitleNameFromLevel(int level)
  {
    GhostReactorProgression.LoadGRPSO();
    for (int index = 0; index < GhostReactorProgression.grPSO.progressionData.Count; ++index)
    {
      if (GhostReactorProgression.grPSO.progressionData[index].tierId >= level)
        return GhostReactorProgression.grPSO.progressionData[index].tierName;
    }
    return "null";
  }

  public static int GetGrade(int points)
  {
    GhostReactorProgression.LoadGRPSO();
    int num = 0;
    for (int index = 0; index < GhostReactorProgression.grPSO.progressionData.Count; ++index)
    {
      num += GhostReactorProgression.grPSO.progressionData[index].grades * GhostReactorProgression.grPSO.progressionData[index].pointsPerGrade;
      if (points < num)
        return GhostReactorProgression.grPSO.progressionData[index].grades - Mathf.FloorToInt((float) ((num - points) / GhostReactorProgression.grPSO.progressionData[index].pointsPerGrade)) + 1;
    }
    return -1;
  }

  public static int GetTitleLevel(int points)
  {
    GhostReactorProgression.LoadGRPSO();
    int num = 0;
    for (int index = 0; index < GhostReactorProgression.grPSO.progressionData.Count; ++index)
    {
      num += GhostReactorProgression.grPSO.progressionData[index].grades * GhostReactorProgression.grPSO.progressionData[index].pointsPerGrade;
      if (points < num)
        return GhostReactorProgression.grPSO.progressionData[index].tierId;
    }
    return -1;
  }

  public static void LoadGRPSO()
  {
    if (!((UnityEngine.Object) GhostReactorProgression.grPSO == (UnityEngine.Object) null))
      return;
    GhostReactorProgression.grPSO = Resources.Load<GRProgressionScriptableObject>("ProgressionTiersData");
  }
}
