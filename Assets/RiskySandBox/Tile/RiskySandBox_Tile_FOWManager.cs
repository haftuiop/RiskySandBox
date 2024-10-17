using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Tile_FOWManager : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] RiskySandBox_Tile my_Tile;


    [SerializeField] ObservableBool enable_FOW;




    private void OnEnable()
    {
        RiskySandBox_Tile.OnVariableUpdate_my_Team_ID_STATIC += EventReceiver_OnVariableUpdate_my_Team_ID_STATIC;

        this.enable_FOW.OnUpdate += EventReceiver_OnVariableUpdate_enable_FOW;
    }

    private void OnDisable()
    {
        RiskySandBox_Tile.OnVariableUpdate_my_Team_ID_STATIC -= EventReceiver_OnVariableUpdate_my_Team_ID_STATIC;
    }

    void EventReceiver_OnVariableUpdate_my_Team_ID_STATIC(RiskySandBox_Tile _Tile)
    {
        recalculateFog();
    }


    private void Start()
    {
        recalculateFog();
    }

    void EventReceiver_OnVariableUpdate_enable_FOW(ObservableBool _enable_FOW)
    {

    }

    void recalculateFog()
    {
        bool _fog_state = false;

        if (RiskySandBox_LevelEditor.is_enabled)
            return;

        //get the local team...
        RiskySandBox_Team _local_Team = RiskySandBox_HumanPlayer.local_player_Team;

        if(_local_Team == null)
        {
            //fantastic! we just show through the fog...
            _fog_state = false;
        }
        else
        {
            _fog_state = !_local_Team.hasVision(my_Tile);
        }


        if (this.debugging)
            GlobalFunctions.print("setting _fog_state to " + _fog_state,this);



        enable_FOW.value = _fog_state;
    }

    
}
