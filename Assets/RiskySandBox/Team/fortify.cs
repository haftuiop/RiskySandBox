using System.Collections;using System.Collections.Generic;using System.Linq;using System;

public partial class RiskySandBox_Team
{

    public static event Action<EventInfo_Onfortify> Onfortify_MultiplayerBridge;
    public static event Action<EventInfo_Onfortify> Onfortify;


    //TODO - put in the route somehow e.g. list or array???
    public struct EventInfo_Onfortify
    {
        public string battle_log_string
        {
            get
            {
                //TODO - we want to put the route into this (for now the route is represented as '?')
                return string.Format("GameEvent_fortify:{0},{1},{2},{3},?,?,?,?", this.Team.ID, this.from.ID, this.to.ID, this.n_troops);
            }

            set
            {
                string[] _data = value.Split(":")[1].Split(",");

                this.Team = RiskySandBox_Team.GET_RiskySandBox_Team(int.Parse(_data[0]));
                this.from = RiskySandBox_Tile.GET_RiskySandBox_Tile(int.Parse(_data[1]));
                this.to = RiskySandBox_Tile.GET_RiskySandBox_Tile(int.Parse(_data[2]));
                this.n_troops = int.Parse(_data[3]);
                //TODO route...

            }
        }

        public string human_log_string
        {
            get
            {
                return string.Format("Team '{0}' fortified {1} troop(s) to {2}", this.Team.team_name.value, (int)this.n_troops, this.to.tile_name.value);
            }
        }

        //TODO - make these a getter private setter...
        public RiskySandBox_Team Team;
        public RiskySandBox_Tile from;
        public RiskySandBox_Tile to;
        public int n_troops;

    }

    public static void invokeEvent_Onfortify(string _battle_log_string,bool _alert_MultiplayerBridge)
    {
        EventInfo_Onfortify _EventInfo = new EventInfo_Onfortify();
        _EventInfo.battle_log_string = _battle_log_string;

        if (_alert_MultiplayerBridge)
            RiskySandBox_Team.Onfortify_MultiplayerBridge?.Invoke(_EventInfo);

        RiskySandBox_Team.Onfortify?.Invoke(_EventInfo);
    }

    public static void TRY_fortifyFromBattleLogEntry(string _battle_log_entry)
    {
        //extract out from,to,num_troops!
        EventInfo_Onfortify _EventInfo = new EventInfo_Onfortify();
        _EventInfo.battle_log_string = _battle_log_entry;

        _EventInfo.Team.fortify(_EventInfo.from, _EventInfo.to, _EventInfo.n_troops);
    }


    public bool canFortify(RiskySandBox_Tile _from,RiskySandBox_Tile _to, int _n_troops)
    {
        if(_from == null)
        {
            GlobalFunctions.printError("_from == null", this);
            return false;
        }

        if(_to == null)
        {
            GlobalFunctions.printError("_to == null...", this);
            return false;
        }

        if (_from.my_Team != this)
        {
            if(this.debugging)
                GlobalFunctions.print("_from.my_Team != this",this);
            return false;
        }

        if(this.current_turn_state != RiskySandBox_Team.turn_state_fortify)
        {
            if (this.debugging)
                GlobalFunctions.print("not in the fortify state...", this);
            return false;
        }

        if (_to.my_Team != this)
        {

            if (this.allow_fortify_to_ally_Tiles == false)
            {
                return false;
            }
            
            //if we can fortify to ally tiles???
            bool _is_ally = RiskySandBox_Team.isAlly(this, _to.my_Team);
            if (_is_ally == false)
                return false;
            
        }

        if(_from.num_troops <= _n_troops)//TODO - think about min troops per tile...
        {
            if (debugging)
                GlobalFunctions.print("unable to fortify as _from.num_troops <= _n_troops", this);
            return false;
        }


        List<RiskySandBox_Tile> _path = RiskySandBox_MainGame.findPath(_from, _to);

        if (_path == null || _path.Count == 0)
        {
            if (debugging)
                GlobalFunctions.print("unable to find a route from " + _from + " _to " + _to + " returning", this);
            return false;
        }


        return true;
    }


    public bool TRY_fortify(RiskySandBox_Tile _from,RiskySandBox_Tile _to,int _n_troops)
    {
        bool _can_fortify = this.canFortify(_from, _to, _n_troops);

        if(_can_fortify == false)
        {
            if (this.debugging)
                GlobalFunctions.print("canFortify returned false...", this);
            return false;
        }

        return this.fortify(_from, _to, _n_troops);
    }


    bool fortify(RiskySandBox_Tile _from,RiskySandBox_Tile _to,int _n_troops)
    {
        _from.num_troops.value -= _n_troops;
        _to.num_troops.value += _n_troops;

        this.remaining_n_fortifies_this_turn.value -= 1;

        EventInfo_Onfortify _new_EventInfo = new EventInfo_Onfortify();
        _new_EventInfo.Team = this;
        _new_EventInfo.from = _from;
        _new_EventInfo.to = _to;
        _new_EventInfo.n_troops = _n_troops;

        if (PrototypingAssets.run_server_code.value == true)
            Onfortify_MultiplayerBridge?.Invoke(_new_EventInfo);
        Onfortify?.Invoke(_new_EventInfo);


        if(this.remaining_n_fortifies_this_turn.value <= 0)
        {
            this.endTurn("fortify condition");
        }

        

        //TODO - end my turn???
        //increase n_fortifues...
        //if n_fortifies >= max_n_fortifies_per_turn...
        //go to next state... (probs end turn???)
        return true;
    }

}
