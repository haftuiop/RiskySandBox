using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainMenuSettingsMenu : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableBool PRIVATE_is_enabled;
	

    public void enable()
    {
        PRIVATE_is_enabled.value = true;
    }

    public void disable()
    {
        PRIVATE_is_enabled.value = false;
    }




}
