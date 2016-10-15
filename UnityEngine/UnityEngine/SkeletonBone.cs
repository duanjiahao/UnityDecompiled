using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	public struct SkeletonBone
	{
		public string name;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;

		public int transformModified;
	}
}
