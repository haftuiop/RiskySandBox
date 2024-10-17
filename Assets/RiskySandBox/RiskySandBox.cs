using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox : MonoBehaviour
{

    public static string maps_folder_path
    {
        get
        {
            string _value = System.IO.Path.Combine(Application.streamingAssetsPath + "/RiskySandBox/Maps");
            return _value;
        }
    }


    public static RiskySandBox instance { get; private set; }
    [SerializeField] bool debugging;


    public static ObservableFloat current_time { get { return instance.PRIVATE_current_time; } }
    [SerializeField] ObservableFloat PRIVATE_current_time;

    public Material null_Team_Material { get { return instance.PRIVATE_neutral_Team_Material; } }
    [SerializeField] Material PRIVATE_neutral_Team_Material;



    public static ObservableBool is_online { get { return instance.PRIVATE_is_online; } }
    [SerializeField] ObservableBool PRIVATE_is_online;

    void Awake()
    {
        instance = this;
        MultiplayerBridge_Mirror.is_enabled.OnUpdate += EventReceiver_OnVariableUpdate_is_enabled;
        MultiplayerBridge_PhotonPun.in_room.OnUpdate += EventReceiver_OnVariableUpdate_in_room;
    }

    private void Start()
    {
        RECALCULATE_is_online();
    }

    void EventReceiver_OnVariableUpdate_is_enabled(ObservableBool _is_enabled)
    {
        RECALCULATE_is_online();
    }

    void EventReceiver_OnVariableUpdate_in_room(ObservableBool _in_room)
    {
        RECALCULATE_is_online();
    }

    void RECALCULATE_is_online()
    {

        RiskySandBox.is_online.value = MultiplayerBridge_Mirror.is_enabled || MultiplayerBridge_PhotonPun.in_room;
    }






    private void Update()
    {
        if (RiskySandBox_MainGame.game_started.value == false)
            return;

        this.PRIVATE_current_time.value += Time.deltaTime;
    }


}
