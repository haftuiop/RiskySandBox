using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class RiskySandBox_TerritoryCard : MonoBehaviour,IPointerClickHandler
{
    public static string progressive_mode { get { return "Progressive"; } }
    public static string fixed_mode { get { return "Fixed"; } }
    public static string geometric_mode { get { return "Geometric"; } }




    public static readonly int wildcard_ID = 0;


    public static readonly string card_type_infantry = "CardType_Infantry";
    public static readonly string card_type_cavalry = "CardType_Cavalry";
    public static readonly string card_type_artillary = "CardType_Artillary";
    public static readonly string card_type_wild = "CardType_Wild";



    public static List<RiskySandBox_TerritoryCard> all_instances = new List<RiskySandBox_TerritoryCard>();

    public static List<RiskySandBox_TerritoryCard> all_instances_is_selected { get { return all_instances.Where(x => x.PRIVATE_is_selected.value == true).ToList(); } }
    public static List<int> selected_IDs { get { return all_instances_is_selected.Select(x => (int)x.tile_ID_READONLY).ToList(); } }

    public static event Action<RiskySandBox_TerritoryCard> OnVariableUpdate_is_selected_STATIC;



    [SerializeField] bool debugging;

    public int tile_ID_READONLY { get { return tile_ID.value; } }

    [SerializeField] ObservableInt tile_ID;

    [SerializeField] Texture2D wildcard_Texture2D { get { return RiskySandBox_Resources.territory_card_Texture2Ds[3]; } }
    [SerializeField] Texture2D infantry_Texture2D { get { return RiskySandBox_Resources.territory_card_Texture2Ds[1]; } }
    [SerializeField] Texture2D cavalry_Texture2D { get { return RiskySandBox_Resources.territory_card_Texture2Ds[2]; } }
    [SerializeField] Texture2D artillary_Texture2D { get { return RiskySandBox_Resources.territory_card_Texture2Ds[0]; } }
    [SerializeField] UnityEngine.UI.RawImage tile_RawImage;

    [SerializeField] UnityEngine.UI.RawImage background_Image;

    [SerializeField] UnityEngine.UI.Text tile_name_Text;

    [SerializeField] ObservableBool PRIVATE_is_selected;



    public static List<string> GET_card_types(IEnumerable<int> _card_IDs)
    {
        List<string> _card_types = new List<string>();
        foreach (int _ID in _card_IDs)
        {
            if (_ID == wildcard_ID)
            {
                _card_types.Add(RiskySandBox_TerritoryCard.card_type_wild);
                continue;
            }

            //modulo 3...
            int _index = _ID % 3;

            if (_index == 1)
                _card_types.Add(RiskySandBox_TerritoryCard.card_type_infantry);
            else if (_index == 2)
                _card_types.Add(RiskySandBox_TerritoryCard.card_type_cavalry);
            else if (_index == 0)
                _card_types.Add(RiskySandBox_TerritoryCard.card_type_artillary);
            else
                GlobalFunctions.printError("WTF?!?!?!??!",null);
        }

        return _card_types;
    }


    private void Awake()
    {
        if (this.debugging)
            GlobalFunctions.print("called Awake", this);
        this.tile_ID.OnUpdate += delegate { updateVisuals(); };
        all_instances.Add(this);
        this.PRIVATE_is_selected.OnUpdate += OnVariableUpdate_is_selected;
        this.tile_ID.OnUpdate += delegate { gameObject.name = "TerritoryCard with id = " + this.tile_ID; };


        updateVisuals();


    }

    void OnEnable()
    {
        updateVisuals();
        gameObject.name = "TerritoryCard with id = " + this.tile_ID;
    }

    private void OnDestroy()
    {
        
        all_instances.Remove(this);
    }

    void OnVariableUpdate_is_selected(ObservableBool _is_selected)
    {
        OnVariableUpdate_is_selected_STATIC?.Invoke(this);
        if(_is_selected.previous_value == false && _is_selected.value == true)//from false -> true
        {
            //shift up...
            this.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, this.background_Image.GetComponent<RectTransform>().rect.height * 0.2f);
        }
        else if(_is_selected.previous_value == true && _is_selected.value == false)//from true -> false
        {
            //shift down...
            this.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, this.background_Image.GetComponent<RectTransform>().rect.height * 0.2f);
        }
    }

    void updateVisuals()
    {
        if (this.debugging)
            GlobalFunctions.print("", this);
        Texture2D _background_Texture2D = wildcard_Texture2D;
        string _tile_name = "";

        if (this.tile_ID != RiskySandBox_TerritoryCard.wildcard_ID)//TODO - well what happens if a tile has the ID = 0???? - this really is an error...
        {
            List<int> _temp_list = new List<int>();
            _temp_list.Add(this.tile_ID);
            string _card_type = RiskySandBox_TerritoryCard.GET_card_types(_temp_list)[0];

            if (_card_type == RiskySandBox_TerritoryCard.card_type_infantry)
                _background_Texture2D = infantry_Texture2D;//TODO magic number!

            else if (_card_type == RiskySandBox_TerritoryCard.card_type_cavalry)
                _background_Texture2D = cavalry_Texture2D;//TODO - magic number!

            else if (_card_type == RiskySandBox_TerritoryCard.card_type_artillary)
                _background_Texture2D = artillary_Texture2D;//TODO - magic number!
            

            RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(this.tile_ID);
            if(_Tile == null)
            {
                GlobalFunctions.printError("unable to find the tile with id = " + this.tile_ID,this);
                return;
            }

            _tile_name = _Tile.tile_name;
            this.tile_RawImage.texture = _Tile.my_TerritoryCardTexture;

            RiskySandBox_Team _Team = _Tile.my_Team;
            if (_Team != null)
            {
                this.tile_RawImage.color = _Team.my_Color;
            }
            else
                this.tile_RawImage.color = Color.grey;//TODO - RiskySandBox.fog_of_war_Material.Color...

        }

        background_Image.texture = _background_Texture2D;
        tile_name_Text.text = _tile_name;
    }



    public static RiskySandBox_TerritoryCard createNew(int _tile_ID, Transform _parent)
    {
        
        GameObject _new = UnityEngine.Object.Instantiate(RiskySandBox_Resources.territory_card,_parent);
        RiskySandBox_TerritoryCard _card = _new.GetComponent<RiskySandBox_TerritoryCard>();
        _card.tile_ID.value = _tile_ID;
        return _card;
    }



    // This method is called when the user has clicked on the UI element this script is attached to.
    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the left mouse button was clicked
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Add your logic here for what should happen when the UI element is clicked

            PRIVATE_is_selected.toggle();
        }
    }



    public static int drawRandomCard(List<int> _availible_tile_IDs,int _n_wilds)
    {

        if (_n_wilds > 0)
        {
            //e.g. if you had 3 wilds and 2 territory cards...
            int _random_int = GlobalFunctions.randomInt(0, _n_wilds + _availible_tile_IDs.Count - 1);
            //the random int is between 0 and 4
            //0,1,2 represent a wild... 3,4 represent a _availible card...

            if (_random_int < _n_wilds)
                return RiskySandBox_TerritoryCard.wildcard_ID;
        }

        
        if (_availible_tile_IDs.Count() > 0)
        {
            int _random_index = GlobalFunctions.randomInt(0, _availible_tile_IDs.Count - 1);
            return _availible_tile_IDs[_random_index];
        }

        Debug.LogError("this should not have happened!!!");
        return -99999;


    }
}
