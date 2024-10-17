using System.Collections;using System.Collections.Generic;using System.Linq;using System;


public partial class RiskySandBox_Team
{
    public static event Action<EventInfo_Oncapture> Oncapture_MultiplayerBridge;
    public static event Action<EventInfo_Oncapture> Oncapture;


    public struct EventInfo_Oncapture
    {
        public string battle_log_string
        {
            get
            {
                return string.Format("GameEvent_capture:{0},{1},{2},{3}", this.Team.ID, this.start_tile.ID, this.target_tile.ID, this.n_troops);
            }
            set
            {
                string[] _data = value.Split(":")[1].Split(",").ToArray();
                this.Team = RiskySandBox_Team.GET_RiskySandBox_Team(int.Parse(_data[0]));
                this.start_tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(int.Parse(_data[1]));
                this.target_tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(int.Parse(_data[2]));
                this.n_troops = int.Parse(_data[3]);
            }
        }
        //TODO also want to say where they captured from?????
        public string human_log_string
        {
            get
            {
                return string.Format("Team '{0}' captured {1} with {2} troop(s)", this.Team.team_name.value, this.target_tile.human_ui_log_string, this.n_troops);
            }
        }


        //TODO - make these a getter private setter...
        public RiskySandBox_Team Team;
        public RiskySandBox_Tile start_tile;
        public RiskySandBox_Tile target_tile;
        public int n_troops;
    }



    public static void TRY_captureFromBattleLogEntry(string _battle_log_entry)
    {
        RiskySandBox_Team.EventInfo_Oncapture _EventInfo = new EventInfo_Oncapture();
        _EventInfo.battle_log_string = _battle_log_entry;

        _EventInfo.Team.current_turn_state.value = RiskySandBox_Team.turn_state_capture;//put team into capture state...
        _EventInfo.Team.capture_start_ID.value = _EventInfo.start_tile.ID;//assign capture_start...
        _EventInfo.Team.capture_end_ID.value = _EventInfo.target_tile.ID;//assign capture_end...

        _EventInfo.Team.TRY_capture(_EventInfo.n_troops);
    }


    public static void invokeEvent_Oncapture(string _battle_log_string, bool _alert_MultiplayerBridge)
    {
        EventInfo_Oncapture _EventInfo = new EventInfo_Oncapture();
        _EventInfo.battle_log_string = _battle_log_string;

        if (_alert_MultiplayerBridge)
            RiskySandBox_Team.Oncapture?.Invoke(_EventInfo);

        RiskySandBox_Team.Oncapture?.Invoke(_EventInfo);
    }



    public bool TRY_capture(int _n_troops)
    {
        //get my capture target id....
        if (this.current_turn_state != RiskySandBox_Team.turn_state_capture)
        {
            if (this.debugging)
                GlobalFunctions.print("not in the capture state...", this);
            return false;
        }

        if(_n_troops <= 0)
        {
            GlobalFunctions.print("_n_troops <= 0", this);
            return false;
        }


        RiskySandBox_Tile _start_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(this.capture_start_ID.value);
        RiskySandBox_Tile _target_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(this.capture_end_ID.value);

        if(_start_Tile == null || _target_Tile == null)
        {
            GlobalFunctions.printError("hmm what is happening here??? has the game ended??? and something is not happening correctly...", this);
            return false;
        }

        //TODO - make sure there is atleast <1> troop left on the captureing tile...
        if (_start_Tile.num_troops - _n_troops < RiskySandBox_Tile.min_troops_per_Tile)
        {
            if (debugging)
                GlobalFunctions.print("not enough troops", this);
            return false;
        }

        return capture(_start_Tile, _target_Tile, _n_troops);
    }


    bool capture(RiskySandBox_Tile _start_Tile,RiskySandBox_Tile _target_Tile,int _n_troops)
    {
        _target_Tile.my_Team_ID.value = this.ID;
        _target_Tile.num_troops.value = _n_troops;
        _start_Tile.num_troops.value -= _n_troops;

        //TODO redo this to say n_captures_this_turn += 1...
        this.has_captured_Tile.value = true;


        this.end_turn_time_stamp.value += this.capture_increment.value;//give the time back to the team...
        this.current_turn_state.value = RiskySandBox_Team.turn_state_attack;

        EventInfo_Oncapture _EventInfo = new EventInfo_Oncapture();
        _EventInfo.Team = this;
        _EventInfo.start_tile = _start_Tile;
        _EventInfo.target_tile = _target_Tile;
        _EventInfo.n_troops = _n_troops;

        RiskySandBox_Team.Oncapture_MultiplayerBridge?.Invoke(_EventInfo);
        RiskySandBox_Team.Oncapture?.Invoke(_EventInfo);


        return true;



    }


}