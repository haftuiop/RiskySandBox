using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class MultiplayerBridge_Photon_VariableSyncing
{


    void ObservableFloatListEventReceiver_Onsynchronize(ObservableFloatList _ObservableList)
    {
        if (sendValueToOthers(_ObservableList.my_VariableSettings, false) == false)
        {
            if (debugging)
                GlobalFunctions.print("sendValueToOthers returned false...", _ObservableList);
            return;
        }

        syncObservableFloatList(_ObservableList, null);
    }



    void syncObservableFloatList(ObservableFloatList _ObservableList,Photon.Realtime.Player _target_Player)
    {
        int _variable_index = this.my_ObservableVariables.my_ObservableFloatLists.IndexOf(_ObservableList);
        string _rpc_name = "Pun_RPCreceiveNewValue_ObservableFloatList";

        if (_target_Player == null)
        {
            if (debugging)
                GlobalFunctions.print("sending rpc to others", _ObservableList);
            my_PhotonView.RPC(_rpc_name, RpcTarget.Others, _variable_index, _ObservableList.ToArray());
            return;
        }

        if (debugging)
            GlobalFunctions.print("sending rpc to _target_player " + _target_Player, _ObservableList);
        my_PhotonView.RPC(_rpc_name, _target_Player, _variable_index, _ObservableList.ToArray());

    }


    [Photon.Pun.PunRPC]
    void Pun_RPCreceiveNewValue_ObservableFloatList(int _index,float[] _values,PhotonMessageInfo _PhotonMessageInfo)
    {
        ObservableFloatList _ObservableList = this.my_ObservableVariables.my_ObservableFloatLists[_index];

        if (this.acceptNewValueFromOther(_ObservableList.my_VariableSettings, _PhotonMessageInfo) == false)
        {
            if (this.debugging)
                GlobalFunctions.print("acceptNewValueFroamOther returned false...", _ObservableList);
            return;
        }

        if (this.debugging)
            GlobalFunctions.print("calling _ObservableVariable.SET_itemsFromMultiplayerBridge", _ObservableList);

        //lets accept the value...
        _ObservableList.SET_itemsFromMultiplayerBridge(_values);
    }





}
