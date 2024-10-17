using System.Collections;using System.Collections.Generic;using System.Linq;using System;using System.IO;
using UnityEngine;

public partial class RiskySandBox_Map
{


    public static event Action<string> OnsaveMap;


    public void saveMap(string _map_ID)
    {

        if (debugging)
            GlobalFunctions.print("saving the map!", this, _map_ID);


        string _new_map_folder_dir = Path.Combine(RiskySandBox.maps_folder_path, _map_ID);

        if (Directory.Exists(_new_map_folder_dir) == false)
        {
            Directory.CreateDirectory(_new_map_folder_dir);
        }

        OnsaveMap?.Invoke(_new_map_folder_dir);



        StreamWriter writer = new StreamWriter(_new_map_folder_dir + "/MapInfo.txt");
        writer.WriteLine("ID:" + _map_ID);
        writer.Close();

        try//we DO NOT WANT to fail saving the map just because the graph fails...
        {
            List<string> _graph_lines = new List<string>();

            foreach(RiskySandBox_Tile _Tile in RiskySandBox_Tile.all_instances)
            {
                if (_Tile.graph_connections.Count <= 0)
                    continue;//weird... but ok lets allow this to happen.... maybe the player is working on the level editor????
                _graph_lines.Add(_Tile.ID + "," + string.Join(",", _Tile.graph_connections.Select(x => x.ID)));
            }

            System.IO.File.WriteAllLines(_new_map_folder_dir + "/Graph.txt", _graph_lines);

        }
        catch (Exception ex)
        {

            Debug.LogError("An error occurred while trying to save the graph... " + ex.Message);
        }


        try
        {
            string _camera_settings_path = Path.Combine(_new_map_folder_dir, "Camera_Settings.txt");
            RiskySandBox_CameraControls.instance.saveSettings(_camera_settings_path);
        }
        catch (Exception ex)
        {
            Debug.LogError("An Error occured while trying to save the camera settings..." + ex.Message);
        }

    }

}
