using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_CreditsMenu : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableBool PRIVATE_ui_root_state;
	

    public void enable()
    {
        this.PRIVATE_ui_root_state.value = true;
    }

    public void disable()
    {
        this.PRIVATE_ui_root_state.value = false;
    }
}
