using System;
using UnityEngine;

namespace UnityEditor.MemoryProfiler
{
	[Serializable]
	public struct Connection
	{
		[SerializeField]
		internal int m_From;

		[SerializeField]
		internal int m_To;

		public int from
		{
			get
			{
				return this.m_From;
			}
			set
			{
				this.m_From = value;
			}
		}

		public int to
		{
			get
			{
				return this.m_To;
			}
			set
			{
				this.m_To = value;
			}
		}
	}
}
