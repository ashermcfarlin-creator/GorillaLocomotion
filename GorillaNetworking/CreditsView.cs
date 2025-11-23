// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.CreditsView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using LitJson;
using PlayFab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

#nullable disable
namespace GorillaNetworking;

public class CreditsView : MonoBehaviour
{
  private const string CREDITS_KEY = "CREDITS";
  private const string CREDITS_PRESS_ENTER_KEY = "CREDITS_PRESS_ENTER";
  private const string CREDITS_CONTINUED_KEY = "CREDITS_CONTINUED";
  private CreditsSection[] creditsSections;
  public int pageSize = 7;
  private int currentPage;
  private const string PlayFabKey = "CreditsData";

  private int TotalPages
  {
    get
    {
      return ((IEnumerable<CreditsSection>) this.creditsSections).Sum<CreditsSection>((Func<CreditsSection, int>) (section => this.PagesPerSection(section)));
    }
  }

  private void Start()
  {
    this.creditsSections = new CreditsSection[3]
    {
      new CreditsSection()
      {
        Title = "DEV TEAM",
        Entries = new List<string>()
        {
          "Anton \"NtsFranz\" Franzluebbers",
          "Carlo Grossi Jr",
          "Cody O'Quinn",
          "David Neubelt",
          "David \"AA_DavidY\" Yee",
          "Derek \"DunkTrain\" Arabian",
          "Elie Arabian",
          "John Sleeper",
          "Haunted Army",
          "Kerestell Smith",
          "Keith \"ElectronicWall\" Taylor",
          "Laura \"Poppy\" Lorian",
          "Lilly Tothill",
          "Matt \"Crimity\" Ostgard",
          "Nick Taylor",
          "Ross Furmidge",
          "Sasha \"Kayze\" Sanders"
        }
      },
      new CreditsSection()
      {
        Title = "SPECIAL THANKS",
        Entries = new List<string>()
        {
          "The \"Sticks\"",
          "Alpha Squad",
          "Meta",
          "Scout House",
          "Mighty PR",
          "Caroline Arabian",
          "Clarissa & Declan",
          "Calum Haigh",
          "EZ ICE",
          "Gwen"
        }
      },
      new CreditsSection()
      {
        Title = "MUSIC BY",
        Entries = new List<string>()
        {
          "Stunshine",
          "David Anderson Kirk",
          "Jaguar Jen",
          "Audiopfeil",
          "Owlobe"
        }
      }
    };
    PlayFabTitleDataCache.Instance.GetTitleData("CreditsData", (Action<string>) (result => this.creditsSections = JsonMapper.ToObject<CreditsSection[]>(result)), (Action<PlayFabError>) (error => Debug.Log((object) ("Error fetching credits data: " + error.ErrorMessage))));
  }

  private int PagesPerSection(CreditsSection section)
  {
    return (int) Math.Ceiling((double) section.Entries.Count / (double) this.pageSize);
  }

  private IEnumerable<string> PageOfSection(CreditsSection section, int page)
  {
    return section.Entries.Skip<string>(this.pageSize * page).Take<string>(this.pageSize);
  }

  private (CreditsSection creditsSection, int subPage) GetPageEntries(int page)
  {
    int num1 = 0;
    foreach (CreditsSection creditsSection in this.creditsSections)
    {
      int num2 = this.PagesPerSection(creditsSection);
      if (num1 + num2 > page)
      {
        int num3 = page - num1;
        return (creditsSection, num3);
      }
      num1 += num2;
    }
    return (((IEnumerable<CreditsSection>) this.creditsSections).First<CreditsSection>(), 0);
  }

  public void ProcessButtonPress(GorillaKeyboardBindings buttonPressed)
  {
    if (buttonPressed != GorillaKeyboardBindings.enter)
      return;
    ++this.currentPage;
    this.currentPage %= this.TotalPages;
  }

  public string GetScreenText() => this.GetPage(this.currentPage);

  private string GetPage(int page)
  {
    (CreditsSection creditsSection, int num) = this.GetPageEntries(page);
    IEnumerable<string> source = this.PageOfSection(creditsSection, num);
    string defaultResult1 = "CREDITS";
    string result1;
    LocalisationManager.TryGetKeyForCurrentLocale("CREDITS", out result1, defaultResult1);
    string defaultResult2 = "(CONT)";
    string result2;
    LocalisationManager.TryGetKeyForCurrentLocale("CREDITS_CONTINUED", out result2, defaultResult2);
    string str1 = $"{result1} - {(num == 0 ? creditsSection.Title : $"{creditsSection.Title} {result2}")}";
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine(str1);
    stringBuilder.AppendLine();
    foreach (string str2 in source)
      stringBuilder.AppendLine(str2);
    for (int index = 0; index < this.pageSize - source.Count<string>(); ++index)
      stringBuilder.AppendLine();
    stringBuilder.AppendLine();
    string defaultResult3 = "PRESS ENTER TO CHANGE PAGES";
    string result3;
    LocalisationManager.TryGetKeyForCurrentLocale("CREDITS_PRESS_ENTER", out result3, defaultResult3);
    stringBuilder.AppendLine(result3);
    return stringBuilder.ToString();
  }
}
