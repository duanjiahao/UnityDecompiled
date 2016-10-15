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
		private HashSet<string> referencedMethods = new HashSet<string>();

		private HashSet<string> referencedTypes = new HashSet<string>();

		private HashSet<string> definedMethods = new HashSet<string>();

		private HashSet<AssemblyDefinition> assemblyDefinitions = new HashSet<AssemblyDefinition>();

		private HashSet<string> assemblyFileNames = new HashSet<string>();

		private DateTime startTime = DateTime.MinValue;

		private void CollectReferencesFromRootsRecursive(string dir, IEnumerable<string> roots, bool ignoreSystemDlls)
		{
			foreach (string current in roots)
			{
				string fileName = Path.Combine(dir, current);
				if (!this.assemblyFileNames.Contains(current))
				{
					AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(fileName);
					if (!ignoreSystemDlls || !AssemblyReferenceChecker.IsIgnoredSystemDll(assemblyDefinition.Name.Name))
					{
						this.assemblyFileNames.Add(current);
						this.assemblyDefinitions.Add(assemblyDefinition);
						foreach (AssemblyNameReference current2 in assemblyDefinition.MainModule.AssemblyReferences)
						{
							string text = current2.Name + ".dll";
							if (!this.assemblyFileNames.Contains(text))
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

		public void CollectReferencesFromRoots(string dir, IEnumerable<string> roots, bool withMethods, float progressValue, bool ignoreSystemDlls)
		{
			this.CollectReferencesFromRootsRecursive(dir, roots, ignoreSystemDlls);
			AssemblyDefinition[] array = this.assemblyDefinitions.ToArray<AssemblyDefinition>();
			this.referencedTypes = MonoAOTRegistration.BuildReferencedTypeList(array);
			if (withMethods)
			{
				this.CollectReferencedMethods(array, this.referencedMethods, this.definedMethods, progressValue);
			}
		}

		public void CollectReferences(string path, bool withMethods, float progressValue, bool ignoreSystemDlls)
		{
			this.assemblyDefinitions = new HashSet<AssemblyDefinition>();
			string[] array = (!Directory.Exists(path)) ? new string[0] : Directory.GetFiles(path);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (!(Path.GetExtension(text) != ".dll"))
				{
					AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(text);
					if (!ignoreSystemDlls || !AssemblyReferenceChecker.IsIgnoredSystemDll(assemblyDefinition.Name.Name))
					{
						this.assemblyFileNames.Add(Path.GetFileName(text));
						this.assemblyDefinitions.Add(assemblyDefinition);
					}
				}
			}
			AssemblyDefinition[] array3 = this.assemblyDefinitions.ToArray<AssemblyDefinition>();
			this.referencedTypes = MonoAOTRegistration.BuildReferencedTypeList(array3);
			if (withMethods)
			{
				this.CollectReferencedMethods(array3, this.referencedMethods, this.definedMethods, progressValue);
			}
		}

		private void CollectReferencedMethods(AssemblyDefinition[] definitions, HashSet<string> referencedMethods, HashSet<string> definedMethods, float progressValue)
		{
			for (int i = 0; i < definitions.Length; i++)
			{
				AssemblyDefinition assemblyDefinition = definitions[i];
				foreach (TypeDefinition current in assemblyDefinition.MainModule.Types)
				{
					this.CollectReferencedMethods(current, referencedMethods, definedMethods, progressValue);
				}
			}
		}

		private void CollectReferencedMethods(TypeDefinition typ, HashSet<string> referencedMethods, HashSet<string> definedMethods, float progressValue)
		{
			this.DisplayProgress(progressValue);
			foreach (TypeDefinition current in typ.NestedTypes)
			{
				this.CollectReferencedMethods(current, referencedMethods, definedMethods, progressValue);
			}
			foreach (MethodDefinition current2 in typ.Methods)
			{
				if (current2.HasBody)
				{
					foreach (Instruction current3 in current2.Body.Instructions)
					{
						if (OpCodes.Call == current3.OpCode)
						{
							referencedMethods.Add(current3.Operand.ToString());
						}
					}
					definedMethods.Add(current2.ToString());
				}
			}
		}

		private void DisplayProgress(float progressValue)
		{
			TimeSpan timeSpan = DateTime.Now - this.startTime;
			string[] array = new string[]
			{
				"Fetching assembly references",
				"Building list of referenced assemblies..."
			};
			if (timeSpan.TotalMilliseconds >= 100.0)
			{
				if (EditorUtility.DisplayCancelableProgressBar(array[0], array[1], progressValue))
				{
					throw new OperationCanceledException();
				}
				this.startTime = DateTime.Now;
			}
		}

		public bool HasReferenceToMethod(string methodName)
		{
			return this.referencedMethods.Any((string item) => item.Contains(methodName));
		}

		public bool HasDefinedMethod(string methodName)
		{
			return this.definedMethods.Any((string item) => item.Contains(methodName));
		}

		public bool HasReferenceToType(string typeName)
		{
			return this.referencedTypes.Any((string item) => item.StartsWith(typeName));
		}

		public AssemblyDefinition[] GetAssemblyDefinitions()
		{
			return this.assemblyDefinitions.ToArray<AssemblyDefinition>();
		}

		public string[] GetAssemblyFileNames()
		{
			return this.assemblyFileNames.ToArray<string>();
		}

		public string WhoReferencesClass(string klass, bool ignoreSystemDlls)
		{
			foreach (AssemblyDefinition current in this.assemblyDefinitions)
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
						return current.Name.Name;
					}
				}
			}
			return null;
		}

		public static bool IsIgnoredSystemDll(string name)
		{
			return name.StartsWith("System") || name.Equals("UnityEngine") || name.Equals("UnityEngine.Networking") || name.Equals("Mono.Posix");
		}

		public static bool GetScriptsHaveMouseEvents(string path)
		{
			AssemblyReferenceChecker assemblyReferenceChecker = new AssemblyReferenceChecker();
			assemblyReferenceChecker.CollectReferences(path, true, 0f, true);
			return assemblyReferenceChecker.HasDefinedMethod("OnMouse");
		}
	}
}
