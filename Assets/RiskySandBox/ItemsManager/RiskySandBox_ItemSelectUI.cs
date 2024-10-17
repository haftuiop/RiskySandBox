using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_ItemSelectUI : MonoBehaviour
{
    [SerializeField] bool debugging;

   
    [SerializeField] UnityEngine.UI.Button my_Button;

    public ObservableString item_type { get { return this.PRIVATE_item_type; } }
    [SerializeField] ObservableString PRIVATE_item_type;

    public ObservableInt current_quantity { get { return this.PRIVATE_current_quantity; } }
    [SerializeField] ObservableInt PRIVATE_current_quantity;


    public event Action<RiskySandBox_ItemSelectUI> OnButtonPressed;



    private void Awake()
    {
        my_Button.onClick.AddListener(delegate { EventReceiver_OnButtonPressed(); });
        this.item_type.OnUpdate += EventReceiver_OnVariableUpdate_item_type;
    }


    void EventReceiver_OnVariableUpdate_item_type(ObservableString _item_type)
    {

        try
        {
            Texture2D _Texture2D = RiskySandBox_ItemsManager.GET_itemTexture2D(_item_type);
            // Create a sprite from the texture
            Sprite sprite = Sprite.Create(
                _Texture2D,
                new Rect(0, 0, _Texture2D.width, _Texture2D.height),
                new Vector2(0.5f, 0.5f)  // Pivot point at the center
            );

            // Assign the sprite to the SpriteRenderer component of the GameObject
            my_Button.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            
        }

        catch (IndexOutOfRangeException e)
        {
            Debug.LogError("Index out of range! " + e.Message);
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.LogError("Argument out of range! " + e.Message);
        }



    }

    public void EventReceiver_OnButtonPressed()
    {
        this.OnButtonPressed?.Invoke(this);
    }

    
}
