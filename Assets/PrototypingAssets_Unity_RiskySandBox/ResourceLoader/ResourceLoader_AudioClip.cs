using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ResourceLoader_AudioClip : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] string path;

    public AudioClip loaded_AudioClip { get { return this.PRIVATE_loaded_AudioClip; } }
    [SerializeField] AudioClip PRIVATE_loaded_AudioClip;

    public List<AudioSource> my_AudioSources_READONLY { get { return this.PRIVATE_my_AudioSources; } }
    [SerializeField] List<AudioSource> PRIVATE_my_AudioSources;

    public float length
    {
        get
        {
            if (PRIVATE_loaded_AudioClip == null)
                return 0;
            return PRIVATE_loaded_AudioClip.length;
        }
    }

	

    // Start is called before the first frame update
    void Start()
    {
        loadAudioClip();
    }

    void loadAudioClip()
    {
        this.PRIVATE_loaded_AudioClip = Resources.Load<AudioClip>(path);
        foreach(AudioSource _AudioSource in this.PRIVATE_my_AudioSources)
        {
            if (_AudioSource == null)
                return;

            _AudioSource.clip = this.PRIVATE_loaded_AudioClip;

        }
    }


    public void playOneShot(AudioSource _AudioSource)
    {
        if(loaded_AudioClip == null)
        {
            GlobalFunctions.printError("loaded_AudioClip is null?",this);
            return;
        }

        if(_AudioSource == null)
        {
            GlobalFunctions.printError("_AudioSource is null???", this);
            return;
        }

        _AudioSource.PlayOneShot(this.loaded_AudioClip);

    }

}
