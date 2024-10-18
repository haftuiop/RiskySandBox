#if UNITY_EDITOR
using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using UnityEditor;

using System.Diagnostics;

public partial class BuildAllVersions
{

    public static string itch_username = "monkeywearingafezwithamop";//replace with your username
    public static string itch_project = "project2-turnbasedfighting";//replace with your project name...


    [MenuItem("Building/Build & upload (itch.io)")]
    public static void BuildAndUpload_itchIO()
    {

        UnityEngine.Debug.Log("BuildAndUpload_itchIO called!");

        BuildOutput _BuildAll_output = BuildAll();

        foreach (KeyValuePair<string, string> _KVP in _BuildAll_output.standalone_locations)
        {
            //UnityEngine.Debug.Log("handling " + _KVP.Key + " at: " + _KVP.Value);

            string _build_directory = _KVP.Value;
            string _standalone_push_command = string.Format("push {0} {1}/{2}:{3}", _build_directory, itch_username, itch_project, _KVP.Key);

            RunTerminalCommands_butler(_standalone_push_command);
            
            
        }

        
        string _webgl_push_command = string.Format("push {0} {1}/{2}:HTML5", _BuildAll_output.webgl_path, itch_username, itch_project);

        RunTerminalCommands_butler(_webgl_push_command);
        



        UnityEngine.Debug.Log("BuildAndUpload_itchIO done!... check itch io (may take a minute to update page)");




    }


   


    static void RunTerminalCommands_butler(string commands)
    {

        //UnityEngine.Debug.Log("running the terminal command: '" + commands+"'");

        ProcessStartInfo processInfo = new ProcessStartInfo(project_path + "/Assets/PrototypingAssets_Unity_RiskySandBox/UnityBuild-Upload/itch/Butler");

        processInfo.Arguments = commands;



        processInfo.RedirectStandardOutput = true;
        processInfo.RedirectStandardError = true;
        processInfo.UseShellExecute = false;
        processInfo.CreateNoWindow = true;
        

        Process process = new Process();
        process.StartInfo = processInfo;
        process.Start(); 

        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();


        //UnityEngine.Debug.Log("command line output: " + output);
        //if(error != "")
            //UnityEngine.Debug.LogError("command line error: " + error);

    }









}

#endif
