// Decompiled with JetBrains decompiler
// Type: GorillaNetworking.Store.BundleStand
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#nullable disable
namespace GorillaNetworking.Store;

public class BundleStand : MonoBehaviour
{
  public BundlePurchaseButton _bundlePurchaseButton;
  [SerializeField]
  public StoreBundleData _bundleDataReference;
  public GameObject[] EditorOnlyObjects;
  public Text _bundleDescriptionText;
  public Image _bundleIcon;
  public UnityEvent AlreadyOwnEvent;
  public UnityEvent ErrorHappenedEvent;

  public string playfabBundleID => this._bundleDataReference.playfabBundleID;

  public void Awake()
  {
    this._bundlePurchaseButton.playfabID = this.playfabBundleID;
    if (!((Object) this._bundleIcon != (Object) null) || !((Object) this._bundleDataReference != (Object) null) || !((Object) this._bundleDataReference.bundleImage != (Object) null))
      return;
    this._bundleIcon.sprite = this._bundleDataReference.bundleImage;
  }

  public void InitializeEventListeners()
  {
    this.AlreadyOwnEvent.AddListener(new UnityAction(this._bundlePurchaseButton.AlreadyOwn));
    this.ErrorHappenedEvent.AddListener(new UnityAction(this._bundlePurchaseButton.ErrorHappened));
  }

  public void NotifyAlreadyOwn() => this.AlreadyOwnEvent.Invoke();

  public void ErrorHappened() => this.ErrorHappenedEvent.Invoke();

  public void UpdatePurchaseButtonText(string purchaseText)
  {
    if (!((Object) this._bundlePurchaseButton != (Object) null))
      return;
    this._bundlePurchaseButton.UpdatePurchaseButtonText(purchaseText);
  }

  public void UpdateDescriptionText(string descriptionText)
  {
    if (!((Object) this._bundleDescriptionText != (Object) null))
      return;
    this._bundleDescriptionText.text = descriptionText;
  }
}
