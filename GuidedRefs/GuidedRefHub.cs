// Decompiled with JetBrains decompiler
// Type: GorillaTag.GuidedRefs.GuidedRefHub
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaTag.GuidedRefs.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag.GuidedRefs;

[DefaultExecutionOrder(-2147483648 /*0x80000000*/)]
public class GuidedRefHub : MonoBehaviour, IGuidedRefMonoBehaviour, IGuidedRefObject
{
  [SerializeField]
  private bool isRootInstance;
  public GuidedRefHubIdSO hubId;
  [OnEnterPlay_SetNull]
  [NonSerialized]
  public static GuidedRefHub rootInstance;
  [OnEnterPlay_Set(false)]
  [NonSerialized]
  public static bool hasRootInstance;
  [DebugReadout]
  private readonly Dictionary<GuidedRefTargetIdSO, RelayInfo> lookupRelayInfoByTargetId = new Dictionary<GuidedRefTargetIdSO, RelayInfo>(256 /*0x0100*/);
  private static readonly Dictionary<RelayInfo, GuidedRefTargetIdSO> static_relayInfo_to_targetId = new Dictionary<RelayInfo, GuidedRefTargetIdSO>(256 /*0x0100*/);
  [OnEnterPlay_Clear]
  private static readonly Dictionary<int, List<GuidedRefHub>> globalLookupHubsThatHaveRegisteredInstId = new Dictionary<int, List<GuidedRefHub>>(256 /*0x0100*/);
  [OnEnterPlay_Clear]
  private static readonly Dictionary<GuidedRefHub, List<int>> globalLookupRefInstIDsByHub = new Dictionary<GuidedRefHub, List<int>>(256 /*0x0100*/);
  [OnEnterPlay_Clear]
  private static readonly List<GuidedRefHub> globalHubsTransientList = new List<GuidedRefHub>(32 /*0x20*/);
  private const string kUnsuppliedCallerName = "UNSUPPLIED_CALLER_NAME";
  [DebugReadout]
  [OnEnterPlay_Clear]
  internal static readonly HashSet<IGuidedRefReceiverMono> kReceiversWaitingToFullyResolve = new HashSet<IGuidedRefReceiverMono>(256 /*0x0100*/);
  [DebugReadout]
  [OnEnterPlay_Clear]
  internal static readonly HashSet<IGuidedRefReceiverMono> kReceiversFullyRegistered = new HashSet<IGuidedRefReceiverMono>(256 /*0x0100*/);

  protected void Awake() => this.GuidedRefInitialize();

  protected void OnDestroy()
  {
    if (ApplicationQuittingState.IsQuitting)
      return;
    if (this.isRootInstance)
    {
      GuidedRefHub.hasRootInstance = false;
      GuidedRefHub.rootInstance = (GuidedRefHub) null;
    }
    List<int> intList;
    if (!GuidedRefHub.globalLookupRefInstIDsByHub.TryGetValue(this, out intList))
      return;
    foreach (int key in intList)
      GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId[key].Remove(this);
    GuidedRefHub.globalLookupRefInstIDsByHub.Remove(this);
  }

  public void GuidedRefInitialize()
  {
    if (this.isRootInstance)
    {
      if (GuidedRefHub.hasRootInstance)
      {
        Debug.LogError((object) $"GuidedRefHub: Attempted to assign global instance when one was already assigned:\n- This path: {this.transform.GetPath()}\n- Global instance: {GuidedRefHub.rootInstance.transform.GetPath()}\n", (UnityEngine.Object) this);
        UnityEngine.Object.Destroy((UnityEngine.Object) this);
        return;
      }
      GuidedRefHub.hasRootInstance = true;
      GuidedRefHub.rootInstance = this;
    }
    GuidedRefHub.globalLookupRefInstIDsByHub[this] = new List<int>(2);
  }

  public static bool IsInstanceIDRegisteredWithAnyHub(int instanceID)
  {
    return GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.ContainsKey(instanceID);
  }

