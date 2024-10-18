using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Mirror;

public partial class MultiplayerBridge_Mirror_VariableSyncing
{
    [TargetRpc]
    void receiveObservableVector3ListFromServerTarget(NetworkConnectionToClient _target, int _index, Vector3[] _values)
    {
        ObservableVector3List _List = this.my_ObservableVariables.my_ObservableVector3Lists[_index];
        _List.SET_itemsFromMultiplayerBridge(_values);
    }

    [ClientRpc]
    void receiveObservableVector3ListFromServer(int _index, Vector3[] _values)
    {
        if (PrototypingAssets.run_server_code.value == true)
            return;

        ObservableVector3List _List = this.my_ObservableVariables.my_ObservableVector3Lists[_index];
        _List.SET_itemsFromMultiplayerBridge(_values);
    }

    void ObservableVector3ListEventReceiver_Onsynchronize(ObservableVector3List _List)
    {
        if (sendValueToOthers(_List.my_VariableSettings, false))
            syncObservableVector3List(_List, null);
    }

    void syncObservableVector3List(ObservableVector3List _List, NetworkConnectionToClient _client)
    {
        int _index = my_ObservableVariables.my_ObservableVector3Lists.IndexOf(_List);
        Vector3[] _values = _List.ToArray();


        //if we are the server???
        if (PrototypingAssets.run_server_code.value == true)
        {
            //send to everyone...
            if (_client == null)
                this.receiveObservableVector3ListFromServer(_index, _values);
            else
                this.receiveObservableVector3ListFromServerTarget(_client, _index, _values);
        }
        else
        {
            //the server will receive the new change then send back out to the clients...
            syncObservableVector3ListViaServer(_index, _values);

        }
    }



    [Command(requiresAuthority = false)]
    void syncObservableVector3ListViaServer(int _index, Vector3[] _values, NetworkConnectionToClient _sender = null)
    {
        //if the value is server authority...
        ObservableVector3List _List = this.my_ObservableVariables.my_ObservableVector3Lists[_index];

        if (acceptValueFromOther(_List.my_VariableSettings, _sender))
        {
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn == _sender || conn == NetworkServer.localConnection)
                    continue;

                receiveObservableVector3ListFromServerTarget(conn, _index, _values);
            }

            _List.SET_itemsFromMultiplayerBridge(_values);
        }
    }
}
