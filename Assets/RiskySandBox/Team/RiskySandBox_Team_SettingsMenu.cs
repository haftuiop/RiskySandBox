using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team_SettingsMenu : MonoBehaviour
{

    public static ObservableList<RiskySandBox_Team> open_settings_menu = new ObservableList<RiskySandBox_Team>();

    [SerializeField] bool debugging;

    [SerializeField] ObservableBool PRIVATE_show_settings_menu;


    [SerializeField] ObservableInt PRIVATE_sub_menu_index;

    [SerializeField] RiskySandBox_Team my_Team;

    private void Awake()
    {

        this.PRIVATE_show_settings_menu.OnUpdate_true += delegate { open_settings_menu.Add(this.my_Team); } ;
        this.PRIVATE_show_settings_menu.OnUpdate_false += delegate { open_settings_menu.Remove(this.my_Team); };

        
    }

    private void OnDestroy()
    {
        if (open_settings_menu.Contains(this.my_Team))
            open_settings_menu.Remove(this.my_Team);
    }

}
