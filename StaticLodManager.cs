// Decompiled with JetBrains decompiler
// Type: GorillaTag.StaticLodManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace GorillaTag;

[DefaultExecutionOrder(2000)]
public class StaticLodManager : MonoBehaviour, IGorillaSliceableSimple
{
  [OnEnterPlay_Clear]
  private static readonly List<StaticLodGroup> groupMonoBehaviours = new List<StaticLodGroup>(32 /*0x20*/);
  [OnEnterPlay_Clear]
  private static readonly Dictionary<int, int> _groupInstId_to_index = new Dictionary<int, int>(32 /*0x20*/);
  [DebugReadout]
  [OnEnterPlay_Clear]
  private static readonly List<StaticLodManager.GroupInfo> groupInfos = new List<StaticLodManager.GroupInfo>(32 /*0x20*/);
  [OnEnterPlay_Clear]
  private static readonly Stack<int> freeSlots = new Stack<int>();
  private Camera mainCamera;
  private bool hasMainCamera;

  public void OnEnable()
  {
    GorillaSlicerSimpleManager.RegisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
    this.mainCamera = Camera.main;
    this.hasMainCamera = (UnityEngine.Object) this.mainCamera != (UnityEngine.Object) null;
  }

  public void OnDisable()
  {
    GorillaSlicerSimpleManager.UnregisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
  }

  public static int Register(StaticLodGroup lodGroup)
  {
    int count;
    if (StaticLodManager.freeSlots.TryPop(ref count))
    {
      StaticLodManager.groupMonoBehaviours[count] = lodGroup;
      StaticLodManager.groupInfos[count] = new StaticLodManager.GroupInfo();
    }
    else
    {
      count = StaticLodManager.groupMonoBehaviours.Count;
      StaticLodManager.groupMonoBehaviours.Add(lodGroup);
      StaticLodManager.groupInfos.Add(new StaticLodManager.GroupInfo());
    }
    StaticLodManager._groupInstId_to_index[lodGroup.GetInstanceID()] = count;
    StaticLodManager.GroupInfo groupInfo1 = StaticLodManager.groupInfos[count] with
    {
      isLoaded = true,
      componentEnabled = lodGroup.isActiveAndEnabled,
      uiEnabled = true,
      collidersEnabled = true,
      uiEnableDistanceSq = lodGroup.uiFadeDistanceMax * lodGroup.uiFadeDistanceMax,
      collisionEnableDistanceSq = lodGroup.collisionEnableDistance * lodGroup.collisionEnableDistance
    };
    StaticLodManager.groupInfos[count] = groupInfo1;
    StaticLodManager._TryAddMembersToLodGroup(true, count);
    StaticLodManager.GroupInfo groupInfo2 = StaticLodManager.groupInfos[count];
    if (Mathf.Approximately(groupInfo2.radiusSq, 0.0f))
    {
      groupInfo2.bounds = new Bounds(lodGroup.transform.position, Vector3.one * 0.01f);
      groupInfo2.center = groupInfo2.bounds.center;
      groupInfo2.radiusSq = groupInfo2.bounds.extents.sqrMagnitude;
      StaticLodManager.groupInfos[count] = groupInfo2;
    }
    return count;
  }

