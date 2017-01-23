using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEditor.Utils;

namespace UnityEditorInternal
{
	internal class AssemblyStripper
	{
		[CompilerGenerated]
		private static Func<string, bool> <>f__mg$cache0;

		private static string[] Il2CppBlacklistPaths
		{
			get
			{
				return new string[]
				{
					Path.Combine("..", "platform_native_link.xml")
				};
			}
		}

		private static string MonoLinkerPath
		{
			get
			{
				return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/UnusedBytecodeStripper.exe");
			}
		}

		private static string ModulesWhitelistPath
		{
			get
			{
				return Path.Combine(Path.GetDirectoryName(AssemblyStripper.MonoLinkerPath), "ModuleStrippingInformation");
			}
		}

		private static string MonoLinker2Path
		{
			get
			{
				return Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/UnusedByteCodeStripper2/UnusedBytecodeStripper2.exe");
			}
		}

		private static string BlacklistPath
		{
			get
			{
				return Path.Combine(Path.GetDirectoryName(AssemblyStripper.MonoLinkerPath), "Core.xml");
			}
		}

		private static string GetModuleWhitelist(string module, string moduleStrippingInformationFolder)
		{
			return Paths.Combine(new string[]
			{
				moduleStrippingInformationFolder,
				module + ".xml"
			});
		}

		private static bool StripAssembliesTo(string[] assemblies, string[] searchDirs, string outputFolder, string workingDirectory, out string output, out string error, string linkerPath, IIl2CppPlatformProvider platformProvider, IEnumerable<string> additionalBlacklist, bool developmentBuild)
		{
			if (!Directory.Exists(outputFolder))
			{
				Directory.CreateDirectory(outputFolder);
			}
			IEnumerable<string> arg_50_0 = from s in additionalBlacklist
			select (!Path.IsPathRooted(s)) ? Path.Combine(workingDirectory, s) : s;
			if (AssemblyStripper.<>f__mg$cache0 == null)
			{
				AssemblyStripper.<>f__mg$cache0 = new Func<string, bool>(File.Exists);
			}
			additionalBlacklist = arg_50_0.Where(AssemblyStripper.<>f__mg$cache0);
			IEnumerable<string> userBlacklistFiles = AssemblyStripper.GetUserBlacklistFiles();
			foreach (string current in userBlacklistFiles)
			{
				Console.WriteLine("UserBlackList: " + current);
			}
			additionalBlacklist = additionalBlacklist.Concat(userBlacklistFiles);
			List<string> list = new List<string>
			{
				"--api " + PlayerSettings.apiCompatibilityLevel.ToString(),
				"-out \"" + outputFolder + "\"",
				"-l none",
				"-c link",
				"-b " + developmentBuild,
				"-x \"" + AssemblyStripper.GetModuleWhitelist("Core", platformProvider.moduleStrippingInformationFolder) + "\"",
				"-f \"" + Path.Combine(platformProvider.il2CppFolder, "LinkerDescriptors") + "\""
			};
			list.AddRange(from path in additionalBlacklist
			select "-x \"" + path + "\"");
			list.AddRange(from d in searchDirs
			select "-d \"" + d + "\"");
			list.AddRange(from assembly in assemblies
			select "-a  \"" + Path.GetFullPath(assembly) + "\"");
			return AssemblyStripper.RunAssemblyLinker(list, out output, out error, linkerPath, workingDirectory);
		}

		private static bool RunAssemblyLinker(IEnumerable<string> args, out string @out, out string err, string linkerPath, string workingDirectory)
		{
			string text = args.Aggregate((string buff, string s) => buff + " " + s);
			Console.WriteLine("Invoking UnusedByteCodeStripper2 with arguments: " + text);
			Runner.RunManagedProgram(linkerPath, text, workingDirectory, null, null);
			@out = "";
			err = "";
			return true;
		}

		private static List<string> GetUserAssemblies(RuntimeClassRegistry rcr, string managedDir)
		{
			return (from s in rcr.GetUserAssemblies()
			where rcr.IsDLLUsed(s)
			select Path.Combine(managedDir, s)).ToList<string>();
		}

