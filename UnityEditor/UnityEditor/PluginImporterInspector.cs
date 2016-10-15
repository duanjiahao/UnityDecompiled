using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Modules;
using UnityEngine;

namespace UnityEditor
{
	[CanEditMultipleObjects, CustomEditor(typeof(PluginImporter))]
	internal class PluginImporterInspector : AssetImporterInspector
	{
		private delegate bool GetComptability(PluginImporter imp);

		private bool m_HasModified;

		private int m_CompatibleWithAnyPlatform;

		private int m_CompatibleWithEditor;

		private int[] m_CompatibleWithPlatform = new int[PluginImporterInspector.GetPlatformGroupArraySize()];

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

		private EditorPluginImporterExtension m_EditorExtension = new EditorPluginImporterExtension();

		private DesktopPluginImporterExtension m_DesktopExtension = new DesktopPluginImporterExtension();

		private IPluginImporterExtension[] m_AdditionalExtensions;

		internal override bool showImportedObject
		{
			get
			{
				return false;
			}
		}

		internal IPluginImporterExtension[] additionalExtensions
		{
			get
			{
				if (this.m_AdditionalExtensions == null)
				{
					this.m_AdditionalExtensions = new IPluginImporterExtension[]
					{
						this.m_EditorExtension,
						this.m_DesktopExtension
					};
				}
				return this.m_AdditionalExtensions;
			}
		}

		internal PluginImporter importer
		{
			get
			{
				return this.target as PluginImporter;
			}
		}

		internal PluginImporter[] importers
		{
			get
			{
				return base.targets.Cast<PluginImporter>().ToArray<PluginImporter>();
			}
		}

