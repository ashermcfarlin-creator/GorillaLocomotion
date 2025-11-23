// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Swimming.RigidbodyWaterInteractionManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Swimming;

public class RigidbodyWaterInteractionManager : MonoBehaviour
{
  public static RigidbodyWaterInteractionManager instance;
  [OnEnterPlay_Set(false)]
  public static bool hasInstance = false;
  public static List<RigidbodyWaterInteraction> allrBWI = new List<RigidbodyWaterInteraction>();

  protected void Awake()
  {
    if (RigidbodyWaterInteractionManager.hasInstance && (Object) RigidbodyWaterInteractionManager.instance != (Object) this)
      Object.Destroy((Object) this);
    else
      RigidbodyWaterInteractionManager.SetInstance(this);
  }

  public static void CreateManager()
  {
    RigidbodyWaterInteractionManager.SetInstance(new GameObject(nameof (RigidbodyWaterInteractionManager)).AddComponent<RigidbodyWaterInteractionManager>());
  }

  private static void SetInstance(RigidbodyWaterInteractionManager manager)
  {
    RigidbodyWaterInteractionManager.instance = manager;
    RigidbodyWaterInteractionManager.hasInstance = true;
    if (!Application.isPlaying)
      return;
    Object.DontDestroyOnLoad((Object) manager);
  }

  public static void RegisterRBWI(RigidbodyWaterInteraction rbWI)
  {
    if (!RigidbodyWaterInteractionManager.hasInstance)
      RigidbodyWaterInteractionManager.CreateManager();
    if (RigidbodyWaterInteractionManager.allrBWI.Contains(rbWI))
      return;
    RigidbodyWaterInteractionManager.allrBWI.Add(rbWI);
  }

  public static void UnregisterRBWI(RigidbodyWaterInteraction rbWI)
  {
    if (!RigidbodyWaterInteractionManager.hasInstance)
      RigidbodyWaterInteractionManager.CreateManager();
    if (!RigidbodyWaterInteractionManager.allrBWI.Contains(rbWI))
      return;
    RigidbodyWaterInteractionManager.allrBWI.Remove(rbWI);
  }

  public void FixedUpdate()
  {
    for (int index = 0; index < RigidbodyWaterInteractionManager.allrBWI.Count; ++index)
      RigidbodyWaterInteractionManager.allrBWI[index].InvokeFixedUpdate();
  }
}
