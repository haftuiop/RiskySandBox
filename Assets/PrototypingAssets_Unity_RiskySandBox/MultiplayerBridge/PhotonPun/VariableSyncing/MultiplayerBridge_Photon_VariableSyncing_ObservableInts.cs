using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

//Functions to sync the ObservableInts
public partial class MultiplayerBridge_Photon_VariableSyncing
{
	void ObservableIntEventReceiver_Onsynchronize(ObservableInt _ObservableVariable)
	{
		if (sendValueToOthers(_ObservableVariable.my_VariableSettings, false) == false)
		{
			if (debugging)
				GlobalFunctions.print("sendValueToOthers returned false...", _ObservableVariable);
			return;
		}

		syncObservableInt(_ObservableVariable, null);
	}

	void syncObservableInt(ObservableInt _ObservableVariable,Photon.Realtime.Player _target_Player)
	{
		string _rpc_name = "PunRPC_receiveNewValue_ObservableInt";//the name of the rpc that needs to get called (see below)
		int _variable_index = my_ObservableVariables.my_ObservableInts.IndexOf(_ObservableVariable);

		if (_target_Player == null)
		{
			if (debugging)
				GlobalFunctions.print("sending rpc to others", _ObservableVariable);
			my_PhotonView.RPC(_rpc_name, RpcTarget.Others, _variable_index, _ObservableVariable.value,_ObservableVariable.min_value,_ObservableVariable.max_value);
			return;
		}

		if (debugging)
			GlobalFunctions.print("sending rpc to _target_player " + _target_Player, _ObservableVariable);
		my_PhotonView.RPC(_rpc_name, _target_Player, _variable_index, _ObservableVariable.value,_ObservableVariable.min_value,_ObservableVariable.max_value);
	}




	[PunRPC]
	void PunRPC_receiveNewValue_ObservableInt(int _index, int _new_value,int _min_value,int _max_value, PhotonMessageInfo _PhotonMessageInfo)
	{
		ObservableInt _ObservableVariable = this.my_ObservableVariables.my_ObservableInts[_index];//get the ObservabelVariable that needs syncronising

		if (acceptNewValueFromOther(_ObservableVariable.my_VariableSettings, _PhotonMessageInfo) == false)
		{
			if (this.debugging)
				GlobalFunctions.print("acceptNewValueFromOther returned false...", _ObservableVariable);
			return;
		}

		if (this.debugging)
			GlobalFunctions.print("calling _ObservableVariable.SET_value(" + _new_value + ", false)", _ObservableVariable);

		_ObservableVariable.SET_valueFromMultiplayerBridge(_new_value,_min_value,_max_value);//update the value - pass in false to indicate that syncronise should not be called...
	}
}

