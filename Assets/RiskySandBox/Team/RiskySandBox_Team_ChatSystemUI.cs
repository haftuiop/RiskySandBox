using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team_ChatSystemUI : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] RiskySandBox_Team my_Team;

    [SerializeField] UnityEngine.UI.Text text_message_Text;
    [SerializeField] UnityEngine.UI.RawImage emote_message_RawImage;


    [SerializeField] ObservableBool show_emote_Image;
    [SerializeField] ObservableBool show_text_message_Text;


    float hide_emote_image_time;
    float hide_text_message_time;

    float current_time;


    public static Dictionary<RiskySandBox_Team, RiskySandBox_Team_ChatSystemUI> CACHE_GET_UI = new Dictionary<RiskySandBox_Team, RiskySandBox_Team_ChatSystemUI>();
    public static RiskySandBox_Team_ChatSystemUI GET_UI(RiskySandBox_Team _Team)
    {
        if(CACHE_GET_UI.TryGetValue(_Team, out RiskySandBox_Team_ChatSystemUI _UI))
            return _UI;
        return null;
    }


    private void Awake()
    {
        CACHE_GET_UI[this.my_Team] = this;
    }

    private void OnDestroy()
    {
        CACHE_GET_UI.Remove(this.my_Team);
    }


    public void playTextMessage(string _message,float _duration)
    {
        this.show_emote_Image.value = false;

        this.show_text_message_Text.value = true;
        this.hide_text_message_time = this.current_time + _duration;
        this.text_message_Text.text = _message;
    }

    public void playEmoteMessage(Texture2D _emote_Texture,float _duration)
    {
        this.show_text_message_Text.value = false;

        this.show_emote_Image.value = true;
        this.hide_emote_image_time = this.current_time + _duration;
        this.emote_message_RawImage.texture = _emote_Texture;

    }


    void Update()
    {
        this.current_time += Time.deltaTime;

        if(this.current_time > this.hide_emote_image_time && this.show_emote_Image.value == true)
        {
            this.show_emote_Image.value = false;
        }

        if(this.current_time > this.hide_text_message_time && this.show_text_message_Text.value == true)
        {
            this.show_text_message_Text.value = false;
        }
    }




}
