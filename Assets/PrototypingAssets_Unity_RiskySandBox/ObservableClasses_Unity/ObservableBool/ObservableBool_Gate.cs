using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using System.Text.RegularExpressions;
using UnityEngine;

public partial class ObservableBool_Gate : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] gate_types gate_type;

    [SerializeField] List<ObservableBool> inputs = new List<ObservableBool>();
    [SerializeField] ObservableBool output;

    //!0|1&2   would be equvilant to not 0 or 1 and 2     0,1,2 are indices of the inputs...
    [SerializeField] string string_expression;
    //TODO - add in debug statements here... to say if we have string_expression being empty...

    public enum gate_types
    {
        and,
        or,
        not,
        xor,
        nand,
        nor,
        xnor,
        boolean_string
    }

    

    void recalculate()
    {
        bool _calculated_value = false;

        switch (gate_type)
        {
            //TODO - note this is not 100% tested but i havn' noticed any problems yet...
            case gate_types.and:
                _calculated_value = inputs.All(input => input.value);
                break;
            case gate_types.or:
                _calculated_value = inputs.Any(input => input.value);
                break;
            case gate_types.not:
                if (inputs.Count == 1)
                {
                    _calculated_value = !inputs[0].value;
                }
                else
                {
                    Debug.LogError("NOT gate should have exactly one input.");
                }
                break;
            case gate_types.xor:
                _calculated_value = inputs.Count(input => input.value) % 2 == 1;
                break;
            case gate_types.nand:
                _calculated_value = !inputs.All(input => input.value);
                break;
            case gate_types.nor:
                _calculated_value = !inputs.Any(input => input.value);
                break;
            case gate_types.xnor:
                _calculated_value = inputs.Count(input => input.value) % 2 == 0;
                break;
            case gate_types.boolean_string:
            {
                _calculated_value = EvaluateBooleanExpression(this.string_expression, this.inputs.Select(x => x.value).ToList());
                break;
            }
        }

        if (this.output == null)
            GlobalFunctions.printError("no output on this game???", this);
        else
            output.value = _calculated_value;
    }



    private void OnEnable()
    {
        foreach(ObservableBool _ObservableBool in inputs)
        {
            if (_ObservableBool == null)
            {
                GlobalFunctions.printError("null input on this gate???", this);
                continue;
            }
            _ObservableBool.OnUpdate += EventReceiver_OnUpdateInput;
        }
        this.recalculate();
    }

    private void OnDisable()
    {
        foreach (ObservableBool _ObservableBool in inputs)
        {
            if (_ObservableBool == null)
                continue;
            _ObservableBool.OnUpdate -= EventReceiver_OnUpdateInput;
        }
    }



    void subscribe(ObservableBool _ObservableBool)
    {
        if (_ObservableBool == null)
            return;

        if (this.inputs.Contains(_ObservableBool))
            return;

        _ObservableBool.OnUpdate += EventReceiver_OnUpdateInput;
        this.inputs.Add(_ObservableBool);
    }

    void unsubscribe(ObservableBool _ObservableBool)
    {
        if (_ObservableBool == null)
            return;

        _ObservableBool.OnUpdate -= EventReceiver_OnUpdateInput;
        this.inputs.Remove(_ObservableBool);
    }

    void EventReceiver_OnUpdateInput(ObservableBool _Input)
    {
        recalculate();
    }

    public static bool EvaluateBooleanExpression(string expression, List<bool> boolList)
    {
        string parsedExpression = ParseExpression(expression, boolList);
        List<string> postfixExpression = ConvertToPostfix(parsedExpression);
        return EvaluatePostfixExpression(postfixExpression);
    }

    private static string ParseExpression(string expression, List<bool> boolList)
    {
        var regex = new Regex(@"\d+");
        return regex.Replace(expression, match =>
        {
            int index = int.Parse(match.Value);
            if (index >= 0 && index < boolList.Count)
            {
                return boolList[index] ? "1" : "0";
            }
            else
            {
                throw new ArgumentException($"Invalid index {index} in the expression.");
            }
        });
    }

    private static List<string> ConvertToPostfix(string expression)
    {
        List<string> output = new List<string>();
        Stack<string> operators = new Stack<string>();
        var regex = new Regex(@"\d+|[!&|()]");
        var matches = regex.Matches(expression);

        foreach (Match match in matches)
        {
            string token = match.Value;
            if (token == "1" || token == "0")
            {
                output.Add(token);
            }
            else if (token == "!")
            {
                operators.Push(token);
            }
            else if (token == "&" || token == "|")
            {
                while (operators.Count > 0 && Precedence(operators.Peek()) >= Precedence(token))
                {
                    output.Add(operators.Pop());
                }
                operators.Push(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    output.Add(operators.Pop());
                }
                operators.Pop(); // Remove "("
            }
        }

        while (operators.Count > 0)
        {
            output.Add(operators.Pop());
        }

        return output;
    }

    private static int Precedence(string op)
    {
        switch (op)
        {
            case "!":
                return 3;
            case "&":
                return 2;
            case "|":
                return 1;
            default:
                return 0;
        }
    }

    private static bool EvaluatePostfixExpression(List<string> tokens)
    {
        Stack<bool> stack = new Stack<bool>();

        foreach (string token in tokens)
        {
            if (token == "1")
            {
                stack.Push(true);
            }
            else if (token == "0")
            {
                stack.Push(false);
            }
            else if (token == "!")
            {
                bool operand = stack.Pop();
                stack.Push(!operand);
            }
            else if (token == "&")
            {
                bool rightOperand = stack.Pop();
                bool leftOperand = stack.Pop();
                stack.Push(leftOperand && rightOperand);
            }
            else if (token == "|")
            {
                bool rightOperand = stack.Pop();
                bool leftOperand = stack.Pop();
                stack.Push(leftOperand || rightOperand);
            }
        }

        return stack.Pop();
    }
}
