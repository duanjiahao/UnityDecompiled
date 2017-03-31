using System;
using System.Reflection;
using UnityEngine.Scripting;
using UnityEngine.Serialization;

namespace UnityEngine.Events
{
	[UsedByNativeCode]
	[Serializable]
	public abstract class UnityEventBase : ISerializationCallbackReceiver
	{
		private InvokableCallList m_Calls;

		[FormerlySerializedAs("m_PersistentListeners"), SerializeField]
		private PersistentCallGroup m_PersistentCalls;

		[SerializeField]
		private string m_TypeName;

		private bool m_CallsDirty = true;

		protected UnityEventBase()
		{
			this.m_Calls = new InvokableCallList();
			this.m_PersistentCalls = new PersistentCallGroup();
			this.m_TypeName = base.GetType().AssemblyQualifiedName;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.DirtyPersistentCalls();
			this.m_TypeName = base.GetType().AssemblyQualifiedName;
		}

		protected abstract MethodInfo FindMethod_Impl(string name, object targetObj);

		internal abstract BaseInvokableCall GetDelegate(object target, MethodInfo theFunction);

		internal MethodInfo FindMethod(PersistentCall call)
		{
			Type argumentType = typeof(UnityEngine.Object);
			if (!string.IsNullOrEmpty(call.arguments.unityObjectArgumentAssemblyTypeName))
			{
				argumentType = (Type.GetType(call.arguments.unityObjectArgumentAssemblyTypeName, false) ?? typeof(UnityEngine.Object));
			}
			return this.FindMethod(call.methodName, call.target, call.mode, argumentType);
		}

