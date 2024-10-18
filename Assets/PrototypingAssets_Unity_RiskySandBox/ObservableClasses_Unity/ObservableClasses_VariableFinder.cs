using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif



/// <summary>
/// a script that searched through GameObject(s) to find all the ObservableBools,floats,ints,strings,Vector3s accociated with the GameObject(s)
/// </summary>
public partial class ObservableClasses_VariableFinder : MonoBehaviour
{
    

    public static event Action<ObservableClasses_VariableFinder> OnVariableUpdate_copy_target_STATIC;
    public static ObservableClasses_VariableFinder copy_target
    {
        get { return ObservableClasses_VariableFinder.PRIVATE_copy_target; }
        set
        {
            ObservableClasses_VariableFinder.PRIVATE_copy_target = value;
            ObservableClasses_VariableFinder.OnVariableUpdate_copy_target_STATIC?.Invoke(value);
        }
    }
    static ObservableClasses_VariableFinder PRIVATE_copy_target;


    public event Action<ObservableClasses_VariableFinder> OnAwakeSearchCompleted;

    [SerializeField] bool debugging = false;

    public List<ObservableBool> my_ObservableBools = new List<ObservableBool>();
    public List<ObservableFloat> my_ObservableFloats = new List<ObservableFloat>();
    public List<ObservableInt> my_ObservableInts = new List<ObservableInt>();
    public List<ObservableString> my_ObservableStrings = new List<ObservableString>();
    public List<ObservableVector3> my_ObservableVector3s = new List<ObservableVector3>();

    //================================lists=======================================
    public List<ObservableBoolList> my_ObservableBoolLists = new List<ObservableBoolList>();
    public List<ObservableFloatList> my_ObservableFloatLists = new List<ObservableFloatList>();
    public List<ObservableIntList> my_ObservableIntLists = new List<ObservableIntList>();
    public List<ObservableStringList> my_ObservableStringLists = new List<ObservableStringList>();
    public List<ObservableVector3List> my_ObservableVector3Lists = new List<ObservableVector3List>();



    [SerializeField] List<Transform> search_Transforms = new List<Transform>();


    public bool search_at_Awake
    {
        get { return this.PRIVATE_search_at_Awake;}
    }
    [SerializeField] bool PRIVATE_search_at_Awake = true;

    public bool search_at_Awake_completed
    {
        get { return this.PRIVATE_search_at_Awake_completed; }
    }
    [SerializeField] bool PRIVATE_search_at_Awake_completed;

    private void Awake()
    {
        if(search_at_Awake)
        {
            search(this);
            PRIVATE_search_at_Awake_completed = true;
            OnAwakeSearchCompleted?.Invoke(this);
        }
    }

    /// <summary>
    /// ObservableClasses_VariableFinder.copy_target = this;
    /// </summary>
    public void becomeCopyTarget()
    {
        ObservableClasses_VariableFinder.copy_target = this;
    }

    public void copyFromCopyTarget()
    {
        if (copy_target == null)
        {
            if (this.debugging)
                GlobalFunctions.print("copy_target is null... returning",this);
            return;
        }
        if(copy_target == this)
        {
            if (this.debugging)
                GlobalFunctions.print("i am the copy_target??? returning...",this);
            return;
        }
        if (this.debugging)
            GlobalFunctions.print("copying from the copy_target!", this);
        ObservableClasses_VariableFinder.pasteValues(copy_target, this);
    }


    public static void search(ObservableClasses_VariableFinder _VariableFinder)
    {
        _VariableFinder.my_ObservableBools.Clear();
        _VariableFinder.my_ObservableFloats.Clear();
        _VariableFinder.my_ObservableInts.Clear();
        _VariableFinder.my_ObservableStrings.Clear();
        _VariableFinder.my_ObservableVector3s.Clear();
        _VariableFinder.my_ObservableIntLists.Clear();
        _VariableFinder.my_ObservableVector3Lists.Clear();
        _VariableFinder.my_ObservableStringLists.Clear();

        bool _added_variables = false;
        List<Transform> _to_search = new List<Transform>(_VariableFinder.search_Transforms.Where(x => x != null));
        while (_to_search.Count > 0)
        {
            //pop the first one...
            _added_variables |= addBools(_VariableFinder, _to_search[0]);
            _added_variables |= addFloats(_VariableFinder, _to_search[0]);
            _added_variables |= addInts(_VariableFinder, _to_search[0]);
            _added_variables |= addStrings(_VariableFinder, _to_search[0]);
            _added_variables |= addVector3s(_VariableFinder, _to_search[0]);


            _added_variables |= addObservableBoolLists(_VariableFinder, _to_search[0]);
            _added_variables |= addObservableFloatLists(_VariableFinder, _to_search[0]);
            _added_variables |= addObservableIntLists(_VariableFinder, _to_search[0]);
            _added_variables |= addObservableStringLists(_VariableFinder, _to_search[0]);
            _added_variables |= addObservableVector3Lists(_VariableFinder, _to_search[0]);

            for (int i = 0; i < _to_search[0].childCount; i += 1)
            {
                //add the child to the serach transform...
                _to_search.Add(_to_search[0].GetChild(i));
            }

            _to_search.RemoveAt(0);
        }
#if UNITY_EDITOR
        if (_added_variables == true)
            EditorUtility.SetDirty(_VariableFinder);
#endif
    }

