using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using System.IO;

public partial class RiskySandBox_LevelEditor : MonoBehaviour
{
    public static ObservableBool is_editing_bonus { get { return instance.PRIVATE_is_editing_bonus; } }
    public static ObservableBool is_graph_mode { get { return instance.PRIVATE_is_graph_mode; } }

    public static RiskySandBox_LevelEditor instance;


    [SerializeField] bool debugging;


    [SerializeField] ObservableBool PRIVATE_is_editing_bonus;
    [SerializeField] ObservableBool PRIVATE_is_graph_mode;




    public static event Action Onenable;
    public static event Action Ondisable;
    //e.g. when you go into "bonus mode" or "graph mode"
    public static event Action OnrequestCloseOtherBehaviours;


    [SerializeField] ObservableBool show_escape_menu;


    public static ObservableBool is_enabled { get { return instance.PRIVATE_is_enabled; } }
    [SerializeField] ObservableBool PRIVATE_is_enabled;






    public void save()
    {
        //ask the map to save to the current time stamp...

        DateTime currentTime = DateTime.Now;

        string _map_name = string.Format("LevelEditorOutput_{0}_{1}_{2}_{3}_{4}_{5}", currentTime.Day, currentTime.Month, currentTime.Year, currentTime.Hour, currentTime.Minute, currentTime.Second);
        _map_name = RiskySandBox_MainGame.instance.map_ID;
        
        RiskySandBox_Map.instance.saveMap(_map_name);
        
    }


    public void load()
    {
        if(this.debugging)
            GlobalFunctions.print(string.Format("asking the Map to load the map... '{0}'",RiskySandBox_MainGame.instance.map_ID),this);
        RiskySandBox_Map.instance.loadMap(RiskySandBox_MainGame.instance.map_ID);
    }


    


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        disable();
    }


    public void requestCloseOtherBehaviours()
    {
        OnrequestCloseOtherBehaviours?.Invoke();
    }
    









    private void Update()
    {
        if (this.PRIVATE_is_enabled == false)
            return;



        RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;


   
        //if the "delete_tile" observablebool is true... why because accidently deleting a tile would be really annoying...

        if(_current_Tile != null && Input.GetKeyDown(KeyCode.P))
        {
            if (this.debugging)
                GlobalFunctions.print("p key pressed - deleting a tile!",this);
            //lets delete the tile...
            RiskySandBox_Tile.destroyTile(_current_Tile);
            
        }

        if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
            if (this.debugging)
                GlobalFunctions.print("escape key pressed - opening escape menu!",this);
            //open the escape menu...
            this.show_escape_menu.value = !this.show_escape_menu;
        }
        



    }

    public void enable()
    {
        this.PRIVATE_is_enabled.value = true;
        Onenable?.Invoke();
    }


    public void disable()
    {
        this.PRIVATE_is_enabled.value = false;
        Ondisable?.Invoke();

    }



}
