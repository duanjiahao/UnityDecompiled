using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace UnityEditor.BuildReporting
{
	internal sealed class BuildReport : UnityEngine.Object
	{
		public event Action<BuildReport> Changed
		{
			add
			{
				Action<BuildReport> action = this.Changed;
				Action<BuildReport> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<BuildReport>>(ref this.Changed, (Action<BuildReport>)Delegate.Combine(action2, value), action);
				}
				while (action != action2);
			}
			remove
			{
				Action<BuildReport> action = this.Changed;
				Action<BuildReport> action2;
				do
				{
					action2 = action;
					action = Interlocked.CompareExchange<Action<BuildReport>>(ref this.Changed, (Action<BuildReport>)Delegate.Remove(action2, value), action);
				}
				while (action != action2);
			}
		}

		public extern uint crc
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern long totalTimeMS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern long totalSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern BuildTarget buildTarget
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern BuildOptions buildOptions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern string outputPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool succeeded
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int totalErrors
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int totalWarnings
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RelocateFiles(string originalPathPrefix, string newPathPrefix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddFile(string path, string role);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddFilesRecursive(string rootDir, string role);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DeleteFile(string path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void DeleteFilesRecursive(string rootDir);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string SummarizeErrors();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddMessage(LogType messageType, string message);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BeginBuildStepNoTiming(string stepName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BeginBuildStep(string stepName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddAppendix(UnityEngine.Object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern UnityEngine.Object[] GetAppendicesByClassID(int classID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern UnityEngine.Object[] GetAppendices(Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern UnityEngine.Object[] GetAllAppendices();

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
