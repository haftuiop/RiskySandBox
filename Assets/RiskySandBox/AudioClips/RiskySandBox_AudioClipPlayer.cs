using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public partial class RiskySandBox_AudioClipPlayer : MonoBehaviour
{

    public static string audio_clips_folder { get { return Application.streamingAssetsPath + "/RiskySandBox/AudioClips"; } }

    [SerializeField] bool debugging;

    [SerializeField] AudioSource missile_AudioSource;


    [SerializeField] AudioSource my_AudioSource { get { return GetComponent<AudioSource>(); } }

    [SerializeField] ResourceLoader_AudioClip deploy_AudioClipLoader;
    [SerializeField] ResourceLoader_AudioClip attack_AudioClipLoader;
    [SerializeField] ResourceLoader_AudioClip fortify_AudioClipLoader;
    [SerializeField] ResourceLoader_AudioClip capture_AudioClipLoader;

    [SerializeField] ResourceLoader_AudioClip card_selected_AudioClipLoader;



    [SerializeField] List<AudioClip> nuke_AudioClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> landmine_detonate_AudioClips = new List<AudioClip>();
    

    [SerializeField] float next_ai_deploy_time;
    [SerializeField] float next_ai_attack_time;
    [SerializeField] float next_ai_capture_time;
    [SerializeField] float next_ai_fortify_time;


    private void Awake()
    {
        if (this.debugging)
            GlobalFunctions.print("called Awake!",this);

        RiskySandBox_Team.Ondeploy += EventReceiver_Ondeploy;
        RiskySandBox_Team.Onattack += EventReceiver_Onattack;
        RiskySandBox_Team.Onfortify += EventReceiver_Onfortify;
        RiskySandBox_Team.Oncapture += EventReceiver_Oncapture;

        Missile.all_instances.OnUpdate += MissileEventReceiver_OnUpdate_all_instances;
        Missile.OnComplete_STATIC += MissileEventReceiver_OnComplete_STATIC;

        RiskySandBox_ItemsManager.OndetonateLandMine += EventReceiver_Ondetonatelandmine;

        RiskySandBox_TerritoryCard.OnVariableUpdate_is_selected_STATIC += RiskySandBox_TerritoryCardEventReceiver_OnVariableUpdate_is_selected;
    }

    private void OnDestroy()
    {
        if (this.debugging)
            GlobalFunctions.print("called OnDestroy!", this);

        RiskySandBox_Team.Ondeploy -= EventReceiver_Ondeploy;
        RiskySandBox_Team.Onattack -= EventReceiver_Onattack;
        RiskySandBox_Team.Onfortify -= EventReceiver_Onfortify;
        RiskySandBox_Team.Oncapture -= EventReceiver_Oncapture;

        Missile.all_instances.OnUpdate -= MissileEventReceiver_OnUpdate_all_instances;
        Missile.OnComplete_STATIC -= MissileEventReceiver_OnComplete_STATIC;

        RiskySandBox_ItemsManager.OndetonateLandMine -= EventReceiver_Ondetonatelandmine;

        RiskySandBox_TerritoryCard.OnVariableUpdate_is_selected_STATIC -= RiskySandBox_TerritoryCardEventReceiver_OnVariableUpdate_is_selected;
    }


    void TRY_playRandomClip(List<AudioClip> _options)
    {
        if (_options.Count == 0)
            return;

        AudioClip _random_clip = GlobalFunctions.GetRandomItem(_options);
        this.my_AudioSource.PlayOneShot(_random_clip);
    }


    void EventReceiver_Ondeploy(RiskySandBox_Team.EventInfo_Ondeploy _EventInfo)
    {
        //TODO - if the player doesnt want the sounds to play? e.g. they have disabled the sfx? or the deploy sfx? - return

        RiskySandBox_HumanPlayer _HumanPlayer = RiskySandBox_HumanPlayer.GET_RiskySandBox_HumanPlayer(_EventInfo.Team);

        
        if(_HumanPlayer == null)
        {
            //essentially the ai may make several small deploys in a very short space of time... we dont want to "spam" the deploy sfx if this happens
            if (Time.time < next_ai_deploy_time)
            {
                if (this.debugging)
                    GlobalFunctions.print("ignoring this event (to not spam the deploy sound)", this);
                return;
            }
            next_ai_deploy_time = Time.time + this.deploy_AudioClipLoader.length;
        }
        if (this.debugging)
            GlobalFunctions.print("trying to play a random deploy_AudioClips",this);

        this.deploy_AudioClipLoader.playOneShot(this.my_AudioSource);
    }

    void EventReceiver_Onattack(RiskySandBox_Team.EventInfo_Onattack _EventInfo)
    {
        //TODO - if the player doesnt want the sounds to play? e.g. they have disabled the sfx? or the deploy sfx? - return

        RiskySandBox_HumanPlayer _HumanPlayer = RiskySandBox_HumanPlayer.GET_RiskySandBox_HumanPlayer(_EventInfo.attacking_Team);

        if(_HumanPlayer == null)
        { 
            if (Time.time < next_ai_attack_time)
            {
                if (this.debugging)
                    GlobalFunctions.print("ignoring this event (to not spam the attack AudioClip)",this);
                return;
            }
                
            next_ai_attack_time = Time.time + this.attack_AudioClipLoader.length;
        }

        this.attack_AudioClipLoader.playOneShot(this.my_AudioSource);
    }

    void EventReceiver_Onfortify(RiskySandBox_Team.EventInfo_Onfortify _EventInfo)
    {
        //TODO - if the player doesnt want the sounds to play? e.g. they have disabled the sfx? or the deploy sfx? - return

        RiskySandBox_HumanPlayer _HumanPlayer = RiskySandBox_HumanPlayer.GET_RiskySandBox_HumanPlayer(_EventInfo.Team);
        if(_HumanPlayer == null)
        {
            if (Time.time < next_ai_fortify_time)
            {
                if (this.debugging)
                    GlobalFunctions.print("ingoreing this event (to not spam the fortify AudioClip)",this);
                return;
            }
            next_ai_fortify_time = Time.time + fortify_AudioClipLoader.length;
        }

        fortify_AudioClipLoader.playOneShot(this.my_AudioSource);
    }

    void EventReceiver_Oncapture(RiskySandBox_Team.EventInfo_Oncapture _EventInfo)
    {
        //TODO - if the player doesnt want the sounds to play? e.g. they have disabled the sfx? or the deploy sfx? - return

        
        RiskySandBox_HumanPlayer _HumanPlayer = RiskySandBox_HumanPlayer.GET_RiskySandBox_HumanPlayer(_EventInfo.Team);

        if(_HumanPlayer == null)
        {
            if (Time.time < next_ai_capture_time)
            {
                if (this.debugging)
                    GlobalFunctions.print("ignoring this event... (to not spam the capture AudioClip)",this);
                return;
            }
                
            next_ai_capture_time = Time.time + capture_AudioClipLoader.length;
        }

        capture_AudioClipLoader.playOneShot(this.my_AudioSource);
    }

    void RiskySandBox_TerritoryCardEventReceiver_OnVariableUpdate_is_selected(RiskySandBox_TerritoryCard _TerritoryCard)
    {
        this.card_selected_AudioClipLoader.playOneShot(this.my_AudioSource);
    }

    void MissileEventReceiver_OnUpdate_all_instances()
    {
        bool _enabled = Missile.all_instances.Count > 0;

        this.missile_AudioSource.enabled = _enabled;

        if (_enabled)
        {
            this.missile_AudioSource.Play();
        }
    }

    void MissileEventReceiver_OnComplete_STATIC(Missile _Missile)
    {
        if(_Missile.on_complete_AudioClip != null)
            this.my_AudioSource.PlayOneShot(_Missile.on_complete_AudioClip);

    }


    void EventReceiver_Ondetonatelandmine(RiskySandBox_Tile _Tile)
    {
        AudioClip _random_clip = GlobalFunctions.GetRandomItem(this.landmine_detonate_AudioClips);
        if (_random_clip == null)
        { 
            if (this.debugging)
                GlobalFunctions.print("detected a landmine detonation... _random clip was null??", this);

            return;
        }

        if (this.debugging)
            GlobalFunctions.print("detected a landmine detonation... playing " + _random_clip, this);
        this.my_AudioSource.PlayOneShot(_random_clip);
    }





}