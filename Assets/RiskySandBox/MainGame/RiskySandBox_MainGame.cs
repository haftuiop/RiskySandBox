using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{

    public static RiskySandBox_MainGame instance;

    


    public static event Action<ObservableInt> OnVariableUpdate_turn_number;

    public static ObservableInt turn_number { get { return instance.PRIVATE_turn_number; } }

    public ObservableString map_ID { get { return this.PRIVATE_map_ID; } }
    public static ObservableBool game_started { get { return instance.PRIVATE_game_started; } }
    public static ObservableInt n_Teams { get { return instance.PRIVATE_n_Teams; } }





    public ObservableInt num_wildcards { get { return this.PRIVATE_num_wildcards; } }


    

    public ObservableInt n_stable_portals { get { return this.PRIVATE_n_stable_portals;} }
    public ObservableInt n_unstable_portals { get { return this.PRIVATE_n_unstable_portals; } }
    public ObservableInt n_blizards { get { return this.PRIVATE_n_blizards; } }

    public ObservableBool display_bonuses { get { return this.PRIVATE_display_bonuses; } }




    
    public ObservableBool show_escape_menu { get { return this.PRIVATE_show_escape_menu; } }

    List<RiskySandBox_Team> turn_order { get { return RiskySandBox_Team.all_instances.Where(x => x != null && x.defeated.value == false).OrderByDescending(x => x.ID.value).Reverse().ToList(); } }

    public ObservableBool veto_reconnections { get { return RiskySandBox_MainGame.instance.PRIVATE_veto_reconnections; } }



    






    void TeamEventReceiver_OnendTurn(RiskySandBox_Team _Team)
    {
        if (PrototypingAssets.run_server_code == false)
            return;


        bool _last_Team = _Team.ID.value == RiskySandBox_Team.all_instances.Max(x => x.ID.value);

        if (_last_Team)
        {
            RiskySandBox_MainGame.turn_number.value += 1;
            RiskySandBox_ZombiesManager.instance.EventReceiver_OnstartNewRound();
        }


        RiskySandBox_Team _next_Team = GET_nextTeam(_Team);
        if (_next_Team != null)
        {
            if (this.debugging)
                GlobalFunctions.print("just detected an end turn from team with id = " + _Team.ID + " telling the next team to start their turn... (team with id = " + _next_Team.ID.value + ")",this);

            _next_Team.startTurn();
        }




    }


    public void drawTerritoryCard(RiskySandBox_Team _Team, int _n)
    {

        if(PrototypingAssets.run_server_code.value == false)
        {
            GlobalFunctions.printWarning("not the server???... why is this happening???", this);
            return;
        }
        
        //give them a card! (if any)
        List<int> _availible_cards = RiskySandBox_Tile.all_instances.Select(x => x.ID.value).ToList();


        int _n_wilds = RiskySandBox_MainGame.instance.num_wildcards;

        foreach (RiskySandBox_Team _Team1 in RiskySandBox_Team.all_instances)
        {
            if (_Team == null)
            {
                //TODO - WTF?!?!?!? - really this isnt my problem so maybe lets just not worry?
                continue;
            }

            _n_wilds -= _Team1.territory_card_IDs.Where(x => x == RiskySandBox_TerritoryCard.wildcard_ID).Count();

            foreach (int _ID in _Team1.territory_card_IDs)
            {
                //remove from availbile cards
                if (_ID == RiskySandBox_TerritoryCard.wildcard_ID)
                    continue;
                _availible_cards.Remove(_ID);
            }
        }


        if (_n_wilds > 0 || _availible_cards.Count() > 0)
        {
            int _random_card = RiskySandBox_TerritoryCard.drawRandomCard(_availible_cards, _n_wilds);

            _Team.territory_card_IDs.Add(_random_card);
        }

        
    }



    public RiskySandBox_Team GET_nextTeam(RiskySandBox_Team _current_Team)
    {

        List<RiskySandBox_Team> _turn_order = new List<RiskySandBox_Team>(turn_order);
        if (_current_Team == null)
            return _turn_order[0];


        int _current_index = _turn_order.IndexOf(_current_Team);

        _current_index += 1;

        if (_current_index >= _turn_order.Count)
            _current_index = 0;

        return _turn_order[_current_index];


    }




}



