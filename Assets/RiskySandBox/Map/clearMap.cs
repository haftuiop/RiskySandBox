using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Map
{
    public static event Action OnclearMap;

    public void clearMap()
    {
        
        RiskySandBox_Tile.destroyAllTiles();
        RiskySandBox_Bonus.destroyAllBonuses();

        OnclearMap?.Invoke();
    }
}
