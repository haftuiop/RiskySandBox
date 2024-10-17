using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MusicPlayer : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] AudioSource main_menu_AudioSource;
    [SerializeField] AudioSource main_game_AudioSource;


    [SerializeField] ObservableBool PRIVATE_main_game_started;


    private void Awake()
    {
        RiskySandBox_MainMenu.Onenable += MainMenuEventReceiver_Onenable;
        PRIVATE_main_game_started.OnUpdate_true += EventReceiver_OnstartGame;
    }

    private void OnDestroy()
    {
        RiskySandBox_MainMenu.Onenable -= MainMenuEventReceiver_Onenable;
    }


    void MainMenuEventReceiver_Onenable()
    {
        main_menu_AudioSource.Play();
        main_game_AudioSource.Stop();
    }


    void EventReceiver_OnstartGame(ObservableBool _game_started)
    {
        main_menu_AudioSource.Stop();
        main_game_AudioSource.Play();

    }
   

}
