// Decompiled with JetBrains decompiler
// Type: GorillaLocomotion.Gameplay.TraverseSpline
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Fusion;
using Photon.Pun;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable
namespace GorillaLocomotion.Gameplay;

[NetworkBehaviourWeaved(1)]
public class TraverseSpline : NetworkComponent
{
  public BezierSpline spline;
  public float duration = 30f;
  public float speedMultiplierWhileHeld = 2f;
  private float currentSpeedMultiplier;
  public float acceleration = 1f;
  public float deceleration = 1f;
  private bool isHeldByLocalPlayer;
  public bool lookForward = true;
  public SplineWalkerMode mode;
  [SerializeField]
  private float SplineProgressOffet;
  private float progress;
  private float progressLerpStart;
  private float progressLerpEnd;
  private const float progressLerpDuration = 1f;
  private float progressLerpStartTime;
  private bool goingForward = true;
  [SerializeField]
  private bool constantVelocity;
  [WeaverGenerated]
  [SerializeField]
  [DefaultForProperty("Data", 0, 1)]
  [DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
  private float _Data;

  protected override void Awake()
  {
    base.Awake();
    this.progress = this.SplineProgressOffet % 1f;
  }

  protected virtual void FixedUpdate()
  {
    if (!this.IsMine && (double) this.progressLerpStartTime + 1.0 > (double) Time.time)
    {
      this.progress = Mathf.Lerp(this.progressLerpStart, this.progressLerpEnd, (float) (((double) Time.time - (double) this.progressLerpStartTime) / 1.0));
    }
    else
    {
      this.currentSpeedMultiplier = !this.isHeldByLocalPlayer ? Mathf.MoveTowards(this.currentSpeedMultiplier, 1f, this.deceleration * Time.deltaTime) : Mathf.MoveTowards(this.currentSpeedMultiplier, this.speedMultiplierWhileHeld, this.acceleration * Time.deltaTime);
      if (this.goingForward)
      {
        this.progress += Time.deltaTime * this.currentSpeedMultiplier / this.duration;
        if ((double) this.progress > 1.0)
        {
          if (this.mode == SplineWalkerMode.Once)
            this.progress = 1f;
          else if (this.mode == SplineWalkerMode.Loop)
          {
            this.progress %= 1f;
          }
          else
          {
            this.progress = 2f - this.progress;
            this.goingForward = false;
          }
        }
      }
      else
      {
        this.progress -= Time.deltaTime * this.currentSpeedMultiplier / this.duration;
        if ((double) this.progress < 0.0)
        {
          this.progress = -this.progress;
          this.goingForward = true;
        }
      }
    }
    this.transform.position = this.spline.GetPoint(this.progress, this.constantVelocity);
    if (!this.lookForward)
      return;
    this.transform.LookAt(this.transform.position + this.spline.GetDirection(this.progress, this.constantVelocity));
  }

  [Networked]
  [NetworkedWeaved(0, 1)]
  public unsafe float Data
  {
    get
    {
      if ((IntPtr) this.Ptr == IntPtr.Zero)
        throw new InvalidOperationException("Error when accessing TraverseSpline.Data. Networked properties can only be accessed when Spawned() has been called.");
      return *(float*) (this.Ptr + 0);
    }
    set
    {
      if ((IntPtr) this.Ptr == IntPtr.Zero)
        throw new InvalidOperationException("Error when accessing TraverseSpline.Data. Networked properties can only be accessed when Spawned() has been called.");
      *(float*) (this.Ptr + 0) = value;
    }
  }

  public override void WriteDataFusion()
  {
    this.Data = this.progress + this.currentSpeedMultiplier * 1f / this.duration;
  }

  public override void ReadDataFusion()
  {
    this.progressLerpEnd = this.Data;
    this.ReadDataShared();
  }

  protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
  {
    stream.SendNext((object) (float) ((double) this.progress + (double) this.currentSpeedMultiplier * 1.0 / (double) this.duration));
  }

  protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
  {
    this.progressLerpEnd = (float) stream.ReceiveNext();
    this.ReadDataShared();
  }

  private void ReadDataShared()
  {
    if (float.IsNaN(this.progressLerpEnd) || float.IsInfinity(this.progressLerpEnd))
    {
      this.progressLerpEnd = 1f;
    }
    else
    {
      this.progressLerpEnd = Mathf.Abs(this.progressLerpEnd);
      if ((double) this.progressLerpEnd > 1.0)
        this.progressLerpEnd %= 1f;
    }
    this.progressLerpStart = (double) Mathf.Abs(this.progressLerpEnd - this.progress) > (double) Mathf.Abs(this.progressLerpEnd - (this.progress - 1f)) ? this.progress - 1f : this.progress;
    this.progressLerpStartTime = Time.time;
  }

  protected float GetProgress() => this.progress;

  public float GetCurrentSpeed() => this.currentSpeedMultiplier;

  [WeaverGenerated]
  public override void CopyBackingFieldsToState([In] bool obj0)
  {
    base.CopyBackingFieldsToState(obj0);
    this.Data = this._Data;
  }

  [WeaverGenerated]
  public override void CopyStateToBackingFields()
  {
    base.CopyStateToBackingFields();
    this._Data = this.Data;
  }
}
