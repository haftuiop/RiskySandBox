using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RiskySandBox_MainGame))]
public class INSPECTOR_RiskySandBox_MainGame : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RiskySandBox_MainGame mainGame = (RiskySandBox_MainGame)target;

        // Add a button to start the game
        if (GUILayout.Button("Start Game"))
        {
            mainGame.startGame();
        }

        if(GUILayout.Button("save map"))
        {

            RiskySandBox_Map.instance.saveMap(RiskySandBox_MainGame.instance.map_ID);
        }


        if(GUILayout.Button("Load Map"))
        {
            RiskySandBox_Map.instance.loadMap(RiskySandBox_MainGame.instance.map_ID);
        }
    }
}
