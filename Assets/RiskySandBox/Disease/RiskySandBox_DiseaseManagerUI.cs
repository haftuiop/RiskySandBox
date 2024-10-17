using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_DiseaseManagerUI : MonoBehaviour
{
    public static RiskySandBox_DiseaseManagerUI instance;
    public static ObservableBool root_state { get { return instance.PRIVATE_root_state; } }

    [SerializeField] bool debugging;

    [SerializeField] ObservableBool PRIVATE_root_state;


    public ObservableInt current_disease_index { get { return this.PRIVATE_current_disease_index; } }
    [SerializeField] ObservableInt PRIVATE_current_disease_index;

    private void Awake()
    {
        instance = this;

        this.current_disease_index.min_value = 0;
        this.current_disease_index.max_value = RiskySandBox_Disease.all_instances.Count() - 1;

        this.current_disease_index.OnUpdate += EventReceiver_OnVariableUpdate_current_disease_index;

        this.PRIVATE_root_state.OnUpdate += EventReceiver_OnVariableUpdate_root_state;

        RiskySandBox_Disease.all_instances.OnUpdate += RiskySandBox_DiseaseEventReceiver_OnUpdate_all_instances;
    }

    private void OnDestroy()
    {
        RiskySandBox_Disease.all_instances.OnUpdate -= RiskySandBox_DiseaseEventReceiver_OnUpdate_all_instances;
    }


    void showCurrentDiseaseUI()
    {
        foreach (RiskySandBox_Disease _Disease in RiskySandBox_Disease.all_instances)
        {
            _Disease.root_state.value = false;
        }

        if (RiskySandBox_Disease.all_instances.Count() == 0)
            return;

        RiskySandBox_Disease _current_Disease = RiskySandBox_Disease.all_instances[this.current_disease_index];

        _current_Disease.root_state.value = true && this.PRIVATE_root_state.value == true;

    }

    void EventReceiver_OnVariableUpdate_root_state(ObservableBool _root_state)
    {
        if (RiskySandBox_Disease.all_instances.Count == 0)
            return;

        showCurrentDiseaseUI();
    }

    void EventReceiver_OnVariableUpdate_current_disease_index(ObservableInt _current_disease_index)
    {
        showCurrentDiseaseUI();
    }

    void RiskySandBox_DiseaseEventReceiver_OnUpdate_all_instances()
    {
        this.current_disease_index.min_value = 0;
        this.current_disease_index.max_value = RiskySandBox_Disease.all_instances.Count() - 1;
    }









}
