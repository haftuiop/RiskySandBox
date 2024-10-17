using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;


/// <summary>
/// functions to implement the "blizards" (objects that "block" certain tiles from being passable creating "choke points" and "corners")
/// </summary>
public partial class RiskySandBox_BlizardFunctions : MonoBehaviour
{
    public static string credits = "Wouter Lambrecht";

    Dictionary<int, List<int>> test_map = new Dictionary<int, List<int>>
        {
            { 1, new List<int> { 2, 3, 5 } },
            { 2, new List<int> { 1, 3, 4 } },
            { 3, new List<int> { 1, 2, 4 } },
            { 4, new List<int> { 2, 3 } },
            { 5, new List<int> { 1, 6, 7 } },
            { 6, new List<int> { 11, 10, 9, 8, 7, 5 } },
            { 7, new List<int> { 5, 6, 8, 16 } },
            { 8, new List<int> { 6, 7, 9, 17, 16 } },
            { 9, new List<int> { 10, 6, 8, 17 } },
            { 10, new List<int> { 9, 6, 11, 14, 15 } },
            { 11, new List<int> { 12, 13, 14, 10, 6 } },
            { 12, new List<int> { 11, 13 } },
            { 13, new List<int> { 11, 12, 14, 15, 39 } },
            { 14, new List<int> { 11, 13, 15, 10 } },
            { 15, new List<int> { 10, 13, 14 } },
            { 16, new List<int> { 7, 8, 17, 23, 24, 26 } },
            { 17, new List<int> { 8, 9, 16, 23, 21, 18 } },
            { 18, new List<int> { 19, 20, 21, 17 } },
            { 19, new List<int> { 18, 20, 37 } },
            { 20, new List<int> { 18, 19, 21, 22 } },
            { 21, new List<int> { 17, 18, 20, 22, 23 } },
            { 22, new List<int> { 20, 21, 23, 25 } },
            { 23, new List<int> { 16, 17, 21, 22, 24, 25 } },
            { 24, new List<int> { 16, 23, 25, 26 } },
            { 25, new List<int> { 22, 23, 24, 26, 29, 30 } },
            { 26, new List<int> { 16, 24, 25, 27, 28, 29 } },
            { 27, new List<int> { 26, 28 } },
            { 28, new List<int> { 26, 27, 29 } },
            { 29, new List<int> { 25, 26, 28 } },
            { 30, new List<int> { 25, 31, 32, 33 } },
            { 31, new List<int> { 30, 32 } },
            { 32, new List<int> { 30, 31, 33 } },
            { 33, new List<int> { 30, 32, 34 } },
            { 34, new List<int> { 33, 35, 41 } },
            { 35, new List<int> { 34, 36, 41, 42 } },
            { 36, new List<int> { 35, 37, 42 } },
            { 37, new List<int> { 19, 36, 38, 42 } },
            { 38, new List<int> { 37, 39, 40, 42 } },
            { 39, new List<int> { 13, 40, 38 } },
            { 40, new List<int> { 41, 42, 38, 39 } },
            { 41, new List<int> { 40, 42, 35, 34 } },
            { 42, new List<int> { 35, 36, 37, 38, 40, 41 } }
        };



    private void Start()
    {
        Debug.Log(string.Join(", ", RiskySandBox_BlizardFunctions.GET_blizardCandidates(test_map)));
        Debug.Log(string.Join(", ", RiskySandBox_BlizardFunctions.selectStartOfGameBlizard(test_map, 6)));
    }


    /// <summary>
    /// create the blizards for the start of the game
    /// </summary>
    public static List<int> selectStartOfGameBlizard(Dictionary<int,List<int>> _graph, int _target_n_blizards)
    {
        List<int> _blizards = new List<int>();

        //go through the graph...
        //select the tiles that should become blizards...


        // Use backtracking to find a valid configuration
        if (Backtrack(_graph, _blizards, _target_n_blizards))
        {
            return _blizards;
        }
        else
        {
            return null;
        }
    }

    private static bool Backtrack(Dictionary<int, List<int>> graph, List<int> blizzards, int target)
    {
        if (blizzards.Count == target)
        {
            return true;
        }

        // Get a shuffled list of all keys in the graph
        List<int> candidates = DictionaryExtensions.GetShuffledKeys(graph);


        for (int i = 0; i < candidates.Count; i++)
        {
            int candidate = candidates[i];

            // Check if the graph remains connected
            if (canBecomeBlizard(graph, candidate))
            {
                blizzards.Add(candidate);

                // Remove the candidate from the graph
                Dictionary<int, List<int>> graphCopy = graph.DeepCopy();
                GraphHelper.DeleteNode(graphCopy, candidate);

                if (Backtrack(graphCopy, blizzards, target))
                {
                    return true;
                }

                // Backtrack
                blizzards.Remove(candidate);
            }
        }

        return false;
    }



