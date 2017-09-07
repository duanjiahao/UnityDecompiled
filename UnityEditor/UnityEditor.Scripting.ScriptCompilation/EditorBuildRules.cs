using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Scripting.Compilers;
using UnityEngine;

namespace UnityEditor.Scripting.ScriptCompilation
{
	internal static class EditorBuildRules
	{
		internal enum TargetAssemblyType
		{
			Undefined,
			Predefined,
			Custom
		}

		internal class TargetAssembly
		{
			public string Filename
			{
				get;
				private set;
			}

			public SupportedLanguage Language
			{
				get;
				set;
			}

			public AssemblyFlags Flags
			{
				get;
				private set;
			}

			public Func<string, int> PathFilter
			{
				get;
				private set;
			}

			public List<EditorBuildRules.TargetAssembly> References
			{
				get;
				private set;
			}

			public EditorBuildRules.TargetAssemblyType Type
			{
				get;
				private set;
			}

			public bool EditorOnly
			{
				get
				{
					return (this.Flags & AssemblyFlags.EditorOnly) == AssemblyFlags.EditorOnly;
				}
			}

			public TargetAssembly()
			{
				this.References = new List<EditorBuildRules.TargetAssembly>();
			}

			public TargetAssembly(string name, SupportedLanguage language, AssemblyFlags flags, EditorBuildRules.TargetAssemblyType type) : this(name, language, flags, type, null)
			{
			}

			public TargetAssembly(string name, SupportedLanguage language, AssemblyFlags flags, EditorBuildRules.TargetAssemblyType type, Func<string, int> pathFilter) : this()
			{
				this.Language = language;
				this.Filename = name;
				this.Flags = flags;
				this.PathFilter = pathFilter;
				this.Type = type;
			}

			public string FilenameWithSuffix(string filenameSuffix)
			{
				string result;
				if (!string.IsNullOrEmpty(filenameSuffix))
				{
					result = this.Filename.Replace(".dll", filenameSuffix + ".dll");
				}
				else
				{
					result = this.Filename;
				}
				return result;
			}

			public string FullPath(string outputDirectory, string filenameSuffix)
			{
				return Path.Combine(outputDirectory, this.FilenameWithSuffix(filenameSuffix));
			}
		}

		public class CompilationAssemblies
		{
			public PrecompiledAssembly[] UnityAssemblies
			{
				get;
				set;
			}

			public PrecompiledAssembly[] PrecompiledAssemblies
			{
				get;
				set;
			}

			public EditorBuildRules.TargetAssembly[] CustomTargetAssemblies
			{
				get;
				set;
			}

			public string[] EditorAssemblyReferences
			{
				get;
				set;
			}
		}

		public class GenerateChangedScriptAssembliesArgs
		{
			public IEnumerable<string> AllSourceFiles
			{
				get;
				set;
			}

			public IEnumerable<string> DirtySourceFiles
			{
				get;
				set;
			}

			public string ProjectDirectory
			{
				get;
				set;
			}

			public BuildFlags BuildFlags
			{
				get;
				set;
			}

			public ScriptAssemblySettings Settings
			{
				get;
				set;
			}

			public EditorBuildRules.CompilationAssemblies Assemblies
			{
				get;
				set;
			}

			public HashSet<string> RunUpdaterAssemblies
			{
				get;
				set;
			}

			public HashSet<EditorBuildRules.TargetAssembly> NotCompiledTargetAssemblies
			{
				get;
				set;
			}

			public GenerateChangedScriptAssembliesArgs()
			{
				this.NotCompiledTargetAssemblies = new HashSet<EditorBuildRules.TargetAssembly>();
			}
		}

		private static readonly EditorBuildRules.TargetAssembly[] predefinedTargetAssemblies;

		[CompilerGenerated]
		private static Func<string, int> <>f__mg$cache0;

		[CompilerGenerated]
		private static Func<string, int> <>f__mg$cache1;

		[CompilerGenerated]
		private static Func<string, int> <>f__mg$cache2;

		static EditorBuildRules()
		{
			EditorBuildRules.predefinedTargetAssemblies = EditorBuildRules.CreatePredefinedTargetAssemblies();
		}

