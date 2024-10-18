using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableInt_SetGameObjectStates : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] List<GameObject> my_GameObjects = new List<GameObject>();

    [SerializeField] ObservableInt index;

    [SerializeField] bool index_state = true;
    [SerializeField] bool others_state = false;
    [SerializeField] bool control_index_minmax_values = true;


    private void OnEnable()
    {
        if(this.index == null)
        {
            GlobalFunctions.printError("this.index == null", this);
            return;
        }

        if(control_index_minmax_values)
        {
            index.min_value = 0;
            index.max_value = my_GameObjects.Count() - 1;
        }

        this.index.OnUpdate += EventReceiver_OnVariableUpdate_index;
        updateGameObjects();
    }

    private void OnDisable()
    {
        if(this.index != null)
            this.index.OnUpdate -= EventReceiver_OnVariableUpdate_index;
    }

    void EventReceiver_OnVariableUpdate_index(ObservableInt _index)
    {
        updateGameObjects();
    }


    void updateGameObjects()
    {
        for(int i = 0; i < my_GameObjects.Count; i += 1)
        {
            GameObject _GO = my_GameObjects[i];

            if(_GO == null)
            {
                GlobalFunctions.printWarning("null GameObject", this);
                continue;
            }

            if (i == index)
                _GO.SetActive(index_state);
            else
                _GO.SetActive(others_state);
        }
    }


    public void SET_my_GameObjects(IEnumerable<GameObject> _new_content)
    {
        this.my_GameObjects = new List<GameObject>(_new_content);
        updateGameObjects();

        if(control_index_minmax_values)
        {
            this.index.min_value = 0;
            this.index.max_value = this.my_GameObjects.Count;
        }
    }


}
