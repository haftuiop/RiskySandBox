#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RiskySandBox_AlertSystem))]
public class INSPECTOR_RiskySandBox_AlertSystem : Editor
{

    [SerializeField] int infect_target;
    public override void OnInspectorGUI()
    {
        // Draw the default inspector first
        DrawDefaultInspector();

        // Reference to the script instance
        RiskySandBox_AlertSystem alertSystem = (RiskySandBox_AlertSystem)target;



        // Add a button to the inspector
        if (GUILayout.Button("Create Test Message"))
        {
            // Call a method to create a test alert message
            CreateTestAlert(alertSystem);
        }
    }

    private void CreateTestAlert(RiskySandBox_AlertSystem alertSystem)
    {
        // Create a test message with a default texture and tile reference (null in this case)
        string testMessage = "This is a test alert!";
        Texture2D testTexture = Texture2D.blackTexture; // A simple black texture
        RiskySandBox_Tile focusTile = null; // Assuming you don't have a tile to focus on

        RiskySandBox_Alert newAlert = RiskySandBox_AlertSystem.createAlert(testMessage, testTexture, focusTile);

        // Optionally, log the creation to the console for debugging
        Debug.Log("Test alert created: " + newAlert.alert_message.value);
    }
}
#endif
