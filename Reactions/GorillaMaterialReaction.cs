// Decompiled with JetBrains decompiler
// Type: GorillaTag.Reactions.GorillaMaterialReaction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using GorillaNetworking;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

#nullable disable
namespace GorillaTag.Reactions;

public class GorillaMaterialReaction : MonoBehaviour, ITickSystemPost
{
  [SerializeField]
  private GorillaMaterialReaction.ReactionEntry[] _statusEffectReactions;
  private int _previousMatIndex;
  private GorillaMaterialReaction.EMomentInState _currentMomentInState;
  private double _currentMatIndexStartTime;
  private double _currentMomentDuration;
  private int _reactionsRemaining;
  private int _momentEnumCount;
  private int _matCount;
  private GameObject[][] _mat_x_moment_x_activeBool_to_gObjs;
  private VRRig _ownerVRRig;

  public void PopulateRuntimeLookupArrays()
  {
    this._momentEnumCount = ((GorillaMaterialReaction.EMomentInState[]) Enum.GetValues(typeof (GorillaMaterialReaction.EMomentInState))).Length;
    this._matCount = this._ownerVRRig.materialsToChangeTo.Length;
    this._mat_x_moment_x_activeBool_to_gObjs = new GameObject[this._momentEnumCount * this._matCount * 2][];
    for (int index1 = 0; index1 < this._matCount; ++index1)
    {
      for (int index2 = 0; index2 < this._momentEnumCount; ++index2)
      {
        GorillaMaterialReaction.EMomentInState emomentInState = (GorillaMaterialReaction.EMomentInState) index2;
        List<GameObject> gameObjectList1 = new List<GameObject>();
        List<GameObject> gameObjectList2 = new List<GameObject>();
        foreach (GorillaMaterialReaction.ReactionEntry statusEffectReaction in this._statusEffectReactions)
        {
          foreach (int statusMaterialIndex in statusEffectReaction.statusMaterialIndexes)
          {
            if (statusMaterialIndex == index1)
            {
              foreach (GorillaMaterialReaction.GameObjectStates gameObjectState in statusEffectReaction.gameObjectStates)
              {
                switch (emomentInState)
                {
                  case GorillaMaterialReaction.EMomentInState.OnEnter:
                    if (gameObjectState.onEnter.change)
                    {
                      if (gameObjectState.onEnter.activeState)
                      {
                        gameObjectList1.Add(this.gameObject);
                        break;
                      }
                      gameObjectList2.Add(this.gameObject);
                      break;
                    }
                    break;
                  case GorillaMaterialReaction.EMomentInState.OnStay:
                    if (gameObjectState.onStay.change)
                    {
                      if (gameObjectState.onEnter.activeState)
                      {
                        gameObjectList1.Add(this.gameObject);
                        break;
                      }
                      gameObjectList2.Add(this.gameObject);
                      break;
                    }
                    break;
                  case GorillaMaterialReaction.EMomentInState.OnExit:
                    if (gameObjectState.onExit.change)
                    {
                      if (gameObjectState.onEnter.activeState)
                      {
                        gameObjectList1.Add(this.gameObject);
                        break;
                      }
                      gameObjectList2.Add(this.gameObject);
                      break;
                    }
                    break;
                  default:
                    Debug.LogError((object) $"Unhandled enum value for {"EMomentInState"}: {emomentInState}");
                    break;
                }
              }
            }
          }
        }
        int index3 = index1 * this._momentEnumCount * 2 + index2 * 2;
        this._mat_x_moment_x_activeBool_to_gObjs[index3] = gameObjectList2.ToArray();
        this._mat_x_moment_x_activeBool_to_gObjs[index3 + 1] = gameObjectList1.ToArray();
      }
    }
  }

  protected void Awake()
  {
    this.RemoveAndReportNulls();
    this.PopulateRuntimeLookupArrays();
  }

  protected void OnEnable()
  {
    if ((UnityEngine.Object) this._ownerVRRig == (UnityEngine.Object) null)
      this._ownerVRRig = this.GetComponentInParent<VRRig>(true);
    if ((UnityEngine.Object) this._ownerVRRig == (UnityEngine.Object) null)
    {
      Debug.LogError((object) ("GorillaMaterialReaction: Disabling because could not find VRRig! Hierarchy path: " + this.transform.GetPath()), (UnityEngine.Object) this);
      this.enabled = false;
    }
    else
    {
      this._reactionsRemaining = 0;
      for (int index = 0; index < this._statusEffectReactions.Length; ++index)
        this._reactionsRemaining += this._statusEffectReactions[index].gameObjectStates.Length;
      this._currentMatIndexStartTime = 0.0;
      TickSystem<object>.AddCallbackTarget((object) this);
    }
  }

  protected void OnDisable() => TickSystem<object>.RemoveCallbackTarget((object) this);

  bool ITickSystemPost.PostTickRunning { get; set; }

