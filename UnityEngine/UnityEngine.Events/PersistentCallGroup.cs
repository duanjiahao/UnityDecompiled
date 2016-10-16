using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.Events
{
	[Serializable]
	internal class PersistentCallGroup
	{
		[FormerlySerializedAs("m_Listeners"), SerializeField]
		private List<PersistentCall> m_Calls;

		public int Count
		{
			get
			{
				return this.m_Calls.Count;
			}
		}

		public PersistentCallGroup()
		{
			this.m_Calls = new List<PersistentCall>();
		}

		public PersistentCall GetListener(int index)
		{
			return this.m_Calls[index];
		}

		public IEnumerable<PersistentCall> GetListeners()
		{
			return this.m_Calls;
		}

		public void AddListener()
		{
			this.m_Calls.Add(new PersistentCall());
		}

		public void AddListener(PersistentCall call)
		{
			this.m_Calls.Add(call);
		}

		public void RemoveListener(int index)
		{
			this.m_Calls.RemoveAt(index);
		}

		public void Clear()
		{
			this.m_Calls.Clear();
		}

		public void RegisterEventPersistentListener(int index, UnityEngine.Object targetObj, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.EventDefined;
		}

		public void RegisterVoidPersistentListener(int index, UnityEngine.Object targetObj, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Void;
		}

		public void RegisterObjectPersistentListener(int index, UnityEngine.Object targetObj, UnityEngine.Object argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Object;
			listener.arguments.unityObjectArgument = argument;
		}

		public void RegisterIntPersistentListener(int index, UnityEngine.Object targetObj, int argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Int;
			listener.arguments.intArgument = argument;
		}

		public void RegisterFloatPersistentListener(int index, UnityEngine.Object targetObj, float argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Float;
			listener.arguments.floatArgument = argument;
		}

		public void RegisterStringPersistentListener(int index, UnityEngine.Object targetObj, string argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.String;
			listener.arguments.stringArgument = argument;
		}

		public void RegisterBoolPersistentListener(int index, UnityEngine.Object targetObj, bool argument, string methodName)
		{
			PersistentCall listener = this.GetListener(index);
			listener.RegisterPersistentListener(targetObj, methodName);
			listener.mode = PersistentListenerMode.Bool;
			listener.arguments.boolArgument = argument;
		}

		public void UnregisterPersistentListener(int index)
		{
			PersistentCall listener = this.GetListener(index);
			listener.UnregisterPersistentListener();
		}

		public void RemoveListeners(UnityEngine.Object target, string methodName)
		{
			List<PersistentCall> list = new List<PersistentCall>();
			for (int i = 0; i < this.m_Calls.Count; i++)
			{
				if (this.m_Calls[i].target == target && this.m_Calls[i].methodName == methodName)
				{
					list.Add(this.m_Calls[i]);
				}
			}
			this.m_Calls.RemoveAll(new Predicate<PersistentCall>(list.Contains));
		}

		public void Initialize(InvokableCallList invokableList, UnityEventBase unityEventBase)
		{
			foreach (PersistentCall current in this.m_Calls)
			{
				if (current.IsValid())
				{
					BaseInvokableCall runtimeCall = current.GetRuntimeCall(unityEventBase);
					if (runtimeCall != null)
					{
						invokableList.AddPersistentInvokableCall(runtimeCall);
					}
				}
			}
		}
	}
}
