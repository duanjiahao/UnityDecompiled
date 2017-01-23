using System;
using System.Runtime.CompilerServices;

namespace UnityEditor
{
	public sealed class SketchUpImporter : ModelImporter
	{
		public extern double latitude
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern double longitude
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern double northCorrection
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern SketchUpImportScene[] GetScenes();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern SketchUpImportCamera GetDefaultCamera();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern SketchUpNodeInfo[] GetNodes();
	}
}
