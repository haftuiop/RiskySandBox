using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableInt_MusicTrackController : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableInt track_index;

    [SerializeField] List<AudioSource> my_AudioSources = new List<AudioSource>();
    [SerializeField] List<AudioClip> music_clips = new List<AudioClip>();


    private void Awake()
    {
        if(track_index == null)
        {
            GlobalFunctions.printError("track_index == null", this);
            return;
        }

        track_index.min_value = 0;
        track_index.max_value = music_clips.Count() - 1;

        this.track_index.OnUpdate += EventReceiver_OnVariableUpdate_track_index;

        updateAudioSources();
    }

    private void OnDestroy()
    {
        if (track_index != null)
            track_index.OnUpdate -= EventReceiver_OnVariableUpdate_track_index;
    }

    void EventReceiver_OnVariableUpdate_track_index(ObservableInt _track_index)
    {
        updateAudioSources();
    }

    void updateAudioSources()
    {
        AudioClip _AudioClip = music_clips[track_index];

        foreach(AudioSource _AudioSource in my_AudioSources)
        {
            if(_AudioSource == null)
            {
                GlobalFunctions.printWarning("null AudioSource???", this);
                continue;
            }

            _AudioSource.clip = _AudioClip;
            _AudioSource.Play();
        }
    }
}
