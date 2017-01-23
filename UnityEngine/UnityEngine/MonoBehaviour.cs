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
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool runInEditMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[ThreadAndSerializationSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern MonoBehaviour();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Internal_CancelInvokeAll();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_IsInvokingAll();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Invoke(string methodName, float time);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void InvokeRepeating(string methodName, float time, float repeatRate);

		public void CancelInvoke()
		{
			this.Internal_CancelInvokeAll();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void CancelInvoke(string methodName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool IsInvoking(string methodName);

		public bool IsInvoking()
		{
			return this.Internal_IsInvokingAll();
		}

		public Coroutine StartCoroutine(IEnumerator routine)
		{
			return this.StartCoroutine_Auto_Internal(routine);
		}

		[Obsolete("StartCoroutine_Auto has been deprecated. Use StartCoroutine instead (UnityUpgradable) -> StartCoroutine([mscorlib] System.Collections.IEnumerator)", false)]
		public Coroutine StartCoroutine_Auto(IEnumerator routine)
		{
			return this.StartCoroutine(routine);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Coroutine StartCoroutine_Auto_Internal(IEnumerator routine);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value);

		[ExcludeFromDocs]
		public Coroutine StartCoroutine(string methodName)
		{
			object value = null;
			return this.StartCoroutine(methodName, value);
		}

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void StopCoroutineViaEnumerator_Auto(IEnumerator routine);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void StopCoroutine_Auto(Coroutine routine);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void StopAllCoroutines();

		public static void print(object message)
		{
			Debug.Log(message);
		}
	}
}
