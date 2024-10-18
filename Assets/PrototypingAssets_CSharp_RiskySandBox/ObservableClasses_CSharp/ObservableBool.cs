using System;using System.Collections.Generic;
public partial class ObservableBool
{
	public static implicit operator bool(ObservableBool _ObservableBool) { return _ObservableBool.value; }//converts the ObservableBool into a regular bool...
	public static implicit operator string(ObservableBool _ObservableBool) { if (_ObservableBool.value == true) return "true"; return "false"; }

	private static Dictionary<ObservableBool, Dictionary<string, bool>> save_dictionary = new Dictionary<ObservableBool, Dictionary<string, bool>>();


	//update functions...
	public event Action<ObservableBool> OnUpdate;//invoked whenever the value changes...
	public event Action<ObservableBool> OnUpdate_true;//invoked when the value becomes true...
	public event Action<ObservableBool> OnUpdate_false;//invoked when the value becomes false....


	public event Action<ObservableBool> Onsynchronize;//the code that needs to run in order to sync this value to the clients (or server)


	



	public bool value
	{
		get { return this.PRIVATE_value; }
		set { this.SET_value(value, this.my_VariableSettings.synchronise_immediately); }
	}

	public bool previous_value { get { return this.PRIVATE_previous_value; } }



	public void synchronize()
	{
		if (debugging)
			GlobalFunctions.print("called synchronize!", this);

		Onsynchronize?.Invoke(this);
	}

	public void SET_valueFromMultiplayerBridge(bool _new_value)
    {
		this.SET_value(_new_value,false);
    }

	void SET_value(bool _new_value,bool _synchronize)
	{
		this.PRIVATE_previous_value = this.PRIVATE_value;
		this.PRIVATE_value = _new_value;

		if (debugging)
			GlobalFunctions.print("ObservableBool.SET_value(" + _new_value+")", this);

		if (_synchronize == true)
			this.synchronize();

        //TODO - we want to do something like this - to stop stupid errors breaking the entire game? do this for all classes?
        //foreach (Delegate _delegate in this.OnUpdate.GetInvocationList())
        //{
        //    try
        //    {
        //        _delegate.DynamicInvoke(this);
        //    }
        //    catch
        //    {
				//print the error for debugging...
        //    }

        //}

        OnUpdate?.Invoke(this);
		if (_new_value == true)
			OnUpdate_true?.Invoke(this);//tell everyone that this is now true?
		else
			OnUpdate_false?.Invoke(this);//tell everyone that this is now false?
	}

	/// <summary>
    /// this.value = true
    /// </summary>
	public void SET_true()
    {
		this.value = true;
    }

	/// <summary>
    /// this.value = false
    /// </summary>
	public void SET_false()
    {
		this.value = false;
    }

	/// <summary>
    /// this.value = !this.value
    /// </summary>
	public void toggle()
    {
		this.value = !this.value;
    }


	//TODO - save/load previous value????
	public void saveToCache(string _dictionary_key)
    {
		if(ObservableBool.save_dictionary.ContainsKey(this))
        {
			ObservableBool.save_dictionary[this][_dictionary_key] = this.value;
			return;
        }
		Dictionary<string,bool> _new_dict = new Dictionary<string, bool>();
		_new_dict[_dictionary_key] = this.value;

		ObservableBool.save_dictionary[this] = _new_dict;
    }

	public bool TRY_loadFromCache(string _dictionary_key)
    {
		if (ObservableBool.save_dictionary.TryGetValue(this, out Dictionary<string, bool> _my_data))
		{
			if (_my_data.TryGetValue(_dictionary_key, out bool _saved_value))
			{
				this.value = _saved_value;
				return true;
			}
		}
		return false;
    }


	public static void paste(IEnumerable<ObservableBool> _sources,IEnumerable<ObservableBool> _destinations)
    {
		var _sources_list = new List<ObservableBool>(_sources);
		var _destination_list = new List<ObservableBool>(_destinations);

		if (_sources_list.Count != _destination_list.Count)
		{
			GlobalFunctions.print("_sources.Count() != _destinations.Count()", null);
			return;
		}

		for (int i = 0; i < _sources_list.Count; i += 1)
		{
			ObservableBool.paste(_sources_list[i], _destination_list[i]);
		}
	}
	public static void paste(ObservableBool _source,ObservableBool _destination)
    {
		_destination.PRIVATE_previous_value = _source.previous_value;
		_destination.PRIVATE_value = _source.PRIVATE_value;
		
		if (_destination.my_VariableSettings.synchronise_immediately)
			_destination.synchronize();

		_destination.OnUpdate?.Invoke(_destination);
    }




}

