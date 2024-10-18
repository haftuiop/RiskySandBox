using System.Collections;using System.Collections.Generic;using System.Linq;using System;

public partial class ObservableFloatList : IList<float>
{
    public event Action<ObservableFloatList> OnUpdate;
    public event Action<ObservableFloatList> Onsynchronize;



    public void synchronize()
    {
        if (debugging)
            GlobalFunctions.print("called synchronize!", this);

        Onsynchronize?.Invoke(this);
    }

    public void SET_itemsFromMultiplayerBridge(IEnumerable<float> _content)
    {
        this.items.Clear();
        this.items.AddRange(_content);
        this.OnUpdate?.Invoke(this);
    }



    public float this[int index]
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

    public void Add(float item)
    {
        this.items.Add(item);
        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();

        this.OnUpdate?.Invoke(this);
    }

    public void Clear()
    {
        this.items.Clear();
        if (my_VariableSettings.synchronise_immediately)
            this.synchronize();

        this.OnUpdate?.Invoke(this);
    }

    public bool Contains(float item)
    {
        return this.items.Contains(item);
    }

    public void CopyTo(float[] array, int arrayIndex)
    {
        this.items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<float> GetEnumerator()
    {
        return this.items.GetEnumerator();
    }

    public int IndexOf(float item)
    {
        return this.items.IndexOf(item);
    }

    public void Insert(int index, float item)
    {
        this.items.Insert(index, item);
        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);
    }

    public bool Remove(float item)
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
        this.RemoveAt(index);
        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();

        this.OnUpdate?.Invoke(this);
    }



    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.items.GetEnumerator();
    }
}
