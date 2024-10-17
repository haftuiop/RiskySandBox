using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;


//this script is designed to simulate the "flag" from https://www.youtube.com/watch?v=prulSmWK3lw

public partial class RiskySandBox_ArcoFlag : MonoBehaviour
{
    static string end_game_debug_string { get { return "RiskySandBox_ArcoFlag"; } }

    static int time_to_capture { get { return 1; } }//so you must hold the flag for 1 turn...



    [SerializeField] bool debugging;
    [SerializeField] ObservableInt tile_ID;
    [SerializeField] ObservableInt victory_turn_number;

    RiskySandBox_Tile my_Tile { get { return RiskySandBox_Tile.GET_RiskySandBox_Tile(this.tile_ID); } }
    RiskySandBox_Team my_Team { get { return my_Tile.my_Team; } }

    private void Awake()
    {
        RiskySandBox_Team.OnstartTurn += EventReceiver_OnstartTurn;
        RiskySandBox_Team.Oncapture += EventReceiver_Oncapture;
    }


    void EventReceiver_Oncapture(RiskySandBox_Team.EventInfo_Oncapture _EventInfo)
    {
        if (_EventInfo.target_tile != this.my_Tile)
        {
            if (this.debugging)
                GlobalFunctions.print("wasnt my_Tile... returning", this);
            return;
        }

        victory_turn_number.value = RiskySandBox_MainGame.turn_number.value + time_to_capture;
    }


    void EventReceiver_OnstartTurn(RiskySandBox_Team _Team)
    {
        //ok did my team just start their turn?????
        if (_Team != this.my_Team)
        {
            if (this.debugging)
                GlobalFunctions.print("wasnt my team... returning", this);
            return;
        }

        //ok so my team just started the turn...
        int _turn_number = RiskySandBox_MainGame.turn_number.value;

        if (_turn_number != victory_turn_number)
        {
            if (this.debugging)
                GlobalFunctions.print("_turn_number != this.victory_turn_number...", this);
            return;
        }
        RiskySandBox_MainGame.instance.endGame(end_game_debug_string, my_Team);
    }
}
