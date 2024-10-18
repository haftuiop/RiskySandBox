using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

//functions to sync the ObservableStrings
public partial class MultiplayerBridge_Photon_VariableSyncing
{
	void ObservableStringEventReceiver_Onsynchronize(ObservableString _ObservableVariable)
    {
		if (sendValueToOthers(_ObservableVariable.my_VariableSettings,false) == false)
		{
			if (debugging)
				GlobalFunctions.print("sendValueToOthers returned false...", _ObservableVariable);
			return;
		}

		syncObservableString(_ObservableVariable,null);
	}


	void syncObservableString(ObservableString _ObservableVariable,Photon.Realtime.Player _target_Player)
	{
		string _rpc_name = "PunRPC_receiveNewValue_ObservableString";//the name of the rpc that needs to get called (see below)
		int _variable_index = my_ObservableVariables.my_ObservableStrings.IndexOf(_ObservableVariable);

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
	void PunRPC_receiveNewValue_ObservableString(int _index, string _new_value, PhotonMessageInfo _PhotonMessageInfo)
	{
		ObservableString _ObservableVariable = this.my_ObservableVariables.my_ObservableStrings[_index];//get the ObservabelVariable that needs syncronising

		if (acceptNewValueFromOther(_ObservableVariable.my_VariableSettings, _PhotonMessageInfo) == false)
		{
			if (this.debugging)
				GlobalFunctions.print("acceptNewValueFromOther returned false...", _ObservableVariable);
			return;
		}

		if (this.debugging)
			GlobalFunctions.print("calling _ObservableVariable.SET_value(" + _new_value + ", false)", _ObservableVariable);

		_ObservableVariable.SET_valueFromMultiplayerBridge(_new_value);//update the value - pass in false to indicate that syncronise should not be called...
	}


}

