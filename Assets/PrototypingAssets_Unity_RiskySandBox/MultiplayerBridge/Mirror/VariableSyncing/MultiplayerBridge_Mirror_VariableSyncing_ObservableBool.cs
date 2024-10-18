using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Mirror;

public partial class MultiplayerBridge_Mirror_VariableSyncing
{

    [TargetRpc]
    void receiveObservableBoolFromServerTarget(NetworkConnectionToClient _target, int _index,bool _value)
    {
        ObservableBool _ObservableBool = this.my_ObservableVariables.my_ObservableBools[_index];
        _ObservableBool.SET_valueFromMultiplayerBridge(_value);
    }

    [ClientRpc]
    void receiveObservableBoolFromServer(int _index, bool _value)
    {
        if (PrototypingAssets.run_server_code.value == true)
            return;

        ObservableBool _ObservableBool = this.my_ObservableVariables.my_ObservableBools[_index];
        _ObservableBool.SET_valueFromMultiplayerBridge(_value);
    }

    void ObservableBoolEventReceiver_Onsynchronize(ObservableBool _ObservableBool)
    {
        if (sendValueToOthers(_ObservableBool.my_VariableSettings, false))
            syncObservableBool(_ObservableBool, null);
    }


    void syncObservableBool(ObservableBool _ObservableBool,NetworkConnectionToClient _client)
    {
        //if we are the server???
        if(PrototypingAssets.run_server_code.value == true)
        {
            int _index = my_ObservableVariables.my_ObservableBools.IndexOf(_ObservableBool);
            bool _value = _ObservableBool.value;
            //send to everyone...
            if (_client == null)
                this.receiveObservableBoolFromServer(_index, _value);
            else
                this.receiveObservableBoolFromServerTarget(_client,_index, _value);
        }
        else
        {
            //the server will receive the new change then send back out to the clients...
            syncObservableBoolViaServer(my_ObservableVariables.my_ObservableBools.IndexOf(_ObservableBool), _ObservableBool.value);

        }
    }



    [Command(requiresAuthority = false)]
    void syncObservableBoolViaServer(int _index, bool _value, NetworkConnectionToClient sender = null)
    {
        ObservableBool _ObservableBool = this.my_ObservableVariables.my_ObservableBools[_index];

        if(acceptValueFromOther(_ObservableBool.my_VariableSettings,sender))
        {
            //send change to all connected clients....
            foreach(var conn in NetworkServer.connections.Values)
            {
                if (conn == sender || conn == NetworkServer.localConnection)
                    continue;

                receiveObservableBoolFromServerTarget(conn, _index, _value);
            }

            _ObservableBool.SET_valueFromMultiplayerBridge(_value);//update for myself...
        }
    }

}
