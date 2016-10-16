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

		private bool m_NeedsUpdate = true;

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
			this.m_NeedsUpdate = true;
		}

		public void AddListener(BaseInvokableCall call)
		{
			this.m_RuntimeCalls.Add(call);
			this.m_NeedsUpdate = true;
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
			this.m_NeedsUpdate = true;
		}

		public void Clear()
		{
			this.m_RuntimeCalls.Clear();
			this.m_NeedsUpdate = true;
		}

		public void ClearPersistent()
		{
			this.m_PersistentCalls.Clear();
			this.m_NeedsUpdate = true;
		}

		public void Invoke(object[] parameters)
		{
			if (this.m_NeedsUpdate)
			{
				this.m_ExecutingCalls.Clear();
				this.m_ExecutingCalls.AddRange(this.m_PersistentCalls);
				this.m_ExecutingCalls.AddRange(this.m_RuntimeCalls);
				this.m_NeedsUpdate = false;
			}
			for (int i = 0; i < this.m_ExecutingCalls.Count; i++)
			{
				this.m_ExecutingCalls[i].Invoke(parameters);
			}
		}
	}
}
