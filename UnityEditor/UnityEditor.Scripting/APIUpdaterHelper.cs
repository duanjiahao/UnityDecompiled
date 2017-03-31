using ICSharpCode.NRefactory;
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
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.Scripting
{
	internal class APIUpdaterHelper
	{
		private static string[] _ignoredAssemblies = new string[]
		{
			"^UnityScript$",
			"^System\\..*",
			"^mscorlib$"
		};

		public static bool IsReferenceToMissingObsoleteMember(string namespaceName, string className)
		{
			bool result;
			try
			{
				Type type = APIUpdaterHelper.FindTypeInLoadedAssemblies((Type t) => t.Name == className && t.Namespace == namespaceName && APIUpdaterHelper.IsUpdateable(t));
				result = (type != null);
			}
			catch (ReflectionTypeLoadException ex)
			{
				throw new Exception(ex.Message + ex.LoaderExceptions.Aggregate("", (string acc, Exception curr) => acc + "\r\n\t" + curr.Message));
			}
			return result;
		}

		public static bool IsReferenceToTypeWithChangedNamespace(string normalizedErrorMessage)
		{
			bool result;
			try
			{
				string[] lines = normalizedErrorMessage.Split(new char[]
				{
					'\n'
				});
				string valueFromNormalizedMessage = APIUpdaterHelper.GetValueFromNormalizedMessage(lines, "EntityName=");
				Type type = APIUpdaterHelper.FindExactTypeMatchingMovedType(valueFromNormalizedMessage) ?? APIUpdaterHelper.FindTypeMatchingMovedTypeBasedOnNamespaceFromError(lines);
				result = (type != null);
			}
			catch (ReflectionTypeLoadException ex)
			{
				throw new Exception(ex.Message + ex.LoaderExceptions.Aggregate("", (string acc, Exception curr) => acc + "\r\n\t" + curr.Message));
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
					if (APIUpdaterHelper.IsError(num))
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

		private static bool IsError(int exitCode)
		{
			return (exitCode & 128) != 0;
		}

		private static string ResolveAssemblyPath(string assemblyPath)
		{
			return CommandLineFormatter.PrepareFileName(assemblyPath);
		}

		public static bool DoesAssemblyRequireUpgrade(string assemblyFullPath)
		{
			bool result;
			if (!File.Exists(assemblyFullPath))
			{
				result = false;
			}
			else if (!AssemblyHelper.IsManagedAssembly(assemblyFullPath))
			{
				result = false;
			}
			else if (!APIUpdaterHelper.MayContainUpdatableReferences(assemblyFullPath))
			{
				result = false;
			}
			else
			{
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
					result = false;
					break;
				case 2:
					result = true;
					break;
				default:
					UnityEngine.Debug.LogError(text + Environment.NewLine + text2);
					result = false;
					break;
				}
			}
			return result;
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
			ManagedProgram managedProgram = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), null, executable2, arguments, false, null);
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
			bool result;
			if (customAttributes.Length != 1)
			{
				result = false;
			}
			else
			{
				ObsoleteAttribute obsoleteAttribute = (ObsoleteAttribute)customAttributes[0];
				result = obsoleteAttribute.Message.Contains("UnityUpgradable");
			}
			return result;
		}

		private static bool NamespaceHasChanged(Type type, string namespaceName)
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(MovedFromAttribute), false);
			bool result;
			if (customAttributes.Length != 1)
			{
				result = false;
			}
			else if (string.IsNullOrEmpty(namespaceName))
			{
				result = true;
			}
			else
			{
				MovedFromAttribute movedFromAttribute = (MovedFromAttribute)customAttributes[0];
				result = (movedFromAttribute.Namespace == namespaceName);
			}
			return result;
		}

		private static Type FindTypeInLoadedAssemblies(Func<Type, bool> predicate)
		{
			return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
			where !APIUpdaterHelper.IsIgnoredAssembly(assembly.GetName())
			select assembly).SelectMany((Assembly a) => APIUpdaterHelper.GetValidTypesIn(a)).FirstOrDefault(predicate);
		}

		private static IEnumerable<Type> GetValidTypesIn(Assembly a)
		{
			Type[] types;
			try
			{
				types = a.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				types = ex.Types;
			}
			return from t in types
			where t != null
			select t;
		}

		private static bool IsIgnoredAssembly(AssemblyName assemblyName)
		{
			string name = assemblyName.Name;
			return APIUpdaterHelper._ignoredAssemblies.Any((string candidate) => Regex.IsMatch(name, candidate));
		}

		internal static bool MayContainUpdatableReferences(string assemblyPath)
		{
			bool result;
			using (FileStream fileStream = File.Open(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(fileStream);
				if (assemblyDefinition.Name.IsWindowsRuntime)
				{
					result = false;
					return result;
				}
				if (!APIUpdaterHelper.IsTargetFrameworkValidOnCurrentOS(assemblyDefinition))
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}

		private static bool IsTargetFrameworkValidOnCurrentOS(AssemblyDefinition assembly)
		{
			bool arg_4D_0;
			if (Environment.OSVersion.Platform != PlatformID.Win32NT)
			{
				int arg_48_0;
				if (assembly.HasCustomAttributes)
				{
					arg_48_0 = (assembly.CustomAttributes.Any((CustomAttribute attr) => APIUpdaterHelper.TargetsWindowsSpecificFramework(attr)) ? 1 : 0);
				}
				else
				{
					arg_48_0 = 0;
				}
				arg_4D_0 = (arg_48_0 == 0);
			}
			else
			{
				arg_4D_0 = true;
			}
			return arg_4D_0;
		}

		private static bool TargetsWindowsSpecificFramework(CustomAttribute targetFrameworkAttr)
		{
			bool result;
			if (!targetFrameworkAttr.AttributeType.FullName.Contains("System.Runtime.Versioning.TargetFrameworkAttribute"))
			{
				result = false;
			}
			else
			{
				Regex regex = new Regex("\\.NETCore|\\.NETPortable");
				bool flag = targetFrameworkAttr.ConstructorArguments.Any((CustomAttributeArgument arg) => arg.Type.FullName == typeof(string).FullName && regex.IsMatch((string)arg.Value));
				result = flag;
			}
			return result;
		}

		private static Type FindExactTypeMatchingMovedType(string simpleOrQualifiedName)
		{
			Match match = Regex.Match(simpleOrQualifiedName, "^(?:(?<namespace>.*)(?=\\.)\\.)?(?<typename>[a-zA-Z_0-9]+)$");
			Type result;
			if (!match.Success)
			{
				result = null;
			}
			else
			{
				string typename = match.Groups["typename"].Value;
				string namespaceName = match.Groups["namespace"].Value;
				result = APIUpdaterHelper.FindTypeInLoadedAssemblies((Type t) => t.Name == typename && APIUpdaterHelper.NamespaceHasChanged(t, namespaceName));
			}
			return result;
		}

		private static Type FindTypeMatchingMovedTypeBasedOnNamespaceFromError(IEnumerable<string> lines)
		{
			string valueFromNormalizedMessage = APIUpdaterHelper.GetValueFromNormalizedMessage(lines, "Line=");
			int num = (valueFromNormalizedMessage == null) ? -1 : int.Parse(valueFromNormalizedMessage);
			valueFromNormalizedMessage = APIUpdaterHelper.GetValueFromNormalizedMessage(lines, "Column=");
			int num2 = (valueFromNormalizedMessage == null) ? -1 : int.Parse(valueFromNormalizedMessage);
			string valueFromNormalizedMessage2 = APIUpdaterHelper.GetValueFromNormalizedMessage(lines, "Script=");
			Type result;
			if (num == -1 || num2 == -1 || valueFromNormalizedMessage2 == null)
			{
				result = null;
			}
			else
			{
				using (FileStream fileStream = File.Open(valueFromNormalizedMessage2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					IParser parser = ParserFactory.CreateParser(ICSharpCode.NRefactory.SupportedLanguage.CSharp, new StreamReader(fileStream));
					parser.Lexer.EvaluateConditionalCompilation = false;
					parser.Parse();
					string text = InvalidTypeOrNamespaceErrorTypeMapper.IsTypeMovedToNamespaceError(parser.CompilationUnit, num, num2);
					if (text == null)
					{
						result = null;
					}
					else
					{
						result = APIUpdaterHelper.FindExactTypeMatchingMovedType(text);
					}
				}
			}
			return result;
		}

		private static string GetValueFromNormalizedMessage(IEnumerable<string> lines, string marker)
		{
			string result = null;
			string text = lines.FirstOrDefault((string l) => l.StartsWith(marker));
			if (text != null)
			{
				result = text.Substring(marker.Length).Trim();
			}
			return result;
		}
	}
}
