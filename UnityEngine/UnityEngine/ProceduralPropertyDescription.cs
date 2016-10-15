using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class ProceduralPropertyDescription
	{
		public string name;

		public string label;

		public string group;

		public ProceduralPropertyType type;

		public bool hasRange;

		public float minimum;

		public float maximum;

		public float step;

		public string[] enumOptions;

		public string[] componentLabels;
	}
}
