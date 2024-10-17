using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Alert : MonoBehaviour
{
    public static ObservableList<RiskySandBox_Alert> all_instances = new ObservableList<RiskySandBox_Alert>();

    [SerializeField] bool debugging;

    public ObservableString alert_message { get { return this.PRIVATE_alert_message; } }
    [SerializeField] ObservableString PRIVATE_alert_message;


    [SerializeField] UnityEngine.UI.RawImage alert_icon_RawImage;

    public Texture2D alert_icon
    {
        get { return this.PRIVATE_alert_icon; }
        set
        {
            PRIVATE_alert_icon = value;
            alert_icon_RawImage.texture = value;

        }
    }
    Texture2D PRIVATE_alert_icon;

    private void Awake()
    {
        RiskySandBox_Alert.all_instances.Add(this);
    }

    private void OnDestroy()
    {
        RiskySandBox_Alert.all_instances.Remove(this);
    }



    public void selfDestruct()
    {
        UnityEngine.Object.Destroy(this.gameObject);
    }
}
