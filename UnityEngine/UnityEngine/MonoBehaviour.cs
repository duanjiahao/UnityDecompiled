using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	public class MonoBehaviour : Behaviour
	{
		public extern bool useGUILayout
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[ThreadAndSerializationSafe, WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern MonoBehaviour();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_CancelInvokeAll();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_IsInvokingAll();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Invoke(string methodName, float time);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InvokeRepeating(string methodName, float time, float repeatRate);

		public void CancelInvoke()
		{
			this.Internal_CancelInvokeAll();
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CancelInvoke(string methodName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsInvoking(string methodName);

		public bool IsInvoking()
		{
			return this.Internal_IsInvokingAll();
		}

		public Coroutine StartCoroutine(IEnumerator routine)
		{
			return this.StartCoroutine_Auto(routine);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Coroutine StartCoroutine_Auto(IEnumerator routine);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value);

		[ExcludeFromDocs]
		public Coroutine StartCoroutine(string methodName)
		{
			object value = null;
			return this.StartCoroutine(methodName, value);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StopCoroutine(string methodName);

		public void StopCoroutine(IEnumerator routine)
		{
			this.StopCoroutineViaEnumerator_Auto(routine);
		}

		public void StopCoroutine(Coroutine routine)
		{
			this.StopCoroutine_Auto(routine);
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void StopCoroutineViaEnumerator_Auto(IEnumerator routine);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void StopCoroutine_Auto(Coroutine routine);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StopAllCoroutines();

		public static void print(object message)
		{
			Debug.Log(message);
		}
	}
}
