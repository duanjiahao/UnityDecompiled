using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ClothInspectorState : ScriptableSingleton<ClothInspectorState>
	{
		[SerializeField]
		public ClothInspector.DrawMode DrawMode = ClothInspector.DrawMode.MaxDistance;

		[SerializeField]
		public bool ManipulateBackfaces;

		[SerializeField]
		public bool PaintMaxDistanceEnabled = true;

		[SerializeField]
		public bool PaintCollisionSphereDistanceEnabled;

		[SerializeField]
		public float PaintMaxDistance = 0.2f;

		[SerializeField]
		public float PaintCollisionSphereDistance;

		[SerializeField]
		public ClothInspector.ToolMode ToolMode;
	}
}
