// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ContinuousProperty
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#nullable disable
namespace GorillaTag.Cosmetics;

[Serializable]
public class ContinuousProperty
{
  [SerializeField]
  private ContinuousPropertyModeSO mode;
  [FormerlySerializedAs("component")]
  [SerializeField]
  protected UnityEngine.Object target;
  [SerializeField]
  private Gradient color;
  [SerializeField]
  private AnimationCurve curve = AnimationCurves.Linear;
  [FormerlySerializedAs("materialIndex")]
  [SerializeField]
  private int intValue;
  [SerializeField]
  private string stringValue;
  [SerializeField]
  private BezierCurve bezierCurve;
  private const string ENUM_ERROR = "Internal values were changed at some point. Please select a new value.";
  [SerializeField]
  private ContinuousProperty.RotationAxis localAxis = ContinuousProperty.RotationAxis.X;
  [SerializeField]
  private ContinuousProperty.InterpolationMode interpolationMode = ContinuousProperty.InterpolationMode.PositionAndRotation;
  [SerializeField]
  private ParticleSystemStopBehavior stopType = ParticleSystemStopBehavior.StopEmitting;
  [SerializeField]
  private Transform transformA;
  [SerializeField]
  private Transform transformB;
  [SerializeField]
  private XformOffset offsetA;
  [SerializeField]
  private XformOffset offsetB;
  [SerializeField]
  private Vector2 range = new Vector2(0.5f, 1f);
  [SerializeField]
  private ContinuousProperty.ThresholdOption thresholdOption = ContinuousProperty.ThresholdOption.Normal;
  [SerializeField]
  private ContinuousProperty.EventMode eventMode = ContinuousProperty.EventMode.Passthrough;
  [SerializeField]
  private UnityEvent<float> unityEvent;
  [Tooltip("Check this box if only the owner/local player is supposed to run this property.")]
  [SerializeField]
  private bool runOnlyLocally;
  private bool rigLocal;
  private int internalSwitchValue;
  private ParticleSystem.MainModule particleMain;
  private ParticleSystem.EmissionModule particleEmission;
  private ParticleSystem.MinMaxCurve speedCurveCache;
  private ParticleSystem.MinMaxCurve rateCurveCache;
  private float frequencyTimer;
  private bool previousBoolValue;
  private int stringHash;

  private static ContinuousProperty.Cast GetTargetCast(UnityEngine.Object o)
  {
    ContinuousProperty.Cast targetCast;
    switch (o)
    {
      case ParticleSystem _:
        targetCast = ContinuousProperty.Cast.ParticleSystem;
        break;
      case SkinnedMeshRenderer _:
        targetCast = ContinuousProperty.Cast.SkinnedMeshRenderer;
        break;
      case Animator _:
        targetCast = ContinuousProperty.Cast.Animator;
        break;
      case AudioSource _:
        targetCast = ContinuousProperty.Cast.AudioSource;
        break;
      case VoicePitchShiftCosmetic _:
        targetCast = ContinuousProperty.Cast.VoicePitchShiftCosmetic;
        break;
      case Rigidbody _:
        targetCast = ContinuousProperty.Cast.Rigidbody;
        break;
      case Transform _:
        targetCast = ContinuousProperty.Cast.Transform;
        break;
      case Renderer _:
        targetCast = ContinuousProperty.Cast.Renderer;
        break;
      case Behaviour _:
        targetCast = ContinuousProperty.Cast.Behaviour;
        break;
      case GameObject _:
        targetCast = ContinuousProperty.Cast.GameObject;
        break;
      default:
        targetCast = ContinuousProperty.Cast.Null;
        break;
    }
    return targetCast;
  }

