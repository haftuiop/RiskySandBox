using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_TerritoryCardTradeInUI : MonoBehaviour
{
    public static RiskySandBox_TerritoryCardTradeInUI instance;

    public static RiskySandBox_Team display_Team
    {
        get { return instance.PRIVATE_display_Team; }
        set
        {
            instance.PRIVATE_display_Team = value;
            instance.updateUI(value);
        }
    }


    public static RiskySandBox_HumanPlayer local_HumanPlayer { get { return RiskySandBox_HumanPlayer.local_player; } }


    [SerializeField] bool debugging;
    [SerializeField] ObservableBool PRIVATE_auto_trade;

    [SerializeField] Vector2 territory_card_start_point = new Vector2(-350, 0);

    List<GameObject> instantiated_territory_cards = new List<GameObject>();

    [SerializeField] RiskySandBox_Team PRIVATE_display_Team;

    [SerializeField] GameObject root;
    [SerializeField] ObservableBool PRIVATE_root_state;

    [SerializeField] List<UnityEngine.UI.Text> geometric_reward_Texts = new List<UnityEngine.UI.Text>();
    [SerializeField] List<UnityEngine.UI.Text> progressive_reward_Texts = new List<UnityEngine.UI.Text>();

    [SerializeField] UnityEngine.UI.Text fixed_reward_Text_3_infantry;
    [SerializeField] UnityEngine.UI.Text fixed_reward_Text_3_cavalry;
    [SerializeField] UnityEngine.UI.Text fixed_reward_Text_3_cannons;
    [SerializeField] UnityEngine.UI.Text fixed_reward_Text_all_3;


    private void Awake()
    {
        instance = this;

        RiskySandBox_Team.OnUpdate_territory_card_IDs_STATIC += EventReceiver_OnUpdate_territory_card_IDs_STATIC;
        RiskySandBox_Team.OnTerritoryCardTradeInParameterChange_STATIC += EventReceiver_OnTerritoryCardTradeInParameterChange_STATIC;
    }

    private void OnDestroy()
    {
        RiskySandBox_Team.OnUpdate_territory_card_IDs_STATIC -= EventReceiver_OnUpdate_territory_card_IDs_STATIC;
        RiskySandBox_Team.OnTerritoryCardTradeInParameterChange_STATIC -= EventReceiver_OnTerritoryCardTradeInParameterChange_STATIC;
    }

    void EventReceiver_OnUpdate_territory_card_IDs_STATIC(RiskySandBox_Team _Team) { updateUI(this.PRIVATE_display_Team); }
    void EventReceiver_OnTerritoryCardTradeInParameterChange_STATIC(RiskySandBox_Team _Team) { if (_Team != this.PRIVATE_display_Team) return;updateUI(this.PRIVATE_display_Team); }

    void updateUI(RiskySandBox_Team _Team)
    {
        foreach (var _GameObject in this.instantiated_territory_cards.Where(x => x != null))
        {
            UnityEngine.Object.Destroy(_GameObject.gameObject);
        }
        this.instantiated_territory_cards.Clear();

        this.PRIVATE_root_state.value = _Team != null;

        if (_Team == null)
            return;

        for (int i = 0; i < this.progressive_reward_Texts.Count(); i += 1)
        {
            this.progressive_reward_Texts[i].text = "" + _Team.calculateProgressiveTradeIn(_Team.n_progressive_card_trade_in + i);
        }

        for (int i = 0; i < this.geometric_reward_Texts.Count(); i += 1)
        {
            this.geometric_reward_Texts[i].text = "" + _Team.calculateGeometricTradeIn(_Team.n_geometric_tradein + i);
        }

        this.fixed_reward_Text_3_infantry.text = "" + _Team.fixed_card_tradein_3_person;
        this.fixed_reward_Text_3_cavalry.text = "" + _Team.fixed_card_tradein_3_horse;
        this.fixed_reward_Text_3_cannons.text = "" + _Team.fixed_card_tradein_3_artillary;
        this.fixed_reward_Text_all_3.text = "" + _Team.fixed_card_tradein_all_3;



        List<int> _territory_card_IDs = new List<int>(_Team.territory_card_IDs);

        for (int i = 0; i < _territory_card_IDs.Count; i += 1)
        {
            if (this.debugging)
                GlobalFunctions.print("creating a new TerritoryCard with id " + _territory_card_IDs[i], this);
            //instantiate a new trading card...
            RiskySandBox_TerritoryCard _new_card = RiskySandBox_TerritoryCard.createNew(_territory_card_IDs[i], root.transform);

            this.instantiated_territory_cards.Add(_new_card.gameObject);

            //_new_card update position...
            _new_card.GetComponent<RectTransform>().anchoredPosition = territory_card_start_point + new Vector2(100 * i, 0);//TODO - magic 100 this should be something like TerritoryCard.card_ui_width or something...

        }



    }


    public void OntradeButtonPressed_progressive()
    {
        if (this.debugging)
            GlobalFunctions.print("asking my HumanPlayer to trade in the selected cards! (progressive mode)", this);

        if (local_HumanPlayer == null)
        {
            GlobalFunctions.printWarning("no local human player???", this);
            return;
        }

        local_HumanPlayer.TRY_tradeInSelectedTerritoryCards(RiskySandBox_TerritoryCard.progressive_mode);
    }


    public void OntradeButtonPressed_fixed()
    {
        if (this.debugging)
            GlobalFunctions.print("asking my_HumanPlayer to trade in the selected cards! (fixed mode)", this);

        if (local_HumanPlayer == null)
        {
            GlobalFunctions.printWarning("no local human player???", this);
            return;
        }

        local_HumanPlayer.TRY_tradeInSelectedTerritoryCards(RiskySandBox_TerritoryCard.fixed_mode);
    }

    public void OntradeButtonPressed_geometric()
    {
        if (this.debugging)
            GlobalFunctions.print("asking my_HumanPlayer to trade in the selected cards! (fixed mode)", this);

        if (local_HumanPlayer == null)
        {
            GlobalFunctions.printWarning("no local human player???", this);
            return;
        }
        local_HumanPlayer.TRY_tradeInSelectedTerritoryCards(RiskySandBox_TerritoryCard.geometric_mode);
    }


    public void autoTrade(RiskySandBox_Team _Team)
    {
        //ok! we shall have a look
    }

    public void EventReceiver_OnautoTradeButtonPressed()
    {
        if (this.PRIVATE_display_Team == null)
        {
            if (this.debugging)
                GlobalFunctions.print("auto trade button pressed but this.PRIVATE_display_Team == null", this);
            return;
        }

        if(this.debugging)
            GlobalFunctions.print("asking the display team to autoTrade...", this);
        
        this.PRIVATE_display_Team.autoTrade();
    }
}
