using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGameUI : MonoBehaviour
{
    public static RiskySandBox_MainGameUI instance;

    public static ObservableInt team_info_shift { get { return instance.PRIVATE_team_info_shift; } }

    [SerializeField] bool debugging;

    [SerializeField] ObservableInt PRIVATE_team_info_shift;

    public Vector2 team_info_start = new Vector2(-100, -60);

    [SerializeField] ObservableBool PRIVATE_root_state;
    [SerializeField] ObservableBool PRIVATE_show_team_info_UIs;

    [SerializeField] UnityEngine.UI.Text num_cards_Text;

    void Awake()
    {
        instance = this;

        this.PRIVATE_team_info_shift.OnUpdate += delegate { redrawTeamInfoUIs(); };
        this.PRIVATE_root_state.OnUpdate += delegate { redrawTeamInfoUIs(); };
        this.PRIVATE_show_team_info_UIs.OnUpdate += delegate { redrawTeamInfoUIs(); };

        RiskySandBox_Team.all_instances.OnUpdate += EventReceiver_OnteamListUpdate;
        RiskySandBox_Team.OnUpdate_territory_card_IDs_STATIC += EventReceiver_OnUpdate_territory_card_IDs_STATIC;

        RiskySandBox_HumanPlayer.OnVariableUpdate_my_Team_ID_STATIC += EventReceiver_OnVariableUpdate_my_Team_ID_STATIC;

    }

    private void OnDestroy()
    {
        RiskySandBox_Team.all_instances.OnUpdate -= EventReceiver_OnteamListUpdate;
        RiskySandBox_Team.OnUpdate_territory_card_IDs_STATIC -= EventReceiver_OnUpdate_territory_card_IDs_STATIC;

        RiskySandBox_HumanPlayer.OnVariableUpdate_my_Team_ID_STATIC -= EventReceiver_OnVariableUpdate_my_Team_ID_STATIC;
    }

    void EventReceiver_OnteamListUpdate() { redrawTeamInfoUIs(); }
    void EventReceiver_OnUpdate_territory_card_IDs_STATIC(RiskySandBox_Team _Team) { updateNumCardsText(); }
    void EventReceiver_OnVariableUpdate_my_Team_ID_STATIC(RiskySandBox_HumanPlayer _HumanPlayer) { updateNumCardsText(); }

    private void Start()
    {
        updateNumCardsText();
    }

    public void redrawTeamInfoUIs()
    {
        foreach(RiskySandBox_Team _Team in RiskySandBox_Team.all_instances.Where(x => x != null && x.my_UI != null))
        {
            int _shift = PRIVATE_team_info_shift - _Team.ID;
            _Team.my_UI.gameObject.SetActive(this.PRIVATE_root_state.value && this.PRIVATE_show_team_info_UIs.value && (_shift >= -5 && _shift <= 0));//disable if "off screen or we are trying to hide the ui...)

            Vector2 _final_position = team_info_start + new Vector2(0, (_shift) * 60);//TODO remove magic 60??? (background.height??)
            _Team.my_UI.GetComponent<RectTransform>().anchoredPosition = _final_position;
        }
    }

    public void EventReceiver_OnshopButtonPressed()
    {
        RiskySandBox_Team _local_player_Team = RiskySandBox_HumanPlayer.local_player_Team;
        if (_local_player_Team != null)
        {
            if (RiskySandBox_ShopUI.display_Team == _local_player_Team)
            {
                RiskySandBox_ShopUI.display_Team = null;
                this.PRIVATE_root_state.value = true;
            }
            else
            {
                RiskySandBox_ShopUI.display_Team = _local_player_Team;
                this.PRIVATE_root_state.value = false;
            }
        }
    }

    public void EventReceiver_OncardsMenuButtonPressed()
    {
        RiskySandBox_Team _local_player_Team = RiskySandBox_HumanPlayer.local_player_Team;
        if(_local_player_Team != null)
        {
            if(RiskySandBox_TerritoryCardTradeInUI.display_Team == _local_player_Team)
            {
                RiskySandBox_TerritoryCardTradeInUI.display_Team = null;
                this.PRIVATE_root_state.value = true;
            }
            else
            {
                RiskySandBox_TerritoryCardTradeInUI.display_Team = _local_player_Team;
                this.PRIVATE_root_state.value = false;
            }
        }
    }

    public void EventReceiver_OnBattleLogButtonPressed()
    {
        if(RiskySandBox_BattleLog.root_state == true)
        {
            RiskySandBox_BattleLog.root_state.value = false;
            this.PRIVATE_root_state.value = true;
        }
        else
        {
            RiskySandBox_BattleLog.root_state.value = true;
            this.PRIVATE_root_state.value = false;
        }
    }

    public void EventReceiver_OnDiseaseMenuButtonPressed()
    {
        if (RiskySandBox_DiseaseManagerUI.root_state == true)
        {
            RiskySandBox_DiseaseManagerUI.root_state.value = false;
            this.PRIVATE_root_state.value = true;
        }
        else
        {
            RiskySandBox_DiseaseManagerUI.root_state.value = true;
            this.PRIVATE_root_state.value = false;
        }
    }

    public void EventReceiver_OnItemSelectMenuButtonPressed()
    {
        if(RiskySandBox_ItemSelectMenu.root_state.value == true)
        {
            RiskySandBox_ItemSelectMenu.root_state.value = false;
            this.PRIVATE_root_state.value = true;
        }
        else
        {
            RiskySandBox_ItemSelectMenu.root_state.value = true;
            this.PRIVATE_root_state.value = false;

            if (RiskySandBox_HumanPlayer.local_player_Team != null)
                RiskySandBox_ItemSelectMenu.display_Team = RiskySandBox_HumanPlayer.local_player_Team;

        }
    }


    void updateNumCardsText()
    {
        RiskySandBox_Team _local_player_Team = RiskySandBox_HumanPlayer.local_player_Team;

        if(_local_player_Team != null)
        {
            this.num_cards_Text.text = "" + _local_player_Team.territory_card_IDs.Count();
        }
        else
        {
            this.num_cards_Text.text = "-1";
        }
    }
    
}
