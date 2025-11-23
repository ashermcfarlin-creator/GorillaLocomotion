// Decompiled with JetBrains decompiler
// Type: GorillaTag.RigidbodyHighlighter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaExtensions;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag;

public class RigidbodyHighlighter : MonoBehaviour
{
  [CanBeNull]
  public static RigidbodyHighlighter Instance;
  [SerializeField]
  private float _inGameDuration = 10f;
  [SerializeField]
  private LineRenderer _lineRenderer;
  [SerializeField]
  private float _lineWidth = 0.01f;
  [SerializeField]
  private Vector3 _tracerOffset = 0.5f * Vector3.down;
  private readonly List<Rigidbody> _rigidbodies = new List<Rigidbody>();

  private string ButtonText => !this.Active ? "Highlight Rigidbodies" : "Unhighlight Rigidbodies";

  public bool Active { get; set; }

  private void Awake()
  {
    UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    if ((UnityEngine.Object) RigidbodyHighlighter.Instance != (UnityEngine.Object) null && (UnityEngine.Object) RigidbodyHighlighter.Instance != (UnityEngine.Object) this)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    RigidbodyHighlighter.Instance = this;
    this._lineRenderer.startWidth = this._lineWidth;
    this._lineRenderer.endWidth = this._lineWidth;
  }

  private void Update()
  {
    if (!this.Active)
    {
      this._lineRenderer.positionCount = 0;
    }
    else
    {
      this._rigidbodies.Clear();
      this._rigidbodies.AddAll<Rigidbody>((IEnumerable<Rigidbody>) RigidbodyHighlighter.GetAwakeRigidbodies());
      this.DrawTracers();
      foreach (Component rigidbody in this._rigidbodies)
        RigidbodyHighlighter.DrawBox(rigidbody.transform, Color.red, 0.1f);
    }
  }

  private static List<Rigidbody> GetAwakeRigidbodies()
  {
    List<Rigidbody> awakeRigidbodies = new List<Rigidbody>();
    foreach (UnityEngine.Object @object in UnityEngine.Object.FindObjectsByType(typeof (Rigidbody), FindObjectsSortMode.None))
    {
      if (!(@object is Rigidbody rigidbody))
        throw new Exception("Non-rigidbody found by FindObjectsByType.");
      if (!rigidbody.IsSleeping())
        awakeRigidbodies.Add(rigidbody);
    }
    return awakeRigidbodies;
  }

  private void HighlightActiveRigidbodies() => this.Active = !this.Active;

  private void GetRigidbodyNames()
  {
    List<Rigidbody> rigidbodyList = this._rigidbodies.Count > 0 ? this._rigidbodies : RigidbodyHighlighter.GetAwakeRigidbodies();
    for (int index = 0; index < rigidbodyList.Count; ++index)
      Debug.Log((object) $"Rigidbody {index} of {rigidbodyList.Count}: {rigidbodyList[index].name}");
  }

  private void OnDrawGizmos()
  {
    if (!this.Active)
      return;
    Gizmos.color = Color.red;
    foreach (Component rigidbody in this._rigidbodies)
      Gizmos.DrawWireCube(rigidbody.transform.position, Vector3.one);
  }

  private static void DrawBox(Transform tx, Color color, float duration)
  {
    Matrix4x4 matrix4x4 = new Matrix4x4();
    matrix4x4.SetTRS(tx.position, tx.rotation, tx.lossyScale);
    Vector3 vector3_1 = matrix4x4.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));
    Vector3 vector3_2 = matrix4x4.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
    Vector3 vector3_3 = matrix4x4.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));
    Vector3 vector3_4 = matrix4x4.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
    Vector3 vector3_5 = matrix4x4.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
    Vector3 vector3_6 = matrix4x4.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
    Vector3 vector3_7 = matrix4x4.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
    Vector3 vector3_8 = matrix4x4.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
    Debug.DrawLine(vector3_1, vector3_2, color, duration, false);
    Debug.DrawLine(vector3_2, vector3_4, color, duration, false);
    Debug.DrawLine(vector3_4, vector3_3, color, duration, false);
    Debug.DrawLine(vector3_3, vector3_1, color, duration, false);
    Debug.DrawLine(vector3_8, vector3_7, color, duration, false);
    Debug.DrawLine(vector3_7, vector3_5, color, duration, false);
    Debug.DrawLine(vector3_5, vector3_6, color, duration, false);
    Debug.DrawLine(vector3_6, vector3_8, color, duration, false);
    Debug.DrawLine(vector3_1, vector3_5, color, duration, false);
    Debug.DrawLine(vector3_2, vector3_6, color, duration, false);
    Debug.DrawLine(vector3_3, vector3_7, color, duration, false);
    Debug.DrawLine(vector3_4, vector3_8, color, duration, false);
  }

  private void DrawTracers()
  {
    Vector3[] positions = new Vector3[this._rigidbodies.Count * 2 + 1];
    for (int index = 0; index < positions.Length; ++index)
      positions[index] = index % 2 == 0 ? Camera.main.transform.position + this._tracerOffset : this._rigidbodies[index / 2].transform.position;
    this._lineRenderer.positionCount = positions.Length;
    this._lineRenderer.SetPositions(positions);
  }
}
