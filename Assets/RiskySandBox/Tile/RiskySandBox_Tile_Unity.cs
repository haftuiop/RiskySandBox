using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;


public partial class RiskySandBox_Tile : PrototypingAssets_Tile
{


    public static RiskySandBox_Tile GET_RiskySandBox_Tile(Collider _Collider)
    {
        return (RiskySandBox_Tile) PrototypingAssets_Tile.GET_Tile(_Collider);
    }


     
    public Material my_LevelEditor_Material
    {
        get { return this.PRIVATE_my_LevelEditor_Material; }
        set
        {
            this.PRIVATE_my_LevelEditor_Material = value;
            updateVisuals();
        }
    }
    [SerializeField] Material PRIVATE_my_LevelEditor_Material;


    public Texture2D my_TerritoryCardTexture { get { return this.boxcast_Texture2D; } }






    public ObservableFloat UI_scale_factor { get { return this.PRIVATE_ui_scale_factor; } }



    public ObservableVector3 UI_position {get { return this.PRIVATE_ui_position; }}

    [SerializeField] ObservableVector3 PRIVATE_ui_position;



    [SerializeField] RectTransform ui_Canvas;

    [SerializeField] UnityEngine.UI.Text PRIVATE_ID_Text;







    [SerializeField] ObservableInt PRIVATE_num_troops;


    [SerializeField] ObservableBool PRIVATE_is_deploy_target;

    [SerializeField] ObservableBool PRIVATE_is_attack_start;
    [SerializeField] ObservableBool PRIVATE_is_attack_target;

    [SerializeField] ObservableBool PRIVATE_is_fortify_start;
    [SerializeField] ObservableBool PRIVATE_is_fortify_target;


    [SerializeField] ObservableBool PRIVATE_show_level_editor_ui;
    [SerializeField] ObservableInt PRIVATE_my_Team_ID;
    [SerializeField] ObservableFloat PRIVATE_ui_scale_factor;
    [SerializeField] ObservableBool PRIVATE_has_capital;//is there a "capital" on this tile?

    [SerializeField] ObservableBool PRIVATE_has_stable_portal;
    [SerializeField] ObservableBool PRIVATE_has_blizard;
    [SerializeField] ObservableBool PRIVATE_has_unstable_portal;
    [SerializeField] ObservableString PRIVATE_name;


    [SerializeField] UnityEngine.UI.Text PRIVATE_num_troops_Text;
    [SerializeField] ObservableIntList PRIVATE_graph_connection_IDs;



    [SerializeField] MeshRenderer my_MeshRenderer;


    [SerializeField] GameObject PRIVATE_has_capital_icon;

    public Material my_Material;


    protected override void Awake()
    {
        base.Awake();
        my_Material = new Material(Shader.Find("Standard"));


        this.my_Team_ID.OnUpdate += delegate { updateVisuals(); };
        this.PRIVATE_has_capital.OnUpdate += delegate { updateVisuals(); };
        this.PRIVATE_ID.OnUpdate += delegate { this.gameObject.name = "RiskySandBox_Tile ID = " + this.PRIVATE_ID.value; };

        this.extrusion_height.OnUpdate += delegate { updateVisuals(); };
        this.enable_dark_fow.OnUpdate += delegate { updateVisuals(); };

        this.my_Team_ID.OnUpdate += delegate { RiskySandBox_Tile.OnVariableUpdate_my_Team_ID_STATIC?.Invoke(this); };
        this.num_troops.OnUpdate += delegate { RiskySandBox_Tile.OnVariableUpdate_num_troops_STATIC?.Invoke(this); };
        this.has_stable_portal.OnUpdate += delegate { RiskySandBox_Tile.OnVariableUpdate_has_stable_portal_STATIC?.Invoke(this); };
        this.has_blizard.OnUpdate += delegate { RiskySandBox_Tile.OnVariableUpdate_has_blizard_STATIC?.Invoke(this); };

        


        RiskySandBox_MainGame.instance.display_bonuses.OnUpdate += EventReceiver_OnVariableUpdate_display_bonuses;
        RiskySandBox_Map.OnsaveMap += EventReceiver_OnsaveMap;
       




    }





