using System.Collections;using System.Collections.Generic;using System.Linq;using System;


public partial class ObservableStringList : IList<string>
{

    public event Action<ObservableStringList> Onsynchronize;
    public event Action<ObservableStringList> OnUpdate;


    void synchronize()
    {
        if (this.debugging)
            GlobalFunctions.print("called synchronize", this);
        this.Onsynchronize?.Invoke(this);
    }


    public void SET_itemsFromMultiplayerBridge(IEnumerable<string> _content)
    {
        this.items.Clear();
        this.items.AddRange(_content);
        this.OnUpdate?.Invoke(this);
    }



    public string this[int index]
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
        

    public int Count{ get { return this.items.Count(); }}

    /// <summary>
    /// how many occurences of _item are in this list...
    /// </summary>
    public int CountOf(string _item)
    {
        int _count = 0;
        foreach(string _string in this.items)
        {
            if (_string == _item)
                _count += 1;
        }
        return _count;
    }

    public bool IsReadOnly { get { return false; } }

    public void Add(string item)
    {
        this.items.Add(item);

        if (my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);
    }

    public void AddRange(IEnumerable<string> _items)
    {
        this.items.AddRange(_items);

        if (my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);
    }

    public void Clear()
    {
        this.items.Clear();

        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);
    }

    public bool Contains(string item)
    {
        return this.items.Contains(item);
    }

    public void CopyTo(string[] array, int arrayIndex)
    {
        this.items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<string> GetEnumerator()
    {
        return this.items.GetEnumerator();
    }

    public int IndexOf(string item)
    {
        return this.items.IndexOf(item);
    }

    public void Insert(int index, string item)
    {
        this.items.Insert(index, item);

        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);
    }

    public bool Remove(string item)
    {
        bool _removed = this.items.Remove(item);
        if(_removed)
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




}
