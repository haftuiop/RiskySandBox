using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableBool_UIManager : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] ObservableBool my_ObservableBool;

    [SerializeField] private List<UnityEngine.UI.Toggle> UI_Toggles = new List<UnityEngine.UI.Toggle>();


    private void Awake()
    {
        if (my_ObservableBool == null)
        {
            if (this.gameObject.TryGetComponent(out my_ObservableBool) == false)
            {
                GlobalFunctions.printError("unable to find my_ObservableBool!", this);
                return;
            }
        }
        
        my_ObservableBool.OnUpdate += EventReceiver_OnUpdate;
        updateUIElements();

        foreach (UnityEngine.UI.Toggle _Toggle in this.UI_Toggles)
        {
            if(_Toggle == null)
            {
                GlobalFunctions.printWarning("null Toggle!",this);
                continue;
            }
            _Toggle.onValueChanged.AddListener(updateFromToggle);//listen to all the toggles... (when the value of the toggle changes we must update this.value!
        }
        
    }

    private void OnDestroy()
    {
        if(my_ObservableBool != null)
            my_ObservableBool.OnUpdate -= EventReceiver_OnUpdate;

        //TODO - unsub to toggles
    }

    void EventReceiver_OnUpdate(ObservableBool _my_ObservableBool)
    {
        updateUIElements();
    }

    void updateUIElements()
    {
        foreach (UnityEngine.UI.Toggle _Toggle in this.UI_Toggles)
        {
            if (_Toggle == null)
            {
                GlobalFunctions.printWarning("null toggle!", this);
                continue;
            }
            _Toggle.SetIsOnWithoutNotify(this.my_ObservableBool.value);
        }
    }

    //toggle value just changed! - lets update the value to match!
    void updateFromToggle(bool _incoming_value)
    {
        this.my_ObservableBool.value = _incoming_value;
    }
}
