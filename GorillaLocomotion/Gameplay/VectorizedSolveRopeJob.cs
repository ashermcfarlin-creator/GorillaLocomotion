// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.VectorizedSolveRopeJob
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

#nullable disable
namespace GorillaLocomotion.Gameplay;

[BurstCompile(FloatPrecision.Low, FloatMode.Fast)]
public struct VectorizedSolveRopeJob : IJob
{
  [ReadOnly]
  public int applyConstraintIterations;
  [ReadOnly]
  public int finalPassIterations;
  [ReadOnly]
  public float deltaTime;
  [ReadOnly]
  public float lastDeltaTime;
  [ReadOnly]
  public int ropeCount;
  public VectorizedBurstRopeData data;
  [ReadOnly]
  public float gravity;
  [ReadOnly]
  public float nodeDistance;

  public void Execute()
  {
    this.Simulate();
    for (int index = 0; index < this.applyConstraintIterations; ++index)
      this.ApplyConstraint();
    for (int index = 0; index < this.finalPassIterations; ++index)
      this.FinalPass();
  }

  private void Simulate()
  {
    for (int index = 0; index < this.data.posX.Length; ++index)
    {
      float4 float4_1 = (this.data.posX[index] - this.data.lastPosX[index]) / this.lastDeltaTime;
      float4 float4_2 = (this.data.posY[index] - this.data.lastPosY[index]) / this.lastDeltaTime;
      float4 float4_3 = (this.data.posZ[index] - this.data.lastPosZ[index]) / this.lastDeltaTime;
      this.data.lastPosX[index] = this.data.posX[index];
      this.data.lastPosY[index] = this.data.posY[index];
      this.data.lastPosZ[index] = this.data.posZ[index];
      float4 float4_4 = this.data.lastPosX[index] + float4_1 * this.deltaTime * 0.996f;
      float4 float4_5 = this.data.lastPosY[index] + float4_2 * this.deltaTime;
      float4 float4_6 = this.data.lastPosZ[index] + float4_3 * this.deltaTime * 0.996f;
      float4 float4_7 = float4_5 + this.gravity * this.deltaTime;
      this.data.posX[index] = float4_4 * (float4) this.data.validNodes[index];
      this.data.posY[index] = float4_7 * (float4) this.data.validNodes[index];
      this.data.posZ[index] = float4_6 * (float4) this.data.validNodes[index];
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static void dot4(
    ref float4 ax,
    ref float4 ay,
    ref float4 az,
    ref float4 bx,
    ref float4 by,
    ref float4 bz,
    ref float4 output)
  {
    output = ax * bx + ay * by + az * bz;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static void length4(
    ref float4 xVals,
    ref float4 yVals,
    ref float4 zVals,
    ref float4 output)
  {
    float4 zero = float4.zero;
    VectorizedSolveRopeJob.dot4(ref xVals, ref yVals, ref zVals, ref xVals, ref yVals, ref zVals, ref zero);
    float4 x = math.abs(zero);
    output = math.sqrt(x);
  }

  private void ConstrainRoots()
  {
    int index1 = 0;
    for (int index2 = 0; index2 < this.data.posX.Length; index2 += 32 /*0x20*/)
    {
      for (int index3 = 0; index3 < 4; ++index3)
      {
        float4 float4_1 = this.data.posX[index2];
        float4 float4_2 = this.data.posY[index2];
        float4 float4_3 = this.data.posZ[index2];
        float4_1[index3] = this.data.ropeRoots[index1].x;
        float4_2[index3] = this.data.ropeRoots[index1].y;
        float4_3[index3] = this.data.ropeRoots[index1].z;
        this.data.posX[index2] = float4_1;
        this.data.posY[index2] = float4_2;
        this.data.posZ[index2] = float4_3;
        ++index1;
      }
    }
  }

  private void ApplyConstraint()
  {
    this.ConstrainRoots();
    float4 float4_1 = (float4) math.int4(-1, -1, -1, -1);
    for (int index1 = 0; index1 < this.ropeCount; index1 += 4)
    {
      for (int index2 = 0; index2 < 31 /*0x1F*/; ++index2)
      {
        int index3 = index1 / 4 * 32 /*0x20*/ + index2;
        float4 validNode1 = (float4) this.data.validNodes[index3];
        float4 validNode2 = (float4) this.data.validNodes[index3 + 1];
        if ((double) math.lengthsq(validNode2) >= 0.10000000149011612)
        {
          float4 zero = float4.zero;
          float4 xVals = this.data.posX[index3] - this.data.posX[index3 + 1];
          float4 yVals = this.data.posY[index3] - this.data.posY[index3 + 1];
          float4 zVals = this.data.posZ[index3] - this.data.posZ[index3 + 1];
          VectorizedSolveRopeJob.length4(ref xVals, ref yVals, ref zVals, ref zero);
          float4 float4_2 = math.abs(zero - this.nodeDistance);
          float4 float4_3 = math.sign(zero - this.nodeDistance);
          float4 float4_4 = zero + (validNode1 - float4_1) + 0.01f;
          float4 float4_5 = xVals / float4_4;
          float4 float4_6 = yVals / float4_4;
          float4 float4_7 = zVals / float4_4;
          float4 float4_8 = float4_3 * float4_5 * float4_2;
          float4 float4_9 = float4_3 * float4_6 * float4_2;
          float4 float4_10 = float4_3 * float4_7 * float4_2;
          float4 float4_11 = this.data.nodeMass[index3] / (this.data.nodeMass[index3] + this.data.nodeMass[index3 + 1]);
          float4 float4_12 = this.data.nodeMass[index3 + 1] / (this.data.nodeMass[index3] + this.data.nodeMass[index3 + 1]);
          this.data.posX[index3] -= float4_8 * validNode2 * float4_11;
          this.data.posY[index3] -= float4_9 * validNode2 * float4_11;
          this.data.posZ[index3] -= float4_10 * validNode2 * float4_11;
          this.data.posX[index3 + 1] += float4_8 * validNode2 * float4_12;
          this.data.posY[index3 + 1] += float4_9 * validNode2 * float4_12;
          this.data.posZ[index3 + 1] += float4_10 * validNode2 * float4_12;
        }
      }
    }
  }

  private void FinalPass()
  {
    this.ConstrainRoots();
    float4 float4_1 = (float4) math.int4(-1, -1, -1, -1);
    for (int index1 = 0; index1 < this.ropeCount; index1 += 4)
    {
      for (int index2 = 0; index2 < 31 /*0x1F*/; ++index2)
      {
        int index3 = index1 / 4 * 32 /*0x20*/ + index2;
        float4 validNode1 = (float4) this.data.validNodes[index3];
        float4 validNode2 = (float4) this.data.validNodes[index3 + 1];
        float4 zero = float4.zero;
        float4 xVals = this.data.posX[index3] - this.data.posX[index3 + 1];
        float4 yVals = this.data.posY[index3] - this.data.posY[index3 + 1];
        float4 zVals = this.data.posZ[index3] - this.data.posZ[index3 + 1];
        VectorizedSolveRopeJob.length4(ref xVals, ref yVals, ref zVals, ref zero);
        float4 float4_2 = math.abs(zero - this.nodeDistance);
        float4 float4_3 = math.sign(zero - this.nodeDistance);
        float4 float4_4 = zero + ((float4) this.data.validNodes[index3] - float4_1) + 0.01f;
        float4 float4_5 = xVals / float4_4;
        float4 float4_6 = yVals / float4_4;
        float4 float4_7 = zVals / float4_4;
        float4 float4_8 = float4_3 * float4_5 * float4_2;
        float4 float4_9 = float4_3 * float4_6 * float4_2;
        float4 float4_10 = float4_3 * float4_7 * float4_2;
        this.data.posX[index3 + 1] += float4_8 * validNode2;
        this.data.posY[index3 + 1] += float4_9 * validNode2;
        this.data.posZ[index3 + 1] += float4_10 * validNode2;
      }
    }
  }
}
