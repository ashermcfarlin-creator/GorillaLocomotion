// Decompiled with JetBrains decompiler
// Type: GorillaTag.DrinkableHoldable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using emotitron.Compression;
using GorillaNetworking;
using System;
using UnityEngine;

#nullable disable
namespace GorillaTag;

public class DrinkableHoldable : TransferrableObject
{
  [AssignInCorePrefab]
  public ContainerLiquid containerLiquid;
  [AssignInCorePrefab]
  [SoundBankInfo]
  public SoundBankPlayer sipSoundBankPlayer;
  [AssignInCorePrefab]
  public float sipRate = 0.1f;
  [AssignInCorePrefab]
  public float sipSoundCooldown = 0.5f;
  [AssignInCorePrefab]
  public Vector3 headToMouthOffset = new Vector3(0.0f, 13f / 625f, 0.171f);
  [AssignInCorePrefab]
  public float sipRadius = 0.15f;
  private float lastTimeSipSoundPlayed;
  private bool wasSipping;
  private bool coolingDown;
  private bool wasCoolingDown;
  private byte[] myByteArray;

  internal override void OnEnable()
  {
    base.OnEnable();
    this.enabled = (UnityEngine.Object) this.containerLiquid != (UnityEngine.Object) null;
    this.itemState = (TransferrableObject.ItemStates) DrinkableHoldable.PackValues(this.sipSoundCooldown, this.containerLiquid.fillAmount, this.coolingDown);
    this.myByteArray = new byte[32 /*0x20*/];
  }

  protected override void LateUpdateLocal()
  {
    if (!this.containerLiquid.isActiveAndEnabled || !GorillaParent.hasInstance || !GorillaComputer.hasInstance)
    {
      base.LateUpdateLocal();
    }
    else
    {
      float num1 = (float) ((GorillaComputer.instance.startupMillis + (long) Time.realtimeSinceStartup * 1000L) % 259200000L) / 1000f;
      if ((double) Mathf.Abs(num1 - this.lastTimeSipSoundPlayed) > 129600.0)
        this.lastTimeSipSoundPlayed = num1;
      float num2 = this.sipRadius * this.sipRadius;
      Vector3 vector3 = GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.TransformPoint(this.headToMouthOffset) - this.containerLiquid.cupTopWorldPos;
      bool flag = (double) vector3.sqrMagnitude < (double) num2;
      if (!flag)
      {
        foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
        {
          if (!vrrig.isOfflineVRRig)
          {
            if (!flag)
            {
              if (vrrig.head != null)
              {
                if (!((UnityEngine.Object) vrrig.head.rigTarget == (UnityEngine.Object) null))
                {
                  vector3 = vrrig.head.rigTarget.transform.TransformPoint(this.headToMouthOffset) - this.containerLiquid.cupTopWorldPos;
                  flag = (double) vector3.sqrMagnitude < (double) num2;
                }
                else
                  break;
              }
              else
                break;
            }
            else
              break;
          }
        }
      }
      if (flag)
      {
        this.containerLiquid.fillAmount = Mathf.Clamp01(this.containerLiquid.fillAmount - this.sipRate * Time.deltaTime);
        if ((double) num1 > (double) this.lastTimeSipSoundPlayed + (double) this.sipSoundCooldown)
        {
          if (!this.wasSipping)
          {
            this.lastTimeSipSoundPlayed = num1;
            this.coolingDown = true;
          }
        }
        else
          this.coolingDown = false;
      }
      this.wasSipping = flag;
      this.itemState = (TransferrableObject.ItemStates) DrinkableHoldable.PackValues(this.lastTimeSipSoundPlayed, this.containerLiquid.fillAmount, this.coolingDown);
      base.LateUpdateLocal();
    }
  }

  protected override void LateUpdateReplicated()
  {
    base.LateUpdateReplicated();
    this.UnpackValuesNonstatic((int) this.itemState, out this.lastTimeSipSoundPlayed, out this.containerLiquid.fillAmount, out this.coolingDown);
  }

  protected override void LateUpdateShared()
  {
    base.LateUpdateShared();
    if (this.coolingDown && !this.wasCoolingDown)
      this.sipSoundBankPlayer.Play();
    this.wasCoolingDown = this.coolingDown;
  }

  private static int PackValues(float cooldownStartTime, float fillAmount, bool coolingDown)
  {
    byte[] buffer = new byte[32 /*0x20*/];
    int bitposition = 0;
    buffer.WriteBool(coolingDown, ref bitposition);
    buffer.Write((ulong) ((double) cooldownStartTime * 100.0), ref bitposition, 25);
    buffer.Write((ulong) ((double) fillAmount * 63.0), ref bitposition, 6);
    return BitConverter.ToInt32(buffer, 0);
  }

  private void UnpackValuesNonstatic(
    in int packed,
    out float cooldownStartTime,
    out float fillAmount,
    out bool coolingDown)
  {
    DrinkableHoldable.GetBytes(packed, ref this.myByteArray);
    int bitposition = 0;
    coolingDown = this.myByteArray.ReadBool(ref bitposition);
    cooldownStartTime = (float) this.myByteArray.Read(ref bitposition, 25) / 100f;
    fillAmount = (float) this.myByteArray.Read(ref bitposition, 6) / 63f;
  }

  public static void GetBytes(int value, ref byte[] bytes)
  {
    for (int index = 0; index < bytes.Length; ++index)
      bytes[index] = (byte) (value >> 8 * index & (int) byte.MaxValue);
  }

  private static void UnpackValuesStatic(
    in int packed,
    out float cooldownStartTime,
    out float fillAmount,
    out bool coolingDown)
  {
    byte[] bytes = BitConverter.GetBytes(packed);
    int bitposition = 0;
    coolingDown = bytes.ReadBool(ref bitposition);
    cooldownStartTime = (float) bytes.Read(ref bitposition, 25) / 100f;
    fillAmount = (float) bytes.Read(ref bitposition, 6) / 63f;
  }
}
