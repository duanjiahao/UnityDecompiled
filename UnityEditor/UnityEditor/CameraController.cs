using System;
using UnityEngine;

namespace UnityEditor
{
	internal abstract class CameraController
	{
		public abstract void Update(CameraState cameraState, Camera cam);
	}
}
