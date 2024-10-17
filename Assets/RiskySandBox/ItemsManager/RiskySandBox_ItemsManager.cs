using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_ItemsManager : MonoBehaviour
{
    public static RiskySandBox_ItemsManager instance;

    public static event Action<RiskySandBox_Tile, RiskySandBox_Tile> Onnuke_MultiplayerBridge;
    /// <summary>
    /// invoked whenever a nuclear weapon is used on a tile...
    /// </summary>
    public static event Action<RiskySandBox_Tile,RiskySandBox_Tile> Onnuke;



    public static event Action<RiskySandBox_Tile> OndetonateLandMine_MultiplayerBridge;
    public static event Action<RiskySandBox_Tile> OndetonateLandMine;



    public static string item_type_landmine { get { return "LandMine"; } }
    public static string item_type_nuke { get { return "Nuke"; } }
    public static string item_type_airstrike { get { return "AirStrike"; } }


    [SerializeField] KeyCode nuclear_strike_test_KeyCode = KeyCode.N;
    [SerializeField] KeyCode airstrike_test_KeyCode = KeyCode.Y;
    [SerializeField] KeyCode landmine_test_KeyCode = KeyCode.L;

    [SerializeField] float nuclear_missile_animation_duration = 2f;

    [SerializeField] bool debugging;
    [SerializeField] bool testing;


    public ObservableIntList land_mine_Tile_IDs { get { return this.PRIVATE_land_mine_Tile_IDs; } }
    [SerializeField] ObservableIntList PRIVATE_land_mine_Tile_IDs;



    public List<string> item_types = new List<string>();


    [SerializeField] ResourceLoader_ImageFolder items_ImageFolder;


    public ObservableList<Texture2D> item_Texture2Ds
    {
        get { return items_ImageFolder.images; }
    }

    public static Texture2D GET_itemTexture2D(string _item_type)
    {
        int _index = RiskySandBox_ItemsManager.instance.item_types.IndexOf(_item_type);
        return RiskySandBox_ItemsManager.instance.item_Texture2Ds[_index];
    }




    private void Awake()
    {
        if (instance != null)
            GlobalFunctions.printError("instance != null??? instance.gameObject.name = '"+instance.gameObject.name+"'",this);
        instance = this;

        if (this.debugging)
            GlobalFunctions.print("called Awake just set instance to this!", this);

        RiskySandBox_Team.Oncapture += EventReceiver_Oncapture;
    }

    private void OnDestroy()
    {
        RiskySandBox_Team.Oncapture -= EventReceiver_Oncapture;
    }


    public void invokeEvent_Onnuke(RiskySandBox_Tile _from,RiskySandBox_Tile _target_Tile,bool _alert_MultiplayerBridge)
    {
        if (_alert_MultiplayerBridge)
            RiskySandBox_ItemsManager.Onnuke_MultiplayerBridge?.Invoke(_from, _target_Tile);
        RiskySandBox_ItemsManager.Onnuke?.Invoke(_from, _target_Tile);
    }

    public void invokeEvent_OndetonateLandMine(RiskySandBox_Tile _Tile,bool _alert_MultiplayerBridge)
    {
        if (_alert_MultiplayerBridge)
            RiskySandBox_ItemsManager.OndetonateLandMine_MultiplayerBridge?.Invoke(_Tile);
        RiskySandBox_ItemsManager.OndetonateLandMine?.Invoke(_Tile);
    }

    void EventReceiver_Oncapture(RiskySandBox_Team.EventInfo_Oncapture _EventInfo)
    {
        detonateLandMinesOnTile(_EventInfo.target_tile);
    }


    public void detonateLandMinesOnTile(RiskySandBox_Tile _Tile)
    {
        int _n_landmines = land_mine_Tile_IDs.CountOf(_Tile.ID.value);

        if (_n_landmines == 0)
        {
            if (this.debugging)
                GlobalFunctions.print("there are no landmines on this Tile... returning", this);
            return;
        }

        if(this.debugging)
        {
            GlobalFunctions.print(string.Format("detonating {0} landmines", _n_landmines), this);
        }

        for (int i = 0; i < _n_landmines; i += 1)
        {
            //detonate a landmine...

        }
        //TODO - remove all linemines on this tile...
        land_mine_Tile_IDs.SET_items(this.land_mine_Tile_IDs.Where(x => x != _Tile.ID.value));
        OndetonateLandMine?.Invoke(_Tile);
    }

    public void airStrike(RiskySandBox_Tile _Tile)
    {
        //damage by a percentage? or by a fixed amount?


    }

    public void nuclearStrike(RiskySandBox_Tile _from,RiskySandBox_Tile _target)
    {
        _target.num_troops.value = 0;

        this.invokeEvent_Onnuke(_from, _target, PrototypingAssets.run_server_code.value);
    }

    


    private void Update()
    {
        if(this.testing)
        {
            if(Input.GetKeyDown(nuclear_strike_test_KeyCode))
            {
                //fire a nuke at the tile...
                RiskySandBox_Tile _Tile = RiskySandBox_CameraControls.current_hovering_Tile;
                if (_Tile == null)
                    GlobalFunctions.print("testing nuke but not hovering over a tile???", this);

                nuclearStrike(null, _Tile);

            }

            if(Input.GetKeyDown(landmine_test_KeyCode))
            {
                RiskySandBox_Tile _Tile = RiskySandBox_CameraControls.current_hovering_Tile;

                if (_Tile == null)
                    GlobalFunctions.print("testing the landmine but not hovering over a tile???", this);

                detonateLandMinesOnTile(_Tile);
            }
        }
    }



    public bool TRY_useItem(RiskySandBox_Tile _Tile,string _item_type)
    {
        if (this.debugging)
            GlobalFunctions.print("", this);

        //if the item type is a landmine???
        //place a landmine on that tile...
        if(_item_type == item_type_landmine)
        {
            land_mine_Tile_IDs.Add(_Tile.ID.value);
        }

        else if(_item_type == item_type_nuke)
        {
            nuclearStrike(null, _Tile);
        }

        else if(_item_type == item_type_airstrike)
        {
            airStrike(_Tile);
        }

        else
        {
            GlobalFunctions.print(string.Format("unimplemented item type??? '{0}'",_item_type),this);
            return false;
        }




        return true;
        //if its a nuke???



    }





}
