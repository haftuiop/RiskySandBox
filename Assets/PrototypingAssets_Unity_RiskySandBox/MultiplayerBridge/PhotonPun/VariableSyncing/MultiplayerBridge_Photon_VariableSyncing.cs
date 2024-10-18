using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


[RequireComponent(typeof(PhotonView))]
public partial class MultiplayerBridge_Photon_VariableSyncing : MonoBehaviour
{

	[SerializeField] bool debugging;
	[SerializeField] ObservableClasses_VariableFinder my_ObservableVariables;

	PhotonView my_PhotonView;


	/// <summary>
	/// another client just tried to send us a new value... should we accept this new value??? or ignore this...
	/// </summary>
	bool acceptNewValueFromOther(ObservableClasses_VariableSettings _VariableSettings,PhotonMessageInfo _PhotonMessageInfo)
	{
		if(_VariableSettings.is_server_authority && _PhotonMessageInfo.Sender.IsMasterClient == false)//if it is a server variable (and the sender isnt the server...)
        {
			if (this.debugging)
				GlobalFunctions.print("was a server variable but the _PhotonMessageInfo.Sender wasnt the MasterClient... returning false cheater or bug?!?!?", _VariableSettings);
			return false;
        }

		if (_VariableSettings.is_owner_authority && _PhotonMessageInfo.Sender != my_PhotonView.Owner)//only allow the owner to change an owner variable...
        {
			if(this.debugging)
				GlobalFunctions.print("was a owner variable but the PhotonMessageInfo.Sender wasn't my_PhotonView.Owner... returning false cheater or bug?!?!?", _VariableSettings);
			return false;
        }

		if(_VariableSettings.is_local_authority)
        {
			if(this.debugging)
				GlobalFunctions.print("was local authority... returning false cheater or bug?!?!?!", _VariableSettings);
			return false;
        }

		if (this.debugging)
			GlobalFunctions.print("returning true!", _VariableSettings);
		return true;
	}

	bool sendValueToOthers(ObservableClasses_VariableSettings _VariableSettings,bool _new_player_connected)
    {
		if(MultiplayerBridge_PhotonPun.in_room == false)
        {
			if (this.debugging)
				GlobalFunctions.print("not in a room... returning false",this);
			return false;
        }

		if(_VariableSettings.is_local_authority)
        {
			if (this.debugging)
				GlobalFunctions.print("variable is local authority... returning false",_VariableSettings);
			return false;
        }

		if(_VariableSettings.is_owner_authority && my_PhotonView.IsMine)
        {
			if (this.debugging)
				GlobalFunctions.print("variable is owner authority and I am the owner of my_PhotonView", _VariableSettings);
			return true;
        }

		if(_VariableSettings.is_server_authority && (PrototypingAssets.run_server_code.value == false))
        {
			if (this.debugging)
				GlobalFunctions.print("variable is server authority and I am not the server... returning false",_VariableSettings);
			return false;
        }

		if(_VariableSettings.is_shared_authority)
        {
			if(_new_player_connected && (PrototypingAssets.run_server_code.value == false))
            {
				if (this.debugging)
					GlobalFunctions.print("variable is shared authority but I am not the server and a new player just connected...", _VariableSettings);
				return false;
            }
        }


		return true;
    }




	void EventReceiver_OnplayerConnected(Player _newPlayer)
	{
		//sync all the values to the new player!

		foreach(ObservableBool _ObservableVariable in my_ObservableVariables.my_ObservableBools.Where(x => x != null))
		{
			if(sendValueToOthers(_ObservableVariable.my_VariableSettings,true))
				syncObservableBool(_ObservableVariable, _newPlayer);
		}

		foreach (ObservableFloat _ObservableVariable in my_ObservableVariables.my_ObservableFloats.Where(x => x != null))
		{
			if(sendValueToOthers(_ObservableVariable.my_VariableSettings,true))
				syncObservableFloat(_ObservableVariable, _newPlayer);
		}

		foreach (ObservableInt _ObservableVariable in my_ObservableVariables.my_ObservableInts.Where(x => x != null))
		{
			if (sendValueToOthers(_ObservableVariable.my_VariableSettings, true))
				syncObservableInt(_ObservableVariable, _newPlayer);
			
		}

		foreach (ObservableString _ObservableVariable in my_ObservableVariables.my_ObservableStrings.Where(x => x != null))
		{
			if (sendValueToOthers(_ObservableVariable.my_VariableSettings, true))
				syncObservableString(_ObservableVariable, _newPlayer);
		}

		foreach (ObservableVector3 _ObservableVariable in my_ObservableVariables.my_ObservableVector3s.Where(x => x != null))
		{
			if (sendValueToOthers(_ObservableVariable.my_VariableSettings, true))
				syncObservableVector3(_ObservableVariable, _newPlayer);
		}

		foreach(ObservableIntList _ObservableList in my_ObservableVariables.my_ObservableIntLists.Where(x => x != null))
        {
			if (sendValueToOthers(_ObservableList.my_VariableSettings, true))
				syncObservableIntList(_ObservableList, _newPlayer);
		}

		foreach (ObservableVector3List _ObservableList in my_ObservableVariables.my_ObservableVector3Lists.Where(x => x != null))
		{
			if (sendValueToOthers(_ObservableList.my_VariableSettings, true))
				syncObservableVector3List(_ObservableList, _newPlayer);
		}

	}

