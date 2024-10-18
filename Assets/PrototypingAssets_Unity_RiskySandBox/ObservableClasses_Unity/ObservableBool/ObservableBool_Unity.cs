//extra functions of the ObservableString class so it can be used with the Unity Game Engine
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(ObservableClasses_VariableSettings))]
public partial class ObservableBool : MonoBehaviour
{
	[SerializeField] private bool debugging;
	[SerializeField] private bool PRIVATE_value;
	[SerializeField] private bool PRIVATE_previous_value;

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

	[SerializeField] private UnityEvent<ObservableBool> OnUpdate_Inspector;
	[SerializeField] private UnityEvent<ObservableBool> OnUpdate_true_Inspector;
	[SerializeField] private UnityEvent<ObservableBool> OnUpdate_false_Inspector;


	[SerializeField] private string player_prefs_string = "";
	[SerializeField] private bool write_player_prefs = false;


	private void Awake()
	{
		this.PRIVATE_my_VariableSettings = GetComponent<ObservableClasses_VariableSettings>();
		this.cached_my_VariableSettings = true;

		if(this.player_prefs_string != "")
        {
			//internally we are going to represent this as a 1 or 0
			if(PlayerPrefs.HasKey(this.player_prefs_string))
				this.value = PlayerPrefs.GetInt(this.player_prefs_string) != 0;

			if(write_player_prefs)
				this.OnUpdate += delegate { PlayerPrefs.SetInt(this.player_prefs_string,this.value ? 1 : 0); };
        }

		if (Application.isEditor || this.OnUpdate_Inspector.GetPersistentEventCount() > 0)
			this.OnUpdate += delegate { OnUpdate_Inspector.Invoke(this); };

		if (Application.isEditor || this.OnUpdate_true_Inspector.GetPersistentEventCount() > 0)
			this.OnUpdate_true += delegate { OnUpdate_true_Inspector.Invoke(this); };

		if (Application.isEditor || this.OnUpdate_false_Inspector.GetPersistentEventCount() > 0)
			this.OnUpdate_false += delegate { OnUpdate_false_Inspector.Invoke(this); };


		float _resync_rate = my_VariableSettings.auto_resync_rate;
		if (_resync_rate > 0)
			InvokeRepeating("synchronize", _resync_rate, _resync_rate);

	}

#if UNITY_EDITOR

	bool last_OnValidate;

	private void OnValidate()
	{
		if (last_OnValidate != this.PRIVATE_value)
		{
			this.value = this.PRIVATE_value;//ensure the value gets syncronised/OnUpdate happens...
			last_OnValidate = this.PRIVATE_value;

		}
	}

#endif



}

