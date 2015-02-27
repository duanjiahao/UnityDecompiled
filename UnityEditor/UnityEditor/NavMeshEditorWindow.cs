using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace UnityEditor
{
	internal class NavMeshEditorWindow : EditorWindow, IHasCustomMenu
	{
		private enum Mode
		{
			ObjectSettings,
			BakeSettings,
			LayerSettings
		}
		private class Styles
		{
			public readonly GUIContent m_AgentRadiusContent = EditorGUIUtility.TextContent("NavMeshEditorWindow.Radius");
			public readonly GUIContent m_AgentHeightContent = EditorGUIUtility.TextContent("NavMeshEditorWindow.Height");
			public readonly GUIContent m_AgentSlopeContent = EditorGUIUtility.TextContent("NavMeshEditorWindow.MaxSlope");
			public readonly GUIContent m_AgentDropContent = EditorGUIUtility.TextContent("NavMeshEditorWindow.DropHeight");
			public readonly GUIContent m_AgentClimbContent = EditorGUIUtility.TextContent("NavMeshEditorWindow.StepHeight");
			public readonly GUIContent m_AgentJumpContent = EditorGUIUtility.TextContent("NavMeshEditorWindow.JumpDistance");
			public readonly GUIContent m_AgentPlacementContent = EditorGUIUtility.TextContent("NavMeshEditorWindow.HeightMesh");
			public readonly GUIContent m_MinRegionAreaContent = EditorGUIUtility.TextContent("NavMeshEditorWindow.MinRegionArea");
			public readonly GUIContent m_WidthInaccuracyContent = EditorGUIUtility.TextContent("NavMeshEditorWindow.WidthInaccuracy");
			public readonly GUIContent m_HeightInaccuracyContent = EditorGUIUtility.TextContent("NavMeshEditorWindow.HeightInaccuracy");
			public readonly GUIContent m_GeneralHeader = new GUIContent("General");
			public readonly GUIContent m_OffmeshHeader = new GUIContent("Generated Off Mesh Links");
			public readonly GUIContent m_AdvancedHeader = new GUIContent("Advanced");
			public readonly GUIContent[] m_ModeToggles = new GUIContent[]
			{
				EditorGUIUtility.TextContent("NavmeshEditor.ObjectSettings"),
				EditorGUIUtility.TextContent("NavmeshEditor.BakeSettings"),
				EditorGUIUtility.TextContent("NavmeshEditor.LayerSettings")
			};
		}
		private const string kRootPath = "m_BuildSettings.";
		private static NavMeshEditorWindow s_MsNavMeshEditorWindow;
		private SerializedObject m_Object;
		private SerializedProperty m_AgentRadius;
		private SerializedProperty m_AgentHeight;
		private SerializedProperty m_AgentSlope;
		private SerializedProperty m_AgentClimb;
		private SerializedProperty m_LedgeDropHeight;
		private SerializedProperty m_MaxJumpAcrossDistance;
		private SerializedProperty m_AccuratePlacement;
		private SerializedProperty m_MinRegionArea;
		private SerializedProperty m_WidthInaccuracy;
		private SerializedProperty m_HeightInaccuracy;
		private static NavMeshEditorWindow.Styles s_Styles;
		private Vector2 m_ScrollPos = Vector2.zero;
		private bool m_HasSelectedNavMeshAgents;
		private NavMeshEditorWindow.Mode m_Mode;
		private bool m_Advanced;
		[MenuItem("Window/Navigation", false, 2100)]
		public static void SetupWindow()
		{
			NavMeshEditorWindow window = EditorWindow.GetWindow<NavMeshEditorWindow>(new Type[]
			{
				typeof(InspectorWindow)
			});
			window.title = EditorGUIUtility.TextContent("NavmeshEditor.WindowTitle").text;
			window.minSize = new Vector2(300f, 360f);
		}
		public void OnEnable()
		{
			NavMeshEditorWindow.s_MsNavMeshEditorWindow = this;
			NavMeshEditorWindow.s_Styles = new NavMeshEditorWindow.Styles();
			this.Init();
			EditorApplication.searchChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(base.Repaint));
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
			base.Repaint();
		}
		private void Init()
		{
			this.m_Object = new SerializedObject(NavMeshBuilder.navMeshSettingsObject);
			this.m_AgentRadius = this.m_Object.FindProperty("m_BuildSettings.agentRadius");
			this.m_AgentHeight = this.m_Object.FindProperty("m_BuildSettings.agentHeight");
			this.m_AgentSlope = this.m_Object.FindProperty("m_BuildSettings.agentSlope");
			this.m_LedgeDropHeight = this.m_Object.FindProperty("m_BuildSettings.ledgeDropHeight");
			this.m_AgentClimb = this.m_Object.FindProperty("m_BuildSettings.agentClimb");
			this.m_MaxJumpAcrossDistance = this.m_Object.FindProperty("m_BuildSettings.maxJumpAcrossDistance");
			this.m_AccuratePlacement = this.m_Object.FindProperty("m_BuildSettings.accuratePlacement");
			this.m_MinRegionArea = this.m_Object.FindProperty("m_BuildSettings.minRegionArea");
			this.m_WidthInaccuracy = this.m_Object.FindProperty("m_BuildSettings.widthInaccuracy");
			this.m_HeightInaccuracy = this.m_Object.FindProperty("m_BuildSettings.heightInaccuracy");
		}
		public void OnDisable()
		{
			NavMeshEditorWindow.s_MsNavMeshEditorWindow = null;
			EditorApplication.searchChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(base.Repaint));
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
		}
		private void OnSelectionChange()
		{
			this.m_HasSelectedNavMeshAgents = false;
			Transform[] transforms = Selection.GetTransforms((SelectionMode)12);
			Transform[] array = transforms;
			for (int i = 0; i < array.Length; i++)
			{
				Transform transform = array[i];
				if (transform.gameObject.GetComponent<NavMeshAgent>() != null)
				{
					this.m_HasSelectedNavMeshAgents = true;
					break;
				}
			}
			this.m_ScrollPos = Vector2.zero;
			if (this.m_Mode == NavMeshEditorWindow.Mode.ObjectSettings)
			{
				base.Repaint();
			}
		}
		private void ModeToggle()
		{
			this.m_Mode = (NavMeshEditorWindow.Mode)GUILayout.Toolbar((int)this.m_Mode, NavMeshEditorWindow.s_Styles.m_ModeToggles, "LargeButton", new GUILayoutOption[0]);
		}
		public void OnGUI()
		{
			if (this.m_Object.targetObject == null)
			{
				this.Init();
			}
			this.m_Object.Update();
			EditorGUIUtility.labelWidth = 130f;
			EditorGUILayout.Space();
			this.ModeToggle();
			EditorGUILayout.Space();
			this.m_ScrollPos = EditorGUILayout.BeginScrollView(this.m_ScrollPos, new GUILayoutOption[0]);
			switch (this.m_Mode)
			{
			case NavMeshEditorWindow.Mode.ObjectSettings:
				NavMeshEditorWindow.ObjectSettings();
				break;
			case NavMeshEditorWindow.Mode.BakeSettings:
				this.BakeSettings();
				break;
			case NavMeshEditorWindow.Mode.LayerSettings:
				this.LayerSettings();
				break;
			}
			EditorGUILayout.EndScrollView();
			NavMeshEditorWindow.BakeButtons();
			this.m_Object.ApplyModifiedProperties();
		}
		public void OnBecameVisible()
		{
			if (NavMeshVisualizationSettings.showNavigation)
			{
				return;
			}
			NavMeshVisualizationSettings.showNavigation = true;
			NavMeshEditorWindow.RepaintSceneAndGameViews();
		}
		public void OnBecameInvisible()
		{
			NavMeshVisualizationSettings.showNavigation = false;
			NavMeshEditorWindow.RepaintSceneAndGameViews();
		}
		private static void RepaintSceneAndGameViews()
		{
			SceneView.RepaintAll();
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(GameView));
			for (int i = 0; i < array.Length; i++)
			{
				GameView gameView = (GameView)array[i];
				gameView.Repaint();
			}
		}
		public void OnSceneViewGUI(SceneView sceneView)
		{
			if (!NavMeshVisualizationSettings.showNavigation)
			{
				return;
			}
			SceneViewOverlay.Window(new GUIContent("Navmesh Display"), new SceneViewOverlay.WindowFunction(NavMeshEditorWindow.DisplayControls), 300, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
			if (this.m_HasSelectedNavMeshAgents)
			{
				SceneViewOverlay.Window(new GUIContent("Agent Display"), new SceneViewOverlay.WindowFunction(NavMeshEditorWindow.DisplayAgentControls), 300, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
			}
		}
		private static void DisplayControls(UnityEngine.Object target, SceneView sceneView)
		{
			EditorGUIUtility.labelWidth = 150f;
			bool flag = false;
			bool showNavMesh = NavMeshVisualizationSettings.showNavMesh;
			if (showNavMesh != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("NavMeshEditorWindow.ShowNavMesh"), showNavMesh, new GUILayoutOption[0]))
			{
				NavMeshVisualizationSettings.showNavMesh = !showNavMesh;
				flag = true;
			}
			EditorGUI.BeginDisabledGroup(!NavMeshVisualizationSettings.hasHeightMesh);
			bool showHeightMesh = NavMeshVisualizationSettings.showHeightMesh;
			if (showHeightMesh != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("NavMeshEditorWindow.ShowHeightMesh"), showHeightMesh, new GUILayoutOption[0]))
			{
				NavMeshVisualizationSettings.showHeightMesh = !showHeightMesh;
				flag = true;
			}
			EditorGUI.EndDisabledGroup();
			if (flag)
			{
				NavMeshEditorWindow.RepaintSceneAndGameViews();
			}
		}
		private static void DisplayAgentControls(UnityEngine.Object target, SceneView sceneView)
		{
			EditorGUIUtility.labelWidth = 150f;
			bool flag = false;
			bool showAgentPath = NavMeshVisualizationSettings.showAgentPath;
			if (showAgentPath != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("NavMeshEditorWindow.ShowAgentPath"), showAgentPath, new GUILayoutOption[0]))
			{
				NavMeshVisualizationSettings.showAgentPath = !showAgentPath;
				flag = true;
			}
			bool showAgentPathInfo = NavMeshVisualizationSettings.showAgentPathInfo;
			if (showAgentPathInfo != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("NavMeshEditorWindow.ShowAgentPathInfo"), showAgentPathInfo, new GUILayoutOption[0]))
			{
				NavMeshVisualizationSettings.showAgentPathInfo = !showAgentPathInfo;
				flag = true;
			}
			bool showAgentNeighbours = NavMeshVisualizationSettings.showAgentNeighbours;
			if (showAgentNeighbours != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("NavMeshEditorWindow.ShowAgentNeighbours"), showAgentNeighbours, new GUILayoutOption[0]))
			{
				NavMeshVisualizationSettings.showAgentNeighbours = !showAgentNeighbours;
				flag = true;
			}
			bool showAgentWalls = NavMeshVisualizationSettings.showAgentWalls;
			if (showAgentWalls != EditorGUILayout.Toggle(EditorGUIUtility.TextContent("NavMeshEditorWindow.ShowAgentWalls"), showAgentWalls, new GUILayoutOption[0]))
			{
				NavMeshVisualizationSettings.showAgentWalls = !showAgentWalls;
				flag = true;
			}
			if (flag)
			{
				NavMeshEditorWindow.RepaintSceneAndGameViews();
			}
		}
		public virtual void AddItemsToMenu(GenericMenu menu)
		{
			menu.AddItem(new GUIContent("Reset Bake Settings"), false, new GenericMenu.MenuFunction(this.ResetBakeSettings));
		}
		private void ResetBakeSettings()
		{
			Unsupported.SmartReset(NavMeshBuilder.navMeshSettingsObject);
		}
		public static void BackgroundTaskStatusChanged()
		{
			if (NavMeshEditorWindow.s_MsNavMeshEditorWindow != null)
			{
				NavMeshEditorWindow.s_MsNavMeshEditorWindow.Repaint();
			}
		}
		private static IEnumerable<GameObject> GetObjectsRecurse(GameObject root)
		{
			List<GameObject> list = new List<GameObject>
			{
				root
			};
			foreach (Transform transform in root.transform)
			{
				list.AddRange(NavMeshEditorWindow.GetObjectsRecurse(transform.gameObject));
			}
			return list;
		}
		private static List<GameObject> GetObjects(bool includeChildren)
		{
			if (includeChildren)
			{
				List<GameObject> list = new List<GameObject>();
				GameObject[] gameObjects = Selection.gameObjects;
				for (int i = 0; i < gameObjects.Length; i++)
				{
					GameObject root = gameObjects[i];
					list.AddRange(NavMeshEditorWindow.GetObjectsRecurse(root));
				}
				return list;
			}
			return new List<GameObject>(Selection.gameObjects);
		}
		private static bool SelectionHasChildren()
		{
			return Selection.gameObjects.Any((GameObject obj) => obj.transform.childCount > 0);
		}
		private static void SetNavMeshLayer(int layer, bool includeChildren)
		{
			List<GameObject> objects = NavMeshEditorWindow.GetObjects(includeChildren);
			if (objects.Count <= 0)
			{
				return;
			}
			Undo.RecordObjects(objects.ToArray(), "Change NavMesh layer");
			foreach (GameObject current in objects)
			{
				GameObjectUtility.SetNavMeshLayer(current, layer);
			}
		}
		private static void ObjectSettings()
		{
			bool flag = true;
			SceneModeUtility.SearchBar(new Type[]
			{
				typeof(MeshRenderer),
				typeof(Terrain)
			});
			EditorGUILayout.Space();
			GameObject[] array;
			MeshRenderer[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<MeshRenderer>(out array, new Type[0]);
			if (array.Length > 0)
			{
				flag = false;
				NavMeshEditorWindow.ObjectSettings(selectedObjectsOfType, array);
			}
			Terrain[] selectedObjectsOfType2 = SceneModeUtility.GetSelectedObjectsOfType<Terrain>(out array, new Type[0]);
			if (array.Length > 0)
			{
				flag = false;
				NavMeshEditorWindow.ObjectSettings(selectedObjectsOfType2, array);
			}
			if (flag)
			{
				GUILayout.Label("Select a MeshRenderer or a Terrain from the scene.", EditorStyles.helpBox, new GUILayoutOption[0]);
			}
		}
		private static void ObjectSettings(UnityEngine.Object[] components, GameObject[] gos)
		{
			EditorGUILayout.MultiSelectionObjectTitleBar(components);
			SerializedObject serializedObject = new SerializedObject(gos);
			EditorGUI.BeginDisabledGroup(!SceneModeUtility.StaticFlagField("Navigation Static", serializedObject.FindProperty("m_StaticEditorFlags"), 8));
			SceneModeUtility.StaticFlagField("OffMeshLink Generation", serializedObject.FindProperty("m_StaticEditorFlags"), 32);
			SerializedProperty serializedProperty = serializedObject.FindProperty("m_NavMeshLayer");
			EditorGUI.BeginChangeCheck();
			EditorGUI.showMixedValue = serializedProperty.hasMultipleDifferentValues;
			string[] navMeshLayerNames = GameObjectUtility.GetNavMeshLayerNames();
			int navMeshLayer = GameObjectUtility.GetNavMeshLayer(gos[0]);
			int selectedIndex = -1;
			for (int i = 0; i < navMeshLayerNames.Length; i++)
			{
				if (GameObjectUtility.GetNavMeshLayerFromName(navMeshLayerNames[i]) == navMeshLayer)
				{
					selectedIndex = i;
					break;
				}
			}
			int num = EditorGUILayout.Popup("Navigation Layer", selectedIndex, navMeshLayerNames, new GUILayoutOption[0]);
			EditorGUI.showMixedValue = false;
			if (EditorGUI.EndChangeCheck())
			{
				int navMeshLayerFromName = GameObjectUtility.GetNavMeshLayerFromName(navMeshLayerNames[num]);
				GameObjectUtility.ShouldIncludeChildren shouldIncludeChildren = GameObjectUtility.DisplayUpdateChildrenDialogIfNeeded(Selection.gameObjects, "Change Navigation Layer", "Do you want change the navigation layer to " + navMeshLayerNames[num] + " for all the child objects as well?");
				if (shouldIncludeChildren != GameObjectUtility.ShouldIncludeChildren.Cancel)
				{
					serializedProperty.intValue = navMeshLayerFromName;
					NavMeshEditorWindow.SetNavMeshLayer(navMeshLayerFromName, shouldIncludeChildren == GameObjectUtility.ShouldIncludeChildren.IncludeChildren);
				}
			}
			EditorGUI.EndDisabledGroup();
			serializedObject.ApplyModifiedProperties();
		}
		private void BakeSettings()
		{
			EditorGUILayout.LabelField(NavMeshEditorWindow.s_Styles.m_GeneralHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
			float num = EditorGUILayout.FloatField(NavMeshEditorWindow.s_Styles.m_AgentRadiusContent, this.m_AgentRadius.floatValue, new GUILayoutOption[0]);
			if (num >= 0.001f && !Mathf.Approximately(num - this.m_AgentRadius.floatValue, 0f))
			{
				this.m_AgentRadius.floatValue = num;
			}
			float num2 = EditorGUILayout.FloatField(NavMeshEditorWindow.s_Styles.m_AgentHeightContent, this.m_AgentHeight.floatValue, new GUILayoutOption[0]);
			if (num2 >= 0.001f && !Mathf.Approximately(num2 - this.m_AgentHeight.floatValue, 0f))
			{
				this.m_AgentHeight.floatValue = num2;
			}
			EditorGUILayout.Slider(this.m_AgentSlope, 0f, 90f, NavMeshEditorWindow.s_Styles.m_AgentSlopeContent, new GUILayoutOption[0]);
			float num3 = EditorGUILayout.FloatField(NavMeshEditorWindow.s_Styles.m_AgentClimbContent, this.m_AgentClimb.floatValue, new GUILayoutOption[0]);
			if (num3 >= 0f && !Mathf.Approximately(this.m_AgentClimb.floatValue - num3, 0f))
			{
				this.m_AgentClimb.floatValue = num3;
			}
			if (this.m_AgentClimb.floatValue >= this.m_AgentHeight.floatValue)
			{
				EditorGUILayout.HelpBox("Step height should be less than agent height.\nClamping step height to " + this.m_AgentHeight.floatValue + ".", MessageType.Warning);
			}
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(NavMeshEditorWindow.s_Styles.m_OffmeshHeader, EditorStyles.boldLabel, new GUILayoutOption[0]);
			bool flag = !Application.HasProLicense();
			if (flag)
			{
				EditorGUILayout.HelpBox("This is only available in the Pro version of Unity.", MessageType.Warning);
				if (this.m_LedgeDropHeight.floatValue != 0f)
				{
					this.m_LedgeDropHeight.floatValue = 0f;
				}
				if (this.m_MaxJumpAcrossDistance.floatValue != 0f)
				{
					this.m_MaxJumpAcrossDistance.floatValue = 0f;
				}
				GUI.enabled = false;
			}
			float num4 = EditorGUILayout.FloatField(NavMeshEditorWindow.s_Styles.m_AgentDropContent, this.m_LedgeDropHeight.floatValue, new GUILayoutOption[0]);
			if (num4 >= 0f && !Mathf.Approximately(num4 - this.m_LedgeDropHeight.floatValue, 0f))
			{
				this.m_LedgeDropHeight.floatValue = num4;
			}
			float num5 = EditorGUILayout.FloatField(NavMeshEditorWindow.s_Styles.m_AgentJumpContent, this.m_MaxJumpAcrossDistance.floatValue, new GUILayoutOption[0]);
			if (num5 >= 0f && !Mathf.Approximately(num5 - this.m_MaxJumpAcrossDistance.floatValue, 0f))
			{
				this.m_MaxJumpAcrossDistance.floatValue = num5;
			}
			if (flag)
			{
				GUI.enabled = true;
			}
			EditorGUILayout.Space();
			this.m_Advanced = GUILayout.Toggle(this.m_Advanced, NavMeshEditorWindow.s_Styles.m_AdvancedHeader, EditorStyles.foldout, new GUILayoutOption[0]);
			if (this.m_Advanced)
			{
				float num6 = EditorGUILayout.FloatField(NavMeshEditorWindow.s_Styles.m_MinRegionAreaContent, this.m_MinRegionArea.floatValue, new GUILayoutOption[0]);
				if (num6 >= 0f && num6 != this.m_MinRegionArea.floatValue)
				{
					this.m_MinRegionArea.floatValue = num6;
				}
				EditorGUILayout.Slider(this.m_WidthInaccuracy, 1f, 100f, NavMeshEditorWindow.s_Styles.m_WidthInaccuracyContent, new GUILayoutOption[0]);
				EditorGUILayout.Slider(this.m_HeightInaccuracy, 1f, 100f, NavMeshEditorWindow.s_Styles.m_HeightInaccuracyContent, new GUILayoutOption[0]);
				bool flag2 = EditorGUILayout.Toggle(NavMeshEditorWindow.s_Styles.m_AgentPlacementContent, this.m_AccuratePlacement.boolValue, new GUILayoutOption[0]);
				if (flag2 != this.m_AccuratePlacement.boolValue)
				{
					this.m_AccuratePlacement.boolValue = flag2;
				}
			}
		}
		private void LayerSettings()
		{
			UnityEngine.Object serializedAssetInterfaceSingleton = Unsupported.GetSerializedAssetInterfaceSingleton("NavMeshLayers");
			SerializedObject obj = new SerializedObject(serializedAssetInterfaceSingleton);
			Editor.DoDrawDefaultInspector(obj);
		}
		private static void BakeButtons()
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			bool enabled = GUI.enabled;
			GUI.enabled &= !Application.isPlaying;
			if (GUILayout.Button("Clear", new GUILayoutOption[]
			{
				GUILayout.Width(95f)
			}))
			{
				NavMeshBuilder.ClearAllNavMeshes();
			}
			GUI.enabled = enabled;
			if (NavMeshBuilder.isRunning)
			{
				if (GUILayout.Button("Cancel", new GUILayoutOption[]
				{
					GUILayout.Width(95f)
				}))
				{
					NavMeshBuilder.Cancel();
				}
			}
			else
			{
				enabled = GUI.enabled;
				GUI.enabled &= !Application.isPlaying;
				if (GUILayout.Button("Bake", new GUILayoutOption[]
				{
					GUILayout.Width(95f)
				}))
				{
					NavMeshBuilder.BuildNavMeshAsync();
				}
				GUI.enabled = enabled;
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
	}
}
