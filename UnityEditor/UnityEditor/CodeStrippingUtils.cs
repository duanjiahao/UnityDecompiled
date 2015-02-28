using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditorInternal;
namespace UnityEditor
{
	internal class CodeStrippingUtils
	{
		public static readonly string[] NativeClassBlackList = new string[]
		{
			"PreloadData",
			"Material",
			"Cubemap",
			"Texture3D",
			"RenderTexture"
		};
		private static readonly string[] s_UserAssemblies = new string[]
		{
			"Assembly-CSharp.dll",
			"Assembly-CSharp-firstpass.dll",
			"Assembly-UnityScript.dll",
			"Assembly-UnityScript-firstpass.dll"
		};
		public static void WriteModuleAndClassRegistrationFile(string strippedAssemblyDir, string outputDir, RuntimeClassRegistry rcr)
		{
			HashSet<string> hashSet = (PlayerSettings.strippingLevel != StrippingLevel.Disabled) ? CodeStrippingUtils.GenerateNativeClassList(rcr, strippedAssemblyDir, CodeStrippingUtils.GetUserAssemblies(strippedAssemblyDir)) : null;
			if (hashSet != null)
			{
				CodeStrippingUtils.ExcludeModuleManagers(ref hashSet);
			}
			HashSet<string> nativeModules = (hashSet != null) ? CodeStrippingUtils.GetRequiredStrippableModules(hashSet) : CodeStrippingUtils.GetAllStrippableModules();
			string file = Path.Combine(outputDir, "UnityClassRegistration.cpp");
			CodeStrippingUtils.WriteModuleAndClassRegistrationFile(file, nativeModules, hashSet);
		}
		private static HashSet<string> GetClassNames(IEnumerable<int> classIds)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (int current in classIds)
			{
				hashSet.Add(BaseObjectTools.ClassIDToString(current));
			}
			return hashSet;
		}
		private static HashSet<string> GetAllStrippableModules()
		{
			HashSet<string> hashSet = new HashSet<string>();
			string[] moduleNames = ModuleMetadata.GetModuleNames();
			for (int i = 0; i < moduleNames.Length; i++)
			{
				string text = moduleNames[i];
				if (ModuleMetadata.GetModuleStrippable(text))
				{
					hashSet.Add(text);
				}
			}
			return hashSet;
		}
		private static HashSet<string> GetRequiredStrippableModules(HashSet<string> nativeClasses)
		{
			HashSet<string> hashSet = new HashSet<string>();
			string[] moduleNames = ModuleMetadata.GetModuleNames();
			for (int i = 0; i < moduleNames.Length; i++)
			{
				string text = moduleNames[i];
				if (ModuleMetadata.GetModuleStrippable(text))
				{
					HashSet<string> classNames = CodeStrippingUtils.GetClassNames(ModuleMetadata.GetModuleClasses(text));
					if (nativeClasses.Overlaps(classNames))
					{
						hashSet.Add(text);
					}
				}
			}
			return hashSet;
		}
		private static void ExcludeModuleManagers(ref HashSet<string> nativeClasses)
		{
			string[] moduleNames = ModuleMetadata.GetModuleNames();
			int derivedFromClassID = BaseObjectTools.StringToClassID("GlobalGameManager");
			string[] array = moduleNames;
			for (int i = 0; i < array.Length; i++)
			{
				string moduleName = array[i];
				if (ModuleMetadata.GetModuleStrippable(moduleName))
				{
					int[] moduleClasses = ModuleMetadata.GetModuleClasses(moduleName);
					HashSet<int> hashSet = new HashSet<int>();
					HashSet<string> hashSet2 = new HashSet<string>();
					int[] array2 = moduleClasses;
					for (int j = 0; j < array2.Length; j++)
					{
						int num = array2[j];
						if (BaseObjectTools.IsDerivedFromClassID(num, derivedFromClassID))
						{
							hashSet.Add(num);
						}
						else
						{
							hashSet2.Add(BaseObjectTools.ClassIDToString(num));
						}
					}
					if (hashSet2.Count != 0)
					{
						if (!nativeClasses.Overlaps(hashSet2))
						{
							foreach (int current in hashSet)
							{
								nativeClasses.Remove(BaseObjectTools.ClassIDToString(current));
							}
						}
					}
				}
			}
		}
		private static HashSet<string> GenerateNativeClassList(RuntimeClassRegistry rcr, string directory, string[] rootAssemblies)
		{
			HashSet<string> hashSet = CodeStrippingUtils.CollectNativeClassListFromRoots(directory, rootAssemblies);
			string[] nativeClassBlackList = CodeStrippingUtils.NativeClassBlackList;
			for (int i = 0; i < nativeClassBlackList.Length; i++)
			{
				string item = nativeClassBlackList[i];
				hashSet.Add(item);
			}
			foreach (string current in rcr.GetAllNativeClassesIncludingManagersAsString())
			{
				int num = BaseObjectTools.StringToClassID(current);
				if (num != -1 && !BaseObjectTools.IsBaseObject(num))
				{
					hashSet.Add(current);
				}
			}
			return hashSet;
		}
		private static HashSet<string> CollectNativeClassListFromRoots(string directory, string[] rootAssemblies)
		{
			HashSet<string> hashSet = new HashSet<string>();
			HashSet<string> hashSet2 = CodeStrippingUtils.CollectManagedTypeReferencesFromRoots(directory, rootAssemblies);
			foreach (string current in hashSet2)
			{
				int num = BaseObjectTools.StringToClassID(current);
				if (num != -1 && !BaseObjectTools.IsBaseObject(num))
				{
					hashSet.Add(current);
				}
			}
			return hashSet;
		}
		private static HashSet<string> CollectManagedTypeReferencesFromRoots(string directory, string[] rootAssemblies)
		{
			HashSet<string> hashSet = new HashSet<string>();
			AssemblyReferenceChecker assemblyReferenceChecker = new AssemblyReferenceChecker();
			bool withMethods = false;
			bool ignoreSystemDlls = false;
			assemblyReferenceChecker.CollectReferencesFromRoots(directory, rootAssemblies, withMethods, 0f, ignoreSystemDlls);
			string[] assemblyFileNames = assemblyReferenceChecker.GetAssemblyFileNames();
			AssemblyDefinition[] assemblyDefinitions = assemblyReferenceChecker.GetAssemblyDefinitions();
			AssemblyDefinition[] array = assemblyDefinitions;
			for (int i = 0; i < array.Length; i++)
			{
				AssemblyDefinition assemblyDefinition = array[i];
				foreach (TypeDefinition current in assemblyDefinition.MainModule.Types)
				{
					if (current.Namespace.StartsWith("UnityEngine") && (current.Fields.Count > 0 || current.Methods.Count > 0 || current.Properties.Count > 0))
					{
						string name = current.Name;
						hashSet.Add(name);
					}
				}
			}
			AssemblyDefinition assemblyDefinition2 = null;
			for (int j = 0; j < assemblyFileNames.Length; j++)
			{
				if (assemblyFileNames[j] == "UnityEngine.dll")
				{
					assemblyDefinition2 = assemblyDefinitions[j];
				}
			}
			AssemblyDefinition[] array2 = assemblyDefinitions;
			for (int k = 0; k < array2.Length; k++)
			{
				AssemblyDefinition assemblyDefinition3 = array2[k];
				if (assemblyDefinition3 != assemblyDefinition2)
				{
					foreach (TypeReference current2 in assemblyDefinition3.MainModule.GetTypeReferences())
					{
						if (current2.Namespace.StartsWith("UnityEngine"))
						{
							string name2 = current2.Name;
							hashSet.Add(name2);
						}
					}
				}
			}
			return hashSet;
		}
		private static void WriteStaticallyLinkedModuleRegistration(TextWriter w, HashSet<string> nativeModules, HashSet<string> nativeClasses)
		{
			w.WriteLine("struct ClassRegistrationContext;");
			w.WriteLine("void InvokeRegisterStaticallyLinkedModuleClasses(ClassRegistrationContext& context)");
			w.WriteLine("{");
			if (nativeClasses == null)
			{
				w.WriteLine("\tvoid RegisterStaticallyLinkedModuleClasses(ClassRegistrationContext&);");
				w.WriteLine("\tRegisterStaticallyLinkedModuleClasses(context);");
			}
			else
			{
				w.WriteLine("\t// Do nothing (we're in stripping mode)");
			}
			w.WriteLine("}");
			w.WriteLine();
			w.WriteLine("void RegisterStaticallyLinkedModulesGranular()");
			w.WriteLine("{");
			foreach (string current in nativeModules)
			{
				w.WriteLine("\tvoid RegisterModule_" + current + "();");
				w.WriteLine("\tRegisterModule_" + current + "();");
				w.WriteLine();
			}
			w.WriteLine("}");
		}
		private static void WriteModuleAndClassRegistrationFile(string file, HashSet<string> nativeModules, HashSet<string> nativeClasses)
		{
			using (TextWriter textWriter = new StreamWriter(file))
			{
				CodeStrippingUtils.WriteStaticallyLinkedModuleRegistration(textWriter, nativeModules, nativeClasses);
				textWriter.WriteLine();
				textWriter.WriteLine("void RegisterAllClasses()");
				textWriter.WriteLine("{");
				if (nativeClasses == null)
				{
					textWriter.WriteLine("\tvoid RegisterAllClassesGranular();");
					textWriter.WriteLine("\tRegisterAllClassesGranular();");
				}
				else
				{
					textWriter.WriteLine(string.Format("\t//Total: {0} classes", nativeClasses.Count));
					int num = 0;
					foreach (string current in nativeClasses)
					{
						textWriter.WriteLine(string.Format("\t//{0}. {1}", num, current));
						if (current == "MasterServerInterface")
						{
							textWriter.WriteLine("\t//Skipping");
						}
						else
						{
							if (current == "NetworkManager")
							{
								textWriter.WriteLine("\t//Skipping");
							}
							else
							{
								textWriter.WriteLine(string.Format("\tvoid RegisterClass_{0}();", current));
								textWriter.WriteLine(string.Format("\tRegisterClass_{0}();", current));
							}
						}
						textWriter.WriteLine();
						num++;
					}
				}
				textWriter.WriteLine("}");
				textWriter.Close();
			}
		}
		private static string[] GetUserAssemblies(string strippedAssemblyDir)
		{
			List<string> list = new List<string>();
			string[] array = CodeStrippingUtils.s_UserAssemblies;
			for (int i = 0; i < array.Length; i++)
			{
				string assemblyName = array[i];
				list.AddRange(CodeStrippingUtils.GetAssembliesInDirectory(strippedAssemblyDir, assemblyName));
			}
			return list.ToArray();
		}
		private static IEnumerable<string> GetAssembliesInDirectory(string strippedAssemblyDir, string assemblyName)
		{
			return Directory.GetFiles(strippedAssemblyDir, assemblyName, SearchOption.TopDirectoryOnly);
		}
	}
}
