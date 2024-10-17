using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team
{



    public bool TRY_goToNextTurnState()
    {

        if(this.current_turn_state == RiskySandBox_Team.turn_state_waiting)
        {
            GlobalFunctions.print("the team is currently in waiting state... returning false", this);
            return false;
        }

        if(this.current_turn_state == RiskySandBox_Team.turn_state_deploy)
        {
            //make sure no deployable troops...
            if(this.deployable_troops > 0)
            {
                if(this.debugging)
                    GlobalFunctions.print("this.deployable_troops > 0", this);
                return false;
            }

            this.current_turn_state.value = RiskySandBox_Team.turn_state_attack;
        }

        else if(this.current_turn_state == RiskySandBox_Team.turn_state_attack)
        {
            //ok! go to fortify...
            this.current_turn_state.value = RiskySandBox_Team.turn_state_fortify;
        }

        else if(this.current_turn_state == RiskySandBox_Team.turn_state_capture)
        {
            if(this.debugging)
                GlobalFunctions.print("in capture state... returning false",this);
            return false;
        }

        else if(this.current_turn_state.value == RiskySandBox_Team.turn_state_fortify)
        {
            //end turn...
            this.endTurn("end turn in fortify state");
        }



        return true;
    }
}
