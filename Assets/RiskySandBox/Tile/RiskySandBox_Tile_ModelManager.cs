using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Tile_ModelManager : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] RiskySandBox_Tile my_Tile;

    [SerializeField] List<GameObject> team_token_models = new List<GameObject>();

    [SerializeField] ObservableInt my_Team_ID { get { return this.my_Tile.my_Team_ID; } }


    




    private void Awake()
    {
        my_Team_ID.OnUpdate += delegate { updateModel(); };

    }





    private void Start()
    {
        updateModel();
    }





    void updateModel()
    {
        foreach (GameObject _token in this.team_token_models)
        {
            _token.SetActive(false);
        }

        if (my_Team_ID.value != RiskySandBox_Team.null_ID)
        {
            try { team_token_models[my_Team_ID].SetActive(true); }
            catch { }
        }
    }
    
}