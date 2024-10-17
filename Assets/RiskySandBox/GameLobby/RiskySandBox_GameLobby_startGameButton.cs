using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_GameLobby_startGameButton : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] UnityEngine.UI.Button my_Button;



    private void Awake()
    {
        PrototypingAssets.run_server_code.OnUpdate += EventReceiver_OnVariableUpdate_run_server_code;
        MultiplayerBridge_Mirror.is_enabled.OnUpdate += MultiplayerBridge_MirrorEventReceiver_OnVariableUpdate_is_enabled;

        my_Button.onClick.AddListener(delegate { EventReceiver_OnmyButtonPressed(); });
    }

    private void Start()
    {
        recalculateButtonState();
    }

    void EventReceiver_OnVariableUpdate_game_started(ObservableBool _game_started) { recalculateButtonState(); }
    void MultiplayerBridge_MirrorEventReceiver_OnVariableUpdate_is_enabled(ObservableBool _is_enabled) { recalculateButtonState(); }
    void EventReceiver_OnVariableUpdate_run_server_code(ObservableBool _run_server_code){ recalculateButtonState(); }

    void recalculateButtonState()
    {
        bool _state = (PrototypingAssets.run_server_code.value || MultiplayerBridge_Mirror.is_enabled) && (RiskySandBox_MainGame.game_started == false);
        this.my_Button.interactable = _state;
    }

    void EventReceiver_OnmyButtonPressed()
    {
        //ok! we shall try to start the game...


        if(MultiplayerBridge_Mirror.is_enabled == true)
        {
            //TODO - this is temporary ideally the dedicated server will automatically start the game itself once "startGame conditions" are met
            
            RiskySandBox_HumanPlayer.local_player.GetComponent<RiskySandBox_HumanPlayer_DedicatedServerCommands>().TRY_startGame();
        }
        else if(PhotonNetwork.IsMasterClient)
        {
            RiskySandBox_MainGame.instance.startGame();
        }
    }

    
}
