using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class PrototypingAssets_Tile : MonoBehaviour
{
    public static Dictionary<Collider, PrototypingAssets_Tile> CACHE_GET_Tile_Collider = new Dictionary<Collider, PrototypingAssets_Tile>();

    public static PrototypingAssets_Tile GET_Tile(Collider _Collider)
    {
        PrototypingAssets_Tile _Tile;
        CACHE_GET_Tile_Collider.TryGetValue(_Collider, out _Tile);
        return _Tile;
    }

    /// <summary>
    /// invoked whenever a Tile changes its "boxcast_Texture2D"
    /// </summary>
    public static event Action<PrototypingAssets_Tile> OnVariableUpdate_boxcast_Texture2D_STATIC;


    [SerializeField] protected bool debugging;

    [SerializeField] protected ObservableInt PRIVATE_ID;
    [SerializeField] protected ObservableString PRIVATE_tile_name;

    [SerializeField] protected ObservableVector3List PRIVATE_mesh_points_2D;
    [SerializeField] protected ObservableFloat PRIVATE_extrusion_height;

    [SerializeField] protected ObservableBool PRIVATE_enable_dark_fow;

    public tile_shapes tile_shape;

    /// <summary>
    /// this shows a little "preview" of the shape of the tile...
    /// </summary>
    public Texture2D boxcast_Texture2D
    {
        get { return this.PRIVATE_boxcast_Texture2D; }
        set
        {
            this.PRIVATE_boxcast_Texture2D = value;
            PrototypingAssets_Tile.OnVariableUpdate_boxcast_Texture2D_STATIC?.Invoke(this);

        }
    }
    [SerializeField] Texture2D PRIVATE_boxcast_Texture2D;

    public int boxcast_Texture2D_width = 100;
    public int boxcast_Texture2D_height = 100;


    protected virtual void Awake()
    {
        all_instances.Add(this);
    }

    protected virtual void OnDestroy()
    {
        all_instances.Remove(this);
    }

    protected virtual void Start()
    {
        if (this.tile_shape == PrototypingAssets_Tile.tile_shapes.square)
        {
            // Hard-code points for square (4 points for a square) in 3D (x, y, z)
            Vector3[] _square_points = new Vector3[4]
            {
                new Vector3(-0.5f, 0f, -0.5f),  // Bottom-left
                new Vector3(0.5f, 0f, -0.5f),   // Bottom-right
                new Vector3(0.5f, 0f, 0.5f),    // Top-right
                new Vector3(-0.5f, 0f, 0.5f)    // Top-left
            };

            this.mesh_points_2D.SET_items(_square_points);
        }
        else if (this.tile_shape == PrototypingAssets_Tile.tile_shapes.hex_flat)
        {
            // Hard-code points for flat-top hexagon (6 points) in 3D (x, y, z)
            Vector3[] _hex_flat_points = new Vector3[6]
            {
                new Vector3(0.5f, 0f, 0f),         // Right
                new Vector3(0.25f, 0f, 0.433f),    // Top-right
                new Vector3(-0.25f, 0f, 0.433f),   // Top-left
                new Vector3(-0.5f, 0f, 0f),        // Left
                new Vector3(-0.25f, 0f, -0.433f),  // Bottom-left
                new Vector3(0.25f, 0f, -0.433f)    // Bottom-right
            };

            this.mesh_points_2D.SET_items(_hex_flat_points);
        }
        else if (this.tile_shape == PrototypingAssets_Tile.tile_shapes.hex_pointy)
        {
            // Hard-code points for pointy-top hexagon (6 points) in 3D (x, y, z)
            Vector3[] _hex_pointy_points = new Vector3[6]
            {
                new Vector3(0f, 0f, 0.5f),        // Top
                new Vector3(0.433f, 0f, 0.25f),   // Top-right
                new Vector3(0.433f, 0f, -0.25f),  // Bottom-right
                new Vector3(0f, 0f, -0.5f),       // Bottom
                new Vector3(-0.433f, 0f, -0.25f), // Bottom-left
                new Vector3(-0.433f, 0f, 0.25f)   // Top-left
            };

            this.mesh_points_2D.SET_items(_hex_pointy_points);
        }
    }

}
