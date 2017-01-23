using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	public sealed class HumanTemplate : UnityEngine.Object
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern HumanTemplate();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Insert(string name, string templateName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string Find(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearTemplate();
	}
}
