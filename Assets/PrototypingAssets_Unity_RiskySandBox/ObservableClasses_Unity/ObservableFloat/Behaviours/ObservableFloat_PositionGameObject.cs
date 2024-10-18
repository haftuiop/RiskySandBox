using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableFloat_PositionGameObject : MonoBehaviour
{
    [SerializeField] bool debugging;


    [SerializeField] ObservableFloat my_ObservableFloat;

    [SerializeField] List<GameObject> my_GameObjects = new List<GameObject>();


    [SerializeField] bool local_position;

    [SerializeField] bool x;
    [SerializeField] bool y;
    [SerializeField] bool z;


    private void OnEnable()
    {
        Debug.LogWarning("deprecated...", this);
        if (this.debugging)
            GlobalFunctions.print("", this);

        if (my_ObservableFloat != null)
        {
            updateGameObjects();//immediatley adjust the scale...
            my_ObservableFloat.OnUpdate += EventReceiver_OnUpdate;
        }
            
        else
            GlobalFunctions.printError("my_ObservableFloat is null...", this);

    }

    private void OnDisable()
    {
        if (this.debugging)
            GlobalFunctions.print("", this);
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

        foreach(GameObject _GameObject in this.my_GameObjects)
        {
            if (_GameObject == null)
                continue;

            Transform _Transform = _GameObject.transform;

            Vector3 _new_pos;

            if (this.local_position == true)
                _new_pos = _Transform.localPosition;
            else
                _new_pos = transform.position;


            if (x == true)
                _new_pos.x = _value;

            if (y == true)
                _new_pos.y = _value;

            if (z == true)
                _new_pos.z = _value;

            if (this.local_position)
                _Transform.localPosition = _new_pos;
            else
                _Transform.position = _new_pos;

        }
    }



}
