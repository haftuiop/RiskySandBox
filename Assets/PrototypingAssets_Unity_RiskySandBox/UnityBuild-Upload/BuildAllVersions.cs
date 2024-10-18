#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;


//code to build all availible versions....
public partial class BuildAllVersions
{

    public static string product_name { get { return PlayerSettings.productName; } }

    public static string project_path
    {
        get
        {
            // Get the path to the Assets folder
            string assetsPath = Application.dataPath;

            // Get the path to the project root folder
            string projectPath = System.IO.Path.GetDirectoryName(assetsPath);
            return projectPath;
        }
    }

    [MenuItem("Building/Build Server (Mac)")]
    public static void BuildMacServer()
    {
        // Set output path for the build
        string outputPath = "Builds/Server-Mac"; // Path to save the build

        // Define build options
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        buildPlayerOptions.locationPathName = outputPath;

        buildPlayerOptions.target = BuildTarget.StandaloneOSX;
        buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Server; // Fix here: No need to cast to int
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;
        buildPlayerOptions.scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray();

        // Perform the build and get the report
        UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        // Check if build was successful
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("Mac Server Build Completed Successfully!");
        }
        else
        {
            Debug.LogError("Mac Server Build Failed!");
        }
    }

    [MenuItem("Building/Build Server (Linux)")]
    public static void BuildLinuxServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.locationPathName = "Builds/Server-Linux/Server.x86_64";
        buildPlayerOptions.target = BuildTarget.StandaloneLinux64;

        // Set the compression option for the build
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;

        // Set the standalone build subtarget to Server
        buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Server;

        buildPlayerOptions.scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray();

        BuildPipeline.BuildPlayer(buildPlayerOptions);
        UnityEngine.Debug.Log("Built Server (Linux).");
    }


    [MenuItem("Building/Build Standalone (Mac)")]
    public static void BuildMacStandalone()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        // Set the output path and file name for the macOS standalone build
        buildPlayerOptions.locationPathName = "Builds/" + product_name +".app";

        // Set the target platform to macOS
        buildPlayerOptions.target = BuildTarget.StandaloneOSX;

        // Set the compression option for the build (optional)
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;

        // Get all scenes included in the build from EditorBuildSettings
        buildPlayerOptions.scenes = EditorBuildSettings.scenes.Select(x => x.path).ToArray();

        // Build the project
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        UnityEngine.Debug.Log("Built Standalone (Mac).");
    }


    /// <summary>
    /// build all versions of the game and returns a dictionary {"channel":absolute path}
    /// </summary>
    [MenuItem("Building/Build All Versions")]

    public static BuildOutput BuildAll()
    {
        // Define your build settings for each version here
        List<BuildSettings> allBuilds = new List<BuildSettings>();


        BuildOutput _output = new BuildOutput();




        try { allBuilds.Add(new BuildSettings("Android", BuildTarget.Android, BuildOptions.None)); }
        catch { }

        try { allBuilds.Add(new BuildSettings("iOS", BuildTarget.iOS, BuildOptions.None)); }
        catch { }

        try { allBuilds.Add(new BuildSettings("macOS", BuildTarget.StandaloneOSX, BuildOptions.None)); }
        catch { }

        try { allBuilds.Add(new BuildSettings("win32", BuildTarget.StandaloneWindows, BuildOptions.None)); }
        catch { }

        try { allBuilds.Add(new BuildSettings("win64", BuildTarget.StandaloneWindows64, BuildOptions.None)); }
        catch { }

        try { allBuilds.Add(new BuildSettings("linux", BuildTarget.StandaloneLinux64, BuildOptions.None)); }
        catch { }




        Dictionary<string,string> _standalone_dict = new Dictionary<string, string>();



        //create builds into path_to_your_project/Builds (up one level from /Assets)

        foreach (BuildSettings build in allBuilds)
        {
            string _pipeline_output_path = "Builds/" + product_name  + "_"+ build.buildName+"/";
            string _zip_file_path = "Builds/" + product_name + "-" + build.buildName + ".zip";
            _standalone_dict.Add(build.buildName,Path.Combine(project_path,_zip_file_path));

            try
            {
                string _build_folder = _pipeline_output_path + product_name + "_" + build.buildName;

                BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, _build_folder, build.buildTarget, build.buildOptions);

                if (File.Exists(_zip_file_path))
                    File.Delete(_zip_file_path);
                ZipFile.CreateFromDirectory(_pipeline_output_path, _zip_file_path);

            }
            catch
            {
                UnityEngine.Debug.LogError("error while trying to create a build for: " + build.buildName);
                
                continue;
            }

            
            
        }

        _output.standalone_locations = _standalone_dict;

        _output.webgl_path = buildForBrowser();


        return _output;


    }


    static string buildForBrowser()
    {
        string _pipeline_output_path = "Builds/" + product_name + "-browser";
        string _zip_file_path = "Builds/"+ product_name + "-browser.zip";

        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes,_pipeline_output_path, BuildTarget.WebGL, BuildOptions.None);

        if (File.Exists(_zip_file_path))
            File.Delete(_zip_file_path);
        ZipFile.CreateFromDirectory(_pipeline_output_path, _zip_file_path);


        return _zip_file_path;
    }


    public struct BuildOutput
    {

        public Dictionary<string, string> standalone_locations;

        public string webgl_path;


    }

    
    private class BuildSettings
    {
        public string buildName;
        public BuildTarget buildTarget;
        public BuildOptions buildOptions;

        public BuildSettings(string name, BuildTarget target, BuildOptions options)
        {
            buildName = name;
            buildTarget = target;
            buildOptions = options;
        }
    }
}
#endif
