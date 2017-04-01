using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor.Utils;

namespace UnityEditor
{
	internal class MonoAssemblyStripping
	{
		private class AssemblyDefinitionComparer : IEqualityComparer<AssemblyDefinition>
		{
			public bool Equals(AssemblyDefinition x, AssemblyDefinition y)
			{
				return x.FullName == y.FullName;
			}

			public int GetHashCode(AssemblyDefinition obj)
			{
				return obj.FullName.GetHashCode();
			}
		}

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
				Process process = MonoProcessUtility.PrepareMonoProcess(managedLibrariesDirectory);
				string text2 = text + ".out";
				process.StartInfo.Arguments = "\"" + str + "\"";
				ProcessStartInfo expr_5D = process.StartInfo;
				string arguments = expr_5D.Arguments;
				expr_5D.Arguments = string.Concat(new string[]
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
					if (assemblyDefinition.Name.Name.StartsWith("UnityEngine."))
					{
						foreach (string current2 in usedClasses.GetAllManagedClassesAsString())
						{
							textWriter.WriteLine(string.Format("<type fullname=\"UnityEngine.{0}\" preserve=\"{1}\"/>", current2, usedClasses.GetRetentionLevel(current2)));
						}
					}
					MonoAssemblyStripping.GenerateBlackListTypeXML(textWriter, assemblyDefinition.MainModule.Types, usedClasses.GetAllManagedBaseClassesAsString());
					textWriter.WriteLine("</assembly>");
				}
				textWriter.WriteLine("</linker>");
			}
			return text;
		}

		public static string GenerateLinkXmlToPreserveDerivedTypes(string stagingArea, string librariesFolder, RuntimeClassRegistry usedClasses)
		{
			string fullPath = Path.GetFullPath(Path.Combine(stagingArea, "preserved_derived_types.xml"));
			using (TextWriter textWriter = new StreamWriter(fullPath))
			{
				textWriter.WriteLine("<linker>");
				foreach (AssemblyDefinition current in MonoAssemblyStripping.CollectAllAssemblies(librariesFolder, usedClasses))
				{
					if (!(current.Name.Name == "UnityEngine"))
					{
						HashSet<TypeDefinition> hashSet = new HashSet<TypeDefinition>();
						MonoAssemblyStripping.CollectBlackListTypes(hashSet, current.MainModule.Types, usedClasses.GetAllManagedBaseClassesAsString());
						if (hashSet.Count != 0)
						{
							textWriter.WriteLine("<assembly fullname=\"{0}\">", current.Name.Name);
							foreach (TypeDefinition current2 in hashSet)
							{
								textWriter.WriteLine("<type fullname=\"{0}\" preserve=\"all\"/>", current2.FullName);
							}
							textWriter.WriteLine("</assembly>");
						}
					}
				}
				textWriter.WriteLine("</linker>");
			}
			return fullPath;
		}

		public static IEnumerable<AssemblyDefinition> CollectAllAssemblies(string librariesFolder, RuntimeClassRegistry usedClasses)
		{
			DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
			resolver.RemoveSearchDirectory(".");
			resolver.RemoveSearchDirectory("bin");
			resolver.AddSearchDirectory(librariesFolder);
			IEnumerable<AssemblyNameReference> source = from s in usedClasses.GetUserAssemblies()
			where usedClasses.IsDLLUsed(s)
			select s into file
			select AssemblyNameReference.Parse(Path.GetFileNameWithoutExtension(file));
			return MonoAssemblyStripping.CollectAssembliesRecursive(from dll in source
			select MonoAssemblyStripping.ResolveAssemblyReference(resolver, dll) into a
			where a != null
			select a);
		}

		private static HashSet<AssemblyDefinition> CollectAssembliesRecursive(IEnumerable<AssemblyDefinition> assemblies)
		{
			HashSet<AssemblyDefinition> hashSet = new HashSet<AssemblyDefinition>(assemblies, new MonoAssemblyStripping.AssemblyDefinitionComparer());
			int num = 0;
			while (hashSet.Count > num)
			{
				num = hashSet.Count;
				hashSet.UnionWith(hashSet.ToArray<AssemblyDefinition>().SelectMany((AssemblyDefinition a) => MonoAssemblyStripping.ResolveAssemblyReferences(a)));
			}
			return hashSet;
		}

		public static IEnumerable<AssemblyDefinition> ResolveAssemblyReferences(AssemblyDefinition assembly)
		{
			return MonoAssemblyStripping.ResolveAssemblyReferences(assembly.MainModule.AssemblyResolver, assembly.MainModule.AssemblyReferences);
		}

		public static IEnumerable<AssemblyDefinition> ResolveAssemblyReferences(IAssemblyResolver resolver, IEnumerable<AssemblyNameReference> assemblyReferences)
		{
			return from reference in assemblyReferences
			select MonoAssemblyStripping.ResolveAssemblyReference(resolver, reference) into a
			where a != null
			select a;
		}

		public static AssemblyDefinition ResolveAssemblyReference(IAssemblyResolver resolver, AssemblyNameReference assemblyName)
		{
			AssemblyDefinition result;
			try
			{
				result = resolver.Resolve(assemblyName, new ReaderParameters
				{
					AssemblyResolver = resolver,
					ApplyWindowsRuntimeProjections = true
				});
			}
			catch (AssemblyResolutionException ex)
			{
				if (!ex.AssemblyReference.IsWindowsRuntime)
				{
					throw;
				}
				result = null;
			}
			return result;
		}

		private static void CollectBlackListTypes(HashSet<TypeDefinition> typesToPreserve, IList<TypeDefinition> types, List<string> baseTypes)
		{
			if (types != null)
			{
				foreach (TypeDefinition current in types)
				{
					if (current != null)
					{
						foreach (string current2 in baseTypes)
						{
							if (MonoAssemblyStripping.DoesTypeEnheritFrom(current, current2))
							{
								typesToPreserve.Add(current);
								break;
							}
						}
						MonoAssemblyStripping.CollectBlackListTypes(typesToPreserve, current.NestedTypes, baseTypes);
					}
				}
			}
		}

		private static void GenerateBlackListTypeXML(TextWriter w, IList<TypeDefinition> types, List<string> baseTypes)
		{
			HashSet<TypeDefinition> hashSet = new HashSet<TypeDefinition>();
			MonoAssemblyStripping.CollectBlackListTypes(hashSet, types, baseTypes);
			foreach (TypeDefinition current in hashSet)
			{
				w.WriteLine("<type fullname=\"{0}\" preserve=\"all\"/>", current.FullName);
			}
		}

		private static bool DoesTypeEnheritFrom(TypeReference type, string typeName)
		{
			bool result;
			while (type != null)
			{
				if (type.FullName == typeName)
				{
					result = true;
				}
				else
				{
					TypeDefinition typeDefinition = type.Resolve();
					if (typeDefinition != null)
					{
						type = typeDefinition.BaseType;
						continue;
					}
					result = false;
				}
				return result;
			}
			result = false;
			return result;
		}

		private static string StripperExe()
		{
			return "Tools/UnusedBytecodeStripper.exe";
		}

		public static void MonoLink(BuildTarget buildTarget, string managedLibrariesDirectory, string[] input, string[] allAssemblies, RuntimeClassRegistry usedClasses)
		{
			Process process = MonoProcessUtility.PrepareMonoProcess(managedLibrariesDirectory);
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
				ProcessStartInfo expr_81 = process.StartInfo;
				expr_81.Arguments = expr_81.Arguments + " -a \"" + str + "\"";
			}
			ProcessStartInfo expr_B4 = process.StartInfo;
			string arguments = expr_B4.Arguments;
			expr_B4.Arguments = string.Concat(new string[]
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
				ProcessStartInfo expr_111 = process.StartInfo;
				expr_111.Arguments = expr_111.Arguments + " -x \"" + text5 + "\"";
			}
			string text6 = Path.Combine(Path.GetDirectoryName(text2), "Core.xml");
			if (File.Exists(text6))
			{
				ProcessStartInfo expr_152 = process.StartInfo;
				expr_152.Arguments = expr_152.Arguments + " -x \"" + text6 + "\"";
			}
			string[] files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "Assets"), "link.xml", SearchOption.AllDirectories);
			string[] array = files;
			for (int j = 0; j < array.Length; j++)
			{
				string str2 = array[j];
				ProcessStartInfo expr_1A4 = process.StartInfo;
				expr_1A4.Arguments = expr_1A4.Arguments + " -x \"" + str2 + "\"";
			}
			if (usedClasses != null)
			{
				text = MonoAssemblyStripping.GenerateBlackList(managedLibrariesDirectory, usedClasses, allAssemblies);
				ProcessStartInfo expr_1E9 = process.StartInfo;
				expr_1E9.Arguments = expr_1E9.Arguments + " -x \"" + text + "\"";
			}
			string path = Path.Combine(BuildPipeline.GetPlaybackEngineDirectory(EditorUserBuildSettings.activeBuildTarget, BuildOptions.None), "Whitelists");
			string[] files2 = Directory.GetFiles(path, "*.xml");
			for (int k = 0; k < files2.Length; k++)
			{
				string str3 = files2[k];
				ProcessStartInfo expr_240 = process.StartInfo;
				expr_240.Arguments = expr_240.Arguments + " -x \"" + str3 + "\"";
			}
			MonoProcessUtility.RunMonoProcess(process, "assemblies stripper", Path.Combine(text4, "mscorlib.dll"));
			MonoAssemblyStripping.DeleteAllDllsFrom(managedLibrariesDirectory);
			MonoAssemblyStripping.CopyAllDlls(managedLibrariesDirectory, text4);
			string[] files3 = Directory.GetFiles(managedLibrariesDirectory);
			for (int l = 0; l < files3.Length; l++)
			{
				string text7 = files3[l];
				if (text7.Contains(".mdb"))
				{
					string path2 = text7.Replace(".mdb", "");
					if (!File.Exists(path2))
					{
						FileUtil.DeleteFileOrDirectory(text7);
					}
				}
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
