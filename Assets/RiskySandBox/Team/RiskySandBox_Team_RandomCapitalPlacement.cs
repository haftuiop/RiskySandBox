using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team_RandomCapitalPlacement : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] RiskySandBox_Team my_Team { get { return GetComponent<RiskySandBox_Team>(); } }

    


    private void Awake()
    {
        //listen in for when we change to a new turn state...
        my_Team.current_turn_state.OnUpdate += EventReceiver_OnVariableUpdate_current_turn_state;
    }



    
    void EventReceiver_OnVariableUpdate_current_turn_state(ObservableString _current_turn_state)
    {
        if (my_Team.random_capital_placement.value == false)
        {
            if (this.debugging)
                GlobalFunctions.print("random capital placement is disabled for this team... just returning immediantly", this);
            return;
        }

        if (_current_turn_state != RiskySandBox_Team.turn_state_placing_capital)
        {
            if (this.debugging)
                GlobalFunctions.print("not in the capital placement state... so just returning immediatly", this);
            return;
        }

        if (PrototypingAssets.run_server_code.value == false)
        {
            if (this.debugging)
                GlobalFunctions.print("not the server (so not my responsibility to do this...)", this);
            return;
        }

        //ok! lets try to place all the required capitals...
        int _required_capital_placements = this.my_Team.required_capital_placements.value;

        if (this.debugging)
            GlobalFunctions.print(string.Format("trying to place {0} capitals (randomly)",_required_capital_placements), this);

        for(int i = 0; i < _required_capital_placements; i += 1)
        {
            this.my_Team.TRY_placeCapital_random();
        }
    }
}
