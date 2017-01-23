using System;

namespace UnityEditor.Connect
{
	internal struct ProjectInfo
	{
		private int m_Valid;

		private int m_BuildAllowed;

		private int m_ProjectBound;

		private string m_ProjectGUID;

		private string m_ProjectName;

		private string m_OrganizationID;

		private string m_OrganizationName;

		private string m_OrganizationForeignKey;

		private int m_COPPA;

		private int m_COPPALock;

		private int m_MoveLock;

		public bool valid
		{
			get
			{
				return this.m_Valid != 0;
			}
		}

		public bool buildAllowed
		{
			get
			{
				return this.m_BuildAllowed != 0;
			}
		}

		public bool projectBound
		{
			get
			{
				return this.m_ProjectBound != 0;
			}
		}

		public string projectGUID
		{
			get
			{
				return this.m_ProjectGUID;
			}
		}

		public string projectName
		{
			get
			{
				return this.m_ProjectName;
			}
		}

		public string organizationId
		{
			get
			{
				return this.m_OrganizationID;
			}
		}

		public string organizationName
		{
			get
			{
				return this.m_OrganizationName;
			}
		}

		public string organizationForeignKey
		{
			get
			{
				return this.m_OrganizationForeignKey;
			}
		}

		public COPPACompliance COPPA
		{
			get
			{
				COPPACompliance result;
				if (this.m_COPPA == 1)
				{
					result = COPPACompliance.COPPACompliant;
				}
				else if (this.m_COPPA == 2)
				{
					result = COPPACompliance.COPPANotCompliant;
				}
				else
				{
					result = COPPACompliance.COPPAUndefined;
				}
				return result;
			}
		}

		public bool coppaLock
		{
			get
			{
				return this.m_COPPALock != 0;
			}
		}

		public bool moveLock
		{
			get
			{
				return this.m_MoveLock != 0;
			}
		}
	}
}
