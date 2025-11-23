// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.GorillaNetworkDisconnectTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using UnityEngine;

#nullable disable
namespace GorillaNetworking;

public class GorillaNetworkDisconnectTrigger : GorillaTriggerBox
{
  public PhotonNetworkController photonNetworkController;
  public GameObject offlineVRRig;
  public GameObject makeSureThisIsEnabled;
  public GameObject[] makeSureTheseAreEnabled;
  public string componentTypeToRemove;
  public GameObject componentTarget;

  public override void OnBoxTriggered()
  {
    base.OnBoxTriggered();
    if ((Object) this.makeSureThisIsEnabled != (Object) null)
      this.makeSureThisIsEnabled.SetActive(true);
    foreach (GameObject gameObject in this.makeSureTheseAreEnabled)
      gameObject.SetActive(true);
    if (!PhotonNetwork.InRoom)
      return;
    if (this.componentTypeToRemove != "" && (Object) this.componentTarget.GetComponent(this.componentTypeToRemove) != (Object) null)
      Object.Destroy((Object) this.componentTarget.GetComponent(this.componentTypeToRemove));
    PhotonNetwork.Disconnect();
    foreach (Renderer renderer in this.photonNetworkController.offlineVRRig)
      renderer.enabled = true;
    PhotonNetwork.ConnectUsingSettings();
  }
}
