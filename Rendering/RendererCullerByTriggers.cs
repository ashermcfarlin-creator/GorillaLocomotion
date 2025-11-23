// Decompiled with JetBrains decompiler
// Type: GorillaTag.Rendering.RendererCullerByTriggers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Rendering;

public class RendererCullerByTriggers : MonoBehaviour, IBuildValidation
{
  [Tooltip("These renderers will be enabled/disabled depending on if the main camera is the colliders.")]
  public Renderer[] renderers;
  public Collider[] colliders;
  private bool camWasTouching;
  private const float cameraRadiusSq = 0.0100000007f;
  private Transform mainCameraTransform;

  protected void OnEnable()
  {
    this.camWasTouching = false;
    foreach (Renderer renderer in this.renderers)
    {
      if ((Object) renderer != (Object) null)
        renderer.enabled = false;
    }
    if (!((Object) this.mainCameraTransform == (Object) null))
      return;
    this.mainCameraTransform = Camera.main.transform;
  }

  protected void LateUpdate()
  {
    if ((Object) this.mainCameraTransform == (Object) null)
      this.mainCameraTransform = Camera.main.transform;
    Vector3 position = this.mainCameraTransform.position;
    bool flag = false;
    foreach (Collider collider in this.colliders)
    {
      if (!((Object) collider == (Object) null) && (double) (collider.ClosestPoint(position) - position).sqrMagnitude < 0.010000000707805157)
      {
        flag = true;
        break;
      }
    }
    if (this.camWasTouching == flag)
      return;
    this.camWasTouching = flag;
    foreach (Renderer renderer in this.renderers)
    {
      if ((Object) renderer != (Object) null)
        renderer.enabled = flag;
    }
  }

  public bool BuildValidationCheck()
  {
    for (int index = 0; index < this.renderers.Length; ++index)
    {
      if ((Object) this.renderers[index] == (Object) null)
      {
        Debug.LogError((object) "rendererculllerbytriggers has null renderer", (Object) this.gameObject);
        return false;
      }
    }
    for (int index = 0; index < this.colliders.Length; ++index)
    {
      if ((Object) this.colliders[index] == (Object) null)
      {
        Debug.LogError((object) "rendererculllerbytriggers has null collider", (Object) this.gameObject);
        return false;
      }
    }
    return true;
  }
}
