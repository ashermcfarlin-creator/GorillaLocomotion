// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.GorillaKeyboardBindingExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace GorillaNetworking;

public static class GorillaKeyboardBindingExtensions
{
  public static bool FromNumberBindingToInt(this GorillaKeyboardBindings binding, out int result)
  {
    result = -1;
    switch (binding)
    {
      case GorillaKeyboardBindings.zero:
        result = 0;
        break;
      case GorillaKeyboardBindings.one:
        result = 1;
        break;
      case GorillaKeyboardBindings.two:
        result = 2;
        break;
      case GorillaKeyboardBindings.three:
        result = 3;
        break;
      case GorillaKeyboardBindings.four:
        result = 4;
        break;
      case GorillaKeyboardBindings.five:
        result = 5;
        break;
      case GorillaKeyboardBindings.six:
        result = 6;
        break;
      case GorillaKeyboardBindings.seven:
        result = 7;
        break;
      case GorillaKeyboardBindings.eight:
        result = 8;
        break;
      case GorillaKeyboardBindings.nine:
        result = 9;
        break;
      default:
        return false;
    }
    return true;
  }
}
