using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using System.IO;

public partial class RiskySandBox_BattleLog : MonoBehaviour
{
    public static RiskySandBox_BattleLog instance;
    public static ObservableBool root_state { get { return instance.PRIVATE_root_state; } }


    [SerializeField] bool debugging;
    [SerializeField] bool testing;


    public static int internal_version_number { get { return 1; } }


    [SerializeField] ObservableStringList debug_log;//developer log for correcting errors... (unimplemented....)

    [SerializeField] ObservableStringList server_log;//a more detailed log with info needed to reconstruct the game...   "attack_event,0,5,10"

    [SerializeField] ObservableStringList human_ui_log;//what message(s) get shown to a human player... (more readable) e.g. "blue team attacked india with 10 troops"


    [SerializeField] RectTransform battle_log_start;

    public List<UnityEngine.UI.Text> text_fields = new List<UnityEngine.UI.Text>();

    [SerializeField] GameObject developer_text_prefab;
    [SerializeField] GameObject ui_root;


    [SerializeField] ObservableBool PRIVATE_root_state;


    [SerializeField] ObservableString gameID;


    [SerializeField] ObservableBool PRIVATE_server_mode;
    [SerializeField] ObservableBool PRIVATE_debug_mode;


    [SerializeField] ObservableInt PRIVATE_battle_log_shift;

    [SerializeField] ObservableBool PRIVATE_ui_enabled;



    void updateTextfields()
    {
        foreach (UnityEngine.UI.Text _Text in this.text_fields)
        {
            UnityEngine.Object.Destroy(_Text.gameObject);
        }

        text_fields.Clear();

        ObservableStringList _display_log = this.human_ui_log;//default to human mode...

        if (PRIVATE_server_mode)
            _display_log = this.server_log;

        else if (PRIVATE_debug_mode)
            _display_log = this.debug_log;
            


        foreach (string _log_entry in _display_log)
        {
            UnityEngine.UI.Text _new_Text = UnityEngine.Object.Instantiate(developer_text_prefab, this.ui_root.transform).GetComponent<UnityEngine.UI.Text>();

            _new_Text.text = _log_entry;

            this.text_fields.Add(_new_Text);
        }

        updateTextPositions();

    }

    void updateTextPositions()
    {
        for (int i = 0; i < text_fields.Count; i += 1)
        {
            this.text_fields[i].GetComponent<RectTransform>().anchoredPosition = battle_log_start.anchoredPosition + new Vector2(0, -30 * (i + PRIVATE_battle_log_shift));
        }
    }


    public void enableUI()
    {
        this.PRIVATE_root_state.value = true;
    }

    public void disableUI()
    {
        this.PRIVATE_root_state.value = false;
    }

    private void Awake()
    {
        RiskySandBox_Team.Ondeploy += EventReceiver_Ondeploy;
        RiskySandBox_Team.Onattack += EventReceiver_OnAttack;
        RiskySandBox_Team.Onfortify += EventReceiver_OnFortify;
        RiskySandBox_Team.Oncapture += EventReceiver_OnCapture;

        RiskySandBox_Tile.OnVariableUpdate_has_blizard_STATIC += EventReceiver_OnVariableUpdate_has_blizard_STATIC;
        RiskySandBox_Tile.OnVariableUpdate_has_stable_portal_STATIC += EventReceiver_OnVariableUpdate_has_stable_portal_STATIC;

        this.PRIVATE_debug_mode.OnUpdate += delegate { this.updateTextfields(); };
        this.PRIVATE_server_mode.OnUpdate += delegate { this.updateTextfields(); };
        this.PRIVATE_battle_log_shift.OnUpdate += delegate { this.updateTextPositions(); };

        instance = this;

    }

    private void OnDestroy()
    {

    }


    void EventReceiver_OnUpdate_log()
    {
        updateTextfields();
    }


    public void outputToFile()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return; //TODO this is unsuported... and probably always will be... essentially it is a right pain to save stuff in webgl builds of the game...
#endif
        this.gameID.randomize(32);
       

        // Define the subdirectory and filename
        string subdirectory = "RiskySandBox/BattleLogs";
        string fileName = string.Format("{0}.txt",this.gameID.value);

        // Combine the path to StreamingAssets with the subdirectory and filename
        string folderPath = Path.Combine(Application.streamingAssetsPath, subdirectory);
        string filePath = Path.Combine(folderPath, fileName);

