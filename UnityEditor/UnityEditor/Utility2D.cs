using System;
using UnityEngine;

namespace UnityEditor
{
	internal class Utility2D
	{
		public static Vector3 ScreenToLocal(Transform transform, Vector2 screenPosition)
		{
			Plane plane = new Plane(transform.forward * -1f, transform.position);
			Ray ray;
			if (Camera.current.orthographic)
			{
				Vector2 v = GUIClip.Unclip(screenPosition);
				v.y = (float)Screen.height - v.y;
				Vector3 origin = Camera.current.ScreenToWorldPoint(v);
				ray = new Ray(origin, Camera.current.transform.forward);
			}
			else
			{
				ray = HandleUtility.GUIPointToWorldRay(screenPosition);
			}
			float distance;
			plane.Raycast(ray, out distance);
			Vector3 point = ray.GetPoint(distance);
			return transform.InverseTransformPoint(point);
		}
	}
}