  public static bool CastMatches(ContinuousProperty.Cast cast, ContinuousProperty.Cast test)
  {
    bool flag;
    switch (cast)
    {
      case ContinuousProperty.Cast.Null:
        flag = false;
        break;
      case ContinuousProperty.Cast.Any:
        flag = true;
        break;
      case ContinuousProperty.Cast.Renderer:
        flag = test == ContinuousProperty.Cast.Renderer || test == ContinuousProperty.Cast.SkinnedMeshRenderer;
        break;
      case ContinuousProperty.Cast.Behaviour:
        flag = test != ContinuousProperty.Cast.Transform && test != ContinuousProperty.Cast.GameObject && test != ContinuousProperty.Cast.Rigidbody;
        break;
      default:
        flag = test == cast;
        break;
    }
    return flag;
  }

  public static bool HasAllFlags(
    ContinuousProperty.DataFlags flags,
    ContinuousProperty.DataFlags test)
  {
    return (flags & test) == test;
  }

  public static bool HasAnyFlag(
    ContinuousProperty.DataFlags flags,
    ContinuousProperty.DataFlags test)
  {
    return (flags & test) != 0;
  }

  private static void GetAllValidObjectsNonAlloc(Transform t, List<UnityEngine.Object> objects)
  {
    objects.Clear();
    objects.Add((UnityEngine.Object) t.gameObject);
    foreach (UnityEngine.Object component in t.GetComponents<Component>())
    {
      if (ContinuousProperty.IsValidObject(component.GetType()))
        objects.Add(component);
    }
  }

  private static bool IsValidObject(System.Type t)
  {
    return t != typeof (Renderer) && t != typeof (ParticleSystemRenderer);
  }

  public ContinuousProperty()
  {
  }

  public ContinuousProperty(ContinuousPropertyModeSO mode, Transform initialTarget, Vector2 range = default (Vector2))
  {
    this.mode = mode;
    this.target = (UnityEngine.Object) initialTarget;
    this.range = range;
    this.ShiftTarget(0);
  }

  private string ModeTooltip
  {
    get
    {
      return !(bool) (UnityEngine.Object) this.mode ? "" : $"{this.mode.type}: {this.mode.GetDescriptionForCast(ContinuousProperty.GetTargetCast(this.target))}";
    }
  }

  private bool ModeInfoVisible => (UnityEngine.Object) this.mode == (UnityEngine.Object) null;

  private bool ModeErrorVisible => !this.IsValid();

  private string ModeErrorMessage
  {
    get
    {
      return !((UnityEngine.Object) this.mode != (UnityEngine.Object) null) ? "How did we get here?" : $"I couldn't find any valid target to apply my '{this.mode.name}' to in the whole prefab.\n\n{this.mode.ListValidCasts()}";
    }
  }

  public ContinuousPropertyModeSO Mode => this.mode;

  public ContinuousProperty.Type MyType
  {
    get => !((UnityEngine.Object) this.mode != (UnityEngine.Object) null) ? ContinuousProperty.Type.Color : this.mode.type;
  }

  private bool HasTarget => this.MyType != ContinuousProperty.Type.UnityEvent;

  private bool TargetInfoVisible => this.HasTarget && this.target == (UnityEngine.Object) null;

  private string TargetTooltip
  {
    get => !((UnityEngine.Object) this.mode != (UnityEngine.Object) null) ? "" : this.mode.ListValidCasts();
  }

  private bool ShiftButtonsVisible => (UnityEngine.Object) this.mode != (UnityEngine.Object) null;

  public UnityEngine.Object Target => this.target;

  private void PreviousTarget() => this.ShiftTarget(-1);

  private void NextTarget() => this.ShiftTarget(1);

