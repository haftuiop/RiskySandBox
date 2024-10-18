using System.Collections;using System.Collections.Generic;using System.Linq;using System;

public partial class PrototypingAssets_Tile
{
    public static int null_ID { get { return -1; } }

    public static Dictionary<int, PrototypingAssets_Tile> CACHE_GET_Tile = new Dictionary<int, PrototypingAssets_Tile>();

    public static PrototypingAssets_Tile GET_Tile(int _ID)
    {
        foreach(PrototypingAssets_Tile _slow_Tile in all_instances)
        {
            if (_slow_Tile.ID.value == _ID)
                return _slow_Tile;
        }
        return null;
        PrototypingAssets_Tile _Tile;
        CACHE_GET_Tile.TryGetValue(_ID, out _Tile);
        return _Tile;
    }

    public static ObservableList<PrototypingAssets_Tile> all_instances = new ObservableList<PrototypingAssets_Tile>();

    public enum tile_shapes
    {
        square,
        hex_flat,
        hex_pointy,
        custom
    }


    public ObservableVector3List mesh_points_2D { get { return PRIVATE_mesh_points_2D; } }
    public ObservableFloat extrusion_height { get { return this.PRIVATE_extrusion_height; } }

    public ObservableBool enable_dark_fow { get { return this.PRIVATE_enable_dark_fow; } }

    public ObservableInt ID { get { return this.PRIVATE_ID; } }

    public ObservableString tile_name { get { return this.PRIVATE_tile_name; } }

	

}
