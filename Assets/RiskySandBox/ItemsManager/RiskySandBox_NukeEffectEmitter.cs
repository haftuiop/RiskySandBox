using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_NukeEffectEmitter : MonoBehaviour
{
    [SerializeField] bool debugging;


    private void Awake()
    {
        RiskySandBox_ItemsManager.Onnuke += EventReceiver_Onnuke;
    }

    private void OnDestroy()
    {
        RiskySandBox_ItemsManager.Onnuke -= EventReceiver_Onnuke;
    }

    void EventReceiver_Onnuke(RiskySandBox_Tile _start_Tile,RiskySandBox_Tile _target_Tile)
    {

        if (PrototypingAssets.run_client_code == false)//only client needs to fire the visual effect for the nuke...
        {
            if (this.debugging)
                GlobalFunctions.print("run client code == false... not going to create a missile...",this);
            return;
        }

        Vector3 _start_point = new Vector3(0, 0, 0);



        //a tile has just been nuked...
        //so we want to emit a missile...


        Vector3 _end_point = _target_Tile.UI_position;

        if (_start_Tile != null)
            _start_point = _start_Tile.UI_position;


        Missile.createNew_duration(RiskySandBox_Resources.nuclear_missile_prefab, _start_point, _end_point, 3);//TODO - magic number...
    }
}
