// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.RopeSwingManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Fusion;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

#nullable disable
namespace GorillaLocomotion.Gameplay;

public class RopeSwingManager : NetworkSceneObject
{
  private Dictionary<int, GorillaRopeSwing> ropes = new Dictionary<int, GorillaRopeSwing>();

  public static RopeSwingManager instance { get; private set; }

  private void Awake()
  {
    if ((UnityEngine.Object) RopeSwingManager.instance != (UnityEngine.Object) null && (UnityEngine.Object) RopeSwingManager.instance != (UnityEngine.Object) this)
    {
      GTDev.LogWarning<string>("Instance of RopeSwingManager already exists. Destroying.");
      UnityEngine.Object.Destroy((UnityEngine.Object) this);
    }
    else
    {
      if (!((UnityEngine.Object) RopeSwingManager.instance == (UnityEngine.Object) null))
        return;
      RopeSwingManager.instance = this;
    }
  }

  private void RegisterInstance(GorillaRopeSwing t) => this.ropes.Add(t.ropeId, t);

  private void UnregisterInstance(GorillaRopeSwing t) => this.ropes.Remove(t.ropeId);

  public static void Register(GorillaRopeSwing t) => RopeSwingManager.instance.RegisterInstance(t);

  public static void Unregister(GorillaRopeSwing t)
  {
    RopeSwingManager.instance.UnregisterInstance(t);
  }

  public void SendSetVelocity_RPC(int ropeId, int boneIndex, Vector3 velocity, bool wholeRope)
  {
    if (NetworkSystem.Instance.InRoom)
      this.photonView.RPC("SetVelocity", RpcTarget.All, (object) ropeId, (object) boneIndex, (object) velocity, (object) wholeRope);
    else
      this.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, new PhotonMessageInfoWrapped());
  }

  public bool TryGetRope(int ropeId, out GorillaRopeSwing result)
  {
    return this.ropes.TryGetValue(ropeId, out result);
  }

  [PunRPC]
  public void SetVelocity(
    int ropeId,
    int boneIndex,
    Vector3 velocity,
    bool wholeRope,
    PhotonMessageInfo info)
  {
    PhotonMessageInfoWrapped info1 = new PhotonMessageInfoWrapped(info);
    this.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, info1);
    Utils.Log((object) "Receiving RPC for ropes");
  }

  [Rpc]
  public static unsafe void RPC_SetVelocity(
    NetworkRunner runner,
    int ropeId,
    int boneIndex,
    Vector3 velocity,
    bool wholeRope,
    RpcInfo info = default (RpcInfo))
  {
    if (NetworkBehaviourUtils.InvokeRpc)
      NetworkBehaviourUtils.InvokeRpc = false;
    else
      goto label_3;
label_2:
    PhotonMessageInfoWrapped info1 = new PhotonMessageInfoWrapped(info);
    RopeSwingManager.instance.SetVelocityShared(ropeId, boneIndex, velocity, wholeRope, info1);
    return;
label_3:
    if (runner == null)
      throw new ArgumentNullException(nameof (runner));
    if (runner.Stage == SimulationStages.Resimulate)
      return;
    int num1 = 8 + 4 + 4 + 12 + 4;
    if (!SimulationMessage.CanAllocateUserPayload(num1))
    {
      NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaLocomotion.Gameplay.RopeSwingManager::RPC_SetVelocity(Fusion.NetworkRunner,System.Int32,System.Int32,UnityEngine.Vector3,System.Boolean,Fusion.RpcInfo)", num1);
    }
    else
    {
      if (runner.HasAnyActiveConnections())
      {
        SimulationMessage* simulationMessagePtr = SimulationMessage.Allocate(runner.Simulation, num1);
        byte* numPtr = (byte*) ((IntPtr) simulationMessagePtr + 28);
        *(RpcHeader*) numPtr = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaLocomotion.Gameplay.RopeSwingManager::RPC_SetVelocity(Fusion.NetworkRunner,System.Int32,System.Int32,UnityEngine.Vector3,System.Boolean,Fusion.RpcInfo)"));
        int num2 = 8;
        *(int*) (numPtr + num2) = ropeId;
        int num3 = num2 + 4;
        *(int*) (numPtr + num3) = boneIndex;
        int num4 = num3 + 4;
        *(Vector3*) (numPtr + num4) = velocity;
        int num5 = num4 + 12;
        ReadWriteUtilsForWeaver.WriteBoolean((int*) (numPtr + num5), wholeRope);
        int num6 = num5 + 4;
        simulationMessagePtr->Offset = num6 * 8;
        NetworkRunner networkRunner = runner;
        simulationMessagePtr->SetStatic();
        SimulationMessage* message = simulationMessagePtr;
        networkRunner.SendRpc(message);
      }
      info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
      goto label_2;
    }
  }

  private void SetVelocityShared(
    int ropeId,
    int boneIndex,
    Vector3 velocity,
    bool wholeRope,
    PhotonMessageInfoWrapped info)
  {
    if (info.Sender != null)
      GorillaNot.IncrementRPCCall(info, nameof (SetVelocityShared));
    GorillaRopeSwing result;
    if (!this.TryGetRope(ropeId, out result) || !((UnityEngine.Object) result != (UnityEngine.Object) null))
      return;
    result.SetVelocity(boneIndex, velocity, wholeRope, info);
  }

  [NetworkRpcStaticWeavedInvoker("System.Void GorillaLocomotion.Gameplay.RopeSwingManager::RPC_SetVelocity(Fusion.NetworkRunner,System.Int32,System.Int32,UnityEngine.Vector3,System.Boolean,Fusion.RpcInfo)")]
  [Preserve]
  [WeaverGenerated]
  protected static unsafe void RPC_SetVelocity\u0040Invoker(
    NetworkRunner runner,
    SimulationMessage* message)
  {
    byte* numPtr = (byte*) ((IntPtr) message + 28);
    int num1 = 8;
    NetworkRunner runner1 = runner;
    int num2 = *(int*) (numPtr + num1);
    int num3 = num1 + 4;
    int ropeId = num2;
    int num4 = *(int*) (numPtr + num3);
    int num5 = num3 + 4;
    int boneIndex = num4;
    Vector3 vector3 = *(Vector3*) (numPtr + num5);
    int num6 = num5 + 12;
    Vector3 velocity = vector3;
    int num7 = ReadWriteUtilsForWeaver.ReadBoolean((int*) (numPtr + num6)) ? 1 : 0;
    int num8 = num6 + 4;
    bool wholeRope = num7 != 0;
    RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
    NetworkBehaviourUtils.InvokeRpc = true;
    RopeSwingManager.RPC_SetVelocity(runner1, ropeId, boneIndex, velocity, wholeRope, info);
  }
}
