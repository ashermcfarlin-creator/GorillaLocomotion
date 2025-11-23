// Decompiled with JetBrains decompiler
// Type: GorillaTag.CosmeticSystem.GTHardCodedBones
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable disable
namespace GorillaTag.CosmeticSystem;

public static class GTHardCodedBones
{
  public const int kBoneCount = 53;
  public static readonly string[] kBoneNames = new string[53]
  {
    "None",
    "rig",
    "body",
    "head",
    "head_end",
    "shoulder.L",
    "upper_arm.L",
    "forearm.L",
    "hand.L",
    "palm.01.L",
    "palm.02.L",
    "thumb.01.L",
    "thumb.02.L",
    "thumb.03.L",
    "thumb.03.L_end",
    "f_index.01.L",
    "f_index.02.L",
    "f_index.03.L",
    "f_index.03.L_end",
    "f_middle.01.L",
    "f_middle.02.L",
    "f_middle.03.L",
    "f_middle.03.L_end",
    "shoulder.R",
    "upper_arm.R",
    "forearm.R",
    "hand.R",
    "palm.01.R",
    "palm.02.R",
    "thumb.01.R",
    "thumb.02.R",
    "thumb.03.R",
    "thumb.03.R_end",
    "f_index.01.R",
    "f_index.02.R",
    "f_index.03.R",
    "f_index.03.R_end",
    "f_middle.01.R",
    "f_middle.02.R",
    "f_middle.03.R",
    "f_middle.03.R_end",
    "body_AnchorTop_Neck",
    "body_AnchorFront_StowSlot",
    "body_AnchorFrontLeft_Badge",
    "body_AnchorFrontRight_NameTag",
    "body_AnchorBack",
    "body_AnchorBackLeft_StowSlot",
    "body_AnchorBackRight_StowSlot",
    "body_AnchorBottom",
    "body_AnchorBackBottom_Tail",
    "hand_L_AnchorBack",
    "hand_R_AnchorBack",
    "hand_L_AnchorFront_GameModeItemSlot"
  };
  private const long kLeftSideMask = 1728432283058160;
  private const long kRightSideMask = 1769114204897280;
  private static readonly Dictionary<BodyDockPositions.DropPositions, GTHardCodedBones.EBone> _k_bodyDockDropPosition_to_eBone = new Dictionary<BodyDockPositions.DropPositions, GTHardCodedBones.EBone>()
  {
    {
      BodyDockPositions.DropPositions.None,
      GTHardCodedBones.EBone.None
    },
    {
      BodyDockPositions.DropPositions.LeftArm,
      GTHardCodedBones.EBone.forearm_L
    },
    {
      BodyDockPositions.DropPositions.RightArm,
      GTHardCodedBones.EBone.forearm_R
    },
    {
      BodyDockPositions.DropPositions.Chest,
      GTHardCodedBones.EBone.body_AnchorFront_StowSlot
    },
    {
      BodyDockPositions.DropPositions.LeftBack,
      GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot
    },
    {
      BodyDockPositions.DropPositions.RightBack,
      GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot
    }
  };
  private static readonly Dictionary<TransferrableObject.PositionState, GTHardCodedBones.EBone> _k_transferrablePosState_to_eBone = new Dictionary<TransferrableObject.PositionState, GTHardCodedBones.EBone>()
  {
    {
      TransferrableObject.PositionState.None,
      GTHardCodedBones.EBone.None
    },
    {
      TransferrableObject.PositionState.OnLeftArm,
      GTHardCodedBones.EBone.forearm_L
    },
    {
      TransferrableObject.PositionState.OnRightArm,
      GTHardCodedBones.EBone.forearm_R
    },
    {
      TransferrableObject.PositionState.InLeftHand,
      GTHardCodedBones.EBone.hand_L
    },
    {
      TransferrableObject.PositionState.InRightHand,
      GTHardCodedBones.EBone.hand_R
    },
    {
      TransferrableObject.PositionState.OnChest,
      GTHardCodedBones.EBone.body_AnchorFront_StowSlot
    },
    {
      TransferrableObject.PositionState.OnLeftShoulder,
      GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot
    },
    {
      TransferrableObject.PositionState.OnRightShoulder,
      GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot
    },
    {
      TransferrableObject.PositionState.Dropped,
      GTHardCodedBones.EBone.None
    }
  };
  private static readonly Dictionary<GTHardCodedBones.EBone, TransferrableObject.PositionState> _k_eBone_to_transferrablePosState = new Dictionary<GTHardCodedBones.EBone, TransferrableObject.PositionState>()
  {
    {
      GTHardCodedBones.EBone.None,
      TransferrableObject.PositionState.None
    },
    {
      GTHardCodedBones.EBone.forearm_L,
      TransferrableObject.PositionState.OnLeftArm
    },
    {
      GTHardCodedBones.EBone.forearm_R,
      TransferrableObject.PositionState.OnRightArm
    },
    {
      GTHardCodedBones.EBone.hand_L,
      TransferrableObject.PositionState.InLeftHand
    },
    {
      GTHardCodedBones.EBone.hand_R,
      TransferrableObject.PositionState.InRightHand
    },
    {
      GTHardCodedBones.EBone.body_AnchorFront_StowSlot,
      TransferrableObject.PositionState.OnChest
    },
    {
      GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot,
      TransferrableObject.PositionState.OnLeftShoulder
    },
    {
      GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot,
      TransferrableObject.PositionState.OnRightShoulder
    }
  };
  [OnEnterPlay_Clear]
  [OnExitPlay_Clear]
  private static readonly List<int> _gMissingBonesReport = new List<int>(53);
  [OnEnterPlay_Clear]
  [OnExitPlay_Clear]
  private static readonly Dictionary<int, Transform[]> _gInstIds_To_boneXforms = new Dictionary<int, Transform[]>(20);
  [OnEnterPlay_Clear]
  [OnExitPlay_Clear]
  private static readonly Dictionary<int, Transform[]> _gInstIds_To_slotXforms = new Dictionary<int, Transform[]>(20);

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  private static void HandleRuntimeInitialize_OnBeforeSceneLoad()
  {
    VRRigCache.OnPostInitialize += new Action(GTHardCodedBones.HandleVRRigCache_OnPostInitialize);
  }

