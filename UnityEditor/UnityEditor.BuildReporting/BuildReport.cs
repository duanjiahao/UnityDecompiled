using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor.BuildReporting
{
	internal sealed class BuildReport : UnityEngine.Object
	{
		public event Action<BuildReport> Changed
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.Changed = (Action<BuildReport>)Delegate.Combine(this.Changed, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.Changed = (Action<BuildReport>)Delegate.Remove(this.Changed, value);
			}
		}

		public extern uint crc
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern long totalTimeMS
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern long totalSize
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern BuildTarget buildTarget
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern BuildOptions buildOptions
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string outputPath
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int totalErrors
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int totalWarnings
		{
			[WrapperlessIcall]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RelocateFiles(string originalPathPrefix, string newPathPrefix);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddFile(string path, string role);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddFilesRecursive(string rootDir, string role);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string SummarizeErrors();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddMessage(LogType messageType, string message);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BeginBuildStepNoTiming(string stepName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BeginBuildStep(string stepName);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddAppendix(UnityEngine.Object obj);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern UnityEngine.Object[] GetAppendicesByClassID(int classID);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern UnityEngine.Object[] GetAppendices(Type type);

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern UnityEngine.Object[] GetAllAppendices();

		[WrapperlessIcall]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern BuildReport GetLatestReport();

		public void SendChanged()
		{
			if (this.Changed != null)
			{
				this.Changed(this);
			}
		}
	}
}
