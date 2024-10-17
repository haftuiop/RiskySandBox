using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_AllianceSettings : MonoBehaviour
{

    public static RiskySandBox_AllianceSettings instance;

    [SerializeField] bool debugging;


    public static ObservableBool enable_alliances { get { return instance.PRIVATE_enable_alliances; } }

    [SerializeField] ObservableBool PRIVATE_enable_alliances;





    //todo create other allinace options e.g. 2v2 mode


    private void Awake()
    {
        if (debugging)
            GlobalFunctions.print("", this);
        instance = this;
    }


}
