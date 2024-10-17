using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_AccountSystem : MonoBehaviour
{
    public static RiskySandBox_AccountSystem instance;
    //NOTE - this should be completley seperate from anything related to authenticating the users commands (this is JUST what gets shown on the (UIs) to the human players....
    public static ObservableString display_name { get { return instance.PRIVATE_display_name; } }
    public static ObservableBool ui_root_state { get { return instance.PRIVATE_ui_root_state; } }


    [SerializeField] bool debugging;
    [SerializeField] ObservableString PRIVATE_display_name;
    [SerializeField] ObservableBool PRIVATE_ui_root_state;

    private void Awake()
    {
        instance = this;
        this.PRIVATE_display_name.OnUpdate += delegate { SET_multiplayerNickNames(this.PRIVATE_display_name); };
    }


    private void Start()
    {
        this.PRIVATE_display_name.value = PlayerPrefs.GetString("RiskySandBox_display_name", "Player: " + GlobalFunctions.randomInt(0, 1000000));
        this.disableUI();
    }



    public void enableUI()
    {
        RiskySandBox_AccountSystem.ui_root_state.value = true;
    }

    public void disableUI()
    {
        RiskySandBox_AccountSystem.ui_root_state.value = false;
    }


    void SET_multiplayerNickNames(string _new_value)
    {
        MultiplayerBridge_Mirror.NickName.value = _new_value;
        Photon.Pun.PhotonNetwork.NickName = _new_value;
    }



}
