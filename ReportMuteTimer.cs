// Decompiled with JetBrains decompiler
// Type: GorillaTag.ReportMuteTimer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using Photon.Realtime;

#nullable disable
namespace GorillaTag;

internal class ReportMuteTimer : TickSystemTimerAbstract, ObjectPoolEvents
{
  private static readonly NetEventOptions netEventOptions = new NetEventOptions()
  {
    Flags = new WebFlags((byte) 3),
    TargetActors = new int[1]{ -1 }
  };
  private static readonly object[] content = new object[6];
  private const byte evCode = 51;
  private string m_playerID;
  private string m_nickName;

  public int Muted { get; set; }

  public override void OnTimedEvent()
  {
    if (!NetworkSystem.Instance.InRoom)
    {
      this.Stop();
    }
    else
    {
      ReportMuteTimer.content[0] = (object) this.m_playerID;
      ReportMuteTimer.content[1] = (object) this.Muted;
      ReportMuteTimer.content[2] = this.m_nickName.Length > 12 ? (object) this.m_nickName.Remove(12) : (object) this.m_nickName;
      ReportMuteTimer.content[3] = (object) NetworkSystem.Instance.LocalPlayer.NickName;
      ReportMuteTimer.content[4] = (object) !NetworkSystem.Instance.SessionIsPrivate;
      ReportMuteTimer.content[5] = (object) NetworkSystem.Instance.RoomStringStripped();
      NetworkSystemRaiseEvent.RaiseEvent((byte) 51, (object) ReportMuteTimer.content, ReportMuteTimer.netEventOptions, true);
      this.Stop();
    }
  }

  public void SetReportData(string id, string name, int muted)
  {
    this.Muted = muted;
    this.m_playerID = id;
    this.m_nickName = name;
  }

  void ObjectPoolEvents.OnTaken()
  {
  }

  void ObjectPoolEvents.OnReturned()
  {
    if (this.Running)
      this.OnTimedEvent();
    this.m_playerID = string.Empty;
    this.m_nickName = string.Empty;
    this.Muted = 0;
  }
}
