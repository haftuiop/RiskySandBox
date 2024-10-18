using System.Collections;using System.Collections.Generic;using System.Linq;using System;

public partial class GlobalFunctions
{

    public static IEnumerable<IEnumerable<T>> GetCombinations<T>(IEnumerable<T> list, int length)
    {
        if (length == 0)
        {
            // Return a list with an empty enumerable, representing one combination of zero elements
            return new List<IEnumerable<T>> { new List<T>() };
        }

        return list.SelectMany((item, index) =>
            GetCombinations(list.Skip(index + 1), length - 1)
            .Select(subCombination => new[] { item }.Concat(subCombination)));
    }
}

