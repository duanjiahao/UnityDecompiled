using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor.Analytics;
using UnityEditor.BuildReporting;
using UnityEditor.Utils;
using UnityEditorInternal;

namespace UnityEditor
{
	internal class CodeStrippingUtils
	{
		private static UnityType s_GameManagerTypeInfo = null;

		private static string[] s_blackListNativeClassNames = new string[]
		{
			"PreloadData",
			"Material",
			"Cubemap",
			"Texture3D",
			"Texture2DArray",
			"RenderTexture",
			"Mesh",
			"Sprite"
		};

		private static UnityType[] s_blackListNativeClasses;

		private static readonly Dictionary<string, string> s_blackListNativeClassesDependencyNames = new Dictionary<string, string>
		{
			{
				"ParticleSystemRenderer",
				"ParticleSystem"
			}
		};

		private static Dictionary<UnityType, UnityType> s_blackListNativeClassesDependency;

		private static readonly string[] s_UserAssemblies = new string[]
		{
			"Assembly-CSharp.dll",
			"Assembly-CSharp-firstpass.dll",
			"Assembly-UnityScript.dll",
			"Assembly-UnityScript-firstpass.dll",
			"UnityEngine.Analytics.dll"
		};

		private static UnityType GameManagerTypeInfo
		{
			get
			{
				if (CodeStrippingUtils.s_GameManagerTypeInfo == null)
				{
					CodeStrippingUtils.s_GameManagerTypeInfo = CodeStrippingUtils.FindTypeByNameChecked("GameManager", "initializing code stripping utils");
				}
				return CodeStrippingUtils.s_GameManagerTypeInfo;
			}
		}

		public static UnityType[] BlackListNativeClasses
		{
			get
			{
				if (CodeStrippingUtils.s_blackListNativeClasses == null)
				{
					CodeStrippingUtils.s_blackListNativeClasses = (from typeName in CodeStrippingUtils.s_blackListNativeClassNames
					select CodeStrippingUtils.FindTypeByNameChecked(typeName, "code stripping blacklist native class")).ToArray<UnityType>();
				}
				return CodeStrippingUtils.s_blackListNativeClasses;
			}
		}

		public static Dictionary<UnityType, UnityType> BlackListNativeClassesDependency
		{
			get
			{
				if (CodeStrippingUtils.s_blackListNativeClassesDependency == null)
				{
					CodeStrippingUtils.s_blackListNativeClassesDependency = new Dictionary<UnityType, UnityType>();
					foreach (KeyValuePair<string, string> current in CodeStrippingUtils.s_blackListNativeClassesDependencyNames)
					{
						CodeStrippingUtils.BlackListNativeClassesDependency.Add(CodeStrippingUtils.FindTypeByNameChecked(current.Key, "code stripping blacklist native class dependency key"), CodeStrippingUtils.FindTypeByNameChecked(current.Value, "code stripping blacklist native class dependency value"));
					}
				}
				return CodeStrippingUtils.s_blackListNativeClassesDependency;
			}
		}

		public static string[] UserAssemblies
		{
			get
			{
				string[] assembliesWithSuffix;
				try
				{
					assembliesWithSuffix = CodeStrippingUtils.GetAssembliesWithSuffix();
				}
				catch
				{
					assembliesWithSuffix = CodeStrippingUtils.s_UserAssemblies;
				}
				return assembliesWithSuffix;
			}
		}

		private static UnityType FindTypeByNameChecked(string name, string msg)
		{
			UnityType unityType = UnityType.FindTypeByName(name);
			if (unityType == null)
			{
				throw new ArgumentException(string.Format("Could not map typename '{0}' to type info ({1})", name, msg ?? "no context"));
			}
			return unityType;
		}

		public static HashSet<string> GetModulesFromICalls(string icallsListFile)
		{
			string[] array = File.ReadAllLines(icallsListFile);
			HashSet<string> hashSet = new HashSet<string>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string icall = array2[i];
				string iCallModule = ModuleMetadata.GetICallModule(icall);
				if (!string.IsNullOrEmpty(iCallModule))
				{
					hashSet.Add(iCallModule);
				}
			}
			return hashSet;
		}

