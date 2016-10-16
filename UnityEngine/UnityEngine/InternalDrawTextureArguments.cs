using System;

namespace UnityEngine
{
	internal struct InternalDrawTextureArguments
	{
		public Rect screenRect;

		public Texture texture;

		public Rect sourceRect;

		public int leftBorder;

		public int rightBorder;

		public int topBorder;

		public int bottomBorder;

		public Color32 color;

		public Material mat;
	}
}
