using System;

namespace UnityEditor
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class EditorWindowTitleAttribute : Attribute
	{
		public string title
		{
			get;
			set;
		}

		public string icon
		{
			get;
			set;
		}

		public bool useTypeNameAsIconName
		{
			get;
			set;
		}
	}
}
