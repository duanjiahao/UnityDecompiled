using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor
{
	internal class RuntimeClassRegistry
	{
		internal class MethodDescription
		{
			public string assembly;

			public string fullTypeName;

			public string methodName;
		}

		protected BuildTarget buildTarget;

		protected HashSet<string> monoBaseClasses = new HashSet<string>();

		protected Dictionary<string, string[]> m_UsedTypesPerUserAssembly = new Dictionary<string, string[]>();

		protected Dictionary<int, List<string>> classScenes = new Dictionary<int, List<string>>();

		protected UnityType objectUnityType = null;

		protected Dictionary<int, string> allNativeClasses = new Dictionary<int, string>();

		internal List<RuntimeClassRegistry.MethodDescription> m_MethodsToPreserve = new List<RuntimeClassRegistry.MethodDescription>();

		internal List<string> m_UserAssemblies = new List<string>();

		public Dictionary<string, string[]> UsedTypePerUserAssembly
		{
			get
			{
				return this.m_UsedTypesPerUserAssembly;
			}
		}

		public List<string> GetScenesForClass(int ID)
		{
			List<string> result;
			if (!this.classScenes.ContainsKey(ID))
			{
				result = null;
			}
			else
			{
				result = this.classScenes[ID];
			}
			return result;
		}

		public void AddNativeClassID(int ID)
		{
			string name = UnityType.FindTypeByPersistentTypeID(ID).name;
			if (name.Length > 0)
			{
				this.allNativeClasses[ID] = name;
			}
		}

		public void SetUsedTypesInUserAssembly(string[] typeNames, string assemblyName)
		{
			this.m_UsedTypesPerUserAssembly[assemblyName] = typeNames;
		}

		public bool IsDLLUsed(string dll)
		{
			return this.m_UsedTypesPerUserAssembly == null || Array.IndexOf<string>(CodeStrippingUtils.UserAssemblies, dll) != -1 || this.m_UsedTypesPerUserAssembly.ContainsKey(dll);
		}

		protected void AddManagedBaseClass(string className)
		{
			this.monoBaseClasses.Add(className);
		}

		protected void AddNativeClassFromName(string className)
		{
			if (this.objectUnityType == null)
			{
				this.objectUnityType = UnityType.FindTypeByName("Object");
			}
			UnityType unityType = UnityType.FindTypeByName(className);
			if (unityType != null && unityType.persistentTypeID != this.objectUnityType.persistentTypeID)
			{
				this.allNativeClasses[unityType.persistentTypeID] = className;
			}
		}

		public List<string> GetAllNativeClassesIncludingManagersAsString()
		{
			return new List<string>(this.allNativeClasses.Values);
		}

		public List<string> GetAllManagedBaseClassesAsString()
		{
			return new List<string>(this.monoBaseClasses);
		}

		public static RuntimeClassRegistry Create()
		{
			return new RuntimeClassRegistry();
		}

		public void Initialize(int[] nativeClassIDs, BuildTarget buildTarget)
		{
			this.buildTarget = buildTarget;
			this.InitRuntimeClassRegistry();
			for (int i = 0; i < nativeClassIDs.Length; i++)
			{
				int iD = nativeClassIDs[i];
				this.AddNativeClassID(iD);
			}
		}

		public void SetSceneClasses(int[] nativeClassIDs, string scene)
		{
			for (int i = 0; i < nativeClassIDs.Length; i++)
			{
				int num = nativeClassIDs[i];
				this.AddNativeClassID(num);
				if (!this.classScenes.ContainsKey(num))
				{
					this.classScenes[num] = new List<string>();
				}
				this.classScenes[num].Add(scene);
			}
		}

		internal void AddMethodToPreserve(string assembly, string @namespace, string klassName, string methodName)
		{
			this.m_MethodsToPreserve.Add(new RuntimeClassRegistry.MethodDescription
			{
				assembly = assembly,
				fullTypeName = @namespace + ((@namespace.Length <= 0) ? "" : ".") + klassName,
				methodName = methodName
			});
		}

		internal List<RuntimeClassRegistry.MethodDescription> GetMethodsToPreserve()
		{
			return this.m_MethodsToPreserve;
		}

		internal void AddUserAssembly(string assembly)
		{
			if (!this.m_UserAssemblies.Contains(assembly))
			{
				this.m_UserAssemblies.Add(assembly);
			}
		}

		internal string[] GetUserAssemblies()
		{
			return this.m_UserAssemblies.ToArray();
		}

		protected void InitRuntimeClassRegistry()
		{
			BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(this.buildTarget);
			this.AddManagedBaseClass("UnityEngine.MonoBehaviour");
			this.AddManagedBaseClass("UnityEngine.ScriptableObject");
			if (buildTargetGroup == BuildTargetGroup.Android)
			{
				this.AddManagedBaseClass("UnityEngine.AndroidJavaProxy");
			}
			string[] dontStripClassNames = RuntimeInitializeOnLoadManager.dontStripClassNames;
			string[] array = dontStripClassNames;
			for (int i = 0; i < array.Length; i++)
			{
				string className = array[i];
				this.AddManagedBaseClass(className);
			}
		}
	}
}
