using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame_CapitalsMode : MonoBehaviour
{

    public static RiskySandBox_MainGame_CapitalsMode instance;

    [SerializeField] bool debugging;

    




    public static ObservableBool setup_complete { get { return instance.PRIVATE_setup_complete; } }



    [SerializeField] ObservableBool PRIVATE_setup_complete;





    private void Awake()
    {
        instance = this;
        RiskySandBox_Team.OnplaceCapital += EventReceiver_OnplaceCapital;
    }

    void EventReceiver_OnplaceCapital(RiskySandBox_Team _Team,RiskySandBox_Tile _Tile)
    {
        if (RiskySandBox_MainGame_CapitalsMode.setup_complete == true)
            return;
        //place the
        _Team.current_turn_state.value = RiskySandBox_Team.turn_state_waiting;

        List<RiskySandBox_Team> _remaining_Teams = RiskySandBox_Team.all_instances.Where(x => x.required_capital_placements > 0).ToList();

        if(_remaining_Teams.Count == 0)//if all the capitals have been placed???
        {
            RiskySandBox_MainGame_CapitalsMode.setup_complete.value = true;
            RiskySandBox_MainGame.instance.EventReceiver_OncapitalsModeSetupComplete();//hand control back to the maingame!
            return;
        }

        int _index = RiskySandBox_Team.all_instances.IndexOf(_Team);
        while(true)
        {
            _index += 1;
            if(_index >= RiskySandBox_Team.all_instances.Count)
            {
                _index = 0;
            }
            if(RiskySandBox_Team.all_instances[_index].required_capital_placements > 0)
            {
                RiskySandBox_Team.all_instances[_index].current_turn_state.value = RiskySandBox_Team.turn_state_placing_capital;
                break;
            }
        }




        
    }

    //TODO -semi auto setup, auto setup, manual placement...
    public void startGame()
    {

        foreach(RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
        {
            _Team.required_capital_placements.value += _Team.num_capitals_startGame;
            //all the tiles this team can place a capital onto...
            List<RiskySandBox_Tile> _valid_Tiles = _Team.my_Tiles.Where(x => x.has_capital == false).ToList();

            //TODO - we also want to add any ally Tiles???

            if (_Team.required_capital_placements > _valid_Tiles.Count())
            {
                if (this.debugging)
                    GlobalFunctions.print("clamping required_capital_placements to " + _valid_Tiles,this);
                _Team.required_capital_placements.value = _valid_Tiles.Count;
            }
                

        }

        RiskySandBox_Team _start_Team = RiskySandBox_Team.all_instances.Where(x => x.required_capital_placements.value > 0).FirstOrDefault();

        if(_start_Team == null)
        {
            this.PRIVATE_setup_complete.value = true;
            RiskySandBox_MainGame.instance.EventReceiver_OncapitalsModeSetupComplete();
            return;
        }

        _start_Team.current_turn_state.value = RiskySandBox_Team.turn_state_placing_capital;
    }
}
