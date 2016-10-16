using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class TextAsset : Object
	{
		public extern string text
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern byte[] bytes
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public override string ToString()
		{
			return this.text;
		}
	}
}
