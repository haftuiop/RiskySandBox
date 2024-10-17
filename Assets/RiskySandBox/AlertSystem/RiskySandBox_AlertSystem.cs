using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_AlertSystem : MonoBehaviour
{
    public static RiskySandBox_AlertSystem instance;

    [SerializeField] bool debugging;
    [SerializeField] GameObject root;
    [SerializeField] GameObject PRIVATE_alert_prefab;


    [SerializeField] Vector2 alert_start;
    [SerializeField] float alert_height = 30f;


    private void Awake()
    {
        instance = this;
        RiskySandBox_Alert.all_instances.OnUpdate += RiskySandBox_AlertEventReceiver_OnUpdate_all_instances;
    }

    private void OnDestroy()
    {
        instance = null;
        RiskySandBox_Alert.all_instances.OnUpdate -= RiskySandBox_AlertEventReceiver_OnUpdate_all_instances;
    }

    void RiskySandBox_AlertEventReceiver_OnUpdate_all_instances()
    {
        updateAlertPositions();
    }

    public void updateAlertPositions()
    {
        for(int i = 0; i < RiskySandBox_Alert.all_instances.Count; i += 1)
        {
            RiskySandBox_Alert.all_instances[i].GetComponent<RectTransform>().anchoredPosition = this.alert_start + new Vector2(0, alert_height * i);
        }
    }

    public static RiskySandBox_Alert createAlert(string _alert_message,Texture2D _alert_Texture2D,RiskySandBox_Tile _focus_Tile)
    {
        RiskySandBox_Alert _new_Alert = UnityEngine.Object.Instantiate(instance.PRIVATE_alert_prefab,instance.root.transform).GetComponent<RiskySandBox_Alert>();

        _new_Alert.alert_message.value = _alert_message;
        _new_Alert.alert_icon = _alert_Texture2D;
        //TODO - focus_Tile

        return _new_Alert;
    }


}
