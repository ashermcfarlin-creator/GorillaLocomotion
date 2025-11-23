// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.SolveRopeJob
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Gameplay;

[BurstCompile]
public struct SolveRopeJob : IJob
{
  [ReadOnly]
  public float fixedDeltaTime;
  [WriteOnly]
  public NativeArray<BurstRopeNode> nodes;
  [ReadOnly]
  public Vector3 gravity;
  [ReadOnly]
  public Vector3 rootPos;
  [ReadOnly]
  public float nodeDistance;

  public void Execute()
  {
    this.Simulate();
    for (int index = 0; index < 20; ++index)
      this.ApplyConstraint();
  }

  private void Simulate()
  {
    for (int index = 0; index < this.nodes.Length; ++index)
    {
      BurstRopeNode node = this.nodes[index];
      Vector3 vector3_1 = node.curPos - node.lastPos;
      node.lastPos = node.curPos;
      Vector3 vector3_2 = node.curPos + vector3_1 + this.gravity * this.fixedDeltaTime;
      node.curPos = vector3_2;
      this.nodes[index] = node;
    }
  }

  private void ApplyConstraint()
  {
    this.nodes[0] = this.nodes[0] with
    {
      curPos = this.rootPos
    };
    for (int index = 0; index < this.nodes.Length - 1; ++index)
    {
      BurstRopeNode node1 = this.nodes[index];
      BurstRopeNode node2 = this.nodes[index + 1];
      float magnitude = (node1.curPos - node2.curPos).magnitude;
      float num = Mathf.Abs(magnitude - this.nodeDistance);
      Vector3 vector3_1 = Vector3.zero;
      if ((double) magnitude > (double) this.nodeDistance)
        vector3_1 = (node1.curPos - node2.curPos).normalized;
      else if ((double) magnitude < (double) this.nodeDistance)
        vector3_1 = (node2.curPos - node1.curPos).normalized;
      Vector3 vector3_2 = vector3_1 * num;
      node1.curPos -= vector3_2 * 0.5f;
      node2.curPos += vector3_2 * 0.5f;
      this.nodes[index] = node1;
      this.nodes[index + 1] = node2;
    }
  }
}
