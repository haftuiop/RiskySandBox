using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team
{

    public static event Action<RiskySandBox_Team, RiskySandBox_Tile> OnplaceCapital;

    public bool canPlaceCaptital(RiskySandBox_Tile _Tile)
    {
        if(_Tile == null)
        {
            GlobalFunctions.printError("why is _Tile null...", this);
            return false;
        }

        if (_Tile.has_capital == true)
        {
            if (this.debugging)
                GlobalFunctions.print("_Tile.has_capital == true... returning false...", this);
            return false;
        }

        if(_Tile.my_Team != this)//TODO - well really what if we allow the teams to place capitals onto allies??? (or neutrals???)
        {
            if (this.debugging)
                GlobalFunctions.print("_Tile.my_Team != this... - returning false...", this, _Tile);
            return false;
        }

        return true;
    }

    public bool TRY_placeCapital(RiskySandBox_Tile _Tile)
    {
        bool _can_place = this.canPlaceCaptital(_Tile);//make sure i can actually place the capital on this tile...

        if(_can_place == false)//if I can't???
        {
            GlobalFunctions.print("canPlaceCapital returned false...",this);
            return false;
        }

        return this.placeCapital(_Tile);
    }

    public bool TRY_placeCapital_random()
    {

        List<RiskySandBox_Tile> _candidates = RiskySandBox_Tile.all_instances.Where(x => canPlaceCaptital(x) == true).ToList();

        if (_candidates.Count <= 0)
        {
            if (this.debugging)
                GlobalFunctions.print("_candidates.Count <= 0... returning false...",this);
            return false;
        }

        RiskySandBox_Tile _random_Tile = GlobalFunctions.GetRandomItem(_candidates);

        if (this.debugging)
            GlobalFunctions.print("returning this.placeCapital(_random_Tile)",this);
        return placeCapital(_random_Tile);
    }

    bool placeCapital(RiskySandBox_Tile _Tile)
    {
        if(this.required_capital_placements.value > 0)
            this.required_capital_placements.value -= 1;

        _Tile.has_capital.value = true;
        _Tile.num_troops.value += this.capital_troop_generation;

        RiskySandBox_Team.OnplaceCapital?.Invoke(this, _Tile);

        return true;
    }
}
