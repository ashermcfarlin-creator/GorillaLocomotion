// Decompiled with JetBrains decompiler
// Type: GorillaTag.ObjectPool`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace GorillaTag;

public class ObjectPool<T> where T : ObjectPoolEvents, new()
{
  private Stack<T> pool;
  public int maxInstances = 500;

  protected ObjectPool()
  {
  }

  public ObjectPool(int amount)
    : this(amount, amount)
  {
  }

  public ObjectPool(int initialAmount, int maxAmount)
  {
    this.InitializePool(initialAmount, maxAmount);
  }

  protected void InitializePool(int initialAmount, int maxAmount)
  {
    this.maxInstances = maxAmount;
    this.pool = new Stack<T>(initialAmount);
    for (int index = 0; index < initialAmount; ++index)
      this.pool.Push(this.CreateInstance());
  }

  public T Take()
  {
    T obj = this.pool.Count >= 1 ? this.pool.Pop() : this.CreateInstance();
    obj.OnTaken();
    return obj;
  }

  public void Return(T instance)
  {
    instance.OnReturned();
    if (this.pool.Count == this.maxInstances)
      return;
    this.pool.Push(instance);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public virtual T CreateInstance() => new T();
}
