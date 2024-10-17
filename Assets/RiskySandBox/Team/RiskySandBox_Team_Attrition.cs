using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team_Attrition : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] RiskySandBox_Team my_Team;


    [SerializeField] ObservableInt PRIVATE_fixed_attrition;
    [SerializeField] ObservableInt PRIVATE_fixed_attrition_threshold;
    [SerializeField] ObservableFloat PRIVATE_percentage_attrition;
    [SerializeField] ObservableInt PRIVATE_percentage_attrition_threshold;

    [SerializeField] ObservableBool PRIVATE_check_at_start_of_turn;
    [SerializeField] ObservableBool PRIVATE_check_at_end_of_turn;
    [SerializeField] ObservableBool PRIVATE_check_at_start_of_round;
    [SerializeField] ObservableBool PRIVATE_check_at_end_of_round;
    [SerializeField] ObservableBool PRIVATE_check_at_deploy_to_attack_transition;



    private void Awake()
    {
        my_Team.current_turn_state.OnUpdate += EventReceiver_OnVariableUpdate_current_turn_state;
        my_Team.is_my_turn.OnUpdate += EventReceiver_OnVariableUpdate_is_my_turn;
    }


    void EventReceiver_OnVariableUpdate_current_turn_state(ObservableString _current_turn_state)
    {
        if (PrototypingAssets.run_server_code.value == false)
            return;

        if(this.PRIVATE_check_at_deploy_to_attack_transition && _current_turn_state.previous_value == RiskySandBox_Team.turn_state_deploy && _current_turn_state.value == RiskySandBox_Team.turn_state_attack)
        {
            if (this.debugging)
                GlobalFunctions.print("deploy -> attack transition... applying attrition!", this);

            applyAttrition();
        }
    }

    void EventReceiver_OnVariableUpdate_is_my_turn(ObservableBool _is_my_turn)
    {
        if (PrototypingAssets.run_server_code.value == false)
            return;

        //false -> true
        if(PRIVATE_check_at_start_of_turn && _is_my_turn.previous_value == false && _is_my_turn.value == true)
        {
            if (this.debugging)
                GlobalFunctions.print("start of turn... applying attrition!", this);
            applyAttrition();
        }
        if(PRIVATE_check_at_end_of_turn && _is_my_turn.previous_value == true && _is_my_turn.value == false)
        {
            if (this.debugging)
                GlobalFunctions.print("end of turn... applying attrition!", this);
            applyAttrition();
        }
    }


    void applyAttrition()
    {

        foreach(RiskySandBox_Tile _Tile in this.my_Team.my_Tiles)
        {
            if(_Tile.num_troops >= PRIVATE_percentage_attrition_threshold)
            {
                //apply percentage attrition...
                applyPercentageAttrition(_Tile);
            }

            if(_Tile.num_troops >= PRIVATE_fixed_attrition_threshold)
            {
                applyFixedAttrition(_Tile);
            }
        }



    }


    //TODO become neutral (if run out of troops???)
    void applyPercentageAttrition(RiskySandBox_Tile _Tile)
    {

    }


    //TODO - become neutral (if num_troops <= 0)?????
    void applyFixedAttrition(RiskySandBox_Tile _Tile)
    {
        _Tile.num_troops.value -= this.PRIVATE_fixed_attrition;
        
    }
    
}
