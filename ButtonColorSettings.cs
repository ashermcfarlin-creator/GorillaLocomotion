// Decompiled with JetBrains decompiler
// Type: GorillaTag.ButtonColorSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag;

[CreateAssetMenu(fileName = "GorillaButtonColorSettings", menuName = "ScriptableObjects/GorillaButtonColorSettings", order = 0)]
public class ButtonColorSettings : ScriptableObject
{
  public Color UnpressedColor;
  public Color PressedColor;
  [Tooltip("Optional\nThe time the change will be in effect")]
  public float PressedTime;
}
