using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_GameLobby : MonoBehaviour
{

    public static ObservableBool show_UI { get { return instance.PRIVATE_show_UI; } }
    public static RiskySandBox_GameLobby instance;

    [SerializeField] bool debugging;

    [SerializeField] ObservableBool PRIVATE_show_UI;





    [SerializeField] ObservableBool game_started_ObservableBool;

    
    [SerializeField] ObservableBool open_settings_menu_count_greater_than_zero;

    public ObservableInt team_ui_shift { get { return instance.PRIVATE_team_ui_shift; } }
    [SerializeField] ObservableInt PRIVATE_team_ui_shift;



    private void Awake()
    {
        instance = this;
    }


    void OnEnable()
    {
        MultiplayerBridge_PhotonPun.in_room.OnUpdate_true += EventReceiver_OnVariableUpdate_in_room_true;

        RiskySandBox_Team_SettingsMenu.open_settings_menu.OnUpdate += EventReceiver_OnVariableUpdate_open_settings_menu;
    }

    void OnDisable()
    {
        MultiplayerBridge_PhotonPun.in_room.OnUpdate_true -= EventReceiver_OnVariableUpdate_in_room_true;

        RiskySandBox_Team_SettingsMenu.open_settings_menu.OnUpdate -= EventReceiver_OnVariableUpdate_open_settings_menu;
    }




    void EventReceiver_OnVariableUpdate_open_settings_menu()
    {
        this.open_settings_menu_count_greater_than_zero.value = RiskySandBox_Team_SettingsMenu.open_settings_menu.Count > 0;
    }




    public void createTeams()
    {
        GameObject _Team_prefab = RiskySandBox_Resources.Team_prefab;
        for(int i = 0; i < RiskySandBox_MainGame.n_Teams; i += 1)
        {
            RiskySandBox_Resources.createTeam(i);
        }

    }


    void EventReceiver_OnVariableUpdate_in_room_true(ObservableBool _in_room)
    {
        RiskySandBox_HumanPlayer _my_HumanPlayer = null;

        if (RiskySandBox_LevelEditor.is_enabled == false)
            _my_HumanPlayer = createMyHumanPlayer();

        if(PrototypingAssets.run_server_code)
        {
            createTeams();
        }


        if (PhotonNetwork.IsMasterClient && _my_HumanPlayer != null)
        {
            //join the first team...
            _my_HumanPlayer.my_Team_ID.value = 0;
            
        }

    }




    RiskySandBox_HumanPlayer createMyHumanPlayer()
    {

        RiskySandBox_HumanPlayer _my_HumanPlayer = GET_RiskySandBox_HumanPlayer(PhotonNetwork.LocalPlayer);
        //TODO add in some debug statements... to say WTF?!?!?!
        if (_my_HumanPlayer != null)
            return null;

        if (PhotonNetwork.InRoom == false)
            return null;

        return RiskySandBox_Resources.createLocalHumanPlayer();
    }




    RiskySandBox_HumanPlayer GET_RiskySandBox_HumanPlayer(Photon.Realtime.Player _PhotonPlayer)
    {
        foreach(RiskySandBox_HumanPlayer _RiskySandBox_HumanPlayer in RiskySandBox_HumanPlayer.all_instances)
        {
            if (_RiskySandBox_HumanPlayer.GetComponent<PhotonView>().CreatorActorNr == _PhotonPlayer.ActorNumber)
                return _RiskySandBox_HumanPlayer;
        }
        //unable to find... WTF?!?!?!?!?
        return null;
    }





}
