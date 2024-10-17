using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_ExtrusionBehaviour : MonoBehaviour
{
    public static float extrusion_height_scale_factor = 0.05f;


    [SerializeField] bool debugging;


    [SerializeField] RiskySandBox_Tile my_Tile;



    private void Awake()
    {
        RiskySandBox_HumanPlayer.OnVariableUpdate_deploy_target_STATIC += EventReceiver_OnVariableUpdate_deploy_target;
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_start_STATIC += EventReceiver_OnVariableUpdate_attack_start;
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_target_STATIC += EventReceiver_OnVariableUpdate_attack_target;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_start_STATIC += EventReceiver_OnVariableUpdate_fortify_start;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_target_STATIC += EventReceiver_OnVariableUpdate_fortify_target;
    }

    private void OnDestroy()
    {
        RiskySandBox_HumanPlayer.OnVariableUpdate_deploy_target_STATIC -= EventReceiver_OnVariableUpdate_deploy_target;
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_start_STATIC -= EventReceiver_OnVariableUpdate_attack_start;
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_target_STATIC -= EventReceiver_OnVariableUpdate_attack_target;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_start_STATIC -= EventReceiver_OnVariableUpdate_fortify_start;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_target_STATIC -= EventReceiver_OnVariableUpdate_fortify_target;
    }

    void EventReceiver_OnVariableUpdate_deploy_target(RiskySandBox_HumanPlayer _HumanPlayer) { updateExtrusion(); }
    void EventReceiver_OnVariableUpdate_attack_start(RiskySandBox_HumanPlayer _HumanPlayer) { updateExtrusion(); }
    void EventReceiver_OnVariableUpdate_attack_target(RiskySandBox_HumanPlayer _HumanPlayer) { updateExtrusion(); }
    void EventReceiver_OnVariableUpdate_fortify_start(RiskySandBox_HumanPlayer _HumanPlayer) { updateExtrusion(); }
    void EventReceiver_OnVariableUpdate_fortify_target(RiskySandBox_HumanPlayer _HumanPlayer) { updateExtrusion(); }


    void updateExtrusion()
    {
        RiskySandBox_Team _LocalTeam = RiskySandBox_HumanPlayer.local_player_Team;
        RiskySandBox_HumanPlayer _LocalPlayer = RiskySandBox_HumanPlayer.local_player;

        if(_LocalTeam == null)
        {
            if (this.debugging)
                GlobalFunctions.print("_LocalTeam is null... setting extrusion_height to 0f", this);
            my_Tile.extrusion_height.value = 0f;
            return;
        }

        bool _should_extrude = false;
        string _current_turn_state = _LocalTeam.current_turn_state;

        if(_current_turn_state == RiskySandBox_Team.turn_state_deploy)
        {
            _should_extrude = _LocalPlayer.deploy_target == this.my_Tile;
        }
        else if(_current_turn_state == RiskySandBox_Team.turn_state_attack)
        {
            if(_LocalPlayer.attack_start != null)
                _should_extrude = _LocalPlayer.attack_start == this.my_Tile || _LocalTeam.canAttack(_LocalPlayer.attack_start, this.my_Tile,1,_LocalPlayer.current_attack_method);//TODO - remove magic 1!
        }
        else if(_current_turn_state == RiskySandBox_Team.turn_state_capture)
        {
            _should_extrude = _LocalTeam.capture_start == this.my_Tile || _LocalTeam.capture_target == this.my_Tile;
        }
        else if(_current_turn_state == RiskySandBox_Team.turn_state_fortify)
        {
            _should_extrude = _LocalPlayer.fortify_start == this.my_Tile || _LocalPlayer.fortify_target == this.my_Tile;
            if(_LocalPlayer.fortify_start != null)
                _should_extrude |= _LocalPlayer.fortify_target == null && _LocalTeam.canFortify(_LocalPlayer.fortify_start, this.my_Tile, 1);
        }


        if (_should_extrude)
            this.my_Tile.extrusion_height.value = extrusion_height_scale_factor * RiskySandBox_CameraControls.instance.GET_cameraPosition().y;
        else
            this.my_Tile.extrusion_height.value = 0f;
    }

}
