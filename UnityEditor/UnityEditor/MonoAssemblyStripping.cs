using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor.Utils;
using UnityEngine;
namespace UnityEditor
{
	internal class MonoAssemblyStripping
	{
		private static void ReplaceFile(string src, string dst)
		{
			if (File.Exists(dst))
			{
				FileUtil.DeleteFileOrDirectory(dst);
			}
			FileUtil.CopyFileOrDirectory(src, dst);
		}
		public static void MonoCilStrip(BuildTarget buildTarget, string managedLibrariesDirectory, string[] fileNames)
		{
			string buildToolsDirectory = BuildPipeline.GetBuildToolsDirectory(buildTarget);
			string str = Path.Combine(buildToolsDirectory, "mono-cil-strip.exe");
			for (int i = 0; i < fileNames.Length; i++)
			{
				string text = fileNames[i];
				Process process = MonoProcessUtility.PrepareMonoProcess(buildTarget, managedLibrariesDirectory);
				string text2 = text + ".out";
				process.StartInfo.Arguments = "\"" + str + "\"";
				ProcessStartInfo expr_5B = process.StartInfo;
				string arguments = expr_5B.Arguments;
				expr_5B.Arguments = string.Concat(new string[]
				{
					arguments,
					" \"",
					text,
					"\" \"",
					text,
					".out\""
				});
				MonoProcessUtility.RunMonoProcess(process, "byte code stripper", Path.Combine(managedLibrariesDirectory, text2));
				MonoAssemblyStripping.ReplaceFile(managedLibrariesDirectory + "/" + text2, managedLibrariesDirectory + "/" + text);
				File.Delete(managedLibrariesDirectory + "/" + text2);
			}
		}
		public static string GenerateBlackList(string librariesFolder, RuntimeClassRegistry usedClasses, string[] allAssemblies)
		{
			string text = "tmplink.xml";
			usedClasses.SynchronizeClasses();
			using (TextWriter textWriter = new StreamWriter(Path.Combine(librariesFolder, text)))
			{
				textWriter.WriteLine("<linker>");
				textWriter.WriteLine("<assembly fullname=\"UnityEngine\">");
				foreach (string current in usedClasses.GetAllManagedClassesAsString())
				{
					textWriter.WriteLine(string.Format("<type fullname=\"UnityEngine.{0}\" preserve=\"{1}\"/>", current, usedClasses.GetRetentionLevel(current)));
				}
				textWriter.WriteLine("</assembly>");
				DefaultAssemblyResolver defaultAssemblyResolver = new DefaultAssemblyResolver();
				defaultAssemblyResolver.AddSearchDirectory(librariesFolder);
				for (int i = 0; i < allAssemblies.Length; i++)
				{
					string path = allAssemblies[i];
					AssemblyDefinition assemblyDefinition = defaultAssemblyResolver.Resolve(Path.GetFileNameWithoutExtension(path), new ReaderParameters
					{
						AssemblyResolver = defaultAssemblyResolver
					});
					textWriter.WriteLine("<assembly fullname=\"{0}\">", assemblyDefinition.Name.Name);
					MonoAssemblyStripping.GenerateBlackListTypeXML(textWriter, assemblyDefinition.MainModule.Types, usedClasses.GetAllManagedBaseClassesAsString());
					textWriter.WriteLine("</assembly>");
				}
				textWriter.WriteLine("</linker>");
			}
			return text;
		}
		private static void GenerateBlackListTypeXML(TextWriter w, IList<TypeDefinition> types, List<string> baseTypes)
		{
			if (types == null)
			{
				return;
			}
			foreach (TypeDefinition current in types)
			{
				if (current != null)
				{
					foreach (string current2 in baseTypes)
					{
						if (MonoAssemblyStripping.DoesTypeEnheritFrom(current, current2))
						{
							w.WriteLine("<type fullname=\"{0}\" preserve=\"all\"/>", current.FullName);
							break;
						}
					}
					MonoAssemblyStripping.GenerateBlackListTypeXML(w, current.NestedTypes, baseTypes);
				}
			}
		}
		private static bool DoesTypeEnheritFrom(TypeReference type, string typeName)
		{
			while (type != null)
			{
				if (type.FullName == typeName)
				{
					return true;
				}
				type = type.Resolve().BaseType;
			}
			return false;
		}
		private static string StripperExe()
		{
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.WindowsEditor)
			{
				return "Tools/UnusedByteCodeStripper/UnusedBytecodeStripper.exe";
			}
			return "Tools/UnusedBytecodeStripper.exe";
		}
		public static void MonoLink(BuildTarget buildTarget, string managedLibrariesDirectory, string[] input, string[] allAssemblies, RuntimeClassRegistry usedClasses)
		{
			Process process = MonoProcessUtility.PrepareMonoProcess(buildTarget, managedLibrariesDirectory);
			string buildToolsDirectory = BuildPipeline.GetBuildToolsDirectory(buildTarget);
			string text = null;
			string frameWorksFolder = MonoInstallationFinder.GetFrameWorksFolder();
			string text2 = Path.Combine(frameWorksFolder, MonoAssemblyStripping.StripperExe());
			string text3 = Path.Combine(Path.GetDirectoryName(text2), "link.xml");
			string text4 = Path.Combine(managedLibrariesDirectory, "output");
			Directory.CreateDirectory(text4);
			process.StartInfo.Arguments = "\"" + text2 + "\" -l none -c link";
			for (int i = 0; i < input.Length; i++)
			{
				string str = input[i];
				ProcessStartInfo expr_80 = process.StartInfo;
				expr_80.Arguments = expr_80.Arguments + " -a \"" + str + "\"";
			}
			ProcessStartInfo expr_B3 = process.StartInfo;
			string arguments = expr_B3.Arguments;
			expr_B3.Arguments = string.Concat(new string[]
			{
				arguments,
				" -out output -x \"",
				text3,
				"\" -d \"",
				managedLibrariesDirectory,
				"\""
			});
			string text5 = Path.Combine(buildToolsDirectory, "link.xml");
			if (File.Exists(text5))
			{
				ProcessStartInfo expr_110 = process.StartInfo;
				expr_110.Arguments = expr_110.Arguments + " -x \"" + text5 + "\"";
			}
			string[] files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "link.xml", SearchOption.AllDirectories);
			string[] array = files;
			for (int j = 0; j < array.Length; j++)
			{
				string str2 = array[j];
				ProcessStartInfo expr_161 = process.StartInfo;
				expr_161.Arguments = expr_161.Arguments + " -x \"" + str2 + "\"";
			}
			if (usedClasses != null)
			{
				text = MonoAssemblyStripping.GenerateBlackList(managedLibrariesDirectory, usedClasses, allAssemblies);
				ProcessStartInfo expr_1A5 = process.StartInfo;
				expr_1A5.Arguments = expr_1A5.Arguments + " -x \"" + text + "\"";
			}
			MonoProcessUtility.RunMonoProcess(process, "assemblies stripper", Path.Combine(text4, "mscorlib.dll"));
			if (buildTarget == BuildTarget.FlashPlayer)
			{
				IEnumerable<string> files2 = 
					from _ in input
					select Path.GetFileName(_);
				MonoAssemblyStripping.CopyFiles(files2, text4, managedLibrariesDirectory);
			}
			else
			{
				MonoAssemblyStripping.DeleteAllDllsFrom(managedLibrariesDirectory);
				MonoAssemblyStripping.CopyAllDlls(managedLibrariesDirectory, text4);
			}
			if (text != null)
			{
				FileUtil.DeleteFileOrDirectory(Path.Combine(managedLibrariesDirectory, text));
			}
			FileUtil.DeleteFileOrDirectory(text4);
		}
		private static void CopyFiles(IEnumerable<string> files, string fromDir, string toDir)
		{
			foreach (string current in files)
			{
				FileUtil.ReplaceFile(Path.Combine(fromDir, current), Path.Combine(toDir, current));
			}
		}
		private static void CopyAllDlls(string fromDir, string toDir)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(toDir);
			FileInfo[] files = directoryInfo.GetFiles("*.dll");
			FileInfo[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				FileInfo fileInfo = array[i];
				FileUtil.ReplaceFile(Path.Combine(toDir, fileInfo.Name), Path.Combine(fromDir, fileInfo.Name));
			}
		}
		private static void DeleteAllDllsFrom(string managedLibrariesDirectory)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(managedLibrariesDirectory);
			FileInfo[] files = directoryInfo.GetFiles("*.dll");
			FileInfo[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				FileInfo fileInfo = array[i];
				FileUtil.DeleteFileOrDirectory(fileInfo.FullName);
			}
		}
	}
}
