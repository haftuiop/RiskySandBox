using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team_JoinTeamButton : MonoBehaviour
{

    [SerializeField] bool debugging;
    [SerializeField] RiskySandBox_Team my_Team;
    [SerializeField] UnityEngine.UI.Button my_Button;



    private void Awake()
    {
        my_Button.onClick.AddListener
        (
            delegate
            {
                RiskySandBox_HumanPlayer _local_HumanPlayer = RiskySandBox_HumanPlayer.local_player;

                if (_local_HumanPlayer != null)
                {
                    if (this.debugging)
                        GlobalFunctions.print("my_Button was pressed... calling RiskySandBox_HumanPlayer.local_player.TRY_joinTeam()", this);
                    _local_HumanPlayer.TRY_joinTeam(this.my_Team);
                }
                else
                    GlobalFunctions.print("why is RiskySandBox_HumanPlayer.local_player null???", this);
            }
        );


        //TODO - set the button interactable state depending on if this is a human team and if there is a HumanPlayer with this team id
        RiskySandBox_HumanPlayer.OnVariableUpdate_my_Team_ID_STATIC += HumanPlayerEventReceiver_OnVariableUpdate_my_Team_ID_STATIC;
        RiskySandBox_Team.OnVariableUpdate_ID_STATIC += RiskySandBox_TeamEventReceiver_OnVariableUpdate_ID;

    }

    private void OnDestroy()
    {
        RiskySandBox_HumanPlayer.OnVariableUpdate_my_Team_ID_STATIC -= HumanPlayerEventReceiver_OnVariableUpdate_my_Team_ID_STATIC;
        RiskySandBox_Team.OnVariableUpdate_ID_STATIC -= RiskySandBox_TeamEventReceiver_OnVariableUpdate_ID;
    }

    void HumanPlayerEventReceiver_OnVariableUpdate_my_Team_ID_STATIC(RiskySandBox_HumanPlayer _HumanPlayer)
    {
        recalculateButtonInteractable();
    }

    void RiskySandBox_TeamEventReceiver_OnVariableUpdate_ID(RiskySandBox_Team _Team)
    {
        recalculateButtonInteractable();
    }


    void recalculateButtonInteractable()
    {
        bool _interactable = true;

        RiskySandBox_HumanPlayer _HumanPlayer = RiskySandBox_HumanPlayer.GET_RiskySandBox_HumanPlayer(this.my_Team);
        if (_HumanPlayer != null)
            _interactable = false;


        this.my_Button.interactable = _interactable;
    }



}
