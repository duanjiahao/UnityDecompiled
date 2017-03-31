using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false), UsedByNativeCode]
	public sealed class HelpURLAttribute : Attribute
	{
		internal readonly string m_Url;

		public string URL
		{
			get
			{
				return this.m_Url;
			}
		}

		public HelpURLAttribute(string url)
		{
			this.m_Url = url;
		}
	}
}