  public bool ShiftTarget(int shiftAmount)
  {
    if ((UnityEngine.Object) this.mode == (UnityEngine.Object) null)
      return false;
    int num = -1;
    Transform transform1 = this.target != (UnityEngine.Object) null ? (this.target is GameObject target ? target.transform : (Transform) null) ?? ((Component) this.target).transform : (Transform) null;
    Transform transform2 = transform1;
    if ((UnityEngine.Object) transform2 == (UnityEngine.Object) null)
      return false;
    Stack<Transform> transformStack = new Stack<Transform>();
    transformStack.Push(transform2);
    List<UnityEngine.Object> objectList = new List<UnityEngine.Object>();
    List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
    Transform t;
    while (transformStack.TryPop(ref t))
    {
      if (num < 0 && (UnityEngine.Object) t == (UnityEngine.Object) transform1)
        num = objectList.Count;
      ContinuousProperty.GetAllValidObjectsNonAlloc(t, objects);
      foreach (UnityEngine.Object o in objects)
      {
        if (this.mode.IsCastValid(ContinuousProperty.GetTargetCast(o)))
        {
          if (o == this.target)
            num = objectList.Count;
          objectList.Add(o);
        }
      }
      for (int index = t.childCount - 1; index >= 0; --index)
        transformStack.Push(t.GetChild(index));
    }
    if (objectList.Count == 0)
      return false;
    this.target = objectList[num < 0 ? 0 : (num + shiftAmount + objectList.Count) % objectList.Count];
    return true;
  }

  private void OnModeOrTargetChanged()
  {
    if (this.IsValid())
      return;
    this.ShiftTarget(0);
  }

  public bool IsShaderProperty_Cached { get; private set; }

  public bool UsesThreshold_Cached { get; private set; }

  public bool IsValid()
  {
    return (UnityEngine.Object) this.mode == (UnityEngine.Object) null || this.target == (UnityEngine.Object) null || this.mode.IsCastValid(ContinuousProperty.GetTargetCast(this.target));
  }

  public int GetTargetInstanceID() => this.target.GetInstanceID();

  private bool HasAllFlags(ContinuousProperty.DataFlags test)
  {
    return (UnityEngine.Object) this.mode != (UnityEngine.Object) null && ContinuousProperty.HasAllFlags(this.mode.GetFlagsForClosestCast(ContinuousProperty.GetTargetCast(this.target)), test);
  }

  private bool HasAnyFlag(ContinuousProperty.DataFlags test)
  {
    return (UnityEngine.Object) this.mode != (UnityEngine.Object) null && ContinuousProperty.HasAnyFlag(this.mode.GetFlagsForClosestCast(ContinuousProperty.GetTargetCast(this.target)), test);
  }

  private bool HasGradient => this.HasAllFlags(ContinuousProperty.DataFlags.HasColor);

  private bool HasCurve => this.HasAllFlags(ContinuousProperty.DataFlags.HasCurve);

  private string DynamicIntLabel()
  {
    if (!this.HasAllFlags(ContinuousProperty.DataFlags.IsShaderProperty))
    {
      switch (this.MyType)
      {
        case ContinuousProperty.Type.Color:
        case ContinuousProperty.Type.BlendShape:
          break;
        default:
          return "Int Value";
      }
    }
    return "Material Index";
  }

  private bool HasInt => this.HasAllFlags(ContinuousProperty.DataFlags.HasInteger);

  public int IntValue => this.intValue;

  private string DynamicStringLabel()
  {
    if (this.HasAllFlags(ContinuousProperty.DataFlags.IsShaderProperty))
      return "Property Name";
    return this.HasAllFlags(ContinuousProperty.DataFlags.IsAnimatorParameter) ? "Parameter Name" : "String Value";
  }

  private bool HasString
  {
    get
    {
      return this.HasAnyFlag(ContinuousProperty.DataFlags.IsShaderProperty | ContinuousProperty.DataFlags.IsAnimatorParameter);
    }
  }

  public string StringValue => this.stringValue;

  private bool HasBezier => this.MyType == ContinuousProperty.Type.BezierInterpolation;

  private bool MissingBezier => (UnityEngine.Object) this.bezierCurve == (UnityEngine.Object) null;

  private bool AxisError
  {
    get => !Enum.IsDefined(typeof (ContinuousProperty.RotationAxis), (object) this.localAxis);
  }

  private bool HasAxisMode => this.HasAllFlags(ContinuousProperty.DataFlags.HasAxis);

  private bool InterpolationError
  {
    get
    {
      return !Enum.IsDefined(typeof (ContinuousProperty.InterpolationMode), (object) this.interpolationMode);
    }
  }

