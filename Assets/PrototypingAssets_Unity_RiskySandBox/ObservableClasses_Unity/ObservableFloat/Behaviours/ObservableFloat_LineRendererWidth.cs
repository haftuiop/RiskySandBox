using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableFloat_LineRendererWidth : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] ObservableFloat start_width;
    [SerializeField] ObservableFloat end_width;

    [SerializeField] List<LineRenderer> my_LineRenderers = new List<LineRenderer>();


    private void OnEnable()
    {
        if (this.debugging)
            GlobalFunctions.print("", this);

        if (this.start_width != null)
            this.start_width.OnUpdate += EventReceiver_OnUpdate_width_value;

        if (this.end_width != null)
            this.end_width.OnUpdate += EventReceiver_OnUpdate_width_value;

        updateLineRenderers();

    }

    private void OnDisable()
    {
        if (this.debugging)
            GlobalFunctions.print("", this);

        if (this.start_width != null)
            this.start_width.OnUpdate -= EventReceiver_OnUpdate_width_value;

        if (this.end_width != null)
            this.end_width.OnUpdate -= EventReceiver_OnUpdate_width_value;
    }

    void EventReceiver_OnUpdate_width_value(ObservableFloat _ObservableFloat)
    {
        updateLineRenderers();
    }


    void updateLineRenderers()
    {
        if (this.debugging)
            GlobalFunctions.print("updating LineRenderers", this);

        if(start_width != null)
        {
            foreach(LineRenderer _LineRenderer in this.my_LineRenderers.Where(x => x != null))
            {
                _LineRenderer.startWidth = this.start_width;
            }
        }
        else
        {
            GlobalFunctions.printError("start_width is null", this);
        }

        if(end_width != null)
        {
            foreach(LineRenderer _LineRenderer in this.my_LineRenderers.Where(x => x != null))
            {
                _LineRenderer.endWidth = this.end_width;
            }
        }
        else
        {
            GlobalFunctions.printError("end_width is null", this);
        }
    }

}
