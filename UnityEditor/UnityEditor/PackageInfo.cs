using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public struct PackageInfo
	{
		public string packagePath;

		public string jsonInfo;

		public string iconURL;

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern PackageInfo[] GetPackageList();
	}
}
