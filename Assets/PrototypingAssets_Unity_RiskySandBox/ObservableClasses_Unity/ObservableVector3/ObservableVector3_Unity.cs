//extra functions of the ObservableVector3 class so it can be used with the Unity Game Engine
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(ObservableClasses_VariableSettings))]
public partial class ObservableVector3 : MonoBehaviour
{
	public static implicit operator Vector3(ObservableVector3 _ObservableVector3) { return _ObservableVector3.value; }

	private static Dictionary<ObservableVector3, Dictionary<string, Vector3>> cache_dictionary_value = new Dictionary<ObservableVector3, Dictionary<string, Vector3>>();
	private static Dictionary<ObservableVector3, Dictionary<string, Vector3>> cache_dictionary_min_value = new Dictionary<ObservableVector3, Dictionary<string, Vector3>>();
	private static Dictionary<ObservableVector3, Dictionary<string, Vector3>> cache_dictionary_max_value = new Dictionary<ObservableVector3, Dictionary<string, Vector3>>();

	[SerializeField] private bool debugging;

	[SerializeField] private Vector3 PRIVATE_value = new Vector3(0,0,0);
	[SerializeField] private Vector3 PRIVATE_previous_value = new Vector3(0,0,0);
	[SerializeField] Vector3 PRIVATE_min_value = new Vector3(float.MinValue, float.MinValue, float.MinValue);
	[SerializeField] Vector3 PRIVATE_max_value = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

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

	[SerializeField] private UnityEvent<ObservableVector3> OnUpdate_Inspector;




	public Vector3 min_value
    {
		get { return this.PRIVATE_min_value; }
		set
        {

			this.PRIVATE_min_value = value;

			bool _update_value = false;

			float _x = this.x;
			float _y = this.y;
			float _z = this.z;

			if(value.x > this.x)
            {
				_x = value.x;
				_update_value = true;
            }

			if (value.y > this.y)
			{
				_y = value.y;
				_update_value = true;
			}

			if (value.z > this.z)
			{
				_z = value.z;
				_update_value = true;
			}

			if (_update_value)
				this.PRIVATE_value = new Vector3(_x, _y, _z);


			if (this.my_VariableSettings.synchronise_immediately == true)
				this.synchronize();

			OnUpdate?.Invoke(this);
        }
			
    }


	public Vector3 max_value
    {
		get { return this.PRIVATE_max_value; }
		set
        {
			this.PRIVATE_max_value = value;

			bool _update_value = false;

			float _x = this.x;
			float _y = this.y;
			float _z = this.z;

			if (value.x < this.x)
			{
				_x = value.x;
				_update_value = true;
			}

			if(value.y < this.y)
            {
				_y = value.y;
				_update_value = true;
            }

			if(value.z < this.z)
            {
				_z = value.z;
				_update_value = true;
            }

			if (_update_value)
				this.PRIVATE_value = new Vector3(_x, _y, _z);


			if (this.my_VariableSettings.synchronise_immediately == true)
				this.synchronize();

			OnUpdate?.Invoke(this);
        }
    }

	





	/// <summary>
	/// the value of the ObservableVector3...
	/// </summary>
	public Vector3 value
	{
		get { return this.PRIVATE_value; }
		set { SET_value(value, this.my_VariableSettings.synchronise_immediately); }
	}

	/// <summary>
	/// the previous value of the ObservableVector3
	/// </summary>
	public Vector3 previous_value { get { return this.PRIVATE_previous_value; } }

	public void SET_valueFromMultiplayerBridge(Vector3 _new_value,Vector3 _min_value,Vector3 _max_value)
    {
		if(_min_value != this.PRIVATE_min_value)
        {
			this.PRIVATE_min_value = _min_value;
        }

		if(_max_value != this.PRIVATE_max_value)
        {
			this.PRIVATE_max_value = _max_value;
        }

		this.SET_value(_new_value, false);
    }

	void SET_value(Vector3 _new_value,bool _invoke_multiplayer_sync_event)
	{
		if (debugging)
			GlobalFunctions.print("ObservableVector3.SET_value(" + _new_value + ")", this);


		this.PRIVATE_previous_value = this.PRIVATE_value;


		//TODO - add in clamping debug statements... AND events...
		if(_new_value.x < this.min_value.x)
        {
			_new_value.x = this.min_value.x;
        }
		if(_new_value.x > this.max_value.x)
        {
			_new_value.x = this.max_value.x;
        }

		if(_new_value.y < this.min_value.y)
        {
			_new_value.y = this.min_value.y;
        }
		if(_new_value.y > this.max_value.y)
        {
			_new_value.y = this.max_value.y;
        }

		if(_new_value.z < this.min_value.z)
        {
			_new_value.z = this.min_value.z;
        }
		if(_new_value.z > this.max_value.z)
        {
			_new_value.z = this.max_value.z;
        }





		this.PRIVATE_value = _new_value;

		if (_invoke_multiplayer_sync_event)
			this.synchronize();

		OnUpdate?.Invoke(this);
	}

	
	void SET_x(float _new)
    {
		this.value = new Vector3(_new, this.y, this.z);
    }

