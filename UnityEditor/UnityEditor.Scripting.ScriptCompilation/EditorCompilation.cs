using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Modules;
using UnityEditor.Scripting.Compilers;
using UnityEditor.Scripting.Serialization;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Scripting.ScriptCompilation
{
	internal class EditorCompilation
	{
		public enum CompileStatus
		{
			Idle,
			Compiling,
			CompilationStarted,
			CompilationFailed,
			CompilationComplete
		}

		public struct TargetAssemblyInfo
		{
			public string Name;

			public AssemblyFlags Flags;
		}

		public struct AssemblyCompilerMessages
		{
			public string assemblyFilename;

			public CompilerMessage[] messages;
		}

		private bool areAllScriptsDirty;

		private string projectDirectory = string.Empty;

		private string assemblySuffix = string.Empty;

		private HashSet<string> allScripts = null;

		private HashSet<string> dirtyScripts = new HashSet<string>();

		private HashSet<string> runScriptUpdaterAssemblies = new HashSet<string>();

		private PrecompiledAssembly[] precompiledAssemblies;

		private CustomScriptAssembly[] customScriptAssemblies;

		private EditorBuildRules.TargetAssembly[] customTargetAssemblies;

		private PrecompiledAssembly[] unityAssemblies;

		private CompilationTask compilationTask;

		private static readonly string EditorAssemblyPath;

		private static readonly string EditorTempPath;

		private static readonly string AssemblyTimestampPath;

		static EditorCompilation()
		{
			EditorCompilation.EditorTempPath = "Temp";
			EditorCompilation.EditorAssemblyPath = Path.Combine("Library", "ScriptAssemblies");
			EditorCompilation.AssemblyTimestampPath = Path.Combine(EditorCompilation.EditorAssemblyPath, "BuiltinAssemblies.stamp");
		}

		internal void SetProjectDirectory(string projectDirectory)
		{
			this.projectDirectory = projectDirectory;
		}

		internal void SetAssemblySuffix(string assemblySuffix)
		{
			this.assemblySuffix = assemblySuffix;
		}

		public void SetAllScripts(string[] allScripts)
		{
			this.allScripts = new HashSet<string>(allScripts);
		}

		public bool IsExtensionSupportedByCompiler(string extension)
		{
			List<SupportedLanguage> supportedLanguages = ScriptCompilers.SupportedLanguages;
			return supportedLanguages.Count((SupportedLanguage l) => l.GetExtensionICanCompile() == extension) > 0;
		}

		public void DirtyAllScripts()
		{
			this.areAllScriptsDirty = true;
		}

		public void DirtyScript(string path)
		{
			this.allScripts.Add(path);
			this.dirtyScripts.Add(path);
		}

		public void RunScriptUpdaterOnAssembly(string assemblyFilename)
		{
			this.runScriptUpdaterAssemblies.Add(assemblyFilename);
		}

		public void SetAllUnityAssemblies(PrecompiledAssembly[] unityAssemblies)
		{
			this.unityAssemblies = unityAssemblies;
		}

		public void SetAllPrecompiledAssemblies(PrecompiledAssembly[] precompiledAssemblies)
		{
			this.precompiledAssemblies = precompiledAssemblies;
		}

		private static CustomScriptAssembly LoadCustomScriptAssemblyFromJson(string path)
		{
			string json = File.ReadAllText(path);
			CustomScriptAssemblyData customScriptAssemblyData = CustomScriptAssemblyData.FromJson(json);
			return CustomScriptAssembly.FromCustomScriptAssemblyData(path, customScriptAssemblyData);
		}

		private static void CheckCyclicAssemblyReferencesDFS(CustomScriptAssembly visitAssembly, HashSet<CustomScriptAssembly> visited, IDictionary<string, CustomScriptAssembly> nameToCustomScriptAssembly)
		{
			if (visited.Contains(visitAssembly))
			{
				throw new Exception(string.Format("Cyclic assembly references detected. Assemblies: {0}", string.Join(", ", (from a in visited
				select string.Format("'{0}'", a.Name)).ToArray<string>())));
			}
			visited.Add(visitAssembly);
			string[] references = visitAssembly.References;
			for (int i = 0; i < references.Length; i++)
			{
				string text = references[i];
				CustomScriptAssembly visitAssembly2;
				if (!nameToCustomScriptAssembly.TryGetValue(text, out visitAssembly2))
				{
					throw new Exception(string.Format("Reference to non-existent assembly. Assembly {0} has a reference to {1}", visitAssembly.Name, text));
				}
				EditorCompilation.CheckCyclicAssemblyReferencesDFS(visitAssembly2, visited, nameToCustomScriptAssembly);
			}
			visited.Remove(visitAssembly);
		}

		private static void CheckCyclicAssemblyReferences(CustomScriptAssembly[] customScriptAssemblies)
		{
			if (customScriptAssemblies != null && customScriptAssemblies.Length >= 2)
			{
				Dictionary<string, CustomScriptAssembly> dictionary = new Dictionary<string, CustomScriptAssembly>();
				for (int i = 0; i < customScriptAssemblies.Length; i++)
				{
					CustomScriptAssembly customScriptAssembly = customScriptAssemblies[i];
					dictionary[customScriptAssembly.Name] = customScriptAssembly;
				}
				HashSet<CustomScriptAssembly> visited = new HashSet<CustomScriptAssembly>();
				for (int j = 0; j < customScriptAssemblies.Length; j++)
				{
					CustomScriptAssembly visitAssembly = customScriptAssemblies[j];
					EditorCompilation.CheckCyclicAssemblyReferencesDFS(visitAssembly, visited, dictionary);
				}
			}
		}

		public void SetAllCustomScriptAssemblyJsons(string[] paths)
		{
			List<CustomScriptAssembly> list = new List<CustomScriptAssembly>();
			for (int i = 0; i < paths.Length; i++)
			{
				string text = paths[i];
				try
				{
					string path = (!Path.IsPathRooted(text)) ? Path.Combine(this.projectDirectory, text) : text;
					CustomScriptAssembly loadedCustomScriptAssembly = EditorCompilation.LoadCustomScriptAssemblyFromJson(path);
					if (list.Any((CustomScriptAssembly a) => string.Equals(a.Name, loadedCustomScriptAssembly.Name, StringComparison.OrdinalIgnoreCase)))
					{
						throw new Exception(string.Format("Assembly with name '{0}' is already defined ({1})", loadedCustomScriptAssembly.Name.Length, loadedCustomScriptAssembly.FilePath));
					}
					if (loadedCustomScriptAssembly.References == null)
					{
						loadedCustomScriptAssembly.References = new string[0];
					}
					if (loadedCustomScriptAssembly.References.Length != loadedCustomScriptAssembly.References.Distinct<string>().Count<string>())
					{
						throw new Exception(string.Format("Duplicate assembly references in {0}", loadedCustomScriptAssembly.FilePath));
					}
					list.Add(loadedCustomScriptAssembly);
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message + " - '" + text + "'");
				}
			}
			this.customScriptAssemblies = list.ToArray();
			try
			{
				EditorCompilation.CheckCyclicAssemblyReferences(this.customScriptAssemblies);
			}
			catch (Exception ex2)
			{
				this.customScriptAssemblies = null;
				this.customTargetAssemblies = null;
				throw ex2;
			}
			this.customTargetAssemblies = EditorBuildRules.CreateTargetAssemblies(this.customScriptAssemblies);
		}

		public void DeleteUnusedAssemblies()
		{
			string text = Path.Combine(Path.GetDirectoryName(Application.dataPath), EditorCompilation.EditorAssemblyPath);
			if (Directory.Exists(text))
			{
				List<string> list = new List<string>(Directory.GetFiles(text));
				list.Remove(Path.Combine(Path.GetDirectoryName(Application.dataPath), EditorCompilation.AssemblyTimestampPath));
				ScriptAssembly[] allScriptAssemblies = this.GetAllScriptAssemblies(BuildFlags.BuildingForEditor, EditorScriptCompilationOptions.BuildingForEditor);
				ScriptAssembly[] array = allScriptAssemblies;
				for (int i = 0; i < array.Length; i++)
				{
					ScriptAssembly scriptAssembly = array[i];
					if (scriptAssembly.Files.Length > 0)
					{
						string text2 = Path.Combine(text, scriptAssembly.Filename);
						list.Remove(text2);
						list.Remove(EditorCompilation.MDBPath(text2));
						list.Remove(EditorCompilation.PDBPath(text2));
					}
				}
				foreach (string current in list)
				{
					File.Delete(current);
				}
			}
		}

		public void CleanScriptAssemblies()
		{
			string path = Path.Combine(Path.GetDirectoryName(Application.dataPath), EditorCompilation.EditorAssemblyPath);
			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path);
				for (int i = 0; i < files.Length; i++)
				{
					string path2 = files[i];
					File.Delete(path2);
				}
			}
		}

		private static bool MoveOrReplaceFile(string sourcePath, string destinationPath)
		{
			bool flag = true;
			try
			{
				File.Move(sourcePath, destinationPath);
			}
			catch (IOException)
			{
				flag = false;
			}
			if (!flag)
			{
				flag = true;
				try
				{
					File.Replace(sourcePath, destinationPath, null);
				}
				catch (IOException)
				{
					flag = false;
				}
			}
			return flag;
		}

		private static string PDBPath(string dllPath)
		{
			return dllPath.Replace(".dll", ".pdb");
		}

		private static string MDBPath(string dllPath)
		{
			return dllPath + ".mdb";
		}

		private static void CopyAssembly(string sourcePath, string destinationPath)
		{
			if (EditorCompilation.MoveOrReplaceFile(sourcePath, destinationPath))
			{
				string text = EditorCompilation.MDBPath(sourcePath);
				string text2 = EditorCompilation.MDBPath(destinationPath);
				if (File.Exists(text))
				{
					EditorCompilation.MoveOrReplaceFile(text, text2);
				}
				else if (File.Exists(text2))
				{
					File.Delete(text2);
				}
				string text3 = EditorCompilation.PDBPath(sourcePath);
				string text4 = EditorCompilation.PDBPath(destinationPath);
				if (File.Exists(text3))
				{
					EditorCompilation.MoveOrReplaceFile(text3, text4);
				}
				else if (File.Exists(text4))
				{
					File.Delete(text4);
				}
			}
		}

		internal CustomScriptAssembly FindCustomScriptAssembly(string scriptPath)
		{
			EditorBuildRules.TargetAssembly customTargetAssembly = EditorBuildRules.GetCustomTargetAssembly(scriptPath, this.projectDirectory, this.customTargetAssemblies);
			return this.customScriptAssemblies.Single((CustomScriptAssembly a) => a.Name == Path.GetFileNameWithoutExtension(customTargetAssembly.Filename));
		}

		public bool CompileScripts(EditorScriptCompilationOptions definesOptions, BuildTargetGroup platformGroup, BuildTarget platform)
		{
			ScriptAssemblySettings scriptAssemblySettings = this.CreateScriptAssemblySettings(platformGroup, platform, definesOptions);
			BuildFlags buildFlags = BuildFlags.None;
			if ((definesOptions & EditorScriptCompilationOptions.BuildingForEditor) == EditorScriptCompilationOptions.BuildingForEditor)
			{
				buildFlags |= BuildFlags.BuildingForEditor;
			}
			if ((definesOptions & EditorScriptCompilationOptions.BuildingDevelopmentBuild) == EditorScriptCompilationOptions.BuildingDevelopmentBuild)
			{
				buildFlags |= BuildFlags.BuildingDevelopmentBuild;
			}
			EditorBuildRules.TargetAssembly[] array = null;
			bool result = this.CompileScripts(scriptAssemblySettings, EditorCompilation.EditorTempPath, buildFlags, ref array);
			if (array != null)
			{
				EditorBuildRules.TargetAssembly[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					EditorBuildRules.TargetAssembly targetAssembly = array2[i];
					CustomScriptAssembly customScriptAssembly = this.customScriptAssemblies.Single((CustomScriptAssembly a) => a.Name == Path.GetFileNameWithoutExtension(targetAssembly.Filename));
					string text = customScriptAssembly.FilePath;
					if (text.StartsWith(this.projectDirectory))
					{
						text = text.Substring(this.projectDirectory.Length);
					}
					Debug.LogWarning(string.Format("Script assembly '{0}' has not been compiled. Folder containing assembly definition file '{1}' contains script files for different script languages. Folder must only contain script files for one script language.", targetAssembly.Filename, text));
				}
			}
			return result;
		}

		internal bool CompileScripts(ScriptAssemblySettings scriptAssemblySettings, string tempBuildDirectory, BuildFlags buildflags, ref EditorBuildRules.TargetAssembly[] notCompiledTargetAssemblies)
		{
			this.DeleteUnusedAssemblies();
			this.allScripts.RemoveWhere((string path) => !File.Exists(Path.Combine(this.projectDirectory, path)));
			this.StopAllCompilation();
			if (!Directory.Exists(scriptAssemblySettings.OutputDirectory))
			{
				Directory.CreateDirectory(scriptAssemblySettings.OutputDirectory);
			}
			if (!Directory.Exists(tempBuildDirectory))
			{
				Directory.CreateDirectory(tempBuildDirectory);
			}
			IEnumerable<string> enumerable = (!this.areAllScriptsDirty) ? this.dirtyScripts.ToArray<string>() : this.allScripts.ToArray<string>();
			this.areAllScriptsDirty = false;
			this.dirtyScripts.Clear();
			bool result;
			if (!enumerable.Any<string>() && this.runScriptUpdaterAssemblies.Count == 0)
			{
				result = false;
			}
			else
			{
				EditorBuildRules.CompilationAssemblies assemblies = new EditorBuildRules.CompilationAssemblies
				{
					UnityAssemblies = this.unityAssemblies,
					PrecompiledAssemblies = this.precompiledAssemblies,
					CustomTargetAssemblies = this.customTargetAssemblies,
					EditorAssemblyReferences = ModuleUtils.GetAdditionalReferencesForUserScripts()
				};
				EditorBuildRules.GenerateChangedScriptAssembliesArgs generateChangedScriptAssembliesArgs = new EditorBuildRules.GenerateChangedScriptAssembliesArgs
				{
					AllSourceFiles = this.allScripts,
					DirtySourceFiles = enumerable,
					ProjectDirectory = this.projectDirectory,
					BuildFlags = buildflags,
					Settings = scriptAssemblySettings,
					Assemblies = assemblies,
					RunUpdaterAssemblies = this.runScriptUpdaterAssemblies
				};
				ScriptAssembly[] array = EditorBuildRules.GenerateChangedScriptAssemblies(generateChangedScriptAssembliesArgs);
				notCompiledTargetAssemblies = generateChangedScriptAssembliesArgs.NotCompiledTargetAssemblies.ToArray<EditorBuildRules.TargetAssembly>();
				if (!array.Any<ScriptAssembly>())
				{
					result = false;
				}
				else
				{
					this.compilationTask = new CompilationTask(array, tempBuildDirectory, buildflags, SystemInfo.processorCount);
					this.compilationTask.OnCompilationStarted += delegate(ScriptAssembly assembly, int phase)
					{
						Console.WriteLine("- Starting compile {0}", Path.Combine(scriptAssemblySettings.OutputDirectory, assembly.Filename));
					};
					IEnumerable<MonoIsland> compilingMonoIslands = from i in this.GetAllMonoIslands()
					where 0 < i._files.Length
					select i;
					this.compilationTask.OnCompilationFinished += delegate(ScriptAssembly assembly, List<CompilerMessage> messages)
					{
						Console.WriteLine("- Finished compile {0}", Path.Combine(scriptAssemblySettings.OutputDirectory, assembly.Filename));
						if (this.runScriptUpdaterAssemblies.Contains(assembly.Filename))
						{
							this.runScriptUpdaterAssemblies.Remove(assembly.Filename);
						}
						if (!messages.Any((CompilerMessage m) => m.type == CompilerMessageType.Error))
						{
							string engineAssemblyPath = InternalEditorUtility.GetEngineAssemblyPath();
							string unityUNet = EditorApplication.applicationContentsPath + "/UnityExtensions/Unity/Networking/UnityEngine.Networking.dll";
							if (!Weaver.WeaveUnetFromEditor(compilingMonoIslands, Path.Combine(tempBuildDirectory, assembly.Filename), Path.Combine(EditorCompilation.EditorTempPath, assembly.Filename), engineAssemblyPath, unityUNet, (buildflags & BuildFlags.BuildingForEditor) != BuildFlags.None))
							{
								messages.Add(new CompilerMessage
								{
									message = "UNet Weaver failed",
									type = CompilerMessageType.Error,
									file = assembly.FullPath,
									line = -1,
									column = -1
								});
								this.StopAllCompilation();
							}
							else
							{
								EditorCompilation.CopyAssembly(Path.Combine(tempBuildDirectory, assembly.Filename), assembly.FullPath);
							}
						}
					};
					this.compilationTask.Poll();
					result = true;
				}
			}
			return result;
		}

		public bool DoesProjectFolderHaveAnyDirtyScripts()
		{
			bool result;
			if (this.dirtyScripts.Count > 0)
			{
				result = true;
			}
			else if (!this.areAllScriptsDirty)
			{
				result = false;
			}
			else
			{
				this.allScripts.RemoveWhere((string path) => !File.Exists(Path.Combine(this.projectDirectory, path)));
				result = (this.allScripts.Count > 0);
			}
			return result;
		}

		public bool DoesProjectFolderHaveAnyScripts()
		{
			return this.allScripts != null && this.allScripts.Count > 0;
		}

		private ScriptAssemblySettings CreateScriptAssemblySettings(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget, EditorScriptCompilationOptions definesOptions)
		{
			string[] compilationDefines = InternalEditorUtility.GetCompilationDefines(definesOptions, buildTargetGroup, buildTarget);
			return new ScriptAssemblySettings
			{
				BuildTarget = buildTarget,
				BuildTargetGroup = buildTargetGroup,
				OutputDirectory = EditorCompilation.EditorAssemblyPath,
				Defines = compilationDefines,
				ApiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(buildTargetGroup),
				FilenameSuffix = this.assemblySuffix
			};
		}

		private ScriptAssemblySettings CreateEditorScriptAssemblySettings(EditorScriptCompilationOptions defines)
		{
			return this.CreateScriptAssemblySettings(EditorUserBuildSettings.activeBuildTargetGroup, EditorUserBuildSettings.activeBuildTarget, defines);
		}

		public EditorCompilation.AssemblyCompilerMessages[] GetCompileMessages()
		{
			EditorCompilation.AssemblyCompilerMessages[] result;
			if (this.compilationTask == null)
			{
				result = null;
			}
			else
			{
				EditorCompilation.AssemblyCompilerMessages[] array = new EditorCompilation.AssemblyCompilerMessages[this.compilationTask.CompilerMessages.Count];
				int num = 0;
				foreach (KeyValuePair<ScriptAssembly, CompilerMessage[]> current in this.compilationTask.CompilerMessages)
				{
					ScriptAssembly key = current.Key;
					CompilerMessage[] value = current.Value;
					array[num++] = new EditorCompilation.AssemblyCompilerMessages
					{
						assemblyFilename = key.Filename,
						messages = value
					};
				}
				Array.Sort<EditorCompilation.AssemblyCompilerMessages>(array, (EditorCompilation.AssemblyCompilerMessages m1, EditorCompilation.AssemblyCompilerMessages m2) => string.Compare(m1.assemblyFilename, m2.assemblyFilename));
				result = array;
			}
			return result;
		}

		public bool IsCompilationPending()
		{
			return this.DoesProjectFolderHaveAnyDirtyScripts() || this.runScriptUpdaterAssemblies.Count<string>() > 0;
		}

		public bool IsCompiling()
		{
			return this.IsCompilationTaskCompiling() || this.IsCompilationPending();
		}

		private bool IsCompilationTaskCompiling()
		{
			return this.compilationTask != null && this.compilationTask.IsCompiling;
		}

		public void StopAllCompilation()
		{
			if (this.compilationTask != null)
			{
				this.compilationTask.Stop();
				this.compilationTask = null;
			}
		}

		public EditorCompilation.CompileStatus TickCompilationPipeline(EditorScriptCompilationOptions options, BuildTargetGroup platformGroup, BuildTarget platform)
		{
			EditorCompilation.CompileStatus result;
			if (!this.IsCompilationTaskCompiling() && this.IsCompilationPending())
			{
				if (this.CompileScripts(options, platformGroup, platform))
				{
					result = EditorCompilation.CompileStatus.CompilationStarted;
					return result;
				}
			}
			if (this.IsCompilationTaskCompiling())
			{
				if (this.compilationTask.Poll())
				{
					result = ((this.compilationTask != null && !this.compilationTask.CompileErrors) ? EditorCompilation.CompileStatus.CompilationComplete : EditorCompilation.CompileStatus.CompilationFailed);
				}
				else
				{
					result = EditorCompilation.CompileStatus.Compiling;
				}
			}
			else
			{
				result = EditorCompilation.CompileStatus.Idle;
			}
			return result;
		}

		private string AssemblyFilenameWithSuffix(string assemblyFilename)
		{
			string result;
			if (!string.IsNullOrEmpty(this.assemblySuffix))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assemblyFilename);
				string extension = Path.GetExtension(assemblyFilename);
				result = fileNameWithoutExtension + this.assemblySuffix + extension;
			}
			else
			{
				result = assemblyFilename;
			}
			return result;
		}

		public EditorCompilation.TargetAssemblyInfo[] GetTargetAssemblies()
		{
			EditorBuildRules.TargetAssembly[] predefinedTargetAssemblies = EditorBuildRules.GetPredefinedTargetAssemblies();
			EditorCompilation.TargetAssemblyInfo[] array = new EditorCompilation.TargetAssemblyInfo[predefinedTargetAssemblies.Length + ((this.customTargetAssemblies == null) ? 0 : this.customTargetAssemblies.Count<EditorBuildRules.TargetAssembly>())];
			for (int i = 0; i < predefinedTargetAssemblies.Length; i++)
			{
				array[i].Name = this.AssemblyFilenameWithSuffix(predefinedTargetAssemblies[i].Filename);
				array[i].Flags = predefinedTargetAssemblies[i].Flags;
			}
			if (this.customTargetAssemblies != null)
			{
				for (int j = 0; j < this.customTargetAssemblies.Count<EditorBuildRules.TargetAssembly>(); j++)
				{
					int num = predefinedTargetAssemblies.Length + j;
					array[num].Name = this.AssemblyFilenameWithSuffix(this.customTargetAssemblies[j].Filename);
					array[num].Flags = this.customTargetAssemblies[j].Flags;
				}
			}
			return array;
		}

		public EditorCompilation.TargetAssemblyInfo GetTargetAssembly(string scriptPath)
		{
			EditorBuildRules.TargetAssembly targetAssembly = EditorBuildRules.GetTargetAssembly(scriptPath, this.projectDirectory, this.customTargetAssemblies);
			EditorCompilation.TargetAssemblyInfo result;
			result.Name = this.AssemblyFilenameWithSuffix(targetAssembly.Filename);
			result.Flags = targetAssembly.Flags;
			return result;
		}

		public EditorBuildRules.TargetAssembly GetTargetAssemblyDetails(string scriptPath)
		{
			return EditorBuildRules.GetTargetAssembly(scriptPath, this.projectDirectory, this.customTargetAssemblies);
		}

		private ScriptAssembly[] GetAllScriptAssemblies(BuildFlags buildFlags, EditorScriptCompilationOptions options)
		{
			return this.GetAllScriptAssemblies(buildFlags, options, this.unityAssemblies, this.precompiledAssemblies);
		}

		private ScriptAssembly[] GetAllScriptAssemblies(BuildFlags buildFlags, EditorScriptCompilationOptions options, PrecompiledAssembly[] unityAssembliesArg, PrecompiledAssembly[] precompiledAssembliesArg)
		{
			EditorBuildRules.CompilationAssemblies assemblies = new EditorBuildRules.CompilationAssemblies
			{
				UnityAssemblies = unityAssembliesArg,
				PrecompiledAssemblies = precompiledAssembliesArg,
				CustomTargetAssemblies = this.customTargetAssemblies,
				EditorAssemblyReferences = ModuleUtils.GetAdditionalReferencesForUserScripts()
			};
			ScriptAssemblySettings settings = this.CreateEditorScriptAssemblySettings(options);
			return EditorBuildRules.GetAllScriptAssemblies(this.allScripts, this.projectDirectory, buildFlags, settings, assemblies);
		}

		public MonoIsland[] GetAllMonoIslands()
		{
			return this.GetAllMonoIslands(this.unityAssemblies, this.precompiledAssemblies, BuildFlags.BuildingForEditor);
		}

		public MonoIsland[] GetAllMonoIslands(PrecompiledAssembly[] unityAssembliesArg, PrecompiledAssembly[] precompiledAssembliesArg, BuildFlags buildFlags)
		{
			EditorScriptCompilationOptions options = ((buildFlags & BuildFlags.BuildingForEditor) == BuildFlags.None) ? EditorScriptCompilationOptions.BuildingEmpty : EditorScriptCompilationOptions.BuildingForEditor;
			ScriptAssembly[] allScriptAssemblies = this.GetAllScriptAssemblies(buildFlags, options, unityAssembliesArg, precompiledAssembliesArg);
			MonoIsland[] array = new MonoIsland[allScriptAssemblies.Length];
			for (int i = 0; i < allScriptAssemblies.Length; i++)
			{
				array[i] = allScriptAssemblies[i].ToMonoIsland(BuildFlags.BuildingForEditor, EditorCompilation.EditorTempPath);
			}
			return array;
		}
	}
}
