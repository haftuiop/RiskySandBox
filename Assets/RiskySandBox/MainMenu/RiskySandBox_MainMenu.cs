using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainMenu : MonoBehaviour
{
    public static RiskySandBox_MainMenu instance;

    [SerializeField] bool debugging;

    public static ObservableBool is_enabled { get { return instance.PRIVATE_is_enabled; } }

    [SerializeField] ObservableBool PRIVATE_is_enabled;



    [SerializeField] UnityEngine.UI.InputField nickname_InputField;



    public UnityEngine.Events.UnityEvent Onenable_Inspector;

    public static event Action Onenable;


    public ObservableBool enable_full_screen;


    private void Awake()
    {
        instance = this;
    }


    public void enable()
    {
        this.PRIVATE_is_enabled.value = true;

        if (Photon.Pun.PhotonNetwork.IsConnected)
            Photon.Pun.PhotonNetwork.Disconnect();

        if ( (MultiplayerBridge_Mirror.is_enabled == true) && (PrototypingAssets.run_client_code == true))//dont want to shut down a dedicated server...
            MultiplayerBridge_Mirror.instance.shutdown();


        Onenable?.Invoke();
        Onenable_Inspector.Invoke();

    }

    public void disable()
    {
        this.PRIVATE_is_enabled.value = false;
    }



    public void EventReceiver_OnVariableUpdate_enable_full_screen(ObservableBool _enable_full_screen)
    {
        Screen.fullScreen = _enable_full_screen;
    }


    public void returnToMainMenu()
    {
        this.enable();
    }
    



    private void Start()
    {

        enable();


        
    }


     





    public void quitGame()
    {
        Application.Quit();
    }




}
