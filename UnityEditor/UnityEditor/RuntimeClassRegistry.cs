using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

		protected Dictionary<int, string> nativeClasses = new Dictionary<int, string>();

		protected HashSet<string> monoClasses = new HashSet<string>();

		protected HashSet<string> monoBaseClasses = new HashSet<string>();

		protected Dictionary<string, string[]> m_UsedTypesPerUserAssembly = new Dictionary<string, string[]>();

		protected Dictionary<int, List<string>> classScenes = new Dictionary<int, List<string>>();

		protected UnityType objectUnityType = null;

		protected Dictionary<int, string> allNativeClasses = new Dictionary<int, string>();

		internal List<RuntimeClassRegistry.MethodDescription> m_MethodsToPreserve = new List<RuntimeClassRegistry.MethodDescription>();

		internal List<string> m_UserAssemblies = new List<string>();

		protected Dictionary<string, string> retentionLevel = new Dictionary<string, string>();

		protected Dictionary<string, string> functionalityGroups = new Dictionary<string, string>();

		protected Dictionary<string, HashSet<string>> groupNativeDependencies = new Dictionary<string, HashSet<string>>();

		protected Dictionary<string, HashSet<string>> groupManagedDependencies = new Dictionary<string, HashSet<string>>();

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
				if (!this.functionalityGroups.ContainsValue(name))
				{
					this.nativeClasses[ID] = name;
				}
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

		public void AddMonoClass(string className)
		{
			this.monoClasses.Add(className);
		}

		public void AddMonoClasses(List<string> classes)
		{
			foreach (string current in classes)
			{
				this.AddMonoClass(current);
			}
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
				this.nativeClasses[unityType.persistentTypeID] = className;
			}
		}

		protected void SynchronizeMonoToNativeClasses()
		{
			foreach (string current in this.monoClasses)
			{
				this.AddNativeClassFromName(current);
			}
		}

		protected void SynchronizeNativeToMonoClasses()
		{
			foreach (string current in this.nativeClasses.Values)
			{
				this.AddMonoClass(current);
			}
		}

		public void SynchronizeClasses()
		{
			this.SynchronizeMonoToNativeClasses();
			this.SynchronizeNativeToMonoClasses();
			this.InjectFunctionalityGroupDependencies();
			this.SynchronizeMonoToNativeClasses();
		}

		public List<string> GetAllNativeClassesAsString()
		{
			return new List<string>(this.nativeClasses.Values);
		}

		public List<string> GetAllNativeClassesIncludingManagersAsString()
		{
			return new List<string>(this.allNativeClasses.Values);
		}

		public List<string> GetAllManagedClassesAsString()
		{
			return new List<string>(this.monoClasses);
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

		protected void InjectFunctionalityGroupDependencies()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string current in this.functionalityGroups.Keys)
			{
				foreach (string current2 in this.monoClasses)
				{
					if (this.groupManagedDependencies[current].Contains(current2) || this.groupNativeDependencies[current].Contains(current2))
					{
						hashSet.Add(current);
					}
				}
			}
			foreach (string current3 in hashSet)
			{
				foreach (string current4 in this.groupManagedDependencies[current3])
				{
					this.AddMonoClass(current4);
				}
				foreach (string current5 in this.groupNativeDependencies[current3])
				{
					this.AddNativeClassFromName(current5);
				}
			}
		}

		protected void AddFunctionalityGroup(string groupName, string managerClassName)
		{
			this.functionalityGroups.Add(groupName, managerClassName);
			this.groupManagedDependencies[groupName] = new HashSet<string>();
			this.groupNativeDependencies[groupName] = new HashSet<string>();
		}

		protected void AddNativeDependenciesForFunctionalityGroup(string groupName, string depClassName)
		{
			this.groupNativeDependencies[groupName].Add(depClassName);
		}

		protected void AddManagedDependenciesForFunctionalityGroup(string groupName, Type depClass)
		{
			this.AddManagedDependenciesForFunctionalityGroup(groupName, this.ResolveTypeName(depClass));
		}

		protected void AddManagedDependenciesForFunctionalityGroup(string groupName, string depClassName)
		{
			this.AddManagedDependenciesForFunctionalityGroup(groupName, depClassName, null);
		}

		protected string ResolveTypeName(Type type)
		{
			string fullName = type.FullName;
			return fullName.Substring(fullName.LastIndexOf(".") + 1).Replace("+", "/");
		}

		protected void AddManagedDependenciesForFunctionalityGroup(string groupName, Type depClass, string retain)
		{
			this.AddManagedDependenciesForFunctionalityGroup(groupName, this.ResolveTypeName(depClass), retain);
		}

		protected void AddManagedDependenciesForFunctionalityGroup(string groupName, string depClassName, string retain)
		{
			this.groupManagedDependencies[groupName].Add(depClassName);
			if (retain != null)
			{
				this.SetRetentionLevel(depClassName, retain);
			}
		}

		protected void SetRetentionLevel(string className, string level)
		{
			this.retentionLevel[className] = level;
		}

		public string GetRetentionLevel(string className)
		{
			string result;
			if (this.retentionLevel.ContainsKey(className))
			{
				result = this.retentionLevel[className];
			}
			else
			{
				result = "fields";
			}
			return result;
		}

		protected void InitRuntimeClassRegistry()
		{
			BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(this.buildTarget);
			this.AddFunctionalityGroup("Runtime", "[no manager]");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "GameObject");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "Material");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "PreloadData");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "PlayerSettings");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "InputManager");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "BuildSettings");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "GraphicsSettings");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "QualitySettings");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "MonoManager");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "AudioManager");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "ScriptMapper");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "DelayedCallManager");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "TimeManager");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "Cubemap");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "Texture3D");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "Texture2DArray");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "LODGroup");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GameObject), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Transform), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Mesh), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SkinnedMeshRenderer), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(MeshRenderer), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(UnityException), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Resolution));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(LayerMask));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SerializeField));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(WaitForSeconds));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(WaitForSecondsRealtime));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(WaitForFixedUpdate));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(WaitForEndOfFrame));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AssetBundle), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AssetBundleRequest));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Event), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(HideInInspector));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SerializePrivateVariables));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SerializeField));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Font), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUIStyle));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUISkin), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUITargetAttribute), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUI), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(TextGenerator), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SendMouseEvents), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SetupCoroutine), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Coroutine));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AttributeHelperEngine), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(StackTraceUtility), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUIUtility), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUI), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Application), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Animation), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AnimationClip), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AnimationEvent));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AsyncOperation));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Resources), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(CacheIndex));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Keyframe));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(RenderTexture));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AnimationCurve), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(BoneWeight));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Particle));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SliderState), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GUIScrollGroup), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(ScrollViewState), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(TextEditor), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(ClassLibraryInitializer), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AssetBundleCreateRequest), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "ImageEffectTransformsToLDR");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "ImageEffectOpaque");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Gradient), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GradientColorKey));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(GradientAlphaKey));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Canvas), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(RectTransform), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AssemblyIsEditorAssembly), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Camera), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(CullingGroup), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(StateMachineBehaviour), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "Networking.DownloadHandler", "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "Experimental.Director.Playable", "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "Experimental.Director.ScriptPlayable", "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "Experimental.Director.GenericMixerPlayable", "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SharedBetweenAnimatorsAttribute), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AnimatorStateInfo), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AnimatorTransitionInfo), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AnimatorClipInfo), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(SkeletonBone), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(HumanBone), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(UIVertex), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(UICharInfo), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(UILineInfo), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AudioClip), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AudioMixer), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(AudioSettings), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "iPhone", "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJNI", "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJNIHelper", "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "_AndroidJNIHelper", "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJavaObject", "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJavaClass", "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJavaRunnableProxy", "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "SamsungTV", "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(ISerializationCallbackReceiver), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "UnhandledExceptionHandler", "all");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", "Display", "all");
			if (buildTargetGroup == BuildTargetGroup.Android)
			{
				this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJNI", "all");
				this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJNIHelper", "all");
				this.AddManagedDependenciesForFunctionalityGroup("Runtime", "_AndroidJNIHelper", "all");
				this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJavaObject", "all");
				this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJavaClass", "all");
				this.AddManagedDependenciesForFunctionalityGroup("Runtime", "AndroidJavaRunnableProxy", "all");
			}
			if (buildTargetGroup == BuildTargetGroup.SamsungTV)
			{
				this.AddManagedDependenciesForFunctionalityGroup("Runtime", "SamsungTV", "all");
			}
			if (buildTargetGroup == BuildTargetGroup.iPhone)
			{
				this.AddManagedDependenciesForFunctionalityGroup("Runtime", "iPhoneKeyboard");
			}
			if (buildTargetGroup == BuildTargetGroup.iPhone || (buildTargetGroup == BuildTargetGroup.Standalone && (this.buildTarget == BuildTarget.StandaloneOSXIntel || this.buildTarget == BuildTarget.StandaloneOSXIntel64 || this.buildTarget == BuildTarget.StandaloneOSXUniversal)))
			{
				this.AddManagedDependenciesForFunctionalityGroup("Runtime", "SocialPlatforms.GameCenter.GameCenterPlatform", "all");
				this.AddManagedDependenciesForFunctionalityGroup("Runtime", "SocialPlatforms.GameCenter.GcLeaderboard", "all");
			}
			if (buildTargetGroup == BuildTargetGroup.iPhone || buildTargetGroup == BuildTargetGroup.Android || buildTargetGroup == BuildTargetGroup.WP8 || buildTargetGroup == BuildTargetGroup.WSA || buildTargetGroup == BuildTargetGroup.Tizen)
			{
				this.AddManagedDependenciesForFunctionalityGroup("Runtime", "TouchScreenKeyboard");
			}
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "NetworkManager");
			this.AddNativeDependenciesForFunctionalityGroup("Runtime", "NetworkView");
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Network));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(NetworkMessageInfo));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(RPC));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(HostData));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(BitStream));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(NetworkPlayer));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(NetworkViewID));
			this.AddManagedDependenciesForFunctionalityGroup("Runtime", typeof(Ping), "all");
			this.AddFunctionalityGroup("Physics", "PhysicsManager");
			this.AddNativeDependenciesForFunctionalityGroup("Physics", "PhysicsManager");
			this.AddNativeDependenciesForFunctionalityGroup("Physics", "Rigidbody");
			this.AddNativeDependenciesForFunctionalityGroup("Physics", "Collider");
			this.AddManagedDependenciesForFunctionalityGroup("Physics", typeof(ControllerColliderHit));
			this.AddManagedDependenciesForFunctionalityGroup("Physics", typeof(RaycastHit));
			this.AddManagedDependenciesForFunctionalityGroup("Physics", typeof(Collision));
			this.AddManagedDependenciesForFunctionalityGroup("Physics", typeof(MeshCollider));
			this.AddFunctionalityGroup("Physics2D", "Physics2DSettings");
			this.AddNativeDependenciesForFunctionalityGroup("Physics2D", "Physics2DSettings");
			this.AddNativeDependenciesForFunctionalityGroup("Physics2D", "Rigidbody2D");
			this.AddNativeDependenciesForFunctionalityGroup("Physics2D", "Collider2D");
			this.AddNativeDependenciesForFunctionalityGroup("Physics2D", "Joint2D");
			this.AddNativeDependenciesForFunctionalityGroup("Physics2D", "PhysicsMaterial2D");
			this.AddManagedDependenciesForFunctionalityGroup("Physics2D", typeof(RaycastHit2D));
			this.AddManagedDependenciesForFunctionalityGroup("Physics2D", typeof(Collision2D));
			this.AddManagedDependenciesForFunctionalityGroup("Physics2D", typeof(JointMotor2D));
			this.AddManagedDependenciesForFunctionalityGroup("Physics2D", typeof(JointAngleLimits2D));
			this.AddManagedDependenciesForFunctionalityGroup("Physics2D", typeof(JointTranslationLimits2D));
			this.AddManagedDependenciesForFunctionalityGroup("Physics2D", typeof(JointSuspension2D));
			this.AddFunctionalityGroup("Terrain", "Terrain");
			this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(Terrain), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(TerrainData), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(TerrainCollider), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(DetailPrototype), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(TreePrototype), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(TreeInstance), "all");
			this.AddManagedDependenciesForFunctionalityGroup("Terrain", typeof(SplatPrototype), "all");
			this.AddFunctionalityGroup("Shuriken", "ParticleSystem");
			this.AddManagedDependenciesForFunctionalityGroup("Shuriken", typeof(ParticleSystem));
			this.AddManagedDependenciesForFunctionalityGroup("Shuriken", typeof(ParticleSystemRenderer));
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
