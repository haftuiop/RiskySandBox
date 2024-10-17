using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{
    public static event Action OnstartGame_MultiplayerBridge;




   

    public void startGame()
    {

        if (PrototypingAssets.run_server_code.value == false)
        {
            if (this.debugging)
                GlobalFunctions.print("not the server - returning", this);
            return;
        }

        RiskySandBox_Map.instance.loadMap(this.map_ID.value);//load the current map...

       



    }


    void EventReceiver_OnloadMapCompleted()
    {
        
        if (RiskySandBox_LevelEditor.is_enabled == true)//so actually the level editor probably loaded a map... so the main game should NOT DO ANYTHING
            return;

        if (RiskySandBox_ReplaySystem.is_enabled == true)//ok so what is probably happening is the replay system is trying to run through the game...
            return;

        OnstartGame_MultiplayerBridge?.Invoke();




        foreach (RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
        {
            _Team.current_turn_state.value = RiskySandBox_Team.turn_state_waiting;
        }



        Dictionary<int, List<int>> _graph = RiskySandBox_Map.GET_graph(null);
        //TODO - this createStablePortals(n)
        List<RiskySandBox_Tile> _stable_portals = RiskySandBox_PortalFunctions.selectStartOfGameStablePortals(_graph, (int)n_stable_portals);

        foreach (RiskySandBox_Tile _portal_Tile in _stable_portals)
        {
            _portal_Tile.has_stable_portal.value = true;
        }





        //TODO - this.createInitialBlizards() or createBlizards(n)... so that way as time goes on we can "shrink" the map with more and more blizards to force the endgame...

        _graph = RiskySandBox_Map.GET_graph(null);

        List<RiskySandBox_Tile> _blizard_selection = RiskySandBox_Tile.GET_RiskySandBox_Tiles(RiskySandBox_BlizardFunctions.selectStartOfGameBlizard(_graph,this.n_blizards).ToList());

        foreach(RiskySandBox_Tile _Tile in _blizard_selection)
        {
            _Tile.has_blizard.value = true;
        }







        //TODO - this.createInitialUnStablePortals()
        for (int _up = 0; _up < PRIVATE_n_unstable_portals; _up += 1)
        {
            Debug.LogWarning("WARNING - unstable portals are unimplemented...");
        }


        RiskySandBox_UnorganisedFunctions.distributeStartOfGameTroops();

        RiskySandBox_BattleLog.instance.EventReceiver_OnInitialPlacementComplete();



        ///the game has now officially begun!
        RiskySandBox_MainGame.game_started.value = true;


        RiskySandBox_MainGame_CapitalsMode.instance.startGame();//note capitals mode should switch control back to the main game (even if all teams dont actually start with capitals...)


    }

    public void EventReceiver_OncapitalsModeSetupComplete()
    {
        //well surely we know this is just the Team with the ID = 0 (or whatever the first id is...)
        RiskySandBox_Team _next_Team = GET_nextTeam(null);
        _next_Team.startTurn();
    }






}