		public static IEnumerable<EditorBuildRules.TargetAssembly> GetTargetAssemblies(SupportedLanguage language, EditorBuildRules.TargetAssembly[] customTargetAssemblies)
		{
			IEnumerable<EditorBuildRules.TargetAssembly> enumerable = from a in EditorBuildRules.predefinedTargetAssemblies
			where a.Language.GetLanguageName() == language.GetLanguageName()
			select a;
			IEnumerable<EditorBuildRules.TargetAssembly> result;
			if (customTargetAssemblies == null)
			{
				result = enumerable;
			}
			else
			{
				result = enumerable.Concat(customTargetAssemblies);
			}
			return result;
		}

		public static EditorBuildRules.TargetAssembly[] GetPredefinedTargetAssemblies()
		{
			return EditorBuildRules.predefinedTargetAssemblies;
		}

		public static PrecompiledAssembly CreateUserCompiledAssembly(string path)
		{
			AssemblyFlags assemblyFlags = AssemblyFlags.None;
			string text = path.ToLower();
			if (text.Contains("/editor/") || text.Contains("\\editor\\"))
			{
				assemblyFlags |= AssemblyFlags.EditorOnly;
			}
			return new PrecompiledAssembly
			{
				Path = path,
				Flags = assemblyFlags
			};
		}

		public static PrecompiledAssembly CreateEditorCompiledAssembly(string path)
		{
			return new PrecompiledAssembly
			{
				Path = path,
				Flags = AssemblyFlags.EditorOnly
			};
		}

