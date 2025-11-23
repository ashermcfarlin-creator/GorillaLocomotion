// Decompiled with JetBrains decompiler
// Type: GorillaTag.Sports.SportScoreboard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Fusion;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable
namespace GorillaTag.Sports;

[RequireComponent(typeof (AudioSource))]
[NetworkBehaviourWeaved(2)]
public class SportScoreboard : NetworkComponent
{
  [OnEnterPlay_SetNull]
  public static SportScoreboard Instance;
  [SerializeField]
  private List<SportScoreboard.TeamParameters> teamParameters = new List<SportScoreboard.TeamParameters>();
  [SerializeField]
  private int matchEndScore = 3;
  [SerializeField]
  private float matchEndScoreResetDelayTime = 3f;
  private List<int> teamScores = new List<int>();
  private List<int> teamScoresPrev = new List<int>();
  private bool runningMatchEndCoroutine;
  private AudioSource audioSource;
  private SportScoreboardVisuals[] scoreVisuals;
  [WeaverGenerated]
  [SerializeField]
  [DefaultForProperty("Data", 0, 2)]
  [DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
  private int[] _Data;

  protected override void Awake()
  {
    base.Awake();
    SportScoreboard.Instance = this;
    this.audioSource = this.GetComponent<AudioSource>();
    this.scoreVisuals = new SportScoreboardVisuals[this.teamParameters.Count];
    for (int index = 0; index < this.teamParameters.Count; ++index)
    {
      this.teamScores.Add(0);
      this.teamScoresPrev.Add(0);
    }
  }

  public void RegisterTeamVisual(int TeamIndex, SportScoreboardVisuals visuals)
  {
    this.scoreVisuals[TeamIndex] = visuals;
    this.UpdateScoreboard();
  }

  private void UpdateScoreboard()
  {
    for (int index = 0; index < this.teamParameters.Count; ++index)
    {
      if (!((UnityEngine.Object) this.scoreVisuals[index] == (UnityEngine.Object) null))
      {
        int teamScore = this.teamScores[index];
        if ((UnityEngine.Object) this.scoreVisuals[index].score1s != (UnityEngine.Object) null)
          this.scoreVisuals[index].score1s.SetUVOffset(teamScore % 10);
        if ((UnityEngine.Object) this.scoreVisuals[index].score10s != (UnityEngine.Object) null)
          this.scoreVisuals[index].score10s.SetUVOffset(teamScore / 10 % 10);
      }
    }
  }

  private void OnScoreUpdated()
  {
    for (int index = 0; index < this.teamScores.Count; ++index)
    {
      if (this.teamScores[index] > this.teamScoresPrev[index] && (UnityEngine.Object) this.teamParameters[index].goalScoredAudio != (UnityEngine.Object) null && this.teamScores[index] < this.matchEndScore)
        this.audioSource.GTPlayOneShot(this.teamParameters[index].goalScoredAudio);
      this.teamScoresPrev[index] = this.teamScores[index];
    }
    if (!this.runningMatchEndCoroutine)
    {
      for (int index = 0; index < this.teamScores.Count; ++index)
      {
        if (this.teamScores[index] >= this.matchEndScore)
        {
          this.StartCoroutine(this.MatchEndCoroutine(index));
          break;
        }
      }
    }
    this.UpdateScoreboard();
  }

  public void TeamScored(int team)
  {
    if (!this.IsMine || this.runningMatchEndCoroutine)
      return;
    if (team >= 0 && team < this.teamScores.Count)
      this.teamScores[team] = this.teamScores[team] + 1;
    this.OnScoreUpdated();
  }

  public void ResetScores()
  {
    if (!this.IsMine || this.runningMatchEndCoroutine)
      return;
    for (int index = 0; index < this.teamScores.Count; ++index)
      this.teamScores[index] = 0;
    this.OnScoreUpdated();
  }

  private IEnumerator MatchEndCoroutine(int winningTeam)
  {
    this.runningMatchEndCoroutine = true;
    if (winningTeam >= 0 && winningTeam < this.teamParameters.Count && (UnityEngine.Object) this.teamParameters[winningTeam].matchWonAudio != (UnityEngine.Object) null)
      this.audioSource.GTPlayOneShot(this.teamParameters[winningTeam].matchWonAudio);
    yield return (object) new WaitForSeconds(this.matchEndScoreResetDelayTime);
    this.runningMatchEndCoroutine = false;
    this.ResetScores();
  }

  [Networked]
  [Capacity(2)]
  [NetworkedWeaved(0, 2)]
  [NetworkedWeavedArray(2, 1, typeof (ElementReaderWriterInt32))]
  public unsafe NetworkArray<int> Data
  {
    get
    {
      return (IntPtr) this.Ptr != IntPtr.Zero ? new NetworkArray<int>((byte*) (this.Ptr + 0), 2, ElementReaderWriterInt32.GetInstance()) : throw new InvalidOperationException("Error when accessing SportScoreboard.Data. Networked properties can only be accessed when Spawned() has been called.");
    }
  }

  public override void WriteDataFusion()
  {
    this.Data.CopyFrom(this.teamScores, 0, this.teamScores.Count);
  }

  public override void ReadDataFusion()
  {
    this.teamScores.Clear();
    this.Data.CopyTo(this.teamScores);
    this.OnScoreUpdated();
  }

  protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
  {
    for (int index = 0; index < this.teamScores.Count; ++index)
      stream.SendNext((object) this.teamScores[index]);
  }

  protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
  {
    for (int index = 0; index < this.teamScores.Count; ++index)
      this.teamScores[index] = (int) stream.ReceiveNext();
    this.OnScoreUpdated();
  }

  [WeaverGenerated]
  public override void CopyBackingFieldsToState([In] bool obj0)
  {
    base.CopyBackingFieldsToState(obj0);
    NetworkBehaviourUtils.InitializeNetworkArray<int>(this.Data, this._Data, "Data");
  }

  [WeaverGenerated]
  public override void CopyStateToBackingFields()
  {
    base.CopyStateToBackingFields();
    NetworkBehaviourUtils.CopyFromNetworkArray<int>(this.Data, ref this._Data);
  }

  [Serializable]
  private class TeamParameters
  {
    [SerializeField]
    public AudioClip matchWonAudio;
    [SerializeField]
    public AudioClip goalScoredAudio;
  }
}
