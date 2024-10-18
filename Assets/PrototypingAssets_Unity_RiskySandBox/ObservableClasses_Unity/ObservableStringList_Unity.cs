using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

[RequireComponent(typeof(ObservableClasses_VariableSettings))]
public partial class ObservableStringList : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] List<string> items = new List<string>();


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


	void Awake()
	{
		this.PRIVATE_my_VariableSettings = gameObject.GetComponent<ObservableClasses_VariableSettings>();
		this.cached_my_VariableSettings = true;


		float _resync_rate = this.my_VariableSettings.auto_resync_rate;
		if (_resync_rate > 0)
			InvokeRepeating("synchronize", _resync_rate, _resync_rate);

	}

}
