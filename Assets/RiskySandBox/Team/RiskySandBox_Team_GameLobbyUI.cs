using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team_GameLobbyUI : MonoBehaviour
{

    [SerializeField] bool debugging;

    [SerializeField] GameObject root;

    [SerializeField] RiskySandBox_Team my_Team;



    private void Awake()
    {
        RiskySandBox_MainGame.game_started.OnUpdate += EventReceiver_OnVariableUpdate_game_started;
        my_Team.ID.OnUpdate += EventReceiver_OnVariableUpdate_my_ID;
        RiskySandBox_HumanPlayer.OnVariableUpdate_my_Team_ID_STATIC += RiskySandBox_HumanPlayerEventReceiver_OnVariableUpdate_my_Team_ID_STATIC;

        RiskySandBox_GameLobby.show_UI.OnUpdate += RiskySandBox_GameLobbyEventReceiver_OnVariableUpdate_show_UI;

        RiskySandBox_GameLobby.instance.team_ui_shift.OnUpdate += EventReceiver_OnVariableUpdate_team_ui_shift;

    }
    private void OnDestroy()
    {
        RiskySandBox_MainGame.game_started.OnUpdate -= EventReceiver_OnVariableUpdate_game_started;
        my_Team.ID.OnUpdate -= EventReceiver_OnVariableUpdate_my_ID;
        RiskySandBox_HumanPlayer.OnVariableUpdate_my_Team_ID_STATIC -= RiskySandBox_HumanPlayerEventReceiver_OnVariableUpdate_my_Team_ID_STATIC;

        RiskySandBox_GameLobby.show_UI.OnUpdate -= RiskySandBox_GameLobbyEventReceiver_OnVariableUpdate_show_UI;
    }

    void EventReceiver_OnVariableUpdate_team_ui_shift(ObservableInt _team_ui_shift)
    {
        if (this.debugging)
            GlobalFunctions.print("RiskySandBox_GameLobby.team_ui_shift just changed! - updating the ui", this);
        updateUI();
    }

    void EventReceiver_OnVariableUpdate_game_started(ObservableBool _game_started)
    {
        if (this.debugging)
            GlobalFunctions.print("game_started value just changed - updating the ui", this);
        updateUI();
    }

    void EventReceiver_OnVariableUpdate_my_ID(ObservableInt _ID)
    {
        if (this.debugging)
            GlobalFunctions.print("my_Team.ID just changed! - updating the ui", this);
        updateUI();
    }

    void RiskySandBox_HumanPlayerEventReceiver_OnVariableUpdate_my_Team_ID_STATIC(RiskySandBox_HumanPlayer _HumanPlayer)
    {
        if (this.debugging)
            GlobalFunctions.print("a human players team id just changed! - updating the ui",this);
        updateUI();
    }

    void RiskySandBox_GameLobbyEventReceiver_OnVariableUpdate_show_UI(ObservableBool _show_UI)
    {
        if (debugging)
            GlobalFunctions.print("",this);
        updateUI();
    }

    void updateUI()
    {
        

        int _shift = this.my_Team.ID.value + RiskySandBox_GameLobby.instance.team_ui_shift;
        bool _enable = RiskySandBox_GameLobby.show_UI && _shift >= 0 && _shift <= 10;//TODO - magic numbers!


        root.SetActive(_enable);



        root.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30f * _shift);//TODO - remove magic 30f (and 0????)

        RiskySandBox_HumanPlayer _HumanPlayer = RiskySandBox_HumanPlayer.GET_RiskySandBox_HumanPlayer(this.my_Team);
        if (_HumanPlayer == null)
        {
            string _team_name = my_Team.default_names[this.my_Team.ID];
            if (this.debugging)
                GlobalFunctions.print("unable to find the _HumanPlayer for this team setting the name to be '"+ _team_name+"'", this);
            this.my_Team.team_name.value = _team_name;
        }
        else
        {
            string _team_name = _HumanPlayer.multiplayer_name;
            if (this.debugging)
                GlobalFunctions.print("found the _HuamnPlayer for this team! setting the team name to be '" + _team_name + "'", this);
            this.my_Team.team_name.value = _team_name;
        }
            

    }
}
