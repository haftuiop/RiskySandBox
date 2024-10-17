using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_LevelEditorHandle : MonoBehaviour
{
    



    [SerializeField] bool debugging;


    public Material hovering_Material { get { return PrototypingAssets_Materials.red; } }
    public Material default_Material { get { return PrototypingAssets_Materials.white; } }


    public static ObservableFloat handles_width { get { return RiskySandBox_LevelEditorHandlesManager.instance.handle_width; } }




    private void Awake()
    {
        gameObject.name = "new LevelEditor Handle";
        handles_width.OnUpdate += EventReceiver_OnVariableUpdate_handles_width;
    }

    // Start is called before the first frame update
    void Start()
    {
        updateTransformScale();
    }
    private void OnDestroy()
    {
        handles_width.OnUpdate -= EventReceiver_OnVariableUpdate_handles_width;
    }

    private void OnDisable()
    {
        if (RiskySandBox_LevelEditorHandlesManager.instance.hovering_handles.Contains(gameObject))
            RiskySandBox_LevelEditorHandlesManager.instance.hovering_handles.Remove(gameObject);
    }

    void EventReceiver_OnVariableUpdate_handles_width(ObservableFloat _handles_width)
    {
        updateTransformScale();
    }

    void updateTransformScale()
    {
        this.transform.localScale = new Vector3(handles_width, handles_width, handles_width);
    }



    private void OnMouseEnter()
    {
        if (RiskySandBox_LevelEditorHandlesManager.instance.hovering_handles.Contains(gameObject) == false)
            RiskySandBox_LevelEditorHandlesManager.instance.hovering_handles.Add(gameObject);

        this.GetComponent<MeshRenderer>().material = hovering_Material;
    }

    private void OnMouseExit()
    {
        if(RiskySandBox_LevelEditorHandlesManager.instance.hovering_handles.Contains(gameObject))
            RiskySandBox_LevelEditorHandlesManager.instance.hovering_handles.Remove(gameObject);
        this.GetComponent<MeshRenderer>().material = default_Material;
    }








}
