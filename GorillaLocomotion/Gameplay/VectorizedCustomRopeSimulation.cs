// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.VectorizedCustomRopeSimulation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Gameplay;

public class VectorizedCustomRopeSimulation : MonoBehaviour
{
  public static VectorizedCustomRopeSimulation instance;
  public const int MAX_NODE_COUNT = 32 /*0x20*/;
  public const float MAX_ROPE_SPEED = 15f;
  private List<Transform> nodes = new List<Transform>();
  [SerializeField]
  private float nodeDistance = 1f;
  [SerializeField]
  private int applyConstraintIterations = 20;
  [SerializeField]
  private int finalPassIterations = 1;
  [SerializeField]
  private float gravity = -0.15f;
  private VectorizedBurstRopeData burstData;
  private float lastDelta = 0.02f;
  private List<GorillaRopeSwing> ropes = new List<GorillaRopeSwing>();
  private static List<GorillaRopeSwing> registerQueue = new List<GorillaRopeSwing>();
  private static List<GorillaRopeSwing> deregisterQueue = new List<GorillaRopeSwing>();

  private void Awake() => VectorizedCustomRopeSimulation.instance = this;

  public static void Register(GorillaRopeSwing rope)
  {
    VectorizedCustomRopeSimulation.registerQueue.Add(rope);
  }

  public static void Unregister(GorillaRopeSwing rope)
  {
    VectorizedCustomRopeSimulation.deregisterQueue.Add(rope);
  }

  private void RegenerateData()
  {
    this.Dispose();
    foreach (GorillaRopeSwing register in VectorizedCustomRopeSimulation.registerQueue)
      this.ropes.Add(register);
    foreach (GorillaRopeSwing deregister in VectorizedCustomRopeSimulation.deregisterQueue)
      this.ropes.Remove(deregister);
    VectorizedCustomRopeSimulation.registerQueue.Clear();
    VectorizedCustomRopeSimulation.deregisterQueue.Clear();
    int count = this.ropes.Count;
    while (count % 4 != 0)
      ++count;
    int length = count * 32 /*0x20*/ / 4;
    this.burstData = new VectorizedBurstRopeData()
    {
      posX = new NativeArray<float4>(length, Allocator.Persistent),
      posY = new NativeArray<float4>(length, Allocator.Persistent),
      posZ = new NativeArray<float4>(length, Allocator.Persistent),
      validNodes = new NativeArray<int4>(length, Allocator.Persistent),
      lastPosX = new NativeArray<float4>(length, Allocator.Persistent),
      lastPosY = new NativeArray<float4>(length, Allocator.Persistent),
      lastPosZ = new NativeArray<float4>(length, Allocator.Persistent),
      ropeRoots = new NativeArray<float3>(count, Allocator.Persistent),
      nodeMass = new NativeArray<float4>(length, Allocator.Persistent)
    };
    for (int index1 = 0; index1 < this.ropes.Count; index1 += 4)
    {
      for (int index2 = 0; index2 < 4 && this.ropes.Count > index1 + index2; ++index2)
      {
        this.ropes[index1 + index2].ropeDataStartIndex = 32 /*0x20*/ * index1 / 4;
        this.ropes[index1 + index2].ropeDataIndexOffset = index2;
      }
    }
    int num = 0;
    for (int index3 = 0; index3 < length; ++index3)
    {
      float4 float4_1 = this.burstData.posX[index3];
      float4 float4_2 = this.burstData.posY[index3];
      float4 float4_3 = this.burstData.posZ[index3];
      int4 validNode = this.burstData.validNodes[index3];
      for (int index4 = 0; index4 < 4; ++index4)
      {
        int index5 = num * 4 + index4;
        int index6 = index3 - num * 32 /*0x20*/;
        if (this.ropes.Count > index5 && this.ropes[index5].nodes.Length > index6)
        {
          Vector3 localPosition = this.ropes[index5].nodes[index6].localPosition;
          float4_1[index4] = localPosition.x;
          float4_2[index4] = localPosition.y;
          float4_3[index4] = localPosition.z;
          validNode[index4] = 1;
        }
        else
        {
          float4_1[index4] = 0.0f;
          float4_2[index4] = 0.0f;
          float4_3[index4] = 0.0f;
          validNode[index4] = 0;
        }
      }
      if (index3 > 0 && (index3 + 1) % 32 /*0x20*/ == 0)
        ++num;
      this.burstData.posX[index3] = float4_1;
      this.burstData.posY[index3] = float4_2;
      this.burstData.posZ[index3] = float4_3;
      this.burstData.lastPosX[index3] = float4_1;
      this.burstData.lastPosY[index3] = float4_2;
      this.burstData.lastPosZ[index3] = float4_3;
      this.burstData.validNodes[index3] = validNode;
      this.burstData.nodeMass[index3] = math.float4(1f, 1f, 1f, 1f);
    }
    for (int index = 0; index < this.ropes.Count; ++index)
      this.burstData.ropeRoots[index] = float3.zero;
  }

