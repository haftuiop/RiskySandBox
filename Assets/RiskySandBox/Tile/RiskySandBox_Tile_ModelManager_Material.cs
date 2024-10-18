using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Tile_ModelManager_Material : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] RiskySandBox_Tile my_Tile;
    ObservableInt my_Team_ID { get { return this.my_Tile.my_Team_ID; } }
    ObservableBool enable_FOW { get { return my_Tile.enable_dark_fow; } }
    

    [SerializeField] MeshRenderer my_MeshRenderer;
    [SerializeField] Material fow_Material;
    [SerializeField] Material null_Team_Material;



    private void Awake()
    {
        my_Team_ID.OnUpdate += delegate { updateMaterial(); };
        this.enable_FOW.OnUpdate += delegate { updateMaterial(); };

        RiskySandBox_MainGame.instance.display_bonuses.OnUpdate += EventReceiver_OnVariableUpdate_display_bonuses;

        RiskySandBox_HumanPlayer.OnVariableUpdate_deploy_target_STATIC += EventReceiver_OnVariableUpdate_deploy_target;
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_start_STATIC += EventReceiver_OnVariableUpdate_attack_start;
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_target_STATIC += EventReceiver_OnVariableUpdate_attack_target;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_start_STATIC += EventReceiver_OnVariableUpdate_fortify_start;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_target_STATIC += EventReceiver_OnVariableUpdate_fortify_target;

        RiskySandBox_Tile.OnVariableUpdate_my_LevelEditor_Material_STATIC += EventReceiver_OnVariableUpdate_my_LevelEditor_Material;
    }

    private void OnDestroy()
    {
        RiskySandBox_MainGame.instance.display_bonuses.OnUpdate -= EventReceiver_OnVariableUpdate_display_bonuses;

        RiskySandBox_HumanPlayer.OnVariableUpdate_deploy_target_STATIC -= EventReceiver_OnVariableUpdate_deploy_target;
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_start_STATIC -= EventReceiver_OnVariableUpdate_attack_start;
        RiskySandBox_HumanPlayer.OnVariableUpdate_attack_target_STATIC -= EventReceiver_OnVariableUpdate_attack_target;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_start_STATIC -= EventReceiver_OnVariableUpdate_fortify_start;
        RiskySandBox_HumanPlayer.OnVariableUpdate_fortify_target_STATIC -= EventReceiver_OnVariableUpdate_fortify_target;

        RiskySandBox_Tile.OnVariableUpdate_my_LevelEditor_Material_STATIC -= EventReceiver_OnVariableUpdate_my_LevelEditor_Material;
    }

    void EventReceiver_OnVariableUpdate_deploy_target(RiskySandBox_HumanPlayer _HumanPlayer) { updateMaterial(); }
    void EventReceiver_OnVariableUpdate_attack_start(RiskySandBox_HumanPlayer _HumanPlayer) { updateMaterial(); }
    void EventReceiver_OnVariableUpdate_attack_target(RiskySandBox_HumanPlayer _HumanPlayer) { updateMaterial(); }
    void EventReceiver_OnVariableUpdate_fortify_start(RiskySandBox_HumanPlayer _HumanPlayer) { updateMaterial(); }
    void EventReceiver_OnVariableUpdate_fortify_target(RiskySandBox_HumanPlayer _HumanPlayer) { updateMaterial(); }
    void EventReceiver_OnVariableUpdate_display_bonuses(ObservableBool _display_bonuses) { this.updateMaterial(); }
    void EventReceiver_OnVariableUpdate_my_LevelEditor_Material(RiskySandBox_Tile _Tile){ if (_Tile == this.my_Tile) { this.updateMaterial(); } }


    // Start is called before the first frame update
    void Start()
    {
        updateMaterial();
    }

    

    void updateMaterial()
    {
        if (RiskySandBox_LevelEditor.is_enabled)//if we are in LevelEditor mode...
        {
            my_MeshRenderer.material = this.my_Tile.my_LevelEditor_Material;
            return;
        }

        if (RiskySandBox_ReplaySystem.is_enabled)
        {
            //decide if we should enable fow???
        }


        my_MeshRenderer.material = this.my_Tile.my_Material;

        if (this.my_Tile.my_Team == null)
        {
            my_MeshRenderer.material = this.null_Team_Material;
        }

        else
        {
            this.my_Tile.my_Material.color = this.my_Tile.my_Team.my_Color;
        }

        if (this.enable_FOW)
        {
            my_MeshRenderer.material = this.fow_Material;

        }

        if (RiskySandBox_MainGame.instance.display_bonuses == true)
        {
            //ok! lets set the material color match the color of that bonus...
            RiskySandBox_Bonus _my_Bonus = RiskySandBox_Bonus.GET_RiskySandBox_Bonus(this.my_Tile);

            if (_my_Bonus != null)
            {
                my_Tile.my_Material.color = _my_Bonus.my_Color;
            }

            my_MeshRenderer.material = my_Tile.my_Material;

        }


    }
}
