using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sebastian.Geometry;
using System.Linq;


[RequireComponent(typeof(MeshFilter))]

public class ShapeCreator : MonoBehaviour
{
    public MeshFilter my_MeshFilter { get { return GetComponent<MeshFilter>(); } }
    
    public List<Sebastian.Geometry.Shape> shapes = new List<Sebastian.Geometry.Shape>();




    public void UpdateMeshDisplay()
    {
        CompositeShape compShape = new CompositeShape(shapes);
        my_MeshFilter.mesh = compShape.GetMesh();
    }



    public static Mesh createMesh(IEnumerable<Vector3> _verts,bool _3d = false)
    {

        List<Shape> _shapes = new List<Shape>();

        _shapes.Add(new Sebastian.Geometry.Shape());


        _shapes[0].points = new List<Vector3>(_verts);

        CompositeShape _CompositeShape = new CompositeShape(_shapes);


        Mesh _Mesh = _CompositeShape.GetMesh();

        if (_3d == false)
            return _Mesh;


        //handle 3d case...

        
        List<Vector3> _points = new List<Vector3>(_Mesh.vertices);
        List<int> _triangles = new List<int>(_Mesh.triangles);



        int _n_points = _points.Count;


        List<int> _bottom_face_triangles = new List<int>(_Mesh.triangles.Select(x => x + _n_points)).ToList();

        _bottom_face_triangles.Reverse();



        _points.AddRange(_points);

        //ok now we must stich together the sides...
        for(int i = 0; i < _n_points - 1; i += 1)
        {
            //triangles
            _triangles.Add(i);
            _triangles.Add(i + 1);
            _triangles.Add(i + _n_points);

            _triangles.Add(i + 1);
            _triangles.Add(i + _n_points + 1);
            _triangles.Add(i + _n_points);
        }

        _triangles.Add(_n_points - 1);
        _triangles.Add(0);
        _triangles.Add(_n_points * 2 - 1);

        _triangles.Add(0);
        _triangles.Add(_n_points);
        _triangles.Add(_n_points * 2 - 1);


        _triangles.AddRange(_bottom_face_triangles);




        _Mesh.vertices = _points.ToArray();
        _Mesh.triangles = _triangles.ToArray();
        _Mesh.RecalculateNormals();

        List<Vector3> _normals = new List<Vector3>();

        for (int i = 0; i < _n_points; i += 1)
        {
            _normals.Add(new Vector3(0, 1, 0));
        }

        for (int i = 0; i < _n_points; i += 1)
        {
            _normals.Add(new Vector3(0, 0, -1));
        }

        _Mesh.normals = _normals.ToArray();





        

        return _Mesh;

    }



}
