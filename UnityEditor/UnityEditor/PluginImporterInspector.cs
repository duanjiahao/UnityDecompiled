using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Modules;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(PluginImporter))]
	internal class PluginImporterInspector : AssetImporterInspector
	{
		private bool hasModified;
		private bool compatibleWithAnyPlatform;
		private bool compatibleWithEditor;
		private bool isPreloaded;
		private bool[] compatibleWithPlatform = new bool[PluginImporterInspector.GetPlatformGroupArraySize()];
		private Vector2 informationScrollPosition = Vector2.zero;
		private Dictionary<string, string> pluginInformation;
		private EditorPluginImporterExtension editorExtension = new EditorPluginImporterExtension();
		private DesktopPluginImporterExtension desktopExtension = new DesktopPluginImporterExtension();
		private IPluginImporterExtension[] _additionalExtensions;
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
				if (this._additionalExtensions == null)
				{
					this._additionalExtensions = new IPluginImporterExtension[]
					{
						this.editorExtension,
						this.desktopExtension
					};
				}
				return this._additionalExtensions;
			}
		}
		internal PluginImporter importer
		{
			get
			{
				return this.target as PluginImporter;
			}
		}
		private bool compatibleWithStandalone
		{
			get
			{
				return this.compatibleWithPlatform[4] || this.compatibleWithPlatform[27] || this.compatibleWithPlatform[2] || this.compatibleWithPlatform[5] || this.compatibleWithPlatform[17] || this.compatibleWithPlatform[24] || this.compatibleWithPlatform[25] || this.compatibleWithPlatform[19];
			}
			set
			{
				bool[] arg_5A_0 = this.compatibleWithPlatform;
				int arg_5A_1 = 4;
				bool[] arg_58_0 = this.compatibleWithPlatform;
				int arg_58_1 = 27;
				bool[] arg_54_0 = this.compatibleWithPlatform;
				int arg_54_1 = 2;
				bool[] arg_50_0 = this.compatibleWithPlatform;
				int arg_50_1 = 5;
				bool[] arg_4C_0 = this.compatibleWithPlatform;
				int arg_4C_1 = 17;
				bool[] arg_48_0 = this.compatibleWithPlatform;
				int arg_48_1 = 24;
				bool[] arg_44_0 = this.compatibleWithPlatform;
				int arg_44_1 = 25;
				this.compatibleWithPlatform[19] = value;
				arg_5A_0[arg_5A_1] = (arg_58_0[arg_58_1] = (arg_54_0[arg_54_1] = (arg_50_0[arg_50_1] = (arg_4C_0[arg_4C_1] = (arg_48_0[arg_48_1] = (arg_44_0[arg_44_1] = value))))));
			}
		}
		private static bool IgnorePlatform(BuildTarget platform)
		{
			return platform == BuildTarget.StandaloneGLESEmu;
		}
		private static int GetPlatformGroupArraySize()
		{
			int num = 0;
			IEnumerator enumerator = Enum.GetValues(typeof(BuildTarget)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BuildTarget buildTarget = (BuildTarget)((int)enumerator.Current);
					if (num < (int)(buildTarget + 1))
					{
						num = (int)(buildTarget + 1);
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return num;
		}
		private static bool IsStandaloneTarget(BuildTarget target)
		{
			return target == BuildTarget.StandaloneOSXIntel || target == BuildTarget.StandaloneOSXIntel64 || target == BuildTarget.StandaloneOSXUniversal || target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneLinux || target == BuildTarget.StandaloneLinux64 || target == BuildTarget.StandaloneLinuxUniversal || target == BuildTarget.StandaloneWindows64;
		}
		private static List<BuildTarget> GetValidBuildTargets()
		{
			List<BuildTarget> list = new List<BuildTarget>();
			IEnumerator enumerator = Enum.GetValues(typeof(BuildTarget)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BuildTarget buildTarget = (BuildTarget)((int)enumerator.Current);
					if (!PluginImporterInspector.IgnorePlatform(buildTarget) && !list.Contains(buildTarget))
					{
						if (!ModuleManager.IsPlatformSupported(buildTarget) || ModuleManager.IsPlatformSupportLoaded(ModuleManager.GetTargetStringFromBuildTarget(buildTarget)))
						{
							list.Add(buildTarget);
						}
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return list;
		}
		private BuildPlayerWindow.BuildPlatform[] GetBuildPlayerValidPlatforms()
		{
			List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
			List<BuildPlayerWindow.BuildPlatform> list = new List<BuildPlayerWindow.BuildPlatform>();
			if (this.compatibleWithAnyPlatform || this.compatibleWithEditor)
			{
				list.Add(new BuildPlayerWindow.BuildPlatform("BuildSettings.Editor", "Editor settings", BuildTargetGroup.Unknown, true)
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
						if (!this.compatibleWithAnyPlatform && !this.compatibleWithStandalone)
						{
							continue;
						}
					}
					else
					{
						if (!this.compatibleWithAnyPlatform && !this.compatibleWithPlatform[(int)current.DefaultTarget])
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
		internal override void ResetValues()
		{
			base.ResetValues();
			this.hasModified = false;
			this.compatibleWithAnyPlatform = this.importer.GetCompatibleWithAnyPlatform();
			this.compatibleWithEditor = this.importer.GetCompatibleWithEditor();
			this.isPreloaded = this.importer.GetIsPreloaded();
			IPluginImporterExtension[] additionalExtensions = this.additionalExtensions;
			for (int i = 0; i < additionalExtensions.Length; i++)
			{
				IPluginImporterExtension pluginImporterExtension = additionalExtensions[i];
				pluginImporterExtension.ResetValues(this);
			}
			foreach (BuildTarget current in PluginImporterInspector.GetValidBuildTargets())
			{
				this.compatibleWithPlatform[(int)current] = this.importer.GetCompatibleWithPlatform(current);
				IPluginImporterExtension pluginImporterExtension2 = ModuleManager.GetPluginImporterExtension(current);
				if (pluginImporterExtension2 != null)
				{
					pluginImporterExtension2.ResetValues(this);
				}
			}
		}
		internal override bool HasModified()
		{
			bool flag = this.hasModified || base.HasModified();
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
			this.importer.SetCompatibleWithAnyPlatform(this.compatibleWithAnyPlatform);
			this.importer.SetCompatibleWithEditor(this.compatibleWithEditor);
			this.importer.SetIsPreloaded(this.isPreloaded);
			IPluginImporterExtension[] additionalExtensions = this.additionalExtensions;
			for (int i = 0; i < additionalExtensions.Length; i++)
			{
				IPluginImporterExtension pluginImporterExtension = additionalExtensions[i];
				pluginImporterExtension.Apply(this);
			}
			foreach (BuildTarget current in PluginImporterInspector.GetValidBuildTargets())
			{
				this.importer.SetCompatibleWithPlatform(current, this.compatibleWithPlatform[(int)current]);
				IPluginImporterExtension pluginImporterExtension2 = ModuleManager.GetPluginImporterExtension(current);
				if (pluginImporterExtension2 != null)
				{
					pluginImporterExtension2.Apply(this);
				}
			}
		}
		private void OnEnable()
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
			PluginImporter pluginImporter = (PluginImporter)this.target;
			this.pluginInformation = new Dictionary<string, string>();
			this.pluginInformation["Path"] = pluginImporter.assetPath;
			this.pluginInformation["Type"] = ((!pluginImporter.isNativePlugin) ? "Managed" : "Native");
		}
		private new void OnDisable()
		{
			base.OnDisable();
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
		private void ShowGeneralOptions()
		{
			EditorGUI.BeginChangeCheck();
			this.compatibleWithAnyPlatform = GUILayout.Toggle(this.compatibleWithAnyPlatform, "Any Platform", new GUILayoutOption[0]);
			EditorGUI.BeginDisabledGroup(this.compatibleWithAnyPlatform);
			this.compatibleWithEditor = GUILayout.Toggle(this.compatibleWithEditor, "Editor", new GUILayoutOption[0]);
			this.compatibleWithStandalone = GUILayout.Toggle(this.compatibleWithStandalone, "Standalone", new GUILayoutOption[0]);
			foreach (BuildTarget current in PluginImporterInspector.GetValidBuildTargets())
			{
				if (!PluginImporterInspector.IsStandaloneTarget(current))
				{
					this.compatibleWithPlatform[(int)current] = GUILayout.Toggle(this.compatibleWithPlatform[(int)current], current.ToString(), new GUILayoutOption[0]);
				}
			}
			EditorGUI.EndDisabledGroup();
			if (EditorGUI.EndChangeCheck())
			{
				this.hasModified = true;
			}
		}
		private void ShowEditorSettings()
		{
			this.editorExtension.OnPlatformSettingsGUI(this);
		}
		public override void OnInspectorGUI()
		{
			EditorGUI.BeginDisabledGroup(false);
			BuildPlayerWindow.BuildPlatform[] buildPlayerValidPlatforms = this.GetBuildPlayerValidPlatforms();
			GUILayout.Label("Select platforms for plugin", EditorStyles.boldLabel, new GUILayoutOption[0]);
			EditorGUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[0]);
			this.ShowGeneralOptions();
			EditorGUILayout.EndVertical();
			EditorGUI.BeginChangeCheck();
			if (EditorGUI.EndChangeCheck())
			{
				this.hasModified = true;
			}
			GUILayout.Space(10f);
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
			EditorGUI.EndDisabledGroup();
			base.ApplyRevertGUI();
			GUILayout.Label("Information", EditorStyles.boldLabel, new GUILayoutOption[0]);
			this.informationScrollPosition = EditorGUILayout.BeginVerticalScrollView(this.informationScrollPosition, new GUILayoutOption[0]);
			foreach (KeyValuePair<string, string> current in this.pluginInformation)
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
