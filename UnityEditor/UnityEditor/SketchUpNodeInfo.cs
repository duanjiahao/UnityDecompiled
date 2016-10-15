using System;

namespace UnityEditor
{
	[Serializable]
	internal struct SketchUpNodeInfo
	{
		public string name;

		public int parent;

		public bool enabled;

		public int nodeIndex;
	}
}
