// Decompiled with JetBrains decompiler
// Type: GorillaTag.Rendering.EdMeshCombinerPrefab
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

#nullable disable
namespace GorillaTag.Rendering;

[DefaultExecutionOrder(-2147482648)]
public class EdMeshCombinerPrefab : MonoBehaviour
{
  public EdMeshCombinedPrefabData combinedData;
  private const uint _k_maxVertsForUInt16 = 65535 /*0xFFFF*/;
  private const uint _k_maxVertsForUInt32 = 4294967295 /*0xFFFFFFFF*/;
  private const uint _k_maxVertCount = 65535 /*0xFFFF*/;

  private void Awake()
  {
    if (this.combinedData == null)
      this.combinedData = new EdMeshCombinedPrefabData();
    EdMeshCombinerPrefab.CombineMeshesRuntime(this, combinedPrefabData: this.combinedData);
  }

  private static void Special_MarkDoNotCombine(Component component)
  {
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    GameObject gameObject = component.gameObject;
    if (!((UnityEngine.Object) gameObject.GetComponent<EdDoNotMeshCombine>() == (UnityEngine.Object) null))
      return;
    gameObject.AddComponent<EdDoNotMeshCombine>();
  }

  public static void CombineMeshesRuntime(
    EdMeshCombinerPrefab combiner,
    bool undo = false,
    EdMeshCombinedPrefabData combinedPrefabData = null)
  {
    bool flag1 = true;
    foreach (Campfire componentsInChild in combiner.GetComponentsInChildren<Campfire>(true))
    {
      EdMeshCombinerPrefab.Special_MarkDoNotCombine((Component) componentsInChild.baseFire);
      EdMeshCombinerPrefab.Special_MarkDoNotCombine((Component) componentsInChild.middleFire);
      EdMeshCombinerPrefab.Special_MarkDoNotCombine((Component) componentsInChild.topFire);
    }
    foreach (Component componentsInChild in combiner.GetComponentsInChildren<GameEntity>(true))
      EdMeshCombinerPrefab.Special_MarkDoNotCombine(componentsInChild);
    foreach (Component componentsInChild in combiner.GetComponentsInChildren<StaticLodGroup>(true))
      EdMeshCombinerPrefab.Special_MarkDoNotCombine(componentsInChild);
    foreach (Component componentsInChild in combiner.GetComponentsInChildren<GorillaCaveCrystalVisuals>(false))
      EdMeshCombinerPrefab.Special_MarkDoNotCombine(componentsInChild);
    foreach (Component componentsInChild in combiner.GetComponentsInChildren<WaterSurfaceMaterialController>(false))
      EdMeshCombinerPrefab.Special_MarkDoNotCombine(componentsInChild);
    List<Renderer> componentsInChildrenUntil = combiner.GetComponentsInChildrenUntil<Renderer, EdDoNotMeshCombine, EdMeshCombinerPrefab, TMP_Text>(stopAtRoot: false);
    List<Renderer> rendererList = new List<Renderer>(componentsInChildrenUntil.Count);
    foreach (Renderer renderer in componentsInChildrenUntil)
    {
      if (renderer is SkinnedMeshRenderer || renderer is MeshRenderer)
        rendererList.Add(renderer);
    }
    Dictionary<EdMeshCombinerPrefab.CombinerCriteria, List<List<EdMeshCombinerPrefab.CombinerInfo>>> dictionary1 = new Dictionary<EdMeshCombinerPrefab.CombinerCriteria, List<List<EdMeshCombinerPrefab.CombinerInfo>>>(rendererList.Count);
    List<Transform> transformList = new List<Transform>(rendererList.Count);
    EdMeshCombinerPrefab.CombinerCriteria combinerCriteria1;
    foreach (Renderer renderer1 in rendererList)
    {
      if (renderer1.enabled)
      {
        GameObject gameObject = renderer1.gameObject;
        int num1 = gameObject.isStatic ? 1 : 0;
        if (gameObject.isStatic)
        {
          SkinnedMeshRenderer skinnedMeshRenderer = renderer1 as SkinnedMeshRenderer;
          bool flag2 = (UnityEngine.Object) skinnedMeshRenderer != (UnityEngine.Object) null;
          MeshFilter meshFilter = (MeshFilter) null;
          UnityEngine.Mesh sharedMesh;
          if (flag2)
          {
            sharedMesh = skinnedMeshRenderer.sharedMesh;
          }
          else
          {
            meshFilter = renderer1.GetComponent<MeshFilter>();
            if (!((UnityEngine.Object) meshFilter == (UnityEngine.Object) null))
              sharedMesh = meshFilter.sharedMesh;
            else
              continue;
          }
          if (!((UnityEngine.Object) sharedMesh == (UnityEngine.Object) null) && sharedMesh.vertexCount < (int) ushort.MaxValue)
          {
            MeshCollider component1 = renderer1.GetComponent<MeshCollider>();
            bool flag3 = (UnityEngine.Object) component1 != (UnityEngine.Object) null;
            if (!(!flag1 & flag3) || !((UnityEngine.Object) component1.sharedMesh == (UnityEngine.Object) null) && !component1.convex && !((UnityEngine.Object) component1.sharedMesh != (UnityEngine.Object) sharedMesh))
            {
              GorillaSurfaceOverride component2 = renderer1.GetComponent<GorillaSurfaceOverride>();
              int overrideIndex = (UnityEngine.Object) component2 != (UnityEngine.Object) null ? component2.overrideIndex : 0;
              int num2 = Mathf.Min(renderer1.sharedMaterials.Length, sharedMesh.subMeshCount);
              if (num2 != 0)
              {
                int num3 = 0;
                int num4 = 0;
                for (int index = 0; index < num2; ++index)
                {
                  num3 += sharedMesh.GetSubMesh(index).topology != MeshTopology.Triangles ? 1 : 0;
                  num4 += (UnityEngine.Object) renderer1.sharedMaterials[index] == (UnityEngine.Object) null ? 1 : 0;
                }
                if (num3 > 0)
                {
                  string str = "?????";
                  Debug.LogError((object) $"{$"Cannot combine mesh \"{sharedMesh.name}\" because it has {num3} submeshes with "}a non-triangle topology. Verify FBX import settings does not have \"Keep Quads\" on.\n  - Asset path=\"{str}\"\n  - Path in scene={renderer1.transform.GetPathQ()}", (UnityEngine.Object) sharedMesh);
                }
                else if (num4 > 0)
                {
                  Debug.LogError((object) $"EdMeshCombinerPrefab: Cannot combine Renderer \"{combiner.name}\" because it does not have {$"{num4} materials assigned. Path in scene={combiner.transform.GetPathQ()}"}", (UnityEngine.Object) combiner);
                }
                else
                {
                  for (int index1 = 0; index1 < num2; ++index1)
                  {
                    Material sharedMaterial = renderer1.sharedMaterials[index1];
                    int layer = renderer1.gameObject.layer;
                    combinerCriteria1 = new EdMeshCombinerPrefab.CombinerCriteria();
                    combinerCriteria1.mat = sharedMaterial;
                    combinerCriteria1.staticFlags = num1;
                    combinerCriteria1.lightmapIndex = renderer1.lightmapIndex;
                    combinerCriteria1.hasMeshCollider = !flag1 & flag3;
                    combinerCriteria1.meshCollPhysicsMat = flag1 ? (PhysicsMaterial) null : (flag3 ? component1.sharedMaterial : (PhysicsMaterial) null);
                    combinerCriteria1.surfOverrideIndex = flag1 ? 0 : overrideIndex;
                    combinerCriteria1.surfExtraVelMultiplier = flag1 ? 0.0f : ((UnityEngine.Object) component2 != (UnityEngine.Object) null ? component2.extraVelMultiplier : 1f);
                    combinerCriteria1.surfExtraVelMaxMultiplier = flag1 ? 0.0f : ((UnityEngine.Object) component2 != (UnityEngine.Object) null ? component2.extraVelMaxMultiplier : 1f);
                    combinerCriteria1.surfSendOnTapEvent = !flag1 && (UnityEngine.Object) component2 != (UnityEngine.Object) null && component2.sendOnTapEvent;
                    combinerCriteria1.objectLayer = layer == 27 ? UnityLayer.NoMirror : UnityLayer.Default;
                    EdMeshCombinerPrefab.CombinerCriteria key = combinerCriteria1;
                    List<List<EdMeshCombinerPrefab.CombinerInfo>> combinerInfoListList;
                    if (!dictionary1.TryGetValue(key, out combinerInfoListList))
                    {
                      combinerInfoListList = new List<List<EdMeshCombinerPrefab.CombinerInfo>>()
                      {
                        new List<EdMeshCombinerPrefab.CombinerInfo>(1)
                      };
                      dictionary1[key] = combinerInfoListList;
                    }
                    int index2 = combinerInfoListList.Count - 1;
                    int vertexCount = sharedMesh.vertexCount;
                    foreach (EdMeshCombinerPrefab.CombinerInfo combinerInfo in combinerInfoListList[index2])
                    {
                      if (combinerInfo.isSkinnedMesh)
                      {
                        SkinnedMeshRenderer renderer2 = (SkinnedMeshRenderer) combinerInfo.renderer;
                        vertexCount += renderer2.sharedMesh.vertexCount;
                      }
                      else
                        vertexCount += combinerInfo.meshFilter.sharedMesh.vertexCount;
                    }
                    if (vertexCount >= (int) ushort.MaxValue)
                    {
                      index2 = combinerInfoListList.Count;
                      combinerInfoListList.Add(new List<EdMeshCombinerPrefab.CombinerInfo>(1));
                    }
                    transformList.Add(gameObject.transform);
                    combinerInfoListList[index2].Add(new EdMeshCombinerPrefab.CombinerInfo()
                    {
                      meshFilter = meshFilter,
                      renderer = renderer1,
                      uvOffsetModifier = renderer1.GetComponent<EdMeshCombinerModifierUVOffset>(),
                      subMeshIndex = index1,
                      isSkinnedMesh = flag2,
                      layer = renderer1.sortingLayerID
                    });
                  }
                }
              }
            }
          }
        }
      }
    }
    Matrix4x4 worldToLocalMatrix = combiner.transform.worldToLocalMatrix;
    PerSceneRenderData perSceneRenderData = (PerSceneRenderData) null;
    bool flag4 = false;
    Unity.Mathematics.Random random = new Unity.Mathematics.Random(6746U);
    foreach (KeyValuePair<EdMeshCombinerPrefab.CombinerCriteria, List<List<EdMeshCombinerPrefab.CombinerInfo>>> keyValuePair in dictionary1)
    {
      List<List<EdMeshCombinerPrefab.CombinerInfo>> combinerInfoListList1;
      keyValuePair.Deconstruct(ref combinerCriteria1, ref combinerInfoListList1);
      EdMeshCombinerPrefab.CombinerCriteria combinerCriteria2 = combinerCriteria1;
      List<List<EdMeshCombinerPrefab.CombinerInfo>> combinerInfoListList2 = combinerInfoListList1;
      bool flag5 = false;
      foreach (List<EdMeshCombinerPrefab.CombinerInfo> combinerInfoList in combinerInfoListList2)
      {
        List<UnityEngine.Mesh> meshes = new List<UnityEngine.Mesh>(combinerInfoList.Count);
        List<int> intList1 = new List<int>(combinerInfoList.Count);
        List<Matrix4x4> matrix4x4List = new List<Matrix4x4>(combinerInfoList.Count);
        List<Color> colorList = new List<Color>(combinerInfoList.Count);
        List<int> intList2 = new List<int>(combinerInfoList.Count);
        List<float4> float4List1 = new List<float4>(combinerInfoList.Count);
        List<float4> float4List2 = new List<float4>(combinerInfoList.Count);
        Dictionary<(Renderer, int), (Color, int)> dictionary2 = new Dictionary<(Renderer, int), (Color, int)>();
        foreach (EdMeshCombinerPrefab.CombinerInfo combinerInfo in combinerInfoList)
        {
          MaterialCombinerPerRendererMono component;
          MaterialCombinerPerRendererInfo data;
          if (combinerInfo.renderer.TryGetComponent<MaterialCombinerPerRendererMono>(out component) && component.TryGetData(combinerInfo.renderer, combinerInfo.subMeshIndex, out data))
            dictionary2[(combinerInfo.renderer, combinerInfo.subMeshIndex)] = (data.baseColor, data.sliceIndex);
          else
            dictionary2[(combinerInfo.renderer, combinerInfo.subMeshIndex)] = (Color.white, -1);
        }
        for (int index = 0; index < combinerInfoList.Count; ++index)
        {
          EdMeshCombinerPrefab.CombinerInfo combinerInfo = combinerInfoList[index];
          UnityEngine.Mesh mesh1;
          if (combinerInfo.isSkinnedMesh)
          {
            SkinnedMeshRenderer renderer = (SkinnedMeshRenderer) combinerInfo.renderer;
            mesh1 = new UnityEngine.Mesh();
            UnityEngine.Mesh mesh2 = mesh1;
            renderer.BakeMesh(mesh2, true);
          }
          else
            mesh1 = combinerInfo.meshFilter.sharedMesh;
          if (mesh1.vertexCount != 0)
          {
            if ((UnityEngine.Object) perSceneRenderData != (UnityEngine.Object) null && (UnityEngine.Object) perSceneRenderData.representativeRenderer == (UnityEngine.Object) combinerInfo.renderer)
              flag4 = true;
            meshes.Add(mesh1);
            intList1.Add(combinerInfo.subMeshIndex);
            matrix4x4List.Add(worldToLocalMatrix * combinerInfo.renderer.transform.localToWorldMatrix);
            float4List2.Add(combinerInfo.uvOffsetModifier == null ? float4.zero : new float4(combinerInfo.uvOffsetModifier.minUvOffset.x, combinerInfo.uvOffsetModifier.minUvOffset.y, combinerInfo.uvOffsetModifier.maxUvOffset.x, combinerInfo.uvOffsetModifier.maxUvOffset.y));
            float4List1.Add((float4) combinerInfo.renderer.lightmapScaleOffset);
            (Color color, int num) = dictionary2[(combinerInfo.renderer, combinerInfo.subMeshIndex)];
            colorList.Add(color);
            intList2.Add(num);
          }
        }
        using (UnityEngine.Mesh.MeshDataArray meshDataArray = UnityEngine.Mesh.AcquireReadOnlyMeshData(meshes))
        {
          int totalVertexCount = 0;
          int indexCount = 0;
          for (int index = 0; index < meshDataArray.Length; ++index)
          {
            UnityEngine.Mesh.MeshData meshData = meshDataArray[index];
            totalVertexCount += meshData.vertexCount;
            indexCount += meshData.GetSubMesh(intList1[index]).indexCount;
          }
          UnityEngine.Mesh.MeshDataArray data = UnityEngine.Mesh.AllocateWritableMeshData(1);
          UnityEngine.Mesh.MeshData writeData = data[0];
          IndexFormat format = totalVertexCount > (int) ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;
          GTVertexDataStreams_Descriptors.DoSetVertexBufferParams(ref writeData, totalVertexCount);
          writeData.SetIndexBufferParams(indexCount, format);
          writeData.subMeshCount = 1;
          NativeArray<int> nativeArray1 = new NativeArray<int>();
          NativeArray<ushort> nativeArray2 = new NativeArray<ushort>();
          if (format == IndexFormat.UInt32)
            nativeArray1 = writeData.GetIndexData<int>();
          else
            nativeArray2 = writeData.GetIndexData<ushort>();
          EdMeshCombinerPrefab.CopyMeshJob jobData = new EdMeshCombinerPrefab.CopyMeshJob()
          {
            meshDataArray = meshDataArray,
            sourceSubmeshIndices = new NativeArray<int>(intList1.ToArray(), Allocator.TempJob),
            sourceTransforms = new NativeArray<Matrix4x4>(matrix4x4List.ToArray(), Allocator.TempJob),
            lightmapScaleOffsets = new NativeArray<float4>(float4List1.ToArray(), Allocator.TempJob),
            baseColors = new NativeArray<Color>(colorList.ToArray(), Allocator.TempJob),
            atlasSlices = new NativeArray<int>(intList2.ToArray(), Allocator.TempJob),
            uvModifiersMinMax = new NativeArray<float4>(float4List2.ToArray(), Allocator.TempJob),
            isCandleFlame = flag5,
            randSeed = 6746,
            dst0 = writeData.GetVertexData<GTVertexDataStream0>(),
            dst1 = writeData.GetVertexData<GTVertexDataStream1>(1),
            idxDst32 = nativeArray1,
            idxDst16 = nativeArray2,
            use32BitIndices = format == IndexFormat.UInt32
          };
          jobData.Schedule<EdMeshCombinerPrefab.CopyMeshJob>().Complete();
          jobData.sourceSubmeshIndices.Dispose();
          jobData.sourceTransforms.Dispose();
          jobData.baseColors.Dispose();
          jobData.atlasSlices.Dispose();
          jobData.uvModifiersMinMax.Dispose();
          writeData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount));
          UnityEngine.Mesh mesh = new UnityEngine.Mesh();
          UnityEngine.Mesh.ApplyAndDisposeWritableMeshData(data, mesh);
          mesh.RecalculateBounds();
          GameObject gameObject = new GameObject(combinerCriteria2.mat.name + " (combined by EdMeshCombinerPrefab)");
          combinedPrefabData?.combined.Add(gameObject);
          if ((UnityEngine.Object) combiner.transform != (UnityEngine.Object) null)
            gameObject.transform.parent = combiner.transform;
          else
            SceneManager.MoveGameObjectToScene(gameObject, combiner.gameObject.scene);
          gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
          gameObject.transform.localScale = Vector3.one;
          gameObject.isStatic = true;
          gameObject.layer = (int) combinerCriteria2.objectLayer;
          MeshRenderer mR = gameObject.AddComponent<MeshRenderer>();
          mR.sharedMaterial = combinerCriteria2.mat;
          mR.lightmapIndex = combinerCriteria2.lightmapIndex;
          if (flag4)
            perSceneRenderData.representativeRenderer = (Renderer) mR;
          if ((UnityEngine.Object) perSceneRenderData != (UnityEngine.Object) null)
            perSceneRenderData.AddMeshToList(gameObject, mR);
          MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
          meshFilter.sharedMesh = mesh;
          if (!flag1)
          {
            if (combinerCriteria2.hasMeshCollider)
            {
              MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
              meshCollider.sharedMesh = meshFilter.sharedMesh;
              meshCollider.convex = false;
              meshCollider.sharedMaterial = combinerCriteria2.meshCollPhysicsMat;
              GorillaSurfaceOverride gorillaSurfaceOverride = gameObject.AddComponent<GorillaSurfaceOverride>();
              gorillaSurfaceOverride.overrideIndex = combinerCriteria2.surfOverrideIndex;
              gorillaSurfaceOverride.extraVelMultiplier = combinerCriteria2.surfExtraVelMultiplier;
              gorillaSurfaceOverride.extraVelMaxMultiplier = combinerCriteria2.surfExtraVelMaxMultiplier;
              gorillaSurfaceOverride.sendOnTapEvent = combinerCriteria2.surfSendOnTapEvent;
            }
          }
        }
      }
    }
    transformList.Sort((Comparison<Transform>) ((a, b) => -a.GetDepth().CompareTo(b.GetDepth())));
    foreach (Transform transform in transformList)
    {
      if (!((UnityEngine.Object) transform == (UnityEngine.Object) null) && combinedPrefabData != null)
      {
        MeshRenderer component = transform.GetComponent<MeshRenderer>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          component.enabled = false;
          combinedPrefabData.disabled.Add((Renderer) component);
        }
      }
    }
  }

  protected void OnEnable()
  {
  }

  [Serializable]
  public struct CombinerInfo
  {
    public MeshFilter meshFilter;
    public Renderer renderer;
    public EdMeshCombinerModifierUVOffset uvOffsetModifier;
    public int subMeshIndex;
    public bool isSkinnedMesh;
    public int layer;
  }

  private struct CombinerCriteria
  {
    public Material mat;
    public int staticFlags;
    public int lightmapIndex;
    public bool hasMeshCollider;
    public PhysicsMaterial meshCollPhysicsMat;
    public int surfOverrideIndex;
    public float surfExtraVelMultiplier;
    public float surfExtraVelMaxMultiplier;
    public bool surfSendOnTapEvent;
    public UnityLayer objectLayer;

    public override int GetHashCode()
    {
      return HashCode.Combine<int, int, int, bool, int, float, float, bool>(this.mat.GetInstanceID(), this.staticFlags, this.lightmapIndex, this.hasMeshCollider, this.surfOverrideIndex, this.surfExtraVelMultiplier, this.surfExtraVelMaxMultiplier, this.surfSendOnTapEvent);
    }
  }

  [BurstCompile]
  private struct CopyMeshJob : IJob
  {
    [ReadOnly]
    public UnityEngine.Mesh.MeshDataArray meshDataArray;
    [ReadOnly]
    public NativeArray<int> sourceSubmeshIndices;
    [ReadOnly]
    public NativeArray<Matrix4x4> sourceTransforms;
    [ReadOnly]
    public NativeArray<float4> lightmapScaleOffsets;
    [ReadOnly]
    public NativeArray<Color> baseColors;
    [ReadOnly]
    public NativeArray<int> atlasSlices;
    [ReadOnly]
    public NativeArray<float4> uvModifiersMinMax;
    public bool isCandleFlame;
    public uint randSeed;
    [WriteOnly]
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<GTVertexDataStream0> dst0;
    [WriteOnly]
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<GTVertexDataStream1> dst1;
    [WriteOnly]
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<int> idxDst32;
    [WriteOnly]
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<ushort> idxDst16;
    public bool use32BitIndices;

    public void Execute()
    {
      int num1 = 0;
      int num2 = 0;
      Unity.Mathematics.Random random = new Unity.Mathematics.Random(this.randSeed);
      for (int index1 = 0; index1 < this.meshDataArray.Length; ++index1)
      {
        UnityEngine.Mesh.MeshData meshData = this.meshDataArray[index1];
        int sourceSubmeshIndex = this.sourceSubmeshIndices[index1];
        SubMeshDescriptor subMesh = meshData.GetSubMesh(sourceSubmeshIndex);
        int vertexCount = meshData.vertexCount;
        int indexCount = subMesh.indexCount;
        Matrix4x4 sourceTransform = this.sourceTransforms[index1];
        bool flag = (double) math.determinant((float4x4) sourceTransform) < 0.0;
        NativeArray<Vector3> outVertices = new NativeArray<Vector3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        if (meshData.HasVertexAttribute(VertexAttribute.Position))
        {
          meshData.GetVertices(outVertices);
        }
        else
        {
          for (int index2 = 0; index2 < vertexCount; ++index2)
            outVertices[index2] = Vector3.zero;
        }
        NativeArray<Vector3> outNormals = new NativeArray<Vector3>(vertexCount, Allocator.Temp);
        if (meshData.HasVertexAttribute(VertexAttribute.Normal))
        {
          meshData.GetNormals(outNormals);
        }
        else
        {
          for (int index3 = 0; index3 < vertexCount; ++index3)
            outNormals[index3] = Vector3.up;
        }
        NativeArray<Vector4> outTangents = new NativeArray<Vector4>(vertexCount, Allocator.Temp);
        if (meshData.HasVertexAttribute(VertexAttribute.Tangent))
        {
          meshData.GetTangents(outTangents);
        }
        else
        {
          for (int index4 = 0; index4 < vertexCount; ++index4)
            outTangents[index4] = new Vector4(1f, 0.0f, 0.0f, 1f);
        }
        NativeArray<Color> outColors = new NativeArray<Color>(vertexCount, Allocator.Temp);
        if (meshData.HasVertexAttribute(VertexAttribute.Color))
        {
          meshData.GetColors(outColors);
        }
        else
        {
          for (int index5 = 0; index5 < vertexCount; ++index5)
            outColors[index5] = Color.white;
        }
        NativeArray<Vector2> outUVs1 = new NativeArray<Vector2>(vertexCount, Allocator.Temp);
        if (meshData.HasVertexAttribute(VertexAttribute.TexCoord0))
        {
          meshData.GetUVs(0, outUVs1);
        }
        else
        {
          for (int index6 = 0; index6 < vertexCount; ++index6)
            outUVs1[index6] = Vector2.zero;
        }
        NativeArray<Vector2> outUVs2 = new NativeArray<Vector2>(vertexCount, Allocator.Temp);
        if (meshData.HasVertexAttribute(VertexAttribute.TexCoord1))
        {
          meshData.GetUVs(1, outUVs2);
        }
        else
        {
          for (int index7 = 0; index7 < vertexCount; ++index7)
            outUVs2[index7] = Vector2.zero;
        }
        Color baseColor = this.baseColors[index1];
        int atlasSlice = this.atlasSlices[index1];
        Vector4 vector4_1 = (Vector4) this.uvModifiersMinMax[index1];
        Vector2 vector2_1 = new Vector2(random.NextFloat(vector4_1.x, vector4_1.z), random.NextFloat(vector4_1.y, vector4_1.w));
        float w = this.isCandleFlame ? random.NextFloat(0.0f, 1f) : 1f;
        Matrix4x4 transpose = sourceTransform.inverse.transpose;
        for (int index8 = 0; index8 < vertexCount; ++index8)
        {
          Vector3 point = outVertices[index8];
          Vector3 vector = outNormals[index8];
          Vector4 vector4_2 = outTangents[index8];
          Color color = outColors[index8];
          Vector2 vector2_2 = outUVs1[index8];
          Vector3 vector3_1 = sourceTransform.MultiplyPoint3x4(point);
          Vector3 vector3_2 = transpose.MultiplyVector(vector);
          Vector3 vector3_3 = vector3_2.normalized;
          vector3_2 = transpose.MultiplyVector(new Vector3(vector4_2.x, vector4_2.y, vector4_2.z));
          Vector3 vector3_4 = vector3_2.normalized;
          if (flag)
          {
            vector3_3 = -vector3_3;
            vector3_4 = -vector3_4;
            vector4_2.w = -vector4_2.w;
          }
          GTVertexDataStream0 vertexDataStream0 = new GTVertexDataStream0()
          {
            position = (float3) vector3_1,
            color = (Color32) new Color(color.r * baseColor.r, color.g * baseColor.g, color.b * baseColor.b, this.isCandleFlame ? w : color.a * baseColor.a),
            uv1 = new half4((half) (vector2_2.x + vector2_1.x), (half) (vector2_2.y + vector2_1.y), (half) (float) atlasSlice, (half) w),
            lightmapUv = new half2((half) (outUVs2[index8].x * this.lightmapScaleOffsets[index1].x + this.lightmapScaleOffsets[index1].z), (half) (outUVs2[index8].y * this.lightmapScaleOffsets[index1].y + this.lightmapScaleOffsets[index1].w))
          };
          this.dst0[num1 + index8] = vertexDataStream0;
          GTVertexDataStream1 vertexDataStream1 = new GTVertexDataStream1()
          {
            normal = (float3) vector3_3,
            tangent = (Color32) new Color(vector3_4.x, vector3_4.y, vector3_4.z, vector4_2.w)
          };
          this.dst1[num1 + index8] = vertexDataStream1;
        }
        if (this.use32BitIndices)
        {
          NativeArray<int> outIndices = new NativeArray<int>(indexCount, Allocator.Temp);
          meshData.GetIndices(outIndices, sourceSubmeshIndex);
          if (!flag)
          {
            for (int index9 = 0; index9 < indexCount; ++index9)
              this.idxDst32[num2 + index9] = num1 + outIndices[index9];
          }
          else
          {
            for (int index10 = 0; index10 < indexCount; index10 += 3)
            {
              this.idxDst32[num2 + index10] = num1 + outIndices[index10 + 2];
              this.idxDst32[num2 + index10 + 1] = num1 + outIndices[index10 + 1];
              this.idxDst32[num2 + index10 + 2] = num1 + outIndices[index10];
            }
          }
          outIndices.Dispose();
        }
        else
        {
          NativeArray<ushort> outIndices = new NativeArray<ushort>(indexCount, Allocator.Temp);
          meshData.GetIndices(outIndices, sourceSubmeshIndex);
          if (!flag)
          {
            for (int index11 = 0; index11 < indexCount; ++index11)
              this.idxDst16[num2 + index11] = (ushort) ((uint) num1 + (uint) outIndices[index11]);
          }
          else
          {
            for (int index12 = 0; index12 < indexCount; index12 += 3)
            {
              this.idxDst16[num2 + index12] = (ushort) ((uint) num1 + (uint) outIndices[index12 + 2]);
              this.idxDst16[num2 + index12 + 1] = (ushort) ((uint) num1 + (uint) outIndices[index12 + 1]);
              this.idxDst16[num2 + index12 + 2] = (ushort) ((uint) num1 + (uint) outIndices[index12]);
            }
          }
          outIndices.Dispose();
        }
        outVertices.Dispose();
        outNormals.Dispose();
        outTangents.Dispose();
        outColors.Dispose();
        outUVs1.Dispose();
        outUVs2.Dispose();
        num1 += vertexCount;
        num2 += indexCount;
      }
    }
  }
}