  private void RegisterTarget_Internal<TIGuidedRefTargetMono>(TIGuidedRefTargetMono targetMono) where TIGuidedRefTargetMono : IGuidedRefTargetMono
  {
    RelayInfo relayInfoByTargetId = this.GetOrAddRelayInfoByTargetId(targetMono.GRefTargetInfo.targetId);
    if (relayInfoByTargetId == null)
      return;
    IGuidedRefTargetMono guidedRefTargetMono = (IGuidedRefTargetMono) targetMono;
    if (relayInfoByTargetId.targetMono != null && relayInfoByTargetId.targetMono != guidedRefTargetMono)
    {
      if (targetMono.GRefTargetInfo.hackIgnoreDuplicateRegistration)
        return;
      Debug.LogError((object) $"GuidedRefHub: Multiple targets registering with the same Hub. Maybe look at the HubID you are using:- hub=\"{this.transform.GetPath()}\"\n- target1=\"{relayInfoByTargetId.targetMono.transform.GetPath()}\",\n- target2=\"{targetMono.transform.GetPath()}\"", (UnityEngine.Object) this);
    }
    else
    {
      int instanceId = targetMono.GetInstanceID();
      GuidedRefHub.GetHubsThatHaveRegisteredInstId(instanceId).Add(this);
      List<int> intList;
      if (!GuidedRefHub.globalLookupRefInstIDsByHub.TryGetValue(this, out intList))
      {
        Debug.LogError((object) $"GuidedRefHub: It appears hub was not registered before `RegisterTarget` was called on it: - hub: \"{this.transform.GetPath()}\"\n- target: \"{targetMono.transform.GetPath()}\"", (UnityEngine.Object) this);
      }
      else
      {
        intList.Add(instanceId);
        relayInfoByTargetId.targetMono = (IGuidedRefTargetMono) targetMono;
        GuidedRefHub.ResolveReferences(relayInfoByTargetId);
      }
    }
  }

  public static void RegisterTarget<TIGuidedRefTargetMono>(
    TIGuidedRefTargetMono targetMono,
    GuidedRefHubIdSO[] hubIds = null,
    Component debugCaller = null)
    where TIGuidedRefTargetMono : IGuidedRefTargetMono
  {
    if ((object) targetMono == null)
    {
      Debug.LogError((object) $"GuidedRefHub: Cannot register null target from \"{((UnityEngine.Object) debugCaller == (UnityEngine.Object) null ? "UNSUPPLIED_CALLER_NAME" : debugCaller.name)}\".", (UnityEngine.Object) debugCaller);
    }
    else
    {
      if ((UnityEngine.Object) targetMono.GRefTargetInfo.targetId == (UnityEngine.Object) null)
        return;
      GuidedRefHub.globalHubsTransientList.Clear();
      targetMono.transform.GetComponentsInParent<GuidedRefHub>(true, GuidedRefHub.globalHubsTransientList);
      if (GuidedRefHub.hasRootInstance)
        GuidedRefHub.globalHubsTransientList.Add(GuidedRefHub.rootInstance);
      bool flag = false;
      foreach (GuidedRefHub globalHubsTransient in GuidedRefHub.globalHubsTransientList)
      {
        if (hubIds == null || hubIds.Length <= 0 || Array.IndexOf<GuidedRefHubIdSO>(hubIds, globalHubsTransient.hubId) != -1)
        {
          flag = true;
          globalHubsTransient.RegisterTarget_Internal<TIGuidedRefTargetMono>(targetMono);
        }
      }
      if (flag || !Application.isPlaying)
        return;
      Debug.LogError((object) $"GuidedRefHub: Could not find hub for target: \"{targetMono.transform.GetPath()}\"", (UnityEngine.Object) targetMono.transform);
    }
  }

  public static void UnregisterTarget<TIGuidedRefTargetMono>(
    TIGuidedRefTargetMono targetMono,
    bool destroyed = true)
    where TIGuidedRefTargetMono : IGuidedRefTargetMono
  {
    if (ApplicationQuittingState.IsQuitting)
      return;
    if ((object) targetMono == null)
    {
      Debug.LogError((object) "GuidedRefHub: Cannot unregister null target.");
    }
    else
    {
      int instanceId = targetMono.GetInstanceID();
      List<GuidedRefHub> guidedRefHubList;
      if (!GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.TryGetValue(instanceId, out guidedRefHubList))
        return;
      foreach (GuidedRefHub guidedRefHub in guidedRefHubList)
      {
        RelayInfo relayInfo;
        if (guidedRefHub.lookupRelayInfoByTargetId.TryGetValue(targetMono.GRefTargetInfo.targetId, out relayInfo))
        {
          foreach (RegisteredReceiverFieldInfo registeredField in relayInfo.registeredFields)
          {
            if (registeredField.receiverMono != null)
            {
              registeredField.receiverMono.OnGuidedRefTargetDestroyed(registeredField.fieldId);
              GuidedRefHub.kReceiversWaitingToFullyResolve.Remove(registeredField.receiverMono);
              relayInfo.resolvedFields.Remove(registeredField);
              relayInfo.registeredFields.Add(registeredField);
              ++registeredField.receiverMono.GuidedRefsWaitingToResolveCount;
            }
          }
        }
      }
      foreach (GuidedRefHub key in guidedRefHubList)
      {
        RelayInfo relayInfo;
        if (key.lookupRelayInfoByTargetId.TryGetValue(targetMono.GRefTargetInfo.targetId, out relayInfo))
          relayInfo.targetMono = (IGuidedRefTargetMono) null;
        GuidedRefHub.globalLookupRefInstIDsByHub[key].Remove(instanceId);
      }
      GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.Remove(instanceId);
    }
  }

