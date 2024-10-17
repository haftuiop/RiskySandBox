using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class RiskySandBox_WaterBackgroundGeneration : MonoBehaviour
{
    [SerializeField] bool debugging;


    [SerializeField] ObservableInt horizontal_resolution;
    [SerializeField] ObservableInt vertical_resolution;

    [SerializeField] ObservableVector3 bottom_left;
    [SerializeField] ObservableVector3 top_right;

    Texture2D background_Texture2D;

    [SerializeField] UnityEngine.UI.RawImage background_Image;

    [SerializeField] ObservableInt sea_width;
    [SerializeField] ObservableInt sea_foam_width;


    private void Awake()
    {
        this.bottom_left.OnUpdate += delegate { regenerate(); };
        this.top_right.OnUpdate += delegate { regenerate(); };
        this.sea_width.OnUpdate += delegate { regenerate(); };
        this.sea_foam_width.OnUpdate += delegate { regenerate(); };
        this.horizontal_resolution.OnUpdate += delegate { regenerate(); };
        this.vertical_resolution.OnUpdate += delegate { regenerate(); };
    }

    bool[,] createLandGrid()
    {
        bool[,] boolArray = new bool[horizontal_resolution, vertical_resolution];

        // Optionally, initialize the array elements
        for (int x = 0; x < horizontal_resolution; x++)
        {
            Vector3 _query_point = new Vector3(0, 0, 0);

            _query_point.x = this.bottom_left.x + x * (top_right.x - bottom_left.x) / (float)horizontal_resolution;

            for (int z = 0; z < vertical_resolution; z++)
            {
                _query_point.z = this.bottom_left.z + z * (top_right.z - bottom_left.z) / (float)vertical_resolution;


                Collider[] _objects = Physics.OverlapBox(_query_point, new Vector3(((top_right.x - bottom_left.x) / horizontal_resolution / 2f), 1f, ((top_right.z - bottom_left.z) / vertical_resolution) / 2f));


                boolArray[x, z] = _objects.Count() > 0; // Set default value
            }
        }
        return boolArray;
    }

    void regenerate()
    {


        bool[,] _land_array = createLandGrid();
        bool[,] _sea_foam_array = CreateProximityArray(_land_array, this.sea_foam_width);
        bool[,] _sea_array = CreateProximityArray(_land_array, this.sea_width);



        background_Texture2D = new Texture2D(horizontal_resolution, vertical_resolution, TextureFormat.RGBA32, false);
        background_Texture2D.filterMode = FilterMode.Point;

        for (int x = 0; x < _land_array.GetLength(0); x += 1)
        {

            
            for(int z = 0; z < _land_array.GetLength(1); z += 1)
            {
                Color _pixel_Color = new Color(0, 0, 0, 0);


                if (_land_array[x, z] == true)
                    _pixel_Color = new Color(0, 0, 0, 0);
                
                else if (_sea_foam_array[x, z] == true)
                    _pixel_Color = Color.white;

                else if (_sea_array[x, z] == true)
                    _pixel_Color = Color.cyan;

                else
                    _pixel_Color = Color.blue;
                





                background_Texture2D.SetPixel(x, z, _pixel_Color);


            }
        }




        background_Texture2D.Apply();
        background_Image.texture = background_Texture2D;


    }


    bool[,] CreateProximityArray(bool[,] inputArray, int n)
    {
        int rows = inputArray.GetLength(0);
        int cols = inputArray.GetLength(1);
        bool[,] resultArray = new bool[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                resultArray[i, j] = IsTrueWithinDistance(inputArray, i, j, n);
            }
        }

        return resultArray;
    }

    bool IsTrueWithinDistance(bool[,] array, int row, int col, int n)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);

        for (int i = Mathf.Max(0, row - n); i <= Mathf.Min(rows - 1, row + n); i++)
        {
            for (int j = Mathf.Max(0, col - n); j <= Mathf.Min(cols - 1, col + n); j++)
            {
                if (array[i, j])
                {
                    return true;
                }
            }
        }

        return false;
    }

}
