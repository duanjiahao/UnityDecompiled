using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ClothInspectorState : ScriptableSingleton<ClothInspectorState>
	{
		[SerializeField]
		public ClothInspector.DrawMode DrawMode = ClothInspector.DrawMode.MaxDistance;

		[SerializeField]
		public bool ManipulateBackfaces = false;

		[SerializeField]
		public bool PaintMaxDistanceEnabled = true;

		[SerializeField]
		public bool PaintCollisionSphereDistanceEnabled = false;

		[SerializeField]
		public float PaintMaxDistance = 0.2f;

		[SerializeField]
		public float PaintCollisionSphereDistance = 0f;

		[SerializeField]
		public ClothInspector.ToolMode ToolMode = ClothInspector.ToolMode.Select;
	}
}
