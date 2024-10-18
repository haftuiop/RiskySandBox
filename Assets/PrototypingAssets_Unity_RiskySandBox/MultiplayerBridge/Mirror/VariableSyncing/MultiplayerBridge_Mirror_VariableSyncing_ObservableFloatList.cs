using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Mirror;

public partial class MultiplayerBridge_Mirror_VariableSyncing
{

    [TargetRpc]
    void receiveObservableFloatListFromServerTarget(NetworkConnectionToClient _target, int _index, float[] _values)
    {
        ObservableFloatList _List = this.my_ObservableVariables.my_ObservableFloatLists[_index];
        _List.SET_itemsFromMultiplayerBridge(_values);
    }

    [ClientRpc]
    void receiveObservableFloatListFromServer(int _index, float[] _values)
    {
        if (PrototypingAssets.run_server_code.value == true)
            return;

        ObservableFloatList _List = this.my_ObservableVariables.my_ObservableFloatLists[_index];
        _List.SET_itemsFromMultiplayerBridge(_values);
    }


    void ObservableFloatListEventReceiver_Onsynchronize(ObservableFloatList _List)
    {
        if (sendValueToOthers(_List.my_VariableSettings, false))
            syncObservableFloatList(_List, null);
    }

    void syncObservableFloatList(ObservableFloatList _List, NetworkConnectionToClient _client)
    {
        int _index = this.my_ObservableVariables.my_ObservableFloatLists.IndexOf(_List);
        float[] _values = _List.ToArray();

        if(PrototypingAssets.run_server_code == true)
        {
            if (_client == null)
                receiveObservableFloatListFromServer(_index, _values);
            else
                receiveObservableFloatListFromServerTarget(_client, _index, _values);
        }

    }

    [Command(requiresAuthority = false)]
    void syncObservableFloatListViaServer(int _index, float[] _values, NetworkConnectionToClient _sender = null)
    {
        //if the value is server authority...
        ObservableFloatList _List = this.my_ObservableVariables.my_ObservableFloatLists[_index];

        if (acceptValueFromOther(_List.my_VariableSettings, _sender))
        {
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn == _sender || conn == NetworkServer.localConnection)
                    continue;

                receiveObservableFloatListFromServerTarget(conn, _index, _values);
            }

            _List.SET_itemsFromMultiplayerBridge(_values);
        }
    }


}
