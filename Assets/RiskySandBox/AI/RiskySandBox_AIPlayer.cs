using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_AIPlayer : MonoBehaviour
{

    [SerializeField] bool debugging;

    [SerializeField] RiskySandBox_Team my_Team;

    [SerializeField] RiskySandBox_HumanPlayer existing_HumanPlayer { get { return RiskySandBox_HumanPlayer.GET_RiskySandBox_HumanPlayer(my_Team); } }

    List<RiskySandBox_Tile> my_Tiles { get { return this.my_Team.my_Tiles; } }


    private void Awake()
    {

    }

    private void OnDestroy()
    {
        
    }

    private void Start()
    {
        InvokeRepeating("logicStep",1f,1f);
    }


    private void logicStep()
    {
        EventReceiver_OnVariableUpdate_current_turn_state(this.my_Team.current_turn_state);
    }

    //my team turn state just changed...
    void EventReceiver_OnVariableUpdate_current_turn_state(ObservableString _current_turn_state)
    {
        if (RiskySandBox_ReplaySystem.is_enabled)
        {
            if (this.debugging)
                GlobalFunctions.print("replay system enabled... reutrning", this);
            return;
        }

        if (PrototypingAssets.run_server_code.value == false)
        {
            GlobalFunctions.printWarning("not the server???... why is this happening...", this);
            return;
        }


        if (this.my_Team.defeated == true)
        {
            //DEBUG wtf?!?!?!
            return;
        }

        if(this.existing_HumanPlayer != null)
        {
            if (this.debugging)
                GlobalFunctions.print("existint player != null...",this);
            return;
        }

        if (_current_turn_state == RiskySandBox_Team.turn_state_waiting)
        {
            return;
        }
        else if(_current_turn_state == RiskySandBox_Team.turn_state_placing_capital)
        {
            doCapitalLogic();
        }
        else if(_current_turn_state == RiskySandBox_Team.turn_state_force_trade_in)
        {
            doTradeLogic();
        }
        else if(_current_turn_state == RiskySandBox_Team.turn_state_deploy)
        {
            doDeployLogic();
        }
        else if(_current_turn_state == RiskySandBox_Team.turn_state_attack)
        {
            doAttackLogic();
        }
        else if(_current_turn_state == RiskySandBox_Team.turn_state_capture)
        {
            doCaptureLogic();
        }
        else if(_current_turn_state == RiskySandBox_Team.turn_state_fortify)
        {
            doFortifyLogic();
        }



    }

    void doCapitalLogic()
    {
        RiskySandBox_Tile _random_Tile = GlobalFunctions.GetRandomItem(my_Tiles);
        my_Team.TRY_placeCapital(_random_Tile);
    }

    void doTradeLogic()
    {
        this.my_Team.autoTrade();
    }

    void doDeployLogic()
    {
        Dictionary<RiskySandBox_Tile, int> _planned_deploys = new Dictionary<RiskySandBox_Tile, int>();

        List<RiskySandBox_Tile> _sensible_deploy_targets = new List<RiskySandBox_Tile>();

        foreach (RiskySandBox_Tile _Tile in this.my_Team.my_Tiles)
        {
            bool _is_sensible = false;
            foreach(RiskySandBox_Tile _connection in _Tile.graph_connections)
            {
                if(this.my_Team.canAttack(_Tile,_connection,1,"",_bypass_attack_state_check:true) == true)//so really you want to deploy troops onto tiles that can attack?
                {
                    _is_sensible = true;
                    break;
                }
            }

            if(_is_sensible)
            {
                _sensible_deploy_targets.Add(_Tile);
            }
        }

        if(_sensible_deploy_targets.Count == 0)
        {
            //ok we also want to think about "guarding" our bonus... we also would prefer to put troops onto portals... especially unstable ones... because these are very likely to become useful
            if(this.my_Tiles.Count > 0)
            {
                _sensible_deploy_targets.AddRange(this.my_Tiles);
            }
        }

        //if we are at the "start" of the game pla




        for(int i = 0; i < my_Team.deployable_troops; i += 1)
        {
            RiskySandBox_Tile _random_choice = GlobalFunctions.GetRandomItem(_sensible_deploy_targets);
            if(_planned_deploys.TryAdd(_random_choice,1) == false)
            {
                _planned_deploys[_random_choice] += 1;
            }
        }

        foreach (KeyValuePair<RiskySandBox_Tile, int> _KVP in _planned_deploys)
        {
            this.my_Team.TRY_deploy(_KVP.Key, _KVP.Value);
        }
    }

    public void doAttackLogic()
    {
        if (this.debugging)
            GlobalFunctions.print("doing attack code!", this);


        int _max_attacks = 3;
        float _attack_chance = 0.5f;
        //TODO - do a simple attack?
        //TODO - NOTE if we do a attack and we capture a tile we must "capture" the tile
        foreach (RiskySandBox_Tile _Tile in my_Team.my_Tiles)
        {
            int _rng = GlobalFunctions.randomInt(1, 100);

            if (_rng < _attack_chance * 100)
                continue;


            foreach (RiskySandBox_Tile _connection in _Tile.graph_connections)
            {
                if (my_Team.canAttack(_Tile, _connection, 1, "", _bypass_attack_state_check: true))//TODO - magic 1 and ""
                {
                    my_Team.TRY_attack(_Tile, _connection, _Tile.num_troops - RiskySandBox_Tile.min_troops_per_Tile, "");
                    break;
                }
            }
        }

        my_Team.TRY_goToNextTurnState();
    }

    public void doCaptureLogic()
    {
        int _n_troops = my_Team.capture_start.num_troops - RiskySandBox_Tile.min_troops_per_Tile;
        
        my_Team.TRY_capture(_n_troops);
    }

    public void doFortifyLogic()
    {
        if (this.debugging)
            GlobalFunctions.print("doing fortify code!", this);

        List<RiskySandBox_Tile> _fortify_from_candidates = this.GET_fortifyFromCandidates();

        if (_fortify_from_candidates.Count > 0)
        {
            List<RiskySandBox_Tile> _fortify_target_candidates = this.GET_fortifyTargetCandidates();

            foreach (RiskySandBox_Tile _start in _fortify_from_candidates)
            {
                foreach (RiskySandBox_Tile _end in _fortify_target_candidates)
                {
                    if (this.my_Team.canFortify(_start, _end, 1))
                    {

                        this.my_Team.TRY_fortify(_start, _end, _start.num_troops - RiskySandBox_Tile.min_troops_per_Tile);
                        break;
                    }
                }
            }

        }





        //TODO - foritfy troops out of the "corners"
        //TODO - fortify in such a way as to protect captured bonuses
        //try not to move troops off of a capital???
        this.my_Team.endTurn("simple ai...");
    }


    public List<RiskySandBox_Tile> GET_fortifyFromCandidates()
    {
        //ok so really the tiles that are not able to attack is probably a VERY good candidate to fortify from...
        List<RiskySandBox_Tile> _inactive_Tiles = new List<RiskySandBox_Tile>();
        foreach (RiskySandBox_Tile _Tile in this.my_Tiles)
        {
            if (_Tile.num_troops <= RiskySandBox_Tile.min_troops_per_Tile)
                continue;

            bool _can_attack = false;
            foreach (RiskySandBox_Tile _connection in _Tile.graph_connections)
            {
                if (this.my_Team.canAttack(_Tile, _connection, 1, "", _bypass_attack_state_check: true) == true)
                {
                    _can_attack = true;
                    break;
                }

            }

            if (_can_attack == false)
                _inactive_Tiles.Add(_Tile);

        }
        //make sure to push more inactive troops... e.g. if you have 1 tile with 10 troops and 1 with 5 troops... you are probably* better off moving the 10...
        return _inactive_Tiles.OrderByDescending(x => x.num_troops.value).ToList();
    }

    public List<RiskySandBox_Tile> GET_fortifyTargetCandidates()
    {
        //ok so these are all of my tiles...
        //we want to prioritise the tiles that can attack something...
        List<RiskySandBox_Tile> _return_value = new List<RiskySandBox_Tile>();
        foreach (RiskySandBox_Tile _Tile in this.my_Tiles)
        {
            bool _can_attack = false;
            foreach (RiskySandBox_Tile _connection in _Tile.graph_connections)
            {
                if (this.my_Team.canAttack(_Tile, _connection, 1, "", _bypass_attack_state_check: true) == true)
                {
                    _can_attack = true;
                    break;
                }
            }

            if (_can_attack == true)
                _return_value.Add(_Tile);
        }

        return _return_value;
    }



}
