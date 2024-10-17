using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;



public partial class RiskySandBox_CommunityMenu : MonoBehaviour
{
    public static RiskySandBox_CommunityMenu instance { get; private set; }
    public static ObservableBool ui_root_state { get { return instance.PRIVATE_ui_root_state; } }


    [SerializeField] bool debugging;
    [SerializeField] ObservableBool PRIVATE_ui_root_state;


    void Awake()
    {
        instance = this;
    }


    public void enable()
    {
        RiskySandBox_CommunityMenu.ui_root_state.value = true;
    }

    public void disable()
    {
        RiskySandBox_CommunityMenu.ui_root_state.value = false;
    }

    /// <summary>
    /// hide the menu instantly...
    /// </summary>
    private void Start()
    {
        RiskySandBox_CommunityMenu.ui_root_state.value = false;
    }


    
    public void openFriendsOfRiskWebPage()
    {
        Application.OpenURL("https://friendsofrisk.com/");
    }

    /// <summary>
    /// so someone else can design the tutorials without having to import them into the game?
    /// </summary>
    public void openTutorialsWebPage()
    {
        Application.OpenURL("https://www.youtube.com/watch?v=y-ESVSkkMus&list=PLwXlWYmDjEDnFq6Tblcjq4irk__2CrBiM");
    }


    public void openTournementsMenu()
    {
        GlobalFunctions.printWarning("unimplemented...",this);
    }

    public void openVideosMenu()
    {
        GlobalFunctions.printWarning("unimplemented...", this);
        Application.OpenURL("https://www.youtube.com/results?search_query=risk+global+domination");
    }

    public void openLiveStreamsMenu()
    {   
        GlobalFunctions.printWarning("unimplemented...", this);
        Application.OpenURL("https://www.twitch.tv/search?term=risk");
    }



}
