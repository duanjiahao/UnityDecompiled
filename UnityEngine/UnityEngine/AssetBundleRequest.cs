using System;
using System.Runtime.InteropServices;
namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class AssetBundleRequest : AsyncOperation
	{
		internal AssetBundle m_AssetBundle;
		internal string m_Path;
		internal Type m_Type;
		public Object asset
		{
			get
			{
				return this.m_AssetBundle.Load(this.m_Path, this.m_Type);
			}
		}
	}
}
