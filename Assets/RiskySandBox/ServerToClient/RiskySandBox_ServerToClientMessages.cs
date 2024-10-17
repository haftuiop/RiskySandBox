using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_ServerToClientMessages : MonoBehaviour
{
    public static RiskySandBox_ServerToClientMessages instance;

    [SerializeField] bool debugging;

    PhotonView my_PhotonView;

    //TODO - include the debug statements this is very important server and client!

    void Awake()
    {
        instance = this;
        my_PhotonView = GetComponent<PhotonView>();



        RiskySandBox_MainGame.OnstartGame_MultiplayerBridge += EventReceiver_OnstartGame_MultiplayerBridge;
        RiskySandBox_MainGame.OnendGame_MultiplayerBridge += EventReceiver_OnendGame_MultiplayerBridge;

        
        RiskySandBox_Team.Ondeploy_MultiplayerBridge += RiskySandBox_TeamEventReceiver_Ondeploy_MultiplayerBridge;
        RiskySandBox_Team.Onattack_MultiplayerBridge += EventReceiver_Onattack_MultiplayerBridge;
        RiskySandBox_Team.Onfortify_MultiplayerBridge += EventReceiver_Onfortify_MultiplayerBridge;
        RiskySandBox_Team.Oncapture_MultiplayerBridge += EventReceiver_Oncapture_MultiplayerBridge;

        RiskySandBox_ItemsManager.Onnuke_MultiplayerBridge += EventReceiver_Onnuke_MultiplayerBridge;

    }




    void EventReceiver_OnstartGame_MultiplayerBridge()
    {

    }


    void EventReceiver_OnendGame_MultiplayerBridge()
    {
        if (PrototypingAssets.run_server_code.value == false)//if we are not the server?
            return;//TODO - debug WTF?!?!?!?!?!

        my_PhotonView.RPC("ServerInvokedRPC_endGame", RpcTarget.Others);//tell everyone to end the game (disconnect and display endGame screen with the "winner")

    }




    void RiskySandBox_TeamEventReceiver_Ondeploy_MultiplayerBridge(RiskySandBox_Team.EventInfo_Ondeploy _EventInfo)
    {
        if (PrototypingAssets.run_server_code == false)
            return;//TODO - WTF>!!?!?!?!?!?

        //ok great... lets tell all? connected clients this has happened...
        foreach(Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - make sure the fog of war doesnt hide this infomation...
            my_PhotonView.RPC("ServerInvokedRPC_Ondeploy", _Player, _EventInfo.battle_log_string);
        }
    }


    void EventReceiver_Onattack_MultiplayerBridge(RiskySandBox_Team.EventInfo_Onattack _EventInfo)
    {
        if (PrototypingAssets.run_server_code.value == false)//we are not the server...
            return;//just dont worry... TODO - debug WTF?!?!?!?

        int _defending_Team_ID = RiskySandBox_Team.null_ID;

        if (_EventInfo.defending_Team != null)
            _defending_Team_ID = _EventInfo.defending_Team.ID;


        foreach (Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - make sure the fog of war doesnt hide this event for the player...

            my_PhotonView.RPC("ServerInvokedRPC_Onattack", _Player, _EventInfo.battle_log_string);
        }
    }

    void EventReceiver_Oncapture_MultiplayerBridge(RiskySandBox_Team.EventInfo_Oncapture _EventInfo)
    {
        if (PrototypingAssets.run_server_code.value == false)//if we are not the server???
            return;//ok something weird is happening... but ok lets just supress this for now... TODO - add debug WTF??!?!?! statement?

        foreach(Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            
            //TODO - if the fog of war hides this event? - continue//or you hide just the start or final territory or num troops...
            my_PhotonView.RPC("ServerInvokedRPC_Oncapture", _Player, _EventInfo.battle_log_string);
        }
               
    }

    void EventReceiver_Onfortify_MultiplayerBridge(RiskySandBox_Team.EventInfo_Onfortify _EventInfo)
    {
        if (PrototypingAssets.run_server_code.value == false)//if we are not the server????
            return;//ok something weird is happening... but fine lets just supress for now... TODO - debug WTF??!?!?!??!!?

        foreach(Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - if the fog of war hides this event - continue;
            //TODO we also may want to tell the player  the "path" that the fortify took??
            //but dont tell them about the route if they cant see the tiles
            my_PhotonView.RPC("ServerInvokedRPC_Onfortify", _Player, _EventInfo.battle_log_string);
        }

    }


    void EventReceiver_Onnuke_MultiplayerBridge(RiskySandBox_Tile _start_Tile, RiskySandBox_Tile _target_Tile)
    {
        if (PrototypingAssets.run_server_code.value == false)//if we are NOT the server???
            return;//ok... why is this happening...

        int _start_ID = RiskySandBox_Tile.null_ID;
        int _target_Tile_ID = (int)_target_Tile.ID;

        foreach (Photon.Realtime.Player _Player in PhotonNetwork.PlayerListOthers)
        {
            //TODO - if the fog of war hides this event? - continue

            my_PhotonView.RPC("ServerInvokedRPC_Onnuke", _Player, _start_ID, _target_Tile_ID);
        }

    }

    [PunRPC]
    void ServerInvokedRPC_Ondeploy(string _server_battle_log, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender != PhotonNetwork.MasterClient)//only listen to the server!
            return;

        //server just told this client about a deploy event...
        RiskySandBox_Team.invokeEvent_Ondeploy(_server_battle_log,_alert_MultiplayerBridge:false);
    }

    [PunRPC]
    void ServerInvokedRPC_Onattack(string _server_battle_log, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender != PhotonNetwork.MasterClient)//only listen to the server!
            return;

        //server has just told this client about a attack event...

        RiskySandBox_Team.invokeEvent_Onattack(_server_battle_log,_alert_MultiplayerBridge:false);
    }

    [PunRPC]
    void ServerInvokedRPC_Oncapture(string _server_battle_log, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender.IsMasterClient == false)//ONLY LISTEND TO THE SERVER
            return;//TODO - report this player as a cheater? - or something is going wrong...

        //ok! - the server just told us a about a capture event!
        RiskySandBox_Team.invokeEvent_Oncapture(_server_battle_log,_alert_MultiplayerBridge:false);
    }

    [PunRPC]
    void ServerInvokedRPC_Onfortify(string _battle_log_string, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender.IsMasterClient == false)//ONLY LISTEND TO THE SERVER
            return;//TODO - report this player as a cheater? - or something is going wrong...

        RiskySandBox_Team.invokeEvent_Onfortify(_battle_log_string,_alert_MultiplayerBridge:false);
    }


    [PunRPC]
    void ServerInvokedRPC_endGame(PhotonMessageInfo _PhotonMessageInfo)
    {
        //disconnect...
        if (_PhotonMessageInfo.Sender != PhotonNetwork.MasterClient)//only listen to the server!
            return;

        //server just told this client about the endGame has happened...
        //TODO... no do RiskySandBox_MainGame.invokeEvent_endGame....
        RiskySandBox_MainGame.instance.endGame("ServerInfovedRPC_endGame",null);
    }

    [PunRPC]
    void ServerInvokedRPC_Onnuke(int _start_Tile_ID,int _target_Tile_ID, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (_PhotonMessageInfo.Sender != PhotonNetwork.MasterClient)
            return;

        RiskySandBox_Tile _start_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_start_Tile_ID);
        RiskySandBox_Tile _target_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_target_Tile_ID);

        RiskySandBox_ItemsManager.instance.invokeEvent_Onnuke(_start_Tile, _target_Tile, _alert_MultiplayerBridge: false);

    }






}
