using System.Collections;using System.Collections.Generic;using System.Linq;using System;
using UnityEngine;

public partial class GlobalFunctions
{
	static void SortRaycastHitsByDistance(RaycastHit[] hits)
	{
		//chatgpt magic...
		int n = hits.Length;
		for (int i = 1; i < n; ++i)
		{
			RaycastHit key = hits[i];
			int j = i - 1;

			// Move elements that are greater than the key to one position ahead of their current position
			while (j >= 0 && hits[j].distance > key.distance)
			{
				hits[j + 1] = hits[j];
				j = j - 1;
			}
			hits[j + 1] = key;
		}
	}
	public static RaycastHit[] RaycastAll_ordered(Vector3 _origin, Vector3 _direction, float _max_distance = Mathf.Infinity, int _layermask = Physics.AllLayers, bool _include_triggers = false)
	{
		RaycastHit[] _hits;
		if (_include_triggers)
			_hits = Physics.RaycastAll(_origin, _direction, _max_distance, _layermask, QueryTriggerInteraction.Collide);
		else
			_hits = Physics.RaycastAll(_origin, _direction, _max_distance, _layermask, QueryTriggerInteraction.Ignore);

		//Array.Sort(_hits, (x, y) => x.distance.CompareTo(y.distance));
		//Array.Sort(_hits, new RaycastHitComparer());
		SortRaycastHitsByDistance(_hits);//use this directly since it cuts out much of the random functions like 'sorterhelper' whatever that means...
		return _hits;
	}
	public static RaycastHit[] RaycastAll_ordered(Ray _ray, float _max_distance, int _layermask = Physics.AllLayers, bool _include_triggers = false)
	{
		RaycastHit[] _hits;
		if (_include_triggers)
			_hits = Physics.RaycastAll(_ray.origin, _ray.direction, _max_distance, _layermask, QueryTriggerInteraction.Collide);
		else
			_hits = Physics.RaycastAll(_ray.origin, _ray.direction, _max_distance, _layermask, QueryTriggerInteraction.Ignore);
		SortRaycastHitsByDistance(_hits);//use this directly since it cuts out much of the random functions like 'sorterhelper' whatever that means...
		return _hits;
	}
}