        Debug.Log(filePath);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }



        StreamWriter _StreamWriter = new StreamWriter(filePath);



        _StreamWriter.WriteLine("BattleLogVersion:" + RiskySandBox_BattleLog.internal_version_number);
        _StreamWriter.WriteLine("BattleLogType:Server");

        _StreamWriter.WriteLine("region_start:GameInfo");
        _StreamWriter.WriteLine("GameID:" + (string)this.gameID);
        _StreamWriter.WriteLine("MapID:" + RiskySandBox_MainGame.instance.map_ID);
        _StreamWriter.WriteLine("start_date:01_01_2024|00_00_00");



        //TODO other critical info....

        _StreamWriter.WriteLine("region_end:GameInfo");

        


        foreach (RiskySandBox_Team _Team in RiskySandBox_Team.all_instances)
        {
            int _Team_ID = (int)_Team.ID.value;
            _StreamWriter.WriteLine("region_start:Team_" + _Team_ID);
            _StreamWriter.WriteLine("ID:" + _Team_ID);
            _StreamWriter.WriteLine("Name:" + (string)_Team.team_name);
            if (_Team.is_human)
                _StreamWriter.WriteLine("TeamType:" + "human");
            else
                _StreamWriter.WriteLine("TeamType:" + "AI");


            //TODO - other interesting info...

            _StreamWriter.WriteLine("region_end:Team_" + _Team_ID);
        }


        _StreamWriter.WriteLine("region_start:server_log");

        //so here we put the battle log...
        foreach (string _line in this.server_log)
        {
            _StreamWriter.WriteLine(_line);
        }

        _StreamWriter.WriteLine("region_end:server_log");

        _StreamWriter.Write("end:end");
        //no end of file or empty line errors...
        
        _StreamWriter.Close();
        Debug.Log("battle log done?!?!??");



    }


    void EventReceiver_OnVariableUpdate_has_blizard_STATIC(RiskySandBox_Tile _Tile)
    {
        if (RiskySandBox_ReplaySystem.is_enabled)
        {
            if (this.debugging)
                GlobalFunctions.print("RiskySandBox_ReplaySystem.is_enabled... returning", this);
            return;
        }

        server_log.Add(string.Format("TileEvent_has_blizard:{0},{1}", _Tile.ID.value, _Tile.has_blizard.value));

        if (_Tile.has_blizard)
            human_ui_log.Add(string.Format("Blizard created on Tile '{0}'", _Tile.tile_name.value));
        else
            human_ui_log.Add(string.Format("Blizard removed from Tile '{0}'", _Tile.tile_name.value));
    }

    void EventReceiver_OnVariableUpdate_has_stable_portal_STATIC(RiskySandBox_Tile _Tile)
    {
        if (RiskySandBox_ReplaySystem.is_enabled)
        {
            if (this.debugging)
                GlobalFunctions.print("RiskySandBox_ReplaySystem.is_enabled... returning", this);
            return;
        }

        server_log.Add(string.Format("TileEvent_has_stable_portal: '{0}'", _Tile.ID.value, _Tile.has_blizard.value));

        if (_Tile.has_stable_portal)
            human_ui_log.Add(string.Format("Stable Portal created on Tile '{0}", _Tile.ID.value));
        else
            human_ui_log.Add(string.Format("Stable Portal remove from Tile '{0}'", _Tile.ID.value, _Tile.tile_name.value));


    }


    void EventReceiver_Ondeploy(RiskySandBox_Team.EventInfo_Ondeploy _EventInfo)
    {
        if (RiskySandBox_ReplaySystem.is_enabled)
        {
            if (this.debugging)
                GlobalFunctions.print("RiskySandBox_ReplaySystem.is_enabled... returning", this);
            return;
        }

        server_log.Add(_EventInfo.battle_log_string);
        human_ui_log.Add(_EventInfo.human_log_string);

        this.EventReceiver_OnUpdate_log();
    }

    void EventReceiver_OnAttack(RiskySandBox_Team.EventInfo_Onattack _EventInfo)
    {
        if (RiskySandBox_ReplaySystem.is_enabled)
        {
            if (this.debugging)
                GlobalFunctions.print("RiskySandBox_ReplaySystem.is_enabled... returning", this);
            return;
        }

        server_log.Add(_EventInfo.battle_log_string);
        human_ui_log.Add(_EventInfo.human_log_string);

        this.EventReceiver_OnUpdate_log();
    }

    void EventReceiver_OnFortify(RiskySandBox_Team.EventInfo_Onfortify _EventInfo)
    {
        if (RiskySandBox_ReplaySystem.is_enabled)
        {
            if (this.debugging)
                GlobalFunctions.print("RiskySandBox_ReplaySystem.is_enabled... returning", this);
            return;
        }
            

        server_log.Add(_EventInfo.battle_log_string);
        human_ui_log.Add(_EventInfo.human_log_string);

        this.EventReceiver_OnUpdate_log();
    }

    void EventReceiver_OnCapture(RiskySandBox_Team.EventInfo_Oncapture _EventInfo)
    {
        if (RiskySandBox_ReplaySystem.is_enabled)
        {
            if (this.debugging)
                GlobalFunctions.print("RiskySandBox_ReplaySystem.is_enabled... returning", this);
            return;
        }

        this.server_log.Add(_EventInfo.battle_log_string);
        this.human_ui_log.Add(_EventInfo.human_log_string);

        this.EventReceiver_OnUpdate_log();
    }

    public void EventReceiver_OnInitialPlacementComplete()
    {
        if (RiskySandBox_ReplaySystem.is_enabled)
        {
            if (this.debugging)
                GlobalFunctions.print("RiskySandBox_ReplaySystem.is_enabled... returning", this);
            return;
        }

        string _teams_string = "";
        string _n_troops_string = "";

        for(int i = 0; i < RiskySandBox_Tile.all_instances.Count; i += 1)
        {
            RiskySandBox_Tile _Tile = RiskySandBox_Tile.all_instances[i];

            if (i == 0)
            {
                _teams_string = ""+_Tile.ID.value;
                _n_troops_string = "" + _Tile.num_troops.value;
            }
            else
            {
                _n_troops_string = _n_troops_string + string.Format(",{0}", _Tile.num_troops.value);
                _teams_string = _teams_string + string.Format(",{0}", _Tile.my_Team_ID.value);
            }
        }

        this.server_log.Add("GameEvent_initial_tile_team_values:"+_teams_string);
        this.server_log.Add("GameEvent_initial_tile_num_troops_values:"+_n_troops_string);

    }
}
