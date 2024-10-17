using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Tile_VirusModelManager : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] bool testing;

    [SerializeField] RiskySandBox_Tile my_Tile;


    [SerializeField] float radius = 1f;

    List<GameObject> instantiated_models = new List<GameObject>();


    [SerializeField] GameObject virus_model_prefab;



    private void Awake()
    {
        RiskySandBox_Disease.OnVariableUpdate_infected_Tile_IDs_STATIC += EventReceiver_OnVariableUpdate_infected_Tile_IDs_STATIC;
        RiskySandBox_Disease.OnVariableUpdate_remaining_durations_STATIC += EventReceiver_OnVariableUpdate_remaining_durations_STATIC;
    }

    private void OnDestroy()
    {
        RiskySandBox_Disease.OnVariableUpdate_infected_Tile_IDs_STATIC -= EventReceiver_OnVariableUpdate_infected_Tile_IDs_STATIC;
        RiskySandBox_Disease.OnVariableUpdate_remaining_durations_STATIC -= EventReceiver_OnVariableUpdate_remaining_durations_STATIC;
    }


    void EventReceiver_OnVariableUpdate_infected_Tile_IDs_STATIC(RiskySandBox_Disease _Disease)
    {
        refresh();
    }

    void EventReceiver_OnVariableUpdate_remaining_durations_STATIC(RiskySandBox_Disease _Disease)
    {
        refresh();
    }


    void refresh()
    {
        foreach(GameObject _GameObject in this.instantiated_models)
        {
            UnityEngine.Object.Destroy(_GameObject);
        }

        List<Vector3> _positions = GET_model_positions();

        
        int i = 0;
        foreach(RiskySandBox_Disease _Disease in RiskySandBox_Disease.all_instances)
        {
            if (_Disease.infected_Tiles.Contains(this.my_Tile) == false)
                continue;

            if (_Disease.infected_Tiles.Count != _Disease.remaining_durations.Count)
                continue;

            if (_Disease.GET_remaining_duration(this.my_Tile) <= 0)
                continue;


            if(_Disease.disease_model == null)
            {
                GlobalFunctions.print("disease model is unassigned for this disease???", _Disease);
                continue;
            }
            GameObject _new = UnityEngine.Object.Instantiate(_Disease.disease_model);

            this.instantiated_models.Add(_new);

            _new.transform.position = _positions[i];


            i += 1;

        }




    }


    private void Update()
    {
        if (this.testing == false)
            return;

        if(Input.GetKeyDown(KeyCode.T))
        {
            refresh();
        }
    }


    List<Vector3> GET_model_positions()
    {
        List<Vector3> _points = new List<Vector3>();


        int _n_diseases = RiskySandBox_Disease.all_instances.Where(x => x.infected_Tiles.Contains(this.my_Tile)).Count();

        float _increment = 2 * MathF.PI / _n_diseases;



        for(int i = 0; i < _n_diseases; i += 1)
        {
            float _angle = i * _increment;


            _points.Add(this.my_Tile.UI_position + (this.my_Tile.UI_scale_factor / 2) * new Vector3( MathF.Sin(_angle), 0f, MathF.Cos(_angle) ) );
        }


        return _points;
    }
}
