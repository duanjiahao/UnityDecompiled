using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
				Process process = MonoProcessUtility.PrepareMonoProcessBleedingEdge(managedLibrariesDirectory);
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
	}
}