  public static int OldRegister(StaticLodGroup lodGroup)
  {
    StaticLodGroupExcluder componentInParent1 = lodGroup.GetComponentInParent<StaticLodGroupExcluder>();
    List<Graphic> pooledList1;
    using (lodGroup.GTGetComponentsListPool<Graphic>(true, out pooledList1))
    {
      for (int index = pooledList1.Count - 1; index >= 0; --index)
      {
        StaticLodGroupExcluder componentInParent2 = pooledList1[index].GetComponentInParent<StaticLodGroupExcluder>(true);
        if ((UnityEngine.Object) componentInParent2 != (UnityEngine.Object) null && (UnityEngine.Object) componentInParent2 != (UnityEngine.Object) componentInParent1)
          pooledList1.RemoveAt(index);
      }
      Graphic[] array1 = pooledList1.ToArray();
      List<Renderer> pooledList2;
      using (lodGroup.GTGetComponentsListPool<Renderer>(true, out pooledList2))
      {
        for (int index = pooledList2.Count - 1; index >= 0; --index)
        {
          switch (pooledList2[index].gameObject.layer)
          {
            case 5:
            case 18:
              if (pooledList2[index].enabled)
              {
                StaticLodGroupExcluder componentInParent3 = pooledList1[index].GetComponentInParent<StaticLodGroupExcluder>(true);
                if ((UnityEngine.Object) componentInParent3 != (UnityEngine.Object) null && (UnityEngine.Object) componentInParent3 != (UnityEngine.Object) componentInParent1)
                {
                  pooledList2.RemoveAt(index);
                  break;
                }
                break;
              }
              goto default;
            default:
              pooledList2.RemoveAt(index);
              break;
          }
        }
        Renderer[] array2 = pooledList2.ToArray();
        List<Collider> pooledList3;
        using (lodGroup.GTGetComponentsListPool<Collider>(true, out pooledList3))
        {
          for (int index = 0; index < pooledList3.Count; ++index)
          {
            Collider collider = pooledList3[index];
            if (!collider.gameObject.IsOnLayer(UnityLayer.GorillaInteractable))
            {
              pooledList3.RemoveAt(index);
            }
            else
            {
              StaticLodGroupExcluder componentInParent4 = collider.GetComponentInParent<StaticLodGroupExcluder>();
              if ((UnityEngine.Object) componentInParent4 != (UnityEngine.Object) null && (UnityEngine.Object) componentInParent4 != (UnityEngine.Object) componentInParent1)
                pooledList3.RemoveAt(index);
            }
          }
          Collider[] array3 = pooledList3.ToArray();
          Bounds bounds = array2.Length != 0 ? array2[0].bounds : (array3.Length != 0 ? array3[0].bounds : (array1.Length != 0 ? new Bounds(array1[0].transform.position, Vector3.one * 0.01f) : new Bounds(lodGroup.transform.position, Vector3.one * 0.01f)));
          for (int index = 0; index < array1.Length; ++index)
            bounds.Encapsulate(array1[index].transform.position);
          for (int index = 0; index < array2.Length; ++index)
            bounds.Encapsulate(array2[index].bounds);
          for (int index = 0; index < array3.Length; ++index)
            bounds.Encapsulate(array3[index].bounds);
          StaticLodManager.GroupInfo groupInfo = new StaticLodManager.GroupInfo()
          {
            isLoaded = true,
            componentEnabled = lodGroup.isActiveAndEnabled,
            center = bounds.center,
            radiusSq = bounds.extents.sqrMagnitude,
            uiEnabled = true,
            uiEnableDistanceSq = lodGroup.uiFadeDistanceMax * lodGroup.uiFadeDistanceMax,
            uiGraphics = array1,
            renderers = array2,
            collidersEnabled = true,
            collisionEnableDistanceSq = lodGroup.collisionEnableDistance * lodGroup.collisionEnableDistance,
            interactableColliders = array3
          };
          int count;
          if (StaticLodManager.freeSlots.TryPop(ref count))
          {
            StaticLodManager.groupMonoBehaviours[count] = lodGroup;
            StaticLodManager.groupInfos[count] = groupInfo;
          }
          else
          {
            count = StaticLodManager.groupMonoBehaviours.Count;
            StaticLodManager.groupMonoBehaviours.Add(lodGroup);
            StaticLodManager.groupInfos.Add(groupInfo);
          }
          StaticLodManager._groupInstId_to_index[lodGroup.GetInstanceID()] = count;
          return count;
        }
      }
    }
  }

  public static void Unregister(int lodGroupIndex)
  {
    StaticLodGroup groupMonoBehaviour = StaticLodManager.groupMonoBehaviours[lodGroupIndex];
    if ((UnityEngine.Object) groupMonoBehaviour != (UnityEngine.Object) null)
      StaticLodManager._groupInstId_to_index.Remove(groupMonoBehaviour.GetInstanceID());
    StaticLodManager.groupMonoBehaviours[lodGroupIndex] = (StaticLodGroup) null;
    StaticLodManager.groupInfos[lodGroupIndex] = new StaticLodManager.GroupInfo();
    StaticLodManager.freeSlots.Push(lodGroupIndex);
  }

