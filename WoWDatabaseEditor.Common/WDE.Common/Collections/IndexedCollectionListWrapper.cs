using System;
using System.Collections.Generic;

namespace WDE.Common.Collections;

internal class IndexedCollectionListWrapper<T> : IIndexedCollection<T>
{
    private readonly IReadOnlyList<T> list;

    public IndexedCollectionListWrapper(IReadOnlyList<T> list)
    {
        this.list = list;
    }

    public T this[int index] => list[index];
    public int Count => list.Count;
    public event Action? OnCountChanged;
}