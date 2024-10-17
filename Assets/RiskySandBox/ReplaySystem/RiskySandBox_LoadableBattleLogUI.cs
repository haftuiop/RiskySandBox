using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_LoadableBattleLogUI : MonoBehaviour
{
    [SerializeField] bool debugging;

    public ObservableString start_date { get { return this.PRIVATE_start_date; } }
    [SerializeField] ObservableString PRIVATE_start_date;

    public ObservableString game_ID { get { return this.PRIVATE_game_ID; } }
    [SerializeField] ObservableString PRIVATE_game_ID;

    //todo put in some more info to make it easier for players to remeber what game it was e.g. map and players???
    //or have a little "preview" button...


    public event Action OnloadButtonPressed;

    public void EventReceiver_OnLoadButtonPressed()
    {
        OnloadButtonPressed?.Invoke();
    }

}
