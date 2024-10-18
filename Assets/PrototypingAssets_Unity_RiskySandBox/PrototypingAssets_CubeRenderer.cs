using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PrototypingAssets_CubeRenderer : MonoBehaviour
{
	// Variables to hold cube positions and mesh data
	[SerializeField] private Vector3[] cube_positions;//the centre positions of each of the cubes
	private MeshFilter my_MeshFilter;
	private MeshRenderer my_MeshRenderer;
	private Mesh my_Mesh;

	[SerializeField] Mesh unity_cube_Mesh;

	private bool is_dirty;//does the mesh need to be redrawn...

	void Start()
	{
		// Initialize cube positions and setup mesh
		//cube_positions = new Vector3[] { new Vector3(0, 0, 0),new Vector3(0,0,5) };
		cube_positions = new Vector3[0];

		my_MeshFilter = gameObject.GetComponent<MeshFilter>();
		my_MeshRenderer = gameObject.GetComponent<MeshRenderer>();

		my_Mesh = new Mesh();
		my_Mesh.name = "PrototypingAssets_CubeRenderer Mesh";

		my_MeshFilter.sharedMesh = my_Mesh;



		// Call function to update mesh
		redrawMesh();
	}




	public void createCube(Vector3 _position)
	{
		cube_positions = cube_positions.Append(_position).ToArray();

		this.is_dirty = true;
	}

	public void destroyCube(Vector3 position)
	{
		//TODO implement this...
		this.is_dirty = false;
	}

	private void Update()
	{
		if (this.is_dirty == false)
			return;
		redrawMesh();
	}


	void redrawMesh()
	{
		this.is_dirty = false;
		//remove all data about the mesh
		my_Mesh.Clear();
		Vector3[] _new_verts = new Vector3[24 * this.cube_positions.Length];
		int[] _new_triangles = new int[36 * this.cube_positions.Length];
		Vector3[] _new_normals = new Vector3[24 * this.cube_positions.Length];
		
		//go through all the positions...
		for(int _i = 0; _i < this.cube_positions.Length; _i += 1)
		{
			Vector3[] _verticies = GET_vertices(this.cube_positions[_i], 1f);//get the vertices for this cube_position
			_verticies.CopyTo(_new_verts, _i * _verticies.Length);

			
			int[] _triangles = GET_triangles(_i * _verticies.Length);
			_triangles.CopyTo(_new_triangles, _i * _triangles.Length);

			Vector3[] _normals = GET_normals();
			_normals.CopyTo(_new_normals, _i * _verticies.Length);



		}

		my_Mesh.vertices = _new_verts;
		my_Mesh.triangles = _new_triangles;
		my_Mesh.normals = _new_normals;
		my_Mesh.RecalculateBounds();

	}

	Vector3[] GET_normals()
	{
		//TODO -actually calclualte
		return unity_cube_Mesh.normals;

	}

	int[] GET_triangles(int _start_index)
	{
		//todo actually calculate these values instead of stealing them from Unity's inbluilt cube mesh
		int[] _to_fill = new int[36];

		for(int i = 0; i < unity_cube_Mesh.triangles.Length; i += 1)
		{
			_to_fill[i] = _start_index + unity_cube_Mesh.triangles[i];
		}

		return _to_fill;


	}

	Vector3[] GET_vertices(Vector3 _centre, float _width)
	{
		

		//todo - actually calculate these values here instead of stealing them from the Unity's inbuilt cube mesh
		Vector3[] _to_fill = new Vector3[24];
		for(int i = 0; i < unity_cube_Mesh.vertices.Length; i += 1)
		{
			_to_fill[i] = _centre + _width * unity_cube_Mesh.vertices[i];
		}

		return _to_fill;

	}

	

}
