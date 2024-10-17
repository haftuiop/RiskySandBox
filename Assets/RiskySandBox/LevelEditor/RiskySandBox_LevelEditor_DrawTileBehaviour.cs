using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_LevelEditor_DrawTileBehaviour : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableBool enable_behaviour;

    [SerializeField] ObservableBool PRIVATE_draw_LineRenderer;

    public ObservableFloat tile_ui_scale_factor;


    bool just_enabled_behaviour;






    private void Awake()
    {
        RiskySandBox_LevelEditor.Ondisable += RiskySandBox_LevelEditorEventReceiver_Ondisable;
        RiskySandBox_LevelEditor.OnrequestCloseOtherBehaviours += EventReceiver_OnrequestCloseOtherBehaviours;

        this.enable_behaviour.OnUpdate_true += delegate
        {
            this.just_enabled_behaviour = true;
            RiskySandBox_LevelEditor.instance.requestCloseOtherBehaviours();
            RiskySandBox_LevelEditorHandlesManager.instance.destroyAllHandles();
        };

    }


    void RiskySandBox_LevelEditorEventReceiver_Ondisable()
    {
        this.enable_behaviour.value = false;
    }


    void EventReceiver_OnrequestCloseOtherBehaviours()
    {
        if(this.just_enabled_behaviour)
        {
            this.just_enabled_behaviour = false;
            return;
        }

        this.enable_behaviour.value = false;


    }









    // Update is called once per frame
    void Update()
    {
        if (this.enable_behaviour == false)
        {
            if (this.debugging)
                GlobalFunctions.print("enable behaviour is false... returning",this);
            return;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            if (this.debugging)
                GlobalFunctions.print("detected a 'c' key press - creating a new Tile! (and exiting draw mode...)",this);

            int _creation_ID = 1;
            if (RiskySandBox_Tile.all_instances.Count > 0)
                _creation_ID = RiskySandBox_Tile.all_instances.Max(x => x.ID.value) + 1;//give a unique id to the tile...


            RiskySandBox_Tile _created_Tile = RiskySandBox_LevelEditorHandlesManager.instance.TRY_createTile(_creation_ID,true);


            if (_created_Tile != null)
            {
                Vector3[] _mesh_points = _created_Tile.mesh_points_2D.ToArray();
                _created_Tile.UI_scale_factor.value = this.tile_ui_scale_factor;
                _created_Tile.UI_position.value = new Vector3(_mesh_points.Sum(v => v.x) / _mesh_points.Count(), 0, _mesh_points.Sum(v => v.z) / _mesh_points.Count());
            }

            this.enable_behaviour.value = false;
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            if (this.debugging)
                GlobalFunctions.print("'m' key pressed! - inverting MeshRenderer...",this);
            RiskySandBox_LevelEditorHandlesManager.instance.enable_MeshRenderer.toggle();
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            if (this.debugging)
                GlobalFunctions.print("'l' key pressed! - inverting LineRenderer", this);
            this.PRIVATE_draw_LineRenderer.toggle();
        }

        RiskySandBox_Tile _current_Tile = RiskySandBox_CameraControls.current_hovering_Tile;

        if (_current_Tile != null && Input.GetKeyDown(KeyCode.F))//if we are hovering over a tile and hit the 'f' button...
        {
            if (this.debugging)
                GlobalFunctions.print("'f' key pressed - placing the ui onto the tile with id" + _current_Tile.ID.value,this);
            _current_Tile.UI_position.value = RiskySandBox_CameraControls.current_hit_point;//place the ui for the tile at the hit point
            _current_Tile.UI_scale_factor.value = this.tile_ui_scale_factor;//update the scale factor for the ui...

        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (this.debugging)
                GlobalFunctions.print("'space' key pressed - creating a new handle!",this);
            RiskySandBox_LevelEditorHandlesManager.instance.createHandle(RiskySandBox_CameraControls.mouse_position);
        }

        






        



    }



}
