using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_ShopItemUI : MonoBehaviour
{
    [SerializeField] bool debugging;

    public ObservableString item_type;
    public ObservableInt item_price;
    public ObservableBool can_purchase;
    public ObservableInt current_quantity;





    public void EventReceiver_OnpurchaseButtonPressed()
    {
        //ask the local player to try and buy the item...
        RiskySandBox_HumanPlayer.local_player.TRY_purchaseItem(this.item_type,1);
    }





}
