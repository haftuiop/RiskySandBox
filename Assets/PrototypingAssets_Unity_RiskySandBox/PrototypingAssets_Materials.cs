using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class PrototypingAssets_Materials : MonoBehaviour
{
    public static PrototypingAssets_Materials instance;


    public static Material black { get { return instance.PRIVATE_black; } }
    public static Material blue { get { return instance.PRIVATE_blue; } }
    public static Material brown { get { return instance.PRIVATE_brown; } }
    public static Material cyan { get { return instance.PRIVATE_cyan; } }
    public static Material dark_green { get { return instance.PRIVATE_dark_green; } }
    public static Material green { get { return instance.PRIVATE_green; } }
    public static Material grey { get { return instance.PRIVATE_grey; } }
    public static Material orange { get { return instance.PRIVATE_orange; } }
    public static Material pink { get { return instance.PRIVATE_pink; } }
    public static Material purple { get { return instance.PRIVATE_purple; } }
    public static Material red { get { return instance.PRIVATE_red; } }
    public static Material white { get { return instance.PRIVATE_white; } }
    public static Material yellow { get { return instance.PRIVATE_yellow; } }




    [SerializeField] Material PRIVATE_black;
    [SerializeField] Material PRIVATE_blue;
    [SerializeField] Material PRIVATE_brown;
    [SerializeField] Material PRIVATE_cyan;
    [SerializeField] Material PRIVATE_dark_green;
    [SerializeField] Material PRIVATE_green;
    [SerializeField] Material PRIVATE_grey;
    [SerializeField] Material PRIVATE_orange;
    [SerializeField] Material PRIVATE_pink;
    [SerializeField] Material PRIVATE_purple;
    [SerializeField] Material PRIVATE_red;
    [SerializeField] Material PRIVATE_white;
    [SerializeField] Material PRIVATE_yellow;



    void Awake()
    {
        instance = this;
    }



}
