using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_Team_MuteButton : MonoBehaviour
{
    public static event Action<RiskySandBox_Team> OnmuteButtonPressed;

    [SerializeField] bool debugging;
    [SerializeField] Sprite muted_sprite;
    [SerializeField] Sprite unmuted_sprite;

    [SerializeField] RiskySandBox_Team my_Team;
    [SerializeField] UnityEngine.UI.Button my_Button;

    bool is_muted = false;


    private void Awake()
    {
        my_Button.onClick.AddListener(delegate { this.EventReceiver_OnbuttonPressed(); });

        updateVisuals();
    }



    void updateVisuals()
    {
        if (this.is_muted)
            my_Button.GetComponent<UnityEngine.UI.Image>().sprite = muted_sprite;
        else
            my_Button.GetComponent<UnityEngine.UI.Image>().sprite = unmuted_sprite;
    }

    void EventReceiver_OnbuttonPressed()
    {
        OnmuteButtonPressed?.Invoke(this.my_Team);
        this.is_muted = !this.is_muted;

        updateVisuals();




    }
}
