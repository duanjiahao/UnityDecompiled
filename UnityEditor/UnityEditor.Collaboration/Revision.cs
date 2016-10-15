using System;
using System.Runtime.InteropServices;

namespace UnityEditor.Collaboration
{
	[StructLayout(LayoutKind.Sequential)]
	internal class Revision
	{
		private string m_AuthorName;

		private string m_Author;

		private string m_Comment;

		private string m_RevisionID;

		private string m_Reference;

		private ulong m_TimeStamp;

		public string authorName
		{
			get
			{
				return this.m_AuthorName;
			}
		}

		public string author
		{
			get
			{
				return this.m_Author;
			}
		}

		public string comment
		{
			get
			{
				return this.m_Comment;
			}
		}

		public string revisionID
		{
			get
			{
				return this.m_RevisionID;
			}
		}

		public string reference
		{
			get
			{
				return this.m_Reference;
			}
		}

		public ulong timeStamp
		{
			get
			{
				return this.m_TimeStamp;
			}
		}

		private Revision()
		{
		}
	}
}
