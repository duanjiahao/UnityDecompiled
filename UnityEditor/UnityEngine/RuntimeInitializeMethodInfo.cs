using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class RuntimeInitializeMethodInfo
	{
		private string m_FullClassName;

		private string m_MethodName;

		private int m_OrderNumber = 0;

		private bool m_IsUnityClass = false;

		internal string fullClassName
		{
			get
			{
				return this.m_FullClassName;
			}
			set
			{
				this.m_FullClassName = value;
			}
		}

		internal string methodName
		{
			get
			{
				return this.m_MethodName;
			}
			set
			{
				this.m_MethodName = value;
			}
		}

		internal int orderNumber
		{
			get
			{
				return this.m_OrderNumber;
			}
			set
			{
				this.m_OrderNumber = value;
			}
		}

		internal bool isUnityClass
		{
			get
			{
				return this.m_IsUnityClass;
			}
			set
			{
				this.m_IsUnityClass = value;
			}
		}
	}
}
