using System;
using System.Reflection;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine.Events
{
	[Serializable]
	public class UnityEvent : UnityEventBase
	{
		private readonly object[] m_InvokeArray = new object[0];

		[RequiredByNativeCode]
		public UnityEvent()
		{
		}

		public void AddListener(UnityAction call)
		{
			base.AddCall(UnityEvent.GetDelegate(call));
		}

		public void RemoveListener(UnityAction call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[0]);
		}

		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction action)
		{
			return new InvokableCall(action);
		}

		public void Invoke()
		{
			base.Invoke(this.m_InvokeArray);
		}

		internal void AddPersistentListener(UnityAction call)
		{
			this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
		}

		internal void AddPersistentListener(UnityAction call, UnityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		internal void RegisterPersistentListener(int index, UnityAction call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}
	}
	[Serializable]
	public abstract class UnityEvent<T0> : UnityEventBase
	{
		private readonly object[] m_InvokeArray = new object[1];

		[RequiredByNativeCode]
		public UnityEvent()
		{
		}

		public void AddListener(UnityAction<T0> call)
		{
			base.AddCall(UnityEvent<T0>.GetDelegate(call));
		}

		public void RemoveListener(UnityAction<T0> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[]
			{
				typeof(T0)
			});
		}

		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0> action)
		{
			return new InvokableCall<T0>(action);
		}

		public void Invoke(T0 arg0)
		{
			this.m_InvokeArray[0] = arg0;
			base.Invoke(this.m_InvokeArray);
		}

		internal void AddPersistentListener(UnityAction<T0> call)
		{
			this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
		}

		internal void AddPersistentListener(UnityAction<T0> call, UnityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		internal void RegisterPersistentListener(int index, UnityAction<T0> call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}
	}
	[Serializable]
	public abstract class UnityEvent<T0, T1> : UnityEventBase
	{
		private readonly object[] m_InvokeArray = new object[2];

		[RequiredByNativeCode]
		public UnityEvent()
		{
		}

		public void AddListener(UnityAction<T0, T1> call)
		{
			base.AddCall(UnityEvent<T0, T1>.GetDelegate(call));
		}

		public void RemoveListener(UnityAction<T0, T1> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[]
			{
				typeof(T0),
				typeof(T1)
			});
		}

		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0, T1> action)
		{
			return new InvokableCall<T0, T1>(action);
		}

		public void Invoke(T0 arg0, T1 arg1)
		{
			this.m_InvokeArray[0] = arg0;
			this.m_InvokeArray[1] = arg1;
			base.Invoke(this.m_InvokeArray);
		}

		internal void AddPersistentListener(UnityAction<T0, T1> call)
		{
			this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
		}

		internal void AddPersistentListener(UnityAction<T0, T1> call, UnityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		internal void RegisterPersistentListener(int index, UnityAction<T0, T1> call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}
	}
	[Serializable]
	public abstract class UnityEvent<T0, T1, T2> : UnityEventBase
	{
		private readonly object[] m_InvokeArray = new object[3];

		[RequiredByNativeCode]
		public UnityEvent()
		{
		}

		public void AddListener(UnityAction<T0, T1, T2> call)
		{
			base.AddCall(UnityEvent<T0, T1, T2>.GetDelegate(call));
		}

		public void RemoveListener(UnityAction<T0, T1, T2> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[]
			{
				typeof(T0),
				typeof(T1),
				typeof(T2)
			});
		}

		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1, T2>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2> action)
		{
			return new InvokableCall<T0, T1, T2>(action);
		}

		public void Invoke(T0 arg0, T1 arg1, T2 arg2)
		{
			this.m_InvokeArray[0] = arg0;
			this.m_InvokeArray[1] = arg1;
			this.m_InvokeArray[2] = arg2;
			base.Invoke(this.m_InvokeArray);
		}

		internal void AddPersistentListener(UnityAction<T0, T1, T2> call)
		{
			this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
		}

		internal void AddPersistentListener(UnityAction<T0, T1, T2> call, UnityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		internal void RegisterPersistentListener(int index, UnityAction<T0, T1, T2> call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}
	}
	[Serializable]
	public abstract class UnityEvent<T0, T1, T2, T3> : UnityEventBase
	{
		private readonly object[] m_InvokeArray = new object[4];

		[RequiredByNativeCode]
		public UnityEvent()
		{
		}

		public void AddListener(UnityAction<T0, T1, T2, T3> call)
		{
			base.AddCall(UnityEvent<T0, T1, T2, T3>.GetDelegate(call));
		}

		public void RemoveListener(UnityAction<T0, T1, T2, T3> call)
		{
			base.RemoveListener(call.Target, call.GetMethodInfo());
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[]
			{
				typeof(T0),
				typeof(T1),
				typeof(T2),
				typeof(T3)
			});
		}

		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1, T2, T3>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2, T3> action)
		{
			return new InvokableCall<T0, T1, T2, T3>(action);
		}

		public void Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			this.m_InvokeArray[0] = arg0;
			this.m_InvokeArray[1] = arg1;
			this.m_InvokeArray[2] = arg2;
			this.m_InvokeArray[3] = arg3;
			base.Invoke(this.m_InvokeArray);
		}

		internal void AddPersistentListener(UnityAction<T0, T1, T2, T3> call)
		{
			this.AddPersistentListener(call, UnityEventCallState.RuntimeOnly);
		}

		internal void AddPersistentListener(UnityAction<T0, T1, T2, T3> call, UnityEventCallState callState)
		{
			int persistentEventCount = base.GetPersistentEventCount();
			base.AddPersistentListener();
			this.RegisterPersistentListener(persistentEventCount, call);
			base.SetPersistentListenerState(persistentEventCount, callState);
		}

		internal void RegisterPersistentListener(int index, UnityAction<T0, T1, T2, T3> call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
				return;
			}
			base.RegisterPersistentListener(index, call.Target as UnityEngine.Object, call.Method);
		}
	}
}
