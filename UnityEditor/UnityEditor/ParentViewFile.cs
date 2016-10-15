using System;

namespace UnityEditor
{
	[Serializable]
	internal class ParentViewFile
	{
		public string guid;

		public string name;

		public ChangeFlags changeFlags;

		public ParentViewFile(string name, string guid)
		{
			this.guid = guid;
			this.name = name;
			this.changeFlags = ChangeFlags.None;
		}

		public ParentViewFile(string name, string guid, ChangeFlags flags)
		{
			this.guid = guid;
			this.name = name;
			this.changeFlags = flags;
		}
	}
}
