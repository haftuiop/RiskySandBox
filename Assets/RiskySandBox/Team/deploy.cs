using System.Collections;using System.Collections.Generic;using System.Linq;using System;


public partial class RiskySandBox_Team
{
    public static event Action<EventInfo_Ondeploy> Ondeploy_MultiplayerBridge;
    public static event Action<EventInfo_Ondeploy> Ondeploy;

    public struct EventInfo_Ondeploy
    {

        public string battle_log_string
        {
            get { return string.Format("GameEvent_deploy:{0},{1},{2}", this.Team.ID.value, this.Tile.ID.value, this.n_troops); }
            set
            {
                string[] _data = value.Split(":")[1].Split(",").ToArray();
                this.Team = RiskySandBox_Team.GET_RiskySandBox_Team(int.Parse(_data[0]));
                this.Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(int.Parse(_data[1]));
                this.n_troops = int.Parse(_data[2]);
            }
        }

        /// <summary>
        /// what should be displayed on the battle log (when in human mode)
        /// </summary>
        public string human_log_string
        {
            get { return string.Format("Team '{0}' deployed {1} troop(s) to '{2}'", (string)this.Team.team_name, (int)this.n_troops, this.Tile.tile_name.value); }
        }

        /// <summary>
        /// the team who deployed...
        /// </summary>
        public RiskySandBox_Team Team;

        /// <summary>
        /// the tile that was deployed to...
        /// </summary>
        public RiskySandBox_Tile Tile;
        /// <summary>
        /// how many troops were deployed...
        /// </summary>
        public int n_troops;
    }


    public static void invokeEvent_Ondeploy(string _battle_log_string,bool _alert_MultiplayerBridge)
    {
        EventInfo_Ondeploy _EventInfo = new EventInfo_Ondeploy();
        _EventInfo.battle_log_string = _battle_log_string;

        if (_alert_MultiplayerBridge)
            RiskySandBox_Team.Ondeploy_MultiplayerBridge?.Invoke(_EventInfo);

        RiskySandBox_Team.Ondeploy?.Invoke(_EventInfo);
    }

    public static void TRY_deployFromBattleLogEntry(string _battle_log_entry)
    {

    }


    /// <summary>
    /// can this team deploy
    /// </summary>
    public virtual bool canDeploy(RiskySandBox_Tile _Tile, int _n_troops)
    {
        if(_Tile == null)
        {
            GlobalFunctions.printError("_Tile is null?!?!?!", this);
            return false;
        }

        if(_Tile.my_Team != this)
        {
            //if i am allowed to deploy to ally tiles???
            if (this.allow_deploy_to_ally_Tiles == false)
            {
                return false;
            }

            bool _is_ally = RiskySandBox_Team.isAlly(this, _Tile.my_Team);
            if (_is_ally == false)
            {
                return false;
            }
        }

        if(this.current_turn_state != RiskySandBox_Team.turn_state_deploy)
        {
            return false;
        }

        if(this.deployable_troops.value < _n_troops)
        {
            return false;
        }
        //TODO maybe you can only deploy a certain number of troops each turn???

        //TODO - if the tile is my_Team and I am not allowed to deploy to my own tiles???



        return true;

    }


    void deploy(RiskySandBox_Tile _Tile, int _n_troops)
    {
        _Tile.num_troops.value += _n_troops;
        this.deployable_troops.value -= _n_troops;

        EventInfo_Ondeploy _new_EventInfo = new EventInfo_Ondeploy();
        _new_EventInfo.Team = this;
        _new_EventInfo.Tile = _Tile;
        _new_EventInfo.n_troops = _n_troops;

        if(PrototypingAssets.run_server_code.value == true)
            RiskySandBox_Team.Ondeploy_MultiplayerBridge?.Invoke(_new_EventInfo);

        RiskySandBox_Team.Ondeploy?.Invoke(_new_EventInfo);

        if (this.deployable_troops <= 0)//TODO - and auto switch to attack???
        {
            this.TRY_goToNextTurnState();
        }

        
    }

    public virtual bool TRY_deploy(RiskySandBox_Tile _Tile, int _n_troops)
    {

        bool _can_deploy = this.canDeploy(_Tile, _n_troops);

        if (_can_deploy == false)
            return false;

        deploy(_Tile, _n_troops);

        return true;
    }






}
