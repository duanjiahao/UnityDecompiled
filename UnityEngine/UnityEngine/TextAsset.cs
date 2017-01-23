using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class TextAsset : Object
	{
		public extern string text
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern byte[] bytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public override string ToString()
		{
			return this.text;
		}
	}
}
