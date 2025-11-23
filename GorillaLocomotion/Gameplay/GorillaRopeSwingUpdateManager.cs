// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.GorillaRopeSwingUpdateManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Gameplay;

public class GorillaRopeSwingUpdateManager : MonoBehaviour
{
  public static GorillaRopeSwingUpdateManager instance;
  public static bool hasInstance = false;
  public static List<GorillaRopeSwing> allGorillaRopeSwings = new List<GorillaRopeSwing>();

  protected void Awake()
  {
    if (GorillaRopeSwingUpdateManager.hasInstance && (Object) GorillaRopeSwingUpdateManager.instance != (Object) null && (Object) GorillaRopeSwingUpdateManager.instance != (Object) this)
      Object.Destroy((Object) this);
    else
      GorillaRopeSwingUpdateManager.SetInstance(this);
  }

  public static void CreateManager()
  {
    GorillaRopeSwingUpdateManager.SetInstance(new GameObject(nameof (GorillaRopeSwingUpdateManager)).AddComponent<GorillaRopeSwingUpdateManager>());
  }

  private static void SetInstance(GorillaRopeSwingUpdateManager manager)
  {
    GorillaRopeSwingUpdateManager.instance = manager;
    GorillaRopeSwingUpdateManager.hasInstance = true;
    if (!Application.isPlaying)
      return;
    Object.DontDestroyOnLoad((Object) manager);
  }

  public static void RegisterRopeSwing(GorillaRopeSwing ropeSwing)
  {
    if (!GorillaRopeSwingUpdateManager.hasInstance)
      GorillaRopeSwingUpdateManager.CreateManager();
    if (GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Contains(ropeSwing))
      return;
    GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Add(ropeSwing);
  }

  public static void UnregisterRopeSwing(GorillaRopeSwing ropeSwing)
  {
    if (!GorillaRopeSwingUpdateManager.hasInstance)
      GorillaRopeSwingUpdateManager.CreateManager();
    if (!GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Contains(ropeSwing))
      return;
    GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Remove(ropeSwing);
  }

  public void Update()
  {
    for (int index = 0; index < GorillaRopeSwingUpdateManager.allGorillaRopeSwings.Count; ++index)
      GorillaRopeSwingUpdateManager.allGorillaRopeSwings[index].InvokeUpdate();
  }
}
