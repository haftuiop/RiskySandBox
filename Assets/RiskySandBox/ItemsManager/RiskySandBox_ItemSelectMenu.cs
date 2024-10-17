using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_ItemSelectMenu : MonoBehaviour
{

    public static RiskySandBox_ItemSelectMenu instance;

    public static RiskySandBox_Team display_Team
    {
        get { return instance.PRIVATE_display_Team; }
        set
        {
            instance.PRIVATE_display_Team = value;
            instance.refresh();
        }
    }

    public static ObservableBool root_state { get { return instance.PRIVATE_root_state; } }


    [SerializeField] bool debugging;
    [SerializeField] bool testing;
    [SerializeField] Vector2 start_point = new Vector2(15, -15);

    [SerializeField] RiskySandBox_Team PRIVATE_display_Team;

    [SerializeField] GameObject root;

    [SerializeField] GameObject rename_this_variable_to_something_useful;

    

    [SerializeField] ObservableBool PRIVATE_root_state;


    [SerializeField] int items_per_row;



    [SerializeField] List<GameObject> instantiated_gameObjects = new List<GameObject>();

    List<Texture2D> textures = new List<Texture2D>();

    [SerializeField] UnityEngine.UI.Text item_description_Text;
    [SerializeField] UnityEngine.UI.Text current_quantity_Text;
    [SerializeField] UnityEngine.UI.Text current_price_Text;
    [SerializeField] UnityEngine.UI.Button purchase_Button;
    [SerializeField] UnityEngine.UI.Button equip_Button;

    List<string> item_descriptions
    {
        get
        {
            return item_descriptions_english;
        }
    }
    [SerializeField] List<string> item_descriptions_english = new List<string>();

    //which items has the player clicked on...
    [SerializeField] ObservableInt PRIVATE_current_Item_index;

    public string current_item_type
    {
        get
        {
            if (this.PRIVATE_current_Item_index == -1)
                return "null";
            else
                return RiskySandBox_ItemsManager.instance.item_types[this.PRIVATE_current_Item_index];
        }
    }




    private void Awake()
    {
        instance = this;
        this.PRIVATE_root_state.OnUpdate_true += delegate { refresh(); };
        this.PRIVATE_current_Item_index.OnUpdate += EventReceiver_OnVariableUpdate_current_Item_index;

        RiskySandBox_Team.OnVariableUpdate_purchased_shop_items_STATIC += EventReceiver_OnVariableUpdate_purchased_shop_items_STATIC;

        purchase_Button.onClick.AddListener(delegate { EventReceiver_OnpurchaseButtonPressed(); } );
        equip_Button.onClick.AddListener(delegate { EventReceiver_OnequipButtonPressed(); });

        
    }

    private void OnDestroy()
    {
        RiskySandBox_Team.OnVariableUpdate_purchased_shop_items_STATIC -= EventReceiver_OnVariableUpdate_purchased_shop_items_STATIC;
    }

    void EventReceiver_OnequipButtonPressed()
    {
        if (RiskySandBox_HumanPlayer.local_player == null)
            return;

        RiskySandBox_HumanPlayer.local_player.current_Item.value = this.current_item_type;
    }

    void EventReceiver_OnpurchaseButtonPressed()
    {
        if (RiskySandBox_HumanPlayer.local_player == null)
            return;

        RiskySandBox_HumanPlayer.local_player.TRY_purchaseItem(this.current_item_type, 1);
    }

    void EventReceiver_OnVariableUpdate_current_Item_index(ObservableInt _index)
    {
        refresh();
    }


    void EventReceiver_OnVariableUpdate_purchased_shop_items_STATIC(RiskySandBox_Team _Team)
    {

        if (_Team != this.PRIVATE_display_Team)
        {
            if (this.debugging)
                GlobalFunctions.print("_Team wasnt this.display_Team... returning",this);
            return;
        }



        if (_Team == this.PRIVATE_display_Team)
            refresh();
    }


    private void Start()
    {
        refresh();
    }

    private void Update()
    {
        if (this.testing && Input.GetKeyDown(KeyCode.T))
            refresh();
    }

    void refresh()
    {
        if (this.debugging)
            GlobalFunctions.print("refreshing...", this);

        foreach(GameObject _GameObject in this.instantiated_gameObjects)
        {
            UnityEngine.Object.Destroy(_GameObject);
        }

        instantiated_gameObjects.Clear();

        if (display_Team == null)
        {
            if (this.debugging)
                GlobalFunctions.print("display_Team is null... returning",this);
            return;
        }
            


        for(int i = 0; i < RiskySandBox_ItemsManager.instance.item_types.Count; i += 1) 
        {

            string _item_type = RiskySandBox_ItemsManager.instance.item_types[i];


            GameObject _new = UnityEngine.Object.Instantiate(rename_this_variable_to_something_useful, root.transform);
            _new.GetComponent<RectTransform>().anchoredPosition = start_point + new Vector2(30 * (i % items_per_row), -30 * (i / items_per_row));

            

            RiskySandBox_ItemSelectUI _ui = _new.GetComponent<RiskySandBox_ItemSelectUI>();
            _ui.item_type.value = RiskySandBox_ItemsManager.instance.item_types[i];
            _ui.current_quantity.value = PRIVATE_display_Team.purchased_shop_items.CountOf(_item_type);

            int _i = i;
            _ui.OnButtonPressed += delegate { this.PRIVATE_current_Item_index.value = _i; };

            this.instantiated_gameObjects.Add(_new);
        }


        int _current_item_index = this.PRIVATE_current_Item_index.value;
        this.item_description_Text.gameObject.SetActive(_current_item_index != -1);
        this.current_quantity_Text.gameObject.SetActive(_current_item_index != -1);
        this.current_price_Text.gameObject.SetActive(_current_item_index != -1);
        this.equip_Button.gameObject.SetActive(_current_item_index != -1);
        this.purchase_Button.gameObject.SetActive(_current_item_index != -1);

        if (_current_item_index == -1)
            return;

        string _current_item_type = this.current_item_type;

        this.item_description_Text.text = this.item_descriptions[_current_item_index];
        this.current_quantity_Text.text = "" + this.PRIVATE_display_Team.purchased_shop_items.CountOf(_current_item_type);
        this.current_price_Text.text = "" + this.PRIVATE_display_Team.GET_itemPrice(_current_item_type);
        this.equip_Button.interactable = this.PRIVATE_display_Team.purchased_shop_items.CountOf(_current_item_type) > 0;
        this.purchase_Button.interactable = this.PRIVATE_display_Team.shop_credits > this.PRIVATE_display_Team.GET_itemPrice(_current_item_type);
    }






}