  public static void ReceiverFullyRegistered<TIGuidedRefReceiverMono>(
    TIGuidedRefReceiverMono receiverMono)
    where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
  {
    GuidedRefHub.kReceiversFullyRegistered.Add((IGuidedRefReceiverMono) receiverMono);
    GuidedRefHub.kReceiversWaitingToFullyResolve.Add((IGuidedRefReceiverMono) receiverMono);
    GuidedRefHub.CheckAndNotifyIfReceiverFullyResolved<TIGuidedRefReceiverMono>(receiverMono);
  }

  private static void CheckAndNotifyIfReceiverFullyResolved<TIGuidedRefReceiverMono>(
    TIGuidedRefReceiverMono receiverMono)
    where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
  {
    if (receiverMono.GuidedRefsWaitingToResolveCount != 0 || !GuidedRefHub.kReceiversFullyRegistered.Contains((IGuidedRefReceiverMono) receiverMono))
      return;
    GuidedRefHub.kReceiversWaitingToFullyResolve.Remove((IGuidedRefReceiverMono) receiverMono);
    receiverMono.OnAllGuidedRefsResolved();
  }

  private void RegisterReceiverField(
    RegisteredReceiverFieldInfo registeredReceiverFieldInfo,
    GuidedRefTargetIdSO targetId)
  {
    GuidedRefHub.globalLookupRefInstIDsByHub[this].Add(registeredReceiverFieldInfo.receiverMono.GetInstanceID());
    GuidedRefHub.GetHubsThatHaveRegisteredInstId(registeredReceiverFieldInfo.receiverMono.GetInstanceID()).Add(this);
    RelayInfo relayInfoByTargetId = this.GetOrAddRelayInfoByTargetId(targetId);
    relayInfoByTargetId.registeredFields.Add(registeredReceiverFieldInfo);
    GuidedRefHub.ResolveReferences(relayInfoByTargetId);
  }

  private static void RegisterReceiverField_Internal<TIGuidedRefReceiverMono>(
    GuidedRefHubIdSO hubId,
    TIGuidedRefReceiverMono receiverMono,
    int fieldId,
    GuidedRefTargetIdSO targetId,
    int index)
    where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
  {
    if ((object) receiverMono == null)
    {
      Debug.LogError((object) "GuidedRefHub: Cannot register null receiver.");
    }
    else
    {
      GuidedRefHub.globalHubsTransientList.Clear();
      receiverMono.transform.GetComponentsInParent<GuidedRefHub>(true, GuidedRefHub.globalHubsTransientList);
      if (GuidedRefHub.hasRootInstance)
        GuidedRefHub.globalHubsTransientList.Add(GuidedRefHub.rootInstance);
      RegisteredReceiverFieldInfo registeredReceiverFieldInfo = new RegisteredReceiverFieldInfo()
      {
        receiverMono = (IGuidedRefReceiverMono) receiverMono,
        fieldId = fieldId,
        index = index
      };
      bool flag = false;
      foreach (GuidedRefHub globalHubsTransient in GuidedRefHub.globalHubsTransientList)
      {
        if (!((UnityEngine.Object) hubId != (UnityEngine.Object) null) || !((UnityEngine.Object) globalHubsTransient.hubId != (UnityEngine.Object) hubId))
        {
          flag = true;
          globalHubsTransient.RegisterReceiverField(registeredReceiverFieldInfo, targetId);
          break;
        }
      }
      if (flag)
        ++receiverMono.GuidedRefsWaitingToResolveCount;
      else
        Debug.LogError((object) ("Could not find matching GuidedRefHub to register with for receiver at: " + receiverMono.transform.GetPath()), (UnityEngine.Object) receiverMono.transform);
    }
  }

