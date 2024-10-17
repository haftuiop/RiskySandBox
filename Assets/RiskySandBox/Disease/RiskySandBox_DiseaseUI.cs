using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_DiseaseUI : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] RiskySandBox_Disease my_Disease;

    [SerializeField] Vector2 case_start_position;


    [SerializeField] GameObject case_ui_prefab;
    [SerializeField] GameObject root;
    [SerializeField] List<GameObject> instantiated_elements = new List<GameObject>();





    private void Awake()
    {
        RiskySandBox_Disease.OnVariableUpdate_STATIC += EventReceiver_OnVariableUpdate_STATIC;
        RiskySandBox_Disease.OnVariableUpdate_infected_Tile_IDs_STATIC += EventReceiver_OnUpdate_infected_Tile_IDs_STATIC;
        RiskySandBox_Disease.OnVariableUpdate_remaining_durations_STATIC += EventReceiver_OnUpdate_remaining_durations_STATIC;
    }

    private void OnDestroy()
    {
        RiskySandBox_Disease.OnVariableUpdate_STATIC -= EventReceiver_OnVariableUpdate_STATIC;
        RiskySandBox_Disease.OnVariableUpdate_infected_Tile_IDs_STATIC -= EventReceiver_OnUpdate_infected_Tile_IDs_STATIC;
        RiskySandBox_Disease.OnVariableUpdate_remaining_durations_STATIC -= EventReceiver_OnUpdate_remaining_durations_STATIC;
    }


    private void Start()
    {
        this.updateUI();
    }



    void destroyTempElements()
    {
        foreach (GameObject _GameObject in instantiated_elements)
        {
            UnityEngine.Object.Destroy(_GameObject);
        }

        this.instantiated_elements.Clear();
    }


    void EventReceiver_OnVariableUpdate_STATIC(RiskySandBox_Disease _Disease)
    {
        if (_Disease != this.my_Disease)
            return;

        updateUI();
    }

    void EventReceiver_OnUpdate_infected_Tile_IDs_STATIC(RiskySandBox_Disease _Disease)
    {
        if (_Disease != this.my_Disease)
            return;

        updateUI();
    }

    void EventReceiver_OnUpdate_remaining_durations_STATIC(RiskySandBox_Disease _Disease)
    {
        if (_Disease != this.my_Disease)
            return;

        updateUI();
    }


    void updateUI()
    {
        destroyTempElements();

        if (this.my_Disease.infected_Tiles.Count != this.my_Disease.remaining_durations.Count)
            return;


        int i = 0;
        foreach (RiskySandBox_Tile _Tile in this.my_Disease.infected_Tiles)
        {
            //ok first thing we want to instantiate a ui element showing this...
            //then foreach tile it has infected...
            //show that on the ui

            if (this.my_Disease.GET_remaining_duration(_Tile) <= 0)
                continue;


            RiskySandBox_DiseaseCaseUI _new_CaseUI = UnityEngine.Object.Instantiate(case_ui_prefab, this.root.transform).GetComponent<RiskySandBox_DiseaseCaseUI>();
            _new_CaseUI.my_Tile = _Tile;
            _new_CaseUI.my_Disease = this.my_Disease;
            _new_CaseUI.GetComponent<RectTransform>().anchoredPosition = case_start_position + new Vector2(0, -30 * i);
            this.instantiated_elements.Add(_new_CaseUI.gameObject);
            i += 1;

        }
    }
}
