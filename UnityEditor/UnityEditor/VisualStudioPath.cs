using System;

namespace UnityEditor
{
	internal class VisualStudioPath
	{
		public string Path
		{
			get;
			set;
		}

		public string Edition
		{
			get;
			set;
		}

		public VisualStudioPath(string path, string edition = "")
		{
			this.Path = path;
			this.Edition = edition;
		}
	}
}
