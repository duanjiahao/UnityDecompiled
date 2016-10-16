using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class UnityAPICompatibilityVersionAttribute : Attribute
	{
		private string _version;

		public string version
		{
			get
			{
				return this._version;
			}
		}

		public UnityAPICompatibilityVersionAttribute(string version)
		{
			this._version = version;
		}
	}
}
