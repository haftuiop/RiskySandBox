using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame : MonoBehaviour
{
    [SerializeField] bool debugging;


    [SerializeField] ObservableInt PRIVATE_turn_number;
    [SerializeField] ObservableString PRIVATE_map_ID;
    
    [SerializeField] ObservableBool PRIVATE_game_started;




    [SerializeField] ObservableInt PRIVATE_n_troops_startGame;


    [SerializeField] ObservableInt PRIVATE_n_Teams;



    [SerializeField] ObservableInt PRIVATE_num_wildcards;


    [SerializeField] ObservableInt PRIVATE_n_stable_portals;
    [SerializeField] ObservableInt PRIVATE_n_unstable_portals;
    [SerializeField] ObservableInt PRIVATE_n_blizards;
    [SerializeField] ObservableBool PRIVATE_display_bonuses;
    [SerializeField] ObservableBool PRIVATE_show_escape_menu;

    [SerializeField] ObservableBool PRIVATE_veto_reconnections;//TODO - when a new player connects... tell all players and they can decide if they dont want to allow this player to rejoin....

    




    private void Awake()
    {
        instance = this;
        RiskySandBox_Team.OnVariableUpdate_defeated_STATIC += delegate { endGameCheck(); };
        RiskySandBox_Team.OnendTurn += TeamEventReceiver_OnendTurn;

        RiskySandBox_Map.OnloadMapCompleted += EventReceiver_OnloadMapCompleted;

        RiskySandBox_MainMenu.Onenable += EventReceiver_OnenterMainMenu;

        RiskySandBox_MainGame.turn_number.OnUpdate += delegate { RiskySandBox_MainGame.OnVariableUpdate_turn_number?.Invoke(RiskySandBox_MainGame.turn_number); };



    }

    public void EventReceiver_OnenterMainMenu()
    {
        RiskySandBox_Map.instance.clearMap();
        RiskySandBox_MainGame.game_started.value = false;

        
        this.PRIVATE_show_escape_menu.value = false;
    }


}
