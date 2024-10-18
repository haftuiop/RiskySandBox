using System.Collections;using System.Collections.Generic;using System.Linq;using System;

public partial class ObservableBoolList : IList<bool>
{
    public event Action<ObservableBoolList> OnUpdate;
    public event Action<ObservableBoolList> Onsynchronize;


    public void synchronize()
    {
        if (debugging)
            GlobalFunctions.print("called synchronize!", this);

        Onsynchronize?.Invoke(this);
    }

    public void SET_itemsFromMultiplayerBridge(IEnumerable<bool> _content)
    {
        this.items.Clear();
        this.items.AddRange(_content);
        this.OnUpdate?.Invoke(this);
    }



    public bool this[int index]
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

    public bool IsReadOnly => false;


    public void Add(bool item)
    {
        this.items.Add(item);
        if (this.my_VariableSettings.synchronise_immediately)
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

    public bool Contains(bool item)
    {
        return this.items.Contains(item);//this doesnt really make sense??? but ok lets just do it...
    }

    public void CopyTo(bool[] array, int arrayIndex)
    {
        this.items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<bool> GetEnumerator()
    {
        return this.items.GetEnumerator();
    }

    public int IndexOf(bool item)
    {
        return this.items.IndexOf(item);//this doesnt really make sense??? but ok lets just do it...
    }

    public void Insert(int index, bool item)
    {
        this.items.Insert(index, item);
        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);
    }

    public bool Remove(bool item)
    {
        bool _removed = this.items.Remove(item);
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
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.items.GetEnumerator();
    }
}
