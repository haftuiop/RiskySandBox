using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;


public partial class RiskySandBox_JoinRoomUI : MonoBehaviourPunCallbacks
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableBool PRIVATE_is_enabled;
    [SerializeField] UnityEngine.UI.InputField photon_room_name_InputField;
    [SerializeField] UnityEngine.UI.Button join_photon_room_Button;

    [SerializeField] UnityEngine.UI.InputField mirror_ip_address_InputField;
    [SerializeField] UnityEngine.UI.Button join_mirror_server_Button;

    [SerializeField] ObservableBool PRIVATE_is_online;



    private void Awake()
    {
        join_photon_room_Button.onClick.AddListener(delegate { EventReceiver_OnjoinPhotonRoomButtonPressed(); });
        join_mirror_server_Button.onClick.AddListener(delegate { EventReceiver_OnjoinMirrorServerButtonPressed(); });

        PRIVATE_is_online.OnUpdate_true += EventReceiver_OnVariableUpdate_is_online_true;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.disable();
    }

    void EventReceiver_OnjoinPhotonRoomButtonPressed()
    {
        MultiplayerBridge_PhotonPun.instance.joinRoom(photon_room_name_InputField.text);
    }

    void EventReceiver_OnjoinMirrorServerButtonPressed()
    {
        MultiplayerBridge_Mirror.instance.startClient(mirror_ip_address_InputField.text);
    }

    

    void EventReceiver_OnVariableUpdate_is_online_true(ObservableBool _is_online)
    {
        this.disable();
    }



    public void enable()
    {
        this.PRIVATE_is_enabled.value = true;
    }


    private void Update()
    {
        join_photon_room_Button.interactable = photon_room_name_InputField.text != "";
    }

    public void disable()
    {
        this.PRIVATE_is_enabled.value = false;
    }



}