    /// <summary>
    /// returns a list of all the tile ids that can become blizards
    /// </summary>
    public static List<int> GET_blizardCandidates(Dictionary<int,List<int>> _graph)
    {
        List<int> _blizard_candidates = new List<int>();

        //look at the graph....
        //detemine which tiles could become blizards....

        foreach (var kvp in _graph)
        {
            int node = kvp.Key;
            if (canBecomeBlizard(_graph, node))
            {
                _blizard_candidates.Add(node);
            }
        }

        return _blizard_candidates;
    }



    /// <summary>
    /// return true if the tile can become a blizard...
    /// the given graph can be a directional graph
    /// </summary>
    public static bool canBecomeBlizard(Dictionary<int,List<int>> _graph, int _tile_ID)
    {
        //decide if _tile_ID can become a blizard   (this MIGHT be a powerup in the shop at some point (e.g. ice bomb) which will create a temporary? blizard
        //in order to do this there must be a way to determine if the tile can become a blizard...

        Dictionary<int,List<int>> _graphCopy = _graph.DeepCopy();

        if (_graphCopy.TryGetValue(_tile_ID, out List<int> _neighbors))
        {
            List<int> incomingNeighbors = _graphCopy.GetKeysWithValue(_tile_ID);
            _neighbors.AddRange(incomingNeighbors);
            // Remove duplicate entries
            _neighbors = _neighbors.Distinct().ToList();

            GraphHelper.DeleteNode(_graphCopy, _tile_ID);

            for (int i = 0; i < _neighbors.Count; i++)
            {
                int currentNeighbor = _neighbors[i];
                int nextNeighbor = _neighbors[(i + 1) % _neighbors.Count];

                if (!GraphHelper.BreadthFirstSearch(_graphCopy, currentNeighbor, nextNeighbor))
                {
                    return false; // next neighbor cannot be found from current neighbor, so current node splits map
                }
            }

            if (RiskySandBox_Tile.GET_RiskySandBox_Tile(_tile_ID).has_stable_portal)
                return false;
            return true;
        }
        else
        {
            // Handle the case where the tile ID is not found in the graph
            return false;
        }

        return false;
    }


}

public static class DictionaryExtensions
{
    public static Dictionary<int, List<int>> DeepCopy(this Dictionary<int, List<int>> original)
    {
        return original.ToDictionary(
            entry => entry.Key,
            entry => new List<int>(entry.Value)
        );        
    }

    public static List<int> GetShuffledKeys(Dictionary<int, List<int>> dictionary)
    {
        List<int> keys = dictionary.Keys.ToList();
        System.Random rng = new System.Random();
        int n = keys.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = keys[k];
            keys[k] = keys[n];
            keys[n] = value;
        }

        return keys;
    }


    public static List<int> GetKeysWithValue(this Dictionary<int, List<int>> dictionary, int value)
    {
        List<int> keysWithValue = new List<int>();

        foreach (var kvp in dictionary)
        {
            if (kvp.Value.Contains(value))
            {
                keysWithValue.Add(kvp.Key);
            }
        }

        return keysWithValue;
    }


}


public static class GraphHelper
{
    public static bool BreadthFirstSearch(Dictionary<int, List<int>> graph, int startNode, int nodeToFind)
    {
        List<int> visited = new List<int>();
        Queue<int> queue = new Queue<int>();

        queue.Enqueue(startNode);

        while (queue.Count > 0)
        {
            int currentNode = queue.Dequeue();

            if (currentNode == nodeToFind)
            {
                return true; // Node found
            }

            if (!visited.Contains(currentNode))
            {
                visited.Add(currentNode);

                if (graph.TryGetValue(currentNode, out List<int> neighbors))
                {
                    foreach (int neighbor in neighbors)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }
        }

        return false; // Node not found
    }

    public static void DeleteNode(Dictionary<int, List<int>> graph, int node)
    {
        // Remove the node from the graph
        if (graph.ContainsKey(node))
        {
            graph.Remove(node);
        }

        // Remove the node from the adjacency lists of other nodes
        foreach (var key in graph.Keys.ToList())
        {
            graph[key].Remove(node);
        }

    }

}