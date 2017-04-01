using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Light Explorer", icon = "Lighting")]
	internal class LightingExplorerWindow : EditorWindow
	{
		private static class Styles
		{
			public static readonly GUIContent[] TabTypes = new GUIContent[]
			{
				EditorGUIUtility.TextContent("Lights"),
				EditorGUIUtility.TextContent("Reflection Probes"),
				EditorGUIUtility.TextContent("Light Probes"),
				EditorGUIUtility.TextContent("Static Emissives")
			};
		}

		private enum TabType
		{
			Lights,
			Reflections,
			LightProbes,
			Emissives,
			Count
		}

		private List<LightingExplorerWindowTab> m_TableTabs;

		private float m_ToolbarPadding = -1f;

		private LightingExplorerWindow.TabType m_SelectedTab = LightingExplorerWindow.TabType.Lights;

		[CompilerGenerated]
		private static SerializedPropertyTable.HeaderDelegate <>f__mg$cache0;

		[CompilerGenerated]
		private static SerializedPropertyTable.HeaderDelegate <>f__mg$cache1;

		[CompilerGenerated]
		private static SerializedPropertyTable.HeaderDelegate <>f__mg$cache2;

		[CompilerGenerated]
		private static SerializedPropertyTable.HeaderDelegate <>f__mg$cache3;

		private float toolbarPadding
		{
			get
			{
				if (this.m_ToolbarPadding == -1f)
				{
					this.m_ToolbarPadding = EditorStyles.iconButton.CalcSize(EditorGUI.GUIContents.helpIcon).x * 2f + 6f;
				}
				return this.m_ToolbarPadding;
			}
		}

		[MenuItem("Window/Lighting/Light Explorer", false, 2099)]
		private static void CreateLightingExplorerWindow()
		{
			LightingExplorerWindow window = EditorWindow.GetWindow<LightingExplorerWindow>();
			window.minSize = new Vector2(500f, 250f);
			window.Show();
		}

		private void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
			if (this.m_TableTabs == null || this.m_TableTabs.Count != 4)
			{
				List<LightingExplorerWindowTab> list = new List<LightingExplorerWindowTab>();
				List<LightingExplorerWindowTab> arg_7B_0 = list;
				string arg_71_0 = "LightTable";
				SerializedPropertyDataStore.GatherDelegate arg_71_1 = () => UnityEngine.Object.FindObjectsOfType<Light>();
				if (LightingExplorerWindow.<>f__mg$cache0 == null)
				{
					LightingExplorerWindow.<>f__mg$cache0 = new SerializedPropertyTable.HeaderDelegate(LightTableColumns.CreateLightColumns);
				}
				arg_7B_0.Add(new LightingExplorerWindowTab(new SerializedPropertyTable(arg_71_0, arg_71_1, LightingExplorerWindow.<>f__mg$cache0)));
				List<LightingExplorerWindowTab> arg_CA_0 = list;
				string arg_C0_0 = "ReflectionTable";
				SerializedPropertyDataStore.GatherDelegate arg_C0_1 = () => UnityEngine.Object.FindObjectsOfType<ReflectionProbe>();
				if (LightingExplorerWindow.<>f__mg$cache1 == null)
				{
					LightingExplorerWindow.<>f__mg$cache1 = new SerializedPropertyTable.HeaderDelegate(LightTableColumns.CreateReflectionColumns);
				}
				arg_CA_0.Add(new LightingExplorerWindowTab(new SerializedPropertyTable(arg_C0_0, arg_C0_1, LightingExplorerWindow.<>f__mg$cache1)));
				List<LightingExplorerWindowTab> arg_119_0 = list;
				string arg_10F_0 = "LightProbeTable";
				SerializedPropertyDataStore.GatherDelegate arg_10F_1 = () => UnityEngine.Object.FindObjectsOfType<LightProbeGroup>();
				if (LightingExplorerWindow.<>f__mg$cache2 == null)
				{
					LightingExplorerWindow.<>f__mg$cache2 = new SerializedPropertyTable.HeaderDelegate(LightTableColumns.CreateLightProbeColumns);
				}
				arg_119_0.Add(new LightingExplorerWindowTab(new SerializedPropertyTable(arg_10F_0, arg_10F_1, LightingExplorerWindow.<>f__mg$cache2)));
				List<LightingExplorerWindowTab> arg_151_0 = list;
				string arg_147_0 = "EmissiveMaterialTable";
				SerializedPropertyDataStore.GatherDelegate arg_147_1 = this.StaticEmissivesGatherDelegate();
				if (LightingExplorerWindow.<>f__mg$cache3 == null)
				{
					LightingExplorerWindow.<>f__mg$cache3 = new SerializedPropertyTable.HeaderDelegate(LightTableColumns.CreateEmissivesColumns);
				}
				arg_151_0.Add(new LightingExplorerWindowTab(new SerializedPropertyTable(arg_147_0, arg_147_1, LightingExplorerWindow.<>f__mg$cache3)));
				this.m_TableTabs = list;
			}
			for (int i = 0; i < this.m_TableTabs.Count; i++)
			{
				this.m_TableTabs[i].OnEnable();
			}
			EditorApplication.searchChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(base.Repaint));
			base.Repaint();
		}

		private void OnDisable()
		{
			if (this.m_TableTabs != null)
			{
				for (int i = 0; i < this.m_TableTabs.Count; i++)
				{
					this.m_TableTabs[i].OnDisable();
				}
			}
			EditorApplication.searchChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(base.Repaint));
		}

		private void OnInspectorUpdate()
		{
			if (this.m_TableTabs != null && this.m_SelectedTab >= LightingExplorerWindow.TabType.Lights && this.m_SelectedTab < (LightingExplorerWindow.TabType)this.m_TableTabs.Count)
			{
				this.m_TableTabs[(int)this.m_SelectedTab].OnInspectorUpdate();
			}
		}

		private void OnSelectionChange()
		{
			if (this.m_TableTabs != null)
			{
				for (int i = 0; i < this.m_TableTabs.Count; i++)
				{
					if (i == this.m_TableTabs.Count - 1)
					{
						int[] instanceIDs = (from m in (from mr in UnityEngine.Object.FindObjectsOfType<MeshRenderer>()
						where Selection.instanceIDs.Contains(mr.gameObject.GetInstanceID())
						select mr).SelectMany((MeshRenderer meshRenderer) => meshRenderer.sharedMaterials)
						where m != null && (m.globalIlluminationFlags & MaterialGlobalIlluminationFlags.AnyEmissive) != MaterialGlobalIlluminationFlags.None
						select m.GetInstanceID()).Union(Selection.instanceIDs).Distinct<int>().ToArray<int>();
						this.m_TableTabs[i].OnSelectionChange(instanceIDs);
					}
					else
					{
						this.m_TableTabs[i].OnSelectionChange();
					}
				}
			}
			base.Repaint();
		}

		private void OnHierarchyChange()
		{
			if (this.m_TableTabs != null)
			{
				for (int i = 0; i < this.m_TableTabs.Count; i++)
				{
					this.m_TableTabs[i].OnHierarchyChange();
				}
			}
		}

		private void OnGUI()
		{
			EditorGUIUtility.labelWidth = 130f;
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Space(this.toolbarPadding);
			float width = base.position.width - this.toolbarPadding * 2f;
			this.m_SelectedTab = (LightingExplorerWindow.TabType)GUILayout.Toolbar((int)this.m_SelectedTab, LightingExplorerWindow.Styles.TabTypes, "LargeButton", new GUILayoutOption[]
			{
				GUILayout.Width(width)
			});
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
			if (this.m_TableTabs != null && this.m_SelectedTab >= LightingExplorerWindow.TabType.Lights && this.m_SelectedTab < (LightingExplorerWindow.TabType)this.m_TableTabs.Count)
			{
				this.m_TableTabs[(int)this.m_SelectedTab].OnGUI();
			}
			EditorGUILayout.Space();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}

		private SerializedPropertyDataStore.GatherDelegate StaticEmissivesGatherDelegate()
		{
			return () => (from m in (from mr in UnityEngine.Object.FindObjectsOfType<MeshRenderer>()
			where GameObjectUtility.AreStaticEditorFlagsSet(mr.gameObject, StaticEditorFlags.LightmapStatic)
			select mr).SelectMany((MeshRenderer meshRenderer) => meshRenderer.sharedMaterials)
			where m != null && (m.globalIlluminationFlags & MaterialGlobalIlluminationFlags.AnyEmissive) != MaterialGlobalIlluminationFlags.None && m.HasProperty("_EmissionColor")
			select m).Distinct<Material>().ToArray<Material>();
		}
	}
}
