using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_DiseaseCaseUI : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] UnityEngine.UI.RawImage background_RawImage;

    public RiskySandBox_Tile my_Tile
    {
        get { return this.PRIVATE_my_Tile; }
        set
        {
            this.PRIVATE_my_Tile = value;
        }
    }
    [SerializeField] RiskySandBox_Tile PRIVATE_my_Tile;

    public RiskySandBox_Disease my_Disease
    {
        get { return this.PRIVATE_my_Disease; }
        set
        {
            this.PRIVATE_my_Disease = value;
        }
    }
    [SerializeField] RiskySandBox_Disease PRIVATE_my_Disease;

    [SerializeField] UnityEngine.UI.Text tile_name_Text;
    [SerializeField] UnityEngine.UI.Text remaining_duration_Text;
    [SerializeField] UnityEngine.UI.Text estimated_deaths_Text;

    public void EventReceiver_OncureButtonPressed()
    {
        //so the player wants to cure the disesase on this tile...

    }

    private void Start()
    {
        updateInfo();
    }

    void updateInfo()
    {
        if(this.my_Tile != null)
        {
            this.tile_name_Text.text = this.my_Tile.human_ui_log_string;

            if (this.background_RawImage != null)
            {
                if(this.my_Tile.my_Team != null)
                    this.background_RawImage.color = this.my_Tile.my_Team.my_Color;
                
            }
        }

        if(this.my_Disease != null)
        {
            if (this.my_Tile != null)
            {
                int _remaining_duration = this.my_Disease.GET_remaining_duration(this.my_Tile);
                this.remaining_duration_Text.text = "" + _remaining_duration;

                int _left_over_troops = Mathf.RoundToInt(this.my_Tile.num_troops * Mathf.Pow(1 - this.my_Disease.lethality, _remaining_duration));
                if (_left_over_troops > this.my_Tile.num_troops)
                    _left_over_troops = this.my_Tile.num_troops;


                this.estimated_deaths_Text.text = "" + (this.my_Tile.num_troops - _left_over_troops);


            }
        }



    }
}