		public static void InjectCustomDependencies(StrippingInfo strippingInfo, HashSet<UnityType> nativeClasses)
		{
			UnityType item = UnityType.FindTypeByName("UnityAnalyticsManager");
			if (nativeClasses.Contains(item))
			{
				if (PlayerSettings.submitAnalytics)
				{
					strippingInfo.RegisterDependency("UnityAnalyticsManager", "Required by HW Statistics (See Player Settings)");
					strippingInfo.SetIcon("Required by HW Statistics (See Player Settings)", "class/PlayerSettings");
				}
				if (AnalyticsSettings.enabled)
				{
					strippingInfo.RegisterDependency("UnityAnalyticsManager", "Required by Unity Analytics (See Services Window)");
					strippingInfo.SetIcon("Required by Unity Analytics (See Services Window)", "class/PlayerSettings");
				}
			}
		}

		public static void GenerateDependencies(string strippedAssemblyDir, string icallsListFile, RuntimeClassRegistry rcr, bool doStripping, out HashSet<UnityType> nativeClasses, out HashSet<string> nativeModules, IIl2CppPlatformProvider platformProvider)
		{
			StrippingInfo strippingInfo = (platformProvider != null) ? StrippingInfo.GetBuildReportData(platformProvider.buildReport) : null;
			string[] userAssemblies = CodeStrippingUtils.GetUserAssemblies(strippedAssemblyDir);
			nativeClasses = ((!doStripping) ? null : CodeStrippingUtils.GenerateNativeClassList(rcr, strippedAssemblyDir, userAssemblies, strippingInfo));
			if (nativeClasses != null)
			{
				CodeStrippingUtils.ExcludeModuleManagers(ref nativeClasses);
			}
			nativeModules = CodeStrippingUtils.GetNativeModulesToRegister(nativeClasses, strippingInfo);
			if (nativeClasses != null && icallsListFile != null)
			{
				HashSet<string> modulesFromICalls = CodeStrippingUtils.GetModulesFromICalls(icallsListFile);
				foreach (string current in modulesFromICalls)
				{
					if (!nativeModules.Contains(current))
					{
						if (strippingInfo != null)
						{
							strippingInfo.RegisterDependency(StrippingInfo.ModuleName(current), "Required by Scripts");
						}
					}
					UnityType[] moduleTypes = ModuleMetadata.GetModuleTypes(current);
					UnityType[] array = moduleTypes;
					for (int i = 0; i < array.Length; i++)
					{
						UnityType unityType = array[i];
						if (unityType.IsDerivedFrom(CodeStrippingUtils.GameManagerTypeInfo))
						{
							nativeClasses.Add(unityType);
						}
					}
				}
				nativeModules.UnionWith(modulesFromICalls);
			}
			bool flag = true;
			if (platformProvider != null)
			{
				while (flag)
				{
					flag = false;
					foreach (string current2 in nativeModules.ToList<string>())
					{
						string moduleWhitelist = CodeStrippingUtils.GetModuleWhitelist(current2, platformProvider.moduleStrippingInformationFolder);
						if (File.Exists(moduleWhitelist))
						{
							foreach (string current3 in CodeStrippingUtils.GetDependentModules(moduleWhitelist))
							{
								if (!nativeModules.Contains(current3))
								{
									nativeModules.Add(current3);
									flag = true;
								}
								if (strippingInfo != null)
								{
									string text = StrippingInfo.ModuleName(current2);
									strippingInfo.RegisterDependency(StrippingInfo.ModuleName(current3), "Required by " + text);
									if (strippingInfo.icons.ContainsKey(text))
									{
										strippingInfo.SetIcon("Required by " + text, strippingInfo.icons[text]);
									}
								}
							}
						}
					}
				}
			}
			AssemblyReferenceChecker assemblyReferenceChecker = new AssemblyReferenceChecker();
			assemblyReferenceChecker.CollectReferencesFromRoots(strippedAssemblyDir, userAssemblies, true, 0f, true);
			if (strippingInfo != null)
			{
				foreach (string current4 in nativeModules)
				{
					strippingInfo.AddModule(StrippingInfo.ModuleName(current4));
				}
				strippingInfo.AddModule(StrippingInfo.ModuleName("Core"));
			}
			if (nativeClasses != null && strippingInfo != null)
			{
				CodeStrippingUtils.InjectCustomDependencies(strippingInfo, nativeClasses);
			}
		}

		public static string GetModuleWhitelist(string module, string moduleStrippingInformationFolder)
		{
			return Paths.Combine(new string[]
			{
				moduleStrippingInformationFolder,
				module + ".xml"
			});
		}

