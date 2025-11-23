// Decompiled with JetBrains decompiler
// Type: GorillaTag.Cosmetics.NetworkedRandomProvider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaNetworking;
using Photon.Pun;
using System;
using System.Text;
using UnityEngine;

#nullable disable
namespace GorillaTag.Cosmetics;

public class NetworkedRandomProvider : MonoBehaviour
{
  [Header("Time Granularity")]
  [Min(0.01f)]
  [Tooltip("Length of the time bucket (seconds). Within a bucket the pick is fixed; re-rolls next bucket.")]
  [SerializeField]
  private float windowSeconds = 1f;
  [Tooltip("Mix room name into seed so different rooms never collide.")]
  [SerializeField]
  private bool includeRoomNameInSeed = true;
  [Tooltip("Optional - If multiple component live on the same cosmetic, use different salts.")]
  [SerializeField]
  private int objectSalt;
  [Header("Output")]
  [SerializeField]
  private NetworkedRandomProvider.OutputMode outputMode;
  [SerializeField]
  private Vector2 floatRange = new Vector2(0.0f, 1f);
  [SerializeField]
  private double doubleMin;
  [SerializeField]
  private double doubleMax = 1.0;
  private TransferrableObject parentTransferable;
  private int OwnerID;
  [Header("Debug")]
  [SerializeField]
  private long debugWindow;
  [SerializeField]
  private float debugResult;

  private void Awake()
  {
    if (!((UnityEngine.Object) this.parentTransferable == (UnityEngine.Object) null))
      return;
    this.parentTransferable = this.GetComponentInParent<TransferrableObject>();
  }

  private void OnEnable() => this.EnsureOwner();

  private void OnValidate()
  {
    if ((double) this.windowSeconds < 0.0099999997764825821)
      this.windowSeconds = 0.01f;
    if ((double) this.floatRange.y < (double) this.floatRange.x)
    {
      ref float local1 = ref this.floatRange.x;
      ref float local2 = ref this.floatRange.y;
      float y = this.floatRange.y;
      float x = this.floatRange.x;
      local1 = y;
      double num = (double) x;
      local2 = (float) num;
    }
    if (this.doubleMax >= this.doubleMin)
      return;
    double doubleMax = this.doubleMax;
    double doubleMin = this.doubleMin;
    this.doubleMin = doubleMax;
    this.doubleMax = doubleMin;
  }

  private void Update()
  {
    this.debugWindow = (long) Math.Floor(this.GetSharedTime() / (double) this.windowSeconds);
  }

  private bool ShowFloatRange() => this.outputMode == NetworkedRandomProvider.OutputMode.FloatRange;

  private bool ShowDoubleRange()
  {
    return this.outputMode == NetworkedRandomProvider.OutputMode.DoubleRange;
  }

  private long GetWindowIndex()
  {
    return (long) Math.Floor(this.GetSharedTime() / (double) this.windowSeconds);
  }

  private double GetSharedTime()
  {
    return PhotonNetwork.InRoom ? PhotonNetwork.Time : (double) Time.realtimeSinceStartup;
  }

  private static ulong Mix64(ulong x)
  {
    x += 11400714819323198485UL;
    x = (ulong) (((long) x ^ (long) (x >> 30)) * -4658895280553007687L);
    x = (ulong) (((long) x ^ (long) (x >> 27)) * -7723592293110705685L);
    x ^= x >> 31 /*0x1F*/;
    return x;
  }

  private static ulong BuildSeed(long windowIndex, int ownerId, int objectSalt, uint roomSalt)
  {
    return (ulong) (windowIndex ^ (long) (uint) ownerId << 32 /*0x20*/ ^ (long) (uint) objectSalt * -7046029254386353131L ^ (long) roomSalt * -3263064605168079213L);
  }

  private static float UnitFloat01(long windowIndex, int ownerId, int objectSalt, uint roomSalt)
  {
    return (float) (uint) (NetworkedRandomProvider.Mix64(NetworkedRandomProvider.BuildSeed(windowIndex, ownerId, objectSalt, roomSalt)) >> 40) * 5.96046448E-08f;
  }

  private static double UnitDouble01(long windowIndex, int ownerId, int objectSalt, uint roomSalt)
  {
    return (double) (NetworkedRandomProvider.Mix64(NetworkedRandomProvider.BuildSeed(windowIndex, ownerId, objectSalt, roomSalt)) >> 11) * 1.1102230246251565E-16;
  }

