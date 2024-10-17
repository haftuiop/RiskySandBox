using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_RankingSystem_SimpleDetailsUI : MonoBehaviour
{
    [SerializeField] bool debugging;

    public ObservableString display_name { get { return PRIVATE_display_name; } }
    [SerializeField] ObservableString PRIVATE_display_name;

    public ObservableInt rank { get { return this.PRIVATE_rank; } }
    [SerializeField] ObservableInt PRIVATE_rank;



    private void Awake()
    {
        RiskySandBox_LeaderBoardUI.OnVariableUpdate_display_start += EventReceiver_OnVariableUpdate_display_start;
        RiskySandBox_LeaderBoardUI.OnVariableUpdate_display_start += EventReceiver_OnVariableUpdate_display_end;
    }

    private void OnDestroy()
    {
        RiskySandBox_LeaderBoardUI.OnVariableUpdate_display_start -= EventReceiver_OnVariableUpdate_display_start;
        RiskySandBox_LeaderBoardUI.OnVariableUpdate_display_start -= EventReceiver_OnVariableUpdate_display_end;
    }





    private void Start()
    {
        updateUI();
        gameObject.name = string.Format("{0},rank = {1}",this.display_name.value,this.rank.value);
    }


    void EventReceiver_OnVariableUpdate_display_start()
    {
        updateUI();
    }

    void EventReceiver_OnVariableUpdate_display_end()
    {
        updateUI();
    }


    void updateUI()
    {
        this.gameObject.SetActive(this.rank <= RiskySandBox_LeaderBoardUI.display_end && this.rank >= RiskySandBox_LeaderBoardUI.display_start);
    }

}
