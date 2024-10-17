using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_GameSetupUI : MonoBehaviour
{

    public static RiskySandBox_GameSetupUI instance;

    [SerializeField] ObservableBool PRIVATE_is_enabled;


    [SerializeField] UnityEngine.UI.Button create_sp_Button;

    //multiplayer room
    [SerializeField] ObservableString create_mp_room_code;
    [SerializeField] UnityEngine.UI.Button create_mp_Button;

    




    void Awake()
    {
        instance = this;
        MultiplayerBridge_PhotonPun.in_room.OnUpdate_true += delegate { this.disable(); };

        create_mp_Button.onClick.AddListener(delegate { MultiplayerBridge_PhotonPun.instance.createMultiplayerRoom(create_mp_room_code.value); });
        create_sp_Button.onClick.AddListener(delegate { MultiplayerBridge_PhotonPun.instance.createSinglePlayerRoom(); });

    }

    public void enable()
    {
        PRIVATE_is_enabled.value = true;
    }

    void disable()
    {
        PRIVATE_is_enabled.value = false;
    }

    private void Start()
    {
        this.disable();
    }


    private void Update()
    {
        create_mp_Button.interactable = create_mp_room_code.value != "";
    }
}
