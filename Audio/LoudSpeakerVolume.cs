// Decompiled with JetBrains decompiler
// Type: GorillaTag.Audio.LoudSpeakerVolume
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Audio;

public class LoudSpeakerVolume : MonoBehaviour
{
  [SerializeField]
  private LoudSpeakerTrigger _trigger;

  public void OnTriggerEnter(Collider other)
  {
    if (!other.CompareTag("GorillaPlayer"))
      return;
    VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
    if ((Object) component != (Object) null && component.creator != null)
    {
      if (!(component.creator.UserId == NetworkSystem.Instance.LocalPlayer.UserId))
        return;
      this._trigger.OnPlayerEnter(component);
    }
    else
      Debug.LogWarning((object) "LoudSpeakerNetworkVolume :: OnTriggerEnter no colliding rig found!");
  }

  public void OnTriggerExit(Collider other)
  {
    VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
    if ((Object) component != (Object) null && component.creator != null)
    {
      if (!(component.creator.UserId == NetworkSystem.Instance.LocalPlayer.UserId))
        return;
      this._trigger.OnPlayerExit(component);
    }
    else
      Debug.LogWarning((object) "LoudSpeakerNetworkVolume :: OnTriggerExit no colliding rig found!");
  }
}
