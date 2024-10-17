using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_HumanPlayer : MonoBehaviour
{
    public static ObservableList<RiskySandBox_HumanPlayer> all_instances = new ObservableList<RiskySandBox_HumanPlayer>();

    [SerializeField] RiskySandBox_HumanPlayer_DedicatedServerCommands my_DedicatedServerCommands { get { return GetComponent<RiskySandBox_HumanPlayer_DedicatedServerCommands>();} }


    public static event Action<RiskySandBox_HumanPlayer,string> OnconfirmFromUI;
    public static event Action<RiskySandBox_HumanPlayer,string> OnleftClick;
    
    public static event Action<RiskySandBox_HumanPlayer,string> Oncancel;

    //the space key is a shortcut for "confirm action"
    public static event Action<RiskySandBox_HumanPlayer,string> OnspaceKey;

    public static event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_my_Team_ID_STATIC;
    public static event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_deploy_target_STATIC;
    public static event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_attack_start_STATIC;
    public static event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_attack_target_STATIC;
    public static event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_fortify_start_STATIC;
    public static event Action<RiskySandBox_HumanPlayer> OnVariableUpdate_fortify_target_STATIC;


    public static RiskySandBox_HumanPlayer local_player
    {
        get
        {
            foreach(RiskySandBox_HumanPlayer _Player in RiskySandBox_HumanPlayer.all_instances)
            {
                if (_Player.is_local_player == true)
                    return _Player;
            }
            return null;
        }
    }

    public static RiskySandBox_Team local_player_Team
    {
        get
        {
            RiskySandBox_HumanPlayer _local_HumanPlayer = RiskySandBox_HumanPlayer.local_player;
            if (_local_HumanPlayer == null)
                return null;

            return _local_HumanPlayer.my_Team;

        }
    }


    [SerializeField] bool debugging;

    public ObservableString current_Item { get { return this.PRIVATE_current_Item; } }
    [SerializeField] ObservableString PRIVATE_current_Item;

    [SerializeField] Photon.Pun.PhotonView my_PhotonView;

    public bool is_mine { get { return my_PhotonView.IsMine; } }

    public RiskySandBox_Team my_Team { get { return RiskySandBox_Team.GET_RiskySandBox_Team(PRIVATE_my_Team_ID.value); } }
    public ObservableInt my_Team_ID { get { return PRIVATE_my_Team_ID; } }
    [SerializeField] ObservableInt PRIVATE_my_Team_ID;


    //note sometimes you want to absolutely rampage across the map...
    //e.g. at the end of the game you  want to just blast straight through anything in your path and send all surviving troops into the newly captured region...
    [SerializeField] ObservableString control_scheme;
    public ObservableString current_attack_method { get { return this.PRIVATE_current_attack_method; } }
    [SerializeField] ObservableString PRIVATE_current_attack_method;

    /// <summary>
    /// the tile this HumanPlayer is trying to "deploy" to
    /// </summary>
    public RiskySandBox_Tile deploy_target
    {
        get { return this.PRIVATE_deploy_target; }
        set
        {
            if(this != RiskySandBox_HumanPlayer.local_player)
            {
                if (this.debugging)
                    GlobalFunctions.print("not the local player... returning", this);
                return;
            }

            if (this.PRIVATE_deploy_target != null)
                this.PRIVATE_deploy_target.is_deploy_target.value = false;

            if(value == null)
            {
                //fine just allow this and return
                this.PRIVATE_deploy_target = null;
                OnVariableUpdate_deploy_target_STATIC?.Invoke(this);
                return;
            }

            bool _can_deploy = my_Team.canDeploy(value, 1);//TODO - magic 1
            if (_can_deploy == false)
                return;

            this.PRIVATE_deploy_target = value;
            value.is_deploy_target.value = true;

            OnVariableUpdate_deploy_target_STATIC?.Invoke(this);

        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_deploy_target;

    public RiskySandBox_Tile attack_start
    {
        get { return PRIVATE_attack_start;}
        set
        {
            if (this != RiskySandBox_HumanPlayer.local_player)
            {
                if (this.debugging)
                    GlobalFunctions.print("not the local player... returning", this);
                return;
            }

            if (PRIVATE_attack_start != null)
                PRIVATE_attack_start.is_attack_start.value = false;

            if(value == null)
            {
                this.PRIVATE_attack_start = null;
                OnVariableUpdate_attack_start_STATIC?.Invoke(this);
                return;
            }
            //make sure it is one of my tiles...
            if (value.my_Team != this.my_Team)
                return;

            if (value.num_troops <= RiskySandBox_Tile.min_troops_per_Tile)
                return;

            this.PRIVATE_attack_start = value;
            value.is_attack_start.value = true;

            OnVariableUpdate_attack_start_STATIC?.Invoke(this);
        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_attack_start;

    public RiskySandBox_Tile attack_target
    {
        get { return this.PRIVATE_attack_target; }
        set
        {
            if (this != RiskySandBox_HumanPlayer.local_player)
            {
                if (this.debugging)
                    GlobalFunctions.print("not the local player... returning", this);
                return;
            }

            if (PRIVATE_attack_target != null)
                PRIVATE_attack_target.is_attack_target.value = false;

            if(value == null)
            {
                this.PRIVATE_attack_target = null;
                OnVariableUpdate_attack_target_STATIC?.Invoke(this);
                return;
            }

            if(this.my_Team == null)
            {
                if (this.debugging)
                    GlobalFunctions.print("this.my_Team == null... returning",this);
                return;
            }


            bool _can_attack = this.my_Team.canAttack(this.attack_start, value,1,this.current_attack_method);//TODO - magic 1
            if (_can_attack == false)
                return;

            this.PRIVATE_attack_target = value;
            value.is_attack_target.value = true;

            OnVariableUpdate_attack_target_STATIC?.Invoke(this);
        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_attack_target;

    public RiskySandBox_Tile fortify_start
    {
        get { return PRIVATE_fortify_start; }
        set
        {
            if (this != RiskySandBox_HumanPlayer.local_player)
            {
                if (this.debugging)
                    GlobalFunctions.print("not the local player... returning", this);
                return;
            }

            if (PRIVATE_fortify_start != null)
                PRIVATE_fortify_start.is_fortify_start.value = false;

            if(value == null)
            {
                this.PRIVATE_fortify_start = null;
                OnVariableUpdate_fortify_start_STATIC?.Invoke(this);
                return;
            }
            if (value.my_Team != this.my_Team)
                return;

            if (value.num_troops <= RiskySandBox_Tile.min_troops_per_Tile)
                return;

            this.PRIVATE_fortify_start = value;
            value.is_fortify_start.value = true;

            OnVariableUpdate_fortify_start_STATIC?.Invoke(this);
        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_fortify_start;

    public RiskySandBox_Tile fortify_target
    {
        get { return this.PRIVATE_fortify_target; }
        set
        {
            if (this != RiskySandBox_HumanPlayer.local_player)
            {
                if (this.debugging)
                    GlobalFunctions.print("not the local player... returning", this);
                return;
            }

            if (PRIVATE_fortify_target != null)
                PRIVATE_fortify_target.is_fortify_target.value = false;

            if(value == null)
            {
                this.PRIVATE_fortify_target = null;
                OnVariableUpdate_fortify_target_STATIC?.Invoke(this);
                return;
            }

            if(my_Team == null)
            {
                if (this.debugging)
                    GlobalFunctions.print("this.my_Team == null",this);
                return;
            }

            bool _can_fortify = my_Team.canFortify(this.fortify_start, value, 1);
            if (_can_fortify == false)
                return;

            this.PRIVATE_fortify_target = value;
            value.is_fortify_target.value = true;

            OnVariableUpdate_fortify_target_STATIC?.Invoke(this);
        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_fortify_target;

    public ObservableString multiplayer_name { get { return this.PRIVATE_multiplayer_name; } }
    [SerializeField] ObservableString PRIVATE_multiplayer_name;

    public ObservableBool is_local_player { get { return this.PRIVATE_is_local_player; } }
    [SerializeField] ObservableBool PRIVATE_is_local_player;



    public static RiskySandBox_Team GET_RiskySandBox_Team(RiskySandBox_HumanPlayer _HumanPlayer)
    {
        foreach(RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
        {
            if (_Team.ID.value == _HumanPlayer.my_Team_ID.value)
                return _Team ;
        }
        return null;
    }

    public static RiskySandBox_Team GET_RiskySandBox_Team(Photon.Realtime.Player _Player)
    {
        foreach(RiskySandBox_HumanPlayer _HumanPlayer in RiskySandBox_HumanPlayer.all_instances)
        {
            if(_HumanPlayer.my_PhotonView.Owner == _Player)
            {
                return GET_RiskySandBox_Team(_HumanPlayer);
            }
        }
        return null;
    }


    private void Awake()
    {
        RiskySandBox_HumanPlayer.all_instances.Add(this);
        my_PhotonView = GetComponent<Photon.Pun.PhotonView>();
        my_Team_ID.OnUpdate += delegate { OnVariableUpdate_my_Team_ID_STATIC?.Invoke(this); };

        RiskySandBox_Team.OnVariableUpdate_purchased_shop_items_STATIC += OnVariableUpdate_purchased_items_STATIC;

    }

    private void Start()
    {
        if(MultiplayerBridge_Mirror.is_enabled)
        {
            if (this.GetComponent<Mirror.NetworkIdentity>().isOwned)
            {
                this.is_local_player.value = true;
                this.multiplayer_name.value = MultiplayerBridge_Mirror.NickName.value;
            }
        }
    }

    private void OnDestroy()
    {
        RiskySandBox_HumanPlayer.all_instances.Remove(this);

        RiskySandBox_Team.OnVariableUpdate_purchased_shop_items_STATIC -= OnVariableUpdate_purchased_items_STATIC;
    }


    void OnVariableUpdate_purchased_items_STATIC(RiskySandBox_Team _Team)
    {
        if (_Team != this.my_Team)
        {
            if (this.debugging)
                GlobalFunctions.print("wasnt my_Team", this);
            return;
        }

        if (this.PRIVATE_current_Item == "")
        {
            if (this.debugging)
                GlobalFunctions.print("no current_Item...returning", this);
            return;
        }

        int _current_item_count = _Team.purchased_shop_items.CountOf(this.PRIVATE_current_Item);

        if (_current_item_count == 0)
            this.current_Item.value = "";

    }






    private void Update()
    {
        if (this.is_local_player == false)
        {
            if (this.debugging)
                GlobalFunctions.print("is_local_player == false... returning", this);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            if (this.deploy_target != null || this.attack_start != null || this.attack_target != null || this.fortify_start != null || this.fortify_target != null)
                cancel();
            else
                RiskySandBox_MainGame.instance.show_escape_menu.value = !RiskySandBox_MainGame.instance.show_escape_menu.value;
        }


        //if we left click on a tile?
        if (Input.GetMouseButtonDown(0))
        {
            if (my_Team != null)
                OnleftClick?.Invoke(this, my_Team.current_turn_state);
        }

        if(Input.GetKeyDown(KeyCode.Space) == true)
        {

            //if i currently have an item???
            if(this.PRIVATE_current_Item != "")
            {
                //try to use the item....
                RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;
                if (_current_Tile != null)
                {
                    this.TRY_useItem(_current_Tile,this.PRIVATE_current_Item);

                }
            }
            else
            {
                if (my_Team != null)
                    OnspaceKey?.Invoke(this, my_Team.current_turn_state);
            }

        }


    }

 


    public void cancel()
    {
        this.deploy_target = null;
        this.attack_start = null;
        this.attack_target = null;
        this.fortify_start = null;
        this.fortify_target = null;

        Oncancel?.Invoke(this,this.my_Team.current_turn_state);
    }

    public void confirmFromUI()
    {
        if (debugging)
            GlobalFunctions.print("current_turn_state = " + my_Team.current_turn_state,this);

        OnconfirmFromUI?.Invoke(this,this.my_Team.current_turn_state);

    }

    public void TRY_nextState()
    {
        if (MultiplayerBridge_PhotonPun.in_room)
            my_PhotonView.RPC("ClientInvokedRPC_goToNextTurnState", RpcTarget.MasterClient);//ask the MasterClient to put me into the next turn state....
        else if (MultiplayerBridge_Mirror.is_enabled)
            my_DedicatedServerCommands.TRY_nextState();
        else
            GlobalFunctions.printError("not connected?",this);
    }

    //rpc to allow the player to move into the next "turn state"
    [PunRPC]
    void ClientInvokedRPC_goToNextTurnState(PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
            return;

        my_Team.TRY_goToNextTurnState();
    }


    [PunRPC]
    void ClientInvokedRPC_tradeInTerritoryCards(int[] _card_IDs,string _trade_mode, PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code.value == false || _PhotonMessageInfo.Sender != this.my_PhotonView.Owner)
        {
            if (this.debugging)
                GlobalFunctions.print("not the server or the sender wasnt the owner of this PhotonView...",this);
            return;
        }
        if (this.debugging)
            GlobalFunctions.print("asking the MainGame to call TRY_tradeInCards...", this);

        //ask the maingame to "trade in" these cards....


        this.my_Team.TRY_tradeInCards(_card_IDs,_trade_mode);

        



    }

    //TODO - add in a quantity parameter (e.g. maybe you have a "sabotage" item that kills 1 troop on a tile....) but you can use multiple times... (there is no point sending 100 rpc to remove 1 troop 100 times???)
    public void TRY_useItem(RiskySandBox_Tile _Tile,string _item_type)
    {
        //e.g. try to use a
        if (this.debugging)
            GlobalFunctions.print("trying to use '" + _item_type + "' on the tile with ID = " + _Tile.ID.value + "' - sending rpc",this);
        //ask the server to send use the item on that tile for me
        if (MultiplayerBridge_PhotonPun.in_room)
            my_PhotonView.RPC("ClientInvokedRPC_useItem", PhotonNetwork.MasterClient, _Tile.ID.value, _item_type);
        else if (MultiplayerBridge_Mirror.is_enabled)
            my_DedicatedServerCommands.TRY_useItem(_Tile.ID.value, _item_type);
        else
            GlobalFunctions.printError("not connected?", this);
    }

    public void TRY_purchaseItem(string _item_type,int _amount)
    {
        if (this.debugging)
            GlobalFunctions.print(string.Format("trying to purchase {0}", _item_type),this);

        if (MultiplayerBridge_PhotonPun.in_room)
            my_PhotonView.RPC("ClientInvokedRPC_purchaseItem", RpcTarget.MasterClient, _item_type, _amount);
        else if (MultiplayerBridge_Mirror.is_enabled)
            this.my_DedicatedServerCommands.TRY_purchaseItem(_item_type, _amount);
        else
            GlobalFunctions.print("not connected?", this);
    }


    public void TRY_tradeInSelectedTerritoryCards(string _trade_mode)
    {
        if (this.debugging)
            GlobalFunctions.print("trying to trade in cards", this);//TODO print out which cards?

        List<RiskySandBox_TerritoryCard> _selected_Cards = new List<RiskySandBox_TerritoryCard>(RiskySandBox_TerritoryCard.all_instances_is_selected);

        int[] _selected_Card_IDs = _selected_Cards.Select(x => x.tile_ID_READONLY).ToArray();

        if (MultiplayerBridge_PhotonPun.in_room)
            my_PhotonView.RPC("ClientInvokedRPC_tradeInTerritoryCards", RpcTarget.MasterClient, _selected_Card_IDs, _trade_mode);
        else if (MultiplayerBridge_Mirror.is_enabled)
            this.my_DedicatedServerCommands.TRY_tradeInSelectedTerritoryCards(_selected_Card_IDs, _trade_mode);
        else
            GlobalFunctions.printError("not connected?", this);
    }


    public static RiskySandBox_HumanPlayer GET_RiskySandBox_HumanPlayer(RiskySandBox_Team _Team)
    {
        foreach(RiskySandBox_HumanPlayer _HumanPlayer in RiskySandBox_HumanPlayer.all_instances)
        {
            if (_HumanPlayer.my_Team_ID.value == _Team.ID)
            {
                return _HumanPlayer;
            }
                
        }
        return null;
    }

    public void TRY_joinTeam(RiskySandBox_Team _Team)
    {
        if (MultiplayerBridge_PhotonPun.in_room)
            my_PhotonView.RPC("ClientInvokedRPC_joinTeam", RpcTarget.MasterClient, _Team.ID.value);
        else if (MultiplayerBridge_Mirror.is_enabled)
            my_DedicatedServerCommands.TRY_joinTeam(_Team.ID.value);
        else
            GlobalFunctions.print("not connected?", this);
    }



    [PunRPC]
    public void ClientInvokedRPC_useItem(int _tile_ID,string _item_type,PhotonMessageInfo _PhotonMessageInfo)
    {
        if(PrototypingAssets.run_server_code.value == false)
        {
            GlobalFunctions.printError("not the server! why is this happening... returning",this);
            return;
        }
        if(_PhotonMessageInfo.Sender != my_PhotonView.Owner)
        {
            GlobalFunctions.printError("_PhotonMessageInfo.Sender != my_PhotonView.owner",this);
            return;
        }


        RiskySandBox_Tile _target_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_tile_ID);

        if (this.debugging)
            GlobalFunctions.print("calling my_Team.TRY_useItem _Tile_ID = " + _tile_ID + " _item_type = " + _item_type, this);


        my_Team.TRY_useItem(_target_Tile,_item_type);
    }

    [PunRPC]
    void ClientInvokedRPC_purchaseItem(string _item_type,int _amount,PhotonMessageInfo _PhotonMessageInfo)
    {
        if(PrototypingAssets.run_server_code.value == false)
        {
            if (this.debugging)
                GlobalFunctions.print("not the server... returning",this);
            return;
        }
        if(_PhotonMessageInfo.Sender != my_PhotonView.Owner)
        {
            if (this.debugging)
                GlobalFunctions.print("_PhotonMessageInfo.sender wasnt my_PhotonView.owner... returning", this);
            return;
        }

        RiskySandBox_Shop.instance.TRY_purchaseItem(this.my_Team, _item_type, _amount);
    }

    [PunRPC]
    void ClientInvokedRPC_joinTeam(int _team_ID,PhotonMessageInfo _PhotonMessageInfo)
    {
        if (PrototypingAssets.run_server_code == false || _PhotonMessageInfo.Sender != my_PhotonView.Owner)
        {
            if (this.debugging)
                GlobalFunctions.print("not the server or the sender wasnt the owner of this PhotonView...", this);
            return;
        }

        RiskySandBox_Team _Team = RiskySandBox_Team.GET_RiskySandBox_Team(_team_ID);
        //TODO - if the team is null... print wtf?!?!?! - or just return (but probably? something is going wrong for this player???)
        RiskySandBox_HumanPlayer _HumanPlayer = RiskySandBox_HumanPlayer.GET_RiskySandBox_HumanPlayer(_Team);

        if(_HumanPlayer != null)
        {
            //dont allow this player to switch to this team...
            if (this.debugging)
                GlobalFunctions.print("there is already a HumanPlayer with this Team_ID... _team_ID = "+_team_ID,this);
            return;

        }
        //TODO - dont allow if ai team???

        if (RiskySandBox_MainGame.game_started)
        {
            if (this.debugging)
                GlobalFunctions.print("the game is started so HumanPlayer can't switch teams (this is not great? because of rejoining???)",this);
            return;
            //DONT allow players to switch teams halfway through!
            //TODO well actually how does rejoining the game
        }

        if(this.debugging)
            GlobalFunctions.print("setting this HumanPlayer.my_Team_ID to " + _team_ID, this);
        

        this.my_Team_ID.value = _team_ID;
        

    }






}
