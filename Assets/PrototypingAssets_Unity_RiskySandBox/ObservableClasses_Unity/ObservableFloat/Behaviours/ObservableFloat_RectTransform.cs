using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableFloat_RectTransform : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableFloat my_ObservableFloat;
    [SerializeField] List<RectTransform> my_RectTransforms = new List<RectTransform>();

    [SerializeField] bool control_width;
    [SerializeField] bool control_height;

    [SerializeField] bool control_anchored_x;
    [SerializeField] bool control_anchored_y;
    [SerializeField] bool control_anchoed_z;


    private void Awake()
    {
        if(my_ObservableFloat == null)
        {
            GlobalFunctions.printError("my_ObservableFloat == null...", this);
            return;
        }


        this.my_ObservableFloat.OnUpdate += EventReceiver_OnUpdate;

        updateRectTransforms();
    }

    private void OnDestroy()
    {
        if (my_ObservableFloat != null)
            my_ObservableFloat.OnUpdate -= EventReceiver_OnUpdate;
    }


    void EventReceiver_OnUpdate(ObservableFloat _my_ObservableFloat)
    {
        updateRectTransforms();
    }


    void updateRectTransforms()
    {
        float _value = this.my_ObservableFloat.value;

        foreach(RectTransform _RectTransform in this.my_RectTransforms)
        {
            if(_RectTransform == null)
            {
                GlobalFunctions.printWarning("null _RectTransform???", this);
                continue;
            }

            if (control_width)
                _RectTransform.sizeDelta = new Vector2(_value,_RectTransform.sizeDelta.y);

            if (control_height)
                _RectTransform.sizeDelta = new Vector2(_RectTransform.sizeDelta.x,_value);

            Vector3 _anchored_position = _RectTransform.anchoredPosition;
            if (control_anchored_x)
                _anchored_position.x = _value;

            if (control_anchored_y)
                _anchored_position.y = _value;

            if (control_anchoed_z)
                _anchored_position.z = _value;

            _RectTransform.anchoredPosition = _anchored_position;

        }
    }
    
}
