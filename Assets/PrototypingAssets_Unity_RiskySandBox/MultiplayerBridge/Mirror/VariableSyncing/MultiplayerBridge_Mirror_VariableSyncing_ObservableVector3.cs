using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Mirror;

public partial class MultiplayerBridge_Mirror_VariableSyncing
{

    [TargetRpc]
    void receiveObservableVector3FromServerTarget(NetworkConnectionToClient _target, int _index, Vector3 _value,Vector3 _min_value,Vector3 _max_value)
    {
        ObservableVector3 _ObservableVector3 = this.my_ObservableVariables.my_ObservableVector3s[_index];
        _ObservableVector3.SET_valueFromMultiplayerBridge(_value,_min_value,_max_value);
    }

    [ClientRpc]
    void receiveObservableVector3FromServer(int _index, Vector3 _value,Vector3 _min_value,Vector3 _max_value)
    {
        if (PrototypingAssets.run_server_code.value == true)
            return;

        ObservableVector3 _ObservableVector3 = this.my_ObservableVariables.my_ObservableVector3s[_index];
        _ObservableVector3.SET_valueFromMultiplayerBridge(_value,_min_value,_max_value);
    }

    void ObservableVector3EventReceiver_Onsynchronize(ObservableVector3 _ObservableVector3)
    {
        if (sendValueToOthers(_ObservableVector3.my_VariableSettings, false))
            syncObservableVector3(_ObservableVector3, null);
    }

    void syncObservableVector3(ObservableVector3 _ObservableVector3, NetworkConnectionToClient _client)
    {
        int _index = my_ObservableVariables.my_ObservableVector3s.IndexOf(_ObservableVector3);
        Vector3 _value = _ObservableVector3.value;
        Vector3 _min_value = _ObservableVector3.min_value;
        Vector3 _max_value = _ObservableVector3.max_value;

        //if we are the server???
        if (PrototypingAssets.run_server_code.value == true)
        {
            //send to everyone...
            if (_client == null)
                this.receiveObservableVector3FromServer(_index, _value, _min_value, _max_value);
            else
                this.receiveObservableVector3FromServerTarget(_client, _index, _value, _min_value, _max_value);
        }
        else
        {
            //the server will receive the new change then send back out to the clients...
            syncObservableVector3ViaServer(_index, _value,_min_value,_max_value);

        }
    }



    [Command(requiresAuthority = false)]
    void syncObservableVector3ViaServer(int _index, Vector3 _value, Vector3 _min_value, Vector3 _max_value,NetworkConnectionToClient _sender = null)
    {
        //if the value is server authority...
        ObservableVector3 _ObservableVector3 = this.my_ObservableVariables.my_ObservableVector3s[_index];

        if (acceptValueFromOther(_ObservableVector3.my_VariableSettings, _sender))
        {
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn == _sender || conn == NetworkServer.localConnection)
                    continue;

                receiveObservableVector3FromServerTarget(conn, _index, _value, _min_value, _max_value);
            }

            _ObservableVector3.SET_valueFromMultiplayerBridge(_value, _min_value, _max_value);
        }
    }
}