		public static EditorBuildRules.TargetAssembly[] CreateTargetAssemblies(IEnumerable<CustomScriptAssembly> customScriptAssemblies)
		{
			EditorBuildRules.TargetAssembly[] result;
			if (customScriptAssemblies == null)
			{
				result = null;
			}
			else
			{
				List<EditorBuildRules.TargetAssembly> list = new List<EditorBuildRules.TargetAssembly>();
				Dictionary<string, EditorBuildRules.TargetAssembly> dictionary = new Dictionary<string, EditorBuildRules.TargetAssembly>();
				foreach (CustomScriptAssembly current in customScriptAssemblies)
				{
					string pathPrefixLowerCase = current.PathPrefix.ToLower();
					EditorBuildRules.TargetAssembly targetAssembly = new EditorBuildRules.TargetAssembly(current.Name + ".dll", null, current.AssemblyFlags, EditorBuildRules.TargetAssemblyType.Custom, (string path) => (!path.StartsWith(pathPrefixLowerCase)) ? -1 : pathPrefixLowerCase.Length);
					list.Add(targetAssembly);
					dictionary[current.Name] = targetAssembly;
				}
				List<EditorBuildRules.TargetAssembly>.Enumerator enumerator2 = list.GetEnumerator();
				foreach (CustomScriptAssembly current2 in customScriptAssemblies)
				{
					enumerator2.MoveNext();
					EditorBuildRules.TargetAssembly current3 = enumerator2.Current;
					if (current2.References != null)
					{
						string[] references = current2.References;
						for (int i = 0; i < references.Length; i++)
						{
							string text = references[i];
							EditorBuildRules.TargetAssembly item = null;
							if (!dictionary.TryGetValue(text, out item))
							{
								Debug.LogWarning(string.Format("Could not find reference '{0}' for assembly '{1}'", text, current2.Name));
							}
							else
							{
								current3.References.Add(item);
							}
						}
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		public static ScriptAssembly[] GetAllScriptAssemblies(IEnumerable<string> allSourceFiles, string projectDirectory, BuildFlags buildFlags, ScriptAssemblySettings settings, EditorBuildRules.CompilationAssemblies assemblies)
		{
			ScriptAssembly[] result;
			if (allSourceFiles == null || allSourceFiles.Count<string>() == 0)
			{
				result = new ScriptAssembly[0];
			}
			else
			{
				bool flag = (buildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
				Dictionary<EditorBuildRules.TargetAssembly, HashSet<string>> dictionary = new Dictionary<EditorBuildRules.TargetAssembly, HashSet<string>>();
				foreach (string current in allSourceFiles)
				{
					EditorBuildRules.TargetAssembly targetAssembly = EditorBuildRules.GetTargetAssembly(current, projectDirectory, assemblies.CustomTargetAssemblies);
					if (targetAssembly != null)
					{
						if (flag || !targetAssembly.EditorOnly)
						{
							HashSet<string> hashSet;
							if (!dictionary.TryGetValue(targetAssembly, out hashSet))
							{
								hashSet = new HashSet<string>();
								dictionary[targetAssembly] = hashSet;
							}
							hashSet.Add(Path.Combine(projectDirectory, current));
						}
					}
				}
				result = EditorBuildRules.ToScriptAssemblies(dictionary, settings, buildFlags, assemblies, null);
			}
			return result;
		}

		public static ScriptAssembly[] GenerateChangedScriptAssemblies(EditorBuildRules.GenerateChangedScriptAssembliesArgs args)
		{
			bool flag = (args.BuildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
			Dictionary<EditorBuildRules.TargetAssembly, HashSet<string>> dictionary = new Dictionary<EditorBuildRules.TargetAssembly, HashSet<string>>();
			EditorBuildRules.TargetAssembly[] array = (args.Assemblies.CustomTargetAssemblies != null) ? EditorBuildRules.predefinedTargetAssemblies.Concat(args.Assemblies.CustomTargetAssemblies).ToArray<EditorBuildRules.TargetAssembly>() : EditorBuildRules.predefinedTargetAssemblies;
			if (args.RunUpdaterAssemblies != null)
			{
				using (HashSet<string>.Enumerator enumerator = args.RunUpdaterAssemblies.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string assemblyFilename = enumerator.Current;
						EditorBuildRules.TargetAssembly key = array.First((EditorBuildRules.TargetAssembly a) => a.FilenameWithSuffix(args.Settings.FilenameSuffix) == assemblyFilename);
						dictionary[key] = new HashSet<string>();
					}
				}
			}
			foreach (string current in args.DirtySourceFiles)
			{
				EditorBuildRules.TargetAssembly targetAssembly = EditorBuildRules.GetTargetAssembly(current, args.ProjectDirectory, args.Assemblies.CustomTargetAssemblies);
				if (flag || !targetAssembly.EditorOnly)
				{
					string extensionOfSourceFile = ScriptCompilers.GetExtensionOfSourceFile(current);
					SupportedLanguage languageFromExtension = ScriptCompilers.GetLanguageFromExtension(extensionOfSourceFile);
					HashSet<string> hashSet;
					if (!dictionary.TryGetValue(targetAssembly, out hashSet))
					{
						hashSet = new HashSet<string>();
						dictionary[targetAssembly] = hashSet;
						if (targetAssembly.Type == EditorBuildRules.TargetAssemblyType.Custom)
						{
							targetAssembly.Language = languageFromExtension;
						}
					}
					hashSet.Add(Path.Combine(args.ProjectDirectory, current));
					if (languageFromExtension != targetAssembly.Language)
					{
						args.NotCompiledTargetAssemblies.Add(targetAssembly);
					}
				}
			}
			bool flag2 = dictionary.Any((KeyValuePair<EditorBuildRules.TargetAssembly, HashSet<string>> entry) => entry.Key.Type == EditorBuildRules.TargetAssemblyType.Custom);
			if (flag2)
			{
				EditorBuildRules.TargetAssembly[] array2 = EditorBuildRules.predefinedTargetAssemblies;
				for (int i = 0; i < array2.Length; i++)
				{
					EditorBuildRules.TargetAssembly targetAssembly2 = array2[i];
					if (flag || !targetAssembly2.EditorOnly)
					{
						if (!dictionary.ContainsKey(targetAssembly2))
						{
							dictionary[targetAssembly2] = new HashSet<string>();
						}
					}
				}
			}
			int num;
			do
			{
				num = 0;
				EditorBuildRules.TargetAssembly[] array3 = array;
				for (int j = 0; j < array3.Length; j++)
				{
					EditorBuildRules.TargetAssembly targetAssembly3 = array3[j];
					if (flag || !targetAssembly3.EditorOnly)
					{
						if (!dictionary.ContainsKey(targetAssembly3))
						{
							foreach (EditorBuildRules.TargetAssembly current2 in targetAssembly3.References)
							{
								if (dictionary.ContainsKey(current2))
								{
									dictionary[targetAssembly3] = new HashSet<string>();
									num++;
									break;
								}
							}
						}
					}
				}
			}
			while (num > 0);
			foreach (string current3 in args.AllSourceFiles)
			{
				EditorBuildRules.TargetAssembly targetAssembly4 = EditorBuildRules.GetTargetAssembly(current3, args.ProjectDirectory, args.Assemblies.CustomTargetAssemblies);
				if (flag || !targetAssembly4.EditorOnly)
				{
					string extensionOfSourceFile2 = ScriptCompilers.GetExtensionOfSourceFile(current3);
					SupportedLanguage languageFromExtension2 = ScriptCompilers.GetLanguageFromExtension(extensionOfSourceFile2);
					if (targetAssembly4.Language == null && targetAssembly4.Type == EditorBuildRules.TargetAssemblyType.Custom)
					{
						targetAssembly4.Language = languageFromExtension2;
					}
					if (languageFromExtension2 != targetAssembly4.Language)
					{
						args.NotCompiledTargetAssemblies.Add(targetAssembly4);
					}
					HashSet<string> hashSet2;
					if (dictionary.TryGetValue(targetAssembly4, out hashSet2))
					{
						hashSet2.Add(Path.Combine(args.ProjectDirectory, current3));
					}
				}
			}
			dictionary = (from e in dictionary
			where e.Value.Count > 0
			select e).ToDictionary((KeyValuePair<EditorBuildRules.TargetAssembly, HashSet<string>> e) => e.Key, (KeyValuePair<EditorBuildRules.TargetAssembly, HashSet<string>> e) => e.Value);
			foreach (EditorBuildRules.TargetAssembly current4 in args.NotCompiledTargetAssemblies)
			{
				dictionary.Remove(current4);
			}
			return EditorBuildRules.ToScriptAssemblies(dictionary, args.Settings, args.BuildFlags, args.Assemblies, args.RunUpdaterAssemblies);
		}

		internal static ScriptAssembly[] ToScriptAssemblies(IDictionary<EditorBuildRules.TargetAssembly, HashSet<string>> targetAssemblies, ScriptAssemblySettings settings, BuildFlags buildFlags, EditorBuildRules.CompilationAssemblies assemblies, HashSet<string> runUpdaterAssemblies)
		{
			ScriptAssembly[] array = new ScriptAssembly[targetAssemblies.Count];
			Dictionary<EditorBuildRules.TargetAssembly, ScriptAssembly> dictionary = new Dictionary<EditorBuildRules.TargetAssembly, ScriptAssembly>();
			int num = 0;
			bool flag = (buildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
			foreach (KeyValuePair<EditorBuildRules.TargetAssembly, HashSet<string>> current in targetAssemblies)
			{
				EditorBuildRules.TargetAssembly key = current.Key;
				HashSet<string> value = current.Value;
				ScriptAssembly scriptAssembly = new ScriptAssembly();
				array[num] = scriptAssembly;
				dictionary[key] = array[num++];
				scriptAssembly.BuildTarget = settings.BuildTarget;
				if (key.EditorOnly || (flag && settings.ApiCompatibilityLevel == ApiCompatibilityLevel.NET_4_6))
				{
					scriptAssembly.ApiCompatibilityLevel = ((EditorApplication.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest) ? ApiCompatibilityLevel.NET_2_0 : ApiCompatibilityLevel.NET_4_6);
				}
				else
				{
					scriptAssembly.ApiCompatibilityLevel = settings.ApiCompatibilityLevel;
				}
				if (!string.IsNullOrEmpty(settings.FilenameSuffix))
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(key.Filename);
					string extension = Path.GetExtension(key.Filename);
					scriptAssembly.Filename = fileNameWithoutExtension + settings.FilenameSuffix + extension;
				}
				else
				{
					scriptAssembly.Filename = key.Filename;
				}
				if (runUpdaterAssemblies != null && runUpdaterAssemblies.Contains(scriptAssembly.Filename))
				{
					scriptAssembly.RunUpdater = true;
				}
				scriptAssembly.OutputDirectory = settings.OutputDirectory;
				scriptAssembly.Defines = settings.Defines;
				scriptAssembly.Files = value.ToArray<string>();
				Array.Sort<string>(scriptAssembly.Files);
			}
			num = 0;
			foreach (KeyValuePair<EditorBuildRules.TargetAssembly, HashSet<string>> current2 in targetAssemblies)
			{
				EditorBuildRules.AddScriptAssemblyReferences(ref array[num++], current2.Key, settings, buildFlags, assemblies, dictionary, settings.FilenameSuffix);
			}
			return array;
		}

		private static bool IsCompiledAssemblyCompatibleWithTargetAssembly(PrecompiledAssembly compiledAssembly, EditorBuildRules.TargetAssembly targetAssembly, BuildTarget buildTarget, EditorBuildRules.TargetAssembly[] customTargetAssemblies)
		{
			bool flag = WSAHelpers.UseDotNetCore(targetAssembly.Filename, buildTarget, customTargetAssemblies);
			bool result;
			if (flag)
			{
				bool flag2 = (compiledAssembly.Flags & AssemblyFlags.UseForDotNet) == AssemblyFlags.UseForDotNet;
				result = flag2;
			}
			else
			{
				bool flag3 = (compiledAssembly.Flags & AssemblyFlags.UseForMono) == AssemblyFlags.UseForMono;
				result = flag3;
			}
			return result;
		}

		internal static void AddScriptAssemblyReferences(ref ScriptAssembly scriptAssembly, EditorBuildRules.TargetAssembly targetAssembly, ScriptAssemblySettings settings, BuildFlags buildFlags, EditorBuildRules.CompilationAssemblies assemblies, IDictionary<EditorBuildRules.TargetAssembly, ScriptAssembly> targetToScriptAssembly, string filenameSuffix)
		{
			List<ScriptAssembly> list = new List<ScriptAssembly>();
			List<string> list2 = new List<string>();
			bool flag = (buildFlags & BuildFlags.BuildingForEditor) == BuildFlags.BuildingForEditor;
			bool flag2 = (targetAssembly.Flags & AssemblyFlags.EditorOnly) == AssemblyFlags.EditorOnly;
			if (assemblies.UnityAssemblies != null)
			{
				PrecompiledAssembly[] unityAssemblies = assemblies.UnityAssemblies;
				for (int i = 0; i < unityAssemblies.Length; i++)
				{
					PrecompiledAssembly compiledAssembly = unityAssemblies[i];
					if (flag || flag2)
					{
						if ((compiledAssembly.Flags & AssemblyFlags.UseForMono) != AssemblyFlags.None)
						{
							list2.Add(compiledAssembly.Path);
						}
					}
					else if ((compiledAssembly.Flags & AssemblyFlags.EditorOnly) != AssemblyFlags.EditorOnly)
					{
						if (EditorBuildRules.IsCompiledAssemblyCompatibleWithTargetAssembly(compiledAssembly, targetAssembly, settings.BuildTarget, assemblies.CustomTargetAssemblies))
						{
							list2.Add(compiledAssembly.Path);
						}
					}
				}
			}
			foreach (EditorBuildRules.TargetAssembly current in targetAssembly.References)
			{
				ScriptAssembly item;
				if (targetToScriptAssembly.TryGetValue(current, out item))
				{
					list.Add(item);
				}
				else
				{
					string text = Path.Combine(settings.OutputDirectory, current.Filename);
					if (!string.IsNullOrEmpty(filenameSuffix))
					{
						text = text.Replace(".dll", filenameSuffix + ".dll");
					}
					if (File.Exists(text))
					{
						list2.Add(text);
					}
				}
			}
			if (assemblies.CustomTargetAssemblies != null && targetAssembly.Type == EditorBuildRules.TargetAssemblyType.Predefined)
			{
				EditorBuildRules.TargetAssembly[] customTargetAssemblies = assemblies.CustomTargetAssemblies;
				for (int j = 0; j < customTargetAssemblies.Length; j++)
				{
					EditorBuildRules.TargetAssembly key = customTargetAssemblies[j];
					ScriptAssembly item2;
					if (targetToScriptAssembly.TryGetValue(key, out item2))
					{
						list.Add(item2);
					}
				}
			}
			if (assemblies.PrecompiledAssemblies != null)
			{
				PrecompiledAssembly[] precompiledAssemblies = assemblies.PrecompiledAssemblies;
				for (int k = 0; k < precompiledAssemblies.Length; k++)
				{
					PrecompiledAssembly compiledAssembly2 = precompiledAssemblies[k];
					bool flag3 = (compiledAssembly2.Flags & AssemblyFlags.EditorOnly) == AssemblyFlags.EditorOnly;
					if (!flag3 || flag2)
					{
						if (EditorBuildRules.IsCompiledAssemblyCompatibleWithTargetAssembly(compiledAssembly2, targetAssembly, settings.BuildTarget, assemblies.CustomTargetAssemblies))
						{
							list2.Add(compiledAssembly2.Path);
						}
					}
				}
			}
			if (flag && assemblies.EditorAssemblyReferences != null)
			{
				list2.AddRange(assemblies.EditorAssemblyReferences);
			}
			scriptAssembly.ScriptAssemblyReferences = list.ToArray();
			scriptAssembly.References = list2.ToArray();
		}

		internal static EditorBuildRules.TargetAssembly[] CreatePredefinedTargetAssemblies()
		{
			List<EditorBuildRules.TargetAssembly> list = new List<EditorBuildRules.TargetAssembly>();
			List<EditorBuildRules.TargetAssembly> list2 = new List<EditorBuildRules.TargetAssembly>();
			List<EditorBuildRules.TargetAssembly> list3 = new List<EditorBuildRules.TargetAssembly>();
			List<EditorBuildRules.TargetAssembly> list4 = new List<EditorBuildRules.TargetAssembly>();
			List<SupportedLanguage> supportedLanguages = ScriptCompilers.SupportedLanguages;
			List<EditorBuildRules.TargetAssembly> list5 = new List<EditorBuildRules.TargetAssembly>();
			foreach (SupportedLanguage current in supportedLanguages)
			{
				string languageName = current.GetLanguageName();
				string arg_7B_0 = "Assembly-" + languageName + "-firstpass.dll";
				SupportedLanguage arg_7B_1 = current;
				AssemblyFlags arg_7B_2 = AssemblyFlags.None;
				EditorBuildRules.TargetAssemblyType arg_7B_3 = EditorBuildRules.TargetAssemblyType.Predefined;
				if (EditorBuildRules.<>f__mg$cache0 == null)
				{
					EditorBuildRules.<>f__mg$cache0 = new Func<string, int>(EditorBuildRules.FilterAssemblyInFirstpassFolder);
				}
				EditorBuildRules.TargetAssembly item = new EditorBuildRules.TargetAssembly(arg_7B_0, arg_7B_1, arg_7B_2, arg_7B_3, EditorBuildRules.<>f__mg$cache0);
				EditorBuildRules.TargetAssembly item2 = new EditorBuildRules.TargetAssembly("Assembly-" + languageName + ".dll", current, AssemblyFlags.None, EditorBuildRules.TargetAssemblyType.Predefined);
				string arg_D0_0 = "Assembly-" + languageName + "-Editor-firstpass.dll";
				SupportedLanguage arg_D0_1 = current;
				AssemblyFlags arg_D0_2 = AssemblyFlags.EditorOnly;
				EditorBuildRules.TargetAssemblyType arg_D0_3 = EditorBuildRules.TargetAssemblyType.Predefined;
				if (EditorBuildRules.<>f__mg$cache1 == null)
				{
					EditorBuildRules.<>f__mg$cache1 = new Func<string, int>(EditorBuildRules.FilterAssemblyInFirstpassEditorFolder);
				}
				EditorBuildRules.TargetAssembly item3 = new EditorBuildRules.TargetAssembly(arg_D0_0, arg_D0_1, arg_D0_2, arg_D0_3, EditorBuildRules.<>f__mg$cache1);
				string arg_109_0 = "Assembly-" + languageName + "-Editor.dll";
				SupportedLanguage arg_109_1 = current;
				AssemblyFlags arg_109_2 = AssemblyFlags.EditorOnly;
				EditorBuildRules.TargetAssemblyType arg_109_3 = EditorBuildRules.TargetAssemblyType.Predefined;
				if (EditorBuildRules.<>f__mg$cache2 == null)
				{
					EditorBuildRules.<>f__mg$cache2 = new Func<string, int>(EditorBuildRules.FilterAssemblyInEditorFolder);
				}
				EditorBuildRules.TargetAssembly item4 = new EditorBuildRules.TargetAssembly(arg_109_0, arg_109_1, arg_109_2, arg_109_3, EditorBuildRules.<>f__mg$cache2);
				list.Add(item);
				list2.Add(item2);
				list3.Add(item3);
				list4.Add(item4);
				list5.Add(item);
				list5.Add(item2);
				list5.Add(item3);
				list5.Add(item4);
			}
			foreach (EditorBuildRules.TargetAssembly current2 in list2)
			{
				current2.References.AddRange(list);
			}
			foreach (EditorBuildRules.TargetAssembly current3 in list3)
			{
				current3.References.AddRange(list);
			}
			foreach (EditorBuildRules.TargetAssembly current4 in list4)
			{
				current4.References.AddRange(list);
				current4.References.AddRange(list2);
				current4.References.AddRange(list3);
			}
			return list5.ToArray();
		}

		internal static EditorBuildRules.TargetAssembly GetTargetAssembly(string scriptPath, string projectDirectory, EditorBuildRules.TargetAssembly[] customTargetAssemblies)
		{
			EditorBuildRules.TargetAssembly customTargetAssembly = EditorBuildRules.GetCustomTargetAssembly(scriptPath, projectDirectory, customTargetAssemblies);
			EditorBuildRules.TargetAssembly result;
			if (customTargetAssembly != null)
			{
				result = customTargetAssembly;
			}
			else
			{
				result = EditorBuildRules.GetPredefinedTargetAssembly(scriptPath);
			}
			return result;
		}

		internal static EditorBuildRules.TargetAssembly GetPredefinedTargetAssembly(string scriptPath)
		{
			EditorBuildRules.TargetAssembly result = null;
			string a = Path.GetExtension(scriptPath).Substring(1).ToLower();
			string arg = "/" + scriptPath.ToLower();
			int num = -1;
			EditorBuildRules.TargetAssembly[] array = EditorBuildRules.predefinedTargetAssemblies;
			for (int i = 0; i < array.Length; i++)
			{
				EditorBuildRules.TargetAssembly targetAssembly = array[i];
				if (!(a != targetAssembly.Language.GetExtensionICanCompile()))
				{
					Func<string, int> pathFilter = targetAssembly.PathFilter;
					int num2;
					if (pathFilter == null)
					{
						num2 = 0;
					}
					else
					{
						num2 = pathFilter(arg);
					}
					if (num2 > num)
					{
						result = targetAssembly;
						num = num2;
					}
				}
			}
			return result;
		}

		internal static EditorBuildRules.TargetAssembly GetCustomTargetAssembly(string scriptPath, string projectDirectory, EditorBuildRules.TargetAssembly[] customTargetAssemblies)
		{
			EditorBuildRules.TargetAssembly result;
			if (customTargetAssemblies == null)
			{
				result = null;
			}
			else
			{
				int num = -1;
				EditorBuildRules.TargetAssembly targetAssembly = null;
				bool flag = Path.IsPathRooted(scriptPath);
				string arg = (!flag) ? Path.Combine(projectDirectory, scriptPath).ToLower() : scriptPath.ToLower();
				for (int i = 0; i < customTargetAssemblies.Length; i++)
				{
					EditorBuildRules.TargetAssembly targetAssembly2 = customTargetAssemblies[i];
					int num2 = targetAssembly2.PathFilter(arg);
					if (num2 > num)
					{
						targetAssembly = targetAssembly2;
						num = num2;
					}
				}
				result = targetAssembly;
			}
			return result;
		}

		private static int FilterAssemblyInFirstpassFolder(string pathName)
		{
			int num = EditorBuildRules.FilterAssemblyPathBeginsWith(pathName, "/assets/plugins/");
			int result;
			if (num >= 0)
			{
				result = num;
			}
			else
			{
				num = EditorBuildRules.FilterAssemblyPathBeginsWith(pathName, "/assets/standard assets/");
				if (num >= 0)
				{
					result = num;
				}
				else
				{
					num = EditorBuildRules.FilterAssemblyPathBeginsWith(pathName, "/assets/pro standard assets/");
					if (num >= 0)
					{
						result = num;
					}
					else
					{
						num = EditorBuildRules.FilterAssemblyPathBeginsWith(pathName, "/assets/iphone standard assets/");
						if (num >= 0)
						{
							result = num;
						}
						else
						{
							result = -1;
						}
					}
				}
			}
			return result;
		}

		private static int FilterAssemblyInFirstpassEditorFolder(string pathName)
		{
			int num = EditorBuildRules.FilterAssemblyInFirstpassFolder(pathName);
			int result;
			if (num == -1)
			{
				result = -1;
			}
			else
			{
				result = EditorBuildRules.FilterAssemblyInEditorFolder(pathName);
			}
			return result;
		}

		private static int FilterAssemblyInEditorFolder(string pathName)
		{
			int num = pathName.IndexOf("/editor/");
			int result;
			if (num == -1)
			{
				result = -1;
			}
			else
			{
				result = num + "/editor/".Length;
			}
			return result;
		}

		private static int FilterAssemblyPathBeginsWith(string pathName, string prefix)
		{
			return (!pathName.StartsWith(prefix)) ? -1 : prefix.Length;
		}
	}
}
