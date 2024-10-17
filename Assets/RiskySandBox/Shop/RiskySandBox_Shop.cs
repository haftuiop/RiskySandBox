using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Shop : MonoBehaviour
{
    public static RiskySandBox_Shop instance;


    [SerializeField] bool debugging;



    private void Awake()
    {
        instance = this;
    }

    public void TRY_purchaseItem(RiskySandBox_Team _buyer,string _item_type,int _amount)
    {
        if(PrototypingAssets.run_server_code.value == false)
        {
            GlobalFunctions.printError("not the server??? why is this happening...", this);
            return;
        }

        //TODO - replay system check... (if enabled) we shouldn't be purchasing items instead just give the item???


        if (_buyer == null)
        {
            GlobalFunctions.printError("why is _buyer null????", this);
            return;
        }


        if (this.debugging)
            GlobalFunctions.print("'{0}' is buying {1} of item {2}",this);

        for(int i = 0; i < _amount; i += 1)
        {
            int _buyer_price = _buyer.GET_itemPrice(_item_type);

            if(_buyer.shop_credits  >= _buyer_price)
            {
                _buyer.purchased_shop_items.Add(_item_type);
                _buyer.shop_credits.value -= _buyer_price;
            }
        }


    }
}
