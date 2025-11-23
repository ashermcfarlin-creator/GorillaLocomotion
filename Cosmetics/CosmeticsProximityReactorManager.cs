// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.CosmeticsProximityReactorManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class CosmeticsProximityReactorManager : MonoBehaviour, IGorillaSliceableSimple
{
  private static CosmeticsProximityReactorManager _instance;
  private readonly List<CosmeticsProximityReactor> cosmetics = new List<CosmeticsProximityReactor>();
  private readonly List<CosmeticsProximityReactor> gorillaBodyPart = new List<CosmeticsProximityReactor>();
  private readonly Dictionary<string, List<CosmeticsProximityReactor>> byType = new Dictionary<string, List<CosmeticsProximityReactor>>((IEqualityComparer<string>) StringComparer.Ordinal);
  private readonly Dictionary<CosmeticsProximityReactor, int> matchedFrame = new Dictionary<CosmeticsProximityReactor, int>();
  [Tooltip("Perf - How many cosmetic groups should we fully process per frame (slice)")]
  [SerializeField]
  private int groupsPerSlice = 1;
  private readonly List<string> typeKeysCache = new List<string>();
  private bool typeKeysDirty;
  private int groupCursor;
  internal static readonly List<string> SharedKeysCache = new List<string>();

  public static CosmeticsProximityReactorManager Instance
  {
    get => CosmeticsProximityReactorManager._instance;
  }

  private void Awake()
  {
    if ((UnityEngine.Object) CosmeticsProximityReactorManager._instance != (UnityEngine.Object) null && (UnityEngine.Object) CosmeticsProximityReactorManager._instance != (UnityEngine.Object) this)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    else
      CosmeticsProximityReactorManager._instance = this;
  }

  public void OnEnable()
  {
    GorillaSlicerSimpleManager.RegisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.Update);
  }

  public void OnDisable()
  {
    GorillaSlicerSimpleManager.UnregisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.Update);
    if (!((UnityEngine.Object) CosmeticsProximityReactorManager._instance == (UnityEngine.Object) this))
      return;
    CosmeticsProximityReactorManager._instance = (CosmeticsProximityReactorManager) null;
  }

  public void Register(CosmeticsProximityReactor cosmetic)
  {
    if ((UnityEngine.Object) cosmetic == (UnityEngine.Object) null)
      return;
    if (cosmetic.IsGorillaBody())
    {
      if (this.gorillaBodyPart.Contains(cosmetic))
        return;
      this.gorillaBodyPart.Add(cosmetic);
    }
    else
    {
      if (!this.cosmetics.Contains(cosmetic))
        this.cosmetics.Add(cosmetic);
      IReadOnlyList<string> types = cosmetic.GetTypes();
      for (int index = 0; index < types.Count; ++index)
      {
        string key = types[index];
        if (!string.IsNullOrEmpty(key))
        {
          List<CosmeticsProximityReactor> proximityReactorList;
          if (!this.byType.TryGetValue(key, out proximityReactorList))
          {
            proximityReactorList = new List<CosmeticsProximityReactor>();
            this.byType[key] = proximityReactorList;
          }
          if (!proximityReactorList.Contains(cosmetic))
          {
            proximityReactorList.Add(cosmetic);
            this.typeKeysDirty = true;
          }
        }
      }
    }
  }

  public void Unregister(CosmeticsProximityReactor cosmetic)
  {
    if ((UnityEngine.Object) cosmetic == (UnityEngine.Object) null)
      return;
    this.cosmetics.Remove(cosmetic);
    this.gorillaBodyPart.Remove(cosmetic);
    this.matchedFrame.Remove(cosmetic);
    foreach (KeyValuePair<string, List<CosmeticsProximityReactor>> keyValuePair in this.byType)
    {
      if (keyValuePair.Value.Remove(cosmetic))
        this.typeKeysDirty = true;
    }
  }

  public void SliceUpdate()
  {
    if (this.cosmetics.Count == 0)
      return;
    if (this.AnyGroupHasTwo())
    {
      if (this.typeKeysDirty)
        this.RebuildTypeKeysCache();
      if (this.typeKeysCache.Count > 0)
      {
        for (int index = 0; index < this.groupsPerSlice && this.typeKeysCache.Count > 0; ++index)
        {
          if (this.groupCursor >= this.typeKeysCache.Count)
            this.groupCursor = 0;
          List<CosmeticsProximityReactor> group;
          if (this.byType.TryGetValue(this.typeKeysCache[this.groupCursor], out group) && group != null && group.Count > 0)
            this.ProcessOneGroup(group);
          ++this.groupCursor;
        }
      }
    }
    if (this.gorillaBodyPart.Count > 0)
    {
      for (int index1 = 0; index1 < this.cosmetics.Count; ++index1)
      {
        CosmeticsProximityReactor cosmetic = this.cosmetics[index1];
        if (!((UnityEngine.Object) cosmetic == (UnityEngine.Object) null))
        {
          if (!cosmetic.AcceptsAnySource())
          {
            cosmetic.OnSourceAboveAll();
          }
          else
          {
            bool flag = false;
            Vector3 contact = new Vector3();
            for (int index2 = 0; index2 < this.gorillaBodyPart.Count; ++index2)
            {
              CosmeticsProximityReactor proximityReactor = this.gorillaBodyPart[index2];
              if (!((UnityEngine.Object) proximityReactor == (UnityEngine.Object) null) && cosmetic.AcceptsThisSource(proximityReactor.gorillaBodyParts))
              {
                bool any;
                float sourceThresholdFor = cosmetic.GetSourceThresholdFor(proximityReactor, out any);
                Vector3 contactPoint;
                if (any && CosmeticsProximityReactorManager.AreCollidersWithinThreshold(proximityReactor, cosmetic, sourceThresholdFor, out contactPoint))
                {
                  cosmetic.OnSourceBelow(contactPoint, proximityReactor.gorillaBodyParts, proximityReactor.GetComponentInParent<VRRig>());
                  contact = contactPoint;
                  flag = true;
                }
              }
            }
            if (flag)
              cosmetic.WhileSourceBelow(contact, CosmeticsProximityReactor.GorillaBodyPart.HandLeft | CosmeticsProximityReactor.GorillaBodyPart.HandRight | CosmeticsProximityReactor.GorillaBodyPart.Mouth, (UnityEngine.Object) this.gorillaBodyPart[0] != (UnityEngine.Object) null ? this.gorillaBodyPart[0].GetComponentInParent<VRRig>() : (VRRig) null);
            else
              cosmetic.OnSourceAboveAll();
          }
        }
      }
    }
    if (this.typeKeysDirty)
      this.RebuildTypeKeysCache();
    for (int index = 0; index < this.typeKeysCache.Count; ++index)
    {
      List<CosmeticsProximityReactor> group;
      if (this.byType.TryGetValue(this.typeKeysCache[index], out group) && group != null && group.Count > 0)
        this.BreakTheBoundForGroup(group);
    }
  }

  private void ProcessOneGroup(List<CosmeticsProximityReactor> group)
  {
    if (this.CheckProximity(group))
      return;
    this.BreakTheBoundForGroup(group);
  }

  private bool CheckProximity(List<CosmeticsProximityReactor> group)
  {
    bool flag = false;
    for (int index1 = 0; index1 < group.Count; ++index1)
    {
      CosmeticsProximityReactor proximityReactor1 = group[index1];
      if (!((UnityEngine.Object) proximityReactor1 == (UnityEngine.Object) null))
      {
        for (int index2 = index1 + 1; index2 < group.Count; ++index2)
        {
          CosmeticsProximityReactor proximityReactor2 = group[index2];
          if (!((UnityEngine.Object) proximityReactor2 == (UnityEngine.Object) null) && !CosmeticsProximityReactorManager.ShouldSkipSameIdPair(proximityReactor1, proximityReactor2))
          {
            bool any1;
            float pairThresholdWith1 = proximityReactor1.GetCosmeticPairThresholdWith(proximityReactor2, out any1);
            bool any2;
            float pairThresholdWith2 = proximityReactor2.GetCosmeticPairThresholdWith(proximityReactor1, out any2);
            if (any1 & any2)
            {
              float threshold = Mathf.Min(pairThresholdWith1, pairThresholdWith2);
              Vector3 contactPoint;
              if (CosmeticsProximityReactorManager.AreCollidersWithinThreshold(proximityReactor1, proximityReactor2, threshold, out contactPoint))
              {
                proximityReactor1.OnCosmeticBelowWith(proximityReactor2, contactPoint);
                proximityReactor2.OnCosmeticBelowWith(proximityReactor1, contactPoint);
                if (proximityReactor1.IsBelow && proximityReactor2.IsBelow)
                {
                  proximityReactor1.RefreshAggregateMatched();
                  proximityReactor2.RefreshAggregateMatched();
                  this.matchedFrame[proximityReactor1] = Time.frameCount;
                  this.matchedFrame[proximityReactor2] = Time.frameCount;
                  flag = true;
                }
              }
            }
          }
        }
      }
    }
    return flag;
  }

  private void BreakTheBoundForGroup(List<CosmeticsProximityReactor> group)
  {
    for (int index = 0; index < group.Count; ++index)
    {
      CosmeticsProximityReactor proximityReactor = group[index];
      int num;
      if (!((UnityEngine.Object) proximityReactor == (UnityEngine.Object) null) && proximityReactor.HasAnyCosmeticMatch() && (!this.matchedFrame.TryGetValue(proximityReactor, out num) || num != Time.frameCount))
      {
        CosmeticsProximityReactor partner;
        Vector3 contact;
        if (this.TryFindAnyCosmeticPartner(proximityReactor, out partner, out contact))
        {
          proximityReactor.WhileCosmeticBelowWith(partner, contact);
          partner.WhileCosmeticBelowWith(proximityReactor, contact);
        }
        else
          proximityReactor.OnCosmeticAboveAll();
      }
    }
  }

  private bool TryFindAnyCosmeticPartner(
    CosmeticsProximityReactor a,
    out CosmeticsProximityReactor partner,
    out Vector3 contact)
  {
    partner = (CosmeticsProximityReactor) null;
    contact = new Vector3();
    IReadOnlyList<string> types = a.GetTypes();
    for (int index1 = 0; index1 < types.Count; ++index1)
    {
      string key = types[index1];
      List<CosmeticsProximityReactor> proximityReactorList;
      if (!string.IsNullOrEmpty(key) && this.byType.TryGetValue(key, out proximityReactorList) && proximityReactorList != null)
      {
        for (int index2 = 0; index2 < proximityReactorList.Count; ++index2)
        {
          CosmeticsProximityReactor proximityReactor = proximityReactorList[index2];
          if (!((UnityEngine.Object) proximityReactor == (UnityEngine.Object) null) && !((UnityEngine.Object) proximityReactor == (UnityEngine.Object) a) && !CosmeticsProximityReactorManager.ShouldSkipSameIdPair(a, proximityReactor))
          {
            bool any1;
            float pairThresholdWith1 = a.GetCosmeticPairThresholdWith(proximityReactor, out any1);
            bool any2;
            float pairThresholdWith2 = proximityReactor.GetCosmeticPairThresholdWith(a, out any2);
            if (any1 & any2)
            {
              float threshold = Mathf.Min(pairThresholdWith1, pairThresholdWith2);
              Vector3 contactPoint;
              if (CosmeticsProximityReactorManager.AreCollidersWithinThreshold(a, proximityReactor, threshold, out contactPoint))
              {
                partner = proximityReactor;
                contact = contactPoint;
                return true;
              }
            }
          }
        }
      }
    }
    return false;
  }

  private static bool ShouldSkipSameIdPair(CosmeticsProximityReactor a, CosmeticsProximityReactor b)
  {
    return (a.ignoreSameCosmeticInstances || b.ignoreSameCosmeticInstances) && !string.IsNullOrEmpty(a.PlayFabID) && !string.IsNullOrEmpty(b.PlayFabID) && string.Equals(a.PlayFabID, b.PlayFabID, StringComparison.Ordinal);
  }

  private static bool AreCollidersWithinThreshold(
    CosmeticsProximityReactor a,
    CosmeticsProximityReactor b,
    float threshold,
    out Vector3 contactPoint)
  {
    Vector3 vector3 = (UnityEngine.Object) b.collider == (UnityEngine.Object) null ? b.transform.position : b.collider.ClosestPoint(a.transform.position);
    Vector3 a1 = (UnityEngine.Object) a.collider == (UnityEngine.Object) null ? a.transform.position : a.collider.ClosestPoint(vector3);
    contactPoint = (a1 + vector3) * 0.5f;
    return (double) Vector3.Distance(a1, vector3) <= (double) threshold;
  }

  private bool AnyGroupHasTwo()
  {
    foreach (KeyValuePair<string, List<CosmeticsProximityReactor>> keyValuePair in this.byType)
    {
      List<CosmeticsProximityReactor> proximityReactorList = keyValuePair.Value;
      if (proximityReactorList != null && proximityReactorList.Count >= 2)
        return true;
    }
    return false;
  }

  private void RebuildTypeKeysCache()
  {
    this.typeKeysCache.Clear();
    foreach (KeyValuePair<string, List<CosmeticsProximityReactor>> keyValuePair in this.byType)
    {
      List<CosmeticsProximityReactor> proximityReactorList = keyValuePair.Value;
      if (proximityReactorList != null && proximityReactorList.Count > 0)
        this.typeKeysCache.Add(keyValuePair.Key);
    }
    this.typeKeysDirty = false;
    if (this.groupCursor < this.typeKeysCache.Count)
      return;
    this.groupCursor = 0;
  }
}
