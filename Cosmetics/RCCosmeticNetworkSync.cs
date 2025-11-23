// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.RCCosmeticNetworkSync
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class RCCosmeticNetworkSync : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback
{
  public RCCosmeticNetworkSync.SyncedState syncedState;
  private RCRemoteHoldable rcRemote;

  public void OnPhotonInstantiate(PhotonMessageInfo info)
  {
    if (info.Sender == null)
      this.DestroyThis();
    else if (info.Sender != this.photonView.Owner || this.photonView.IsRoomView)
    {
      GorillaNot.instance.SendReport("spoofed rc instantiate", info.Sender.UserId, info.Sender.NickName);
      this.DestroyThis();
    }
    else
    {
      object[] instantiationData = info.photonView.InstantiationData;
      if (instantiationData != null && instantiationData.Length >= 1 && instantiationData[0] is int index)
      {
        RigContainer playerRig;
        if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender.ActorNumber), out playerRig) && index > -1 && index < playerRig.Rig.myBodyDockPositions.allObjects.Length)
        {
          this.rcRemote = playerRig.Rig.myBodyDockPositions.allObjects[index] as RCRemoteHoldable;
          if ((Object) this.rcRemote != (Object) null)
          {
            this.rcRemote.networkSync = this;
            this.rcRemote.WakeUpRemoteVehicle();
          }
        }
        if (!((Object) this.rcRemote == (Object) null))
          return;
        this.DestroyThis();
      }
      else
        this.DestroyThis();
    }
  }

  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
  {
    if (info.Sender != this.photonView.Owner)
      return;
    if (stream.IsWriting)
    {
      stream.SendNext((object) this.syncedState.state);
      stream.SendNext((object) this.syncedState.position);
      stream.SendNext((object) (int) BitPackUtils.PackRotation(this.syncedState.rotation));
      stream.SendNext((object) this.syncedState.dataA);
      stream.SendNext((object) this.syncedState.dataB);
      stream.SendNext((object) this.syncedState.dataC);
    }
    else
    {
      if (!stream.IsReading)
        return;
      int state1 = (int) this.syncedState.state;
      this.syncedState.state = (byte) stream.ReceiveNext();
      this.syncedState.position.SetValueSafe((Vector3) stream.ReceiveNext());
      this.syncedState.rotation.SetValueSafe(BitPackUtils.UnpackRotation((uint) (int) stream.ReceiveNext()));
      this.syncedState.dataA = (byte) stream.ReceiveNext();
      this.syncedState.dataB = (byte) stream.ReceiveNext();
      this.syncedState.dataC = (byte) stream.ReceiveNext();
      int state2 = (int) this.syncedState.state;
      if (state1 == state2 || !((Object) this.rcRemote != (Object) null) || !((Object) this.rcRemote.Vehicle != (Object) null) || this.rcRemote.Vehicle.enabled)
        return;
      this.rcRemote.WakeUpRemoteVehicle();
    }
  }

  [PunRPC]
  public void HitRCVehicleRPC(Vector3 hitVelocity, bool isProjectile, PhotonMessageInfo info)
  {
    GorillaNot.IncrementRPCCall(info, nameof (HitRCVehicleRPC));
    if (!hitVelocity.IsValid())
    {
      GorillaNot.instance.SendReport("nan rc hit", info.Sender.UserId, info.Sender.NickName);
    }
    else
    {
      if (!((Object) this.rcRemote != (Object) null) || !((Object) this.rcRemote.Vehicle != (Object) null))
        return;
      this.rcRemote.Vehicle.AuthorityApplyImpact(hitVelocity, isProjectile);
    }
  }

  private void DestroyThis()
  {
    if (this.photonView.IsMine)
      PhotonNetwork.Destroy(this.gameObject);
    else
      this.gameObject.SetActive(false);
  }

  public struct SyncedState
  {
    public byte state;
    public Vector3 position;
    public Quaternion rotation;
    public byte dataA;
    public byte dataB;
    public byte dataC;
  }
}