		private int compatibleWithStandalone
		{
			get
			{
				bool flag = false;
				BuildTarget[] standaloneTargets = PluginImporterInspector.m_StandaloneTargets;
				for (int i = 0; i < standaloneTargets.Length; i++)
				{
					BuildTarget buildTarget = standaloneTargets[i];
					if (this.m_CompatibleWithPlatform[(int)buildTarget] == -1)
					{
						return -1;
					}
					flag |= (this.m_CompatibleWithPlatform[(int)buildTarget] > 0);
				}
				return (!flag) ? 0 : 1;
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

		internal bool GetCompatibleWithPlatform(string platformName)
		{
			if (base.targets.Length > 1)
			{
				throw new InvalidOperationException("Cannot be used while multiple plugins are selected");
			}
			return this.m_CompatibleWithPlatform[(int)BuildPipeline.GetBuildTargetByName(platformName)] == 1;
		}

		internal void SetCompatibleWithPlatform(string platformName, bool enabled)
		{
			if (base.targets.Length > 1)
			{
				throw new InvalidOperationException("Cannot be used while multiple plugins are selected");
			}
			int num = (!enabled) ? 0 : 1;
			int buildTargetByName = (int)BuildPipeline.GetBuildTargetByName(platformName);
			if (this.m_CompatibleWithPlatform[buildTargetByName] == num)
			{
				return;
			}
			this.m_CompatibleWithPlatform[buildTargetByName] = num;
			this.m_HasModified = true;
		}

		private static List<BuildTarget> GetValidBuildTargets()
		{
			List<BuildTarget> list = new List<BuildTarget>();
			using (List<Enum>.Enumerator enumerator = typeof(BuildTarget).EnumGetNonObsoleteValues().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					BuildTarget buildTarget = (BuildTarget)enumerator.Current;
					if (!PluginImporterInspector.IgnorePlatform(buildTarget))
					{
						if (!ModuleManager.IsPlatformSupported(buildTarget) || ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(buildTarget)) || PluginImporterInspector.IsStandaloneTarget(buildTarget))
						{
							list.Add(buildTarget);
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
			if (this.m_CompatibleWithAnyPlatform > 0 || this.m_CompatibleWithEditor > 0)
			{
				list.Add(new BuildPlayerWindow.BuildPlatform("Editor settings", "BuildSettings.Editor", BuildTargetGroup.Unknown, true)
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
						if (this.m_CompatibleWithAnyPlatform < 1 && this.compatibleWithStandalone < 1)
						{
							continue;
						}
					}
					else
					{
						if (this.m_CompatibleWithAnyPlatform < 1 && this.m_CompatibleWithPlatform[(int)current.DefaultTarget] < 1)
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

		private void ResetCompatability(ref int value, PluginImporterInspector.GetComptability getComptability)
		{
			value = ((!getComptability(this.importer)) ? 0 : 1);
			PluginImporter[] importers = this.importers;
			for (int i = 0; i < importers.Length; i++)
			{
				PluginImporter imp = importers[i];
				if (value != ((!getComptability(imp)) ? 0 : 1))
				{
					value = -1;
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
			foreach (BuildTarget platform in PluginImporterInspector.GetValidBuildTargets())
			{
				this.ResetCompatability(ref this.m_CompatibleWithPlatform[(int)platform], (PluginImporter imp) => imp.GetCompatibleWithPlatform(platform));
			}
			if (!this.IsEditingPlatformSettingsSupported())
			{
				return;
			}
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

		internal override bool HasModified()
		{
			bool flag = this.m_HasModified || base.HasModified();
			if (!this.IsEditingPlatformSettingsSupported())
			{
				return flag;
			}
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
			return flag;
		}

		internal override void Apply()
		{
			base.Apply();
			PluginImporter[] importers = this.importers;
			for (int i = 0; i < importers.Length; i++)
			{
				PluginImporter pluginImporter = importers[i];
				if (this.m_CompatibleWithAnyPlatform > -1)
				{
					pluginImporter.SetCompatibleWithAnyPlatform(this.m_CompatibleWithAnyPlatform == 1);
				}
				if (this.m_CompatibleWithEditor > -1)
				{
					pluginImporter.SetCompatibleWithEditor(this.m_CompatibleWithEditor == 1);
				}
				foreach (BuildTarget current in PluginImporterInspector.GetValidBuildTargets())
				{
					if (this.m_CompatibleWithPlatform[(int)current] > -1)
					{
						pluginImporter.SetCompatibleWithPlatform(current, this.m_CompatibleWithPlatform[(int)current] == 1);
					}
				}
			}
			if (!this.IsEditingPlatformSettingsSupported())
			{
				return;
			}
			IPluginImporterExtension[] additionalExtensions = this.additionalExtensions;
			for (int j = 0; j < additionalExtensions.Length; j++)
			{
				IPluginImporterExtension pluginImporterExtension = additionalExtensions[j];
				pluginImporterExtension.Apply(this);
			}
			foreach (BuildTarget current2 in PluginImporterInspector.GetValidBuildTargets())
			{
				IPluginImporterExtension pluginImporterExtension2 = ModuleManager.GetPluginImporterExtension(current2);
				if (pluginImporterExtension2 != null)
				{
					pluginImporterExtension2.Apply(this);
				}
			}
		}

		private void OnEnable()
		{
			if (!this.IsEditingPlatformSettingsSupported())
			{
				return;
			}
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
		}

		private new void OnDisable()
		{
			base.OnDisable();
			if (!this.IsEditingPlatformSettingsSupported())
			{
				return;
			}
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

		private int ToggleWithMixedValue(int value, string title)
		{
			EditorGUI.showMixedValue = (value == -1);
			EditorGUI.BeginChangeCheck();
			bool flag = EditorGUILayout.Toggle(title, value == 1, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				return (!flag) ? 0 : 1;
			}
			EditorGUI.showMixedValue = false;
			return value;
		}

		private void ShowGeneralOptions()
		{
			EditorGUI.BeginChangeCheck();
			this.m_CompatibleWithAnyPlatform = this.ToggleWithMixedValue(this.m_CompatibleWithAnyPlatform, "Any Platform");
			using (new EditorGUI.DisabledScope(this.m_CompatibleWithAnyPlatform == 1))
			{
				this.m_CompatibleWithEditor = this.ToggleWithMixedValue(this.m_CompatibleWithEditor, "Editor");
				EditorGUI.BeginChangeCheck();
				int compatibleWithStandalone = this.ToggleWithMixedValue(this.compatibleWithStandalone, "Standalone");
				if (EditorGUI.EndChangeCheck())
				{
					this.compatibleWithStandalone = compatibleWithStandalone;
					this.m_DesktopExtension.ValidateSingleCPUTargets(this);
				}
				foreach (BuildTarget current in PluginImporterInspector.GetValidBuildTargets())
				{
					if (!PluginImporterInspector.IsStandaloneTarget(current))
					{
						this.m_CompatibleWithPlatform[(int)current] = this.ToggleWithMixedValue(this.m_CompatibleWithPlatform[(int)current], current.ToString());
					}
				}
			}
			if (EditorGUI.EndChangeCheck())
			{
				this.m_HasModified = true;
			}
		}

		private void ShowEditorSettings()
		{
			this.m_EditorExtension.OnPlatformSettingsGUI(this);
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
						this.m_DesktopExtension.OnPlatformSettingsGUI(this);
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
			if (base.targets.Length > 1)
			{
				return;
			}
			GUILayout.Label("Information", EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.m_InformationScrollPosition = EditorGUILayout.BeginVerticalScrollView(this.m_InformationScrollPosition, new GUILayoutOption[0]);
			foreach (KeyValuePair<string, string> current in this.m_PluginInformation)
			{
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.Label(current.Key, new GUILayoutOption[]
				{
					GUILayout.Width(50f)
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
		}
	}
}
