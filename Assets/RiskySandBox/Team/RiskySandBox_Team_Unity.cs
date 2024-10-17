using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team : MonoBehaviour
{

    [SerializeField] bool debugging;

    [SerializeField] private ObservableInt PRIVATE_capture_start_ID;
    [SerializeField] private ObservableInt PRIVATE_capture_end_ID;
    [SerializeField] private ObservableInt PRIVATE_n_capitals;
    [SerializeField] private ObservableInt PRIVATE_ID;
    [SerializeField] private ObservableBool PRIVATE_defeated;
    [SerializeField] private ObservableInt PRIVATE_defeated_index;

    [SerializeField] private ObservableInt PRIVATE_required_capital_placements;
    [SerializeField] private ObservableInt PRIVATE_n_Tiles;

    [SerializeField] private ObservableInt PRIVATE_deployable_troops;

    [SerializeField] private ObservableInt PRIVATE_assassin_target_ID;
    [SerializeField] private ObservableInt PRIVATE_killer_ID;
    [SerializeField] private ObservableBool PRIVATE_show_assassin_target_indicator;

    [SerializeField] ObservableFloat PRIVATE_turn_length_seconds;
    [SerializeField] ObservableFloat PRIVATE_capture_increment;

    [SerializeField] ObservableString PRIVATE_team_name;
    [SerializeField] ObservableFloat PRIVATE_world_domination_percentage;

    [SerializeField] private ObservableBool PRIVATE_has_captured_Tile;
    [SerializeField] private ObservableBool PRIVATE_allow_deploy_to_ally_Tiles;
    [SerializeField] private ObservableBool PRIVATE_allow_attack_ally_Tiles;
    [SerializeField] private ObservableBool PRIVATE_allow_fortify_to_ally_Tiles;
    [SerializeField] private ObservableBool PRIVATE_allow_move_through_ally_Tiles;

    [SerializeField] ObservableInt PRIVATE_shop_credits;
    [SerializeField] ObservableStringList PRIVATE_purchased_shop_items;
    [SerializeField] ObservableStringList PRIVATE_shop_item_types;
    [SerializeField] ObservableIntList PRIVATE_shop_item_prices;
    [SerializeField] ObservableBool PRIVATE_random_capital_placement;

    [SerializeField] ObservableFloat PRIVATE_end_turn_time_stamp;
    [SerializeField] ObservableBool PRIVATE_is_my_turn;
    [SerializeField] GameObject PRIVATE_my_UI;

    //is the team a human????
    [SerializeField] ObservableBool PRIVATE_is_human;

    [SerializeField] ObservableInt PRIVATE_game_over_placement;

    [SerializeField] ObservableInt PRIVATE_remaining_n_fortifies_this_turn;

    [SerializeField] ObservableInt PRIVATE_num_capitals_startGame;
    [SerializeField] ObservableFloat PRIVATE_capital_conquest_percentage;
    [SerializeField] ObservableInt PRIVATE_capital_troop_generation;

    [SerializeField] ObservableString PRIVATE_alliance_string;
    [SerializeField] ObservableInt PRIVATE_max_n_territory_cards;
    [SerializeField] ObservableInt PRIVATE_n_territory_cards_from_capture;

    [SerializeField] ObservableIntList PRIVATE_territory_card_IDs;




    [SerializeField] private ProgressiveCardSettings my_ProgressiveCardSettings;
    [SerializeField] private FixedCardSettings my_FixedCardSettings;
    [SerializeField] private GeometricCardSettings my_GeometricCardSettings;



    [SerializeField] ObservableBool PRIVATE_enable_FOW;
    [SerializeField] ObservableBool PRIVATE_share_FOW_with_allies;

    [SerializeField] private ObservableString PRIVATE_current_turn_state;

    [SerializeField] private ObservableBool PRIVATE_shared_victory;


    private void Awake()
    {
        all_instances.Add(this);
        RiskySandBox_Tile.OnVariableUpdate_my_Team_ID_STATIC += RiskySandBox_TileEventReceiver_OnSET_my_Team_STATIC;

        this.defeated.OnUpdate_true += delegate
        {
            if (PrototypingAssets.run_server_code == true)
            {
                int _value = RiskySandBox_Team.all_instances.Max(x => x.defeated_index.value) + 1;
                this.defeated_index.value = RiskySandBox_Team.all_instances.Max(x => x.defeated_index.value) + 1;
            }

            RiskySandBox_Team.OnVariableUpdate_defeated_STATIC?.Invoke(this);
        };


        this.ID.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_ID_STATIC?.Invoke(this); };
        this.ID.OnUpdate += delegate { this.gameObject.name = "RiskySandBox_Team with id = " + this.ID; };
        this.capture_end_ID.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_capture_end_ID_STATIC?.Invoke(this); };
        this.assassin_target_ID.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_assassin_target_ID_STATIC?.Invoke(this); };
        this.is_my_turn.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_is_my_turn_STATIC?.Invoke(this); };
        this.killer_ID.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_killer_ID_STATIC?.Invoke(this); };
        this.territory_card_IDs.OnUpdate += delegate { RiskySandBox_Team.OnUpdate_territory_card_IDs_STATIC?.Invoke(this); };

        this.deployable_troops.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_deployable_troops_STATIC?.Invoke(this); };
        this.current_turn_state.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_current_turn_state_STATIC?.Invoke(this); };

        this.shop_credits.OnUpdate += delegate { RiskySandBox_Team.OnShopVariableChange_STATIC?.Invoke(this); };
        this.purchased_shop_items.OnUpdate += delegate { RiskySandBox_Team.OnShopVariableChange_STATIC?.Invoke(this); };
        this.shop_item_types.OnUpdate += delegate { RiskySandBox_Team.OnShopVariableChange_STATIC?.Invoke(this); };
        this.shop_item_prices.OnUpdate += delegate { RiskySandBox_Team.OnShopVariableChange_STATIC?.Invoke(this); };

        this.purchased_shop_items.OnUpdate += delegate { RiskySandBox_Team.OnVariableUpdate_purchased_shop_items_STATIC?.Invoke(this); };

        RiskySandBox.current_time.OnUpdate += EventReceiver_OnVariableUpdate_current_time;




        this.n_progressive_card_trade_in.OnUpdate += delegate { RiskySandBox_Team.OnTerritoryCardTradeInParameterChange_STATIC?.Invoke(this); };
        this.progressive_tradein_start_string.OnUpdate += delegate { RiskySandBox_Team.OnTerritoryCardTradeInParameterChange_STATIC?.Invoke(this); };
        this.progressive_tradein_increment.OnUpdate += delegate { RiskySandBox_Team.OnTerritoryCardTradeInParameterChange_STATIC?.Invoke(this); };

        this.n_geometric_tradein.OnUpdate += delegate { RiskySandBox_Team.OnTerritoryCardTradeInParameterChange_STATIC?.Invoke(this); };
        this.geometric_tradein_a0.OnUpdate += delegate { RiskySandBox_Team.OnTerritoryCardTradeInParameterChange_STATIC?.Invoke(this); };
        this.geometric_tradein_r.OnUpdate += delegate { RiskySandBox_Team.OnTerritoryCardTradeInParameterChange_STATIC?.Invoke(this); };

        this.fixed_card_tradein_3_person.OnUpdate += delegate { RiskySandBox_Team.OnTerritoryCardTradeInParameterChange_STATIC?.Invoke(this); };
        this.fixed_card_tradein_3_horse.OnUpdate += delegate { RiskySandBox_Team.OnTerritoryCardTradeInParameterChange_STATIC?.Invoke(this); };
        this.fixed_card_tradein_3_artillary.OnUpdate += delegate { RiskySandBox_Team.OnTerritoryCardTradeInParameterChange_STATIC?.Invoke(this); };
        this.fixed_card_tradein_all_3.OnUpdate += delegate { RiskySandBox_Team.OnTerritoryCardTradeInParameterChange_STATIC?.Invoke(this); };

        RiskySandBox_Team.OnterritoryCardTradeIn += EventReceiver_OnterritoryCardTradeIn;


    }

    private void OnDestroy()
    {
        all_instances.Remove(this);
        RiskySandBox_Tile.OnVariableUpdate_my_Team_ID_STATIC -= RiskySandBox_TileEventReceiver_OnSET_my_Team_STATIC;
        RiskySandBox_Team.OnterritoryCardTradeIn -= EventReceiver_OnterritoryCardTradeIn;

        RiskySandBox.current_time.OnUpdate -= EventReceiver_OnVariableUpdate_current_time;
    }



    void EventReceiver_OnterritoryCardTradeIn(RiskySandBox_Team _Team,string _trade_mode,int _amount)
    {

        if (_Team == this)
        {
            GlobalFunctions.print("i just traded!",this);
            return;
        }

        GlobalFunctions.print("another team just traded...", this);

        //TODO - ok there is one other thing... lets if you were doing a 2v2 game what if we want to say the team shares progressive trade in (team 1 and 2) vs (team 3 and 4)
        //is there a reason????
        //probs not but maybe????
        //also geometric mode...

        if(this.shared_progressive_mode && _trade_mode == RiskySandBox_TerritoryCard.progressive_mode)
        {
            this.n_progressive_card_trade_in.value += 1;
        }

        if(this.shared_geometric_tradein && _trade_mode == RiskySandBox_TerritoryCard.geometric_mode)
        {
            this.n_geometric_tradein.value += 1;
        }



    }



    public static void destroyAllTeams()
    {


        foreach (RiskySandBox_Team _Team in new List<RiskySandBox_Team>(RiskySandBox_Team.all_instances))
        {
            if (MultiplayerBridge_PhotonPun.in_room)
                Photon.Pun.PhotonNetwork.Destroy(_Team.gameObject);

            else if (MultiplayerBridge_Mirror.is_enabled)
                Mirror.NetworkServer.Destroy(_Team.gameObject);

            else
                UnityEngine.Object.Destroy(_Team.gameObject);

        }
    }


    [Serializable]
    private struct FixedCardSettings
    {
        public ObservableInt infanty_tradein_reward;
        public ObservableInt cavalry_tradein_reward;
        public ObservableInt artillary_tradein_reward;
        public ObservableInt all_3_tradein_reward;
        public ObservableBool allow_fixed_tradein;
    }

    [Serializable]
    private struct GeometricCardSettings
    {
        public ObservableInt n_geometric_tradein;
        public ObservableInt geometric_tradein_a0;
        public ObservableFloat geometric_tradein_r;
        public ObservableBool allow_geometric_tradein;
        public ObservableBool shared_geometric_mode;
    }


    [Serializable]
    private struct ProgressiveCardSettings
    {
        public ObservableBool allow_progressive_tradein;
        public ObservableBool shared_progressive_mode;
        public ObservableInt progressive_tradein_increment;
        public ObservableString progressive_tradein_start_string;
        public ObservableInt n_progressive_card_trade_in;
    }
}
