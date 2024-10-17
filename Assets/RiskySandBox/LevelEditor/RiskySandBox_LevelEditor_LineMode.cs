using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_LevelEditor_LineMode : MonoBehaviour
{

    [SerializeField] bool debugging;

    public ObservableBool enable_behaviour{get{return this.PRIVATE_enable_behaviour;}}
    [SerializeField] ObservableBool PRIVATE_enable_behaviour;

    [SerializeField] LineRenderer my_LineRenderer;

    bool just_enabled_behaviour;

    public ObservableFloat line_width { get { return PRIVATE_line_width; } }
    [SerializeField] ObservableFloat PRIVATE_line_width;


    public ObservableInt line_Color_r { get { return PRIVATE_line_Color_r; } }
    public ObservableInt line_Color_g { get { return PRIVATE_line_Color_g; } }
    public ObservableInt line_Color_b { get { return PRIVATE_line_Color_b; } }
    public Color line_Color { get { return new Color(line_Color_r / 255f, line_Color_g / 255f, line_Color_b / 255f); } }

    [SerializeField] ObservableInt PRIVATE_line_Color_r;
    [SerializeField] ObservableInt PRIVATE_line_Color_g;
    [SerializeField] ObservableInt PRIVATE_line_Color_b;

    



    private void Awake()
    {

        my_LineRenderer.material = new Material(Shader.Find("Standard"));

        RiskySandBox_LevelEditor.Ondisable += EventReceiver_Ondisable;
        RiskySandBox_LevelEditor.OnrequestCloseOtherBehaviours += EventReceiver_OnrequestCloseOtherBehaviours;
       

        this.enable_behaviour.OnUpdate_true += delegate
        {
            this.just_enabled_behaviour = true;
            RiskySandBox_LevelEditor.instance.requestCloseOtherBehaviours();
            RiskySandBox_LevelEditorHandlesManager.instance.destroyAllHandles();
        };
;

        this.line_Color_r.OnUpdate += delegate { updateLineRenderer(); };
        this.line_Color_g.OnUpdate += delegate { updateLineRenderer(); };
        this.line_Color_b.OnUpdate += delegate { updateLineRenderer(); };

        updateLineRenderer();

    }

    private void OnDestroy()
    {
        RiskySandBox_LevelEditor.Ondisable -= EventReceiver_Ondisable;
        RiskySandBox_LevelEditor.OnrequestCloseOtherBehaviours -= EventReceiver_OnrequestCloseOtherBehaviours;

    }

    void EventReceiver_Ondisable()
    {
        this.enable_behaviour.value = false;

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

    void updateLineRenderer()
    {
        if (my_LineRenderer == null)
            return;

        my_LineRenderer.material.color = this.line_Color;        
    }

   


    private void Update()
    {
        if (this.enable_behaviour == false)
            return;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            RiskySandBox_LevelEditorHandlesManager.instance.createHandle(RiskySandBox_CameraControls.mouse_position);
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            RiskySandBox_LevelEditorHandlesManager.instance.createPermanentLine(this.line_width, this.line_Color_r, this.line_Color_g, this.line_Color_b, true);
        }

    }


}