  private void Dispose()
  {
    if (!this.burstData.posX.IsCreated)
      return;
    this.burstData.posX.Dispose();
    this.burstData.posY.Dispose();
    this.burstData.posZ.Dispose();
    this.burstData.validNodes.Dispose();
    this.burstData.lastPosX.Dispose();
    this.burstData.lastPosY.Dispose();
    this.burstData.lastPosZ.Dispose();
    this.burstData.ropeRoots.Dispose();
    this.burstData.nodeMass.Dispose();
  }

  private void OnDestroy() => this.Dispose();

  public void SetRopePos(
    GorillaRopeSwing ropeTarget,
    Vector3[] positions,
    bool setCurPos,
    bool setLastPos,
    int onlySetIndex = -1)
  {
    if (!this.ropes.Contains(ropeTarget))
      return;
    int ropeDataIndexOffset = ropeTarget.ropeDataIndexOffset;
    for (int index1 = 0; index1 < positions.Length; ++index1)
    {
      if (onlySetIndex < 0 || index1 == onlySetIndex)
      {
        int index2 = ropeTarget.ropeDataStartIndex + index1;
        if (setCurPos)
        {
          float4 float4_1 = this.burstData.posX[index2];
          float4 float4_2 = this.burstData.posY[index2];
          float4 float4_3 = this.burstData.posZ[index2];
          float4_1[ropeDataIndexOffset] = positions[index1].x;
          float4_2[ropeDataIndexOffset] = positions[index1].y;
          float4_3[ropeDataIndexOffset] = positions[index1].z;
          this.burstData.posX[index2] = float4_1;
          this.burstData.posY[index2] = float4_2;
          this.burstData.posZ[index2] = float4_3;
        }
        if (setLastPos)
        {
          float4 float4_4 = this.burstData.lastPosX[index2];
          float4 float4_5 = this.burstData.lastPosY[index2];
          float4 float4_6 = this.burstData.lastPosZ[index2];
          float4_4[ropeDataIndexOffset] = positions[index1].x;
          float4_5[ropeDataIndexOffset] = positions[index1].y;
          float4_6[ropeDataIndexOffset] = positions[index1].z;
          this.burstData.lastPosX[index2] = float4_4;
          this.burstData.lastPosY[index2] = float4_5;
          this.burstData.lastPosZ[index2] = float4_6;
        }
      }
    }
  }

  public void SetVelocity(
    GorillaRopeSwing ropeTarget,
    Vector3 velocity,
    bool wholeRope,
    int boneIndex = 1)
  {
    List<Vector3> vector3List = new List<Vector3>();
    float maxLength = math.min(velocity.magnitude, 15f);
    int ropeDataStartIndex = ropeTarget.ropeDataStartIndex;
    int ropeDataIndexOffset = ropeTarget.ropeDataIndexOffset;
    if (ropeTarget.SupportsMovingAtRuntime)
      velocity = Quaternion.Inverse(ropeTarget.transform.rotation) * velocity;
    for (int index = 0; index < ropeTarget.nodes.Length; ++index)
    {
      Vector3 vector3_1;
      ref Vector3 local = ref vector3_1;
      float4 float4 = this.burstData.lastPosX[ropeDataStartIndex + index];
      double x = (double) float4[ropeDataIndexOffset];
      float4 = this.burstData.lastPosY[ropeDataStartIndex + index];
      double y = (double) float4[ropeDataIndexOffset];
      float4 = this.burstData.lastPosZ[ropeDataStartIndex + index];
      double z = (double) float4[ropeDataIndexOffset];
      local = new Vector3((float) x, (float) y, (float) z);
      if ((wholeRope || boneIndex == index) && boneIndex > 0)
      {
        Vector3 vector3_2 = Vector3.ClampMagnitude(velocity / (float) boneIndex * (float) index, maxLength);
        vector3List.Add(vector3_1 += vector3_2 * this.lastDelta);
      }
      else
        vector3List.Add(vector3_1);
    }
    int onlySetIndex = -1;
    if (!wholeRope && boneIndex > 0)
      onlySetIndex = boneIndex;
    this.SetRopePos(ropeTarget, vector3List.ToArray(), true, false, onlySetIndex);
  }