	private void Awake()
	{
		MultiplayerBridge_PhotonPun.OnplayerConnected += EventReceiver_OnplayerConnected;

		my_PhotonView = GetComponent<PhotonView>();


		if(my_ObservableVariables == null)
        {
			GlobalFunctions.printError("my_MultiplayerVariables is null!!!", this);
			return;
        }


		if (my_ObservableVariables.search_at_Awake)
		{
			if (my_ObservableVariables.search_at_Awake_completed)
				subscribeToVariables();
			else
				my_ObservableVariables.OnAwakeSearchCompleted += EventReceiver_OnsearchComplete;
		}
		else
			subscribeToVariables();
			



	}

    private void OnDestroy()
    {

		MultiplayerBridge_PhotonPun.OnplayerConnected -= EventReceiver_OnplayerConnected;

		if (my_ObservableVariables != null)
			my_ObservableVariables.OnAwakeSearchCompleted -= EventReceiver_OnsearchComplete;
	}

    void EventReceiver_OnsearchComplete(ObservableClasses_VariableFinder _VariableFinder)
    {
		subscribeToVariables();
    }

	void subscribeToVariables()
    {
		foreach (ObservableBool _ObservableVariable in this.my_ObservableVariables.my_ObservableBools)
		{
			if (_ObservableVariable == null){ GlobalFunctions.printWarning("null variable in my_ObservableBools...", this); continue; }

			_ObservableVariable.Onsynchronize += delegate { ObservableBoolEventReceiver_Onsynchronize(_ObservableVariable); };
		}

		foreach (ObservableFloat _ObservableVariable in this.my_ObservableVariables.my_ObservableFloats)
		{
			if (_ObservableVariable == null){ GlobalFunctions.printWarning("null variable in my_ObservableFloats...", this); continue; }

			_ObservableVariable.Onsynchronize += delegate { ObservableFloatEventReceiver_Onsynchronize(_ObservableVariable); };
		}

		foreach (ObservableInt _ObservableVariable in this.my_ObservableVariables.my_ObservableInts)
		{
			if (_ObservableVariable == null){ GlobalFunctions.printWarning("null variable in my_ObservableInts...", this); continue; }

			_ObservableVariable.Onsynchronize += delegate { ObservableIntEventReceiver_Onsynchronize(_ObservableVariable); };
		}

		foreach (ObservableString _ObservableVariable in this.my_ObservableVariables.my_ObservableStrings)
		{
			if (_ObservableVariable == null){ GlobalFunctions.printWarning("null variable in my_ObservableStrings...", this); continue; }

			_ObservableVariable.Onsynchronize += delegate { ObservableStringEventReceiver_Onsynchronize(_ObservableVariable); };
		}

		foreach (ObservableVector3 _ObservableVariable in this.my_ObservableVariables.my_ObservableVector3s)
		{
			if (_ObservableVariable == null){ GlobalFunctions.printWarning("null variable in my_ObservableVector3s...", this); continue; }

			_ObservableVariable.Onsynchronize += delegate { ObservableVector3EventReceiver_Onsynchronize(_ObservableVariable); };
		}


		//lists....
		foreach(ObservableBoolList _ObservableList in this.my_ObservableVariables.my_ObservableBoolLists)
        {
			if(_ObservableList == null) { GlobalFunctions.printWarning("null variable in my_ObservableBoolLists...", this);continue; }

			_ObservableList.Onsynchronize += delegate { ObservableBoolListEventReceiver_Onsynchronize(_ObservableList); };
        }

		foreach (ObservableFloatList _ObservableList in this.my_ObservableVariables.my_ObservableFloatLists)
		{
			if (_ObservableList == null) { GlobalFunctions.printWarning("null variable in my_ObservableFloatLists...", this); continue; }

			_ObservableList.Onsynchronize += delegate { ObservableFloatListEventReceiver_Onsynchronize(_ObservableList); };
		}


		foreach (ObservableIntList _ObservableList in this.my_ObservableVariables.my_ObservableIntLists)
        {
			if(_ObservableList == null){ GlobalFunctions.print("null variable in my_ObservableIntLists...",this);continue; }

			_ObservableList.Onsynchronize += delegate { ObservableIntListEventReceiver_Onsynchronize(_ObservableList); };
        }

		foreach (ObservableStringList _ObservableList in this.my_ObservableVariables.my_ObservableStringLists)
		{
			if (_ObservableList == null) { GlobalFunctions.print("null variable in my_ObservableStringLists...", this); continue; }

			_ObservableList.Onsynchronize += delegate { ObservableStringListEventReceiver_Onsynchronize(_ObservableList); };
		}

		foreach (ObservableVector3List _ObservableList in this.my_ObservableVariables.my_ObservableVector3Lists)
        {
			if(_ObservableList == null) { GlobalFunctions.printWarning("null variable in my_ObservableVector3Lists",this);continue; }

			_ObservableList.Onsynchronize += delegate { ObservableVector3ListEventReceiver_Onsynchronize(_ObservableList); };

        }
	}
}

