using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_LeaderBoardUI : MonoBehaviour
{

    public static RiskySandBox_LeaderBoardUI instance;

    [SerializeField] bool debugging;

    public ObservableBool root_state { get { return this.PRIVATE_root_state; } }
    [SerializeField] ObservableBool PRIVATE_root_state;


    [SerializeField] UnityEngine.UI.Text current_channel_Text;


    public static event Action OnVariableUpdate_display_start;
    public static event Action OnVariableUpdate_display_end;


    public static ObservableInt display_start { get { return instance.PRIVATE_display_start; } }
    public static ObservableInt display_end { get { return instance.PRIVATE_display_end; } }


    [SerializeField] private ObservableInt PRIVATE_display_start;
    [SerializeField] private ObservableInt PRIVATE_display_end;


    [SerializeField] RectTransform ui_start_RectTransform;


    public List<GameObject> temp_elements = new List<GameObject>();


    public GameObject player_data_UI_prefab;
    public GameObject player_data_root;


    public ObservableInt channel_index;
    public ObservableString map_ID;


    [SerializeField] List<string> channels = new List<string>();

    private void Awake()
    {
        instance = this;

        this.channel_index.min_value = 0;
        this.channel_index.max_value = this.channels.Count - 1;

        FriendsOfRiskBridge.OnUpdate_RankingData += EventReceiver_OnUpdate_RankingData;
        this.channel_index.OnUpdate += EventReceiver_OnVariableUpdate_channel_index;

        this.root_state.OnUpdate += EventReceiver_OnVariableUpdate_root_State;


    }

    private void OnDestroy()
    {
        FriendsOfRiskBridge.OnUpdate_RankingData -= EventReceiver_OnUpdate_RankingData;
        this.channel_index.OnUpdate -= EventReceiver_OnVariableUpdate_channel_index;
    }

    void EventReceiver_OnUpdate_RankingData()
    {
        if (this.debugging)
            GlobalFunctions.print("", this);
        updateUI();
    }

    void EventReceiver_OnVariableUpdate_root_State(ObservableBool _root_state)
    {
        if (_root_state.value == false)
            return;

        FriendsOfRiskBridge.instance.updateRankingData(this.channels[channel_index], this.map_ID);
        this.updateUI();

        this.PRIVATE_display_start.value = 1;
        this.PRIVATE_display_end.value = 12;

    }


    void EventReceiver_OnVariableUpdate_channel_index(ObservableInt _channel_index)
    {
        FriendsOfRiskBridge.instance.updateRankingData(this.channels[channel_index], this.map_ID);

    }



    public void EventReceiver_OnMainMenuButtonPressed()
    {
        RiskySandBox_MainMenu.instance.returnToMainMenu();
    }


    public void disable()
    {
        this.root_state.value = false;
    }





    void updateUI()
    {


        try
        {
            this.current_channel_Text.text = this.channels[this.channel_index];
        }
        catch
        {
            //weird.... but ok
        }


        foreach(GameObject _GameObject in temp_elements)
        {
            UnityEngine.Object.Destroy(_GameObject);
        }

        temp_elements.Clear();

        //pull the most reasont ranking data...
        var _leaderboard_data = FriendsOfRiskBridge.ranking_data;
        for (int i = 0; i < _leaderboard_data.Count(); i += 1)
        {
            RiskySandBox_RankingSystem_SimpleDetailsUI _simpledetails_ui = UnityEngine.Object.Instantiate(player_data_UI_prefab,player_data_root.transform).GetComponent< RiskySandBox_RankingSystem_SimpleDetailsUI>();
            _simpledetails_ui.GetComponent<RectTransform>().anchoredPosition = ui_start_RectTransform.anchoredPosition + new Vector2(0, -30 * i);

            temp_elements.Add(_simpledetails_ui.gameObject);

            _simpledetails_ui.display_name.value = _leaderboard_data[i].player;
            _simpledetails_ui.rank.value = _leaderboard_data[i].rank;
            
        }




    }

}
