using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	public struct PackageInfo
	{
		public string packagePath;

		public string jsonInfo;

		public string iconURL;

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern PackageInfo[] GetPackageList();
	}
}
