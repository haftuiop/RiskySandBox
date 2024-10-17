using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Tile_LandMineModelManager : MonoBehaviour
{
    [SerializeField] bool debugging;


    [SerializeField] ObservableBool PRIVATE_has_landmine;

    [SerializeField] RiskySandBox_Tile my_Tile;


    private void Awake()
    {
        RiskySandBox_ItemsManager.instance.land_mine_Tile_IDs.OnUpdate += EventReceiver_OnVariableUpdate_land_mine_Tile_IDs;
    }


    private void OnDestroy()
    {
        RiskySandBox_ItemsManager.instance.land_mine_Tile_IDs.OnUpdate -= EventReceiver_OnVariableUpdate_land_mine_Tile_IDs;
    }


    void EventReceiver_OnVariableUpdate_land_mine_Tile_IDs(ObservableIntList _landmine_tile_IDs)
    {
        PRIVATE_has_landmine.value = _landmine_tile_IDs.Contains(this.my_Tile.ID);
    }


}
