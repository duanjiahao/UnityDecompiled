using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEditor
{
	[CustomEditor(typeof(LODGroup))]
	internal class LODGroupEditor : Editor
	{
		private class LODAction
		{
			public delegate void Callback();

			private readonly float m_Percentage;

			private readonly List<LODGroupGUI.LODInfo> m_LODs;

			private readonly Vector2 m_ClickedPosition;

			private readonly SerializedObject m_ObjectRef;

			private readonly SerializedProperty m_LODsProperty;

			private readonly LODGroupEditor.LODAction.Callback m_Callback;

			public LODAction(List<LODGroupGUI.LODInfo> lods, float percentage, Vector2 clickedPosition, SerializedProperty propLODs, LODGroupEditor.LODAction.Callback callback)
			{
				this.m_LODs = lods;
				this.m_Percentage = percentage;
				this.m_ClickedPosition = clickedPosition;
				this.m_LODsProperty = propLODs;
				this.m_ObjectRef = propLODs.serializedObject;
				this.m_Callback = callback;
			}

			public void InsertLOD()
			{
				if (!this.m_LODsProperty.isArray)
				{
					return;
				}
				int num = -1;
				foreach (LODGroupGUI.LODInfo current in this.m_LODs)
				{
					if (this.m_Percentage > current.RawScreenPercent)
					{
						num = current.LODLevel;
						break;
					}
				}
				if (num < 0)
				{
					this.m_LODsProperty.InsertArrayElementAtIndex(this.m_LODs.Count);
					num = this.m_LODs.Count;
				}
				else
				{
					this.m_LODsProperty.InsertArrayElementAtIndex(num);
				}
				SerializedProperty serializedProperty = this.m_ObjectRef.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", num));
				serializedProperty.arraySize = 0;
				SerializedProperty arrayElementAtIndex = this.m_LODsProperty.GetArrayElementAtIndex(num);
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
				foreach (LODGroupGUI.LODInfo current in this.m_LODs)
				{
					int arraySize = this.m_ObjectRef.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", current.LODLevel)).arraySize;
					if (current.m_RangePosition.Contains(this.m_ClickedPosition) && (arraySize == 0 || EditorUtility.DisplayDialog("Delete LOD", "Are you sure you wish to delete this LOD?", "Yes", "No")))
					{
						SerializedProperty serializedProperty = this.m_ObjectRef.FindProperty(string.Format("m_LODs.Array.data[{0}]", current.LODLevel));
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

		private const string kLODDataPath = "m_LODs.Array.data[{0}]";

		private const string kPixelHeightDataPath = "m_LODs.Array.data[{0}].screenRelativeHeight";

		private const string kRenderRootPath = "m_LODs.Array.data[{0}].renderers";

		private const string kFadeTransitionWidthDataPath = "m_LODs.Array.data[{0}].fadeTransitionWidth";

		private int m_SelectedLODSlider = -1;

		private int m_SelectedLOD = -1;

		private int m_NumberOfLODs;

		private bool m_IsPrefab;

		private SerializedProperty m_FadeMode;

		private SerializedProperty m_AnimateCrossFading;

		private SerializedProperty m_LODs;

		private AnimBool m_ShowAnimateCrossFading = new AnimBool();

		private AnimBool m_ShowFadeTransitionWidth = new AnimBool();

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
			this.m_FadeMode = base.serializedObject.FindProperty("m_FadeMode");
			this.m_AnimateCrossFading = base.serializedObject.FindProperty("m_AnimateCrossFading");
			this.m_LODs = base.serializedObject.FindProperty("m_LODs");
			this.m_ShowAnimateCrossFading.value = (this.m_FadeMode.intValue != 0);
			this.m_ShowAnimateCrossFading.valueChanged.AddListener(new UnityAction(base.Repaint));
			this.m_ShowFadeTransitionWidth.value = false;
			this.m_ShowFadeTransitionWidth.valueChanged.AddListener(new UnityAction(base.Repaint));
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
			this.m_ShowAnimateCrossFading.valueChanged.RemoveListener(new UnityAction(base.Repaint));
			this.m_ShowFadeTransitionWidth.valueChanged.RemoveListener(new UnityAction(base.Repaint));
		}

		private static Rect CalculateScreenRect(IEnumerable<Vector3> points)
		{
			List<Vector2> list = (from p in points
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
			if (Event.current.type != EventType.Repaint || Camera.current == null || SceneView.lastActiveSceneView != SceneView.currentDrawingSceneView || Vector3.Dot(Camera.current.transform.forward, (Camera.current.transform.position - ((LODGroup)this.target).transform.position).normalized) > 0f)
			{
				return;
			}
			Camera camera = SceneView.lastActiveSceneView.camera;
			LODGroup lODGroup = this.target as LODGroup;
			Vector3 vector = lODGroup.transform.TransformPoint(lODGroup.localReferencePoint);
			LODVisualizationInformation lODVisualizationInformation = LODUtility.CalculateVisualizationData(camera, lODGroup, -1);
			float worldSpaceSize = lODVisualizationInformation.worldSpaceSize;
			Handles.color = ((lODVisualizationInformation.activeLODLevel == -1) ? LODGroupGUI.kCulledLODColor : LODGroupGUI.kLODColors[lODVisualizationInformation.activeLODLevel]);
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
			EditorGUI.DoDropShadowLabel(position, GUIContent.Temp((lODVisualizationInformation.activeLODLevel < 0) ? "Culled" : ("LOD " + lODVisualizationInformation.activeLODLevel)), LODGroupGUI.Styles.m_LODLevelNotifyText, 0.3f);
			Handles.EndGUI();
		}

		public void Update()
		{
			if (SceneView.lastActiveSceneView == null || SceneView.lastActiveSceneView.camera == null)
			{
				return;
			}
			if (SceneView.lastActiveSceneView.camera.transform.position != this.m_LastCameraPos)
			{
				this.m_LastCameraPos = SceneView.lastActiveSceneView.camera.transform.position;
				base.Repaint();
			}
		}

		private ModelImporter GetImporter()
		{
			return AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(this.target))) as ModelImporter;
		}

		private bool IsLODUsingCrossFadeWidth(int lod)
		{
			if (this.m_FadeMode.intValue == 0 || this.m_AnimateCrossFading.boolValue)
			{
				return false;
			}
			if (this.m_FadeMode.intValue == 1)
			{
				return true;
			}
			if (this.m_NumberOfLODs > 0 && this.m_SelectedLOD == this.m_NumberOfLODs - 1)
			{
				return true;
			}
			if (this.m_NumberOfLODs > 1 && this.m_SelectedLOD == this.m_NumberOfLODs - 2)
			{
				SerializedProperty serializedProperty = base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", this.m_NumberOfLODs - 1));
				if (serializedProperty.arraySize == 1 && serializedProperty.GetArrayElementAtIndex(0).FindPropertyRelative("renderer").objectReferenceValue is BillboardRenderer)
				{
					return true;
				}
			}
			return false;
		}

		public override void OnInspectorGUI()
		{
			bool enabled = GUI.enabled;
			base.serializedObject.Update();
			EditorGUILayout.PropertyField(this.m_FadeMode, new GUILayoutOption[0]);
			this.m_ShowAnimateCrossFading.target = (this.m_FadeMode.intValue != 0);
			if (EditorGUILayout.BeginFadeGroup(this.m_ShowAnimateCrossFading.faded))
			{
				EditorGUILayout.PropertyField(this.m_AnimateCrossFading, new GUILayoutOption[0]);
			}
			EditorGUILayout.EndFadeGroup();
			this.m_NumberOfLODs = this.m_LODs.arraySize;
			if (this.m_SelectedLOD >= this.m_NumberOfLODs)
			{
				this.m_SelectedLOD = this.m_NumberOfLODs - 1;
			}
			if (this.m_NumberOfLODs > 0 && this.activeLOD >= 0)
			{
				SerializedProperty serializedProperty = base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", this.activeLOD));
				for (int k = serializedProperty.arraySize - 1; k >= 0; k--)
				{
					SerializedProperty serializedProperty2 = serializedProperty.GetArrayElementAtIndex(k).FindPropertyRelative("renderer");
					Renderer x = serializedProperty2.objectReferenceValue as Renderer;
					if (x == null)
					{
						serializedProperty.DeleteArrayElementAtIndex(k);
					}
				}
			}
			GUILayout.Space(18f);
			Rect rect = GUILayoutUtility.GetRect(0f, 30f, new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			List<LODGroupGUI.LODInfo> list = LODGroupGUI.CreateLODInfos(this.m_NumberOfLODs, rect, (int i) => string.Format("LOD {0}", i), (int i) => base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].screenRelativeHeight", i)).floatValue);
			this.DrawLODLevelSlider(rect, list);
			GUILayout.Space(16f);
			GUILayout.Label(string.Format("LODBias of {0:0.00} active", QualitySettings.lodBias), EditorStyles.boldLabel, new GUILayoutOption[0]);
			if (this.m_NumberOfLODs > 0 && this.activeLOD >= 0 && this.activeLOD < this.m_NumberOfLODs)
			{
				this.m_ShowFadeTransitionWidth.target = this.IsLODUsingCrossFadeWidth(this.activeLOD);
				if (EditorGUILayout.BeginFadeGroup(this.m_ShowFadeTransitionWidth.faded))
				{
					EditorGUILayout.PropertyField(base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].fadeTransitionWidth", this.activeLOD)), new GUILayoutOption[0]);
				}
				EditorGUILayout.EndFadeGroup();
				this.DrawRenderersInfo(EditorGUIUtility.currentViewWidth);
			}
			GUILayout.Space(8f);
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label("Recalculate:", EditorStyles.boldLabel, new GUILayoutOption[0]);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			LODGroup lODGroup = this.target as LODGroup;
			if (GUILayout.Button(LODGroupGUI.Styles.m_RecalculateBounds, new GUILayoutOption[0]))
			{
				LODUtility.CalculateLODGroupBoundingBox(lODGroup);
			}
			if (GUILayout.Button(LODGroupGUI.Styles.m_LightmapScale, new GUILayoutOption[0]))
			{
				this.SendPercentagesToLightmapScale();
			}
			GUILayout.EndVertical();
			GUILayout.Label(string.Format("Extents {0:0.00}m", lODGroup.size), new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
			GUILayout.Space(5f);
			ModelImporter modelImporter = (PrefabUtility.GetPrefabType(this.target) != PrefabType.ModelPrefabInstance) ? null : this.GetImporter();
			if (modelImporter != null)
			{
				SerializedObject serializedObject = new SerializedObject(modelImporter);
				SerializedProperty serializedProperty3 = serializedObject.FindProperty("m_LODScreenPercentages");
				bool flag = serializedProperty3.isArray && serializedProperty3.arraySize == list.Count;
				bool enabled2 = GUI.enabled;
				if (!flag)
				{
					GUI.enabled = false;
				}
				if (modelImporter != null && GUILayout.Button((!flag) ? LODGroupGUI.Styles.m_UploadToImporterDisabled : LODGroupGUI.Styles.m_UploadToImporter, new GUILayoutOption[0]))
				{
					for (int j = 0; j < serializedProperty3.arraySize; j++)
					{
						serializedProperty3.GetArrayElementAtIndex(j).floatValue = list[j].RawScreenPercent;
					}
					serializedObject.ApplyModifiedProperties();
					AssetDatabase.ImportAsset(modelImporter.assetPath);
				}
				GUI.enabled = enabled2;
			}
			base.serializedObject.ApplyModifiedProperties();
			GUI.enabled = enabled;
		}

		private void DrawRenderersInfo(float availableWidth)
		{
			int num = Mathf.FloorToInt(availableWidth / 60f);
			Rect rect = GUILayoutUtility.GetRect(LODGroupGUI.Styles.m_RendersTitle, LODGroupGUI.Styles.m_LODSliderTextSelected);
			if (Event.current.type == EventType.Repaint)
			{
				EditorStyles.label.Draw(rect, LODGroupGUI.Styles.m_RendersTitle, false, false, false, false);
			}
			SerializedProperty serializedProperty = base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", this.activeLOD));
			int num2 = serializedProperty.arraySize + 1;
			int num3 = Mathf.CeilToInt((float)num2 / (float)num);
			Rect rect2 = GUILayoutUtility.GetRect(0f, (float)(num3 * 60), new GUILayoutOption[]
			{
				GUILayout.ExpandWidth(true)
			});
			Rect rect3 = rect2;
			GUI.Box(rect2, GUIContent.none);
			rect3.width -= 6f;
			rect3.x += 3f;
			float num4 = rect3.width / (float)num;
			List<Rect> list = new List<Rect>();
			for (int i = 0; i < num3; i++)
			{
				int num5 = 0;
				while (num5 < num && i * num + num5 < serializedProperty.arraySize)
				{
					Rect rect4 = new Rect(2f + rect3.x + (float)num5 * num4, 2f + rect3.y + (float)(i * 60), num4 - 4f, 56f);
					list.Add(rect4);
					this.DrawRendererButton(rect4, i * num + num5);
					num5++;
				}
			}
			if (this.m_IsPrefab)
			{
				return;
			}
			int num6 = (num2 - 1) % num;
			int num7 = num3 - 1;
			this.HandleAddRenderer(new Rect(2f + rect3.x + (float)num6 * num4, 2f + rect3.y + (float)(num7 * 60), num4 - 4f, 56f), list, rect2);
		}

		private void HandleAddRenderer(Rect position, IEnumerable<Rect> alreadyDrawn, Rect drawArea)
		{
			Event evt = Event.current;
			EventType type = evt.type;
			switch (type)
			{
			case EventType.Repaint:
				LODGroupGUI.Styles.m_LODStandardButton.Draw(position, GUIContent.none, false, false, false, false);
				LODGroupGUI.Styles.m_LODRendererAddButton.Draw(new Rect(position.x - 2f, position.y, position.width, position.height), "Add", false, false, false, false);
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
						IEnumerable<GameObject> selectedGameObjects = from go in DragAndDrop.objectReferences
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
					GameObject gameObject = ObjectSelector.GetCurrentObject() as GameObject;
					if (gameObject != null)
					{
						this.AddGameObjectRenderers(this.GetRenderers(new List<GameObject>
						{
							gameObject
						}, true), true);
					}
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
			SerializedProperty serializedProperty = base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", this.activeLOD));
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
						else if (renderer is SkinnedMeshRenderer)
						{
							content = new GUIContent(AssetPreview.GetAssetPreview((renderer as SkinnedMeshRenderer).sharedMesh), renderer.gameObject.name);
						}
						else
						{
							content = new GUIContent(ObjectNames.NicifyVariableName(renderer.GetType().Name), renderer.gameObject.name);
						}
						LODGroupGUI.Styles.m_LODBlackBox.Draw(position, GUIContent.none, false, false, false, false);
						LODGroupGUI.Styles.m_LODRendererButton.Draw(new Rect(position.x + 2f, position.y + 2f, position.width - 4f, position.height - 4f), content, false, false, false, false);
					}
					else
					{
						LODGroupGUI.Styles.m_LODBlackBox.Draw(position, GUIContent.none, false, false, false, false);
						LODGroupGUI.Styles.m_LODRendererButton.Draw(position, "<Empty>", false, false, false, false);
					}
					if (!this.m_IsPrefab)
					{
						LODGroupGUI.Styles.m_LODBlackBox.Draw(position2, GUIContent.none, false, false, false, false);
						LODGroupGUI.Styles.m_LODRendererRemove.Draw(position2, LODGroupGUI.Styles.m_IconRendererMinus, false, false, false, false);
					}
				}
			}
			else if (!this.m_IsPrefab && position2.Contains(current.mousePosition))
			{
				serializedProperty.DeleteArrayElementAtIndex(rendererIndex);
				current.Use();
				base.serializedObject.ApplyModifiedProperties();
				LODUtility.CalculateLODGroupBoundingBox(this.target as LODGroup);
			}
			else if (position.Contains(current.mousePosition))
			{
				EditorGUIUtility.PingObject(renderer);
				current.Use();
			}
		}

		private IEnumerable<Renderer> GetRenderers(IEnumerable<GameObject> selectedGameObjects, bool searchChildren)
		{
			LODGroup lodGroup = this.target as LODGroup;
			if (lodGroup == null || EditorUtility.IsPersistent(lodGroup))
			{
				return new List<Renderer>();
			}
			IEnumerable<GameObject> enumerable = from go in selectedGameObjects
			where go.transform.IsChildOf(lodGroup.transform)
			select go;
			IEnumerable<GameObject> enumerable2 = from go in selectedGameObjects
			where !go.transform.IsChildOf(lodGroup.transform)
			select go;
			List<GameObject> list = new List<GameObject>();
			if (enumerable2.Count<GameObject>() > 0 && EditorUtility.DisplayDialog("Reparent GameObjects", "Some objects are not children of the LODGroup GameObject. Do you want to reparent them and add them to the LODGroup?", "Yes, Reparent", "No, Use Only Existing Children"))
			{
				foreach (GameObject current in enumerable2)
				{
					if (EditorUtility.IsPersistent(current))
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(current);
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
			IEnumerable<Renderer> collection = from go in DragAndDrop.objectReferences
			where go as Renderer != null
			select go as Renderer;
			list2.AddRange(collection);
			return list2;
		}

		private void AddGameObjectRenderers(IEnumerable<Renderer> toAdd, bool add)
		{
			SerializedProperty serializedProperty = base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", this.activeLOD));
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
			base.serializedObject.ApplyModifiedProperties();
			LODUtility.CalculateLODGroupBoundingBox(this.target as LODGroup);
		}

		private void DeletedLOD()
		{
			this.m_SelectedLOD--;
		}

		private static void UpdateCamera(float desiredPercentage, LODGroup group)
		{
			Vector3 pos = group.transform.TransformPoint(group.localReferencePoint);
			float num = LODUtility.CalculateDistance(SceneView.lastActiveSceneView.camera, (desiredPercentage > 0f) ? desiredPercentage : 1E-06f, group);
			if (SceneView.lastActiveSceneView.camera.orthographic)
			{
				num = Mathf.Sqrt(num * num * (1f + SceneView.lastActiveSceneView.camera.aspect));
			}
			SceneView.lastActiveSceneView.LookAtDirect(pos, SceneView.lastActiveSceneView.camera.transform.rotation, num);
		}

		private void UpdateSelectedLODFromCamera(IEnumerable<LODGroupGUI.LODInfo> lods, float cameraPercent)
		{
			foreach (LODGroupGUI.LODInfo current in lods)
			{
				if (cameraPercent > current.RawScreenPercent)
				{
					this.m_SelectedLOD = current.LODLevel;
					break;
				}
			}
		}

		private static float GetCameraPercentForCurrentQualityLevel(float clickPosition, float sliderStart, float sliderWidth)
		{
			float percentage = Mathf.Clamp(1f - (clickPosition - sliderStart) / sliderWidth, 0.01f, 1f);
			return LODGroupGUI.LinearizeScreenPercentage(percentage);
		}

		private void DrawLODLevelSlider(Rect sliderPosition, List<LODGroupGUI.LODInfo> lods)
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
						genericMenu.AddItem(EditorGUIUtility.TextContent("Insert Before"), false, new GenericMenu.MenuFunction(new LODGroupEditor.LODAction(lods, LODGroupGUI.LinearizeScreenPercentage(num), current.mousePosition, this.m_LODs, null).InsertLOD));
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
						genericMenu.AddItem(EditorGUIUtility.TextContent("Delete"), false, new GenericMenu.MenuFunction(new LODGroupEditor.LODAction(lods, LODGroupGUI.LinearizeScreenPercentage(num), current.mousePosition, this.m_LODs, new LODGroupEditor.LODAction.Callback(this, ldftn(DeletedLOD))).DeleteLOD));
					}
					genericMenu.ShowAsContext();
					bool flag2 = false;
					foreach (LODGroupGUI.LODInfo current2 in lods)
					{
						if (current2.m_RangePosition.Contains(current.mousePosition))
						{
							this.m_SelectedLOD = current2.LODLevel;
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						this.m_SelectedLOD = -1;
					}
					current.Use();
					goto IL_7B4;
				}
				Rect rect = sliderPosition;
				rect.x -= 5f;
				rect.width += 10f;
				if (rect.Contains(current.mousePosition))
				{
					current.Use();
					GUIUtility.hotControl = controlID;
					bool flag3 = false;
					IOrderedEnumerable<LODGroupGUI.LODInfo> collection = from lod in lods
					where lod.ScreenPercent > 0.5f
					select lod into x
					orderby x.LODLevel descending
					select x;
					IOrderedEnumerable<LODGroupGUI.LODInfo> collection2 = from lod in lods
					where lod.ScreenPercent <= 0.5f
					select lod into x
					orderby x.LODLevel
					select x;
					List<LODGroupGUI.LODInfo> list = new List<LODGroupGUI.LODInfo>();
					list.AddRange(collection);
					list.AddRange(collection2);
					foreach (LODGroupGUI.LODInfo current3 in list)
					{
						if (current3.m_ButtonPosition.Contains(current.mousePosition))
						{
							this.m_SelectedLODSlider = current3.LODLevel;
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
						foreach (LODGroupGUI.LODInfo current4 in lods)
						{
							if (current4.m_RangePosition.Contains(current.mousePosition))
							{
								this.m_SelectedLOD = current4.LODLevel;
								break;
							}
						}
					}
				}
				goto IL_7B4;
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
				goto IL_7B4;
			case EventType.MouseMove:
			case EventType.KeyDown:
			case EventType.KeyUp:
			case EventType.ScrollWheel:
			case EventType.Layout:
				IL_75:
				if (typeForControl != EventType.DragExited)
				{
					goto IL_7B4;
				}
				current.Use();
				goto IL_7B4;
			case EventType.MouseDrag:
				if (GUIUtility.hotControl == controlID && this.m_SelectedLODSlider >= 0 && lods[this.m_SelectedLODSlider] != null)
				{
					current.Use();
					float num2 = Mathf.Clamp01(1f - (current.mousePosition.x - sliderPosition.x) / sliderPosition.width);
					LODGroupGUI.SetSelectedLODLevelPercentage(num2 - 0.001f, this.m_SelectedLODSlider, lods);
					SerializedProperty serializedProperty = base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].screenRelativeHeight", lods[this.m_SelectedLODSlider].LODLevel));
					serializedProperty.floatValue = lods[this.m_SelectedLODSlider].RawScreenPercent;
					if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null && !this.m_IsPrefab)
					{
						LODGroupEditor.UpdateCamera(LODGroupGUI.LinearizeScreenPercentage(num2), lODGroup);
						SceneView.RepaintAll();
					}
				}
				goto IL_7B4;
			case EventType.Repaint:
				LODGroupGUI.DrawLODSlider(sliderPosition, lods, this.activeLOD);
				goto IL_7B4;
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				int num3 = -2;
				foreach (LODGroupGUI.LODInfo current5 in lods)
				{
					if (current5.m_RangePosition.Contains(current.mousePosition))
					{
						num3 = current5.LODLevel;
						break;
					}
				}
				if (num3 == -2 && LODGroupGUI.GetCulledBox(sliderPosition, (lods.Count <= 0) ? 1f : lods[lods.Count - 1].ScreenPercent).Contains(current.mousePosition))
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
							IEnumerable<GameObject> selectedGameObjects = from go in DragAndDrop.objectReferences
							where go as GameObject != null
							select go as GameObject;
							IEnumerable<Renderer> renderers = this.GetRenderers(selectedGameObjects, true);
							if (num3 == -1)
							{
								this.m_LODs.arraySize++;
								SerializedProperty serializedProperty2 = base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].screenRelativeHeight", lods.Count));
								if (lods.Count == 0)
								{
									serializedProperty2.floatValue = 0.5f;
								}
								else
								{
									SerializedProperty serializedProperty3 = base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].screenRelativeHeight", lods.Count - 1));
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
					goto IL_7B4;
				}
				goto IL_7B4;
			}
			}
			goto IL_75;
			IL_7B4:
			if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null && !this.m_IsPrefab)
			{
				Camera camera = SceneView.lastActiveSceneView.camera;
				float num4 = LODUtility.CalculateVisualizationData(camera, lODGroup, -1).activeRelativeScreenSize / QualitySettings.lodBias;
				float value = LODGroupGUI.DelinearizeScreenPercentage(num4);
				Vector3 normalized = (SceneView.lastActiveSceneView.camera.transform.position - ((LODGroup)this.target).transform.position).normalized;
				if (Vector3.Dot(camera.transform.forward, normalized) > 0f)
				{
					value = 1f;
				}
				Rect rect2 = LODGroupGUI.CalcLODButton(sliderPosition, Mathf.Clamp01(value));
				Rect position = new Rect(rect2.center.x - 15f, rect2.y - 25f, 32f, 32f);
				Rect position2 = new Rect(rect2.center.x - 1f, rect2.y, 2f, rect2.height);
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
					LODGroupGUI.Styles.m_LODCameraLine.Draw(position2, false, false, false, false);
					GUI.backgroundColor = backgroundColor;
					GUI.Label(position, LODGroupGUI.Styles.m_CameraIcon, GUIStyle.none);
					LODGroupGUI.Styles.m_LODSliderText.Draw(position3, string.Format("{0:0}%", Mathf.Clamp01(num4) * 100f), false, false, false, false);
					break;
				}
				}
			}
		}

		private static float CalculatePercentageFromBar(Rect totalRect, Vector2 clickPosition)
		{
			clickPosition.x -= totalRect.x;
			totalRect.x = 0f;
			return (totalRect.width <= 0f) ? 0f : (1f - clickPosition.x / totalRect.width);
		}

		private void SendPercentagesToLightmapScale()
		{
			List<LODGroupEditor.LODLightmapScale> list = new List<LODGroupEditor.LODLightmapScale>();
			for (int i = 0; i < this.m_NumberOfLODs; i++)
			{
				SerializedProperty serializedProperty = base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", i));
				List<SerializedProperty> list2 = new List<SerializedProperty>();
				for (int j = 0; j < serializedProperty.arraySize; j++)
				{
					SerializedProperty serializedProperty2 = serializedProperty.GetArrayElementAtIndex(j).FindPropertyRelative("renderer");
					if (serializedProperty2 != null)
					{
						list2.Add(serializedProperty2);
					}
				}
				float scale = (i != 0) ? base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].screenRelativeHeight", i - 1)).floatValue : 1f;
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
			this.m_PreviewUtility.EndAndDrawPreview(r);
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
			SerializedProperty serializedProperty = base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", this.activeLOD));
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
			this.m_PreviewUtility.m_Light[0].intensity = 1f;
			this.m_PreviewUtility.m_Light[0].transform.rotation = Quaternion.Euler(50f, 50f, 0f);
			this.m_PreviewUtility.m_Light[1].intensity = 1f;
			Color ambient = new Color(0.2f, 0.2f, 0.2f, 0f);
			InternalEditorUtility.SetCustomLighting(this.m_PreviewUtility.m_Light, ambient);
			foreach (MeshFilter current in list)
			{
				for (int j = 0; j < current.sharedMesh.subMeshCount; j++)
				{
					if (j < current.GetComponent<Renderer>().sharedMaterials.Length)
					{
						Matrix4x4 matrix = Matrix4x4.TRS(current.transform.position, current.transform.rotation, current.transform.localScale);
						this.m_PreviewUtility.DrawMesh(current.sharedMesh, matrix, current.GetComponent<Renderer>().sharedMaterials[j], j);
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
			SerializedProperty serializedProperty = base.serializedObject.FindProperty(string.Format("m_LODs.Array.data[{0}].renderers", this.activeLOD));
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
