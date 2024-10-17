using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Mirror;

public partial class RiskySandBox_DedicatedServer : MonoBehaviour
{

    public static RiskySandBox_DedicatedServer instance { get; private set; }


    [SerializeField] bool debugging;
    [SerializeField] List<GameObject> PRIVATE_spawn_GameObjects = new List<GameObject>();
    


    public static bool is_dedicated_server_build { get { return PrototypingAssets.is_dedicated_server.value; } }



    private void Awake()
    {
        instance = this;
        MultiplayerBridge_Mirror.OnplayerDisconnect += EventReceiver_OnplayerDisconnect;
        MultiplayerBridge_Mirror.OnplayerConnected += EventReceiver_OnplayerConnected;
    }


    private void Start()
    {
        if (is_dedicated_server_build == false)
        {

            if (this.debugging)
                GlobalFunctions.print("is_dedicated_server == false just returning...", this);

            return;


        }


        if (this.debugging)
            GlobalFunctions.print("is_dedicated_server == true... calling initialize", this);

        PrototypingAssets.run_server_code.OnUpdate_true += EventReceiver_OnstartServer;
        RiskySandBox_MainMenu.Onenable += EventReceiver_OnreturnToMainMenu;


        Initialize();


    }

    private void Initialize()
    {
        //ok first things first...

        if (this.debugging)
            GlobalFunctions.print("calling MultiplayerBridge_Mirror.instance.startServer()",this);

        MultiplayerBridge_Mirror.instance.startServer();
    }

    void EventReceiver_OnstartServer(ObservableBool _is_server)
    {
        // ok! we wait for a player to connect...
        //then once they do we can tell them we are a dedicated server!
        //once this happens the player can decide what type of gamemode we are going to run...
        //once this happens we can play the main game!

        foreach(GameObject _GameObject in this.PRIVATE_spawn_GameObjects)
        {
            if (_GameObject == null)
                continue;

            Mirror.NetworkServer.Spawn(_GameObject);
        }





    }

    void EventReceiver_OnreturnToMainMenu()
    {
        RiskySandBox_Team.destroyAllTeams();

    }

    void EventReceiver_OnplayerConnected(NetworkConnectionToClient _connection)
    {
        if (MultiplayerBridge_Mirror.n_players > 1)
            return;


        //right what we want to say here is "ok player what gamemode do you want?"
        //the player should select the gamemode... then we start the game for reals...

        RiskySandBox_GameLobby.instance.createTeams();
        print("server started! there are " + RiskySandBox_Team.all_instances.Count + " teams!");
    }

    void EventReceiver_OnplayerDisconnect(NetworkConnectionToClient _connection)
    {
        //if we have 0 players?
        int _n_players = MultiplayerBridge_Mirror.n_players;

        if (_n_players > 0)
            return;

        if (this.debugging)
            GlobalFunctions.print("ending the game (there are no players connected...)",this);

        //TODO - well no? actually we are potentially going to have to end the game via the ai?
        


        RiskySandBox_MainMenu.instance.returnToMainMenu();
        


    }


}
