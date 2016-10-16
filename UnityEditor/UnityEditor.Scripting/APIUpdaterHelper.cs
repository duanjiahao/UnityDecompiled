using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Unity.DataContract;
using UnityEditor.Modules;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Utils;
using UnityEngine;

namespace UnityEditor.Scripting
{
	internal class APIUpdaterHelper
	{
		public static bool IsReferenceToMissingObsoleteMember(string namespaceName, string className)
		{
			bool result;
			try
			{
				Type type = AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly a) => a.GetTypes()).FirstOrDefault((Type t) => t.Name == className && t.Namespace == namespaceName && APIUpdaterHelper.IsUpdateable(t));
				result = (type != null);
			}
			catch (ReflectionTypeLoadException ex)
			{
				throw new Exception(ex.Message + ex.LoaderExceptions.Aggregate(string.Empty, (string acc, Exception curr) => acc + "\r\n\t" + curr.Message));
			}
			return result;
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
				if (AssemblyHelper.IsManagedAssembly(text))
				{
					string text2 = APIUpdaterHelper.ResolveAssemblyPath(text);
					string text3;
					string text4;
					int num = APIUpdaterHelper.RunUpdatingProgram("AssemblyUpdater.exe", string.Concat(new string[]
					{
						"-u -a ",
						text2,
						APIUpdaterHelper.APIVersionArgument(),
						APIUpdaterHelper.AssemblySearchPathArgument(),
						APIUpdaterHelper.ConfigurationProviderAssembliesPathArgument()
					}), out text3, out text4);
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

		public static bool DoesAssemblyRequireUpgrade(string assemblyFullPath)
		{
			if (!File.Exists(assemblyFullPath))
			{
				return false;
			}
			if (!AssemblyHelper.IsManagedAssembly(assemblyFullPath))
			{
				return false;
			}
			if (!APIUpdaterHelper.MayContainUpdatableReferences(assemblyFullPath))
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
				CommandLineFormatter.PrepareFileName(assemblyFullPath),
				APIUpdaterHelper.AssemblySearchPathArgument(),
				APIUpdaterHelper.ConfigurationProviderAssembliesPathArgument()
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
			string str = string.Concat(new string[]
			{
				Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Managed"),
				",+",
				Path.Combine(EditorApplication.applicationContentsPath, "UnityExtensions/Unity"),
				",+",
				Application.dataPath
			});
			return " -s \"" + str + "\"";
		}

		private static string ConfigurationProviderAssembliesPathArgument()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Unity.DataContract.PackageInfo current in ModuleManager.packageManager.unityExtensions)
			{
				foreach (string current2 in from f in current.files
				where f.Value.type == PackageFileType.Dll
				select f into pi
				select pi.Key)
				{
					stringBuilder.AppendFormat(" {0}", CommandLineFormatter.PrepareFileName(Path.Combine(current.basePath, current2)));
				}
			}
			string unityEditorManagedPath = APIUpdaterHelper.GetUnityEditorManagedPath();
			stringBuilder.AppendFormat(" {0}", CommandLineFormatter.PrepareFileName(Path.Combine(unityEditorManagedPath, "UnityEngine.dll")));
			stringBuilder.AppendFormat(" {0}", CommandLineFormatter.PrepareFileName(Path.Combine(unityEditorManagedPath, "UnityEditor.dll")));
			return stringBuilder.ToString();
		}

		private static string GetUnityEditorManagedPath()
		{
			return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Managed");
		}

		private static int RunUpdatingProgram(string executable, string arguments, out string stdOut, out string stdErr)
		{
			string executable2 = EditorApplication.applicationContentsPath + "/Tools/ScriptUpdater/" + executable;
			ManagedProgram managedProgram = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.0", executable2, arguments, null);
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

		internal static bool MayContainUpdatableReferences(string assemblyPath)
		{
			using (FileStream fileStream = File.Open(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(fileStream);
				if (assemblyDefinition.Name.IsWindowsRuntime)
				{
					bool result = false;
					return result;
				}
				if (!APIUpdaterHelper.IsTargetFrameworkValidOnCurrentOS(assemblyDefinition))
				{
					bool result = false;
					return result;
				}
			}
			return true;
		}

		private static bool IsTargetFrameworkValidOnCurrentOS(AssemblyDefinition assembly)
		{
			bool arg_4C_0;
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				int arg_47_0;
				if (assembly.HasCustomAttributes)
				{
					arg_47_0 = (assembly.CustomAttributes.Any((CustomAttribute attr) => APIUpdaterHelper.TargetsWindowsSpecificFramework(attr)) ? 1 : 0);
				}
				else
				{
					arg_47_0 = 0;
				}
				arg_4C_0 = (arg_47_0 == 0);
			}
			else
			{
				arg_4C_0 = true;
			}
			return arg_4C_0;
		}

		private static bool TargetsWindowsSpecificFramework(CustomAttribute targetFrameworkAttr)
		{
			if (!targetFrameworkAttr.AttributeType.FullName.Contains("System.Runtime.Versioning.TargetFrameworkAttribute"))
			{
				return false;
			}
			Regex regex = new Regex("\\.NETCore|\\.NETPortable");
			return targetFrameworkAttr.ConstructorArguments.Any((CustomAttributeArgument arg) => arg.Type.FullName == typeof(string).FullName && regex.IsMatch((string)arg.Value));
		}
	}
}
