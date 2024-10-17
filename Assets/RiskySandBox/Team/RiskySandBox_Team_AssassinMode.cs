using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

//TODO - handle the case where assassins get assigned after <n> rounds...

public partial class RiskySandBox_Team_AssassinMode : MonoBehaviour
{
    [SerializeField] bool debugging;


    [SerializeField] ObservableBool select_random_target;//completly random...
    
    [SerializeField] ObservableBool select_random_unique_target;//random (that noone else has...)

    [SerializeField] ObservableBool remove_this;

    [SerializeField] RiskySandBox_Team my_Team;


    [SerializeField] ObservableInt my_target_ID;

    RiskySandBox_Team my_target { get { return RiskySandBox_Team.GET_RiskySandBox_Team(my_target_ID); } }




    private void Awake()
    {

        RiskySandBox_Team.Onattack += EventReceiver_Onattack;

        RiskySandBox_MainGame.game_started.OnUpdate_true += EventReceiver_OnVariableUpdate_game_started_true;
    }

    private void OnDestroy()
    {
        RiskySandBox_Team.Onattack -= EventReceiver_Onattack;

        RiskySandBox_MainGame.game_started.OnUpdate_true -= EventReceiver_OnVariableUpdate_game_started_true;
    }


    void EventReceiver_OnVariableUpdate_game_started_true(ObservableBool _game_started)
    {
        //ok! lets work out our target...

        if (PrototypingAssets.run_server_code.value == false)
            return;


        if(RiskySandBox_Team.all_instances.Count <= 1)
        {
            GlobalFunctions.printWarning("RiskySandBox_Team.all_instances.Count <= 1 so unable to assign assassin target...",this);
            return;
        }
        
        List<int> _valid_targets = RiskySandBox_Team.all_instances.Where(x => x.defeated).Select(x => x.ID.value).ToList();

        if(this.remove_this == true)
            _valid_targets.Remove(this.my_Team.ID.value);


        if(select_random_target == true)
        {
            //select a random team (that isnt me...)
            this.my_target_ID.value = GlobalFunctions.GetRandomItem(_valid_targets);
        }

        else if(select_random_unique_target)
        {
            //remove other teams targets...
            List<int> _other_Team_targets = RiskySandBox_Team.all_instances.Where(x => x != this.my_Team && x.assassin_target != null).Select(x => x.assassin_target_ID.value).ToList();

            foreach(int _ID in _other_Team_targets)
            {
                _valid_targets.Remove(_ID);
            }
            this.my_target_ID.value = GlobalFunctions.GetRandomItem(_valid_targets);
        }

    }

    void EventReceiver_Onattack(RiskySandBox_Team.EventInfo_Onattack _EventInfo)
    {
       

        if (PrototypingAssets.run_server_code.value == false)
        {
            if (this.debugging)
                GlobalFunctions.print("run_server_code.value == false... returning", this);
            return;
        }

        if(_EventInfo.attacking_Team != this.my_Team)
        {
            if (this.debugging)
                GlobalFunctions.print("_EventInfo.attacking_Team != this... returning",this);
            return;
        }

        if (this.my_target == null)
        {
            if (this.debugging)
                GlobalFunctions.print("my assassin target is null!... returning", this);
            return;
        }
        if (_EventInfo.capture_flag == false)
        {
            if (this.debugging)
                GlobalFunctions.print("wasnt a capture event... returning", this);
            return;
        }



        RiskySandBox_Team _attacked_Team = _EventInfo.defending_Team;

        List<RiskySandBox_Tile> _attacked_Teams_Tiles = RiskySandBox_Tile.all_instances.Where(t => t.my_Team_ID.value == _attacked_Team.ID.value).ToList();
        

        if (_attacked_Teams_Tiles.Count > 0)

        {
            if (this.debugging)
                GlobalFunctions.print(_attacked_Team.ID.value + " still has tiles (so hasnt been killed...) returning", this);
            return;
        }


        int _killer_ID = this.my_Team.ID;


        if (this.debugging)
            GlobalFunctions.print("setting the team with id = " + _attacked_Team.ID + " killer_ID.value to " + _killer_ID, this);
        _attacked_Team.killer_ID.value = _killer_ID;


        if(_attacked_Team.ID.value == this.my_target_ID.value)
            RiskySandBox_MainGame.instance.endGame("AssassinMode",this.my_Team);
    }





}