		internal static void StripAssemblies(string stagingAreaData, IIl2CppPlatformProvider platformProvider, RuntimeClassRegistry rcr, bool developmentBuild)
		{
			string fullPath = Path.GetFullPath(Path.Combine(stagingAreaData, "Managed"));
			List<string> userAssemblies = AssemblyStripper.GetUserAssemblies(rcr, fullPath);
			string[] assembliesToStrip = userAssemblies.ToArray();
			string[] searchDirs = new string[]
			{
				fullPath
			};
			AssemblyStripper.RunAssemblyStripper(stagingAreaData, userAssemblies, fullPath, assembliesToStrip, searchDirs, AssemblyStripper.MonoLinker2Path, platformProvider, rcr, developmentBuild);
		}

		internal static void GenerateInternalCallSummaryFile(string icallSummaryPath, string managedAssemblyFolderPath, string strippedDLLPath)
		{
			string exe = Path.Combine(MonoInstallationFinder.GetFrameWorksFolder(), "Tools/InternalCallRegistrationWriter/InternalCallRegistrationWriter.exe");
			string args = string.Format("-assembly=\"{0}\" -output=\"{1}\" -summary=\"{2}\"", Path.Combine(strippedDLLPath, "UnityEngine.dll"), Path.Combine(managedAssemblyFolderPath, "UnityICallRegistration.cpp"), icallSummaryPath);
			Runner.RunManagedProgram(exe, args);
		}

		internal static IEnumerable<string> GetUserBlacklistFiles()
		{
			return from s in Directory.GetFiles("Assets", "link.xml", SearchOption.AllDirectories)
			select Path.Combine(Directory.GetCurrentDirectory(), s);
		}

		private static bool AddWhiteListsForModules(IEnumerable<string> nativeModules, ref IEnumerable<string> blacklists, string moduleStrippingInformationFolder)
		{
			bool result = false;
			foreach (string current in nativeModules)
			{
				string moduleWhitelist = AssemblyStripper.GetModuleWhitelist(current, moduleStrippingInformationFolder);
				if (File.Exists(moduleWhitelist))
				{
					if (!blacklists.Contains(moduleWhitelist))
					{
						blacklists = blacklists.Concat(new string[]
						{
							moduleWhitelist
						});
						result = true;
					}
				}
			}
			return result;
		}

