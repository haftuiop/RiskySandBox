using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_CaptureUI : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;
    RiskySandBox_Team my_Team { get { return my_HumanPlayer.my_Team; } }

    [SerializeField] ObservableBool PRIVATE_in_capture_state;


    private void Awake()
    {
        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC += EventReceiver_OnVariableUpdate_current_turn_state_STATIC;
    }

    private void OnDestroy()
    {
        RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC -= EventReceiver_OnVariableUpdate_current_turn_state_STATIC;
    }



    void EventReceiver_OnVariableUpdate_current_turn_state_STATIC(RiskySandBox_Team _Team)
    {
        if (_Team != this.my_Team)
            return;
        this.PRIVATE_in_capture_state.value = _Team.current_turn_state == RiskySandBox_Team.turn_state_capture;
    }

}
