using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

using Photon.Pun;

public partial class RiskySandBox_HumanPlayer
{





 


    public void TRY_fortify(RiskySandBox_Tile _from,RiskySandBox_Tile _to, int _n_troops)
    {
        //TODO - add if(debugging) statemenets...
        if (_from == null)
            return;
        if (_to == null)
            return;

        if (this.my_Team.current_turn_state != RiskySandBox_Team.turn_state_fortify)
            return;


        int _from_ID = (int)_from.ID;
        int _to_ID = (int)_to.ID;

        if (MultiplayerBridge_PhotonPun.in_room)
        {
            if (this.debugging)
                GlobalFunctions.print("asking the master client to fortify...", this);

            my_PhotonView.RPC("ClientInvokedRPC_fortify", RpcTarget.MasterClient, _from_ID, _to_ID, (int)_n_troops);
        }

        else if (MultiplayerBridge_Mirror.is_enabled)
        {
            if (this.debugging)
                GlobalFunctions.print("calling my_DedicatedServerCommands.TRY_fortify...", this);

            my_DedicatedServerCommands.TRY_fortify(_from_ID, _to_ID, _n_troops);
        }

        else
            GlobalFunctions.printError("not connected?", this);


        
    }





    [PunRPC]
    void ClientInvokedRPC_fortify(int _from_ID, int _to_ID, int _n_troops, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;

        RiskySandBox_Tile _from = RiskySandBox_Tile.GET_RiskySandBox_Tile(_from_ID);
        RiskySandBox_Tile _to = RiskySandBox_Tile.GET_RiskySandBox_Tile(_to_ID);

        this.my_Team.TRY_fortify(_from, _to, _n_troops);
    }
}
