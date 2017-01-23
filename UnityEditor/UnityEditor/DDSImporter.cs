using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	public sealed class DDSImporter : AssetImporter
	{
		public extern bool isReadable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
