using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableFloat_ScaleGameObject : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] ObservableFloat my_ObservableFloat;

    [SerializeField] List<GameObject> my_GameObjects = new List<GameObject>();


    [SerializeField] bool scale_x;
    [SerializeField] bool scale_y;
    [SerializeField] bool scale_z;



    private void OnEnable()
    {
        Debug.LogWarning("deprecated...", this);
        if (my_ObservableFloat != null)
        {
            my_ObservableFloat.OnUpdate += EventReceiver_OnUpdate;
            updateGameObjects();//immediatley adjust the scale...
        }
        else
            GlobalFunctions.printError("my_ObservableFloat is null...", this);

    }

    private void OnDisable()
    {
        if (my_ObservableFloat == null)
            return;
        my_ObservableFloat.OnUpdate -= EventReceiver_OnUpdate;
    }



    void EventReceiver_OnUpdate(ObservableFloat _my_ObservableFloat)
    {
        updateGameObjects();
    }

    void updateGameObjects()
    {
        float _value = my_ObservableFloat.value;

        foreach (GameObject _GameObject in my_GameObjects)
        {
            if (_GameObject == null)//TODO - debug warning?
                continue;

            Transform _Transform = _GameObject.transform;
            
            Vector3 _scale = _Transform.localScale;

            if (scale_x)
                _scale.x = _value;

            if (scale_y)
                _scale.y = _value;

            if (scale_z)
                _scale.z = _value;

            
            _Transform.localScale = _scale;
            

        }
    }



}
