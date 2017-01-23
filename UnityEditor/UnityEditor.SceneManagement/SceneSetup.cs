using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEditor.SceneManagement
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class SceneSetup
	{
		[SerializeField]
		private string m_Path = null;

		[SerializeField]
		private bool m_IsLoaded = false;

		[SerializeField]
		private bool m_IsActive = false;

		public string path
		{
			get
			{
				return this.m_Path;
			}
			set
			{
				this.m_Path = value;
			}
		}

		public bool isLoaded
		{
			get
			{
				return this.m_IsLoaded;
			}
			set
			{
				this.m_IsLoaded = value;
			}
		}

		public bool isActive
		{
			get
			{
				return this.m_IsActive;
			}
			set
			{
				this.m_IsActive = value;
			}
		}
	}
}
