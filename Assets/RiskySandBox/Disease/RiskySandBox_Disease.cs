using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Disease : MonoBehaviour
{
    /// <summary>
    /// invoked whenever a variable to do with the disease is changed...
    /// </summary>
    public static event Action<RiskySandBox_Disease> OnVariableUpdate_STATIC;
    public static event Action<RiskySandBox_Disease> OnVariableUpdate_infected_Tile_IDs_STATIC;
    public static event Action<RiskySandBox_Disease> OnVariableUpdate_remaining_durations_STATIC;

    public static ObservableList<RiskySandBox_Disease> all_instances = new ObservableList<RiskySandBox_Disease>();

    public ObservableBool root_state { get { return this.PRIVATE_root_state; } }
    [SerializeField] ObservableBool PRIVATE_root_state;
    [SerializeField] Texture2D disease_alert_icon { get { return disease_icon_ResourceLoader.loaded_image; } }
    [SerializeField] ResourceLoader_Image disease_icon_ResourceLoader;

    [SerializeField] bool debugging;

    [SerializeField] List<GameObject> PRIVATE_disease_models = new List<GameObject>();

    public GameObject disease_model
    {
        get
        {
            return this.PRIVATE_disease_models[disease_model_index];
        }
    }

    public ObservableString disease_name { get { return this.PRIVATE_disease_name; } }
    [SerializeField] ObservableString PRIVATE_disease_name;

    public ObservableFloat lethality { get { return this.PRIVATE_lethality; } }
    [SerializeField] ObservableFloat PRIVATE_lethality;


    public ObservableInt duration { get { return this.PRIVATE_duration; } }
    [SerializeField] ObservableInt PRIVATE_duration;


    public ObservableFloat infectivity { get { return this.PRIVATE_infectivity; } }
    [SerializeField] ObservableFloat PRIVATE_infectivity;


    public ObservableIntList infected_Tile_IDs { get { return this.PRIVATE_infected_Tile_IDs; } }
    [SerializeField] ObservableIntList PRIVATE_infected_Tile_IDs;//all the tiles currently infected with the disease...

    public ObservableIntList remaining_durations { get { return this.PRIVATE_remaining_durations; } }
    [SerializeField] ObservableIntList PRIVATE_remaining_durations;//the time left for each of the tiles...

    public ObservableInt disease_model_index { get { return this.PRIVATE_disease_model_index; } }
    [SerializeField] ObservableInt PRIVATE_disease_model_index;


    bool eradicated
    {
        get
        {
            for(int i = 0; i < this.infected_Tiles.Count; i += 1)
            {
                if (this.remaining_durations[i] > 0)
                    return false;
            }
            return true;
        }
    }


    public List<RiskySandBox_Tile> infected_Tiles
    {
        get
        {
            List<RiskySandBox_Tile> _return_value = new List<RiskySandBox_Tile>();

            foreach(int _id in this.infected_Tile_IDs)
            {
                _return_value.Add(RiskySandBox_Tile.GET_RiskySandBox_Tile(_id));
            }

            return _return_value;
        }
    }


    private void Awake()
    {
        RiskySandBox_Disease.all_instances.Add(this);
        this.disease_name.OnUpdate += delegate { RiskySandBox_Disease.OnVariableUpdate_STATIC?.Invoke(this); };
        this.lethality.OnUpdate += delegate { RiskySandBox_Disease.OnVariableUpdate_STATIC?.Invoke(this); };
        this.infectivity.OnUpdate += delegate { RiskySandBox_Disease.OnVariableUpdate_STATIC?.Invoke(this); };
        this.duration.OnUpdate += delegate { RiskySandBox_Disease.OnVariableUpdate_STATIC?.Invoke(this); };
        this.infected_Tile_IDs.OnUpdate += delegate { RiskySandBox_Disease.OnVariableUpdate_infected_Tile_IDs_STATIC?.Invoke(this); };
        this.remaining_durations.OnUpdate += delegate { RiskySandBox_Disease.OnVariableUpdate_remaining_durations_STATIC?.Invoke(this); };
    }

    private void OnDestroy()
    {
        RiskySandBox_Disease.all_instances.Remove(this);
    }


    public static void loadFromFile(string[] _lines)
    {
        List<string> _keys = _lines.Select(x => x.Split(":")[0]).ToList();
        List<string> _values = _lines.Select(x => x.Split(":")[1]).ToList();
    }

    public int GET_remaining_duration(RiskySandBox_Tile _Tile)
    {
        if(this.infected_Tile_IDs.Contains(_Tile.ID) == false)
        {
            Debug.LogWarning("weird case..... needs more thinking about...",this);
            return 0;
        }

        int _index = this.infected_Tile_IDs.IndexOf(_Tile.ID);

        return this.remaining_durations[_index];
    }



    public void infectTile(RiskySandBox_Tile _Tile)
    {
        if(this.infected_Tile_IDs.Contains(_Tile.ID))
        {
            remaining_durations[this.infected_Tile_IDs.IndexOf(_Tile.ID)] = this.duration;
        }
        else
        {
            this.infected_Tile_IDs.Add(_Tile.ID);
            this.remaining_durations.Add(this.duration);
        }



        RiskySandBox_AlertSystem.createAlert(_Tile.human_ui_log_string + " just got infected with " + this.disease_name,this.disease_alert_icon, _Tile);

        RiskySandBox_Disease.OnVariableUpdate_STATIC?.Invoke(this);
    }

    void infectNeighbours(RiskySandBox_Tile _Tile)
    {
        List<RiskySandBox_Tile> _neighbours = _Tile.graph_connections;

        foreach (RiskySandBox_Tile _neighbour in _neighbours)
        {
            //roll a random number...
            int _rng = GlobalFunctions.randomInt(0, 100);

            if (_rng / 100f > this.infectivity)
                continue;

            //ok we must spread to the neighbour.... (unless it is already infected? OR the tile is immune in some way... already been caught in the disease...)

            if (this.infected_Tile_IDs.Contains(_neighbour.ID))//if the tile is already infected...
                continue;//dont care!

            if (_neighbour.num_troops <= 0)
                continue;

            //TODO - update all at the same time instead of individually (just sends unnecassary events...)
            this.infectTile(_neighbour);
        }
    }


    public void step()
    {

        if(PrototypingAssets.run_server_code.value == false)
        {
            GlobalFunctions.printWarning("not the server??? why is this happening...", this);
            return;
        }

        if (this.debugging)
            GlobalFunctions.print("called step",this);

        if(this.eradicated)
        {
            GlobalFunctions.print("called step - this.erradicated == false... returning",this);
            return;
        }

        List<RiskySandBox_Tile> _infected_Tiles = this.infected_Tile_IDs.Select(x => RiskySandBox_Tile.GET_RiskySandBox_Tile(x)).ToList();

        int i = 0;
        foreach(RiskySandBox_Tile _Tile in _infected_Tiles)
        {
            if (_Tile == null)//ok weirdly the tile may have been deleted at some point???
            {
                i += 1;
                continue;
            }

            if (this.remaining_durations[i] <= 0)//if the disease has ran out of duration (for this tile)
            {
                i += 1;
                continue;
            }

            this.remaining_durations[i] -= 1;

            //if the tile has 0 troops??
            if (_Tile.num_troops <= 0)
            {
                i += 1;
                continue;
            }

            int _deaths = 0;
            for(int _x = 0; _x < _Tile.num_troops; _x += 1)
            {
                int _rng = GlobalFunctions.randomInt(0, 100);

                if (_rng / 100f > this.lethality)
                    continue;

                _deaths += 1;
            }




            if (_deaths <= 0)
                _deaths = 0;

            if (_deaths > _Tile.num_troops)
                _deaths = _Tile.num_troops;

            _Tile.num_troops.value -= _deaths;

            RiskySandBox_AlertSystem.createAlert(string.Format("{0} just killed {1} in {2}", this.disease_name.value, _deaths, _Tile.human_ui_log_string), this.disease_alert_icon, _Tile);

            if(_Tile.num_troops > 0)
                this.infectNeighbours(_Tile);

 
            i += 1;
        }

        //lets clean up...
        bool _erradicated = true;
        for(i = 0; i < this.infected_Tile_IDs.Count; i += 1)
        {
            if (this.remaining_durations[i] > 0)
            {
                _erradicated = false;
            }

        }

        if (_erradicated)
        {
            string _message = string.Format("{0} has been erradicated...", this.disease_name.value);
            RiskySandBox_AlertSystem.createAlert(_message, this.disease_alert_icon, null);

            this.infected_Tile_IDs.Clear();
            this.remaining_durations.Clear();
        }

        RiskySandBox_Disease.OnVariableUpdate_STATIC?.Invoke(this);
    }

    private void OnDrawGizmosSelected()
    {
        for(int i = 0; i < this.infected_Tile_IDs.Count; i += 1)
        {
            RiskySandBox_Tile _Tile = RiskySandBox_Tile.GET_RiskySandBox_Tile(this.infected_Tile_IDs[i]);
            Collider _Tile_Collider;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_Tile.UI_position, 0.1f);
        }
        
    }




}
