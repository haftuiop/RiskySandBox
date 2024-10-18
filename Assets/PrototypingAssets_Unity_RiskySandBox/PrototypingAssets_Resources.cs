using System.Collections;using System.Collections.Generic;
using UnityEngine;

public partial class PrototypingAssets_Resources : MonoBehaviour
{
    static PrototypingAssets_Resources instance;

    [SerializeField] bool debugging;



    private void Awake()
    {
        instance = this;
    }





}
