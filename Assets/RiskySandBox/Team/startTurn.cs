using System.Collections;using System.Collections.Generic;using System.Linq;using System;


public partial class RiskySandBox_Team
{

    public static event Action<RiskySandBox_Team> OnstartTurn;

    public void startTurn()
    {
        if (this.debugging)
            GlobalFunctions.print("starting my turn!", this);

        this.end_turn_time_stamp.value = RiskySandBox.current_time.value + this.turn_length_seconds.value;
        this.is_my_turn.value = true;


        this.deployable_troops.value += this.calculateTroopGeneration();

        RiskySandBox_Team.OnstartTurn?.Invoke(this);





        if (this.required_capital_placements > 0)//if the team has to place a capital... what happens if the team has no capital options...
        {
            this.current_turn_state.value = RiskySandBox_Team.turn_state_placing_capital;
        }

        else if (this.num_cards >= this.max_n_territory_cards && (this.allow_fixed_tradein || this.allow_progressive_tradein || this.allow_geometric_tradein))
        {
            if (this.debugging)
                GlobalFunctions.print("putting _Team into the force trade in state... because they have too many cards...", this);
            this.current_turn_state.value = RiskySandBox_Team.turn_state_force_trade_in;
        }

        else if (this.deployable_troops > 0)
        {
            this.current_turn_state.value = RiskySandBox_Team.turn_state_deploy;
        }

    }


}
