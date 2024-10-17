#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RiskySandBox_DiseaseCreator))]
public class RiskySandBox_DiseaseCreatorEditor : Editor
{
    string diseaseName = "New Disease";
    int duration = 7;
    float lethality = 0.1f;
    float infectivity = 0.5f;

    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get reference to the target object (RiskySandBox_DiseaseCreator instance)
        RiskySandBox_DiseaseCreator diseaseCreator = (RiskySandBox_DiseaseCreator)target;

        GUILayout.Space(10);

        // Fields for Disease Parameters
        diseaseName = EditorGUILayout.TextField("Disease Name", diseaseName);
        duration = EditorGUILayout.IntField("Duration", duration);
        lethality = EditorGUILayout.FloatField("Lethality", lethality);
        infectivity = EditorGUILayout.FloatField("Infectivity", infectivity);

        GUILayout.Space(10);

        // Button to Create Disease
        if (GUILayout.Button("Create Disease"))
        {
            // Call the method to create the disease with the specified parameters
            diseaseCreator.createDisease(diseaseName, duration, lethality, infectivity);
        }
    }
}
#endif
