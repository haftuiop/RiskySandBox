using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using TMPro;


public partial class ObservableFloat_UIManager : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableFloat my_ObservableFloat;

    [SerializeField] private List<UnityEngine.UI.Text> UI_Text_components = new List<UnityEngine.UI.Text>();
    [SerializeField] private List<UnityEngine.UI.Image> UI_fill_amount_Images = new List<UnityEngine.UI.Image>();
    [SerializeField] private List<UnityEngine.UI.Slider> UI_Sliders = new List<UnityEngine.UI.Slider>();
    [SerializeField] private List<UnityEngine.UI.InputField> UI_InputFields = new List<UnityEngine.UI.InputField>();


	[SerializeField] private List<TMP_Text> UI_Texts_TextMeshPro = new List<TMP_Text>();
	[SerializeField] private List<TMP_InputField> UI_InputFields_TextMeshPro = new List<TMP_InputField>();


    private void Awake()
    {
		if(my_ObservableFloat == null)
        {
			if (this.gameObject.TryGetComponent(out my_ObservableFloat) == false)
			{
				GlobalFunctions.printError("unable to find my_ObservableFloat!", this);
				return;
			}
		}			
        my_ObservableFloat.OnUpdate += EventReceiver_OnUpdate;

		this.updateUIElements();

		foreach (UnityEngine.UI.InputField _InputField in this.UI_InputFields)
		{
			if (_InputField == null)
			{ 
				GlobalFunctions.printWarning("null InputField", this);
				continue;
			}

			_InputField.contentType = UnityEngine.UI.InputField.ContentType.DecimalNumber;
			_InputField.onValueChanged.AddListener(updateFromInputField);
		}

		foreach (UnityEngine.UI.Slider _Slider in this.UI_Sliders)
		{
			if(_Slider == null)
            {
				GlobalFunctions.printWarning("null Slider", this);
				continue;
            }

			_Slider.wholeNumbers = false;
			_Slider.onValueChanged.AddListener(updateFromSlider);
		}

		foreach(TMP_InputField _InputField in this.UI_InputFields_TextMeshPro)
        {
			if(_InputField == null)
            {
				GlobalFunctions.printWarning("WARNING null InputField", this);
				continue;
            }

			_InputField.contentType = TMP_InputField.ContentType.DecimalNumber;
			_InputField.onValueChanged.AddListener(updateFromInputField);
        }

	}


    private void OnDestroy()
    {
        if(my_ObservableFloat != null)
            my_ObservableFloat.OnUpdate -= EventReceiver_OnUpdate;
		//TODO unsub to ui elements...
    }


    void EventReceiver_OnUpdate(ObservableFloat _my_ObservableFloat)
    {
		updateUIElements();
    }


	void updateFromInputField(string _incoming_value)
	{
		if (_incoming_value.EndsWith("."))//user could be trying to enter something like 2. <whatever>  
			return;

		if (_incoming_value.EndsWith("0") && _incoming_value.Contains("."))//the user has entered something like 2.1420 (which we dont care about as the final 0 is not actually changing the number...
			return;

		if (_incoming_value == "-0")
			return;

		if (float.TryParse(_incoming_value, out float _out_value) == true)
		{
			this.my_ObservableFloat.value = _out_value;
		}
		else
		{
			GlobalFunctions.printWarning("WARNING - tried to set a ObservableFloat using an input field but was unable to pass the value... '" + _incoming_value + "'", this);
		}

	}

	void updateFromSlider(float _incoming_value)
	{
		this.my_ObservableFloat.value = _incoming_value;
	}




	void updateUIElements()
	{
		string _value_string = "" + this.my_ObservableFloat.value;
		foreach(UnityEngine.UI.Text _Text in this.UI_Text_components)
		{
			if(_Text == null)
            {
				GlobalFunctions.printWarning("null Text in this.UI_Text_Components...", this);
				continue;
            }
			_Text.text = _value_string;
		}

		float _fill_amount = this.my_ObservableFloat.ilerp_value;
		foreach (UnityEngine.UI.Image _Image in this.UI_fill_amount_Images)
		{
			if(_Image == null)
            {
				GlobalFunctions.printWarning("null Image in this.UI_fill_amount_Images",this);
				continue;
            }
			_Image.fillAmount = _fill_amount;
		}

		foreach (UnityEngine.UI.InputField _InputField in this.UI_InputFields)
		{
			if (_InputField == null)
			{
				GlobalFunctions.printWarning("null value in this.UI_InputFields component...", this);
				continue;
			}
			_InputField.SetTextWithoutNotify(_value_string);
		}

		foreach (UnityEngine.UI.Slider _Slider in this.UI_Sliders)
		{
			if (_Slider == null)
			{
				GlobalFunctions.printWarning("null value in this.UI_SLiders component...", this);
				continue;
			}
			_Slider.minValue = this.my_ObservableFloat.min_value;
			_Slider.maxValue = this.my_ObservableFloat.max_value;
			_Slider.SetValueWithoutNotify(this.my_ObservableFloat.value);
		}

		foreach(TMP_Text _Text in this.UI_Texts_TextMeshPro)
        {
			if(_Text == null)
            {
				GlobalFunctions.printWarning("null Text???", this);
				continue;
            }
			_Text.text = _value_string;
        }

		foreach(TMP_InputField _InputField in this.UI_InputFields_TextMeshPro)
		{
			if(_InputField == null)
            {
				GlobalFunctions.printWarning("null value in this.UI_InputFields_TextMeshPro",this);
				continue;
            }
			_InputField.SetTextWithoutNotify(_value_string);
        }

	}



}