		internal MethodInfo FindMethod(string name, object listener, PersistentListenerMode mode, Type argumentType)
		{
			MethodInfo result;
			switch (mode)
			{
			case PersistentListenerMode.EventDefined:
				result = this.FindMethod_Impl(name, listener);
				break;
			case PersistentListenerMode.Void:
				result = UnityEventBase.GetValidMethodInfo(listener, name, new Type[0]);
				break;
			case PersistentListenerMode.Object:
				result = UnityEventBase.GetValidMethodInfo(listener, name, new Type[]
				{
					argumentType ?? typeof(UnityEngine.Object)
				});
				break;
			case PersistentListenerMode.Int:
				result = UnityEventBase.GetValidMethodInfo(listener, name, new Type[]
				{
					typeof(int)
				});
				break;
			case PersistentListenerMode.Float:
				result = UnityEventBase.GetValidMethodInfo(listener, name, new Type[]
				{
					typeof(float)
				});
				break;
			case PersistentListenerMode.String:
				result = UnityEventBase.GetValidMethodInfo(listener, name, new Type[]
				{
					typeof(string)
				});
				break;
			case PersistentListenerMode.Bool:
				result = UnityEventBase.GetValidMethodInfo(listener, name, new Type[]
				{
					typeof(bool)
				});
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		public int GetPersistentEventCount()
		{
			return this.m_PersistentCalls.Count;
		}

		public UnityEngine.Object GetPersistentTarget(int index)
		{
			PersistentCall listener = this.m_PersistentCalls.GetListener(index);
			return (listener == null) ? null : listener.target;
		}

		public string GetPersistentMethodName(int index)
		{
			PersistentCall listener = this.m_PersistentCalls.GetListener(index);
			return (listener == null) ? string.Empty : listener.methodName;
		}

		private void DirtyPersistentCalls()
		{
			this.m_Calls.ClearPersistent();
			this.m_CallsDirty = true;
		}

		private void RebuildPersistentCallsIfNeeded()
		{
			if (this.m_CallsDirty)
			{
				this.m_PersistentCalls.Initialize(this.m_Calls, this);
				this.m_CallsDirty = false;
			}
		}

		public void SetPersistentListenerState(int index, UnityEventCallState state)
		{
			PersistentCall listener = this.m_PersistentCalls.GetListener(index);
			if (listener != null)
			{
				listener.callState = state;
			}
			this.DirtyPersistentCalls();
		}

		protected void AddListener(object targetObj, MethodInfo method)
		{
			this.m_Calls.AddListener(this.GetDelegate(targetObj, method));
		}

		internal void AddCall(BaseInvokableCall call)
		{
			this.m_Calls.AddListener(call);
		}

		protected void RemoveListener(object targetObj, MethodInfo method)
		{
			this.m_Calls.RemoveListener(targetObj, method);
		}

		public void RemoveAllListeners()
		{
			this.m_Calls.Clear();
		}

		protected void Invoke(object[] parameters)
		{
			this.RebuildPersistentCallsIfNeeded();
			this.m_Calls.Invoke(parameters);
		}

		public override string ToString()
		{
			return base.ToString() + " " + base.GetType().FullName;
		}

		public static MethodInfo GetValidMethodInfo(object obj, string functionName, Type[] argumentTypes)
		{
			Type type = obj.GetType();
			MethodInfo result;
			while (type != typeof(object) && type != null)
			{
				MethodInfo method = type.GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, argumentTypes, null);
				if (method != null)
				{
					ParameterInfo[] parameters = method.GetParameters();
					bool flag = true;
					int num = 0;
					ParameterInfo[] array = parameters;
					for (int i = 0; i < array.Length; i++)
					{
						ParameterInfo parameterInfo = array[i];
						Type type2 = argumentTypes[num];
						Type parameterType = parameterInfo.ParameterType;
						flag = (type2.IsPrimitive == parameterType.IsPrimitive);
						if (!flag)
						{
							break;
						}
						num++;
					}
					if (flag)
					{
						result = method;
						return result;
					}
				}
				type = type.BaseType;
			}
			result = null;
			return result;
		}

		protected bool ValidateRegistration(MethodInfo method, object targetObj, PersistentListenerMode mode)
		{
			return this.ValidateRegistration(method, targetObj, mode, typeof(UnityEngine.Object));
		}

		protected bool ValidateRegistration(MethodInfo method, object targetObj, PersistentListenerMode mode, Type argumentType)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method", UnityString.Format("Can not register null method on {0} for callback!", new object[]
				{
					targetObj
				}));
			}
			UnityEngine.Object @object = targetObj as UnityEngine.Object;
			if (@object == null || @object.GetInstanceID() == 0)
			{
				throw new ArgumentException(UnityString.Format("Could not register callback {0} on {1}. The class {2} does not derive from UnityEngine.Object", new object[]
				{
					method.Name,
					targetObj,
					(targetObj != null) ? targetObj.GetType().ToString() : "null"
				}));
			}
			if (method.IsStatic)
			{
				throw new ArgumentException(UnityString.Format("Could not register listener {0} on {1} static functions are not supported.", new object[]
				{
					method,
					base.GetType()
				}));
			}
			bool result;
			if (this.FindMethod(method.Name, targetObj, mode, argumentType) == null)
			{
				Debug.LogWarning(UnityString.Format("Could not register listener {0}.{1} on {2} the method could not be found.", new object[]
				{
					targetObj,
					method,
					base.GetType()
				}));
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		internal void AddPersistentListener()
		{
			this.m_PersistentCalls.AddListener();
		}

		protected void RegisterPersistentListener(int index, object targetObj, MethodInfo method)
		{
			if (this.ValidateRegistration(method, targetObj, PersistentListenerMode.EventDefined))
			{
				this.m_PersistentCalls.RegisterEventPersistentListener(index, targetObj as UnityEngine.Object, method.Name);
				this.DirtyPersistentCalls();
			}
		}

		internal void RemovePersistentListener(UnityEngine.Object target, MethodInfo method)
		{
			if (method != null && !method.IsStatic && !(target == null) && target.GetInstanceID() != 0)
			{
				this.m_PersistentCalls.RemoveListeners(target, method.Name);
				this.DirtyPersistentCalls();
			}
		}

		internal void RemovePersistentListener(int index)
		{
			this.m_PersistentCalls.RemoveListener(index);
			this.DirtyPersistentCalls();
		}

		internal void UnregisterPersistentListener(int index)
		{
			this.m_PersistentCalls.UnregisterPersistentListener(index);
			this.DirtyPersistentCalls();
		}

		internal void AddVoidPersistentListener(UnityAction call)
		{
			int persistentEventCount = this.GetPersistentEventCount();
			this.AddPersistentListener();
			this.RegisterVoidPersistentListener(persistentEventCount, call);
		}

		internal void RegisterVoidPersistentListener(int index, UnityAction call)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
			}
			else if (this.ValidateRegistration(call.Method, call.Target, PersistentListenerMode.Void))
			{
				this.m_PersistentCalls.RegisterVoidPersistentListener(index, call.Target as UnityEngine.Object, call.Method.Name);
				this.DirtyPersistentCalls();
			}
		}

		internal void AddIntPersistentListener(UnityAction<int> call, int argument)
		{
			int persistentEventCount = this.GetPersistentEventCount();
			this.AddPersistentListener();
			this.RegisterIntPersistentListener(persistentEventCount, call, argument);
		}

		internal void RegisterIntPersistentListener(int index, UnityAction<int> call, int argument)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
			}
			else if (this.ValidateRegistration(call.Method, call.Target, PersistentListenerMode.Int))
			{
				this.m_PersistentCalls.RegisterIntPersistentListener(index, call.Target as UnityEngine.Object, argument, call.Method.Name);
				this.DirtyPersistentCalls();
			}
		}

		internal void AddFloatPersistentListener(UnityAction<float> call, float argument)
		{
			int persistentEventCount = this.GetPersistentEventCount();
			this.AddPersistentListener();
			this.RegisterFloatPersistentListener(persistentEventCount, call, argument);
		}

		internal void RegisterFloatPersistentListener(int index, UnityAction<float> call, float argument)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
			}
			else if (this.ValidateRegistration(call.Method, call.Target, PersistentListenerMode.Float))
			{
				this.m_PersistentCalls.RegisterFloatPersistentListener(index, call.Target as UnityEngine.Object, argument, call.Method.Name);
				this.DirtyPersistentCalls();
			}
		}

		internal void AddBoolPersistentListener(UnityAction<bool> call, bool argument)
		{
			int persistentEventCount = this.GetPersistentEventCount();
			this.AddPersistentListener();
			this.RegisterBoolPersistentListener(persistentEventCount, call, argument);
		}

		internal void RegisterBoolPersistentListener(int index, UnityAction<bool> call, bool argument)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
			}
			else if (this.ValidateRegistration(call.Method, call.Target, PersistentListenerMode.Bool))
			{
				this.m_PersistentCalls.RegisterBoolPersistentListener(index, call.Target as UnityEngine.Object, argument, call.Method.Name);
				this.DirtyPersistentCalls();
			}
		}

		internal void AddStringPersistentListener(UnityAction<string> call, string argument)
		{
			int persistentEventCount = this.GetPersistentEventCount();
			this.AddPersistentListener();
			this.RegisterStringPersistentListener(persistentEventCount, call, argument);
		}

		internal void RegisterStringPersistentListener(int index, UnityAction<string> call, string argument)
		{
			if (call == null)
			{
				Debug.LogWarning("Registering a Listener requires an action");
			}
			else if (this.ValidateRegistration(call.Method, call.Target, PersistentListenerMode.String))
			{
				this.m_PersistentCalls.RegisterStringPersistentListener(index, call.Target as UnityEngine.Object, argument, call.Method.Name);
				this.DirtyPersistentCalls();
			}
		}

		internal void AddObjectPersistentListener<T>(UnityAction<T> call, T argument) where T : UnityEngine.Object
		{
			int persistentEventCount = this.GetPersistentEventCount();
			this.AddPersistentListener();
			this.RegisterObjectPersistentListener<T>(persistentEventCount, call, argument);
		}

		internal void RegisterObjectPersistentListener<T>(int index, UnityAction<T> call, T argument) where T : UnityEngine.Object
		{
			if (call == null)
			{
				throw new ArgumentNullException("call", "Registering a Listener requires a non null call");
			}
			if (this.ValidateRegistration(call.Method, call.Target, PersistentListenerMode.Object, (!(argument == null)) ? argument.GetType() : typeof(UnityEngine.Object)))
			{
				this.m_PersistentCalls.RegisterObjectPersistentListener(index, call.Target as UnityEngine.Object, argument, call.Method.Name);
				this.DirtyPersistentCalls();
			}
		}
	}
}
