using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;


public partial class GlobalFunctions
{



	public static T GetRandomItem<T>(List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			throw new ArgumentException("The list cannot be null or empty.", nameof(list));
		}

		int index = GlobalFunctions.randomInt(0, list.Count - 1);
		return list[index];
	}



	/// <summary>
	/// print a '_message' into the 'console'
	/// </summary>
	[HideInCallstack]
	public static void print(string _message, System.Object _context, params object[] _parameter_values)
	{

		
	#if UNITY_EDITOR == true || UNITY_WEBGL == false//apparently webgl doesn't allow the code below
		var st = new System.Diagnostics.StackTrace();
		string class_name = st.GetFrame(1).GetMethod().DeclaringType.Name;
		string _method_name = st.GetFrame(1).GetMethod().Name;


		string _parameter_string = "";
		System.Reflection.ParameterInfo[] _ParameterInfos = st.GetFrame(1).GetMethod().GetParameters();


		if (_parameter_values.Count() > 0)
        {
			if(_parameter_values.Count() != _ParameterInfos.Count())
            {
				Debug.LogWarning("WARNING - _parameter_values.Count != _ParameterInfos.Count... this means _parameter_values will be ignored...");

				_parameter_values = new object[0];//just ignore the parameter values...
			}
        }


		



		foreach (System.Reflection.ParameterInfo _ParameterInfo in _ParameterInfos)
		{
			if (_parameter_values.Count() > 0)
			{ 
				_parameter_string += _ParameterInfo.Name + "='" + _parameter_values[_ParameterInfo.Position].ToString() + "', ";
			}
				
			else
				_parameter_string += _ParameterInfo.Name + ", ";

		}

		if (st.GetFrame(1).GetMethod().GetParameters().Length > 0)
			_parameter_string = _parameter_string.Substring(0, _parameter_string.Length - 2);

		

		_message = class_name + "." + _method_name + "("+ _parameter_string+"): " + _message;
	#endif

		if (_context == null)
			Debug.Log(_message);
		else
			Debug.Log(_message, (UnityEngine.Object)_context);
	}
	/// <summary>
	/// print a 'warning' message into the 'console'
	/// </summary>
	[HideInCallstack]
	public static void printWarning(string _message, System.Object _context)
	{
	#if UNITY_EDITOR == true || UNITY_WEBGL == false//apparently webgl doesn't allow the code below

		var st = new System.Diagnostics.StackTrace();
		string class_name = st.GetFrame(1).GetMethod().DeclaringType.Name;
		string _method_name = st.GetFrame(1).GetMethod().Name;

		_message = "WARNING - " + class_name + "." + _method_name + ": " + _message;
	#endif

		if (_context == null)
			Debug.LogWarning(_message);
		else
			Debug.LogWarning(_message, (UnityEngine.Object)_context);

	}
	/// <summary>
	/// print a 'error' into the 'console'
	/// </summary>
	[HideInCallstack]
	public static void printError(string _message, System.Object _context)
	{
	#if UNITY_EDITOR == true || UNITY_WEBGL == false//apparently webgl doesn't allow the code below
		var st = new System.Diagnostics.StackTrace();
		string class_name = st.GetFrame(1).GetMethod().DeclaringType.Name;
		string _method_name = st.GetFrame(1).GetMethod().Name;

		_message = "ERROR - " + class_name + "." + _method_name + ": " + _message;
	#endif

		if (_context == null)
			Debug.LogError(_message);
		else
			Debug.LogError(_message, (UnityEngine.Object)_context);

	}


	public static int randomInt(int _min,int _max)
	{
		return UnityEngine.Random.Range(_min, _max + 1);
	}

	public static List<int> randomInts(int _N,int _min,int _max)
    {
		List<int> _return = new List<int>();
		for(int i = 0; i < _N; i += 1)
        {
			_return.Add(randomInt(_min, _max));
        }
		return _return;
    }

	


	public static Vector3 snapToGrid(Vector3 _value,Vector3 _grid_size)
	{
		float snappedX = Mathf.Round(_value.x / _grid_size.x) * _grid_size.x;
		float snappedY = Mathf.Round(_value.y / _grid_size.y) * _grid_size.y;
		float snappedZ = Mathf.Round(_value.z / _grid_size.z) * _grid_size.z;
		return new Vector3(snappedX, snappedY, snappedZ);
	}

	public static Vector3 randomVector3(Vector3 _v1,Vector3 _v2)
	{
		float _x = UnityEngine.Random.Range(_v1.x, _v2.x);
		float _y = UnityEngine.Random.Range(_v1.y, _v2.y);
		float _z = UnityEngine.Random.Range(_v1.z, _v2.z);
		return new Vector3(_x, _y, _z);
	}

}