  public static void RegisterReceiverField<TIGuidedRefReceiverMono>(
    TIGuidedRefReceiverMono receiverMono,
    string fieldIdName,
    ref GuidedRefReceiverFieldInfo fieldInfo)
    where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
  {
    if (!GRef.ShouldResolveNow(fieldInfo.resolveModes))
      return;
    fieldInfo.fieldId = Shader.PropertyToID(fieldIdName);
    GuidedRefHub.RegisterReceiverField_Internal<TIGuidedRefReceiverMono>(fieldInfo.hubId, receiverMono, fieldInfo.fieldId, fieldInfo.targetId, -1);
  }

  public static void RegisterReceiverArray<TIGuidedRefReceiverMono, T>(
    TIGuidedRefReceiverMono receiverMono,
    string fieldIdName,
    ref T[] receiverArray,
    ref GuidedRefReceiverArrayInfo arrayInfo)
    where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
    where T : UnityEngine.Object
  {
    if (!GRef.ShouldResolveNow(arrayInfo.resolveModes))
      return;
    if (receiverArray == null)
      receiverArray = new T[arrayInfo.targets.Length];
    else if (receiverArray.Length != arrayInfo.targets.Length)
      Array.Resize<T>(ref receiverArray, arrayInfo.targets.Length);
    arrayInfo.fieldId = Shader.PropertyToID(fieldIdName);
    for (int index = 0; index < arrayInfo.targets.Length; ++index)
      GuidedRefHub.RegisterReceiverField_Internal<TIGuidedRefReceiverMono>(arrayInfo.hubId, receiverMono, arrayInfo.fieldId, arrayInfo.targets[index], index);
  }

  public static void UnregisterReceiver<TIGuidedRefReceiverMono>(
    TIGuidedRefReceiverMono receiverMono)
    where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
  {
    if ((object) receiverMono == null)
    {
      Debug.LogError((object) "GuidedRefHub: Cannot unregister null receiver.");
    }
    else
    {
      int instanceId = receiverMono.GetInstanceID();
      List<GuidedRefHub> guidedRefHubList;
      if (!GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.TryGetValue(instanceId, out guidedRefHubList))
      {
        Debug.LogError((object) "Tried to unregister a receiver before it was registered.");
      }
      else
      {
        IGuidedRefReceiverMono iReceiverMono = (IGuidedRefReceiverMono) receiverMono;
        foreach (GuidedRefHub key in guidedRefHubList)
        {
          foreach (RelayInfo relayInfo in key.lookupRelayInfoByTargetId.Values)
            relayInfo.registeredFields.RemoveAll((Predicate<RegisteredReceiverFieldInfo>) (fieldInfo => fieldInfo.receiverMono == iReceiverMono));
          GuidedRefHub.globalLookupRefInstIDsByHub[key].Remove(instanceId);
        }
        GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.Remove(instanceId);
        receiverMono.GuidedRefsWaitingToResolveCount = 0;
      }
    }
  }

