using System;
using System.Reflection;

namespace UnityEngine.Events
{
	internal class CachedInvokableCall<T> : InvokableCall<T>
	{
		private readonly object[] m_Arg1 = new object[1];

		public CachedInvokableCall(UnityEngine.Object target, MethodInfo theFunction, T argument) : base(target, theFunction)
		{
			this.m_Arg1[0] = argument;
		}

		public override void Invoke(object[] args)
		{
			base.Invoke(this.m_Arg1);
		}
	}
}
