// Decompiled with JetBrains decompiler
// Type: Liv.Lck.GorillaTag.SocialCoconutCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Liv.Lck.GorillaTag;

public class SocialCoconutCamera : MonoBehaviour
{
  [SerializeField]
  private GameObject _visuals;
  [SerializeField]
  private MeshRenderer _bodyRenderer;
  private bool _isActive;
  private MaterialPropertyBlock _propertyBlock;
  private string IS_RECORDING = "_Is_Recording";

  private void Awake()
  {
    if (this._propertyBlock == null)
      this._propertyBlock = new MaterialPropertyBlock();
    this._propertyBlock.SetInt(this.IS_RECORDING, 0);
    this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
  }

  public void SetVisualsActive(bool active)
  {
    this._isActive = active;
    this._visuals.SetActive(active);
  }

  public void SetRecordingState(bool isRecording)
  {
    if (!this._isActive)
      return;
    this._propertyBlock.SetInt(this.IS_RECORDING, isRecording ? 1 : 0);
    this._bodyRenderer.SetPropertyBlock(this._propertyBlock);
  }
}
