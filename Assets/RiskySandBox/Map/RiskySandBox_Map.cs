using System.Collections;using System.Collections.Generic;using System.Linq;using System;using System.IO;


using UnityEngine;
#if UNITY_WEBGL
using UnityEngine.Networking;
#endif

public partial class RiskySandBox_Map : MonoBehaviour
{
    public static RiskySandBox_Map instance { get; private set; }


    [SerializeField] bool debugging;


    private void Awake()
    {
        if (this.debugging)
            GlobalFunctions.print("called Awake... setting instance to be this", this);
        instance = this;
    }

    private void OnDestroy()
    {
        if (this.debugging)
            GlobalFunctions.print("destroying the map...",this);
    }


    public static Dictionary<int,List<int>> GET_graph(RiskySandBox_Team _Team)
    {
        //ok! lets run through all the tiles...

        Dictionary<int, List<int>> _graph = new Dictionary<int, List<int>>();

        foreach(RiskySandBox_Tile _Tile in RiskySandBox_Tile.all_instances)
        {
            List<int> _connections_IDs = _Tile.graph_connections.Select(x => x.ID.value).ToList();

            _graph[_Tile.ID.value] = _connections_IDs;
        }



        return _graph;

    }






}
