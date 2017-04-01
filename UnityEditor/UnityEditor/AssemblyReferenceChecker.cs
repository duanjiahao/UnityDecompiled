using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnityEditor
{
	internal class AssemblyReferenceChecker
	{
		private readonly HashSet<string> _referencedMethods = new HashSet<string>();

		private HashSet<string> _referencedTypes = new HashSet<string>();

		private readonly HashSet<string> _userReferencedMethods = new HashSet<string>();

		private readonly HashSet<string> _definedMethods = new HashSet<string>();

		private HashSet<AssemblyDefinition> _assemblyDefinitions = new HashSet<AssemblyDefinition>();

		private readonly HashSet<string> _assemblyFileNames = new HashSet<string>();

		private DateTime _startTime = DateTime.MinValue;

		private float _progressValue = 0f;

		private Action _updateProgressAction;

		public bool HasMouseEvent
		{
			get;
			private set;
		}

		public AssemblyReferenceChecker()
		{
			this.HasMouseEvent = false;
			this._updateProgressAction = new Action(this.DisplayProgress);
		}

		public static AssemblyReferenceChecker AssemblyReferenceCheckerWithUpdateProgressAction(Action action)
		{
			return new AssemblyReferenceChecker
			{
				_updateProgressAction = action
			};
		}

		private void CollectReferencesFromRootsRecursive(string dir, IEnumerable<string> roots, bool ignoreSystemDlls)
		{
			DefaultAssemblyResolver assemblyResolver = AssemblyReferenceChecker.AssemblyResolverFor(dir);
			foreach (string current in roots)
			{
				string fileName = Path.Combine(dir, current);
				if (!this._assemblyFileNames.Contains(current))
				{
					AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(fileName, new ReaderParameters
					{
						AssemblyResolver = assemblyResolver
					});
					if (!ignoreSystemDlls || !AssemblyReferenceChecker.IsIgnoredSystemDll(assemblyDefinition.Name.Name))
					{
						this._assemblyFileNames.Add(current);
						this._assemblyDefinitions.Add(assemblyDefinition);
						foreach (AssemblyNameReference current2 in assemblyDefinition.MainModule.AssemblyReferences)
						{
							string text = current2.Name + ".dll";
							if (!this._assemblyFileNames.Contains(text))
							{
								this.CollectReferencesFromRootsRecursive(dir, new string[]
								{
									text
								}, ignoreSystemDlls);
							}
						}
					}
				}
			}
		}

		public void CollectReferencesFromRoots(string dir, IEnumerable<string> roots, bool collectMethods, float progressValue, bool ignoreSystemDlls)
		{
			this._progressValue = progressValue;
			this.CollectReferencesFromRootsRecursive(dir, roots, ignoreSystemDlls);
			AssemblyDefinition[] array = this._assemblyDefinitions.ToArray<AssemblyDefinition>();
			this._referencedTypes = MonoAOTRegistration.BuildReferencedTypeList(array);
			if (collectMethods)
			{
				this.CollectReferencedAndDefinedMethods(array);
			}
		}

		public void CollectReferences(string path, bool collectMethods, float progressValue, bool ignoreSystemDlls)
		{
			this._progressValue = progressValue;
			this._assemblyDefinitions = new HashSet<AssemblyDefinition>();
			string[] array = (!Directory.Exists(path)) ? new string[0] : Directory.GetFiles(path);
			DefaultAssemblyResolver assemblyResolver = AssemblyReferenceChecker.AssemblyResolverFor(path);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (!(Path.GetExtension(text) != ".dll"))
				{
					AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(text, new ReaderParameters
					{
						AssemblyResolver = assemblyResolver
					});
					if (!ignoreSystemDlls || !AssemblyReferenceChecker.IsIgnoredSystemDll(assemblyDefinition.Name.Name))
					{
						this._assemblyFileNames.Add(Path.GetFileName(text));
						this._assemblyDefinitions.Add(assemblyDefinition);
					}
				}
			}
			AssemblyDefinition[] array3 = this._assemblyDefinitions.ToArray<AssemblyDefinition>();
			this._referencedTypes = MonoAOTRegistration.BuildReferencedTypeList(array3);
			if (collectMethods)
			{
				this.CollectReferencedAndDefinedMethods(array3);
			}
		}

		private void CollectReferencedAndDefinedMethods(IEnumerable<AssemblyDefinition> assemblyDefinitions)
		{
			foreach (AssemblyDefinition current in assemblyDefinitions)
			{
				bool isSystem = AssemblyReferenceChecker.IsIgnoredSystemDll(current.Name.Name);
				foreach (TypeDefinition current2 in current.MainModule.Types)
				{
					this.CollectReferencedAndDefinedMethods(current2, isSystem);
				}
			}
		}

		internal void CollectReferencedAndDefinedMethods(TypeDefinition type)
		{
			this.CollectReferencedAndDefinedMethods(type, false);
		}

		internal void CollectReferencedAndDefinedMethods(TypeDefinition type, bool isSystem)
		{
			if (this._updateProgressAction != null)
			{
				this._updateProgressAction();
			}
			foreach (TypeDefinition current in type.NestedTypes)
			{
				this.CollectReferencedAndDefinedMethods(current, isSystem);
			}
			foreach (MethodDefinition current2 in type.Methods)
			{
				if (current2.HasBody)
				{
					foreach (Instruction current3 in current2.Body.Instructions)
					{
						if (OpCodes.Call == current3.OpCode)
						{
							string item = current3.Operand.ToString();
							if (!isSystem)
							{
								this._userReferencedMethods.Add(item);
							}
							this._referencedMethods.Add(item);
						}
					}
					this._definedMethods.Add(current2.ToString());
					this.HasMouseEvent |= this.MethodIsMouseEvent(current2);
				}
			}
		}

		private bool MethodIsMouseEvent(MethodDefinition method)
		{
			return (method.Name == "OnMouseDown" || method.Name == "OnMouseDrag" || method.Name == "OnMouseEnter" || method.Name == "OnMouseExit" || method.Name == "OnMouseOver" || method.Name == "OnMouseUp" || method.Name == "OnMouseUpAsButton") && method.Parameters.Count == 0 && this.InheritsFromMonoBehaviour(method.DeclaringType);
		}

		private bool InheritsFromMonoBehaviour(TypeReference type)
		{
			bool result;
			if (type.Namespace == "UnityEngine" && type.Name == "MonoBehaviour")
			{
				result = true;
			}
			else
			{
				TypeDefinition typeDefinition = type.Resolve();
				result = (typeDefinition.BaseType != null && this.InheritsFromMonoBehaviour(typeDefinition.BaseType));
			}
			return result;
		}

		private void DisplayProgress()
		{
			TimeSpan timeSpan = DateTime.Now - this._startTime;
			string[] array = new string[]
			{
				"Fetching assembly references",
				"Building list of referenced assemblies..."
			};
			if (timeSpan.TotalMilliseconds >= 100.0)
			{
				if (EditorUtility.DisplayCancelableProgressBar(array[0], array[1], this._progressValue))
				{
					throw new OperationCanceledException();
				}
				this._startTime = DateTime.Now;
			}
		}

		public bool HasReferenceToMethod(string methodName)
		{
			return this.HasReferenceToMethod(methodName, false);
		}

		public bool HasReferenceToMethod(string methodName, bool ignoreSystemDlls)
		{
			return ignoreSystemDlls ? this._userReferencedMethods.Any((string item) => item.Contains(methodName)) : this._referencedMethods.Any((string item) => item.Contains(methodName));
		}

		public bool HasDefinedMethod(string methodName)
		{
			return this._definedMethods.Any((string item) => item.Contains(methodName));
		}

		public bool HasReferenceToType(string typeName)
		{
			return this._referencedTypes.Any((string item) => item.StartsWith(typeName));
		}

		public AssemblyDefinition[] GetAssemblyDefinitions()
		{
			return this._assemblyDefinitions.ToArray<AssemblyDefinition>();
		}

		public string[] GetAssemblyFileNames()
		{
			return this._assemblyFileNames.ToArray<string>();
		}

		public string WhoReferencesClass(string klass, bool ignoreSystemDlls)
		{
			string result;
			foreach (AssemblyDefinition current in this._assemblyDefinitions)
			{
				if (!ignoreSystemDlls || !AssemblyReferenceChecker.IsIgnoredSystemDll(current.Name.Name))
				{
					AssemblyDefinition[] assemblies = new AssemblyDefinition[]
					{
						current
					};
					HashSet<string> source = MonoAOTRegistration.BuildReferencedTypeList(assemblies);
					if (source.Any((string item) => item.StartsWith(klass)))
					{
						result = current.Name.Name;
						return result;
					}
				}
			}
			result = null;
			return result;
		}

		public static bool IsIgnoredSystemDll(string name)
		{
			return name.StartsWith("System") || name.Equals("UnityEngine") || name.Equals("UnityEngine.Networking") || name.Equals("Mono.Posix") || name.Equals("Moq");
		}

		public static bool GetScriptsHaveMouseEvents(string path)
		{
			AssemblyReferenceChecker assemblyReferenceChecker = new AssemblyReferenceChecker();
			assemblyReferenceChecker.CollectReferences(path, true, 0f, true);
			return assemblyReferenceChecker.HasMouseEvent;
		}

		private static DefaultAssemblyResolver AssemblyResolverFor(string path)
		{
			DefaultAssemblyResolver defaultAssemblyResolver = new DefaultAssemblyResolver();
			if (File.Exists(path) || Directory.Exists(path))
			{
				FileAttributes attributes = File.GetAttributes(path);
				if ((attributes & FileAttributes.Directory) != FileAttributes.Directory)
				{
					path = Path.GetDirectoryName(path);
				}
				defaultAssemblyResolver.AddSearchDirectory(Path.GetFullPath(path));
			}
			return defaultAssemblyResolver;
		}
	}
}
