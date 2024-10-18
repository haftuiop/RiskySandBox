using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ObservableList<T> : IList<T>
{
    [UnityEngine.SerializeField] private List<T> items = new List<T>();

    // Event to notify subscribers when the list changes
    public event Action OnUpdate;
    public event Action<T> OnAdd;
    public event Action<T> OnRemove;


    public void SET_items(IEnumerable<T> _content)
    {
        items.Clear();
        items.AddRange(_content);
        OnUpdate?.Invoke();
    }

    // Add an item to the list
    public void Add(T item)
    {
        items.Add(item);
        OnAdd?.Invoke(item);
        OnUpdate?.Invoke();
    }

    // Remove an item from the list
    public bool Remove(T item)
    {
        bool result = items.Remove(item);
        if (result)
        {
            OnRemove?.Invoke(item);
            OnUpdate?.Invoke();
        }
        return result;
    }

    public void AddRange(IEnumerable<T> _items)
    {
        foreach(T _element in _items)
        {
            this.items.Add(_element);
            OnAdd?.Invoke(_element);
        }
        this.OnUpdate?.Invoke();
    }

    // IList<T> interface implementation
    public T this[int index]
    {
        get { return items[index]; }
        set { items[index] = value; OnUpdate?.Invoke(); }
    }

    public int Count => items.Count;

    public bool IsReadOnly => false;

    public IEnumerator<T> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Insert(int index, T item)
    {
        items.Insert(index, item);
        OnUpdate?.Invoke();
    }

    public void RemoveAt(int index)
    {
        T item = items[index];
        items.RemoveAt(index);
        OnUpdate?.Invoke();
    }

    public void Clear()
    {
        items.Clear();
        OnUpdate?.Invoke();
    }

    public bool Contains(T item)
    {
        return items.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        items.CopyTo(array, arrayIndex);
    }

    public int IndexOf(T item)
    {
        return items.IndexOf(item);
    }

}