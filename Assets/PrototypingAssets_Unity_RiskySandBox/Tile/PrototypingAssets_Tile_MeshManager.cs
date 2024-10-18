using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public partial class PrototypingAssets_Tile_MeshManager : MonoBehaviour
{
    [SerializeField] bool debugging;
    [SerializeField] PrototypingAssets_Tile my_Tile;

    [SerializeField] Material dark_fow_Material;

    MeshFilter my_MeshFilter;
    MeshCollider my_MeshCollider;
    MeshRenderer my_MeshRenderer;


    private void Awake()
    {
        if (this.my_MeshFilter == null)
            my_MeshFilter = GetComponent<MeshFilter>();

        if (this.my_MeshCollider == null)
            my_MeshCollider = GetComponent<MeshCollider>();

        if (this.my_MeshRenderer == null)
            my_MeshRenderer = GetComponent<MeshRenderer>();

        PrototypingAssets_Tile.CACHE_GET_Tile_Collider[this.my_MeshCollider] = this.my_Tile;

        this.my_Tile.mesh_points_2D.OnUpdate += delegate
        {
            updateMesh();
            this.my_Tile.boxcast_Texture2D = calculateBoxCastTexture2D() ;
        };
        this.my_Tile.extrusion_height.OnUpdate += EventReceiver_OnVariableUpdate_extrusion_height;

        this.my_Tile.enable_dark_fow.OnUpdate_true += delegate { my_MeshRenderer.material = this.dark_fow_Material; };
    }

    Texture2D calculateBoxCastTexture2D()
    {
        int _texture_width = this.my_Tile.boxcast_Texture2D_width;
        int _texture_height = this.my_Tile.boxcast_Texture2D_height;


        Texture2D _generated_Texture = new Texture2D(_texture_width, _texture_height, TextureFormat.RGBA32, false);

        Vector3 _grid_min = this.my_MeshCollider.sharedMesh.bounds.min;
        _grid_min.y = 0;

        Vector3 _grid_max = this.my_MeshCollider.sharedMesh.bounds.max;
        _grid_max.y = 0;

        Vector3 _boxcast_size = new Vector3((_grid_max.x - _grid_min.x) / _texture_width, 1f, (_grid_max.z - _grid_min.z) / _texture_height);

        Vector3 _tile_position = this.my_Tile.transform.position;
        for (int x = 0; x < _texture_width; x += 1)
        {
            for (int z = 0; z < _texture_height; z += 1)
            {
                float _query_point_x = Mathf.Lerp(_grid_min.x, _grid_max.x, (float)x / _texture_width);
                float _query_point_z = Mathf.Lerp(_grid_min.z, _grid_max.z, (float)z / _texture_height);

                Vector3 _query_point = _tile_position + new Vector3(_query_point_x, 0f, _query_point_z);

                Collider[] _Colliders = Physics.OverlapBox(_query_point, _boxcast_size);

                if (this.debugging)
                    Debug.Log("_query = " + _query_point, this);

                if (_Colliders.Contains(this.my_MeshCollider))
                {
                    _generated_Texture.SetPixel(x, z, Color.white);
                }
                else
                {
                    _generated_Texture.SetPixel(x, z, new Color(0, 0, 0, 0));
                }
            }
        }
        _generated_Texture.Apply();
        return _generated_Texture;
    }

    void EventReceiver_OnVariableUpdate_extrusion_height(ObservableFloat _extrusion_height)
    {
        if (_extrusion_height.delta_value == 0)
            return;
        updateMesh();
    }


    Mesh updateMesh_2dCase()
    {
        List<Vector3> _points = new List<Vector3>(this.my_Tile.mesh_points_2D);
        Vector2[] _uvs;
        try
        {
            Mesh _new_Mesh = ShapeCreator.createMesh(this.my_Tile.mesh_points_2D, false);

            _uvs = new Vector2[_points.Count];

            if (_points.Count > 0)
            {
                Vector3 _min = _points[0];
                Vector3 _max = _points[0];

                for (int i = 1; i < _new_Mesh.vertices.Count(); i += 1)
                {
                    _min = Vector3.Min(_min, _new_Mesh.vertices[i]);
                    _max = Vector3.Max(_max, _new_Mesh.vertices[i]);
                }

                for (int i = 0; i < _new_Mesh.vertices.Count(); i += 1)
                {
                    float _ilerp_x = Mathf.InverseLerp(_min.x, _max.x, _new_Mesh.vertices[i].x);
                    float _ilerp_z = Mathf.InverseLerp(_min.z, _max.z, _new_Mesh.vertices[i].z);

                    _uvs[i] = new Vector2(_ilerp_x, _ilerp_z);
                }

                _new_Mesh.uv = _uvs;


            }

            return _new_Mesh;
        }

        catch
        {
            if (this.debugging)
                GlobalFunctions.print("an error occured... probably an invalid triangulation???", this);
            return null;
        }



    }

    Mesh updateMesh_3dCase()
    {
        List<Vector3> _points = new List<Vector3>(this.my_Tile.mesh_points_2D);
        Vector2[] _uvs;

        try
        {
            if (this.my_MeshFilter.mesh.vertices.Count() < 2 * _points.Count())
            {
                this.my_MeshFilter.mesh = ShapeCreator.createMesh(_points, true);

                Mesh _Mesh = this.my_MeshFilter.mesh;

                _uvs = new Vector2[2 * _points.Count];

                if (_points.Count > 0)
                {
                    Vector3 _min = _points[0];
                    Vector3 _max = _points[0];

                    for (int i = 1; i < _Mesh.vertices.Count() / 2; i += 1)
                    {
                        _min = Vector3.Min(_min, _Mesh.vertices[i]);
                        _max = Vector3.Max(_max, _Mesh.vertices[i]);
                    }

                    for (int i = 0; i < _Mesh.vertices.Count() / 2; i += 1)
                    {
                        float _ilerp_x = Mathf.InverseLerp(_min.x, _max.x, _Mesh.vertices[i].x);
                        float _ilerp_z = Mathf.InverseLerp(_min.z, _max.z, _Mesh.vertices[i].z);

                        _uvs[i] = new Vector2(_ilerp_x, _ilerp_z);
                        _uvs[i * 2] = new Vector2(_ilerp_x, _ilerp_z);
                    }

                    _Mesh.uv = _uvs;


                }
            }

            List<Vector3> _new_verts = new List<Vector3>(_points);
            _new_verts.AddRange(_new_verts);

            for (int _i = 0; _i < _new_verts.Count() / 2; _i += 1)
            {
                _new_verts[_i] = new Vector3(_new_verts[_i].x, this.my_Tile.extrusion_height.value, _new_verts[_i].z);
            }
            this.my_MeshFilter.mesh.vertices = _new_verts.ToArray();
            return this.my_MeshFilter.mesh;
        }

        catch
        {
            if (this.debugging)
                GlobalFunctions.print("an error occured... probably an invalid triangulation???", this);
            return null;
        }

    }

    void updateMesh()
    {
        Mesh _new_Mesh;
        if (this.my_Tile.extrusion_height.value == 0)
        {
            _new_Mesh = updateMesh_2dCase();
        }
        else
        {
            _new_Mesh = updateMesh_3dCase();
        }


        this.my_MeshFilter.mesh = _new_Mesh;

        if (this.my_MeshCollider != null)
            GetComponent<MeshCollider>().sharedMesh = _new_Mesh;

    }


}
