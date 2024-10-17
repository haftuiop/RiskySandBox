using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_FortifyUI : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] ObservableBool PRIVATE_root_state;
    [SerializeField] ObservableBool PRIVATE_show_input_elements;


    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;
    RiskySandBox_Team my_Team { get { return my_HumanPlayer.my_Team; } }

    private void Awake()
    {
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_target_STATIC += EventReceiver_OnVariableUpdate_fortify_target_STATIC;

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC += EventReceiver_OnVariableUpdate_current_turn_state;
    }

    private void OnDestroy()
    {
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_target_STATIC -= EventReceiver_OnVariableUpdate_fortify_target_STATIC;

        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC -= EventReceiver_OnVariableUpdate_current_turn_state;
    }

    private void Start()
    {
        this.PRIVATE_root_state.value = this.my_Team != null && this.my_Team.current_turn_state == RiskySandBox_Team.turn_state_fortify;
    }

    void EventReceiver_OnVariableUpdate_fortify_target_STATIC(RiskySandBox_HumanPlayer _HumanPlayer)
    {
        if (_HumanPlayer != this.my_HumanPlayer)
            return;

        this.PRIVATE_show_input_elements.value = _HumanPlayer.fortify_target != null;

    }

    void EventReceiver_OnVariableUpdate_current_turn_state(RiskySandBox_Team _Team)
    {
        if (_Team != this.my_HumanPlayer.my_Team)
            return;

        PRIVATE_root_state.value = _Team.current_turn_state == RiskySandBox_Team.turn_state_fortify;
    }



}
