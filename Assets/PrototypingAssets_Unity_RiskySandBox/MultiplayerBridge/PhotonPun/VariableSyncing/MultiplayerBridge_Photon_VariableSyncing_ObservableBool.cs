using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

//Functions to syncronise the ObservableBools
public partial class MultiplayerBridge_Photon_VariableSyncing
{
	void ObservableBoolEventReceiver_Onsynchronize(ObservableBool _ObservableVariable)
	{
		if(this.sendValueToOthers(_ObservableVariable.my_VariableSettings,false) == false)
        {
			if (this.debugging)
				GlobalFunctions.print("sendValueToOthers returned false...", this);
			return;
        }

		syncObservableBool(_ObservableVariable, null);
	}

	void syncObservableBool(ObservableBool _ObservableVariable,Photon.Realtime.Player _target_Player)
    {
		string _rpc_name = "PunRPC_receiveNewValue_ObservableBool";//the name of the rpc that needs to get called (see below)
		int _variable_index = my_ObservableVariables.my_ObservableBools.IndexOf(_ObservableVariable);

		if (_target_Player == null)
		{
			if (debugging)
				GlobalFunctions.print("sending rpc to others", _ObservableVariable);
			my_PhotonView.RPC(_rpc_name, RpcTarget.Others, _variable_index, _ObservableVariable.value);
			return;
		}

		if (debugging)
			GlobalFunctions.print("sending rpc to _target_player " + _target_Player, _ObservableVariable);
		my_PhotonView.RPC(_rpc_name, _target_Player, _variable_index, _ObservableVariable.value);
	}

	[PunRPC]
	void PunRPC_receiveNewValue_ObservableBool(int _index, bool _new_value, PhotonMessageInfo _PhotonMessageInfo)
	{
		ObservableBool _ObservableVariable = this.my_ObservableVariables.my_ObservableBools[_index];//get the ObservabelVariable that needs syncronising

		if(acceptNewValueFromOther(_ObservableVariable.my_VariableSettings,_PhotonMessageInfo) == false)
		{
			if (this.debugging)
				GlobalFunctions.print("acceptNewValueFromOther returned false...", _ObservableVariable);
			return;
		}

		if (this.debugging)
			GlobalFunctions.print("calling _ObservableVariable.SET_valueFromMultiplayerBridge(" + _new_value + ", false)",_ObservableVariable);

		_ObservableVariable.SET_valueFromMultiplayerBridge(_new_value);
	}



}

