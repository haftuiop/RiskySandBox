using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableVector3_ScaleGameObject : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] ObservableVector3 my_ObservableVector3;
    [SerializeField] List<GameObject> my_GameObjects = new List<GameObject>();



    private void OnEnable()
    {
        GlobalFunctions.printWarning("deprecated...", this);

        if (my_ObservableVector3 == null)
        {
            GlobalFunctions.printError("my ObservableVector3 is null!",this);
            return;
        }
        my_ObservableVector3.OnUpdate += EventReceiver_OnUpdate;
        updateGameObjects();
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

        foreach (GameObject _GameObject in this.my_GameObjects)
        {
            if (_GameObject == null)
                continue;
            _GameObject.transform.localScale = this.my_ObservableVector3.value;
        }
    }


}
