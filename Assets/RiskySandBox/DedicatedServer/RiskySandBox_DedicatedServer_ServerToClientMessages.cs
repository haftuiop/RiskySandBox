using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Mirror;

public partial class RiskySandBox_DedicatedServer_ServerToClientMessages : NetworkBehaviour
{
    [SerializeField] bool debugging;
	

    void Awake()
    {
        RiskySandBox_MainGame.OnstartGame_MultiplayerBridge += EventReceiver_OnstartGame_MultiplayerBridge;
        RiskySandBox_MainGame.OnendGame_MultiplayerBridge += EventReceiver_OnendGame_MultiplayerBridge;
        RiskySandBox_ItemsManager.Onnuke_MultiplayerBridge += EventReceiver_Onnuke_MultiplayerBridge;
        RiskySandBox_ItemsManager.OndetonateLandMine_MultiplayerBridge += EventReceiver_OndetonateLandMine_MultiplayerBridge;


        RiskySandBox_Team.Ondeploy_MultiplayerBridge += RiskySandBox_TeamEventReceiver_Ondeploy_MultiplayerBridge;
        RiskySandBox_Team.Onattack_MultiplayerBridge += EventReceiver_Onattack_MultiplayerBridge;
        RiskySandBox_Team.Onfortify_MultiplayerBridge += EventReceiver_Onfortify_MultiplayerBridge;
        RiskySandBox_Team.Oncapture_MultiplayerBridge += EventReceiver_Oncapture_MultiplayerBridge;
    }

    void EventReceiver_OnstartGame_MultiplayerBridge()
    {

    }

    void EventReceiver_OnendGame_MultiplayerBridge()
    {

    }

    void EventReceiver_OndetonateLandMine_MultiplayerBridge(RiskySandBox_Tile _Tile)
    {
        if (MultiplayerBridge_Mirror.is_enabled == false)
            return;

        if (PrototypingAssets.run_server_code == false)
            return;//TODO - wtf?!?!?!?

        foreach(KeyValuePair<int,NetworkConnectionToClient> _KVP in NetworkServer.connections)
        {
            //TODO - decide if we should tell this player about the event...
            this.ServerInvokedRPC_OndetonateLandMine(_KVP.Value, _Tile.ID.value);
        }


    }

    void EventReceiver_Onnuke_MultiplayerBridge(RiskySandBox_Tile _from, RiskySandBox_Tile _Tile)
    {
        if (MultiplayerBridge_Mirror.is_enabled == false)
            return;

        if (PrototypingAssets.run_server_code == false)
            return;//TODO - WTF?!?!?!?!?!

        int _from_ID = RiskySandBox_Tile.null_ID;
        if (_from != null)
            _from_ID = _from.ID.value;

        foreach(KeyValuePair<int,NetworkConnectionToClient> _KVP in NetworkServer.connections)
        {
            //TODO - decide if this player should know about this event??? e.g. if fog of war is enabled... and they cant see the Tile??? - dont allow this...
            this.ServerInvokedRPC_Onnuke(_KVP.Value, _from_ID, _Tile.ID.value);
        }
        
    } 

    void RiskySandBox_TeamEventReceiver_Ondeploy_MultiplayerBridge(RiskySandBox_Team.EventInfo_Ondeploy _EventInfo)
    {
        if (MultiplayerBridge_Mirror.is_enabled == false)
            return;

        if (PrototypingAssets.run_server_code == false)
            return;//TODO - WTF>!!?!?!?!?!?


        foreach (KeyValuePair<int,NetworkConnectionToClient> _KVP in NetworkServer.connections)
        {
            //TODO - make sure the fog of war doesnt hide this infomation...
            ServerInvokedRPC_Ondeploy(_KVP.Value, _EventInfo.battle_log_string);
        }
    }

    void EventReceiver_Onattack_MultiplayerBridge(RiskySandBox_Team.EventInfo_Onattack _EventInfo)
    {
        if (MultiplayerBridge_Mirror.is_enabled == false)
            return;

        if (PrototypingAssets.run_server_code == false)
            return;//TODO - WTF>!!?!?!?!?!?


        foreach (KeyValuePair<int, NetworkConnectionToClient> _KVP in NetworkServer.connections)
        {
            //TODO - make sure the fog of war doesnt hide this infomation...
            ServerInvokedRPC_Onattack(_KVP.Value, _EventInfo.battle_log_string);
        }
    }

