using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class MultiplayerBridge_Photon_VariableSyncing
{
    void ObservableVector3ListEventReceiver_Onsynchronize(ObservableVector3List _ObservableList)
    {
		if (sendValueToOthers(_ObservableList.my_VariableSettings,false) == false)
		{
			if (debugging)
				GlobalFunctions.print("sendValueToOthers returned false...", _ObservableList);
			return;
		}

		syncObservableVector3List(_ObservableList,null);
	}

	void syncObservableVector3List(ObservableVector3List _ObservableList, Photon.Realtime.Player _target_Player)
	{


		string _rpc_name = "Pun_RPCreceiveNewValue_ObservableVector3List";//the name of the rpc that needs to get called (see below)
		int _variable_index = my_ObservableVariables.my_ObservableVector3Lists.IndexOf(_ObservableList);

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
	void Pun_RPCreceiveNewValue_ObservableVector3List(int _index, Vector3[] _content, PhotonMessageInfo _PhotonMessageInfo)
	{
		var _ObservableList = this.my_ObservableVariables.my_ObservableVector3Lists[_index];

		if (acceptNewValueFromOther(_ObservableList.my_VariableSettings, _PhotonMessageInfo) == false)
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
