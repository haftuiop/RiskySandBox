using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;


public partial class MultiplayerBridge_Photon_VariableSyncing
{

         
    void ObservableBoolListEventReceiver_Onsynchronize(ObservableBoolList _ObservableList)
    {

        if(sendValueToOthers(_ObservableList.my_VariableSettings,false) == false)
        {
            if (debugging)
                GlobalFunctions.print("sendValueToOthers returned false...", _ObservableList);
            return;
        }

        this.syncObservableBoolList(_ObservableList,null);
    }

    void syncObservableBoolList(ObservableBoolList _ObservableList, Photon.Realtime.Player _target_Player)
    {
        int _variable_index = this.my_ObservableVariables.my_ObservableBoolLists.IndexOf(_ObservableList);
        string _rpc_name = "Pun_RPCreceiveNewValue_ObservableBoolList";


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


    void Pun_RPCreceiveNewValue_ObservableBoolList(int _index,bool[] _values, PhotonMessageInfo _PhotonMessageInfo)
    {
        ObservableBoolList _ObservableList = this.my_ObservableVariables.my_ObservableBoolLists[_index];

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
