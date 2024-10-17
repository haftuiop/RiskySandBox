using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using Photon.Pun;

public partial class RiskySandBox_ChatSystem : MonoBehaviour
{

    //TODO - send the messages via the server (so that 


    [SerializeField] bool debugging;

    [SerializeField] ObservableBool PRIVATE_is_enabled;
    [SerializeField] ObservableInt PRIVATE_current_emote_message_menu_index;
    [SerializeField] ObservableInt PRIVATE_current_text_message_menu_index;

    [SerializeField] ObservableBool PRIVATE_show_chat_panels;

    [SerializeField] List<int> muted_Team_IDs = new List<int>();

    [SerializeField] ResourceLoader_ImageFolder emote_ImageFolder;
    [SerializeField] ObservableList<Texture2D> emote_Texture2Ds { get { return emote_ImageFolder.images; } }


    [SerializeField] List<TextMessageOption> text_message_options = new List<TextMessageOption>();

    [SerializeField] bool allow_swearing;

    [SerializeField] GameObject emote_message_button_prefab;
    [SerializeField] GameObject text_message_button_prefab;


    [SerializeField] UnityEngine.UI.Text emote_message_menu_index_Text;
    [SerializeField] UnityEngine.UI.Text text_message_menu_index_Text;

    [SerializeField] List<GameObject> instantiated_buttons = new List<GameObject>();

    [SerializeField] GameObject root_canvas;


    [SerializeField] Photon.Pun.PhotonView my_PhotonView { get { return GetComponent<PhotonView>(); } }


    /// <summary>
    /// remove awful text from message (e.g. racist, sexist (.....ist), anti religion, anti lgbt etc...) 
    /// </summary>
    string abuseFilter(string _message)
    {
        if (allow_swearing)
            return _message;

        //remove swearing from the message...
        return _message;
    }


    public void enable()
    {
        this.PRIVATE_is_enabled.value = true;
    }

    public void disable()
    {
        this.PRIVATE_is_enabled.value = false;
    }


    private void Awake()
    {
        this.PRIVATE_is_enabled.OnUpdate += delegate { updateChatMessageOptions(); };
        this.PRIVATE_current_emote_message_menu_index.OnUpdate += delegate { updateChatMessageOptions(); };
        this.PRIVATE_current_text_message_menu_index.OnUpdate += delegate { updateChatMessageOptions(); };


        this.PRIVATE_current_emote_message_menu_index.min_value = 0;

        if(this.emote_Texture2Ds.Count > 0)
            this.PRIVATE_current_emote_message_menu_index.max_value = Mathf.CeilToInt(this.emote_Texture2Ds.Count / 8);

        this.emote_ImageFolder.images.OnUpdate += EventReceiver_OnUpdate_emote_options;


        this.PRIVATE_current_text_message_menu_index.min_value = 0;
        if (this.text_message_options.Count > 0)
            this.PRIVATE_current_text_message_menu_index.max_value = this.text_message_options.Max(x => x.menu_index);


        RiskySandBox_Team_MuteButton.OnmuteButtonPressed += EventReceiver_OnmuteButtonPressed;


    }

    private void OnDestroy()
    {
        RiskySandBox_Team_MuteButton.OnmuteButtonPressed -= EventReceiver_OnmuteButtonPressed;
    }

    void EventReceiver_OnUpdate_emote_options()
    {
        this.PRIVATE_current_emote_message_menu_index.max_value = Mathf.CeilToInt(this.emote_Texture2Ds.Count / 8);
    }


