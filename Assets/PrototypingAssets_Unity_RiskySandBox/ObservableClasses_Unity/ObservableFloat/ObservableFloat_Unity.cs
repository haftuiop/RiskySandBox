//extra functions of the ObservableString class so it can be used with the Unity Game Engine
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(ObservableClasses_VariableSettings))]
public partial class ObservableFloat : MonoBehaviour
{
	[SerializeField] private bool debugging;
	
	[SerializeField] private float PRIVATE_value;//the actual value of this ObservableFloat
	[SerializeField] private float PRIVATE_previous_value;//the actual previous value of the float...
	[SerializeField] private float PRIVATE_min_value = float.NegativeInfinity; 
	[SerializeField] private float PRIVATE_max_value = float.PositiveInfinity;

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


	[SerializeField] private UnityEvent<ObservableFloat> OnUpdate_Inspector;

	[SerializeField] private string player_prefs_string = "";
	[SerializeField] private bool write_player_prefs = false;



	private void Awake()
	{
		this.PRIVATE_my_VariableSettings = gameObject.GetComponent<ObservableClasses_VariableSettings>();
		this.cached_my_VariableSettings = true;

		if (player_prefs_string != "")
		{
			//load it...
			if (PlayerPrefs.HasKey(player_prefs_string))
				this.value = PlayerPrefs.GetInt(this.player_prefs_string);

			if (write_player_prefs)
				this.OnUpdate += delegate { PlayerPrefs.SetFloat(player_prefs_string,this.value); };
		}

		if (Application.isEditor || this.OnUpdate_Inspector.GetPersistentEventCount() > 0)
			this.OnUpdate += delegate { OnUpdate_Inspector.Invoke(this); };

		float _resync_rate = this.my_VariableSettings.auto_resync_rate;
		if (_resync_rate > 0)
			InvokeRepeating("synchronize", _resync_rate, _resync_rate);


	}

#if UNITY_EDITOR

	float last_OnValidate;

    private void OnValidate()
    {

		if (last_OnValidate != this.PRIVATE_value)
        {
			this.value = this.PRIVATE_value;//ensure the value gets clamped... AND the syncronising/OnUpdate happens...
			last_OnValidate = this.PRIVATE_value;
			
        }
    }

#endif

}