  private bool HasInterpolationMode
  {
    get => this.HasAllFlags(ContinuousProperty.DataFlags.HasInterpolation);
  }

  private bool HasStopAction
  {
    get => this.MyType == ContinuousProperty.Type.PlayStop && this.target is ParticleSystem;
  }

  private bool HasXforms => this.MyType == ContinuousProperty.Type.TransformInterpolation;

  private bool MissingXforms
  {
    get => (UnityEngine.Object) this.transformA == (UnityEngine.Object) null || (UnityEngine.Object) this.transformB == (UnityEngine.Object) null;
  }

  private bool HasOffsets => this.MyType == ContinuousProperty.Type.OffsetInterpolation;

  private string ThresholdErrorMessage
  {
    get
    {
      return "The threshold will always be " + (this.thresholdOption == ContinuousProperty.ThresholdOption.Normal ^ (double) this.range.x >= (double) this.range.y ? "true." : "false.");
    }
  }

  private string ThresholdTooltip
  {
    get
    {
      return !this.ThresholdError ? $"The threshold will be true{(this.thresholdOption == ContinuousProperty.ThresholdOption.Normal ? ((double) this.range.x <= 0.0 || (double) this.range.y >= 1.0 ? ((double) this.range.x > 0.0 ? " above " + this.range.x.ToString() : " below " + this.range.y.ToString()) : $" between {this.range.x} and {this.range.y}") : ((double) this.range.x > 0.0 ? " below " + this.range.x.ToString() : "") + ((double) this.range.x <= 0.0 || (double) this.range.y >= 1.0 ? "" : " and") + ((double) this.range.y < 1.0 ? " above " + this.range.y.ToString() : ""))}, and false otherwise." : this.ThresholdErrorMessage;
    }
  }

  private bool HasThreshold => this.HasAllFlags(ContinuousProperty.DataFlags.HasThreshold);

  private bool ThresholdError
  {
    get
    {
      return (double) this.range.x <= 0.0 && (double) this.range.y >= 1.0 || (double) this.range.x >= (double) this.range.y;
    }
  }

  private bool HasEventMode
  {
    get
    {
      return this.MyType == ContinuousProperty.Type.UnityEvent && !this.HasAnyFlag(ContinuousProperty.DataFlags.HasThreshold);
    }
  }

  private bool HasUnityEvent => this.MyType == ContinuousProperty.Type.UnityEvent;

  public bool RunOnlyLocally => this.runOnlyLocally;

  public void SetRigIsLocal(bool v) => this.rigLocal = v;

