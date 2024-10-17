using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Resources : MonoBehaviour
{
    public static RiskySandBox_Resources instance;


    public static GameObject human_player_prefab { get { return instance.PRIVATE_human_player_prefab; } }

    public static GameObject Team_prefab { get { return instance.PRIVATE_Team_prefab; } }

    public static GameObject tile_prefab { get { return instance.PRIVATE_tile_prefab; } }

    public static GameObject Bonus_prefab { get { return instance.PRIVATE_bonus_prefab; } }

    public static GameObject fortify_arrow_prefab { get { return instance.PRIVATE_fortify_arrow_prefab; } }

    public static GameObject territory_card { get { return instance.PRIVATE_territory_card; } }

    public static GameObject PermanentLine_prefab { get { return instance.PRIVATE_PermanentLine_prefab; } }

    public static GameObject shop_item_ui_prefab { get { return instance.PRIVATE_shop_item_ui_prefab; } }

    public static GameObject disease_prefab { get { return instance.PRIVATE_disease_prefab; } }

    public static GameObject nuclear_missile_prefab { get { return instance.PRIVATE_nuclear_missile_prefab; } }



    public static List<Texture2D> item_icons { get { return instance.PRIVATE_item_icons; } }
    public static List<Texture2D> territory_card_Texture2Ds { get { return instance.PRIVATE_territory_card_Texture2Ds.images.ToList(); } }



    [SerializeField] bool debugging;
    [SerializeField] GameObject PRIVATE_tile_parent;
    [SerializeField] GameObject PRIVATE_bonus_parent;
    [SerializeField] GameObject PRIVATE_PermanentLine_line_parent;

    [SerializeField] GameObject PRIVATE_tile_prefab;
    [SerializeField] private GameObject PRIVATE_human_player_prefab;
    [SerializeField] private GameObject PRIVATE_Team_prefab;
    [SerializeField] private GameObject PRIVATE_bonus_prefab;
    [SerializeField] private GameObject PRIVATE_fortify_arrow_prefab;
    [SerializeField] private GameObject PRIVATE_territory_card;
    [SerializeField] private GameObject PRIVATE_PermanentLine_prefab;
    [SerializeField] private GameObject PRIVATE_shop_item_ui_prefab;
    [SerializeField] private GameObject PRIVATE_disease_prefab;
    [SerializeField] List<Texture2D> PRIVATE_item_icons;
    [SerializeField] ResourceLoader_ImageFolder PRIVATE_territory_card_Texture2Ds;
    [SerializeField] private GameObject PRIVATE_nuclear_missile_prefab;


    private void Awake()
    {
        instance = this;
    }


    public static RiskySandBox_Team createTeam(int _ID)
    {
        RiskySandBox_Team _new_Team;

        if(MultiplayerBridge_PhotonPun.in_room)
        {
            if (PrototypingAssets.run_server_code == false)
                return null;

            _new_Team = Photon.Pun.PhotonNetwork.InstantiateRoomObject(RiskySandBox_Resources.Team_prefab.name, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<RiskySandBox_Team>();
        }

        else if(MultiplayerBridge_Mirror.is_enabled)
        {
            if (PrototypingAssets.run_server_code == false)
                return null;

            _new_Team = UnityEngine.Object.Instantiate(RiskySandBox_Resources.Team_prefab).GetComponent<RiskySandBox_Team>();
            Mirror.NetworkServer.Spawn(_new_Team.gameObject);

        }
        else
            _new_Team = UnityEngine.Object.Instantiate(RiskySandBox_Resources.Team_prefab).GetComponent<RiskySandBox_Team>();

        _new_Team.ID.value = _ID;


        return _new_Team;
    }

    public static RiskySandBox_Tile createTile(int _id)
    {
        RiskySandBox_Tile _new_Tile_script;

        if (MultiplayerBridge_PhotonPun.in_room)
        {
            if (PrototypingAssets.run_server_code == false)
                return null;

            _new_Tile_script = Photon.Pun.PhotonNetwork.InstantiateRoomObject(RiskySandBox_Resources.tile_prefab.name, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<RiskySandBox_Tile>();
        }

        else if (MultiplayerBridge_Mirror.is_enabled)
        {
            if (PrototypingAssets.run_server_code == false)
                return null;

            _new_Tile_script = UnityEngine.Object.Instantiate(RiskySandBox_Resources.tile_prefab).GetComponent<RiskySandBox_Tile>();
            Mirror.NetworkServer.Spawn(_new_Tile_script.gameObject);
        }

        else
            _new_Tile_script = UnityEngine.Object.Instantiate(RiskySandBox_Resources.tile_prefab).GetComponent<RiskySandBox_Tile>();


        _new_Tile_script.ID.value = _id;

        _new_Tile_script.gameObject.transform.parent = instance.PRIVATE_tile_parent.transform;
        return _new_Tile_script;
    }

    public static RiskySandBox_HumanPlayer createLocalHumanPlayer()
    {
        RiskySandBox_HumanPlayer _new_HumanPlayer = Photon.Pun.PhotonNetwork.Instantiate(RiskySandBox_Resources.human_player_prefab.name, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<RiskySandBox_HumanPlayer>();
        _new_HumanPlayer.multiplayer_name.value = Photon.Pun.PhotonNetwork.NickName;
        _new_HumanPlayer.is_local_player.value = true;

        return _new_HumanPlayer;
    }

    public static RiskySandBox_Bonus createNewBonus()
    {
        RiskySandBox_Bonus _new_bonus;

        if (MultiplayerBridge_PhotonPun.in_room)
        {
            if (PrototypingAssets.run_server_code == false)
                return null;

            _new_bonus = Photon.Pun.PhotonNetwork.InstantiateRoomObject(RiskySandBox_Resources.Bonus_prefab.name, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<RiskySandBox_Bonus>();
        }

        else if (MultiplayerBridge_Mirror.is_enabled)
        {
            if (PrototypingAssets.run_server_code == false)
                return null;

            _new_bonus = UnityEngine.Object.Instantiate(RiskySandBox_Resources.Bonus_prefab).GetComponent<RiskySandBox_Bonus>();
            Mirror.NetworkServer.Spawn(_new_bonus.gameObject);
        }

        else
            _new_bonus = UnityEngine.Object.Instantiate(RiskySandBox_Resources.Bonus_prefab).GetComponent<RiskySandBox_Bonus>();


        _new_bonus.transform.parent = instance.PRIVATE_bonus_parent.transform;
        return _new_bonus.GetComponent<RiskySandBox_Bonus>();
    }

    public static RiskySandBox_PermanentLine createLine(float _width, int _red, int _green, int _blue)
    {
        RiskySandBox_PermanentLine _new_Line;

        if (MultiplayerBridge_PhotonPun.in_room)
        {
            if (PrototypingAssets.run_server_code == false)
                return null;

            _new_Line = Photon.Pun.PhotonNetwork.InstantiateRoomObject(RiskySandBox_Resources.PermanentLine_prefab.name, Vector3.zero, Quaternion.identity).GetComponent<RiskySandBox_PermanentLine>();
        }

        else if (MultiplayerBridge_Mirror.is_enabled)
        {
            if (PrototypingAssets.run_server_code == false)
                return null;

            _new_Line = UnityEngine.Object.Instantiate(RiskySandBox_Resources.PermanentLine_prefab).GetComponent<RiskySandBox_PermanentLine>();
            Mirror.NetworkServer.Spawn(_new_Line.gameObject);
        }

        else
            _new_Line = UnityEngine.Object.Instantiate(RiskySandBox_Resources.PermanentLine_prefab).GetComponent<RiskySandBox_PermanentLine>();
            

        _new_Line.transform.parent = instance.PRIVATE_PermanentLine_line_parent.transform;//keep hierarchy organised...

        _new_Line.line_width.value = _width;
        _new_Line.my_Color_r.value = _red;
        _new_Line.my_Color_g.value = _green;
        _new_Line.my_Color_b.value = _blue;

        return _new_Line;
    }


    public static RiskySandBox_Disease createDisease()
    {
        RiskySandBox_Disease _new_Disease;

        if(MultiplayerBridge_PhotonPun.in_room)
        {
            if (PrototypingAssets.run_server_code == false)
                return null;

            _new_Disease = Photon.Pun.PhotonNetwork.InstantiateRoomObject(RiskySandBox_Resources.disease_prefab.name, Vector3.zero, Quaternion.identity).GetComponent<RiskySandBox_Disease>();
        }

        else if(MultiplayerBridge_Mirror.is_enabled)
        {
            if (PrototypingAssets.run_server_code == false)
                return null;

            _new_Disease = UnityEngine.Object.Instantiate(RiskySandBox_Resources.disease_prefab).GetComponent<RiskySandBox_Disease>();
            Mirror.NetworkServer.Spawn(_new_Disease.gameObject);
        }

        else
            _new_Disease = UnityEngine.Object.Instantiate(RiskySandBox_Resources.disease_prefab).GetComponent<RiskySandBox_Disease>();

        return _new_Disease;
    }





}
