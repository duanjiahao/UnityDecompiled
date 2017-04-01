using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	public struct ShaderPassName
	{
		private int nameIndex;

		public ShaderPassName(string name)
		{
			this.nameIndex = ShaderPassName.Init(name);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Init(string name);
	}
}
