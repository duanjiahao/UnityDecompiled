using System;

namespace UnityEditor
{
	public struct ExternalVersionControl
	{
		private string m_Value;

		public static readonly string Disabled = "Hidden Meta Files";

		public static readonly string AutoDetect = "Auto detect";

		public static readonly string Generic = "Visible Meta Files";

		[Obsolete("Asset Server VCS support has been removed.")]
		public static readonly string AssetServer = "Asset Server";

		public ExternalVersionControl(string value)
		{
			this.m_Value = value;
		}

		public static implicit operator string(ExternalVersionControl d)
		{
			return d.ToString();
		}

		public static implicit operator ExternalVersionControl(string d)
		{
			return new ExternalVersionControl(d);
		}

		public override string ToString()
		{
			return this.m_Value;
		}
	}
}
