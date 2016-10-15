using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace UnityEditor
{
	internal class MonoCrossCompile
	{
		private class JobCompileAOT
		{
			private BuildTarget m_target;

			private string m_crossCompilerAbsolutePath;

			private string m_assembliesAbsoluteDirectory;

			private CrossCompileOptions m_crossCompileOptions;

			public string m_input;

			public string m_output;

			public string m_additionalOptions;

			public ManualResetEvent m_doneEvent = new ManualResetEvent(false);

			public Exception m_Exception;

			public JobCompileAOT(BuildTarget target, string crossCompilerAbsolutePath, string assembliesAbsoluteDirectory, CrossCompileOptions crossCompileOptions, string input, string output, string additionalOptions)
			{
				this.m_target = target;
				this.m_crossCompilerAbsolutePath = crossCompilerAbsolutePath;
				this.m_assembliesAbsoluteDirectory = assembliesAbsoluteDirectory;
				this.m_crossCompileOptions = crossCompileOptions;
				this.m_input = input;
				this.m_output = output;
				this.m_additionalOptions = additionalOptions;
			}

			public void ThreadPoolCallback(object threadContext)
			{
				try
				{
					MonoCrossCompile.CrossCompileAOT(this.m_target, this.m_crossCompilerAbsolutePath, this.m_assembliesAbsoluteDirectory, this.m_crossCompileOptions, this.m_input, this.m_output, this.m_additionalOptions);
				}
				catch (Exception exception)
				{
					this.m_Exception = exception;
				}
				this.m_doneEvent.Set();
			}
		}

		public static string ArtifactsPath;

		public static void CrossCompileAOTDirectory(BuildTarget buildTarget, CrossCompileOptions crossCompileOptions, string sourceAssembliesFolder, string targetCrossCompiledASMFolder, string additionalOptions)
		{
			MonoCrossCompile.CrossCompileAOTDirectory(buildTarget, crossCompileOptions, sourceAssembliesFolder, targetCrossCompiledASMFolder, string.Empty, additionalOptions);
		}

		public static void CrossCompileAOTDirectory(BuildTarget buildTarget, CrossCompileOptions crossCompileOptions, string sourceAssembliesFolder, string targetCrossCompiledASMFolder, string pathExtension, string additionalOptions)
		{
			string text = BuildPipeline.GetBuildToolsDirectory(buildTarget);
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				text = Path.Combine(Path.Combine(text, pathExtension), "mono-xcompiler");
			}
			else
			{
				text = Path.Combine(Path.Combine(text, pathExtension), "mono-xcompiler.exe");
			}
			sourceAssembliesFolder = Path.Combine(Directory.GetCurrentDirectory(), sourceAssembliesFolder);
			targetCrossCompiledASMFolder = Path.Combine(Directory.GetCurrentDirectory(), targetCrossCompiledASMFolder);
			string[] files = Directory.GetFiles(sourceAssembliesFolder);
			for (int i = 0; i < files.Length; i++)
			{
				string path = files[i];
				if (!(Path.GetExtension(path) != ".dll"))
				{
					string fileName = Path.GetFileName(path);
					string output = Path.Combine(targetCrossCompiledASMFolder, fileName + ".s");
					if (EditorUtility.DisplayCancelableProgressBar("Building Player", "AOT cross compile " + fileName, 0.95f))
					{
						throw new OperationCanceledException();
					}
					MonoCrossCompile.CrossCompileAOT(buildTarget, text, sourceAssembliesFolder, crossCompileOptions, fileName, output, additionalOptions);
				}
			}
		}

		public static bool CrossCompileAOTDirectoryParallel(BuildTarget buildTarget, CrossCompileOptions crossCompileOptions, string sourceAssembliesFolder, string targetCrossCompiledASMFolder, string additionalOptions)
		{
			return MonoCrossCompile.CrossCompileAOTDirectoryParallel(buildTarget, crossCompileOptions, sourceAssembliesFolder, targetCrossCompiledASMFolder, string.Empty, additionalOptions);
		}

		public static bool CrossCompileAOTDirectoryParallel(BuildTarget buildTarget, CrossCompileOptions crossCompileOptions, string sourceAssembliesFolder, string targetCrossCompiledASMFolder, string pathExtension, string additionalOptions)
		{
			string text = BuildPipeline.GetBuildToolsDirectory(buildTarget);
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				text = Path.Combine(Path.Combine(text, pathExtension), "mono-xcompiler");
			}
			else
			{
				text = Path.Combine(Path.Combine(text, pathExtension), "mono-xcompiler.exe");
			}
			return MonoCrossCompile.CrossCompileAOTDirectoryParallel(text, buildTarget, crossCompileOptions, sourceAssembliesFolder, targetCrossCompiledASMFolder, additionalOptions);
		}

		private static bool WaitForBuildOfFile(List<ManualResetEvent> events, ref long timeout)
		{
			long num = DateTime.Now.Ticks / 10000L;
			int num2 = WaitHandle.WaitAny(events.ToArray(), (int)timeout);
			long num3 = DateTime.Now.Ticks / 10000L;
			if (num2 == 258)
			{
				return false;
			}
			events.RemoveAt(num2);
			timeout -= num3 - num;
			if (timeout < 0L)
			{
				timeout = 0L;
			}
			return true;
		}

		public static void DisplayAOTProgressBar(int totalFiles, int filesFinished)
		{
			string info = string.Format("AOT cross compile ({0}/{1})", (filesFinished + 1).ToString(), totalFiles.ToString());
			EditorUtility.DisplayProgressBar("Building Player", info, 0.95f);
		}

		public static bool CrossCompileAOTDirectoryParallel(string crossCompilerPath, BuildTarget buildTarget, CrossCompileOptions crossCompileOptions, string sourceAssembliesFolder, string targetCrossCompiledASMFolder, string additionalOptions)
		{
			sourceAssembliesFolder = Path.Combine(Directory.GetCurrentDirectory(), sourceAssembliesFolder);
			targetCrossCompiledASMFolder = Path.Combine(Directory.GetCurrentDirectory(), targetCrossCompiledASMFolder);
			int num = 1;
			int num2 = 1;
			ThreadPool.GetMaxThreads(out num, out num2);
			List<MonoCrossCompile.JobCompileAOT> list = new List<MonoCrossCompile.JobCompileAOT>();
			List<ManualResetEvent> list2 = new List<ManualResetEvent>();
			bool flag = true;
			List<string> list3 = new List<string>(from path in Directory.GetFiles(sourceAssembliesFolder)
			where Path.GetExtension(path) == ".dll"
			select path);
			int count = list3.Count;
			int num3 = 0;
			MonoCrossCompile.DisplayAOTProgressBar(count, num3);
			long num4 = (long)Math.Min(1800000, (count + 3) * 1000 * 30);
			foreach (string current in list3)
			{
				string fileName = Path.GetFileName(current);
				string output = Path.Combine(targetCrossCompiledASMFolder, fileName + ".s");
				MonoCrossCompile.JobCompileAOT jobCompileAOT = new MonoCrossCompile.JobCompileAOT(buildTarget, crossCompilerPath, sourceAssembliesFolder, crossCompileOptions, fileName, output, additionalOptions);
				list.Add(jobCompileAOT);
				list2.Add(jobCompileAOT.m_doneEvent);
				ThreadPool.QueueUserWorkItem(new WaitCallback(jobCompileAOT.ThreadPoolCallback));
				if (list2.Count >= Environment.ProcessorCount)
				{
					flag = MonoCrossCompile.WaitForBuildOfFile(list2, ref num4);
					MonoCrossCompile.DisplayAOTProgressBar(count, num3);
					num3++;
					if (!flag)
					{
						break;
					}
				}
			}
			while (list2.Count > 0)
			{
				flag = MonoCrossCompile.WaitForBuildOfFile(list2, ref num4);
				MonoCrossCompile.DisplayAOTProgressBar(count, num3);
				num3++;
				if (!flag)
				{
					break;
				}
			}
			foreach (MonoCrossCompile.JobCompileAOT current2 in list)
			{
				if (current2.m_Exception != null)
				{
					UnityEngine.Debug.LogErrorFormat("Cross compilation job {0} failed.\n{1}", new object[]
					{
						current2.m_input,
						current2.m_Exception
					});
					flag = false;
				}
			}
			return flag;
		}

		private static bool IsDebugableAssembly(string fname)
		{
			fname = Path.GetFileName(fname);
			return fname.StartsWith("Assembly", StringComparison.OrdinalIgnoreCase);
		}

		private static void CrossCompileAOT(BuildTarget target, string crossCompilerAbsolutePath, string assembliesAbsoluteDirectory, CrossCompileOptions crossCompileOptions, string input, string output, string additionalOptions)
		{
			string text = string.Empty;
			if (!MonoCrossCompile.IsDebugableAssembly(input))
			{
				crossCompileOptions &= ~CrossCompileOptions.Debugging;
				crossCompileOptions &= ~CrossCompileOptions.LoadSymbols;
			}
			bool flag = (crossCompileOptions & CrossCompileOptions.Debugging) != CrossCompileOptions.Dynamic;
			bool flag2 = (crossCompileOptions & CrossCompileOptions.LoadSymbols) != CrossCompileOptions.Dynamic;
			bool flag3 = flag || flag2;
			if (flag3)
			{
				text += "--debug ";
			}
			if (flag)
			{
				text += "--optimize=-linears ";
			}
			text += "--aot=full,asmonly,";
			if (flag3)
			{
				text += "write-symbols,";
			}
			if ((crossCompileOptions & CrossCompileOptions.Debugging) != CrossCompileOptions.Dynamic)
			{
				text += "soft-debug,";
			}
			else if (!flag3)
			{
				text += "nodebug,";
			}
			if (target != BuildTarget.iOS)
			{
				text += "print-skipped,";
			}
			if (additionalOptions != null & additionalOptions.Trim().Length > 0)
			{
				text = text + additionalOptions.Trim() + ",";
			}
			string fileName = Path.GetFileName(output);
			string text2 = Path.Combine(assembliesAbsoluteDirectory, fileName);
			if ((crossCompileOptions & CrossCompileOptions.FastICall) != CrossCompileOptions.Dynamic)
			{
				text += "ficall,";
			}
			if ((crossCompileOptions & CrossCompileOptions.Static) != CrossCompileOptions.Dynamic)
			{
				text += "static,";
			}
			string text3 = text;
			text = string.Concat(new string[]
			{
				text3,
				"outfile=\"",
				fileName,
				"\" \"",
				input,
				"\" "
			});
			Process process = new Process();
			process.StartInfo.FileName = crossCompilerAbsolutePath;
			process.StartInfo.Arguments = text;
			process.StartInfo.EnvironmentVariables["MONO_PATH"] = assembliesAbsoluteDirectory;
			process.StartInfo.EnvironmentVariables["GAC_PATH"] = assembliesAbsoluteDirectory;
			process.StartInfo.EnvironmentVariables["GC_DONT_GC"] = "yes please";
			if ((crossCompileOptions & CrossCompileOptions.ExplicitNullChecks) != CrossCompileOptions.Dynamic)
			{
				process.StartInfo.EnvironmentVariables["MONO_DEBUG"] = "explicit-null-checks";
			}
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.RedirectStandardOutput = true;
			if (MonoCrossCompile.ArtifactsPath != null)
			{
				if (!Directory.Exists(MonoCrossCompile.ArtifactsPath))
				{
					Directory.CreateDirectory(MonoCrossCompile.ArtifactsPath);
				}
				File.AppendAllText(MonoCrossCompile.ArtifactsPath + "output.txt", process.StartInfo.FileName + "\n");
				File.AppendAllText(MonoCrossCompile.ArtifactsPath + "output.txt", process.StartInfo.Arguments + "\n");
				File.AppendAllText(MonoCrossCompile.ArtifactsPath + "output.txt", assembliesAbsoluteDirectory + "\n");
				File.AppendAllText(MonoCrossCompile.ArtifactsPath + "output.txt", text2 + "\n");
				File.AppendAllText(MonoCrossCompile.ArtifactsPath + "output.txt", input + "\n");
				File.AppendAllText(MonoCrossCompile.ArtifactsPath + "houtput.txt", fileName + "\n\n");
				File.Copy(assembliesAbsoluteDirectory + "\\" + input, MonoCrossCompile.ArtifactsPath + "\\" + input, true);
			}
			process.StartInfo.WorkingDirectory = assembliesAbsoluteDirectory;
			MonoProcessUtility.RunMonoProcess(process, "AOT cross compiler", text2);
			File.Move(text2, output);
			if ((crossCompileOptions & CrossCompileOptions.Static) == CrossCompileOptions.Dynamic)
			{
				string text4 = Path.Combine(assembliesAbsoluteDirectory, fileName + ".def");
				if (File.Exists(text4))
				{
					File.Move(text4, output + ".def");
				}
			}
		}
	}
}
