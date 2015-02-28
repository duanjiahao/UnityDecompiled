using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor.Scripting
{
	internal class APIUpdaterHelper
	{
		public static bool IsReferenceToMissingObsoleteMember(string namespaceName, string className)
		{
			Type type = AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly a) => a.GetTypes()).FirstOrDefault((Type t) => t.Name == className && t.Namespace == namespaceName && APIUpdaterHelper.IsUpdateable(t));
			return type != null;
		}
		public static void Run(string commaSeparatedListOfAssemblies)
		{
			string[] array = commaSeparatedListOfAssemblies.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			APIUpdaterLogger.WriteToFile("Started to update {0} assemblie(s)", new object[]
			{
				array.Count<string>()
			});
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (InternalEditorUtility.DetectDotNetDll(text))
				{
					string text2 = APIUpdaterHelper.ResolveAssemblyPath(text);
					string text3;
					string text4;
					int num = APIUpdaterHelper.RunUpdatingProgram("AssemblyUpdater.exe", "-u -a " + text2 + APIUpdaterHelper.APIVersionArgument() + APIUpdaterHelper.AssemblySearchPathArgument(), out text3, out text4);
					if (text3.Length > 0)
					{
						APIUpdaterLogger.WriteToFile("Assembly update output ({0})\r\n{1}", new object[]
						{
							text2,
							text3
						});
					}
					if (num < 0)
					{
						APIUpdaterLogger.WriteErrorToConsole("Error {0} running AssemblyUpdater. Its output is: `{1}`", new object[]
						{
							num,
							text4
						});
					}
				}
			}
			APIUpdaterLogger.WriteToFile("Update finished in {0}s", new object[]
			{
				stopwatch.Elapsed.TotalSeconds
			});
		}
		private static string ResolveAssemblyPath(string assemblyPath)
		{
			return CommandLineFormatter.PrepareFileName(assemblyPath);
		}
		public static bool DoesAssemblyRequireUpgrade(string assetFullPath)
		{
			if (!File.Exists(assetFullPath))
			{
				return false;
			}
			if (!InternalEditorUtility.DetectDotNetDll(assetFullPath))
			{
				return false;
			}
			string text;
			string text2;
			int num = APIUpdaterHelper.RunUpdatingProgram("AssemblyUpdater.exe", string.Concat(new string[]
			{
				APIUpdaterHelper.TimeStampArgument(),
				APIUpdaterHelper.APIVersionArgument(),
				"--check-update-required -a ",
				CommandLineFormatter.PrepareFileName(assetFullPath),
				APIUpdaterHelper.AssemblySearchPathArgument()
			}), out text, out text2);
			Console.WriteLine("{0}{1}", text, text2);
			switch (num)
			{
			case 0:
			case 1:
				return false;
			case 2:
				return true;
			default:
				UnityEngine.Debug.LogError(text + Environment.NewLine + text2);
				return false;
			}
		}
		private static string AssemblySearchPathArgument()
		{
			return " -s " + CommandLineFormatter.PrepareFileName(APIUpdaterHelper.GetUnityEngineDLLPath()) + ",+" + CommandLineFormatter.PrepareFileName(Application.dataPath);
		}
		private static string GetUnityEngineDLLPath()
		{
			return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Managed");
		}
		private static int RunUpdatingProgram(string executable, string arguments, out string stdOut, out string stdErr)
		{
			string executable2 = EditorApplication.applicationContentsPath + "/Tools/ScriptUpdater/" + executable;
			ManagedProgram managedProgram = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", executable2, arguments);
			managedProgram.LogProcessStartInfo();
			managedProgram.Start();
			managedProgram.WaitForExit();
			stdOut = managedProgram.GetStandardOutputAsString();
			stdErr = string.Join("\r\n", managedProgram.GetErrorOutput());
			return managedProgram.ExitCode;
		}
		private static string APIVersionArgument()
		{
			return " --api-version " + Application.unityVersion + " ";
		}
		private static string TimeStampArgument()
		{
			return " --timestamp " + DateTime.Now.Ticks + " ";
		}
		private static bool IsUpdateable(Type type)
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(ObsoleteAttribute), false);
			if (customAttributes.Length != 1)
			{
				return false;
			}
			ObsoleteAttribute obsoleteAttribute = (ObsoleteAttribute)customAttributes[0];
			return obsoleteAttribute.Message.Contains("UnityUpgradable");
		}
	}
}
