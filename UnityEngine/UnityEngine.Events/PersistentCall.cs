using System;
using System.Reflection;
using UnityEngine.Serialization;

namespace UnityEngine.Events
{
	[Serializable]
	internal class PersistentCall
	{
		[FormerlySerializedAs("instance"), SerializeField]
		private UnityEngine.Object m_Target;

		[FormerlySerializedAs("methodName"), SerializeField]
		private string m_MethodName;

		[FormerlySerializedAs("mode"), SerializeField]
		private PersistentListenerMode m_Mode;

		[FormerlySerializedAs("arguments"), SerializeField]
		private ArgumentCache m_Arguments = new ArgumentCache();

		[FormerlySerializedAs("m_Enabled"), FormerlySerializedAs("enabled"), SerializeField]
		private UnityEventCallState m_CallState = UnityEventCallState.RuntimeOnly;

		public UnityEngine.Object target
		{
			get
			{
				return this.m_Target;
			}
		}

		public string methodName
		{
			get
			{
				return this.m_MethodName;
			}
		}

		public PersistentListenerMode mode
		{
			get
			{
				return this.m_Mode;
			}
			set
			{
				this.m_Mode = value;
			}
		}

		public ArgumentCache arguments
		{
			get
			{
				return this.m_Arguments;
			}
		}

		public UnityEventCallState callState
		{
			get
			{
				return this.m_CallState;
			}
			set
			{
				this.m_CallState = value;
			}
		}

		public bool IsValid()
		{
			return this.target != null && !string.IsNullOrEmpty(this.methodName);
		}

		public BaseInvokableCall GetRuntimeCall(UnityEventBase theEvent)
		{
			if (this.m_CallState == UnityEventCallState.RuntimeOnly && !Application.isPlaying)
			{
				return null;
			}
			if (this.m_CallState == UnityEventCallState.Off || theEvent == null)
			{
				return null;
			}
			MethodInfo methodInfo = theEvent.FindMethod(this);
			if (methodInfo == null)
			{
				return null;
			}
			switch (this.m_Mode)
			{
			case PersistentListenerMode.EventDefined:
				return theEvent.GetDelegate(this.target, methodInfo);
			case PersistentListenerMode.Void:
				return new InvokableCall(this.target, methodInfo);
			case PersistentListenerMode.Object:
				return PersistentCall.GetObjectCall(this.target, methodInfo, this.m_Arguments);
			case PersistentListenerMode.Int:
				return new CachedInvokableCall<int>(this.target, methodInfo, this.m_Arguments.intArgument);
			case PersistentListenerMode.Float:
				return new CachedInvokableCall<float>(this.target, methodInfo, this.m_Arguments.floatArgument);
			case PersistentListenerMode.String:
				return new CachedInvokableCall<string>(this.target, methodInfo, this.m_Arguments.stringArgument);
			case PersistentListenerMode.Bool:
				return new CachedInvokableCall<bool>(this.target, methodInfo, this.m_Arguments.boolArgument);
			default:
				return null;
			}
		}

		private static BaseInvokableCall GetObjectCall(UnityEngine.Object target, MethodInfo method, ArgumentCache arguments)
		{
			Type type = typeof(UnityEngine.Object);
			if (!string.IsNullOrEmpty(arguments.unityObjectArgumentAssemblyTypeName))
			{
				type = (Type.GetType(arguments.unityObjectArgumentAssemblyTypeName, false) ?? typeof(UnityEngine.Object));
			}
			Type typeFromHandle = typeof(CachedInvokableCall<>);
			Type type2 = typeFromHandle.MakeGenericType(new Type[]
			{
				type
			});
			ConstructorInfo constructor = type2.GetConstructor(new Type[]
			{
				typeof(UnityEngine.Object),
				typeof(MethodInfo),
				type
			});
			UnityEngine.Object @object = arguments.unityObjectArgument;
			if (@object != null && !type.IsAssignableFrom(@object.GetType()))
			{
				@object = null;
			}
			return constructor.Invoke(new object[]
			{
				target,
				method,
				@object
			}) as BaseInvokableCall;
		}

		public void RegisterPersistentListener(UnityEngine.Object ttarget, string mmethodName)
		{
			this.m_Target = ttarget;
			this.m_MethodName = mmethodName;
		}

		public void UnregisterPersistentListener()
		{
			this.m_MethodName = string.Empty;
			this.m_Target = null;
		}
	}
}
