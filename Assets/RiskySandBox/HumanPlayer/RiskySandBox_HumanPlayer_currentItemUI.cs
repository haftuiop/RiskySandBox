using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_HumanPlayer_currentItemUI : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableBool PRIVATE_root_state;
    [SerializeField] RiskySandBox_HumanPlayer my_HumanPlayer;
    ObservableString current_Item { get { return my_HumanPlayer.current_Item; } }

    [SerializeField] RiskySandBox_Team my_Team { get { return my_HumanPlayer.my_Team; } }

    List<string> purchased_item_types { get { return my_Team.purchased_shop_item_types; } }

    //so there are a few items in the game that players will want to use...
    //you have landmines, airstrikes and nukes....


    [SerializeField] UnityEngine.UI.Text current_item_Text;
    [SerializeField] UnityEngine.UI.RawImage current_item_background;



    public void OncancelButtonPressed()
    {
        this.my_HumanPlayer.current_Item.value = "";
        this.PRIVATE_root_state.value = false;
    }


    public void OnnextItemButtonPressed()
    {
        if (my_Team == null)
            return;

        if (purchased_item_types.Count() == 0)
            return;

        if(purchased_item_types.Count() == 1)
        {
            my_HumanPlayer.current_Item.value = this.purchased_item_types[0];
            return;
        }

        //bump by one...
        //wrap round to the beginnning...
        //select
        int _current_index = this.purchased_item_types.IndexOf(this.current_Item);
        _current_index += 1;

        if (_current_index >= this.purchased_item_types.Count())
            _current_index = 0;

        this.current_Item.value = this.purchased_item_types[_current_index];


    }

    public void OnpreviousItemButtonPressed()
    {
        if (my_Team == null)
            return;

        if (purchased_item_types.Count() == 0)
            return;

        if(this.purchased_item_types.Count() == 1)
        {
            this.current_Item.value = this.purchased_item_types[0];
            return;
        }

        int _current_index = this.purchased_item_types.IndexOf(this.current_Item);
        _current_index -= 1;

        if (_current_index < 0)
            _current_index = this.purchased_item_types.Count() - 1;

        this.current_Item.value = this.purchased_item_types[_current_index];

    }

    private void Awake()
    {
        this.current_Item.OnUpdate += EventReceiver_OnVariableUpdate_current_Item;
    }



    void EventReceiver_OnVariableUpdate_current_Item(ObservableString _current_Item)
    {
        this.PRIVATE_root_state.value = _current_Item.value != "";
    }




}
