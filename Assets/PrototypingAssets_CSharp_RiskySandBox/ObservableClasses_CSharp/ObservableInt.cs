using System;using System.Collections.Generic;
public partial class ObservableInt
{
	/// <summary>
    /// all the ObservableInts that we are currently overriding the ui
    /// </summary>
	public static List<ObservableInt> all_instances_overrideUI_flag = new List<ObservableInt>();

	private static Dictionary<ObservableInt, Dictionary<string, int>> save_dictionary_value = new Dictionary<ObservableInt, Dictionary<string, int>>();
	private static Dictionary<ObservableInt, Dictionary<string, int>> save_dictionary_min_value = new Dictionary<ObservableInt, Dictionary<string, int>>();
	private static Dictionary<ObservableInt, Dictionary<string, int>> save_dictionary_max_value = new Dictionary<ObservableInt, Dictionary<string, int>>();


	public static implicit operator int(ObservableInt _ObservableInt) { return _ObservableInt.value; }//converts the ObservableInt into a regular int
	public static implicit operator string(ObservableInt _ObservableInt) { return "" + _ObservableInt.value; }//converts the ObservableInt into a string
    public override string ToString()
    {
		return "" + this.value;
    }

    /// <summary>
    /// event that is invoked whenever the value, min_value and max_value are changed...
    /// </summary>
    public event Action<ObservableInt> OnUpdate;
	/// <summary>
    /// the code that needs to run in order to syncronise the value to the connected client(s) (or server)
    /// </summary>
	public event Action<ObservableInt> Onsynchronize;//the code that needs to run in order to sync this value to the clients (or server)

	public event Action<ObservableInt> OnoverrideUI;

	/// <summary>
	/// the current value of the integer...
	/// </summary>
	public int value
	{
		get { return this.PRIVATE_value; }
		set { SET_value(value, this.my_VariableSettings.synchronise_immediately); }
	}

	/// <summary>
    /// what was the value of the variable before it changed...
    /// </summary>
	public int previous_value { get { return this.PRIVATE_previous_value; } }

	/// <summary>
    /// how much as the value changed...
    /// </summary>
	public int delta_value { get { return this.value - this.PRIVATE_previous_value; } }

	/// <summary>
	/// the smallest value the integer can have...
	/// </summary>
	public int min_value
	{
		get { return PRIVATE_min_value; }
		set
		{
			this.PRIVATE_min_value = value;
			if (value > this.PRIVATE_value)
				this.PRIVATE_value = value;

			if (this.my_VariableSettings.synchronise_immediately)
				this.synchronize();

			OnUpdate?.Invoke(this);
		}
	}
	/// <summary>
	/// the largest value the integer can have
	/// </summary>
	public int max_value
	{
		get { return this.PRIVATE_max_value; }
		set
		{
			this.PRIVATE_max_value = value;
			if (value < this.PRIVATE_value)
				this.PRIVATE_value = value;

			if (this.my_VariableSettings.synchronise_immediately)
				this.synchronize();

			OnUpdate?.Invoke(this);

		}
	}


	public int ui_value
    {
        get
		{
			if (overrideUI_flag == true)
				return this.overrideUI_value;
			return this.value;
		}
    }
	bool overrideUI_flag;
	int overrideUI_value;

	public void overrideUI(int _new_value)
    {
		if (this.debugging)
			GlobalFunctions.print("overriding the ui", this, _new_value);
		this.overrideUI_flag = true;
		overrideUI_value = _new_value;
		ObservableInt.all_instances_overrideUI_flag.Add(this);
		this.OnoverrideUI?.Invoke(this);
    }

	public void resetUI()
    {
		this.overrideUI_flag = false;
		overrideUI_value = this.value;
		ObservableInt.all_instances_overrideUI_flag.Remove(this);
		this.OnoverrideUI?.Invoke(this);
	}


	public static void resetAllUIs()
    {
		foreach(ObservableInt _Int in new List<ObservableInt>(all_instances_overrideUI_flag))
        {
			_Int.resetUI();
        }
    }



	/// <summary>
	/// invokes the Onsynchronize event (you need to subscribe code to the event in order to actually do this...)
	/// </summary>
	public void synchronize()
	{
		if (debugging)
			GlobalFunctions.print("called synchronize!", this);
		Onsynchronize?.Invoke(this);
	}

	/// <summary>
    /// set the value using the MultiplayerBridge...
    /// </summary>
	public void SET_valueFromMultiplayerBridge(int _value,int _min_value,int _max_value)
    {
		if (this.min_value != _min_value)
			this.PRIVATE_min_value = _min_value;
		if (this.max_value != _max_value)
			this.PRIVATE_max_value = _max_value;
		SET_value(_value, false);
    }

