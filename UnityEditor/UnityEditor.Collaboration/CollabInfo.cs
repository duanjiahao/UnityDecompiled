using System;
using UnityEngine.SceneManagement;

namespace UnityEditor.Collaboration
{
	internal struct CollabInfo
	{
		private int m_Ready;

		private int m_Update;

		private int m_Publish;

		private int m_InProgress;

		private int m_Maintenance;

		private int m_Conflict;

		private int m_Refresh;

		private string m_Tip;

		public bool ready
		{
			get
			{
				return this.m_Ready != 0;
			}
		}

		public bool update
		{
			get
			{
				return this.m_Update != 0;
			}
		}

		public bool publish
		{
			get
			{
				return this.m_Publish != 0;
			}
		}

		public bool inProgress
		{
			get
			{
				return this.m_InProgress != 0;
			}
		}

		public bool maintenance
		{
			get
			{
				return this.m_Maintenance != 0;
			}
		}

		public bool conflict
		{
			get
			{
				return this.m_Conflict != 0;
			}
		}

		public bool dirty
		{
			get
			{
				return SceneManager.GetActiveScene().isDirty;
			}
		}

		public bool refresh
		{
			get
			{
				return this.m_Refresh != 0;
			}
		}

		public string tip
		{
			get
			{
				return this.m_Tip;
			}
		}
	}
}
