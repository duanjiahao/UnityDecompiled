using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class ParticleEffectUtils
	{
		private static List<GameObject> s_Planes = new List<GameObject>();

		public static GameObject GetPlane(int index)
		{
			while (ParticleEffectUtils.s_Planes.Count <= index)
			{
				GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				ParticleEffectUtils.s_Planes.Add(gameObject);
			}
			return ParticleEffectUtils.s_Planes[index];
		}

		public static void HidePlaneIfExists(int index)
		{
			if (index < ParticleEffectUtils.s_Planes.Count)
			{
				ParticleEffectUtils.s_Planes[index].transform.localScale = Vector3.zero;
			}
		}

		public static void ClearPlanes()
		{
			if (ParticleEffectUtils.s_Planes.Count > 0)
			{
				foreach (GameObject current in ParticleEffectUtils.s_Planes)
				{
					UnityEngine.Object.DestroyImmediate(current);
				}
				ParticleEffectUtils.s_Planes.Clear();
			}
		}
	}
}
