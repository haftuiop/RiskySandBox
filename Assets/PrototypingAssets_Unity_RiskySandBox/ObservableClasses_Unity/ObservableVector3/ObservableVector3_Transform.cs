using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableVector3_Transform : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableVector3 my_ObservableVector3;

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
        if(my_ObservableVector3 == null)
        {
            if(this.gameObject.TryGetComponent<ObservableVector3>(out this.my_ObservableVector3) == false)
            {
                GlobalFunctions.printError("unable to find my_ObservableVector3", this);
                return;
            }
        }

        my_ObservableVector3.OnUpdate += EventReceiver_OnUpdate;
        updateTransforms();
    }

    void OnDestroy()
    {
        if (this.my_ObservableVector3 != null)
            my_ObservableVector3.OnUpdate -= EventReceiver_OnUpdate;
    }



    void EventReceiver_OnUpdate(ObservableVector3 _my_ObservableVector3)
    {
        updateTransforms();
    }

    void updateTransforms()
    {
        Vector3 _value = this.my_ObservableVector3.value;

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
                    _position.x = _value.x;

                if (this.control_position_y)
                    _position.y = _value.y;

                if (this.control_position_z)
                    _position.z = _value.z;

                if (this.local_position)
                    _Transform.localPosition = _position;
                else
                    _Transform.position = _position;
            }

            if (this.control_rotation_x || this.control_rotation_y || this.control_rotation_z)
            {
                Vector3 _euler_angles;

                if (this.local_rotation)
                    _euler_angles = _Transform.localEulerAngles;
                else
                    _euler_angles = _Transform.eulerAngles;

                if (this.control_rotation_x)
                    _euler_angles.x = _value.x;

                if (this.control_rotation_y)
                    _euler_angles.y = _value.y;

                if (this.control_rotation_z)
                    _euler_angles.z = _value.z;

                if (this.local_rotation)
                    _Transform.localEulerAngles = _euler_angles;
                else
                    _Transform.eulerAngles = _euler_angles;
            }

            if (this.control_scale_x || this.control_scale_y || this.control_scale_z)
            {
                Vector3 _scale;

                if (this.local_scale)
                    _scale = _Transform.localScale;
                else
                {
                    _scale = _Transform.lossyScale;
                    GlobalFunctions.printWarning("unimplemented...", this);
                    continue;
                }

                if (this.control_scale_x)
                    _scale.x = _value.x;

                if (this.control_scale_y)
                    _scale.y = _value.y;

                if (this.control_scale_z)
                    _scale.z = _value.z;

                if (this.local_scale)
                    _Transform.localScale = _scale;
                else
                {
                    GlobalFunctions.printWarning("unimplemented...", this);
                    continue;
                }
            }
        }
    }
}
