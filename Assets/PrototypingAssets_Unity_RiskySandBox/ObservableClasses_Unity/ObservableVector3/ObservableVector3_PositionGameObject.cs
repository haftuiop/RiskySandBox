using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableVector3_PositionGameObject : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] bool local_position;
     
    [SerializeField] ObservableVector3 my_ObservableVector3;


    [SerializeField] List<GameObject> my_GameObjects = new List<GameObject>();

    


    private void OnEnable()
    {
        GlobalFunctions.printWarning("deprecated...", this);
        if (my_ObservableVector3 != null)
        {
            updateGameObjects();
            my_ObservableVector3.OnUpdate += EventReceiver_OnUpdate;
        }
        else
            GlobalFunctions.printError("my_ObservableVector3 is null!", this);
           

    }

    private void OnDisable()
    {
        if (my_ObservableVector3 == null)
            return;
        my_ObservableVector3.OnUpdate -= EventReceiver_OnUpdate;

    }



    void EventReceiver_OnUpdate(ObservableVector3 _my_ObservableVector3)
    {
        updateGameObjects();
    }





    void updateGameObjects()
    {
        Vector3 _value = my_ObservableVector3.value;

        foreach(GameObject _GameObject in this.my_GameObjects)
        {
            if (_GameObject == null)
                continue;

            if (this.local_position)
                _GameObject.transform.localPosition = _value;
            else
                _GameObject.transform.position = _value;

        }

    }


}
