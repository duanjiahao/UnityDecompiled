using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Modules;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(PluginImporter))]
	internal class PluginImporterInspector : AssetImporterInspector
	{
		private delegate PluginImporterInspector.Compatibility ValueSwitcher(PluginImporterInspector.Compatibility value);

		internal enum Compatibility
		{
			Mixed = -1,
			NotCompatible,
			Compatible
		}

		private delegate bool GetComptability(PluginImporter imp);

		private bool m_HasModified;

		private PluginImporterInspector.Compatibility m_CompatibleWithAnyPlatform;

		private PluginImporterInspector.Compatibility m_CompatibleWithEditor;

		private PluginImporterInspector.Compatibility[] m_CompatibleWithPlatform = new PluginImporterInspector.Compatibility[PluginImporterInspector.GetPlatformGroupArraySize()];

		private static readonly BuildTarget[] m_StandaloneTargets = new BuildTarget[]
		{
			BuildTarget.StandaloneOSXIntel,
			BuildTarget.StandaloneOSXIntel64,
			BuildTarget.StandaloneOSXUniversal,
			BuildTarget.StandaloneWindows,
			BuildTarget.StandaloneWindows64,
			BuildTarget.StandaloneLinux,
			BuildTarget.StandaloneLinux64,
			BuildTarget.StandaloneLinuxUniversal
		};

		private Vector2 m_InformationScrollPosition = Vector2.zero;

		private Dictionary<string, string> m_PluginInformation;

		private EditorPluginImporterExtension m_EditorExtension = null;

		private DesktopPluginImporterExtension m_DesktopExtension = null;

		internal override bool showImportedObject
		{
			get
			{
				return false;
			}
		}

		internal EditorPluginImporterExtension editorExtension
		{
			get
			{
				if (this.m_EditorExtension == null)
				{
					this.m_EditorExtension = new EditorPluginImporterExtension();
				}
				return this.m_EditorExtension;
			}
		}

		internal DesktopPluginImporterExtension desktopExtension
		{
			get
			{
				if (this.m_DesktopExtension == null)
				{
					this.m_DesktopExtension = new DesktopPluginImporterExtension();
				}
				return this.m_DesktopExtension;
			}
		}

		internal IPluginImporterExtension[] additionalExtensions
		{
			get
			{
				return new IPluginImporterExtension[]
				{
					this.editorExtension,
					this.desktopExtension
				};
			}
		}

		internal PluginImporter importer
		{
			get
			{
				return base.target as PluginImporter;
			}
		}

		internal PluginImporter[] importers
		{
			get
			{
				return base.targets.Cast<PluginImporter>().ToArray<PluginImporter>();
			}
		}

		private PluginImporterInspector.Compatibility compatibleWithStandalone
		{
			get
			{
				bool flag = false;
				BuildTarget[] standaloneTargets = PluginImporterInspector.m_StandaloneTargets;
				PluginImporterInspector.Compatibility result;
				for (int i = 0; i < standaloneTargets.Length; i++)
				{
					BuildTarget buildTarget = standaloneTargets[i];
					if (this.m_CompatibleWithPlatform[(int)buildTarget] == PluginImporterInspector.Compatibility.Mixed)
					{
						result = PluginImporterInspector.Compatibility.Mixed;
						return result;
					}
					flag |= (this.m_CompatibleWithPlatform[(int)buildTarget] > PluginImporterInspector.Compatibility.NotCompatible);
				}
				result = ((!flag) ? PluginImporterInspector.Compatibility.NotCompatible : PluginImporterInspector.Compatibility.Compatible);
				return result;
			}
			set
			{
				BuildTarget[] standaloneTargets = PluginImporterInspector.m_StandaloneTargets;
				for (int i = 0; i < standaloneTargets.Length; i++)
				{
					BuildTarget buildTarget = standaloneTargets[i];
					this.m_CompatibleWithPlatform[(int)buildTarget] = value;
				}
			}
		}

		private static bool IgnorePlatform(BuildTarget platform)
		{
			return false;
		}

		private bool IsEditingPlatformSettingsSupported()
		{
			return base.targets.Length == 1;
		}

		private static int GetPlatformGroupArraySize()
		{
			int num = 0;
			using (List<Enum>.Enumerator enumerator = typeof(BuildTarget).EnumGetNonObsoleteValues().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BuildTarget buildTarget = (BuildTarget)enumerator.Current;
					if (num < (int)(buildTarget + 1))
					{
						num = (int)(buildTarget + 1);
					}
				}
			}
			return num;
		}

		private static bool IsStandaloneTarget(BuildTarget buildTarget)
		{
			return PluginImporterInspector.m_StandaloneTargets.Contains(buildTarget);
		}

		internal PluginImporterInspector.Compatibility GetPlatformCompatibility(string platformName)
		{
			return this.m_CompatibleWithPlatform[(int)BuildPipeline.GetBuildTargetByName(platformName)];
		}

		internal void SetPlatformCompatibility(string platformName, bool compatible)
		{
			this.SetPlatformCompatibility(platformName, (!compatible) ? PluginImporterInspector.Compatibility.NotCompatible : PluginImporterInspector.Compatibility.Compatible);
		}

		internal void SetPlatformCompatibility(string platformName, PluginImporterInspector.Compatibility compatibility)
		{
			if (compatibility == PluginImporterInspector.Compatibility.Mixed)
			{
				throw new ArgumentException("compatibility value cannot be Mixed");
			}
			int buildTargetByName = (int)BuildPipeline.GetBuildTargetByName(platformName);
			if (this.m_CompatibleWithPlatform[buildTargetByName] != compatibility)
			{
				this.m_CompatibleWithPlatform[buildTargetByName] = compatibility;
				this.m_HasModified = true;
			}
		}

		private static List<BuildTarget> GetValidBuildTargets()
		{
			List<BuildTarget> list = new List<BuildTarget>();
			using (List<Enum>.Enumerator enumerator = typeof(BuildTarget).EnumGetNonObsoleteValues().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BuildTarget buildTarget = (BuildTarget)enumerator.Current;
					if (buildTarget > (BuildTarget)0)
					{
						if (!PluginImporterInspector.IgnorePlatform(buildTarget))
						{
							if (!ModuleManager.IsPlatformSupported(buildTarget) || ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(buildTarget)) || PluginImporterInspector.IsStandaloneTarget(buildTarget))
							{
								list.Add(buildTarget);
							}
						}
					}
				}
			}
			return list;
		}

		private BuildPlayerWindow.BuildPlatform[] GetBuildPlayerValidPlatforms()
		{
			List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
			List<BuildPlayerWindow.BuildPlatform> list = new List<BuildPlayerWindow.BuildPlatform>();
			if (this.m_CompatibleWithEditor > PluginImporterInspector.Compatibility.NotCompatible)
			{
				list.Add(new BuildPlayerWindow.BuildPlatform("Editor settings", "Editor Settings", "BuildSettings.Editor", BuildTargetGroup.Unknown, true)
				{
					name = BuildPipeline.GetEditorTargetName()
				});
			}
			foreach (BuildPlayerWindow.BuildPlatform current in validPlatforms)
			{
				if (!PluginImporterInspector.IgnorePlatform(current.DefaultTarget))
				{
					if (current.targetGroup == BuildTargetGroup.Standalone)
					{
						if (this.compatibleWithStandalone < PluginImporterInspector.Compatibility.Compatible)
						{
							continue;
						}
					}
					else
					{
						if (this.m_CompatibleWithPlatform[(int)current.DefaultTarget] < PluginImporterInspector.Compatibility.Compatible)
						{
							continue;
						}
						if (ModuleManager.GetPluginImporterExtension(current.targetGroup) == null)
						{
							continue;
						}
					}
					list.Add(current);
				}
			}
			return list.ToArray();
		}

		private void ResetCompatability(ref PluginImporterInspector.Compatibility value, PluginImporterInspector.GetComptability getComptability)
		{
			value = ((!getComptability(this.importer)) ? PluginImporterInspector.Compatibility.NotCompatible : PluginImporterInspector.Compatibility.Compatible);
			PluginImporter[] importers = this.importers;
			for (int i = 0; i < importers.Length; i++)
			{
				PluginImporter imp = importers[i];
				if (value != ((!getComptability(imp)) ? PluginImporterInspector.Compatibility.NotCompatible : PluginImporterInspector.Compatibility.Compatible))
				{
					value = PluginImporterInspector.Compatibility.Mixed;
					break;
				}
			}
		}

		internal override void ResetValues()
		{
			base.ResetValues();
			this.m_HasModified = false;
			this.ResetCompatability(ref this.m_CompatibleWithAnyPlatform, (PluginImporter imp) => imp.GetCompatibleWithAnyPlatform());
			this.ResetCompatability(ref this.m_CompatibleWithEditor, (PluginImporter imp) => imp.GetCompatibleWithEditor());
			if (this.m_CompatibleWithAnyPlatform < PluginImporterInspector.Compatibility.Compatible)
			{
				this.ResetCompatability(ref this.m_CompatibleWithEditor, (PluginImporter imp) => imp.GetCompatibleWithEditor("", ""));
				using (List<BuildTarget>.Enumerator enumerator = PluginImporterInspector.GetValidBuildTargets().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BuildTarget platform = enumerator.Current;
						this.ResetCompatability(ref this.m_CompatibleWithPlatform[(int)platform], (PluginImporter imp) => imp.GetCompatibleWithPlatform(platform));
					}
				}
			}
			else
			{
				this.ResetCompatability(ref this.m_CompatibleWithEditor, (PluginImporter imp) => !imp.GetExcludeEditorFromAnyPlatform());
				using (List<BuildTarget>.Enumerator enumerator2 = PluginImporterInspector.GetValidBuildTargets().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						BuildTarget platform = enumerator2.Current;
						this.ResetCompatability(ref this.m_CompatibleWithPlatform[(int)platform], (PluginImporter imp) => !imp.GetExcludeFromAnyPlatform(platform));
					}
				}
			}
			if (this.IsEditingPlatformSettingsSupported())
			{
				IPluginImporterExtension[] additionalExtensions = this.additionalExtensions;
				for (int i = 0; i < additionalExtensions.Length; i++)
				{
					IPluginImporterExtension pluginImporterExtension = additionalExtensions[i];
					pluginImporterExtension.ResetValues(this);
				}
				foreach (BuildTarget current in PluginImporterInspector.GetValidBuildTargets())
				{
					IPluginImporterExtension pluginImporterExtension2 = ModuleManager.GetPluginImporterExtension(current);
					if (pluginImporterExtension2 != null)
					{
						pluginImporterExtension2.ResetValues(this);
					}
				}
			}
		}

		internal override bool HasModified()
		{
			bool flag = this.m_HasModified || base.HasModified();
			bool result;
			if (!this.IsEditingPlatformSettingsSupported())
			{
				result = flag;
			}
			else
			{
				IPluginImporterExtension[] additionalExtensions = this.additionalExtensions;
				for (int i = 0; i < additionalExtensions.Length; i++)
				{
					IPluginImporterExtension pluginImporterExtension = additionalExtensions[i];
					flag |= pluginImporterExtension.HasModified(this);
				}
				foreach (BuildTarget current in PluginImporterInspector.GetValidBuildTargets())
				{
					IPluginImporterExtension pluginImporterExtension2 = ModuleManager.GetPluginImporterExtension(current);
					if (pluginImporterExtension2 != null)
					{
						flag |= pluginImporterExtension2.HasModified(this);
					}
				}
				result = flag;
			}
			return result;
		}

		internal override void Apply()
		{
			base.Apply();
			PluginImporter[] importers = this.importers;
			for (int i = 0; i < importers.Length; i++)
			{
				PluginImporter pluginImporter = importers[i];
				if (this.m_CompatibleWithAnyPlatform > PluginImporterInspector.Compatibility.Mixed)
				{
					pluginImporter.SetCompatibleWithAnyPlatform(this.m_CompatibleWithAnyPlatform == PluginImporterInspector.Compatibility.Compatible);
				}
				if (this.m_CompatibleWithEditor > PluginImporterInspector.Compatibility.Mixed)
				{
					pluginImporter.SetCompatibleWithEditor(this.m_CompatibleWithEditor == PluginImporterInspector.Compatibility.Compatible);
				}
				foreach (BuildTarget current in PluginImporterInspector.GetValidBuildTargets())
				{
					if (this.m_CompatibleWithPlatform[(int)current] > PluginImporterInspector.Compatibility.Mixed)
					{
						pluginImporter.SetCompatibleWithPlatform(current, this.m_CompatibleWithPlatform[(int)current] == PluginImporterInspector.Compatibility.Compatible);
					}
				}
				if (this.m_CompatibleWithEditor > PluginImporterInspector.Compatibility.Mixed)
				{
					pluginImporter.SetExcludeEditorFromAnyPlatform(this.m_CompatibleWithEditor == PluginImporterInspector.Compatibility.NotCompatible);
				}
				foreach (BuildTarget current2 in PluginImporterInspector.GetValidBuildTargets())
				{
					if (this.m_CompatibleWithPlatform[(int)current2] > PluginImporterInspector.Compatibility.Mixed)
					{
						pluginImporter.SetExcludeFromAnyPlatform(current2, this.m_CompatibleWithPlatform[(int)current2] == PluginImporterInspector.Compatibility.NotCompatible);
					}
				}
			}
			if (this.IsEditingPlatformSettingsSupported())
			{
				IPluginImporterExtension[] additionalExtensions = this.additionalExtensions;
				for (int j = 0; j < additionalExtensions.Length; j++)
				{
					IPluginImporterExtension pluginImporterExtension = additionalExtensions[j];
					pluginImporterExtension.Apply(this);
				}
				foreach (BuildTarget current3 in PluginImporterInspector.GetValidBuildTargets())
				{
					IPluginImporterExtension pluginImporterExtension2 = ModuleManager.GetPluginImporterExtension(current3);
					if (pluginImporterExtension2 != null)
					{
						pluginImporterExtension2.Apply(this);
					}
				}
			}
		}

		internal override void Awake()
		{
			this.m_EditorExtension = new EditorPluginImporterExtension();
			this.m_DesktopExtension = new DesktopPluginImporterExtension();
			base.Awake();
		}

		private void OnEnable()
		{
			if (this.IsEditingPlatformSettingsSupported())
			{
				IPluginImporterExtension[] additionalExtensions = this.additionalExtensions;
				for (int i = 0; i < additionalExtensions.Length; i++)
				{
					IPluginImporterExtension pluginImporterExtension = additionalExtensions[i];
					pluginImporterExtension.OnEnable(this);
				}
				foreach (BuildTarget current in PluginImporterInspector.GetValidBuildTargets())
				{
					IPluginImporterExtension pluginImporterExtension2 = ModuleManager.GetPluginImporterExtension(current);
					if (pluginImporterExtension2 != null)
					{
						pluginImporterExtension2.OnEnable(this);
						pluginImporterExtension2.ResetValues(this);
					}
				}
				this.m_PluginInformation = new Dictionary<string, string>();
				this.m_PluginInformation["Path"] = this.importer.assetPath;
				this.m_PluginInformation["Type"] = ((!this.importer.isNativePlugin) ? "Managed" : "Native");
				if (!this.importer.isNativePlugin)
				{
					string value;
					switch (this.importer.dllType)
					{
					case DllType.UnknownManaged:
						value = "Targets Unknown .NET";
						break;
					case DllType.ManagedNET35:
						value = "Targets .NET 3.5";
						break;
					case DllType.ManagedNET40:
						value = "Targets .NET 4.x";
						break;
					case DllType.WinMDNative:
						value = "Native WinMD";
						break;
					case DllType.WinMDNET40:
						value = "Managed WinMD";
						break;
					default:
						throw new Exception("Unknown managed dll type: " + this.importer.dllType.ToString());
					}
					this.m_PluginInformation["Assembly Info"] = value;
				}
			}
		}

		private new void OnDisable()
		{
			base.OnDisable();
			if (this.IsEditingPlatformSettingsSupported())
			{
				IPluginImporterExtension[] additionalExtensions = this.additionalExtensions;
				for (int i = 0; i < additionalExtensions.Length; i++)
				{
					IPluginImporterExtension pluginImporterExtension = additionalExtensions[i];
					pluginImporterExtension.OnDisable(this);
				}
				foreach (BuildTarget current in PluginImporterInspector.GetValidBuildTargets())
				{
					IPluginImporterExtension pluginImporterExtension2 = ModuleManager.GetPluginImporterExtension(current);
					if (pluginImporterExtension2 != null)
					{
						pluginImporterExtension2.OnDisable(this);
					}
				}
			}
		}

		private PluginImporterInspector.Compatibility ToggleWithMixedValue(PluginImporterInspector.Compatibility value, string title)
		{
			EditorGUI.showMixedValue = (value == PluginImporterInspector.Compatibility.Mixed);
			EditorGUI.BeginChangeCheck();
			bool flag = EditorGUILayout.Toggle(title, value == PluginImporterInspector.Compatibility.Compatible, new GUILayoutOption[0]);
			PluginImporterInspector.Compatibility result;
			if (EditorGUI.EndChangeCheck())
			{
				result = ((!flag) ? PluginImporterInspector.Compatibility.NotCompatible : PluginImporterInspector.Compatibility.Compatible);
			}
			else
			{
				EditorGUI.showMixedValue = false;
				result = value;
			}
			return result;
		}

		private void ShowPlatforms(PluginImporterInspector.ValueSwitcher switcher)
		{
			this.m_CompatibleWithEditor = switcher(this.ToggleWithMixedValue(switcher(this.m_CompatibleWithEditor), "Editor"));
			EditorGUI.BeginChangeCheck();
			PluginImporterInspector.Compatibility value = this.ToggleWithMixedValue(switcher(this.compatibleWithStandalone), "Standalone");
			if (EditorGUI.EndChangeCheck())
			{
				this.compatibleWithStandalone = switcher(value);
				if (this.compatibleWithStandalone != PluginImporterInspector.Compatibility.Mixed)
				{
					this.desktopExtension.ValidateSingleCPUTargets(this);
				}
			}
			foreach (BuildTarget current in PluginImporterInspector.GetValidBuildTargets())
			{
				if (!PluginImporterInspector.IsStandaloneTarget(current))
				{
					this.m_CompatibleWithPlatform[(int)current] = switcher(this.ToggleWithMixedValue(switcher(this.m_CompatibleWithPlatform[(int)current]), current.ToString()));
				}
			}
		}

		private PluginImporterInspector.Compatibility SwitchToInclude(PluginImporterInspector.Compatibility value)
		{
			return value;
		}

		private PluginImporterInspector.Compatibility SwitchToExclude(PluginImporterInspector.Compatibility value)
		{
			PluginImporterInspector.Compatibility result;
			switch (value + 1)
			{
			case PluginImporterInspector.Compatibility.NotCompatible:
				result = PluginImporterInspector.Compatibility.Mixed;
				break;
			case PluginImporterInspector.Compatibility.Compatible:
				result = PluginImporterInspector.Compatibility.Compatible;
				break;
			case (PluginImporterInspector.Compatibility)2:
				result = PluginImporterInspector.Compatibility.NotCompatible;
				break;
			default:
				throw new InvalidEnumArgumentException("Invalid value: " + value.ToString());
			}
			return result;
		}

		private void ShowGeneralOptions()
		{
			EditorGUI.BeginChangeCheck();
			this.m_CompatibleWithAnyPlatform = this.ToggleWithMixedValue(this.m_CompatibleWithAnyPlatform, "Any Platform");
			if (this.m_CompatibleWithAnyPlatform == PluginImporterInspector.Compatibility.Compatible)
			{
				GUILayout.Label("Exclude Platforms", EditorStyles.boldLabel, new GUILayoutOption[0]);
				this.ShowPlatforms(new PluginImporterInspector.ValueSwitcher(this.SwitchToExclude));
			}
			else if (this.m_CompatibleWithAnyPlatform == PluginImporterInspector.Compatibility.NotCompatible)
			{
				GUILayout.Label("Include Platforms", EditorStyles.boldLabel, new GUILayoutOption[0]);
				this.ShowPlatforms(new PluginImporterInspector.ValueSwitcher(this.SwitchToInclude));
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.m_HasModified = true;
			}
		}

		private void ShowEditorSettings()
		{
			this.editorExtension.OnPlatformSettingsGUI(this);
		}

		private void ShowPlatformSettings()
		{
			BuildPlayerWindow.BuildPlatform[] buildPlayerValidPlatforms = this.GetBuildPlayerValidPlatforms();
			if (buildPlayerValidPlatforms.Length > 0)
			{
				GUILayout.Label("Platform settings", EditorStyles.boldLabel, new GUILayoutOption[0]);
				int num = EditorGUILayout.BeginPlatformGrouping(buildPlayerValidPlatforms, null);
				if (buildPlayerValidPlatforms[num].name == BuildPipeline.GetEditorTargetName())
				{
					this.ShowEditorSettings();
				}
				else
				{
					BuildTargetGroup targetGroup = buildPlayerValidPlatforms[num].targetGroup;
					if (targetGroup == BuildTargetGroup.Standalone)
					{
						this.desktopExtension.OnPlatformSettingsGUI(this);
					}
					else
					{
						IPluginImporterExtension pluginImporterExtension = ModuleManager.GetPluginImporterExtension(targetGroup);
						if (pluginImporterExtension != null)
						{
							pluginImporterExtension.OnPlatformSettingsGUI(this);
						}
					}
				}
				EditorGUILayout.EndPlatformGrouping();
			}
		}

		public override void OnInspectorGUI()
		{
			using (new EditorGUI.DisabledScope(false))
			{
				GUILayout.Label("Select platforms for plugin", EditorStyles.boldLabel, new GUILayoutOption[0]);
				EditorGUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[0]);
				this.ShowGeneralOptions();
				EditorGUILayout.EndVertical();
				GUILayout.Space(10f);
				if (this.IsEditingPlatformSettingsSupported())
				{
					this.ShowPlatformSettings();
				}
			}
			base.ApplyRevertGUI();
			if (base.targets.Length <= 1)
			{
				GUILayout.Label("Information", EditorStyles.boldLabel, new GUILayoutOption[0]);
				this.m_InformationScrollPosition = EditorGUILayout.BeginVerticalScrollView(this.m_InformationScrollPosition, new GUILayoutOption[0]);
				foreach (KeyValuePair<string, string> current in this.m_PluginInformation)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[0]);
					GUILayout.Label(current.Key, new GUILayoutOption[]
					{
						GUILayout.Width(85f)
					});
					GUILayout.TextField(current.Value, new GUILayoutOption[0]);
					GUILayout.EndHorizontal();
				}
				EditorGUILayout.EndScrollView();
				GUILayout.FlexibleSpace();
				if (this.importer.isNativePlugin)
				{
					EditorGUILayout.HelpBox("Once a native plugin is loaded from script, it's never unloaded. If you deselect a native plugin and it's already loaded, please restart Unity.", MessageType.Warning);
				}
				if (this.importer.dllType == DllType.ManagedNET40 && this.m_CompatibleWithEditor == PluginImporterInspector.Compatibility.Compatible)
				{
					EditorGUILayout.HelpBox("Plugin targets .NET 4.x and is marked as compatible with Editor, Editor can only use assemblies targeting .NET 3.5 or lower, please unselect Editor as compatible platform.", MessageType.Error);
				}
			}
		}
	}
}
