// Decompiled with JetBrains decompiler
// Type: GorillaTag.CustomMapTestingScript
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
namespace GorillaTag;

public class CustomMapTestingScript : GorillaPressableButton
{
  public override void ButtonActivation()
  {
    base.ButtonActivation();
    this.StartCoroutine(this.ButtonPressed_Local());
  }

  private IEnumerator ButtonPressed_Local()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    CustomMapTestingScript mapTestingScript = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      mapTestingScript.isOn = false;
      mapTestingScript.UpdateColor();
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    mapTestingScript.isOn = true;
    mapTestingScript.UpdateColor();
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(mapTestingScript.debounceTime);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }
}
