using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Tile_TextEmitter : MonoBehaviour
{

    [SerializeField] GameObject emitText_prefab;


    private void OnEnable()
    {
        RiskySandBox_Tile.OnVariableUpdate_num_troops_STATIC += EventReceiver_OnSET_num_troops;
    }

    private void OnDisable()
    {
        RiskySandBox_Tile.OnVariableUpdate_num_troops_STATIC -= EventReceiver_OnSET_num_troops;
    }

    void emitText(RiskySandBox_Tile _Tile, string _text)
    {
        UnityEngine.UI.Text _new_Text = UnityEngine.Object.Instantiate(emitText_prefab).transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        _new_Text.gameObject.transform.position = _Tile.transform.position;
        _new_Text.text = _text;
        _new_Text.GetComponent<RectTransform>().localScale = new Vector3(_Tile.UI_scale_factor, _Tile.UI_scale_factor, _Tile.UI_scale_factor);
        _new_Text.transform.parent.GetComponent<RiskySandBox_Tile_TextPrefab>().movement_speed *= _Tile.UI_scale_factor * 0.02f;
        _new_Text.transform.position = _Tile.UI_position;
    }

    void EventReceiver_OnSET_num_troops(RiskySandBox_Tile _Tile)
    {
        emitText(_Tile, "" + _Tile.num_troops.delta_value);
    }
      






}
