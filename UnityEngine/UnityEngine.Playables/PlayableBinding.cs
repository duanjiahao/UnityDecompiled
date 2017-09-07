using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Playables
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct PlayableBinding
	{
		public static readonly PlayableBinding[] None = new PlayableBinding[0];

		public static readonly double DefaultDuration = double.PositiveInfinity;

		public string streamName
		{
			get;
			set;
		}

		public DataStreamType streamType
		{
			get;
			set;
		}

		public UnityEngine.Object sourceObject
		{
			get;
			set;
		}

		public Type sourceBindingType
		{
			get;
			set;
		}
	}
}
