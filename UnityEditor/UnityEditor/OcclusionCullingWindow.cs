using System;
using UnityEngine;

namespace UnityEditor
{
	[EditorWindowTitle(title = "Occlusion", icon = "Occlusion")]
	internal class OcclusionCullingWindow : EditorWindow
	{
		private class Styles
		{
			public GUIContent[] ModeToggles = new GUIContent[]
			{
				new GUIContent("Object"),
				new GUIContent("Bake"),
				new GUIContent("Visualization")
			};

			public GUIStyle labelStyle = EditorStyles.wordWrappedMiniLabel;

			public GUIContent emptyAreaSelection = new GUIContent("Select a Mesh Renderer or an Occlusion Area from the scene.");

			public GUIContent emptyCameraSelection = new GUIContent("Select a Camera from the scene.");

			public GUIContent visualizationNote = new GUIContent("The visualization may not correspond to current bake settings and Occlusion Area placements if they have been changed since last bake.");

			public GUIContent seeVisualizationInScene = new GUIContent("See the occlusion culling visualization in the Scene View based on the selected Camera.");

			public GUIContent noOcclusionData = new GUIContent("No occlusion data has been baked.");

			public GUIContent smallestHole = EditorGUIUtility.TextContent("Smallest Hole|Smallest hole in the geometry through which the camera is supposed to see. The single float value of the parameter represents the diameter of the imaginary smallest hole, i.e. the maximum extent of a 3D object that fits through the hole.");

			public GUIContent backfaceThreshold = EditorGUIUtility.TextContent("Backface Threshold|The backface threshold is a size optimization that reduces unnecessary details by testing backfaces. A value of 100 is robust and never removes any backfaces. A value of 5 aggressively reduces the data based on locations with visible backfaces. The idea is that typically valid camera positions cannot see many backfaces.  For example geometry under terrain and inside solid objects can be removed.");

			public GUIContent farClipPlane = EditorGUIUtility.TextContent("Far Clip Plane|Far Clip Plane used during baking. This should match the largest far clip plane used by any camera in the scene. A value of 0.0 sets the far plane to Infinity.");

			public GUIContent smallestOccluder = EditorGUIUtility.TextContent("Smallest Occluder|The size of the smallest object that will be used to hide other objects when doing occlusion culling. For example, if a value of 4 is chosen, then all the objects that are higher or wider than 4 meters will block visibility and the objects that are smaller than that will not. This value is a tradeoff between occlusion accuracy and storage size.");

			public GUIContent defaultParameterText = EditorGUIUtility.TextContent("Default Parameters|The default parameters guarantee that any given scene computes fast and the occlusion culling results are good. As the parameters are always scene specific, better results will be achieved when fine tuning the parameters on a scene to scene basis. All the parameters are dependent on the unit scale of the scene and it is imperative that the unit scale parameter is set correctly before setting the default values.");
		}

		private enum Mode
		{
			AreaSettings,
			BakeSettings,
			Visualization
		}

		private static bool s_IsVisible;

		private bool m_PreVis;

		private string m_Warning;

		private static OcclusionCullingWindow ms_OcclusionCullingWindow;

		private UnityEngine.Object[] m_Objects;

		private Vector2 m_ScrollPosition = Vector2.zero;

		private OcclusionCullingWindow.Mode m_Mode;

		private static OcclusionCullingWindow.Styles s_Styles;

		private void OnBecameVisible()
		{
			if (OcclusionCullingWindow.s_IsVisible)
			{
				return;
			}
			OcclusionCullingWindow.s_IsVisible = true;
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
			StaticOcclusionCullingVisualization.showOcclusionCulling = true;
			SceneView.RepaintAll();
		}

