#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObservableBool))]
public partial class Inspector_ObservableBool : Editor
{

    ObservableBool my_ObservableBooL { get { return (ObservableBool)target; } }
    public override void OnInspectorGUI()
    {






        // Display default inspector gui
        DrawDefaultInspector();


        if (GUILayout.Button("SET_true"))
        {
            my_ObservableBooL.value = true;
        }

        if (GUILayout.Button("SET_false"))
        {
            my_ObservableBooL.value = false;
        }





    }
}
#endif