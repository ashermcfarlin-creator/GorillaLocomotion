// Decompiled with JetBrains decompiler
// Type: GorillaTag.GTTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using GorillaNetworking;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable disable
namespace GorillaTag;

public static class GTTime
{
  private const string preLog = "[GTTime]  ";
  private const string preErr = "[GTTime]  ERROR!!!  ";
  private static bool _isInitialized;

  public static TimeZoneInfo timeZoneInfoLA { get; private set; }

  static GTTime() => GTTime._Init();

  [RuntimeInitializeOnLoadMethod]
  private static void _Init()
  {
    if (GTTime._isInitialized)
      return;
    try
    {
      GTTime.timeZoneInfoLA = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
    }
    catch
    {
      try
      {
        GTTime.timeZoneInfoLA = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
      }
      catch
      {
        TimeZoneInfo out_tz;
        if (GTTime._TryCreateCustomPST(out out_tz))
        {
          GTTime.timeZoneInfoLA = out_tz;
          Debug.Log((object) "[GTTime]  _Init: Could not get US Pacific Time Zone, so using manual created Pacific time zone instead.");
        }
        else
        {
          Debug.LogError((object) "[GTTime]  ERROR!!!  _Init: Could not get US Pacific Time Zone and manual Pacific time zone creation failed. Using UTC instead.");
          GTTime.timeZoneInfoLA = TimeZoneInfo.Utc;
        }
      }
    }
    finally
    {
      GTTime._isInitialized = true;
    }
  }

  private static bool _TryCreateCustomPST(out TimeZoneInfo out_tz)
  {
    TimeZoneInfo.AdjustmentRule[] adjustmentRules = new TimeZoneInfo.AdjustmentRule[1]
    {
      TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2007, 1, 1), DateTime.MaxValue.Date, TimeSpan.FromHours(1.0), TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 3, 2, DayOfWeek.Sunday), TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 11, 1, DayOfWeek.Sunday))
    };
    try
    {
      out_tz = TimeZoneInfo.CreateCustomTimeZone("Custom/America_Los_Angeles", TimeSpan.FromHours(-8.0), "(UTC-08:00) Pacific Time (US & Canada)", "Pacific Standard Time", "Pacific Daylight Time", adjustmentRules, false);
      return true;
    }
    catch (Exception ex)
    {
      Debug.LogError((object) ("[GTTime]  ERROR!!!  _TryCreateCustomPST: Encountered exception: " + ex.Message));
      out_tz = (TimeZoneInfo) null;
      return false;
    }
  }

  public static bool usingServerTime { get; private set; }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static long GetServerStartupTimeAsMilliseconds()
  {
    return GorillaComputer.instance.startupMillis;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static long GetDeviceStartupTimeAsMilliseconds()
  {
    return (long) (TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - Time.realtimeSinceStartupAsDouble * 1000.0);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static long GetStartupTimeAsMilliseconds()
  {
    GTTime.usingServerTime = true;
    long timeAsMilliseconds = 0;
    if (GorillaComputer.hasInstance)
      timeAsMilliseconds = GTTime.GetServerStartupTimeAsMilliseconds();
    if (timeAsMilliseconds == 0L)
    {
      GTTime.usingServerTime = false;
      timeAsMilliseconds = GTTime.GetDeviceStartupTimeAsMilliseconds();
    }
    return timeAsMilliseconds;
  }

  public static long TimeAsMilliseconds()
  {
    return GTTime.GetStartupTimeAsMilliseconds() + (long) (Time.realtimeSinceStartupAsDouble * 1000.0);
  }

  public static double TimeAsDouble()
  {
    return (double) GTTime.GetStartupTimeAsMilliseconds() / 1000.0 + Time.realtimeSinceStartupAsDouble;
  }

  public static DateTime GetAAxiomDateTime()
  {
    return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GTTime.timeZoneInfoLA);
  }

  public static string GetAAxiomDateTimeAsStringForDisplay()
  {
    return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
  }

  public static string GetAAxiomDateTimeAsStringForFilename()
  {
    return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd_HH-mm-ss-fff");
  }

  public static long GetAAxiomDateTimeAsHumanReadableLong()
  {
    return long.Parse(GTTime.GetAAxiomDateTime().ToString("yyyyMMddHHmmssfff00"));
  }

  public static DateTime ConvertDateTimeHumanReadableLongToDateTime(long humanReadableLong)
  {
    return DateTime.ParseExact(humanReadableLong.ToString(), "yyyyMMddHHmmssfff'00'", (IFormatProvider) CultureInfo.InvariantCulture);
  }
}
