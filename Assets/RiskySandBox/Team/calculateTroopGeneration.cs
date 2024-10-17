using System.Collections;using System.Collections.Generic;using System.Linq;using System;


public partial class RiskySandBox_Team
{

    int calculateTroopGeneration()
    {
        List<RiskySandBox_Tile> _my_Tiles = new List<RiskySandBox_Tile>(this.my_Tiles);


        int _natural_generation = this.calculateNaturalGeneration(_my_Tiles);
        //TODO - clamp this between the min and max "natural geneation values"

        //TODO - clamp to "max_generation_from_bonuses"/"min_generation_from_bonuses"
        int _from_bonus = calculateGenerationFromBonuses(_my_Tiles);


        //TODO - maybe if you have a "leader" for the team (like civilisation video games) you can have more troops????


        if (debugging)
        {
            string _debug_string = string.Format("_natural_generation {0}, _from_bonus {1}", _natural_generation, _from_bonus);
            GlobalFunctions.print(_debug_string, this);
        }
            


        //TODO - min/max_generation
        return _natural_generation + _from_bonus;//TODO + _Team.bonus_generation e.g. maybe some people get different generation than others...
    }

    int calculateNaturalGeneration(List<RiskySandBox_Tile> _my_Tiles)
    {
        //TODO allow the user to play around with these values...
        int _natural_generation = Math.Max(3, UnityEngine.Mathf.FloorToInt(_my_Tiles.Count() / 3f));
        return _natural_generation;
    }


    int calculateGenerationFromBonuses(List<RiskySandBox_Tile> _Tiles)
    {
        int _return_value = 0;
        List<int> _Tiles_IDs = _Tiles.Select(x => x.ID.value).ToList();
        foreach (RiskySandBox_Bonus _Bonus in RiskySandBox_Bonus.all_instances)//go through every bonus...
        {

            bool _has_bonus = true;
            //if they are missing any of the tiles?
            foreach (int _required_ID in _Bonus.tile_IDs)//go through each of the IDs that the Team needs in order to have this bonus
            {
                if (_Tiles_IDs.Contains(_required_ID) == false)//if they don't have this tile id...
                {
                    if (debugging)
                        GlobalFunctions.print("checking if the Team has the '" + _Bonus.name + "' bonus - answer is no as they dont control the tile with id " + _required_ID, this);
                    _has_bonus = false;
                    break;
                }
            }
            if (_has_bonus == true)//great! - give them the bonus troops...
                _return_value += _Bonus.generation;
        }

        return _return_value;//TODO - * '_Team.bonus_generation_multiplier'  for now lets keep it simple...
    }
}
