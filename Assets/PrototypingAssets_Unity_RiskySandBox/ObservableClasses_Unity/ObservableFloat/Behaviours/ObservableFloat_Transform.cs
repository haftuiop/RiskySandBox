using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableFloat_Transform : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableFloat my_ObservableFloat;
    [SerializeField] List<Transform> my_Transforms = new List<Transform>();

    [SerializeField] bool local_position;
    [SerializeField] bool control_position_x;
    [SerializeField] bool control_position_y;
    [SerializeField] bool control_position_z;


    [SerializeField] bool local_rotation;
    [SerializeField] bool control_rotation_x;
    [SerializeField] bool control_rotation_y;
    [SerializeField] bool control_rotation_z;

    [SerializeField] bool local_scale = true;
    [SerializeField] bool control_scale_x;
    [SerializeField] bool control_scale_y;
    [SerializeField] bool control_scale_z;


    private void Awake()
    {
        if(my_ObservableFloat == null)
        {
            if (this.gameObject.TryGetComponent<ObservableFloat>(out this.my_ObservableFloat) == false)
            {
                GlobalFunctions.printError("unable to find my_ObservableFloat!!!", this);
                return;
            }
        }

        
        my_ObservableFloat.OnUpdate += EventReceiver_OnUpdate;
        updateTransforms();
        
    }

    private void OnDestroy()
    {
        if(my_ObservableFloat != null)
        {
            my_ObservableFloat.OnUpdate -= EventReceiver_OnUpdate;
        }
    }



    void EventReceiver_OnUpdate(ObservableFloat _my_ObservableFloat)
    {
        this.updateTransforms();
    }

    void updateTransforms()
    {
        float _value = this.my_ObservableFloat.value;

        foreach (Transform _Transform in this.my_Transforms)
        {
            if (this.control_position_x || this.control_position_y || this.control_position_z)
            {
                Vector3 _position;

                if (this.local_position)
                    _position = _Transform.localPosition;
                else
                    _position = _Transform.position;

                if (this.control_position_x)
                    _position.x = _value;

                if (this.control_position_y)
                    _position.y = _value;

                if (this.control_position_z)
                    _position.z = _value;

                if (this.local_position)
                    _Transform.localPosition = _position;
                else
                    _Transform.position = _position;
            }

            if(this.control_rotation_x || this.control_rotation_y || this.control_rotation_z)
            {
                Vector3 _angles;

                if (this.local_rotation)
                    _angles = _Transform.localEulerAngles;
                else
                    _angles = _Transform.eulerAngles;

                if (this.control_rotation_x)
                    _angles.x = _value;

                if (this.control_rotation_y)
                    _angles.y = _value;

                if (this.control_rotation_z)
                    _angles.z = _value;

                if (this.local_rotation)
                    _Transform.localEulerAngles = _angles;
                else
                    _Transform.eulerAngles = _angles;
            }

            if(this.control_scale_x || this.control_scale_y || this.control_scale_z)
            {
                Vector3 _scale;

                if (this.local_scale)
                    _scale = _Transform.localScale;
                else
                {
                    _scale = _Transform.lossyScale;
                    Debug.LogWarning("unimplemented...");
                    continue;
                }

                if (this.control_scale_x)
                    _scale.x = _value;

                if (this.control_scale_y)
                    _scale.y = _value;

                if (this.control_scale_z)
                    _scale.z = _value;

                if (this.local_scale)
                    _Transform.localScale = _scale;
                else
                    Debug.LogWarning("unimplemented...");
            }

        }
    }
}
