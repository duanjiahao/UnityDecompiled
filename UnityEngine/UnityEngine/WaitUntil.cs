using System;

namespace UnityEngine
{
	public sealed class WaitUntil : CustomYieldInstruction
	{
		private Func<bool> m_Predicate;

		public override bool keepWaiting
		{
			get
			{
				return !this.m_Predicate();
			}
		}

		public WaitUntil(Func<bool> predicate)
		{
			this.m_Predicate = predicate;
		}
	}
}