  public void Init()
  {
    if ((UnityEngine.Object) this.mode == (UnityEngine.Object) null)
    {
      this.internalSwitchValue = 0;
    }
    else
    {
      ContinuousProperty.Type type = this.mode.type;
      ContinuousProperty.Cast cast = this.mode.GetClosestCast(ContinuousProperty.GetTargetCast(this.target));
      ContinuousProperty.DataFlags flagsForCast = this.mode.GetFlagsForCast(cast);
      if (cast == ContinuousProperty.Cast.Null || type == ContinuousProperty.Type.BezierInterpolation && this.MissingBezier || type == ContinuousProperty.Type.TransformInterpolation && this.MissingXforms || type == ContinuousProperty.Type.UnityEvent && this.unityEvent == null)
      {
        this.internalSwitchValue = 0;
        this.IsShaderProperty_Cached = false;
        this.UsesThreshold_Cached = false;
      }
      else
      {
        if (type == ContinuousProperty.Type.Color && ContinuousProperty.CastMatches(ContinuousProperty.Cast.Renderer, cast))
        {
          type = ContinuousProperty.Type.ShaderColor;
          cast = ContinuousProperty.Cast.Renderer;
          flagsForCast |= ContinuousProperty.DataFlags.IsShaderProperty;
          this.stringValue = "_BaseColor";
        }
        else if (type == ContinuousProperty.Type.PlayStop && cast == ContinuousProperty.Cast.Animator)
        {
          type = ContinuousProperty.Type.EnableDisable;
          cast = ContinuousProperty.Cast.Behaviour;
        }
        this.internalSwitchValue = (int) (type | (ContinuousProperty.Type) cast | (this.HasAxisMode ? (ContinuousProperty.Type) this.localAxis : ContinuousProperty.Type.Color) | (this.HasInterpolationMode ? (ContinuousProperty.Type) this.interpolationMode : ContinuousProperty.Type.Color) | (this.HasEventMode ? (ContinuousProperty.Type) this.eventMode : ContinuousProperty.Type.Color));
        this.IsShaderProperty_Cached = ContinuousProperty.HasAllFlags(flagsForCast, ContinuousProperty.DataFlags.IsShaderProperty);
        this.UsesThreshold_Cached = ContinuousProperty.HasAllFlags(flagsForCast, ContinuousProperty.DataFlags.HasThreshold);
        if (cast == ContinuousProperty.Cast.ParticleSystem)
        {
          this.particleMain = ((ParticleSystem) this.target).main;
          this.particleEmission = ((ParticleSystem) this.target).emission;
          this.speedCurveCache = this.particleMain.startSpeed;
          this.rateCurveCache = this.particleEmission.rateOverTime;
        }
        if (this.IsShaderProperty_Cached)
          this.stringHash = Shader.PropertyToID(this.stringValue);
        else if (ContinuousProperty.HasAllFlags(flagsForCast, ContinuousProperty.DataFlags.IsAnimatorParameter))
          this.stringHash = Animator.StringToHash(this.stringValue);
        if (ContinuousProperty.HasAnyFlag(flagsForCast, ContinuousProperty.DataFlags.HasCurve))
          return;
        this.curve = AnimationCurves.Linear;
      }
    }
  }

  public void InitThreshold()
  {
    if (!this.UsesThreshold_Cached)
      return;
    int num = (int) this.CheckThreshold(0.0f);
    if (this.IsShaderProperty_Cached)
      return;
    this.previousBoolValue = !this.previousBoolValue;
    this.Apply(0.0f, 0.0f, (MaterialPropertyBlock) null);
  }

