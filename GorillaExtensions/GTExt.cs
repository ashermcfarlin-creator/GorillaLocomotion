// Decompiled with JetBrains decompiler
// Type: GorillaExtensions.GTExt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Cysharp.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

#nullable disable
namespace GorillaExtensions;

public static class GTExt
{
  private static Dictionary<Transform, Dictionary<string, Transform>> caseSenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();
  private static Dictionary<Transform, Dictionary<string, Transform>> caseInsenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();
  public static Dictionary<string, string> allStringsUsed = new Dictionary<string, string>();

  public static T GetComponentInHierarchy<T>(this Scene scene, bool includeInactive = true) where T : Component
  {
    if (!scene.IsValid())
      return default (T);
    foreach (GameObject rootGameObject in scene.GetRootGameObjects())
    {
      T component1 = rootGameObject.GetComponent<T>();
      if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
        return component1;
      foreach (Component componentsInChild in rootGameObject.GetComponentsInChildren<Transform>(includeInactive))
      {
        T component2 = componentsInChild.GetComponent<T>();
        if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
          return component2;
      }
    }
    return default (T);
  }

  public static List<T> GetComponentsInHierarchy<T>(
    this Scene scene,
    bool includeInactive = true,
    int capacity = 64 /*0x40*/)
  {
    List<T> componentsInHierarchy = new List<T>(capacity);
    if (!scene.IsValid())
      return componentsInHierarchy;
    foreach (GameObject rootGameObject in scene.GetRootGameObjects())
    {
      T[] componentsInChildren = rootGameObject.GetComponentsInChildren<T>(includeInactive);
      componentsInHierarchy.AddRange((IEnumerable<T>) componentsInChildren);
    }
    return componentsInHierarchy;
  }

  public static List<UnityEngine.Object> GetComponentsInHierarchy(
    this Scene scene,
    System.Type type,
    bool includeInactive = true,
    int capacity = 64 /*0x40*/)
  {
    List<UnityEngine.Object> componentsInHierarchy = new List<UnityEngine.Object>(capacity);
    foreach (GameObject rootGameObject in scene.GetRootGameObjects())
    {
      Component[] componentsInChildren = rootGameObject.GetComponentsInChildren(type, includeInactive);
      componentsInHierarchy.AddRange((IEnumerable<UnityEngine.Object>) componentsInChildren);
    }
    return componentsInHierarchy;
  }

  public static List<GameObject> GetGameObjectsInHierarchy(
    this Scene scene,
    bool includeInactive = true,
    int capacity = 64 /*0x40*/)
  {
    return scene.GetComponentsInHierarchy<GameObject>(includeInactive, capacity);
  }

  public static List<T> GetComponentsInHierarchyUntil<T, TStop1>(
    this Scene scene,
    bool includeInactive = false,
    bool stopAtRoot = true,
    int capacity = 64 /*0x40*/)
    where T : Component
    where TStop1 : Component
  {
    List<T> inHierarchyUntil = new List<T>(capacity);
    foreach (GameObject rootGameObject in scene.GetRootGameObjects())
    {
      List<T> componentsInChildrenUntil = rootGameObject.transform.GetComponentsInChildrenUntil<T, TStop1>(includeInactive, stopAtRoot, capacity);
      inHierarchyUntil.AddRange((IEnumerable<T>) componentsInChildrenUntil);
    }
    return inHierarchyUntil;
  }

  public static List<T> GetComponentsInHierarchyUntil<T, TStop1, TStop2>(
    this Scene scene,
    bool includeInactive = false,
    bool stopAtRoot = true,
    int capacity = 64 /*0x40*/)
    where T : Component
    where TStop1 : Component
    where TStop2 : Component
  {
    List<T> inHierarchyUntil = new List<T>(capacity);
    foreach (GameObject rootGameObject in scene.GetRootGameObjects())
    {
      List<T> componentsInChildrenUntil = rootGameObject.transform.GetComponentsInChildrenUntil<T, TStop1, TStop2>(includeInactive, stopAtRoot, capacity);
      inHierarchyUntil.AddRange((IEnumerable<T>) componentsInChildrenUntil);
    }
    return inHierarchyUntil;
  }

  public static List<T> GetComponentsInHierarchyUntil<T, TStop1, TStop2, TStop3>(
    this Scene scene,
    bool includeInactive = false,
    bool stopAtRoot = true,
    int capacity = 64 /*0x40*/)
    where T : Component
    where TStop1 : Component
    where TStop2 : Component
    where TStop3 : Component
  {
    List<T> inHierarchyUntil = new List<T>(capacity);
    foreach (GameObject rootGameObject in scene.GetRootGameObjects())
    {
      List<T> componentsInChildrenUntil = rootGameObject.transform.GetComponentsInChildrenUntil<T, TStop1, TStop2, TStop3>(includeInactive, stopAtRoot, capacity);
      inHierarchyUntil.AddRange((IEnumerable<T>) componentsInChildrenUntil);
    }
    return inHierarchyUntil;
  }

  public static List<T> GetComponentsInChildrenUntil<T, TStop1>(
    this Component root,
    bool includeInactive = false,
    bool stopAtRoot = true,
    int capacity = 64 /*0x40*/)
    where T : Component
    where TStop1 : Component
  {
    List<T> components = new List<T>(capacity);
    if (stopAtRoot && (UnityEngine.Object) root.GetComponent<TStop1>() != (UnityEngine.Object) null)
      return components;
    T component = root.GetComponent<T>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      components.Add(component);
    GetRecursive(root.transform, ref components);
    return components;

    void GetRecursive(Transform currentTransform, ref List<T> components)
    {
      foreach (Transform currentTransform1 in currentTransform)
      {
        if ((includeInactive || currentTransform1.gameObject.activeSelf) && !((UnityEngine.Object) currentTransform1.GetComponent<TStop1>() != (UnityEngine.Object) null))
        {
          T component = currentTransform1.GetComponent<T>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            components.Add(component);
          GetRecursive(currentTransform1, ref components);
        }
      }
    }
  }

