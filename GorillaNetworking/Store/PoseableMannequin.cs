// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.PoseableMannequin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaNetworking.Store;

public class PoseableMannequin : MonoBehaviour
{
  public SkinnedMeshRenderer skinnedMeshRenderer;
  [FormerlySerializedAs("meshCollider")]
  public MeshCollider skinnedMeshCollider;
  public GTPosRotConstraints[] cosmeticConstraints;
  public UnityEngine.Mesh BakedColliderMesh;
  [SerializeField]
  [FormerlySerializedAs("liveAssetPath")]
  protected string prefabAssetPath;
  [SerializeField]
  protected string prefabFolderPath;
  [SerializeField]
  protected string prefabAssetName;
  public MeshFilter staticGorillaMesh;
  public MeshCollider staticGorillaMeshCollider;
  public MeshRenderer staticGorillaMeshRenderer;

  public void Start()
  {
    this.skinnedMeshRenderer.gameObject.SetActive(false);
    this.staticGorillaMesh.gameObject.SetActive(true);
  }

  private string GetPrefabPathFromCurrentPrefabStage() => "";

  private string GetMeshPathFromPrefabPath(string prefabPath) => "";

  public void BakeSkinnedMesh()
  {
    this.BakeAndSaveMeshInPath(this.GetMeshPathFromPrefabPath(this.GetPrefabPathFromCurrentPrefabStage()));
  }

  public void BakeAndSaveMeshInPath(string meshPath)
  {
  }

  private void UpdateStaticMeshMannequin()
  {
    this.staticGorillaMesh.sharedMesh = this.BakedColliderMesh;
    this.staticGorillaMeshRenderer.sharedMaterials = this.skinnedMeshRenderer.sharedMaterials;
    this.staticGorillaMeshCollider.sharedMesh = this.BakedColliderMesh;
  }

  private void UpdateSkinnedMeshCollider()
  {
    this.skinnedMeshCollider.sharedMesh = this.BakedColliderMesh;
  }

  public void UpdateGTPosRotConstraints()
  {
    foreach (GTPosRotConstraints cosmeticConstraint in this.cosmeticConstraints)
      ((IEnumerable<GorillaPosRotConstraint>) cosmeticConstraint.constraints).ForEach<GorillaPosRotConstraint>((Action<GorillaPosRotConstraint>) (c =>
      {
        c.follower.rotation = c.source.rotation;
        c.follower.position = c.source.position;
      }));
  }

  private void HookupCosmeticConstraints()
  {
    this.cosmeticConstraints = this.GetComponentsInChildren<GTPosRotConstraints>();
    foreach (GTPosRotConstraints cosmeticConstraint in this.cosmeticConstraints)
    {
      for (int index = 0; index < cosmeticConstraint.constraints.Length; ++index)
        cosmeticConstraint.constraints[index].source = this.FindBone(cosmeticConstraint.constraints[index].follower.name);
    }
  }

  private Transform FindBone(string boneName)
  {
    foreach (Transform bone in this.skinnedMeshRenderer.bones)
    {
      if (bone.name == boneName)
        return bone;
    }
    return (Transform) null;
  }

  public void CreasteTestClip()
  {
  }

  public void SerializeVRRig() => this.StartCoroutine(this.SaveLocalPlayerPose());

  public IEnumerator SaveLocalPlayerPose()
  {
    yield return (object) null;
  }

  public void SerializeOutBonesFromSkinnedMesh(SkinnedMeshRenderer paramSkinnedMeshRenderer)
  {
  }

  public void SetCurvesForBone(
    SkinnedMeshRenderer paramSkinnedMeshRenderer,
    AnimationClip clip,
    Transform bone)
  {
    Keyframe[] keyframeArray1 = new Keyframe[1]
    {
      new Keyframe(0.0f, bone.parent.localRotation.x)
    };
    Keyframe[] keyframeArray2 = new Keyframe[1]
    {
      new Keyframe(0.0f, bone.parent.localRotation.y)
    };
    Keyframe[] keyframeArray3 = new Keyframe[1]
    {
      new Keyframe(0.0f, bone.parent.localRotation.z)
    };
    Keyframe[] keyframeArray4 = new Keyframe[1]
    {
      new Keyframe(0.0f, bone.parent.localRotation.w)
    };
    AnimationCurve curve1 = new AnimationCurve(keyframeArray1);
    AnimationCurve curve2 = new AnimationCurve(keyframeArray2);
    AnimationCurve curve3 = new AnimationCurve(keyframeArray3);
    AnimationCurve curve4 = new AnimationCurve(keyframeArray4);
    string relativePath = "";
    string str = bone.name.Replace("_new", "");
    foreach (Transform bone1 in this.skinnedMeshRenderer.bones)
    {
      if (bone1.name == str)
      {
        relativePath = bone1.GetPath(this.skinnedMeshRenderer.transform.parent).TrimStart('/');
        break;
      }
    }
    clip.SetCurve(relativePath, typeof (Transform), "m_LocalRotation.x", curve1);
    clip.SetCurve(relativePath, typeof (Transform), "m_LocalRotation.y", curve2);
    clip.SetCurve(relativePath, typeof (Transform), "m_LocalRotation.z", curve3);
    clip.SetCurve(relativePath, typeof (Transform), "m_LocalRotation.w", curve4);
  }

  public void UpdatePrefabWithAnimationClip(string AnimationFileName)
  {
  }

  public void LoadPoseOntoMannequin(AnimationClip clip, float frameTime = 0.0f)
  {
  }

  public void OnValidate()
  {
  }
}
