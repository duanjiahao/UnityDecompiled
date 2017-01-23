using System;

namespace UnityEngine.EventSystems
{
	public abstract class AbstractEventData
	{
		protected bool m_Used;

		public virtual bool used
		{
			get
			{
				return this.m_Used;
			}
		}

		public virtual void Reset()
		{
			this.m_Used = false;
		}

		public virtual void Use()
		{
			this.m_Used = true;
		}
	}
}