  public static bool TryAddLateInstantiatedMembers(GameObject root)
  {
    StaticLodGroup componentInParent1 = root.GetComponentInParent<StaticLodGroup>(true);
    int groupIndex;
    if ((UnityEngine.Object) componentInParent1 == (UnityEngine.Object) null || !StaticLodManager._groupInstId_to_index.TryGetValue(componentInParent1.GetInstanceID(), out groupIndex))
      return false;
    if ((UnityEngine.Object) componentInParent1.gameObject != (UnityEngine.Object) root)
    {
      StaticLodGroupExcluder componentInParent2 = root.GetComponentInParent<StaticLodGroupExcluder>(true);
      if ((UnityEngine.Object) componentInParent2 != (UnityEngine.Object) null && componentInParent1.transform.GetDepth() < componentInParent2.transform.GetDepth())
        return false;
    }
    return StaticLodManager._TryAddMembersToLodGroup(false, groupIndex);
  }

  private static bool _TryAddMembersToLodGroup(bool isNew, int groupIndex)
  {
    StaticLodGroup groupMonoBehaviour = StaticLodManager.groupMonoBehaviours[groupIndex];
    StaticLodManager.GroupInfo groupInfo = StaticLodManager.groupInfos[groupIndex];
    StaticLodGroupExcluder componentInParent = groupMonoBehaviour.GetComponentInParent<StaticLodGroupExcluder>();
    int num = 0 | (StaticLodManager._TryAddComponentsToGroup<Collider>(groupMonoBehaviour, componentInParent, ref groupInfo, ref groupInfo.interactableColliders, (Predicate<Collider>) (coll => coll.gameObject.IsOnLayer(UnityLayer.GorillaInteractable)), (StaticLodManager._GetBoundsDelegate<Collider>) (coll => coll.bounds)) ? 1 : 0) | (StaticLodManager._TryAddComponentsToGroup<Renderer>(groupMonoBehaviour, componentInParent, ref groupInfo, ref groupInfo.renderers, (Predicate<Renderer>) (rend =>
    {
      switch (rend.gameObject.layer)
      {
        case 5:
        case 18:
          return rend.enabled;
        default:
          return false;
      }
    }), (StaticLodManager._GetBoundsDelegate<Renderer>) (rend => rend.bounds)) ? 1 : 0) | (StaticLodManager._TryAddComponentsToGroup<Graphic>(groupMonoBehaviour, componentInParent, ref groupInfo, ref groupInfo.uiGraphics, (Predicate<Graphic>) (_ => true), (StaticLodManager._GetBoundsDelegate<Graphic>) (gfx => new Bounds(gfx.transform.position, Vector3.one * 0.01f))) ? 1 : 0);
    StaticLodManager.groupInfos[groupIndex] = groupInfo;
    return num != 0;
  }

