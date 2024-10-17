using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_PortalFunctions : MonoBehaviour
{

    public static List<RiskySandBox_Tile> selectStartOfGameStablePortals(Dictionary<int,List<int>> _graph,int _target_n_portals)
    {
        List<RiskySandBox_Tile> _return_value = new List<RiskySandBox_Tile>();

        for(int i = 0; i < _target_n_portals; i += 1)
        {
            List<RiskySandBox_Tile> _current_options = RiskySandBox_Tile.all_instances.Where(x => (_return_value.Contains(x) == false) && canBecomeStablePortal(x,_return_value)).ToList();

            if (_current_options.Count <= 0)
                return _return_value;

            RiskySandBox_Tile _random = GlobalFunctions.GetRandomItem(_current_options);
            _return_value.Add(_random);
        }

        return _return_value;
    }

    /// <summary>
    /// returns a list of all the "Tiles" that can have a stable portal on it...
    /// </summary>
    public static List<RiskySandBox_Tile> GET_stablePortalCandidates()
    {
        return new List<RiskySandBox_Tile>(RiskySandBox_Tile.all_instances.Where(x => canBecomeStablePortal(x)));
    }


    /// <summary>
    /// can this 'Tile' become a portal
    /// </summary>
    private static bool canBecomeStablePortal(RiskySandBox_Tile _Tile,List<RiskySandBox_Tile> _starting_portals)
    {
        if (_Tile.has_stable_portal == true)
            return false;
        if (_Tile.has_blizard == true)
            return false;

        foreach (int _ID in _Tile.graph_connections_IDs)
        {
            RiskySandBox_Tile _connection = RiskySandBox_Tile.GET_RiskySandBox_Tile(_ID);

            if (_connection.has_stable_portal == true || _starting_portals.Contains(_connection))
                return false;
        }

        return true;
    }

    /// <summary>
    /// can this 'Tile' become a portal
    /// </summary>
    public static bool canBecomeStablePortal(RiskySandBox_Tile _Tile)
    {
        if (_Tile.has_stable_portal == true)
            return false;
        if (_Tile.has_blizard == true)
            return false;

        foreach(int _ID in _Tile.graph_connections_IDs)
        {
            RiskySandBox_Tile _connection = RiskySandBox_Tile.GET_RiskySandBox_Tile(_ID);

            if (_connection.has_stable_portal == true)
                return false;
        }

        return true;
    }

    /// <summary>
    /// decides if the tile with the given id can get a stable portal on it
    /// </summary>
    public static bool canBecomeStablePortal(int _tile_ID)
    {
        RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_tile_ID);
        return canBecomeStablePortal(_Tile);
    }




}
