// Decompiled with JetBrains decompiler
// Type: GorillaExtensions.GTTryFindByExactPath
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable disable
namespace GorillaExtensions;

public static class GTTryFindByExactPath
{
  public static bool WithSiblingIndexAndTypeName<T>(string path, out T out_component) where T : Component
  {
    out_component = default (T);
    if (string.IsNullOrEmpty(path))
      return false;
    int length1 = path.IndexOf("/->/", StringComparison.Ordinal);
    if (length1 < 0)
      return GTTryFindByExactPath.WithSiblingIndex<T>(path, out out_component);
    string xformPath = path.Substring(0, length1);
    string str1 = path.Substring(length1 + 4);
    int result = -1;
    int length2 = str1.IndexOf('#');
    string str2;
    if (length2 >= 0)
    {
      str2 = str1.Substring(0, length2);
      if (!int.TryParse(str1.Substring(length2 + 1), out result))
        result = -1;
    }
    else
      str2 = str1;
    Transform transform;
    ref Transform local = ref transform;
    if (!GTTryFindByExactPath.XformWithSiblingIndex(xformPath, out local))
      return false;
    System.Type type = typeof (T);
    if (!string.Equals(type.Name, str2, StringComparison.Ordinal))
    {
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      System.Type c = (System.Type) null;
      foreach (Assembly assembly in assemblies)
      {
        c = assembly.GetType(str2);
        if (c != (System.Type) null && typeof (Component).IsAssignableFrom(c))
        {
          type = c;
          break;
        }
      }
      if (c == (System.Type) null)
      {
        out_component = transform.GetComponent<T>();
        return (UnityEngine.Object) out_component != (UnityEngine.Object) null;
      }
    }
    Component[] components = transform.GetComponents(type);
    T obj = default (T);
    if (components.Length != 0)
    {
      if (result < 0)
      {
        obj = components[0] as T;
      }
      else
      {
        if (result >= components.Length)
          return false;
        obj = components[result] as T;
      }
    }
    out_component = obj;
    return (UnityEngine.Object) out_component != (UnityEngine.Object) null;
  }

  private static bool WithSiblingIndex<T>(string xformPath, out T component) where T : Component
  {
    component = default (T);
    Transform finalXform;
    if (!GTTryFindByExactPath.XformWithSiblingIndex(xformPath, out finalXform))
      return false;
    component = finalXform.GetComponent<T>();
    return (UnityEngine.Object) component != (UnityEngine.Object) null;
  }

  public static bool XformWithSiblingIndex(string xformPath, out Transform finalXform)
  {
    finalXform = (Transform) null;
    if (string.IsNullOrEmpty(xformPath))
      return false;
    string[] strArray = xformPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
    if (strArray.Length == 0)
      return false;
    Transform transform1 = (Transform) null;
    for (int index1 = 0; index1 < strArray.Length; ++index1)
    {
      string str = strArray[index1];
      int length = str.IndexOf('|');
      if (length < 0)
        return false;
      string s = str.Substring(0, length);
      string b = str.Substring(length + 1);
      int result;
      if (!int.TryParse(s, out result))
        return false;
      if (index1 == 0)
      {
        Transform transform2 = (Transform) null;
        for (int index2 = 0; index2 < SceneManager.sceneCount && (UnityEngine.Object) transform2 == (UnityEngine.Object) null; ++index2)
        {
          Scene sceneAt = SceneManager.GetSceneAt(index2);
          if (sceneAt.IsValid() && sceneAt.isLoaded)
          {
            GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
            if (result >= 0 && result < rootGameObjects.Length)
            {
              Transform transform3 = rootGameObjects[result].transform;
              if (string.Equals(transform3.name, b, StringComparison.Ordinal))
                transform2 = transform3;
            }
          }
        }
        if ((UnityEngine.Object) transform2 == (UnityEngine.Object) null)
          return false;
        transform1 = transform2;
      }
      else
      {
        if (result < 0 || result >= transform1.childCount)
          return false;
        Transform child = transform1.GetChild(result);
        if (!string.Equals(child.name, b, StringComparison.Ordinal))
          return false;
        transform1 = child;
      }
    }
    finalXform = transform1;
    return (UnityEngine.Object) finalXform != (UnityEngine.Object) null;
  }
}
