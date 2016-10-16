using System;
using UnityEngine.Scripting;

namespace UnityEngine.Serialization
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false), RequiredByNativeCode]
	public class FormerlySerializedAsAttribute : Attribute
	{
		private string m_oldName;

		public string oldName
		{
			get
			{
				return this.m_oldName;
			}
		}

		public FormerlySerializedAsAttribute(string oldName)
		{
			this.m_oldName = oldName;
		}
	}
}
