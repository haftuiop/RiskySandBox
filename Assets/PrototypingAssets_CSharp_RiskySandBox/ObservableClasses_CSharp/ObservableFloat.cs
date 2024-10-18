using System;using System.Collections.Generic;
public partial class ObservableFloat
{
	public static implicit operator float(ObservableFloat _ObservableFloat) { return _ObservableFloat.value; }//converts the ObservableFloat into a regular float
	public static implicit operator string(ObservableFloat _ObservableFloat) { return "" + _ObservableFloat.value; }

	private static Dictionary<ObservableFloat, Dictionary<string, float>> save_dictionary_value = new Dictionary<ObservableFloat, Dictionary<string, float>>();
	private static Dictionary<ObservableFloat, Dictionary<string, float>> save_dictionary_min_value = new Dictionary<ObservableFloat, Dictionary<string, float>>();
	private static Dictionary<ObservableFloat, Dictionary<string, float>> save_dictionary_max_value = new Dictionary<ObservableFloat, Dictionary<string, float>>();

	public event Action<ObservableFloat> OnUpdate;//update event..
	public event Action<ObservableFloat> Onsynchronize;//the code that needs to run in order to sync this value to the clients (or server)

	public event Action<ObservableFloat> OnclampToMin;
	public event Action<ObservableFloat> OnclampToMax;



	public float value
	{
		get { return this.PRIVATE_value; }
		set { this.SET_value(value, this.my_VariableSettings.synchronise_immediately); }
	}
	public float previous_value { get { return this.PRIVATE_previous_value; } }
	public float delta_value { get { return this.PRIVATE_value - this.PRIVATE_previous_value; } }


	public float min_value
	{
		get { return this.PRIVATE_min_value; }
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
	public float max_value
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

	public float ilerp_value
	{
		get
		{
			if (this.min_value == this.max_value)
				return 0f;
			float _return_value = (this.value - this.min_value) / (float)(this.max_value - this.min_value);
			if (_return_value > 1f)
				return 1f;
			if (_return_value < 0f)
				return 0f;
			return _return_value;
		}
	}



	public void synchronize()
	{
		if (debugging)
			GlobalFunctions.print("called synchronize!", this);

		Onsynchronize?.Invoke(this);
	}


	public void SET_valueFromMultiplayerBridge(float _new_value,float _min_value,float _max_value)
	{
		if (this.min_value != _min_value)
			this.PRIVATE_min_value = _min_value;

		if (this.max_value != _max_value)
			this.PRIVATE_max_value = _max_value;

		SET_value(_new_value, false);
    }

	void SET_value(float _new_value, bool _synchronize)
	{
		if (_new_value < this.min_value)//clamp to the min value!
		{
			if (this.debugging)
				GlobalFunctions.print("clamping to the min value (" + this.min_value + ")", this,_new_value, _synchronize);
			_new_value = this.min_value;
			OnclampToMin?.Invoke(this);
		}


		else if (_new_value > this.max_value)//clamp to the max value!
		{
			if (this.debugging)
				GlobalFunctions.print("clamping the the max_value (" + this.max_value + ")", this, _new_value, _synchronize);
			_new_value = this.max_value;
			OnclampToMax?.Invoke(this);
		}

		else
        {
			if(this.debugging)
				GlobalFunctions.print("setting the value to "+_new_value, this,_new_value,_synchronize);

		}

			

		//actually update the value...
		this.PRIVATE_previous_value = this.PRIVATE_value;
		this.PRIVATE_value = _new_value;

		if (_synchronize == true)
			this.synchronize();

		OnUpdate?.Invoke(this);
	}

	//TODO - save/load previous value???
	public void saveToCache(string _dictionary_key)
    {
		if(ObservableFloat.save_dictionary_value.ContainsKey(this))
        {
			ObservableFloat.save_dictionary_value[this][_dictionary_key] = this.value;
			ObservableFloat.save_dictionary_min_value[this][_dictionary_key] = this.min_value;
			ObservableFloat.save_dictionary_max_value[this][_dictionary_key] = this.max_value;

			return;
		}

		Dictionary<string, float> _value_dict = new Dictionary<string, float>();
		_value_dict[_dictionary_key] = this.value;

		Dictionary<string, float> _min_value_dict = new Dictionary<string, float>();
		_min_value_dict[_dictionary_key] = this.min_value;

		Dictionary<string, float> _max_value_dict = new Dictionary<string, float>();
		_max_value_dict[_dictionary_key] = this.max_value;

		ObservableFloat.save_dictionary_value[this] = _value_dict;
		ObservableFloat.save_dictionary_min_value[this] = _min_value_dict;
		ObservableFloat.save_dictionary_max_value[this] = _max_value_dict;

	}

	public bool TRY_loadFromCache(string _dictionary_key)
    {
		if(ObservableFloat.save_dictionary_value.TryGetValue(this,out Dictionary<string,float> _my_Data))
        {
			if(_my_Data.TryGetValue(_dictionary_key,out float _value))
            {
				this.PRIVATE_min_value = ObservableFloat.save_dictionary_min_value[this][_dictionary_key];
				this.PRIVATE_max_value = ObservableFloat.save_dictionary_max_value[this][_dictionary_key];
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


	public static bool operator ==(ObservableFloat _float, object other)
	{
		if (ReferenceEquals(_float, other))
			return true;

		if (ReferenceEquals(_float, null))
		{
			if (ReferenceEquals(other, null))
				return true;
			throw new ArgumentNullException("just tried to do ObservableFloat == <other type> but the ObservableFloat was null...");
		}

		if (ReferenceEquals(other, null))
			return ReferenceEquals(_float, null);

		GlobalFunctions.printWarning("HIGHLY recommended to use ObservableFloat.value == <other type> (instead of ObservableFloat == <other type>)", null);

		if (other is int)
			return _float.value == (int)other;

		if (other is float)
			return _float.value == (float)other;

		if (other is ObservableFloat)
			return _float.value == ((ObservableFloat)other).value;

		throw new Exception("unimplemented comparison between ObservableFloat and '" + other.GetType() +"'... maybe try doing ObservableFloat.value == <other type> or modify this function");
	}
	public static bool operator !=(ObservableFloat _float, object other)
	{
		return !(_float == other);
	}


	public static void paste(IEnumerable<ObservableFloat> _sources,IEnumerable<ObservableFloat> _destinations)
    {
		var _sources_list = new List<ObservableFloat>(_sources);
		var _destination_list = new List<ObservableFloat>(_destinations);

		if (_sources_list.Count != _destination_list.Count)
		{
			GlobalFunctions.print("_sources.Count() != _destinations.Count()", null);
			return;
		}

		for (int i = 0; i < _sources_list.Count; i += 1)
		{
			ObservableFloat.paste(_sources_list[i], _destination_list[i]);
		}
	}

	public static void paste(ObservableFloat _source,ObservableFloat _destination)
    {
		_destination.PRIVATE_previous_value = _source.PRIVATE_previous_value;
		_destination.PRIVATE_min_value = _source.PRIVATE_min_value;
		_destination.PRIVATE_max_value = _source.PRIVATE_max_value;
		_destination.PRIVATE_value = _source.PRIVATE_value;

		if (_destination.my_VariableSettings.synchronise_immediately)
			_destination.synchronize();

		_destination.OnUpdate?.Invoke(_destination);
	}



}

