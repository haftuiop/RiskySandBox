using System.Collections;using System.Collections.Generic;
using UnityEngine;

public partial class PrototypingAssets_DontDestroyOnLoad : MonoBehaviour
{
    static readonly string class_name = "PrototypingAssets_DonDestroyOnLoad"; 
    [SerializeField] bool debugging;


    [SerializeField] List<GameObject> GameObjects = new List<GameObject>();


    void Awake()
    {
        if (debugging)
            GlobalFunctions.print(class_name+".Awake", this);
        foreach(GameObject _GameObject in GameObjects)
        {
            if (_GameObject == null)
                continue;
            DontDestroyOnLoad(_GameObject);
        }
    }
}
