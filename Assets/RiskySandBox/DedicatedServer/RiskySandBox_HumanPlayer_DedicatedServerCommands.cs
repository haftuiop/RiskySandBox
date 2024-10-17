using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_DedicatedServerCommands : Mirror.NetworkBehaviour
{
    [SerializeField] bool debugging;

    RiskySandBox_HumanPlayer my_HumanPlayer { get { return GetComponent<RiskySandBox_HumanPlayer>(); } }
    RiskySandBox_Team my_Team { get { return my_HumanPlayer.my_Team; } }
	

    [Mirror.Command(requiresAuthority = true)]
    public void TRY_joinTeam(int _id)
    {

        RiskySandBox_Team _Team = RiskySandBox_Team.GET_RiskySandBox_Team(_id);
        //TODO - if the team is null... print wtf?!?!?! - or just return (but probably? something is going wrong for this player???)
        RiskySandBox_HumanPlayer _HumanPlayer = RiskySandBox_HumanPlayer.GET_RiskySandBox_HumanPlayer(_Team);

        if (_HumanPlayer != null)
        {
            //dont allow this player to switch to this team...
            if (this.debugging)
                GlobalFunctions.print("there is already a HumanPlayer with this Team_ID... _team_ID = " + _id, this);
            return;

        }
        //TODO - dont allow if ai team???

        if (RiskySandBox_MainGame.game_started)
        {
            if (this.debugging)
                GlobalFunctions.print("the game is started so HumanPlayer can't switch teams (this is not great? because of rejoining???)", this);
            return;
            //DONT allow players to switch teams halfway through!
            //TODO well actually how does rejoining the game
        }

        if (this.debugging)
            GlobalFunctions.print("setting this HumanPlayer.my_Team_ID to " + _id, this);


        my_HumanPlayer.my_Team_ID.value = _id;
    }

    [Mirror.Command(requiresAuthority = true)]
    public void TRY_startGame()
    {
        if (this.debugging)
            GlobalFunctions.print("client called TRY_startGame... checking", this);
        //TODO - eventually remove the enitre function... the server should start itself when its ready to...
        RiskySandBox_MainGame.instance.startGame();
    }

    [Mirror.Command(requiresAuthority = true)]
    public void TRY_nextState()
    {
        if (this.debugging)
            GlobalFunctions.print("client called TRY_nextState... checking",this);

        my_Team.TRY_goToNextTurnState();
    }

    [Mirror.Command(requiresAuthority = false)]
    public void TRY_purchaseItem(string _item_type,int _amount)
    {
        RiskySandBox_Shop.instance.TRY_purchaseItem(this.my_Team, _item_type, _amount);
    }

    [Mirror.Command(requiresAuthority = false)]
    public void TRY_useItem(int _tile_ID,string _item_type)
    {
        RiskySandBox_Tile _target_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_tile_ID);
        my_Team.TRY_useItem(_target_Tile, _item_type);
    }

    [Mirror.Command(requiresAuthority = false)]
    public void TRY_tradeInSelectedTerritoryCards(int[] _card_ids, string _trade_mode)
    {
        my_Team.TRY_tradeInCards(_card_ids, _trade_mode);
    }


    [Mirror.Command(requiresAuthority = false)]
    public void TRY_deploy(int _Tile_ID,int _n_troops)
    {
        RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_Tile_ID);

        my_HumanPlayer.my_Team.TRY_deploy(_Tile, _n_troops);

    }

    [Mirror.Command(requiresAuthority = true)]
    public void TRY_attack(int _from_ID, int _to_ID, int _n_troops, string _attack_method)
    {
        RiskySandBox_Tile _from = RiskySandBox_Tile.GET_RiskySandBox_Tile(_from_ID);
        RiskySandBox_Tile _to = RiskySandBox_Tile.GET_RiskySandBox_Tile(_to_ID);

        this.my_Team.TRY_attack(_from, _to, _n_troops, _attack_method);
    }

    [Mirror.Command(requiresAuthority = true)]
    public void TRY_capture(int _n_troops)
    {
        my_Team.TRY_capture(_n_troops);
    }

    [Mirror.Command(requiresAuthority = true)]
    public void TRY_fortify(int _from_ID,int _to_ID, int _n_troops)
    {
        RiskySandBox_Tile _from = RiskySandBox_Tile.GET_RiskySandBox_Tile(_from_ID);
        RiskySandBox_Tile _to = RiskySandBox_Tile.GET_RiskySandBox_Tile(_to_ID);

        my_Team.TRY_fortify(_from, _to, _n_troops);
    }



}