		private static void RunAssemblyStripper(string stagingAreaData, IEnumerable assemblies, string managedAssemblyFolderPath, string[] assembliesToStrip, string[] searchDirs, string monoLinkerPath, IIl2CppPlatformProvider platformProvider, RuntimeClassRegistry rcr, bool developmentBuild)
		{
			bool flag = rcr != null && PlayerSettings.stripEngineCode && platformProvider.supportsEngineStripping;
			IEnumerable<string> enumerable = AssemblyStripper.Il2CppBlacklistPaths;
			if (rcr != null)
			{
				enumerable = enumerable.Concat(new string[]
				{
					AssemblyStripper.WriteMethodsToPreserveBlackList(stagingAreaData, rcr),
					MonoAssemblyStripping.GenerateLinkXmlToPreserveDerivedTypes(stagingAreaData, managedAssemblyFolderPath, rcr)
				});
			}
			if (!flag)
			{
				string[] files = Directory.GetFiles(platformProvider.moduleStrippingInformationFolder, "*.xml");
				for (int i = 0; i < files.Length; i++)
				{
					string text = files[i];
					enumerable = enumerable.Concat(new string[]
					{
						text
					});
				}
			}
			string fullPath = Path.GetFullPath(Path.Combine(managedAssemblyFolderPath, "tempStrip"));
			string text3;
			while (true)
			{
				bool flag2 = false;
				if (EditorUtility.DisplayCancelableProgressBar("Building Player", "Stripping assemblies", 0f))
				{
					break;
				}
				string text2;
				if (!AssemblyStripper.StripAssembliesTo(assembliesToStrip, searchDirs, fullPath, managedAssemblyFolderPath, out text2, out text3, monoLinkerPath, platformProvider, enumerable, developmentBuild))
				{
					goto Block_6;
				}
				string text4 = Path.Combine(managedAssemblyFolderPath, "ICallSummary.txt");
				AssemblyStripper.GenerateInternalCallSummaryFile(text4, managedAssemblyFolderPath, fullPath);
				if (flag)
				{
					HashSet<UnityType> hashSet;
					HashSet<string> nativeModules;
					CodeStrippingUtils.GenerateDependencies(fullPath, text4, rcr, flag, out hashSet, out nativeModules, platformProvider);
					flag2 = AssemblyStripper.AddWhiteListsForModules(nativeModules, ref enumerable, platformProvider.moduleStrippingInformationFolder);
				}
				if (!flag2)
				{
					goto Block_8;
				}
			}
			throw new OperationCanceledException();
			Block_6:
			throw new Exception(string.Concat(new object[]
			{
				"Error in stripping assemblies: ",
				assemblies,
				", ",
				text3
			}));
			Block_8:
			string fullPath2 = Path.GetFullPath(Path.Combine(managedAssemblyFolderPath, "tempUnstripped"));
			Directory.CreateDirectory(fullPath2);
			string[] files2 = Directory.GetFiles(managedAssemblyFolderPath);
			for (int j = 0; j < files2.Length; j++)
			{
				string text5 = files2[j];
				string extension = Path.GetExtension(text5);
				if (string.Equals(extension, ".dll", StringComparison.InvariantCultureIgnoreCase) || string.Equals(extension, ".mdb", StringComparison.InvariantCultureIgnoreCase))
				{
					File.Move(text5, Path.Combine(fullPath2, Path.GetFileName(text5)));
				}
			}
			string[] files3 = Directory.GetFiles(fullPath);
			for (int k = 0; k < files3.Length; k++)
			{
				string text6 = files3[k];
				File.Move(text6, Path.Combine(managedAssemblyFolderPath, Path.GetFileName(text6)));
			}
			Directory.Delete(fullPath);
		}

		private static string WriteMethodsToPreserveBlackList(string stagingAreaData, RuntimeClassRegistry rcr)
		{
			string text = (!Path.IsPathRooted(stagingAreaData)) ? (Directory.GetCurrentDirectory() + "/") : "";
			text = text + stagingAreaData + "/methods_pointedto_by_uievents.xml";
			File.WriteAllText(text, AssemblyStripper.GetMethodPreserveBlacklistContents(rcr));
			return text;
		}

		private static string GetMethodPreserveBlacklistContents(RuntimeClassRegistry rcr)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<linker>");
			IEnumerable<IGrouping<string, RuntimeClassRegistry.MethodDescription>> enumerable = from m in rcr.GetMethodsToPreserve()
			group m by m.assembly;
			foreach (IGrouping<string, RuntimeClassRegistry.MethodDescription> current in enumerable)
			{
				stringBuilder.AppendLine(string.Format("\t<assembly fullname=\"{0}\">", current.Key));
				IEnumerable<IGrouping<string, RuntimeClassRegistry.MethodDescription>> enumerable2 = from m in current
				group m by m.fullTypeName;
				foreach (IGrouping<string, RuntimeClassRegistry.MethodDescription> current2 in enumerable2)
				{
					stringBuilder.AppendLine(string.Format("\t\t<type fullname=\"{0}\">", current2.Key));
					foreach (RuntimeClassRegistry.MethodDescription current3 in current2)
					{
						stringBuilder.AppendLine(string.Format("\t\t\t<method name=\"{0}\"/>", current3.methodName));
					}
					stringBuilder.AppendLine("\t\t</type>");
				}
				stringBuilder.AppendLine("\t</assembly>");
			}
			stringBuilder.AppendLine("</linker>");
			return stringBuilder.ToString();
		}
	}
}
