using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class LightmapData
	{
		internal Texture2D m_Light;

		internal Texture2D m_Dir;

		public Texture2D lightmapLight
		{
			get
			{
				return this.m_Light;
			}
			set
			{
				this.m_Light = value;
			}
		}

		public Texture2D lightmapDir
		{
			get
			{
				return this.m_Dir;
			}
			set
			{
				this.m_Dir = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property LightmapData.lightmap has been deprecated. Use LightmapData.lightmapLight instead (UnityUpgradable) -> lightmapLight", true)]
		public Texture2D lightmap
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property LightmapData.lightmapFar has been deprecated. Use LightmapData.lightmapLight instead (UnityUpgradable) -> lightmapLight", true)]
		public Texture2D lightmapFar
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property LightmapData.lightmapNear has been deprecated. Use LightmapData.lightmapDir instead (UnityUpgradable) -> lightmapDir", true)]
		public Texture2D lightmapNear
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
	}
}
