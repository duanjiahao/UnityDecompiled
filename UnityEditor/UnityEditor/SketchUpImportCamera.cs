using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	public struct SketchUpImportCamera
	{
		public Vector3 position;

		public Vector3 lookAt;

		public Vector3 up;

		public float fieldOfView;

		public float aspectRatio;

		public float orthoSize;

		public bool isPerspective;
	}
}
