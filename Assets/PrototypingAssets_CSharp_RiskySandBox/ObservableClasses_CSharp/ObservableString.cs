 using System;using System.Collections.Generic;

public partial class ObservableString
{
	public static implicit operator string(ObservableString _ObservableString) { return _ObservableString.value; }//converts the ObservableString into a regular string

	private static Dictionary<ObservableString, Dictionary<string, string>> save_dictionary = new Dictionary<ObservableString, Dictionary<string, string>>();

	
	public event Action<ObservableString> OnUpdate;//update event...
	public event Action<ObservableString> Onsynchronize;//the code that needs to run in order to sync this value to the clients (or server)



	public int Length { get { return this.PRIVATE_value.Length; } }
	public int Count { get { return this.PRIVATE_value.Length; } }


	public string value
	{
		get { return this.PRIVATE_value; }
		set { this.SET_value(value, this.my_VariableSettings.synchronise_immediately); }
	}

	public string previous_value { get { return this.PRIVATE_previous_value; } }

	
	public void synchronize()
    {
		if (debugging)
			GlobalFunctions.print("called synchronize!", this);
		Onsynchronize?.Invoke(this);
	}

	public void SET_valueFromMultiplayerBridge(string _value)
    {
		this.SET_value(_value,false);
    }

	void SET_value(string _new_value, bool _synchronize)
	{
		if (debugging)
			GlobalFunctions.print("ObservableString.SET_value(" + _new_value + ")", this);

		this.PRIVATE_previous_value = this.PRIVATE_value;
		this.PRIVATE_value = _new_value;

		if (_synchronize == true)
			this.synchronize();

		OnUpdate?.Invoke(this);
	}


	//TODO - save load previous value???
	/// <summary>
    /// save the values into the cache
    /// </summary>
	public void saveToCache(string _dictionary_key)
    {
		if(ObservableString.save_dictionary.ContainsKey(this))
        {
			ObservableString.save_dictionary[this][_dictionary_key] = this.value;
			return;
        }
		Dictionary<string, string> _new_dictionary = new Dictionary<string, string>();
		_new_dictionary[_dictionary_key] = this.value;
		ObservableString.save_dictionary[this] = _new_dictionary;
    }

	/// <summary>
	/// load values from the cache
	/// </summary>
	public bool TRY_loadFromCache(string _dictionary_key)
    {
		if(ObservableString.save_dictionary.TryGetValue(this,out Dictionary<string,string> _my_data))
        {
			if(_my_data.TryGetValue(_dictionary_key,out string _value))
            {
				this.value = _value;
				return true;
            }
        }
		return false;
    }


	public void randomize(int _max_length)
    {
		this.randomize(_max_length, "");
    }

	/// <summary>
    /// sets this string to a random value 
    /// </summary>
	public void randomize(int _max_length,string _characters = "")
    {
		string _random_value = "";

		if (string.IsNullOrEmpty(_characters))
		{
			_characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		}

		Random random = new Random();

		for (int i = 0; i < _max_length; i++)
		{
			int index = random.Next(_characters.Length);
			_random_value = _random_value + _characters[index];
		}

		this.value = _random_value;
    }


	public static void paste(IEnumerable<ObservableString> _sources,IEnumerable<ObservableString> _destinations)
    {
		var _sources_list = new List<ObservableString>(_sources);
		var _destination_list = new List<ObservableString>(_destinations);

		if (_sources_list.Count != _destination_list.Count)
		{
			GlobalFunctions.print("_sources.Count() != _destinations.Count()", null);
			return;
		}

		for (int i = 0; i < _sources_list.Count; i += 1)
		{
			ObservableString.paste(_sources_list[i], _destination_list[i]);
		}
	}

	public static void paste(ObservableString _source, ObservableString _destination)
	{
		_destination.PRIVATE_previous_value = _source.PRIVATE_previous_value;
		_destination.PRIVATE_value = _source.PRIVATE_value;

		if (_destination.my_VariableSettings.synchronise_immediately)
			_destination.synchronize();

		_destination.OnUpdate?.Invoke(_destination);
	}

	

}