  void ITickSystemPost.PostTick()
  {
    if (!GorillaComputer.hasInstance || (UnityEngine.Object) this._ownerVRRig == (UnityEngine.Object) null)
      return;
    GorillaComputer instance1 = GorillaComputer.instance;
    int num1 = (UnityEngine.Object) GorillaGameManager.instance == (UnityEngine.Object) null ? 0 : GorillaGameManager.instance.MyMatIndex(this._ownerVRRig.creator);
    if (this._previousMatIndex == num1 && this._reactionsRemaining <= 0)
      return;
    double num2 = (double) instance1.startupMillis / 1000.0 + Time.realtimeSinceStartupAsDouble;
    bool flag = false;
    if (this._currentMomentInState == GorillaMaterialReaction.EMomentInState.OnExit && this._previousMatIndex != num1)
    {
      this._currentMomentInState = GorillaMaterialReaction.EMomentInState.OnEnter;
      flag = true;
      this._currentMatIndexStartTime = num2;
      this._currentMomentDuration = -1.0;
      GorillaGameManager instance2 = GorillaGameManager.instance;
      if ((UnityEngine.Object) instance2 != (UnityEngine.Object) null && instance2 is GorillaTagManager gorillaTagManager)
        this._currentMomentDuration = (double) gorillaTagManager.tagCoolDown;
    }
    else if (this._currentMomentInState == GorillaMaterialReaction.EMomentInState.OnEnter && this._previousMatIndex == num1 && (this._currentMomentDuration < 0.0 || this._currentMomentDuration < num2 - this._currentMatIndexStartTime))
    {
      this._currentMomentInState = GorillaMaterialReaction.EMomentInState.OnStay;
      flag = true;
      this._currentMomentDuration = -1.0;
    }
    else if (this._currentMomentInState == GorillaMaterialReaction.EMomentInState.OnStay && this._previousMatIndex != num1)
    {
      this._currentMomentInState = GorillaMaterialReaction.EMomentInState.OnExit;
      flag = true;
      this._currentMomentDuration = -1.0;
    }
    this._previousMatIndex = num1;
    if (!flag)
      return;
    for (int index = 0; index < 2; ++index)
    {
      foreach (GameObject gameObject in this._mat_x_moment_x_activeBool_to_gObjs[num1 * this._momentEnumCount * 2 + (int) this._currentMomentInState * 2 + index])
        gameObject.SetActive(index == 1);
    }
  }

  private void RemoveAndReportNulls()
  {
    StringBuilder stringBuilder = new StringBuilder(1024 /*0x0400*/);
    if (this._statusEffectReactions == null)
    {
      Debug.Log((object) ($"{nameof (GorillaMaterialReaction)}: The array `{this._statusEffectReactions}` is null. " + "(this should never happen)"));
      this._statusEffectReactions = Array.Empty<GorillaMaterialReaction.ReactionEntry>();
    }
    for (int index1 = 0; index1 < this._statusEffectReactions.Length; ++index1)
    {
      GorillaMaterialReaction.GameObjectStates[] gameObjectStates = this._statusEffectReactions[index1].gameObjectStates;
      if (gameObjectStates == null)
      {
        this._statusEffectReactions[index1].gameObjectStates = Array.Empty<GorillaMaterialReaction.GameObjectStates>();
      }
      else
      {
        int index2 = 0;
        int[] numArray = new int[gameObjectStates.Length];
        for (int index3 = 0; index3 < gameObjectStates.Length; ++index3)
        {
          if ((UnityEngine.Object) gameObjectStates[index3].gameObject == (UnityEngine.Object) null)
          {
            numArray[index2] = index3;
            ++index2;
          }
          else
            numArray[index2] = -1;
        }
        if (index2 == 0)
          break;
        stringBuilder.Clear();
        stringBuilder.Append(nameof (GorillaMaterialReaction));
        stringBuilder.Append(": Removed null references in array `");
        stringBuilder.Append("_statusEffectReactions");
        stringBuilder.Append("[").Append(index1).Append("].").Append("gameObjectStates");
        stringBuilder.Append("' at indexes: ");
        stringBuilder.AppendJoin<int>(", ", (IEnumerable<int>) numArray);
        stringBuilder.Append(".");
        Debug.LogError((object) stringBuilder.ToString(), (UnityEngine.Object) this);
        GorillaMaterialReaction.GameObjectStates[] gameObjectStatesArray = new GorillaMaterialReaction.GameObjectStates[gameObjectStates.Length - index2];
        int num = 0;
        for (int index4 = 0; index4 < gameObjectStates.Length; ++index4)
        {
          if (!((UnityEngine.Object) gameObjectStates[index4].gameObject == (UnityEngine.Object) null))
            gameObjectStatesArray[num++] = gameObjectStates[index4];
        }
        this._statusEffectReactions[index1].gameObjectStates = gameObjectStatesArray;
      }
    }
  }

  [Serializable]
  public struct ReactionEntry
  {
    [Tooltip("If any of these statuses are true then this reaction will be executed.")]
    public int[] statusMaterialIndexes;
    public GorillaMaterialReaction.GameObjectStates[] gameObjectStates;
  }

  [Serializable]
  public struct GameObjectStates
  {
    public GameObject gameObject;
    [GorillaMaterialReaction.MomentInState]
    public GorillaMaterialReaction.MomentInStateActiveOption onEnter;
    [GorillaMaterialReaction.MomentInState]
    public GorillaMaterialReaction.MomentInStateActiveOption onStay;
    [GorillaMaterialReaction.MomentInState]
    public GorillaMaterialReaction.MomentInStateActiveOption onExit;
  }

  [Serializable]
  public struct MomentInStateActiveOption
  {
    public bool change;
    public bool activeState;
  }

  public enum EMomentInState
  {
    OnEnter,
    OnStay,
    OnExit,
  }

  public class MomentInStateAttribute : Attribute
  {
  }
}