  public float NextFloat01()
  {
    this.EnsureOwner();
    long windowIndex = this.GetWindowIndex();
    uint num1 = this.includeRoomNameInSeed ? NetworkedRandomProvider.StableHash(PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom?.Name ?? "no_room" : "no_room") : 0U;
    int ownerId = this.OwnerID;
    int objectSalt = this.objectSalt;
    int roomSalt = (int) num1;
    float num2 = NetworkedRandomProvider.UnitFloat01(windowIndex, ownerId, objectSalt, (uint) roomSalt);
    this.debugResult = num2;
    return num2;
  }

  public float NextFloat(float min, float max)
  {
    float t = this.NextFloat01();
    if ((double) max < (double) min)
    {
      double num1 = (double) max;
      float num2 = min;
      min = (float) num1;
      max = num2;
    }
    return Mathf.Lerp(min, max, t);
  }

  public double NextDouble(double min, double max)
  {
    this.EnsureOwner();
    long windowIndex = this.GetWindowIndex();
    uint num1 = this.includeRoomNameInSeed ? NetworkedRandomProvider.StableHash(PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom?.Name ?? "no_room" : "no_room") : 0U;
    int ownerId = this.OwnerID;
    int objectSalt = this.objectSalt;
    int roomSalt = (int) num1;
    double num2 = NetworkedRandomProvider.UnitDouble01(windowIndex, ownerId, objectSalt, (uint) roomSalt);
    if (max < min)
    {
      double num3 = max;
      double num4 = min;
      min = num3;
      max = num4;
    }
    double num5 = min + (max - min) * num2;
    this.debugResult = (float) num5;
    return num5;
  }

  public float GetSelectedAsFloat()
  {
    switch (this.outputMode)
    {
      case NetworkedRandomProvider.OutputMode.Double01:
        return (float) this.NextDouble(0.0, 1.0);
      case NetworkedRandomProvider.OutputMode.FloatRange:
        return this.NextFloat(this.floatRange.x, this.floatRange.y);
      case NetworkedRandomProvider.OutputMode.DoubleRange:
        return (float) this.NextDouble(this.doubleMin, this.doubleMax);
      default:
        return this.NextFloat01();
    }
  }

  public double GetSelectedAsDouble()
  {
    switch (this.outputMode)
    {
      case NetworkedRandomProvider.OutputMode.Double01:
        return this.NextDouble(0.0, 1.0);
      case NetworkedRandomProvider.OutputMode.FloatRange:
        return (double) this.NextFloat(this.floatRange.x, this.floatRange.y);
      case NetworkedRandomProvider.OutputMode.DoubleRange:
        return this.NextDouble(this.doubleMin, this.doubleMax);
      default:
        return (double) this.NextFloat01();
    }
  }

  private static uint StableHash(string s)
  {
    if (string.IsNullOrEmpty(s))
      return 0;
    uint num = 2166136261;
    for (int index = 0; index < s.Length; ++index)
      num = (num ^ (uint) s[index]) * 16777619U;
    return num;
  }

  private void EnsureOwner()
  {
    if (this.OwnerID != 0)
      return;
    this.TrySetID();
  }

  private void TrySetID()
  {
    if ((UnityEngine.Object) this.parentTransferable == (UnityEngine.Object) null)
      this.OwnerID = $"{this.gameObject.scene.name}/{NetworkedRandomProvider.GetHierarchyPath(this.transform)}{this.GetType()?.ToString()}".GetStaticHash();
    else if (this.parentTransferable.IsLocalObject())
    {
      PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
      if (!((UnityEngine.Object) instance != (UnityEngine.Object) null))
        return;
      this.OwnerID = (instance.GetPlayFabPlayerId() + this.GetType()?.ToString()).GetStaticHash();
    }
    else
    {
      if (!((UnityEngine.Object) this.parentTransferable.targetRig != (UnityEngine.Object) null) || this.parentTransferable.targetRig.creator == null)
        return;
      this.OwnerID = (this.parentTransferable.targetRig.creator.UserId + this.GetType()?.ToString()).GetStaticHash();
    }
  }

  private static string GetHierarchyPath(Transform t)
  {
    StringBuilder stringBuilder = new StringBuilder();
    for (; (UnityEngine.Object) t != (UnityEngine.Object) null; t = t.parent)
      stringBuilder.Insert(0, $"/{t.name}#{t.GetSiblingIndex().ToString()}");
    return stringBuilder.ToString();
  }

  public enum OutputMode
  {
    Float01,
    Double01,
    FloatRange,
    DoubleRange,
  }
}
