using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

/// <summary>
/// increases and decreases the ObservableFloat.value up to the maximum then back down to the minimum
/// </summary>
public partial class ObservableFloat_Oscillator : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableFloat my_ObservableFloat;

    [SerializeField] ObservableBool enable_behaviour;

    [SerializeField] bool increasing = true;

    [SerializeField] float period = 1f;

    private void Awake()
    {
        my_ObservableFloat.OnclampToMin += EventReceiver_OnclampMinValue;
        my_ObservableFloat.OnclampToMax += EventReceiver_OnclampMaxValue;
    }

    private void OnDestroy()
    {
        if (my_ObservableFloat != null)
        {
            my_ObservableFloat.OnclampToMin -= EventReceiver_OnclampMinValue;
            my_ObservableFloat.OnclampToMax -= EventReceiver_OnclampMaxValue;
        }
    }

    private void Update()
    {
        if (enable_behaviour == false)
            return;

        float _delta = Time.deltaTime * 2 * (my_ObservableFloat.max_value - my_ObservableFloat.min_value) / this.period;

        if (this.increasing == false)
            _delta = _delta * -1;

        this.my_ObservableFloat.value += _delta;

    }



    void EventReceiver_OnclampMinValue(ObservableFloat _my_ObservableFloat)
    {
        this.increasing = !this.increasing;
    }


    void EventReceiver_OnclampMaxValue(ObservableFloat _my_ObservableFloat)
    {
        this.increasing = !this.increasing;
    }

}