    void EventReceiver_Onfortify_MultiplayerBridge(RiskySandBox_Team.EventInfo_Onfortify _EventInfo)
    {
        if (MultiplayerBridge_Mirror.is_enabled == false)
            return;

        if (PrototypingAssets.run_server_code == false)
            return;//TODO - wtf?!?!?!

        foreach(KeyValuePair<int,NetworkConnectionToClient> _KVP in NetworkServer.connections)
        {
            //TODO - if the fog of war hides this? - continue
            ServerInvokedRPC_Onfortify(_KVP.Value, _EventInfo.battle_log_string);
        }

    }

    void EventReceiver_Oncapture_MultiplayerBridge(RiskySandBox_Team.EventInfo_Oncapture _EventInfo)
    {
        if (MultiplayerBridge_Mirror.is_enabled == false)
            return;

        if (PrototypingAssets.run_server_code == false)
            return;//TODO - wtf?!?!?!

        foreach (KeyValuePair<int, NetworkConnectionToClient> _KVP in NetworkServer.connections)
        {
            //TODO - if the fog of war hides this? -continue
            ServerInvokedRPC_Oncapture(_KVP.Value, _EventInfo.battle_log_string);
        }

    }


    [Mirror.TargetRpc]
    void ServerInvokedRPC_Ondeploy(NetworkConnectionToClient _target, string _battle_log_string)
    {
        if (PrototypingAssets.run_server_code == true)
            return;
        RiskySandBox_Team.invokeEvent_Ondeploy(_battle_log_string,false);//this client doesnt need to alert the multiplayer bridge...
    }

    [Mirror.TargetRpc]
    void ServerInvokedRPC_Onattack(NetworkConnectionToClient _target, string _battle_log_string)
    {
        if (PrototypingAssets.run_server_code == true)
            return;
        RiskySandBox_Team.invokeEvent_Onattack(_battle_log_string, false);//this client doesnt need to alert the multiplayer bridge...
    }

    [Mirror.TargetRpc]
    void ServerInvokedRPC_Onfortify(NetworkConnectionToClient _target, string _battle_log_string)
    {
        if (PrototypingAssets.run_server_code == true)
            return;
        RiskySandBox_Team.invokeEvent_Onfortify(_battle_log_string, false);//this client doesnt need to alert the multiplayer bridge...
    }

    [Mirror.TargetRpc]
    void ServerInvokedRPC_Oncapture(NetworkConnectionToClient _target, string _battle_log_string)
    {
        if (PrototypingAssets.run_server_code == true)
            return;
        RiskySandBox_Team.invokeEvent_Oncapture(_battle_log_string, false);//this client doesnt need to alert the multiplayer bridge...
    }

    [Mirror.TargetRpc]
    void ServerInvokedRPC_Onnuke(NetworkConnectionToClient _target, int _start_Tile_ID, int _target_Tile_ID)
    {
        if (PrototypingAssets.run_server_code)//server MIGHT have sent itself a message... but it doesnt need to worry if this happened (probably running as a server and client)
            return;

        //the server just sent us a message saying a tile has been nuked...

        RiskySandBox_Tile _start_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_start_Tile_ID);
        RiskySandBox_Tile _target_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_target_Tile_ID);

        RiskySandBox_ItemsManager.instance.invokeEvent_Onnuke(_start_Tile, _target_Tile,_alert_MultiplayerBridge:false);
    }

    [Mirror.TargetRpc]
    void ServerInvokedRPC_OndetonateLandMine(NetworkConnectionToClient _target,int _tile_ID)
    {
        if (PrototypingAssets.run_server_code)
            return;
        //ok! the server just told us that a landmine detonated...
        RiskySandBox_ItemsManager.instance.invokeEvent_OndetonateLandMine(RiskySandBox_Tile.GET_RiskySandBox_Tile(_tile_ID),_alert_MultiplayerBridge:false);
    }


}
