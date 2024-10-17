using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_ShopUI : MonoBehaviour
{

    public static RiskySandBox_ShopUI instance;

    [SerializeField] bool debugging;
    public static RiskySandBox_Team display_Team
    {
        get {return  instance.PRIVATE_display_Team; }
        set
        {
            instance.PRIVATE_display_Team = value;
            instance.updateUI(value);
        }
    }
    [SerializeField] RiskySandBox_Team PRIVATE_display_Team;

    [SerializeField] List<GameObject> instantiated_GameObjects = new List<GameObject>();


    [SerializeField] GameObject root;

    [SerializeField] ObservableBool PRIVATE_root_state;

    [SerializeField] Vector2 shop_item_ui_start;



    [SerializeField] UnityEngine.UI.Text shop_credits_Text;

    private void Awake()
    {
        instance = this;
        RiskySandBox_Team.OnShopVariableChange_STATIC += TeamEventReceiver_OnShopVariableChange;
    }

    private void OnDestroy()
    {
        instance = null;
        RiskySandBox_Team.OnShopVariableChange_STATIC -= TeamEventReceiver_OnShopVariableChange;
    }

    void TeamEventReceiver_OnShopVariableChange(RiskySandBox_Team _Team)
    {
        if (_Team != display_Team)
            return;

        updateUI(_Team);
    }


    private void Start()
    {
        updateUI(RiskySandBox_ShopUI.display_Team);
    }

    void updateUI(RiskySandBox_Team _Team)
    {
        foreach (GameObject _GameObject in this.instantiated_GameObjects)
        {
            UnityEngine.Object.Destroy(_GameObject);
        }
        instantiated_GameObjects.Clear();

        this.PRIVATE_root_state.value = _Team != null;

        if (display_Team == null)
            return;




        for (int i = 0; i < _Team.shop_item_types.Count(); i += 1)
        {
            string _item_type = _Team.shop_item_types[i];
            int _price = _Team.GET_itemPrice(_item_type);

            GameObject _new_GameObject = UnityEngine.Object.Instantiate(RiskySandBox_Resources.shop_item_ui_prefab, this.root.transform);
            _new_GameObject.GetComponent<RectTransform>().anchoredPosition = shop_item_ui_start + new Vector2(0, -30 * i);
            instantiated_GameObjects.Add(_new_GameObject);

            RiskySandBox_ShopItemUI _UI = _new_GameObject.GetComponent<RiskySandBox_ShopItemUI>();

            _UI.item_type.value = _item_type;
            _UI.item_price.value = _price;
            _UI.can_purchase.value = _Team.shop_credits >= _price;
            _UI.current_quantity.value = _Team.purchased_shop_items.Where(x => x == _item_type).Count();

        }

        shop_credits_Text.text = "" + _Team.shop_credits;



    }


}