  public void Apply(float f, float deltaTime, MaterialPropertyBlock mpb)
  {
    if (this.runOnlyLocally && !this.rigLocal)
      return;
    int num1 = (int) ((ContinuousProperty.ThresholdResult) this.internalSwitchValue | this.CheckThreshold(f));
    if (num1 <= 1057808)
    {
      if (num1 <= 6157)
      {
        if (num1 <= 3083)
        {
          if (num1 <= 2049)
          {
            if (num1 == 0 || num1 != 2049)
              return;
            ((Transform) this.target).localScale = this.curve.Evaluate(f) * Vector3.one;
            return;
          }
          if (num1 != 3072 /*0x0C00*/)
          {
            if (num1 != 3073)
            {
              if (num1 != 3083)
                return;
              this.particleMain.startSpeed = this.ScaleCurve(in this.speedCurveCache, this.curve.Evaluate(f));
              return;
            }
            this.particleMain.startSize = (ParticleSystem.MinMaxCurve) this.curve.Evaluate(f);
            return;
          }
          this.particleMain.startColor = (ParticleSystem.MinMaxGradient) this.color.Evaluate(f);
          return;
        }
        if (num1 <= 4098)
        {
          if (num1 != 3084)
          {
            if (num1 != 4098)
              return;
            ((SkinnedMeshRenderer) this.target).SetBlendShapeWeight(this.intValue, this.curve.Evaluate(f) * 100f);
            return;
          }
          this.particleEmission.rateOverTime = this.ScaleCurve(in this.rateCurveCache, this.curve.Evaluate(f));
          return;
        }
        if (num1 != 5123)
        {
          if (num1 != 5131)
          {
            if (num1 != 6157)
              return;
            ((AudioSource) this.target).volume = Mathf.Clamp01(this.curve.Evaluate(f));
            return;
          }
          ((Animator) this.target).speed = this.curve.Evaluate(f);
          return;
        }
        ((Animator) this.target).SetFloat(this.stringHash, this.curve.Evaluate(f));
        return;
      }
      if (num1 <= 1051663)
      {
        if (num1 <= 7173)
        {
          if (num1 != 6158)
          {
            switch (num1 - 7171)
            {
              case 0:
                mpb.SetFloat(this.stringHash, this.curve.Evaluate(f));
                return;
              case 1:
                mpb.SetVector(this.stringHash, (Vector4) new Vector2(this.curve.Evaluate(f), 0.0f));
                return;
              case 2:
                mpb.SetColor(this.stringHash, this.color.Evaluate(f));
                return;
              default:
                return;
            }
          }
          else
          {
            ((AudioSource) this.target).pitch = Mathf.Clamp(this.curve.Evaluate(f), -3f, 3f);
            return;
          }
        }
        else
        {
          if (num1 != 11278)
          {
            if (num1 != 1049617)
            {
              if (num1 != 1051663)
                return;
              ((ParticleSystem) this.target).Play();
              return;
            }
            this.unityEvent.Invoke(this.curve.Evaluate(f));
            return;
          }
          ((VoicePitchShiftCosmetic) this.target).Pitch = this.curve.Evaluate(f);
          return;
        }
      }
      else if (num1 <= 1054735)
      {
        if (num1 != 1053706)
        {
          if (num1 != 1053714)
          {
            if (num1 != 1054735)
              return;
            ((AudioSource) this.target).Play();
            return;
          }
          ((Animator) this.target).SetTrigger(this.stringHash);
          return;
        }
      }
      else
      {
        if (num1 != 1055760)
        {
          if (num1 != 1056784)
          {
            if (num1 != 1057808)
              return;
            goto label_98;
          }
          goto label_97;
        }
        goto label_96;
      }
    }
    else if (num1 <= 3150858)
    {
      if (num1 <= 2103311)
      {
        if (num1 <= 2100239)
        {
          if (num1 == 2098193 || num1 != 2100239)
            return;
          ((ParticleSystem) this.target).Stop(true, this.stopType);
          return;
        }
        if (num1 != 2102282)
        {
          if (num1 == 2102290 || num1 != 2103311)
            return;
          ((AudioSource) this.target).Stop();
          return;
        }
      }
      else
      {
        if (num1 <= 2106384)
        {
          if (num1 != 2104336)
          {
            if (num1 != 2105360)
            {
              if (num1 != 2106384)
                return;
              goto label_98;
            }
            goto label_97;
          }
          goto label_96;
        }
        if (num1 == 3146769 || num1 == 3148815)
          ;
        return;
      }
    }
    else
    {
      if (num1 <= 3154960)
      {
        if (num1 <= 3151887)
        {
          if (num1 == 3150866)
            ;
          return;
        }
        if (num1 == 3152912 || num1 == 3153936)
          ;
        return;
      }
      if (num1 <= 8389649)
      {
        if (num1 != 4195345)
        {
          switch (num1 - 4196358)
          {
            case 0:
              ((Transform) this.target).position = this.bezierCurve.GetPoint(this.curve.Evaluate(f));
              return;
            case 1:
              ((Transform) this.target).localRotation = Quaternion.Euler(this.curve.Evaluate(f) * 360f, 0.0f, 0.0f);
              return;
            case 2:
              ((Transform) this.target).position = Vector3.Lerp(this.transformA.position, this.transformB.position, this.curve.Evaluate(f));
              return;
            case 3:
              ((Transform) this.target).localPosition = Vector3.Lerp(this.offsetA.pos, this.offsetB.pos, this.curve.Evaluate(f));
              return;
            default:
              if (num1 != 8389649)
                return;
              float num2 = this.curve.Evaluate(f);
              float length = 1f / num2;
              this.frequencyTimer += deltaTime;
              if ((double) this.frequencyTimer < (double) length)
                return;
              this.frequencyTimer = Mathf.Repeat(this.frequencyTimer - length, length);
              this.unityEvent.Invoke(num2);
              return;
          }
        }
        else
        {
          this.unityEvent.Invoke(this.curve.Evaluate(f));
          return;
        }
      }
      else
      {
        switch (num1 - 8390662)
        {
          case 0:
            ((Transform) this.target).rotation = Quaternion.LookRotation(this.bezierCurve.GetDirection(this.curve.Evaluate(f)));
            return;
          case 1:
            ((Transform) this.target).localRotation = Quaternion.Euler(0.0f, this.curve.Evaluate(f) * 360f, 0.0f);
            return;
          case 2:
            ((Transform) this.target).rotation = Quaternion.Slerp(this.transformA.rotation, this.transformB.rotation, this.curve.Evaluate(f));
            return;
          case 3:
            ((Transform) this.target).localRotation = Quaternion.Slerp(this.offsetA.rot, this.offsetB.rot, this.curve.Evaluate(f));
            return;
          default:
            if (num1 != 12583953)
            {
              switch (num1 - 12584966)
              {
                case 0:
                  float t1 = this.curve.Evaluate(f);
                  ((Transform) this.target).SetPositionAndRotation(this.bezierCurve.GetPoint(t1), Quaternion.LookRotation(this.bezierCurve.GetDirection(t1)));
                  return;
                case 1:
                  ((Transform) this.target).localRotation = Quaternion.Euler(0.0f, 0.0f, this.curve.Evaluate(f) * 360f);
                  return;
                case 2:
                  Vector3 position1;
                  Quaternion rotation1;
                  this.transformA.GetPositionAndRotation(out position1, out rotation1);
                  Vector3 position2;
                  Quaternion rotation2;
                  this.transformB.GetPositionAndRotation(out position2, out rotation2);
                  float t2 = this.curve.Evaluate(f);
                  ((Transform) this.target).SetPositionAndRotation(Vector3.Lerp(position1, position2, t2), Quaternion.Slerp(rotation1, rotation2, t2));
                  return;
                case 3:
                  float t3 = this.curve.Evaluate(f);
                  ((Transform) this.target).SetLocalPositionAndRotation(Vector3.Lerp(this.offsetA.pos, this.offsetB.pos, t3), Quaternion.Slerp(this.offsetA.rot, this.offsetB.rot, t3));
                  return;
                default:
                  return;
              }
            }
            else
            {
              float num3 = this.curve.Evaluate(f);
              if ((double) UnityEngine.Random.value >= (double) (1f - Mathf.Exp(-num3 * deltaTime)))
                return;
              this.unityEvent.Invoke(num3);
              return;
            }
        }
      }
    }
    ((Animator) this.target).SetBool(this.stringHash, this.previousBoolValue);
    return;
label_96:
    ((Renderer) this.target).enabled = this.previousBoolValue;
    return;
label_97:
    ((Behaviour) this.target).enabled = this.previousBoolValue;
    return;
label_98:
    ((GameObject) this.target).SetActive(this.previousBoolValue);
  }

