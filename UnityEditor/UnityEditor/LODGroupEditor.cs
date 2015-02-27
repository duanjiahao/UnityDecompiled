using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
namespace UnityEditor
{
	[CustomEditor(typeof(LODGroup))]
	internal class LODGroupEditor : Editor
	{
		private class Styles
		{
			public const int kSceneLabelHalfWidth = 100;
			public const int kSceneLabelHeight = 45;
			public const int kSceneHeaderOffset = 40;
			public const int kSliderBarHeight = 30;
			public const int kRenderersButtonHeight = 60;
			public const int kButtonPadding = 2;
			public const int kDeleteButtonSize = 20;
			public const int kSelectedLODRangePadding = 3;
			public const int kRenderAreaForegroundPadding = 3;
			public readonly GUIStyle m_LODSliderBG = "LODSliderBG";
			public readonly GUIStyle m_LODSliderRange = "LODSliderRange";
			public readonly GUIStyle m_LODSliderRangeSelected = "LODSliderRangeSelected";
			public readonly GUIStyle m_LODSliderText = "LODSliderText";
			public readonly GUIStyle m_LODSliderTextSelected = "LODSliderTextSelected";
			public readonly GUIStyle m_LODStandardButton = "Button";
			public readonly GUIStyle m_LODRendererButton = "LODRendererButton";
			public readonly GUIStyle m_LODRendererAddButton = "LODRendererAddButton";
			public readonly GUIStyle m_LODRendererRemove = "LODRendererRemove";
			public readonly GUIStyle m_LODBlackBox = "LODBlackBox";
			public readonly GUIStyle m_LODCameraLine = "LODCameraLine";
			public readonly GUIStyle m_LODSceneText = "LODSceneText";
			public readonly GUIStyle m_LODRenderersText = "LODRenderersText";
			public readonly GUIStyle m_LODLevelNotifyText = "LODLevelNotifyText";
			public readonly GUIContent m_IconRendererPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add New Renderers");
			public readonly GUIContent m_IconRendererMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove Renderer");
			public readonly GUIContent m_CameraIcon = EditorGUIUtility.IconContent("Camera Icon");
			public readonly GUIContent m_UploadToImporter = new GUIContent("Upload to Importer", "Upload the modified screen percentages to the model importer.");
			public readonly GUIContent m_UploadToImporterDisabled = new GUIContent("Upload to Importer", "Number of LOD's in the scene instance differ from the number of LOD's in the imported model.");
			public readonly GUIContent m_RecalculateBounds = new GUIContent("Bounds", "Recalculate bounds for the current LOD group.");
			public readonly GUIContent m_LightmapScale = new GUIContent("Lightmap Scale", "Set the lightmap scale to match the LOD percentages");
			public readonly GUIContent m_RendersTitle = new GUIContent("Renderers:");
		}
		private class LODInfo
		{
			private float m_ScreenPercentage;
			public readonly int m_LODLevel;
			public Rect m_ButtonPosition;
			public Rect m_RangePosition;
			public float ScreenPercent
			{
				get
				{
					return LODGroupEditor.DelinearizeScreenPercentage(this.m_ScreenPercentage);
				}
				set
				{
					this.m_ScreenPercentage = LODGroupEditor.LinearizeScreenPercentage(value);
				}
			}
			public float RawScreenPercent
			{
				get
				{
					return this.m_ScreenPercentage;
				}
			}
			public LODInfo(int lodLevel, float screenPercentage)
			{
				this.m_LODLevel = lodLevel;
				this.m_ScreenPercentage = screenPercentage;
			}
		}
		private class LODAction
		{
			public delegate void Callback();
			private readonly float m_Percentage;
			private readonly List<LODGroupEditor.LODInfo> m_LODs;
			private readonly Vector2 m_ClickedPosition;
			private readonly SerializedObject m_ObjectRef;
			private readonly LODGroupEditor.LODAction.Callback m_Callback;
			public LODAction(List<LODGroupEditor.LODInfo> loDs, float percentage, Vector2 clickedPosition, SerializedObject objectRef, LODGroupEditor.LODAction.Callback callback)
			{
				this.m_LODs = loDs;
				this.m_Percentage = percentage;
				this.m_ClickedPosition = clickedPosition;
				this.m_ObjectRef = objectRef;
				this.m_Callback = callback;
			}
			public void InsertLOD()
			{
				SerializedProperty serializedProperty = this.m_ObjectRef.FindProperty("m_LODs");
				if (!serializedProperty.isArray)
				{
					return;
				}
				int num = -1;
				foreach (LODGroupEditor.LODInfo current in this.m_LODs)
				{
					if (this.m_Percentage > current.RawScreenPercent)
					{
						num = current.m_LODLevel;
						break;
					}
				}
				if (num < 0)
				{
					serializedProperty.InsertArrayElementAtIndex(this.m_LODs.Count);
					num = this.m_LODs.Count;
				}
				else
				{
					serializedProperty.InsertArrayElementAtIndex(num);
				}
				SerializedProperty serializedProperty2 = this.m_ObjectRef.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", num));
				serializedProperty2.arraySize = 0;
				SerializedProperty arrayElementAtIndex = serializedProperty.GetArrayElementAtIndex(num);
				arrayElementAtIndex.FindPropertyRelative("screenRelativeHeight").floatValue = this.m_Percentage;
				if (this.m_Callback != null)
				{
					this.m_Callback();
				}
				this.m_ObjectRef.ApplyModifiedProperties();
			}
			public void DeleteLOD()
			{
				if (this.m_LODs.Count <= 0)
				{
					return;
				}
				foreach (LODGroupEditor.LODInfo current in this.m_LODs)
				{
					int arraySize = this.m_ObjectRef.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", current.m_LODLevel)).arraySize;
					if (current.m_RangePosition.Contains(this.m_ClickedPosition) && (arraySize == 0 || EditorUtility.DisplayDialog("Delete LOD", "Are you sure you wish to delete this LOD?", "Yes", "No")))
					{
						SerializedProperty serializedProperty = this.m_ObjectRef.FindProperty(string.Format("m_LODs.Array.data[{0}]", current.m_LODLevel));
						serializedProperty.DeleteCommand();
						this.m_ObjectRef.ApplyModifiedProperties();
						if (this.m_Callback != null)
						{
							this.m_Callback();
						}
						break;
					}
				}
			}
		}
		private class LODLightmapScale
		{
			public readonly float m_Scale;
			public readonly List<SerializedProperty> m_Renderers;
			public LODLightmapScale(float scale, List<SerializedProperty> renderers)
			{
				this.m_Scale = scale;
				this.m_Renderers = renderers;
			}
		}
		private const string kLODRootPath = "m_LODs";
		private const string kLODDataPath = "m_LODs.Array.data[{0}]";
		private const string kPixelHeightDataPath = "m_LODs.Array.data[{0}].screenRelativeHeight";
		private const string kRenderRootPath = "m_LODs.Array.data[{0}].renderers";
		private static readonly Color[] kLODColors = new Color[]
		{
			new Color(0.4831376f, 0.6211768f, 0.0219608f, 1f),
			new Color(0.279216f, 0.4078432f, 0.5835296f, 1f),
			new Color(0.2070592f, 0.5333336f, 0.6556864f, 1f),
			new Color(0.5333336f, 0.16f, 0.0282352f, 1f),
			new Color(0.3827448f, 0.2886272f, 0.5239216f, 1f),
			new Color(0.8f, 0.4423528f, 0f, 1f),
			new Color(0.4486272f, 0.4078432f, 0.050196f, 1f),
			new Color(0.7749016f, 0.6368624f, 0.0250984f, 1f)
		};
		private static readonly Color kCulledLODColor = new Color(0.4f, 0f, 0f, 1f);
		private static LODGroupEditor.Styles s_Styles;
		private SerializedObject m_Object;
		private int m_SelectedLODSlider = -1;
		private int m_SelectedLOD = -1;
		private int m_NumberOfLODs;
		private bool m_IsPrefab;
		private Vector3 m_LastCameraPos = Vector3.zero;
		private readonly int m_LODSliderId = "LODSliderIDHash".GetHashCode();
		private readonly int m_CameraSliderId = "LODCameraIDHash".GetHashCode();
		private PreviewRenderUtility m_PreviewUtility;
		private static readonly GUIContent[] kSLightIcons = new GUIContent[2];
		private Vector2 m_PreviewDir = new Vector2(0f, -20f);
		private int activeLOD
		{
			get
			{
				return this.m_SelectedLOD;
			}
		}
		private void OnEnable()
		{
			this.m_Object = new SerializedObject(this.target);
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Combine(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
			PrefabType prefabType = PrefabUtility.GetPrefabType(((LODGroup)this.target).gameObject);
			if (prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab)
			{
				this.m_IsPrefab = true;
			}
			else
			{
				this.m_IsPrefab = false;
			}
			base.Repaint();
		}
		private void OnDisable()
		{
			EditorApplication.update = (EditorApplication.CallbackFunction)Delegate.Remove(EditorApplication.update, new EditorApplication.CallbackFunction(this.Update));
		}
		private static Rect CalculateScreenRect(IEnumerable<Vector3> points)
		{
			List<Vector2> list = (
				from p in points
				select HandleUtility.WorldToGUIPoint(p)).ToList<Vector2>();
			Vector2 vector = new Vector2(3.40282347E+38f, 3.40282347E+38f);
			Vector2 vector2 = new Vector2(-3.40282347E+38f, -3.40282347E+38f);
			foreach (Vector2 current in list)
			{
				vector.x = ((current.x >= vector.x) ? vector.x : current.x);
				vector2.x = ((current.x <= vector2.x) ? vector2.x : current.x);
				vector.y = ((current.y >= vector.y) ? vector.y : current.y);
				vector2.y = ((current.y <= vector2.y) ? vector2.y : current.y);
			}
			return new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
		}
		public void OnSceneGUI()
		{
			if (!Application.HasAdvancedLicense() || Event.current.type != EventType.Repaint || Camera.current == null || SceneView.lastActiveSceneView != SceneView.currentDrawingSceneView || Vector3.Dot(Camera.current.transform.forward, (Camera.current.transform.position - ((LODGroup)this.target).transform.position).normalized) > 0f)
			{
				return;
			}
			if (LODGroupEditor.s_Styles == null)
			{
				LODGroupEditor.s_Styles = new LODGroupEditor.Styles();
			}
			Camera camera = SceneView.lastActiveSceneView.camera;
			LODGroup lODGroup = this.target as LODGroup;
			Vector3 vector = lODGroup.transform.TransformPoint(lODGroup.localReferencePoint);
			LODVisualizationInformation lODVisualizationInformation = LODUtility.CalculateVisualizationData(camera, lODGroup, -1);
			float worldSpaceSize = lODVisualizationInformation.worldSpaceSize;
			Handles.color = ((lODVisualizationInformation.activeLODLevel == -1) ? LODGroupEditor.kCulledLODColor : LODGroupEditor.kLODColors[lODVisualizationInformation.activeLODLevel]);
			Handles.SelectionFrame(0, vector, camera.transform.rotation, worldSpaceSize / 2f);
			Vector3 b = camera.transform.right * worldSpaceSize / 2f;
			Vector3 b2 = camera.transform.up * worldSpaceSize / 2f;
			Rect position = LODGroupEditor.CalculateScreenRect(new Vector3[]
			{
				vector - b + b2,
				vector - b - b2,
				vector + b + b2,
				vector + b - b2
			});
			float num = position.x + position.width / 2f;
			position = new Rect(num - 100f, position.yMax, 200f, 45f);
			if (position.yMax > (float)(Screen.height - 45))
			{
				position.y = (float)(Screen.height - 45 - 40);
			}
			Handles.BeginGUI();
			GUI.Label(position, GUIContent.none, EditorStyles.notificationBackground);
			EditorGUI.DoDropShadowLabel(position, GUIContent.Temp((lODVisualizationInformation.activeLODLevel < 0) ? "Culled" : ("LOD " + lODVisualizationInformation.activeLODLevel)), LODGroupEditor.s_Styles.m_LODLevelNotifyText, 0.3f);
			Handles.EndGUI();
		}
		public void Update()
		{
			if (SceneView.lastActiveSceneView == null || SceneView.lastActiveSceneView.camera == null)
			{
				return;
			}
			if (!Mathf.Approximately(0f, Vector3.Distance(SceneView.lastActiveSceneView.camera.transform.position, this.m_LastCameraPos)))
			{
				this.m_LastCameraPos = SceneView.lastActiveSceneView.camera.transform.position;
				base.Repaint();
			}
		}
		private static float DelinearizeScreenPercentage(float percentage)
		{
			if (Mathf.Approximately(0f, percentage))
			{
				return 0f;
			}
			return Mathf.Sqrt(percentage);
		}
		private static float LinearizeScreenPercentage(float percentage)
		{
			return percentage * percentage;
		}
		private ModelImporter GetImporter()
		{
			return AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(this.target))) as ModelImporter;
		}
		public override void OnInspectorGUI()
		{
			bool enabled = GUI.enabled;
			if (!Application.HasAdvancedLicense())
			{
				EditorGUILayout.HelpBox("LOD only available in Unity Pro", MessageType.Warning);
				GUI.enabled = false;
			}
			if (LODGroupEditor.s_Styles == null)
			{
				LODGroupEditor.s_Styles = new LODGroupEditor.Styles();
			}
			this.m_Object.Update();
			this.m_NumberOfLODs = this.m_Object.FindProperty("m_LODs").arraySize;
			if (this.m_NumberOfLODs > 0 && this.activeLOD >= 0)
			{
				SerializedProperty serializedProperty = this.m_Object.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", this.activeLOD));
				for (int i = serializedProperty.arraySize - 1; i >= 0; i--)
				{
					SerializedProperty serializedProperty2 = serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("renderer");
					Renderer x = serializedProperty2.objectReferenceValue as Renderer;
					if (x == null)
					{
						serializedProperty.DeleteArrayElementAtIndex(i);
					}
				}
			}
			GUILayout.Space(17f);
			Rect rect = GUILayoutUtility.GetRect(0f, 30f, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			List<LODGroupEditor.LODInfo> list = new List<LODGroupEditor.LODInfo>();
			float num = -1f;
			for (int j = 0; j < this.m_NumberOfLODs; j++)
			{
				SerializedProperty serializedProperty3 = this.m_Object.FindProperty(string.Format("m_LODs.Array.data[{0}].screenRelativeHeight", j));
				LODGroupEditor.LODInfo lODInfo = new LODGroupEditor.LODInfo(j, serializedProperty3.floatValue);
				lODInfo.m_ButtonPosition = LODGroupEditor.CalcLODButton(rect, lODInfo.ScreenPercent);
				float startPercent = (j - 1 >= 0) ? num : 1f;
				lODInfo.m_RangePosition = LODGroupEditor.CalcLODRange(rect, startPercent, lODInfo.ScreenPercent);
				num = lODInfo.ScreenPercent;
				list.Add(lODInfo);
			}
			GUILayout.Space(8f);
			this.DrawLODLevelSlider(rect, list);
			GUILayout.Space(8f);
			GUILayout.Label(string.Format("LODBias of {0:0.00} active", QualitySettings.lodBias), EditorStyles.boldLabel, new GUILayoutOption[0]);
			if (this.m_NumberOfLODs > 0 && this.activeLOD >= 0 && this.activeLOD < this.m_NumberOfLODs)
			{
				this.DrawRenderersInfo(Screen.width / 60);
			}
			GUILayout.Space(8f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Recalculate:", EditorStyles.boldLabel, new GUILayoutOption[0]);
			if (GUILayout.Button(LODGroupEditor.s_Styles.m_RecalculateBounds, new GUILayoutOption[0]))
			{
				LODUtility.CalculateLODGroupBoundingBox(this.target as LODGroup);
			}
			if (GUILayout.Button(LODGroupEditor.s_Styles.m_LightmapScale, new GUILayoutOption[0]))
			{
				this.SendPercentagesToLightmapScale();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			bool flag = PrefabUtility.GetPrefabType(this.target) == PrefabType.ModelPrefabInstance;
			if (flag)
			{
				ModelImporter importer = this.GetImporter();
				SerializedObject serializedObject = new SerializedObject(importer);
				SerializedProperty serializedProperty4 = serializedObject.FindProperty("m_LODScreenPercentages");
				bool flag2 = serializedProperty4.isArray && serializedProperty4.arraySize == list.Count;
				bool enabled2 = GUI.enabled;
				if (!flag2)
				{
					GUI.enabled = false;
				}
				if (importer != null && GUILayout.Button((!flag2) ? LODGroupEditor.s_Styles.m_UploadToImporterDisabled : LODGroupEditor.s_Styles.m_UploadToImporter, new GUILayoutOption[0]))
				{
					for (int k = 0; k < serializedProperty4.arraySize; k++)
					{
						serializedProperty4.GetArrayElementAtIndex(k).floatValue = list[k].RawScreenPercent;
					}
					serializedObject.ApplyModifiedProperties();
					AssetDatabase.ImportAsset(importer.assetPath);
				}
				GUI.enabled = enabled2;
			}
			this.m_Object.ApplyModifiedProperties();
			GUI.enabled = enabled;
		}
		private void DrawRenderersInfo(int horizontalNumber)
		{
			Rect rect = GUILayoutUtility.GetRect(LODGroupEditor.s_Styles.m_RendersTitle, LODGroupEditor.s_Styles.m_LODSliderTextSelected);
			if (Event.current.type == EventType.Repaint)
			{
				EditorStyles.label.Draw(rect, LODGroupEditor.s_Styles.m_RendersTitle, false, false, false, false);
			}
			SerializedProperty serializedProperty = this.m_Object.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", this.activeLOD));
			int num = serializedProperty.arraySize + 1;
			int num2 = Mathf.CeilToInt((float)num / (float)horizontalNumber);
			Rect rect2 = GUILayoutUtility.GetRect(0f, (float)(num2 * 60), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			Rect rect3 = rect2;
			GUI.Box(rect2, GUIContent.none);
			rect3.width -= 6f;
			rect3.x += 3f;
			float num3 = rect3.width / (float)horizontalNumber;
			List<Rect> list = new List<Rect>();
			for (int i = 0; i < num2; i++)
			{
				int num4 = 0;
				while (num4 < horizontalNumber && i * horizontalNumber + num4 < serializedProperty.arraySize)
				{
					Rect rect4 = new Rect(2f + rect3.x + (float)num4 * num3, 2f + rect3.y + (float)(i * 60), num3 - 4f, 56f);
					list.Add(rect4);
					this.DrawRendererButton(rect4, i * horizontalNumber + num4);
					num4++;
				}
			}
			if (this.m_IsPrefab)
			{
				return;
			}
			int num5 = (num - 1) % horizontalNumber;
			int num6 = num2 - 1;
			this.HandleAddRenderer(new Rect(2f + rect3.x + (float)num5 * num3, 2f + rect3.y + (float)(num6 * 60), num3 - 4f, 56f), list, rect2);
		}
		private void HandleAddRenderer(Rect position, IEnumerable<Rect> alreadyDrawn, Rect drawArea)
		{
			Event evt = Event.current;
			EventType type = evt.type;
			switch (type)
			{
			case EventType.Repaint:
				LODGroupEditor.s_Styles.m_LODStandardButton.Draw(position, GUIContent.none, false, false, false, false);
				LODGroupEditor.s_Styles.m_LODRendererAddButton.Draw(new Rect(position.x - 2f, position.y, position.width, position.height), "Add", false, false, false, false);
				return;
			case EventType.Layout:
			case EventType.Ignore:
			case EventType.Used:
			case EventType.ValidateCommand:
				IL_4A:
				if (type != EventType.MouseDown)
				{
					return;
				}
				if (position.Contains(evt.mousePosition))
				{
					evt.Use();
					int hashCode = "LODGroupSelector".GetHashCode();
					ObjectSelector.get.Show(null, typeof(Renderer), null, true);
					ObjectSelector.get.objectSelectorID = hashCode;
					GUIUtility.ExitGUI();
				}
				return;
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				bool flag = false;
				if (drawArea.Contains(evt.mousePosition) && alreadyDrawn.All((Rect x) => !x.Contains(evt.mousePosition)))
				{
					flag = true;
				}
				if (!flag)
				{
					return;
				}
				if (DragAndDrop.objectReferences.Count<UnityEngine.Object>() > 0)
				{
					DragAndDrop.visualMode = ((!this.m_IsPrefab) ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.None);
					if (evt.type == EventType.DragPerform)
					{
						IEnumerable<GameObject> selectedGameObjects = 
							from go in DragAndDrop.objectReferences
							where go as GameObject != null
							select go as GameObject;
						IEnumerable<Renderer> renderers = this.GetRenderers(selectedGameObjects, true);
						this.AddGameObjectRenderers(renderers, true);
						DragAndDrop.AcceptDrag();
						evt.Use();
						return;
					}
				}
				evt.Use();
				return;
			}
			case EventType.ExecuteCommand:
			{
				string commandName = evt.commandName;
				if (commandName == "ObjectSelectorClosed" && ObjectSelector.get.objectSelectorID == "LODGroupSelector".GetHashCode())
				{
					this.AddGameObjectRenderers(this.GetRenderers(new List<GameObject>
					{
						ObjectSelector.GetCurrentObject() as GameObject
					}, true), true);
					evt.Use();
					GUIUtility.ExitGUI();
				}
				return;
			}
			}
			goto IL_4A;
		}
		private void DrawRendererButton(Rect position, int rendererIndex)
		{
			SerializedProperty serializedProperty = this.m_Object.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", this.activeLOD));
			SerializedProperty serializedProperty2 = serializedProperty.GetArrayElementAtIndex(rendererIndex).FindPropertyRelative("renderer");
			Renderer renderer = serializedProperty2.objectReferenceValue as Renderer;
			Rect position2 = new Rect(position.xMax - 20f, position.yMax - 20f, 20f, 20f);
			Event current = Event.current;
			EventType type = current.type;
			if (type != EventType.MouseDown)
			{
				if (type == EventType.Repaint)
				{
					if (renderer != null)
					{
						MeshFilter component = renderer.GetComponent<MeshFilter>();
						GUIContent content;
						if (component != null && component.sharedMesh != null)
						{
							content = new GUIContent(AssetPreview.GetAssetPreview(component.sharedMesh), renderer.gameObject.name);
						}
						else
						{
							if (renderer is SkinnedMeshRenderer)
							{
								content = new GUIContent(AssetPreview.GetAssetPreview((renderer as SkinnedMeshRenderer).sharedMesh), renderer.gameObject.name);
							}
							else
							{
								content = new GUIContent(ObjectNames.NicifyVariableName(renderer.GetType().Name), renderer.gameObject.name);
							}
						}
						LODGroupEditor.s_Styles.m_LODBlackBox.Draw(position, GUIContent.none, false, false, false, false);
						GUIStyle gUIStyle = "LODRendererButton";
						gUIStyle.Draw(new Rect(position.x + 2f, position.y + 2f, position.width - 4f, position.height - 4f), content, false, false, false, false);
					}
					else
					{
						LODGroupEditor.s_Styles.m_LODBlackBox.Draw(position, GUIContent.none, false, false, false, false);
						LODGroupEditor.s_Styles.m_LODRendererButton.Draw(position, "<Empty>", false, false, false, false);
					}
					if (!this.m_IsPrefab)
					{
						LODGroupEditor.s_Styles.m_LODBlackBox.Draw(position2, GUIContent.none, false, false, false, false);
						LODGroupEditor.s_Styles.m_LODRendererRemove.Draw(position2, LODGroupEditor.s_Styles.m_IconRendererMinus, false, false, false, false);
					}
				}
			}
			else
			{
				if (!this.m_IsPrefab && position2.Contains(current.mousePosition))
				{
					serializedProperty.DeleteArrayElementAtIndex(rendererIndex);
					current.Use();
					this.m_Object.ApplyModifiedProperties();
					LODUtility.CalculateLODGroupBoundingBox(this.target as LODGroup);
				}
				else
				{
					if (position.Contains(current.mousePosition))
					{
						EditorGUIUtility.PingObject(renderer);
						current.Use();
					}
				}
			}
		}
		private IEnumerable<Renderer> GetRenderers(IEnumerable<GameObject> selectedGameObjects, bool searchChildren)
		{
			LODGroup lodGroup = this.target as LODGroup;
			if (lodGroup == null || EditorUtility.IsPersistent(lodGroup))
			{
				return new List<Renderer>();
			}
			IEnumerable<GameObject> enumerable = 
				from go in selectedGameObjects
				where go.transform.IsChildOf(lodGroup.transform)
				select go;
			IEnumerable<GameObject> enumerable2 = 
				from go in selectedGameObjects
				where !go.transform.IsChildOf(lodGroup.transform)
				select go;
			List<GameObject> list = new List<GameObject>();
			if (enumerable2.Count<GameObject>() > 0 && EditorUtility.DisplayDialog("Reparent GameObjects", "Some objects are not children of the LODGroup GameObject. Do you want to reparent them and add them to the LODGroup?", "Yes, Reparent", "No, Use Only Existing Children"))
			{
				foreach (GameObject current in enumerable2)
				{
					if (EditorUtility.IsPersistent(current))
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(current) as GameObject;
						if (gameObject != null)
						{
							gameObject.transform.parent = lodGroup.transform;
							gameObject.transform.localPosition = Vector3.zero;
							gameObject.transform.localRotation = Quaternion.identity;
							list.Add(gameObject);
						}
					}
					else
					{
						current.transform.parent = lodGroup.transform;
						list.Add(current);
					}
				}
				enumerable = enumerable.Union(list);
			}
			List<Renderer> list2 = new List<Renderer>();
			foreach (GameObject current2 in enumerable)
			{
				if (searchChildren)
				{
					list2.AddRange(current2.GetComponentsInChildren<Renderer>());
				}
				else
				{
					list2.Add(current2.GetComponent<Renderer>());
				}
			}
			IEnumerable<Renderer> collection = 
				from go in DragAndDrop.objectReferences
				where go as Renderer != null
				select go as Renderer;
			list2.AddRange(collection);
			return list2;
		}
		private void AddGameObjectRenderers(IEnumerable<Renderer> toAdd, bool add)
		{
			SerializedProperty serializedProperty = this.m_Object.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", this.activeLOD));
			if (!add)
			{
				serializedProperty.ClearArray();
			}
			List<Renderer> list = new List<Renderer>();
			for (int i = 0; i < serializedProperty.arraySize; i++)
			{
				SerializedProperty serializedProperty2 = serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("renderer");
				Renderer renderer = serializedProperty2.objectReferenceValue as Renderer;
				if (!(renderer == null))
				{
					list.Add(renderer);
				}
			}
			foreach (Renderer current in toAdd)
			{
				if (!list.Contains(current))
				{
					serializedProperty.arraySize++;
					serializedProperty.GetArrayElementAtIndex(serializedProperty.arraySize - 1).FindPropertyRelative("renderer").objectReferenceValue = current;
					list.Add(current);
				}
			}
			this.m_Object.ApplyModifiedProperties();
			LODUtility.CalculateLODGroupBoundingBox(this.target as LODGroup);
		}
		private void DeletedLOD()
		{
			this.m_SelectedLOD--;
		}
		private static void UpdateCamera(float desiredPercentage, LODGroup group)
		{
			Vector3 position = group.transform.TransformPoint(group.localReferencePoint);
			float num = LODUtility.CalculateDistance(SceneView.lastActiveSceneView.camera, (desiredPercentage > 0f) ? desiredPercentage : 1E-06f, group);
			if (SceneView.lastActiveSceneView.camera.orthographic)
			{
				num = Mathf.Sqrt(num * num * (1f + SceneView.lastActiveSceneView.camera.aspect));
			}
			SceneView.lastActiveSceneView.LookAtDirect(position, SceneView.lastActiveSceneView.camera.transform.rotation, num);
		}
		private void UpdateSelectedLODFromCamera(IEnumerable<LODGroupEditor.LODInfo> lods, float cameraPercent)
		{
			foreach (LODGroupEditor.LODInfo current in lods)
			{
				if (cameraPercent > current.RawScreenPercent)
				{
					this.m_SelectedLOD = current.m_LODLevel;
					break;
				}
			}
		}
		private static float GetCameraPercentForCurrentQualityLevel(float clickPosition, float sliderStart, float sliderWidth)
		{
			float percentage = Mathf.Clamp(1f - (clickPosition - sliderStart) / sliderWidth, 0.01f, 1f);
			return LODGroupEditor.LinearizeScreenPercentage(percentage);
		}
		private void DrawLODLevelSlider(Rect sliderPosition, List<LODGroupEditor.LODInfo> lods)
		{
			int controlID = GUIUtility.GetControlID(this.m_LODSliderId, FocusType.Passive);
			int controlID2 = GUIUtility.GetControlID(this.m_CameraSliderId, FocusType.Passive);
			Event current = Event.current;
			LODGroup lODGroup = this.target as LODGroup;
			if (lODGroup == null)
			{
				return;
			}
			EventType typeForControl = current.GetTypeForControl(controlID);
			switch (typeForControl)
			{
			case EventType.MouseDown:
			{
				if (current.button == 1 && sliderPosition.Contains(current.mousePosition))
				{
					float num = LODGroupEditor.CalculatePercentageFromBar(sliderPosition, current.mousePosition);
					GenericMenu genericMenu = new GenericMenu();
					if (lods.Count >= 8)
					{
						genericMenu.AddDisabledItem(EditorGUIUtility.TextContent("Insert Before"));
					}
					else
					{
						genericMenu.AddItem(EditorGUIUtility.TextContent("Insert Before"), false, new GenericMenu.MenuFunction(new LODGroupEditor.LODAction(lods, LODGroupEditor.LinearizeScreenPercentage(num), current.mousePosition, this.m_Object, null).InsertLOD));
					}
					bool flag = true;
					if (lods.Count > 0 && lods[lods.Count - 1].ScreenPercent < num)
					{
						flag = false;
					}
					if (flag)
					{
						genericMenu.AddDisabledItem(EditorGUIUtility.TextContent("Delete"));
					}
					else
					{
						genericMenu.AddItem(EditorGUIUtility.TextContent("Delete"), false, new GenericMenu.MenuFunction(new LODGroupEditor.LODAction(lods, LODGroupEditor.LinearizeScreenPercentage(num), current.mousePosition, this.m_Object, new LODGroupEditor.LODAction.Callback(this, ldftn(DeletedLOD))).DeleteLOD));
					}
					genericMenu.ShowAsContext();
					bool flag2 = false;
					foreach (LODGroupEditor.LODInfo current2 in lods)
					{
						if (current2.m_RangePosition.Contains(current.mousePosition))
						{
							this.m_SelectedLOD = current2.m_LODLevel;
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						this.m_SelectedLOD = -1;
					}
					current.Use();
					goto IL_851;
				}
				Rect rect = sliderPosition;
				rect.x -= 5f;
				rect.width += 10f;
				if (rect.Contains(current.mousePosition))
				{
					current.Use();
					GUIUtility.hotControl = controlID;
					bool flag3 = false;
					IOrderedEnumerable<LODGroupEditor.LODInfo> collection = 
						from lod in lods
						where lod.ScreenPercent > 0.5f
						select lod into x
						orderby x.m_LODLevel descending
						select x;
					IOrderedEnumerable<LODGroupEditor.LODInfo> collection2 = 
						from lod in lods
						where lod.ScreenPercent <= 0.5f
						select lod into x
						orderby x.m_LODLevel
						select x;
					List<LODGroupEditor.LODInfo> list = new List<LODGroupEditor.LODInfo>();
					list.AddRange(collection);
					list.AddRange(collection2);
					foreach (LODGroupEditor.LODInfo current3 in list)
					{
						if (current3.m_ButtonPosition.Contains(current.mousePosition))
						{
							this.m_SelectedLODSlider = current3.m_LODLevel;
							flag3 = true;
							if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null && !this.m_IsPrefab)
							{
								LODGroupEditor.UpdateCamera(current3.RawScreenPercent + 0.001f, lODGroup);
								SceneView.lastActiveSceneView.ClearSearchFilter();
								SceneView.lastActiveSceneView.SetSceneViewFiltering(true);
								HierarchyProperty.FilterSingleSceneObject(lODGroup.gameObject.GetInstanceID(), false);
								SceneView.RepaintAll();
							}
							break;
						}
					}
					if (!flag3)
					{
						foreach (LODGroupEditor.LODInfo current4 in lods)
						{
							if (current4.m_RangePosition.Contains(current.mousePosition))
							{
								this.m_SelectedLOD = current4.m_LODLevel;
								break;
							}
						}
					}
				}
				goto IL_851;
			}
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlID)
				{
					GUIUtility.hotControl = 0;
					this.m_SelectedLODSlider = -1;
					if (SceneView.lastActiveSceneView != null)
					{
						SceneView.lastActiveSceneView.SetSceneViewFiltering(false);
						SceneView.lastActiveSceneView.ClearSearchFilter();
					}
					current.Use();
				}
				goto IL_851;
			case EventType.MouseMove:
			case EventType.KeyDown:
			case EventType.KeyUp:
			case EventType.ScrollWheel:
			case EventType.Layout:
				IL_75:
				if (typeForControl != EventType.DragExited)
				{
					goto IL_851;
				}
				current.Use();
				goto IL_851;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID && this.m_SelectedLODSlider >= 0 && lods[this.m_SelectedLODSlider] != null)
				{
					current.Use();
					float num2 = Mathf.Clamp01(1f - (current.mousePosition.x - sliderPosition.x) / sliderPosition.width);
					this.SetSelectedLODLevelPercentage(num2 - 0.001f, lods);
					if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null && !this.m_IsPrefab)
					{
						LODGroupEditor.UpdateCamera(LODGroupEditor.LinearizeScreenPercentage(num2), lODGroup);
						SceneView.RepaintAll();
					}
				}
				goto IL_851;
			case EventType.Repaint:
			{
				Rect rect2 = sliderPosition;
				rect2.width += 2f;
				rect2.height += 2f;
				rect2.center -= new Vector2(1f, 1f);
				LODGroupEditor.s_Styles.m_LODSliderBG.Draw(sliderPosition, GUIContent.none, false, false, false, false);
				for (int i = 0; i < lods.Count; i++)
				{
					LODGroupEditor.LODInfo currentLOD = lods[i];
					this.DrawLODRange(currentLOD, (i != 0) ? lods[i - 1].RawScreenPercent : 1f);
					LODGroupEditor.DrawLODButton(currentLOD);
				}
				LODGroupEditor.DrawCulledRange(sliderPosition, (lods.Count <= 0) ? 1f : lods[lods.Count - 1].RawScreenPercent);
				goto IL_851;
			}
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				int num3 = -2;
				foreach (LODGroupEditor.LODInfo current5 in lods)
				{
					if (current5.m_RangePosition.Contains(current.mousePosition))
					{
						num3 = current5.m_LODLevel;
						break;
					}
				}
				if (num3 == -2 && LODGroupEditor.GetCulledBox(sliderPosition, (lods.Count <= 0) ? 1f : lods[lods.Count - 1].ScreenPercent).Contains(current.mousePosition))
				{
					num3 = -1;
				}
				if (num3 >= -1)
				{
					this.m_SelectedLOD = num3;
					if (DragAndDrop.objectReferences.Count<UnityEngine.Object>() > 0)
					{
						DragAndDrop.visualMode = ((!this.m_IsPrefab) ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.None);
						if (current.type == EventType.DragPerform)
						{
							IEnumerable<GameObject> selectedGameObjects = 
								from go in DragAndDrop.objectReferences
								where go as GameObject != null
								select go as GameObject;
							IEnumerable<Renderer> renderers = this.GetRenderers(selectedGameObjects, true);
							if (num3 == -1)
							{
								SerializedProperty serializedProperty = this.m_Object.FindProperty("m_LODs");
								serializedProperty.arraySize++;
								SerializedProperty serializedProperty2 = this.m_Object.FindProperty(string.Format("m_LODs.Array.data[{0}].screenRelativeHeight", lods.Count));
								if (lods.Count == 0)
								{
									serializedProperty2.floatValue = 0.5f;
								}
								else
								{
									SerializedProperty serializedProperty3 = this.m_Object.FindProperty(string.Format("m_LODs.Array.data[{0}].screenRelativeHeight", lods.Count - 1));
									serializedProperty2.floatValue = serializedProperty3.floatValue / 2f;
								}
								this.m_SelectedLOD = lods.Count;
								this.AddGameObjectRenderers(renderers, false);
							}
							else
							{
								this.AddGameObjectRenderers(renderers, true);
							}
							DragAndDrop.AcceptDrag();
						}
					}
					current.Use();
					goto IL_851;
				}
				goto IL_851;
			}
			}
			goto IL_75;
			IL_851:
			if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null && !this.m_IsPrefab)
			{
				Camera camera = SceneView.lastActiveSceneView.camera;
				float num4 = LODUtility.CalculateVisualizationData(camera, lODGroup, -1).activeRelativeScreenSize / QualitySettings.lodBias;
				float value = LODGroupEditor.DelinearizeScreenPercentage(num4);
				Vector3 normalized = (SceneView.lastActiveSceneView.camera.transform.position - ((LODGroup)this.target).transform.position).normalized;
				if (Vector3.Dot(camera.transform.forward, normalized) > 0f)
				{
					value = 1f;
				}
				Rect rect3 = LODGroupEditor.CalcLODButton(sliderPosition, Mathf.Clamp01(value));
				Rect position = new Rect(rect3.center.x - 15f, rect3.y - 25f, 32f, 32f);
				Rect position2 = new Rect(rect3.center.x - 1f, rect3.y, 2f, rect3.height);
				Rect position3 = new Rect(position.center.x - 5f, position2.yMax, 35f, 20f);
				switch (current.GetTypeForControl(controlID2))
				{
				case EventType.MouseDown:
					if (position.Contains(current.mousePosition))
					{
						current.Use();
						float cameraPercentForCurrentQualityLevel = LODGroupEditor.GetCameraPercentForCurrentQualityLevel(current.mousePosition.x, sliderPosition.x, sliderPosition.width);
						LODGroupEditor.UpdateCamera(cameraPercentForCurrentQualityLevel, lODGroup);
						this.UpdateSelectedLODFromCamera(lods, cameraPercentForCurrentQualityLevel);
						GUIUtility.hotControl = controlID2;
						SceneView.lastActiveSceneView.ClearSearchFilter();
						SceneView.lastActiveSceneView.SetSceneViewFiltering(true);
						HierarchyProperty.FilterSingleSceneObject(lODGroup.gameObject.GetInstanceID(), false);
						SceneView.RepaintAll();
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == controlID2)
					{
						SceneView.lastActiveSceneView.SetSceneViewFiltering(false);
						SceneView.lastActiveSceneView.ClearSearchFilter();
						GUIUtility.hotControl = 0;
						current.Use();
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlID2)
					{
						current.Use();
						float cameraPercentForCurrentQualityLevel2 = LODGroupEditor.GetCameraPercentForCurrentQualityLevel(current.mousePosition.x, sliderPosition.x, sliderPosition.width);
						this.UpdateSelectedLODFromCamera(lods, cameraPercentForCurrentQualityLevel2);
						LODGroupEditor.UpdateCamera(cameraPercentForCurrentQualityLevel2, lODGroup);
						SceneView.RepaintAll();
					}
					break;
				case EventType.Repaint:
				{
					Color backgroundColor = GUI.backgroundColor;
					GUI.backgroundColor = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0.8f);
					LODGroupEditor.s_Styles.m_LODCameraLine.Draw(position2, false, false, false, false);
					GUI.backgroundColor = backgroundColor;
					GUI.Label(position, LODGroupEditor.s_Styles.m_CameraIcon, GUIStyle.none);
					LODGroupEditor.s_Styles.m_LODSliderText.Draw(position3, string.Format("{0:0}%", Mathf.Clamp01(num4) * 100f), false, false, false, false);
					break;
				}
				}
			}
		}
		private void SetSelectedLODLevelPercentage(float newScreenPercentage, List<LODGroupEditor.LODInfo> lods)
		{
			IEnumerable<LODGroupEditor.LODInfo> source = 
				from lod in lods
				where lod.m_LODLevel == lods[this.m_SelectedLODSlider].m_LODLevel + 1
				select lod;
			float num = 0f;
			if (source.FirstOrDefault<LODGroupEditor.LODInfo>() != null)
			{
				num = source.FirstOrDefault<LODGroupEditor.LODInfo>().ScreenPercent;
			}
			IEnumerable<LODGroupEditor.LODInfo> source2 = 
				from lod in lods
				where lod.m_LODLevel == lods[this.m_SelectedLODSlider].m_LODLevel - 1
				select lod;
			float num2 = 1f;
			if (source2.FirstOrDefault<LODGroupEditor.LODInfo>() != null)
			{
				num2 = source2.FirstOrDefault<LODGroupEditor.LODInfo>().ScreenPercent;
			}
			num2 = Mathf.Clamp01(num2);
			num = Mathf.Clamp01(num);
			lods[this.m_SelectedLODSlider].ScreenPercent = Mathf.Clamp(newScreenPercentage, num, num2);
			SerializedProperty serializedProperty = this.m_Object.FindProperty(string.Format("m_LODs.Array.data[{0}].screenRelativeHeight", lods[this.m_SelectedLODSlider].m_LODLevel));
			serializedProperty.floatValue = lods[this.m_SelectedLODSlider].RawScreenPercent;
		}
		private static void DrawLODButton(LODGroupEditor.LODInfo currentLOD)
		{
			EditorGUIUtility.AddCursorRect(currentLOD.m_ButtonPosition, MouseCursor.ResizeHorizontal);
		}
		private void DrawLODRange(LODGroupEditor.LODInfo currentLOD, float previousLODPercentage)
		{
			Color backgroundColor = GUI.backgroundColor;
			string text = string.Format("LOD: {0}\n{1:0}%", currentLOD.m_LODLevel, previousLODPercentage * 100f);
			if (currentLOD.m_LODLevel == this.activeLOD)
			{
				Rect rangePosition = currentLOD.m_RangePosition;
				rangePosition.width -= 6f;
				rangePosition.height -= 6f;
				rangePosition.center += new Vector2(3f, 3f);
				LODGroupEditor.s_Styles.m_LODSliderRangeSelected.Draw(currentLOD.m_RangePosition, GUIContent.none, false, false, false, false);
				GUI.backgroundColor = LODGroupEditor.kLODColors[currentLOD.m_LODLevel];
				if (rangePosition.width > 0f)
				{
					LODGroupEditor.s_Styles.m_LODSliderRange.Draw(rangePosition, GUIContent.none, false, false, false, false);
				}
				LODGroupEditor.s_Styles.m_LODSliderText.Draw(currentLOD.m_RangePosition, text, false, false, false, false);
			}
			else
			{
				GUI.backgroundColor = LODGroupEditor.kLODColors[currentLOD.m_LODLevel];
				GUI.backgroundColor *= 0.6f;
				LODGroupEditor.s_Styles.m_LODSliderRange.Draw(currentLOD.m_RangePosition, GUIContent.none, false, false, false, false);
				LODGroupEditor.s_Styles.m_LODSliderText.Draw(currentLOD.m_RangePosition, text, false, false, false, false);
			}
			GUI.backgroundColor = backgroundColor;
		}
		private static Rect GetCulledBox(Rect totalRect, float previousLODPercentage)
		{
			Rect result = LODGroupEditor.CalcLODRange(totalRect, previousLODPercentage, 0f);
			result.height -= 2f;
			result.width -= 1f;
			result.center += new Vector2(0f, 1f);
			return result;
		}
		private static void DrawCulledRange(Rect totalRect, float previousLODPercentage)
		{
			if (Mathf.Approximately(previousLODPercentage, 0f))
			{
				return;
			}
			Rect culledBox = LODGroupEditor.GetCulledBox(totalRect, LODGroupEditor.DelinearizeScreenPercentage(previousLODPercentage));
			Color color = GUI.color;
			GUI.color = LODGroupEditor.kCulledLODColor;
			LODGroupEditor.s_Styles.m_LODSliderRange.Draw(culledBox, GUIContent.none, false, false, false, false);
			GUI.color = color;
			string text = string.Format("Culled\n{0:0}%", previousLODPercentage * 100f);
			LODGroupEditor.s_Styles.m_LODSliderText.Draw(culledBox, text, false, false, false, false);
		}
		private static float CalculatePercentageFromBar(Rect totalRect, Vector2 clickPosition)
		{
			clickPosition.x -= totalRect.x;
			totalRect.x = 0f;
			return (totalRect.width <= 0f) ? 0f : (1f - clickPosition.x / totalRect.width);
		}
		private static Rect CalcLODButton(Rect totalRect, float percentage)
		{
			return new Rect(totalRect.x + Mathf.Round(totalRect.width * (1f - percentage)) - 5f, totalRect.y, 10f, totalRect.height);
		}
		private static Rect CalcLODRange(Rect totalRect, float startPercent, float endPercent)
		{
			float num = Mathf.Round(totalRect.width * (1f - startPercent));
			float num2 = Mathf.Round(totalRect.width * (1f - endPercent));
			return new Rect(totalRect.x + num, totalRect.y, num2 - num, totalRect.height);
		}
		private void SendPercentagesToLightmapScale()
		{
			List<LODGroupEditor.LODLightmapScale> list = new List<LODGroupEditor.LODLightmapScale>();
			for (int i = 0; i < this.m_NumberOfLODs; i++)
			{
				SerializedProperty serializedProperty = this.m_Object.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", i));
				List<SerializedProperty> list2 = new List<SerializedProperty>();
				for (int j = 0; j < serializedProperty.arraySize; j++)
				{
					SerializedProperty serializedProperty2 = serializedProperty.GetArrayElementAtIndex(j).FindPropertyRelative("renderer");
					if (serializedProperty2 != null)
					{
						list2.Add(serializedProperty2);
					}
				}
				float scale = (i != 0) ? this.m_Object.FindProperty(string.Format("m_LODs.Array.data[{0}].screenRelativeHeight", i - 1)).floatValue : 1f;
				list.Add(new LODGroupEditor.LODLightmapScale(scale, list2));
			}
			for (int k = 0; k < this.m_NumberOfLODs; k++)
			{
				LODGroupEditor.SetLODLightmapScale(list[k]);
			}
		}
		private static void SetLODLightmapScale(LODGroupEditor.LODLightmapScale lodRenderer)
		{
			foreach (SerializedProperty current in lodRenderer.m_Renderers)
			{
				SerializedObject serializedObject = new SerializedObject(current.objectReferenceValue);
				SerializedProperty serializedProperty = serializedObject.FindProperty("m_ScaleInLightmap");
				serializedProperty.floatValue = Mathf.Max(0f, lodRenderer.m_Scale * (1f / LightmapVisualization.GetLightmapLODLevelScale((Renderer)current.objectReferenceValue)));
				serializedObject.ApplyModifiedProperties();
			}
		}
		public override bool HasPreviewGUI()
		{
			return this.target != null;
		}
		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (!ShaderUtil.hardwareSupportsRectRenderTexture)
			{
				if (Event.current.type == EventType.Repaint)
				{
					EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "LOD preview \nnot available");
				}
				return;
			}
			this.InitPreview();
			this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, r);
			this.m_PreviewDir.y = Mathf.Clamp(this.m_PreviewDir.y, -89f, 89f);
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}
			this.m_PreviewUtility.BeginPreview(r, background);
			this.DoRenderPreview();
			Texture image = this.m_PreviewUtility.EndPreview();
			GUI.DrawTexture(r, image, ScaleMode.StretchToFill, false);
		}
		private void InitPreview()
		{
			if (this.m_PreviewUtility == null)
			{
				this.m_PreviewUtility = new PreviewRenderUtility();
			}
			if (LODGroupEditor.kSLightIcons[0] == null)
			{
				LODGroupEditor.kSLightIcons[0] = EditorGUIUtility.IconContent("PreMatLight0");
				LODGroupEditor.kSLightIcons[1] = EditorGUIUtility.IconContent("PreMatLight1");
			}
		}
		protected void DoRenderPreview()
		{
			if (this.m_PreviewUtility.m_RenderTexture.width <= 0 || this.m_PreviewUtility.m_RenderTexture.height <= 0 || this.m_NumberOfLODs <= 0 || this.activeLOD < 0)
			{
				return;
			}
			Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
			bool flag = false;
			List<MeshFilter> list = new List<MeshFilter>();
			SerializedProperty serializedProperty = this.m_Object.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", this.activeLOD));
			for (int i = 0; i < serializedProperty.arraySize; i++)
			{
				SerializedProperty serializedProperty2 = serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("renderer");
				Renderer renderer = serializedProperty2.objectReferenceValue as Renderer;
				if (!(renderer == null))
				{
					MeshFilter component = renderer.GetComponent<MeshFilter>();
					if (component != null && component.sharedMesh != null && component.sharedMesh.subMeshCount > 0)
					{
						list.Add(component);
					}
					if (!flag)
					{
						bounds = renderer.bounds;
						flag = true;
					}
					else
					{
						bounds.Encapsulate(renderer.bounds);
					}
				}
			}
			if (!flag)
			{
				return;
			}
			float magnitude = bounds.extents.magnitude;
			float d = magnitude * 10f;
			Vector2 vector = -(this.m_PreviewDir / 100f);
			this.m_PreviewUtility.m_Camera.transform.position = bounds.center + new Vector3(Mathf.Sin(vector.x) * Mathf.Cos(vector.y), Mathf.Sin(vector.y), Mathf.Cos(vector.x) * Mathf.Cos(vector.y)) * d;
			this.m_PreviewUtility.m_Camera.transform.LookAt(bounds.center);
			this.m_PreviewUtility.m_Camera.nearClipPlane = 0.05f;
			this.m_PreviewUtility.m_Camera.farClipPlane = 1000f;
			this.m_PreviewUtility.m_Light[0].intensity = 0.5f;
			this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(50f, 50f, 0f);
			this.m_PreviewUtility.m_Light[1].intensity = 0.5f;
			Color ambient = new Color(0.2f, 0.2f, 0.2f, 0f);
			InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
			foreach (MeshFilter current in list)
			{
				for (int j = 0; j < current.sharedMesh.subMeshCount; j++)
				{
					if (j < current.renderer.sharedMaterials.Length)
					{
						Matrix4x4 matrix = Matrix4x4.TRS(current.transform.position, current.transform.rotation, current.transform.localScale);
						this.m_PreviewUtility.DrawMesh(current.sharedMesh, matrix, current.renderer.sharedMaterials[j], j);
					}
				}
			}
			bool fog = RenderSettings.fog;
			Unsupported.SetRenderSettingsUseFogNoDirty(false);
			this.m_PreviewUtility.m_Camera.Render();
			Unsupported.SetRenderSettingsUseFogNoDirty(fog);
			InternalEditorUtility.RemoveCustomLighting();
		}
		public override string GetInfoString()
		{
			if (SceneView.lastActiveSceneView == null || SceneView.lastActiveSceneView.camera == null || this.m_NumberOfLODs <= 0 || this.activeLOD < 0)
			{
				return string.Empty;
			}
			List<Material> list = new List<Material>();
			SerializedProperty serializedProperty = this.m_Object.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", this.activeLOD));
			for (int i = 0; i < serializedProperty.arraySize; i++)
			{
				SerializedProperty serializedProperty2 = serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("renderer");
				Renderer renderer = serializedProperty2.objectReferenceValue as Renderer;
				if (renderer != null)
				{
					list.AddRange(renderer.sharedMaterials);
				}
			}
			Camera camera = SceneView.lastActiveSceneView.camera;
			LODGroup group = this.target as LODGroup;
			LODVisualizationInformation lODVisualizationInformation = LODUtility.CalculateVisualizationData(camera, group, this.activeLOD);
			return (this.activeLOD == -1) ? "LOD: culled" : string.Format("{0} Renderer(s)\n{1} Triangle(s)\n{2} Material(s)", serializedProperty.arraySize, lODVisualizationInformation.triangleCount, list.Distinct<Material>().Count<Material>());
		}
	}
}
