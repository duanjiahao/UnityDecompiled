using System;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[Serializable]
	internal class SceneViewGrid
	{
		private static PrefColor kViewGridColor = new PrefColor("Scene/Grid", 0.5f, 0.5f, 0.5f, 0.4f);

		[SerializeField]
		private AnimBool xGrid = new AnimBool();

		[SerializeField]
		private AnimBool yGrid = new AnimBool();

		[SerializeField]
		private AnimBool zGrid = new AnimBool();

		public void Register(SceneView source)
		{
			this.xGrid.valueChanged.AddListener(new UnityAction(source.Repaint));
			this.yGrid.valueChanged.AddListener(new UnityAction(source.Repaint));
			this.zGrid.valueChanged.AddListener(new UnityAction(source.Repaint));
		}

		public DrawGridParameters PrepareGridRender(Camera camera, Vector3 pivot, Quaternion rotation, float size, bool orthoMode, bool gridVisible)
		{
			bool target = false;
			bool target2 = false;
			bool target3 = false;
			if (gridVisible)
			{
				if (orthoMode)
				{
					Vector3 lhs = rotation * Vector3.forward;
					if (Mathf.Abs(lhs.y) > 0.2f)
					{
						target2 = true;
					}
					else if (lhs == Vector3.left || lhs == Vector3.right)
					{
						target = true;
					}
					else if (lhs == Vector3.forward || lhs == Vector3.back)
					{
						target3 = true;
					}
				}
				else
				{
					target2 = true;
				}
			}
			this.xGrid.target = target;
			this.yGrid.target = target2;
			this.zGrid.target = target3;
			DrawGridParameters result;
			result.pivot = pivot;
			result.color = SceneViewGrid.kViewGridColor;
			result.size = size;
			result.alphaX = this.xGrid.faded;
			result.alphaY = this.yGrid.faded;
			result.alphaZ = this.zGrid.faded;
			return result;
		}
	}
}
