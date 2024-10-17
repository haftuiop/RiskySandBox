using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_UnorganisedFunctions : MonoBehaviour
{
    [SerializeField] bool debugging;
	

    public static void distributeStartOfGameTroops()
    {

        //TODO - remove the blizard tiles?
        List<RiskySandBox_Tile> _remaining_options = new List<RiskySandBox_Tile>(RiskySandBox_Tile.all_instances);

        foreach (RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
        {
            int[] _distribution_values = _Team.troop_distribution_startGame.value.Split(",").Select(x => int.Parse(x)).ToArray();
           

            foreach (int _int in _distribution_values)
            {
                RiskySandBox_Tile _random_Tile = GlobalFunctions.GetRandomItem(_remaining_options);//TODO - dont allow the blizard tiles...

                _remaining_options.Remove(_random_Tile);

                _random_Tile.my_Team_ID.value = _Team.ID;
                _random_Tile.num_troops.value = _int;

            }
        }

        if (RiskySandBox_NeutralTileSettings.enable_neutral_Tiles == true)
        {
            int _distribution_index = 0;
            int[] _distribution_values = RiskySandBox_NeutralTileSettings.n_troops_startGame.value.Split(",").Select(x => int.Parse(x)).ToArray();

            foreach (RiskySandBox_Tile _Tile in RiskySandBox_Tile.all_instances.Where(x => x.my_Team_ID == PrototypingAssets_Tile.null_ID))
            {
                _Tile.num_troops.value += _distribution_values[_distribution_index];

                _distribution_index += 1;

                if (_distribution_index >= _distribution_values.Count())
                    _distribution_index = 0;
            }
             
        }



    }


}
