using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using TMPro;

public partial class ObservableVector3_UIManager : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableVector3 my_ObservableVector3;

	[SerializeField] private List<UnityEngine.UI.InputField> UI_InputFields_x = new List<UnityEngine.UI.InputField>();
	[SerializeField] private List<UnityEngine.UI.InputField> UI_InputFields_y = new List<UnityEngine.UI.InputField>();
	[SerializeField] private List<UnityEngine.UI.InputField> UI_InputFields_z = new List<UnityEngine.UI.InputField>();

	[SerializeField] private List<UnityEngine.UI.Text> UI_Text = new List<UnityEngine.UI.Text>();
	[SerializeField] private List<UnityEngine.UI.Text> UI_Text_x = new List<UnityEngine.UI.Text>();
	[SerializeField] private List<UnityEngine.UI.Text> UI_Text_y = new List<UnityEngine.UI.Text>();
	[SerializeField] private List<UnityEngine.UI.Text> UI_Text_z = new List<UnityEngine.UI.Text>();


	[SerializeField] private List<UnityEngine.UI.Slider> UI_Sliders_x = new List<UnityEngine.UI.Slider>();
	[SerializeField] private List<UnityEngine.UI.Slider> UI_Sliders_y = new List<UnityEngine.UI.Slider>();
	[SerializeField] private List<UnityEngine.UI.Slider> UI_Sliders_z = new List<UnityEngine.UI.Slider>();


	[SerializeField] private List<TMP_InputField> UI_InputFields_x_TextMeshPro = new List<TMP_InputField>();
	[SerializeField] private List<TMP_InputField> UI_InputFields_y_TextMeshPro = new List<TMP_InputField>();
	[SerializeField] private List<TMP_InputField> UI_InputFields_z_TextMeshPro = new List<TMP_InputField>();

	[SerializeField] private List<TMP_Text> UI_Texts_TextMeshPro = new List<TMP_Text>();
	[SerializeField] private List<TMP_Text> UI_Texts_x_TextMeshPro = new List<TMP_Text>();
	[SerializeField] private List<TMP_Text> UI_Texts_y_TextMeshPro = new List<TMP_Text>();
	[SerializeField] private List<TMP_Text> UI_Texts_z_TextMeshPro = new List<TMP_Text>();


	private void Awake()
    {
        if(my_ObservableVector3 == null)
        {
			if (this.gameObject.TryGetComponent(out my_ObservableVector3) == false)
			{
				GlobalFunctions.printError("unable to find my_ObservableVector3!", this);
				return;
			}
		}

		my_ObservableVector3.OnUpdate += EventReceiver_OnUpdate;
		updateUIElements();

		foreach (UnityEngine.UI.InputField _InputField in UI_InputFields_x)
		{
			_InputField.contentType = UnityEngine.UI.InputField.ContentType.DecimalNumber;
			_InputField.onValueChanged.AddListener(updateXFromInputField);
		}

		foreach (UnityEngine.UI.InputField _InputField in UI_InputFields_y)
		{
			_InputField.contentType = UnityEngine.UI.InputField.ContentType.DecimalNumber;
			_InputField.onValueChanged.AddListener(updateYFromInputField);
		}

		foreach (UnityEngine.UI.InputField _InputField in UI_InputFields_z)
		{
			_InputField.contentType = UnityEngine.UI.InputField.ContentType.DecimalNumber;
			_InputField.onValueChanged.AddListener(updateZFromInputField);
		}

		foreach(UnityEngine.UI.Slider _Slider in this.UI_Sliders_x)
        {
			_Slider.onValueChanged.AddListener(this.updateFromSlider_x);
        }

		foreach (UnityEngine.UI.Slider _Slider in this.UI_Sliders_y)
		{
			_Slider.onValueChanged.AddListener(this.updateFromSlider_y);
		}

		foreach (UnityEngine.UI.Slider _Slider in this.UI_Sliders_z)
		{
			_Slider.onValueChanged.AddListener(this.updateFromSlider_z);
		}


		foreach (TMP_InputField _InputField in UI_InputFields_x_TextMeshPro)
		{
			_InputField.contentType = TMP_InputField.ContentType.DecimalNumber;
			_InputField.onValueChanged.AddListener(updateXFromInputField);
		}

		foreach (TMP_InputField _InputField in UI_InputFields_y_TextMeshPro)
		{
			_InputField.contentType = TMP_InputField.ContentType.DecimalNumber;
			_InputField.onValueChanged.AddListener(updateYFromInputField);
		}

		foreach (TMP_InputField _InputField in UI_InputFields_z_TextMeshPro)
		{
			_InputField.contentType = TMP_InputField.ContentType.DecimalNumber;
			_InputField.onValueChanged.AddListener(updateZFromInputField);
		}



	}

    private void OnDestroy()
    {
        if (my_ObservableVector3 != null)
            my_ObservableVector3.OnUpdate -= EventReceiver_OnUpdate;
		//UNSUB to inputfuelds
    }

    void EventReceiver_OnUpdate(ObservableVector3 _my_ObservableVector3)
    {
		updateUIElements();
    }



	void updateUIElements()
	{
		string _x_string = this.my_ObservableVector3.x.ToString();
		string _y_string = this.my_ObservableVector3.y.ToString();
		string _z_string = this.my_ObservableVector3.z.ToString();

		foreach (UnityEngine.UI.InputField _InputField in this.UI_InputFields_x)
		{
			if (_InputField == null)
			{
				GlobalFunctions.printWarning("null value in this.UI_InputFields component...", this);
				continue;
			}

			_InputField.SetTextWithoutNotify(_x_string);
		}

		foreach (UnityEngine.UI.InputField _InputField in this.UI_InputFields_y)
		{
			if (_InputField == null)
			{
				GlobalFunctions.printWarning("null value in this.UI_InputFields component...", this);
				continue;
			}

			_InputField.SetTextWithoutNotify(_y_string);
		}

		foreach (UnityEngine.UI.InputField _InputField in this.UI_InputFields_z)
		{
			if (_InputField == null)
			{
				GlobalFunctions.printWarning("null value in this.UI_InputFields component...", this);
				continue;
			}

			_InputField.SetTextWithoutNotify(_z_string);
		}

		foreach(UnityEngine.UI.Text _Text in this.UI_Text)
        {
			_Text.text = string.Format("({0}, {1}, {2})", this.my_ObservableVector3.x, this.my_ObservableVector3.y, this.my_ObservableVector3.z);
        }

		foreach (UnityEngine.UI.Text _Text in this.UI_Text_x)
		{
			_Text.text = ""+ this.my_ObservableVector3.x;
		}

		foreach (UnityEngine.UI.Text _Text in this.UI_Text_y)
		{
			_Text.text = "" + this.my_ObservableVector3.y;
		}

		foreach (UnityEngine.UI.Text _Text in this.UI_Text_z)
		{
			_Text.text = "" + this.my_ObservableVector3.z;
		}


		foreach (TMP_InputField _InputField in this.UI_InputFields_x_TextMeshPro)
		{
			if (_InputField == null)
			{
				GlobalFunctions.printWarning("null value in this.UI_InputFields component...", this);
				continue;
			}

			_InputField.SetTextWithoutNotify(_x_string);
		}

		foreach (TMP_InputField _InputField in this.UI_InputFields_y_TextMeshPro)
		{
			if (_InputField == null)
			{
				GlobalFunctions.printWarning("null value in this.UI_InputFields component...", this);
				continue;
			}

			_InputField.SetTextWithoutNotify(_y_string);
		}

		foreach (TMP_InputField _InputField in this.UI_InputFields_z_TextMeshPro)
		{
			if (_InputField == null)
			{
				GlobalFunctions.printWarning("null value in this.UI_InputFields component...", this);
				continue;
			}

			_InputField.SetTextWithoutNotify(_z_string);
		}

		foreach(UnityEngine.UI.Slider _Slider in this.UI_Sliders_x)
        {
			if(_Slider == null)
            {
				GlobalFunctions.printWarning("null value in this.UI_Sliders_x...", this);
				continue;
            }

			_Slider.minValue = this.my_ObservableVector3.min_value.x;
			_Slider.maxValue = this.my_ObservableVector3.max_value.x;
			_Slider.SetValueWithoutNotify(this.my_ObservableVector3.value.x);
        }

		foreach(UnityEngine.UI.Slider _Slider in this.UI_Sliders_y)
        {
			if(_Slider == null)
            {
				GlobalFunctions.printWarning("null value in this.UI_Sliders_y...",this);
				continue;
            }

			_Slider.minValue = this.my_ObservableVector3.min_value.y;
			_Slider.maxValue = this.my_ObservableVector3.max_value.y;
			_Slider.SetValueWithoutNotify(this.my_ObservableVector3.value.y);
        }

		foreach (UnityEngine.UI.Slider _Slider in this.UI_Sliders_z)
		{
			if (_Slider == null)
			{
				GlobalFunctions.printWarning("null value in this.UI_Sliders_z...", this);
				continue;
			}

			_Slider.minValue = this.my_ObservableVector3.min_value.z;
			_Slider.maxValue = this.my_ObservableVector3.max_value.z;
			_Slider.SetValueWithoutNotify(this.my_ObservableVector3.value.z);
		}


		foreach (TMP_Text _Text in this.UI_Texts_TextMeshPro)
		{
			_Text.text = string.Format("({0}, {1}, {2})", this.my_ObservableVector3.x, this.my_ObservableVector3.y, this.my_ObservableVector3.z);
		}

		foreach (TMP_Text _Text in this.UI_Texts_x_TextMeshPro)
		{
			_Text.text = "" + this.my_ObservableVector3.x;
		}

		foreach (TMP_Text _Text in this.UI_Texts_y_TextMeshPro)
		{
			_Text.text = "" + this.my_ObservableVector3.y;
		}

		foreach (TMP_Text _Text in this.UI_Texts_z_TextMeshPro)
		{
			_Text.text = "" + this.my_ObservableVector3.z;
		}

	}

	bool parseInputFieldString(string _incoming_value, out float _return_value)
	{
		_return_value = 0;
		if (_incoming_value.EndsWith("."))//user could be trying to enter something like 2. <whatever>  
			return false;

		if (_incoming_value.EndsWith("0") && _incoming_value.Contains("."))//the user has entered something like 2.1420 (which we dont care about as the final 0 is not actually changing the number...
			return false;


		if (_incoming_value == "-0")
			return false;


		if (float.TryParse(_incoming_value, out _return_value) == true)
		{
			return true;
		}
		return false;
	}


	void updateXFromInputField(string _incoming_value)
	{
		if (parseInputFieldString(_incoming_value, out float _parsed_value))
		{
			this.my_ObservableVector3.x = _parsed_value;
		}
	}

	void updateYFromInputField(string _incoming_value)
	{
		if (parseInputFieldString(_incoming_value, out float _parsed_value))
		{
			this.my_ObservableVector3.y = _parsed_value;
		}
	}

	void updateZFromInputField(string _incoming_value)
	{
		if (parseInputFieldString(_incoming_value, out float _parsed_value))
		{
			this.my_ObservableVector3.z = _parsed_value;
		}
	}

	void updateFromSlider_x(float _incoming_value)
    {
		this.my_ObservableVector3.x = _incoming_value;
    }

	void updateFromSlider_y(float _incoming_value)
    {
		this.my_ObservableVector3.y = _incoming_value;
    }

	void updateFromSlider_z(float _incoming_value)
    {
		this.my_ObservableVector3.z = _incoming_value;
    }
}
