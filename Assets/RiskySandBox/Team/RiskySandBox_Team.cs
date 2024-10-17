using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team
{
    public static int null_ID = -1;
    public static ObservableList<RiskySandBox_Team> all_instances = new ObservableList<RiskySandBox_Team>();
    public static List<RiskySandBox_Team> undefeated_Teams { get { return all_instances.Where(x => x.defeated == false).ToList(); } }


    //TODO - rename to _onvariableupdate_defeated_true_STATIC;

    public static event Action<RiskySandBox_Team> OnVariableUpdate_defeated_STATIC;

    /// <summary>
    /// called whenever a Team.deployable_troops.value changes....
    /// </summary>
    public static event Action<RiskySandBox_Team> OnVariableUpdate_deployable_troops_STATIC;

    public ObservableString current_turn_state { get { return PRIVATE_current_turn_state; } }

    public ObservableInt deployable_troops { get { return PRIVATE_deployable_troops; } }


    /// <summary>
    /// should be invoked whenever a Teams "ID" changes...
    /// </summary>
    public static event Action<RiskySandBox_Team> OnVariableUpdate_ID_STATIC;

    public static event Action<RiskySandBox_Team> OnVariableUpdate_killer_ID_STATIC;

    public static event Action<RiskySandBox_Team> OnVariableUpdate_assassin_target_ID_STATIC;

    public static event Action<RiskySandBox_Team> OnVariableUpdate_is_my_turn_STATIC;

    public static event Action<RiskySandBox_Team> OnUpdate_territory_card_IDs_STATIC;

    public static event Action<RiskySandBox_Team> OnVariableUpdate_purchased_shop_items_STATIC;



    /// <summary>
    /// invoked whenever a Team.current_turn_state.value changes...
    /// </summary>
    public static event Action<RiskySandBox_Team> OnVariableUpdate_current_turn_state_STATIC;



    /// <summary>
    /// invoked whenever a parameter (required for calculating card trade in rewards) changes
    /// </summary>
    public static event Action<RiskySandBox_Team> OnTerritoryCardTradeInParameterChange_STATIC;
    
    public static event Action<RiskySandBox_Team,string,int> OnterritoryCardTradeIn;


    /// <summary>
    /// invoked whenever a variable (related to the shop) changes e.g. purchased_items, item_prices shop_credits
    /// </summary>
    public static event Action<RiskySandBox_Team> OnShopVariableChange_STATIC;


    /// <summary>
    /// called whenever a RiskySandBox_Team.capture_end_ID value changes...
    /// </summary>
    public static event Action<RiskySandBox_Team> OnVariableUpdate_capture_end_ID_STATIC;

    public static readonly string turn_state_deploy = "deploy";
    public static readonly string turn_state_attack = "attack";
    public static readonly string turn_state_fortify = "fortify";
    public static readonly string turn_state_capture = "capture";
    public static readonly string turn_state_waiting = "waiting";
    public static readonly string turn_state_placing_capital = "capital placement";
    public static readonly string turn_state_force_trade_in = "force trade in";
    //TODO - other extra turn states that other people want... maybe we dump this into another file?
    //this is more than enough for now!


    public ObservableInt shop_credits { get { return this.PRIVATE_shop_credits; } }
    public ObservableStringList purchased_shop_items { get { return this.PRIVATE_purchased_shop_items; } }
    public List<string> purchased_shop_item_types { get { return new List<string>(new HashSet<string>(this.purchased_shop_items)); } }
    public ObservableStringList shop_item_types { get { return this.PRIVATE_shop_item_types; } }
    public ObservableIntList shop_item_prices { get { return this.PRIVATE_shop_item_prices; } }

    public ObservableBool is_human { get { return this.PRIVATE_is_human; } }

    /// <summary>
    /// NOTE - 0 = winner, 1 = 2nd place, 2 = 3rd place... there might be multiple winner(s) e.g. in a team game...
    /// </summary>
    public ObservableInt game_over_placement { get { return this.PRIVATE_game_over_placement; } }


    public ObservableBool random_capital_placement { get { return this.PRIVATE_random_capital_placement; } }



    

    public ObservableFloat end_turn_time_stamp { get { return this.PRIVATE_end_turn_time_stamp; } }
    

    public ObservableBool is_my_turn { get { return this.PRIVATE_is_my_turn; } }
    
    public GameObject my_UI { get { return PRIVATE_my_UI; } }


    [SerializeField] List<Material> Team_Materials = new List<Material>();
    [SerializeField] List<Material> Bonus_Materials = new List<Material>();
    [SerializeField] List<Color> text_Colors = new List<Color>();




    public Material bonus_border_Material { get { return this.my_Material; } }
    public Material my_Material { get { return this.Team_Materials[this.ID.value]; } }

    public Material my_Bonus_Material { get { return this.Bonus_Materials[this.ID.value]; } }

    public Color my_Color { get { return Team_Materials.Select(x => x.color).ToList()[this.ID.value]; } }

    public Color text_Color { get { return text_Colors[this.ID.value]; } }



    /// <summary>
    /// how many territory cards does the team have... (this.territory_card_IDs.Count()...)
    /// </summary>
    public int num_cards { get { return this.territory_card_IDs.Count(); } }


    public ObservableInt ID {get { return PRIVATE_ID; }}
    public ObservableBool defeated { get { return PRIVATE_defeated; } }
    /// <summary>
    /// defeated_index = 0 means I was defeated first, 1 = defeated 2nd and so on...
    /// </summary>
    public ObservableInt defeated_index { get { return this.PRIVATE_defeated_index; } }

    public RiskySandBox_Tile capture_start { get { return RiskySandBox_Tile.GET_RiskySandBox_Tile(capture_start_ID); } }
    public RiskySandBox_Tile capture_target { get { return RiskySandBox_Tile.GET_RiskySandBox_Tile(capture_end_ID); } }

    public ObservableInt capture_start_ID { get { return this.PRIVATE_capture_start_ID; } }
    public ObservableInt capture_end_ID { get { return this.PRIVATE_capture_end_ID; } }

    public ObservableInt n_Tiles { get { return PRIVATE_n_Tiles; } }
    public ObservableInt n_capitals { get { return PRIVATE_n_capitals; } }
    public int n_troops { get { return this.my_Tiles.Sum(x => x.num_troops); } }//TODO + deployable troops????
    
    public ObservableInt required_capital_placements { get { return this.PRIVATE_required_capital_placements; } }


    public ObservableInt assassin_target_ID { get { return this.PRIVATE_assassin_target_ID; } }
    public RiskySandBox_Team assassin_target
    {
        get
        {
            if (this.assassin_target_ID.value == null_ID)
                return null;
            return RiskySandBox_Team.GET_RiskySandBox_Team(assassin_target_ID);
        }
    }

    public ObservableBool show_assassin_target_indicator { get { return this.PRIVATE_show_assassin_target_indicator; } }

    //TODO - edit the summary of this (its not great)
    /// <summary>
    /// the team who killed this Team (took the last terrotory? )
    /// </summary>
    public ObservableInt killer_ID { get { return this.PRIVATE_killer_ID; } }


    public List<RiskySandBox_Tile> my_Tiles { get { return RiskySandBox_Tile.all_instances.Where(x => x.my_Team_ID.value == this.ID.value).ToList(); } }

    public ObservableIntList territory_card_IDs{get { return PRIVATE_territory_card_IDs; }}

    /// <summary>
    /// has the team "caputred" a Tile on the turn (required for giving "territory cards")
    /// </summary>
    public ObservableBool has_captured_Tile { get { return this.PRIVATE_has_captured_Tile; } }

    //so here we can say 9,5,4,8  (this would mean all the Team would get a tile with 9, 5, 4, 8 troops) (assuming they have 
    public ObservableString troop_distribution_startGame { get { return this.PRIVATE_troop_distribution_startGame; } }
    [SerializeField] ObservableString PRIVATE_troop_distribution_startGame;


    public List<string> default_names = new List<string>();

    public ObservableFloat turn_length_seconds { get { return this.PRIVATE_turn_length_seconds; } }
    public ObservableFloat capture_increment { get { return this.PRIVATE_capture_increment; } }

    /// <summary>
    /// what value is displayed on the ui/to humans...
    /// </summary>
    public ObservableString team_name { get { return PRIVATE_team_name; } }

    public ObservableFloat world_domination_percentage { get { return this.PRIVATE_world_domination_percentage; } }

    public ObservableBool allow_deploy_to_ally_Tiles { get { return this.PRIVATE_allow_deploy_to_ally_Tiles; } }
    public ObservableBool allow_attack_ally_Tiles { get { return this.PRIVATE_allow_attack_ally_Tiles; } }
    public ObservableBool allow_fortify_to_ally_Tiles { get { return this.PRIVATE_allow_fortify_to_ally_Tiles; } }
    public ObservableBool allow_move_through_ally_Tiles { get { return this.PRIVATE_allow_move_through_ally_Tiles; } }


    //capital settings
    public ObservableInt num_capitals_startGame { get { return this.PRIVATE_num_capitals_startGame; } }
    public ObservableFloat capital_conquest_percentage { get { return this.PRIVATE_capital_conquest_percentage; } }
    public ObservableInt capital_troop_generation { get { return this.PRIVATE_capital_troop_generation; } }


    public ObservableString alliance_string { get { return this.PRIVATE_alliance_string; } }

    public List<RiskySandBox_Team> ally_Teams
    {
        get
        {
            return this.alliance_string.value.Split(",").Where(x => int.TryParse(x, out int _t) == true).Select(x => RiskySandBox_Team.GET_RiskySandBox_Team(int.Parse(x))).Where(x => x != null && x != this).ToList();
        }
    }    

    //territory cards

    public ObservableInt max_n_territory_cards { get { return this.PRIVATE_max_n_territory_cards; } }

    /// <summary>
    /// how many cards does the team get if they "capture" a territory
    /// </summary>
    public ObservableInt n_territory_cards_from_capture { get { return this.PRIVATE_n_territory_cards_from_capture; } }


    //Progressive trade settings...
    public ObservableString progressive_tradein_start_string { get { return this.my_ProgressiveCardSettings.progressive_tradein_start_string; } }
    public ObservableInt progressive_tradein_increment { get { return this.my_ProgressiveCardSettings.progressive_tradein_increment; } }
    public ObservableInt n_progressive_card_trade_in { get { return this.my_ProgressiveCardSettings.n_progressive_card_trade_in; } }
    public ObservableBool shared_progressive_mode { get { return this.my_ProgressiveCardSettings.shared_progressive_mode; } }
    public ObservableBool allow_progressive_tradein { get { return this.my_ProgressiveCardSettings.allow_progressive_tradein; } }



    //Fixed trade settings...
    public ObservableInt fixed_card_tradein_3_person { get { return this.my_FixedCardSettings.infanty_tradein_reward; } }
    public ObservableInt fixed_card_tradein_3_horse { get { return this.my_FixedCardSettings.cavalry_tradein_reward; } }
    public ObservableInt fixed_card_tradein_3_artillary { get { return this.my_FixedCardSettings.artillary_tradein_reward ; } }
    public ObservableInt fixed_card_tradein_all_3 { get { return this.my_FixedCardSettings.all_3_tradein_reward; } }
    public ObservableBool allow_fixed_tradein { get { return this.my_FixedCardSettings.allow_fixed_tradein; } }


    //Geometric trade settings...
    public ObservableInt geometric_tradein_a0 { get { return this.my_GeometricCardSettings.geometric_tradein_a0; } }
    public ObservableFloat geometric_tradein_r { get { return this.my_GeometricCardSettings.geometric_tradein_r; } }
    public ObservableBool allow_geometric_tradein { get { return this.my_GeometricCardSettings.allow_geometric_tradein; } }
    public ObservableBool shared_geometric_tradein { get { return this.my_GeometricCardSettings.shared_geometric_mode; } }
    public ObservableInt n_geometric_tradein { get { return this.my_GeometricCardSettings.n_geometric_tradein; } }

    public ObservableBool enable_FOW { get { return this.PRIVATE_enable_FOW; } }
    public ObservableBool share_FOW_with_allies { get { return this.PRIVATE_share_FOW_with_allies; } }

    public ObservableBool shared_victory { get { return this.PRIVATE_shared_victory; } }


    public ObservableInt remaining_n_fortifies_this_turn { get { return this.PRIVATE_remaining_n_fortifies_this_turn; } } 


    public static RiskySandBox_Team GET_RiskySandBox_Team(int _ID)
    {
        foreach(RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
        {
            if (_Team.ID.value == _ID)
                return _Team;
        }
        return null;
    }





    public bool hasVision(RiskySandBox_Tile _query_Tile)
    {
        //TODO - what if the tile is temporarily blocked from vision e.g. with a shop powerup????

        if (this.enable_FOW == false)//if this team doesnt use FOW...
            return true;

        if (_query_Tile.my_Team_ID == this.ID)//if the query tile belongs to this team???
            return true;


        foreach (RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)//go through each team
        {
            if(_Team != this && RiskySandBox_Team.isAlly(this,_Team))// if its an ally????
            {
                if (_Team.share_FOW_with_allies == true && _Team.enable_FOW == false)//if the team shares fow and it doesnt have fow enabled...
                    return true;
            }
        }




        RiskySandBox_Team _query_Tile_Team = _query_Tile.my_Team;

        if (_query_Tile_Team != null)
        {
            if (_query_Tile_Team.share_FOW_with_allies && RiskySandBox_Team.isAlly(this, _query_Tile_Team))
                return true;
        }


        foreach(RiskySandBox_Tile _Tile in RiskySandBox_Tile.all_instances)
        {
            RiskySandBox_Team _Team = _Tile.my_Team;

            if (_Team == null)
                continue;

            if (_Team == this)
            {
                if (_Tile.graph_connections.Contains(_query_Tile))
                    return true;
            }

            
            if (_Team.share_FOW_with_allies && RiskySandBox_Team.isAlly(this, _Team))
            {
                if (_Tile.graph_connections.Contains(_query_Tile))
                    return true;
            }
            


            
        }

        return false;
    }


    void EventReceiver_OnVariableUpdate_current_time(ObservableFloat _current_time)
    {
        if (this.PRIVATE_is_my_turn.value == false)
            return;

        float _remaining_turn_length = end_turn_time_stamp - RiskySandBox.current_time;

        if (_remaining_turn_length <= 0 && PrototypingAssets.run_server_code.value == true)
        {
            this.endTurn("ran out of time...");
        }
    }



    void RiskySandBox_TileEventReceiver_OnSET_my_Team_STATIC(RiskySandBox_Tile _Tile)
    {
        if (_Tile.previous_my_Team == this)
        {
            this.n_Tiles.value -= 1;

            if (this.n_Tiles <= 0)
            {
                //i have been defeated... as i have no tiles left...

                this.defeated.value = true;
            }

        }


        if (_Tile.my_Team == this)
            this.n_Tiles.value += 1;
    }


  


    public static bool isAlly(RiskySandBox_Team _Team1,RiskySandBox_Team _Team2)
    {

        if(_Team1 == null || _Team2 == null)
        {
            GlobalFunctions.printWarning("try not to do this... it can lead to weird results??? returning false",null);
            return false;
        }


        if (_Team1.PRIVATE_alliance_string == "")
        {
            if (_Team1.debugging)
                GlobalFunctions.print("returning false (alliance_string is empty string",_Team1.PRIVATE_alliance_string);
            return false;
        }        

        //TODO - try parse since the human users may accidently type a bad valu (doesnt contain , or is wrong like 2,,3,4,5,6 (etc)
        foreach(int _ID in _Team1.PRIVATE_alliance_string.value.Split(',').Select(x => int.Parse(x)))
        {
            if (_ID == _Team2.ID.value)
                return true;
        }
        return false;
    }

    public void createAlliance(RiskySandBox_Team _other)
    {
        GlobalFunctions.printWarning("unimplemented! (on the back end)",null);


        //TODO - check this is not already the case?
        //TODO - we wish to edit the RiskySandBox_AllianceSettings.alliance_string to include this.id,_other.id ( and then add in any required |

    }

    public void breakAlliance(RiskySandBox_Team _other)
    {
        GlobalFunctions.printWarning("unimplemented! - on the backend",null);
        //TODO - we want to edit the RiskySandBox_AllianceSettings.alliance_string to remove this.id,_other.id
    }

    public int calculateProgressiveTradeIn(int _trade_number)
    {

        if (this.progressive_tradein_start_string.value == "")
        {
            return this.progressive_tradein_increment * (_trade_number + 1);
        }


        //so first look at the string!
        List<int> _start_values = this.progressive_tradein_start_string.value.Split(',').Select(x => int.Parse(x)).ToList();

        if (_start_values.Count == 0)
        {
            return progressive_tradein_increment * (_trade_number + 1);
        }

        if (_trade_number >= _start_values.Count)
        {
            return _start_values[_start_values.Count - 1] + progressive_tradein_increment * (_trade_number - _start_values.Count() + 1);
        }

        return _start_values[_trade_number];
    }

    public int calculateGeometricTradeIn(int _trade_number)
    {
        return Mathf.RoundToInt(this.geometric_tradein_a0 * Mathf.Pow(this.geometric_tradein_r, _trade_number));
    }




    public int calculateFixedTradeIn(IEnumerable<int> _territory_card_IDs)
    {

        List<string> _card_types = RiskySandBox_TerritoryCard.GET_card_types(_territory_card_IDs);

        int _n_infanty = _card_types.Where(x => x == RiskySandBox_TerritoryCard.card_type_infantry).Count();
        int _n_cavalry = _card_types.Where(x => x == RiskySandBox_TerritoryCard.card_type_cavalry).Count();
        int _n_artillary = _card_types.Where(x => x == RiskySandBox_TerritoryCard.card_type_artillary).Count();
        int _n_wild = _card_types.Where(x => x == RiskySandBox_TerritoryCard.card_type_wild).Count();


        if (_n_infanty == 3)
            return this.fixed_card_tradein_3_person;
        if (_n_cavalry == 3)
            return this.fixed_card_tradein_3_horse;
        if (_n_artillary == 3)
            return this.fixed_card_tradein_3_artillary;

        if (_n_infanty == 1 && _n_cavalry == 1 && _n_artillary == 1)
            return this.fixed_card_tradein_all_3;

        if(_n_wild == 2 || _n_wild == 3)
        {
            int _max = this.fixed_card_tradein_3_person;

            if (this.fixed_card_tradein_3_horse > _max)
                _max = this.fixed_card_tradein_3_horse;

            if (this.fixed_card_tradein_3_artillary > _max)
                _max = this.fixed_card_tradein_3_artillary;

            if (this.fixed_card_tradein_all_3 > _max)
                _max = this.fixed_card_tradein_all_3;

            return _max;

        }

        if(_n_wild == 1)
        {
            if (_n_infanty == 2)
                return this.fixed_card_tradein_3_person;

            if (_n_cavalry == 2)
                return this.fixed_card_tradein_3_horse;

            if (_n_artillary == 2)
                return this.fixed_card_tradein_3_artillary;

            return this.fixed_card_tradein_all_3;
        }
        GlobalFunctions.printError("how did we get here?!?!?!?", null);

        return -999;
    }


    public bool isValidTrade(List<int> _territory_card_IDs,string _trade_mode)
    {
        if(_trade_mode == RiskySandBox_TerritoryCard.fixed_mode && this.allow_fixed_tradein == false)
        {
            if (this.debugging)
                GlobalFunctions.print("fixed trade in is not allowed for this team... returning false",this);
            return false;
        }

        if(_trade_mode == RiskySandBox_TerritoryCard.progressive_mode && this.allow_progressive_tradein == false)
        {
            if (this.debugging)
                GlobalFunctions.print("progressive trade in is not allowed for this team... returning false...",this);
            return false;
        }

        if(_trade_mode == RiskySandBox_TerritoryCard.geometric_mode && this.allow_geometric_tradein == false)
        {
            if (this.debugging)
                GlobalFunctions.print("geometric trade in is not allowed for this team... returning false...",this);
            return false;
        }


        List<string> _card_types = RiskySandBox_TerritoryCard.GET_card_types(_territory_card_IDs);

        int _n_infanty = _card_types.Where(x => x == RiskySandBox_TerritoryCard.card_type_infantry).Count();
        int _n_cavalry = _card_types.Where(x => x == RiskySandBox_TerritoryCard.card_type_cavalry).Count();
        int _n_artillary = _card_types.Where(x => x == RiskySandBox_TerritoryCard.card_type_artillary).Count();
        int _n_wild = _card_types.Where(x => x == RiskySandBox_TerritoryCard.card_type_wild).Count();

        HashSet<string> _distinct_card_types = new HashSet<string>(_card_types);

        if (_trade_mode == RiskySandBox_TerritoryCard.fixed_mode || _trade_mode == RiskySandBox_TerritoryCard.progressive_mode || _trade_mode == RiskySandBox_TerritoryCard.geometric_mode)
        {
            //make sure there are 3 cards!
            if (_territory_card_IDs.Count != 3)
                return false;


            if (_n_wild == 0)
            {
                if (this.debugging)
                    GlobalFunctions.print("n wilds == 0 _distinct_card_types.Count() == " + _distinct_card_types.Count(), this);
                //all are the same type... or they are all different types...
                return _distinct_card_types.Count() == 1 || _distinct_card_types.Count() == 3;
            }
            if (_n_wild == 1)
            {
                if (this.debugging)
                    GlobalFunctions.print("n wilds == 1 _distinct_card_types.Count() == " + _distinct_card_types.Count(), this);
                //if there is only 1 type? or there are 2 types?
                return _distinct_card_types.Count() == 1 || _distinct_card_types.Count() == 2;
            }
            if (_n_wild == 2)
            {
                if (this.debugging)
                    GlobalFunctions.print("n wilds == 2 _distinct_card_types.Count() == " + _distinct_card_types.Count(), this);
                return _distinct_card_types.Count() == 1;
            }

            if (_n_wild == 3)
            {
                if (this.debugging)
                    GlobalFunctions.print("n wilds == 3", this);
                return true;
            }

            GlobalFunctions.printError("WTF?!?!??!?!", this);
            return false;





        }
        //presumably you (the person reading this) is trying to implement your own "territory card" algoriythm please read the below instructions...
        //TODO - put in some detailed instructions here to help people do this...
        //https://github.com/MonkeyWearingAFezWithAMop

        GlobalFunctions.printError("UNIMPLEMENTED!!!!", this);
        return false;
    }

    public void TRY_tradeInCards(IEnumerable<int> _card_IDs,string _trade_mode)
    {
        bool _is_valid_trade = this.isValidTrade(_card_IDs.ToList(), _trade_mode);

        if(_is_valid_trade == false)
        {
            if(this.debugging)
                GlobalFunctions.print("this.isValidTrade returned false... returning",this);
            return;
        }



        //TODO - does the team have to be in the "deploy" state? or the force trade in state?
        //the other alternative is the deploy state || they have 5? cards...




        List<int> _card_IDs_List = new List<int>(_card_IDs);


        if (_card_IDs_List.Count() <= 0)
        {
            //no...
            if (this.debugging)
                GlobalFunctions.print("_card_IDs.Count() <= 0?!?!?!", this);
            return;
        }


        foreach (int _ID in _card_IDs_List)
        {
            //if the team doesnt have the required ids...
            int _count = _card_IDs_List.Where(x => x == _ID).ToList().Count();

            int _Team_Count = this.territory_card_IDs.Where(x => x == _ID).ToList().Count();

            if (_count > _Team_Count)
            {
                if (debugging)
                    GlobalFunctions.print("the team doesnt have enough of the cards with the id = " + _ID, this);
                return;
            }

        }

        int _bonus_troops = 0;

        if (_trade_mode == RiskySandBox_TerritoryCard.progressive_mode)//TODO - and this team is allowed to trade fixed mode...
        {
            _bonus_troops = this.calculateProgressiveTradeIn(this.n_progressive_card_trade_in);

            this.n_progressive_card_trade_in.value += 1;
        }

        if (_trade_mode == RiskySandBox_TerritoryCard.fixed_mode)
        {
            this.deployable_troops.value += this.calculateFixedTradeIn(_card_IDs);
        }

        if(_trade_mode == RiskySandBox_TerritoryCard.geometric_mode)
        {
            this.deployable_troops.value += this.calculateGeometricTradeIn(this.n_geometric_tradein);
            this.n_geometric_tradein.value += 1;
        }

        foreach (int _card_id in _card_IDs_List)
        {
            //remove this card from the team...
            this.territory_card_IDs.Remove(_card_id);
        }

        //give the team the deployable troops!
        this.deployable_troops.value += _bonus_troops;

        RiskySandBox_Team.OnterritoryCardTradeIn?.Invoke(this, _trade_mode, _bonus_troops);//tell everyone a trade in just happened...


        //if they are in the force trade in state????
        if (this.current_turn_state == RiskySandBox_Team.turn_state_force_trade_in)
        {
            if (this.num_cards < this.max_n_territory_cards)
            {
                //TODO - nope! - we want to instead say something like _Team.TRY_enterDeployState() - this is because of capitals (or other turn states) may need to be dealt with before entering the deploy state...
                //say for example a player started the game with 5 cards and needed to place a capital! - they would first need to place the capital... play the 5 cards then go into deploy state!
                //obviously this isnt how the standard rules work... but this game is a sandbox mode where many unusual things can happen! - lets try to anticipate (and allow) weird things to happen!
                this.current_turn_state.value = RiskySandBox_Team.turn_state_deploy;
            }
        }

    }

    public void autoTrade()
    {
        if (this.territory_card_IDs.Count <= 2)
            return;

        string _best_card_mode = "";
        int _best_card_reward = -1;
        List<int> _best_card_combination = new List<int>();

        //TODO - magic 3!!!!!
        foreach(IEnumerable<int> _combination in GlobalFunctions.GetCombinations(this.territory_card_IDs,3))//go through each combo of 3 cards
        {
            int _combination_reward = -1;
            string _combination_trade_mode = "";

            List<int> _card_IDs = new List<int>(_combination);

            if(this.isValidTrade(_card_IDs,RiskySandBox_TerritoryCard.fixed_mode))
            {
                _combination_reward = this.calculateFixedTradeIn(_card_IDs);
                _combination_trade_mode = RiskySandBox_TerritoryCard.fixed_mode;
            }

            if(this.isValidTrade(_card_IDs,RiskySandBox_TerritoryCard.progressive_mode))
            {
                int _progressive_reward = this.calculateProgressiveTradeIn(this.n_progressive_card_trade_in);
                if(_progressive_reward >= _combination_reward)
                {
                    _combination_reward = _progressive_reward;
                    _combination_trade_mode = RiskySandBox_TerritoryCard.progressive_mode;
                }
            }

            if(this.isValidTrade(_card_IDs,RiskySandBox_TerritoryCard.geometric_mode))
            {
                int _geometric_reward = this.calculateGeometricTradeIn(this.n_geometric_tradein);
                if(_geometric_reward >= _combination_reward)
                {
                    _combination_reward = _geometric_reward;
                    _combination_trade_mode = RiskySandBox_TerritoryCard.geometric_mode;
                }
                
            }

            if (_combination_trade_mode == "")
                continue;

            if(_best_card_mode == "")
            {
                //we just set this to be the best...
                _best_card_mode = _combination_trade_mode;
                _best_card_reward = _combination_reward;
                _best_card_combination = new List<int>(_card_IDs);
                continue;
            }

            if (_best_card_reward > _combination_reward)
                continue;

            int _n_jokers_in_best = _best_card_combination.Where(x => x == RiskySandBox_TerritoryCard.wildcard_ID).Count();
            int _n_jokers_in_combination = _card_IDs.Where(x => x == RiskySandBox_TerritoryCard.wildcard_ID).Count();

            if(_n_jokers_in_combination < _n_jokers_in_best)//try to save jokers...
            {
                _best_card_combination = _card_IDs;
                _best_card_mode = _combination_trade_mode;
                _best_card_reward = _combination_reward;
                continue;
            }

            //TODO
            //ok so now you "should" try to optimize for trading the same type of card...
            //e.g. if you have 1 infantry, 1 cavalry and 3 artillary
            //you "should" trade the 3 artillary in order to increase your chances of pull another artillary from the deck in order to trade again...
            //honestly, I can't even be bothered to try and do this so good luck if you decide to!
        }

        if(_best_card_mode != "")
        {
            this.TRY_tradeInCards(_best_card_combination, _best_card_mode);
        }

    }

    public int GET_itemPrice(string _item_type)
    {
        if (this.shop_item_types.Contains(_item_type) == false)
        {
            GlobalFunctions.printWarning("this.shop_item_types.Contains('{0}') == false", this);
            return int.MaxValue;

        }

        if(this.shop_item_types.Count != this.shop_item_prices.Count)
        {
            GlobalFunctions.printError("shop item list error...", this);
            return int.MaxValue;
        }

        int _index = this.shop_item_types.IndexOf(_item_type);
        return this.shop_item_prices[_index];
    }



}
