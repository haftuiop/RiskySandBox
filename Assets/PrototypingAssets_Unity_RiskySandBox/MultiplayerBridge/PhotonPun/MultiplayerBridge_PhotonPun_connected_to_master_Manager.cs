using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public partial class MultiplayerBridge_PhotonPun_connected_to_master_Manager : MonoBehaviourPunCallbacks
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableBool PRIVATE_connected_to_master;


    public override void OnConnectedToMaster()
    {
        if (this.debugging)
            GlobalFunctions.print("just connected to master... setting connected_to_master.value to true...",this);

        base.OnConnectedToMaster();
        PRIVATE_connected_to_master.value = true;
    }

    public override void OnDisconnected(DisconnectCause _cause)
    {
        if (this.debugging)
            GlobalFunctions.print("just disconnected... _cause = " + _cause + " setting connected_to_master.value to false...", this);

        base.OnDisconnected(_cause);
        PRIVATE_connected_to_master.value = false;
    }



}
