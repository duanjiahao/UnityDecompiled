using System;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityEditor
{
	public static class AssemblyReloadEvents
	{
		public delegate void AssemblyReloadCallback();

		public static event AssemblyReloadEvents.AssemblyReloadCallback beforeAssemblyReload
		{
			add
			{
				AssemblyReloadEvents.AssemblyReloadCallback assemblyReloadCallback = AssemblyReloadEvents.beforeAssemblyReload;
				AssemblyReloadEvents.AssemblyReloadCallback assemblyReloadCallback2;
				do
				{
					assemblyReloadCallback2 = assemblyReloadCallback;
					assemblyReloadCallback = Interlocked.CompareExchange<AssemblyReloadEvents.AssemblyReloadCallback>(ref AssemblyReloadEvents.beforeAssemblyReload, (AssemblyReloadEvents.AssemblyReloadCallback)Delegate.Combine(assemblyReloadCallback2, value), assemblyReloadCallback);
				}
				while (assemblyReloadCallback != assemblyReloadCallback2);
			}
			remove
			{
				AssemblyReloadEvents.AssemblyReloadCallback assemblyReloadCallback = AssemblyReloadEvents.beforeAssemblyReload;
				AssemblyReloadEvents.AssemblyReloadCallback assemblyReloadCallback2;
				do
				{
					assemblyReloadCallback2 = assemblyReloadCallback;
					assemblyReloadCallback = Interlocked.CompareExchange<AssemblyReloadEvents.AssemblyReloadCallback>(ref AssemblyReloadEvents.beforeAssemblyReload, (AssemblyReloadEvents.AssemblyReloadCallback)Delegate.Remove(assemblyReloadCallback2, value), assemblyReloadCallback);
				}
				while (assemblyReloadCallback != assemblyReloadCallback2);
			}
		}

		public static event AssemblyReloadEvents.AssemblyReloadCallback afterAssemblyReload
		{
			add
			{
				AssemblyReloadEvents.AssemblyReloadCallback assemblyReloadCallback = AssemblyReloadEvents.afterAssemblyReload;
				AssemblyReloadEvents.AssemblyReloadCallback assemblyReloadCallback2;
				do
				{
					assemblyReloadCallback2 = assemblyReloadCallback;
					assemblyReloadCallback = Interlocked.CompareExchange<AssemblyReloadEvents.AssemblyReloadCallback>(ref AssemblyReloadEvents.afterAssemblyReload, (AssemblyReloadEvents.AssemblyReloadCallback)Delegate.Combine(assemblyReloadCallback2, value), assemblyReloadCallback);
				}
				while (assemblyReloadCallback != assemblyReloadCallback2);
			}
			remove
			{
				AssemblyReloadEvents.AssemblyReloadCallback assemblyReloadCallback = AssemblyReloadEvents.afterAssemblyReload;
				AssemblyReloadEvents.AssemblyReloadCallback assemblyReloadCallback2;
				do
				{
					assemblyReloadCallback2 = assemblyReloadCallback;
					assemblyReloadCallback = Interlocked.CompareExchange<AssemblyReloadEvents.AssemblyReloadCallback>(ref AssemblyReloadEvents.afterAssemblyReload, (AssemblyReloadEvents.AssemblyReloadCallback)Delegate.Remove(assemblyReloadCallback2, value), assemblyReloadCallback);
				}
				while (assemblyReloadCallback != assemblyReloadCallback2);
			}
		}

		[RequiredByNativeCode]
		private static void OnBeforeAssemblyReload()
		{
			if (AssemblyReloadEvents.beforeAssemblyReload != null)
			{
				AssemblyReloadEvents.beforeAssemblyReload();
			}
		}

		[RequiredByNativeCode]
		private static void OnAfterAssemblyReload()
		{
			if (AssemblyReloadEvents.afterAssemblyReload != null)
			{
				AssemblyReloadEvents.afterAssemblyReload();
			}
		}
	}
}
