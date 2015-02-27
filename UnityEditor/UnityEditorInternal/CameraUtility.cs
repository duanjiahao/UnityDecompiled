using System;
using UnityEngine;
namespace UnityEditorInternal
{
	internal sealed class CameraUtility
	{
		public static bool DoesAnyCameraUseDeferred()
		{
			bool result = false;
			Camera[] allCameras = Camera.allCameras;
			for (int i = 0; i < allCameras.Length; i++)
			{
				if (allCameras[i].actualRenderingPath == RenderingPath.DeferredLighting)
				{
					result = true;
				}
			}
			return result;
		}
	}
}
