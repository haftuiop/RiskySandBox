using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team_EndGameUI : MonoBehaviour
{
    [SerializeField] bool debugging;

    public RiskySandBox_Team my_Team
    {
        get { return this.PRIVATE_my_Team; }
        set
        {
            this.PRIVATE_my_Team = value;
            EventReceiver_OnVariableUpdate_my_Team();
        }
    }
    [SerializeField] RiskySandBox_Team PRIVATE_my_Team;

    [SerializeField] UnityEngine.UI.RawImage background_RawImage;
    [SerializeField] UnityEngine.UI.Text team_name_Text;

    [SerializeField] UnityEngine.UI.RawImage winner_indicator_RawImage;

    [SerializeField] UnityEngine.UI.Text game_over_placement_Text;

    [SerializeField] List<Texture2D> trophy_Texture2Ds = new List<Texture2D>();


    void EventReceiver_OnVariableUpdate_my_Team()
    {
        //update ui...
        background_RawImage.color = my_Team.my_Color;
        team_name_Text.text = my_Team.team_name.value;
        team_name_Text.color = my_Team.text_Color;

        if (this.my_Team.game_over_placement.value > 2)
        {
            //make it say 4th, 5th, 6th, 7th, 16th
            game_over_placement_Text.text = this.my_Team.game_over_placement.value + 1 + "th";
        }
        else if(this.my_Team.game_over_placement >= 0)//TODO redo this to say if greater than - 1 && < trophy_texures.count...
        {
            this.winner_indicator_RawImage.texture = trophy_Texture2Ds[this.my_Team.game_over_placement];
        }


    }





}
