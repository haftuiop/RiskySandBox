using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using TMPro;

public partial class ObservableInt_UIManager : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableInt my_ObservableInt;

    [SerializeField] private List<UnityEngine.UI.Text> UI_Text_components = new List<UnityEngine.UI.Text>();//Text components that should display the PRIVATE_value...
    [SerializeField] private List<UnityEngine.UI.Image> UI_fill_amount_Images = new List<UnityEngine.UI.Image>();//UI Images that should have their .fillAmount set to fill_amount
    [SerializeField] private List<UnityEngine.UI.Slider> UI_Sliders = new List<UnityEngine.UI.Slider>();
    [SerializeField] private List<UnityEngine.UI.InputField> UI_InputFields = new List<UnityEngine.UI.InputField>();

	[SerializeField] private List<TMP_Text> UI_Texts_TextMeshPro = new List<TMP_Text>();
	[SerializeField] private List<TMP_InputField> UI_InputFields_TextMeshPro = new List<TMP_InputField>();


    private void Awake()
    {
		if(my_ObservableInt == null)
        {
			if (this.gameObject.TryGetComponent(out my_ObservableInt) == false)
			{
				GlobalFunctions.printError("unable to find my_ObservableInt!", this);
				return;
			}
		}
		updateUIElements();
		my_ObservableInt.OnUpdate += EventReceiver_OnUpdate;
		my_ObservableInt.OnoverrideUI += EventReceiver_OnoverrideUI;

		foreach (UnityEngine.UI.Slider _Slider in this.UI_Sliders)
		{
			if (_Slider == null)
			{
				GlobalFunctions.printWarning("null Slider", this);
				continue;
			}

			_Slider.wholeNumbers = true;
			_Slider.onValueChanged.AddListener(updateFromSlider);
		}

		foreach (UnityEngine.UI.InputField _InputField in this.UI_InputFields)
		{
			if (_InputField == null)
			{
				GlobalFunctions.printWarning("null InputField", this);
				continue;
			}

			_InputField.contentType = UnityEngine.UI.InputField.ContentType.IntegerNumber;
			_InputField.onValueChanged.AddListener(updateFromInputField);
		}

		foreach(TMP_InputField _InputField in this.UI_InputFields_TextMeshPro)
        {
			if(_InputField == null)
            {
				GlobalFunctions.printWarning("null InputField??", this);
				continue;
            }

			_InputField.contentType = TMP_InputField.ContentType.IntegerNumber;
			_InputField.onValueChanged.AddListener(updateFromInputField);
        }

	}

    private void OnDestroy()
    {
		if (my_ObservableInt == null)
			return;
        my_ObservableInt.OnUpdate -= EventReceiver_OnUpdate;
		my_ObservableInt.OnoverrideUI -= EventReceiver_OnoverrideUI;
		//TODO - unsub to ui elements
	}

    void EventReceiver_OnUpdate(ObservableInt _my_ObservableInt)
    {
        updateUIElements();
    }

	void EventReceiver_OnoverrideUI(ObservableInt _my_ObservableInt)
    {
		if (this.debugging)
			GlobalFunctions.print("_my_ObservableInt.ui_value = "+_my_ObservableInt.ui_value, this);
		updateUIElements();
    }


	void updateFromInputField(string _incoming_value)
	{
		if (int.TryParse(_incoming_value, out int _out_value) == true)
		{
			this.my_ObservableInt.value = _out_value;
		}
		else
		{
			GlobalFunctions.printWarning("WARNING - tried to set a ObservableInt using an input field but was unable to pass the value... '" + _incoming_value + "'", this);
		}

	}

	void updateFromSlider(float _incoming_value)
	{
		this.my_ObservableInt.value = Mathf.FloorToInt(_incoming_value);
	}

	void updateUIElements()
	{
		string _value_string = my_ObservableInt.ui_value.ToString();
		float _fill_amount;

		if (this.my_ObservableInt.min_value == this.my_ObservableInt.max_value)
			_fill_amount = 0f;
		else
			_fill_amount = (this.my_ObservableInt.ui_value - this.my_ObservableInt.min_value) / (float)(this.my_ObservableInt.max_value - this.my_ObservableInt.min_value);

		if (_fill_amount > 1f)
			_fill_amount = 1f;
		if (_fill_amount < 0f)
			_fill_amount = 0f;

		if (this.debugging)
			GlobalFunctions.print("updating ui elements - _value_string = " + _value_string + " _fill_amount = " + _fill_amount,this);

		foreach(UnityEngine.UI.Text _Text in this.UI_Text_components)
		{
			if (_Text != null)
				_Text.text = _value_string;
			else
				GlobalFunctions.printWarning("null Text component...", this);
		}

		foreach (UnityEngine.UI.InputField _InputField in this.UI_InputFields)
		{
			if (_InputField != null)
				_InputField.SetTextWithoutNotify(_value_string);
			else
				GlobalFunctions.printWarning("null value in this.UI_InputFields...", this);
		}

		foreach (UnityEngine.UI.Image _Image in this.UI_fill_amount_Images)
		{
			if (_Image != null)
				_Image.fillAmount = _fill_amount;
			else
				GlobalFunctions.printWarning("null value in this.UI_fill_amount_Images...", this);
		}

		foreach (UnityEngine.UI.Slider _Slider in this.UI_Sliders)
		{
			if (_Slider != null)
			{
				_Slider.minValue = this.my_ObservableInt.min_value;
				_Slider.maxValue = this.my_ObservableInt.max_value;
				_Slider.SetValueWithoutNotify(this.my_ObservableInt.value);
			}
			else
				GlobalFunctions.printWarning("null value in this.UI_SLiders...", this);
		}

		foreach(TMP_Text _Text in this.UI_Texts_TextMeshPro)
        {
			if(_Text == null)
            {
				GlobalFunctions.printWarning("null Text component??",this);
				continue;
            }

			_Text.text = _value_string;
        }

		foreach (TMP_InputField _InputField in this.UI_InputFields_TextMeshPro)
		{
			if (_InputField != null)
				_InputField.SetTextWithoutNotify(_value_string);
			else
				GlobalFunctions.printWarning("null value in this.UI_InputFields...", this);
		}


	}

}
