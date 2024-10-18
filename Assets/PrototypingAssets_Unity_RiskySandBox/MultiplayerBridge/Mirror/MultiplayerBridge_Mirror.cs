using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Mirror;

public partial class MultiplayerBridge_Mirror : NetworkManager
{

    public static MultiplayerBridge_Mirror instance;
    public static ObservableBool is_enabled { get { return instance.PRIVATE_is_enabled; } }

    /// <summary>
    /// invoked whenever a new player joins the game...
    /// </summary>
    public static event Action<NetworkConnectionToClient> OnplayerConnected;

    /// <summary>
    /// invoked whenever a player "disconnects" from the server (only on the server)
    /// </summary>
    public static event Action<NetworkConnectionToClient> OnplayerDisconnect;

    public static int n_players { get { return NetworkServer.connections.Count; } }

    /// <summary>
    /// what would the player like to be called e.g. on a ui (note this SHOULD be completley and utterly useless in terms of authentication/anti cheat. the user should be able to set this to whatever they want without any issues
    /// </summary>
    public static ObservableString NickName { get { return instance.PRIVATE_NickName; } }

    [SerializeField] bool debugging;

    [SerializeField] ObservableString PRIVATE_connect_ip_address;
    [SerializeField] ObservableBool PRIVATE_is_enabled;
    [SerializeField] ObservableString PRIVATE_NickName;

    
    



    public override void Awake()
    {
        base.Awake();
        instance = this;

        this.PRIVATE_connect_ip_address.OnUpdate += EventReceiver_OnVariableUpdate_connect_ip_address;

    }

    void EventReceiver_OnVariableUpdate_connect_ip_address(ObservableString _connect_ip_address)
    {
        this.networkAddress = _connect_ip_address;
    }

    public void startServer()
    {
        base.StartServer();
    }

    public void startClient(string _ip_address)
    {
        this.networkAddress = _ip_address;
        base.StartClient();
    }

    public void startClient(string _ip_address,int _port)
    {
        Debug.LogError("unimplemented... how do we get the port???");
    }

    /// <summary>
    /// stops the client (and server)
    /// </summary>
    public void shutdown()
    {
        if (this.debugging)
            GlobalFunctions.print("called shutdown...", this);
        if(PrototypingAssets.run_client_code == true)
            this.StopClient();
        
        if (PrototypingAssets.run_server_code == true)
            this.StopServer();
    }


    public override void OnStartServer()
    {
        base.OnStartServer();

        if (this.debugging)
            GlobalFunctions.print("server has started...",this);

        

        this.PRIVATE_is_enabled.value = true;

        PrototypingAssets.run_server_code.value = true;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        if (this.debugging)
            GlobalFunctions.print("server has stopped...", this);

        

        this.PRIVATE_is_enabled.value = false;

        PrototypingAssets.run_server_code.value = false; 
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (this.debugging)
            GlobalFunctions.print("client has started...",this);

       
        PRIVATE_is_enabled.value = true;
        PrototypingAssets.run_client_code.value = true;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (this.debugging)
            GlobalFunctions.print("client has stopped...", this);

        
        PRIVATE_is_enabled.value = false;
        PrototypingAssets.run_client_code.value = false;

    }




    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        if (this.debugging)
            GlobalFunctions.print("new player connected invoking OnplayerConnected", this);

        MultiplayerBridge_Mirror.OnplayerConnected?.Invoke(conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        if (this.debugging)
            GlobalFunctions.print("a player just disconnected...",this);

        MultiplayerBridge_Mirror.OnplayerDisconnect?.Invoke(conn);
    }



}
