using System.Collections;using System.Collections.Generic;using System.Linq;using System;

public partial class RiskySandBox_Team
{

    public static event Action<EventInfo_Onattack> Onattack_MultiplayerBridge;
    public static event Action<EventInfo_Onattack> Onattack;


    public struct EventInfo_Onattack
    {
        public string battle_log_string
        {
            get
            {
                int _defending_team_ID = RiskySandBox_Team.null_ID;
                if (this.defending_Team != null)
                    _defending_team_ID = this.defending_Team.ID.value;

                return string.Format("GameEvent_attack:{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", (int)this.attacking_Team.ID, _defending_team_ID, (int)start_Tile.ID, (int)target_Tile.ID, this.n_troops, this.attack_method, this.start_Tile_num_troops_before_attack, this.start_Tile_num_troops_after_attack, this.target_Tile_num_troops_before_attack, this.target_Tile_num_troops_after_attack);
            }
            set
            {
                string[] _data = value.Split(":")[1].Split(",");
                this.attacking_Team = RiskySandBox_Team.GET_RiskySandBox_Team(int.Parse(_data[0]));
                this.defending_Team = RiskySandBox_Team.GET_RiskySandBox_Team(int.Parse(_data[1]));
                this.start_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(int.Parse(_data[2]));
                this.target_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(int.Parse(_data[3]));
                this.n_troops = int.Parse(_data[4]);
                this.attack_method = _data[5];
                this.start_Tile_num_troops_before_attack = int.Parse(_data[6]);
                this.start_Tile_num_troops_after_attack = int.Parse(_data[7]);
                this.target_Tile_num_troops_before_attack = int.Parse(_data[8]);
                this.target_Tile_num_troops_after_attack = int.Parse(_data[9]);
            }

        }

        public string human_log_string
        {
            get
            {
                string _defending_Team_name = "null Team";

                if (this.defending_Team != null)
                    _defending_Team_name = this.defending_Team.team_name.value;

                return string.Format("Team '{0}' attacked 'Team {1}' from ('{2}' -> '{3}')", this.attacking_Team.team_name.value, _defending_Team_name, this.start_Tile.human_ui_log_string, this.target_Tile.human_ui_log_string);
            }
        }


        //TODO - make these a getter private setter...
        public RiskySandBox_Team attacking_Team;
        public RiskySandBox_Team defending_Team;

        public RiskySandBox_Tile start_Tile;
        public RiskySandBox_Tile target_Tile;

        public int n_troops;
        public string attack_method;


        public int start_Tile_num_troops_before_attack;
        public int start_Tile_num_troops_after_attack;
        public int target_Tile_num_troops_before_attack;
        public int target_Tile_num_troops_after_attack;


        public readonly int attacker_deaths { get { return this.start_Tile_num_troops_before_attack - this.start_Tile_num_troops_after_attack; } }
        public int defender_deaths { get { return target_Tile_num_troops_before_attack - target_Tile_num_troops_after_attack; } }
        public bool capture_flag { get { return this.target_Tile_num_troops_after_attack <= 0; } }




    }

    public static void invokeEvent_Onattack(string _battle_log_string,bool _alert_MultiplayerBridge)
    {
        EventInfo_Onattack _new_EventInfo = new EventInfo_Onattack();
        _new_EventInfo.battle_log_string = _battle_log_string;

        if (_alert_MultiplayerBridge)
            RiskySandBox_Team.Onattack_MultiplayerBridge?.Invoke(_new_EventInfo);

        Onattack?.Invoke(_new_EventInfo);
    }


    public static void TRY_attackFromBattleLogEntry(string _battle_log_entry)
    {
        EventInfo_Onattack _EventInfo = new EventInfo_Onattack();
        _EventInfo.battle_log_string = _battle_log_entry;

        _EventInfo.start_Tile.num_troops.value = _EventInfo.start_Tile_num_troops_after_attack;
        _EventInfo.target_Tile.num_troops.value = _EventInfo.target_Tile_num_troops_after_attack;

        if (_EventInfo.target_Tile_num_troops_after_attack <= 0)
        {
            _EventInfo.target_Tile.my_Team_ID.value = _EventInfo.attacking_Team.ID;
        }


        RiskySandBox_Team.Onattack?.Invoke(_EventInfo);
    }


