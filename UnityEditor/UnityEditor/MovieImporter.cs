using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	public sealed class MovieImporter : AssetImporter
	{
		public extern float quality
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool linearTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float duration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
