using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;



/// <summary>
/// the replay system reads the server battle log to reconstruct the game (for later analysis/personal interest)
/// </summary>
public partial class RiskySandBox_ReplaySystem : MonoBehaviour
{
    public static RiskySandBox_ReplaySystem instance;

    public static string battle_log_folder {get { return System.IO.Path.Combine(Application.streamingAssetsPath, "RiskySandBox/BattleLogs"); }}


    public static bool is_enabled { get { return instance.PRIVATE_enabled; } }

    [SerializeField] bool debugging;

    [SerializeField] ObservableBool PRIVATE_enabled;


    [SerializeField] ObservableStringList server_log;

    public ObservableInt log_index { get { return this.PRIVATE_log_index; } }
    [SerializeField] ObservableInt PRIVATE_log_index;


    [SerializeField] ObservableString game_ID;

    /// <summary>
    /// is the replay system currently rerunning a game...
    /// </summary>
    [SerializeField] ObservableBool PRIVATE_is_replaying;

    [SerializeField] UnityEngine.UI.Text log_index_Text;


    List<GameObject> instantiated_game_log_UIs = new List<GameObject>();
    [SerializeField] GameObject game_log_ui_prefab;
    [SerializeField] GameObject root;

    [SerializeField] ObservableBool PRIVATE_show_escape_menu;

    public void EventReceiver_OnrefreshGameLogsButtonPressed()
    {
        createLoadGameLogUIs();
    }

    public void EventReceiver_OnexitReplayButtonPressed()
    {
        this.PRIVATE_is_replaying.value = false;

        RiskySandBox_Map.instance.clearMap();
        //TODO we also need to kill the teams...
        RiskySandBox_Team.destroyAllTeams();



        createLoadGameLogUIs();

        this.PRIVATE_show_escape_menu.value = false;
    }