	void SET_value(int _new_value, bool _syncronize)
	{
		if (_new_value < this.min_value)
		{
			if (this.debugging) { GlobalFunctions.print("clamping to min value (" + this.min_value + ")", this, _new_value, _syncronize); }
			_new_value = this.min_value;
		}
		else if (_new_value > this.max_value)
		{
			if (this.debugging) { GlobalFunctions.print("clamping to max value (" + this.max_value + ")", this, _new_value, _syncronize); }
			_new_value = this.max_value;
		}
		else if(debugging) { GlobalFunctions.print("setting value to " + _new_value, this, _new_value, _syncronize); }

		this.PRIVATE_previous_value = this.PRIVATE_value;
		this.PRIVATE_value = _new_value;


		if (_syncronize)
			this.synchronize();

		OnUpdate?.Invoke(this);
	}


	public void increase(int _increase)
	{
		if (this.debugging)
			GlobalFunctions.print("increasing by " + _increase, this,_increase);
		this.value += _increase;
	}

	public void decrease(int _decrease)
	{
		if (this.debugging)
			GlobalFunctions.print("decreasing by " + _decrease, this, _decrease);
		this.value -= _decrease;
	}

	//TODO save/load previous value????
	public void saveToCache(string _dictionary_key)
    {
		if(ObservableInt.save_dictionary_value.ContainsKey(this))
        {
			ObservableInt.save_dictionary_value[this][_dictionary_key] = this.value;
			ObservableInt.save_dictionary_min_value[this][_dictionary_key] = this.min_value;
			ObservableInt.save_dictionary_max_value[this][_dictionary_key] = this.max_value;

			return;
        }

		Dictionary<string, int> _value_dict = new Dictionary<string, int>();
		_value_dict[_dictionary_key] = this.value;
		ObservableInt.save_dictionary_value[this] = _value_dict;

		Dictionary<string, int> _min_value_dict = new Dictionary<string, int>();
		_min_value_dict[_dictionary_key] = this.min_value;
		ObservableInt.save_dictionary_min_value[this] = _min_value_dict;

		Dictionary<string, int> _max_value_dict = new Dictionary<string, int>();
		_max_value_dict[_dictionary_key] = this.max_value;
		ObservableInt.save_dictionary_max_value[this] = _max_value_dict;
	}

	public bool TRY_loadFromCache(string _dictionary_key)
    {
		if(ObservableInt.save_dictionary_value.TryGetValue(this,out Dictionary<string,int> _my_data))
        {
			if(_my_data.TryGetValue(_dictionary_key,out int _value))
            {
				this.PRIVATE_min_value = ObservableInt.save_dictionary_min_value[this][_dictionary_key];
				this.PRIVATE_max_value = ObservableInt.save_dictionary_max_value[this][_dictionary_key];
				this.value = _value;
				return true;
			}
        }
		return false;
    }



	//functions to make sure comparisons work as expected...
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

   
    public static bool operator ==(ObservableInt _int, object other)
	{
		if (ReferenceEquals(_int, other))
			return true;

		if (ReferenceEquals(_int, null))
        {
			if (ReferenceEquals(other, null))
				return true;
			throw new ArgumentNullException("just tried to do ObservableInt == <other type> but the ObservableInt was null...");
		}
			
		if (ReferenceEquals(other, null))
			return ReferenceEquals(_int, null);

		GlobalFunctions.printWarning("HIGHLY recommended to use ObservableInt.value == <other type> (instead of ObservableInt == <other type>)", null);

		if (other is int)
			return _int.value == (int)other;

		if (other is float)
			return _int.value == (float)other;

		if (other is ObservableInt)
			return _int.value == ((ObservableInt)other).value;


		throw new Exception("unimplemented comparison between ObservableInt and '" + other.GetType()+"'... maybe try doing ObservableInt.value == <other type> or modify this function");
	}
	public static bool operator !=(ObservableInt _int,object other)
	{
		return !(_int == other);
	}


	public static void paste(IEnumerable<ObservableInt> _sources, IEnumerable<ObservableInt> _destinations)
    {
		var _sources_list = new List<ObservableInt>(_sources);
		var _destination_list = new List<ObservableInt>(_destinations);

		if (_sources_list.Count != _destination_list.Count)
		{
			GlobalFunctions.print("_sources.Count() != _destinations.Count()", null);
			return;
		}

		for (int i = 0; i < _sources_list.Count; i += 1)
		{
			ObservableInt.paste(_sources_list[i], _destination_list[i]);
		}

	}

	public static void paste(ObservableInt _source,ObservableInt _destination)
    {
		_destination.PRIVATE_min_value = _source.PRIVATE_min_value;
		_destination.PRIVATE_max_value = _source.PRIVATE_max_value;
		_destination.PRIVATE_value = _source.PRIVATE_value;

		if (_destination.my_VariableSettings.synchronise_immediately)
			_destination.synchronize();

		_destination.OnUpdate?.Invoke(_destination);
	}



}



