using System;
using System.Collections.Generic;
using System.Reflection;
namespace UnityEngine.Events
{
	internal class InvokableCallList
	{
		private readonly List<BaseInvokableCall> m_PersistentCalls = new List<BaseInvokableCall>();
		private readonly List<BaseInvokableCall> m_RuntimeCalls = new List<BaseInvokableCall>();
		private readonly List<BaseInvokableCall> m_ExecutingCalls = new List<BaseInvokableCall>();
		public int Count
		{
			get
			{
				return this.m_PersistentCalls.Count + this.m_RuntimeCalls.Count;
			}
		}
		public void AddPersistentInvokableCall(BaseInvokableCall call)
		{
			this.m_PersistentCalls.Add(call);
		}
		public void AddListener(BaseInvokableCall call)
		{
			this.m_RuntimeCalls.Add(call);
		}
		public void RemoveListener(object targetObj, MethodInfo method)
		{
			List<BaseInvokableCall> list = new List<BaseInvokableCall>();
			for (int i = 0; i < this.m_RuntimeCalls.Count; i++)
			{
				if (this.m_RuntimeCalls[i].Find(targetObj, method))
				{
					list.Add(this.m_RuntimeCalls[i]);
				}
			}
			this.m_RuntimeCalls.RemoveAll(new Predicate<BaseInvokableCall>(list.Contains));
		}
		public void Clear()
		{
			this.m_RuntimeCalls.Clear();
		}
		public void ClearPersistent()
		{
			this.m_PersistentCalls.Clear();
		}
		public void Invoke(object[] parameters)
		{
			this.m_ExecutingCalls.AddRange(this.m_PersistentCalls);
			this.m_ExecutingCalls.AddRange(this.m_RuntimeCalls);
			for (int i = 0; i < this.m_ExecutingCalls.Count; i++)
			{
				this.m_ExecutingCalls[i].Invoke(parameters);
			}
			this.m_ExecutingCalls.Clear();
		}
	}
}