  private static void HandleVRRigCache_OnPostInitialize()
  {
    VRRigCache.OnPostInitialize -= new Action(GTHardCodedBones.HandleVRRigCache_OnPostInitialize);
    GTHardCodedBones.HandleVRRigCache_OnPostSpawnRig();
    VRRigCache.OnPostSpawnRig += new Action(GTHardCodedBones.HandleVRRigCache_OnPostSpawnRig);
  }

  private static void HandleVRRigCache_OnPostSpawnRig()
  {
    if (!VRRigCache.isInitialized)
      return;
    int num = ApplicationQuittingState.IsQuitting ? 1 : 0;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int GetBoneIndex(GTHardCodedBones.EBone bone) => (int) bone;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int GetBoneIndex(string name)
  {
    for (int boneIndex = 0; boneIndex < GTHardCodedBones.kBoneNames.Length; ++boneIndex)
    {
      if (GTHardCodedBones.kBoneNames[boneIndex] == name)
        return boneIndex;
    }
    return 0;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryGetBoneIndexByName(string name, out int out_index)
  {
    for (int index = 0; index < GTHardCodedBones.kBoneNames.Length; ++index)
    {
      if (GTHardCodedBones.kBoneNames[index] == name)
      {
        out_index = index;
        return true;
      }
    }
    out_index = 0;
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static GTHardCodedBones.EBone GetBone(string name)
  {
    return (GTHardCodedBones.EBone) GTHardCodedBones.GetBoneIndex(name);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryGetBoneByName(string name, out GTHardCodedBones.EBone out_eBone)
  {
    int out_index;
    if (GTHardCodedBones.TryGetBoneIndexByName(name, out out_index))
    {
      out_eBone = (GTHardCodedBones.EBone) out_index;
      return true;
    }
    out_eBone = GTHardCodedBones.EBone.None;
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string GetBoneName(int boneIndex) => GTHardCodedBones.kBoneNames[boneIndex];

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryGetBoneName(int boneIndex, out string out_name)
  {
    if (boneIndex >= 0 && boneIndex < GTHardCodedBones.kBoneNames.Length)
    {
      out_name = GTHardCodedBones.kBoneNames[boneIndex];
      return true;
    }
    out_name = "None";
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string GetBoneName(GTHardCodedBones.EBone bone)
  {
    return GTHardCodedBones.GetBoneName((int) bone);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryGetBoneName(GTHardCodedBones.EBone bone, out string out_name)
  {
    return GTHardCodedBones.TryGetBoneName((int) bone, out out_name);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static long GetBoneBitFlag(string name)
  {
    if (name == "None")
      return 0;
    for (int index = 0; index < GTHardCodedBones.kBoneNames.Length; ++index)
    {
      if (GTHardCodedBones.kBoneNames[index] == name)
        return 1L << index - 1;
    }
    return 0;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static long GetBoneBitFlag(GTHardCodedBones.EBone bone)
  {
    return bone == GTHardCodedBones.EBone.None ? 0L : 1L << (int) (bone - 1 & (GTHardCodedBones.EBone.thumb_03_R | GTHardCodedBones.EBone.thumb_03_R_end));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static EHandedness GetHandednessFromBone(GTHardCodedBones.EBone bone)
  {
    if ((GTHardCodedBones.GetBoneBitFlag(bone) & 1728432283058160L) != 0L)
      return EHandedness.Left;
    return (GTHardCodedBones.GetBoneBitFlag(bone) & 1769114204897280L) == 0L ? EHandedness.None : EHandedness.Right;
  }

  public static bool TryGetBoneXforms(
    VRRig vrRig,
    out Transform[] outBoneXforms,
    out string outErrorMsg)
  {
    outErrorMsg = string.Empty;
    if ((UnityEngine.Object) vrRig == (UnityEngine.Object) null)
    {
      outErrorMsg = "The VRRig is null.";
      outBoneXforms = Array.Empty<Transform>();
      return false;
    }
    int instanceId = vrRig.GetInstanceID();
    if (GTHardCodedBones._gInstIds_To_boneXforms.TryGetValue(instanceId, out outBoneXforms))
      return true;
    if (!GTHardCodedBones.TryGetBoneXforms(vrRig.mainSkin, out outBoneXforms, out outErrorMsg))
      return false;
    VRRigAnchorOverrides componentInChildren1 = vrRig.GetComponentInChildren<VRRigAnchorOverrides>(true);
    BodyDockPositions componentInChildren2 = vrRig.GetComponentInChildren<BodyDockPositions>(true);
    outBoneXforms[46] = componentInChildren2.leftBackTransform;
    outBoneXforms[47] = componentInChildren2.rightBackTransform;
    outBoneXforms[42] = componentInChildren2.chestTransform;
    outBoneXforms[43] = componentInChildren1.CurrentBadgeTransform;
    outBoneXforms[44] = componentInChildren1.nameTransform;
    outBoneXforms[52] = componentInChildren1.huntComputer;
    outBoneXforms[50] = componentInChildren1.friendshipBraceletLeftAnchor;
    outBoneXforms[51] = componentInChildren1.friendshipBraceletRightAnchor;
    GTHardCodedBones._gInstIds_To_boneXforms[instanceId] = outBoneXforms;
    return true;
  }

  public static bool TryGetSlotAnchorXforms(
    VRRig vrRig,
    out Transform[] outSlotXforms,
    out string outErrorMsg)
  {
    outErrorMsg = string.Empty;
    if ((UnityEngine.Object) vrRig == (UnityEngine.Object) null)
    {
      outErrorMsg = "The VRRig is null.";
      outSlotXforms = Array.Empty<Transform>();
      return false;
    }
    int instanceId = vrRig.GetInstanceID();
    if (GTHardCodedBones._gInstIds_To_slotXforms.TryGetValue(instanceId, out outSlotXforms))
      return true;
    Transform[] outBoneXforms;
    if (!GTHardCodedBones.TryGetBoneXforms(vrRig.mainSkin, out outBoneXforms, out outErrorMsg))
      return false;
    outSlotXforms = new Transform[outBoneXforms.Length];
    for (int index = 0; index < outBoneXforms.Length; ++index)
      outSlotXforms[index] = outBoneXforms[index];
    BodyDockPositions componentInChildren = vrRig.GetComponentInChildren<BodyDockPositions>(true);
    outSlotXforms[7] = componentInChildren.leftArmTransform;
    outSlotXforms[25] = componentInChildren.rightArmTransform;
    outSlotXforms[8] = componentInChildren.leftHandTransform;
    outSlotXforms[26] = componentInChildren.rightHandTransform;
    GTHardCodedBones._gInstIds_To_slotXforms[instanceId] = outSlotXforms;
    return true;
  }

  public static bool TryGetBoneXforms(
    SkinnedMeshRenderer skinnedMeshRenderer,
    out Transform[] outBoneXforms,
    out string outErrorMsg)
  {
    outErrorMsg = string.Empty;
    if ((UnityEngine.Object) skinnedMeshRenderer == (UnityEngine.Object) null)
    {
      outErrorMsg = "The SkinnedMeshRenderer was null.";
      outBoneXforms = Array.Empty<Transform>();
      return false;
    }
    int instanceId = skinnedMeshRenderer.GetInstanceID();
    if (GTHardCodedBones._gInstIds_To_boneXforms.TryGetValue(instanceId, out outBoneXforms))
      return true;
    GTHardCodedBones._gMissingBonesReport.Clear();
    Transform[] bones = skinnedMeshRenderer.bones;
    for (int index = 0; index < bones.Length; ++index)
    {
      if ((UnityEngine.Object) bones[index] == (UnityEngine.Object) null)
        Debug.LogError((object) $"{$"this should never happen -- skinned mesh bone index {index} is null in component: "}\"{skinnedMeshRenderer.GetComponentPath<SkinnedMeshRenderer>()}\"", (UnityEngine.Object) skinnedMeshRenderer);
      else if ((UnityEngine.Object) bones[index].parent == (UnityEngine.Object) null)
        Debug.LogError((object) $"{$"unexpected and unhandled scenario -- skinned mesh bone at index {index} has no parent in "}component: \"{skinnedMeshRenderer.GetComponentPath<SkinnedMeshRenderer>()}\"", (UnityEngine.Object) skinnedMeshRenderer);
      else
        bones[index] = bones[index].name.EndsWith("_new") ? bones[index].parent : bones[index];
    }
    outBoneXforms = new Transform[GTHardCodedBones.kBoneNames.Length];
    for (int index = 1; index < GTHardCodedBones.kBoneNames.Length; ++index)
    {
      string kBoneName = GTHardCodedBones.kBoneNames[index];
      if (!(kBoneName == "None") && !kBoneName.EndsWith("_end") && !kBoneName.Contains("Anchor") && index != 1)
      {
        bool flag = false;
        foreach (Transform transform in bones)
        {
          if (!((UnityEngine.Object) transform == (UnityEngine.Object) null) && !(transform.name != kBoneName))
          {
            outBoneXforms[index] = transform;
            flag = true;
            break;
          }
        }
        if (!flag)
          GTHardCodedBones._gMissingBonesReport.Add(index);
      }
    }
    for (int index = 1; index < GTHardCodedBones.kBoneNames.Length; ++index)
    {
      string kBoneName = GTHardCodedBones.kBoneNames[index];
      if (kBoneName.EndsWith("_end"))
      {
        string str = kBoneName;
        int boneIndex = GTHardCodedBones.GetBoneIndex(str.Substring(0, str.Length - 4));
        if (boneIndex < 0)
        {
          GTHardCodedBones._gMissingBonesReport.Add(index);
        }
        else
        {
          Transform transform1 = outBoneXforms[boneIndex];
          if ((UnityEngine.Object) transform1 == (UnityEngine.Object) null)
          {
            GTHardCodedBones._gMissingBonesReport.Add(index);
          }
          else
          {
            Transform transform2 = transform1.Find(kBoneName);
            if ((UnityEngine.Object) transform2 == (UnityEngine.Object) null)
              GTHardCodedBones._gMissingBonesReport.Add(index);
            else
              outBoneXforms[index] = transform2;
          }
        }
      }
    }
    Transform transform3 = outBoneXforms[2];
    if (transform3 != null && transform3.parent != null)
      outBoneXforms[1] = transform3.parent;
    else
      GTHardCodedBones._gMissingBonesReport.Add(1);
    for (int index = 1; index < GTHardCodedBones.kBoneNames.Length; ++index)
    {
      string kBoneName = GTHardCodedBones.kBoneNames[index];
      if (kBoneName.Contains("Anchor"))
      {
        Transform result;
        if (transform3.TryFindByPath("/**/" + kBoneName, out result))
        {
          outBoneXforms[index] = result;
        }
        else
        {
          GameObject gameObject = new GameObject(kBoneName);
          gameObject.transform.SetParent(transform3, false);
          outBoneXforms[index] = gameObject.transform;
        }
      }
    }
    GTHardCodedBones._gInstIds_To_boneXforms[instanceId] = outBoneXforms;
    if (GTHardCodedBones._gMissingBonesReport.Count == 0)
      return true;
    string str1 = $"The SkinnedMeshRenderer on \"{skinnedMeshRenderer.name}\" did not have these expected bones: ";
    foreach (int index in GTHardCodedBones._gMissingBonesReport)
      str1 = $"{str1}\n- {GTHardCodedBones.kBoneNames[index]}";
    outErrorMsg = str1;
    return true;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryGetBoneXform(
    Transform[] boneXforms,
    string boneName,
    out Transform boneXform)
  {
    boneXform = boneXforms[GTHardCodedBones.GetBoneIndex(boneName)];
    return (UnityEngine.Object) boneXform != (UnityEngine.Object) null;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryGetBoneXform(
    Transform[] boneXforms,
    GTHardCodedBones.EBone eBone,
    out Transform boneXform)
  {
    boneXform = boneXforms[GTHardCodedBones.GetBoneIndex(eBone)];
    return (UnityEngine.Object) boneXform != (UnityEngine.Object) null;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryGetFirstBoneInParents(
    Transform transform,
    out GTHardCodedBones.EBone eBone,
    out Transform boneXform)
  {
    for (; (UnityEngine.Object) transform != (UnityEngine.Object) null; transform = transform.parent)
    {
      string name = transform.name;
      if (name == "DropZoneAnchor" && transform.parent != null)
      {
        switch (transform.parent.name)
        {
          case "Slingshot Chest Snap":
            eBone = GTHardCodedBones.EBone.body_AnchorFront_StowSlot;
            boneXform = transform;
            return true;
          case "TransferrableItemLeftArm":
            eBone = GTHardCodedBones.EBone.forearm_L;
            boneXform = transform;
            return true;
          case "TransferrableItemLeftShoulder":
            eBone = GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot;
            boneXform = transform;
            return true;
          case "TransferrableItemRightShoulder":
            eBone = GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot;
            boneXform = transform;
            return true;
        }
      }
      else
      {
        switch (name)
        {
          case "TransferrableItemLeftHand":
            eBone = GTHardCodedBones.EBone.hand_L;
            boneXform = transform;
            return true;
          case "TransferrableItemRightHand":
            eBone = GTHardCodedBones.EBone.hand_R;
            boneXform = transform;
            return true;
        }
      }
      GTHardCodedBones.EBone bone = GTHardCodedBones.GetBone(transform.name);
      if (bone != GTHardCodedBones.EBone.None)
      {
        eBone = bone;
        boneXform = transform;
        return true;
      }
    }
    eBone = GTHardCodedBones.EBone.None;
    boneXform = (Transform) null;
    return false;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static GTHardCodedBones.EBone GetBoneEnumOfCosmeticPosStateFlag(
    TransferrableObject.PositionState positionState)
  {
    switch (positionState)
    {
      case TransferrableObject.PositionState.None:
      case TransferrableObject.PositionState.Dropped:
        return GTHardCodedBones.EBone.None;
      case TransferrableObject.PositionState.OnLeftArm:
        return GTHardCodedBones.EBone.forearm_L;
      case TransferrableObject.PositionState.OnRightArm:
        return GTHardCodedBones.EBone.forearm_R;
      case TransferrableObject.PositionState.InLeftHand:
        return GTHardCodedBones.EBone.hand_L;
      case TransferrableObject.PositionState.InRightHand:
        return GTHardCodedBones.EBone.hand_R;
      case TransferrableObject.PositionState.OnChest:
        return GTHardCodedBones.EBone.body_AnchorFront_StowSlot;
      case TransferrableObject.PositionState.OnLeftShoulder:
        return GTHardCodedBones.EBone.body_AnchorBackLeft_StowSlot;
      case TransferrableObject.PositionState.OnRightShoulder:
        return GTHardCodedBones.EBone.body_AnchorBackRight_StowSlot;
      default:
        throw new ArgumentOutOfRangeException(positionState.ToString());
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static List<GTHardCodedBones.EBone> GetBoneEnumsFromCosmeticBodyDockDropPosFlags(
    BodyDockPositions.DropPositions enumFlags)
  {
    BodyDockPositions.DropPositions[] values = EnumData<BodyDockPositions.DropPositions>.Shared.Values;
    List<GTHardCodedBones.EBone> dockDropPosFlags = new List<GTHardCodedBones.EBone>(32 /*0x20*/);
    foreach (BodyDockPositions.DropPositions key in values)
    {
      switch (key)
      {
        case BodyDockPositions.DropPositions.None:
        case BodyDockPositions.DropPositions.MaxDropPostions:
        case BodyDockPositions.DropPositions.All:
          continue;
        default:
          if ((enumFlags & key) != BodyDockPositions.DropPositions.None)
          {
            dockDropPosFlags.Add(GTHardCodedBones._k_bodyDockDropPosition_to_eBone[key]);
            continue;
          }
          continue;
      }
    }
    return dockDropPosFlags;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static List<GTHardCodedBones.EBone> GetBoneEnumsFromCosmeticTransferrablePosStateFlags(
    TransferrableObject.PositionState enumFlags)
  {
    TransferrableObject.PositionState[] values = EnumData<TransferrableObject.PositionState>.Shared.Values;
    List<GTHardCodedBones.EBone> transferrablePosStateFlags = new List<GTHardCodedBones.EBone>(32 /*0x20*/);
    foreach (TransferrableObject.PositionState key in values)
    {
      switch (key)
      {
        case TransferrableObject.PositionState.None:
        case TransferrableObject.PositionState.Dropped:
          continue;
        default:
          if ((enumFlags & key) != TransferrableObject.PositionState.None)
          {
            transferrablePosStateFlags.Add(GTHardCodedBones._k_transferrablePosState_to_eBone[key]);
            continue;
          }
          continue;
      }
    }
    return transferrablePosStateFlags;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool TryGetTransferrablePosStateFromBoneEnum(
    GTHardCodedBones.EBone eBone,
    out TransferrableObject.PositionState outPosState)
  {
    return GTHardCodedBones._k_eBone_to_transferrablePosState.TryGetValue(eBone, out outPosState);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Transform GetBoneXformOfCosmeticPosStateFlag(
    TransferrableObject.PositionState anchorPosState,
    Transform[] bones)
  {
    if (bones.Length != 53)
      throw new Exception($"{nameof (GTHardCodedBones)}: Supplied bones array length is {bones.Length} but requires " + $"{53}.");
    int boneIndex = GTHardCodedBones.GetBoneIndex(GTHardCodedBones.GetBoneEnumOfCosmeticPosStateFlag(anchorPosState));
    return boneIndex != -1 ? bones[boneIndex] : (Transform) null;
  }

  public enum EBone
  {
    None,
    rig,
    body,
    head,
    head_end,
    shoulder_L,
    upper_arm_L,
    forearm_L,
    hand_L,
    palm_01_L,
    palm_02_L,
    thumb_01_L,
    thumb_02_L,
    thumb_03_L,
    thumb_03_L_end,
    f_index_01_L,
    f_index_02_L,
    f_index_03_L,
    f_index_03_L_end,
    f_middle_01_L,
    f_middle_02_L,
    f_middle_03_L,
    f_middle_03_L_end,
    shoulder_R,
    upper_arm_R,
    forearm_R,
    hand_R,
    palm_01_R,
    palm_02_R,
    thumb_01_R,
    thumb_02_R,
    thumb_03_R,
    thumb_03_R_end,
    f_index_01_R,
    f_index_02_R,
    f_index_03_R,
    f_index_03_R_end,
    f_middle_01_R,
    f_middle_02_R,
    f_middle_03_R,
    f_middle_03_R_end,
    body_AnchorTop_Neck,
    body_AnchorFront_StowSlot,
    body_AnchorFrontLeft_Badge,
    body_AnchorFrontRight_NameTag,
    body_AnchorBack,
    body_AnchorBackLeft_StowSlot,
    body_AnchorBackRight_StowSlot,
    body_AnchorBottom,
    body_AnchorBackBottom_Tail,
    hand_L_AnchorBack,
    hand_R_AnchorBack,
    hand_L_AnchorFront_GameModeItemSlot,
  }

  public enum EStowSlots
  {
    None = 0,
    forearm_L = 7,
    forearm_R = 25, // 0x00000019
    body_AnchorFront_Chest = 42, // 0x0000002A
    body_AnchorBackLeft = 46, // 0x0000002E
    body_AnchorBackRight = 47, // 0x0000002F
  }

  public enum EHandAndStowSlots
  {
    None = 0,
    forearm_L = 7,
    hand_L = 8,
    forearm_R = 25, // 0x00000019
    hand_R = 26, // 0x0000001A
    body_AnchorFront_Chest = 42, // 0x0000002A
    body_AnchorBackLeft = 46, // 0x0000002E
    body_AnchorBackRight = 47, // 0x0000002F
  }

  public enum ECosmeticSlots
  {
    TagEffect = 0,
    Fur = 1,
    Arms = 2,
    Shirt = 2,
    Face = 3,
    Hat = 4,
    ArmLeft = 6,
    HandLeft = 8,
    ArmRight = 24, // 0x00000018
    HandRight = 26, // 0x0000001A
    Chest = 42, // 0x0000002A
    Badge = 43, // 0x0000002B
    Back = 45, // 0x0000002D
    BackLeft = 46, // 0x0000002E
    BackRight = 47, // 0x0000002F
    Pants = 48, // 0x00000030
  }

  [Serializable]
  public struct SturdyEBone : ISerializationCallbackReceiver
  {
    [SerializeField]
    private GTHardCodedBones.EBone _bone;
    [SerializeField]
    private string _boneName;

    public GTHardCodedBones.EBone Bone
    {
      get => this._bone;
      set
      {
        this._bone = value;
        this._boneName = GTHardCodedBones.GetBoneName(this._bone);
      }
    }

    public SturdyEBone(GTHardCodedBones.EBone bone)
    {
      this._bone = bone;
      this._boneName = (string) null;
    }

    public SturdyEBone(string boneName)
    {
      this._bone = GTHardCodedBones.GetBone(boneName);
      this._boneName = (string) null;
    }

    public static implicit operator GTHardCodedBones.EBone(GTHardCodedBones.SturdyEBone sturdyBone)
    {
      return sturdyBone.Bone;
    }

    public static implicit operator GTHardCodedBones.SturdyEBone(GTHardCodedBones.EBone bone)
    {
      return new GTHardCodedBones.SturdyEBone(bone);
    }

    public static explicit operator int(GTHardCodedBones.SturdyEBone sturdyBone)
    {
      return (int) sturdyBone.Bone;
    }

    public override string ToString() => this._boneName;

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
      if (string.IsNullOrEmpty(this._boneName))
      {
        this._bone = GTHardCodedBones.EBone.None;
        this._boneName = "None";
      }
      else
      {
        GTHardCodedBones.EBone bone = GTHardCodedBones.GetBone(this._boneName);
        if (bone == GTHardCodedBones.EBone.None)
          return;
        this._bone = bone;
      }
    }
  }
}
