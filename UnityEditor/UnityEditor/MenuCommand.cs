using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEditor
{
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class MenuCommand
	{
		public UnityEngine.Object context;

		public int userData;

		public MenuCommand(UnityEngine.Object inContext, int inUserData)
		{
			this.context = inContext;
			this.userData = inUserData;
		}

		public MenuCommand(UnityEngine.Object inContext)
		{
			this.context = inContext;
			this.userData = 0;
		}
	}
}
