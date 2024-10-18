using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

/// <summary>
/// updates a Camera's FOV using an ObservableFloat
/// </summary>
public partial class ObservableFloat_CameraFOV : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableFloat my_ObservableFloat;
    [SerializeField] List<Camera> my_Cameras = new List<Camera>();



    private void OnEnable()
    {
        if(my_ObservableFloat == null)
        {
            GlobalFunctions.printError("my_ObservableFloat is null...",this);
            return;
        }
        updateCameras();
        my_ObservableFloat.OnUpdate += EventReceiver_OnUpdate;
    }

    private void OnDisable()
    {
        if(my_ObservableFloat != null)
            my_ObservableFloat.OnUpdate -= EventReceiver_OnUpdate;
    }

    void updateCameras()
    {
        float _value = this.my_ObservableFloat.value;
        foreach(Camera _Camera in this.my_Cameras)
        {
            if (_Camera == null)
                continue;
            _Camera.fieldOfView = _value;
        }
    }


    void EventReceiver_OnUpdate(ObservableFloat _my_ObservableFloat)
    {
        updateCameras();
    }


}
