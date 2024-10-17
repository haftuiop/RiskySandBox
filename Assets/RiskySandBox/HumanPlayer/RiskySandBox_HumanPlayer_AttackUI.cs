using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_AttackUI : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] ObservableBool PRIVATE_root_state;
    [SerializeField] ObservableBool PRIVATE_show_input_elements;

    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;

    

    void Awake()
    {
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_target_STATIC += EventReceiver_OnVariableUpdate_attack_target_STATIC;

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC += EventReceiver_OnVariableUpdate_current_turn_state_STATIC;
    }

    private void OnDestroy()
    {
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_target_STATIC -= EventReceiver_OnVariableUpdate_attack_target_STATIC;

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC -= EventReceiver_OnVariableUpdate_current_turn_state_STATIC;
    }

    void EventReceiver_OnVariableUpdate_attack_target_STATIC(RiskySandBox_HumanPlayer _HumanPlayer)
    {
        if (_HumanPlayer != this.my_HumanPlayer)
            return;

        PRIVATE_show_input_elements.value = _HumanPlayer.attack_target != null;

    }

    void EventReceiver_OnVariableUpdate_current_turn_state_STATIC(RiskySandBox_Team _Team)
    {
        if (_Team != this.my_HumanPlayer.my_Team)
            return;

        PRIVATE_root_state.value = _Team.current_turn_state == RiskySandBox_Team.turn_state_attack;
    }

}
