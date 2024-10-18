using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Mirror;


[RequireComponent(typeof(Mirror.NetworkIdentity))]
public partial class MultiplayerBridge_Mirror_VariableSyncing : NetworkBehaviour
{
    static bool run_server_code { get { return PrototypingAssets.run_server_code.value; } }

    [SerializeField] bool debugging;




    [SerializeField] ObservableClasses_VariableFinder my_ObservableVariables;

    Mirror.NetworkIdentity my_NetworkIdentity;


    void Awake()
    {
        my_NetworkIdentity = GetComponent<NetworkIdentity>();
        MultiplayerBridge_Mirror.OnplayerConnected += EventReceiver_OnplayerConnected;

        if (my_ObservableVariables.search_at_Awake_completed)
            subscribeToSyncroniseEvents();
        else if(my_ObservableVariables.search_at_Awake)
            my_ObservableVariables.OnAwakeSearchCompleted += delegate { subscribeToSyncroniseEvents(); };
    }

    private void OnDestroy()
    {
        MultiplayerBridge_Mirror.OnplayerConnected -= EventReceiver_OnplayerConnected;
    }

    bool acceptValueFromOther(ObservableClasses_VariableSettings _VariableSettings, NetworkConnectionToClient _sender)
    {
        //if we are the server

        if (_VariableSettings.is_server_authority)
            return false;

        if (_VariableSettings.is_local_authority)
            return false;

        if (_VariableSettings.is_shared_authority)
            return true;

        if (_VariableSettings.is_owner_authority)
        {
            return my_NetworkIdentity.connectionToClient == _sender;
        }

        return false;

    }

    bool sendValueToOthers(ObservableClasses_VariableSettings _VariableSettings,bool _new_player_connected)
    {
        if (MultiplayerBridge_Mirror.is_enabled == false)
            return false;

        if (_VariableSettings.is_local_authority)
        {
            if (this.debugging)
                GlobalFunctions.print("variable is local authority... returning false", _VariableSettings);
            return false;
        }

        if (_VariableSettings.is_owner_authority && my_NetworkIdentity.isOwned)
        {
            if (this.debugging)
                GlobalFunctions.print("variable is owner authority and I am the owner of my_PhotonView", _VariableSettings);
            return true;
        }

        if (_VariableSettings.is_server_authority && run_server_code == false)
        {
            if (this.debugging)
                GlobalFunctions.print("variable is server authority and I am not the server... returning false", _VariableSettings);
            return false;
        }

        if (_VariableSettings.is_shared_authority)
        {
            if (_new_player_connected && run_server_code == false)
            {
                if (this.debugging)
                    GlobalFunctions.print("variable is shared authority but I am not the server and a new player just connected...", _VariableSettings);
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// new player connected! - lets send all the most up to date values...
    /// </summary>
    void EventReceiver_OnplayerConnected(NetworkConnectionToClient conn)
    {
        if (conn == NetworkServer.localConnection)
            return;

        if(PrototypingAssets.run_server_code == false)
        {
            GlobalFunctions.print("not the server (don't have ability to send updated variables to other clients???)", this);
            return;
        }
    
        //sync all the values to the new player....
        foreach(ObservableBool _ObservableVariable in this.my_ObservableVariables.my_ObservableBools)
        {
            if (sendValueToOthers(_ObservableVariable.my_VariableSettings,true) == false)
                continue;
            
            syncObservableBool(_ObservableVariable, conn);
        }

        foreach(ObservableFloat _ObservableVariable in this.my_ObservableVariables.my_ObservableFloats)
        {
            if (sendValueToOthers(_ObservableVariable.my_VariableSettings, true) == false)
                continue;

            syncObservableFloat(_ObservableVariable, conn);
        }

        foreach(ObservableInt _ObservableVariable in this.my_ObservableVariables.my_ObservableInts)
        {
            if (sendValueToOthers(_ObservableVariable.my_VariableSettings, true) == false)
                continue;

            syncObservableInt(_ObservableVariable, conn);
        }

        foreach (ObservableString _ObservableVariable in this.my_ObservableVariables.my_ObservableStrings)
        {
            if (sendValueToOthers(_ObservableVariable.my_VariableSettings, true) == false)
                continue;

            syncObservableString(_ObservableVariable, conn);
        }

        foreach (ObservableVector3 _ObservableVariable in this.my_ObservableVariables.my_ObservableVector3s)
        {
            if (sendValueToOthers(_ObservableVariable.my_VariableSettings, true) == false)
                continue;

            syncObservableVector3(_ObservableVariable, conn);
        }

        foreach(ObservableIntList _List in this.my_ObservableVariables.my_ObservableIntLists)
        {
            if (sendValueToOthers(_List.my_VariableSettings, true) == false)
                continue;

            syncObservableIntList(_List, conn);
        }

        foreach(ObservableVector3List _List in this.my_ObservableVariables.my_ObservableVector3Lists)
        {
            if (sendValueToOthers(_List.my_VariableSettings, true) == false)
                continue;

            syncObservableVector3List(_List, conn);
        }
    }


    void subscribeToSyncroniseEvents()
    {
        foreach(ObservableBool _ObservableVariable in this.my_ObservableVariables.my_ObservableBools)
        {
            _ObservableVariable.Onsynchronize += ObservableBoolEventReceiver_Onsynchronize;
        }

        foreach(ObservableFloat _ObservableVariable in this.my_ObservableVariables.my_ObservableFloats)
        {
            _ObservableVariable.Onsynchronize += ObservableFloatEventReceiver_Onsynchronize;
        }

        foreach(ObservableInt _ObservableVariable in this.my_ObservableVariables.my_ObservableInts)
        {
            _ObservableVariable.Onsynchronize += ObservableIntEventReceiver_Onsynchronize;
        }

        foreach (ObservableString _ObservableVariable in this.my_ObservableVariables.my_ObservableStrings)
        {
            _ObservableVariable.Onsynchronize += ObservableStringEventReceiver_Onsynchronize;
        }

        foreach (ObservableVector3 _ObservableVariable in this.my_ObservableVariables.my_ObservableVector3s)
        {
            _ObservableVariable.Onsynchronize += ObservableVector3EventReceiver_Onsynchronize;
        }

        //======================lists==============================
        foreach(ObservableBoolList _List in this.my_ObservableVariables.my_ObservableBoolLists)
        {
            _List.Onsynchronize += ObservableBoolListEventReceiver_Onsynchronize;
        }

        foreach(ObservableFloatList _List in this.my_ObservableVariables.my_ObservableFloatLists)
        {
            _List.Onsynchronize += ObservableFloatListEventReceiver_Onsynchronize;
        }
        
        foreach(ObservableIntList _List in this.my_ObservableVariables.my_ObservableIntLists)
        {
            _List.Onsynchronize += ObservableIntListEventReceiver_Onsynchronize;
        }

        foreach(ObservableStringList _List in this.my_ObservableVariables.my_ObservableStringLists)
        {
            _List.Onsynchronize += ObservableStringListEventReceiver_Onsynchronize;
        }

        foreach(ObservableVector3List _List in this.my_ObservableVariables.my_ObservableVector3Lists)
        {
            _List.Onsynchronize += ObservableVector3ListEventReceiver_Onsynchronize;
        }
    }

    



}
