// Decompiled with JetBrains decompiler
// Type: GorillaTag.MonkeFX.MonkeFX
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable disable
namespace GorillaTag.MonkeFX;

public class MonkeFX : ITickSystemPost
{
  private static readonly string[] _boneNames = new string[3]
  {
    "body",
    "hand.L",
    "hand.R"
  };
  private static VRRig[] _rigs;
  private static Transform[] _bones;
  private static int _rigsHash;
  private static readonly GTLogErrorLimiter _errorLog_nullVRRigFromVRRigCache = new GTLogErrorLimiter("(This should never happen) Skipping null `VRRig` obtained from `VRRigCache`!");
  private static GTLogErrorLimiter _errorLog_nullMainSkin = new GTLogErrorLimiter("(This should never happen) Skipping null `mainSkin` on `VRRig`! Scene paths: \n");
  private static readonly GTLogErrorLimiter _errorLog_nullBone = new GTLogErrorLimiter("(This should never happen) Skipping null bone obtained from `VRRig.mainSkin.bones`! Index(es): ");
  private readonly HashSet<MonkeFXSettingsSO> _settingsSOs = new HashSet<MonkeFXSettingsSO>(8);
  private readonly Dictionary<int, int> _srcMeshInst_to_meshId = new Dictionary<int, int>(8);
  private readonly List<UnityEngine.Mesh> _srcMeshId_to_sourceMesh = new List<UnityEngine.Mesh>(8);
  private readonly List<GorillaTag.MonkeFX.MonkeFX.ElementsRange> _srcMeshId_to_elemRange = new List<GorillaTag.MonkeFX.MonkeFX.ElementsRange>(8);
  private readonly Dictionary<int, List<MonkeFXSettingsSO>> _meshId_to_settingsUsers = new Dictionary<int, List<MonkeFXSettingsSO>>();
  private const float _k16BitFactor = 65536f;

  private static void InitBonesArray()
  {
    GorillaTag.MonkeFX.MonkeFX._rigs = VRRigCache.Instance.GetAllRigs();
    GorillaTag.MonkeFX.MonkeFX._bones = new Transform[GorillaTag.MonkeFX.MonkeFX._rigs.Length * GorillaTag.MonkeFX.MonkeFX._boneNames.Length];
    for (int index1 = 0; index1 < GorillaTag.MonkeFX.MonkeFX._rigs.Length; ++index1)
    {
      if ((Object) GorillaTag.MonkeFX.MonkeFX._rigs[index1] == (Object) null)
      {
        GorillaTag.MonkeFX.MonkeFX._errorLog_nullVRRigFromVRRigCache.AddOccurrence(index1.ToString());
      }
      else
      {
        int num = index1 * GorillaTag.MonkeFX.MonkeFX._boneNames.Length;
        if ((Object) GorillaTag.MonkeFX.MonkeFX._rigs[index1].mainSkin == (Object) null)
        {
          GorillaTag.MonkeFX.MonkeFX._errorLog_nullMainSkin.AddOccurrence(GorillaTag.MonkeFX.MonkeFX._rigs[index1].transform.GetPath());
          Debug.LogError((object) $"(This should never happen) Skipping null `mainSkin` on `VRRig`! Scene path: \n- \"{GorillaTag.MonkeFX.MonkeFX._rigs[index1].transform.GetPath()}\"");
        }
        else
        {
          for (int index2 = 0; index2 < GorillaTag.MonkeFX.MonkeFX._rigs[index1].mainSkin.bones.Length; ++index2)
          {
            Transform bone = GorillaTag.MonkeFX.MonkeFX._rigs[index1].mainSkin.bones[index2];
            if ((Object) bone == (Object) null)
            {
              GorillaTag.MonkeFX.MonkeFX._errorLog_nullBone.AddOccurrence(index2.ToString());
            }
            else
            {
              for (int index3 = 0; index3 < GorillaTag.MonkeFX.MonkeFX._boneNames.Length; ++index3)
              {
                if (GorillaTag.MonkeFX.MonkeFX._boneNames[index3] == bone.name)
                  GorillaTag.MonkeFX.MonkeFX._bones[num + index3] = bone;
              }
            }
          }
        }
      }
    }
    GorillaTag.MonkeFX.MonkeFX._errorLog_nullVRRigFromVRRigCache.LogOccurrences((Component) VRRigCache.Instance, caller: nameof (InitBonesArray), sourceFilePath: "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", line: 106);
    GorillaTag.MonkeFX.MonkeFX._errorLog_nullMainSkin.LogOccurrences(caller: nameof (InitBonesArray), sourceFilePath: "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", line: 107);
    GorillaTag.MonkeFX.MonkeFX._errorLog_nullBone.LogOccurrences(caller: nameof (InitBonesArray), sourceFilePath: "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\MonkeFX-Bones.cs", line: 108);
  }

