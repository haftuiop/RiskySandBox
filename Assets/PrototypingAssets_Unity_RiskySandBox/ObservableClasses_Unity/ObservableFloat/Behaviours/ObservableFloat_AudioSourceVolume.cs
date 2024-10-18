using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableFloat_AudioSourceVolume : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableFloat my_ObservableFloat;

    [SerializeField] List<AudioSource> my_AudioSources = new List<AudioSource>();



    private void Awake()
    {
        if(my_ObservableFloat == null)
        {
            GlobalFunctions.printError("my_ObservableFloat == null???", this);
            return;
        }

        if(my_ObservableFloat.min_value != 0f)
            GlobalFunctions.printWarning("my_ObservableFloat.min_value != 0??? is this intentional???", this);

        if(my_ObservableFloat.max_value != 1f)
            GlobalFunctions.printWarning("my_ObservableFloat.max_value != 1??? is this intentional???", this);
       

        this.my_ObservableFloat.OnUpdate += EventReceiver_OnUpdate;

        updateAudioSources();
    }

    private void OnDestroy()
    {
        if (this.my_ObservableFloat != null)
            this.my_ObservableFloat.OnUpdate -= EventReceiver_OnUpdate;
    }

    void EventReceiver_OnUpdate(ObservableFloat _my_ObservableFloat)
    {
        updateAudioSources();
    }


    void updateAudioSources()
    {
        foreach(AudioSource _AudioSource in this.my_AudioSources)
        {
            if(_AudioSource == null)
            {
                GlobalFunctions.print("null AudioSource???", this);
                continue;
            }

            _AudioSource.volume = this.my_ObservableFloat;
        }
    }
}
