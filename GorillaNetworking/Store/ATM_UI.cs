// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.ATM_UI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

#nullable disable
namespace GorillaNetworking.Store;

public class ATM_UI : MonoBehaviour
{
  public GameObject creatorCodeObject;
  public TMP_Text atmText;
  public TMP_Text creatorCodeTitle;
  public TMP_Text creatorCodeField;
  public TMP_Text[] ATM_RightColumnButtonText;
  public TMP_Text[] ATM_RightColumnArrowText;
  private UnityEngine.SceneManagement.Scene customMapScene;

  private void Start()
  {
    if (!((Object) ATM_Manager.instance != (Object) null) || ATM_Manager.instance.atmUIs.Contains(this))
      return;
    ATM_Manager.instance.AddATM(this);
  }

  public void SetCustomMapScene(UnityEngine.SceneManagement.Scene scene)
  {
    this.customMapScene = scene;
  }

  public bool IsFromCustomMapScene(UnityEngine.SceneManagement.Scene scene)
  {
    return this.customMapScene == scene;
  }
}
