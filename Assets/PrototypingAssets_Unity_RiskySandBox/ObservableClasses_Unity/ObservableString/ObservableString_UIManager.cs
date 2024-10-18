using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;
using TMPro;

public partial class ObservableString_UIManager : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] ObservableString my_ObservableString;

    [SerializeField] private List<UnityEngine.UI.Text> UI_Text_components = new List<UnityEngine.UI.Text>();
    //TODO - dropdown?
    [SerializeField] private List<UnityEngine.UI.InputField> UI_InputFields = new List<UnityEngine.UI.InputField>();

    [SerializeField] private List<UnityEngine.UI.Dropdown> UI_Dropdowns = new List<UnityEngine.UI.Dropdown>();


    [SerializeField] List<TMP_Text> UI_Texts_TextMeshPro = new List<TMP_Text>();
    [SerializeField] List<TMP_Dropdown> UI_Dropdowns_TextMeshPro = new List<TMP_Dropdown>();
    [SerializeField] List<TMP_InputField> UI_InputFields_TextMeshPro = new List<TMP_InputField>();


    [SerializeField] private List<string> UI_Dropdown_values = new List<string>();

    private void Awake()
    {
        if(my_ObservableString == null)
        {
            if (this.gameObject.TryGetComponent(out my_ObservableString) == false)
            {
                GlobalFunctions.printError("unable to find my_ObservableString!", this);
                return;
            }
        }
        my_ObservableString.OnUpdate += EventReceiver_OnUpdate;

        foreach (UnityEngine.UI.InputField _InputField in this.UI_InputFields)
        {
            if(_InputField == null)
            {
                GlobalFunctions.printWarning("null InputField...", this);
                continue;
            }
            _InputField.contentType = UnityEngine.UI.InputField.ContentType.Standard;
            _InputField.onValueChanged.AddListener(updateFromInputField);
        }

        foreach (UnityEngine.UI.Dropdown _Dropdown in this.UI_Dropdowns)
        {
            if(_Dropdown == null)
            {
                GlobalFunctions.printWarning("null Dropdown", this);
                continue;
            }
            _Dropdown.ClearOptions();
            _Dropdown.AddOptions(UI_Dropdown_values);
            _Dropdown.onValueChanged.AddListener(updateFromDropdown);
        }

        foreach(TMP_InputField _InputField in this.UI_InputFields_TextMeshPro)
        {
            if(_InputField == null)
            {
                GlobalFunctions.printWarning("null InputField...", this);
                continue;
            }
            _InputField.contentType = TMP_InputField.ContentType.Standard;
            _InputField.onValueChanged.AddListener(updateFromInputField);
        }

        foreach (TMP_Dropdown _Dropdown in this.UI_Dropdowns_TextMeshPro)
        {
            if (_Dropdown == null)
            {
                GlobalFunctions.printWarning("null Dropdown", this);
                continue;
            }
            _Dropdown.ClearOptions();
            _Dropdown.AddOptions(UI_Dropdown_values);
            _Dropdown.onValueChanged.AddListener(updateFromDropdown);
        }





        updateUIElements();
    }


    private void Start()
    {
        if (UI_Dropdown_values.Count() > 0)
        {
            bool _is_a_dd_value = false;
            //ok... lets make sure my_ObservableString is one of these values
            foreach (string _dd_value in this.UI_Dropdown_values)
            {
                if (_dd_value == this.my_ObservableString.value)
                {
                    _is_a_dd_value = true;
                    break;
                }
            }

            if (_is_a_dd_value == false)
                this.my_ObservableString.value = this.UI_Dropdown_values[0];

        }
    }



    private void OnDestroy()
    {
        if (my_ObservableString != null)
            my_ObservableString.OnUpdate -= EventReceiver_OnUpdate;

        //TODO unsub to inputfield(s)/dropdowns????
    }

    void updateFromInputField(string _incoming_value)
    {
        this.my_ObservableString.value = _incoming_value;
    }

    void updateFromDropdown(int _incoming_index)
    {
        //get the value...
        this.my_ObservableString.value = this.UI_Dropdown_values[_incoming_index];
    }

    void EventReceiver_OnUpdate(ObservableString _my_ObservableString)
    {
        updateUIElements();
    }

    void updateUIElements()
    {
        foreach (UnityEngine.UI.Text _Text in this.UI_Text_components)
        {
            _Text.text = this.my_ObservableString.value;
        }

        foreach (UnityEngine.UI.InputField _InputField in this.UI_InputFields)
        {
            if (_InputField == null)
            {
                GlobalFunctions.printWarning("WARNING - null value in this.UI_InputFields", this);
                continue;
            }

            _InputField.SetTextWithoutNotify(this.my_ObservableString.value);
        }

        foreach(UnityEngine.UI.Dropdown _dropdown in this.UI_Dropdowns)
        {
            if(_dropdown == null)
            {
                GlobalFunctions.printWarning("WARNING - null value in this.UI_Dropdowns", this);
                continue;
            }

            int _dropdown_index = this.UI_Dropdown_values.IndexOf(this.my_ObservableString.value);
            _dropdown.SetValueWithoutNotify(_dropdown_index);

        }

        foreach(TMP_Text _Text in this.UI_Texts_TextMeshPro)
        {
            if(_Text == null)
            {
                GlobalFunctions.printWarning("WARNING - null text???", this);
                continue;
            }

            _Text.text = this.my_ObservableString.value;
        }

        foreach(TMP_InputField _InputField in this.UI_InputFields_TextMeshPro)
        {
            if(_InputField == null)
            {
                GlobalFunctions.printWarning("WARNING - null InputField???", this);
                continue;
            }

            _InputField.SetTextWithoutNotify(this.my_ObservableString.value);
        }

        foreach(TMP_Dropdown _Dropdown in this.UI_Dropdowns_TextMeshPro)
        {
            if(_Dropdown == null)
            {
                GlobalFunctions.printWarning("WARNING - null dropdown???", this);
                continue;
            }

            int _dropdown_index = this.UI_Dropdown_values.IndexOf(this.my_ObservableString.value);
            _Dropdown.SetValueWithoutNotify(_dropdown_index);
        }

    }
}
