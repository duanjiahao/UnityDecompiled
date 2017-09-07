using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class RenderData
	{
		public RenderTexture pixelCache;

		public Matrix4x4 worldTransForm = Matrix4x4.identity;

		public Rect lastLayout;
	}
}