  public Vector3 GetNodeVelocity(GorillaRopeSwing ropeTarget, int nodeIndex)
  {
    int index = ropeTarget.ropeDataStartIndex + nodeIndex;
    int ropeDataIndexOffset = ropeTarget.ropeDataIndexOffset;
    float4 float4 = this.burstData.posX[index];
    double x1 = (double) float4[ropeDataIndexOffset];
    float4 = this.burstData.posY[index];
    double y1 = (double) float4[ropeDataIndexOffset];
    float4 = this.burstData.posZ[index];
    double z1 = (double) float4[ropeDataIndexOffset];
    Vector3 vector3_1 = new Vector3((float) x1, (float) y1, (float) z1);
    Vector3 vector3_2;
    ref Vector3 local = ref vector3_2;
    float4 = this.burstData.lastPosX[index];
    double x2 = (double) float4[ropeDataIndexOffset];
    float4 = this.burstData.lastPosY[index];
    double y2 = (double) float4[ropeDataIndexOffset];
    float4 = this.burstData.lastPosZ[index];
    double z2 = (double) float4[ropeDataIndexOffset];
    local = new Vector3((float) x2, (float) y2, (float) z2);
    Vector3 vector3_3 = vector3_2;
    Vector3 nodeVelocity = (vector3_1 - vector3_3) / this.lastDelta;
    if (ropeTarget.SupportsMovingAtRuntime)
      nodeVelocity = ropeTarget.transform.rotation * nodeVelocity;
    return nodeVelocity;
  }

  public void SetMassForPlayers(
    GorillaRopeSwing ropeTarget,
    bool hasPlayers,
    int furthestBoneIndex = 0)
  {
    if (!this.ropes.Contains(ropeTarget))
      return;
    int ropeDataIndexOffset = ropeTarget.ropeDataIndexOffset;
    for (int index1 = 0; index1 < 32 /*0x20*/; ++index1)
    {
      int index2 = ropeTarget.ropeDataStartIndex + index1;
      float4 float4 = this.burstData.nodeMass[index2];
      float4[ropeDataIndexOffset] = !hasPlayers || index1 != furthestBoneIndex + 1 ? 1f : 0.1f;
      this.burstData.nodeMass[index2] = float4;
    }
  }

  private void Update()
  {
    if (VectorizedCustomRopeSimulation.registerQueue.Count > 0 || VectorizedCustomRopeSimulation.deregisterQueue.Count > 0)
      this.RegenerateData();
    if (this.ropes.Count <= 0)
      return;
    float num = math.min(Time.deltaTime, 0.05f);
    VectorizedSolveRopeJob jobData = new VectorizedSolveRopeJob()
    {
      applyConstraintIterations = this.applyConstraintIterations,
      finalPassIterations = this.finalPassIterations,
      lastDeltaTime = this.lastDelta,
      deltaTime = num,
      gravity = this.gravity,
      data = this.burstData,
      nodeDistance = this.nodeDistance,
      ropeCount = this.ropes.Count
    };
    jobData.Schedule<VectorizedSolveRopeJob>().Complete();
    for (int index1 = 0; index1 < this.ropes.Count; ++index1)
    {
      GorillaRopeSwing rope = this.ropes[index1];
      if (!rope.isIdle || !rope.isFullyIdle)
      {
        int ropeDataIndexOffset = rope.ropeDataIndexOffset;
        for (int index2 = 0; index2 < rope.nodes.Length; ++index2)
        {
          int index3 = rope.ropeDataStartIndex + index2;
          Transform node = rope.nodes[index2];
          float4 float4 = jobData.data.posX[index3];
          double x = (double) float4[ropeDataIndexOffset];
          float4 = jobData.data.posY[index3];
          double y = (double) float4[ropeDataIndexOffset];
          float4 = jobData.data.posZ[index3];
          double z = (double) float4[ropeDataIndexOffset];
          Vector3 vector3 = new Vector3((float) x, (float) y, (float) z);
          node.localPosition = vector3;
        }
        if (rope.SupportsMovingAtRuntime)
        {
          for (int index4 = 0; index4 < rope.nodes.Length - 1; ++index4)
          {
            Transform node = rope.nodes[index4];
            int ropeDataStartIndex = rope.ropeDataStartIndex;
            node.up = rope.transform.rotation * -(rope.nodes[index4 + 1].localPosition - node.localPosition);
          }
        }
        else
        {
          for (int index5 = 0; index5 < rope.nodes.Length - 1; ++index5)
          {
            Transform node = rope.nodes[index5];
            int ropeDataStartIndex = rope.ropeDataStartIndex;
            node.up = -(rope.nodes[index5 + 1].localPosition - node.localPosition);
          }
        }
      }
    }
    this.lastDelta = num;
  }
}
