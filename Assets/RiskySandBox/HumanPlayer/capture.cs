using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_HumanPlayer
{
    public void TRY_capture(int _n_troops)
    {
        if (this.my_Team.current_turn_state != RiskySandBox_Team.turn_state_capture)
            return;

        if (MultiplayerBridge_PhotonPun.in_room)
        {
            if (this.debugging)
                GlobalFunctions.print("asking master client to capture for me...", this);

            my_PhotonView.RPC("ClientInvokedRPC_capture", RpcTarget.MasterClient, _n_troops);
        }

        else if (MultiplayerBridge_Mirror.is_enabled)
        {
            if (this.debugging)
                GlobalFunctions.print("calling my_DedicatedServerCommands.try_capture...", this);

            my_DedicatedServerCommands.TRY_capture(_n_troops);
        }

        else
            Debug.LogError("not connected?");
        
    }






    [PunRPC]
    void ClientInvokedRPC_capture(int _n_troops, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;

        my_Team.TRY_capture(_n_troops);
    }



}
