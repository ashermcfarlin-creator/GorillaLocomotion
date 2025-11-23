// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.ParticleModifierCosmetic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class ParticleModifierCosmetic : MonoBehaviour
{
  [SerializeField]
  private ParticleSystem ps;
  [Tooltip("For calling gradual functions only")]
  [SerializeField]
  private float transitionSpeed = 5f;
  public ParticleSettingsSO[] particleSettings = new ParticleSettingsSO[0];
  private float originalStartSize;
  private Color originalStartColor;
  private float? targetSize;
  private Color? targetColor;
  private int currentIndex;

  private void Awake()
  {
    this.StoreOriginalValues();
    this.currentIndex = -1;
  }

  private void OnValidate() => this.StoreOriginalValues();

  private void OnEnable() => this.StoreOriginalValues();

  private void OnDisable() => this.ResetToOriginal();

  private void StoreOriginalValues()
  {
    if ((Object) this.ps == (Object) null)
      return;
    ParticleSystem.MainModule main = this.ps.main;
    this.originalStartSize = main.startSize.constant;
    this.originalStartColor = main.startColor.color;
  }

  public void ApplySetting(ParticleSettingsSO setting)
  {
    this.SetStartSize(setting.startSize);
    this.SetStartColor(setting.startColor);
  }

  public void ApplySettingLerp(ParticleSettingsSO setting)
  {
    this.LerpStartSize(setting.startSize);
    this.LerpStartColor(setting.startColor);
  }

  public void MoveToNextSetting()
  {
    ++this.currentIndex;
    if (this.currentIndex <= -1 || this.currentIndex >= this.particleSettings.Length)
      return;
    this.ApplySetting(this.particleSettings[this.currentIndex]);
  }

  public void MoveToNextSettingLerp()
  {
    ++this.currentIndex;
    if (this.currentIndex <= -1 || this.currentIndex >= this.particleSettings.Length)
      return;
    this.ApplySettingLerp(this.particleSettings[this.currentIndex]);
  }

  public void ResetSettings()
  {
    this.currentIndex = -1;
    this.ResetToOriginal();
  }

  public void MoveToSettingIndex(int index)
  {
    if (index <= -1 || index >= this.particleSettings.Length)
      return;
    this.ApplySetting(this.particleSettings[index]);
  }

  public void MoveToSettingIndexLerp(int index)
  {
    if (index <= -1 || index >= this.particleSettings.Length)
      return;
    this.ApplySettingLerp(this.particleSettings[index]);
  }

  public void SetStartSize(float size)
  {
    if ((Object) this.ps == (Object) null)
      return;
    this.ps.main.startSize = (ParticleSystem.MinMaxCurve) size;
    this.targetSize = new float?();
  }

  public void IncreaseStartSize(float delta)
  {
    if ((Object) this.ps == (Object) null)
      return;
    ParticleSystem.MainModule main = this.ps.main;
    float constant = main.startSize.constant;
    main.startSize = (ParticleSystem.MinMaxCurve) (constant + delta);
    this.targetSize = new float?();
  }

  public void LerpStartSize(float size)
  {
    if ((Object) this.ps == (Object) null || (double) Mathf.Abs(this.ps.main.startSize.constant - size) < 0.0099999997764825821)
      return;
    this.targetSize = new float?(size);
  }

  public void SetStartColor(Color color)
  {
    if ((Object) this.ps == (Object) null)
      return;
    this.ps.main.startColor = (ParticleSystem.MinMaxGradient) color;
    this.targetColor = new Color?();
  }

  public void LerpStartColor(Color color)
  {
    if ((Object) this.ps == (Object) null || this.IsColorApproximatelyEqual(this.ps.main.startColor.color, color))
      return;
    this.targetColor = new Color?(color);
  }

  public void SetStartValues(float size, Color color)
  {
    this.SetStartSize(size);
    this.SetStartColor(color);
  }

  public void LerpStartValues(float size, Color color)
  {
    this.LerpStartSize(size);
    this.LerpStartColor(color);
  }

  private void Update()
  {
    if ((Object) this.ps == (Object) null)
      return;
    ParticleSystem.MainModule main = this.ps.main;
    if (this.targetSize.HasValue)
    {
      float num = Mathf.Lerp(main.startSize.constant, this.targetSize.Value, Time.deltaTime * this.transitionSpeed);
      main.startSize = (ParticleSystem.MinMaxCurve) num;
      if ((double) Mathf.Abs(num - this.targetSize.Value) < 0.0099999997764825821)
      {
        main.startSize = (ParticleSystem.MinMaxCurve) this.targetSize.Value;
        this.targetSize = new float?();
      }
    }
    if (!this.targetColor.HasValue)
      return;
    Color a = Color.Lerp(main.startColor.color, this.targetColor.Value, Time.deltaTime * this.transitionSpeed);
    main.startColor = (ParticleSystem.MinMaxGradient) a;
    if (!this.IsColorApproximatelyEqual(a, this.targetColor.Value))
      return;
    main.startColor = (ParticleSystem.MinMaxGradient) this.targetColor.Value;
    this.targetColor = new Color?();
  }

  [ContextMenu("Reset To Original")]
  public void ResetToOriginal()
  {
    if ((Object) this.ps == (Object) null)
      return;
    this.targetSize = new float?();
    this.targetColor = new Color?();
    ParticleSystem.MainModule main = this.ps.main with
    {
      startSize = (ParticleSystem.MinMaxCurve) this.originalStartSize,
      startColor = (ParticleSystem.MinMaxGradient) this.originalStartColor
    };
  }

  private bool IsColorApproximatelyEqual(Color a, Color b, float threshold = 0.0001f)
  {
    double num1 = (double) a.r - (double) b.r;
    float num2 = a.g - b.g;
    float num3 = a.b - b.b;
    float num4 = a.a - b.a;
    return num1 * num1 + (double) num2 * (double) num2 + (double) num3 * (double) num3 + (double) num4 * (double) num4 < (double) threshold;
  }
}