  private ParticleSystem.MinMaxCurve ScaleCurve(in ParticleSystem.MinMaxCurve inCurve, float scale)
  {
    ParticleSystem.MinMaxCurve minMaxCurve = inCurve;
    switch (minMaxCurve.mode)
    {
      case ParticleSystemCurveMode.Constant:
        minMaxCurve.constant *= scale;
        break;
      case ParticleSystemCurveMode.Curve:
      case ParticleSystemCurveMode.TwoCurves:
        minMaxCurve.curveMultiplier *= scale;
        break;
      case ParticleSystemCurveMode.TwoConstants:
        minMaxCurve.constantMin *= scale;
        minMaxCurve.constantMax *= scale;
        break;
    }
    return minMaxCurve;
  }

  private bool CheckContinuousEvent(float f, float deltaTime)
  {
    switch (this.eventMode)
    {
      case ContinuousProperty.EventMode.Passthrough:
        return true;
      case ContinuousProperty.EventMode.Frequency:
        this.frequencyTimer += deltaTime;
        if ((double) this.frequencyTimer < (double) f)
          return false;
        this.frequencyTimer = Mathf.Repeat(this.frequencyTimer - f, f);
        return true;
      case ContinuousProperty.EventMode.AveragePerSecond:
        return (double) UnityEngine.Random.value < (double) (1f - Mathf.Exp(-f * deltaTime));
      default:
        return false;
    }
  }