  private RelayInfo GetOrAddRelayInfoByTargetId(GuidedRefTargetIdSO targetId)
  {
    if ((UnityEngine.Object) targetId == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "GetOrAddRelayInfoByTargetId cannot register null target id");
      return (RelayInfo) null;
    }
    RelayInfo key;
    if (!this.lookupRelayInfoByTargetId.TryGetValue(targetId, out key))
    {
      key = new RelayInfo()
      {
        targetMono = (IGuidedRefTargetMono) null,
        registeredFields = new List<RegisteredReceiverFieldInfo>(1),
        resolvedFields = new List<RegisteredReceiverFieldInfo>(1)
      };
      this.lookupRelayInfoByTargetId[targetId] = key;
      GuidedRefHub.static_relayInfo_to_targetId[key] = targetId;
    }
    return key;
  }

  public static List<GuidedRefHub> GetHubsThatHaveRegisteredInstId(int instanceId)
  {
    List<GuidedRefHub> registeredInstId;
    if (!GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId.TryGetValue(instanceId, out registeredInstId))
    {
      registeredInstId = new List<GuidedRefHub>(1);
      GuidedRefHub.globalLookupHubsThatHaveRegisteredInstId[instanceId] = registeredInstId;
    }
    return registeredInstId;
  }

  private static void ResolveReferences(RelayInfo relayInfo)
  {
    if (ApplicationQuittingState.IsQuitting)
      return;
    if (relayInfo == null)
      Debug.LogError((object) "GuidedRefHub.ResolveReferences: (this should never happen) relayInfo is null.");
    else if (relayInfo.registeredFields == null)
    {
      GuidedRefTargetIdSO guidedRefTargetIdSo = GuidedRefHub.static_relayInfo_to_targetId[relayInfo];
      Debug.LogError((object) $"GuidedRefHub.ResolveReferences: (this should never happen) \"{((UnityEngine.Object) guidedRefTargetIdSo != (UnityEngine.Object) null ? guidedRefTargetIdSo.name : "NULL")}\"relayInfo.registeredFields is null.");
    }
    else
    {
      if (relayInfo.targetMono == null)
        return;
      for (int index = relayInfo.registeredFields.Count - 1; index >= 0; --index)
      {
        RegisteredReceiverFieldInfo registeredField = relayInfo.registeredFields[index];
        if (registeredField.receiverMono.GuidedRefTryResolveReference(new GuidedRefTryResolveInfo()
        {
          fieldId = registeredField.fieldId,
          index = registeredField.index,
          targetMono = relayInfo.targetMono
        }))
        {
          relayInfo.registeredFields.RemoveAt(index);
          GuidedRefHub.CheckAndNotifyIfReceiverFullyResolved<IGuidedRefReceiverMono>(registeredField.receiverMono);
          relayInfo.resolvedFields.Add(registeredField);
        }
      }
    }
  }

  public static bool TryResolveField<TIGuidedRefReceiverMono, T>(
    TIGuidedRefReceiverMono receiverMono,
    ref T refReceiverObj,
    GuidedRefReceiverFieldInfo receiverFieldInfo,
    GuidedRefTryResolveInfo tryResolveInfo)
    where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
    where T : UnityEngine.Object
  {
    if (tryResolveInfo.index > -1 || tryResolveInfo.fieldId != receiverFieldInfo.fieldId || (UnityEngine.Object) refReceiverObj != (UnityEngine.Object) null)
      return false;
    int num = tryResolveInfo.targetMono == null ? 0 : (tryResolveInfo.targetMono.GuidedRefTargetObject != (UnityEngine.Object) null ? 1 : 0);
    T obj = num != 0 ? tryResolveInfo.targetMono.GuidedRefTargetObject as T : default (T);
    if (num == 0)
    {
      string fieldNameById = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
      Debug.LogError((object) $"TryResolveField: Receiver \"{receiverMono.transform.name}\" with field \"{fieldNameById}\": was already assigned to something other than matching target id! Assigning to found target anyway. Make the receiving field null before attempting to resolve to prevent this message. {$"fieldId={tryResolveInfo.fieldId}, receiver path=\"{receiverMono.transform.GetPath()}\""}");
    }
    else if ((UnityEngine.Object) refReceiverObj != (UnityEngine.Object) null && (UnityEngine.Object) refReceiverObj != (UnityEngine.Object) obj)
    {
      Debug.LogError((object) "was assigned didn't match assigning anyway");
      string fieldNameById = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
      Debug.LogError((object) $"TryResolveField: Receiver \"{receiverMono.transform.name}\" with field \"{fieldNameById}\" was already assigned to something other than matching target id! Assigning to found target anyway. Make the receiving field null before attempting to resolve to prevent this message. {$"fieldId={tryResolveInfo.fieldId}, receiver path=\"{receiverMono.transform.GetPath()}\""}");
    }
    refReceiverObj = obj;
    --receiverMono.GuidedRefsWaitingToResolveCount;
    return true;
  }

  public static bool TryResolveArrayItem<TIGuidedRefReceiverMono, T>(
    TIGuidedRefReceiverMono receiverMono,
    IList<T> receivingArray,
    GuidedRefReceiverArrayInfo receiverArrayInfo,
    GuidedRefTryResolveInfo tryResolveInfo)
    where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
    where T : UnityEngine.Object
  {
    return GuidedRefHub.TryResolveArrayItem<TIGuidedRefReceiverMono, T>(receiverMono, receivingArray, receiverArrayInfo, tryResolveInfo, out bool _);
  }

  public static bool TryResolveArrayItem<TIGuidedRefReceiverMono, T>(
    TIGuidedRefReceiverMono receiverMono,
    IList<T> receivingArray,
    GuidedRefReceiverArrayInfo receiverArrayInfo,
    GuidedRefTryResolveInfo tryResolveInfo,
    out bool arrayResolved)
    where TIGuidedRefReceiverMono : IGuidedRefReceiverMono
    where T : UnityEngine.Object
  {
    arrayResolved = false;
    if (tryResolveInfo.index <= -1 && receiverArrayInfo.fieldId != tryResolveInfo.fieldId)
      return false;
    if (receivingArray == null)
    {
      string fieldNameById = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
      Debug.LogError((object) $"TryResolveArrayItem: Receiver \"{receiverMono.transform.name}\" with array \"{fieldNameById}\": Receiving array cannot be null!{$"fieldId={tryResolveInfo.fieldId}, receiver path=\"{receiverMono.transform.GetPath()}\""}");
      return false;
    }
    if (receiverArrayInfo.targets == null)
    {
      string fieldNameById = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
      Debug.LogError((object) $"TryResolveArrayItem: Receiver component \"{receiverMono.transform.name}\" with array \"{fieldNameById}\": Targets array is null! It must have been set to null after registering. If this intentional than the you need to unregister first.{$"fieldId={tryResolveInfo.fieldId}, receiver path=\"{receiverMono.transform.GetPath()}\""}");
      return false;
    }
    int length = receiverArrayInfo.targets.Length;
    if (length <= receiverArrayInfo.resolveCount)
    {
      string fieldNameById = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
      Debug.LogError((object) $"TryResolveArrayItem: Receiver component \"{receiverMono.transform.name}\" with array \"{fieldNameById}\": Targets array size is equal or smaller than resolve count. Did you change the size of the array before it finished resolving or before unregistering?{$"fieldId={tryResolveInfo.fieldId}, receiver path=\"{receiverMono.transform.GetPath()}\""}");
      return false;
    }
    if (length != receivingArray.Count)
    {
      string fieldNameById = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
      Debug.LogError((object) $"TryResolveArrayItem: Receiver component \"{receiverMono.transform.name}\" with array \"{fieldNameById}\": The sizes of `receivingList` and `receiverArrayInfo.fieldInfos` are not equal. They must be the same length before calling.{$"fieldId={tryResolveInfo.fieldId}, receiver path=\"{receiverMono.transform.GetPath()}\""}");
      return false;
    }
    T guidedRefTargetObject = tryResolveInfo.targetMono.GuidedRefTargetObject as T;
    if ((UnityEngine.Object) guidedRefTargetObject == (UnityEngine.Object) null)
    {
      string fieldNameById = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
      Debug.LogError((object) $"TryResolveArrayItem: Receiver \"{receiverMono.transform.name}\" with field \"{fieldNameById}\" found a matching target id but target object was null! Was it destroyed without unregistering? {$"fieldId={tryResolveInfo.fieldId}, receiver path=\"{receiverMono.transform.GetPath()}\""}");
    }
    if ((UnityEngine.Object) receivingArray[tryResolveInfo.index] != (UnityEngine.Object) null && (UnityEngine.Object) receivingArray[tryResolveInfo.index] != (UnityEngine.Object) guidedRefTargetObject)
    {
      string fieldNameById = GuidedRefHub.GetFieldNameByID(tryResolveInfo.fieldId);
      Debug.LogError((object) $"TryResolveArrayItem: Receiver \"{receiverMono.transform.name}\" with array \"{fieldNameById}\" {$"at index {tryResolveInfo.index}: Already assigned to something other than matching target id! "}Assigning to found target anyway. Make the receiving field null before attempting to resolve to prevent this message. {$"fieldId={tryResolveInfo.fieldId}, receiver path=\"{receiverMono.transform.GetPath()}\""}");
    }
    arrayResolved = ++receiverArrayInfo.resolveCount >= length;
    --receiverMono.GuidedRefsWaitingToResolveCount;
    receivingArray[tryResolveInfo.index] = guidedRefTargetObject;
    return true;
  }

  public static string GetFieldNameByID(int fieldId) => "FieldNameOnlyAvailableInEditor";

  Transform IGuidedRefMonoBehaviour.get_transform() => this.transform;

  int IGuidedRefObject.GetInstanceID() => this.GetInstanceID();
}
