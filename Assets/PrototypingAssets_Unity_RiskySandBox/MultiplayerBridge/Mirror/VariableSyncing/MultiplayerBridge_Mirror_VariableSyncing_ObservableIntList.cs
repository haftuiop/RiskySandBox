using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Mirror;

public partial class MultiplayerBridge_Mirror_VariableSyncing
{
    [TargetRpc]
    void receiveObservableIntListFromServerTarget(NetworkConnectionToClient _target, int _index, int[] _values)
    {
        ObservableIntList _List = this.my_ObservableVariables.my_ObservableIntLists[_index];
        _List.SET_itemsFromMultiplayerBridge(_values);
    }

    [ClientRpc]
    void receiveObservableIntListFromServer(int _index, int[] _values)
    {
        if (PrototypingAssets.run_server_code.value == true)
            return;

        ObservableIntList _List = this.my_ObservableVariables.my_ObservableIntLists[_index];
        _List.SET_itemsFromMultiplayerBridge(_values);
    }

    void ObservableIntListEventReceiver_Onsynchronize(ObservableIntList _List)
    {
        if (sendValueToOthers(_List.my_VariableSettings, false))
            syncObservableIntList(_List, null);
    }

    void syncObservableIntList(ObservableIntList _List, NetworkConnectionToClient _client)
    {
        int _index = my_ObservableVariables.my_ObservableIntLists.IndexOf(_List);
        int[] _values = _List.ToArray();


        //if we are the server???
        if (PrototypingAssets.run_server_code.value == true)
        {
            //send to everyone...
            if (_client == null)
                this.receiveObservableIntListFromServer(_index, _values);
            else
                this.receiveObservableIntListFromServerTarget(_client, _index, _values);
        }
        else
        {
            //the server will receive the new change then send back out to the clients...
            syncObservableIntListViaServer(_index, _values);

        }
    }



    [Command(requiresAuthority = false)]
    void syncObservableIntListViaServer(int _index, int[] _values, NetworkConnectionToClient _sender = null)
    {
        //if the value is server authority...
        ObservableIntList _List = this.my_ObservableVariables.my_ObservableIntLists[_index];

        if (acceptValueFromOther(_List.my_VariableSettings, _sender))
        {
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn == _sender || conn == NetworkServer.localConnection)
                    continue;

                receiveObservableIntListFromServerTarget(conn, _index, _values);
            }

            _List.SET_itemsFromMultiplayerBridge(_values);
        }
    }
}
