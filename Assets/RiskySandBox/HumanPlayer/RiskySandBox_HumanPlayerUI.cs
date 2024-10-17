using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayerUI : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableBool PRIVATE_root_state;

    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;
    RiskySandBox_Team my_Team { get { return my_HumanPlayer.my_Team; } }

    

    [SerializeField] ObservableBool PRIVATE_main_game_ended;


    [SerializeField] UnityEngine.UI.Text num_cards_Text;




    private void Awake()
    {
        RiskySandBox_MainGame.game_started.OnUpdate += EventReceiver_OnVariableUpdate_game_started;
        RiskySandBox_MainGame.OnendGame += EventReceiver_OnendGame;

        RiskySandBox_Team.OnVariableUpdate_is_my_turn_STATIC += EventReceiver_OnVariableUpdate_is_my_turn_state;


        this.PRIVATE_main_game_ended.OnUpdate += delegate { recalculateRootState(); };

    }

    private void OnDestroy()
    {
        RiskySandBox_MainGame.game_started.OnUpdate -= EventReceiver_OnVariableUpdate_game_started;
        RiskySandBox_MainGame.OnendGame -= EventReceiver_OnendGame;

        RiskySandBox_Team.OnVariableUpdate_is_my_turn_STATIC -= EventReceiver_OnVariableUpdate_is_my_turn_state;
    }

    void EventReceiver_OnVariableUpdate_game_started(ObservableBool _game_started)
    {
        recalculateRootState();
    }

    void EventReceiver_OnVariableUpdate_is_my_turn_state(RiskySandBox_Team _Team)
    {
        recalculateRootState();
    }

    void EventReceiver_OnendGame()
    {
        this.PRIVATE_main_game_ended.value = true;
    }


    void recalculateRootState()
    {
        bool _state = true;

        if (RiskySandBox_MainGame.game_started == false)
            _state = false;

        if (my_Team == null)
            _state = false;

        else
        {
            if (my_Team.is_my_turn == false)
                _state = false;
        }

        if (PRIVATE_main_game_ended.value == true)
            _state = false;

        PRIVATE_root_state.value = _state;

    }



}