		public static List<string> GetDependentModules(string moduleXml)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(moduleXml);
			List<string> list = new List<string>();
			XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("/linker/dependencies/module");
			IEnumerator enumerator = xmlNodeList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					XmlNode xmlNode = (XmlNode)enumerator.Current;
					list.Add(xmlNode.Attributes["name"].Value);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return list;
		}

		public static void WriteModuleAndClassRegistrationFile(string strippedAssemblyDir, string icallsListFile, string outputDir, RuntimeClassRegistry rcr, IEnumerable<UnityType> classesToSkip, IIl2CppPlatformProvider platformProvider)
		{
			bool stripEngineCode = PlayerSettings.stripEngineCode;
			HashSet<UnityType> nativeClasses;
			HashSet<string> nativeModules;
			CodeStrippingUtils.GenerateDependencies(strippedAssemblyDir, icallsListFile, rcr, stripEngineCode, out nativeClasses, out nativeModules, platformProvider);
			string file = Path.Combine(outputDir, "UnityClassRegistration.cpp");
			CodeStrippingUtils.WriteModuleAndClassRegistrationFile(file, nativeModules, nativeClasses, new HashSet<UnityType>(classesToSkip));
		}

		public static HashSet<string> GetNativeModulesToRegister(HashSet<UnityType> nativeClasses, StrippingInfo strippingInfo)
		{
			return (nativeClasses != null) ? CodeStrippingUtils.GetRequiredStrippableModules(nativeClasses, strippingInfo) : CodeStrippingUtils.GetAllStrippableModules();
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

		private static HashSet<string> GetRequiredStrippableModules(HashSet<UnityType> nativeClasses, StrippingInfo strippingInfo)
		{
			HashSet<UnityType> hashSet = new HashSet<UnityType>();
			HashSet<string> hashSet2 = new HashSet<string>();
			string[] moduleNames = ModuleMetadata.GetModuleNames();
			for (int i = 0; i < moduleNames.Length; i++)
			{
				string text = moduleNames[i];
				if (ModuleMetadata.GetModuleStrippable(text))
				{
					HashSet<UnityType> hashSet3 = new HashSet<UnityType>(ModuleMetadata.GetModuleTypes(text));
					if (nativeClasses.Overlaps(hashSet3))
					{
						hashSet2.Add(text);
						if (strippingInfo != null)
						{
							foreach (UnityType current in hashSet3)
							{
								if (nativeClasses.Contains(current))
								{
									strippingInfo.RegisterDependency(StrippingInfo.ModuleName(text), current.name);
									hashSet.Add(current);
								}
							}
						}
					}
				}
			}
			if (strippingInfo != null)
			{
				foreach (UnityType current2 in nativeClasses)
				{
					if (!hashSet.Contains(current2))
					{
						strippingInfo.RegisterDependency(StrippingInfo.ModuleName("Core"), current2.name);
					}
				}
			}
			return hashSet2;
		}

		private static void ExcludeModuleManagers(ref HashSet<UnityType> nativeClasses)
		{
			string[] moduleNames = ModuleMetadata.GetModuleNames();
			string[] array = moduleNames;
			for (int i = 0; i < array.Length; i++)
			{
				string moduleName = array[i];
				if (ModuleMetadata.GetModuleStrippable(moduleName))
				{
					UnityType[] moduleTypes = ModuleMetadata.GetModuleTypes(moduleName);
					HashSet<UnityType> hashSet = new HashSet<UnityType>();
					HashSet<UnityType> hashSet2 = new HashSet<UnityType>();
					UnityType[] array2 = moduleTypes;
					for (int j = 0; j < array2.Length; j++)
					{
						UnityType unityType = array2[j];
						if (unityType.IsDerivedFrom(CodeStrippingUtils.GameManagerTypeInfo))
						{
							hashSet.Add(unityType);
						}
						else
						{
							hashSet2.Add(unityType);
						}
					}
					if (hashSet2.Count != 0)
					{
						if (!nativeClasses.Overlaps(hashSet2))
						{
							foreach (UnityType current in hashSet)
							{
								nativeClasses.Remove(current);
							}
						}
						else
						{
							foreach (UnityType current2 in hashSet)
							{
								nativeClasses.Add(current2);
							}
						}
					}
				}
			}
		}

		private static HashSet<UnityType> GenerateNativeClassList(RuntimeClassRegistry rcr, string directory, string[] rootAssemblies, StrippingInfo strippingInfo)
		{
			HashSet<UnityType> hashSet = CodeStrippingUtils.CollectNativeClassListFromRoots(directory, rootAssemblies, strippingInfo);
			UnityType[] blackListNativeClasses = CodeStrippingUtils.BlackListNativeClasses;
			for (int i = 0; i < blackListNativeClasses.Length; i++)
			{
				UnityType item = blackListNativeClasses[i];
				hashSet.Add(item);
			}
			foreach (UnityType current in CodeStrippingUtils.BlackListNativeClassesDependency.Keys)
			{
				if (hashSet.Contains(current))
				{
					UnityType item2 = CodeStrippingUtils.BlackListNativeClassesDependency[current];
					hashSet.Add(item2);
				}
			}
			foreach (string current2 in rcr.GetAllNativeClassesIncludingManagersAsString())
			{
				UnityType unityType = UnityType.FindTypeByName(current2);
				if (unityType != null && unityType.baseClass != null)
				{
					hashSet.Add(unityType);
					if (strippingInfo != null)
					{
						if (!unityType.IsDerivedFrom(CodeStrippingUtils.GameManagerTypeInfo))
						{
							List<string> scenesForClass = rcr.GetScenesForClass(unityType.persistentTypeID);
							if (scenesForClass != null)
							{
								foreach (string current3 in scenesForClass)
								{
									strippingInfo.RegisterDependency(current2, current3);
									if (current3.EndsWith(".unity"))
									{
										strippingInfo.SetIcon(current3, "class/SceneAsset");
									}
									else
									{
										strippingInfo.SetIcon(current3, "class/AssetBundle");
									}
								}
							}
						}
					}
				}
			}
			HashSet<UnityType> hashSet2 = new HashSet<UnityType>();
			foreach (UnityType current4 in hashSet)
			{
				UnityType unityType2 = current4;
				while (unityType2.baseClass != null)
				{
					hashSet2.Add(unityType2);
					unityType2 = unityType2.baseClass;
				}
			}
			return hashSet2;
		}

		private static HashSet<UnityType> CollectNativeClassListFromRoots(string directory, string[] rootAssemblies, StrippingInfo strippingInfo)
		{
			HashSet<string> source = CodeStrippingUtils.CollectManagedTypeReferencesFromRoots(directory, rootAssemblies, strippingInfo);
			IEnumerable<UnityType> collection = from name in source
			select UnityType.FindTypeByName(name) into klass
			where klass != null && klass.baseClass != null
			select klass;
			return new HashSet<UnityType>(collection);
		}

		private static HashSet<string> CollectManagedTypeReferencesFromRoots(string directory, string[] rootAssemblies, StrippingInfo strippingInfo)
		{
			HashSet<string> hashSet = new HashSet<string>();
			AssemblyReferenceChecker assemblyReferenceChecker = new AssemblyReferenceChecker();
			bool collectMethods = false;
			bool ignoreSystemDlls = false;
			assemblyReferenceChecker.CollectReferencesFromRoots(directory, rootAssemblies, collectMethods, 0f, ignoreSystemDlls);
			string[] assemblyFileNames = assemblyReferenceChecker.GetAssemblyFileNames();
			AssemblyDefinition[] assemblyDefinitions = assemblyReferenceChecker.GetAssemblyDefinitions();
			AssemblyDefinition[] array = assemblyDefinitions;
			for (int i = 0; i < array.Length; i++)
			{
				AssemblyDefinition assemblyDefinition = array[i];
				using (Collection<TypeDefinition>.Enumerator enumerator = assemblyDefinition.get_MainModule().get_Types().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TypeDefinition current = enumerator.get_Current();
						if (current.get_Namespace().StartsWith("UnityEngine"))
						{
							if (current.get_Fields().get_Count() > 0 || current.get_Methods().get_Count() > 0 || current.get_Properties().get_Count() > 0)
							{
								string name = current.get_Name();
								hashSet.Add(name);
								if (strippingInfo != null)
								{
									string name2 = assemblyDefinition.get_Name().get_Name();
									if (!AssemblyReferenceChecker.IsIgnoredSystemDll(name2))
									{
										strippingInfo.RegisterDependency(name, "Required by Scripts");
									}
								}
							}
						}
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
					foreach (TypeReference current2 in assemblyDefinition3.get_MainModule().GetTypeReferences())
					{
						if (current2.get_Namespace().StartsWith("UnityEngine"))
						{
							string name3 = current2.get_Name();
							hashSet.Add(name3);
							if (strippingInfo != null)
							{
								string name4 = assemblyDefinition3.get_Name().get_Name();
								if (!AssemblyReferenceChecker.IsIgnoredSystemDll(name4))
								{
									strippingInfo.RegisterDependency(name3, "Required by Scripts");
								}
							}
						}
					}
				}
			}
			return hashSet;
		}

		private static void WriteStaticallyLinkedModuleRegistration(TextWriter w, HashSet<string> nativeModules, HashSet<UnityType> nativeClasses)
		{
			w.WriteLine("void InvokeRegisterStaticallyLinkedModuleClasses()");
			w.WriteLine("{");
			if (nativeClasses == null)
			{
				w.WriteLine("\tvoid RegisterStaticallyLinkedModuleClasses();");
				w.WriteLine("\tRegisterStaticallyLinkedModuleClasses();");
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

		private static void WriteModuleAndClassRegistrationFile(string file, HashSet<string> nativeModules, HashSet<UnityType> nativeClasses, HashSet<UnityType> classesToSkip)
		{
			using (TextWriter textWriter = new StreamWriter(file))
			{
				textWriter.WriteLine("template <typename T> void RegisterClass();");
				textWriter.WriteLine("template <typename T> void RegisterStrippedTypeInfo(int, const char*, const char*);");
				textWriter.WriteLine();
				CodeStrippingUtils.WriteStaticallyLinkedModuleRegistration(textWriter, nativeModules, nativeClasses);
				textWriter.WriteLine();
				if (nativeClasses != null)
				{
					foreach (UnityType current in UnityType.GetTypes())
					{
						if (current.baseClass != null && !current.isEditorOnly && !classesToSkip.Contains(current))
						{
							if (current.hasNativeNamespace)
							{
								textWriter.Write("namespace {0} {{ class {1}; }} ", current.nativeNamespace, current.name);
							}
							else
							{
								textWriter.Write("class {0}; ", current.name);
							}
							if (nativeClasses.Contains(current))
							{
								textWriter.WriteLine("template <> void RegisterClass<{0}>();", current.qualifiedName);
							}
							else
							{
								textWriter.WriteLine();
							}
						}
					}
					textWriter.WriteLine();
				}
				textWriter.WriteLine("void RegisterAllClasses()");
				textWriter.WriteLine("{");
				if (nativeClasses == null)
				{
					textWriter.WriteLine("\tvoid RegisterAllClassesGranular();");
					textWriter.WriteLine("\tRegisterAllClassesGranular();");
				}
				else
				{
					textWriter.WriteLine("void RegisterBuiltinTypes();");
					textWriter.WriteLine("RegisterBuiltinTypes();");
					textWriter.WriteLine("\t//Total: {0} non stripped classes", nativeClasses.Count);
					int num = 0;
					foreach (UnityType current2 in nativeClasses)
					{
						textWriter.WriteLine("\t//{0}. {1}", num, current2.qualifiedName);
						if (classesToSkip.Contains(current2))
						{
							textWriter.WriteLine("\t//Skipping {0}", current2.qualifiedName);
						}
						else
						{
							textWriter.WriteLine("\tRegisterClass<{0}>();", current2.qualifiedName);
						}
						num++;
					}
					textWriter.WriteLine();
				}
				textWriter.WriteLine("}");
				textWriter.Close();
			}
		}

		private static string[] GetAssembliesWithSuffix()
		{
			string suffix = EditorSettings.Internal_UserGeneratedProjectSuffix;
			return CodeStrippingUtils.s_UserAssemblies.Select(delegate(string a)
			{
				string result;
				if (a.StartsWith("Assembly-"))
				{
					result = a.Substring(0, a.Length - ".dll".Length) + suffix + ".dll";
				}
				else
				{
					result = a;
				}
				return result;
			}).ToArray<string>();
		}

		private static string[] GetUserAssemblies(string strippedAssemblyDir)
		{
			List<string> list = new List<string>();
			string[] userAssemblies = CodeStrippingUtils.UserAssemblies;
			for (int i = 0; i < userAssemblies.Length; i++)
			{
				string assemblyName = userAssemblies[i];
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
