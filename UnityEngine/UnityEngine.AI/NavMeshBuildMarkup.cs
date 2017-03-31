using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.AI
{
	public struct NavMeshBuildMarkup
	{
		private int m_OverrideArea;

		private int m_Area;

		private int m_IgnoreFromBuild;

		private int m_InstanceID;

		public bool overrideArea
		{
			get
			{
				return this.m_OverrideArea != 0;
			}
			set
			{
				this.m_OverrideArea = ((!value) ? 0 : 1);
			}
		}

		public int area
		{
			get
			{
				return this.m_Area;
			}
			set
			{
				this.m_Area = value;
			}
		}

		public bool ignoreFromBuild
		{
			get
			{
				return this.m_IgnoreFromBuild != 0;
			}
			set
			{
				this.m_IgnoreFromBuild = ((!value) ? 0 : 1);
			}
		}

		public Transform root
		{
			get
			{
				return this.InternalGetRootGO(this.m_InstanceID);
			}
			set
			{
				this.m_InstanceID = ((!(value != null)) ? 0 : value.GetInstanceID());
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Transform InternalGetRootGO(int instanceID);
	}
}