    protected override void OnDestroy()
    {
        base.OnDestroy();

        RiskySandBox_MainGame.instance.display_bonuses.OnUpdate -= EventReceiver_OnVariableUpdate_display_bonuses;
        RiskySandBox_Map.OnsaveMap -= EventReceiver_OnsaveMap;
    }

    void EventReceiver_OnsaveMap(string _directory)
    {
        string _file = System.IO.Path.Combine(_directory, string.Format("Tile_{0}.txt", this.ID.value));

        System.IO.StreamWriter _writer = new System.IO.StreamWriter(_file);

        //TODO - allow multiple meshes by adding in a '|' character???? s- might be nice for visual settings? for a tile e.g. its 2 small islands connected to each other  (japan has 2 islands and you may wish to represent as 2 seperate meshes)

        _writer.WriteLine("verts:" + string.Join(",", this.mesh_points_2D.exportToFloatString())) ;

        _writer.WriteLine("ID:" + this.ID);
        _writer.WriteLine(save_load_key_num_troops + ":" + this.num_troops.value);
        _writer.WriteLine("team:" + this.my_Team_ID.value);
        _writer.WriteLine("name:" + this.name.value);

        _writer.WriteLine(string.Format("position:{0},{1},{2}", this.transform.position.x, this.transform.position.y, this.transform.position.z));//useful for "minor" adjustments to the tiles location...
        _writer.WriteLine("UI_scale_factor:" + this.UI_scale_factor);
        _writer.WriteLine("UI_position:" + this.UI_position.x + "," + this.UI_position.y + "," + this.UI_position.z);


        _writer.Close();
    }



    public void loadMeshFromLevelEditorHandles()
    {
        //get the positions...
        Vector3[] _positions = RiskySandBox_LevelEditorHandlesManager.instance.handle_positions.ToArray();

        this.mesh_points_2D.SET_items(_positions);
    }



    protected override void Start()
    {
        base.Start();

        updateVisuals();
    }


    void EventReceiver_OnVariableUpdate_display_bonuses(ObservableBool _display_bonuses)
    {
        updateVisuals();
    }




    public void updateVisuals()
    {
        if (base.debugging)
            GlobalFunctions.print("updating visuals! my_Team is "+this.my_Team,this);



            

        if (my_Team != null)
            this.PRIVATE_num_troops_Text.color = my_Team.text_Color;
        else
            this.PRIVATE_num_troops_Text.color = Color.black;

        PRIVATE_num_troops_Text.gameObject.SetActive(RiskySandBox_LevelEditor.is_enabled == false && (this.enable_dark_fow.value == false));

        
        PRIVATE_ID_Text.gameObject.SetActive(RiskySandBox_LevelEditor.is_enabled);

        this.PRIVATE_ui_position.value = new Vector3(this.PRIVATE_ui_position.x, this.extrusion_height + 0.001f, this.PRIVATE_ui_position.z);
           


    }


    public static void destroyTile(RiskySandBox_Tile _Tile)
    {
        if (MultiplayerBridge_PhotonPun.in_room)
            UnityEngine.Object.Destroy(_Tile.gameObject);
        else if (MultiplayerBridge_Mirror.is_enabled)
            Mirror.NetworkServer.Destroy(_Tile.gameObject);
        else
            UnityEngine.Object.Destroy(_Tile.gameObject);
    }



    public static void destroyAllTiles()
    {
        foreach(RiskySandBox_Tile _Tile in new List<RiskySandBox_Tile>(RiskySandBox_Tile.all_instances))
        {
            destroyTile(_Tile);
        }
    }






}
