using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Mirror;

public partial class MultiplayerBridge_Mirror_VariableSyncing
{
    [TargetRpc]
    void receiveObservableStringListFromServerTarget(NetworkConnectionToClient _target, int _index, string[] _values)
    {
        ObservableStringList _List = this.my_ObservableVariables.my_ObservableStringLists[_index];
        _List.SET_itemsFromMultiplayerBridge(_values);
    }

    [ClientRpc]
    void receiveObservableStringListFromServer(int _index, string[] _values)
    {
        if (PrototypingAssets.run_server_code.value == true)
            return;

        ObservableStringList _List = this.my_ObservableVariables.my_ObservableStringLists[_index];
        _List.SET_itemsFromMultiplayerBridge(_values);
    }

    void ObservableStringListEventReceiver_Onsynchronize(ObservableStringList _List)
    {
        if (sendValueToOthers(_List.my_VariableSettings, false))
            syncObservableStringList(_List, null);
    }

    void syncObservableStringList(ObservableStringList _List, NetworkConnectionToClient _client)
    {
        int _index = my_ObservableVariables.my_ObservableStringLists.IndexOf(_List);
        string[] _values = _List.ToArray();


        //if we are the server???
        if (PrototypingAssets.run_server_code.value == true)
        {
            //send to everyone...
            if (_client == null)
                this.receiveObservableStringListFromServer(_index, _values);
            else
                this.receiveObservableStringListFromServerTarget(_client, _index, _values);
        }
        else
        {
            //the server will receive the new change then send back out to the clients...
            syncObservableStringListViaServer(_index, _values);

        }
    }



    [Command(requiresAuthority = false)]
    void syncObservableStringListViaServer(int _index, string[] _values, NetworkConnectionToClient _sender = null)
    {
        //if the value is server authority...
        ObservableStringList _List = this.my_ObservableVariables.my_ObservableStringLists[_index];

        if (acceptValueFromOther(_List.my_VariableSettings, _sender))
        {
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn == _sender || conn == NetworkServer.localConnection)
                    continue;

                receiveObservableStringListFromServerTarget(conn, _index, _values);
            }

            _List.SET_itemsFromMultiplayerBridge(_values);
        }
    }
}
