using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class ObservableClasses_VariableSettings : MonoBehaviour
{
    
    [SerializeField] bool debugging;
    [SerializeField] bool PRIVATE_is_copy_paste_variable = true;


    enum authority_types
    {
        local,//not a multiplayer variable..
        server,//only the server should change the value...
        shared,//anyone can change the value...
        owner//the owner of the object can change the value...
    }

    [SerializeField] authority_types authority_type = authority_types.local;

    public bool synchronise_immediately { get { return this.PRIVATE_synchronise_immediately; } }
    public float auto_resync_rate {get { return PRIVATE_auto_resync_rate; }}

    [SerializeField] private bool PRIVATE_synchronise_immediately = true;
    [SerializeField] private float PRIVATE_auto_resync_rate = 0f;


    public bool is_local_authority { get { return this.authority_type == authority_types.local; } }
    public bool is_server_authority { get { return this.authority_type == authority_types.server; } }
    public bool is_owner_authority { get { return this.authority_type == authority_types.owner; } }
    public bool is_shared_authority { get { return this.authority_type == authority_types.shared; } }
    public bool is_copy_paste_variable { get { return this.PRIVATE_is_copy_paste_variable; } }


}
