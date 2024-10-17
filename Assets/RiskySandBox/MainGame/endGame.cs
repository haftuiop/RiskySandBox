using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_MainGame
{

    public static event Action OnendGame_MultiplayerBridge;
    public static event Action OnendGame;
    public UnityEngine.Events.UnityEvent OnendGame_Inspector;





    public void endGame(string _debug_reason,RiskySandBox_Team _winner)
    {
        GlobalFunctions.print("ending the game: _debug_reason = '" + _debug_reason + "'",this);



        if (PrototypingAssets.run_server_code.value == true)
        {
            List<RiskySandBox_Team> _to_sort = new List<RiskySandBox_Team>(RiskySandBox_Team.all_instances);

            int _n_winners = 0;

            if (_winner != null)
            {

                _winner.game_over_placement.value = 0;
                _to_sort.Remove(_winner);

                _n_winners += 1;

                if (_winner.shared_victory == true)
                {
                    foreach (RiskySandBox_Team _ally in _winner.ally_Teams)
                    {
                        //make this a winner!
                        _ally.game_over_placement.value = 0;
                        _to_sort.Remove(_ally);

                        _n_winners += 1;
                    }
                }
            }

            //next we sort the remaining teams by their number of troops....
            //TODO - right we actually need to think about the alliances...
            //essentially we should merge up all the alliances number of troops...
            //then depending on who is allied with who they should share the victory...

            _to_sort = _to_sort.Where(x => x.defeated == false).OrderByDescending(x => x.n_troops).ToList();

            for (int i = 0; i < _to_sort.Count; i += 1)
            {
                _to_sort[i].game_over_placement.value = _n_winners + i;
            }

            //TODO - what if the team was a winner (or had a team mate who won???)
            List<RiskySandBox_Team> _defeated_Teams = RiskySandBox_Team.all_instances.Where(x => x.defeated == true).OrderByDescending(x => x.defeated_index.value).ToList();

            if (_defeated_Teams.Count > 0)
            {
                int _defeated_start = RiskySandBox_Team.all_instances.Max(x => x.game_over_placement.value) + 1;
                for (int i = 0; i < _defeated_Teams.Count(); i += 1)
                {
                    _defeated_Teams[i].game_over_placement.value = _defeated_start + i;
                }
            }
        }



        if (PrototypingAssets.run_server_code.value == true)
        {
            //end the game!

            OnendGame_MultiplayerBridge?.Invoke();
        }





        RiskySandBox_Map.instance.clearMap();

        OnendGame_Inspector.Invoke();
        OnendGame?.Invoke();




    }

}
