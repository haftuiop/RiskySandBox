using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class MultiplayerBridge_Photon_VariableSyncing
{
	void ObservableIntListEventReceiver_Onsynchronize(ObservableIntList _ObservableList)
    {
		if (sendValueToOthers(_ObservableList.my_VariableSettings,false) == false)
		{
			if (debugging)
				GlobalFunctions.print("sendValueToOthers returned false...", _ObservableList);
			return;
		}

		syncObservableIntList(_ObservableList, null);
	}

	void syncObservableIntList(ObservableIntList _ObservableList, Photon.Realtime.Player _target_Player)
	{


		string _rpc_name = "Pun_RPCreceiveNewValue_ObservableIntList";//the name of the rpc that needs to get called (see below)
		int _variable_index = my_ObservableVariables.my_ObservableIntLists.IndexOf(_ObservableList);

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




	[PunRPC]
    void Pun_RPCreceiveNewValue_ObservableIntList(int _index,int[] _content,PhotonMessageInfo _PhotonMessageInfo)
    {
        var _ObservableList = this.my_ObservableVariables.my_ObservableIntLists[_index];

        if(acceptNewValueFromOther(_ObservableList.my_VariableSettings,_PhotonMessageInfo) == false)
        {
            if (this.debugging)
                GlobalFunctions.print("acceptNewValueFromOther returned false...", _ObservableList);
            return;
        }

        if (this.debugging)
            GlobalFunctions.print("calling _ObservableVariable.SET_itemsFromMultiplayerBridge", _ObservableList);

        _ObservableList.SET_itemsFromMultiplayerBridge(_content);
    }



}