		private void OnBecameInvisible()
		{
			OcclusionCullingWindow.s_IsVisible = false;
			SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc)Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneViewGUI));
			StaticOcclusionCullingVisualization.showOcclusionCulling = false;
			SceneView.RepaintAll();
		}

		private void OnSelectionChange()
		{
			if (this.m_Mode == OcclusionCullingWindow.Mode.AreaSettings || this.m_Mode == OcclusionCullingWindow.Mode.Visualization)
			{
				base.Repaint();
			}
		}

		private void OnEnable()
		{
			base.titleContent = base.GetLocalizedTitleContent();
			OcclusionCullingWindow.ms_OcclusionCullingWindow = this;
			base.autoRepaintOnSceneChange = true;
			EditorApplication.searchChanged = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(base.Repaint));
			base.Repaint();
		}

		private void OnDisable()
		{
			OcclusionCullingWindow.ms_OcclusionCullingWindow = null;
			EditorApplication.searchChanged = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.searchChanged, new EditorApplication.CallbackFunction(base.Repaint));
		}

		private static void BackgroundTaskStatusChanged()
		{
			if (OcclusionCullingWindow.ms_OcclusionCullingWindow)
			{
				OcclusionCullingWindow.ms_OcclusionCullingWindow.Repaint();
			}
		}

		[MenuItem("Window/Occlusion Culling", false, 2099)]
		private static void GenerateWindow()
		{
			OcclusionCullingWindow window = EditorWindow.GetWindow<OcclusionCullingWindow>(new Type[]
			{
				typeof(InspectorWindow)
			});
			window.minSize = new Vector2(300f, 250f);
		}

		private void SummaryGUI()
		{
			GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
			if (StaticOcclusionCulling.umbraDataSize == 0)
			{
				GUILayout.Label(OcclusionCullingWindow.s_Styles.noOcclusionData, OcclusionCullingWindow.s_Styles.labelStyle, new GUILayoutOption[0]);
			}
			else
			{
				GUILayout.Label("Last bake:", OcclusionCullingWindow.s_Styles.labelStyle, new GUILayoutOption[0]);
				GUILayout.BeginHorizontal(new GUILayoutOption[0]);
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.Label("Occlusion data size ", OcclusionCullingWindow.s_Styles.labelStyle, new GUILayoutOption[0]);
				GUILayout.EndVertical();
				GUILayout.BeginVertical(new GUILayoutOption[0]);
				GUILayout.Label(EditorUtility.FormatBytes(StaticOcclusionCulling.umbraDataSize), OcclusionCullingWindow.s_Styles.labelStyle, new GUILayoutOption[0]);
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
		}

		private OcclusionArea CreateNewArea()
		{
			GameObject gameObject = new GameObject("Occlusion Area");
			OcclusionArea result = gameObject.AddComponent<OcclusionArea>();
			Selection.activeGameObject = gameObject;
			return result;
		}

		private void AreaSelectionGUI()
		{
			bool flag = true;
			Type type = SceneModeUtility.SearchBar(new Type[]
			{
				typeof(Renderer),
				typeof(OcclusionArea)
			});
			EditorGUILayout.Space();
			GameObject[] array;
			OcclusionArea[] selectedObjectsOfType = SceneModeUtility.GetSelectedObjectsOfType<OcclusionArea>(out array, new Type[0]);
			if (array.Length > 0)
			{
				flag = false;
				EditorGUILayout.MultiSelectionObjectTitleBar(selectedObjectsOfType);
				SerializedObject serializedObject = new SerializedObject(selectedObjectsOfType);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_IsViewVolume"), new GUILayoutOption[0]);
				serializedObject.ApplyModifiedProperties();
			}
			Renderer[] selectedObjectsOfType2 = SceneModeUtility.GetSelectedObjectsOfType<Renderer>(out array, new Type[]
			{
				typeof(MeshRenderer),
				typeof(SkinnedMeshRenderer)
			});
			if (array.Length > 0)
			{
				flag = false;
				EditorGUILayout.MultiSelectionObjectTitleBar(selectedObjectsOfType2);
				SerializedObject serializedObject2 = new SerializedObject(array);
				SceneModeUtility.StaticFlagField("Occluder Static", serializedObject2.FindProperty("m_StaticEditorFlags"), 2);
				SceneModeUtility.StaticFlagField("Occludee Static", serializedObject2.FindProperty("m_StaticEditorFlags"), 16);
				serializedObject2.ApplyModifiedProperties();
			}
			if (flag)
			{
				GUILayout.Label(OcclusionCullingWindow.s_Styles.emptyAreaSelection, EditorStyles.helpBox, new GUILayoutOption[0]);
				if (type == typeof(OcclusionArea))
				{
					EditorGUIUtility.labelWidth = 80f;
					EditorGUILayout.Space();
					EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
					EditorGUILayout.PrefixLabel("Create New");
					if (GUILayout.Button("Occlusion Area", EditorStyles.miniButton, new GUILayoutOption[]
					{
						GUILayout.ExpandWidth(false)
					}))
					{
						this.CreateNewArea();
					}
					EditorGUILayout.EndHorizontal();
				}
			}
		}

		private void CameraSelectionGUI()
		{
			SceneModeUtility.SearchBar(new Type[]
			{
				typeof(Camera)
			});
			EditorGUILayout.Space();
			Camera camera = null;
			if (Selection.activeGameObject)
			{
				camera = Selection.activeGameObject.GetComponent<Camera>();
			}
			if (camera)
			{
				Camera[] objects = new Camera[]
				{
					camera
				};
				EditorGUILayout.MultiSelectionObjectTitleBar(objects);
				EditorGUILayout.HelpBox(OcclusionCullingWindow.s_Styles.seeVisualizationInScene.text, MessageType.Info);
			}
			else
			{
				GUILayout.Label(OcclusionCullingWindow.s_Styles.emptyCameraSelection, EditorStyles.helpBox, new GUILayoutOption[0]);
			}
		}

		private void BakeSettings()
		{
			float width = 150f;
			if (GUILayout.Button("Set default parameters", new GUILayoutOption[]
			{
				GUILayout.Width(width)
			}))
			{
				GUIUtility.keyboardControl = 0;
				StaticOcclusionCulling.SetDefaultOcclusionBakeSettings();
			}
			GUILayout.Label(OcclusionCullingWindow.s_Styles.defaultParameterText.tooltip, EditorStyles.helpBox, new GUILayoutOption[0]);
			EditorGUI.BeginChangeCheck();
			float smallestOccluder = EditorGUILayout.FloatField(OcclusionCullingWindow.s_Styles.smallestOccluder, StaticOcclusionCulling.smallestOccluder, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				StaticOcclusionCulling.smallestOccluder = smallestOccluder;
			}
			EditorGUI.BeginChangeCheck();
			float smallestHole = EditorGUILayout.FloatField(OcclusionCullingWindow.s_Styles.smallestHole, StaticOcclusionCulling.smallestHole, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				StaticOcclusionCulling.smallestHole = smallestHole;
			}
			EditorGUI.BeginChangeCheck();
			float backfaceThreshold = EditorGUILayout.Slider(OcclusionCullingWindow.s_Styles.backfaceThreshold, StaticOcclusionCulling.backfaceThreshold, 5f, 100f, new GUILayoutOption[0]);
			if (EditorGUI.EndChangeCheck())
			{
				StaticOcclusionCulling.backfaceThreshold = backfaceThreshold;
			}
		}

		private void BakeButtons()
		{
			float width = 95f;
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			bool flag = !EditorApplication.isPlayingOrWillChangePlaymode;
			GUI.enabled = (StaticOcclusionCulling.umbraDataSize != 0 && flag);
			if (GUILayout.Button("Clear", new GUILayoutOption[]
			{
				GUILayout.Width(width)
			}))
			{
				StaticOcclusionCulling.Clear();
			}
			GUI.enabled = flag;
			if (StaticOcclusionCulling.isRunning)
			{
				if (GUILayout.Button("Cancel", new GUILayoutOption[]
				{
					GUILayout.Width(width)
				}))
				{
					StaticOcclusionCulling.Cancel();
				}
			}
			else if (GUILayout.Button("Bake", new GUILayoutOption[]
			{
				GUILayout.Width(width)
			}))
			{
				StaticOcclusionCulling.GenerateInBackground();
			}
			GUILayout.EndHorizontal();
			GUI.enabled = true;
		}

		private void ModeToggle()
		{
			this.m_Mode = (OcclusionCullingWindow.Mode)GUILayout.Toolbar((int)this.m_Mode, OcclusionCullingWindow.s_Styles.ModeToggles, "LargeButton", new GUILayoutOption[0]);
			if (GUI.changed)
			{
				if (this.m_Mode == OcclusionCullingWindow.Mode.Visualization && StaticOcclusionCulling.umbraDataSize > 0)
				{
					StaticOcclusionCullingVisualization.showPreVisualization = false;
				}
				else
				{
					StaticOcclusionCullingVisualization.showPreVisualization = true;
				}
				SceneView.RepaintAll();
			}
		}

		private void OnGUI()
		{
			if (OcclusionCullingWindow.s_Styles == null)
			{
				OcclusionCullingWindow.s_Styles = new OcclusionCullingWindow.Styles();
			}
			if (this.m_Mode != OcclusionCullingWindow.Mode.Visualization && !StaticOcclusionCullingVisualization.showPreVisualization)
			{
				this.m_Mode = OcclusionCullingWindow.Mode.Visualization;
			}
			EditorGUILayout.Space();
			this.ModeToggle();
			EditorGUILayout.Space();
			this.m_ScrollPosition = EditorGUILayout.BeginScrollView(this.m_ScrollPosition, new GUILayoutOption[0]);
			switch (this.m_Mode)
			{
			case OcclusionCullingWindow.Mode.AreaSettings:
				this.AreaSelectionGUI();
				break;
			case OcclusionCullingWindow.Mode.BakeSettings:
				this.BakeSettings();
				break;
			case OcclusionCullingWindow.Mode.Visualization:
				if (StaticOcclusionCulling.umbraDataSize > 0)
				{
					this.CameraSelectionGUI();
					GUILayout.FlexibleSpace();
					GUILayout.Label(OcclusionCullingWindow.s_Styles.visualizationNote, EditorStyles.helpBox, new GUILayoutOption[0]);
				}
				else
				{
					GUILayout.Label(OcclusionCullingWindow.s_Styles.noOcclusionData, EditorStyles.helpBox, new GUILayoutOption[0]);
				}
				break;
			}
			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space();
			this.BakeButtons();
			EditorGUILayout.Space();
			this.SummaryGUI();
		}

		public void OnSceneViewGUI(SceneView sceneView)
		{
			if (!OcclusionCullingWindow.s_IsVisible)
			{
				return;
			}
			SceneViewOverlay.Window(new GUIContent("Occlusion Culling"), new SceneViewOverlay.WindowFunction(this.DisplayControls), 100, SceneViewOverlay.WindowDisplayOption.OneWindowPerTarget);
		}

		private void OnDidOpenScene()
		{
			StaticOcclusionCulling.InvalidatePrevisualisationData();
			base.Repaint();
		}

		private void SetShowVolumePreVis()
		{
			StaticOcclusionCullingVisualization.showPreVisualization = true;
			if (this.m_Mode == OcclusionCullingWindow.Mode.Visualization)
			{
				this.m_Mode = OcclusionCullingWindow.Mode.AreaSettings;
			}
			if (OcclusionCullingWindow.ms_OcclusionCullingWindow)
			{
				OcclusionCullingWindow.ms_OcclusionCullingWindow.Repaint();
			}
			SceneView.RepaintAll();
		}

		private void SetShowVolumeCulling()
		{
			StaticOcclusionCullingVisualization.showPreVisualization = false;
			this.m_Mode = OcclusionCullingWindow.Mode.Visualization;
			if (OcclusionCullingWindow.ms_OcclusionCullingWindow)
			{
				OcclusionCullingWindow.ms_OcclusionCullingWindow.Repaint();
			}
			SceneView.RepaintAll();
		}

		private bool ShowModePopup(Rect popupRect)
		{
			int umbraDataSize = StaticOcclusionCulling.umbraDataSize;
			if (this.m_PreVis != StaticOcclusionCullingVisualization.showPreVisualization)
			{
				SceneView.RepaintAll();
			}
			if (Event.current.type == EventType.Layout)
			{
				this.m_PreVis = StaticOcclusionCullingVisualization.showPreVisualization;
			}
			string[] array = new string[]
			{
				"Edit",
				"Visualize"
			};
			int num = (!this.m_PreVis) ? 1 : 0;
			if (EditorGUI.ButtonMouseDown(popupRect, new GUIContent(array[num]), FocusType.Passive, EditorStyles.popup))
			{
				GenericMenu genericMenu = new GenericMenu();
				genericMenu.AddItem(new GUIContent(array[0]), num == 0, new GenericMenu.MenuFunction(this.SetShowVolumePreVis));
				if (umbraDataSize > 0)
				{
					genericMenu.AddItem(new GUIContent(array[1]), num == 1, new GenericMenu.MenuFunction(this.SetShowVolumeCulling));
				}
				else
				{
					genericMenu.AddDisabledItem(new GUIContent(array[1]));
				}
				genericMenu.Popup(popupRect, num);
			}
			return this.m_PreVis;
		}

		private void DisplayControls(UnityEngine.Object target, SceneView sceneView)
		{
			if (!sceneView)
			{
				return;
			}
			if (!OcclusionCullingWindow.s_IsVisible)
			{
				return;
			}
			bool flag = this.ShowModePopup(GUILayoutUtility.GetRect(170f, EditorGUIUtility.singleLineHeight));
			if (Event.current.type == EventType.Layout)
			{
				this.m_Warning = string.Empty;
				if (!flag)
				{
					if (StaticOcclusionCullingVisualization.previewOcclucionCamera == null)
					{
						this.m_Warning = "No camera selected for occlusion preview.";
					}
					else if (!StaticOcclusionCullingVisualization.isPreviewOcclusionCullingCameraInPVS)
					{
						this.m_Warning = "Camera is not inside an Occlusion View Area.";
					}
				}
			}
			int num = 12;
			if (!string.IsNullOrEmpty(this.m_Warning))
			{
				Rect rect = GUILayoutUtility.GetRect(100f, (float)(num + 19));
				rect.x += EditorGUI.indent;
				rect.width -= EditorGUI.indent;
				GUI.Label(rect, this.m_Warning, EditorStyles.helpBox);
			}
			else
			{
				Rect rect2 = GUILayoutUtility.GetRect(200f, (float)num);
				rect2.x += EditorGUI.indent;
				rect2.width -= EditorGUI.indent;
				Rect position = new Rect(rect2.x, rect2.y, rect2.width, rect2.height);
				if (flag)
				{
					EditorGUI.DrawLegend(position, Color.white, "View Volumes", StaticOcclusionCullingVisualization.showViewVolumes);
				}
				else
				{
					EditorGUI.DrawLegend(position, Color.white, "Camera Volumes", StaticOcclusionCullingVisualization.showViewVolumes);
				}
				bool flag2 = GUI.Toggle(position, StaticOcclusionCullingVisualization.showViewVolumes, string.Empty, GUIStyle.none);
				if (flag2 != StaticOcclusionCullingVisualization.showViewVolumes)
				{
					StaticOcclusionCullingVisualization.showViewVolumes = flag2;
					SceneView.RepaintAll();
				}
				if (!flag)
				{
					rect2 = GUILayoutUtility.GetRect(100f, (float)num);
					rect2.x += EditorGUI.indent;
					rect2.width -= EditorGUI.indent;
					position = new Rect(rect2.x, rect2.y, rect2.width, rect2.height);
					EditorGUI.DrawLegend(position, Color.green, "Visibility Lines", StaticOcclusionCullingVisualization.showVisibilityLines);
					flag2 = GUI.Toggle(position, StaticOcclusionCullingVisualization.showVisibilityLines, string.Empty, GUIStyle.none);
					if (flag2 != StaticOcclusionCullingVisualization.showVisibilityLines)
					{
						StaticOcclusionCullingVisualization.showVisibilityLines = flag2;
						SceneView.RepaintAll();
					}
					rect2 = GUILayoutUtility.GetRect(100f, (float)num);
					rect2.x += EditorGUI.indent;
					rect2.width -= EditorGUI.indent;
					position = new Rect(rect2.x, rect2.y, rect2.width, rect2.height);
					EditorGUI.DrawLegend(position, Color.grey, "Portals", StaticOcclusionCullingVisualization.showPortals);
					flag2 = GUI.Toggle(position, StaticOcclusionCullingVisualization.showPortals, string.Empty, GUIStyle.none);
					if (flag2 != StaticOcclusionCullingVisualization.showPortals)
					{
						StaticOcclusionCullingVisualization.showPortals = flag2;
						SceneView.RepaintAll();
					}
				}
				if (!flag)
				{
					flag2 = GUILayout.Toggle(StaticOcclusionCullingVisualization.showGeometryCulling, "Occlusion culling", new GUILayoutOption[0]);
					if (flag2 != StaticOcclusionCullingVisualization.showGeometryCulling)
					{
						StaticOcclusionCullingVisualization.showGeometryCulling = flag2;
						SceneView.RepaintAll();
					}
				}
			}
		}
	}
}
