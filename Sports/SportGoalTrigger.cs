// Decompiled with JetBrains decompiler
// Type: GorillaTag.Sports.SportGoalTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag.Sports;

public class SportGoalTrigger : MonoBehaviour
{
  [SerializeField]
  private SportScoreboard scoreboard;
  [SerializeField]
  private int teamScoringOnThisGoal = 1;
  [SerializeField]
  private float ballTriggerExitDistanceFallback = 3f;
  private HashSet<SportBall> ballsPendingTriggerExit = new HashSet<SportBall>();

  public void BallExitedGoalTrigger(SportBall ball)
  {
    if (!this.ballsPendingTriggerExit.Contains(ball))
      return;
    this.ballsPendingTriggerExit.Remove(ball);
  }

  private void PruneBallsPendingTriggerExitByDistance()
  {
    foreach (SportBall sportBall in this.ballsPendingTriggerExit)
    {
      if ((double) (sportBall.transform.position - this.transform.position).sqrMagnitude > (double) this.ballTriggerExitDistanceFallback * (double) this.ballTriggerExitDistanceFallback)
        this.ballsPendingTriggerExit.Remove(sportBall);
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    SportBall componentInParent = other.GetComponentInParent<SportBall>();
    if (!((Object) componentInParent != (Object) null) || !((Object) this.scoreboard != (Object) null))
      return;
    this.PruneBallsPendingTriggerExitByDistance();
    if (this.ballsPendingTriggerExit.Contains(componentInParent))
      return;
    this.scoreboard.TeamScored(this.teamScoringOnThisGoal);
    this.ballsPendingTriggerExit.Add(componentInParent);
  }
}
