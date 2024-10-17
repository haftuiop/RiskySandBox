using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team
{



    public bool TRY_useItem(RiskySandBox_Tile _Tile,string _item_type)
    {
        //ok!
        //first we check if we have one!
        if (this.purchased_shop_items.Contains(_item_type) == false)
        {
            if (this.debugging)
            {
                string _message = string.Format("this.purchased_items.Contains('{0}') == false... returning false",_item_type);
                GlobalFunctions.print("_message", this);
            }
            return false;
        }


        //TODO - other checks e.g. if it isnt my turn???
        //TODO - other checks e.g. we must be in a certain turn state??
        //TODO - maybe the nuke can only be used in the attack state???

        bool _used = RiskySandBox_ItemsManager.instance.TRY_useItem(_Tile, _item_type);//ok now we ask the item manager to do this for me...


        if (_used == false)
        {
            if (this.debugging)
                GlobalFunctions.print("RiskySandBox_ItemsManager.instance.TRY_useItem returned false...", this);
            return false;
        }

        if (this.debugging)
            GlobalFunctions.print(string.Format("RiskySandBox_ItemsManager.instance.TRY_useItem returned true... removing '{0}", _item_type), this);

        //TODO - what happens if we want to use the same item 10 times... ideally we want to send 1 rpc to sync this...
        this.purchased_shop_items.Remove(_item_type);


        return true;
    }



}