    void updateChatMessageOptions()
    {
        foreach(GameObject _instantiated_GameObject in this.instantiated_buttons)
        {
            UnityEngine.Object.Destroy(_instantiated_GameObject.gameObject);
        }
        this.instantiated_buttons.Clear();


        this.emote_message_menu_index_Text.text = string.Format("{0}/{1}",this.PRIVATE_current_emote_message_menu_index.value,this.PRIVATE_current_emote_message_menu_index.max_value);
        this.text_message_menu_index_Text.text = string.Format("{0}/{1}", this.PRIVATE_current_text_message_menu_index.value, this.PRIVATE_current_text_message_menu_index.max_value);


        for(int i = 8 * this.PRIVATE_current_emote_message_menu_index; i < ((this.PRIVATE_current_emote_message_menu_index + 1) * 8); i += 1)
        {
            if (this.emote_Texture2Ds.Count <= i)
                continue;
            Texture2D _Texture2D = this.emote_Texture2Ds[i];

            int _row = i / 4;
            int _column = i % 4;

            UnityEngine.UI.Button _new_Button = UnityEngine.Object.Instantiate(emote_message_button_prefab, this.root_canvas.transform).GetComponent<UnityEngine.UI.Button>();

            this.instantiated_buttons.Add(_new_Button.gameObject);

            _new_Button.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(_Texture2D, new Rect(0, 0, _Texture2D.width, _Texture2D.height), new Vector2(0.5f, 0.5f));

            _new_Button.GetComponent<RectTransform>().anchoredPosition = new Vector2(25 + (50 * _column), -25 + (-50 * _row));
            

            int _i = i;
            _new_Button.onClick.AddListener(delegate { EventReceiver_OnclickEmoteButton(_i); });
        }

        for(int i = 0; i < this.text_message_options.Count(); i += 1)
        {
            TextMessageOption _message_option = this.text_message_options[i];

            if (_message_option.menu_index != this.PRIVATE_current_text_message_menu_index)
                continue;

            UnityEngine.UI.Button _new_Button = UnityEngine.Object.Instantiate(text_message_button_prefab, this.root_canvas.transform).GetComponent<UnityEngine.UI.Button>();

            this.instantiated_buttons.Add(_new_Button.gameObject);

            _new_Button.transform.GetComponentInChildren<UnityEngine.UI.Text>().text = _message_option.message;
            _new_Button.GetComponent<RectTransform>().anchoredPosition = new Vector2(100, -160 - (30 * _message_option.row));

            int _i = i;
            _new_Button.onClick.AddListener(delegate { EventReceiver_OnTextMessageButtonPressed(_i); });
        }
    }

    void EventReceiver_OnclickEmoteButton(int _button_index)
    {
        //send the message to the client...
        Debug.Log("just clicked the button for the emote at index " + _button_index);
        my_PhotonView.RPC("receiveEmoteMessage",RpcTarget.All,_button_index);//TODO - what if we have a specific person we want to send this to...

    }

    void EventReceiver_OnTextMessageButtonPressed(int _button_index)
    {
        Debug.Log("just clicked the button for the text message at index " + _button_index);
        my_PhotonView.RPC("receiveTextMessage", RpcTarget.All, _button_index);//TODO - what if we have a specific person we want to send this to...
    }

    void EventReceiver_OnmuteButtonPressed(RiskySandBox_Team _Team)
    {
        if(this.muted_Team_IDs.Contains(_Team.ID))
        {
            this.muted_Team_IDs.Remove(_Team.ID);
        }
        else
        {
            this.muted_Team_IDs.Add(_Team.ID);
        }
    }



    [Photon.Pun.PunRPC]
    void receiveEmoteMessage(int _index, PhotonMessageInfo _PhotonMessageInfo)
    {
        RiskySandBox_Team _sender_Team = RiskySandBox_HumanPlayer.GET_RiskySandBox_Team(_PhotonMessageInfo.Sender);
        if(_sender_Team == null)
        {
            if (this.debugging)
                GlobalFunctions.print("sender team is null??? returning", this);
            return;
        }

        if(muted_Team_IDs.Contains(_sender_Team.ID))
        {
            if (this.debugging)
                GlobalFunctions.print("sender_Team is muted... returning", this);
            return;
        }

        playEmoteMessage(_index, _sender_Team);
    }

    [Photon.Pun.PunRPC]
    void receiveTextMessage(int _index,PhotonMessageInfo _PhotonMessageInfo)
    {
        RiskySandBox_Team _sender_Team = RiskySandBox_HumanPlayer.GET_RiskySandBox_Team(_PhotonMessageInfo.Sender);
        if(_sender_Team == null)
        {
            if (this.debugging)
                GlobalFunctions.print("sender Team is null???? returning", this);
            return;
        }
        if(muted_Team_IDs.Contains(_sender_Team.ID))
        {
            if (this.debugging)
                GlobalFunctions.print("sender_Team is muted... returning...", this);
            return;
        }

        playTextMessage(_index, _sender_Team);
    }


    void playEmoteMessage(int _index,RiskySandBox_Team _from)
    {
        if (this.debugging)
            GlobalFunctions.print("", this);

        RiskySandBox_Team_ChatSystemUI _UI = RiskySandBox_Team_ChatSystemUI.GET_UI(_from);
        if (_UI == null)//TODO - print WTF?!?!?!
            return;

        _UI.playEmoteMessage(this.emote_Texture2Ds[_index], 2f);

    }

    void playTextMessage(int _index,RiskySandBox_Team _from)
    {
        if (this.debugging)
            GlobalFunctions.print("", this);
        string _message = abuseFilter(this.text_message_options[_index].message);

        RiskySandBox_Team_ChatSystemUI _UI = RiskySandBox_Team_ChatSystemUI.GET_UI(_from);
        if (_UI == null)//TODO - print WTF?!?!?!
            return;

        _UI.playTextMessage(this.text_message_options[_index].message, this.text_message_options[_index].duration);
    }





    [Serializable]
    struct TextMessageOption
    {
        public string message;
        public int menu_index;
        public int row;
        public float duration;
    }
}