  private ContinuousProperty.ThresholdResult CheckThreshold(float f)
  {
    if (!this.UsesThreshold_Cached)
      return ContinuousProperty.ThresholdResult.Null;
    bool flag = (double) f >= (double) this.range.x && (double) f <= (double) this.range.y;
    if (!this.previousBoolValue && (this.thresholdOption == ContinuousProperty.ThresholdOption.Normal & flag || this.thresholdOption == ContinuousProperty.ThresholdOption.Invert && !flag))
    {
      this.previousBoolValue = true;
      return ContinuousProperty.ThresholdResult.RisingEdge;
    }
    if (!this.previousBoolValue || (this.thresholdOption != ContinuousProperty.ThresholdOption.Normal || flag) && !(this.thresholdOption == ContinuousProperty.ThresholdOption.Invert & flag))
      return ContinuousProperty.ThresholdResult.Unchanged;
    this.previousBoolValue = false;
    return ContinuousProperty.ThresholdResult.FallingEdge;
  }

  public enum Type
  {
    Color,
    Scale,
    BlendShape,
    Float,
    ShaderVector2_X,
    ShaderColor,
    BezierInterpolation,
    AxisAngle,
    TransformInterpolation,
    OffsetInterpolation,
    Boolean,
    Speed,
    Rate,
    Volume,
    Pitch,
    PlayStop,
    EnableDisable,
    UnityEvent,
    Trigger,
  }

  public enum Cast
  {
    Null = 0,
    Any = 1024, // 0x00000400
    Transform = 2048, // 0x00000800
    ParticleSystem = 3072, // 0x00000C00
    SkinnedMeshRenderer = 4096, // 0x00001000
    Animator = 5120, // 0x00001400
    AudioSource = 6144, // 0x00001800
    Renderer = 7168, // 0x00001C00
    Behaviour = 8192, // 0x00002000
    GameObject = 9216, // 0x00002400
    Rigidbody = 10240, // 0x00002800
    VoicePitchShiftCosmetic = 11264, // 0x00002C00
  }

  [Flags]
  public enum DataFlags
  {
    None = 0,
    [Tooltip("Expose the AnimationCurve for single values")] HasCurve = 1,
    [Tooltip("Expose the Gradient for colors")] HasColor = 2,
    [Tooltip("Select which axis it should rotate on")] HasAxis = 4,
    [Tooltip("Expose the integer, usually for material index")] HasInteger = 8,
    [Tooltip("Select whether to use position, rotation, or both when interpolating")] HasInterpolation = 16, // 0x00000010
    [Tooltip("Expose the string and hash it into a shader property ID")] IsShaderProperty = 32, // 0x00000020
    [Tooltip("Expose the string and hash it into an animator parameter ID")] IsAnimatorParameter = 64, // 0x00000040
    [Tooltip("Expose the threshold range as a dual slider")] HasThreshold = 128, // 0x00000080
  }

  private enum ThresholdResult
  {
    Null = 0,
    RisingEdge = 1048576, // 0x00100000
    FallingEdge = 2097152, // 0x00200000
    Unchanged = 3145728, // 0x00300000
  }

  private enum ThresholdOption
  {
    Invert,
    Normal,
  }

  private enum RotationAxis
  {
    X = 4194304, // 0x00400000
    Y = 8388608, // 0x00800000
    Z = 12582912, // 0x00C00000
  }

  public enum InterpolationMode
  {
    Position = 4194304, // 0x00400000
    Rotation = 8388608, // 0x00800000
    PositionAndRotation = 12582912, // 0x00C00000
  }

  public enum EventMode
  {
    Passthrough = 4194304, // 0x00400000
    Frequency = 8388608, // 0x00800000
    AveragePerSecond = 12582912, // 0x00C00000
  }
}
