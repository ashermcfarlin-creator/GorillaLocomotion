// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.VectorizedBurstRopeData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Unity.Collections;
using Unity.Mathematics;

#nullable disable
namespace GorillaLocomotion.Gameplay;

public struct VectorizedBurstRopeData
{
  public NativeArray<float4> posX;
  public NativeArray<float4> posY;
  public NativeArray<float4> posZ;
  public NativeArray<int4> validNodes;
  public NativeArray<float4> lastPosX;
  public NativeArray<float4> lastPosY;
  public NativeArray<float4> lastPosZ;
  public NativeArray<float3> ropeRoots;
  public NativeArray<float4> nodeMass;
}
