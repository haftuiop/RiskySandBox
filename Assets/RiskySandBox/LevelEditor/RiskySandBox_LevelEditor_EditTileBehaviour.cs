using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_LevelEditor_EditTileBehaviour : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] UnityEngine.UI.Button next_Tile_Button;
    [SerializeField] UnityEngine.UI.Button previous_Tile_Button;


    [SerializeField] ObservableBool enable_behaviour;

    RiskySandBox_Tile selected_Tile
    {
        get { return PRIVATE_selected_Tile; }
        set
        {
            if (PRIVATE_selected_Tile != null)
            {
                PRIVATE_selected_Tile.show_level_editor_ui.value = false;

                Vector3[] _mesh_points = PRIVATE_selected_Tile.mesh_points_2D.ToArray();
               
                RiskySandBox_LevelEditorHandlesManager.instance.destroyAllHandles();
                
                PRIVATE_selected_Tile.mesh_points_2D.SET_items(_mesh_points);
            }
            PRIVATE_selected_Tile = value;
            if (value != null)
            {
                value.show_level_editor_ui.value = true;

                Vector3[] _mesh_points = value.mesh_points_2D.ToArray();
                RiskySandBox_LevelEditorHandlesManager.instance.destroyAllHandles();
                RiskySandBox_LevelEditorHandlesManager.instance.createHandles(_mesh_points);
            }

        }
    }

    [SerializeField] RiskySandBox_Tile PRIVATE_selected_Tile;

    bool just_enabled_behaviour;

    void EventReceiver_OnVariableUpdate_enable_behaviour(ObservableBool _enable_behaviour)
    {
        if (_enable_behaviour.value == false)
        {
            if(selected_Tile != null)
            {
                this.selected_Tile = null;
            }
        }
            
    }


    void EventReceiver_OnUpdateHandles()
    {
        if(this.selected_Tile != null)
        {
            this.selected_Tile.mesh_points_2D.SET_items(RiskySandBox_LevelEditorHandlesManager.instance.handle_positions);
        }
    }



    private void Awake()
    {
        this.enable_behaviour.OnUpdate += EventReceiver_OnVariableUpdate_enable_behaviour;

        RiskySandBox_LevelEditor.Ondisable += RiskySandBox_LevelEditorEventReceiver_Ondisable;
        RiskySandBox_LevelEditor.OnrequestCloseOtherBehaviours += EventReceiver_OnrequestCloseOtherBehaviours;

        RiskySandBox_LevelEditorHandlesManager.OnUpdateHandles += EventReceiver_OnUpdateHandles;



        this.enable_behaviour.OnUpdate_true += delegate
        {
            this.just_enabled_behaviour = true;
            RiskySandBox_LevelEditor.instance.requestCloseOtherBehaviours();
            RiskySandBox_LevelEditorHandlesManager.instance.destroyAllHandles();
        };
            

    }

    public void EventReceiver_OnnextTileButtonPressed()
    {
        if (RiskySandBox_Tile.all_instances.Count <= 0)
            return;

        if (selected_Tile == null)
        {
            if (RiskySandBox_Tile.all_instances.Count > 0)
                selected_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(RiskySandBox_Tile.min_ID);
            return;
        }

        int _current_ID = this.selected_Tile.ID;
        while (true)
        {
            _current_ID += 1;
            if (_current_ID > RiskySandBox_Tile.all_instances.Max(x => x.ID.value))
            {
                this.selected_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(RiskySandBox_Tile.min_ID);
                return;
            }
            RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_current_ID);
            if(_Tile != null)
            {
                this.selected_Tile = _Tile;
                return;
            }
        }


    }

    public void EventReceiver_OnpreviousTileButtonPress()
    {
        if (RiskySandBox_Tile.all_instances.Count <= 0)
            return;

        if(this.selected_Tile == null)
        {
            if (RiskySandBox_Tile.all_instances.Count > 0)
                selected_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(RiskySandBox_Tile.max_ID);
            return;
        }
        int _current_ID = this.selected_Tile.ID;

        while(true)
        {
            _current_ID -= 1;
            if(_current_ID < RiskySandBox_Tile.all_instances.Min(x => x.ID.value))
            {
                this.selected_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(RiskySandBox_Tile.max_ID);
                return;
            }
            RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(_current_ID);
            if(_Tile != null)
            {
                this.selected_Tile = _Tile;
                return;
            }
        }
    }


    void EventReceiver_OnrequestCloseOtherBehaviours()
    {
        if(this.just_enabled_behaviour == true)
        {
            this.just_enabled_behaviour = false;
            return;
        }
        this.enable_behaviour.value = false;
    }


    void RiskySandBox_LevelEditorEventReceiver_Ondisable()
    {
        this.enable_behaviour.value = false;
        if (this.selected_Tile != null)
            this.selected_Tile = null;
    }



    private void Update()
    {
        if (enable_behaviour == false)
            return;


        if(this.selected_Tile == null)
        {
            //if we click on a tile?
            //select it...
            if(Input.GetMouseButtonDown(0))
            {
                this.selected_Tile = RiskySandBox_CameraControls.current_hovering_Tile;
            }
        }








    }



}