    void createLoadGameLogUIs()
    {
        foreach (GameObject _GameObject in instantiated_game_log_UIs)
        {
            UnityEngine.Object.Destroy(_GameObject);
        }

        this.instantiated_game_log_UIs.Clear();


        //go through every battle log...
        string[] _battle_log_files = System.IO.Directory.GetFiles(battle_log_folder, "*.txt");

        for (int i = 0; i < _battle_log_files.Count(); i += 1)
        {
            string _file = _battle_log_files[i];
            Debug.Log(_file);

            string[] _lines = System.IO.File.ReadAllLines(_file);

            List<string> _keys = _lines.Select(x => x.Split(":")[0]).ToList();
            List<string> _values = _lines.Select(x => x.Split(":")[1]).ToList();

            //pull out the date, the game id, and the map???
            //and the winner???


            RiskySandBox_LoadableBattleLogUI _new_UI = UnityEngine.Object.Instantiate(game_log_ui_prefab, root.transform).GetComponent<RiskySandBox_LoadableBattleLogUI>();
            this.instantiated_game_log_UIs.Add(_new_UI.gameObject);

            _new_UI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,-60) + new Vector2(0, -30 * i);

            _new_UI.game_ID.value = _values[_keys.IndexOf("GameID")];
            _new_UI.start_date.value = _values[_keys.IndexOf("start_date")];

            string _battle_log_ID = _values[_keys.IndexOf("GameID")];
            _new_UI.OnloadButtonPressed += delegate { loadGameLog(_battle_log_ID); };


        }
    }


    private void Awake()
    {
        instance = this;
        this.log_index.OnUpdate += EventReceiver_OnVariableUpdate_log_index;

        this.PRIVATE_enabled.OnUpdate += delegate { createLoadGameLogUIs(); };
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && PRIVATE_is_replaying == true)
        {
            this.PRIVATE_show_escape_menu.toggle();
        }
    }

    public void EventReceiver_stepForwardButtonPressed()
    {
        if (this.debugging)
            GlobalFunctions.print("step button pressed... calling step()", this);
        stepForward();
    }


    public void EventReceiver_stepBackButtonPressed()
    {
        if(this.debugging)
            GlobalFunctions.print("previous step button pressed... going back in time!",this);

        stepBack();
    }



    void loadGameLog(string _game_ID)
    {
        string _expected_FilePath = System.IO.Path.Combine(battle_log_folder, _game_ID + ".txt");
        if (this.debugging)
            GlobalFunctions.print("trying to load battle log at: " + _expected_FilePath,this);

        if(System.IO.File.Exists(_expected_FilePath) == false)
        {
            GlobalFunctions.printError("unable to find the GameLog for the _game_ID = " + _game_ID, this);
            return;
        }

        foreach(GameObject _GameObject in this.instantiated_game_log_UIs)
        {
            UnityEngine.Object.Destroy(_GameObject);
        }


        GlobalFunctions.print("found the game_ID", this);

        string[] _lines = System.IO.File.ReadAllLines(_expected_FilePath);

        List<string> _keys = _lines.Select(x => x.Split(":")[0]).ToList();
        List<string> _values = _lines.Select(x => x.Split(":")[1]).ToList();


        PRIVATE_log_index.value = 0;
        server_log.Clear();



        //first lets create the teams...
        bool _in_Team_region = false;
        bool _in_server_log_region = false;

        for (int i = 0; i < _lines.Count(); i += 1)
        {
            string _key = _keys[i];
            string _value = _values[i];
            string _line = _lines[i];

            if(_line.StartsWith("region_start:Team"))
            {
                print("enteres a team region...");
                _in_Team_region = true;
                continue;
            }

            if(_line.StartsWith("region_end:Team"))
            {
                _in_Team_region = false;
                continue;
            }

            if(_line.StartsWith("region_start:server_log"))
            {
                _in_server_log_region = true;
                continue;
            }

            if(_line.StartsWith("region_end:server_log"))
            {
                _in_server_log_region = false;
                continue;
            }

            if (_in_Team_region == true)
            {
                if (_key == "ID")
                {
                    RiskySandBox_Resources.createTeam(int.Parse(_values[i]));
                    continue;
                }
            }

            if(_in_server_log_region)
            {
                server_log.Add(_line);
            }

            
        }

        RiskySandBox_Map.instance.loadMap(_values[_keys.IndexOf("MapID")]);

        this.PRIVATE_is_replaying.value = true;

        if(server_log.Count > 0)
        {
            this.log_index.value = 0;
        }
    }

    public void enable()
    {
        this.PRIVATE_enabled.value = true;
    }

    public void disable()
    {
        this.PRIVATE_enabled.value = false;
    }

    public void EventReceiver_OnbackToMainMenuButtonPressed()
    {
        this.disable();
        RiskySandBox_MainMenu.instance.returnToMainMenu();
    }

    void EventReceiver_OnVariableUpdate_log_index(ObservableInt _log_index)
    {
        log_index_Text.text = string.Format("{0}/{1}", this.log_index.value, this.server_log.Count());
    }

    void stepForward()
    {

        if(this.log_index >= server_log.Count())
        {
            if (this.debugging)
                GlobalFunctions.print("reached end of log???",this);
            return;
        }

        //ok! great so we now want to make a single step from the log
        string _log_entry = server_log[log_index];

        string _key = _log_entry.Split(":")[0];
        string _value = _log_entry.Split(":")[1];

        if (_key == "GameEvent_initial_tile_team_values")
        {
            int[] _team_values = _value.Split(",").Select(x => int.Parse(x)).ToArray();
            for (int i = 0; i < RiskySandBox_Tile.all_instances.Count(); i += 1)
            {
                RiskySandBox_Tile.all_instances[i].my_Team_ID.value = _team_values[i];
            }
        }

        if(_key == "GameEvent_initial_tile_num_troops_values")
        {
            int[] _n_troop_values = _value.Split(",").Select(x => int.Parse(x)).ToArray();
            for(int i = 0; i < RiskySandBox_Tile.all_instances.Count(); i += 1)
            {
                RiskySandBox_Tile.all_instances[i].num_troops.value = _n_troop_values[i];
            }
        }

        if(_key == "GameEvent_deploy")
        {
            RiskySandBox_Team.TRY_deployFromBattleLogEntry(_log_entry);
        }

        if(_key == "GameEvent_attack")
        {
            RiskySandBox_Team.TRY_attackFromBattleLogEntry(_log_entry);

        }

        if(_key == "GameEvent_fortify")
        {
            RiskySandBox_Team.TRY_fortifyFromBattleLogEntry(_log_entry);
        }

        if(_key == "GameEvent_capture")
        {
            RiskySandBox_Team.TRY_captureFromBattleLogEntry(_log_entry);
        }

        log_index.value += 1;
        //if we got the end of the server log????
        //we must say "done!"

    }

    public void stepBack()
    {


        string _log_entry = server_log[log_index];

        int _previous = this.log_index - 1;


        //TODO - try to remember the state of the game at the start of each round...
        //then we can just roll forwards the required number of steps to get to this point???/

        log_index.value = 0;//for now lets just go back to the beginning...
        for (int i = 0; i < _previous; i += 1)
        {
            stepForward();//the step forwards the number of times needed...
        }

    }








}
