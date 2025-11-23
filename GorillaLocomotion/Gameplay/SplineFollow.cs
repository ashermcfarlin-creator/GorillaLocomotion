// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.SplineFollow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Photon.Pun;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

#nullable disable
namespace GorillaLocomotion.Gameplay;

public sealed class SplineFollow : MonoBehaviour
{
  [SerializeField]
  [Tooltip("If true, approximates the spline position. Only use when exact position does not matter.")]
  private bool _approximate;
  [SerializeField]
  private SplineContainer _unitySpline;
  [SerializeField]
  private float _duration;
  private double _secondsToCycles;
  [SerializeField]
  private float _smoothRotationTrackingRate = 0.5f;
  private float _smoothRotationTrackingRateExp;
  private float _progressPerFixedUpdate;
  [SerializeField]
  private float _splineProgressOffset;
  [SerializeField]
  private Quaternion _rotationFix = Quaternion.identity;
  private NativeSpline _nativeSpline;
  private float _progress;
  [Header("Approximate Spline Parameters")]
  [SerializeField]
  [Range(4f, 200f)]
  private int _approximationResolution = 100;
  private readonly List<SplineFollow.SplineNode> _approximationNodes = new List<SplineFollow.SplineNode>();

  public void Start()
  {
    this.transform.rotation *= this._rotationFix;
    this._smoothRotationTrackingRateExp = Mathf.Exp(this._smoothRotationTrackingRate);
    this._progress = this._splineProgressOffset;
    this._progressPerFixedUpdate = Time.fixedDeltaTime / this._duration;
    this._secondsToCycles = 1.0 / (double) this._duration;
    this._nativeSpline = new NativeSpline((ISpline) this._unitySpline.Spline, (float4x4) this._unitySpline.transform.localToWorldMatrix, Allocator.Persistent);
    if (!this._approximate)
      return;
    this.CalculateApproximationNodes();
  }

  private void CalculateApproximationNodes()
  {
    for (int index = 0; index < this._approximationResolution; ++index)
    {
      float3 position;
      float3 tangent;
      float3 upVector;
      this._nativeSpline.Evaluate<NativeSpline>((float) index / (float) this._approximationResolution, out position, out tangent, out upVector);
      this._approximationNodes.Add(new SplineFollow.SplineNode((Vector3) position, (Vector3) tangent, (Vector3) upVector));
    }
    if (!this._nativeSpline.Closed)
      return;
    this._approximationNodes.Add(this._approximationNodes[0]);
  }

  private void FixedUpdate()
  {
    if (this._approximate)
      return;
    this.FollowSpline();
  }

  private void Update()
  {
    if (!this._approximate)
      return;
    this.FollowSpline();
  }

  private void FollowSpline()
  {
    this._progress = !PhotonNetwork.InRoom ? (float) (((double) this._progress + (double) this._progressPerFixedUpdate) % 1.0) : (float) ((PhotonNetwork.Time * this._secondsToCycles + (double) this._splineProgressOffset) % 1.0);
    SplineFollow.SplineNode spline = this.EvaluateSpline(this._progress);
    this.transform.position = spline.Position;
    this.transform.rotation = Quaternion.Slerp(Quaternion.LookRotation(spline.Tangent) * this._rotationFix, this.transform.rotation, Mathf.Exp(-this._smoothRotationTrackingRateExp * Time.deltaTime));
  }

  private SplineFollow.SplineNode EvaluateSpline(float t)
  {
    t %= 1f;
    if (this._approximate)
    {
      int num;
      float t1 = (float) (num = (int) ((double) t * (double) this._approximationNodes.Count)) - (float) num;
      int index = num % this._approximationNodes.Count;
      return SplineFollow.SplineNode.Lerp(this._approximationNodes[index], this._approximationNodes[(index + 1) % this._approximationNodes.Count], t1);
    }
    float3 position;
    float3 tangent;
    float3 upVector;
    this._nativeSpline.Evaluate<NativeSpline>(t, out position, out tangent, out upVector);
    return new SplineFollow.SplineNode((Vector3) position, (Vector3) tangent, (Vector3) upVector);
  }

  private void OnDestroy() => this._nativeSpline.Dispose();

  private struct SplineNode
  {
    public readonly Vector3 Position;
    public readonly Vector3 Tangent;
    public readonly Vector3 Up;

    public SplineNode(Vector3 position, Vector3 tangent, Vector3 up)
    {
      Vector3 vector3_1 = position;
      Vector3 vector3_2 = tangent;
      Vector3 vector3_3 = up;
      this.Position = vector3_1;
      this.Tangent = vector3_2;
      this.Up = vector3_3;
    }

    public static SplineFollow.SplineNode Lerp(
      SplineFollow.SplineNode a,
      SplineFollow.SplineNode b,
      float t)
    {
      return new SplineFollow.SplineNode(Vector3.Lerp(a.Position, b.Position, t), Vector3.Lerp(a.Tangent, b.Tangent, t), Vector3.Lerp(a.Up, b.Up, t));
    }
  }
}