	void SET_y(float _new)
	{
		this.value = new Vector3(this.x, _new, this.z);
	}

	void SET_z(float _new)
	{
		this.value = new Vector3(this.x, this.y, _new);
	}




	private void Awake()
	{
		this.PRIVATE_my_VariableSettings = GetComponent<ObservableClasses_VariableSettings>();
		this.cached_my_VariableSettings = true;

		if (Application.isEditor || this.OnUpdate_Inspector.GetPersistentEventCount() > 0)
			this.OnUpdate += delegate { OnUpdate_Inspector.Invoke(this); };

		float _resync_rate = this.my_VariableSettings.auto_resync_rate;
		if (_resync_rate > 0)
			InvokeRepeating("synchronize", _resync_rate, _resync_rate);

		

	}

	//TODO save/load the previous value?

	/// <summary>
    /// save the min,max,previous and value into the cache
    /// </summary>
	public void saveToCache(string _dictionary_key)
    {
		if(cache_dictionary_value.ContainsKey(this))
        {
			cache_dictionary_value[this][_dictionary_key] = this.value;
			cache_dictionary_min_value[this][_dictionary_key] = this.min_value;
			cache_dictionary_max_value[this][_dictionary_key] = this.max_value;
			return;
        }
		Dictionary<string, Vector3> _value_dict = new Dictionary<string, Vector3>();
		_value_dict[_dictionary_key] = this.value;
		cache_dictionary_value[this] = _value_dict;

		Dictionary<string, Vector3> _min_value_dict = new Dictionary<string, Vector3>();
		_min_value_dict[_dictionary_key] = this.min_value;
		cache_dictionary_min_value[this] = _min_value_dict;

		Dictionary<string, Vector3> _max_value_dict = new Dictionary<string, Vector3>();
		_max_value_dict[_dictionary_key] = this.max_value;
		cache_dictionary_max_value[this] = _max_value_dict;

	}

	/// <summary>
    /// try to reload the min,max,previous and value from the cache
    /// </summary>
	public bool TRY_loadFromCache(string _dictionary_key)
    {
		if(cache_dictionary_value.TryGetValue(this,out Dictionary<string,Vector3> _my_data))
        {
			if(_my_data.TryGetValue(_dictionary_key,out Vector3 _value))
            {
				this.PRIVATE_min_value = cache_dictionary_min_value[this][_dictionary_key];
				this.PRIVATE_max_value = cache_dictionary_max_value[this][_dictionary_key];
				this.value = _value;

				return true;
            }
        }
		return false;
    }


	public static void paste(IEnumerable<ObservableVector3> _sources, IEnumerable<ObservableVector3> _destinations)
    {
		var _sources_list = new List<ObservableVector3>(_sources);
		var _destination_list = new List<ObservableVector3>(_destinations);

		if (_sources_list.Count != _destination_list.Count)
		{
			GlobalFunctions.print("_sources.Count() != _destinations.Count()", null);
			return;
		}

		for (int i = 0; i < _sources_list.Count; i += 1)
		{
			ObservableVector3.paste(_sources_list[i], _destination_list[i]);
		}
	}

	public static void paste(ObservableVector3 _source,ObservableVector3 _destination)
    {
		_destination.PRIVATE_min_value = _source.PRIVATE_min_value;
		_destination.PRIVATE_max_value = _source.PRIVATE_max_value;
		_destination.PRIVATE_value = _source.PRIVATE_value;

		if (_destination.my_VariableSettings.synchronise_immediately)
			_destination.synchronize();

		_destination.OnUpdate?.Invoke(_destination);
	}



#if UNITY_EDITOR

	Vector3 last_OnValidate;



    private void OnValidate()
    {
		if (last_OnValidate != this.PRIVATE_value)
        {
			this.value = this.PRIVATE_value;
			last_OnValidate = this.PRIVATE_value;
        }
    }


	[SerializeField] bool enable_gizmos;
	[SerializeField] Color value_gizmos_Color = Color.white;
	[SerializeField] Color previous_value_gizmos_Color = Color.black;
	[SerializeField] float gizmos_radius = 0.1f;


	private void OnDrawGizmosSelected()
    {
		if (this.enable_gizmos == false)
			return;

		Gizmos.color = this.previous_value_gizmos_Color;
		Gizmos.DrawWireSphere(this.previous_value, this.gizmos_radius);

		Gizmos.color = this.value_gizmos_Color;
		Gizmos.DrawWireSphere(this.value, this.gizmos_radius);



    }


#endif



}

