using System;
using System.Runtime.CompilerServices;

namespace UnityEditorInternal
{
	internal static class WebURLs
	{
		public static extern string unity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string unityConnect
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string unityForum
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string unityAnswers
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string unityFeedback
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string whatsNewPage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string betaLandingPage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string cloudBuildPage
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