  public static PooledObject<List<T>> GTGetComponentsListPool<T>(
    this Component root,
    bool includeInactive,
    out List<T> pooledList)
    where T : Component
  {
    PooledObject<List<T>> componentsListPool = UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out pooledList);
    root.GetComponentsInChildren<T>(includeInactive, pooledList);
    return componentsListPool;
  }

  public static PooledObject<List<T>> GTGetComponentsListPool<T>(
    this Component root,
    out List<T> pooledList)
    where T : Component
  {
    PooledObject<List<T>> componentsListPool = UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out pooledList);
    root.GetComponentsInChildren<T>(pooledList);
    return componentsListPool;
  }

  public static List<T> GetComponentsInChildrenUntil<T, TStop1, TStop2>(
    this Component root,
    bool includeInactive = false,
    bool stopAtRoot = true,
    int capacity = 64 /*0x40*/)
    where T : Component
    where TStop1 : Component
    where TStop2 : Component
  {
    List<T> components = new List<T>(capacity);
    if (stopAtRoot && ((UnityEngine.Object) root.GetComponent<TStop1>() != (UnityEngine.Object) null || (UnityEngine.Object) root.GetComponent<TStop2>() != (UnityEngine.Object) null))
      return components;
    T component = root.GetComponent<T>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      components.Add(component);
    GetRecursive(root.transform, ref components);
    return components;

    void GetRecursive(Transform currentTransform, ref List<T> components)
    {
      foreach (Transform currentTransform1 in currentTransform)
      {
        if ((includeInactive || currentTransform1.gameObject.activeSelf) && !((UnityEngine.Object) currentTransform1.GetComponent<TStop1>() != (UnityEngine.Object) null) && !((UnityEngine.Object) currentTransform1.GetComponent<TStop2>() != (UnityEngine.Object) null))
        {
          T component = currentTransform1.GetComponent<T>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            components.Add(component);
          GetRecursive(currentTransform1, ref components);
        }
      }
    }
  }

  public static List<T> GetComponentsInChildrenUntil<T, TStop1, TStop2, TStop3>(
    this Component root,
    bool includeInactive = false,
    bool stopAtRoot = true,
    int capacity = 64 /*0x40*/)
    where T : Component
    where TStop1 : Component
    where TStop2 : Component
    where TStop3 : Component
  {
    List<T> components = new List<T>(capacity);
    if (stopAtRoot && ((UnityEngine.Object) root.GetComponent<TStop1>() != (UnityEngine.Object) null || (UnityEngine.Object) root.GetComponent<TStop2>() != (UnityEngine.Object) null || (UnityEngine.Object) root.GetComponent<TStop3>() != (UnityEngine.Object) null))
      return components;
    T component = root.GetComponent<T>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      components.Add(component);
    GetRecursive(root.transform, ref components);
    return components;

    void GetRecursive(Transform currentTransform, ref List<T> components)
    {
      foreach (Transform currentTransform1 in currentTransform)
      {
        if ((includeInactive || currentTransform1.gameObject.activeSelf) && !((UnityEngine.Object) currentTransform1.GetComponent<TStop1>() != (UnityEngine.Object) null) && !((UnityEngine.Object) currentTransform1.GetComponent<TStop2>() != (UnityEngine.Object) null) && !((UnityEngine.Object) currentTransform1.GetComponent<TStop3>() != (UnityEngine.Object) null))
        {
          T component = currentTransform1.GetComponent<T>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            components.Add(component);
          GetRecursive(currentTransform1, ref components);
        }
      }
    }
  }

  public static void GetComponentsInChildrenUntil<T, TStop1, TStop2, TStop3>(
    this Component root,
    out List<T> out_included,
    out HashSet<T> out_excluded,
    bool includeInactive = false,
    bool stopAtRoot = true,
    int capacity = 64 /*0x40*/)
    where T : Component
    where TStop1 : Component
    where TStop2 : Component
    where TStop3 : Component
  {
    out_included = root.GetComponentsInChildrenUntil<T, TStop1, TStop2, TStop3>(includeInactive, stopAtRoot, capacity);
    out_excluded = new HashSet<T>((IEnumerable<T>) root.GetComponentsInChildren<T>(includeInactive));
    out_excluded.ExceptWith((IEnumerable<T>) new HashSet<T>((IEnumerable<T>) out_included));
  }

  private static void _GetComponentsInChildrenUntil_OutExclusions_GetRecursive<T, TStop1, TStop2, TStop3>(
    Transform currentTransform,
    List<T> included,
    List<Component> excluded,
    bool includeInactive)
    where T : Component
    where TStop1 : Component
    where TStop2 : Component
    where TStop3 : Component
  {
    foreach (Transform currentTransform1 in currentTransform)
    {
      if (includeInactive || currentTransform1.gameObject.activeSelf)
      {
        Component stopComponent;
        if (GTExt._HasAnyComponents<TStop1, TStop2, TStop3>((Component) currentTransform1, out stopComponent))
        {
          excluded.Add(stopComponent);
        }
        else
        {
          T component = currentTransform1.GetComponent<T>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            included.Add(component);
          GTExt._GetComponentsInChildrenUntil_OutExclusions_GetRecursive<T, TStop1, TStop2, TStop3>(currentTransform1, included, excluded, includeInactive);
        }
      }
    }
  }

  private static bool _HasAnyComponents<TStop1, TStop2, TStop3>(
    Component component,
    out Component stopComponent)
    where TStop1 : Component
    where TStop2 : Component
    where TStop3 : Component
  {
    stopComponent = (Component) component.GetComponent<TStop1>();
    if ((UnityEngine.Object) stopComponent != (UnityEngine.Object) null)
      return true;
    stopComponent = (Component) component.GetComponent<TStop2>();
    if ((UnityEngine.Object) stopComponent != (UnityEngine.Object) null)
      return true;
    stopComponent = (Component) component.GetComponent<TStop3>();
    return (UnityEngine.Object) stopComponent != (UnityEngine.Object) null;
  }

  public static T GetComponentWithRegex<T>(this Component root, string regexString) where T : Component
  {
    T[] componentsInChildren = root.GetComponentsInChildren<T>();
    Regex regex = new Regex(regexString);
    foreach (T componentWithRegex in componentsInChildren)
    {
      if (regex.IsMatch(componentWithRegex.name))
        return componentWithRegex;
    }
    return default (T);
  }

  private static List<T> GetComponentsWithRegex_Internal<T>(
    IEnumerable<T> allComponents,
    string regexString,
    bool includeInactive,
    int capacity = 64 /*0x40*/)
    where T : Component
  {
    List<T> foundComponents = new List<T>(capacity);
    Regex regex = new Regex(regexString);
    GTExt.GetComponentsWithRegex_Internal<T>(allComponents, regex, ref foundComponents);
    return foundComponents;
  }

  private static void GetComponentsWithRegex_Internal<T>(
    IEnumerable<T> allComponents,
    Regex regex,
    ref List<T> foundComponents)
    where T : Component
  {
    foreach (T allComponent in allComponents)
    {
      string name = allComponent.name;
      if (regex.IsMatch(name))
        foundComponents.Add(allComponent);
    }
  }

  public static List<T> GetComponentsWithRegex<T>(
    this Scene scene,
    string regexString,
    bool includeInactive,
    int capacity)
    where T : Component
  {
    return GTExt.GetComponentsWithRegex_Internal<T>((IEnumerable<T>) scene.GetComponentsInHierarchy<T>(includeInactive, capacity), regexString, includeInactive, capacity);
  }

  public static List<T> GetComponentsWithRegex<T>(
    this Component root,
    string regexString,
    bool includeInactive,
    int capacity)
    where T : Component
  {
    return GTExt.GetComponentsWithRegex_Internal<T>((IEnumerable<T>) root.GetComponentsInChildren<T>(includeInactive), regexString, includeInactive, capacity);
  }

  public static List<GameObject> GetGameObjectsWithRegex(
    this Scene scene,
    string regexString,
    bool includeInactive = true,
    int capacity = 64 /*0x40*/)
  {
    List<Transform> componentsWithRegex = scene.GetComponentsWithRegex<Transform>(regexString, includeInactive, capacity);
    List<GameObject> objectsWithRegex = new List<GameObject>(componentsWithRegex.Count);
    foreach (Transform transform in componentsWithRegex)
      objectsWithRegex.Add(transform.gameObject);
    return objectsWithRegex;
  }

  public static void GetComponentsWithRegex_Internal<T>(
    this List<T> allComponents,
    Regex[] regexes,
    int maxCount,
    ref List<T> foundComponents)
    where T : Component
  {
    if (maxCount == 0)
      return;
    int num = 0;
    foreach (T allComponent in allComponents)
    {
      foreach (Regex regex in regexes)
      {
        if (regex.IsMatch(allComponent.name))
        {
          foundComponents.Add(allComponent);
          ++num;
          if (maxCount > 0 && num >= maxCount)
            return;
        }
      }
    }
  }

  public static List<T> GetComponentsWithRegex<T>(
    this Scene scene,
    string[] regexStrings,
    bool includeInactive = true,
    int maxCount = -1,
    int capacity = 64 /*0x40*/)
    where T : Component
  {
    List<T> componentsInHierarchy = scene.GetComponentsInHierarchy<T>(includeInactive, capacity);
    List<T> foundComponents = new List<T>(componentsInHierarchy.Count);
    Regex[] regexes = new Regex[regexStrings.Length];
    for (int index = 0; index < regexStrings.Length; ++index)
      regexes[index] = new Regex(regexStrings[index]);
    componentsInHierarchy.GetComponentsWithRegex_Internal<T>(regexes, maxCount, ref foundComponents);
    return foundComponents;
  }

  public static List<T> GetComponentsWithRegex<T>(
    this Scene scene,
    string[] regexStrings,
    string[] excludeRegexStrings,
    bool includeInactive = true,
    int maxCount = -1)
    where T : Component
  {
    List<T> componentsInHierarchy = scene.GetComponentsInHierarchy<T>(includeInactive);
    List<T> componentsWithRegex = new List<T>(componentsInHierarchy.Count);
    if (maxCount == 0)
      return componentsWithRegex;
    int num = 0;
    foreach (T obj in componentsInHierarchy)
    {
      bool flag = false;
      foreach (string regexString in regexStrings)
      {
        if (!flag && Regex.IsMatch(obj.name, regexString))
        {
          foreach (string excludeRegexString in excludeRegexStrings)
          {
            if (!flag)
              flag = Regex.IsMatch(obj.name, excludeRegexString);
          }
          if (!flag)
          {
            componentsWithRegex.Add(obj);
            ++num;
            if (maxCount > 0 && num >= maxCount)
              return componentsWithRegex;
          }
        }
      }
    }
    return componentsWithRegex;
  }

  public static List<GameObject> GetGameObjectsWithRegex(
    this Scene scene,
    string[] regexStrings,
    bool includeInactive = true,
    int maxCount = -1)
  {
    List<Transform> componentsWithRegex = scene.GetComponentsWithRegex<Transform>(regexStrings, includeInactive, maxCount);
    List<GameObject> objectsWithRegex = new List<GameObject>(componentsWithRegex.Count);
    foreach (Transform transform in componentsWithRegex)
      objectsWithRegex.Add(transform.gameObject);
    return objectsWithRegex;
  }

  public static List<GameObject> GetGameObjectsWithRegex(
    this Scene scene,
    string[] regexStrings,
    string[] excludeRegexStrings,
    bool includeInactive = true,
    int maxCount = -1)
  {
    List<Transform> componentsWithRegex = scene.GetComponentsWithRegex<Transform>(regexStrings, excludeRegexStrings, includeInactive, maxCount);
    List<GameObject> objectsWithRegex = new List<GameObject>(componentsWithRegex.Count);
    foreach (Transform transform in componentsWithRegex)
      objectsWithRegex.Add(transform.gameObject);
    return objectsWithRegex;
  }

  public static List<T> GetComponentsByName<T>(
    this Transform xform,
    string name,
    bool includeInactive = true)
    where T : Component
  {
    T[] componentsInChildren = xform.GetComponentsInChildren<T>(includeInactive);
    List<T> componentsByName = new List<T>(componentsInChildren.Length);
    foreach (T obj in componentsInChildren)
    {
      if (obj.name == name)
        componentsByName.Add(obj);
    }
    return componentsByName;
  }

  public static T GetComponentByName<T>(this Transform xform, string name, bool includeInactive = true) where T : Component
  {
    foreach (T componentsInChild in xform.GetComponentsInChildren<T>(includeInactive))
    {
      if (componentsInChild.name == name)
        return componentsInChild;
    }
    return default (T);
  }

  public static List<GameObject> GetGameObjectsInHierarchy(
    this Scene scene,
    string name,
    bool includeInactive = true)
  {
    List<GameObject> objectsInHierarchy = new List<GameObject>();
    foreach (GameObject rootGameObject in scene.GetRootGameObjects())
    {
      if (rootGameObject.name.Contains(name))
        objectsInHierarchy.Add(rootGameObject);
      foreach (Transform componentsInChild in rootGameObject.GetComponentsInChildren<Transform>(includeInactive))
      {
        if (componentsInChild.name.Contains(name))
          objectsInHierarchy.Add(componentsInChild.gameObject);
      }
    }
    return objectsInHierarchy;
  }

  public static T GetOrAddComponent<T>(this GameObject gameObject, ref T component) where T : Component
  {
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      component = gameObject.GetOrAddComponent<T>();
    return component;
  }

  public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
  {
    T component;
    if (!gameObject.TryGetComponent<T>(out component))
      component = gameObject.AddComponent<T>();
    return component;
  }

  public static void SetLossyScale(this Transform transform, Vector3 scale)
  {
    scale = transform.InverseTransformVector(scale);
    Vector3 lossyScale = transform.lossyScale;
    transform.localScale = new Vector3(scale.x / lossyScale.x, scale.y / lossyScale.y, scale.z / lossyScale.z);
  }

  public static Quaternion TransformRotation(this Transform transform, Quaternion localRotation)
  {
    return transform.rotation * localRotation;
  }

  public static Quaternion InverseTransformRotation(
    this Transform transform,
    Quaternion localRotation)
  {
    return Quaternion.Inverse(transform.rotation) * localRotation;
  }

  public static Vector3 ProjectOnPlane(
    this Vector3 point,
    Vector3 planeAnchorPosition,
    Vector3 planeNormal)
  {
    return planeAnchorPosition + Vector3.ProjectOnPlane(point - planeAnchorPosition, planeNormal);
  }

  public static void ForEachBackwards<T>(this List<T> list, Action<T> action)
  {
    for (int index = list.Count - 1; index >= 0; --index)
    {
      T obj = list[index];
      try
      {
        action(obj);
      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
      }
    }
  }

  public static void AddSortedUnique<T>(this List<T> list, T item)
  {
    int num = list.BinarySearch(item);
    if (num >= 0)
      return;
    list.Insert(~num, item);
  }

  public static void RemoveSorted<T>(this List<T> list, T item)
  {
    int index = list.BinarySearch(item);
    if (index < 0)
      return;
    list.RemoveAt(index);
  }

  public static bool ContainsSorted<T>(this List<T> list, T item) => list.BinarySearch(item) >= 0;

  public static void SafeForEachBackwards<T>(this List<T> list, Action<T> action)
  {
    for (int index = list.Count - 1; index >= 0; --index)
    {
      T obj = list[index];
      try
      {
        action(obj);
      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
      }
    }
  }

  public static T[] Filled<T>(this T[] array, T value)
  {
    for (int index = 0; index < array.Length; ++index)
      array[index] = value;
    return array;
  }

  public static bool CompareAs255Unclamped(this Color a, Color b)
  {
    int num1 = (int) ((double) a.r * (double) byte.MaxValue);
    int num2 = (int) ((double) a.g * (double) byte.MaxValue);
    int num3 = (int) ((double) a.b * (double) byte.MaxValue);
    int num4 = (int) ((double) a.a * (double) byte.MaxValue);
    int num5 = (int) ((double) b.r * (double) byte.MaxValue);
    int num6 = (int) ((double) b.g * (double) byte.MaxValue);
    int num7 = (int) ((double) b.b * (double) byte.MaxValue);
    int num8 = (int) ((double) b.a * (double) byte.MaxValue);
    int num9 = num5;
    return num1 == num9 && num2 == num6 && num3 == num7 && num4 == num8;
  }

  public static Quaternion QuaternionFromToVec(Vector3 toVector, Vector3 fromVector)
  {
    Vector3 message1 = Vector3.Cross(fromVector, toVector);
    Debug.Log((object) message1);
    Debug.Log((object) message1.magnitude);
    Debug.Log((object) (float) ((double) Vector3.Dot(fromVector, toVector) + 1.0));
    Quaternion message2 = new Quaternion(message1.x, message1.y, message1.z, 1f + Vector3.Dot(toVector, fromVector));
    Debug.Log((object) message2);
    Debug.Log((object) message2.eulerAngles);
    Debug.Log((object) message2.normalized);
    return message2.normalized;
  }

  public static Vector3 Position(this Matrix4x4 matrix)
  {
    double m03 = (double) matrix.m03;
    float m13 = matrix.m13;
    float m23 = matrix.m23;
    double y = (double) m13;
    double z = (double) m23;
    return new Vector3((float) m03, (float) y, (float) z);
  }

  public static Vector3 Scale(this Matrix4x4 m)
  {
    Vector3 vector3;
    ref Vector3 local = ref vector3;
    Vector4 column = m.GetColumn(0);
    double magnitude1 = (double) column.magnitude;
    column = m.GetColumn(1);
    double magnitude2 = (double) column.magnitude;
    column = m.GetColumn(2);
    double magnitude3 = (double) column.magnitude;
    local = new Vector3((float) magnitude1, (float) magnitude2, (float) magnitude3);
    if (Vector3.Cross((Vector3) m.GetColumn(0), (Vector3) m.GetColumn(1)).normalized != (Vector3) m.GetColumn(2).normalized)
      vector3.x *= -1f;
    return vector3;
  }

  public static void SetLocalRelativeToParentMatrixWithParityAxis(
    in this Matrix4x4 matrix,
    GTExt.ParityOptions parity = GTExt.ParityOptions.XFlip)
  {
  }

  public static void MultiplyInPlaceWith(ref this Vector3 a, in Vector3 b)
  {
    a.x *= b.x;
    a.y *= b.y;
    a.z *= b.z;
  }

  public static void DecomposeWithXFlip(
    in this Matrix4x4 matrix,
    out Vector3 transformation,
    out Quaternion rotation,
    out Vector3 scale)
  {
    Matrix4x4 matrix1 = matrix;
    bool flag = matrix1.ValidTRS();
    transformation = matrix1.Position();
    rotation = flag ? Quaternion.LookRotation((Vector3) matrix1.GetColumnNoCopy(2), (Vector3) matrix1.GetColumnNoCopy(1)) : Quaternion.identity;
    scale = flag ? matrix.lossyScale : Vector3.zero;
  }

  public static void SetLocalMatrixRelativeToParentWithXParity(
    this Transform transform,
    in Matrix4x4 matrix4X4)
  {
    Vector3 transformation;
    Quaternion rotation;
    Vector3 scale;
    matrix4X4.DecomposeWithXFlip(out transformation, out rotation, out scale);
    transform.localPosition = transformation;
    transform.localRotation = rotation;
    transform.localScale = scale;
  }

  public static Matrix4x4 Matrix4x4Scale(in Vector3 vector)
  {
    Matrix4x4 matrix4x4;
    matrix4x4.m00 = vector.x;
    matrix4x4.m01 = 0.0f;
    matrix4x4.m02 = 0.0f;
    matrix4x4.m03 = 0.0f;
    matrix4x4.m10 = 0.0f;
    matrix4x4.m11 = vector.y;
    matrix4x4.m12 = 0.0f;
    matrix4x4.m13 = 0.0f;
    matrix4x4.m20 = 0.0f;
    matrix4x4.m21 = 0.0f;
    matrix4x4.m22 = vector.z;
    matrix4x4.m23 = 0.0f;
    matrix4x4.m30 = 0.0f;
    matrix4x4.m31 = 0.0f;
    matrix4x4.m32 = 0.0f;
    matrix4x4.m33 = 1f;
    return matrix4x4;
  }

  public static Vector4 GetColumnNoCopy(in this Matrix4x4 matrix, in int index)
  {
    switch (index)
    {
      case 0:
        return new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30);
      case 1:
        return new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31);
      case 2:
        return new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32);
      case 3:
        return new Vector4(matrix.m03, matrix.m13, matrix.m23, matrix.m33);
      default:
        throw new IndexOutOfRangeException("Invalid column index!");
    }
  }

  public static Quaternion RotationWithScaleContext(in this Matrix4x4 m, in Vector3 scale)
  {
    Matrix4x4 matrix = m * GTExt.Matrix4x4Scale(in scale);
    return Quaternion.LookRotation((Vector3) matrix.GetColumnNoCopy(2), (Vector3) matrix.GetColumnNoCopy(1));
  }

  public static Quaternion Rotation(in this Matrix4x4 m)
  {
    return Quaternion.LookRotation((Vector3) m.GetColumnNoCopy(2), (Vector3) m.GetColumnNoCopy(1));
  }

  public static Vector3 x0y(this Vector2 v) => new Vector3(v.x, 0.0f, v.y);

  public static Vector3 x0y(this Vector3 v) => new Vector3(v.x, 0.0f, v.y);

  public static Vector3 xy0(this Vector2 v) => new Vector3(v.x, v.y, 0.0f);

  public static Vector3 xy0(this Vector3 v) => new Vector3(v.x, v.y, 0.0f);

  public static Vector3 xz0(this Vector3 v) => new Vector3(v.x, v.z, 0.0f);

  public static Vector3 x0z(this Vector3 v) => new Vector3(v.x, 0.0f, v.z);

  public static Matrix4x4 LocalMatrixRelativeToParentNoScale(this Transform transform)
  {
    return Matrix4x4.TRS(transform.localPosition, transform.localRotation, Vector3.one);
  }

  public static Matrix4x4 LocalMatrixRelativeToParentWithScale(this Transform transform)
  {
    return (UnityEngine.Object) transform.parent == (UnityEngine.Object) null ? transform.localToWorldMatrix : transform.parent.worldToLocalMatrix * transform.localToWorldMatrix;
  }

  public static void SetLocalMatrixRelativeToParent(this Transform transform, Matrix4x4 matrix)
  {
    transform.localPosition = matrix.Position();
    transform.localRotation = matrix.Rotation();
    transform.localScale = matrix.Scale();
  }

  public static void SetLocalMatrixRelativeToParentNoScale(
    this Transform transform,
    Matrix4x4 matrix)
  {
    transform.localPosition = matrix.Position();
    transform.localRotation = matrix.Rotation();
  }

  public static void SetLocalToWorldMatrixNoScale(this Transform transform, Matrix4x4 matrix)
  {
    transform.position = matrix.Position();
    transform.rotation = matrix.Rotation();
  }

  public static Matrix4x4 localToWorldNoScale(this Transform transform)
  {
    return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
  }

  public static void SetLocalToWorldMatrixWithScale(this Transform transform, Matrix4x4 matrix)
  {
    transform.position = matrix.Position();
    transform.rotation = matrix.rotation;
    transform.SetLossyScale(matrix.lossyScale);
  }

  public static Matrix4x4 Matrix4X4LerpNoScale(Matrix4x4 a, Matrix4x4 b, float t)
  {
    return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.rotation, b.rotation, t), b.lossyScale);
  }

  public static Matrix4x4 LerpTo(this Matrix4x4 a, Matrix4x4 b, float t)
  {
    return GTExt.Matrix4X4LerpNoScale(a, b, t);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsNaN(in this Vector3 v)
  {
    return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsNan(in this Quaternion q)
  {
    return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsInfinity(in this Vector3 v)
  {
    return float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsInfinity(in this Quaternion q)
  {
    return float.IsInfinity(q.x) || float.IsInfinity(q.y) || float.IsInfinity(q.z) || float.IsInfinity(q.w);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool ValuesInRange(in this Vector3 v, in float maxVal)
  {
    return (double) Mathf.Abs(v.x) < (double) maxVal && (double) Mathf.Abs(v.y) < (double) maxVal && (double) Mathf.Abs(v.z) < (double) maxVal;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsValid(in this Vector3 v, in float maxVal = 10000f)
  {
    return !v.IsNaN() && !v.IsInfinity() && v.ValuesInRange(in maxVal);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector3 GetValidWithFallback(in this Vector3 v, in Vector3 safeVal)
  {
    return !v.IsValid() ? safeVal : v;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void SetValueSafe(ref this Vector3 v, in Vector3 newVal)
  {
    if (!newVal.IsValid())
      return;
    v = newVal;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool IsValid(in this Quaternion q) => !q.IsNan() && !q.IsInfinity();

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Quaternion GetValidWithFallback(in this Quaternion q, in Quaternion safeVal)
  {
    return !q.IsValid() ? safeVal : q;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void SetValueSafe(ref this Quaternion q, in Quaternion newVal)
  {
    if (!newVal.IsValid())
      return;
    q = newVal;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector2 ClampMagnitudeSafe(this Vector2 v2, float magnitude)
  {
    if (!float.IsFinite(v2.x))
      v2.x = 0.0f;
    if (!float.IsFinite(v2.y))
      v2.y = 0.0f;
    if (!float.IsFinite(magnitude))
      magnitude = 0.0f;
    return Vector2.ClampMagnitude(v2, magnitude);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void ClampThisMagnitudeSafe(ref this Vector2 v2, float magnitude)
  {
    if (!float.IsFinite(v2.x))
      v2.x = 0.0f;
    if (!float.IsFinite(v2.y))
      v2.y = 0.0f;
    if (!float.IsFinite(magnitude))
      magnitude = 0.0f;
    v2 = Vector2.ClampMagnitude(v2, magnitude);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Vector3 ClampMagnitudeSafe(this Vector3 v3, float magnitude)
  {
    if (!float.IsFinite(v3.x))
      v3.x = 0.0f;
    if (!float.IsFinite(v3.y))
      v3.y = 0.0f;
    if (!float.IsFinite(v3.z))
      v3.z = 0.0f;
    if (!float.IsFinite(magnitude))
      magnitude = 0.0f;
    return Vector3.ClampMagnitude(v3, magnitude);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void ClampThisMagnitudeSafe(ref this Vector3 v3, float magnitude)
  {
    if (!float.IsFinite(v3.x))
      v3.x = 0.0f;
    if (!float.IsFinite(v3.y))
      v3.y = 0.0f;
    if (!float.IsFinite(v3.z))
      v3.z = 0.0f;
    if (!float.IsFinite(magnitude))
      magnitude = 0.0f;
    v3 = Vector3.ClampMagnitude(v3, magnitude);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float MinSafe(this float value, float min)
  {
    if (!float.IsFinite(value))
      value = 0.0f;
    if (!float.IsFinite(min))
      min = 0.0f;
    return (double) value >= (double) min ? min : value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void ThisMinSafe(ref this float value, float min)
  {
    if (!float.IsFinite(value))
      value = 0.0f;
    if (!float.IsFinite(min))
      min = 0.0f;
    value = (double) value < (double) min ? value : min;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static double MinSafe(this double value, float min)
  {
    if (!double.IsFinite(value))
      value = 0.0;
    if (!double.IsFinite((double) min))
      min = 0.0f;
    return value >= (double) min ? (double) min : value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void ThisMinSafe(ref this double value, float min)
  {
    if (!double.IsFinite(value))
      value = 0.0;
    if (!double.IsFinite((double) min))
      min = 0.0f;
    value = value < (double) min ? value : (double) min;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float MaxSafe(this float value, float max)
  {
    if (!float.IsFinite(value))
      value = 0.0f;
    if (!float.IsFinite(max))
      max = 0.0f;
    return (double) value <= (double) max ? max : value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void ThisMaxSafe(ref this float value, float max)
  {
    if (!float.IsFinite(value))
      value = 0.0f;
    if (!float.IsFinite(max))
      max = 0.0f;
    value = (double) value > (double) max ? value : max;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static double MaxSafe(this double value, float max)
  {
    if (!double.IsFinite(value))
      value = 0.0;
    if (!double.IsFinite((double) max))
      max = 0.0f;
    return value <= (double) max ? (double) max : value;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void ThisMaxSafe(ref this double value, float max)
  {
    if (!double.IsFinite(value))
      value = 0.0;
    if (!double.IsFinite((double) max))
      max = 0.0f;
    value = value > (double) max ? value : (double) max;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float ClampSafe(this float value, float min, float max)
  {
    if (!float.IsFinite(value))
      value = 0.0f;
    if (!float.IsFinite(min))
      min = 0.0f;
    if (!float.IsFinite(max))
      max = 0.0f;
    if ((double) value > (double) max)
      return max;
    return (double) value >= (double) min ? value : min;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static double ClampSafe(this double value, double min, double max)
  {
    if (!double.IsFinite(value))
      value = 0.0;
    if (!double.IsFinite(min))
      min = 0.0;
    if (!double.IsFinite(max))
      max = 0.0;
    if (value > max)
      return max;
    return value >= min ? value : min;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static float GetFinite(this float value) => !float.IsFinite(value) ? 0.0f : value;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static double GetFinite(this double value) => !double.IsFinite(value) ? 0.0 : value;

  public static Matrix4x4 Matrix4X4LerpHandleNegativeScale(Matrix4x4 a, Matrix4x4 b, float t)
  {
    return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.Rotation(), b.Rotation(), t), b.lossyScale);
  }

  public static Matrix4x4 LerpTo_HandleNegativeScale(this Matrix4x4 a, Matrix4x4 b, float t)
  {
    return GTExt.Matrix4X4LerpHandleNegativeScale(a, b, t);
  }

  public static Vector3 LerpToUnclamped(in this Vector3 a, in Vector3 b, float t)
  {
    return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
  }

  public static string ToLongString(this Vector3 self) => $"[{self.x}, {self.y}, {self.z}]";

  public static int GetRandomIndex<T>(this IReadOnlyList<T> self) => UnityEngine.Random.Range(0, self.Count);

  public static T GetRandomItem<T>(this IReadOnlyList<T> self) => self[self.GetRandomIndex<T>()];

  public static Vector2 xx(this float v) => new Vector2(v, v);

  public static Vector2 xx(this Vector2 v) => new Vector2(v.x, v.x);

  public static Vector2 xy(this Vector2 v) => new Vector2(v.x, v.y);

  public static Vector2 yy(this Vector2 v) => new Vector2(v.y, v.y);

  public static Vector2 xx(this Vector3 v) => new Vector2(v.x, v.x);

  public static Vector2 xy(this Vector3 v) => new Vector2(v.x, v.y);

  public static Vector2 xz(this Vector3 v) => new Vector2(v.x, v.z);

  public static Vector2 yy(this Vector3 v) => new Vector2(v.y, v.y);

  public static Vector2 yz(this Vector3 v) => new Vector2(v.y, v.z);

  public static Vector2 zz(this Vector3 v) => new Vector2(v.z, v.z);

  public static Vector2 xx(this Vector4 v) => new Vector2(v.x, v.x);

  public static Vector2 xy(this Vector4 v) => new Vector2(v.x, v.y);

  public static Vector2 xz(this Vector4 v) => new Vector2(v.x, v.z);

  public static Vector2 xw(this Vector4 v) => new Vector2(v.x, v.w);

  public static Vector2 yy(this Vector4 v) => new Vector2(v.y, v.y);

  public static Vector2 yz(this Vector4 v) => new Vector2(v.y, v.z);

  public static Vector2 yw(this Vector4 v) => new Vector2(v.y, v.w);

  public static Vector2 zz(this Vector4 v) => new Vector2(v.z, v.z);

  public static Vector2 zw(this Vector4 v) => new Vector2(v.z, v.w);

  public static Vector2 ww(this Vector4 v) => new Vector2(v.w, v.w);

  public static Vector3 xxx(this float v) => new Vector3(v, v, v);

  public static Vector3 xxx(this Vector2 v) => new Vector3(v.x, v.x, v.x);

  public static Vector3 xxy(this Vector2 v) => new Vector3(v.x, v.x, v.y);

  public static Vector3 xyy(this Vector2 v) => new Vector3(v.x, v.y, v.y);

  public static Vector3 yyy(this Vector2 v) => new Vector3(v.y, v.y, v.y);

  public static Vector3 xxx(this Vector3 v) => new Vector3(v.x, v.x, v.x);

  public static Vector3 xxy(this Vector3 v) => new Vector3(v.x, v.x, v.y);

  public static Vector3 xxz(this Vector3 v) => new Vector3(v.x, v.x, v.z);

  public static Vector3 xyy(this Vector3 v) => new Vector3(v.x, v.y, v.y);

  public static Vector3 xyz(this Vector3 v) => new Vector3(v.x, v.y, v.z);

  public static Vector3 xzz(this Vector3 v) => new Vector3(v.x, v.z, v.z);

  public static Vector3 yyy(this Vector3 v) => new Vector3(v.y, v.y, v.y);

  public static Vector3 yyz(this Vector3 v) => new Vector3(v.y, v.y, v.z);

  public static Vector3 yzz(this Vector3 v) => new Vector3(v.y, v.z, v.z);

  public static Vector3 zzz(this Vector3 v) => new Vector3(v.z, v.z, v.z);

  public static Vector3 xxx(this Vector4 v) => new Vector3(v.x, v.x, v.x);

  public static Vector3 xxy(this Vector4 v) => new Vector3(v.x, v.x, v.y);

  public static Vector3 xxz(this Vector4 v) => new Vector3(v.x, v.x, v.z);

  public static Vector3 xxw(this Vector4 v) => new Vector3(v.x, v.x, v.w);

  public static Vector3 xyy(this Vector4 v) => new Vector3(v.x, v.y, v.y);

  public static Vector3 xyz(this Vector4 v) => new Vector3(v.x, v.y, v.z);

  public static Vector3 xyw(this Vector4 v) => new Vector3(v.x, v.y, v.w);

  public static Vector3 xzz(this Vector4 v) => new Vector3(v.x, v.z, v.z);

  public static Vector3 xzw(this Vector4 v) => new Vector3(v.x, v.z, v.w);

  public static Vector3 xww(this Vector4 v) => new Vector3(v.x, v.w, v.w);

  public static Vector3 yyy(this Vector4 v) => new Vector3(v.y, v.y, v.y);

  public static Vector3 yyz(this Vector4 v) => new Vector3(v.y, v.y, v.z);

  public static Vector3 yyw(this Vector4 v) => new Vector3(v.y, v.y, v.w);

  public static Vector3 yzz(this Vector4 v) => new Vector3(v.y, v.z, v.z);

  public static Vector3 yzw(this Vector4 v) => new Vector3(v.y, v.z, v.w);

  public static Vector3 yww(this Vector4 v) => new Vector3(v.y, v.w, v.w);

  public static Vector3 zzz(this Vector4 v) => new Vector3(v.z, v.z, v.z);

  public static Vector3 zzw(this Vector4 v) => new Vector3(v.z, v.z, v.w);

  public static Vector3 zww(this Vector4 v) => new Vector3(v.z, v.w, v.w);

  public static Vector3 www(this Vector4 v) => new Vector3(v.w, v.w, v.w);

  public static Vector4 xxxx(this float v) => new Vector4(v, v, v, v);

  public static Vector4 xxxx(this Vector2 v) => new Vector4(v.x, v.x, v.x, v.x);

  public static Vector4 xxxy(this Vector2 v) => new Vector4(v.x, v.x, v.x, v.y);

  public static Vector4 xxyy(this Vector2 v) => new Vector4(v.x, v.x, v.y, v.y);

  public static Vector4 xyyy(this Vector2 v) => new Vector4(v.x, v.y, v.y, v.y);

  public static Vector4 yyyy(this Vector2 v) => new Vector4(v.y, v.y, v.y, v.y);

  public static Vector4 xxxx(this Vector3 v) => new Vector4(v.x, v.x, v.x, v.x);

  public static Vector4 xxxy(this Vector3 v) => new Vector4(v.x, v.x, v.x, v.y);

  public static Vector4 xxxz(this Vector3 v) => new Vector4(v.x, v.x, v.x, v.z);

  public static Vector4 xxyy(this Vector3 v) => new Vector4(v.x, v.x, v.y, v.y);

  public static Vector4 xxyz(this Vector3 v) => new Vector4(v.x, v.x, v.y, v.z);

  public static Vector4 xxzz(this Vector3 v) => new Vector4(v.x, v.x, v.z, v.z);

  public static Vector4 xyyy(this Vector3 v) => new Vector4(v.x, v.y, v.y, v.y);

  public static Vector4 xyyz(this Vector3 v) => new Vector4(v.x, v.y, v.y, v.z);

  public static Vector4 xyzz(this Vector3 v) => new Vector4(v.x, v.y, v.z, v.z);

  public static Vector4 xzzz(this Vector3 v) => new Vector4(v.x, v.z, v.z, v.z);

  public static Vector4 yyyy(this Vector3 v) => new Vector4(v.y, v.y, v.y, v.y);

  public static Vector4 yyyz(this Vector3 v) => new Vector4(v.y, v.y, v.y, v.z);

  public static Vector4 yyzz(this Vector3 v) => new Vector4(v.y, v.y, v.z, v.z);

  public static Vector4 yzzz(this Vector3 v) => new Vector4(v.y, v.z, v.z, v.z);

  public static Vector4 zzzz(this Vector3 v) => new Vector4(v.z, v.z, v.z, v.z);

  public static Vector4 xxxx(this Vector4 v) => new Vector4(v.x, v.x, v.x, v.x);

  public static Vector4 xxxy(this Vector4 v) => new Vector4(v.x, v.x, v.x, v.y);

  public static Vector4 xxxz(this Vector4 v) => new Vector4(v.x, v.x, v.x, v.z);

  public static Vector4 xxxw(this Vector4 v) => new Vector4(v.x, v.x, v.x, v.w);

  public static Vector4 xxyy(this Vector4 v) => new Vector4(v.x, v.x, v.y, v.y);

  public static Vector4 xxyz(this Vector4 v) => new Vector4(v.x, v.x, v.y, v.z);

  public static Vector4 xxyw(this Vector4 v) => new Vector4(v.x, v.x, v.y, v.w);

  public static Vector4 xxzz(this Vector4 v) => new Vector4(v.x, v.x, v.z, v.z);

  public static Vector4 xxzw(this Vector4 v) => new Vector4(v.x, v.x, v.z, v.w);

  public static Vector4 xxww(this Vector4 v) => new Vector4(v.x, v.x, v.w, v.w);

  public static Vector4 xyyy(this Vector4 v) => new Vector4(v.x, v.y, v.y, v.y);

  public static Vector4 xyyz(this Vector4 v) => new Vector4(v.x, v.y, v.y, v.z);

  public static Vector4 xyyw(this Vector4 v) => new Vector4(v.x, v.y, v.y, v.w);

  public static Vector4 xyzz(this Vector4 v) => new Vector4(v.x, v.y, v.z, v.z);

  public static Vector4 xyzw(this Vector4 v) => new Vector4(v.x, v.y, v.z, v.w);

  public static Vector4 xyww(this Vector4 v) => new Vector4(v.x, v.y, v.w, v.w);

  public static Vector4 xzzz(this Vector4 v) => new Vector4(v.x, v.z, v.z, v.z);

  public static Vector4 xzzw(this Vector4 v) => new Vector4(v.x, v.z, v.z, v.w);

  public static Vector4 xzww(this Vector4 v) => new Vector4(v.x, v.z, v.w, v.w);

  public static Vector4 xwww(this Vector4 v) => new Vector4(v.x, v.w, v.w, v.w);

  public static Vector4 yyyy(this Vector4 v) => new Vector4(v.y, v.y, v.y, v.y);

  public static Vector4 yyyz(this Vector4 v) => new Vector4(v.y, v.y, v.y, v.z);

  public static Vector4 yyyw(this Vector4 v) => new Vector4(v.y, v.y, v.y, v.w);

  public static Vector4 yyzz(this Vector4 v) => new Vector4(v.y, v.y, v.z, v.z);

  public static Vector4 yyzw(this Vector4 v) => new Vector4(v.y, v.y, v.z, v.w);

  public static Vector4 yyww(this Vector4 v) => new Vector4(v.y, v.y, v.w, v.w);

  public static Vector4 yzzz(this Vector4 v) => new Vector4(v.y, v.z, v.z, v.z);

  public static Vector4 yzzw(this Vector4 v) => new Vector4(v.y, v.z, v.z, v.w);

  public static Vector4 yzww(this Vector4 v) => new Vector4(v.y, v.z, v.w, v.w);

  public static Vector4 ywww(this Vector4 v) => new Vector4(v.y, v.w, v.w, v.w);

  public static Vector4 zzzz(this Vector4 v) => new Vector4(v.z, v.z, v.z, v.z);

  public static Vector4 zzzw(this Vector4 v) => new Vector4(v.z, v.z, v.z, v.w);

  public static Vector4 zzww(this Vector4 v) => new Vector4(v.z, v.z, v.w, v.w);

  public static Vector4 zwww(this Vector4 v) => new Vector4(v.z, v.w, v.w, v.w);

  public static Vector4 wwww(this Vector4 v) => new Vector4(v.w, v.w, v.w, v.w);

  public static Vector4 WithX(this Vector4 v, float x) => new Vector4(x, v.y, v.z, v.w);

  public static Vector4 WithY(this Vector4 v, float y) => new Vector4(v.x, y, v.z, v.w);

  public static Vector4 WithZ(this Vector4 v, float z) => new Vector4(v.x, v.y, z, v.w);

  public static Vector4 WithW(this Vector4 v, float w) => new Vector4(v.x, v.y, v.z, w);

  public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);

  public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);

  public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

  public static Vector4 WithW(this Vector3 v, float w) => new Vector4(v.x, v.y, v.z, w);

  public static Vector2 WithX(this Vector2 v, float x) => new Vector2(x, v.y);

  public static Vector2 WithY(this Vector2 v, float y) => new Vector2(v.x, y);

  public static Vector3 WithZ(this Vector2 v, float z) => new Vector3(v.x, v.y, z);

  public static bool IsShorterThan(this Vector2 v, float len)
  {
    return (double) v.sqrMagnitude < (double) len * (double) len;
  }

  public static bool IsShorterThan(this Vector2 v, Vector2 v2)
  {
    return (double) v.sqrMagnitude < (double) v2.sqrMagnitude;
  }

  public static bool IsShorterThan(this Vector3 v, float len)
  {
    return (double) v.sqrMagnitude < (double) len * (double) len;
  }

  public static bool IsShorterThan(this Vector3 v, Vector3 v2)
  {
    return (double) v.sqrMagnitude < (double) v2.sqrMagnitude;
  }

  public static bool IsLongerThan(this Vector2 v, float len)
  {
    return (double) v.sqrMagnitude > (double) len * (double) len;
  }

  public static bool IsLongerThan(this Vector2 v, Vector2 v2)
  {
    return (double) v.sqrMagnitude > (double) v2.sqrMagnitude;
  }

  public static bool IsLongerThan(this Vector3 v, float len)
  {
    return (double) v.sqrMagnitude > (double) len * (double) len;
  }

  public static bool IsLongerThan(this Vector3 v, Vector3 v2)
  {
    return (double) v.sqrMagnitude > (double) v2.sqrMagnitude;
  }

  public static Vector3 Normalize(this Vector3 value, out float existingMagnitude)
  {
    existingMagnitude = Vector3.Magnitude(value);
    return (double) existingMagnitude > 9.9999997473787516E-06 ? value / existingMagnitude : Vector3.zero;
  }

  public static Vector3 GetClosestPoint(this Ray ray, Vector3 target)
  {
    float num = Vector3.Dot(target - ray.origin, ray.direction);
    return ray.origin + ray.direction * num;
  }

  public static float GetClosestDistSqr(this Ray ray, Vector3 target)
  {
    return (ray.GetClosestPoint(target) - target).sqrMagnitude;
  }

  public static float GetClosestDistance(this Ray ray, Vector3 target)
  {
    return (ray.GetClosestPoint(target) - target).magnitude;
  }

  public static Vector3 ProjectToPlane(
    this Ray ray,
    Vector3 planeOrigin,
    Vector3 planeNormalMustBeLength1)
  {
    Vector3 rhs = planeOrigin - ray.origin;
    float num1 = Vector3.Dot(planeNormalMustBeLength1, rhs);
    float num2 = Vector3.Dot(planeNormalMustBeLength1, ray.direction);
    return ray.origin + ray.direction * num1 / num2;
  }

  public static Vector3 ProjectToLine(this Ray ray, Vector3 lineStart, Vector3 lineEnd)
  {
    Vector3 normalized1 = (lineEnd - lineStart).normalized;
    Vector3 normalized2 = Vector3.Cross(Vector3.Cross(ray.direction, normalized1), normalized1).normalized;
    return ray.ProjectToPlane(lineStart, normalized2);
  }

  public static bool IsNull(this UnityEngine.Object mono) => (object) mono == null || !(bool) mono;

  public static bool IsNotNull(this UnityEngine.Object mono) => !mono.IsNull();

  public static string GetPath(this Transform transform)
  {
    string str = transform.name;
    while ((bool) (UnityEngine.Object) transform.parent)
    {
      transform = transform.parent;
      str = $"{transform.name}/{str}";
    }
    return "/" + str;
  }

  public static string GetPathQ(this Transform transform)
  {
    Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder();
    string pathQ;
    try
    {
      transform.GetPathQ(ref stringBuilder);
    }
    finally
    {
      pathQ = stringBuilder.ToString();
    }
    return pathQ;
  }

  public static void GetPathQ(this Transform transform, ref Utf16ValueStringBuilder sb)
  {
    sb.Append("\"");
    int length = sb.Length;
    do
    {
      if (sb.Length > length)
        sb.Insert(length, "/");
      sb.Insert(length, transform.name);
      transform = transform.parent;
    }
    while ((UnityEngine.Object) transform != (UnityEngine.Object) null);
    sb.Append("\"");
  }

  public static string GetPath(this Transform transform, int maxDepth)
  {
    string str = transform.name;
    for (int index = 0; (bool) (UnityEngine.Object) transform.parent && index < maxDepth; ++index)
    {
      transform = transform.parent;
      str = $"{transform.name}/{str}";
    }
    return "/" + str;
  }

  public static string GetPath(this Transform transform, Transform stopper)
  {
    string str = transform.name;
    while ((bool) (UnityEngine.Object) transform.parent && (UnityEngine.Object) transform.parent != (UnityEngine.Object) stopper)
    {
      transform = transform.parent;
      str = $"{transform.name}/{str}";
    }
    return "/" + str;
  }

  public static string GetPath(this GameObject gameObject) => gameObject.transform.GetPath();

  public static void GetPath(this GameObject gameObject, ref Utf16ValueStringBuilder sb)
  {
    gameObject.transform.GetPathQ(ref sb);
  }

  public static string GetPath(this GameObject gameObject, int limit)
  {
    return gameObject.transform.GetPath(limit);
  }

  public static string[] GetPaths(this GameObject[] gobj)
  {
    string[] paths = new string[gobj.Length];
    for (int index = 0; index < gobj.Length; ++index)
      paths[index] = gobj[index].GetPath();
    return paths;
  }

  public static string[] GetPaths(this Transform[] xform)
  {
    string[] paths = new string[xform.Length];
    for (int index = 0; index < xform.Length; ++index)
      paths[index] = xform[index].GetPath();
    return paths;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void GetRelativePath(
    string fromPath,
    string toPath,
    ref Utf16ValueStringBuilder ZStringBuilder)
  {
    if (string.IsNullOrEmpty(fromPath) || string.IsNullOrEmpty(toPath))
      return;
    int index1 = 0;
    while (index1 < fromPath.Length && fromPath[index1] == '/')
      ++index1;
    int index2 = 0;
    while (index2 < toPath.Length && toPath[index2] == '/')
      ++index2;
    int num1 = -1;
    int num2 = Mathf.Min(fromPath.Length - index1, toPath.Length - index2);
    bool flag = true;
    for (int index3 = 0; index3 < num2; ++index3)
    {
      if ((int) fromPath[index1 + index3] != (int) toPath[index2 + index3])
      {
        flag = false;
        break;
      }
      if (fromPath[index1 + index3] == '/')
        num1 = index3;
    }
    if (flag && fromPath.Length - index1 > num2)
      flag = fromPath[index1 + num2] == '/';
    else if (flag && toPath.Length - index2 > num2)
      flag = toPath[index2 + num2] == '/';
    int num3 = flag ? num2 : num1;
    int num4 = num3 < fromPath.Length - index1 ? num3 + 1 : fromPath.Length - index1;
    int num5 = num3 < toPath.Length - index2 ? num3 + 1 : toPath.Length - index2;
    if (num4 < fromPath.Length - index1)
    {
      ZStringBuilder.Append("../");
      for (int index4 = num4; index4 < fromPath.Length - index1; ++index4)
      {
        if (fromPath[index1 + index4] == '/')
          ZStringBuilder.Append("../");
      }
    }
    else
      ZStringBuilder.Append(toPath.Length - index2 - num5 > 0 ? "./" : ".");
    ZStringBuilder.Append(toPath, index2 + num5, toPath.Length - (index2 + num5));
  }

  public static string GetRelativePath(string fromPath, string toPath)
  {
    Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder();
    string relativePath;
    try
    {
      GTExt.GetRelativePath(fromPath, toPath, ref stringBuilder);
    }
    finally
    {
      relativePath = stringBuilder.ToString();
      stringBuilder.Dispose();
    }
    return relativePath;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void GetRelativePath(
    this Transform fromXform,
    Transform toXform,
    ref Utf16ValueStringBuilder ZStringBuilder)
  {
    GTExt.GetRelativePath(fromXform.GetPath(), toXform.GetPath(), ref ZStringBuilder);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static string GetRelativePath(this Transform fromXform, Transform toXform)
  {
    Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder();
    string relativePath;
    try
    {
      fromXform.GetRelativePath(toXform, ref stringBuilder);
    }
    finally
    {
      relativePath = stringBuilder.ToString();
      stringBuilder.Dispose();
    }
    return relativePath;
  }

  public static void GetPathWithSiblingIndexes(
    this Transform transform,
    ref Utf16ValueStringBuilder strBuilder)
  {
    int length = strBuilder.Length;
    for (; (UnityEngine.Object) transform != (UnityEngine.Object) null; transform = transform.parent)
    {
      strBuilder.Insert(length, transform.name);
      strBuilder.Insert(length, "|");
      strBuilder.Insert(length, transform.GetSiblingIndex().ToString("0000"));
      strBuilder.Insert(length, "/");
    }
  }

  public static string GetComponentPath(this Component component, int maxDepth = 2147483647 /*0x7FFFFFFF*/)
  {
    Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder();
    string componentPath;
    try
    {
      component.GetComponentPath<Component>(ref stringBuilder, maxDepth);
    }
    finally
    {
      componentPath = stringBuilder.ToString();
    }
    return componentPath;
  }

  public static string GetComponentPath<T>(this T component, int maxDepth = 2147483647 /*0x7FFFFFFF*/) where T : Component
  {
    Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder();
    string componentPath;
    try
    {
      component.GetComponentPath<T>(ref stringBuilder, maxDepth);
    }
    finally
    {
      componentPath = stringBuilder.ToString();
    }
    return componentPath;
  }

  public static void GetComponentPath<T>(
    this T component,
    ref Utf16ValueStringBuilder strBuilder,
    int maxDepth = 2147483647 /*0x7FFFFFFF*/)
    where T : Component
  {
    Transform transform = component.transform;
    int length = strBuilder.Length;
    if (maxDepth > 0)
      strBuilder.Append("/");
    strBuilder.Append("->/");
    System.Type type = typeof (T);
    strBuilder.Append(type.Name);
    if (maxDepth <= 0)
      return;
    int num = 0;
    for (; (UnityEngine.Object) transform != (UnityEngine.Object) null; transform = transform.parent)
    {
      strBuilder.Insert(length, transform.name);
      ++num;
      if (maxDepth <= num)
        break;
      strBuilder.Insert(length, "/");
    }
  }

  public static void GetComponentPathWithSiblingIndexes<T>(
    this T component,
    ref Utf16ValueStringBuilder strBuilder)
    where T : Component
  {
    Transform transform = component.transform;
    int length = strBuilder.Length;
    strBuilder.Append("/->/");
    System.Type type = typeof (T);
    strBuilder.Append(type.Name);
    for (; (UnityEngine.Object) transform != (UnityEngine.Object) null; transform = transform.parent)
    {
      strBuilder.Insert(length, transform.name);
      strBuilder.Insert(length, "|");
      strBuilder.Insert(length, transform.GetSiblingIndex().ToString("0000"));
      strBuilder.Insert(length, "/");
    }
  }

  public static string GetComponentPathWithSiblingIndexes<T>(this T component) where T : Component
  {
    Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder();
    string withSiblingIndexes;
    try
    {
      component.GetComponentPathWithSiblingIndexes<T>(ref stringBuilder);
    }
    finally
    {
      withSiblingIndexes = stringBuilder.ToString();
    }
    return withSiblingIndexes;
  }

  public static T GetComponentByPath<T>(this GameObject root, string path) where T : Component
  {
    string[] strArray1 = path.Split(new string[1]{ "/->/" }, StringSplitOptions.None);
    if (strArray1.Length < 2)
      return default (T);
    string[] strArray2 = strArray1[0].Split(new string[1]
    {
      "/"
    }, StringSplitOptions.RemoveEmptyEntries);
    Transform transform = root.transform;
    for (int index = 1; index < strArray2.Length; ++index)
    {
      string n = strArray2[index];
      transform = transform.Find(n);
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null)
        return default (T);
    }
    System.Type type = System.Type.GetType(strArray1[1].Split('#', StringSplitOptions.None)[0]);
    if (type == (System.Type) null)
      return default (T);
    Component component = transform.GetComponent(type);
    return (UnityEngine.Object) component == (UnityEngine.Object) null ? default (T) : component as T;
  }

  public static int GetDepth(this Transform xform)
  {
    int depth = 0;
    for (Transform parent = xform.parent; (UnityEngine.Object) parent != (UnityEngine.Object) null; parent = parent.parent)
      ++depth;
    return depth;
  }

  public static string GetPathWithSiblingIndexes(this Transform transform)
  {
    Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder();
    string withSiblingIndexes;
    try
    {
      transform.GetPathWithSiblingIndexes(ref stringBuilder);
    }
    finally
    {
      withSiblingIndexes = stringBuilder.ToString();
    }
    return withSiblingIndexes;
  }

  public static void GetPathWithSiblingIndexes(
    this GameObject gameObject,
    ref Utf16ValueStringBuilder stringBuilder)
  {
    gameObject.transform.GetPathWithSiblingIndexes(ref stringBuilder);
  }

  public static string GetPathWithSiblingIndexes(this GameObject gameObject)
  {
    return gameObject.transform.GetPathWithSiblingIndexes();
  }

  public static void SetFromMatrix(this Transform transform, Matrix4x4 matrix, bool useLocal = false)
  {
    if (useLocal)
    {
      transform.localPosition = matrix.GetPosition();
      transform.localRotation = matrix.rotation;
      transform.localScale = matrix.lossyScale;
    }
    else
    {
      transform.position = matrix.GetPosition();
      transform.rotation = matrix.rotation;
      transform.SetScaleFromMatrix(matrix);
    }
  }

  public static void SetScale(this Transform transform, Vector3 scale)
  {
    if ((bool) (UnityEngine.Object) transform.parent)
    {
      Matrix4x4 matrix4x4 = transform.parent.worldToLocalMatrix * Matrix4x4.TRS(transform.position, transform.rotation, scale);
      transform.localScale = matrix4x4.lossyScale;
    }
    else
      transform.localScale = scale;
  }

  public static void SetScaleFromMatrix(this Transform transform, Matrix4x4 matrix)
  {
    if ((bool) (UnityEngine.Object) transform.parent)
    {
      Matrix4x4 matrix4x4 = transform.parent.worldToLocalMatrix * matrix;
      transform.localScale = matrix4x4.lossyScale;
    }
    else
      transform.localScale = matrix.lossyScale;
  }

  public static void AddDictValue(Transform xForm, Dictionary<string, Transform> dict)
  {
    GTExt.caseSenseInner.Add(xForm, dict);
  }

  public static void ClearDicts()
  {
    GTExt.caseSenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();
    GTExt.caseInsenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();
  }

  public static bool TryFindByExactPath(
    [NotNull] string path,
    out Transform result,
    FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include)
  {
    if (string.IsNullOrEmpty(path))
      throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
    if (findObjectsInactive == FindObjectsInactive.Exclude)
    {
      if (path[0] != '/')
        path = "/" + path;
      GameObject gameObject = GameObject.Find(path);
      if ((bool) (UnityEngine.Object) gameObject)
      {
        result = gameObject.transform;
        return true;
      }
      result = (Transform) null;
      return false;
    }
    for (int index = 0; index < SceneManager.sceneCount; ++index)
    {
      Scene sceneAt = SceneManager.GetSceneAt(index);
      if (sceneAt.isLoaded && sceneAt.TryFindByExactPath(path, out result))
        return true;
    }
    result = (Transform) null;
    return false;
  }

  public static bool TryFindByExactPath(this Scene scene, string path, out Transform result)
  {
    string[] splitPath = !string.IsNullOrEmpty(path) ? path.Split('/', StringSplitOptions.RemoveEmptyEntries) : throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
    return scene.TryFindByExactPath((IReadOnlyList<string>) splitPath, out result);
  }

  private static bool TryFindByExactPath(
    this Scene scene,
    IReadOnlyList<string> splitPath,
    out Transform result)
  {
    foreach (GameObject rootGameObject in scene.GetRootGameObjects())
    {
      if (GTExt.TryFindByExactPath_Internal(rootGameObject.transform, splitPath, 0, out result))
        return true;
    }
    result = (Transform) null;
    return false;
  }

  public static bool TryFindByExactPath(
    this Transform rootXform,
    string path,
    out Transform result)
  {
    string[] splitPath = !string.IsNullOrEmpty(path) ? path.Split('/', StringSplitOptions.RemoveEmptyEntries) : throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
    foreach (Transform current in rootXform)
    {
      if (GTExt.TryFindByExactPath_Internal(current, (IReadOnlyList<string>) splitPath, 0, out result))
        return true;
    }
    result = (Transform) null;
    return false;
  }

  public static bool TryFindByExactPath(
    this Transform rootXform,
    IReadOnlyList<string> splitPath,
    out Transform result)
  {
    foreach (Transform current in rootXform)
    {
      if (GTExt.TryFindByExactPath_Internal(current, splitPath, 0, out result))
        return true;
    }
    result = (Transform) null;
    return false;
  }

  private static bool TryFindByExactPath_Internal(
    Transform current,
    IReadOnlyList<string> splitPath,
    int index,
    out Transform result)
  {
    if (current.name != splitPath[index])
    {
      result = (Transform) null;
      return false;
    }
    if (index == splitPath.Count - 1)
    {
      result = current;
      return true;
    }
    foreach (Transform current1 in current)
    {
      if (GTExt.TryFindByExactPath_Internal(current1, splitPath, index + 1, out result))
        return true;
    }
    result = (Transform) null;
    return false;
  }

  public static bool TryFindByPath(string globPath, out Transform result, bool caseSensitive = false)
  {
    return GTExt._TryFindByPath((Transform) null, (IReadOnlyList<string>) GTExt._GlobPathToPathPartsRegex(globPath), -1, out result, caseSensitive, true, globPath);
  }

  public static bool TryFindByPath(
    this Scene scene,
    string globPath,
    out Transform result,
    bool caseSensitive = false)
  {
    string[] pathPartsRegex = !string.IsNullOrEmpty(globPath) ? GTExt._GlobPathToPathPartsRegex(globPath) : throw new Exception("TryFindByPath: Provided path cannot be null or empty.");
    return scene.TryFindByPath((IReadOnlyList<string>) pathPartsRegex, out result, globPath, caseSensitive);
  }

  private static bool TryFindByPath(
    this Scene scene,
    IReadOnlyList<string> pathPartsRegex,
    out Transform result,
    string globPath,
    bool caseSensitive = false)
  {
    foreach (GameObject rootGameObject in scene.GetRootGameObjects())
    {
      if (GTExt._TryFindByPath(rootGameObject.transform, pathPartsRegex, 0, out result, caseSensitive, false, globPath))
        return true;
    }
    result = (Transform) null;
    return false;
  }

  public static bool TryFindByPath(
    this Transform rootXform,
    string globPath,
    out Transform result,
    bool caseSensitive = false)
  {
    if (string.IsNullOrEmpty(globPath))
      throw new Exception("TryFindByPath: Provided path cannot be null or empty.");
    switch (globPath[0])
    {
      case '\t':
      case '\n':
      case ' ':
label_4:
        throw new Exception($"TryFindByPath: Provided globPath cannot end or start with whitespace.\nProvided globPath=\"{globPath}\"");
      default:
        string str = globPath;
        switch (str[str.Length - 1])
        {
          case '\t':
          case '\n':
          case ' ':
            goto label_4;
          default:
            string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
            return GTExt._TryFindByPath(rootXform, (IReadOnlyList<string>) pathPartsRegex, -1, out result, caseSensitive, false, globPath);
        }
    }
  }

  public static List<string> ShowAllStringsUsed() => GTExt.allStringsUsed.Keys.ToList<string>();

  private static bool _TryFindByPath(
    Transform current,
    IReadOnlyList<string> pathPartsRegex,
    int index,
    out Transform result,
    bool caseSensitive,
    bool isAtSceneLevel,
    string joinedPath)
  {
    if (joinedPath != null && !GTExt.allStringsUsed.ContainsKey(joinedPath))
      GTExt.allStringsUsed[joinedPath] = joinedPath;
    if (caseSensitive)
    {
      if (GTExt.caseSenseInner.ContainsKey(current))
      {
        if (GTExt.caseSenseInner[current].ContainsKey(joinedPath))
        {
          result = GTExt.caseSenseInner[current][joinedPath];
          return true;
        }
      }
      else
        GTExt.caseSenseInner[current] = new Dictionary<string, Transform>();
    }
    else if (GTExt.caseInsenseInner.ContainsKey(current))
    {
      if (GTExt.caseInsenseInner[current].ContainsKey(joinedPath))
      {
        result = GTExt.caseInsenseInner[current][joinedPath];
        return true;
      }
    }
    else
      GTExt.caseInsenseInner[current] = new Dictionary<string, Transform>();
    if (isAtSceneLevel)
    {
      index = index == -1 ? 0 : index;
      string str = pathPartsRegex[index];
      if (str == ".." || str == "..**" || str == "**..")
      {
        result = (Transform) null;
        return false;
      }
      for (int index1 = 0; index1 < SceneManager.sceneCount; ++index1)
      {
        Scene sceneAt = SceneManager.GetSceneAt(index1);
        if (sceneAt.isLoaded)
        {
          foreach (GameObject rootGameObject in sceneAt.GetRootGameObjects())
          {
            if (GTExt._TryFindByPath(rootGameObject.transform, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
            {
              if (caseSensitive)
                GTExt.caseSenseInner[current][joinedPath] = result;
              else
                GTExt.caseInsenseInner[current][joinedPath] = result;
              return true;
            }
          }
        }
      }
    }
    if (index == -1)
    {
      if (pathPartsRegex.Count == 0)
      {
        result = (Transform) null;
        return false;
      }
      string str = pathPartsRegex[0];
      if (str == "." || str == ".." || str == "..**" || str == "**..")
      {
        int num = GTExt._TryFindByPath(current, pathPartsRegex, 0, out result, caseSensitive, false, joinedPath) ? 1 : 0;
        if (caseSensitive)
        {
          GTExt.caseSenseInner[current][joinedPath] = result;
          return num != 0;
        }
        GTExt.caseInsenseInner[current][joinedPath] = result;
        return num != 0;
      }
      foreach (Transform current1 in current)
      {
        if (GTExt._TryFindByPath(current1, pathPartsRegex, 0, out result, caseSensitive, false, joinedPath))
        {
          if (caseSensitive)
            GTExt.caseSenseInner[current][joinedPath] = result;
          else
            GTExt.caseInsenseInner[current][joinedPath] = result;
          return true;
        }
      }
      result = (Transform) null;
      if (caseSensitive)
        GTExt.caseSenseInner[current][joinedPath] = result;
      else
        GTExt.caseInsenseInner[current][joinedPath] = result;
      return false;
    }
    switch (pathPartsRegex[index])
    {
      case ".":
        for (; pathPartsRegex[index] == "."; ++index)
        {
          if (index == pathPartsRegex.Count - 1)
          {
            result = current;
            return true;
          }
        }
        if (GTExt._TryFindByPath(current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
        {
          if (caseSensitive)
            GTExt.caseSenseInner[current][joinedPath] = result;
          else
            GTExt.caseInsenseInner[current][joinedPath] = result;
          return true;
        }
        IEnumerator enumerator1 = current.GetEnumerator();
        try
        {
          while (enumerator1.MoveNext())
          {
            if (GTExt._TryFindByPath((Transform) enumerator1.Current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
            {
              if (caseSensitive)
                GTExt.caseSenseInner[current][joinedPath] = result;
              else
                GTExt.caseInsenseInner[current][joinedPath] = result;
              return true;
            }
          }
          break;
        }
        finally
        {
          if (enumerator1 is IDisposable disposable)
            disposable.Dispose();
        }
      case "..":
        Transform current2 = current;
        int index2;
        for (index2 = index; pathPartsRegex[index2] == ".."; ++index2)
        {
          if (index2 + 1 >= pathPartsRegex.Count)
          {
            result = current2.parent;
            return result != null;
          }
          if (current2.parent == null)
          {
            int num = GTExt._TryFindByPath(current2, pathPartsRegex, index2 + 1, out result, caseSensitive, true, joinedPath) ? 1 : 0;
            if (caseSensitive)
            {
              GTExt.caseSenseInner[current][joinedPath] = result;
              return num != 0;
            }
            GTExt.caseInsenseInner[current][joinedPath] = result;
            return num != 0;
          }
          current2 = current2.parent;
        }
        IEnumerator enumerator2 = current2.GetEnumerator();
        try
        {
          while (enumerator2.MoveNext())
          {
            if (GTExt._TryFindByPath((Transform) enumerator2.Current, pathPartsRegex, index2, out result, caseSensitive, false, joinedPath))
            {
              if (caseSensitive)
                GTExt.caseSenseInner[current][joinedPath] = result;
              else
                GTExt.caseInsenseInner[current][joinedPath] = result;
              return true;
            }
          }
          break;
        }
        finally
        {
          if (enumerator2 is IDisposable disposable)
            disposable.Dispose();
        }
      case "**":
        if (index == pathPartsRegex.Count - 1)
        {
          result = current.childCount > 0 ? current.GetChild(0) : (Transform) null;
          return current.childCount > 0;
        }
        if (index <= pathPartsRegex.Count - 1 && Regex.IsMatch(current.name, pathPartsRegex[index + 1], caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
        {
          if (index + 2 == pathPartsRegex.Count)
          {
            result = current;
            return true;
          }
          foreach (Transform current3 in current)
          {
            if (GTExt._TryFindByPath(current3, pathPartsRegex, index + 2, out result, caseSensitive, false, joinedPath))
              return true;
          }
        }
        Transform result1;
        if (GTExt._TryBreadthFirstSearchNames(current, pathPartsRegex[index + 1], out result1, caseSensitive))
        {
          if (index + 2 == pathPartsRegex.Count)
          {
            result = result1;
            if (caseSensitive)
              GTExt.caseSenseInner[current][joinedPath] = result;
            else
              GTExt.caseInsenseInner[current][joinedPath] = result;
            return true;
          }
          if (GTExt._TryFindByPath(result1, pathPartsRegex, index + 2, out result, caseSensitive, false, joinedPath))
          {
            if (caseSensitive)
              GTExt.caseSenseInner[current][joinedPath] = result;
            else
              GTExt.caseInsenseInner[current][joinedPath] = result;
            return true;
          }
          break;
        }
        break;
      case "..**":
      case "**..":
        string str1;
        do
        {
          ++index;
          if (index < pathPartsRegex.Count)
            str1 = pathPartsRegex[index];
          else
            break;
        }
        while (str1 == "..**" || str1 == "**..");
        if (index == pathPartsRegex.Count)
        {
          result = current.root;
          if (caseSensitive)
            GTExt.caseSenseInner[current][joinedPath] = result;
          else
            GTExt.caseInsenseInner[current][joinedPath] = result;
          return true;
        }
        Transform parent;
        for (parent = current.parent; (bool) (UnityEngine.Object) parent; parent = parent.parent)
        {
          if (GTExt._TryFindByPath(parent, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
          {
            if (caseSensitive)
              GTExt.caseSenseInner[current][joinedPath] = result;
            else
              GTExt.caseInsenseInner[current][joinedPath] = result;
            return true;
          }
          foreach (Transform current4 in parent)
          {
            if (GTExt._TryFindByPath(current4, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
            {
              if (caseSensitive)
                GTExt.caseSenseInner[current][joinedPath] = result;
              else
                GTExt.caseInsenseInner[current][joinedPath] = result;
              return true;
            }
          }
        }
        if (parent == null)
        {
          int num = GTExt._TryFindByPath(current.root, pathPartsRegex, index, out result, caseSensitive, true, joinedPath) ? 1 : 0;
          if (caseSensitive)
          {
            GTExt.caseSenseInner[current][joinedPath] = result;
            return num != 0;
          }
          GTExt.caseInsenseInner[current][joinedPath] = result;
          return num != 0;
        }
        break;
      default:
        if (Regex.IsMatch(current.name, pathPartsRegex[index], caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
        {
          if (index == pathPartsRegex.Count - 1)
          {
            result = current;
            if (caseSensitive)
              GTExt.caseSenseInner[current][joinedPath] = result;
            else
              GTExt.caseInsenseInner[current][joinedPath] = result;
            return true;
          }
          IEnumerator enumerator3 = current.GetEnumerator();
          try
          {
            while (enumerator3.MoveNext())
            {
              if (GTExt._TryFindByPath((Transform) enumerator3.Current, pathPartsRegex, index + 1, out result, caseSensitive, false, joinedPath))
              {
                if (caseSensitive)
                  GTExt.caseSenseInner[current][joinedPath] = result;
                else
                  GTExt.caseInsenseInner[current][joinedPath] = result;
                return true;
              }
            }
            break;
          }
          finally
          {
            if (enumerator3 is IDisposable disposable)
              disposable.Dispose();
          }
        }
        else
          break;
    }
    result = (Transform) null;
    if (caseSensitive)
      GTExt.caseSenseInner[current][joinedPath] = result;
    else
      GTExt.caseInsenseInner[current][joinedPath] = result;
    return false;
  }

  private static bool _TryBreadthFirstSearchNames(
    Transform root,
    string regexPattern,
    out Transform result,
    bool caseSensitive)
  {
    Queue<Transform> transformQueue = new Queue<Transform>();
    foreach (Transform transform in root)
      transformQueue.Enqueue(transform);
    while (transformQueue.Count > 0)
    {
      Transform transform1 = transformQueue.Dequeue();
      if (Regex.IsMatch(transform1.name, regexPattern, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
      {
        result = transform1;
        return true;
      }
      foreach (Transform transform2 in transform1)
        transformQueue.Enqueue(transform2);
    }
    result = (Transform) null;
    return false;
  }

  public static T[] FindComponentsByExactPath<T>(string path) where T : Component
  {
    List<T> list;
    using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out list))
    {
      list.EnsureCapacity<T>(64 /*0x40*/);
      for (int index = 0; index < SceneManager.sceneCount; ++index)
      {
        Scene sceneAt = SceneManager.GetSceneAt(index);
        if (sceneAt.isLoaded)
          list.AddRange((IEnumerable<T>) sceneAt.FindComponentsByExactPath<T>(path));
      }
      return list.ToArray();
    }
  }

  public static T[] FindComponentsByExactPath<T>(this Scene scene, string path) where T : Component
  {
    string[] splitPath = !string.IsNullOrEmpty(path) ? path.Split('/', StringSplitOptions.RemoveEmptyEntries) : throw new Exception("FindComponentsByExactPath: Provided path cannot be null or empty.");
    return scene.FindComponentsByExactPath<T>(splitPath);
  }

  private static T[] FindComponentsByExactPath<T>(this Scene scene, string[] splitPath) where T : Component
  {
    List<T> objList;
    using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out objList))
    {
      objList.EnsureCapacity<T>(64 /*0x40*/);
      foreach (GameObject rootGameObject in scene.GetRootGameObjects())
        GTExt._FindComponentsByExactPath<T>(rootGameObject.transform, splitPath, 0, objList);
      return objList.ToArray();
    }
  }

  public static T[] FindComponentsByExactPath<T>(this Transform rootXform, string path) where T : Component
  {
    string[] splitPath = !string.IsNullOrEmpty(path) ? path.Split('/', StringSplitOptions.RemoveEmptyEntries) : throw new Exception("FindComponentsByExactPath: Provided path cannot be null or empty.");
    List<T> objList;
    using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out objList))
    {
      objList.EnsureCapacity<T>(64 /*0x40*/);
      foreach (Transform current in rootXform)
        GTExt._FindComponentsByExactPath<T>(current, splitPath, 0, objList);
      return objList.ToArray();
    }
  }

  public static T[] FindComponentsByExactPath<T>(this Transform rootXform, string[] splitPath) where T : Component
  {
    List<T> objList;
    using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out objList))
    {
      objList.EnsureCapacity<T>(64 /*0x40*/);
      foreach (Transform current in rootXform)
        GTExt._FindComponentsByExactPath<T>(current, splitPath, 0, objList);
      return objList.ToArray();
    }
  }

  private static void _FindComponentsByExactPath<T>(
    Transform current,
    string[] splitPath,
    int index,
    List<T> components)
    where T : Component
  {
    if (current.name != splitPath[index])
      return;
    if (index == splitPath.Length - 1)
    {
      T component = current.GetComponent<T>();
      if (!(bool) (UnityEngine.Object) component)
        return;
      components.Add(component);
    }
    else
    {
      foreach (Transform current1 in current)
        GTExt._FindComponentsByExactPath<T>(current1, splitPath, index + 1, components);
    }
  }

  public static T[] FindComponentsByPathInLoadedScenes<T>(string wildcardPath, bool caseSensitive = false) where T : Component
  {
    List<T> objList;
    using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out objList))
    {
      objList.EnsureCapacity<T>(64 /*0x40*/);
      string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(wildcardPath);
      for (int index = 0; index < SceneManager.sceneCount; ++index)
      {
        Scene sceneAt = SceneManager.GetSceneAt(index);
        if (sceneAt.isLoaded)
        {
          foreach (GameObject rootGameObject in sceneAt.GetRootGameObjects())
            GTExt._FindComponentsByPath<T>(rootGameObject.transform, pathPartsRegex, objList, caseSensitive);
        }
      }
      return objList.ToArray();
    }
  }

  public static T[] FindComponentsByPath<T>(this Scene scene, string globPath, bool caseSensitive = false) where T : Component
  {
    string[] pathPartsRegex = !string.IsNullOrEmpty(globPath) ? GTExt._GlobPathToPathPartsRegex(globPath) : throw new Exception("FindComponentsByPath: Provided path cannot be null or empty.");
    return scene.FindComponentsByPath<T>(pathPartsRegex, caseSensitive);
  }

  private static T[] FindComponentsByPath<T>(
    this Scene scene,
    string[] pathPartsRegex,
    bool caseSensitive = false)
    where T : Component
  {
    List<T> objList;
    using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out objList))
    {
      objList.EnsureCapacity<T>(64 /*0x40*/);
      foreach (GameObject rootGameObject in scene.GetRootGameObjects())
        GTExt._FindComponentsByPath<T>(rootGameObject.transform, pathPartsRegex, objList, caseSensitive);
      return objList.ToArray();
    }
  }

  public static T[] FindComponentsByPath<T>(
    this Transform rootXform,
    string globPath,
    bool caseSensitive = false)
    where T : Component
  {
    string[] pathPartsRegex = !string.IsNullOrEmpty(globPath) ? GTExt._GlobPathToPathPartsRegex(globPath) : throw new Exception("FindComponentsByPath: Provided path cannot be null or empty.");
    return rootXform.FindComponentsByPath<T>(pathPartsRegex, caseSensitive);
  }

  public static T[] FindComponentsByPath<T>(
    this Transform rootXform,
    string[] pathPartsRegex,
    bool caseSensitive = false)
    where T : Component
  {
    List<T> objList;
    using (UnityEngine.Pool.CollectionPool<List<T>, T>.Get(out objList))
    {
      objList.EnsureCapacity<T>(64 /*0x40*/);
      GTExt._FindComponentsByPath<T>(rootXform, pathPartsRegex, objList, caseSensitive);
      return objList.ToArray();
    }
  }

  public static void _FindComponentsByPath<T>(
    Transform current,
    string[] pathPartsRegex,
    List<T> components,
    bool caseSensitive)
    where T : Component
  {
    List<Transform> transformList;
    using (UnityEngine.Pool.CollectionPool<List<Transform>, Transform>.Get(out transformList))
    {
      transformList.EnsureCapacity<Transform>(64 /*0x40*/);
      if (!GTExt._TryFindAllByPath(current, (IReadOnlyList<string>) pathPartsRegex, 0, transformList, caseSensitive))
        return;
      for (int index = 0; index < transformList.Count; ++index)
      {
        T[] components1 = transformList[index].GetComponents<T>();
        components.AddRange((IEnumerable<T>) components1);
      }
    }
  }

  private static bool _TryFindAllByPath(
    Transform current,
    IReadOnlyList<string> pathPartsRegex,
    int index,
    List<Transform> results,
    bool caseSensitive,
    bool isAtSceneLevel = false)
  {
    bool allByPath = false;
    if (isAtSceneLevel)
    {
      string str = pathPartsRegex[index];
      if (str == ".." || str == "..**" || str == "**..")
        return false;
      for (int index1 = 0; index1 < SceneManager.sceneCount; ++index1)
      {
        Scene sceneAt = SceneManager.GetSceneAt(index1);
        if (sceneAt.isLoaded)
        {
          foreach (GameObject rootGameObject in sceneAt.GetRootGameObjects())
            allByPath |= GTExt._TryFindAllByPath(rootGameObject.transform, pathPartsRegex, index, results, caseSensitive);
        }
      }
    }
    switch (pathPartsRegex[index])
    {
      case ".":
        if (index == pathPartsRegex.Count - 1)
        {
          results.Add(current);
          return true;
        }
        allByPath |= GTExt._TryFindAllByPath(current, pathPartsRegex, index + 1, results, caseSensitive);
        break;
      case "..":
        if ((bool) (UnityEngine.Object) current.parent)
        {
          if (index == pathPartsRegex.Count - 1)
          {
            results.Add(current.parent);
            return true;
          }
          allByPath |= GTExt._TryFindAllByPath(current.parent, pathPartsRegex, index + 1, results, caseSensitive);
          break;
        }
        break;
      case "**":
        if (index == pathPartsRegex.Count - 1)
        {
          for (int index2 = 0; index2 < current.childCount; ++index2)
          {
            results.Add(current.GetChild(index2));
            allByPath = true;
          }
          break;
        }
        Transform result;
        if (GTExt._TryBreadthFirstSearchNames(current, pathPartsRegex[index + 1], out result, caseSensitive))
        {
          if (index + 2 == pathPartsRegex.Count)
          {
            results.Add(result);
            return true;
          }
          allByPath |= GTExt._TryFindAllByPath(result, pathPartsRegex, index + 2, results, caseSensitive);
          break;
        }
        break;
      case "..**":
      case "**..":
        int index3;
        for (index3 = index + 1; index3 < pathPartsRegex.Count; ++index3)
        {
          string str = pathPartsRegex[index3];
          if (!(str == "..**") && !(str == "**.."))
            break;
        }
        if (index3 == pathPartsRegex.Count)
        {
          results.Add(current.root);
          return true;
        }
        for (Transform current1 = current; (bool) (UnityEngine.Object) current1; current1 = current1.parent)
          allByPath |= GTExt._TryFindAllByPath(current1, pathPartsRegex, index + 1, results, caseSensitive);
        break;
      default:
        if (Regex.IsMatch(current.name, pathPartsRegex[index], caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
        {
          if (index == pathPartsRegex.Count - 1)
          {
            results.Add(current);
            return true;
          }
          IEnumerator enumerator = current.GetEnumerator();
          try
          {
            while (enumerator.MoveNext())
            {
              Transform current2 = (Transform) enumerator.Current;
              allByPath |= GTExt._TryFindAllByPath(current2, pathPartsRegex, index + 1, results, caseSensitive);
            }
            break;
          }
          finally
          {
            if (enumerator is IDisposable disposable)
              disposable.Dispose();
          }
        }
        else
          break;
    }
    return allByPath;
  }

  public static string[] _GlobPathToPathPartsRegex(string path)
  {
    string[] array = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
    int num = 0;
    for (int index = 0; index < array.Length; ++index)
    {
      if (index > 0)
      {
        string str1 = array[index];
        if (str1 == "**" || str1 == "..**" || str1 == "**..")
        {
          string str2 = array[index - 1];
          if (str2 == "**" || str2 == "..**" || str2 == "**..")
            ++num;
        }
      }
      array[index - num] = array[index];
    }
    if (num > 0)
      Array.Resize<string>(ref array, array.Length - num);
    for (int index = 0; index < array.Length; ++index)
      array[index] = GTExt._GlobPathPartToRegex(array[index]);
    return array;
  }

  private static string _GlobPathPartToRegex(string pattern)
  {
    return pattern == "." || pattern == ".." || pattern == "**" || pattern == "..**" || pattern == "**.." || pattern.StartsWith("^") ? pattern : $"^{Regex.Escape(pattern).Replace("\\*", ".*")}$";
  }

  public enum ParityOptions
  {
    XFlip,
    YFlip,
    ZFlip,
    AllFlip,
  }
}
