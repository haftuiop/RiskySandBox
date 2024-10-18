using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Mirror;

public partial class MultiplayerBridge_Mirror_VariableSyncing
{

    [TargetRpc]
    void receiveObservableIntFromServerTarget(NetworkConnectionToClient _target, int _index, int _value, int _min_value, int _max_value)
    {
        ObservableInt _ObservableInt = this.my_ObservableVariables.my_ObservableInts[_index];
        _ObservableInt.SET_valueFromMultiplayerBridge(_value, _min_value, _max_value);
    }

    [ClientRpc]
    void receiveObservableIntFromServer(int _index, int _value, int _min_value, int _max_value)
    {
        if (PrototypingAssets.run_server_code.value == true)
            return;

        ObservableInt _ObservableInt = this.my_ObservableVariables.my_ObservableInts[_index];
        _ObservableInt.SET_valueFromMultiplayerBridge(_value, _min_value, _max_value);
    }

    void ObservableIntEventReceiver_Onsynchronize(ObservableInt _ObservableInt)
    {
        if (sendValueToOthers(_ObservableInt.my_VariableSettings, false))
            syncObservableInt(_ObservableInt, null);
    }


    void syncObservableInt(ObservableInt _ObservableInt, NetworkConnectionToClient _client)
    {
        int _index = my_ObservableVariables.my_ObservableInts.IndexOf(_ObservableInt);
        int _value = _ObservableInt.value;
        int _min_value = _ObservableInt.min_value;
        int _max_value = _ObservableInt.max_value;

        //if we are the server???
        if (PrototypingAssets.run_server_code.value == true)
        {
            //send to everyone...
            if (_client == null)
                this.receiveObservableIntFromServer(_index, _value, _min_value, _max_value);
            else
                this.receiveObservableIntFromServerTarget(_client, _index, _value, _min_value, _max_value);
        }
        else
        {
            //the server will receive the new change then send back out to the clients...
            syncObservableIntViaServer(_index, _value, _min_value, _max_value);

        }
    }



    [Command(requiresAuthority = false)]
    void syncObservableIntViaServer(int _index, int _value, int _min_value, int _max_value,NetworkConnectionToClient _sender = null)
    {
        //if the value is server authority...
        ObservableInt _ObservableInt = this.my_ObservableVariables.my_ObservableInts[_index];

        if (acceptValueFromOther(_ObservableInt.my_VariableSettings, _sender))
        {
            foreach(var conn in NetworkServer.connections.Values)
            {
                if (conn == _sender || conn == NetworkServer.localConnection)
                    continue;

                receiveObservableIntFromServerTarget(_sender, _index, _value, _min_value, _max_value);
            }

            _ObservableInt.SET_valueFromMultiplayerBridge(_value, _min_value, _max_value);
            
        }
    }






}
