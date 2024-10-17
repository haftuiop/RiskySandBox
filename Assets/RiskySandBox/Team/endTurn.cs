using System.Collections;using System.Collections.Generic;using System.Linq;using System;

public partial class RiskySandBox_Team
{

    public static event Action<RiskySandBox_Team> OnendTurn;


    public void endTurn(string _debug_reason)
    {

        if (PrototypingAssets.run_server_code.value == false)
        {

            GlobalFunctions.printWarning("not the server???... why is this happening???", this);
            return;
        }


        if (this.debugging)
            GlobalFunctions.print("ending the teams turn..." + _debug_reason, this, _debug_reason);


        if (this.has_captured_Tile && this.num_cards < this.max_n_territory_cards)
            RiskySandBox_MainGame.instance.drawTerritoryCard(this, this.n_territory_cards_from_capture);


        this.has_captured_Tile.value = false;//TODO redo this to something like this.n_captures_this_turn.value = 0

        //auto place the required capital
        //deply required troops?
        //capture
        //cancel any fortify...

        this.current_turn_state.value = RiskySandBox_Team.turn_state_waiting;
        this.deployable_troops.value = 0;//TODO - what if the player is allowed to "save" troops each turn???
        this.is_my_turn.value = false;



        



        this.current_turn_state.value = RiskySandBox_Team.turn_state_waiting;

        RiskySandBox_Team.OnendTurn?.Invoke(this);
    }

}