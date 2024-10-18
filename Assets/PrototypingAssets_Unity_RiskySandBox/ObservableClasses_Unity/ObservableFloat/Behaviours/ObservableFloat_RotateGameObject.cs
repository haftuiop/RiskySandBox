using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableFloat_RotateGameObject : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableFloat my_ObservableFloat;
    [SerializeField] List<GameObject> my_GameObjects = new List<GameObject>();


    [SerializeField] bool x;
    [SerializeField] bool y;
    [SerializeField] bool z;
    [SerializeField] bool local_rotation;



    private void OnEnable()
    {
        Debug.LogWarning("deprecated...", this);
        if (my_ObservableFloat == null)
        {
            Debug.LogWarning("WARNING - why is my_ObservableFloat null...");
            return;
        }

        
        my_ObservableFloat.OnUpdate += EventReceiver_OnUpdate;
        updateGameObjects();
        
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
        Vector3 _rotation = new Vector3(0, 0, 0);
        float _value = this.my_ObservableFloat.value;

        if(this.x == true)
        {
            _rotation.x = _value;
        }

        if (this.y == true)
        {
            _rotation.y = _value;
        }

        if (this.z == true)
        {
            _rotation.z = _value;
        }

        if(this.local_rotation == true)
        {
            foreach(GameObject _GameObject in this.my_GameObjects)
            {
                if (_GameObject == null)
                    continue;
                _GameObject.transform.localEulerAngles = _rotation;
            }
            
        }
        else
        {
            foreach(GameObject _GameObject in this.my_GameObjects)
            {
                if (_GameObject == null)
                    continue;
                _GameObject.transform.eulerAngles = _rotation;
            }
            
        }



    }






}
