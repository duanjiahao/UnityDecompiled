using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	public struct SkeletonBone
	{
		public string name;

		internal string parentName;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;

		[Obsolete("transformModified is no longer used and has been deprecated.", true)]
		public int transformModified
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
	}
}
