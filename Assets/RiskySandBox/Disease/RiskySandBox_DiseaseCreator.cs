using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_DiseaseCreator : MonoBehaviour
{
    public static RiskySandBox_DiseaseCreator instance;

    [SerializeField] bool debugging;



    void Awake()
    {
        instance = this;
    }


    public RiskySandBox_Disease createDisease(string[] _lines)
    {
        RiskySandBox_Disease _new_Disease = RiskySandBox_Resources.createDisease();



        return _new_Disease;

    }


    public RiskySandBox_Disease createDisease(string _name, int _duration, float _leathality, float _infectivity)
    {
        RiskySandBox_Disease _new_Disease = RiskySandBox_Resources.createDisease();

        _new_Disease.disease_name.value = _name;
        _new_Disease.duration.value = _duration;
        _new_Disease.lethality.value = _leathality;
        _new_Disease.infectivity.value = _infectivity;

        return _new_Disease;
    }


}
