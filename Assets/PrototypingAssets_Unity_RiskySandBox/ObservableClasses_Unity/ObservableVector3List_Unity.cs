using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

[RequireComponent(typeof(ObservableClasses_VariableSettings))]
public partial class ObservableVector3List : MonoBehaviour, IList<Vector3>
{
    [SerializeField] bool debugging;

    [SerializeField] List<Vector3> items = new List<Vector3>();


    bool cached_my_VariableSettings = false;
    public ObservableClasses_VariableSettings my_VariableSettings
    {
        get
        {
            if (this.cached_my_VariableSettings == true)
                return PRIVATE_my_VariableSettings;
            return this.GetComponent<ObservableClasses_VariableSettings>();
        }
    }
    [SerializeField] private ObservableClasses_VariableSettings PRIVATE_my_VariableSettings;


    public void SET_itemsFromMultiplayerBridge(IEnumerable<Vector3> _content)
    {
        this.items.Clear();
        this.items.AddRange(_content);
        this.OnUpdate?.Invoke(this);
    }

    public void SET_items(IEnumerable<Vector3> _content)
    {
        this.items.Clear();
        this.items.AddRange(_content);

        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);
    }

    void Awake()
    {
        this.PRIVATE_my_VariableSettings = gameObject.GetComponent<ObservableClasses_VariableSettings>();
        this.cached_my_VariableSettings = true;

        float _resync_rate = this.my_VariableSettings.auto_resync_rate;
        if (_resync_rate > 0)
            InvokeRepeating("synchronize", _resync_rate, _resync_rate);
    }


    /// <summary>
    /// e.g. 1.5,2.3,-1.6,1.7,2.9,3.6 will set items to (1.5,2.3,-1.6),(1.7,2.9,3.6)
    /// </summary>
    public void importFromFloatString(string _string)
    {
        float[] _float_values = _string.Split(',').Select(x => float.Parse(x)).ToArray();

        List<Vector3> _new_points = new List<Vector3>();
        for(int i = 0; i < _float_values.Length / 3; i += 1)
        {
            float _x = _float_values[i * 3 + 0];
            float _y = _float_values[i * 3 + 1];
            float _z = _float_values[i * 3 + 2];

            _new_points.Add(new Vector3(_x, _y, _z));
        }

        this.SET_items(_new_points);
    }

    public string exportToFloatString()
    {
        string _string = "";
        foreach(Vector3 _v3 in this.items)
        {
            if(_string.Length == 0)
                _string = string.Format("{0},{1},{2}", _v3.x, _v3.y, _v3.z);
            else
                _string = _string + string.Format(",{0},{1},{2}", _v3.x, _v3.y, _v3.z);
            
        }

        return _string;
    }


    public Vector3 this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int Count {get { return this.items.Count(); }}

    public bool IsReadOnly{get { return false; }}

    public void Add(Vector3 item)
    {
        this.items.Add(item);

        if (my_VariableSettings.synchronise_immediately)
            this.synchronize();

        OnUpdate?.Invoke(this);
    }

    public void AddRange(IEnumerable<Vector3> _items)
    {
        this.items.AddRange(_items);

        if(this.my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);
    }
    

    public void Clear()
    {
        this.items.Clear();

        if (my_VariableSettings.synchronise_immediately)
            this.synchronize();
        OnUpdate?.Invoke(this);
    }

    public bool Contains(Vector3 item)
    {
        return this.items.Contains(item);
    }

    public void CopyTo(Vector3[] array, int arrayIndex)
    {
        this.items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<Vector3> GetEnumerator()
    {
        return this.items.GetEnumerator();
    }

    public int IndexOf(Vector3 item)
    {
        return this.items.IndexOf(item);
    }

    public void Insert(int index, Vector3 item)
    {
        this.items.Insert(index, item);

        if (this.my_VariableSettings.synchronise_immediately)
            this.synchronize();
        this.OnUpdate?.Invoke(this);

    }

    public bool Remove(Vector3 item)
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
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.items.GetEnumerator();
    }




    public static void paste(IEnumerable<ObservableVector3List> _sources, IEnumerable<ObservableVector3List> _destinations)
    {
        var _sources_list = new List<ObservableVector3List>(_sources);
        var _destination_list = new List<ObservableVector3List>(_destinations);

        if (_sources_list.Count != _destination_list.Count)
        {
            GlobalFunctions.print("_sources.Count() != _destinations.Count()", null);
            return;
        }

        for (int i = 0; i < _sources_list.Count; i += 1)
        {
            ObservableVector3List.paste(_sources_list[i], _destination_list[i]);
        }
    }

    public static void paste(ObservableVector3List _source, ObservableVector3List _destination)
    {
        _destination.items = new List<Vector3>(_source.items);

        if (_destination.my_VariableSettings.synchronise_immediately)
            _destination.synchronize();

        _destination.OnUpdate?.Invoke(_destination);
    }


}
