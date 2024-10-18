using System.Collections;using System.Collections.Generic;using System.Linq;using System;

public partial class ObservableIntList : IList<int>
{
    public event Action<ObservableIntList> OnUpdate;
    public event Action<ObservableIntList> Onsynchronize;



    public void synchronize()
    {
        if (debugging)
            GlobalFunctions.print("called synchronize!", this);

        Onsynchronize?.Invoke(this);
    }


    public void SET_itemsFromMultiplayerBridge(IEnumerable<int> _content)
    {
        this.items.Clear();
        this.items.AddRange(_content);
        this.OnUpdate?.Invoke(this);
    }

    public void SET_items(IEnumerable<int> _content)
    {
        List<int> _content_list = new List<int>(_content);
        this.items.Clear();
        this.items.AddRange(_content_list);

        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);
    }


    public int this[int index]
    {
        get { return this.items[index]; }
        set
        {
            this.items[index] = value;
            if (this.my_VariableSettings.synchronise_immediately)
                this.synchronize();
            this.OnUpdate?.Invoke(this);
        }
    }

    public int Count => this.items.Count;

    public int CountOf(int _item)
    {
        int _count = 0;
        foreach(int _i in this.items)
        {
            if (_i == _item)
                _count += 1;
        }
        return _count;
    }

    public bool IsReadOnly => false;



    public void Add(int item)
    {
        this.items.Add(item);
        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);
    }

    public void AddRange(IEnumerable<int> _ints)
    {
        foreach(int _int in _ints)
        {
            this.items.Add(_int);
        }
        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();
        OnUpdate?.Invoke(this);
    }

    public void Clear()
    {
        this.items.Clear();
        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);
    }

    public bool Contains(int item)
    {
        return this.items.Contains(item);
    }

    public void CopyTo(int[] array, int arrayIndex)
    {
        this.items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<int> GetEnumerator()
    {
        return this.items.GetEnumerator();
    }

    public int IndexOf(int item)
    {
        return this.items.IndexOf(item);
    }

    public void Insert(int index, int item)
    {
        this.items.Insert(index, item);
        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);
    }

    public bool Remove(int item)
    {
        bool _removed =  this.items.Remove(item);
        if (_removed == true)
        {
            if (this.my_VariableSettings.synchronise_immediately)
                this.synchronize();
            this.OnUpdate?.Invoke(this);
        }
        return _removed;
    }

    public void RemoveAt(int index)
    {
        this.items.RemoveAt(index);
        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.items.GetEnumerator();
    }


    public static void paste(IEnumerable<ObservableIntList> _sources, IEnumerable<ObservableIntList> _destinations)
    {
        var _sources_list = new List<ObservableIntList>(_sources);
        var _destination_list = new List<ObservableIntList>(_destinations);

        if (_sources_list.Count != _destination_list.Count)
        {
            GlobalFunctions.print("_sources.Count() != _destinations.Count()", null);
            return;
        }

        for (int i = 0; i < _sources_list.Count; i += 1)
        {
            ObservableIntList.paste(_sources_list[i], _destination_list[i]);
        }
    }

    public static void paste(ObservableIntList _source, ObservableIntList _destination)
    {
        _destination.items = new List<int>(_source.items);

        if (_destination.my_VariableSettings.synchronise_immediately)
            _destination.synchronize();

        _destination.OnUpdate?.Invoke(_destination);
    }



}
