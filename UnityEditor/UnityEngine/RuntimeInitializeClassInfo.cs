using System;
using System.Runtime.InteropServices;
namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class RuntimeInitializeClassInfo
	{
		private string m_AssemblyName;
		private string m_ClassName;
		private string[] m_MethodNames;
		internal string assemblyName
		{
			get
			{
				return this.m_AssemblyName;
			}
			set
			{
				this.m_AssemblyName = value;
			}
		}
		internal string className
		{
			get
			{
				return this.m_ClassName;
			}
			set
			{
				this.m_ClassName = value;
			}
		}
		internal string[] methodNames
		{
			get
			{
				return this.m_MethodNames;
			}
			set
			{
				this.m_MethodNames = value;
			}
		}
	}
}
