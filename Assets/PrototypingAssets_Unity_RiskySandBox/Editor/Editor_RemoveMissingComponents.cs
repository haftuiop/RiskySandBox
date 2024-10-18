#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Editor_RemoveMissingComponents : Editor
{
	[MenuItem("PrototypingAssets/RemoveMissingComponents")]	
	public static void Remove()
	{
		var objs = Resources.FindObjectsOfTypeAll<GameObject>();
		foreach(GameObject _prefab in  objs) 
		{
			removeFromGameObject(_prefab);
		}
		//TODO
		//foreach(scene in project)
		//		foreach(scene_gameobject in scene.gameObjects)
		//			removeFromGameObject(scene_gameObject)
		Debug.Log($"Removed missing scripts");
	}


	static void removeFromGameObject(GameObject _go)
	{
		GameObjectUtility.RemoveMonoBehavioursWithMissingScript(_go);
		foreach(Transform _child in _go.transform)
		{
			
			removeFromGameObject(_child.gameObject);
		}
	}
}
#endif