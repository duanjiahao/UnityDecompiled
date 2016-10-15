using System;
using UnityEditor.BuildReporting;

namespace UnityEditor.Modules
{
	internal struct BuildPostProcessArgs
	{
		public BuildTarget target;

		public string stagingArea;

		public string stagingAreaData;

		public string stagingAreaDataManaged;

		public string playerPackage;

		public string installPath;

		public string companyName;

		public string productName;

		public Guid productGUID;

		public BuildOptions options;

		public BuildReport report;

		internal RuntimeClassRegistry usedClassRegistry;
	}
}
