using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
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
				string text = Path.Combine(dir, current);
				if (!this._assemblyFileNames.Contains(current))
				{
					string arg_4D_0 = text;
					ReaderParameters readerParameters = new ReaderParameters();
					readerParameters.set_AssemblyResolver(assemblyResolver);
					AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(arg_4D_0, readerParameters);
					if (!ignoreSystemDlls || !AssemblyReferenceChecker.IsIgnoredSystemDll(assemblyDefinition.get_Name().get_Name()))
					{
						this._assemblyFileNames.Add(current);
						this._assemblyDefinitions.Add(assemblyDefinition);
						using (Collection<AssemblyNameReference>.Enumerator enumerator2 = assemblyDefinition.get_MainModule().get_AssemblyReferences().GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								AssemblyNameReference current2 = enumerator2.get_Current();
								string text2 = current2.get_Name() + ".dll";
								if (!this._assemblyFileNames.Contains(text2))
								{
									this.CollectReferencesFromRootsRecursive(dir, new string[]
									{
										text2
									}, ignoreSystemDlls);
								}
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
					string arg_74_0 = text;
					ReaderParameters readerParameters = new ReaderParameters();
					readerParameters.set_AssemblyResolver(assemblyResolver);
					AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(arg_74_0, readerParameters);
					if (!ignoreSystemDlls || !AssemblyReferenceChecker.IsIgnoredSystemDll(assemblyDefinition.get_Name().get_Name()))
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
				using (Collection<TypeDefinition>.Enumerator enumerator2 = current.get_MainModule().get_Types().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TypeDefinition current2 = enumerator2.get_Current();
						this.CollectReferencedAndDefinedMethods(current2);
					}
				}
			}
		}

		internal void CollectReferencedAndDefinedMethods(TypeDefinition type)
		{
			if (this._updateProgressAction != null)
			{
				this._updateProgressAction();
			}
			using (Collection<TypeDefinition>.Enumerator enumerator = type.get_NestedTypes().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TypeDefinition current = enumerator.get_Current();
					this.CollectReferencedAndDefinedMethods(current);
				}
			}
			using (Collection<MethodDefinition>.Enumerator enumerator2 = type.get_Methods().GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					MethodDefinition current2 = enumerator2.get_Current();
					if (current2.get_HasBody())
					{
						using (Collection<Instruction>.Enumerator enumerator3 = current2.get_Body().get_Instructions().GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								Instruction current3 = enumerator3.get_Current();
								if (OpCodes.Call == current3.get_OpCode())
								{
									this._referencedMethods.Add(current3.get_Operand().ToString());
								}
							}
						}
						this._definedMethods.Add(current2.ToString());
						this.HasMouseEvent |= this.MethodIsMouseEvent(current2);
					}
				}
			}
		}

		private bool MethodIsMouseEvent(MethodDefinition method)
		{
			return (method.get_Name() == "OnMouseDown" || method.get_Name() == "OnMouseDrag" || method.get_Name() == "OnMouseEnter" || method.get_Name() == "OnMouseExit" || method.get_Name() == "OnMouseOver" || method.get_Name() == "OnMouseUp" || method.get_Name() == "OnMouseUpAsButton") && method.get_Parameters().get_Count() == 0 && this.InheritsFromMonoBehaviour(method.get_DeclaringType());
		}

		private bool InheritsFromMonoBehaviour(TypeReference type)
		{
			bool result;
			if (type.get_Namespace() == "UnityEngine" && type.get_Name() == "MonoBehaviour")
			{
				result = true;
			}
			else
			{
				TypeDefinition typeDefinition = type.Resolve();
				result = (typeDefinition.get_BaseType() != null && this.InheritsFromMonoBehaviour(typeDefinition.get_BaseType()));
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
			return this._referencedMethods.Any((string item) => item.Contains(methodName));
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
				if (!ignoreSystemDlls || !AssemblyReferenceChecker.IsIgnoredSystemDll(current.get_Name().get_Name()))
				{
					AssemblyDefinition[] assemblies = new AssemblyDefinition[]
					{
						current
					};
					HashSet<string> source = MonoAOTRegistration.BuildReferencedTypeList(assemblies);
					if (source.Any((string item) => item.StartsWith(klass)))
					{
						result = current.get_Name().get_Name();
						return result;
					}
				}
			}
			result = null;
			return result;
		}

		public static bool IsIgnoredSystemDll(string name)
		{
			return name.StartsWith("System") || name.Equals("UnityEngine") || name.Equals("UnityEngine.Networking") || name.Equals("Mono.Posix");
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
