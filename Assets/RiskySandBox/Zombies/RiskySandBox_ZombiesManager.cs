using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_ZombiesManager : MonoBehaviour
{
    public static RiskySandBox_ZombiesManager instance;

    [SerializeField] bool debugging;

    List<ZombieDecisionHelper> my_decisionHelpers = new List<ZombieDecisionHelper>();


    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }


    public void EventReceiver_OnstartNewRound()
    {
        //do zombie logic now....
        if (this.debugging)
            GlobalFunctions.print("new round started! (this is currently unimplemented...)", this);


    }

    void doZombieTurn()
    {
        //ok! first we spawn new zombies...
        //then we go through each tile...
        //look up the desicion helper for this tile...
        //decide what to do based on the decisionhelper data...



    }


    void loadDecisionData()
    {
        //look through the current map folder...
        //if there is a zombieDecision.txt file...
        string _zombie_file_path = "unimplemented...";


        if(System.IO.File.Exists(_zombie_file_path) == false)
        {
            if (this.debugging)
                GlobalFunctions.print(string.Format("no zombie file? tried looking at: {0}", _zombie_file_path), this);

            return;
        }

        foreach(string _line in System.IO.File.ReadAllLines(_zombie_file_path))
        {

        }



    }




    struct ZombieDecisionHelper
    {
        int start_ID;

        List<int> target_IDs;
        List<float> target_probabilities;
    }

}
