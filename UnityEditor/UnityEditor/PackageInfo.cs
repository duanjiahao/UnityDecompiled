using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public struct PackageInfo
	{
		public string packagePath;

		public string jsonInfo;

		public string iconURL;

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern PackageInfo[] GetPackageList();
	}
}