  private static bool _TryAddComponentsToGroup<T>(
    StaticLodGroup lodGroup,
    StaticLodGroupExcluder excluderAboveGroup,
    ref StaticLodManager.GroupInfo ref_groupInfo,
    ref T[] ref_components,
    Predicate<T> includeIf,
    StaticLodManager._GetBoundsDelegate<T> getBounds)
    where T : Component
  {
    List<T> pooledList;
    using (lodGroup.GTGetComponentsListPool<T>(true, out pooledList))
    {
      for (int index = pooledList.Count - 1; index >= 0; --index)
      {
        if (!includeIf(pooledList[index]))
        {
          pooledList.RemoveAt(index);
        }
        else
        {
          StaticLodGroupExcluder componentInParent = pooledList[index].GetComponentInParent<StaticLodGroupExcluder>(true);
          if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null && (UnityEngine.Object) componentInParent != (UnityEngine.Object) excluderAboveGroup)
            pooledList.RemoveAt(index);
        }
      }
      if (pooledList.Count == 0)
      {
        if (ref_components == null)
          ref_components = Array.Empty<T>();
        return false;
      }
      T[] objArray = ref_components;
      int length = objArray != null ? objArray.Length : 0;
      if (length == 0)
      {
        ref_components = pooledList.ToArray();
      }
      else
      {
        Array.Resize<T>(ref ref_components, length + pooledList.Count);
        for (int index = length; index < ref_components.Length; ++index)
          ref_components[index] = pooledList[index - length];
      }
      if (Mathf.Approximately(ref_groupInfo.radiusSq, 0.0f))
        ref_groupInfo.bounds = getBounds(ref_components[0]);
      for (int index = length; index < ref_components.Length; ++index)
        ref_groupInfo.bounds.Encapsulate(getBounds(ref_components[index]));
      ref_groupInfo.center = ref_groupInfo.bounds.center;
      ref_groupInfo.radiusSq = ref_groupInfo.bounds.extents.sqrMagnitude;
      return true;
    }
  }

  [Conditional("UNITY_EDITOR")]
  private static void _EdAddPathsToGroup<T>(T[] components, ref string[] ref_edDebugPaths) where T : Component
  {
  }

  public static void SetEnabled(int index, bool enable)
  {
    if (ApplicationQuittingState.IsQuitting)
      return;
    StaticLodManager.GroupInfo groupInfo = StaticLodManager.groupInfos[index] with
    {
      componentEnabled = enable
    };
    StaticLodManager.groupInfos[index] = groupInfo;
  }

  public void SliceUpdate()
  {
    if (!this.hasMainCamera)
      return;
    Vector3 position = this.mainCamera.transform.position;
    for (int index1 = 0; index1 < StaticLodManager.groupInfos.Count; ++index1)
    {
      StaticLodManager.GroupInfo groupInfo = StaticLodManager.groupInfos[index1];
      if (groupInfo.isLoaded && groupInfo.componentEnabled)
      {
        float num1 = Mathf.Max(0.0f, (groupInfo.center - position).sqrMagnitude - groupInfo.radiusSq);
        float num2 = groupInfo.uiEnabled ? 0.0100000007f : 0.0f;
        bool flag1 = (double) num1 < (double) groupInfo.uiEnableDistanceSq + (double) num2;
        if (flag1 != groupInfo.uiEnabled)
        {
          for (int index2 = 0; index2 < groupInfo.uiGraphics.Length; ++index2)
          {
            Graphic uiGraphic = groupInfo.uiGraphics[index2];
            if (!((UnityEngine.Object) uiGraphic == (UnityEngine.Object) null))
              uiGraphic.enabled = flag1;
          }
          for (int index3 = 0; index3 < groupInfo.renderers.Length; ++index3)
          {
            Renderer renderer = groupInfo.renderers[index3];
            if (!((UnityEngine.Object) renderer == (UnityEngine.Object) null))
              renderer.enabled = flag1;
          }
        }
        groupInfo.uiEnabled = flag1;
        float num3 = groupInfo.collidersEnabled ? 0.0100000007f : 0.0f;
        bool flag2 = (double) num1 < (double) groupInfo.collisionEnableDistanceSq + (double) num3;
        if (flag2 != groupInfo.collidersEnabled)
        {
          for (int index4 = 0; index4 < groupInfo.interactableColliders.Length; ++index4)
          {
            if (!((UnityEngine.Object) groupInfo.interactableColliders[index4] == (UnityEngine.Object) null))
              groupInfo.interactableColliders[index4].enabled = flag2;
          }
        }
        groupInfo.collidersEnabled = flag2;
        StaticLodManager.groupInfos[index1] = groupInfo;
      }
    }
  }

  private struct GroupInfo
  {
    public bool isLoaded;
    public bool componentEnabled;
    public Vector3 center;
    public float radiusSq;
    public Bounds bounds;
    public bool uiEnabled;
    public float uiEnableDistanceSq;
    public Graphic[] uiGraphics;
    public Renderer[] renderers;
    public bool collidersEnabled;
    public float collisionEnableDistanceSq;
    public Collider[] interactableColliders;
  }

  private delegate Bounds _GetBoundsDelegate<in T>(T t) where T : Component;
}
