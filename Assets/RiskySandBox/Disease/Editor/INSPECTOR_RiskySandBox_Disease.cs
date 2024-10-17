#if UNITY_EDITOR

using System.Collections;using System.Collections.Generic;using System.Linq;using System;

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RiskySandBox_Disease))]
public partial class INSPECTOR_RiskySandBox_Disease : Editor
{
    [SerializeField] int infect_target;

    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Reference to the target script
        RiskySandBox_Disease script = (RiskySandBox_Disease)target;

        // Add a button that calls the step function
        if (GUILayout.Button("Execute Step"))
        {
            script.step();
        }

        if(GUILayout.Button("infectRandom"))
        {
            //select a random tile...
            RiskySandBox_Tile _random_Tile = GlobalFunctions.GetRandomItem(RiskySandBox_Tile.all_instances.ToList());

            script.infectTile(_random_Tile);

        }


        infect_target = EditorGUILayout.IntField("infect target",this.infect_target);

        if(GUILayout.Button("infectTarget"))
        {
            RiskySandBox_Tile _target_Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(infect_target);

            script.infectTile(_target_Tile);
        }

    }
}


#endif

