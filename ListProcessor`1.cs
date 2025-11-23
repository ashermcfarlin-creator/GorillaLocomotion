// Decompiled with JetBrains decompiler
// Type: GorillaTag.ListProcessor`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 97A23EB5-635F-4763-8E85-4A9FA332BC9B
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag\Gorilla Tag_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace GorillaTag;

public class ListProcessor<T>
{
  protected readonly List<T> m_list;
  protected int m_currentIndex;
  protected int m_listCount;
  protected InAction<T> m_itemProcessorDelegate;

  public int Count => this.m_list.Count;

  public InAction<T> ItemProcessor
  {
    get => this.m_itemProcessorDelegate;
    set => this.m_itemProcessorDelegate = value;
  }

  public ListProcessor()
    : this(10)
  {
  }

  public ListProcessor(int capacity, InAction<T> itemProcessorDelegate = null)
  {
    this.m_list = new List<T>(capacity);
    this.m_currentIndex = -1;
    this.m_listCount = -1;
    this.m_itemProcessorDelegate = itemProcessorDelegate;
  }

  public void Add(in T item)
  {
    ++this.m_listCount;
    this.m_list.Add(item);
  }

  public void Remove(in T item)
  {
    int index = this.m_list.IndexOf(item);
    if (index < 0)
      return;
    if (index < this.m_currentIndex)
      --this.m_currentIndex;
    --this.m_listCount;
    this.m_list.RemoveAt(index);
  }

  public void Clear()
  {
    this.m_list.Clear();
    this.m_currentIndex = -1;
    this.m_listCount = -1;
  }

  public bool Contains(in T item) => this.m_list.Contains(item);

  public virtual void ProcessListSafe()
  {
    if (this.m_itemProcessorDelegate == null)
    {
      Debug.LogError((object) "ListProcessor: ItemProcessor is null");
    }
    else
    {
      this.m_listCount = this.m_list.Count;
      for (this.m_currentIndex = 0; this.m_currentIndex < this.m_listCount; ++this.m_currentIndex)
      {
        try
        {
          this.m_itemProcessorDelegate(this.m_list[this.m_currentIndex]);
        }
        catch (Exception ex)
        {
          Debug.LogError((object) ex.ToString());
        }
      }
    }
  }

  public virtual void ProcessList()
  {
    if (this.m_itemProcessorDelegate == null)
    {
      Debug.LogError((object) "ListProcessor: ItemProcessor is null");
    }
    else
    {
      this.m_listCount = this.m_list.Count;
      for (this.m_currentIndex = 0; this.m_currentIndex < this.m_listCount; ++this.m_currentIndex)
        this.m_itemProcessorDelegate(this.m_list[this.m_currentIndex]);
    }
  }
}
