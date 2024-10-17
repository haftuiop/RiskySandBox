using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_NeutralTileSettings : MonoBehaviour
{

    public static RiskySandBox_NeutralTileSettings instance;

    [SerializeField] bool debugging;
	
    public static ObservableBool enable_neutral_Tiles { get { return instance.PRIVATE_enable_neutral_Tiles; } }
    [SerializeField] ObservableBool PRIVATE_enable_neutral_Tiles;

    public static ObservableString n_troops_startGame { get { return instance.PRIVATE_n_troops_startGame; } }
    [SerializeField] ObservableString PRIVATE_n_troops_startGame;



    private void Awake()
    {
        instance = this;
    }



}
