using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Mirror;

public partial class MultiplayerBridge_Mirror_VariableSyncing
{

    [TargetRpc]
    void receiveObservableFloatFromServerTarget(NetworkConnectionToClient _target, int _index, float _value, float _min_value,float _max_value)
    {
        ObservableFloat _ObservableFloat = this.my_ObservableVariables.my_ObservableFloats[_index];
        _ObservableFloat.SET_valueFromMultiplayerBridge(_value,_min_value,_max_value);
    }

    [ClientRpc]
    void receiveObservableFloatFromServer(int _index, float _value, float _min_value, float _max_value)
    {
        if (PrototypingAssets.run_server_code.value == true)
            return;

        ObservableFloat _ObservableFloat = this.my_ObservableVariables.my_ObservableFloats[_index];
        _ObservableFloat.SET_valueFromMultiplayerBridge(_value,_min_value,_max_value);
    }

    void ObservableFloatEventReceiver_Onsynchronize(ObservableFloat _ObservableFloat)
    {
        if (sendValueToOthers(_ObservableFloat.my_VariableSettings, false))
            syncObservableFloat(_ObservableFloat, null);
    }


    void syncObservableFloat(ObservableFloat _ObservableFloat, NetworkConnectionToClient _client)
    {
        int _index = my_ObservableVariables.my_ObservableFloats.IndexOf(_ObservableFloat);
        float _value = _ObservableFloat.value;
        float _min_value = _ObservableFloat.min_value;
        float _max_value = _ObservableFloat.max_value;

        //if we are the server???
        if (PrototypingAssets.run_server_code.value == true)
        {
            //send to everyone...
            if (_client == null)
                this.receiveObservableFloatFromServer(_index, _value, _min_value, _max_value);
            else
                this.receiveObservableFloatFromServerTarget(_client, _index, _value, _min_value, _max_value);
        }
        else
        {
            //the server will receive the new change then send back out to the clients...
            syncObservableFloatViaServer(_index, _value, _min_value, _max_value);

        }
    }



    [Command(requiresAuthority = false)]
    void syncObservableFloatViaServer(int _index, float _value, float _min_value, float _max_value, NetworkConnectionToClient sender = null)
    {
        //if the value is server authority...
        ObservableFloat _ObservableFloat = this.my_ObservableVariables.my_ObservableFloats[_index];

        if (acceptValueFromOther(_ObservableFloat.my_VariableSettings, sender))
        {
            foreach(var conn in NetworkServer.connections.Values)
            {
                if (conn == sender || conn == NetworkServer.localConnection)
                    continue;

                receiveObservableFloatFromServerTarget(conn, _index, _value,_min_value,_max_value);

            }

            _ObservableFloat.SET_valueFromMultiplayerBridge(_value, _min_value, _max_value);
        }
    }



}
