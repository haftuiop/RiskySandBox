using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_DeployUI : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;

    [SerializeField] ObservableBool PRIVATE_root_state;
    [SerializeField] ObservableBool deploy_target_doesnt_equal_null;


    private void Awake()
    {
        RiskySandBox_HumanPlayer.OnVariableUpdate_deploy_target_STATIC += EventReceiver_OnVariableUpdate_deploy_target_STATIC;
    }

    private void OnDestroy()
    {
        RiskySandBox_HumanPlayer.OnVariableUpdate_deploy_target_STATIC -= EventReceiver_OnVariableUpdate_deploy_target_STATIC;
    }

    void EventReceiver_OnVariableUpdate_deploy_target_STATIC(RiskySandBox_HumanPlayer _HumanPlayer)
    {
        if (_HumanPlayer != this.my_HumanPlayer)
            return;
        this.deploy_target_doesnt_equal_null.value = my_HumanPlayer.deploy_target != null;
    }
}
