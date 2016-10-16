using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(BuoyancyEffector2D), true)]
	internal class BuoyancyEffector2DEditor : Effector2DEditor
	{
		public void OnSceneGUI()
		{
			BuoyancyEffector2D buoyancyEffector2D = (BuoyancyEffector2D)this.target;
			if (!buoyancyEffector2D.enabled)
			{
				return;
			}
			float y = buoyancyEffector2D.transform.position.y + buoyancyEffector2D.transform.lossyScale.y * buoyancyEffector2D.surfaceLevel;
			List<Vector3> list = new List<Vector3>();
			float num = float.NegativeInfinity;
			float num2 = num;
			foreach (Collider2D current in from c in buoyancyEffector2D.gameObject.GetComponents<Collider2D>()
			where c.enabled && c.usedByEffector
			select c)
			{
				Bounds bounds = current.bounds;
				float x = bounds.min.x;
				float x2 = bounds.max.x;
				if (float.IsNegativeInfinity(num))
				{
					num = x;
					num2 = x2;
				}
				else
				{
					if (x < num)
					{
						num = x;
					}
					if (x2 > num2)
					{
						num2 = x2;
					}
				}
				Vector3 item = new Vector3(x, y, 0f);
				Vector3 item2 = new Vector3(x2, y, 0f);
				list.Add(item);
				list.Add(item2);
			}
			Handles.color = Color.red;
			Handles.DrawAAPolyLine(new Vector3[]
			{
				new Vector3(num, y, 0f),
				new Vector3(num2, y, 0f)
			});
			Handles.color = Color.cyan;
			for (int i = 0; i < list.Count - 1; i += 2)
			{
				Handles.DrawAAPolyLine(new Vector3[]
				{
					list[i],
					list[i + 1]
				});
			}
		}
	}
}