    static bool addBools(ObservableClasses_VariableFinder _MultiplayerVariables, Transform _Transform)
    {
        var _new_variables = _Transform.GetComponents<ObservableBool>();
        foreach(var _ObservableVariable in _new_variables)
        {
            _MultiplayerVariables.my_ObservableBools.Add(_ObservableVariable);
        }
        return _new_variables.Count() > 0;
    }
    static bool addFloats(ObservableClasses_VariableFinder _MultiplayerVariables, Transform _Transform)
    {
        var _new_variables = _Transform.GetComponents<ObservableFloat>();
        foreach (var _ObservableVariable in _new_variables)
        {
            _MultiplayerVariables.my_ObservableFloats.Add(_ObservableVariable);
        }
        return _new_variables.Count() > 0;
    }
    static bool addInts(ObservableClasses_VariableFinder _MultiplayerVariables, Transform _Transform)
    {
        var _new_variables = _Transform.GetComponents<ObservableInt>();
        foreach (var _ObservableVariable in _new_variables)
        { 
            _MultiplayerVariables.my_ObservableInts.Add(_ObservableVariable);
        }
        return _new_variables.Count() > 0;
    }
    static bool addStrings(ObservableClasses_VariableFinder _MultiplayerVariables, Transform _Transform)
    {
        var _new_variables = _Transform.GetComponents<ObservableString>();
        foreach (var _ObservableVariable in _new_variables)
        {
            _MultiplayerVariables.my_ObservableStrings.Add(_ObservableVariable);
        }
        return _new_variables.Count() > 0;
    }
    static bool addVector3s(ObservableClasses_VariableFinder _MultiplayerVariables, Transform _Transform)
    {
        var _new_variables = _Transform.GetComponents<ObservableVector3>();
        foreach (var _ObservableVariable in _new_variables)
        {
            _MultiplayerVariables.my_ObservableVector3s.Add(_ObservableVariable);
        }
        return _new_variables.Count() > 0;
    }


    //=================lists===============================================================================

    static bool addObservableBoolLists(ObservableClasses_VariableFinder _VariableFinder, Transform _Transform)
    {
        var _new_variables = _Transform.GetComponents<ObservableBoolList>();
        foreach (var _ObservableVariable in _new_variables)
        {
            _VariableFinder.my_ObservableBoolLists.Add(_ObservableVariable);
        }
        return _new_variables.Count() > 0;
    }

    static bool addObservableFloatLists(ObservableClasses_VariableFinder _VariableFinder, Transform _Transform)
    {
        var _new_variables = _Transform.GetComponents<ObservableFloatList>();
        foreach (var _ObservableVariable in _new_variables)
        {
            _VariableFinder.my_ObservableFloatLists.Add(_ObservableVariable);
        }
        return _new_variables.Count() > 0;
    }

    static bool addObservableIntLists(ObservableClasses_VariableFinder _VariableFinder,Transform _Transform)
    {
        var _new_variables = _Transform.GetComponents<ObservableIntList>();
        foreach (var _ObservableVariable in _new_variables)
        {
            _VariableFinder.my_ObservableIntLists.Add(_ObservableVariable);
        }
        return _new_variables.Count() > 0;
    }

    static bool addObservableStringLists(ObservableClasses_VariableFinder _VariableFinder, Transform _Transform)
    {
        var _new_variables = _Transform.GetComponents<ObservableStringList>();
        foreach(var _ObservableVariable in _new_variables)
        {
            _VariableFinder.my_ObservableStringLists.Add(_ObservableVariable);
        }
        return _new_variables.Count() > 0;
    }
    static bool addObservableVector3Lists(ObservableClasses_VariableFinder _VariableFinder,Transform _Transform)
    {
        var _new_variables = _Transform.GetComponents<ObservableVector3List>();
        foreach (var _ObservableVariable in _new_variables)
        {
            _VariableFinder.my_ObservableVector3Lists.Add(_ObservableVariable);
        }
        return _new_variables.Count() > 0;
    }


    public static void pasteValues(ObservableClasses_VariableFinder _source, ObservableClasses_VariableFinder _destination)
    {
        //TODO DO NOT COPY IF IT IS NOT A COPY PASTE TARGET!
        ObservableBool.paste(_source.my_ObservableBools, _destination.my_ObservableBools);
        ObservableFloat.paste(_source.my_ObservableFloats, _destination.my_ObservableFloats);
        ObservableInt.paste(_source.my_ObservableInts, _destination.my_ObservableInts);
        ObservableString.paste(_source.my_ObservableStrings, _destination.my_ObservableStrings);
        ObservableVector3.paste(_source.my_ObservableVector3s, _destination.my_ObservableVector3s);
        ObservableIntList.paste(_source.my_ObservableIntLists, _destination.my_ObservableIntLists);
        ObservableVector3List.paste(_source.my_ObservableVector3Lists, _destination.my_ObservableVector3Lists);
        //TODO - observablefloat,bool,string lists
    }


}



#if UNITY_EDITOR

[CustomEditor(typeof(ObservableClasses_VariableFinder))]
public partial class Inspector_ObservableClasses_VariableFinder : Editor
{

    ObservableClasses_VariableFinder my_PrototypingAssets_MultiplayerVariables { get { return (ObservableClasses_VariableFinder)target; } }
    public override void OnInspectorGUI()
    {

        // Display default inspector gui
        DrawDefaultInspector();


        if (GUILayout.Button("search"))
        {
            ObservableClasses_VariableFinder.search(my_PrototypingAssets_MultiplayerVariables);
        }

    }
}

#endif