    public bool canAttack(RiskySandBox_Tile _from,RiskySandBox_Tile _to,int _n_troops,string _attack_method,bool _bypass_attack_state_check = false)
    {
        if(_from == null)
        {
            GlobalFunctions.printError("why is _from null...", this);
            return false;
        }

        if(_to == null)
        {
            GlobalFunctions.printError("why is _to null...", this);
            return false;
        }

        if(_from.my_Team != this)
        {
            if(this.debugging)
                GlobalFunctions.print("_from.my_Team != this!?!?!?", this);
            return false;
        }

        if(_to.my_Team == this)//TODO add something like "and this.can_attack_self == false???
        {
            if (this.debugging)
                GlobalFunctions.print("_to.my_Team == this (and this team cant attack itself...", this);
            return false;
        }

        if (_bypass_attack_state_check == false)
        {
            if (this.current_turn_state != RiskySandBox_Team.turn_state_attack)
            {
                if (this.debugging)
                    GlobalFunctions.print("not in attack state... returning false", this);
                return false;
            }
        }

        if(_from.num_troops <= RiskySandBox_Tile.min_troops_per_Tile)
        {
            if (this.debugging)
                GlobalFunctions.print("_from.num_troops <= 0", this);
            return false;
        }

        if(_from.graph_connections.Contains(_to) == false)//TODO - what happens if the team has some magic portal that allows this Team to travel but not others????
        {
            if (this.debugging)
                GlobalFunctions.print("_from.graph_connection.Contains(_to) == false", this);
            return false;
        }

        if (this.allow_attack_ally_Tiles == false)
        {
            bool _is_ally = RiskySandBox_Team.isAlly(this, _from.my_Team);

            if (_is_ally)
            {
                if (this.debugging)
                    GlobalFunctions.print("not allowed to attack ally Tiles", this);
                return false;
            }
        }

        //TODO - add a setting for the max number of attacks per turn???
        return true;

    }

    public bool TRY_attack(RiskySandBox_Tile _start_Tile,RiskySandBox_Tile _target_Tile,int _n_troops,string _attack_method)
    {
        bool _can_attack = this.canAttack(_start_Tile, _target_Tile, _n_troops, _attack_method);
        if(_can_attack == false)
        {
            if (this.debugging)
                GlobalFunctions.print("canAttack returnted false...", this);

            return false;
        }

        //actually do the attack...
        attack(_start_Tile, _target_Tile, _n_troops, _attack_method);

        return true;
    }


    void attack(RiskySandBox_Tile _start_Tile,RiskySandBox_Tile _target_Tile,int _n_troops,string _attack_method)
    {
        //run the battle simulation

        if (_start_Tile.num_troops < _n_troops + RiskySandBox_Tile.min_troops_per_Tile)
        {
            if (this.debugging)
                GlobalFunctions.print("not enough troops!... returning", this);
            return;
        }

        int _attacker_deaths;
        int _defender_deaths;

        if (_target_Tile.has_capital == true)
            RiskySandBox_AttackSimulations.doBattle(_n_troops, _target_Tile.num_troops.value, RiskySandBox_AttackSimulations.capitals_mode_string, out _attacker_deaths, out _defender_deaths);
        else
            RiskySandBox_AttackSimulations.doBattle(_n_troops, _target_Tile.num_troops.value, _attack_method, out _attacker_deaths, out _defender_deaths);



        int _start_Tile_num_troops_before_attack = _start_Tile.num_troops;
        int _target_Tile_num_troops_before_attack = _target_Tile.num_troops;


        _start_Tile.num_troops.value -= _attacker_deaths;
        _target_Tile.num_troops.value -= _defender_deaths;


        EventInfo_Onattack _new_EventInfo = new EventInfo_Onattack();

        _new_EventInfo.attacking_Team = this;
        _new_EventInfo.defending_Team = _target_Tile.my_Team;
        _new_EventInfo.start_Tile = _start_Tile;
        _new_EventInfo.target_Tile = _target_Tile;
        _new_EventInfo.n_troops = _n_troops;
        _new_EventInfo.attack_method = _attack_method;

        _new_EventInfo.start_Tile_num_troops_before_attack = _start_Tile_num_troops_before_attack;
        _new_EventInfo.start_Tile_num_troops_after_attack = _start_Tile.num_troops;

        _new_EventInfo.target_Tile_num_troops_before_attack = _target_Tile_num_troops_before_attack;
        _new_EventInfo.target_Tile_num_troops_after_attack = _target_Tile.num_troops;

        if (PrototypingAssets.run_server_code.value == true)
            Onattack_MultiplayerBridge?.Invoke(_new_EventInfo);

        Onattack?.Invoke(_new_EventInfo);

        
        if (_target_Tile.num_troops <= 0)//if the tile ran out of troops???
        {
            _target_Tile.my_Team_ID.value = this.ID;//capture it
            this.capture_start_ID.value = _start_Tile.ID;
            this.capture_end_ID.value = _target_Tile.ID;
            this.current_turn_state.value = "capture";//go into the capture state...
        }





    }





}