  private static void UpdateBones()
  {
  }

  private static void UpdateBone()
  {
  }

  public static void Register(MonkeFXSettingsSO settingsSO)
  {
    GorillaTag.MonkeFX.MonkeFX.EnsureInstance();
    if ((Object) settingsSO == (Object) null || !GorillaTag.MonkeFX.MonkeFX.instance._settingsSOs.Add(settingsSO))
      return;
    int count = GorillaTag.MonkeFX.MonkeFX.instance._srcMeshId_to_sourceMesh.Count;
    for (int index = 0; index < settingsSO.sourceMeshes.Length; ++index)
    {
      UnityEngine.Mesh mesh = settingsSO.sourceMeshes[index].obj;
      if (!((Object) mesh == (Object) null) && GorillaTag.MonkeFX.MonkeFX.instance._srcMeshInst_to_meshId.TryAdd(mesh.GetInstanceID(), count))
      {
        GorillaTag.MonkeFX.MonkeFX.instance._srcMeshId_to_sourceMesh.Add(mesh);
        ++count;
      }
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float GetScaleToFitInBounds(UnityEngine.Mesh mesh)
  {
    Bounds bounds = mesh.bounds;
    float num = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
    return (double) num <= 0.0 ? 0.0f : 1f / num;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float Pack0To1Floats(float x, float y)
  {
    return Mathf.Clamp01(x) * 65536f + Mathf.Clamp01(y);
  }

  public static GorillaTag.MonkeFX.MonkeFX instance { get; private set; }

  public static bool hasInstance { get; private set; }

  private static void EnsureInstance()
  {
    if (GorillaTag.MonkeFX.MonkeFX.hasInstance)
      return;
    GorillaTag.MonkeFX.MonkeFX.instance = new GorillaTag.MonkeFX.MonkeFX();
    GorillaTag.MonkeFX.MonkeFX.hasInstance = true;
  }

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
  private static void OnAfterFirstSceneLoaded()
  {
    GorillaTag.MonkeFX.MonkeFX.EnsureInstance();
    TickSystem<object>.AddPostTickCallback((ITickSystemPost) GorillaTag.MonkeFX.MonkeFX.instance);
  }

  void ITickSystemPost.PostTick()
  {
    if (ApplicationQuittingState.IsQuitting)
      return;
    GorillaTag.MonkeFX.MonkeFX.UpdateBones();
  }

  bool ITickSystemPost.PostTickRunning { get; set; }

  private static void PauseTick()
  {
    if (!GorillaTag.MonkeFX.MonkeFX.hasInstance)
      GorillaTag.MonkeFX.MonkeFX.instance = new GorillaTag.MonkeFX.MonkeFX();
    TickSystem<object>.RemovePostTickCallback((ITickSystemPost) GorillaTag.MonkeFX.MonkeFX.instance);
  }

  private static void ResumeTick()
  {
    if (!GorillaTag.MonkeFX.MonkeFX.hasInstance)
      GorillaTag.MonkeFX.MonkeFX.instance = new GorillaTag.MonkeFX.MonkeFX();
    TickSystem<object>.AddPostTickCallback((ITickSystemPost) GorillaTag.MonkeFX.MonkeFX.instance);
  }

  private struct ElementsRange
  {
    public int min;
    public int max;
  }
}
