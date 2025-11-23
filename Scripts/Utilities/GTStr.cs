// Decompiled with JetBrains decompiler
// Type: GorillaTag.Scripts.Utilities.GTStr
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Text;

#nullable disable
namespace GorillaTag.Scripts.Utilities;

public static class GTStr
{
  public static void Bullet(StringBuilder builder, IList<string> strings, string bulletStr = "- ")
  {
    for (int index = 0; index < strings.Count; ++index)
      builder.Append(bulletStr).Append(strings[index]).Append("\n");
  }

  public static string Bullet(IList<string> strings, string bulletStr = "- ")
  {
    if (strings == null || strings.Count == 0)
      return string.Empty;
    int capacity = strings.Count * (bulletStr.Length + 1);
    for (int index = 0; index < strings.Count; ++index)
      capacity += strings[index].Length;
    StringBuilder builder = new StringBuilder(capacity);
    GTStr.Bullet(builder, strings, bulletStr);
    return builder.ToString();
  }
}
