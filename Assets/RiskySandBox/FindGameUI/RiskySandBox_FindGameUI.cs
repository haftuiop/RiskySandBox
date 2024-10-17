using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public partial class RiskySandBox_FindGameUI : MonoBehaviourPunCallbacks
{
    [SerializeField] bool debugging;

    public ObservableBool root_state { get { return this.PRIVATE_root_state; } }
    [SerializeField] ObservableBool PRIVATE_root_state;

    public ObservableInt room_display_index { get { return this.PRIVATE_room_display_index; } }
    [SerializeField] ObservableInt PRIVATE_room_display_index;


    //need to destroy these on repaint???
    [SerializeField] List<GameObject> instantiated_room_UIs = new List<GameObject>();

    public GameObject ui_root;
    public UnityEngine.UI.Text room_display_index_Text;

    [SerializeField] ObservableInt_SetGameObjectStates room_uis_state_controller;

    Dictionary<RoomInfo, RiskySandBox_RoomInfoUI> CACHE_GET_RoomInfoUI = new Dictionary<RoomInfo, RiskySandBox_RoomInfoUI>();



    [SerializeField] GameObject room_info_UI_prefab;





    public RiskySandBox_RoomInfoUI GET_RoomInfoUI(Photon.Realtime.RoomInfo _RoomInfo)
    {
        if(CACHE_GET_RoomInfoUI.TryGetValue(_RoomInfo,out RiskySandBox_RoomInfoUI _existing_UI))//if there is one already...
        {
            return _existing_UI;
        }

        //instantiate a new one!
        RiskySandBox_RoomInfoUI _roominfo_ui = UnityEngine.Object.Instantiate(room_info_UI_prefab, ui_root.transform).GetComponent<RiskySandBox_RoomInfoUI>();
        this.instantiated_room_UIs.Add(_roominfo_ui.gameObject);
        this.CACHE_GET_RoomInfoUI[_RoomInfo] = _roominfo_ui;
        return _roominfo_ui;

    }

    private void Awake()
    {
        this.root_state.OnUpdate += EventReceiver_OnVariableUpdate_root_state;
        this.room_display_index.OnUpdate += EventReceiver_OnVariableUpdate_room_display_index;

        RiskySandBox_RoomInfoUI.OnJoinButtonPressed_STATIC += EventReceiver_OnJoinButtonPressed_STATIC;

        MultiplayerBridge_PhotonPun.in_room.OnUpdate += EventReceiver_OnVariableUpdate_in_room;

    }

    private void OnDestroy()
    {
        RiskySandBox_RoomInfoUI.OnJoinButtonPressed_STATIC -= EventReceiver_OnJoinButtonPressed_STATIC;
        if (MultiplayerBridge_PhotonPun.in_room != null)
            MultiplayerBridge_PhotonPun.in_room.OnUpdate -= EventReceiver_OnVariableUpdate_in_room;
            
    }



    void EventReceiver_OnVariableUpdate_in_room(ObservableBool _in_room)
    {
        if(_in_room == true)
        {
            if (PhotonNetwork.InLobby)
                PhotonNetwork.LeaveLobby();

            this.disable();
        }
    }



    public void disable()
    {
        this.root_state.value = false;
    }


    void EventReceiver_OnJoinButtonPressed_STATIC(RiskySandBox_RoomInfoUI _RoomInfoUI)
    {
        //try to join that room...
        MultiplayerBridge_PhotonPun.instance.joinRoom(_RoomInfoUI.room_name);
    }



    public void EventReceiver_OnReturnToMainMenuButtonPressed()
    {
        RiskySandBox_MainMenu.instance.returnToMainMenu();
    }



    void EventReceiver_OnVariableUpdate_room_display_index(ObservableInt _room_display_index)
    {
        room_display_index_Text.text = string.Format("{0},{1}", _room_display_index.value, _room_display_index.max_value);
    }


    void EventReceiver_OnVariableUpdate_root_state(ObservableBool _root_state)
    {
        if (_root_state == true)
        {
            //connect to photon...
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        if (RiskySandBox_LevelEditor.is_enabled)
            return;

        if (this.root_state.value == false)
            return;
        
        PhotonNetwork.JoinLobby();
    }




    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby, now listing rooms...");
        // Photon automatically lists the rooms available in the lobby
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);



        foreach (RoomInfo _photon_network_room in roomList)
        {
            RiskySandBox_RoomInfoUI _roominfo_ui = GET_RoomInfoUI(_photon_network_room);


            if(_photon_network_room.RemovedFromList)
            {
                //destroy ui...
                if(_roominfo_ui != null)
                {
                    if (this.instantiated_room_UIs.Contains(_roominfo_ui.gameObject))
                        this.instantiated_room_UIs.Remove(_roominfo_ui.gameObject);


                    UnityEngine.Object.Destroy(_roominfo_ui.gameObject);
                }
                continue;
            }

            //Debug.Log("Room: " + _photon_network_room.Name + " | Players: " + _photon_network_room.PlayerCount + "/" + _photon_network_room.MaxPlayers);




            //TODO paste in all info about the room (that is available)
            //_photon_network_room.CustomProperties
            //the room will need to set these properties which is unimplemented...
            //e.g. do things like num_teams, num players, num bots, ai difficulty/beheaviour
            //TODO - spectate mode????
            //TODO - is official dedicated server?
            //current game mode

            _roominfo_ui.room_name.value = _photon_network_room.Name;

        }

        this.room_uis_state_controller.SET_my_GameObjects(this.instantiated_room_UIs);

    }



}
