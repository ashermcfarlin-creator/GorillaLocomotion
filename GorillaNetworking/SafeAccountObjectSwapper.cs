// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.SafeAccountObjectSwapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace GorillaNetworking;

public class SafeAccountObjectSwapper : MonoBehaviour
{
  public GameObject[] UnSafeGameObjects;
  public GameObject[] UnSafeTexts;
  public GameObject[] SafeTexts;
  public GameObject[] SafeModeObjects;

  public void Start()
  {
    if (PlayFabAuthenticator.instance.GetSafety())
      this.SwitchToSafeMode();
    PlayFabAuthenticator.instance.OnSafetyUpdate += new Action<bool>(this.SafeAccountUpdated);
  }

  public void SafeAccountUpdated(bool isSafety)
  {
    if (!isSafety)
      return;
    this.SwitchToSafeMode();
  }

  public void SwitchToSafeMode()
  {
    foreach (GameObject unSafeGameObject in this.UnSafeGameObjects)
    {
      if ((UnityEngine.Object) unSafeGameObject != (UnityEngine.Object) null)
        unSafeGameObject.SetActive(false);
    }
    foreach (GameObject unSafeText in this.UnSafeTexts)
    {
      if ((UnityEngine.Object) unSafeText != (UnityEngine.Object) null)
        unSafeText.SetActive(false);
    }
    foreach (GameObject safeText in this.SafeTexts)
    {
      if ((UnityEngine.Object) safeText != (UnityEngine.Object) null)
        safeText.SetActive(true);
    }
    foreach (GameObject safeModeObject in this.SafeModeObjects)
    {
      if ((UnityEngine.Object) safeModeObject != (UnityEngine.Object) null)
        safeModeObject.SetActive(true);
    }
  }
}
