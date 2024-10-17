using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team_MainGameUI : MonoBehaviour
{


    [SerializeField] bool debugging;
    [SerializeField] RiskySandBox_Team my_Team;

    [SerializeField] GameObject team_info_root;


    [SerializeField] ObservableBool enable_root;


    [SerializeField] UnityEngine.UI.Text remaining_turn_length_Text;
    [SerializeField] UnityEngine.UI.Image turn_timer_Image;

    [SerializeField] UnityEngine.UI.Image UI_background_Image;


    [SerializeField] UnityEngine.UI.Text num_troops_Text;
    [SerializeField] UnityEngine.UI.Text num_Tiles_Text;


    [SerializeField] Vector2 team_info_start;


    bool called_EventReceiver_OnendGame;


    private void Awake()
    {
        RiskySandBox_MainGameUI.team_info_shift.OnUpdate += EventReceiver_OnVariableUpdate_team_info_shift;
        RiskySandBox_MainGame.game_started.OnUpdate += EventReceiver_OnVariableUpdate_game_started;

        RiskySandBox_MainGame.OnendGame += EventReceiver_OnendGame;



        my_Team.ID.OnUpdate += EventReceiver_OnVariableUpdate_ID;
    }

    private void OnDestroy()
    {
        RiskySandBox_MainGameUI.team_info_shift.OnUpdate -= EventReceiver_OnVariableUpdate_team_info_shift;
        RiskySandBox_MainGame.game_started.OnUpdate -= EventReceiver_OnVariableUpdate_game_started;

        RiskySandBox_MainGame.OnendGame -= EventReceiver_OnendGame;

    } 

    private void Start()
    {
        this.enable_root.value = this.enable_root.value = RiskySandBox_MainGame.game_started && (called_EventReceiver_OnendGame == false);
    }

    void EventReceiver_OnVariableUpdate_game_started(ObservableBool _game_started)
    {
        this.enable_root.value = this.enable_root.value = RiskySandBox_MainGame.game_started && (called_EventReceiver_OnendGame == false);
    }


    void EventReceiver_OnVariableUpdate_team_info_shift(ObservableInt _team_info_shift)
    {
        updateInfoPosition();
    }

    void EventReceiver_OnendGame()
    {
        this.called_EventReceiver_OnendGame = true;
        this.enable_root.value = this.enable_root.value = RiskySandBox_MainGame.game_started && (called_EventReceiver_OnendGame == false);
    }




    void updateInfoPosition()
    {
        int _shift = RiskySandBox_MainGameUI.team_info_shift - this.my_Team.ID;
        Vector2 _final_position = team_info_start + new Vector2(0, (_shift) * 60);//TODO remove magic 60??? (background.height??)
        this.team_info_root.GetComponent<RectTransform>().anchoredPosition = _final_position;

        if(this.debugging)
            GlobalFunctions.print("updating ui position to be " + _final_position,this);

    }

    public void EventReceiver_OnVariableUpdate_ID(ObservableInt _ID)
    {
        if (this.debugging)
            GlobalFunctions.print("my ID just changed... updating the ui element colors!", this, _ID);

        UI_background_Image.color = my_Team.my_Color;

        this.turn_timer_Image.color = my_Team.my_Color;

        num_troops_Text.color = my_Team.text_Color;
        num_Tiles_Text.color = my_Team.text_Color;
        remaining_turn_length_Text.color = my_Team.text_Color;

        updateInfoPosition();


    }



    // Update is called once per frame
    void Update()
    {
        float _remaining_turn_length = my_Team.end_turn_time_stamp.value - RiskySandBox.current_time.value;//TODO - replace with my_Team.remaining_turn_length - just duplicated code otherwise...

        if (this.remaining_turn_length_Text != null)
            this.remaining_turn_length_Text.text = "" + Mathf.CeilToInt(_remaining_turn_length);

        float _fill_amount = _remaining_turn_length / this.my_Team.turn_length_seconds.value;

        if (_fill_amount < 0)
            _fill_amount = 0;

        if (_fill_amount > 1)
            _fill_amount = 1;

        turn_timer_Image.fillAmount = _fill_amount;
    }
}
