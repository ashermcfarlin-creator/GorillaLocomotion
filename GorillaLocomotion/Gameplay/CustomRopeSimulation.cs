// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.CustomRopeSimulation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Gameplay;

public class CustomRopeSimulation : MonoBehaviour
{
  private List<Transform> nodes = new List<Transform>();
  [SerializeField]
  private GameObject ropeNodePrefab;
  [SerializeField]
  private int nodeCount = 10;
  [SerializeField]
  private float nodeDistance = 0.4f;
  [SerializeField]
  private Vector3 gravity = new Vector3(0.0f, -9.81f, 0.0f);
  private NativeArray<BurstRopeNode> burstNodes;

  private void Start()
  {
    Vector3 position = this.transform.position;
    for (int index = 0; index < this.nodeCount; ++index)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(this.ropeNodePrefab);
      gameObject.transform.parent = this.transform;
      gameObject.transform.position = position;
      this.nodes.Add(gameObject.transform);
      position.y -= this.nodeDistance;
    }
    this.nodes[this.nodes.Count - 1].GetComponentInChildren<Renderer>().enabled = false;
    this.burstNodes = new NativeArray<BurstRopeNode>(this.nodes.Count, Allocator.Persistent);
  }

  private void OnDestroy() => this.burstNodes.Dispose();

  private void Update()
  {
    new SolveRopeJob()
    {
      fixedDeltaTime = Time.deltaTime,
      gravity = this.gravity,
      nodes = this.burstNodes,
      nodeDistance = this.nodeDistance,
      rootPos = this.transform.position
    }.Run<SolveRopeJob>();
    for (int index = 0; index < this.burstNodes.Length; ++index)
    {
      this.nodes[index].position = this.burstNodes[index].curPos;
      if (index > 0)
      {
        Vector3 vector3 = this.burstNodes[index - 1].curPos - this.burstNodes[index].curPos;
        this.nodes[index].up = -vector3;
      }
    }
  }
}
