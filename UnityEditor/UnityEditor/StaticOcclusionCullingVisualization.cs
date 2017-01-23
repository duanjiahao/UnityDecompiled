using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class StaticOcclusionCullingVisualization
	{
		public static extern bool showOcclusionCulling
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showPreVisualization
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showViewVolumes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showDynamicObjectBounds
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showPortals
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showVisibilityLines
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool showGeometryCulling
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool isPreviewOcclusionCullingCameraInPVS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern Camera previewOcclusionCamera
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern Camera previewOcclucionCamera
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
