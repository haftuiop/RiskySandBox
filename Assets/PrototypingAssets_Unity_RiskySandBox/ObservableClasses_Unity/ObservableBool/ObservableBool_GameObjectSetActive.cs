using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableBool_GameObjectSetActive : MonoBehaviour
{

    [SerializeField] bool debugging;
    [SerializeField] ObservableBool my_ObservableBool;
    [SerializeField] bool invert;//if true the gameobjects are disabled if my_ObservableBool is true...
    [SerializeField] List<GameObject> my_GameObjects = new List<GameObject>();

    

    void OnEnable()
    {
        if (this.debugging)
            GlobalFunctions.print("", this);
        

        if (my_ObservableBool != null)
        {
            updateGameObjects();
            my_ObservableBool.OnUpdate += EventReceiver_OnUpdate;
        }
            
        else
            GlobalFunctions.printError("my_ObservableBool is null...", this);
    }

    private void OnDisable()//unsubscribe in Ondestroy sinse we may "disable" ourselves?
    {
        if (my_ObservableBool == null)
            return;

        my_ObservableBool.OnUpdate -= EventReceiver_OnUpdate;
    }

    void EventReceiver_OnUpdate(ObservableBool _ObservableBool)
    {
        if (this.debugging)
            GlobalFunctions.print("my_ObserableBool just changed! updateGameObjects()", this);
        updateGameObjects();
    }


    void updateGameObjects()
    {

        foreach(GameObject _GameObject in this.my_GameObjects)
        {
            if (_GameObject == null)//dont want to crash if this is the case...
                continue;

            if (invert == false)
                _GameObject.SetActive(my_ObservableBool.value);
            else
                _GameObject.SetActive(!my_ObservableBool.value);
        }
    }
}
