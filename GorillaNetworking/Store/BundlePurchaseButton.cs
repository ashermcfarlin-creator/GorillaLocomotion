// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.BundlePurchaseButton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
namespace GorillaNetworking.Store;

public class BundlePurchaseButton : GorillaPressableButton, IGorillaSliceableSimple
{
  private const string MONKE_BLOCKS_BUNDLE_ALREADY_OWN_KEY = "MONKE_BLOCKS_BUNDLE_ALREADY_OWN";
  private const string MONKE_BLOCKS_BUNDLE_UNAVAILABLE_KEY = "MONKE_BLOCKS_BUNDLE_UNAVAILABLE";
  private const string MONKE_BLOCKS_BUNDLE_ERROR_KEY = "MONKE_BLOCKS_BUNDLE_ERROR";
  public bool bError;
  public string ErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME";
  public string AlreadyOwnText = "YOU OWN THE BUNDLE ALREADY! THANK YOU!";
  public string UnavailableText = "UNAVAILABLE";
  public string playfabID = "";

  public new void OnEnable()
  {
    GorillaSlicerSimpleManager.RegisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.Update);
  }

  public new void OnDisable()
  {
    GorillaSlicerSimpleManager.UnregisterSliceable((IGorillaSliceableSimple) this, GorillaSlicerSimpleManager.UpdateStep.Update);
  }

  public void SliceUpdate()
  {
    if (!((Object) NetworkSystem.Instance != (Object) null) || !NetworkSystem.Instance.WrongVersion || this.bError)
      return;
    this.enabled = false;
    this.GetComponent<BoxCollider>().enabled = false;
    this.buttonRenderer.material = this.pressedMaterial;
    this.myText.text = this.UnavailableText;
  }

  public override void ButtonActivation()
  {
    if (this.bError)
      return;
    base.ButtonActivation();
    BundleManager.instance.BundlePurchaseButtonPressed(this.playfabID);
    this.StartCoroutine(this.ButtonColorUpdate());
  }

  public void AlreadyOwn()
  {
    if (this.bError)
      return;
    this.enabled = false;
    this.GetComponent<BoxCollider>().enabled = false;
    this.buttonRenderer.material = this.pressedMaterial;
    this.onText = this.AlreadyOwnText;
    this.myText.text = this.AlreadyOwnText;
    this.isOn = true;
  }

  public void ResetButton()
  {
    if (this.bError)
      return;
    this.enabled = true;
    this.GetComponent<BoxCollider>().enabled = true;
    this.buttonRenderer.material = this.unpressedMaterial;
    this.SetOffText(true);
    this.isOn = false;
  }

  private IEnumerator ButtonColorUpdate()
  {
    BundlePurchaseButton bundlePurchaseButton = this;
    bundlePurchaseButton.buttonRenderer.material = bundlePurchaseButton.pressedMaterial;
    yield return (object) new WaitForSeconds(bundlePurchaseButton.debounceTime);
    bundlePurchaseButton.buttonRenderer.material = bundlePurchaseButton.isOn ? bundlePurchaseButton.pressedMaterial : bundlePurchaseButton.unpressedMaterial;
  }

  public void ErrorHappened()
  {
    this.bError = true;
    this.myText.text = this.ErrorText;
    this.buttonRenderer.material = this.unpressedMaterial;
    this.enabled = false;
    this.offText = this.ErrorText;
    this.onText = this.ErrorText;
    this.isOn = false;
  }

  public void InitializeData()
  {
    if (this.bError)
      return;
    this.SetOffText(true);
    this.buttonRenderer.material = this.unpressedMaterial;
    this.enabled = true;
    this.isOn = false;
  }

  public void UpdatePurchaseButtonText(string purchaseText)
  {
    if (this.bError)
      return;
    this.offText = purchaseText;
    this.UpdateColor();
  }
}
