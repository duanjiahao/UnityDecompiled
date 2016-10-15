using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct TreeInstance
	{
		public Vector3 position;

		public float widthScale;

		public float heightScale;

		public float rotation;

		public Color32 color;

		public Color32 lightmapColor;

		public int prototypeIndex;

		internal float temporaryDistance;
	}
}
