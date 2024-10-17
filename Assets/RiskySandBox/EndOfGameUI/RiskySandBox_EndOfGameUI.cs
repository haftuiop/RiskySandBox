using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_EndOfGameUI : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] ObservableBool PRIVATE_root_state;

    [SerializeField] ObservableInt PRIVATE_shift;


    [SerializeField] GameObject team_end_of_game_ui_prefab;

    [SerializeField] GameObject root;


    [SerializeField] List<GameObject> instantiated_team_UIs = new List<GameObject>();


    [SerializeField] Vector2 team_ui_start;


    public void disable()
    {
        PRIVATE_root_state.value = false;
    }

    public void enable()
    {
        this.PRIVATE_root_state.value = true;
    }

    private void Start()
    {
        this.disable();
    }


    private void Awake()
    {
        this.PRIVATE_root_state.OnUpdate_true += EventReceiver_OnVariableUpdate_PRIVATE_root_state_true;
        this.PRIVATE_shift.OnUpdate += delegate { updatePositions();  };

        RiskySandBox_MainGame.OnendGame += EventReceiver_OnendGame;
    }

    private void OnDestroy()
    {
        RiskySandBox_MainGame.OnendGame -= EventReceiver_OnendGame;
    }

    void EventReceiver_OnendGame()
    {
        this.enable();
    }

    void updatePositions()
    {
        foreach (GameObject _GameObject in this.instantiated_team_UIs)
        {
            int _shift = this.PRIVATE_shift.value + _GameObject.GetComponent<RiskySandBox_Team_EndGameUI>().my_Team.game_over_placement;
            _GameObject.GetComponent<RectTransform>().anchoredPosition = team_ui_start + (new Vector2(100, 0) * _shift);
        }
    }

    void EventReceiver_OnVariableUpdate_PRIVATE_root_state_true(ObservableBool _PRIVATE_root_state)
    {
        foreach(GameObject _GameObject in this.instantiated_team_UIs)
        {
            UnityEngine.Object.Destroy(_GameObject.gameObject);
        }

        this.instantiated_team_UIs.Clear();

        //so show the first,2nd,3rd (and so on)
        foreach(RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
        {
            RiskySandBox_Team_EndGameUI _new_UI = UnityEngine.Object.Instantiate(team_end_of_game_ui_prefab, this.root.transform).GetComponent<RiskySandBox_Team_EndGameUI>();
            _new_UI.my_Team = _Team;
            this.instantiated_team_UIs.Add(_new_UI.gameObject);
            
        }

        updatePositions();


    }



}
