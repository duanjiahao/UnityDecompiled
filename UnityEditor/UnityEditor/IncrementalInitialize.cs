using System;
using UnityEngine;

namespace UnityEditor
{
	[Serializable]
	internal class IncrementalInitialize
	{
		public enum State
		{
			PreInitialize,
			Initialize,
			Initialized
		}

		[SerializeField]
		private IncrementalInitialize.State m_InitState;

		[NonSerialized]
		private bool m_IncrementOnNextEvent;

		public IncrementalInitialize.State state
		{
			get
			{
				return this.m_InitState;
			}
		}

		public void Restart()
		{
			this.m_InitState = IncrementalInitialize.State.PreInitialize;
		}

		public void OnEvent()
		{
			if (this.m_IncrementOnNextEvent)
			{
				this.m_InitState++;
				this.m_IncrementOnNextEvent = false;
			}
			IncrementalInitialize.State initState = this.m_InitState;
			if (initState != IncrementalInitialize.State.PreInitialize)
			{
				if (initState == IncrementalInitialize.State.Initialize)
				{
					this.m_IncrementOnNextEvent = true;
				}
			}
			else if (Event.current.type == EventType.Repaint)
			{
				this.m_IncrementOnNextEvent = true;
				HandleUtility.Repaint();
			}
		}
	}
}
