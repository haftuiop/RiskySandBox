using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

[RequireComponent(typeof(ObservableClasses_VariableSettings))]
public partial class ObservableBoolList : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] List<bool> items = new List<bool>();

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


}
