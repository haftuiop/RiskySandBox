using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;


/// <summary>
/// rename this to something like joinableserversinfohumanui  
/// </summary>
public partial class RiskySandBox_RoomInfoUI : MonoBehaviour
{

    public static event Action<RiskySandBox_RoomInfoUI> OnJoinButtonPressed_STATIC;


    [SerializeField] bool debugging;



    public ObservableString room_name { get { return this.PRIVATE_room_name; } }//TODO - rename to debug_server_name???? or ip or whatever...
    [SerializeField] ObservableString PRIVATE_room_name;

    private void Awake()
    {
        this.room_name.OnUpdate += delegate { this.gameObject.name = "RoomInfoUI for " + this.room_name; };
    }

    public void EventReceiver_OnJoinButtonPressed()
    {
        OnJoinButtonPressed_STATIC?.Invoke(this);
    }
    
}
