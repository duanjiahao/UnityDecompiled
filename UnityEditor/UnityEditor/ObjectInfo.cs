using System;
using System.Collections.Generic;

namespace UnityEditor
{
	internal class ObjectInfo
	{
		public int instanceId;

		public long memorySize;

		public int reason;

		public List<ObjectInfo> referencedBy;

		public string name;

		public string className;
	}
}
