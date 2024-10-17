using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class Test : MonoBehaviour
{
    [SerializeField] bool debugging;

    [SerializeField] List<int> test_list = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        IEnumerable<IEnumerable<int>> _combos = GlobalFunctions.GetCombinations(test_list, 3);
        
        foreach(IEnumerable<int> _combo in _combos)
        {
            GlobalFunctions.print(string.Join(", ", _combo),this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
