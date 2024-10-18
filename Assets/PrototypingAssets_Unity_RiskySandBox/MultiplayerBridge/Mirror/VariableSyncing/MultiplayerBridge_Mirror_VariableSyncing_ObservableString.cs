using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Mirror;

public partial class MultiplayerBridge_Mirror_VariableSyncing
{

    [TargetRpc]
    void receiveObservableStringFromServerTarget(NetworkConnectionToClient _target, int _index, string _value)
    { 
        ObservableString _ObservableString = this.my_ObservableVariables.my_ObservableStrings[_index];
        _ObservableString.SET_valueFromMultiplayerBridge(_value);
    }

    [ClientRpc]
    void receiveObservableStringFromServer(int _index, string _value)
    {
        if (PrototypingAssets.run_server_code.value == true)
            return;

        ObservableString _ObservableString = this.my_ObservableVariables.my_ObservableStrings[_index];
        _ObservableString.SET_valueFromMultiplayerBridge(_value);
    }

    void ObservableStringEventReceiver_Onsynchronize(ObservableString _ObservableString)
    {
        if (sendValueToOthers(_ObservableString.my_VariableSettings, false))
            syncObservableString(_ObservableString, null);
    }

    void syncObservableString(ObservableString _ObservableString, NetworkConnectionToClient _client)
    {
        int _index = my_ObservableVariables.my_ObservableStrings.IndexOf(_ObservableString);
        string _value = _ObservableString.value;

        //if we are the server???
        if (PrototypingAssets.run_server_code.value == true)
        {
            //send to everyone...
            if (_client == null)
                this.receiveObservableStringFromServer(_index, _value);
            else
                this.receiveObservableStringFromServerTarget(_client, _index, _value);
        }
        else
        {
            //the server will receive the new change then send back out to the clients...
            syncObservableStringViaServer(_index, _value);

        }
    }



    [Command(requiresAuthority = false)]
    void syncObservableStringViaServer(int _index, string _value,NetworkConnectionToClient _sender = null)
    {
        //if the value is server authority...
        ObservableString _ObservableString = this.my_ObservableVariables.my_ObservableStrings[_index];

        if (acceptValueFromOther(_ObservableString.my_VariableSettings, _sender))
        {
            foreach(var conn in NetworkServer.connections.Values)
            {
                if (conn == _sender || conn == NetworkServer.localConnection)
                    continue;

                receiveObservableStringFromServerTarget(conn, _index, _value);
            }

            _ObservableString.SET_valueFromMultiplayerBridge(_value);
        }
    }
}
