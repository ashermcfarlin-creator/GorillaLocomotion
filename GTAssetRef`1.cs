// Decompiled with JetBrains decompiler
// Type: GorillaTag.GTAssetRef`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine.AddressableAssets;

#nullable disable
namespace GorillaTag;

[Serializable]
public class GTAssetRef<TObject>(string guid) : AssetReferenceT<TObject>(guid) where TObject : UnityEngine.Object
{
}
